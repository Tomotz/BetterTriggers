﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyType : ICommand
    {
        string commandName = "Modify Variable Type";
        Variable variable;
        string selectedType;
        string previousType;
        Parameter newInitialValue;
        Parameter previousInitialValue;
        RefCollection refCollection;

        public CommandVariableModifyType(Variable variable, string selectedType)
        {
            this.variable = variable;
            this.selectedType = selectedType;
            this.previousType = variable.Type;
            this.previousInitialValue = variable.InitialValue;
            this.newInitialValue = new Parameter();
            this.refCollection = new RefCollection(variable);
        }

        public void Execute()
        {
            refCollection.RemoveRefsFromParent();
            References.UpdateReferences(variable);
            variable.SuppressChangedEvent = true;
            variable.InitialValue = newInitialValue;
            variable.SuppressChangedEvent = false;
            variable.Type = selectedType;

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            refCollection.RemoveRefsFromParent();
            References.UpdateReferences(variable);
            variable.SuppressChangedEvent = true;
            variable.InitialValue = newInitialValue;
            variable.SuppressChangedEvent = false;
            variable.Type = selectedType;
        }

        public void Undo()
        {
            variable.SuppressChangedEvent = true;
            variable.InitialValue = previousInitialValue;
            variable.SuppressChangedEvent = false;
            variable.Type = previousType;
            refCollection.AddRefsToParent();
            References.UpdateReferences(variable);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
