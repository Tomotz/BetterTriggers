﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUI.Components.TriggerEditor;
using Model.Natives;

namespace GUI.Commands
{
    public class CommandTriggerElementPaste : ICommand
    {
        string commandName = "Paste Trigger Element";
        Components.TriggerEditor.TriggerElement triggerElement;
        TreeViewItem parent;
        int pastedIndex = 0;

        public CommandTriggerElementPaste(Function function, TreeViewItem parent, int pastedIndex)
        {
            if (parent is NodeEvent)
                this.triggerElement = new Components.TriggerEditor.TriggerEvent(function);
            else if (parent is NodeCondition)
                throw new NotImplementedException();
            else if (parent is NodeAction)
                this.triggerElement = new Components.TriggerEditor.TriggerAction(function);

            this.parent = parent;
            this.pastedIndex = pastedIndex;
        }

        public void Execute()
        {
            parent.Items.Insert(this.pastedIndex, triggerElement);
            
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            parent.Items.Insert(this.pastedIndex, triggerElement);
        }

        public void Undo()
        {
            parent.Items.Remove(this.triggerElement);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}