using System;
using System.Windows;
using System.Windows.Controls;
using LoL_Info.Classes;
using StaticElementsLibrary;
using System.Windows.Media;
using System.Collections.Generic;

namespace LoL_Info.Windows
{
    public partial class Charts : UserControl
    {
        private string chartName = Properties.Resources.ChartKDA;
        private string matchesType = Properties.Resources.AllMatches;

        private Border currentChartRoleButton = null;
        private Roles currentRole = Roles.None;
        
        public Charts()
        {
            InitializeComponent();

            new MainCounter().LoadData(CurrentUserElements.MatchList, 0);

            ResetButtons();
            LoadChart();
        }

        private void btnKDA_Click(object sender, RoutedEventArgs e)
        {
            chartName = Properties.Resources.ChartKDA;
            seria1.Title = Properties.Resources.TextKDA;
            LoadChart();
        }

        private void btnFarm_Click(object sender, RoutedEventArgs e)
        {
            chartName = Properties.Resources.ChartFarm;
            seria1.Title = Properties.Resources.TextFarm;
            LoadChart();
        }

        private void LoadChart()
        {
            ChartManager.LoadChart(chartName, matchesType, new[] { seria1, seria2, seria3 }, currentRole);
            chartNameLabel.Content = $"{chartName} ({matchesType})";
            lbMatchesCount.Content = $"{Properties.Resources.MatchesCount} {ChartManager.MatchesCount}";
            lbWinRate.Content = $"{Properties.Resources.WinRate} {ChartManager.WinRate}%";
        }

        private void SetActivity(Border button)
        {
            ResetButtons();

            if (currentChartRoleButton != button)
            {
                currentChartRoleButton = button;
                button.BorderBrush = new SolidColorBrush(Color.FromRgb(244, 93, 81));
            }
            else
            {
                currentChartRoleButton = null;
                LoadChart();
            }
        }

        private void ResetButtons()
        {
            var commonColor = new SolidColorBrush(Color.FromRgb(201, 170, 113));

            currentRole = Roles.None;

            btnTank.BorderBrush = commonColor;
            btnFighter.BorderBrush = commonColor;
            btnMage.BorderBrush = commonColor;
            btnMarksman.BorderBrush = commonColor;
            btnSupport.BorderBrush = commonColor;
            btnAssassin.BorderBrush = commonColor;
        }

        #region ButtonsEvents
        private void btnChart_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var button = sender as Border;
            if (currentChartRoleButton != button)
                button.BorderBrush = new SolidColorBrush(Color.FromRgb(244, 93, 81));
        }

        private void btnChart_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var button = sender as Border;
            if (currentChartRoleButton != button)
                button.BorderBrush = new SolidColorBrush(Color.FromRgb(201, 170, 113));
        }

        private void btnTank_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetActivity(sender as Border);

            if (currentChartRoleButton == sender as Border)
                currentRole = Roles.Tank;

            LoadChart();
        }

        private void btnFighter_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetActivity(sender as Border);

            if (currentChartRoleButton == sender as Border)
                currentRole = Roles.Fighter;

            LoadChart();
        }

        private void btnMage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetActivity(sender as Border);

            if (currentChartRoleButton == sender as Border)
                currentRole = Roles.Mage;

            LoadChart();
        }

        private void btnMarksman_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetActivity(sender as Border);

            if (currentChartRoleButton == sender as Border)
                currentRole = Roles.Marksman;

            LoadChart();
        }

        private void btnSupport_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetActivity(sender as Border);

            if (currentChartRoleButton == sender as Border)
                currentRole = Roles.Support;

            LoadChart();
        }

        private void btnAssassin_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetActivity(sender as Border);

            if (currentChartRoleButton == sender as Border)
                currentRole = Roles.Assassin;

            LoadChart();
        }
        #endregion

        private void partAll_Click(object sender, RoutedEventArgs e)
        {
            matchesType = Properties.Resources.AllMatches;
            LoadChart();
        }

        private void partRanked_Click(object sender, RoutedEventArgs e)
        {
            matchesType = Properties.Resources.RankedMatches;
            LoadChart();
        }

        private void partSimple_Click(object sender, RoutedEventArgs e)
        {
            matchesType = Properties.Resources.SimpleMatches;
            LoadChart();
        }
    }
}
