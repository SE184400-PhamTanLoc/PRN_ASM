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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProductService _productService;
        private OrderService _orderService;
        private List<ProductDTO> _allProducts = new List<ProductDTO>();
        private List<CategoryDTO> _categories = new List<CategoryDTO>();
        private List<CartItemDTO> _cartItems;
        private int _currentCustomerId;

        public MainWindow(int customerId)
        {
            InitializeComponent();
            _currentCustomerId = customerId;
            _productService = new ProductService();
            _orderService = new OrderService();
            _cartItems = new List<CartItemDTO>();
            
            LoadData();
            LoadCategories();
            LoadProducts();
        }

        private void LoadData()
        {
            try
            {
                _allProducts = _productService.GetAllProducts();
                _categories = _productService.GetAllCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCategories()
        {
            spCategories.Children.Clear();
            
            // Add "All Categories" button
            var btnAll = new Button
            {
                Content = "All Categories",
                Style = FindResource("CategoryButtonStyle") as Style,
                Tag = 0
            };
            btnAll.Click += CategoryButton_Click;
            spCategories.Children.Add(btnAll);

            foreach (var category in _categories)
            {
                var btn = new Button
                {
                    Content = category.Name,
                    Style = FindResource("CategoryButtonStyle") as Style,
                    Tag = category.CategoryId
                };
                btn.Click += CategoryButton_Click;
                spCategories.Children.Add(btn);
            }
        }

        private void LoadProducts(List<ProductDTO> products = null)
        {
            gridProducts.Children.Clear();
            gridProducts.RowDefinitions.Clear();

            var productsToShow = products ?? _allProducts;
            if (productsToShow == null || !productsToShow.Any()) return;

            int columns = 4;
            int rows = (int)Math.Ceiling((double)productsToShow.Count / columns);

            for (int i = 0; i < rows; i++)
            {
                gridProducts.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            for (int i = 0; i < productsToShow.Count; i++)
            {
                int row = i / columns;
                int col = i % columns;

                var productCard = CreateProductCard(productsToShow[i]);
                Grid.SetRow(productCard, row);
                Grid.SetColumn(productCard, col);
                gridProducts.Children.Add(productCard);
            }
        }

        private Border CreateProductCard(ProductDTO product)
        {
            var card = new Border
            {
                Style = FindResource("ProductCardStyle") as Style,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var stackPanel = new StackPanel();
            
            // Product Image Placeholder
            var imageBorder = new Border
            {
                Height = 120,
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(0, 0, 0, 10)
            };
            
            var imageText = new TextBlock
            {
                Text = "🖼️",
                FontSize = 40,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            imageBorder.Child = imageText;
            stackPanel.Children.Add(imageBorder);

            // Product Name
            var nameText = new TextBlock
            {
                Text = product.Name,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };
            stackPanel.Children.Add(nameText);

            // Category
            var categoryText = new TextBlock
            {
                Text = product.CategoryName,
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray),
                Margin = new Thickness(0, 0, 0, 5)
            };
            stackPanel.Children.Add(categoryText);

            // Price
            var priceText = new TextBlock
            {
                Text = $"${product.Price:N2}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 87, 34)),
                Margin = new Thickness(0, 0, 0, 10)
            };
            stackPanel.Children.Add(priceText);

            // Add to Cart Button
            var addToCartBtn = new Button
            {
                Content = "Add to Cart",
                Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                Foreground = Brushes.White,
                Padding = new Thickness(12, 8, 12, 8),
                Tag = product.ProductId,
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                BorderThickness = new Thickness(0),
                Template = CreateModernButtonTemplate()
            };
            addToCartBtn.Click += AddToCart_Click;
            stackPanel.Children.Add(addToCartBtn);

            card.Child = stackPanel;
            return card;
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int categoryId)
            {
                var products = categoryId == 0 
                    ? _allProducts 
                    : _productService.GetProductsByCategory(categoryId);
                LoadProducts(products);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var keyword = txtSearch.Text?.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadProducts();
                return;
            }

            var searchResults = _productService.SearchProducts(keyword);
            LoadProducts(searchResults);
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productId)
            {
                var product = _allProducts.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    var existingItem = _cartItems.FirstOrDefault(item => item.ProductId == productId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity++;
                    }
                    else
                    {
                        _cartItems.Add(new CartItemDTO
                        {
                            ProductId = product.ProductId,
                            ProductName = product.Name,
                            Price = product.Price,
                            Quantity = 1
                        });
                    }
                    
                    UpdateCartButton();
                    MessageBox.Show($"{product.Name} added to cart!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void UpdateCartButton()
        {
            var totalItems = _cartItems.Sum(item => item.Quantity);
            btnCart.Content = $"🛒 Cart ({totalItems})";
        }

        private void btnCart_Click(object sender, RoutedEventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                MessageBox.Show("Your cart is empty!", "Cart", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var cartWindow = new CartWindow(_cartItems, _currentCustomerId);
            cartWindow.ShowDialog();

            // Refresh cart after checkout
            _cartItems.Clear();
            UpdateCartButton();
        }

        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrdersWindow(_currentCustomerId);
            ordersWindow.ShowDialog();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
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
            var setter1 = new Setter { Property = Border.BackgroundProperty, Value = new SolidColorBrush(Color.FromRgb(69, 160, 73)) }; // Darker green
            trigger1.Setters.Add(setter1);

            var trigger2 = new Trigger { Property = Button.IsPressedProperty, Value = true };
            var setter2 = new Setter { Property = Border.BackgroundProperty, Value = new SolidColorBrush(Color.FromRgb(56, 142, 60)) }; // Even darker green
            trigger2.Setters.Add(setter2);

            template.Triggers.Add(trigger1);
            template.Triggers.Add(trigger2);

            return template;
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}