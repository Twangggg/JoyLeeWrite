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
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace JoyLeeWrite.ViewModels
{
    public class AddInformationViewModel : INotifyPropertyChanged
    {
        private CategoryService categoryService;
        private SeriesService seriesService;
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Category> Categories { get; set; }
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
        public AddInformationViewModel()
        {
            seriesService = new SeriesService();
            categoryService = new CategoryService();
            List<Category> categoryList = categoryService.GetCategories();
            Categories = new ObservableCollection<Category>(categoryList);
            SelectImageCommand = new RelayCommand(_ => SelectImage());
        }
        private void SelectImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Ảnh (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif"
            };

            if (dialog.ShowDialog() == true)
            {
                var image = new BitmapImage();
                using (var stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                BitmapImage = image;
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
