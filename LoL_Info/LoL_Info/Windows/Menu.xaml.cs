using System;
using System.Windows;
using System.Windows.Controls;
using LoL_Info.Classes;
using StaticElementsLibrary;
using System.Threading;

namespace LoL_Info.Windows
{
    public partial class Menu : UserControl
    {
        private MainWindow window;

        public static string mode = Properties.Resources.Team;

        public Menu(MainWindow window)
        {
            InitializeComponent();
            this.window = window;
            btnCompare.Content = mode;
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            window.InfoFrameLoad(0);
            window.ChangeStatus(Properties.Resources.SbStats);
        }

        private void btnChart_Click(object sender, RoutedEventArgs e)
        {
            window.InfoFrameLoad(2);
            window.ChangeStatus(Properties.Resources.SbCharts);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (LoadFromAPI.HasNewMatches(CurrentUserElements.Player.Name, CurrentUserElements.Player.AccountID))
            {
                if (new MessageBoxControl($"{Properties.Resources.TryLoadNewDataMessage}", false).ShowDialog().Value)
                {
                    if (window.progressBar.Value == 0)
                    {
                        window.ChangeStatus(Properties.Resources.SbDownload);
                        ThreadManager.WriteMessage(Properties.Resources.DownloadingMessage);
                        new ProgressBarUpdater(CurrentUserElements.Player.Name, window.MainProgressBar, (object obj) => new LoadFromAPI(obj as string).UpdateInfo());
                    }
                    else
                        ThreadManager.WriteMessage(Properties.Resources.ProcessDontOverMessage);
                }
            }
            else
            {
                ThreadManager.WriteMessage("You don`t have new matches!");
            }
        }

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            if (mode.Contains(Properties.Resources.Team))
            {
                if (new TeamMode().ShowDialog().Value)
                {
                    mode = Properties.Resources.Player;
                    window.ChangeStatus(Properties.Resources.SbPlayerToTeam);
                }
                btnCompare.Content = mode;
            }
            else
            {
                new LoadFromAPI(CurrentUserElements.Player.Name).LoadInfo(window, true);
                mode = Properties.Resources.Team;
                btnCompare.Content = mode;
                window.ChangeStatus(Properties.Resources.SbTeamToPlayer);
            }
        }
    }
}