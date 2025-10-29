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


namespace JoyLeeBookWriter
{
    public partial class MainWindow : Window
    {

        private bool isPlaceholder = true;
        private MainViewModel _mainVM;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                TextFormattingService _textService = new TextFormattingService(EditorRichTextBox);
                this._mainVM = new MainViewModel(_textService);
                this.DataContext = _mainVM;
                this.PreviewKeyDown += _mainVM.EditorToolbarVM.OnPreviewKeyDown;
            };
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
            if (isPlaceholder)
            {
                EditorRichTextBox.Foreground = new SolidColorBrush(Color.FromRgb(31, 41, 55));
                isPlaceholder = false;
            }
        }

        private void EditorRichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (EditorRichTextBox != null)
            {
                TextRange textRange = new TextRange(
                    EditorRichTextBox.Document.ContentStart,
                    EditorRichTextBox.Document.ContentEnd
                );

                string text = textRange.Text.Trim();

                if (string.IsNullOrEmpty(text) || text == "\r\n")
                {
                    EditorRichTextBox.Document.Blocks.Clear();
                    EditorRichTextBox.AppendText("Start writing your story...");
                    isPlaceholder = true;
                }
            }
        }

        private bool isSidebarOpen = true;

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
            if (isSidebarCollapsed)
            {
                AnimateSidebarWidth(SidebarBorder.ActualWidth, 300);
                SidebarContent.Visibility = Visibility.Visible;
                SaveTime.Visibility = Visibility.Visible;
                ToggleIcon.Text = "<";
                isSidebarCollapsed = false;
            }
            else
            {
                AnimateSidebarWidth(SidebarBorder.ActualWidth, 75);
                SidebarContent.Visibility = Visibility.Collapsed;
                SaveTime.Visibility = Visibility.Collapsed;
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
        private void TitleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBox;
            TitleTextBox.Text = "";
            box.Background = Brushes.White;
            box.Cursor = Cursors.IBeam;
        }

        private void TitleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBox;
            if (string.IsNullOrWhiteSpace(box.Text))
                TitleTextBox.Text = "Untitled Story";
            box.BorderThickness = new Thickness(0);
            box.Background = Brushes.Transparent;
            box.Cursor = Cursors.Arrow;

            if (!string.IsNullOrWhiteSpace(box.Text))
                this.Title = $"JoyLeeWrite - {box.Text}";
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
    }
}