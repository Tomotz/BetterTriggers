﻿using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class ActionDefinition : IReferable
    {
        public ExplorerElement explorerElement;
        public int Id;
        public string Comment;
        //public string Category; // TODO: maybe add support for this later?
        public string ParamText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(explorerElement.GetName());
                sb.Append("(");
                for (int i = 0; i < Parameters.Elements.Count; i++)
                {
                    var parameter = (ParameterDefinition)Parameters.Elements[i];
                    sb.Append($",~{parameter.Name},");
                    if(i < Parameters.Elements.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append(")");
                return sb.ToString();
            }
        }
        public ParameterDefinitionCollection Parameters = new(TriggerElementType.ParameterDef);
        public TriggerElementCollection LocalVariables = new(TriggerElementType.LocalVariable);
        public TriggerElementCollection Actions = new(TriggerElementType.Action);

        public ActionDefinition(ExplorerElement explorerElement)
        {
            this.explorerElement = explorerElement;
        }

        public ActionDefinition Clone()
        {
            throw new NotImplementedException("Below constructor takes the old explorerElement. How do we get the new one in?");
            ActionDefinition cloned = new ActionDefinition(explorerElement);
            cloned.Comment = new string(Comment);
            cloned.LocalVariables = LocalVariables.Clone();
            cloned.Actions = Actions.Clone();

            return cloned;
        }
    }
}