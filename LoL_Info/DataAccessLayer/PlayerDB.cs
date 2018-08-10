using System;
using System.Collections.Generic;
using System.IO;
using StaticElementsLibrary;

namespace DataAccessLayer
{
    public class PlayerDB
    {
        private static LoLDataBase LoL_DB = new LoLDataBase();
        private static readonly string mainDirectoryName = string.Format("{0}\\LoL_Info_Data\\Players",
              System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData));
        private static string fileName = "";
        private static string region = "ru";

        public static void SavePlayer(ref Player currentPlayer, string playerName)
        {
            Directory.CreateDirectory($"{mainDirectoryName}\\{region}\\{playerName}");

            var row = LoL_DB.Player.NewPlayerRow();
            row.IconId = currentPlayer.ProfileIconID;
            row.AccountId = currentPlayer.AccountID;
            row.SummonerId = currentPlayer.ID;
            row.Name = currentPlayer.Name;

            LoL_DB.Player.AddPlayerRow(row);
            LoL_DB.Player.AcceptChanges();
            LoL_DB.Player.WriteXml(fileName);
        }

        public static void LoadPlayer(ref Player currentPlayer, string playerName)
        {
            foreach (var player in LoL_DB.Player)
            {
                if (player.Name == playerName)
                {
                    currentPlayer.ProfileIconID = player.IconId;
                    currentPlayer.AccountID = player.AccountId;
                    currentPlayer.ID = player.SummonerId;
                    currentPlayer.Name = player.Name;
                }
            }
        }

        public static bool PlayerExists(string playerName, string currentRegion)
        {
            LoL_DB = new LoLDataBase();
            region = currentRegion;
            fileName = $"{mainDirectoryName}\\{region}\\players_DB.txt";

            if (File.Exists(fileName))
            {
                LoL_DB.Clear();
                LoL_DB.ReadXml(fileName);

                foreach (var player in LoL_DB.Player)
                {
                    if (player.Name == playerName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static List<string> PlayersLoad()
        {
            var playerNames = new List<string>();

            LoL_DB.Clear();
            LoL_DB.ReadXml(fileName);

            foreach (var row in LoL_DB.Player)
            {
                if (!playerNames.Contains(row.Name))
                    playerNames.Add(row.Name);
            }

            return playerNames;
        }
    }
}
