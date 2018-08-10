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
using System.Windows.Threading;

namespace LoL_Info.Windows
{
    public partial class MessageBoxControl : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();

        private int time = 0;

        public MessageBoxControl(string text, bool isInfoMessage = true)
        {
            InitializeComponent();
            messageText.Text = text;

            Left = MainWindow.MainWindowPosition.X + 15;
            Top = MainWindow.MainWindowPosition.Y + 99;

            if (isInfoMessage)
            {
                buttonsPanel.Height = 0;
                buttonsPanel.Width = 0;
                buttonsPanelBorder.Visibility = Visibility.Hidden;

                timer.Tick += Timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 3);
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs ex)
        {
            var opacityTimer = new DispatcherTimer();

            opacityTimer.Tick += (object s, EventArgs e) =>
                {
                    Opacity -= 0.1;
                    time++;
                    if (time == 10)
                    {
                        Hide();
                        Close();
                    }
                };

            opacityTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            opacityTimer.Start();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
