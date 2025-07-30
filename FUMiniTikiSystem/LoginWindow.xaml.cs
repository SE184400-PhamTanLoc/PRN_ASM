using BusinessLayer.Service;
using System;
using System.Windows;
using System.Windows.Input;

namespace FUMiniTikiSystem
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        private bool _isPasswordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();
            var dbContext = new DataAccessLayer.Models.FuminiTikiSystemContext();
            var customerRepo = new DataAccessLayer.Repository.CustomerRepository(dbContext);
            _authService = new AuthService(customerRepo);
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = _isPasswordVisible ? txtPasswordVisible.Text : txtPassword.Password;

            // Validation
            if (string.IsNullOrEmpty(email))
            {
                ShowError("Email is required!");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Password is required!");
                return;
            }

            // TODO: Call AuthService for real authentication
            // For now, just check if email contains "@" and password length > 0
            if (email.Contains("@") && password.Length > 0)
            {
                // Login successful
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ShowError("Email or password is incorrect!");
            }
        }

        private void txtRegister_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Open register window
            RegisterWindow registerWindow = new RegisterWindow(_authService);
            registerWindow.Show();

            // Hide login window
            this.Hide();
        }

        private void btnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            if (_isPasswordVisible)
            {
                txtPasswordVisible.Text = txtPassword.Password;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                iconEye.Text = "🙈";
            }
            else
            {
                txtPassword.Password = txtPasswordVisible.Text;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;
                iconEye.Text = "👁";
            }
        }

        private void ShowError(string message)
        {
            txtErrorMessage.Text = message;
            txtErrorMessage.Visibility = Visibility.Visible;
        }

        private void txtEmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Optional: implement email validation feedback here
        }
    }
}