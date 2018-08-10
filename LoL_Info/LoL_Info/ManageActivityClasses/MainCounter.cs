using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaticElementsLibrary;

namespace LoL_Info.Classes
{
    class MainCounter
    {
        private MatchList matchesList;

        // matchType: 1- all, 2- ranked, 3- simple
        public void LoadData(MatchList matches, int matchType)
        {
            this.matchesList = matches;
            ManageData(matchType);
            MakeAverage();
        }

        // Converts usual information to average stats 
        private void MakeAverage()
        {
            try
            {
                StatVariables.AverageKills /= StatVariables.MatchesCount;
                StatVariables.AverageDeaths /= StatVariables.MatchesCount;
                StatVariables.AverageAssists /= StatVariables.MatchesCount;
                StatVariables.AvarageFarm /= StatVariables.MatchesCount;
                StatVariables.MinionsPerMinute /= StatVariables.MatchesCount;

                if (StatVariables.AverageDeaths == 0)
                    StatVariables.RateKDA = StatVariables.AverageKills + StatVariables.AverageAssists;
                else
                    StatVariables.RateKDA = (float)(StatVariables.AverageKills + StatVariables.AverageAssists) / StatVariables.AverageDeaths;

                StatVariables.RateKDA = Math.Round(StatVariables.RateKDA, 2);
                StatVariables.MinionsPerMinute = Math.Round(StatVariables.MinionsPerMinute, 0);

                StatVariables.WinsPercent = Convert.ToInt32((StatVariables.WinsCount / StatVariables.MatchesCount) * 100);
            }
            catch { }
        }

        private void ManageData(int matchType)
        {
            var wonMatchesDateList = new List<DateTime>();
            var allMatchesDateList = new List<DateTime>();
            var allMatchesRoleDict = new Dictionary<List<int>, bool>();

            StatVariables.ResetValues();

            foreach (var match in matchesList.Matches)
            {
                var matchDate = new DateTime(1970, 1, 1).AddMilliseconds(match.GameCreation).AddHours(2);

                int participantId = 0;

                foreach (var participantIdentity in match.ParticipantIdentities)
                {
                    if (participantIdentity.Player.SummonerId == CurrentUserElements.Player.ID)
                        participantId = participantIdentity.ParticipantID;
                }

                foreach (var participant in match.Participants)
                {
                    var stats = participant.Stats;

                    if (stats.ParticipantID == participantId)
                    {
                        // Is used to check different type matches existense 
                        bool isAllowed = false;
                        string isWinner = Properties.Resources.Winner;

                        switch (matchType)
                        {
                            case 0: // All matches
                                isAllowed = true; break;
                            case 1: // Ranked Matches
                                if (match.QueueID == 420 || match.QueueID == 440 || match.QueueID == 470)
                                    isAllowed = true; break;
                            case 2: // Simple Matches
                                if (match.QueueID != 420 && match.QueueID != 440 && match.QueueID != 470)
                                    isAllowed = true; break;
                        }

                        if (isAllowed)
                        {
                            StatVariables.MatchesCount++;

                            if (stats.Win)
                                StatVariables.WinsCount++;
                            else
                                isWinner = Properties.Resources.Loser;

                            double kdaPercent;
                            double minionsPerMinnute = Math.Round((stats.TotalMinionsKilled / (double)new DateTime(1970, 1, 1).AddSeconds(match.GameDuration).Minute), 0);

                            if (stats.Deaths == 0)
                                kdaPercent = stats.Kills + stats.Assists;
                            else
                                kdaPercent = (double)(stats.Kills + stats.Assists) / (double)stats.Deaths;

                            string day = "";
                            switch (matchDate.DayOfWeek)
                            {
                                case DayOfWeek.Sunday: day = Properties.Resources.Sunday; break;
                                case DayOfWeek.Monday: day = Properties.Resources.Monday; break;
                                case DayOfWeek.Tuesday: day = Properties.Resources.Tuesday; break;
                                case DayOfWeek.Wednesday: day = Properties.Resources.Wednesday; break;
                                case DayOfWeek.Thursday: day = Properties.Resources.Thursday; break;
                                case DayOfWeek.Friday: day = Properties.Resources.Friday; break;
                                case DayOfWeek.Saturday: day = Properties.Resources.Saturday; break;
                            }

                            string champName = "";
                            var currentChampionRoles = new List<string>();

                            foreach (var champ in CurrentUserElements.Champions.Data)
                                if (champ.Value.Key == participant.ChampionID)
                                {
                                    champName = champ.Key;
                                    currentChampionRoles = champ.Value.Tags;
                                }

                            StatVariables.DataGridList.Add(new TableElements($"http://ddragon.leagueoflegends.com/cdn/6.24.1/img/champion/{champName}.png", matchDate,
                                isWinner, kdaPercent, stats.TotalMinionsKilled));

                            StatVariables.AverageKills += stats.Kills;
                            StatVariables.AverageDeaths += stats.Deaths;
                            StatVariables.AverageAssists += stats.Assists;
                            StatVariables.AvarageFarm += stats.TotalMinionsKilled;
                            StatVariables.MinionsPerMinute += minionsPerMinnute;

                            var currentRoles = new List<int>();
                            foreach (var championRole in currentChampionRoles)
                            {
                                try
                                {
                                    currentRoles.Add((int)Enum.Parse(typeof(Roles), championRole));
                                }
                                catch { }
                            }
                            StatVariables.ChartList.Add(new ChartElement(currentRoles, kdaPercent, stats.TotalMinionsKilled, match.QueueID, stats.Win));
                            allMatchesDateList.Add(new DateTime(1970, 1, 1).AddMilliseconds(match.GameCreation).AddHours(2));
                            if (stats.Win)
                            {
                                wonMatchesDateList.Add(new DateTime(1970, 1, 1).AddMilliseconds(match.GameCreation).AddHours(2));
                                allMatchesRoleDict.Add(currentRoles, true);
                            }
                            else
                                allMatchesRoleDict.Add(currentRoles, false);
                        }
                    }
                }
            }

            CountInfluenceForCurrentKDA(Properties.Settings.Default.CurrentKDA);
            TopDay(allMatchesDateList, wonMatchesDateList);
            TopRole(allMatchesRoleDict);
        }

