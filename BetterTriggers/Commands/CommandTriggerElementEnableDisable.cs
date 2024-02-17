﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementEnableDisable : ICommand
    {
        string commandName = "Change Enable Trigger Element";
        ECA triggerElement;

        public CommandTriggerElementEnableDisable(ECA triggerElement)
        {
            this.triggerElement = triggerElement;
        }

        public void Execute()
        {
            triggerElement.isEnabled = !triggerElement.isEnabled;
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            triggerElement.isEnabled = !triggerElement.isEnabled;
        }

        public void Undo()
        {
            triggerElement.isEnabled = !triggerElement.isEnabled;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
