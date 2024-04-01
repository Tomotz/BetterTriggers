﻿using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BetterTriggers.Utility;
using Newtonsoft.Json;
using System.IO;

namespace BetterTriggers.Containers
{
    public class ActionDefinitions
    {
        private static HashSet<ExplorerElement> actionDefinitionContainer = new();
        private static ExplorerElement lastCreated;

        public void Add(ExplorerElement actionDefinition)
        {
            actionDefinitionContainer.Add(actionDefinition);
            lastCreated = actionDefinition;
        }

        /// <returns>Full file path.</returns>
        public string Create()
        {
            string directory = Project.CurrentProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateActionDefName();

            var actionDef = new ActionDefinition_Saveable()
            {
                Id = GenerateId(),
            };
            string json = JsonConvert.SerializeObject(actionDef);

            string fullPath = Path.Combine(directory, name);
            File.WriteAllText(fullPath, json);

            return fullPath;
        }


        internal string GenerateActionDefName(string name = "Untitled Action Definition")
        {
            string generatedName = name;
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Contains(generatedName))
                    ok = true;
                else
                {
                    generatedName = name + i;
                }

                i++;
            }

            return generatedName + ".act";
        }

        public int GenerateId()
        {
            int generatedId = 0;
            bool isIdValid = false;
            while (!isIdValid)
            {
                bool doesIdExist = false;
                var enumerator = actionDefinitionContainer.GetEnumerator();
                while (!doesIdExist && enumerator.MoveNext())
                {
                    if (enumerator.Current.actionDefinition.Id == generatedId)
                        doesIdExist = true;
                }

                if (!doesIdExist)
                    isIdValid = true;
                else
                    generatedId = RandomUtil.GenerateInt();
            }

            return generatedId;
        }

        public int Count()
        {
            return actionDefinitionContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public bool Contains(string name)
        {
            bool found = false;

            foreach (var item in actionDefinitionContainer)
            {
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    found = true;
                }
            }

            return found;
        }

        internal string GetName(int id)
        {
            var element = FindById(id);
            return element.GetName();
        }

        public ExplorerElement FindById(int id)
        {
            ExplorerElement actionDefinition = null;
            var enumerator = actionDefinitionContainer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.actionDefinition.Id == id)
                {
                    actionDefinition = enumerator.Current;
                    break;
                }
            }

            return actionDefinition;
        }

        public ExplorerElement GetLastCreated()
        {
            return lastCreated;
        }

        internal List<ExplorerElement> GetAll()
        {
            return actionDefinitionContainer.Select(x => x).ToList();
        }

        public void Remove(ExplorerElement explorerElement)
        {
            actionDefinitionContainer.Remove(explorerElement);
        }

        internal ExplorerElement GetByReference(ActionDefinition actionDefinitionRef)
        {
            return FindById(actionDefinitionRef.Id);
        }

        internal void Clear()
        {
            actionDefinitionContainer.Clear();
        }
    }
}