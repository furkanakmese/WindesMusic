using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Controls;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for UserStatistics.xaml
    /// </summary>
    public partial class UserStatistics : Page
    {
        private Database db = new Database();

        public UserStatistics()
        {
            InitializeComponent();
            DrawGraphs();
        }

        public void DrawGraphs()
        {
            LineSeries mySeries = new LineSeries
            {
                Values = new ChartValues<int> { 12, 23, 55, 100 }
            };

            myChart.Series.Add(mySeries);
        }

        protected void LoadUserStatistics()
        {
            Statistics.Children.Clear();
            string[,] result = db.GetSongStatistic();

            for (int i = 0; i < result.GetLength(0); i++)
            {
                Label songLabel = new Label();
                songLabel.Content = $"{result[i, 0]} {result[i, 1]}";
                Statistics.Children.Add(songLabel);
            }
            //hoevaak alle nummers beluisterd door gebruiker.
            //SELECT COUNT(*), s.Name FROM History h LEFT JOIN Song s on h.SongID=s.SongID WHERE h.UserID = 1 GROUP BY s.Name;

            //hoevaak alle artiesten beluisterd door gebruiker
            //SELECT COUNT(*), s.Artist FROM History h LEFT JOIN Song s on h.SongID = s.SongID WHERE h.UserID = 1 GROUP BY s.Artist;

            //hoevaak alle genres beluisterd door gebruiker
            //SELECT COUNT(*), s.Genre FROM History h LEFT JOIN Song s on h.SongID=s.SongID WHERE h.UserID = 1 GROUP BY s.Genre;

            //hoevaak alle nummers in een periode beluisterd door gebruiker.
            //SELECT COUNT(*), s.Year FROM History h LEFT JOIN Song s on h.SongID=s.SongID WHERE h.UserID = 1 GROUP BY s.Year;

            //user = db.GetUserData(Properties.Settings.Default.UserID);

            //playlistSongs.rerender += (playlist) => { playlistSongs.playlistToUse = playlist; playlistSongs.reinitialize(playlist, this, user); };
            //queuePage.rerender += (queuePg) => { queuePage = queuePg; queuePage.InitialiseQueuePage(); };
            //Thickness thickness = new Thickness(15, 0, 0, 5);
            //foreach (var item in user.Playlists)
            //{
            //    var PlaylistButton = new Button
            //    {
            //        //Style = StaticResource MenuButton,
            //        Name = $"_{item.PlaylistID}",
            //        Content = $"{item.PlaylistName}",
            //        FontSize = 23,
            //        Margin = thickness
            //    };
            //    StaticResourceExtension menuButton = new StaticResourceExtension("MenuButton");
            //    PlaylistButton.Style = (Style)FindResource("MenuButton");
            //    PlaylistButton.Click += ButtonClickPlaylist;
            //    PlaylistList.Children.Add(PlaylistButton);
        }
    }
}
