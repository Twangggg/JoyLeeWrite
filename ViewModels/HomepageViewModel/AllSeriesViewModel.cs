using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using JoyLeeWrite.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.ViewModels.HomepageViewModel
{
    public class AllSeriesViewModel : INotifyPropertyChanged
    {
        private SeriesService seriesService;
        private ImageService imageService;
        private ChapterService chapterService;
        private ObservableCollection<Series> _filteredSeries;
        private string _searchText;

        public ObservableCollection<Series> AllSeries { get; set; }

        public ObservableCollection<Series> FilteredSeries
        {
            get => _filteredSeries;
            set
            {
                _filteredSeries = value;
                OnPropertyChanged(nameof(FilteredSeries));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterSeries();
            }
        }

        public AllSeriesViewModel()
        {
            seriesService = new SeriesService();
            imageService = new ImageService();
            chapterService = new ChapterService();

            var seriesList = seriesService.GetAllSeries(MainWindow.MainVM.CurrentUser.UserId);
            foreach (Series series in seriesList)
            {
                if (!string.IsNullOrEmpty(series.CoverImgUrl))
                {
                    series.ChapterCount = chapterService.CountChaptersBySeriesId(series.SeriesId);
                    series.CoverImage = imageService.LoadAvifAsBitmap(series.CoverImgUrl);
                }
            }
            AllSeries = new ObservableCollection<Series>(seriesList);
            FilteredSeries = new ObservableCollection<Series>(seriesList);
        }

        public void UpdateAllSeriesVM()
        {
            var seriesList = seriesService.GetAllSeries(MainWindow.MainVM.CurrentUser.UserId);
            foreach (Series series in seriesList)
            {
                if (!string.IsNullOrEmpty(series.CoverImgUrl))
                {
                    series.ChapterCount = chapterService.CountChaptersBySeriesId(series.SeriesId);
                    series.CoverImage = imageService.LoadAvifAsBitmap(series.CoverImgUrl);
                }
            }
            AllSeries = new ObservableCollection<Series>(seriesList);
            FilterSeries();
        }

        private void FilterSeries()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredSeries = new ObservableCollection<Series>(AllSeries);
            }
            else
            {
                var filtered = AllSeries.Where(s =>
                    s.Title.ToLower().Contains(SearchText.ToLower()) ||
                    (!string.IsNullOrEmpty(s.Description) && s.Description.ToLower().Contains(SearchText.ToLower()))
                ).ToList();

                FilteredSeries = new ObservableCollection<Series>(filtered);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}