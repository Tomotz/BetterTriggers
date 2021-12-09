﻿using GUI.Commands;
using GUI.Components.TriggerEditor;
using GUI.Components.Utility;
using Model.Natives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace GUI
{
    /// <summary>
    /// Interaction logic for TriggerControl.xaml
    /// </summary>
    public partial class TriggerControl : UserControl
    {
        NodeEvent categoryEvent;
        NodeCondition categoryCondition;
        NodeAction categoryAction;

        TextBlock currentParameterBlock;
        TextBlock currentDescriptionBlock;

        Point _startPoint;
        TreeViewItem dragItem;
        bool _IsDragging = false;

        TriggerElement copiedTriggerElement;

        public TriggerControl()
        {
            InitializeComponent();

            categoryEvent = new NodeEvent();
            categoryCondition = new NodeCondition();
            categoryAction = new NodeAction();

            treeViewTriggers.Items.Add(categoryEvent);
            treeViewTriggers.Items.Add(categoryCondition);
            treeViewTriggers.Items.Add(categoryAction);
        }

        public void CreateEvent()
        {
            var eventMenu = new EventMenuWindow();
            eventMenu.ShowDialog();
            Model.Natives.Function _event = eventMenu.selectedEvent;

            if (_event != null)
            {
                TriggerEvent item = new TriggerEvent(_event);
                categoryEvent.Items.Add(item);

                categoryEvent.IsExpanded = true;
            }

        }

        public void CreateCondition()
        {
            var conditionMenu = new ConditionMenuWindow();
            conditionMenu.ShowDialog();
            Model.Natives.Condition condition = conditionMenu.selectedContition;

            if (condition != null)
            {
                TriggerCondition item = new TriggerCondition(condition);
                categoryCondition.Items.Add(item);

                categoryCondition.IsExpanded = true;
            }
        }

        public void CreateAction()
        {
            var actionMenu = new ActionMenuWindow();
            actionMenu.ShowDialog();
            Model.Natives.Function action = actionMenu.selectedAction;

            if (action != null)
            {
                Components.TriggerEditor.TriggerAction item = new Components.TriggerEditor.TriggerAction(action);
                categoryAction.Items.Add(item);

                categoryAction.IsExpanded = true;
            }
        }

        private void treeViewTriggers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = treeViewTriggers.SelectedItem as TriggerElement;
            if (item != null)
            {
                var textBlockParameters = item.paramTextBlock;
                var textBlockDescription = item.descriptionTextBlock;

                if (currentParameterBlock != null && currentParameterBlock.Parent != null) { }
                grid.Children.Remove(currentParameterBlock); // remove current active parameter text block so the new one can be added.
                if (currentDescriptionBlock != null && currentDescriptionBlock.Parent != null) { }
                grid.Children.Remove(currentDescriptionBlock);


                // Display appropriate textblock
                grid.Children.Add(textBlockParameters);
                Grid.SetRow(textBlockParameters, 3);
                textBlockParameters.Margin = new Thickness(0, 0, 5, 0);

                grid.Children.Add(textBlockDescription);
                Grid.SetRow(textBlockDescription, 4);
                textBlockDescription.Margin = new Thickness(0, 0, 5, 0);

                currentParameterBlock = textBlockParameters;
                currentDescriptionBlock = textBlockDescription;
            }
        }

        private void treeViewTriggers_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void treeViewTriggers_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed && !_IsDragging)
            {
                Point position = e.GetPosition(null);
                if (Math.Abs(position.X - _startPoint.X) >
                        SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) >
                        SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag(e);
                }
            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            _IsDragging = true;
            dragItem = this.treeViewTriggers.SelectedItem as TreeViewItem;

            if (dragItem is NodeEvent || dragItem is NodeCondition || dragItem is NodeAction)
                return;

            DataObject data = null;

            data = new DataObject("inadt", dragItem);

            if (data != null)
            {
                DragDropEffects dde = DragDropEffects.Move;
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    dde = DragDropEffects.All;
                }
                DragDropEffects de = DragDrop.DoDragDrop(this.treeViewTriggers, data, dde);
            }
            _IsDragging = false;
        }

        private void treeViewTriggers_Drop(object sender, DragEventArgs e)
        {
            if (_IsDragging && dragItem != null)
            {
                var parent = (TreeViewItem)dragItem.Parent;

                // It is necessary to traverse the item's parents since drag & drop picks up
                // things like 'TextBlock' and 'Border' on the drop target when dropping the 
                // dragged element.
                TreeViewItem actionNode = GetDropTarget(e.Source as FrameworkElement);

                TreeViewItem whereItemWasDropped = GetTraversedTargetDropItem(e.Source as FrameworkElement);

                int indexInTree = 0;
                TreeViewItem parentToDropLocation;
                if (whereItemWasDropped is NodeAction)
                    indexInTree = 0;
                else
                {
                    parentToDropLocation = whereItemWasDropped.Parent as TreeViewItem;
                    indexInTree = parentToDropLocation.Items.IndexOf(whereItemWasDropped);
                }

                if (actionNode != dragItem)
                {
                    CommandTriggerElementMove command = new CommandTriggerElementMove(this.dragItem, parent, actionNode, indexInTree);
                    command.Execute();
                }
            }
        }

        private TreeViewItem GetTraversedTargetDropItem(FrameworkElement dropTarget)
        {
            TreeViewItem traversedTarget = null;
            while (traversedTarget == null)
            {
                dropTarget = dropTarget.Parent as FrameworkElement;
                if (dropTarget is TreeViewItem)
                {
                    traversedTarget = (TreeViewItem)dropTarget;
                }
            }

            return traversedTarget;
        }

        private TreeViewItem GetDropTarget(FrameworkElement dropTarget)
        {
            if (dropTarget is NodeEvent || dropTarget is NodeCondition || dropTarget is NodeAction)
                return (TreeViewItem) dropTarget;
                
            TreeViewItem traversedTarget = GetTraversedTargetDropItem(dropTarget);
            if (traversedTarget is NodeEvent || traversedTarget is NodeCondition || traversedTarget is NodeAction)
                return traversedTarget;
            else
                traversedTarget = GetTraversedTargetDropItem(traversedTarget); // traverse one more time to get the action node

            return traversedTarget;
        }

        private void treeViewTriggers_PreviewDragEnter(object sender, DragEventArgs e)
        {

        }

        private void treeViewTriggers_PreviewDrop(object sender, DragEventArgs e)
        {
            // Use this event to display feedback to the user when dragging?
        }

        private void treeViewTriggers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteTriggerElement();
            else if (e.Key == Key.C && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                CopyTriggerElement();
            else if (e.Key == Key.V && Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                PasteTriggerElement();
        }

        private void DeleteTriggerElement()
        {
            var selectedItem = (TreeViewItem)treeViewTriggers.SelectedItem;
            if (selectedItem == null || selectedItem is NodeEvent || selectedItem is NodeCondition || selectedItem is NodeAction)
                return;

            CommandTriggerElementDelete command = new CommandTriggerElementDelete(selectedItem, (TreeViewItem)selectedItem.Parent);
            command.Execute();
        }

        private void CopyTriggerElement()
        {
            var selectedItem = (TreeViewItem)treeViewTriggers.SelectedItem;
            if (selectedItem == null || selectedItem is NodeEvent || selectedItem is NodeCondition || selectedItem is NodeAction)
                return;

            var selectedItemCast = (TriggerElement)selectedItem;
            this.copiedTriggerElement = selectedItemCast;
        }

        private void PasteTriggerElement()
        {
            var selectedItem = (TreeViewItem)treeViewTriggers.SelectedItem;
            if (selectedItem == null || this.copiedTriggerElement == null)
                return;

            // Copy the actual values from the model-layer
            Function function = null;
            if (copiedTriggerElement is Components.TriggerEditor.TriggerEvent)
            {
                var element = (Components.TriggerEditor.TriggerEvent) this.copiedTriggerElement;
                function = (Function) element._event.Clone();
            }
            else if (copiedTriggerElement is Components.TriggerEditor.TriggerCondition)
            {
                var element = (Components.TriggerEditor.TriggerCondition)this.copiedTriggerElement;
                throw new NotImplementedException();
            }
            else if(copiedTriggerElement is Components.TriggerEditor.TriggerAction)
            {
                var element = (Components.TriggerEditor.TriggerAction)this.copiedTriggerElement;
                function = (Function)element.action.Clone();
            }

            // Determine where to place the pasted element
            TreeViewItem targetParentNode;
            int insertIndex = 0;
            if (selectedItem is NodeEvent || selectedItem is NodeCondition || selectedItem is NodeAction)
                targetParentNode = selectedItem;
            else
            {
                targetParentNode = selectedItem.Parent as TreeViewItem;
                insertIndex = targetParentNode.Items.IndexOf(selectedItem);
            }

            // Determine if the copied item is appropriate for the target node
            if (this.copiedTriggerElement is Components.TriggerEditor.TriggerEvent && !(targetParentNode is NodeEvent))
                return;
            if (this.copiedTriggerElement is Components.TriggerEditor.TriggerCondition && !(targetParentNode is NodeCondition))
                return;
            if (this.copiedTriggerElement is Components.TriggerEditor.TriggerAction && !(targetParentNode is NodeAction))
                return;

            CommandTriggerElementPaste command = new CommandTriggerElementPaste(function, targetParentNode, insertIndex);
            command.Execute();
        }
    }
}
