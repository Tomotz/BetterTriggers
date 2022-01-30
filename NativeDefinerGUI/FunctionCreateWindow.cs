﻿using Model.Containers;
using Model.Data;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model.Enums;
using Model.Templates;
using Model.SavableTriggerData;

namespace NativeDefinerGUI
{
    public partial class FunctionCreateWindow : Form
    {
        public ListViewItem returns;
        
        public FunctionCreateWindow()
        {
            InitializeComponent();

            /*
            var types = ContainerTypes.GetAllTypes();
            for(int i = 0; i < ContainerTypes.Size(); i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = types[i].name;
                item.Tag = types[i];

                listViewTypes.Items.Add(item);
            }
            */

            // Populate list of categories
            var enumCategoryValues = Enum.GetValues(typeof(Category));
            for (int i = 0; i < enumCategoryValues.Length; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = Enum.GetName(typeof(Category), enumCategoryValues.GetValue(i));
                item.Tag = enumCategoryValues.GetValue(i);
                listViewCategory.Items.Add(item);
            }
        }

        private void btnAddParam_Click(object sender, EventArgs e)
        {
            if(listViewTypes.SelectedItems.Count > 0)
            {
                var selectedItem = listViewTypes.SelectedItems[0];
                var itemToAdd = new ListViewItem();
                itemToAdd.Text = selectedItem.Text;
                itemToAdd.Tag = selectedItem.Tag;
                listViewParameters.Items.Add(itemToAdd);
            }
        }

        private void btnRemoveParam_Click(object sender, EventArgs e)
        {
            if (listViewParameters.SelectedItems.Count > 0)
            {
                var item = listViewParameters.SelectedItems[0];
                listViewParameters.Items.Remove(item);
            }
        }

        private void btnAddReturnType_Click(object sender, EventArgs e)
        {
            var selectedItem = listViewTypes.SelectedItems[0];
            lblReturnType.Text = selectedItem.Text;
            lblReturnType.Tag = selectedItem.Tag;

            btnCreateFunction.Enabled = true;
        }

        private void btnCreateParam_Click(object sender, EventArgs e)
        {
            List<Parameter> parameters = new List<Parameter>();
            for (int i = 0; i < listViewParameters.Items.Count; i++)
            {
                var type = (string) listViewParameters.Items[i].Tag;
                Parameter parameter = new Parameter()
                {
                    returnType = type
                };
                parameters.Add(parameter);
            }

            var selectedCategory = listViewCategory.SelectedItems[0];
            var category = (Category)selectedCategory.Tag;

           FunctionTemplate _function = new FunctionTemplate()
            {
                identifier = textBoxIdentifier.Text,
                parameters = parameters,
                returnType = (string) lblReturnType.Tag,
                name = textBoxName.Text,
                paramText = richTextParamText.Text,
                description = richTextDescription.Text,
                category = category
            };

            Dispose();
        }
    }
}
