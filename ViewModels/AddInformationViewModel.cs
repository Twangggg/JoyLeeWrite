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
    public enum FormMode
    {
        Create,
        Edit
    }
    public class AddInformationViewModel : INotifyPropertyChanged
    {
        private CategoryService _categoryService;
        private SeriesService _seriesService;
        private ImageService _imageService;
        public FormMode Mode { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Category> Categories { get; set; }
        public List<String> StatusList { get; set; }
        public ICommand SelectImageCommand { get; }
        public ICommand SaveSeriesCommand { get; }
        public AddInformationViewModel(FormMode mode, int seriesId = 0)
        {
            Mode = mode;    
            //Khoi tao service
            _seriesService = new SeriesService();
            _categoryService = new CategoryService();
            _imageService = new ImageService();

            if (mode == FormMode.Create)
            {
                //Khoi tao danh sach trang thai
                List<Category> categoryList = _categoryService.GetCategories();
                Categories = new ObservableCollection<Category>(categoryList);
                StatusList = new List<string> { "Ongoing", "Completed", "Draft" };

                //Bind command
                SelectImageCommand = new RelayCommand(_ => BitmapImage = _imageService.SelectImage());
                SaveSeriesCommand = new RelayCommand(_ => SaveSeries());
            } else if(mode == FormMode.Edit) {
                Series series = _seriesService.GetSeriesById(seriesId);
                LoadSeries(series);
            }
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
                    CoverImgUrl = _imageService.extractPath(Title),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    LastModified = DateTime.Now
                };
                newSeries.Categories.Clear();
                _seriesService.AddSeries(newSeries, _categoryService.GetCategories(GetSelectedCategories()));
                _imageService.SaveAsAvif(BitmapImage, newSeries.CoverImgUrl, 240, 360);
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
        private void LoadSeries(Series series)
        {
            Title = series.Title;
            Description = series.Description;
            BitmapImage = (BitmapImage?)_imageService.LoadAvifAsBitmap(series.CoverImgUrl);
            Status = series.Status;
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
