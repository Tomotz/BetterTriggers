﻿using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class EnumDestructiblesInCircleBJMultiple : TriggerElement
    {
        public readonly int ElementType = 11; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();

        public EnumDestructiblesInCircleBJMultiple()
        {
            function.identifier = "EnumDestructablesInCircleBJMultiple";
        }

        public new EnumDestructiblesInCircleBJMultiple Clone()
        {
            EnumDestructiblesInCircleBJMultiple enumDest = new EnumDestructiblesInCircleBJMultiple();
            enumDest.function = this.function.Clone();
            enumDest.Actions = new List<TriggerElement>();
            Actions.ForEach(element => enumDest.Actions.Add(element.Clone()));

            return enumDest;
        }
    }
}
