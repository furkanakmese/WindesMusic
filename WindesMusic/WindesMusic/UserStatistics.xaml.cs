using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for UserStatistics.xaml
    /// </summary>
    public partial class UserStatistics : Window
    {
        private Database db = new Database();
        private Button ReturnButton;
        private Label WindowName;
        private ScrollViewer StatsScrollViewer;
        private StackPanel Statistics;
        private CartesianChart History;
        private ScrollViewer Graphs;
        private List<Label> StatLabels = new List<Label>();
        private double Width;
        private double Height;

        public UserStatistics()
        {
            InitializeComponent();

            Width = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            LoadUserStatistics();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            //drawgraphs moet voor BuildScreens wanneer gegevens locaal wordten bewaart.
            BuildScreens();
            DrawGraphs();
        }

        private void BuildScreens()
        {
            WindowStackPanel.Children.Clear();
            Statistics?.Children.Clear();

            ReturnButton = new Button
            {
                Height = Height * .1,
                Width = Width * .15,
                Margin = new Thickness(Width * .025, Height * .025, Width * .025, Height * .8),
                Content = "Return",
                FontSize = 20
            };
            ReturnButton.Click += new RoutedEventHandler(ReturnClick);
            WindowStackPanel.Children.Add(ReturnButton);

            WindowName = new Label
            {
                Height = Height * .1,
                Width = Width * .75,
                Margin = new Thickness(Width * .025, Height * .025, Width * .025, Height * .8),
                Content = "User statistics",
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontSize = 30
            };
            WindowStackPanel.Children.Add(WindowName);

            StatsScrollViewer = new ScrollViewer
            {
                Width = Width * .95,
                Height = Height * .8,
                Margin = new Thickness(-Width * .975, Height * .2, Width * .025, Height * .025)
            };
            WindowStackPanel.Children.Add(StatsScrollViewer);

            Statistics = new StackPanel();
            StatsScrollViewer.Content = Statistics;

            Graphs = new ScrollViewer
            {
                Width = Width * .95,
                Height = Height * .95,
                Margin = new Thickness(Width * .025, Height * .025, Width * .025, Height * .025)
            };
            WindowStackPanel.Children.Add(Graphs);

            History = new CartesianChart();
            Graphs.Content = History;


            foreach(Label label in StatLabels)
            {
                Statistics.Children.Add(label);
            }
        }

        public void DrawGraphs()
        {
            LineSeries mySeries = new LineSeries
            {
                Values = new ChartValues<int> { 12, 23, 55, 100 }
            };

            History.Series.Add(mySeries);
        }

        protected void LoadUserStatistics()
        {
            List<string> result = db.GetSongStatistic();

            for (int i = 0; i < result.Count; i+=2)
            {
                Label songLabel = new Label
                {
                    Content = $"{result[i]} keer {result[i+1]} beluisterd.",
                    FontSize = 30,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                StatLabels.Add(songLabel);
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

        private void ReturnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            Width =  this.DesiredSize.Width;
            Height = this.DesiredSize.Height;
            BuildScreens();
        }
    }
}
