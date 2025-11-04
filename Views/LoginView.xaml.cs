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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Page
    {
        public LoginView()
        {
            InitializeComponent();

        }

        private void LoginTab_Click(object sender, RoutedEventArgs e)
        {
            // Switch to Login form
            LoginForm.Visibility = Visibility.Visible;
            RegisterForm.Visibility = Visibility.Collapsed;

            // Update tab styles
            LoginTab.Style = (Style)FindResource("ActiveTabButton");
            RegisterTab.Style = (Style)FindResource("TabButton");
        }

        private void RegisterTab_Click(object sender, RoutedEventArgs e)
        {
            // Switch to Register form
            LoginForm.Visibility = Visibility.Collapsed;
            RegisterForm.Visibility = Visibility.Visible;

            // Update tab styles
            LoginTab.Style = (Style)FindResource("TabButton");
            RegisterTab.Style = (Style)FindResource("ActiveTabButton");
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = RegisterUsername.Text;
            string password = RegisterPassword.Password;
            string confirmPassword = RegisterConfirmPassword.Password;

            // Validate input
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vui lòng nhập tên người dùng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Implement actual registration logic here
            // For now, just show success message
            MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Switch to login tab
            LoginTab_Click(sender, e);

            // Clear registration fields
            RegisterUsername.Clear();
            RegisterPassword.Clear();
            RegisterConfirmPassword.Clear();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement forgot password functionality
            MessageBox.Show("Tính năng này đang được phát triển!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
