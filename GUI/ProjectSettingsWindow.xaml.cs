﻿using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using War3Net.Build.Info;

namespace GUI
{
    /// <summary>
    /// Interaction logic for ProjectSettingsWindow.xaml
    /// </summary>
    public partial class ProjectSettingsWindow : Window
    {
        public ProjectSettingsWindow()
        {
            this.Owner = MainWindow.GetMainWindow();

            InitializeComponent();

            var project = ContainerProject.project;
            if (project.Language == "jass")
                comboboxMapScript.SelectedIndex = 0;
            else
                comboboxMapScript.SelectedIndex = 1;

            checkboxRelativeMapPath.IsChecked = project.UseRelativeMapDirectory;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var project = ContainerProject.project;
            if (comboboxMapScript.SelectedIndex == 0)
                project.Language = "jass";
            else
                project.Language = "lua";

            ControllerProject controllerProject = new ControllerProject();
            project.UseRelativeMapDirectory = (bool)checkboxRelativeMapPath.IsChecked;
            if(project.UseRelativeMapDirectory)
                project.War3MapDirectory = Path.GetFileName(project.War3MapDirectory);
            else
            {
                project.War3MapDirectory = controllerProject.GetFullMapPath();
            }

            ControllerExplorerElement controller = new ControllerExplorerElement();
            var root = controllerProject.GetProjectRoot();
            controller.AddToUnsaved(root);

            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
