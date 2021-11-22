﻿using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor
{
    public class CategoryEvent : TreeViewItem
    {
        public CategoryEvent()
        {
            TreeViewManipulator.SetTreeViewItemAppearance(this, "Events", "Resources/editor-triggerevent.png");
        }
    }
}
