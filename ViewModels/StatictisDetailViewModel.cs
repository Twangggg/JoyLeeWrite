using JoyLeeWrite.Services;
using JoyLeeWrite.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.ViewModels
{
    public class StatictisDetailViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public StatictisDetailViewModel()
        {
            seriesService = new SeriesService();
            chapterService = new ChapterService();
            LoadStatistics();
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
