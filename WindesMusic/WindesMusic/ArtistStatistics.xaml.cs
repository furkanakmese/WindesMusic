using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindesMusic
{
    /// <summary>
    /// Interaction logic for UserStatistics.xaml
    /// </summary>
    public partial class ArtistStatistics : Window
    {
        private Database db = new Database();
        private Button ReturnButton;
        private Label WindowName;
        private ScrollViewer StatsScrollViewer;
        private StackPanel Statistics;
        private CartesianChart History;
        private ScrollViewer Graphs;
        private List<Label> StatLabels = new List<Label>();
        private LineSeries mySeries;
        private Grid statGrid;
        private new double Width;
        private new double Height;
        int userID = Properties.Settings.Default.UserID;

        public ArtistStatistics()
        {
            InitializeComponent();

            Width = SystemParameters.PrimaryScreenWidth / 2;
            Height = SystemParameters.PrimaryScreenHeight;

            LoadUserStatistics();
            GetGraphValues();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            //drawgraphs moet voor BuildScreens wanneer gegevens locaal worden bewaard.
            BuildScreens();
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

            statGrid = new Grid
            {
                //Width = Width * .95,
                //Height = Height * .8,
                //Margin = new Thickness(-Width * .975, Height * .2, Width * .025, Height * .025)
                Margin = new Thickness(0, Height * .2, 0, Height * .025)
            };
            WindowStackPanel.Children.Add(statGrid);


            StatsScrollViewer = new ScrollViewer
            {
                Width = Width * .95,
                Height = Height * .8,
                //Margin = new Thickness(-Width * .975, Height * .2, Width * .025, Height * .025),
                Margin = new Thickness(-Width * .975, 0, Width * .025, 0),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            statGrid.Children.Add(StatsScrollViewer);

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


            foreach (Label label in StatLabels)
            {
                Statistics.Children.Add(label);
            }

            History.Series.Add(mySeries);
        }

        public void GetGraphValues()
        {
            mySeries = new LineSeries
            {
                Values = new ChartValues<int> { 12, 23, 55, 100 }
            };
        }

        public void GetArtistSongs(userdID)
        {
            
        }

        //SELECT COUNT(*), s.Name FROM History h LEFT JOIN Song s on h.SongID=s.SongID WHERE h.UserID = 1 GROUP BY s.Name;
        protected void LoadUserStatistics()
        {
            List<string> result = db.GetSongStatistic();

            //song statistics
            StatLabels.Add(new Label { Content = "Song", FontSize = 40, Foreground = new SolidColorBrush(Colors.White) });
            for (int i = 0; i < result.Count; i += 2)
            {
                Label songLabel = new Label
                {
                    Content = $"{result[i]} keer {result[i + 1]} beluisterd.",
                    FontSize = 30,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                StatLabels.Add(songLabel);
            }

            //genre statistics
            result = db.GetGenreStatistic();
            StatLabels.Add(new Label { Content = "Genre", FontSize = 40, Foreground = new SolidColorBrush(Colors.White) });
            for (int i = 0; i < result.Count; i += 2)
            {
                Label songLabel = new Label
                {
                    Content = $"{result[i]} keer een nummer in genre: {result[i + 1]} beluisterd.",
                    FontSize = 30,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                StatLabels.Add(songLabel);
            }


            //artist statistics
            result = db.GetArtistStatistic();
            StatLabels.Add(new Label { Content = "Artist", FontSize = 40, Foreground = new SolidColorBrush(Colors.White) });
            for (int i = 0; i < result.Count; i += 2)
            {
                Label songLabel = new Label
                {
                    Content = $"{result[i]} keer een nummer van artiest: {result[i + 1]} beluisterd.",
                    FontSize = 30,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                StatLabels.Add(songLabel);
            }

            List<int> periodResult = db.GetPeriodStatistic();
            StatLabels.Add(new Label { Content = "Period", FontSize = 40, Foreground = new SolidColorBrush(Colors.White) });
            for (int i = 0; i < periodResult.Count; i += 2)
            {
                Label songLabel = new Label
                {
                    Content = $"{periodResult[i + 1]} keer een nummer uit jaar: {periodResult[i]} beluisterd.",
                    FontSize = 30,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                StatLabels.Add(songLabel);
            }


            //hoevaak alle nummers in een periode beluisterd door gebruiker.
            //SELECT COUNT(*), s.Year FROM History h LEFT JOIN Song s on h.SongID=s.SongID WHERE h.UserID = 1 GROUP BY s.Year;

            //user = db.GetUserData(Properties.Settings.Default.UserID);

        }

        private void ReturnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            Width = this.DesiredSize.Width / 2;
            Height = this.DesiredSize.Height;
            BuildScreens();
        }
    }
}
