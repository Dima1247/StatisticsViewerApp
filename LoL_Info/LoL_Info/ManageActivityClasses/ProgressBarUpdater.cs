using LoL_Info.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using log4net;

namespace LoL_Info
{
    class ProgressBarUpdater
    {
        private ParameterizedThreadStart actions;

        private static readonly ILog log = LogManager.GetLogger(typeof(ProgressBarUpdater));
        private ProgressBar progressBar;

        private string player;

        public ProgressBarUpdater(string player, ProgressBar progressBar, ParameterizedThreadStart actions)
        {
            var worker = new BackgroundWorker();
            
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            
            this.actions = actions;
            this.player = player;
            this.progressBar = progressBar;

            worker.RunWorkerAsync();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            LoadFromAPI.ResetFullLoad();

            worker.ReportProgress(1);

            ThreadManager.ParamThread(actions, player, true);

            for (int i = 1; LoadFromAPI.FullLoad == 0; i++)
            {
                Thread.Sleep(1000);
                log.Debug($"Wait for 1 second ({i})");
            }

            int max = LoadFromAPI.FullLoad;

            if (max > 0)
            {
                while (ThreadManager.IsActive)
                {
                    if ((LoadFromAPI.IsLoaded * 99) / max > 1)
                        worker.ReportProgress((LoadFromAPI.IsLoaded * 99) / max);

                    log.Debug($"Current percent = {(LoadFromAPI.IsLoaded * 99) / max}");
                    Thread.Sleep(1000);
                }

                Thread.Sleep(1500);
                worker.ReportProgress(99);
                Thread.Sleep(1500);

                worker.ReportProgress(100);
            }
            else
            {
                worker.ReportProgress(100);
            }

        }
    }
}
