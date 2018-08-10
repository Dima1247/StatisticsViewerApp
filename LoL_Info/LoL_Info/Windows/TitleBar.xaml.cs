using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoL_Info.Windows
{
    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>

    public delegate Window SettingsDelegate();

    public partial class TitleBar : UserControl
    {
        private SettingsDelegate SettingsCreation;
        private Window window, settingsWindow;

        public TitleBar(Window mainWindow, string windowName, bool isMaximizeAble = false, SettingsDelegate settingsDelegate = null)
        {
            InitializeComponent();

            window = mainWindow;

            if (!isMaximizeAble)
                btnSize.Visibility = Visibility.Hidden;

            if (settingsDelegate == null)
                btnSettings.Visibility = Visibility.Hidden;

            SettingsCreation = settingsDelegate;
            WindowName.Content = windowName;
        }

        #region TitleEvents
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                window.DragMove();
        }

        private void TitleBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChangeSize();
        }
        #endregion

        #region ButtonsEvents
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            settingsWindow = SettingsCreation.Invoke();
            settingsWindow.ShowDialog();
        }

        private void btnSize_Click(object sender, RoutedEventArgs e)
        {
            ChangeSize();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            settingsWindow = null;

            window.Hide();
            window.Close();
        }
        #endregion

        #region PublicMethods
        public void HideSettingsButton()
        {
            btnSettings.Visibility = Visibility.Hidden;
        }

        public void ShowSettingsButton()
        {
            btnSettings.Visibility = Visibility.Visible;
        }

        public void ChangeSize()
        {
            if (window.WindowState == WindowState.Maximized)
                window.WindowState = WindowState.Normal;
            else if (window.WindowState == WindowState.Normal)
                window.WindowState = WindowState.Maximized;
        }

        public void CheckImages()
        {
            if (window.WindowState == WindowState.Normal)
            {
                imgToMax.Visibility = Visibility.Visible;
                imgToMin.Visibility = Visibility.Hidden;
            }
            else if (window.WindowState == WindowState.Maximized)
            {
                imgToMax.Visibility = Visibility.Hidden;
                imgToMin.Visibility = Visibility.Visible;
            }
        }
        #endregion
    }
}
