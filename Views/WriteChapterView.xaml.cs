using JoyLeeWrite;
using JoyLeeWrite.Services;
using JoyLeeWrite.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;


namespace JoyLeeWrite.Views
{
    public partial class WriteChapterView : Page
    {

        private bool isPlaceholder = true;
        public WriteChapterView( string title, int chapterNumber)
        {
            MainWindow.MainVM.CurrentPageTitle = "Write Chapter";
            MainWindow.MainVM.SupPageTitle = $"Chapter {chapterNumber}: {title}";
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Back button clicked", "Navigation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NewStoryButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Create new story", "New Story", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NewChapterButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Create new chapter", "New Chapter", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Story saved successfully!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Settings", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditorTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void EditorRichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void EditorRichTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var richText = sender as RichTextBox;
            if (richText != null)
            {
                TextRange textRange = new TextRange(richText.Document.ContentStart, richText.Document.ContentEnd);
                if (textRange.Text.Trim() == "Start writing your story...")
                {
                    textRange.Text = "";
                }
            }
        }


        private void AIButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hỏi AI gì đây?", "AI Assistant", MessageBoxButton.OK, MessageBoxImage.Question);
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
            // Cập nhật hiển thị các phần tử con
            if (isSidebarCollapsed)
            {
                SidebarContent.Visibility = Visibility.Visible;
                
                BackSeries.Visibility = Visibility.Visible;
                Logo.Visibility = Visibility.Visible;
                Logo.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/img/logo.png"));
                SidebarIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/img/sidebar.png"));
                isSidebarCollapsed = false;
            }
            else
            {
                SidebarContent.Visibility = Visibility.Collapsed;
                
                BackSeries.Visibility = Visibility.Collapsed;
                Logo.Width = 75;
                Logo.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/img/header_logo.png"));
                SidebarIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/img/sidebar_active.png"));
                isSidebarCollapsed = true;
            }
        }

        private void TitleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBox;
            TitleTextBox.Text = "";
            box.Background = Brushes.White;
            box.Cursor = Cursors.IBeam;
        }

        

        private void TitleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Keyboard.ClearFocus();
            }
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.Tag = "Active";
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void BackToSeries_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.navigate.navigatePage(new SeriesView());
        }
    }
}