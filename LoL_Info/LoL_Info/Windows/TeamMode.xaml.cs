using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LoL_Info.Classes;
using DataAccessLayer;
using StaticElementsLibrary;

namespace LoL_Info.Windows
{
    public partial class TeamMode : Window
    {
        private List<CheckBox> playersCheckList = new List<CheckBox>();
        
        public TeamMode()
        {
            InitializeComponent();

            titleBar.Children.Add(new TitleBar(this, Properties.Resources.TeamModeTitle, false));

            Left = MainWindow.MainWindowPosition.X + 15;
            Top = MainWindow.MainWindowPosition.Y + 99;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var names = PlayerDB.PlayersLoad();

            int j = 0;

            for (int i = 0; i < names.Count; i++)
            {
                if (names[i] != CurrentUserElements.Player.Name)
                {
                    playersCheckList.Add(new CheckBox());
                    playersCheckList[j].Content = names[i];
                    playersCheckList[j].Foreground = playersList.Foreground;
                    playersCheckList[j].Click += CompareFrame_Click;

                    if (!playersCheckList.Contains(playersCheckList[j].Content))
                        playersList.Items.Add(playersCheckList[j]);
                    j++;
                }
            }
        }

        private void CompareFrame_Click(object sender, RoutedEventArgs e)
        {
            int checkedElements = 0;

            for (int i = 0; i < playersCheckList.Count; i++)
            {
                if (playersCheckList[i].IsChecked == true)
                    checkedElements++;
            }

            if (checkedElements == 4)
            {
                for (int i = 0; i < playersCheckList.Count; i++)
                {
                    if (!playersCheckList[i].IsChecked == true)
                        playersCheckList[i].IsEnabled = false;
                }
            }
            else
            {
                for (int i = 0; i < playersCheckList.Count; i++)
                {
                    playersCheckList[i].IsEnabled = true;
                }
            }
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var compareManager = new CompareManager(playersCheckList);
                var matchList = new MatchList() { Matches = compareManager.GetCheckedMatches() };

                new CurrentUserElements(CurrentUserElements.Champions, CurrentUserElements.Player, matchList);
                MatchListDetails.MatchList = compareManager.GetCheckedMatchesDetails();

                DialogResult = true;
            }
            catch
            {
                ThreadManager.WriteMessage(Properties.Resources.SelectedPlayersFailedMessage);
            }
        }
    }
}
