﻿using GUI.Components;
using GUI.Components.TriggerEditor;
using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerTriggerControl
    {
        public void OnTriggerElementCreate(TreeViewTriggerElement item, INode parent, int insertIndex)
        {
            if (item.Parent != null) // needed because of another hack. Basically, the item is already attached, so we need to detach it.
            {
                if (item.Parent is TreeView)
                {
                    var unwantedParent = (TreeView)item.Parent;
                    unwantedParent.Items.Remove(item);
                }
                else if (item.Parent is TreeViewItem)
                {
                    var unwantedParent = (TreeViewItem)item.Parent;
                    unwantedParent.Items.Remove(item);
                }
            }

            var parentTreeItem = (TreeViewItem)parent;
            parentTreeItem.Items.Insert(insertIndex, item);
        }

        /// <summary>
        /// Moves a 'TreeViewTriggerElement' to its correct location based on the 'TriggerElement'.
        /// </summary>
        /// <param name="treeViewTriggerElement"></param>
        /// <param name="insertIndex"></param>
        internal void OnTriggerElementMove(TreeViewTriggerElement treeViewTriggerElement, int insertIndex)
        {
            var parent = (INode)treeViewTriggerElement.Parent;
            var treeView = treeViewTriggerElement.GetTriggerControl().treeViewTriggers;
            parent.Remove(treeViewTriggerElement);

            INode newParent = null;
            for (int i = 0; i < treeView.Items.Count; i++)
            {
                newParent = FindParent(treeView.Items[i] as TreeViewItem, treeViewTriggerElement);
                if (newParent != null)
                    break;
            }
            if (newParent == null)
                throw new Exception("Target 'Parent' was not found.");

            newParent.Insert(treeViewTriggerElement, insertIndex);
        }

        /// <summary>
        /// Finds the parent to attach a TreeViewTriggerElement to.
        /// This assumes the item has 'Parent', otherwise expect a crash.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="treeViewTriggerElement"></param>
        /// <returns></returns>
        internal INode FindParent(TreeViewItem parent, TreeViewTriggerElement treeViewTriggerElement)
        {
            INode node = null;

            // Ugly code
            // we need to check the parent right away, because the loop below
            // will never check it.
            if (parent is INode)
            {
                var tmpNode = (INode)parent;
                if (tmpNode.GetTriggerElements() == treeViewTriggerElement.triggerElement.Parent)
                    return tmpNode;
            }

            for (int i = 0; i < parent.Items.Count; i++)
            {
                var treeItem = (TreeViewItem)parent.Items[i];
                if (treeItem is INode)
                {
                    var tmpNode = (INode)treeItem;
                    if (tmpNode.GetTriggerElements() == treeViewTriggerElement.triggerElement.Parent)
                    {
                        return tmpNode;
                    }
                }

                if (treeItem.Items.Count > 0)
                    node = FindParent(treeItem, treeViewTriggerElement);
            }

            return node;
        }

        internal void CreateSpecialTriggerElement(TreeViewTriggerElement treeViewTriggerElement)
        {
            if (treeViewTriggerElement.triggerElement.function is IfThenElse)
            {
                var function = (IfThenElse)treeViewTriggerElement.triggerElement.function;

                var If = new NodeCondition("If - Conditions");
                var Then = new NodeAction("Then - Actions");
                var Else = new NodeAction("Else - Actions");
                If.SetTriggerElements(function.If);
                Then.SetTriggerElements(function.Then);
                Else.SetTriggerElements(function.Else);
                treeViewTriggerElement.Items.Add(If);
                treeViewTriggerElement.Items.Add(Then);
                treeViewTriggerElement.Items.Add(Else);

                RecurseLoadTrigger(If.GetTriggerElements(), If);
                RecurseLoadTrigger(Then.GetTriggerElements(), Then);
                RecurseLoadTrigger(Else.GetTriggerElements(), Else);
            }
            else if (treeViewTriggerElement.triggerElement.function is AndMultiple)
            {
                var function = (AndMultiple)treeViewTriggerElement.triggerElement.function;
                var And = new NodeCondition("Conditions");
                And.SetTriggerElements(function.And);
                treeViewTriggerElement.Items.Add(And);

                RecurseLoadTrigger(And.GetTriggerElements(), And);
            }
            else if (treeViewTriggerElement.triggerElement.function is OrMultiple)
            {
                var function = (OrMultiple)treeViewTriggerElement.triggerElement.function;
                var Or = new NodeCondition("Conditions");
                Or.SetTriggerElements(function.Or);
                treeViewTriggerElement.Items.Add(Or);

                RecurseLoadTrigger(Or.GetTriggerElements(), Or);
            }
            else if (treeViewTriggerElement.triggerElement.function is ForGroupMultiple)
            {
                var function = (ForGroupMultiple)treeViewTriggerElement.triggerElement.function;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement.function is ForForceMultiple)
            {
                var function = (ForForceMultiple)treeViewTriggerElement.triggerElement.function;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement.function is ForLoopAMultiple)
            {
                var function = (ForLoopAMultiple)treeViewTriggerElement.triggerElement.function;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement.function is ForLoopBMultiple)
            {
                var function = (ForLoopBMultiple)treeViewTriggerElement.triggerElement.function;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
            else if (treeViewTriggerElement.triggerElement.function is ForLoopVarMultiple)
            {
                var function = (ForLoopVarMultiple)treeViewTriggerElement.triggerElement.function;
                var Actions = new NodeAction("Loop - Actions");
                Actions.SetTriggerElements(function.Actions);
                treeViewTriggerElement.Items.Add(Actions);

                RecurseLoadTrigger(Actions.GetTriggerElements(), Actions);
            }
        }

        public void RecurseLoadTrigger(List<TriggerElement> triggerElements, INode parentNode)
        {
            parentNode.SetTriggerElements(triggerElements);
            for (int i = 0; i < triggerElements.Count; i++)
            {
                var triggerElement = triggerElements[i];
                triggerElement.Parent = triggerElements;
                TreeViewTriggerElement treeItem = new TreeViewTriggerElement(triggerElement);
                triggerElement.Attach(treeItem);
                parentNode.Add(treeItem);
            }
        }
    }
}
