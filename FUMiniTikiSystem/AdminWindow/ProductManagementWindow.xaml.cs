using BusinessLayer.DTO;
using BusinessLayer.Service;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace FUMiniTikiSystem
{
    public partial class ProductManagementWindow : Window
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private List<ProductDTO> _products = new List<ProductDTO>();
        private ProductDTO? _selectedProduct;
        private CategoryDTO _currentCategory;
        private bool _isEditMode = false;

        public ProductManagementWindow(CategoryDTO category)
        {
            InitializeComponent();
            _productService = new ProductService();
            _categoryService = new CategoryService();
            _currentCategory = category;
            
            // Update header to show current category
            CategoryInfoText.Text = $"Category: {_currentCategory.Name} - Create, update, and delete products";
            
            LoadProducts();
            ClearForm();
        }

        private void LoadProducts()
        {
            try
            {
                _products = _productService.GetProductsByCategory(_currentCategory.CategoryId);
                ProductListView.ItemsSource = _products;
                UpdateStatus($"Loaded {_products.Count} products for category: {_currentCategory.Name}");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error loading products");
            }
        }

        private void ClearForm()
        {
            _isEditMode = false;
            _selectedProduct = null;

            NameTextBox.Clear();
            PriceTextBox.Clear();
            DescriptionTextBox.Clear();

            FormTitle.Text = "Add New Product";
            SaveButton.Content = "Save";

            // Enable/disable buttons based on mode
            ResetButton.IsEnabled = true;
            SaveButton.IsEnabled = true; // Enable Save button for adding new
            UpdateButton.Visibility = Visibility.Collapsed; // Hide Update button

            ProductListView.SelectedItem = null;
            UpdateStatus($"Ready to add new product to category: {_currentCategory.Name}");
        }

        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = message;
        }

        private void ProductListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ProductListView.SelectedItem as ProductDTO;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create and show CategoryManagementWindow
                var categoryWindow = new CategoryManagementWindow();
                categoryWindow.Show();
                this.Close(); // Close the current product management window
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error opening Category Management: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error opening Category Management");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Please select a product to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isEditMode = true;
            FormTitle.Text = "Edit Product";

            // Populate form with selected product data
            NameTextBox.Text = _selectedProduct.Name;
            PriceTextBox.Text = _selectedProduct.Price.ToString("F2");
            DescriptionTextBox.Text = _selectedProduct.Description ?? "";

            // Enable/disable buttons based on mode
            ResetButton.IsEnabled = false;
            SaveButton.IsEnabled = false; // Disable Save button in edit mode
            UpdateButton.Visibility = Visibility.Visible; // Show Update button

            UpdateStatus($"Editing product: {_selectedProduct.Name} - Click Update to save changes");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Please select a product to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the product '{_selectedProduct.Name}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _productService.DeleteProduct(_selectedProduct.ProductId);

                    if (success)
                    {
                        MessageBox.Show($"Product '{_selectedProduct.Name}' has been deleted successfully.",
                                   "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        string deletedProductName = _selectedProduct.Name;
                        LoadProducts();
                        ClearForm();
                        UpdateStatus($"Deleted product: {deletedProductName}");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete product. It may be associated with an order.",
                                   "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateStatus("Error deleting product");
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Product name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if (NameTextBox.Text.Trim().Length < 2)
            {
                MessageBox.Show("Product name must be at least 2 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                MessageBox.Show("Product price is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid positive price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return;
            }

            var productDTO = new ProductDTO
            {
                Name = NameTextBox.Text.Trim(),
                Price = price,
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim(),
                CategoryId = _currentCategory.CategoryId
            };

            try
            {
                bool success = false;
                string message = "";

                if (_isEditMode)
                {
                    productDTO.ProductId = _selectedProduct.ProductId;
                    success = _productService.UpdateProduct(productDTO);
                    message = success ? "Product updated successfully." : "Failed to update product. Product name might already exist.";
                }
                else
                {
                    success = _productService.AddProduct(productDTO);
                    message = success ? "Product added successfully." : "Failed to add product. Product name might already exist.";
                }

                if (success)
                {
                    MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProducts();
                    ClearForm();
                    UpdateStatus(_isEditMode ? "Product updated" : "Product added");
                }
                else
                {
                    MessageBox.Show(message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error saving product");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("No product selected for update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validation
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Product name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if (NameTextBox.Text.Trim().Length < 2)
            {
                MessageBox.Show("Product name must be at least 2 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                MessageBox.Show("Product price is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid positive price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return;
            }

            var productDTO = new ProductDTO
            {
                ProductId = _selectedProduct.ProductId,
                Name = NameTextBox.Text.Trim(),
                Price = price,
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim(),
                CategoryId = _currentCategory.CategoryId
            };

            try
            {
                bool success = _productService.UpdateProduct(productDTO);

                if (success)
                {
                    MessageBox.Show("Product updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProducts();
                    ClearForm();
                    UpdateStatus("Product updated");
                }
                else
                {
                    MessageBox.Show("Failed to update product. Product name might already exist.", "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error updating product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error updating product");
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            NameTextBox.Focus(); // Focus vào textbox đầu tiên
            UpdateStatus($"Ready to add new product to category: {_currentCategory.Name}");
        }
    }
} 