using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
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
using System.Windows.Threading;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AudioPlayer AudioPlyer = new AudioPlayer();
        private DispatcherTimer dispatcherTimer;
        public MainWindow()
        {
            InitializeComponent();

            //  DispatcherTimer setup
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(UpdateSongSlider);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        //stop button, executes stop function(OnPlayBackStopped).
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AudioPlyer.OnButtonStopClick(sender, e);
        }

        //start, and pause and resume button.
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AudioPlyer.OnButtonPlayClick(sender, e);
        }

        //volume slider
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AudioPlyer.SetVolume((float)e.NewValue / 100);
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (e.HorizontalChange >= PlaceInSongSlider.Maximum)
            {
                AudioPlyer.SetCurrentPlaceInSong(PlaceInSongSlider.Maximum);
            }
            else
            {
                AudioPlyer.SetCurrentPlaceInSong(e.HorizontalChange);
            }
        }

        private void UpdateSongSlider(object sender, EventArgs e)
        {
            PlaceInSongSlider.Value = AudioPlyer.CurrentPlaceInSong();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AudioPlyer.Mute();
        }
    }
}