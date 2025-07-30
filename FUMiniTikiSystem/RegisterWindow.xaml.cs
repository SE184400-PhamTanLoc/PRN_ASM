using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using BusinessLayer.DTO;
using BusinessLayer.Service;

namespace FUMiniTikiSystem
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _authService;

        public RegisterWindow(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // Validation
            if (string.IsNullOrEmpty(fullName))
            {
                ShowError("Full name is required!");
                return;
            }

            if (!IsValidFullName(fullName))
            {
                ShowError("Full name must only contain letters and spaces, and be 2-100 characters.");
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                ShowError("Email is required!");
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowError("Invalid email format!");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Password is required!");
                return;
            }

            if (!IsStrongPassword(password))
            {
                ShowError("Password must be at least 6 characters, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Passwords do not match!");
                return;
            }

            // Call business layer
            var dto = new CustomerCreateDTO
            {
                FullName = fullName,
                Email = email,
                Password = password
            };

            var result = await _authService.RegisterAsync(dto);

            if (result.IsSuccess)
            {
                ShowSuccess("Registration successful! Please login.");
                ClearForm();
            }
            else
            {
                ShowError(result.Message);
            }
        }

        private void txtLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Open login window
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            // Close register window
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidFullName(string fullName)
        {
            // Only letters and spaces, length 2-100
            return Regex.IsMatch(fullName, @"^[a-zA-Z\s]{2,100}$");
        }

        private bool IsStrongPassword(string password)
        {
            // At least 6 chars, 1 upper, 1 lower, 1 digit, 1 special char
            return password.Length >= 6
                && Regex.IsMatch(password, "[A-Z]")
                && Regex.IsMatch(password, "[a-z]")
                && Regex.IsMatch(password, "[0-9]")
                && Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]");
        }

        private void ShowError(string message)
        {
            txtErrorMessage.Text = message;
            txtErrorMessage.Visibility = Visibility.Visible;
            txtSuccessMessage.Visibility = Visibility.Collapsed;
        }

        private void ShowSuccess(string message)
        {
            txtSuccessMessage.Text = message;
            txtSuccessMessage.Visibility = Visibility.Visible;
            txtErrorMessage.Visibility = Visibility.Collapsed;
        }

        private void ClearForm()
        {
            txtName.Text = "";
            txtEmail.Text = "";
            txtPassword.Password = "";
            txtConfirmPassword.Password = "";
        }
    }
}