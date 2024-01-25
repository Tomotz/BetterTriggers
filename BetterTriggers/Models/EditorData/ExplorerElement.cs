﻿using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace BetterTriggers.Models.EditorData
{
    public class ExplorerElement : TreeNodeBase
    {
        public ExplorerElementEnum ElementType { get; private set; }
        public string path { get; set; }
        public ExplorerElement Parent { get; set; }
        public ObservableCollection<ExplorerElement> ExplorerElements { get; set; } = new();
        public bool HasUnsavedChanges { get; set; }
        public bool isEnabled = true;
        public bool isInitiallyOn = true;
        public bool isExpanded = false;
        public event Action OnReload;
        private DateTime LastWrite;
        private long Size;

        public Trigger trigger;
        public Variable variable;
        public string script;


        /// <summary>Reserved for copy-pasting purposes.</summary>
        public ExplorerElement() { }

        /// <summary>Reserved for TriggerConverter.</summary>
        public ExplorerElement(ExplorerElementEnum type)
        {
            ElementType = type;
        }

        public ExplorerElement(string path)
        {
            this.path = path;
            string extension = Path.GetExtension(path);
            bool isReadyForRead = false;
            int sleepTolerance = 100;
            string json = string.Empty;
            switch (extension)
            {
                case ".trg":
                    ElementType = ExplorerElementEnum.Trigger;
                    SetCategory(TriggerCategory.TC_TRIGGER_NEW);
                    while (!isReadyForRead)
                    {
                        try
                        {
                            json = File.ReadAllText(path);
                            isReadyForRead = true;
                        }
                        catch (Exception ex)
                        {
                            if (sleepTolerance < 0)
                                throw new Exception(ex.Message);

                            Thread.Sleep(100);
                            sleepTolerance--;
                        }
                    }
                    trigger = JsonConvert.DeserializeObject<Trigger>(json);
                    StoreLocalVariables();
                    Project.CurrentProject.Triggers.AddTrigger(this);

                    break;
                case ".j":
                case ".lua":
                    ElementType = ExplorerElementEnum.Script;
                    SetCategory(TriggerCategory.TC_SCRIPT);
                    this.script = Project.CurrentProject.Scripts.LoadFromFile(path);
                    Project.CurrentProject.Scripts.AddScript(this);
                    break;
                case ".var":
                    ElementType = ExplorerElementEnum.GlobalVariable;
                    SetCategory(TriggerCategory.TC_SETVARIABLE);
                    while (!isReadyForRead)
                    {
                        try
                        {
                            json = File.ReadAllText(path);
                            isReadyForRead = true;
                        }
                        catch (Exception ex)
                        {
                            if (sleepTolerance < 0)
                                throw new Exception(ex.Message);

                            Thread.Sleep(100);
                            sleepTolerance--;
                        }
                    }

                    variable = JsonConvert.DeserializeObject<Variable>(json);
                    UpdateMetadata();
                    Project.CurrentProject.Variables.AddVariable(this);

                    variable.Name = Path.GetFileNameWithoutExtension(GetPath());
                    break;
                case "":
                    ElementType = ExplorerElementEnum.Folder;
                    SetCategory(TriggerCategory.TC_DIRECTORY);
                    break;
                default:
                    break;
            }


            UpdateMetadata();
        }

        public string GetName()
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetPath()
        {
            return path;
        }

        /// <summary>
        /// Returns the relative path of this file in the 'src' directory.
        /// </summary>
        public string GetRelativePath()
        {
            string relativePath = this.path;
            while (true)
            {
                relativePath = Path.GetDirectoryName(relativePath);
                if (Path.GetFileName(relativePath) == "src")
                {
                    relativePath = relativePath + "\\";
                    relativePath = this.path.Replace(relativePath, "");
                    break;
                }
            }
            return relativePath;
        }

        public void SetPath(string newPath)
        {
            this.path = newPath;
        }

        public int GetId()
        {
            switch (ElementType)
            {
                case ExplorerElementEnum.GlobalVariable:
                    return variable.Id;
                case ExplorerElementEnum.Trigger:
                    return trigger.Id;
                default:
                    throw new Exception($"Element type '{ElementType}' cannot return an ID.");
            }
        }

        public void SetEnabled(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }

        public void SetInitiallyOn(bool isInitiallyOn)
        {
            this.isInitiallyOn = isInitiallyOn;
        }

        public bool GetEnabled()
        {
            return this.isEnabled;
        }

        public bool GetInitiallyOn()
        {
            return this.isInitiallyOn;
        }

        public long GetSize()
        {
            return Size;
        }

        public DateTime GetLastWrite()
        {
            return LastWrite;
        }

        public void UpdateMetadata()
        {
            if (ElementType == ExplorerElementEnum.Folder)
            {
                var info = new DirectoryInfo(path);
                this.Size = info.EnumerateFiles().Sum(file => file.Length);
                this.LastWrite = info.LastWriteTime;
            }
            else
            {
                var info = new FileInfo(path);
                this.Size = info.Length;
                this.LastWrite = info.LastWriteTime;
            }
        }

        public ExplorerElement GetParent()
        {
            if (ElementType == ExplorerElementEnum.Root)
                throw new Exception("Root is the super parent");

            return Parent;
        }

        public void SetParent(ExplorerElement parent, int insertIndex)
        {
            if (ElementType == ExplorerElementEnum.Root)
                throw new Exception("Root is the super parent");

            Parent = parent;
            parent.GetExplorerElements().Insert(insertIndex, this);
            if (ElementType == ExplorerElementEnum.Trigger)
            {
                StoreLocalVariables();
            }
        }

        public void RemoveFromParent()
        {
            Parent.GetExplorerElements().Remove(this);
            Parent = null;
            if (ElementType == ExplorerElementEnum.Trigger)
            {
                RemoveLocalVariables();
            }
        }

        public void AddToUnsaved()
        {
            Project.CurrentProject.UnsavedFiles.AddToUnsaved(this);
        }

        public void RemoveFromUnsaved(bool recursive = false)
        {
            Project.CurrentProject.UnsavedFiles.RemoveFromUnsaved(this);
            if (recursive && ExplorerElements.Count > 0)
            {
                for (int i = 0; i < ExplorerElements.Count; i++)
                {
                    ExplorerElements[i].RemoveFromUnsaved(true);
                }
            }
        }

        public ObservableCollection<ExplorerElement> GetExplorerElements()
        {
            if (ElementType != ExplorerElementEnum.Folder || ElementType != ExplorerElementEnum.Root)
            {
                throw new Exception("'" + path + "' is not a folder.");
            }

            return ExplorerElements;
        }

        public ExplorerElement Clone()
        {
            ExplorerElement newElement = new ExplorerElement();
            newElement.path = new string(this.path); // we need this path in paste command.
            newElement.Parent = this.Parent;
            newElement.isInitiallyOn = this.isInitiallyOn;
            newElement.isEnabled = this.isEnabled;

            switch (ElementType)
            {
                case ExplorerElementEnum.Folder:
                    foreach (var element in ExplorerElements)
                    {
                        newElement.ExplorerElements.Add(element.Clone());
                    }
                    break;
                case ExplorerElementEnum.GlobalVariable:
                    newElement.variable = this.variable.Clone();
                    break;
                case ExplorerElementEnum.Script:
                    break;
                case ExplorerElementEnum.Trigger:
                    newElement.trigger = this.trigger.Clone();
                    break;
                case ExplorerElementEnum.Root:
                    throw new Exception("Cannot clone Root.");
                default:
                    break;
            }

            return newElement;
        }

        public void Save()
        {
            string fileContent;
            switch (ElementType)
            {
                case ExplorerElementEnum.GlobalVariable:
                    fileContent = JsonConvert.SerializeObject(variable, Formatting.Indented);
                    File.WriteAllText(path, fileContent);
                    break;
                case ExplorerElementEnum.Script:
                    File.WriteAllText(path, script);
                    break;
                case ExplorerElementEnum.Trigger:
                    fileContent = JsonConvert.SerializeObject(trigger, Formatting.Indented);
                    File.WriteAllText(path, fileContent);
                    break;
                case ExplorerElementEnum.Folder:
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    break;
                default:
                    break;
            }

            HasUnsavedChanges = false;
        }


        public void Rename()
        {
            var project = Project.CurrentProject;

            string oldPath = GetPath();

            string formattedName = string.Empty;
            if (ElementType == ExplorerElementEnum.Folder)
                formattedName = RenameText;
            else if (ElementType is ExplorerElementEnum.Trigger)
            {
                if (project.Triggers.Contains(RenameText))
                    throw new Exception($"Trigger '{RenameText}' already exists.");

                formattedName = RenameText + ".trg";
            }
            else if (ElementType == ExplorerElementEnum.Script)
                formattedName = RenameText + ".j";
            else if (ElementType == ExplorerElementEnum.GlobalVariable)
            {
                if (project.Variables.Contains(RenameText))
                    throw new Exception($"Variable '{RenameText}' already exists.");

                formattedName = RenameText + ".var";
            }

            FileSystemUtil.Rename(oldPath, formattedName);
        }

        public void Notify()
        {
            if (ElementType == ExplorerElementEnum.Script)
            {
                this.script = Project.CurrentProject.Scripts.LoadFromFile(GetPath());
                OnReload?.Invoke();
            }
        }

        public List<ExplorerElement> GetReferrers()
        {
            switch (ElementType)
            {
                case ExplorerElementEnum.GlobalVariable:
                    return Project.CurrentProject.References.GetReferrers(variable);
                case ExplorerElementEnum.Trigger:
                    return Project.CurrentProject.References.GetReferrers(trigger);
                default:
                    throw new Exception($"'{ElementType} cannot make references to GUI objects.'");
            }
        }

        private void StoreLocalVariables()
        {
            var variables = Project.CurrentProject.Variables;
            trigger.LocalVariables.ForEach(e =>
            {
                var lv = (LocalVariable)e;
                variables.AddLocalVariable(lv);
            });
        }

        private void RemoveLocalVariables()
        {
            var variables = Project.CurrentProject.Variables;
            trigger.LocalVariables.ForEach(e =>
            {
                var lv = (LocalVariable)e;
                variables.RemoveLocalVariable(lv);
            });
        }
    }
}