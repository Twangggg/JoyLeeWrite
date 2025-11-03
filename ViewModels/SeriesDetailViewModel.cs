using JoyLeeWrite.Commands;
using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using JoyLeeWrite.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace JoyLeeWrite.ViewModels
{
    public class SeriesDetailViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private SeriesService _seriesService;
        private ImageService _imageService;
        private ChapterService _chapterService;
        private CategoryService _categoryService;
        private Series series;
        private ObservableCollection<Chapter> _chapters;
        public ObservableCollection<Chapter> Chapters
        {
            get => _chapters;
            set { _chapters = value; OnPropertyChanged(nameof(Chapters)); }
        }
        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(nameof(Categories)); }
        }
        public int SeriesId
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public BitmapSource CoverImage
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }

        public DateTime CreatedDate
        {
            get; set;
        }

        public int ChapterCount
        {
            get; set;
        }
        public string ColorStatus { get; set; }
        public ICommand DeleteSeriesCommand { get; set; }
        public ICommand DeleteChapterCommand { get; set; }
        public SeriesDetailViewModel(int seriesId)
        {
            _seriesService = new SeriesService();
            _imageService = new ImageService();
            _chapterService = new ChapterService();
            _categoryService = new CategoryService();
            series = _seriesService.GetSeriesById(seriesId);
            List<Chapter> chapters = _chapterService.GetChaptersBySeriesId(seriesId);
            Chapters = new ObservableCollection<Chapter>(chapters);
            List<Category> categories = _categoryService.GetCategoriesBySeriesId(seriesId);
            Categories = new ObservableCollection<Category>(categories);
            LoadSeries();

            DeleteSeriesCommand = new RelayCommand(_ => DeleteSeries(seriesId));
            DeleteChapterCommand = new RelayCommand(param => DeleteChapter((int)param));
        }

        private void DeleteSeries(int seriesId)
        {
            var result = MessageBox.Show(
                    "Are you sure delete this series?",
                    "Delete Series",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {

                _seriesService.DeleteSeriesById(seriesId);
                MainWindow.navigate.navigatePage(new HomepageView());
                MainWindow.MainVM.UpdateHomepage();
            }
        }
        private void DeleteChapter(int chapterId)
        {
            var result = MessageBox.Show(
                    "Are you sure delete this chapter?",
                    "Delete Chapter",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _chapterService.DeleteChapterById(chapterId);

                var chapterToRemove = Chapters.FirstOrDefault(c => c.ChapterId == chapterId);
                if (chapterToRemove != null)
                {
                    Chapters.Remove(chapterToRemove);
                }

                // Cập nhật ChapterCount
                ChapterCount = Chapters.Count;
                OnPropertyChanged(nameof(ChapterCount));
            }
        }
        private void LoadSeries()
        {
            SeriesId = series.SeriesId;
            Title = series.Title;
            Description = series.Description;
            CoverImage = _imageService.LoadAvifAsBitmap(series.CoverImgUrl);
            Status = series.Status;
            CreatedDate = series.CreatedDate;
            ChapterCount = _chapterService.CountChaptersBySeriesId(series.SeriesId);
            ColorStatus = Status == "Ongoing" ? "#DAA520" : Status == "Completed" ? "#4CAF50" : "Gray";
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
