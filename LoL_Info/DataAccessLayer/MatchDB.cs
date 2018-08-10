using System;
using System.Collections.Generic;
using System.IO;
using StaticElementsLibrary;

namespace DataAccessLayer
{
    public class MatchDB
    {
        private static LoLDataBase LoL_DB = new LoLDataBase();
        private static readonly string mainDirectoryName = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\LoL_Info_Data\\Players";
        private static string fileName = "";
        private static string region = "ru";

        public static long LastMatchId
        {
            get
            {
                if (LoL_DB.Match.Count < 1)
                    return 0;

                return LoL_DB.Match[LoL_DB.Match.Count - 1].GameID;
            }
        }

        public static void SaveMatches(ref MatchList currentMatchList, string playerName, long summonerID, bool IsUpdate)
        {
            var gameIds = new List<long>();

            Directory.CreateDirectory($"{mainDirectoryName}\\{region}\\{playerName}");

            if (File.Exists(fileName))
                foreach (var match in LoL_DB.Match)
                    gameIds.Add(match.GameID);
            
            if (!IsUpdate)
                LoL_DB.Match.Clear();

            for (int i = currentMatchList.Matches.Count - 1; i >= 0; i--)
            {
                Match match = currentMatchList.Matches[i];

                if (!gameIds.Contains(match.GameID))
                {
                    LoLDataBase.MatchRow matchRow = LoL_DB.Match.NewMatchRow();
                    matchRow.GameID = match.GameID;
                    matchRow.GameCreation = match.GameCreation;
                    matchRow.QueueID = match.QueueID;
                    matchRow.GameDuration = match.GameDuration;

                    matchRow.ChampionID = new int[match.Participants.Count];
                    matchRow.SummonerID = new long[match.Participants.Count];
                    matchRow.ParticipantID = new int[match.Participants.Count];
                    matchRow.ChampLevel = new int[match.Participants.Count];
                    matchRow.SumName = new string[match.Participants.Count];
                    matchRow.Win = new bool[match.Participants.Count];
                    matchRow.Kills = new int[match.Participants.Count];
                    matchRow.Deaths = new int[match.Participants.Count];
                    matchRow.Assists = new int[match.Participants.Count];
                    matchRow.GoldEarned = new int[match.Participants.Count];
                    matchRow.Item1 = new int[match.Participants.Count];
                    matchRow.Item2 = new int[match.Participants.Count];
                    matchRow.Item3 = new int[match.Participants.Count];
                    matchRow.Item4 = new int[match.Participants.Count];
                    matchRow.Item5 = new int[match.Participants.Count];
                    matchRow.Item6 = new int[match.Participants.Count];
                    matchRow.TotalMinionsKilled = new int[match.Participants.Count];

                    int j = 0;
                    foreach (var participant in match.Participants)
                    {
                        var stats = participant.Stats;

                        matchRow.SummonerID[j] = match.ParticipantIdentities[j].Player.SummonerId;
                        matchRow.ChampionID[j] = participant.ChampionID;
                        matchRow.ParticipantID[j] = stats.ParticipantID;
                        matchRow.ChampLevel[j] = stats.ChampLevel;
                        matchRow.SumName[j] = match.ParticipantIdentities[j].Player.SummonerName;
                        matchRow.Win[j] = stats.Win;
                        matchRow.Kills[j] = stats.Kills;
                        matchRow.Deaths[j] = stats.Deaths;
                        matchRow.Assists[j] = stats.Assists;
                        matchRow.GoldEarned[j] = stats.GoldEarned;
                        matchRow.Item1[j] = stats.Item1;
                        matchRow.Item2[j] = stats.Item2;
                        matchRow.Item3[j] = stats.Item3;
                        matchRow.Item4[j] = stats.Item4;
                        matchRow.Item5[j] = stats.Item5;
                        matchRow.Item6[j] = stats.Item6;
                        matchRow.TotalMinionsKilled[j] = stats.TotalMinionsKilled;
                        j++;
                    }

                    LoL_DB.Match.AddMatchRow(matchRow);
                }
            }

            LoL_DB.Match.AcceptChanges();
            LoL_DB.Match.WriteXml(fileName);
            LoadDetailedMatches(playerName);
        }

