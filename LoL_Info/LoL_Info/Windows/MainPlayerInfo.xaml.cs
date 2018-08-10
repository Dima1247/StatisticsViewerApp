using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LoL_Info.Classes;
using StaticElementsLibrary;

namespace LoL_Info.Windows
{
    public partial class MainPlayerInfo : UserControl
    {
        private MainCounter counter = new MainCounter();
        private MatchList matches = new MatchList();
        private List<MatchDetails> detailedMatches;

        public MainPlayerInfo()
        {
            InitializeComponent();
            matches.Matches = CurrentUserElements.MatchList.Matches;
            detailedMatches = MatchListDetails.MatchList;
            fieldCurrentKDA.Text = Properties.Settings.Default.CurrentKDA.ToString();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayInfo(0);
        }

        private void btnAcceptCurrentKDA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.CurrentKDA = double.Parse(fieldCurrentKDA.Text);
                Properties.Settings.Default.Save();

                counter.CountInfluenceForCurrentKDA(Properties.Settings.Default.CurrentKDA);

                if (StatVariables.ResultInfluencePercent != -1)
                    fieldInfluence.Content = $"{Properties.Resources.Influence}{StatVariables.ResultInfluencePercent}%";
                else
                    fieldInfluence.Content = $"{Properties.Resources.Influence}{Properties.Resources.RequiredMatches}";

