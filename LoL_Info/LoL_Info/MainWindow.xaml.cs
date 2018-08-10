using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using LoL_Info.Classes;
using LoL_Info.Windows;
using StaticElementsLibrary;
using System.Windows.Interop;
using System.Windows.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;
using log4net;
using log4net.Config;
using System.Globalization;

namespace LoL_Info
{
    public partial class MainWindow : Window
    {
        private static bool isEntered = false;
        private static Point currentPosition;
        private static TitleBar titleBarControl;

        public static Point MainWindowPosition { get { return currentPosition; } }
        public ProgressBar MainProgressBar { get { return progressBar; } }

        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.Culture);
            InitializeComponent();

            BasicConfigurator.Configure();

            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            currentPosition = new Point(Left, Top);
            Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}//LoL_Info_Data");
            txtName.Text = Properties.Settings.Default.PlayerName;
            titleBarControl = new TitleBar(this, "LoL info", true, new SettingsDelegate(() => { return new SettingsWindow(this); }));
            titleBar.Children.Add(titleBarControl);
            Windows.Menu.mode = Properties.Resources.Team;
            InfoFrameLoad(3);
        }

        public void PLayersDataError()
        {
            ThreadManager.WriteMessage(Properties.Resources.LogProblemMessage);
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (!isEntered)
            {
                Properties.Settings.Default.PlayerName = txtName.Text;
                Properties.Settings.Default.Save();
                ThreadManager.WriteMessage(Properties.Resources.DownloadingMessage);
                new ProgressBarUpdater(txtName.Text, progressBar, (object obj) => new LoadFromAPI(obj as string).LoadInfo(this));
            }
            else
            {
                isEntered = false;
                txtName.IsEnabled = true;
                btnEnter.Content = Properties.Resources.LogIn;
                ChangeStatus(Properties.Resources.SbReady);
                InfoFrameLoad(3);
                CurrentUserElements.MakeNulls();
                Windows.Menu.mode = Properties.Resources.Team;
                titleBarControl.ShowSettingsButton();
            }
        }

        public void InfoFrameLoad(int frameNum)
        {
            infoPanel.Children.Clear();

            switch (frameNum)
            {
                case 0: infoPanel.Children.Add(new MainPlayerInfo()); break;
                case 1: infoPanel.Children.Add(new Windows.Menu(this)); break;
                case 2: infoPanel.Children.Add(new Charts()); break;
                case 3: infoPanel.Children.Add(new Imager()); break;
            }
        }

        private void progressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (progressBar.Value == 0)
            {
                txtName.IsEnabled = true;

                btnEnter.IsEnabled = true;
                btnTable.IsEnabled = true;
            }

            if (progressBar.Value > 0)
            {
                txtName.IsEnabled = false;
                btnEnter.IsEnabled = false;
                if (!isEntered)
                    btnTable.IsEnabled = false;

                titleBarControl.HideSettingsButton();
                ChangeStatus($"{Properties.Resources.SbDownload} ({progressBar.Value}%)");
            }

            if (progressBar.Value == 100)
            {
                progressBar.Value = 0;
                btnEnter.IsEnabled = true;

                if (LoadFromAPI.FullLoad > 0)
                {
                    InfoFrameLoad(0);
                    ThreadManager.WriteMessage(Properties.Resources.LoadedMessage);

                    txtName.IsEnabled = false; isEntered = true;
                    btnEnter.Content = Properties.Resources.LogOut;
                    ChangeStatus(Properties.Resources.SbStats);
                }
                else
                {
                    titleBarControl.ShowSettingsButton();
                    ChangeStatus(Properties.Resources.SbReady);
                }
            }
        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            if (isEntered)
            {
                InfoFrameLoad(1);
                ChangeStatus(Properties.Resources.SbMenu);
            }
            else
                ThreadManager.WriteMessage($"{Properties.Resources.MenuFailedMessage}\n{Properties.Resources.NameFailedMessage}");
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            currentPosition.X = Left;
            currentPosition.Y = Top;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            titleBarControl.CheckImages();
            if (WindowState == WindowState.Maximized)
            {
                currentPosition.X = 0;
                currentPosition.Y = 0;
            }
            else if (WindowState == WindowState.Normal)
            {
                currentPosition.X = Left;
                currentPosition.Y = Top;
            }
        }

        public void ChangeStatus(string text)
        {
            statusText.Content = text;
        }

        public bool AskAboutUpdate()
        {
            return new MessageBoxControl($"{Properties.Resources.TryLoadNewDataMessage}", false).ShowDialog().Value;
        }
    }
}