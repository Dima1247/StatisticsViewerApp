using System;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using StaticElementsLibrary;
using System.Collections.Generic;
using System.Windows.Media;
using LiveCharts.Defaults;

namespace LoL_Info.Classes
{
    public static class ChartManager
    {
        private static int winRate, matchesCount;

        public static int WinRate { get { return winRate; } }
        public static int MatchesCount { get { return matchesCount; } }

        public static void LoadChart(string chartType, string matchesType, LineSeries[] series, Roles roleType)
        {
            int count = 0, wins = 0;
            double totalValue = 0, firstTrendValue = 0, secondTrendValue = 0;

            var chartList = new List<double>();

            matchesCount = 0;

            foreach (var value in StatVariables.ChartList)
            {
                if (value.Roles.Contains((int)roleType) || (roleType == Roles.None && matchesCount < CurrentUserElements.MatchList.Matches.Count))
                {
                    bool isAllowed = false;

                    if (matchesType == Properties.Resources.AllMatches)
                    {
                        isAllowed = true;
                    }
                    else if (matchesType == Properties.Resources.RankedMatches)
                    {
                        if (value.QueueId == 420 || value.QueueId == 440 || value.QueueId == 470)
                            isAllowed = true;
                    }
                    else if (matchesType == Properties.Resources.SimpleMatches)
                    {
                        if (value.QueueId != 420 && value.QueueId != 440 && value.QueueId != 470)
                            isAllowed = true;
                    }

                    if (isAllowed)
                    {
                        if (chartType == Properties.Resources.ChartKDA)
                            chartList.Add(value.KDA);

                        else if (chartType == Properties.Resources.ChartFarm)
                            chartList.Add(value.Farm);

                        matchesCount++;

                        if (value.IsWon)
                            wins++;
                    }
                }
            }

            if (matchesCount != 0)
                winRate = (wins * 100) / matchesCount;

            var graphValues = new ChartValues<double>();
            var averageValueLine = new ChartValues<ObservablePoint>();
            var trendLineValues = new ChartValues<ObservablePoint>();

            for (int i = matchesCount - 1; i >= 0; i--)
            {
                totalValue += chartList[i];

                if (i == matchesCount / 2)
                {
                    firstTrendValue /= count;
                    count = 0;
                }
                else if (i == 0)
                {
                    secondTrendValue /= count;
                    count = 0;
                }
                else
                {
                    if (i > matchesCount / 2)
                        firstTrendValue += chartList[i];
                    else
                        secondTrendValue += chartList[i];
                    count++;
                }
            }

            for (int i = matchesCount - 1; i >= 0; i--)
            {
                graphValues.Add(chartList[i]);
            }

            averageValueLine.Add(new ObservablePoint(0, Math.Round((totalValue / matchesCount), 2)));
            averageValueLine.Add(new ObservablePoint(matchesCount - 1, Math.Round((totalValue / matchesCount), 2)));

            trendLineValues.Add(new ObservablePoint(0, Math.Round(firstTrendValue, 2)));
            trendLineValues.Add(new ObservablePoint(matchesCount - 1, Math.Round(secondTrendValue, 2)));

            series[0].Values = graphValues;
            series[1].Values = averageValueLine;
            series[2].Values = trendLineValues;
        }
    }
}
