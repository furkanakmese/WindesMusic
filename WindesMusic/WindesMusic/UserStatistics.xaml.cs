using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private Button returnButton;
        private Label windowName;
        private ScrollViewer statsScrollViewer;
        private StackPanel statistics;
        private CartesianChart history;
        private List<Label> statLabels = new List<Label>();
        private List<DateTimePoint> graphPoints = new List<DateTimePoint>();
        private Grid statGrid;
        private ColumnSeries columnSeries;
        private ChartValues<int> dateTimePoints;
        private List<string> dates;
        private double width;
        private double height;

        public UserStatistics()
        {
            InitializeComponent();

            width = SystemParameters.PrimaryScreenWidth / 2;
            height = SystemParameters.PrimaryScreenHeight;

            //gets userdata from database.
            LoadUserStatistics();
            GetGraphValues();

            this.SizeToContent = SizeToContent.WidthAndHeight;

            //builds screen with user data
            BuildScreens();
        }

        private void BuildScreens()
        {
            //empties previous data
            WindowStackPanel.Children.Clear();
            statistics?.Children.Clear();

            //adds return button
            returnButton = new Button
            {
                Height = height * .1,
                Width = width * .15,
                Margin = new Thickness(width * .025, height * .025, width * .025, height * .8),
                Content = "Return",
                FontSize = 20
            };
            returnButton.Click += new RoutedEventHandler(ReturnClick);
            WindowStackPanel.Children.Add(returnButton);

            //adds window name
            windowName = new Label
            {
                Height = height * .1,
                Width = width * .75,
                Margin = new Thickness(width * .025, height * .025, width * .025, height * .8),
                Content = "User statistics",
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                FontSize = 30
            };
            WindowStackPanel.Children.Add(windowName);

            //adds grid for user statistics in plain text. 
            statGrid = new Grid
            {
                //Width = Width * .95,
                //Height = Height * .8,
                //Margin = new Thickness(-Width * .975, Height * .2, Width * .025, Height * .025)
                Margin = new Thickness(0, height * .2, 0, height * .025 + 30)
            };
            WindowStackPanel.Children.Add(statGrid);

            //adds scrollviewer to grid for user data.
            statsScrollViewer = new ScrollViewer
            {
                Width = width * .95,
                Height = height * .8,
                //Margin = new Thickness(-Width * .975, Height * .2, Width * .025, Height * .025),
                Margin = new Thickness(-width * .975, 0, width * .025, 50),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            statGrid.Children.Add(statsScrollViewer);

            //statistics has all statlabels
            statistics = new StackPanel();
            statsScrollViewer.Content = statistics;

            //columnSeries is used to display a graph.
            columnSeries = new ColumnSeries
            {
                Values = dateTimePoints,
                DataContext = this,
                Width = width * .95,
                Height = height * .80,
                Margin = new Thickness(width * .025, height * .025, width * .025, height * .025)
            };

            //CartesianChart is used to display data.
            history = new CartesianChart
            {
                DataContext = this,
                Width = width * .95,
                Height = height * .80,
                Margin = new Thickness(width * .025, height * .025, width * .025, height * .025)
            };

            //adds labels to x-axis.
            history.AxisX.Add(new Axis
            {
                Labels = dates
            });

            //adds labels to y-axis.
            history.AxisY.Add(new Axis
            {
                Separator = new Separator
                {
                    Step = 1
                }
            });

            //empties previous chart values.
            history.Series.Clear();

            //adds new values and displays new values.
            history.Series.Add(columnSeries);
            WindowStackPanel.Children.Add(history);

            //adds plaintext user data to stackpanel. 
            foreach (Label label in statLabels)
            {
                statistics.Children.Add(label);
            }
        }

        //gets userdata for graphvalues.
        public void GetGraphValues()
        {
            List<DateTimePoint> dbResult = db.getSongsListened();
            dateTimePoints = new ChartValues<int>();
            dates = new List<string>();
            DateTime today = DateTime.Today;

            //adds dates on which no music was listened with # of songs listened = 0.
            bool isMissing;
            for(int i = 7; i >= 0; i--)
            {
                isMissing = true;
                foreach(DateTimePoint dateTimePoint in dbResult)
                {
                    if (dateTimePoint.DateTime.Date == today.Date.AddDays(-i))
                    {
                        isMissing = false;
                        break;
                    }
                }
                if(isMissing)
                {
                    dbResult.Add(new DateTimePoint(today.AddDays(-i), 0));
                }
                
            }

            //creates a list of all datetimepoints.
            for (int i = 7; i >= 0; i--)
            {
                foreach (DateTimePoint dateTimePoint in dbResult)
                {
                    if (dateTimePoint.DateTime.Date == today.Date.AddDays(-i))
                    {
                        graphPoints.Add(dateTimePoint);
                    }
                }
            }

            //adds all datetimepoints to chartvalues.
            foreach (DateTimePoint dateTimePoint in graphPoints)
            {
                dateTimePoints.Add((int)dateTimePoint.Value);
                dates.Add(dateTimePoint.DateTime.ToString("d", CultureInfo.CreateSpecificCulture("nl-NL")));
            }
        }


        //gets user statistics.
        protected void LoadUserStatistics()
        {
            List<string> result = db.GetUserSongStatistic();

            //gets song statistics from database and creates labels.
            statLabels.Add(new Label { Content = "Song", FontSize = 40, Foreground = new SolidColorBrush(Colors.White) });
            for (int i = 0; i < result.Count; i += 2)
            {
                Label songLabel = new Label
                {
                    Content = $"{result[i]} times listened to {result[i + 1]}.",
                    FontSize = 20,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                statLabels.Add(songLabel);
            }

            //gets genre statistics from database and creates labels.
            result = db.GetGenreStatistic();
            statLabels.Add(new Label { Content = "Genre", FontSize = 40, Foreground = new SolidColorBrush(Colors.White) });
            for (int i = 0; i < result.Count; i += 2)
            {
                Label songLabel = new Label
                {
                    Content = $"{result[i]} times listened to song in genre: {result[i + 1]}.",
                    FontSize = 20,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                statLabels.Add(songLabel);
            }


            //gets artist statistics from database and creates labels.
            result = db.GetArtistStatistic();
            statLabels.Add(new Label { Content = "Artist", FontSize = 40, Foreground = new SolidColorBrush(Colors.White) });
            for (int i = 0; i < result.Count; i += 2)
            {
                Label songLabel = new Label
                {
                    Content = $"{result[i]} times listened to artist: {result[i + 1]}.",
                    FontSize = 20,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                statLabels.Add(songLabel);
            }

            //gets period statistics from database and creates labels.
            List<int> periodResult = db.GetPeriodStatistic();
            statLabels.Add(new Label { Content = "Period", FontSize = 40, Foreground = new SolidColorBrush(Colors.White) });
            for (int i = 0; i < periodResult.Count; i += 2)
            {
                Label songLabel = new Label
                {
                    Content = $"{periodResult[i + 1]} times listened to song from: {periodResult[i]}.",
                    FontSize = 20,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                statLabels.Add(songLabel);
            }


        }

        private void ReturnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
        private void WindowOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            width = this.DesiredSize.Width / 2;
            height = this.DesiredSize.Height;
            BuildScreens();
        }
    }
}