        private void TopDay(List<DateTime> allMatches, List<DateTime> wonMatches)
        {
            var days = new int[7, 3];

            allMatches.Reverse();

            for (int i = 0; i < allMatches.Count; i++)
            {
                int dayId = (int)allMatches[i].DayOfWeek;
                days[dayId, 0]++;

                if (wonMatches.Contains(allMatches[i]))
                    days[dayId, 1]++;
            }

            int maxPercnt = 0;

            for (int i = 0; i < 7; i++)
            {
                if (days[i, 0] != 0)
                    days[i, 2] = (days[i, 1] * 100) / days[i, 0];
                else
                    days[i, 2] = 0;

                StatVariables.TopDayList.Add((DayOfWeek)i, days[i, 2]);

                if (maxPercnt < days[i, 2])
                {
                    StatVariables.TopDayID = i;
                    maxPercnt = days[i, 2];
                }
            }
        }

        private void TopRole(Dictionary<List<int>, bool> allMatches)
        {
            var roles = new int[6, 3];

            allMatches.Reverse();

            foreach (var keysList in allMatches.Keys)
            {
                foreach (var key in keysList)
                    roles[key, 0]++;

                if (allMatches[keysList])
                    foreach (var key in keysList)
                        roles[key, 1]++;
            }

            int maxPercnt = 0;

            for (int i = 0; i < 6; i++)
            {
                if (roles[i, 0] != 0)
                    roles[i, 2] = (roles[i, 1] * 100) / roles[i, 0];
                else
                    roles[i, 2] = 0;

                StatVariables.TopRoleList.Add((Roles)i, roles[i, 2]);

                if (maxPercnt < roles[i, 2])
                {
                    StatVariables.TopRoleID = i;
                    maxPercnt = roles[i, 2];
                }
            }
        }

        public List<MatchDetails> GetDetailsByMatchType(int matchType, MatchList currentMatchesList)
        {
            var detailedMatches = new List<MatchDetails>();

            foreach (var match in currentMatchesList.Matches)
            {
                switch (matchType)
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

        public void CountInfluenceForCurrentKDA(double currentKDA)
        {
            double matchesCount = 0, wonMatchesCount = 0;

            for (int i = 0; i < StatVariables.ChartList.Count; i++)
            {
                if (StatVariables.ChartList[i].KDA >= currentKDA)
                {
                    if (StatVariables.ChartList[i].IsWon)
                        wonMatchesCount++;
                    matchesCount++;
                }
            }

            if (matchesCount != 0)
                StatVariables.ResultInfluencePercent = (int)((100 * wonMatchesCount) / matchesCount);
            else
                StatVariables.ResultInfluencePercent = -1;
        }
    }
}
