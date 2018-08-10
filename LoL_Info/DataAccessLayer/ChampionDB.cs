using System;
using System.Collections.Generic;
using System.IO;
using StaticElementsLibrary;

namespace DataAccessLayer
{
    public class ChampionDB
    {
        private static LoLDataBase LoL_DB = new LoLDataBase();
        private static readonly string mainDirectoryName = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\LoL_Info_Data\\StaticData";
        private static string fileName = "";
        
        public static void SaveChampions(ref Champions championList)
        {
            Directory.CreateDirectory(mainDirectoryName);

            foreach (var champion in championList.Data)
            {
                var row = LoL_DB.Champion.NewChampionRow();
                row.Id = champion.Value.Key;
                row.Name = champion.Key;
                row.Tags = champion.Value.Tags.ToArray();

                LoL_DB.Champion.AddChampionRow(row);
            }

            LoL_DB.Champion.AcceptChanges();
            LoL_DB.Champion.WriteXml(fileName);
        }

        public static void LoadChampions(ref Champions championList)
        {
            LoL_DB.Clear();
            LoL_DB.ReadXml(fileName);
            championList.Data = new Dictionary<string, ChampionData>();

            foreach (var champion in LoL_DB.Champion)
                if (!championList.Data.ContainsKey(champion.Name))
                    championList.Data.Add(champion.Name, new ChampionData(champion.Id, champion.Tags));
        }

        public static bool ChampionsExists(string currentRegion)
        {
            LoL_DB = new LoLDataBase();
            fileName = $"{mainDirectoryName}\\champions_DB.txt";

            if (File.Exists(fileName))
                return true;

            return false;
        }
    }
}
