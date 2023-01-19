﻿using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;

namespace BetterTriggers.WorldEdit
{
    public class ItemTypes
    {
        private static Dictionary<string, ItemType> items;
        private static Dictionary<string, ItemType> itemsBase;
        private static Dictionary<string, ItemType> itemsCustom;

        internal static List<ItemType> GetAll()
        {
            return items.Select(kvp => kvp.Value).ToList();

        }
        internal static List<ItemType> GetBase()
        {
            return itemsBase.Select(kvp => kvp.Value).ToList();
        }
        internal static List<ItemType> GetCustom()
        {
            return itemsCustom.Select(kvp => kvp.Value).ToList();
        }

        public static ItemType GetItemType(string itemcode)
        {
            ItemType destType;
            items.TryGetValue(itemcode, out destType);
            return destType;
        }

        internal static string GetName(string itemcode)
        {
            ItemType itemType = GetItemType(itemcode);
            if (itemType == null)
                return null;

            return itemType.DisplayName;
        }

        internal static void Load(bool isTest = false)
        {
            items = new Dictionary<string, ItemType>();
            itemsBase = new Dictionary<string, ItemType>();
            itemsCustom = new Dictionary<string, ItemType>();

            Stream itemskin;

            if (isTest)
            {
                itemskin = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/itemskin.txt"), FileMode.Open);
            }
            else
            {
                var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];
                CASCFile itemSkins = (CASCFile)units.Entries["itemskin.txt"];
                itemskin = Casc.GetCasc().OpenFile(itemSkins.FullName);
            }


            // Parse ini file
            var reader = new StreamReader(itemskin);
            var text = reader.ReadToEnd();

            var data = IniFileConverter.GetIniData(text);

            var sections = data.Sections.GetEnumerator();
            while (sections.MoveNext())
            {
                var id = sections.Current.SectionName;
                var keys = sections.Current.Keys;
                var model = keys["file"];

                var item = new ItemType()
                {
                    ItemCode = id,
                    DisplayName = Locale.GetDisplayName(id),
                    Model = model,
                };
                items.TryAdd(item.ItemCode, item);
                itemsBase.TryAdd(item.ItemCode, item);
            }

            string filePath = "war3map.w3t";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader bReader = new BinaryReader(s);
                var customItems = bReader.ReadItemObjectData();

                for (int i = 0; i < customItems.BaseItems.Count; i++)
                {
                    var baseItem = customItems.BaseItems[i];
                    SetCustomFields(baseItem, Int32Extensions.ToRawcode(baseItem.OldId));
                }

                for (int i = 0; i < customItems.NewItems.Count; i++)
                {
                    var customItem = customItems.NewItems[i];
                    ItemType baseAbil = GetItemType(Int32Extensions.ToRawcode(customItem.OldId));
                    string name = baseAbil.DisplayName;
                    var item = new ItemType()
                    {
                        ItemCode = customItem.ToString().Substring(0, 4),
                        DisplayName = name,
                    };
                    items.TryAdd(item.ItemCode, item);
                    itemsCustom.TryAdd(item.ItemCode, item);
                    SetCustomFields(customItem, item.ItemCode);
                }
            }
        }

        private static void SetCustomFields(SimpleObjectModification modified, string itemcode)
        {
            ItemType itemType = GetItemType(itemcode);
            string displayName = itemType.DisplayName;

            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "unam")
                    displayName = MapStrings.GetString(modification.ValueAsString);
            }

            itemType.DisplayName = displayName;
        }
    }
}
