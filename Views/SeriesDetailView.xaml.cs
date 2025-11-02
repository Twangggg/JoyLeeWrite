using JoyLeeWrite.Models;
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
        public event EventHandler? AddChapterRequested;
        public SeriesDetailView()
        {
            InitializeComponent();
        }

        private void AddChapter_Click(object sender, RoutedEventArgs e)
        {
            AddChapterRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Chapter_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            // Lấy dữ liệu Chapter hiện tại
            var chapter = border.DataContext as Chapter;
            if (chapter == null) return;

            // Lấy ViewModel cha của trang
            var vm = this.DataContext as SeriesDetailViewModel;
            if (vm == null) return;

            int seriesId = vm.SeriesId;      
            int chapterId = chapter.ChapterId; 

            MainWindow.navigate.navigatePage(new WriteChapterView(seriesId));
            MessageBox.Show($"Series {seriesId} - Chapter {chapterId}: {chapter.Title}");
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
