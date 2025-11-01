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
        public List<String> StatusList { get; set; }
        public ICommand SelectImageCommand { get; }
        public ICommand SaveSeriesCommand { get; }
        public AddInformationViewModel()
        {
            //Khoi tao service
            seriesService = new SeriesService();
            categoryService = new CategoryService();
            imageService = new ImageService();

            //Khoi tao danh sach trang thai
            List<Category> categoryList = categoryService.GetCategories();
            Categories = new ObservableCollection<Category>(categoryList);
            StatusList = new List<string> { "Ongoing", "Completed" };

            //Bind command
            SelectImageCommand = new RelayCommand(_ => BitmapImage = imageService.SelectImage());
            SaveSeriesCommand = new RelayCommand(_ => SaveSeries());
        }

        private string _title = "Enter title for series";
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        private string _status = "Ongoing";
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        private string _description = "Enter a description of the series";
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
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

        public void SaveSeries()
        {
            if (!ValidateInputs())
            {
                return;
            }
            try
            {
                int authorId = 1; // Replace with actual author ID retrieval logic
                Series newSeries = new Series
                {
                    Title = Title,
                    Status = Status,
                    Description = Description,
                    AuthorId = authorId,
                    CoverImgUrl = imageService.extractPath(Title),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    LastModified = DateTime.Now
                };
                newSeries.Categories.Clear();
                imageService.SaveAsAvif(BitmapImage, newSeries.CoverImgUrl, 240, 360);
                seriesService.AddSeries(newSeries, categoryService.GetCategories(GetSelectedCategories()));
                MessageBox.Show("Series saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving series: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(Title) || Title == "Enter title for series")
            {
                MessageBox.Show("Please enter a valid title.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (GetSelectedCategories().Count == 0)
            {
                MessageBox.Show("Please select at least one category.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(Description) || Description == "Enter a description of the series")
            {
                MessageBox.Show("Please enter a valid description.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (BitmapImage == null)
            {
                MessageBox.Show("Please select an image.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        public List<int> GetSelectedCategories()
        {
            return Categories.Where(c => c.IsSelected).Select(c => c.CategoryId).ToList();
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
