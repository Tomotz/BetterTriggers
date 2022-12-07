﻿using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace GUI
{
    public partial class VerifyingTriggersWindow : Window
    {
        List<ExplorerElementTrigger> modifiedTriggers = new List<ExplorerElementTrigger>();

        public VerifyingTriggersWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker workerVerify = new BackgroundWorker();
            workerVerify.DoWork += WorkerVerify_DoWork;
            workerVerify.WorkerReportsProgress = true;
            workerVerify.ProgressChanged += WorkerVerify_ProgressChanged;
            workerVerify.RunWorkerAsync();
        }

        private void WorkerVerify_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (modifiedTriggers.Count == 0)
            {
                this.Close();
                return;
            }

            ChangedTriggersWindow changedTriggersWindow = new ChangedTriggersWindow(modifiedTriggers);
            changedTriggersWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            changedTriggersWindow.Top = this.Top + this.Height / 2 - changedTriggersWindow.Height / 2;
            changedTriggersWindow.Left = this.Left + this.Width / 2 - changedTriggersWindow.Width / 2;
            changedTriggersWindow.Show();

            this.Close();
        }

        private void WorkerVerify_DoWork(object sender, DoWorkEventArgs e)
        {
            modifiedTriggers = ControllerMapData.ReloadMapData();
            (sender as BackgroundWorker).ReportProgress(100);
        }
    }
}
