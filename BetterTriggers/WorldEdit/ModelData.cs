﻿using CASCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.IO;
using IniParser.Parser;
using BetterTriggers.Utility;
using Model.War3Data;

namespace BetterTriggers.WorldEdit
{
    public static class ModelData
    {
        private static List<AssetModel> assetModels = new List<AssetModel>();

        public static List<AssetModel> GetModelsAll()
        {
            return assetModels;
        }

        public static void Load()
        {
            var unitData = UnitTypes.GetUnitTypesAll();
            var destData = DestructibleTypes.GetDestructiblesTypesAll();
            var doodData = DoodadTypes.GetDoodadTypesAll();
            var itemData = ItemData.GetItemsAll();


            // some asset strings occur multiple times
            HashSet<AssetModel> hashset = new HashSet<AssetModel>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];
            CASCFile abilitySkin = (CASCFile)units.Entries["abilityskin.txt"];
            var file = Casc.GetCasc().OpenFile(abilitySkin.FullName);
            StreamReader reader = new StreamReader(file);
            string text = reader.ReadToEnd();

            string iniFile = IniFileConverter.Convert(text);
            IniDataParser parser = new IniDataParser();
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowDuplicateKeys = true;
            IniData data = parser.Parse(iniFile);

            var enumSections = data.Sections.GetEnumerator();
            while (enumSections.MoveNext())
            {
                var section = enumSections.Current;
                var enumKeys = section.Keys.GetEnumerator();
                var category = string.Empty;
                while (enumKeys.MoveNext())
                {
                    var key = enumKeys.Current;
                    if (key.KeyName == "skinType")
                        category = key.Value;
                    if (key.KeyName == "Targetart" || key.KeyName == "Specialart" || key.KeyName == "Missileart" || key.KeyName == "Casterart" || key.KeyName == "Effectart")
                    {
                        if (key.Value != "")
                            hashset.Add(new AssetModel()
                            {
                                Path = key.Value,
                                Category = category
                            });
                    }
                }
            }

            for (int i = 0; i < unitData.Count; i++)
            {
                hashset.Add(new AssetModel()
                {
                    Path = unitData[i].Model,
                    Category = "Unit"
                });
            }
            for (int i = 0; i < destData.Count; i++)
            {
                hashset.Add(new AssetModel()
                {
                    Path = destData[i].Model,
                    Category = "Destructible"
                });
            }
            for (int i = 0; i < doodData.Count; i++)
            {
                hashset.Add(new AssetModel()
                {
                    Path = doodData[i].Model,
                    Category = "Doodad"
                });
            }
            for (int i = 0; i < itemData.Count; i++)
            {
                hashset.Add(new AssetModel()
                {
                    Path = itemData[i].Model,
                    Category = "Items"
                });
            }


            assetModels = hashset.ToList();
        }
    }
}
