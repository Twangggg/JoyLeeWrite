using JoyLeeWrite.Commands;
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
    public class ProfileDetailViewModel : INotifyPropertyChanged
    {
        private UserService _userService;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public ICommand SaveProfileCommand { get; }
        public ProfileDetailViewModel()
        {
            _userService = new UserService();
            _username = MainWindow.MainVM.CurrentUser?.Username ?? string.Empty;
            _email = MainWindow.MainVM.CurrentUser?.Email ?? string.Empty;
            SaveProfileCommand = new RelayCommand(param => SaveProfile(), param => CanSaveProfile());
        }

        public void SaveProfile()
        {
            if (MainWindow.MainVM.CurrentUser != null)
            {
                MainWindow.MainVM.CurrentUser.Username = this.Username;
                MainWindow.MainVM.CurrentUser.Email = this.Email;
                if (_userService.UpdateUserProfile(MainWindow.MainVM.CurrentUser.UserId, this.Username, this.Email))
                {
                    MessageBox.Show("Update Information Successfully!", "Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error updating profile!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private bool CanSaveProfile()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Email);
        }
    }
}
