using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LoL_Info.Properties;

namespace LoL_Info.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private MainWindow window;
        private string defCulture, defRegion;

        public SettingsWindow(MainWindow window)
        {
            InitializeComponent();

            this.window = window;

            Left = MainWindow.MainWindowPosition.X + 15;
            Top = MainWindow.MainWindowPosition.Y + 99;

            titleBar.Children.Add(new TitleBar(this, Properties.Resources.SettingsField));
            txtKey.Text = Settings.Default.ApiKey;
            regionBox.Text = Settings.Default.RegionFullName;
            cultureBox.Text = Settings.Default.CultureFullName;
            defCulture = Settings.Default.Culture;
            defRegion = Settings.Default.Region;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (cultureBox.SelectedIndex)
            {
                case 0:
                    Settings.Default.Culture = "en-US";
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Default.Culture);
                    Settings.Default.CultureFullName = Properties.Resources.Eng;
                    break;
                case 1:
                    Settings.Default.Culture = "ru-RU";
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Default.Culture);
                    Settings.Default.CultureFullName = Properties.Resources.Rus;
                    break;
                case 2:
                    Settings.Default.Culture = "uk-UA";
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Default.Culture);
                    Settings.Default.CultureFullName = Properties.Resources.Ukr;
                    break;
            }

            switch (regionBox.SelectedIndex)
            {
                case 0: Settings.Default.Region = "br1"; Settings.Default.RegionFullName = Properties.Resources.BrRegion; break;
                case 1: Settings.Default.Region = "eun1"; Settings.Default.RegionFullName = Properties.Resources.EuneRegion; break;
                case 2: Settings.Default.Region = "euw1"; Settings.Default.RegionFullName = Properties.Resources.EuwRegion; break;
                case 3: Settings.Default.Region = "jp1"; Settings.Default.RegionFullName = Properties.Resources.JpRegion; break;
                case 4: Settings.Default.Region = "kr"; Settings.Default.RegionFullName = Properties.Resources.KrRegion; break;
                case 5: Settings.Default.Region = "la1"; Settings.Default.RegionFullName = Properties.Resources.LanRegion; break;
                case 6: Settings.Default.Region = "la2"; Settings.Default.RegionFullName = Properties.Resources.LasRegion; break;
                case 7: Settings.Default.Region = "na1"; Settings.Default.RegionFullName = Properties.Resources.NaRegion; break;
                case 8: Settings.Default.Region = "oc1"; Settings.Default.RegionFullName = Properties.Resources.OceRegion; break;
                case 9: Settings.Default.Region = "tr1"; Settings.Default.RegionFullName = Properties.Resources.TrRegion; break;
                case 10: Settings.Default.Region = "ru"; Settings.Default.RegionFullName = Properties.Resources.RuRegion; break;
            }
            
            Settings.Default.ApiKey = txtKey.Text;
            Settings.Default.Save();
            Hide();
            Close();
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (defCulture != Properties.Settings.Default.Culture || defRegion != Properties.Settings.Default.Region)
            {
                window.Hide();
                window.Close();
                window = new MainWindow();
                window.ShowDialog();
            }
        }
    }
}
