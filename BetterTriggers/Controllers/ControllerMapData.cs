﻿using BetterTriggers.Containers;
using BetterTriggers.WorldEdit;
using Model.EditorData;
using Model.SaveableData;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build.Environment;
using War3Net.Build.Widget;

namespace BetterTriggers.Controllers
{
    public class ControllerMapData
    {
        public List<AbilityType> GetAbilitiesAll()
        {
            return AbilityTypes.GetAll();
        }

        public List<BuffType> GetBuffsAll()
        {
            return BuffTypes.GetAll();
        }

        public List<UnitType> GetUnitTypesAll()
        {
            return UnitTypes.GetAll();
        }

        public List<UpgradeType> GetUpgradeTypesAll()
        {
            return UpgradeTypes.GetAll();
        }

        public List<DestructibleType> GetDestTypesAll()
        {
            return DestructibleTypes.GetAll();
        }

        public List<DoodadType> GetDoodadTypesAll()
        {
            return DoodadTypes.GetAll();
        }

        public List<ItemType> GetItemTypesAll()
        {
            return ItemTypes.GetAll();
        }

        public List<UnitData> GetMapUnits()
        {
            return Units.GetAll();
        }

        public List<UnitData> GetMapItems()
        {
            return Units.GetMapItemsAll();
        }

        public List<DoodadData> GetMapDests()
        {
            return Destructibles.GetAll();
        }

        public List<Region> GetMapRegions()
        {
            return Regions.GetAll();
        }

        public List<Camera> GetMapCameras()
        {
            return Cameras.GetAll();
        }

        public List<AssetModel> GetModelData()
        {
            return ModelData.GetModelsAll();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Reference to map data.</param>
        /// <returns></returns>
        internal bool ReferencedDataExists(Value value)
        {
            if (value.returnType == "unitcode")
            {
                List<UnitType> unitTypes = UnitTypes.GetAll();
                for (int i = 0; i < unitTypes.Count; i++)
                {
                    if (value.identifier == unitTypes[i].Id)
                    {
                        return true;
                    }
                }
            }
            else if (value.returnType == "unit")
            {
                var units = Units.GetAll();
                for (int i = 0; i < units.Count; i++)
                {
                    if (value.identifier == $"{units[i].ToString()}_{units[i].CreationNumber}")
                    {
                        return true;

                    }
                }
            }
            else if (value.returnType == "destructablecode")
            {
                List<DestructibleType> destTypes = DestructibleTypes.GetAll();
                for (int i = 0; i < destTypes.Count; i++)
                {
                    if (value.identifier == destTypes[i].DestCode)
                    {
                        return true;
                    }
                }
            }
            else if (value.returnType == "destructable")
            {
                var dests = Destructibles.GetAll();
                for (int i = 0; i < dests.Count; i++)
                {
                    if (value.identifier == $"{dests[i].ToString()}_{dests[i].CreationNumber}")
                    {
                        return true;
                    }
                }
            }
            else if (value.returnType == "itemcode")
            {
                List<ItemType> itemTypes = ItemTypes.GetAll();
                for (int i = 0; i < itemTypes.Count; i++)
                {
                    if (value.identifier == itemTypes[i].ItemCode)
                    {
                        return true;
                    }
                }
            }
            else if (value.returnType == "item")
            {
                List<UnitData> itemTypes = Units.GetMapItemsAll();
                for (int i = 0; i < itemTypes.Count; i++)
                {
                    if (value.identifier == $"{itemTypes[i].ToString()}_{itemTypes[i].CreationNumber}")
                    {
                        return true;
                    }
                }
            }
            else if (value.returnType == "doodadcode")
            {
                List<DoodadType> doodadTypes = DoodadTypes.GetAll();
                for (int i = 0; i < doodadTypes.Count; i++)
                {
                    if (value.identifier == doodadTypes[i].DoodCode)
                    {
                        return true;
                    }
                }
            }
            else if (value.returnType == "abilcode")
            {
                var abilities = AbilityTypes.GetAll();
                for (int i = 0; i < abilities.Count; i++)
                {
                    if (value.identifier == abilities[i].AbilCode)
                    {
                        return true;
                    }
                }
            }
            else if (value.returnType == "buffcode")
            {
                var buffs = BuffTypes.GetAll();
                for (int i = 0; i < buffs.Count; i++)
                {
                    if (value.identifier == buffs[i].BuffCode)
                    {
                        return true;
                    }
                }
            }
            else if (value.returnType == "techcode")
            {
                var tech = UpgradeTypes.GetAll();
                for (int i = 0; i < tech.Count; i++)
                {
                    if (value.identifier == tech[i].UpgradeCode)
                    {
                        return true;
                    }
                }
            }
            else if (value.returnType == "rect")
            {
                var regions = Regions.GetAll();
                for (int i = 0; i < regions.Count; i++)
                {
                    if (value.identifier == regions[i].ToString())
                    {
                        return true;
                    }
                }
            }
            else if (value.returnType == "camerasetup")
            {
                var cameras = Cameras.GetAll();
                for (int i = 0; i < cameras.Count; i++)
                {
                    if (value.identifier == cameras[i].ToString())
                    {
                        return true;
                    }
                }
            }
            else
                return true;

            return false;
        }

        public List<ExplorerElementTrigger> ReloadMapData()
        {
            Commands.CommandManager.Reset();
            CustomMapData.Load();
            var changedTriggers = CustomMapData.RemoveInvalidReferences();
            changedTriggers.ForEach(trig => ContainerUnsavedFiles.AddToUnsaved(trig));

            return changedTriggers;
        }
    }
}
