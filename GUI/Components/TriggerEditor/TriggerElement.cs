﻿using Model.Data;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;
using GUI.Commands;
using Model.Enums;
using Model.Templates;
using Model.SavableTriggerData;
using GUI.Containers;

namespace GUI.Components.TriggerEditor
{
    public class TriggerElement : TreeViewItem
    {
        internal Function function;
        public TextBlock paramTextBlock;
        public TextBlock descriptionTextBlock;
        protected string paramText;
        protected Category category;
        private string formattedParamText = string.Empty;


        public TriggerElement(Function function)
        {
            this.function = function;

            this.paramTextBlock = new TextBlock();
            this.paramTextBlock.Margin = new Thickness(5, 0, 5, 0);
            this.paramTextBlock.FontSize = 18;
            this.paramTextBlock.TextWrapping = TextWrapping.Wrap;
            this.paramTextBlock.Foreground = Brushes.White;
            Grid.SetRow(this.paramTextBlock, 3);

            this.descriptionTextBlock = new TextBlock();
            this.descriptionTextBlock.FontSize = 12;
            this.descriptionTextBlock.TextWrapping = TextWrapping.Wrap;
            this.descriptionTextBlock.Margin = new Thickness(5, 0, 5, 5);
            this.descriptionTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            this.descriptionTextBlock.Background = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            Grid.SetRow(this.descriptionTextBlock, 4);

            this.paramText = ContainerTriggerData.GetParamText(function);
            this.descriptionTextBlock.Text = ContainerTriggerData.GetDescription(function);
            this.category = ContainerTriggerData.GetCategory(function);

            TreeViewManipulator.SetTreeViewItemAppearance(this, "placeholder", this.category);

            this.FormatParameterText(paramTextBlock, this.function.parameters);
        }

        internal void FormatParameterText(TextBlock textBlock, List<Parameter> parameters)
        {
            textBlock.Inlines.Clear();
            this.formattedParamText = string.Empty;

            RecurseParameters(textBlock, parameters, paramText);
            bool areParametersValid = IsParameterListValid(parameters);

            if (areParametersValid)
                TreeViewManipulator.SetTreeViewItemAppearance(this, this.formattedParamText, category);
            else
                TreeViewManipulator.SetTreeViewItemAppearance(this, this.formattedParamText, category, false);
        }

        private bool IsParameterListValid(List<Parameter> parameters)
        {
            bool isValid = true;

            for (int i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i];
                if (param is Function)
                {
                    var func = (Function)param;
                    if (func.parameters.Count > 0)
                    {
                        isValid = IsParameterListValid(func.parameters);
                    }
                }

                if (param.identifier == null)
                    isValid = false;
            }

            return isValid;
        }

