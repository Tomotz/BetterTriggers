﻿using Model.Data;
using Model.Natives;
using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;

namespace GUI.Components.TriggerEditor
{
    public abstract class TriggerElement : TreeViewItem
    {
        public TextBlock paramTextBlock;
        public TextBlock descriptionTextBlock;
        protected List<Parameter> parameters;
        protected string paramText;
        protected EnumCategory category;

        public TriggerElement()
        {
            this.paramTextBlock = new TextBlock();
            this.paramTextBlock.FontSize = 18;
            this.paramTextBlock.TextWrapping = TextWrapping.Wrap;
            this.paramTextBlock.Margin = new Thickness(0, 0, 5, 0);
            this.paramTextBlock.Foreground = Brushes.White;

            this.descriptionTextBlock = new TextBlock();
            this.descriptionTextBlock.FontSize = 12;
            this.descriptionTextBlock.TextWrapping = TextWrapping.Wrap;
            this.descriptionTextBlock.Margin = new Thickness(0, 0, 5, 5);
            this.descriptionTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            this.descriptionTextBlock.Background = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        }

        protected void FormatParameterText(TextBlock textBlock, List<Parameter> parameters)
        {
            textBlock.Inlines.Clear();

            RecurseParameters(textBlock, parameters, paramText);

            TreeViewManipulator.SetTreeViewItemAppearance(this, textBlock.Text, category);
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
                }
                else
                {
                    if (parameters[paramIndex] is Constant)
                    {
                        textBlock.Inlines.Remove(textBlock.Inlines.LastInline); // removes the comma before the '~' indicator

                        var index = paramIndex; // copy current iterated index to prevent referenced values in hyperlink.click delegate
                        var hyperlink = CreateHyperlink(textBlock, parameters[paramIndex].name, parameters, index);
                        hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 255));
                        paramIndex++;

                        while (i < paramText.Length && paramText[i] != ',') // erases placeholder param name
                        {
                            i++;
                        }
                    }
                    else if (parameters[paramIndex] is Function) // recurse if parameter is a function
                    {
                        var index = paramIndex;
                        var hyperlink = CreateHyperlink(textBlock, "(", parameters, index);
                        hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 255));

                        var function = (Function)parameters[paramIndex];
                        paramIndex++;

                        RecurseParameters(textBlock, function.parameters, function.paramText); // recurse
                        textBlock.Inlines.Add(")");
                    }
                    else if (parameters[paramIndex] is Parameter) // In other words, parameter has not yet been set. Redundant?
                    {
                        textBlock.Inlines.Remove(textBlock.Inlines.LastInline); // removes the comma before the '~' indicator
                        i++; // avoids the '~' in the name

                        string paramName = string.Empty;
                        int startIndex = i; // store current letter index
                        int length = 0;
                        bool isParamNameSet = false;


                        while(!isParamNameSet && i < paramText.Length) // scan parameter display name
                        {
                            if (paramText[i] == ',')
                                isParamNameSet = true;
                            else
                            {
                                length++;
                                i++;
                            }
                        }

                        paramName = paramText.Substring(startIndex, length);

                        var index = paramIndex;
                        var hyperlink = CreateHyperlink(textBlock, paramName, parameters, index);
                        hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(255, 75, 75));
                        paramIndex++;
                    }
                }
            }
        }

        private Hyperlink CreateHyperlink(TextBlock textBlock, string hyperlinkText, List<Parameter> parameters, int paramIndex)
        {
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


            hyperlink.Click += delegate { Hyperlink_Click(hyperlink, paramIndex); };
            hyperlink.GotFocus += delegate { hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0)); };
            hyperlink.LostFocus += delegate
            {
                if(parameters[paramIndex] is Constant || parameters[paramIndex] is Function)
                    hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 255));
                else
                    hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(255, 75, 75));
            };

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

            if (window.isOK)
            {
                parameters[paramIndex] = window.selectedParameter; // set parameter on window close.
                FormatParameterText(this.paramTextBlock, this.parameters);
            }
        }
    }
}
