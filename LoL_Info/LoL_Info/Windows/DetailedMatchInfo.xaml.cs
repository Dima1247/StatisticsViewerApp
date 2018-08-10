using System;
using System.Windows;
using StaticElementsLibrary;
using System.Windows.Media.Imaging;
using System.Threading;

namespace LoL_Info.Windows
{
    public partial class DetailedMatchInfo : Window
    {
        public DetailedMatchInfo(MatchDetails match)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Culture);

            InitializeComponent();
            titleBar.Children.Add(new TitleBar(this, $"{Properties.Resources.Match}", false));
            AddPlayers(match);

            Left = MainWindow.MainWindowPosition.X + 15;
            Top = MainWindow.MainWindowPosition.Y + 99;
        }

        private void AddPlayers(MatchDetails match)
        {
            for (int i = 0; i < match.ParticipantID.Count; i++)
            {
                string champName = "";

                foreach (var champ in CurrentUserElements.Champions.Data)
                    if (champ.Value.Key == match.ChampionID[i])
                        champName = champ.Key;
                
                string path = $"http://ddragon.leagueoflegends.com/cdn/6.24.1/img/champion/{champName}.png";
                string kda = $"{match.Kills[i]}/{match.Deaths[i]}/{match.Assists[i]}";
                string position = Properties.Resources.Loser;

                string[] items = new string[6]
                {
                    string.Format("http://ddragon.leagueoflegends.com/cdn/6.24.1/img/item/{0}.png", match.Item1[i]),
                    string.Format("http://ddragon.leagueoflegends.com/cdn/6.24.1/img/item/{0}.png", match.Item2[i]),
                    string.Format("http://ddragon.leagueoflegends.com/cdn/6.24.1/img/item/{0}.png", match.Item3[i]),
                    string.Format("http://ddragon.leagueoflegends.com/cdn/6.24.1/img/item/{0}.png", match.Item4[i]),
                    string.Format("http://ddragon.leagueoflegends.com/cdn/6.24.1/img/item/{0}.png", match.Item5[i]),
                    string.Format("http://ddragon.leagueoflegends.com/cdn/6.24.1/img/item/{0}.png", match.Item6[i])
                };
                
                if (match.Win[i])
                    position = Properties.Resources.Winner;

                table.Items.Add(new InfoTable(match.ChampLevel[i], path, match.SumName[i], position, kda, $"{match.Farm[i]} {Properties.Resources.Minions}", items[0], items[1], items[2], items[3], items[4], items[5]));
            }
        }

        private void table_BeginningEdit(object sender, System.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
