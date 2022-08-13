﻿using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class DestructibleTypes
    {
        private static Dictionary<string, DestructibleType> destructibles;
        private static Dictionary<string, DestructibleType> destructiblesBase;
        private static Dictionary<string, DestructibleType> destructiblesCustom;

        internal static List<DestructibleType> GetAll()
        {
            return destructibles.Select(kvp => kvp.Value).ToList();
        }

        internal static List<DestructibleType> GetBase()
        {
            return destructiblesBase.Select(kvp => kvp.Value).ToList();
        }

        internal static List<DestructibleType> GetCustom()
        {
            return destructiblesCustom.Select(kvp => kvp.Value).ToList();
        }

        public static DestructibleType GetDestType(string unitcode)
        {
            DestructibleType destType;
            destructibles.TryGetValue(unitcode, out destType);
            return destType;
        }

        // TODO: Doesn't return comments with the name (e.g. 'Diagonal 1' or 'Vertical')
        internal static string GetName(string destcode)
        {
            DestructibleType destType = GetDestType(destcode);
            if (destType == null)
                return null;

            string name = destType.DisplayName;
            if (destType.EditorSuffix != null)
                name += " " + destType.EditorSuffix;

            return name;
        }

        internal static void Load(bool isTest = false)
        {
            destructibles = new Dictionary<string, DestructibleType>();
            destructiblesBase = new Dictionary<string, DestructibleType>();
            destructiblesCustom = new Dictionary<string, DestructibleType>();

            Stream destructibleskin;
            Stream destructibleDataSLK;

            if (isTest)
            {
                destructibleskin = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/destructableskin.txt"), FileMode.Open);
                destructibleDataSLK = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/destructabledata.slk"), FileMode.Open);
            }
            else
            {
                var cascFolder = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];
                CASCFile destSkins = (CASCFile)cascFolder.Entries["destructableskin.txt"];
                destructibleskin = Casc.GetCasc().OpenFile(destSkins.FullName);

                CASCFile destData = (CASCFile)cascFolder.Entries["destructabledata.slk"];
                destructibleDataSLK = Casc.GetCasc().OpenFile(destData.FullName);
            }


            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(destructibleDataSLK);
            int count = table.Count();
            for (int i = 1; i < count; i++)
            {
                var row = table.ElementAt(i);
                DestructibleType destType = new DestructibleType()
                {
                    DestCode = (string)row.GetValue(0),
                    DisplayName = Locale.Translate((string)row.GetValue(6)),
                    EditorSuffix = Locale.Translate((string)row.GetValue(7)),
                };

                destructibles.TryAdd(destType.DestCode, destType);
                destructiblesBase.TryAdd(destType.DestCode, destType);
            }

            // Add 'model' from 'destructableskin.txt'
            var reader = new StreamReader(destructibleskin);
            var text = reader.ReadToEnd();
            var data = IniFileConverter.GetIniData(text);
            var destTypesList = GetBase();
            for (int i = 0; i < destTypesList.Count; i++)
            {
                var destType = destTypesList[i];
                var section = data[destType.DestCode];
                string model = section["file"];
                destType.Model = model;
            }


            // Read custom destructible definition data
            string filePath = "war3map.w3b";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(s);
                var customDestructibles = War3Net.Build.Extensions.BinaryReaderExtensions.ReadMapDestructableObjectData(binaryReader);

                for (int i = 0; i < customDestructibles.BaseDestructables.Count; i++)
                {
                    var baseDest = customDestructibles.BaseDestructables[i];
                    SetCustomFields(baseDest, Int32Extensions.ToRawcode(baseDest.OldId));
                }

                for (int i = 0; i < customDestructibles.NewDestructables.Count; i++)
                {
                    var dest = customDestructibles.NewDestructables[i];
                    DestructibleType baseDest = GetDestType(Int32Extensions.ToRawcode(dest.OldId));
                    string name = baseDest.DisplayName;
                    DestructibleType destructible = new DestructibleType()
                    {
                        DestCode = dest.ToString().Substring(0, 4),
                        DisplayName = name,
                    };
                    destructibles.TryAdd(destructible.DestCode, destructible);
                    destructiblesCustom.TryAdd(destructible.DestCode, destructible);
                    SetCustomFields(dest, destructible.DestCode);
                }
            }
        }

        private static void SetCustomFields(SimpleObjectModification modified, string buffcode)
        {
            DestructibleType buffType = GetDestType(buffcode);
            string displayName = buffType.DisplayName;
            string editorSuffix = buffType.EditorSuffix;
            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "bnam")
                    displayName = MapStrings.GetString(modification.ValueAsString);
                else if (Int32Extensions.ToRawcode(modification.Id) == "bsuf")
                    editorSuffix = MapStrings.GetString(modification.ValueAsString);
            }
            buffType.DisplayName = displayName;
            buffType.EditorSuffix = editorSuffix;
        }
    }
}
