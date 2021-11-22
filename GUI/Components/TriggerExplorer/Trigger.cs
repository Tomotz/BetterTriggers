﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class Trigger : TriggerElement, ITriggerElement
    {
        public bool IsEnabled;
        public TriggerControl triggerControl;

        public Trigger(string name, TreeViewItem treeViewItem, TriggerControl triggerControl) : base(treeViewItem)
        {
            this.Name = name;
            this.IsEnabled = true;
            this.triggerControl = triggerControl;
        }

        public void Hide()
        {
            triggerControl.Visibility = Visibility.Hidden;
        }

        public void OnElementClick()
        {
            if (currentTriggerElement != null)
                currentTriggerElement.Hide();

            this.Show();

            currentTriggerElement = this;
        }

        public void Show()
        {
            triggerControl.Visibility = Visibility.Visible;
        }

        public string GetScript()
        {
            throw new NotImplementedException();
        }
    }
}
