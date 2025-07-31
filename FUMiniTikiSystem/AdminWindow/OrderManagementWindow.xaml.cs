using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BusinessLayer.DTO;
using BusinessLayer.Service;

namespace FUMiniTikiSystem
{
    /// <summary>
    /// Interaction logic for OrderManagementWindow.xaml
    /// </summary>
    public partial class OrderManagementWindow : Window
    {
        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        private List<OrderDTO> _allOrders = new List<OrderDTO>();
        private OrderDTO? _selectedOrder;

        public OrderManagementWindow()
        {
            try
            {
                InitializeComponent();
                _orderService = new OrderService();
                _productService = new ProductService();
                
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing Order Management: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrders()
        {
            try
            {
                _allOrders = _orderService.GetAllOrders() ?? new List<OrderDTO>();
                if (OrderListView != null)
                {
                    OrderListView.ItemsSource = _allOrders;
                }
                UpdateStatus($"Loaded {_allOrders.Count} orders");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error loading orders");
            }
        }

        private void UpdateStatus(string message)
        {
            if (StatusTextBlock != null)
            {
                StatusTextBlock.Text = message;
            }
        }

        private void OrderListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedOrder = OrderListView?.SelectedItem as OrderDTO;
            
            if (_selectedOrder != null)
            {
                DisplayOrderDetails(_selectedOrder);
                EnableOrderActions(true);
            }
            else
            {
                ClearOrderDetails();
                EnableOrderActions(false);
            }
        }

        private void DisplayOrderDetails(OrderDTO order)
        {
            if (order == null) return;
            
            if (txtOrderId != null) txtOrderId.Text = $"#{order.OrderId}";
            if (txtCustomerInfo != null) txtCustomerInfo.Text = $"{order.CustomerName}\n{order.CustomerEmail}";
            if (txtOrderDate != null) txtOrderDate.Text = order.FormattedOrderDate;
            if (txtAmount != null) txtAmount.Text = order.FormattedAmount;
            
            // Load product details
            LoadProductDetails(order);
            
            // Set current status in combo box
            SetStatusInComboBox(order.Status);
        }

        private void LoadProductDetails(OrderDTO order)
        {
            if (order?.ProductIds == null || txtProducts == null) return;
            
            try
            {
                var products = _productService.GetAllProducts();
                var orderProducts = products.Where(p => order.ProductIds.Contains(p.ProductId)).ToList();
                
                if (orderProducts.Any())
                {
                    var productText = "";
                    foreach (var product in orderProducts)
                    {
                        productText += $"• {product.Name} - ${product.Price:N2}\n";
                    }
                    txtProducts.Text = productText.TrimEnd('\n');
                }
                else
                {
                    txtProducts.Text = "No products found";
                }
            }
            catch (Exception ex)
            {
                txtProducts.Text = "Error loading products";
                Console.WriteLine($"Error loading product details: {ex.Message}");
            }
        }

        private void SetStatusInComboBox(string status)
        {
            if (StatusUpdateComboBox?.Items == null) return;
            
            foreach (ComboBoxItem item in StatusUpdateComboBox.Items)
            {
                if (item?.Content?.ToString().Equals(status, StringComparison.OrdinalIgnoreCase) == true)
                {
                    StatusUpdateComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void ClearOrderDetails()
        {
            if (txtOrderId != null) txtOrderId.Text = "Select an order";
            if (txtCustomerInfo != null) txtCustomerInfo.Text = "N/A";
            if (txtOrderDate != null) txtOrderDate.Text = "N/A";
            if (txtAmount != null) txtAmount.Text = "N/A";
            if (txtProducts != null) txtProducts.Text = "N/A";
            if (StatusUpdateComboBox != null) StatusUpdateComboBox.SelectedIndex = -1;
        }

        private void EnableOrderActions(bool enable)
        {
            if (StatusUpdateComboBox != null) StatusUpdateComboBox.IsEnabled = enable;
            if (UpdateStatusButton != null) UpdateStatusButton.IsEnabled = enable;
            if (ViewDetailsButton != null) ViewDetailsButton.IsEnabled = enable;
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusFilterComboBox?.SelectedItem is ComboBoxItem selectedItem && OrderListView != null)
            {
                var selectedStatus = selectedItem.Content?.ToString() ?? string.Empty;
                
                if (selectedStatus == "All Status")
                {
                    OrderListView.ItemsSource = _allOrders;
                }
                else
                {
                    var filteredOrders = _allOrders.Where(o => 
                        o.Status.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase)).ToList();
                    OrderListView.ItemsSource = filteredOrders;
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void UpdateStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Please select an order to update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StatusUpdateComboBox?.SelectedItem is ComboBoxItem selectedItem)
            {
                var newStatus = selectedItem.Content?.ToString() ?? string.Empty;
                
                if (newStatus == _selectedOrder.Status)
                {
                    MessageBox.Show("The status is already set to this value.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to update the status of Order #{_selectedOrder.OrderId} from '{_selectedOrder.Status}' to '{newStatus}'?",
                    "Confirm Status Update",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = _orderService.UpdateOrderStatus(_selectedOrder.OrderId, newStatus);
                        
                        if (success)
                        {
                            MessageBox.Show($"Order status updated successfully to '{newStatus}'.", 
                                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            
                            // Refresh the orders list
                            LoadOrders();
                            
                            // Update the selected order
                            //_selectedOrder.Status = newStatus;
                            //DisplayOrderDetails(_selectedOrder);
                            
                            //UpdateStatus($"Order #{_selectedOrder.OrderId} status updated to {newStatus}");
                        }
                        else
                        {
                            MessageBox.Show("Failed to update order status. Please try again.", 
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating order status: {ex.Message}", 
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        UpdateStatus("Error updating order status");
                    }
                }
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Please select an order to view details.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var detailsMessage = $"Order Details:\n\n" +
                               $"Order ID: #{_selectedOrder.OrderId}\n" +
                               $"Customer: {_selectedOrder.CustomerName}\n" +
                               $"Email: {_selectedOrder.CustomerEmail}\n" +
                               $"Order Date: {_selectedOrder.FormattedOrderDate}\n" +
                               $"Total Amount: {_selectedOrder.FormattedAmount}\n" +
                               $"Status: {_selectedOrder.Status}\n" +
                               $"Products: {_selectedOrder.ProductCount} items\n\n" +
                               $"Product IDs: {string.Join(", ", _selectedOrder.ProductIds)}";

            MessageBox.Show(detailsMessage, $"Order #{_selectedOrder.OrderId} Details", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BackToDashboardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create and show AdminMainWindow
                var adminMainWindow = new AdminMainWindow();
                adminMainWindow.Show();
                this.Close(); // Close the current order management window
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Admin Dashboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error opening Admin Dashboard");
            }
        }
    }
} 