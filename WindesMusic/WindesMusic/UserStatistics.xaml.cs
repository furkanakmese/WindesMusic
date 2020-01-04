using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Separator = LiveCharts.Wpf.Separator;

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
        private List<Label> StatLabels = new List<Label>();
        private List<DateTimePoint> graphPoints = new List<DateTimePoint>();
        private LineSeries mySeries;
        private Grid statGrid;
        private ColumnSeries ColumnSeries;
        private ChartValues<int> dateTimePoints;
        private List<string> Dates;
        private new double Width;
        private new double Height;

        public UserStatistics()
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
                Margin = new Thickness(0, Height * .2, 0, Height * .025 + 30)
            };
            WindowStackPanel.Children.Add(statGrid);


            StatsScrollViewer = new ScrollViewer
            {
                Width = Width * .95,
                Height = Height * .8,
                //Margin = new Thickness(-Width * .975, Height * .2, Width * .025, Height * .025),
                Margin = new Thickness(-Width * .975, 0, Width * .025, 50),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            statGrid.Children.Add(StatsScrollViewer);

            Statistics = new StackPanel();
            StatsScrollViewer.Content = Statistics;


            ColumnSeries = new ColumnSeries
            {
                Values = dateTimePoints,
                DataContext = this,
                Width = Width * .95,
                Height = Height * .80,
                Margin = new Thickness(Width * .025, Height * .025, Width * .025, Height * .025)
            };


            History = new CartesianChart
            {
                DataContext = this,
                Width = Width * .95,
                Height = Height * .80,
                Margin = new Thickness(Width * .025, Height * .025, Width * .025, Height * .025)
            };

            History.AxisX.Add(new Axis
            {
                Labels = Dates
            });

            History.AxisY.Add(new Axis
            {
                Separator = new Separator
                {
                    Step = 1
                }
            });


            History.Series.Clear();
            History.Series.Add(ColumnSeries);
            WindowStackPanel.Children.Add(History);


            foreach (Label label in StatLabels)
            {
                Statistics.Children.Add(label);
            }
        }

        public void GetGraphValues()
        {
            graphPoints = db.getSongsListened();
            dateTimePoints = new ChartValues<int>();
            Dates = new List<string>();

            foreach (DateTimePoint dateTimePoint in graphPoints)
            {
                dateTimePoints.Add((int)dateTimePoint.Value);
                Dates.Add(dateTimePoint.DateTime.ToString().Substring(0,10));
            }

            mySeries = new LineSeries
            {
                Values = dateTimePoints,
                Fill = Brushes.Transparent
            };
        }


        //SELECT COUNT(*), s.Name FROM History h LEFT JOIN Song s on h.SongID=s.SongID WHERE h.UserID = 1 GROUP BY s.Name;
        protected void LoadUserStatistics()
        {
            List<string> result = db.GetUserSongStatistic();

            //song statistics
            StatLabels.Add(new Label { Content = "Song", FontSize = 40, Foreground = new SolidColorBrush(Colors.White) });
            for (int i = 0; i < result.Count; i += 2)
            {
                Label songLabel = new Label
                {
                    Content = $"{result[i]} times listened to {result[i + 1]}.",
                    FontSize = 20,
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
                    Content = $"{result[i]} times listened to song in genre: {result[i + 1]}.",
                    FontSize = 20,
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
                    Content = $"{result[i]} times listened to artist: {result[i + 1]}.",
                    FontSize = 20,
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
                    Content = $"{periodResult[i + 1]} times listened to song from: {periodResult[i]}.",
                    FontSize = 20,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                StatLabels.Add(songLabel);
            }


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
