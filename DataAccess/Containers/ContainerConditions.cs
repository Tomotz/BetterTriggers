﻿
using Model.Natives;
using System;
using System.Collections.Generic;

namespace Model.Containers
{
    public static class ContainerConditions
    {
        private static List<Natives.Condition> container = new List<Natives.Condition>();

        public static int Size()
        {
            return container.Count;
        }
        
        public static void AddCondition(Natives.Condition constant)
        {
            bool alreadyExists = false;
            string whichType = string.Empty;
            int index = 0;

            for(int i = 0; i < container.Count; i++)
            {
                if(container[i].identifier == constant.identifier)
                {
                    alreadyExists = true;
                    whichType = container[i].identifier;
                    index = i;
                    break;
                }
            }

            if (!alreadyExists)
                container.Add(constant);
            else
                Console.WriteLine($"At {index}: Type '{constant.identifier}' already exists as '{whichType}' in the container.");
        }

        public static List<Natives.Condition> GetAllTypes()
        {
            return container;
        }

        public static void SetList(List<Natives.Condition> list)
        {
            if (list != null)
                container = list;
            else
                container = new List<Natives.Condition>();
        }
    }
}