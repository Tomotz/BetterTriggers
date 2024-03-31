﻿using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace BetterTriggers.Models.EditorData
{
    public class ExplorerElement : TreeNodeBase
    {
        public ExplorerElementEnum ElementType { get; private set; }
        public string path
        {
            get => _path;
            set
            {
                _path = value;
                DisplayText = Path.GetFileNameWithoutExtension(value);
            }
        }
        public ExplorerElement Parent { get; set; }
        public ObservableCollection<ExplorerElement> ExplorerElements { get; set; } = new();
        public bool IsInitiallyOn
        {
            get => _isInitiallyOn;
            set
            {
                _isInitiallyOn = value;
                OnPropertyChanged();
                OnToggleInitiallyOn?.Invoke();
            }
        }
        public event Action OnReload;
        public event Action OnChanged;
        public event Action OnSaved;
        public event Action OnDeleted;
        public event Action OnToggleInitiallyOn;
        private string _path;
        private bool _isInitiallyOn = true;
        public DateTime LastWrite { get; private set; }
        public long Size { get; private set; }

        public Trigger trigger;
        public Variable variable;
        public string script;
        public ActionDefinition actionDefinition;
        public ConditionDefinition conditionDefinition;

        public UserControl editor;


        /// <summary>Reserved for copy-pasting purposes.</summary>
        public ExplorerElement() { }

        /// <summary>Reserved for TriggerConverter.</summary>
        public ExplorerElement(ExplorerElementEnum type)
        {
            ElementType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="explicitType">Only used when creating root on init. hack...</param>
        /// <exception cref="Exception"></exception>
        public ExplorerElement(string path, ExplorerElementEnum explicitType = ExplorerElementEnum.None)
        {
            this.path = path;
            bool isReadyForRead = false;
            int sleepTolerance = 100;
            string json = string.Empty;

            if (Directory.Exists(path))
            {
                ElementType = ExplorerElementEnum.Folder;
                SetCategory(TriggerCategory.TC_DIRECTORY);
            }
            else if (File.Exists(path))
            {
                string extension = Path.GetExtension(path);
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
                        var savedTrigger = JsonConvert.DeserializeObject<Trigger_Saveable>(json);
                        trigger = TriggerSerializer.Deserialize(savedTrigger);
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

                        var savedVariable = JsonConvert.DeserializeObject<Variable_Saveable>(json);
                        variable = TriggerSerializer.DeserializeVariable(savedVariable);
                        variable.PropertyChanged += AddToUnsaved;
                        UpdateMetadata();
                        Project.CurrentProject.Variables.AddVariable(this);

                        variable.Name = Path.GetFileNameWithoutExtension(GetPath());
                        break;
                    default:
                        ElementType = ExplorerElementEnum.None;
                        break;
                }
            }

            if (explicitType == ExplorerElementEnum.Root)
            {
                var project = Project.CurrentProject;
                DisplayText = Path.GetFileNameWithoutExtension(project.war3project.Name);
                ElementType = ExplorerElementEnum.Root;
                SetCategory(TriggerCategory.TC_MAP);
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

        public void UpdateMetadata()
        {
            if (ElementType == ExplorerElementEnum.Folder || ElementType == ExplorerElementEnum.Root)
            {
                var info = new DirectoryInfo(path);
                this.Size = info.EnumerateFiles().Sum(file => file.Length);
                this.LastWrite = info.LastWriteTime;
            }
            else if (ElementType != ExplorerElementEnum.None)
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ElementType == ExplorerElementEnum.Root)
                    throw new Exception("Root is the super parent");

                Parent = parent;
                parent.GetExplorerElements().Insert(insertIndex, this);
                if (ElementType == ExplorerElementEnum.Trigger)
                {
                    StoreLocalVariables();
                }
            });
        }

        public void RemoveFromParent()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Parent.GetExplorerElements().Remove(this);
                Parent = null;
                if (ElementType == ExplorerElementEnum.Trigger)
                {
                    RemoveLocalVariables();
                }
            });
        }

        public void AddToUnsaved()
        {
            var project = Project.CurrentProject;
            if (project.IsLoading)
                return;

            project.UnsavedFiles.AddToUnsaved(this);
            OnChanged?.Invoke();
        }

        public void RemoveFromUnsaved(bool recursive = false)
        {
            if (Project.CurrentProject == null)
                return;

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
            if (ElementType != ExplorerElementEnum.Folder && ElementType != ExplorerElementEnum.Root)
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
            newElement.IsInitiallyOn = this.IsInitiallyOn;
            newElement.IsEnabled = this.IsEnabled;
            newElement.ElementType = this.ElementType;
            newElement.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(newElement.IconImage, 0);

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

        /// <summary>
        /// Writes the content of the ExplorerElement to disk.
        /// </summary>
        public void Save()
        {
            if (!Directory.Exists(Path.GetDirectoryName(this.path))) // Edge case when a folder containing the file was deleted.
            {
                return;
            }

            string fileContent;
            switch (ElementType)
            {
                case ExplorerElementEnum.GlobalVariable:
                    fileContent = TriggerSerializer.SerializeVariable(variable);
                    File.WriteAllText(path, fileContent);
                    break;
                case ExplorerElementEnum.Script:
                    File.WriteAllText(path, script);
                    break;
                case ExplorerElementEnum.Trigger:
                    fileContent = TriggerSerializer.Serialize(trigger);
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

            RemoveFromUnsaved();
            OnSaved?.Invoke();
        }


        public void Rename()
        {
            if (RenameText == DisplayText)
            {
                RenameBoxVisibility = Visibility.Hidden;
                return;
            }

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
            RenameBoxVisibility = Visibility.Hidden;
        }

        public void Delete()
        {
            FileSystemUtil.Delete(path);
        }

        public void Notify()
        {
            if (ElementType == ExplorerElementEnum.Script)
            {
                this.script = Project.CurrentProject.Scripts.LoadFromFile(GetPath());
                OnReload?.Invoke();
            }
            else if (ElementType == ExplorerElementEnum.Trigger)
            {
                VerifyAndRemoveTriggerErrors();
            }
        }

        public void InvokeChange()
        {
            VerifyAndRemoveTriggerErrors();
            UpdateVariableIdentifier();
            OnChanged?.Invoke();
            AddToUnsaved();
        }

        public void InvokeDelete()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                OnDeleted?.Invoke();
                foreach (var element in ExplorerElements)
                {
                    element.InvokeDelete();
                }
            });
        }

        private void VerifyAndRemoveTriggerErrors()
        {
            if (ElementType == ExplorerElementEnum.Trigger)
            {
                TriggerValidator validator = new TriggerValidator(this, true);
                int errors = validator.RemoveInvalidReferences();
                HasErrors = errors > 0;
            }
        }

        private void UpdateVariableIdentifier()
        {
            if(ElementType == ExplorerElementEnum.GlobalVariable)
            {
                variable.Name = this.GetName();
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
                case ExplorerElementEnum.Folder:
                    return ExplorerElements.SelectMany(el => el.GetReferrers()).ToList();
                default:
                    return new List<ExplorerElement>();
            }
        }

        private void StoreLocalVariables()
        {
            var variables = Project.CurrentProject.Variables;
            trigger.LocalVariables.Elements.ForEach(e =>
            {
                var lv = (LocalVariable)e;
                variables.AddLocalVariable(lv);
            });
        }

        private void RemoveLocalVariables()
        {
            var variables = Project.CurrentProject.Variables;
            trigger.LocalVariables.Elements.ForEach(e =>
            {
                var lv = (LocalVariable)e;
                variables.RemoveLocalVariable(lv);
            });
        }
    }
}