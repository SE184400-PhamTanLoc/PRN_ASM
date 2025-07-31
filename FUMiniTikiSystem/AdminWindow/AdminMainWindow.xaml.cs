using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BusinessLayer.DTO;
using BusinessLayer.Service;

namespace FUMiniTikiSystem
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        private ProductService _productService;
        private OrderService _orderService;
        private CategoryService _categoryService;

        public AdminMainWindow()
        {
            InitializeComponent();
            _productService = new ProductService();
            _orderService = new OrderService();
            _categoryService = new CategoryService();
            
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                // Load statistics
                LoadStatistics();
                
                // Load recent orders
                LoadRecentOrders();
                
                UpdateStatus("Dashboard loaded successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error loading dashboard data");
            }
        }

        private void LoadStatistics()
        {
            try
            {
                // Get categories count
                var categories = _categoryService.GetAllCategories();
                txtCategoriesCount.Text = categories.Count.ToString();

                // Get products count
                var products = _productService.GetAllProducts();
                txtProductsCount.Text = products.Count.ToString();
            }
            catch (Exception ex)
            {
                txtCategoriesCount.Text = "0";
                txtProductsCount.Text = "0";
                Console.WriteLine($"Error loading statistics: {ex.Message}");
            }
        }

        private void LoadRecentOrders()
        {
            try
            {
                var orders = _orderService.GetAllOrders();
                var recentOrders = orders.Take(3).ToList(); // Get last 3 orders

                if (recentOrders.Any())
                {
                    var orderText = "";
                    foreach (var order in recentOrders)
                    {
                        orderText += $"• Order #{order.OrderId} - {order.FormattedAmount} ({order.Status})\n";
                    }
                    txtRecentOrders.Text = orderText.TrimEnd('\n');
                }
                else
                {
                    txtRecentOrders.Text = "No recent orders";
                }
            }
            catch (Exception ex)
            {
                txtRecentOrders.Text = "Unable to load recent orders";
                Console.WriteLine($"Error loading recent orders: {ex.Message}");
            }
        }

        private void UpdateStatus(string message)
        {
            txtStatus.Text = message;
        }

        private void CategoryCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var categoryWindow = new CategoryManagementWindow();
                categoryWindow.Show();
                this.Close(); // Close the admin main window
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Category Management: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error opening Category Management");
            }
        }

        private void OrderCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var orderWindow = new OrderManagementWindow();
                orderWindow.Show();
                this.Close(); // Close the admin main window
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Order Management: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error opening Order Management");
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during logout: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Add hover effects for cards
        private void Card_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = System.Windows.Media.Colors.Black,
                    Direction = 270,
                    ShadowDepth = 12,
                    BlurRadius = 25,
                    Opacity = 0.25
                };
            }
        }

        private void Card_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = System.Windows.Media.Colors.Black,
                    Direction = 270,
                    ShadowDepth = 8,
                    BlurRadius = 20,
                    Opacity = 0.15
                };
            }
        }
    }
} 