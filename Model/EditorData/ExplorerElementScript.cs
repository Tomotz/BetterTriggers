﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Model.EditorData
{
    public class ExplorerElementScript : IExplorerElement
    {
        public string path;
        public string script;
        public bool isEnabled = true;
        public List<IExplorerElementObserver> observers = new List<IExplorerElementObserver>();

        public ExplorerElementScript(string path)
        {
            this.path = path;
            this.script = File.ReadAllText(path);
        }

        public string GetName()
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetPath()
        {
            return path;
        }

        public void SetPath(string newPath)
        {
            this.path = newPath;
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
            this.script = saveableString;
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

        public void InsertIntoList(IExplorerElement element, int insertIndex)
        {
            throw new Exception("This is not a directory");
        }

        public void RemoveFromList(IExplorerElement element)
        {
            throw new Exception("This is not a directory");
        }

        public void SetEnabled(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }

        public void SetInitiallyOn(bool isInitiallyOn)
        {
            throw new NotImplementedException();
        }

        public bool GetEnabled()
        {
            return this.isEnabled;
        }

        public bool GetInitiallyOn()
        {
            return true;
        }

        public string GetSaveableString()
        {
            return script;
        }
    }
}