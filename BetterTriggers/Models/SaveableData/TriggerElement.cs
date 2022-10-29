﻿using BetterTriggers.JsonBaseConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    [JsonConverter(typeof(BaseConverterTriggerElement))]
    public class TriggerElement : ITriggerElement
    {
        public bool isEnabled = true;
        public Function function = new Function();

        
        [JsonIgnore]
        private List<ITriggerElement> Parent;
        [JsonIgnore]
        private List<ITriggerElementUI> triggerElementUIs = new List<ITriggerElementUI>();

        public TriggerElement() { }
        public TriggerElement(string value)
        {
            this.function.value = value;
        }

        public virtual TriggerElement Clone()
        {
            TriggerElement clone = new TriggerElement();
            clone.isEnabled = isEnabled;
            clone.function = function.Clone();

            return clone;
        }

        public void SetParent(List<ITriggerElement> Parent, int insertIndex)
        {
            Parent.Insert(insertIndex, this);
            this.Parent = Parent;
        }

        public void RemoveFromParent()
        {
            this.Parent.Remove(this);
            this.Parent = null;
        }

        public void Attach(ITriggerElementUI elementUI)
        {
            triggerElementUIs.Add(elementUI);
        }

        public void Detach(ITriggerElementUI elementUI)
        {
            triggerElementUIs.Remove(elementUI);
        }

        public void ChangedParams()
        {
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].UpdateParams();
            }
        }

        public void ChangedPosition()
        {
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].UpdatePosition();
            }
        }

        public void ChangedEnabled()
        {
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].UpdateEnabled();
            }
        }

        public void Created(int insertIndex)
        {
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].OnCreated(insertIndex);
            }
        }

        public void Deleted()
        {
            for (int i = 0; i < triggerElementUIs.Count; i++)
            {
                triggerElementUIs[i].OnDeleted();
            }
        }

        public List<ITriggerElement> GetParent()
        {
            return this.Parent;
        }

        public void SetParent(List<ITriggerElement> parent)
        {
            this.Parent = parent;
        }
    }
}
