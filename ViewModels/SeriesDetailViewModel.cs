using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
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
        }
        private void LoadSeries()
        {
            SeriesId = series.SeriesId;
            Title = series.Title;
            Description = series.Description;
            CoverImage = _imageService.LoadAvifAsBitmap(series.CoverImgUrl);
            Status = series.Status;
            CreatedDate = series.CreatedDate;
            ChapterCount = Chapters.Count;
            ColorStatus = Status == "Ongoing" ? "#DAA520" : Status == "Completed" ? "#4CAF50" : "Gray";
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
