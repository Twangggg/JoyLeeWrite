using JoyLeeWrite.Commands;
using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using JoyLeeWrite.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JoyLeeWrite.ViewModels
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        private readonly UserService userService;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
        private string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
                    OnPropertyChanged(nameof(ConfirmPassword));
                }
            }
        }
        private string _registerUsername;
        public string RegisterUsername
        {
            get { return _registerUsername; }
            set
            {
                if (_registerUsername != value)
                {
                    _registerUsername = value;
                    OnPropertyChanged(nameof(RegisterUsername));
                }
            }
        }
        private string _registerPassword;
        public string RegisterPassword
        {
            get { return _registerPassword; }
            set
            {
                if (_registerPassword != value)
                {
                    _registerPassword = value;
                    OnPropertyChanged(nameof(RegisterPassword));
                }
            }
        }
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public AuthViewModel()
        {
            userService = new UserService();
            LoginCommand = new RelayCommand(_ => Login());
            RegisterCommand = new RelayCommand(_ => LoginRegister());
        }

        private void Login()
        {
            if (!ValidateInput())
            {
                return;
            }
            User user = userService.ValidateUserCredentials(Username, Password);
            if (user != null)
            {
                
                MessageBox.Show("Login successful!", "Message",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.MainVM.CurrentUser =  user;
                MainWindow.MainVM.Username = Username;
                MainWindow.navigate.navigatePage(new HomepageView());
                MainWindow.MainVM.addHomepageViewModel();
            }
            else
            {
                MessageBox.Show("Invalid username or password!", "Message",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoginRegister()
        {
            if (!ValidateRegisterInput())
            {
                return;
            }
            if (!RegisterPassword.Equals(ConfirmPassword))
            {
                MessageBox.Show("Passwords do not match!", "Message",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int userId = userService.CreateUser(RegisterUsername, RegisterPassword);
            if (userId == -1)
            {
                MessageBox.Show("Username already exists!", "Message",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Registration successful! Please log in.", "Message",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.navigate.navigatePage(new LoginView());
            }
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show("Please enter your username!", "Message",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Please enter your password!", "Message",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return true;
        }
        private bool ValidateRegisterInput()
        {
            if (string.IsNullOrWhiteSpace(RegisterUsername))
            {
                MessageBox.Show("Please enter your username!", "Message",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (string.IsNullOrWhiteSpace(RegisterPassword))
            {
                MessageBox.Show("Please enter your password!", "Message",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (RegisterPassword != ConfirmPassword)
            {
                MessageBox.Show("Passwords do not match!", "Message",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return true;
        }
    }
}
