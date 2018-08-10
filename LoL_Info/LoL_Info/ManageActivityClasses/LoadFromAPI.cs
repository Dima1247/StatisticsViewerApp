using System.Threading;
using System.Windows;
using DataAccessLayer;
using ApiAccessLayer;
using StaticElementsLibrary;
using System.Windows.Controls;
using log4net;

namespace LoL_Info.Classes
{
    public class LoadFromAPI
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoadFromAPI));

        private static string key;
        private static string region = Properties.Settings.Default.Region;
        private static int isLoaded = 0;
        private static int fullLoad = 0;

        private MatchList currentMatchList = new MatchList() { Matches = new System.Collections.Generic.List<Match>() };
        private Champions currentChampionList = new Champions();
        private Player currentPlayer = new Player();
        private Match match = new Match();

        private string playerName;

        public static int IsLoaded
        {
            get { return isLoaded; }
        }

        public static int FullLoad
        {
            get { return fullLoad; }
        }

        public LoadFromAPI(string name)
        {
            playerName = name;
        }

        public void LoadInfo(MainWindow window, bool isTeamMode = false)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Culture);
                region = Properties.Settings.Default.Region;
                key = $"api_key={Properties.Settings.Default.ApiKey}";

                if (PlayerDB.PlayerExists(playerName, region))
                    PlayerDB.LoadPlayer(ref currentPlayer, playerName);
                else
                    LoadPlayer();

                log.Debug("Player`s data was loaded successfully!");

                if (HasNewMatches(playerName, currentPlayer.AccountID) && !isTeamMode)
                {
                    if (window.AskAboutUpdate())
                    {
                        LoadMatchList();
                        MatchDB.SaveMatches(ref currentMatchList, playerName, currentPlayer.ID, true);
                    }
                }

                if (MatchDB.MatchesExist(playerName, region))
                    MatchDB.LoadMatches(ref currentMatchList, playerName, currentPlayer.ID);
                else
                {
                    LoadMatchList();
                    PlayerDB.SavePlayer(ref currentPlayer, playerName);
                    MatchDB.SaveMatches(ref currentMatchList, playerName, currentPlayer.ID, false);
                }

                log.Debug("Player`s matches were loaded successfully!");

                fullLoad = currentMatchList.Matches.Count;

                if (ChampionDB.ChampionsExists(region))
                    ChampionDB.LoadChampions(ref currentChampionList);
                else
                {
                    LoadChampionList();
                    ChampionDB.SaveChampions(ref currentChampionList);
                }

                log.Debug("Champions were loaded successfully!");

                new CurrentUserElements(currentChampionList, currentPlayer, currentMatchList);
            }
            catch
            {
                log.Debug("There is a problem!");

                window.PLayersDataError();
                fullLoad = -1;
                isLoaded = 0;
                return;
            }
        }

        public static void ResetFullLoad()
        {
            fullLoad = 0;
        }

        public void UpdateInfo()
        {
            currentPlayer = CurrentUserElements.Player;

            LoadMatchList();
            MatchDB.SaveMatches(ref currentMatchList, playerName, CurrentUserElements.Player.ID, true);
            MatchDB.LoadMatches(ref currentMatchList, playerName, currentPlayer.ID);

            new CurrentUserElements(CurrentUserElements.Champions, CurrentUserElements.Player, currentMatchList);
        }

        public static bool HasNewMatches(string playerName, int accountId)
        {
            if (MatchDB.MatchesExist(playerName, region))
            {
                try
                {
                    var localMatchList = new MatchList();
                    new Finder<MatchList>(ref localMatchList, $"https://{region}.api.riotgames.com/lol/match/v3/matchlists/by-account/{accountId}?{key}");

                    if (MatchDB.LastMatchId != localMatchList.Matches[0].GameID)
                        return true;
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;
        }

        #region API_Loaders
        private void LoadPlayer()
        {
            new Finder<Player>(ref currentPlayer, $"https://{region}.api.riotgames.com/lol/summoner/v3/summoners/by-name/{playerName}?{key}");
        }

        private void LoadMatchList()
        {
            new Finder<MatchList>(ref currentMatchList, $"https://{region}.api.riotgames.com/lol/match/v3/matchlists/by-account/{currentPlayer.AccountID}?{key}");

            var localMatchList = new MatchList() { Matches = new System.Collections.Generic.List<Match>() };

            for (int i = 0; i < currentMatchList.Matches.Count; i++)
            {
                if (currentMatchList.Matches[i].GameID == MatchDB.LastMatchId)
                    break;
                else
                    localMatchList.Matches.Add(currentMatchList.Matches[i]);
            }

            currentMatchList = localMatchList;

            fullLoad = currentMatchList.Matches.Count;

            for (int i = 0; i < currentMatchList.Matches.Count; i++)
            {
                new Finder<Match>(ref match, $"https://{region}.api.riotgames.com/lol/match/v3/matches/{currentMatchList.Matches[i].GameID}?{key}");

                currentMatchList.Matches[i] = match;

                Thread.Sleep(1000);
                isLoaded = i + 1;
            }
        }

        private void LoadChampionList()
        {
            new Finder<Champions>(ref currentChampionList, "http://ddragon.leagueoflegends.com/cdn/6.24.1/data/ru_RU/champion.json");
        }
        #endregion
    }
}
