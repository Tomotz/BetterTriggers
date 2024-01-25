﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public abstract class TreeNodeBase
    {
        private string _displayText;
        private string _category;
        private byte[] _icon;

        public string DisplayText
        {
            get
            {
                return _displayText;
            }
            set
            {
                _displayText = value;
                RenameText = value;
            }
        }
        public string RenameText { get; set; }

        public byte[] IconImage
        {
            get
            {
                return _icon;
            }
        }

        public Visibility RenameBoxVisibility { get; set; }

        public void SetCategory(string categoryStr)
        {
            this._category = categoryStr;
            var category = Category.Get(_category);
            _icon = category.Icon;
        }
    }
}
