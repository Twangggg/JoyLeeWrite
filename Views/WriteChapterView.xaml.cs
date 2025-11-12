using JoyLeeWrite;
using JoyLeeWrite.Models;
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


        private void BackToSeries_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.MainVM.CurrentSeriesId == 0)
            {
                MainWindow.navigate.goBack();
            } else
            {
                MainWindow.navigate.navigatePage(new SeriesView());
                MainWindow.MainVM.addSeriesDetailViewModel(MainWindow.MainVM.CurrentSeriesId);
            }
        }

        private void AIButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle chat box visibility
            if (AIChatBox.Visibility == Visibility.Collapsed)
            {
                AIChatBox.Visibility = Visibility.Visible;
                AIFloatingButton.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseAIChat_Click(object sender, RoutedEventArgs e)
        {
            AIChatBox.Visibility = Visibility.Collapsed;
            AIFloatingButton.Visibility = Visibility.Visible;
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string message = ChatInput.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                // Add user message to chat
                AddUserMessage(message);
                ChatInput.Clear();

                // Call AI API here
                // AddAIResponse(aiResponse);
            }
        }

        private void ChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage_Click(sender, e);
            }
        }

        private void AddUserMessage(string message)
        {
            var userBorder = new Border
            {
                Style = (Style)Resources["ChatMessageUserStyle"],
                Margin = new Thickness(0, 0, 0, 12)
            };

            var textBlock = new TextBlock
            {
                Text = message,
                FontSize = 14,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            userBorder.Child = textBlock;
            ChatMessagesPanel.Children.Add(userBorder);
            ChatScrollViewer.ScrollToEnd();
        }
    }
}