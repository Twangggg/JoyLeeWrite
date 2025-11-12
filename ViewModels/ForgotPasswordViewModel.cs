using JoyLeeWrite.Commands;
using JoyLeeWrite.Services;
using JoyLeeWrite.Utils;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace JoyLeeWrite.ViewModels
{
    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        private UserService _userService;
        private string _email;
        private string _verificationCode;
        private string _newPassword;
        private string _confirmPassword;
        private string _message;
        private bool _isCodeSent;

        public ForgotPasswordViewModel()
        {
            _userService = new UserService();
            SendVerificationCodeCommand = new RelayCommand(SendVerificationCode,_ => !string.IsNullOrWhiteSpace(Email));
            ResetPasswordCommand = new RelayCommand(ResetPassword, _ => CanResetPassword);
            VerificationCodeCommand = new RelayCommand(CheckVerificationCode, _ => IsCodeSent);
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string VerificationCode
        {
            get => _verificationCode;
            set
            {
                _verificationCode = value;
                OnPropertyChanged();
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MessageVisibility));
            }
        }

        public bool IsCodeSent
        {
            get => _isCodeSent;
            set
            {
                _isCodeSent = value;
                OnPropertyChanged();
            }
        }

        public bool CanSendCode => !string.IsNullOrWhiteSpace(Email);

        public bool CanResetPassword = false;

        public Visibility MessageVisibility => string.IsNullOrWhiteSpace(Message) ? Visibility.Collapsed : Visibility.Visible;

        public Brush MessageColor { get; set; } = Brushes.Red;

        public ICommand SendVerificationCodeCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand VerificationCodeCommand { get; }


        private async void SendVerificationCode(object parameter)
        {
            try
            {
                if (!Regex.IsMatch(Email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$"))
                {
                    MessageBox.Show("Your email is invalid", "Warning");
                    return; 
                }
                if (_userService.CheckExistEmail(Email))
                {
                    EmailHelper.SendOtpAsync(Email);
                } else
                {
                    MessageBox.Show("Your email isn't has account", "Warning");
                    return;
                }
                    ShowSuccessMessage($"Verification code sent to {Email}!");
                IsCodeSent = true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error: {ex.Message}");
            }
        }
        private async void CheckVerificationCode(object paramter)
        {
            if (_verificationCode == null)
            {
                MessageBox.Show("Your OTP can be empty", "Warning");
                return;
            }
            if (!Regex.IsMatch(_verificationCode, "^[0-9]*$") || _verificationCode.Length != 6)
            {
                MessageBox.Show("Your OTP is invaild. Please try again", "Error");
                return;
            }
            if (_verificationCode.Equals(EmailHelper.CurrentOtp))
            {
                MessageBox.Show("Valid email successful", "Successfully");
                CanResetPassword = true;
            }
            else
            {
                MessageBox.Show("Your OTP is incorrect. Please try again");
                return;
            }
        }
        private async void ResetPassword(object parameter)
        {
            try
            {
                if (NewPassword != ConfirmPassword)
                {
                    ShowErrorMessage("Passwords do not match!");
                    return;
                }

                if (NewPassword.Length < 6)
                {
                    ShowErrorMessage("Password must be at least 6 characters!");
                    return;
                }

                _userService.UpdatePasswordByEmail(Email, NewPassword);
                {
                    ShowSuccessMessage("Password reset successfully!");
                }
                if (parameter is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error: {ex.Message}");
            }
        }

        private void ShowErrorMessage(string message)
        {
            Message = message;
            MessageColor = Brushes.Red;
        }

        private void ShowSuccessMessage(string message)
        {
            Message = message;
            MessageColor = Brushes.Green;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

 
    }
}