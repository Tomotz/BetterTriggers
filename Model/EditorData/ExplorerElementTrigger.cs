﻿using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Model.EditorData
{
    public class ExplorerElementTrigger : IExplorerElement, IExplorerSaveable
    {
        public string path;
        public Trigger trigger;
        public bool isEnabled = true;
        public bool isInitiallyOn = true;
        public List<IExplorerElementUI> observers = new List<IExplorerElementUI>();
        private DateTime LastWrite;
        private long Size;

        private IExplorerElement Parent;

        public ExplorerElementTrigger(string path)
        {
            this.path = path;
            string json = string.Empty;
            bool isReadyForRead = false;
            while (!isReadyForRead)
            {
                try
                {
                    json = File.ReadAllText(path);
                    isReadyForRead = true;
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
            }
            trigger = JsonConvert.DeserializeObject<Trigger>(json);
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

        public void SetPath(string newPath)
        {
            this.path = newPath;
        }

        public void Attach(IExplorerElementUI observer)
        {
            this.observers.Add(observer);
        }

        public void Detach(IExplorerElementUI observer)
        {
            this.observers.Remove(observer);
        }

        public void Notify()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].Update(this);
            }
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
            return trigger.Id;
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

        public string GetSaveableString()
        {
            return JsonConvert.SerializeObject(trigger);
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
            var info = new FileInfo(path);
            this.Size = info.Length;
            this.LastWrite = info.LastWriteTime;
        }

        public IExplorerElement GetParent()
        {
            return Parent;
        }

        public void SetParent(IExplorerElement parent, int insertIndex)
        {
            Parent = parent;
            parent.GetExplorerElements().Insert(insertIndex, this);
        }

        public void RemoveFromParent()
        {
            Parent.GetExplorerElements().Remove(this);
            Parent = null;
        }

        public void Created(int insertIndex)
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].OnCreated(insertIndex);
            }
        }

        public void Deleted()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].Delete();
            }
        }

        public List<IExplorerElement> GetExplorerElements()
        {
            throw new Exception("'" + path + "' is not a folder.");
        }

        public void ChangedPosition()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].UpdatePosition();
            }
        }
    }
}