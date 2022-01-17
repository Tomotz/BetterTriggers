﻿using GUI.Controllers;
using Model.War3Project;
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
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        public War3Project createdProject;
        
        public NewProjectWindow()
        {
            InitializeComponent();

            radBtnJass.IsChecked = true;
        }

        private void radBtnJass_Checked(object sender, RoutedEventArgs e)
        {
            string hint = string.Empty;
            if ((bool)radBtnJass.IsChecked)
            {
                hint += "vJass benefits:\n";
                hint += "- Type checking\n";
                hint += "- Compatibility with 2 decades of Jass resources\n";
            }

            lblLanguageHint.Text = hint;
        }

        private void radBtnLua_Checked(object sender, RoutedEventArgs e)
        {
            string hint = string.Empty;
            if ((bool)radBtnLua.IsChecked)
            {
                hint += "Lua benefits:\n";
                hint += "- General-purpose language\n";
                hint += "- More advanced features\n";
            }

            lblLanguageHint.Text = hint;
        }

        private void btnProjectDestination_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                lblProjectDestination.Content = dialog.SelectedPath;
            }
        }

        private void btnWar3Folder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                lblWar3MapFolder.Content = dialog.SelectedPath;
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string language = string.Empty;

            if ((bool)radBtnJass.IsChecked)
                language = "jass";
            else if ((bool)radBtnLua.IsChecked)
                language = "lua";

            ControllerProject controller = new ControllerProject();
            this.createdProject = controller.CreateProject(language, textBoxProjectName.Text, lblProjectDestination.Content.ToString());

            this.Close();
        }
    }
}