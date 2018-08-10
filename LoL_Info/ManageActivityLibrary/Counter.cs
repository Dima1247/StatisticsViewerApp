using System;
using System.Collections.Generic;
using StaticElementsLibrary;

namespace LoL_Info.Classes
{
    public class Counter
    {
        // Uses private classes to load information to StatVariables
        public void LoadData(int filterType, List<Match> matchList)
        {
            UseData(filterType, matchList);
            MakeAverage();
        }

        // Converts usual information to average stats 
        void MakeAverage()
        {
            try
            {
                StatVariables.AverageKills /= CurrentUserElements.MatchList.Matches.Count;
                StatVariables.AverageDeaths /= CurrentUserElements.MatchList.Matches.Count;
                StatVariables.AverageAssists /= CurrentUserElements.MatchList.Matches.Count;
                StatVariables.AverageGold /= CurrentUserElements.MatchList.Matches.Count;

                StatVariables.WinsPercent = Convert.ToInt32((StatVariables.WinsCount / CurrentUserElements.MatchList.Matches.Count) * 100);
            }
            catch { }
        }

        // Loads information from AllVeriables to StatVeriables
        void UseData(int filterType, List<Match> matchList)
        {
            List<Match> matches = new List<Match>();
            int currentParticipantID = 0; string isWinner = "Loser";
            Dictionary<DateTime, bool> dateList = new Dictionary<DateTime, bool>();
            List<TableElements> dataGridList = new List<TableElements>();

            if (!StatVariables.IsEmpty())
            {
                StatVariables.MakeNulls();
            }

            foreach (var match in matchList)
            {
                DateTime date = new DateTime(1970, 1, 1).AddMilliseconds((match.GameCreation)).AddHours(2);

                foreach (var participantID in match.ParticipantIdentities)
                {
                    if (participantID.Player.SummonerId == CurrentUserElements.Player.ID)
                    {
                        currentParticipantID = participantID.ParticipantID;
                    }
                }

                foreach (var participant in match.Participants)
                {
                    var stats = participant.Stats;
                    if (stats.ParticipantID == currentParticipantID)
                    {
                        isWinner = "Loser";
                        if (stats.Win)
                        {
                            isWinner = "Winner";
                            StatVariables.WinsCount++;
                        }

                        switch (filterType)
                        {
                            case 0: // All matches
                                matches.Add(match);
                                dataGridList.Add(new TableElements(string.Format("http://ddragon.leagueoflegends.com/cdn/6.24.1/img/champion/{0}.png",
                                    CurrentUserElements.Champions.Keys[participant.ChampionID]), date.ToString(), isWinner, string.Format("{0}/{1}/{2}",
                                    stats.Kills, stats.Deaths, stats.Assists), stats.GoldEarned.ToString()));
                                break;
                            case 1: // Ranked Matches
                                matches.Add(match);
                                if (match.QueueID == 420 || match.QueueID == 440 || match.QueueID == 470)
                                    dataGridList.Add(new TableElements(string.Format("http://ddragon.leagueoflegends.com/cdn/6.24.1/img/champion/{0}.png",
                                        CurrentUserElements.Champions.Keys[participant.ChampionID]), date.ToString(), isWinner, string.Format("{0}/{1}/{2}",
                                        stats.Kills, stats.Deaths, stats.Assists), stats.GoldEarned.ToString()));
                                break;
                            case 2: // Simple Matches
                                matches.Add(match);
                                if (match.QueueID != 420 && match.QueueID != 440 && match.QueueID != 470)
                                    dataGridList.Add(new TableElements(string.Format("http://ddragon.leagueoflegends.com/cdn/6.24.1/img/champion/{0}.png",
                                        CurrentUserElements.Champions.Keys[participant.ChampionID]), date.ToString(), isWinner, string.Format("{0}/{1}/{2}",
                                        stats.Kills, stats.Deaths, stats.Assists), stats.GoldEarned.ToString()));
                                break;
                        }

                        StatVariables.AverageKills += stats.Kills;
                        StatVariables.AverageDeaths += stats.Deaths;
                        StatVariables.AverageAssists += stats.Assists;
                        StatVariables.AverageGold += stats.GoldEarned;

                        dateList.Add(new DateTime(1970, 1, 1).AddMilliseconds(match.GameCreation).AddHours(2), stats.Win);
                    }
                }
            }
            TopDayOfTheWeek(dateList);
            StatVariables.DataGridList = dataGridList;
        }

        // Finds the TOP day of this week
        void TopDayOfTheWeek(Dictionary<DateTime, bool> dateList)
        {
            float[] gamesCount = new float[7];
            float[] winsCount = new float[7];
            float winsPercent = 0;
            int gameDayNumber = 0, todayDayNumber = 0;

            foreach (var day in dateList)
            {
                if (day.Key.DayOfWeek == 0)
                    gameDayNumber = 7;
                else
                    gameDayNumber = (int)day.Key.DayOfWeek;


                if (DateTime.Today.DayOfWeek == 0)
                    todayDayNumber = 7;
                else
                    todayDayNumber = (int)DateTime.Today.DayOfWeek;


                if (gameDayNumber <= todayDayNumber && day.Key.AddDays(6) > DateTime.Today)
                {
                    gamesCount[(int)day.Key.DayOfWeek]++;

                    if (day.Value)
                        winsCount[(int)day.Key.DayOfWeek]++;
                }
            }

            for (int i = 0; i < 7; i++)
            {
                winsPercent = winsCount[i] / gamesCount[i];

                if ((StatVariables.TopDayPercent / 100) < winsPercent ||
                    ((StatVariables.TopDayPercent / 100) == winsPercent && winsCount[i] > winsCount[StatVariables.TopDayID]))
                {
                    StatVariables.TopDayPercent = Convert.ToInt32(winsPercent * 100);
                    StatVariables.TopDayID = i;
                }
            }
        }

        public List<MatchDetails> GetDetailsByMatchType(int filterType)
        {
            List<MatchDetails> detailedMatches = new List<MatchDetails>();
            foreach (var match in CurrentUserElements.MatchList.Matches)
            {
                switch (filterType)
                {
                    case 1: // Ranked Matches
                        if (match.QueueID == 420 || match.QueueID == 440 || match.QueueID == 470)
                            foreach (var detailedMatch in MatchListDetails.MatchList)
                                if (match.GameID == detailedMatch.GameID)
                                    detailedMatches.Add(detailedMatch);
                        break;

                    case 2: // Simple Matches
                        if (match.QueueID != 420 && match.QueueID != 440 && match.QueueID != 470)
                            foreach (var detailedMatch in MatchListDetails.MatchList)
                                if (match.GameID == detailedMatch.GameID)
                                    detailedMatches.Add(detailedMatch);
                        break;
                }
            }
            return detailedMatches;
        }

    }
}