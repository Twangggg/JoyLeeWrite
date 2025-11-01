using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.ViewModels
{
    public class SeriesDetailViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private SeriesService _seriesService;
        private int _seriesId;
        private string _title;
        private string _description;
        private string _coverImage;
        private string _status;
        private DateTime _createdDate;
        private int _chapterCount;
        public int SeriesId
        {
            get => _seriesId;
            set { _seriesId = value; OnPropertyChanged(nameof(SeriesId)); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public string CoverImage
        {
            get => _coverImage;
            set { _coverImage = value; OnPropertyChanged(nameof(CoverImage)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set { _createdDate = value; OnPropertyChanged(nameof(CreatedDate)); }
        }

        public int ChapterCount
        {
            get => _chapterCount;
            set { _chapterCount = value; OnPropertyChanged(nameof(ChapterCount)); }
        }

        public SeriesDetailViewModel(int seriesId)
        {
            _seriesService = new SeriesService();
            Series series = _seriesService.GetSeriesById(seriesId);
            LoadSeries(series);
        }
        private void LoadSeries(Series series)
        {
            Title = series.Title;
            Description = series.Description;
            CoverImage = series.CoverImgUrl;
            Status = series.Status;
            CreatedDate = series.CreatedDate;
            //ChapterCount
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
