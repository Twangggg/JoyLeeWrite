using JoyLeeWrite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JoyLeeWrite.Views
{
    /// <summary>
    /// Interaction logic for SeriesView.xaml
    /// </summary>
    public partial class SeriesDetailView : UserControl
    {
        public SeriesDetailView()
        {
            InitializeComponent();
        }
        private void LoadSeriesData()
        {
            // Load series data from database or model
            // Populate ChaptersList with actual data
        }

        private void EditSeries_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to edit series page
            MessageBox.Show("Chuyển đến trang chỉnh sửa series", "Edit", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddChapter_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to create new chapter page
            MessageBox.Show("Tạo chapter mới", "Add Chapter", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Chapter_Click(object sender, MouseButtonEventArgs e)
        {
            // Navigate to chapter detail/edit page
            if (sender is Border border)
            {
                MessageBox.Show("Mở chapter để chỉnh sửa", "Edit Chapter", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditSeries(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int seriesId)
            {
                MainWindow.navigate.navigatePage(new CreateSeriesView());
                MainWindow.MainVM.addInformationViewModel(FormMode.Edit, seriesId);
            }
        }
    }
}
