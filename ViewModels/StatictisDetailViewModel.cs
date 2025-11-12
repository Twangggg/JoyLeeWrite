using JoyLeeWrite.Commands;
using JoyLeeWrite.Services;
using JoyLeeWrite.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JoyLeeWrite.ViewModels
{
    public class ChartDataPoint
    {
        public string Day { get; set; }
        public int WordCount { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class GridLine
    {
        public double Y { get; set; }
        public string Label { get; set; }
        public double LabelY { get; set; }
    }

    public class StatictisDetailViewModel : INotifyPropertyChanged
    {
        private const double CHART_HEIGHT = 360;
        private const double CHART_WIDTH = 980;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<ChartDataPoint> _dataPoints;
        private ChartDataPoint _selectedPoint;

        public ObservableCollection<ChartDataPoint> DataPoints
        {
            get => _dataPoints;
            set
            {
                _dataPoints = value;
                OnPropertyChanged(nameof(DataPoints));
                OnPropertyChanged(nameof(PolylinePoints));
            }
        }

        public ChartDataPoint SelectedPoint
        {
            get => _selectedPoint;
            set
            {
                _selectedPoint = value;
                OnPropertyChanged(nameof(SelectedPoint));
                OnPropertyChanged(nameof(IsTooltipVisible));
                OnPropertyChanged(nameof(TooltipX));
                OnPropertyChanged(nameof(TooltipY));
            }
        }

        private ObservableCollection<GridLine> _gridLines;
        public ObservableCollection<GridLine> GridLines
        {
            get => _gridLines;
            set
            {
                _gridLines = value;
                OnPropertyChanged(nameof(GridLines));
            }
        }

        public string PolylinePoints
        {
            get
            {
                if (DataPoints == null || !DataPoints.Any())
                    return string.Empty;

                return string.Join(" ", DataPoints.Select(p => $"{p.X},{p.Y}"));
            }
        }

        public Visibility IsTooltipVisible => SelectedPoint != null ? Visibility.Visible : Visibility.Collapsed;

        public double TooltipX => SelectedPoint?.X - 30 ?? 0;
        public double TooltipY => SelectedPoint?.Y - 60 ?? 0;

        public ICommand SelectPointCommand { get; }

        private void LoadData()
        {
            var rawData = writingStatisticsService.GetLast7DaysWordStats(MainWindow.MainVM.CurrentUser.UserId);
            if (rawData == null || rawData.Count == 0)
                return;

            TotalWords = rawData.Sum(d => d.WordCount);


            AverageWord = (int)Math.Round(rawData.Average(d => d.WordCount));

            var points = new ObservableCollection<ChartDataPoint>();
            double spacing = CHART_WIDTH / (rawData.Count - 1);

            int maxValue = Math.Max(rawData.Max(d => d.WordCount), 1);
            const double padding = 10;

            for (int i = 0; i < rawData.Count; i++)
            {
                var data = rawData[i];
                double x = i * spacing;
                double y = CHART_HEIGHT - ((data.WordCount / (double)maxValue) * (CHART_HEIGHT - padding));

                string dayShort = data.Day switch
                {
                    "Monday" => "T2",
                    "Tuesday" => "T3",
                    "Wednesday" => "T4",
                    "Thursday" => "T5",
                    "Friday" => "T6",
                    "Saturday" => "T7",
                    "Sunday" => "CN",
                    _ => data.Day
                };

                points.Add(new ChartDataPoint
                {
                    Day = dayShort,
                    WordCount = data.WordCount,
                    X = x,
                    Y = y
                });
            }

            DataPoints = points;
            SelectedPoint = DataPoints.LastOrDefault();
            LoadGridLines(maxValue);
        }
        private void LoadGridLines(int maxValue)
        {
            double step = maxValue / 4.0;
            double heightStep = CHART_HEIGHT / 4.0;
            GridLines = new ObservableCollection<GridLine>();

            for (int i = 0; i <= 4; i++)
            {
                double y = i * heightStep;
                int value = (int)Math.Round(maxValue - i * step);

                GridLines.Add(new GridLine
                {
                    Y = y,
                    Label = value.ToString(),
                    LabelY = y - 8
                });
            }
        }

        private int _totalSeries;
        public int TotalSeries
        {
            get { return _totalSeries; }
            set
            {
                if (_totalSeries != value)
                {
                    _totalSeries = value;
                    OnPropertyChanged(nameof(TotalSeries));
                }
            }
        }

        private int _totalChapters;
        public int TotalChapters
        {
            get { return _totalChapters; }
            set
            {
                if (_totalChapters != value)
                {
                    _totalChapters = value;
                    OnPropertyChanged(nameof(TotalChapters));
                }
            }
        }

        private int _totalWords;
        public int TotalWords
        {
            get { return _totalWords; }
            set
            {
                if (_totalWords != value)
                {
                    _totalWords = value;
                    OnPropertyChanged(nameof(TotalWords));
                }
            }
        }

        private int _writingTime;
        public int WritingTime
        {
            get { return _writingTime; }
            set
            {
                if (_writingTime != value)
                {
                    _writingTime = value;
                    OnPropertyChanged(nameof(WritingTime));
                }
            }
        }

        private int _avervageWord;
        public int AverageWord
        {
            get { return _avervageWord; }
            set
            {
                if (_avervageWord != value)
                {
                    _avervageWord = value;
                    OnPropertyChanged(nameof(AverageWord));
                }
            }
        }

        private SeriesService seriesService;
        private ChapterService chapterService;
        private WritingStatisticsService writingStatisticsService;
        public StatictisDetailViewModel()
        {
            seriesService = new SeriesService();
            chapterService = new ChapterService();
            writingStatisticsService = new WritingStatisticsService();
            SelectPointCommand = new RelayCommand(OnSelectPoint);

            LoadStatistics();
            LoadData();
        }

        private void OnSelectPoint(object parameter)
        {
            if (parameter is ChartDataPoint point)
            {
                SelectedPoint = point;
            }
        }

        private void LoadStatistics()
        {
            TotalSeries = seriesService.GetTotalSeries(MainWindow.MainVM.CurrentUser.UserId);
            TotalChapters = chapterService.GetTotalChapters(MainWindow.MainVM.CurrentUser.UserId);
            TotalWords = chapterService.GetTotalWords(MainWindow.MainVM.CurrentUser.UserId);
            //WritingTime = chapterService.GetTotalWritingTime();
            AverageWord = TotalChapters > 0 ? TotalWords / TotalChapters : 0;
        }
    }
}