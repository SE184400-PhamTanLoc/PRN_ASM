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
    public partial class CartWindow : Window
    {
        private List<CartItemDTO> _cartItems;
        private int _customerId;
        private OrderService _orderService;

        public CartWindow(List<CartItemDTO> cartItems, int customerId)
        {
            InitializeComponent();
            _cartItems = cartItems;
            _customerId = customerId;
            _orderService = new OrderService();
            
            LoadCartItems();
            UpdateTotal();
        }

        private void LoadCartItems()
        {
            spCartItems.Children.Clear();
            
            if (_cartItems.Count == 0)
            {
                var emptyText = new TextBlock
                {
                    Text = "Your cart is empty",
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Gray)
                };
                spCartItems.Children.Add(emptyText);
                return;
            }

            foreach (var item in _cartItems)
            {
                var cartItemCard = CreateCartItemCard(item);
                spCartItems.Children.Add(cartItemCard);
            }

            txtItemCount.Text = $"{_cartItems.Count} items in cart";
        }

        private Border CreateCartItemCard(CartItemDTO item)
        {
            var card = new Border
            {
                Style = FindResource("CartItemStyle") as Style
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Product Image Placeholder
            var imageBorder = new Border
            {
                Width = 80,
                Height = 80,
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(0, 0, 15, 0)
            };
            
            var imageText = new TextBlock
            {
                Text = "🖼️",
                FontSize = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            imageBorder.Child = imageText;
            Grid.SetColumn(imageBorder, 0);
            grid.Children.Add(imageBorder);

            // Product Details
            var detailsPanel = new StackPanel();
            
            var nameText = new TextBlock
            {
                Text = item.ProductName,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };
            detailsPanel.Children.Add(nameText);

            var priceText = new TextBlock
            {
                Text = $"${item.Price:N2} each",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 87, 34))
            };
            detailsPanel.Children.Add(priceText);

            Grid.SetColumn(detailsPanel, 1);
            grid.Children.Add(detailsPanel);

            // Quantity Controls
            var quantityPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var btnMinus = new Button
            {
                Content = "-",
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                Foreground = Brushes.White,
                Width = 28,
                Height = 28,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                BorderThickness = new Thickness(0),
                Tag = item,
                Template = CreateModernButtonTemplate()
            };
            btnMinus.Click += BtnMinus_Click;
            quantityPanel.Children.Add(btnMinus);

            var quantityText = new TextBlock
            {
                Text = item.Quantity.ToString(),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 10, 0)
            };
            quantityPanel.Children.Add(quantityText);

            var btnPlus = new Button
            {
                Content = "+",
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                Foreground = Brushes.White,
                Width = 28,
                Height = 28,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                BorderThickness = new Thickness(0),
                Tag = item,
                Template = CreateModernButtonTemplate()
            };
            btnPlus.Click += BtnPlus_Click;
            quantityPanel.Children.Add(btnPlus);

            Grid.SetColumn(quantityPanel, 2);
            grid.Children.Add(quantityPanel);

            // Total and Remove
            var totalPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var totalText = new TextBlock
            {
                Text = $"${item.Total:N2}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 0, 5)
            };
            totalPanel.Children.Add(totalText);

            var btnRemove = new Button
            {
                Content = "🗑️ Remove",
                Background = new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                Foreground = Brushes.White,
                Padding = new Thickness(10, 6, 10, 6),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                BorderThickness = new Thickness(0),
                Tag = item,
                Template = CreateModernButtonTemplate()
            };
            btnRemove.Click += BtnRemove_Click;
            totalPanel.Children.Add(btnRemove);

            Grid.SetColumn(totalPanel, 3);
            grid.Children.Add(totalPanel);

            card.Child = grid;
            return card;
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CartItemDTO item)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    LoadCartItems();
                    UpdateTotal();
                }
            }
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CartItemDTO item)
            {
                item.Quantity++;
                LoadCartItems();
                UpdateTotal();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CartItemDTO item)
            {
                _cartItems.Remove(item);
                LoadCartItems();
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            var total = _cartItems.Sum(item => item.Total);
            txtTotal.Text = $"Total: ${total:N2}";
        }

        private void btnClearCart_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear your cart?", 
                "Clear Cart", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _cartItems.Clear();
                LoadCartItems();
                UpdateTotal();
            }
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                MessageBox.Show("Your cart is empty!");
                return;
            }

            var paymentWindow = new PaymentWindow(_cartItems, _customerId);
            if (paymentWindow.ShowDialog() == true)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private ControlTemplate CreateModernButtonTemplate()
        {
            var template = new ControlTemplate(typeof(Button));
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(6));
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