        private void RecurseParameters(TextBlock textBlock, List<Parameter> parameters, string paramText)
        {
            int paramIndex = 0;

            for (int i = 0; i < paramText.Length; i++)
            {
                if (paramText[i] != '~')
                {
                    Run run = new Run(paramText[i].ToString());
                    run.FontFamily = new FontFamily("Verdana");

                    textBlock.Inlines.Add(run);
                    formattedParamText += paramText[i];
                }
                else
                {
                    if (parameters[paramIndex] is Constant)
                    {
                        RemoveCommaBeforeParamIndicator(textBlock);

                        var index = paramIndex; // copy current iterated index to prevent referenced values in hyperlink.click delegate
                        CreateHyperlink(textBlock, ContainerTriggerData.GetParamDisplayName(parameters[paramIndex]), parameters, index);
                        paramIndex++;

                        while (i < paramText.Length && paramText[i] != ',') // erases placeholder param name
                        {
                            i++;
                        }
                    }
                    else if (parameters[paramIndex] is Function) // recurse if parameter is a function
                    {
                        RemoveCommaBeforeParamIndicator(textBlock);

                        var function = (Function)parameters[paramIndex];

                        var index = paramIndex;
                        if (function.parameters.Count > 0) // first bracket gets hyperlinked
                        {
                            CreateHyperlink(textBlock, "(", parameters, index);
                            RecurseParameters(textBlock, function.parameters, ContainerTriggerData.GetParamText(function)); // recurse
                        }
                        else // whole displayname gets hyperlinked
                        {
                            formattedParamText += "(";
                            Run runFirstBracket = new Run("(");
                            runFirstBracket.FontFamily = new FontFamily("Verdana");
                            textBlock.Inlines.Add(runFirstBracket);

                            CreateHyperlink(textBlock, ContainerTriggerData.GetParamDisplayName(function), parameters, index);
                        }
                        paramIndex++;

                        Run run = new Run(")");
                        run.FontFamily = new FontFamily("Verdana");
                        textBlock.Inlines.Add(run);

                        formattedParamText += ")";

                        while (i < paramText.Length && paramText[i] != ',') // erases placeholder param name
                        {
                            i++;
                        }
                    }
                    else if (parameters[paramIndex] is Model.SavableTriggerData.Variable)
                    {
                        RemoveCommaBeforeParamIndicator(textBlock);

                        var index = paramIndex;

                        i++;

                        var variable = (Model.SavableTriggerData.Variable)parameters[paramIndex];
                        var varName = ContainerVariables.GetVariableNameById(variable.VariableId);

                        // This exists in case a variable file has been deleted
                        if (varName == null || varName == "")
                        {
                            parameters[paramIndex] = new Parameter()
                            {
                                returnType = variable.returnType,
                            };
                            varName = "null";
                        }

                        CreateHyperlink(textBlock, varName, parameters, index);

                        paramIndex++;

                        while (i < paramText.Length && paramText[i] != ',') // erases placeholder param name
                        {
                            i++;
                        }
                    }
                    else if (parameters[paramIndex] is Parameter) // In other words, parameter has not yet been set. Redundant?
                    {
                        RemoveCommaBeforeParamIndicator(textBlock);

                        i++; // avoids the '~' in the name

                        int startIndex = i; // store current letter index
                        int length = 0;
                        bool isParamNameSet = false;

                        while (!isParamNameSet && i < paramText.Length) // scan parameter display name
                        {
                            if (paramText[i] == ',')
                                isParamNameSet = true;
                            else
                            {
                                length++;
                                i++;
                            }
                        }

                        string paramName = paramText.Substring(startIndex, length);

                        var index = paramIndex;
                        CreateHyperlink(textBlock, paramName, parameters, index);

                        paramIndex++;
                    }
                }
            }
        }

        private void RemoveCommaBeforeParamIndicator(TextBlock textBlock)
        {
            textBlock.Inlines.Remove(textBlock.Inlines.LastInline); // removes the comma before the '~' indicator
            if (formattedParamText.Length != 0)
                this.formattedParamText = this.formattedParamText.Remove(formattedParamText.Length - 1, 1);
        }

        private void RecolorHyperlink(Parameter parameter, Hyperlink hyperlink)
        {
            if (parameter is Constant || parameter is Function || parameter is Model.SavableTriggerData.Variable)
                hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 255));
            else
                hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(255, 75, 75));
        }

        private Hyperlink CreateHyperlink(TextBlock textBlock, string hyperlinkText, List<Parameter> parameters, int paramIndex)
        {
            this.formattedParamText += hyperlinkText;
            Run run = new Run(hyperlinkText); // idk why it's called run
            Hyperlink hyperlink = new Hyperlink(run);
            hyperlink.Tag = parameters;


            // Create an underline text decoration.
            TextDecoration underline = new TextDecoration();
            underline.PenOffset = 2; // Underline offset
            underline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            TextDecorationCollection decorations = new TextDecorationCollection();
            decorations.Add(underline);
            hyperlink.TextDecorations = decorations;

            // Hyperlink font
            hyperlink.FontFamily = new FontFamily("Verdana");

            // Hyperlink color
            RecolorHyperlink(parameters[paramIndex], hyperlink);

            hyperlink.Click += delegate { Hyperlink_Click(hyperlink, paramIndex); };
            hyperlink.GotFocus += delegate { hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0)); };
            hyperlink.LostFocus += delegate { RecolorHyperlink(parameters[paramIndex], hyperlink); };

            textBlock.Inlines.Add(hyperlink); // adds the clickable parameter text

            return hyperlink;
        }

        // We need to pass in the list of parameters to we can change the selected parameter on the list.
        private void Hyperlink_Click(Hyperlink clickedHyperlink, int paramIndex)
        {
            var parameters = (List<Parameter>)clickedHyperlink.Tag;
            var window = new ParameterWindow(parameters[paramIndex].returnType);
            window.Title = parameters[paramIndex].returnType;
            window.ShowDialog();

            if (window.isOK) // set parameter on window close.
            {
                CommandTriggerElementParamModify command = new CommandTriggerElementParamModify(this, parameters, paramIndex, window.selectedParameter);
                command.Execute();

            }
        }
    }
}
