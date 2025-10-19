using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace JoyLeeBookWriter
{
    public partial class MainWindow : Window
    {
        private bool isPlaceholder = true;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
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
            //if (isPlaceholder)
            //{
            //    EditorTextBox.Text = "";
            //    EditorTextBox.Foreground = new SolidColorBrush(Color.FromRgb(31, 41, 55));
            //    isPlaceholder = false;
            //}
        }

        private void EditorTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(EditorTextBox.Text))
            //{
            //    EditorTextBox.Text = "Start writing your story...";
            //    EditorTextBox.Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175));
            //    isPlaceholder = true;
            //}
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private bool isSidebarOpen = true;

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "Start writing your story...")
            {
                tb.Text = "";
                tb.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void AIButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hỏi AI gì đây?", "AI Assistant", MessageBoxButton.OK, MessageBoxImage.Question);
        }
        private bool isSidebarCollapsed = false;

        private void ToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (isSidebarCollapsed)
            {
                // Mở rộng
                AnimateSidebarWidth(SidebarBorder.ActualWidth, 300);
                SidebarContent.Visibility = Visibility.Visible;
                ToggleIcon.Text = "<";
                isSidebarCollapsed = false;
            }
            else
            {
                // Thu nhỏ
                AnimateSidebarWidth(SidebarBorder.ActualWidth, 75);
                SidebarContent.Visibility = Visibility.Collapsed;  // Ẩn toàn bộ phần dưới
                ToggleIcon.Text = ">";
                isSidebarCollapsed = true;
            }
        }

        private void AnimateSidebarWidth(double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            SidebarBorder.BeginAnimation(WidthProperty, animation);
        }

        private void AlignLeft_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AlignCenter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AlignRight_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AlignJustify_Click(object sender, RoutedEventArgs e)
        {

        }
        private void TitleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBox;
            box.BorderThickness = new Thickness(1);
            box.BorderBrush = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            box.Background = Brushes.White;
            box.Cursor = Cursors.IBeam;
        }

        private void TitleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBox;
            box.BorderThickness = new Thickness(0);
            box.Background = Brushes.Transparent;
            box.Cursor = Cursors.Arrow;

            // Cập nhật tiêu đề cửa sổ
            if (!string.IsNullOrWhiteSpace(box.Text))
                this.Title = $"JoyLeeWrite - {box.Text}";
        }

        private void TitleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Keyboard.ClearFocus(); // Mất focus để lưu tiêu đề
            }
        }
    }
}