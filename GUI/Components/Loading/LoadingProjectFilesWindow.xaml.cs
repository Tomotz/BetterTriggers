﻿using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using System;
using System.ComponentModel;
using System.Windows;

namespace GUI.Components.Loading
{
    public partial class LoadingProjectFilesWindow : Window
    {
        public War3Project project;
        private string projectPath;
        private BackgroundWorker worker;

        public LoadingProjectFilesWindow(string projectPath)
        {
            InitializeComponent();
            this.projectPath = projectPath;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            worker = new BackgroundWorker();
            worker.DoWork += WorkerVerify_DoWork;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += WorkerVerify_ProgressChanged;
            worker.RunWorkerAsync();
        }

        private void WorkerVerify_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblInfo.Content = $"Loading Project Files {filesLoaded}/{totalFiles}";
            if (e.ProgressPercentage == 100 && project != null)
                this.Close();

            if(e.ProgressPercentage == -1)
            {
                Dialogs.MessageBox message = new Dialogs.MessageBox("Error", errorMsg);
                message.ShowDialog();
                this.Close();
            }
        }

        int filesLoaded = 0;
        int totalFiles = 0;
        string errorMsg = string.Empty;
        private void WorkerVerify_DoWork(object sender, DoWorkEventArgs e)
        {
            Project.FileLoadEvent += ControllerProject_FileLoadEvent;
            try
            {
                project = Project.Load(projectPath).war3project;
            }
            catch (Exception ex)
            {
                Project.FileLoadEvent -= ControllerProject_FileLoadEvent;
                errorMsg = ex.Message;
                worker.ReportProgress(-1);
                return;
            }
            Project.FileLoadEvent -= ControllerProject_FileLoadEvent;
            worker.ReportProgress(100);
        }

        private void ControllerProject_FileLoadEvent(int arg1, int arg2)
        {
            filesLoaded = arg1;
            totalFiles = arg2;
            float percent = (float)filesLoaded / (float)totalFiles * 100f;
            worker.ReportProgress((int)percent);
        }
    }
}