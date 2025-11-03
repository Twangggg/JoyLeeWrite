using JoyLeeWrite.Models;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JoyLeeWrite.Views
{
    /// <summary>
    /// Interaction logic for SeriesView.xaml
    /// </summary>
    public partial class SeriesView : Page
    {
        public SeriesView()
        {
            InitializeComponent();
        }

        private bool isSidebarCollapsed = false;
        private void ToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            double from = SidebarBorder.ActualWidth;
            double to = isSidebarCollapsed ? 300 : 75;

            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            SidebarBorder.BeginAnimation(FrameworkElement.WidthProperty, animation);
            Logo.BeginAnimation(FrameworkElement.WidthProperty, animation);

            if (isSidebarCollapsed)
            {
                SidebarContent.Visibility = Visibility.Visible;
                AuthorInfoBorder.Visibility = Visibility.Visible;
                BackSeries.Visibility = Visibility.Visible;
                Logo.Visibility = Visibility.Visible;
                Logo.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/img/logo.png"));
                SidebarIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/img/sidebar.png"));
                isSidebarCollapsed = false;
            }
            else
            {
                SidebarContent.Visibility = Visibility.Collapsed;
                AuthorInfoBorder.Visibility = Visibility.Collapsed;
                BackSeries.Visibility = Visibility.Collapsed;
                Logo.Width = 75;
                Logo.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/img/header_logo.png"));
                SidebarIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/img/sidebar_active.png"));

                isSidebarCollapsed = true;
            }
        }

        private void BackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.navigate.navigatePage(new HomepageView());
            MainWindow.MainVM.CurrentPageTitle = "Dashboard";
            MainWindow.MainVM.SupPageTitle = "";
        }
        

        private void AddChapterView_ChapterAdded(string title, int number)
        {
            //// Giả sử DataContext là ViewModel chứa ObservableCollection<Chapter>
            //var vm = DataContext as WriteChapterView;
            //vm?.Chapters.Add(new Chapter
            //{
            //    Title = title,
            //    ChapterNumber = number
            //});

            AddChapterView.Visibility = Visibility.Collapsed;
        }

        private void AddChapterView_Cancelled()
        {
            AddChapterView.Visibility = Visibility.Collapsed;
        }
        private void SeriesDetailView_AddChapterRequested(object sender, EventArgs e)
        {
            AddChapterView.Visibility = Visibility.Visible;
        }

    }
}
