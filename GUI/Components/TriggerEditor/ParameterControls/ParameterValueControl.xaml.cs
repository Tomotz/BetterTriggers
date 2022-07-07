﻿using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterValueControl : UserControl, IParameterControl
    {
        public EventHandler SelectionChanged;
        private IValueControl valueControl;

        public ParameterValueControl(string returnType)
        {
            InitializeComponent();

            var controllerMapData = new ControllerMapData();

            List<Value> values;
            switch (returnType)
            {
                case "boolean":
                    this.valueControl = new ValueControlBoolean();
                    break;
                case "integer":
                    this.valueControl = new ValueControlInteger();
                    break;
                case "real":
                    this.valueControl = new ValueControlReal();
                    break;
                case "StringExt":
                case "string":
                case "animationname":
                case "attachpoint":
                case "stringnoformat":
                case "aiscript":
                case "anyfile":
                case "preloadfile":
                case "imagefile":
                    this.valueControl = new ValueControlString(returnType);
                    break;
                case "unitcode":
                    this.valueControl = new ValueControlUnitTypes();
                    break;
                case "abilcode":
                    var abilities = controllerMapData.GetAbilitiesAll();
                    values = abilities.Select(a => new Value() { identifier = a.AbilCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "abilcode");
                    break;
                case "buffcode":
                    var buffs = controllerMapData.GetBuffsAll();
                    values = buffs.Select(b => new Value() { identifier = b.BuffCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "buffcode");
                    break;
                case "destructablecode":
                    var destructibles = controllerMapData.GetDestTypesAll();
                    values = destructibles.Select(d => new Value() { identifier = d.DestCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "destructablecode");
                    break;
                case "doodadcode":
                    var doodads = controllerMapData.GetDoodadTypesAll();
                    values = doodads.Select(d => new Value() { identifier = d.DoodCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "doodadcode");
                    break;
                case "techcode":
                    var upgrades = controllerMapData.GetUpgradeTypesAll();
                    values = upgrades.Select(up => new Value() { identifier = up.UpgradeCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "techcode");
                    break;
                case "itemcode":
                    var itemTypes = controllerMapData.GetItemTypesAll();
                    values = itemTypes.Select(i => new Value() { identifier = i.ItemCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "itemcode");
                    break;
                case "item":
                    var items = controllerMapData.GetMapItems();
                    values = items.Select(i => new Value() { identifier = $"{i.ToString()}_{i.CreationNumber}" }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "item");
                    break;
                case "unit":
                    var units = controllerMapData.GetMapUnits();
                    values = units.Select(u => new Value() { identifier = $"{u.ToString()}_{u.CreationNumber}" }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "unit");
                    break;
                case "destructable":
                    var mapDestructibles = controllerMapData.GetMapDests();
                    values = mapDestructibles.Select(dest => new Value() { identifier = $"{dest.ToString()}_{dest.CreationNumber}" }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "destructable");
                    break;
                case "rect":
                    var rects = controllerMapData.GetMapRegions();
                    values = rects.Select(rect => new Value() { identifier = rect.ToString() }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "rect");
                    break;
                case "camerasetup":
                    var cameras = controllerMapData.GetMapCameras();
                    values = cameras.Select(c => new Value() { identifier = c.ToString() }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "camerasetup");
                    break;
                case "sound":
                    var sound = controllerMapData.GetMapSounds();
                    values = sound.Select(s => new Value() { identifier = s.Name }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "sound");
                    break;
                case "musicfile":
                    var music = controllerMapData.GetMapMusic();
                    values = music.Select(m => new Value() { identifier = m.Name }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "musicfile");
                    break;
                case "modelfile": // TODO:
                    this.valueControl = new ValueControlModels();
                    break;
                case "trigger":
                    ControllerTrigger controllerTrigger = new ControllerTrigger();
                    var triggers = controllerTrigger.GetTriggersAll();
                    values = triggers.Select(trig => new Value() { identifier = trig.GetId().ToString() }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "trigger");
                    break;
                case "scriptcode":
                    this.valueControl = new ValueControlScript();
                    break;
                default:
                    break;
            }

            if (this.valueControl == null)
                return;

            var control = (UserControl)this.valueControl;
            this.grid.Children.Add(control);
            Grid.SetRow(control, 0);
            Grid.SetColumn(control, 0);
            control.Margin = new Thickness(10, 10, 10, 10);
            control.HorizontalAlignment = HorizontalAlignment.Stretch;
            control.VerticalAlignment = VerticalAlignment.Stretch;
            control.VerticalContentAlignment = VerticalAlignment.Stretch;
            control.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            valueControl.SelectionChanged += ValueControl_SelectionChanged;
        }

        private void ValueControl_SelectionChanged(object sender, EventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        public void SetDefaultSelection(string identifier)
        {
            valueControl.SetDefaultSelection(identifier);
        }

        public int GetElementCount()
        {
            return valueControl.GetElementCount();
        }

        public Parameter GetSelectedItem()
        {
            if (valueControl.GetSelected() == null)
                return null;

            return valueControl.GetSelected();
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        public bool ValueControlExists()
        {
            return valueControl != null;
        }
    }
}
