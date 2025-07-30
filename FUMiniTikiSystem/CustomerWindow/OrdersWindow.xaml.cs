using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using BusinessLayer.DTO;
using BusinessLayer.Service;

namespace FUMiniTikiSystem
{
    public partial class OrdersWindow : Window
    {
        private int _customerId;
        private OrderService _orderService;
        private List<OrderDTO> _orders;

        public OrdersWindow(int customerId)
        {
            InitializeComponent();
            _customerId = customerId;
            _orderService = new OrderService();
            _orders = new List<OrderDTO>();
            
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                _orders = _orderService.GetOrdersByCustomer(_customerId);
                DisplayOrders();
                UpdateOrderCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayOrders()
        {
            spOrders.Children.Clear();
            
            if (_orders.Count == 0)
            {
                var emptyText = new TextBlock
                {
                    Text = "You haven't placed any orders yet.",
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    Margin = new Thickness(0, 50, 0, 0)
                };
                spOrders.Children.Add(emptyText);
                return;
            }

            foreach (var order in _orders.OrderByDescending(o => o.OrderDate))
            {
                var orderCard = CreateOrderCard(order);
                spOrders.Children.Add(orderCard);
            }
        }

        private Border CreateOrderCard(OrderDTO order)
        {
            var card = new Border
            {
                Style = FindResource("OrderCardStyle") as Style
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Order Header
            var headerPanel = new Grid();
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var orderIdText = new TextBlock
            {
                Text = $"Order #{order.OrderId}",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(orderIdText, 0);
            headerPanel.Children.Add(orderIdText);

            var dateText = new TextBlock
            {
                Text = order.OrderDate?.ToString("MMM dd, yyyy 'at' HH:mm"),
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(dateText, 1);
            headerPanel.Children.Add(dateText);

            var statusBadge = CreateStatusBadge(order.Status);
            Grid.SetColumn(statusBadge, 2);
            headerPanel.Children.Add(statusBadge);

            Grid.SetRow(headerPanel, 0);
            grid.Children.Add(headerPanel);

            // Order Details
            var detailsPanel = new StackPanel
            {
                Margin = new Thickness(0, 15, 0, 0)
            };

            var amountText = new TextBlock
            {
                Text = $"Total Amount: ${order.OrderAmount:N2}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 87, 34)),
                Margin = new Thickness(0, 0, 0, 10)
            };
            detailsPanel.Children.Add(amountText);

            var productCountText = new TextBlock
            {
                Text = $"Items: {order.ProductIds.Count} products",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray)
            };
            detailsPanel.Children.Add(productCountText);

            Grid.SetRow(detailsPanel, 1);
            grid.Children.Add(detailsPanel);

            // Order Actions
            var actionsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 15, 0, 0)
            };

            var btnTrackOrder = new Button
            {
                Content = "📦 Track Order",
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                Foreground = Brushes.White,
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(0, 0, 10, 0),
                Tag = order,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                BorderThickness = new Thickness(0),
                Template = CreateModernButtonTemplate()
            };
            btnTrackOrder.Click += BtnTrackOrder_Click;
            actionsPanel.Children.Add(btnTrackOrder);

            var btnViewDetails = new Button
            {
                Content = "👁️ View Details",
                Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                Foreground = Brushes.White,
                Padding = new Thickness(12, 8, 12, 8),
                Tag = order,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                BorderThickness = new Thickness(0),
                Template = CreateModernButtonTemplate()
            };
            btnViewDetails.Click += BtnViewDetails_Click;
            actionsPanel.Children.Add(btnViewDetails);

            Grid.SetRow(actionsPanel, 2);
            grid.Children.Add(actionsPanel);

            card.Child = grid;
            return card;
        }

        private Border CreateStatusBadge(string status)
        {
            var badge = new Border
            {
                Style = FindResource("StatusBadgeStyle") as Style
            };

            Color badgeColor;
            switch (status.ToLower())
            {
                case "paid":
                    badgeColor = Color.FromRgb(76, 175, 80); // Green
                    break;
                case "pending payment":
                    badgeColor = Color.FromRgb(255, 152, 0); // Orange
                    break;
                case "shipped":
                    badgeColor = Color.FromRgb(33, 150, 243); // Blue
                    break;
                case "delivered":
                    badgeColor = Color.FromRgb(76, 175, 80); // Green
                    break;
                case "cancelled":
                    badgeColor = Color.FromRgb(244, 67, 54); // Red
                    break;
                default:
                    badgeColor = Color.FromRgb(158, 158, 158); // Gray
                    break;
            }

            badge.Background = new SolidColorBrush(badgeColor);

            var statusText = new TextBlock
            {
                Text = status.ToUpper(),
                Foreground = Brushes.White,
                FontSize = 12,
                FontWeight = FontWeights.Bold
            };

            badge.Child = statusText;
            return badge;
        }

        private void UpdateOrderCount()
        {
            txtOrderCount.Text = $"{_orders.Count} order(s) found";
        }

        private void BtnTrackOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OrderDTO order)
            {
                var trackingInfo = GetTrackingInfo(order);
                MessageBox.Show(trackingInfo, $"Order #{order.OrderId} Tracking", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OrderDTO order)
            {
                var orderDetails = GetOrderDetails(order);
                MessageBox.Show(orderDetails, $"Order #{order.OrderId} Details", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private string GetTrackingInfo(OrderDTO order)
        {
            var status = order.Status.ToLower();
            var trackingSteps = new List<string>();

            switch (status)
            {
                case "paid":
                    trackingSteps.AddRange(new[]
                    {
                        "✅ Order Confirmed",
                        "✅ Payment Received",
                        "⏳ Processing Order",
                        "⏳ Preparing for Shipment",
                        "⏳ In Transit",
                        "⏳ Out for Delivery",
                        "⏳ Delivered"
                    });
                    break;
                case "pending payment":
                    trackingSteps.AddRange(new[]
                    {
                        "✅ Order Placed",
                        "⏳ Awaiting Payment",
                        "⏳ Processing Order",
                        "⏳ Preparing for Shipment",
                        "⏳ In Transit",
                        "⏳ Out for Delivery",
                        "⏳ Delivered"
                    });
                    break;
                case "shipped":
                    trackingSteps.AddRange(new[]
                    {
                        "✅ Order Confirmed",
                        "✅ Payment Received",
                        "✅ Processing Order",
                        "✅ Preparing for Shipment",
                        "✅ In Transit",
                        "⏳ Out for Delivery",
                        "⏳ Delivered"
                    });
                    break;
                case "delivered":
                    trackingSteps.AddRange(new[]
                    {
                        "✅ Order Confirmed",
                        "✅ Payment Received",
                        "✅ Processing Order",
                        "✅ Preparing for Shipment",
                        "✅ In Transit",
                        "✅ Out for Delivery",
                        "✅ Delivered"
                    });
                    break;
                default:
                    trackingSteps.Add("Order status: " + order.Status);
                    break;
            }

            return string.Join("\n", trackingSteps);
        }

        private string GetOrderDetails(OrderDTO order)
        {
            var details = $"Order ID: #{order.OrderId}\n";
            details += $"Date: {order.OrderDate:MMM dd, yyyy 'at' HH:mm}\n";
            details += $"Status: {order.Status}\n";
            details += $"Total Amount: ${order.OrderAmount:N2}\n";
            details += $"Items: {order.ProductIds.Count} products\n\n";
            details += "Product IDs: " + string.Join(", ", order.ProductIds);

            return details;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private ControlTemplate CreateModernButtonTemplate()
        {
            var template = new ControlTemplate(typeof(Button));
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
            border.SetValue(Border.PaddingProperty, new TemplateBindingExtension(Button.PaddingProperty));

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);
            template.VisualTree = border;

            // Add triggers for hover and pressed states
            var trigger1 = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            var setter1 = new Setter { Property = Border.BackgroundProperty, Value = new SolidColorBrush(Color.FromRgb(25, 118, 210)) }; // Darker blue
            trigger1.Setters.Add(setter1);

            var trigger2 = new Trigger { Property = Button.IsPressedProperty, Value = true };
            var setter2 = new Setter { Property = Border.BackgroundProperty, Value = new SolidColorBrush(Color.FromRgb(21, 101, 192)) }; // Even darker blue
            trigger2.Setters.Add(setter2);

            template.Triggers.Add(trigger1);
            template.Triggers.Add(trigger2);

            return template;
        }
    }
} 