        public static void LoadMatches(ref MatchList currentMatchList, string playerName, long summonerID)
        {
            var matchList = new List<Match>(); int partID = 0;
            
            string localFileName = $"{mainDirectoryName}\\{region}\\{playerName}\\matchDB.txt";

            LoL_DB.Clear();
            LoL_DB.ReadXml(localFileName);

            foreach (var match in LoL_DB.Match)
            {
                Match baseMatch = new Match();
                ParticipantIdent participantIdentity = new ParticipantIdent();
                PlayerAsParticipant player = new PlayerAsParticipant();
                Participant participant = new Participant();
                ParticipantStats stats = new ParticipantStats();

                baseMatch.GameID = match.GameID;
                baseMatch.QueueID = match.QueueID;
                baseMatch.GameCreation = match.GameCreation;
                baseMatch.GameDuration = match.GameDuration;

                for (int i = 0; i < match.SummonerID.Length; i++)
                {
                    if (match.SummonerID[i] == summonerID)
                    {
                        participantIdentity.ParticipantID = match.ParticipantID[i];
                        player.SummonerId = summonerID;
                        participantIdentity.Player = player;
                        partID = i;
                    }
                }

                List<ParticipantIdent> identificatioList = new List<ParticipantIdent>();
                identificatioList.Add(participantIdentity);
                baseMatch.ParticipantIdentities = identificatioList;

                participant.ChampionID = match.ChampionID[partID];
                stats.Win = match.Win[partID];
                stats.ParticipantID = match.ParticipantID[partID];
                stats.Kills = match.Kills[partID];
                stats.Deaths = match.Deaths[partID];
                stats.Assists = match.Assists[partID];
                stats.GoldEarned = match.GoldEarned[partID];
                stats.TotalMinionsKilled = match.TotalMinionsKilled[partID];
                participant.Stats = stats;

                List<Participant> participantList = new List<Participant>();
                participantList.Add(participant);
                baseMatch.Participants = participantList;

                matchList.Add(baseMatch);
            }

            matchList.Reverse();
            currentMatchList.Matches = matchList;
            LoadDetailedMatches(playerName);
        }

        private static void LoadDetailedMatches(string playerName)
        {
            LoL_DB.Clear();
            LoL_DB.ReadXml(fileName);

            MatchListDetails.MatchList = new List<MatchDetails>();

            foreach (var match in LoL_DB.Match)
            {
                MatchDetails matchDetails = new MatchDetails();

                matchDetails.GameID = match.GameID;
                matchDetails.QueueID = match.QueueID;
                matchDetails.GameCreation = match.GameCreation;

                matchDetails.ParticipantID = new List<int>();
                matchDetails.SummonerID = new List<long>();
                matchDetails.ChampionID = new List<int>();
                matchDetails.ChampLevel = new List<int>();
                matchDetails.SumName = new List<string>();
                matchDetails.Win = new List<bool>();
                matchDetails.Kills = new List<int>();
                matchDetails.Deaths = new List<int>();
                matchDetails.Assists = new List<int>();
                matchDetails.Farm = new List<int>();

                matchDetails.Item1 = new List<int>();
                matchDetails.Item2 = new List<int>();
                matchDetails.Item3 = new List<int>();
                matchDetails.Item4 = new List<int>();
                matchDetails.Item5 = new List<int>();
                matchDetails.Item6 = new List<int>();

                for (int i = 0; i < match.ParticipantID.Length; i++)
                {
                    matchDetails.ParticipantID.Add(match.ParticipantID[i]);
                    matchDetails.SummonerID.Add(match.SummonerID[i]);
                    matchDetails.ChampionID.Add(match.ChampionID[i]);
                    matchDetails.ChampLevel.Add(match.ChampLevel[i]);
                    matchDetails.SumName.Add(match.SumName[i]);
                    matchDetails.Win.Add(match.Win[i]);
                    matchDetails.Kills.Add(match.Kills[i]);
                    matchDetails.Deaths.Add(match.Deaths[i]);
                    matchDetails.Assists.Add(match.Assists[i]);
                    matchDetails.Farm.Add(match.TotalMinionsKilled[i]);

                    matchDetails.Item1.Add(match.Item1[i]);
                    matchDetails.Item2.Add(match.Item2[i]);
                    matchDetails.Item3.Add(match.Item3[i]);
                    matchDetails.Item4.Add(match.Item4[i]);
                    matchDetails.Item5.Add(match.Item5[i]);
                    matchDetails.Item6.Add(match.Item6[i]);
                }

                MatchListDetails.MatchList.Add(matchDetails);
            }

            MatchListDetails.MatchList.Reverse();
        }

        public static bool MatchesExist(string playerName, string currentRegion)
        {
            LoL_DB = new LoLDataBase();

            region = currentRegion;
            fileName = $"{mainDirectoryName}\\{region}\\{playerName}\\matchDB.txt";

            if (File.Exists(fileName))
            {
                LoL_DB.Clear();
                LoL_DB.ReadXml(fileName);

                return true;
            }

            return false;
        }
    }
}
