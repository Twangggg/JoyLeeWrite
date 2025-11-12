using JoyLeeWrite.Commands;
using JoyLeeWrite.Services;
using JoyLeeWrite.Utils;
using JoyLeeWrite.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JoyLeeWrite.ViewModels
{
    public class ProfileDetailViewModel : INotifyPropertyChanged
    {
        private bool canSave = false;
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
        private string _otpInput;
        public string OtpInput
        {
            get => _otpInput;
            set
            {
                if (_otpInput != value)
                {
                    _otpInput = value;
                    OnPropertyChanged(nameof(_otpInput));
                }
            }
        }
        public ICommand SaveProfileCommand { get; }
        public ICommand SendOtpCommand { get; }
        public ICommand VerifyOtpCommand { get; }
        public ProfileDetailViewModel()
        {
            _userService = new UserService();
            _username = MainWindow.MainVM.CurrentUser?.Username ?? string.Empty;
            _email = MainWindow.MainVM.CurrentUser?.Email ?? string.Empty;
            canSave = _email.Length > 0;
            SaveProfileCommand = new RelayCommand(param => SaveProfile(), param => CanSaveProfile());
            SendOtpCommand = new RelayCommand(_ => SendOtp(), _ => CanSendOtp());
            VerifyOtpCommand = new RelayCommand(_ => VerifyOtp());
        }
        private void SendOtp()
        {
            if (_userService.CheckExistEmail(Email))
            {
                MessageBox.Show("Your email is exist", "Warning");
                return;
            }
            EmailHelper.SendOtpAsync(Email);
            canSave = false;
        }
        private void VerifyOtp()
        {
            if (_otpInput == null)
            {
                MessageBox.Show("Your OTP can be empty", "Warning");
                return;
            }
            if (!Regex.IsMatch(_otpInput, "^[0-9]*$") || _otpInput.Length != 6)
            {
                MessageBox.Show("Your OTP is invaild. Please try again", "Error");
                return;
            }
            if (_otpInput.Equals(EmailHelper.CurrentOtp))
            {
                MessageBox.Show("Valid email successful", "Successfully");
                canSave = true;
            } else
            {
                MessageBox.Show("Your OTP is incorrect. Please try again");
                return;
            }

        }
        private void SaveProfile()
        {
            if (MainWindow.MainVM.CurrentUser != null)
            {

                if (_userService.CheckExistUsername(Username) && Username.ToLower().Equals(MainWindow.MainVM.Username.ToLower()))
                {
                    MessageBox.Show("This username is already used", "Message");
                    return;
                }
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
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Email) && canSave;
        }
        private bool CanSendOtp()
        {
            return !string.IsNullOrEmpty(Email) && Regex.IsMatch(Email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$");
        }
    }
}
