﻿using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Model.EditorData
{
    public class ExplorerElementRoot : IExplorerElement
    {
        public string path;
        public War3Project project;
        public List<IExplorerElement> explorerElements = new List<IExplorerElement>();
        public List<IExplorerElementObserver> observers = new List<IExplorerElementObserver>();

        public ExplorerElementRoot(War3Project project, string path)
        {
            this.path = path;
            this.project = project;
        }

        public string GetName()
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetPath()
        {
            return project.Root;
        }

        public void SetPath(string newPath)
        {
            this.path = newPath;
        }

        public void InsertIntoList(IExplorerElement element, int insertIndex)
        {
            explorerElements.Insert(insertIndex, element);
        }

        public void RemoveFromList(IExplorerElement element)
        {
            explorerElements.Remove(element);
        }

        public void Attach(IExplorerElementObserver observer)
        {
            this.observers.Add(observer);
        }

        public void Detach(IExplorerElementObserver observer)
        {
            this.observers.Remove(observer);
        }

        public void Notify()
        {
            //foreach (var observer in observers)
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].Update(this);
            }
        }

        public void SaveInMemory(string saveableString)
        {
            project = JsonConvert.DeserializeObject<War3Project>(saveableString);
        }

        public void DeleteObservers()
        {
            foreach (var observer in observers)
            {
                observer.Delete();
            }
        }

        public int GetId()
        {
            throw new NotImplementedException();
        }

        public void SetEnabled(bool isEnabled)
        {
            throw new NotImplementedException();
        }

        public void SetInitiallyOn(bool isInitiallyOn)
        {
            throw new NotImplementedException();
        }

        public bool GetEnabled()
        {
            return true;
        }

        public bool GetInitiallyOn()
        {
            return true;
        }

        public string GetSaveableString()
        {
            throw new NotImplementedException("Is root.");
        }
    }
}