                influencePanel.Visibility = Visibility.Hidden;
                influenceTip.Content = $"{Properties.Resources.InfluenceTip}{Properties.Settings.Default.CurrentKDA})\n\n{Properties.Resources.InfluenceRightButtonClickTip}";
            }
            catch
            {
                ThreadManager.WriteMessage("There is a problem with entered data!");
            }
        }

        #region ContextButtons
        private void partAll_Click(object sender, RoutedEventArgs e)
        {
            MatchListDetails.MatchList = detailedMatches;
            DisplayInfo(0);
        }

        private void partRanked_Click(object sender, RoutedEventArgs e)
        {
            MatchListDetails.MatchList = detailedMatches;
            MatchListDetails.MatchList = counter.GetDetailsByMatchType(1, CurrentUserElements.MatchList);
            DisplayInfo(1);
        }

        private void partSimple_Click(object sender, RoutedEventArgs e)
        {
            MatchListDetails.MatchList = detailedMatches;
            MatchListDetails.MatchList = counter.GetDetailsByMatchType(2, CurrentUserElements.MatchList);
            DisplayInfo(2);
        }
        #endregion
        
        private void fieldInfluence_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Right)
            {
                if (influencePanel.Visibility == Visibility.Hidden)
                    influencePanel.Visibility = Visibility.Visible;
                else
                    influencePanel.Visibility = Visibility.Hidden;
            }
        }

        private void DisplayInfo(int mode)
        {
            counter.LoadData(matches, mode);

            if (table.Items.Count != 0)
            {
                for (int i = table.Items.Count - 1; i >= 0; i--)
                {
                    table.Items.RemoveAt(i);
                }
            }

            fieldWins.Content = $"{Properties.Resources.WinRate}{StatVariables.WinsPercent}%";
            winsTip.Content = $"{Properties.Resources.Wins}: {StatVariables.WinsCount}, {Properties.Resources.Matches}: {StatVariables.MatchesCount}";

            fieldKDA.Content = $"{Properties.Resources.AverageKDA}{StatVariables.AverageKills}/{StatVariables.AverageDeaths}/{StatVariables.AverageAssists}";
            kdaTip.Content = $"{ StatVariables.RateKDA} { Properties.Resources.KDA}";

            fieldFarm.Content = $"{Properties.Resources.AverageFarm}{StatVariables.AvarageFarm} {Properties.Resources.FarmPerMatch}";
            farmTip.Content = $"{StatVariables.MinionsPerMinute} {Properties.Resources.FarmPerMinute}";

            switch (StatVariables.TopDayID)
            {
                case 0: fieldTopDay.Content = $"{Properties.Resources.TopDay}{Properties.Resources.Sunday} ({StatVariables.TopDayList[(DayOfWeek)StatVariables.TopDayID]}%)"; break;
                case 1: fieldTopDay.Content = $"{Properties.Resources.TopDay}{Properties.Resources.Monday} ({StatVariables.TopDayList[(DayOfWeek)StatVariables.TopDayID]}%)"; break;
                case 2: fieldTopDay.Content = $"{Properties.Resources.TopDay}{Properties.Resources.Tuesday} ({StatVariables.TopDayList[(DayOfWeek)StatVariables.TopDayID]}%)"; break;
                case 3: fieldTopDay.Content = $"{Properties.Resources.TopDay}{Properties.Resources.Wednesday} ({StatVariables.TopDayList[(DayOfWeek)StatVariables.TopDayID]}%)"; break;
                case 4: fieldTopDay.Content = $"{Properties.Resources.TopDay}{Properties.Resources.Thursday} ({StatVariables.TopDayList[(DayOfWeek)StatVariables.TopDayID]}%)"; break;
                case 5: fieldTopDay.Content = $"{Properties.Resources.TopDay}{Properties.Resources.Friday} ({StatVariables.TopDayList[(DayOfWeek)StatVariables.TopDayID]}%)"; break;
                case 6: fieldTopDay.Content = $"{Properties.Resources.TopDay}{Properties.Resources.Saturday} ({StatVariables.TopDayList[(DayOfWeek)StatVariables.TopDayID]}%)"; break;
            }

            for (int i = 0; i < 7; i++)
            {
                switch (i)
                {
                    case 0: daysTip.Content = ""; daysTip.Content += $"{Properties.Resources.Sunday} ({StatVariables.TopDayList[(DayOfWeek)i]}%)"; break;
                    case 1: daysTip.Content += $"{Properties.Resources.Monday} ({StatVariables.TopDayList[(DayOfWeek)i]}%)"; break;
                    case 2: daysTip.Content += $"{Properties.Resources.Tuesday} ({StatVariables.TopDayList[(DayOfWeek)i]}%)"; break;
                    case 3: daysTip.Content += $"{Properties.Resources.Wednesday} ({StatVariables.TopDayList[(DayOfWeek)i]}%)"; break;
                    case 4: daysTip.Content += $"{Properties.Resources.Thursday} ({StatVariables.TopDayList[(DayOfWeek)i]}%)"; break;
                    case 5: daysTip.Content += $"{Properties.Resources.Friday} ({StatVariables.TopDayList[(DayOfWeek)i]}%)"; break;
                    case 6: daysTip.Content += $"{Properties.Resources.Saturday} ({StatVariables.TopDayList[(DayOfWeek)i]}%)"; break;
                }

                if (i != 6)
                    daysTip.Content += "\n";
            }

            switch (StatVariables.TopRoleID)
            {
                case 0: fieldTopRole.Content = $"{Properties.Resources.TopRole}{Properties.Resources.RlTank} ({StatVariables.TopRoleList[(Roles)StatVariables.TopRoleID]}%)"; break;
                case 1: fieldTopRole.Content = $"{Properties.Resources.TopRole}{Properties.Resources.RlFighter} ({StatVariables.TopRoleList[(Roles)StatVariables.TopRoleID]}%)"; break;
                case 2: fieldTopRole.Content = $"{Properties.Resources.TopRole}{Properties.Resources.RlMage} ({StatVariables.TopRoleList[(Roles)StatVariables.TopRoleID]}%)"; break;
                case 3: fieldTopRole.Content = $"{Properties.Resources.TopRole}{Properties.Resources.RlMarksman} ({StatVariables.TopRoleList[(Roles)StatVariables.TopRoleID]}%)"; break;
                case 4: fieldTopRole.Content = $"{Properties.Resources.TopRole}{Properties.Resources.RlSupport} ({StatVariables.TopRoleList[(Roles)StatVariables.TopRoleID]}%)"; break;
                case 5: fieldTopRole.Content = $"{Properties.Resources.TopRole}{Properties.Resources.RlAssassin} ({StatVariables.TopRoleList[(Roles)StatVariables.TopRoleID]}%)"; break;
            }

            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0: rolesTip.Content = ""; rolesTip.Content += $"{Properties.Resources.RlTank} ({StatVariables.TopRoleList[(Roles)i]}%)"; break;
                    case 1: rolesTip.Content += $"{Properties.Resources.RlFighter} ({StatVariables.TopRoleList[(Roles)i]}%)"; break;
                    case 2: rolesTip.Content += $"{Properties.Resources.RlMage} ({StatVariables.TopRoleList[(Roles)i]}%)"; break;
                    case 3: rolesTip.Content += $"{Properties.Resources.RlMarksman} ({StatVariables.TopRoleList[(Roles)i]}%)"; break;
                    case 4: rolesTip.Content += $"{Properties.Resources.RlSupport} ({StatVariables.TopRoleList[(Roles)i]}%)"; break;
                    case 5: rolesTip.Content += $"{Properties.Resources.RlAssassin} ({StatVariables.TopRoleList[(Roles)i]}%)"; break;
                }

                if (i != 5)
                    rolesTip.Content += "\n";
            }

            if (StatVariables.ResultInfluencePercent != -1)
                fieldInfluence.Content = $"{Properties.Resources.Influence}{StatVariables.ResultInfluencePercent}%";
            else
                fieldInfluence.Content = $"{Properties.Resources.Influence}{Properties.Resources.RequiredMatches}";

            influenceTip.Content = $"{Properties.Resources.InfluenceTip}{Properties.Settings.Default.CurrentKDA})\n\n{Properties.Resources.InfluenceRightButtonClickTip}";

            foreach (var row in StatVariables.DataGridList)
                table.Items.Add(row);
        }

        private void table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dg = sender as DataGrid;
                var sItem = (TableElements)dg.SelectedItem;

                foreach (var match in MatchListDetails.MatchList)
                {
                    if (new DateTime(1970, 1, 1).AddMilliseconds(match.GameCreation).AddHours(2) == sItem.GameDate)
                    {
                        ThreadManager.ParamThread((object obj) => { new DetailedMatchInfo(match).ShowDialog(); }, match);
                        break;
                    }
                }
            }
            catch { }
        }

        private void table_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }
    }
}