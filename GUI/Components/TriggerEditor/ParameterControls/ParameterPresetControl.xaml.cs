﻿using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterPresetControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterPresetControl(string returnType)
        {
            InitializeComponent();

            var presets = TriggerData.LoadAllConstants();
            List<Searchable> objects = new List<Searchable>();
            for (int i = 0; i < presets.Count; i++)
            {
                var preset = presets[i];
                if (preset.returnType != returnType)
                    continue;

                ListViewItem listItem = new ListViewItem();
                listItem.Content = preset.name;
                listItem.Tag = preset;
                objects.Add(new Searchable()
                {
                    Object = listItem,
                    Words = new List<string>()
                    {
                        preset.name.ToLower(),
                        preset.value.ToLower()
                    },
                });
            }

            var searchables = new Searchables(objects);
            listControl.SetSearchableList(searchables);

            var categoryControl = new GenericCategoryControl(searchables);
            grid.Children.Add(categoryControl);
            Grid.SetRow(categoryControl, 1);
            Grid.SetRowSpan(categoryControl, 3);

            listControl.listView.SelectionChanged += ListView_SelectionChanged;
        }

        public void SetDefaultSelection(Parameter parameter)
        {
            int i = 0;
            bool found = false;
            while (!found && i < listControl.listView.Items.Count)
            {
                var item = listControl.listView.Items[i] as ListViewItem;
                var constant = item.Tag as PresetTemplate;
                if (constant.value == parameter.value)
                    found = true;
                else
                    i++;
            }
            if (found == false)
                return;

            var defaultSelected = listControl.listView.Items[i] as ListViewItem;
            defaultSelected.IsSelected = true;
            listControl.listView.ScrollIntoView(defaultSelected);
        }

        public int GetElementCount()
        {
            return listControl.listView.Items.Count;
        }

        public Parameter GetSelectedItem()
        {
            if (selectedItem == null)
                return null;

            var template = (PresetTemplate)selectedItem.Tag;
            var parameter = new Preset()
            {
                value = template.value,
            };
            return parameter;
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listControl.listView.SelectedItem as ListViewItem;
            if (selectedItem == null)
                return;

            var constant = selectedItem.Tag as PresetTemplate;

            textBoxDescription.Inlines.Clear();
            var brush = (SolidColorBrush)Application.Current.FindResource("TextBrush");
            string colorCode = brush.Color.ToString().Replace("#", "");
            textBoxDescription.Inlines.AddRange(Utility.TextFormatter.War3ColoredText(Locale.Translate(constant.value), colorCode));
        }
    }
}