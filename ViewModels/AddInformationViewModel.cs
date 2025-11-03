using JoyLeeWrite.Commands;
using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using JoyLeeWrite.Views;
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
        public ICommand SelectImageCommand { get; }
        public ICommand SaveSeriesCommand { get; }
        public ICommand CancelCommand { get; }
        public AddInformationViewModel(FormMode mode, int seriesId = 0)
        {
            Mode = mode;    
            //Khoi tao service
            _seriesService = new SeriesService();
            _categoryService = new CategoryService();
            _imageService = new ImageService();
            StatusList = new List<string> { "Ongoing", "Completed", "Draft" };

            if (mode == FormMode.Create)
            {
                //Khoi tao danh sach trang thai
                List<Category> categoryList = _categoryService.GetCategories();
                Categories = new ObservableCollection<Category>(categoryList);
                
                //Bind command
                SelectImageCommand = new RelayCommand(_ => BitmapImage = _imageService.SelectImage());
                SaveSeriesCommand = new RelayCommand(_ => SaveSeries());
                CancelCommand = new RelayCommand(_ => ClearInputs());
            } else if(mode == FormMode.Edit) {
                Series series = _seriesService.GetSeriesById(seriesId);
                
                LoadSeries(series);
                SelectImageCommand = new RelayCommand(_ => BitmapImage = _imageService.SelectImage());
                SaveSeriesCommand = new RelayCommand(_ => UpdateSeries(series));
                CancelCommand = new RelayCommand(_ => LoadSeries(series));
            }
        }
        private List<string> _statusList;
        public List<string> StatusList
        {
            get => _statusList;
            set
            {
                _statusList = value;
                OnPropertyChanged(nameof(StatusList));
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
        private void ClearInputs()
        {
            Title = "Enter title for series";
            Description = "Enter a description of the series";
            BitmapImage = null;
            foreach (var category in Categories)
            {
                category.IsSelected = false;
            }
            Status = "Ongoing";
        }
        public void UpdateSeries(Series series)
        {
            if (!ValidateInputs())
            {
                return;
            }
            try
            {

                series.Title = Title;
                series.Status = Status;
                series.Description = Description;
                series.CoverImgUrl = _imageService.extractPath(Title);
                series.UpdatedDate = DateTime.Now;
                series.LastModified = DateTime.Now;
                series.Categories.Clear();
                if (_seriesService.UpdateSeries(series, _categoryService.GetCategories(GetSelectedCategories())))
                {
                    _imageService.SaveAsAvif(BitmapImage, series.CoverImgUrl, 240, 360);
                }
                MainWindow.navigate.navigatePage(new SeriesView());
                MainWindow.MainVM.addSeriesDetailViewModel(series.SeriesId);
                MainWindow.MainVM.CurrentSeriesId = series.SeriesId;
                MessageBox.Show("Series updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating series: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                int authorId = MainWindow.MainVM.CurrentUserId; 
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
                MainWindow.navigate.navigatePage(new SeriesView());
                int seriesId = _seriesService.GetNewSeriesId(authorId);
                MainWindow.MainVM.addSeriesDetailViewModel(seriesId);
                MainWindow.MainVM.CurrentSeriesId = seriesId;
                MessageBox.Show("Series created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
            List<Category> categoryList = _categoryService.GetCategoriesBySeriesId(series.SeriesId);
            categoryList.AddRange(_categoryService.GetCategoriesNotInSeries(series.SeriesId));
            Categories = new ObservableCollection<Category>(categoryList);
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
