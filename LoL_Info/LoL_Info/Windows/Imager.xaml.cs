using LoL_Info.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LoL_Info.Windows
{
    /// <summary>
    /// Interaction logic for Imager.xaml
    /// </summary>
    public partial class Imager : UserControl
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private List<Bitmap> pictureList = new List<Bitmap>()
        {
            Properties.Resources.Picture1,
            Properties.Resources.Picture2,
            Properties.Resources.Picture3,
            Properties.Resources.Picture4,
            Properties.Resources.Picture5,
            Properties.Resources.Picture6,
            Properties.Resources.Picture7,
            Properties.Resources.Picture8,
            Properties.Resources.Picture9,
            Properties.Resources.Picture10,
            Properties.Resources.Picture11,
            Properties.Resources.Picture12
        };
        private List<ImageSource> imageSources = new List<ImageSource>();

        private int index = 1;

        public Imager()
        {
            InitializeComponent();
            string region = "";

            switch (Properties.Settings.Default.Region)
            {
                case "br1": region = Properties.Resources.BrRegion; break;
                case "eun1": region = Properties.Resources.EuneRegion; break;
                case "euw1": region = Properties.Resources.EuwRegion; break;
                case "jp1": region = Properties.Resources.JpRegion; break;
                case "kr": region = Properties.Resources.KrRegion; break;
                case "la1": region = Properties.Resources.LanRegion; break;
                case "la2": region = Properties.Resources.LasRegion; break;
                case "na1": region = Properties.Resources.NaRegion; break;
                case "oc1": region = Properties.Resources.OceRegion; break;
                case "tr1": region = Properties.Resources.TrRegion; break;
                case "ru": region = Properties.Resources.RuRegion; break;
            }
            lbRegion.Content = $"{Properties.Resources.RegionField} {region}";

            for (int i = 0; i < 12; i++)
                imageSources.Add(Imaging.CreateBitmapSourceFromHBitmap(pictureList[i].GetHbitmap(),
              IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 5, 0);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var brush = new ImageBrush(imageSources[index]);

            images.Source = brush.ImageSource;
            index++;

            if (index == 12)
                index = 0;
        }
    }
}
