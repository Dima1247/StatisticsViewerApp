using System;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using StaticElementsLibrary;

namespace ManageActivityLibrary
{
    public class ChartManager
    {
        CartesianChart chart = new CartesianChart();

        public ChartManager(CartesianChart _chart)
        {
            chart = _chart;
        }

        public CartesianChart ChartFarmLoad(int matchesCount)
        {
            if (CurrentUserElements.MatchList.Matches.Count >= matchesCount)
                CompareFarm(matchesCount);
            else
                MessageBox.Show("You don`t have enough matches!");

            return chart;
        }

        public CartesianChart ChartKDALoad(int matchesCount)
        {
            if (CurrentUserElements.MatchList.Matches.Count >= matchesCount)
                CompareKDA(matchesCount);
            else
                MessageBox.Show("You don`t have enough matches!");

            return chart;
        }

        void CompareFarm(int gameCount)
        {
            int partID = 0;
            int halfGames = gameCount / 2;
            ChartValues<int> graphItems = new ChartValues<int>();
            LineSeries seria = new LineSeries();

            if (chart.Series.Count > 0)
                chart.Series.Clear();

            for (int i = (halfGames * 2 - 1); i > -1; i--)
            {
                Match match = CurrentUserElements.MatchList.Matches[i];

                foreach (var participentIdent in match.ParticipantIdentities)
                {
                    if (participentIdent.Player.SummonerId == CurrentUserElements.Player.ID)
                    {
                        partID = participentIdent.ParticipantID;
                    }
                }

                foreach (var participant in match.Participants)
                {
                    if (participant.Stats.ParticipantID == partID)
                        graphItems.Add(participant.Stats.GoldEarned);
                }

                if (i == halfGames || i == 0)
                {
                    if (i != 0)
                        seria.Title = string.Format("First {0} matches", halfGames);
                    else
                        seria.Title = string.Format("Second {0} matches", halfGames);

                    seria.Values = graphItems;

                    if (seria.Values.Count != 0)
                        chart.Series.Add(seria);

                    graphItems = new ChartValues<int>();
                    seria = new LineSeries();
                }
            }
        }

        void CompareKDA(int gameCount)
        {
            int partID = 0;
            ChartValues<int> graphKills = new ChartValues<int>();
            ChartValues<int> graphDeaths = new ChartValues<int>();
            ChartValues<int> graphAssists = new ChartValues<int>();
            LineSeries seria = new LineSeries();

            if (chart.Series.Count > 0)
                chart.Series.Clear();

            for (int i = gameCount - 1; i >= 0; i--)
            {
                Match match = CurrentUserElements.MatchList.Matches[i];

                foreach (var participentIdent in match.ParticipantIdentities)
                {
                    if (participentIdent.Player.SummonerId == CurrentUserElements.Player.ID)
                    {
                        partID = participentIdent.ParticipantID;
                    }
                }

                foreach (var participant in match.Participants)
                {
                    if (participant.Stats.ParticipantID == partID)
                    {
                        graphKills.Add(participant.Stats.Kills);
                        graphDeaths.Add(participant.Stats.Deaths);
                        graphAssists.Add(participant.Stats.Assists);
                    }
                }
            }

            seria.Title = "Kills";
            seria.Values = graphKills;
            chart.Series.Add(seria);

            seria = new LineSeries();
            seria.Title = "Deaths";
            seria.Values = graphDeaths;
            chart.Series.Add(seria);

            seria = new LineSeries();
            seria.Title = "Assists";
            seria.Values = graphAssists;
            chart.Series.Add(seria);
        }
    }
}
