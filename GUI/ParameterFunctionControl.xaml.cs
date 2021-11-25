﻿using System;
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
    public partial class ParameterFunctionControl : UserControl
    {
        public ParameterFunctionControl(string returnType)
        {
            InitializeComponent();

            List<DataAccess.Natives.Function> functions = DataAccess.LoadData.LoadAllFunctions(@"C:\Users\Lasse Dam\Desktop\JSON\functions.json");

            for (int i = 0; i < functions.Count; i++)
            {
                if(functions[i].returnType.type == returnType)
                {
                    ListViewItem item = new ListViewItem();
                    item.Content = functions[i].name;
                    item.Tag = functions[i];

                    listViewFunction.Items.Add(item);
                }
            }
        }
    }
}
