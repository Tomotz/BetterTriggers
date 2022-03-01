﻿using Facades.Controllers;
using GUI.Utility;
using Model.Data;
using Model.EditorData;
using Model.EditorData.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerExplorer
{
    public class TreeItemExplorerElement : TreeViewItem, IExplorerElementObserver
    {
        public TabItemBT tabItem;
        public IExplorerElement Ielement;
        public IEditor editor;

        public TreeItemExplorerElement(IExplorerElement explorerElement)
        {
            this.Ielement = explorerElement;

            

            RefreshElement();
        }

        public void Delete()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var parent = this.Parent as TreeViewItem;
                if (parent != null)
                    parent.Items.Remove(this);

                if (tabItem != null)
                {
                    var tabControl = tabItem.Parent as TabControl;
                    if (tabControl != null)
                        tabControl.Items.Remove(tabItem);
                }
            });
        }

        public void RefreshElement()
        {
            //if (this.Ielement == null)
            //    return;

            Category category;

            if (Ielement is ExplorerElementRoot)
                category = Category.Map;
            else if (Ielement is ExplorerElementFolder)
                category = Category.Folder;
            else if (Ielement is ExplorerElementTrigger)
                category = Category.Trigger;
            else if (Ielement is ExplorerElementScript)
                category = Category.AI;
            else if (Ielement is ExplorerElementVariable)
                category = Category.SetVariable;
            else
                category = Category.Wait;

            TreeViewManipulator.SetTreeViewItemAppearance(this, this.Ielement.GetName(), category);

            if (this.tabItem != null)
                tabItem.RefreshHeader(this.Ielement.GetName());

            if(this.editor is VariableControl)
            {
                var control = this.editor as VariableControl;
                control.Rename(Ielement.GetName());
            }

            if (this.editor != null)
                this.editor.Refresh();
        }

        public void Save()
        {
            string saveableString = editor.GetSaveString();
            Ielement.SaveInMemory(saveableString);

            ControllerFileSystem controller = new ControllerFileSystem();
            controller.SaveFile(Ielement.GetPath(), saveableString);
        }

        public void Update(IExplorerElement subject)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                RefreshElement();
            });
        }
    }
}
