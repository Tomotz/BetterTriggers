﻿using DataAccess.Natives;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for ParameterFunctionControl.xaml
    /// </summary>
    public partial class ParameterFunctionControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;

        public ParameterFunctionControl(string returnType)
        {
            InitializeComponent();

            List<Function> functions = DataAccess.LoadData.LoadAllFunctions(@"C:\Users\Lasse Dam\Desktop\JSON\functions.json");

            for (int i = 0; i < functions.Count; i++)
            {
                if(functions[i].returnType.type == returnType)
                {
                    ListViewItem item = new ListViewItem();
                    item.Content = functions[i].name;
                    item.Tag = functions[i];

                    listViewFunctions.Items.Add(item);
                    this.selectedItem = listViewFunctions.Items.GetItemAt(0) as ListViewItem;
                }
            }
        }

        public int GetElementCount()
        {
            return listViewFunctions.Items.Count;
        }

        public Parameter GetSelectedItem()
        {
            var parameter = (Function)selectedItem.Tag;
            return parameter;
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        private void listViewFunction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = listViewFunctions.SelectedItem as ListViewItem;
        }
    }
}
