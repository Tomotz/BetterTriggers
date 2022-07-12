﻿using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using GUI.Components;
using GUI.Components.TextEditorExtensions;
using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;

namespace GUI.Components
{
    /// <summary>
    /// Interaction logic for TriggerControl.xaml
    /// </summary>
    public partial class CategoryControl : UserControl, IEditor
    {
        private ExplorerElementFolder explorerElementFolder;
        private List<TreeItemExplorerElement> observers = new List<TreeItemExplorerElement>();

        public CategoryControl(ExplorerElementFolder explorerElementFolder)
        {
            InitializeComponent();

            this.explorerElementFolder = explorerElementFolder;
        }
        
        public string GetSaveString()
        {
            return string.Empty;
        }

        public void SetElementEnabled(bool isEnabled)
        {
            throw new NotImplementedException();
        }

        public void SetElementInitiallyOn(bool isInitiallyOn)
        {
            throw new NotImplementedException();
        }

        public void Attach(TreeItemExplorerElement explorerElement)
        {
            this.observers.Add(explorerElement);
        }

        public void Detach(TreeItemExplorerElement explorerElement)
        {
            this.observers.Add(explorerElement);
        }

        public void OnStateChange()
        {
            foreach (var observer in observers)
            {
                observer.OnStateChange();
            }
        }

        public void OnRemoteChange()
        {
            throw new NotImplementedException();
        }
    }
}
