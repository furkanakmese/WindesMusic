using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using WindesMusic.Properties;

namespace WindesMusic
{
    public partial class ArtistStatistics : Window
    {
        private readonly Database db = new Database();
        private History history = new History();
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public ArtistStatistics()
        {
            InitializeComponent();
            for (var i = 0; i < db.GetArtistSong(Settings.Default.UserID).Count; i++)
                boxSongs.Items.Add(db.GetArtistSong(Settings.Default.UserID).ElementAtOrDefault(i)?.SongName);
        }

        private void ReturnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowDataClick(object sender, RoutedEventArgs e)
        { 
             history = db.GetArtistSongData((string) boxSongs.SelectionBoxItem);
            lblSongName.Text = "Nummer: " + history.SongName;
            lblTotalTimesListened.Text = "Aantal keer beluisterd: " + history.TotalTimesListened;
            lblUniqueListeners.Text = "Unieke luisteraars: " + history.UniqueListeners;
            var totalTimeHour = history.TotalTimeListened / 60 / 60;
            lblTotalTimeListened.Text = "Aantal uren beluisterd: " + totalTimeHour;
            lblTotalPaidListened.Text = "Betaald beluisterd: " + history.TotalPaidListened;
            lblListenedMonth.Text = "Afgelopen maand: " + history.ListenedMonth;
            lblListenedHalfMonth.Text = "Afgelopen twee weken: " + history.ListenedHalfMonth;
            lblListenedWeek.Text = "Afgelopen week: " + history.ListenedWeek;

            var dayOne = Convert.ToDouble(history.ListenedDayOne);
            var dayTwo = Convert.ToDouble(history.ListenedDayTwo);
            var dayThree = Convert.ToDouble(history.ListenedDayThree);
            var dayFour = Convert.ToDouble(history.ListenedDayFour);
            var dayFive = Convert.ToDouble(history.ListenedDayFive);
            var daySix = Convert.ToDouble(history.ListenedDaySix);
            var daySeven = Convert.ToDouble(history.ListenedDaySeven);
           

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 0",
                    Values = new ChartValues<double>
                    {dayOne,dayTwo,dayThree,dayFour,dayFive,daySix,daySeven },
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 5
                }
            };
            Labels = new[] {"Day one", "Day two", "Day three", "Day four", "Day Five", "Day Six", "Day Seven"};
            YFormatter = value => value.ToString("C");

            DataContext = this;
        }
    }

    public class History
    {
        public int SongID { get; set; }
        public string SongName { get; set; }
        public int UniqueListeners { get; set; }
        public int TotalTimeListened { get; set; }
        public int TotalTimesListened { get; set; }
        public int ListenedDayOne { get; set; }
        public int ListenedDayTwo { get; set; }
        public int ListenedDayThree { get; set; }
        public int ListenedDayFour { get; set; }
        public int ListenedDayFive { get; set; }
        public int ListenedDaySix { get; set; }
        public int ListenedDaySeven { get; set; }
        public int ListenedMonth { get; set; }
        public int ListenedWeek { get; set; }
        public int ListenedHalfMonth { get; set; }
        public int TotalPaidListened { get; set; }
    }
}