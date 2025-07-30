using BusinessLayer.Service;
using BusinessLayer.DTO;
using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;

namespace FUMiniTikiSystem
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        private bool _isPasswordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();

            // Khởi tạo configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Khởi tạo repositories và service
            var dbContext = new DataAccessLayer.Models.FuminiTikiSystemContext();
            var customerRepo = new DataAccessLayer.Repository.CustomerRepository(dbContext);
            var adminRepo = new DataAccessLayer.Repository.AdminRepository(configuration);
            _authService = new AuthService(customerRepo, adminRepo);
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

            // Call AuthService with DTO
            var dto = new CustomerLoginDTO
            {
                Email = email,
                Password = password
            };

            var result = await _authService.LoginAsync(dto);

            if (result.IsSuccess)
            {
                // Check if user is admin
                if (result.IsAdmin)
                {
                    // Admin login - redirect to ProductManagement
                    //ProductManagementWindow productManagementWindow = new ProductManagementWindow();
                    //productManagementWindow.Show();
                }
                         else
                 {
                     // Regular user login - redirect to MainWindow
                     if (result.CustomerId.HasValue)
                     {
                         MainWindow mainWindow = new MainWindow(result.CustomerId.Value);
                         mainWindow.Show();
                     }
                     else
                     {
                         ShowError("Login failed: Customer ID not found");
                         return;
                     }
                 }

                this.Close();
            }
            else
            {
                ShowError(result.Message);
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