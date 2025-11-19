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

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {

            var forgotPasswordDialog = new ForgotPasswordDialog
            {
                Owner = Window.GetWindow(this)
            };

            bool? result = forgotPasswordDialog.ShowDialog();

            if (result == true)
            {
                MessageBox.Show("Password has been reset successfully! Please login with your new password.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

    }
}
