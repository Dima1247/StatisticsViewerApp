using System;
using System.Collections.Generic;
using System.Windows.Controls;
using StaticElementsLibrary;
using DataAccessLayer;

namespace LoL_Info.Classes
{
    public class CompareManager
    {
        private MatchList matches = new MatchList();
        private List<MatchList> matchesList = new List<MatchList>();
        private List<string> checkedPlayers = new List<string>();
        private List<long> gameIds = new List<long>();

        public CompareManager(List<CheckBox> playersCheckList)
        {
            CheckedPLayersToMatches(playersCheckList);
            CheckedGameIds();
        }

        // Find checkedPlayers and matchesList collections
        private void CheckedPLayersToMatches(List<CheckBox> playersCheckList)
        {
            // send checked players from playersCheckList to checkedPLayers
            for (int i = 0; i < playersCheckList.Count; i++)
            {
                if (playersCheckList[i].IsChecked == true)
                    checkedPlayers.Add(playersCheckList[i].Content.ToString());
            }

            // load match lists of checkedPlayers to matchList
            for (int i = 0; i < checkedPlayers.Count; i++)
            {
                matches = new MatchList();
                MatchDB.LoadMatches(ref matches, checkedPlayers[i], CurrentUserElements.Player.ID);
                matchesList.Add(matches);
            }
        }

        // Find command gameIds collection of all checkedPlayers
        private void CheckedGameIds()
        {
            var gameIdsFalse = new List<long>();

            // take mainMatches and matches of the first checkedPlayers[0] and 
            // choose gameIds of command matches
            foreach (var mainMatches in CurrentUserElements.MatchList.Matches)
            {
                for (int i = 0; i < matchesList[0].Matches.Count; i++)
                {
                    if (mainMatches.GameID == matchesList[0].Matches[i].GameID)
                    {
                        if (!gameIds.Contains(mainMatches.GameID))
                            gameIds.Add(mainMatches.GameID);
                    }
                }
            }

            // take matches of other checkedPLayers and turn command matches
            // from gameIds collection to falseGameIds collection finally 
            // swap them and get gameIds of all commandMatches for checkedPlayers
            for (int i = 1; i < matchesList.Count; i++)
            {
                gameIdsFalse = new List<long>();

                foreach (var match in matchesList[i].Matches)
                {
                    if (gameIds.Contains(match.GameID))
                    {
                        if (!gameIdsFalse.Contains(match.GameID))
                            gameIdsFalse.Add(match.GameID);
                    }
                }

                gameIds = gameIdsFalse;
            }
        }

        // Get command matches by Match structure
        public List<Match> GetCheckedMatches()
        {
            var checkedMatches = new List<Match>();

            // Takes gameIds collection and find matches by ID
            for (int i = 0; i < gameIds.Count; i++)
            {
                foreach (var row in CurrentUserElements.MatchList.Matches)
                {
                    if (gameIds[i] == row.GameID)
                    {
                        if (!checkedMatches.Contains(row))
                            checkedMatches.Add(row);
                    }
                }
            }

            return checkedMatches;
        }
        
        // Get command matches by Match structure
        public List<MatchDetails> GetCheckedMatchesDetails()
        {
            var checkedMatches = new List<MatchDetails>();

            // Takes gameIds collection and find matches by ID
            for (int i = 0; i < gameIds.Count; i++)
            {
                foreach (var row in MatchListDetails.MatchList)
                {
                    if (gameIds[i] == row.GameID)
                    {
                        if (!checkedMatches.Contains(row))
                            checkedMatches.Add(row);
                    }
                }
            }

            return checkedMatches;
        }

        public List<string> GetCheckedPlayers()
        {
            return checkedPlayers;
        }
    }
}
