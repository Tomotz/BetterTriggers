﻿using Model.Data;
using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor
{
    public class TriggerEvent : TriggerElement
    {
        public readonly Model.Natives.Function _event;
        
        public TriggerEvent(Model.Natives.Function _event)
        {
            this._event = _event;
            this.parameters = _event.parameters;
            this.paramText = _event.paramText;
            this.descriptionTextBlock.Text = _event.description;
            this.category = _event.category;

            TreeViewManipulator.SetTreeViewItemAppearance(this, "placeholder", _event.category);
            this.FormatParameterText(paramTextBlock, this.parameters);
        }

        public void SetCategory(EnumCategory category)
        {
            this.category = _event.category;
        }
    }
}