using JoyLeeWrite.Commands;
using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace JoyLeeWrite.ViewModels
{
    public class AddInformationViewModel : INotifyPropertyChanged
    {
        private CategoryService categoryService;
        private SeriesService seriesService;
        private ImageService imageService;
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Category> Categories { get; set; }
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        private BitmapImage? _selectedImage;
        public BitmapImage? BitmapImage
        {
            get => _selectedImage;
            set
            {
                _selectedImage = value;
                OnPropertyChanged(nameof(BitmapImage));
            }
        }
        public ICommand SelectImageCommand { get; } 
        public ICommand SaveSeriesCommand { get; }
        public AddInformationViewModel()
        {
            seriesService = new SeriesService();
            categoryService = new CategoryService();
            imageService = new ImageService();
            List<Category> categoryList = categoryService.GetCategories();
            Categories = new ObservableCollection<Category>(categoryList);
            SelectImageCommand = new RelayCommand(_ => BitmapImage = imageService.SelectImage());
            SaveSeriesCommand = new RelayCommand(_ => SaveSeries());
        }

        public void SaveSeries()
        {
            imageService.SaveAsAvif(BitmapImage, imageService.extractPath(_title), 240, 360);
            MessageBox.Show("Đã lưu ảnh dưới dạng AVIF.", "Thông báo");
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
