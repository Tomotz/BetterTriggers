﻿using BetterTriggers;
using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Components;
using GUI.Components.Dialogs;
using GUI.Components.Shared;
using GUI.Components.VariableEditor;
using GUI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace GUI.Components
{
    public partial class VariableControl : UserControl, IEditor
    {
        public event Action OnChange;

        private Variable variable;
        private War3Type previousSelected;
        private bool isLoading = true;
        private int defaultSelected = 0;

        private string previousText0 = "1";
        private string previousText1 = "1";

        private bool preventStateChange = true;
        private bool suppressUIEvents = false;

        private VariableControlViewModel _viewModel;

        public VariableControl(Variable variable)
        {
            this.variable = variable;
            previousText0 = variable.ArraySize[0].ToString();
            previousText1 = variable.ArraySize[1].ToString();
            InitializeComponent();

            _viewModel = new VariableControlViewModel(variable);
            DataContext = _viewModel;

            var usedByList = Project.CurrentProject.References.GetReferrers(variable);
            usedByList.ForEach(r => _viewModel.ReferenceTriggers.Add(r));
            if (usedByList.Count == 0)
            {
                listViewUsedBy.Visibility = Visibility.Hidden;
                lblUsedBy.Visibility = Visibility.Hidden;
            }

            checkBoxIsArray.IsChecked = variable.IsArray;
            textBoxArraySize0.IsEnabled = variable.IsArray;
            comboBoxArrayDimensions.IsEnabled = variable.IsArray;
            textBoxArraySize0.Text = previousText0;
            textBoxArraySize1.Text = previousText1;
            if (!variable.IsTwoDimensions)
                comboBoxArrayDimensions.SelectedIndex = 0;
            else
            {
                comboBoxArrayDimensions.SelectedIndex = 1;
                textBoxArraySize1.IsEnabled = variable.IsArray;
            }

            preventStateChange = false;
        }

        private void Variable_ValuesChanged()
        {
            suppressUIEvents = true;
            Application.Current.Dispatcher.Invoke(delegate
            {
                UpdateIdentifierText();
                checkBoxIsArray.IsChecked = variable.IsArray;
                comboBoxArrayDimensions.SelectedIndex = variable.IsTwoDimensions ? 1 : 0;
                foreach (var i in comboBoxVariableType.Items)
                {
                    War3Type item = (War3Type)i;
                    if (item.Type == variable.Type)
                    {
                        comboBoxVariableType.SelectedItem = item;
                        break;
                    }
                }

            });

            suppressUIEvents = false;
        }

        private void comboBoxVariableType_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoading)
                return;

            comboBoxVariableType.SelectedIndex = defaultSelected;
            isLoading = false;
        }

        public void UpdateIdentifierText()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                this.textBlockVariableNameUDG.Text = variable.GetIdentifierName();
            });
        }

        private void comboBoxVariableType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading || suppressUIEvents)
                return;

            if (ResetVarRefs())
            {
                var selected = (War3Type)comboBoxVariableType.SelectedItem;

                CommandVariableModifyType command = new CommandVariableModifyType(variable, selected.Type);
                command.Execute();
                OnStateChange();

                previousSelected = (War3Type)comboBoxVariableType.SelectedItem;
                defaultSelected = comboBoxVariableType.SelectedIndex;

                ParamTextBuilder controllerParamText = new ParamTextBuilder();
                this.textblockInitialValue.Inlines.Clear();
                var inlines = controllerParamText.GenerateParamText(variable);
                this.textblockInitialValue.Inlines.AddRange(inlines);
            }
            else
            {
                comboBoxVariableType.SelectedItem = previousSelected;
                e.Handled = false;
            }
        }

        private void checkBoxIsArray_Click(object sender, RoutedEventArgs e)
        {
            if (isLoading)
                return;

            if (ResetVarRefs())
            {
                CommandVariableModifyArray command = new CommandVariableModifyArray(variable, (bool)checkBoxIsArray.IsChecked);
                command.Execute();
                OnStateChange();

                textBoxArraySize0.IsEnabled = (bool)checkBoxIsArray.IsChecked;
                comboBoxArrayDimensions.IsEnabled = (bool)checkBoxIsArray.IsChecked;
                if (comboBoxArrayDimensions.SelectedIndex == 1)
                    textBoxArraySize1.IsEnabled = (bool)checkBoxIsArray.IsChecked;
            }
            else
            {
                checkBoxIsArray.IsChecked = !checkBoxIsArray.IsChecked;
                e.Handled = false;
            }
        }


        private void comboBoxArrayDimensions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading)
                return;

            bool isTwoDimensions = comboBoxArrayDimensions.SelectedIndex == 1;
            if (ResetVarRefs())
            {
                CommandVariableModifyDimension command = new CommandVariableModifyDimension(variable, isTwoDimensions);
                command.Execute();
                OnStateChange();
            }
            else
            {
                if (!variable.IsTwoDimensions)
                    comboBoxArrayDimensions.SelectedIndex = 0;
                else
                    comboBoxArrayDimensions.SelectedIndex = 1;

                e.Handled = false;
            }

            if (!isTwoDimensions)
                textBoxArraySize1.IsEnabled = false;
            else
                textBoxArraySize1.IsEnabled = true;
        }

        private bool ResetVarRefs()
        {
            bool ok = true;
            List<ExplorerElement> refs = Project.CurrentProject.References.GetReferrers(this.variable);
            if (refs.Count > 0)
            {
                DialogBoxReferences dialog = new DialogBoxReferences(refs, ExplorerAction.Reset);
                dialog.ShowDialog();
                ok = dialog.OK;
            }

            return ok;
        }

        public void SetElementEnabled(bool isEnabled)
        {
            throw new NotImplementedException();
        }

        public void SetElementInitiallyOn(bool isInitiallyOn)
        {
            throw new NotImplementedException();
        }

        public void OnStateChange()
        {
            OnChange?.Invoke();
        }

        public void OnRemoteChange()
        {
            // TODO: Why is this here?
            throw new Exception("Hello. Notice this call plz.");
        }

        private void textBoxArraySize0_TextChanged(object sender, TextChangedEventArgs e)
        {
            ArraySizeTextChanged(e);
        }

        private void textBoxArraySize1_TextChanged(object sender, TextChangedEventArgs e)
        {
            ArraySizeTextChanged(e);
        }

        private void ArraySizeTextChanged(TextChangedEventArgs e)
        {
            // prevent exception and state change on init
            if (textBoxArraySize0 == null || textBoxArraySize1 == null || preventStateChange)
                return;

            try
            {
                int size0 = int.Parse(textBoxArraySize0.Text);
                int size1 = int.Parse(textBoxArraySize1.Text);
                if (size0 < 1 || size1 < 1)
                    throw new Exception("Array dimension cannot go below 1.");

                textBoxArraySize0.Text = textBoxArraySize0.Text;
                textBoxArraySize1.Text = textBoxArraySize1.Text;
                previousText0 = textBoxArraySize0.Text;
                previousText1 = textBoxArraySize1.Text;

                variable.ArraySize[0] = size0;
                variable.ArraySize[1] = size1;

                OnStateChange();
            }
            catch (Exception ex)
            {
                textBoxArraySize0.Text = previousText0;
                textBoxArraySize1.Text = previousText1;
            }

            //e.Handled = true;
        }

        internal void Dispose()
        {
            variable.PropertyChanged -= Variable_ValuesChanged;
        }

        private void listViewUsedBy_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = listViewUsedBy.SelectedItem as ListViewItem;
            if (item == null)
                return;

            var treeNode = (TreeNodeBase) listViewUsedBy.ItemContainerGenerator.ItemFromContainer(item);
            var triggerExplorer = TriggerExplorer.Current;
            triggerExplorer.Search(treeNode.DisplayText);
        }
    }
}
