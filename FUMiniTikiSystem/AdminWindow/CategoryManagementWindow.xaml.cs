using BusinessLayer.DTO;
using BusinessLayer.Service;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FUMiniTikiSystem
{
    public partial class CategoryManagementWindow : Window
    {
        private readonly CategoryService _categoryService;
        private List<CategoryDTO> _categories = new List<CategoryDTO>();
        private CategoryDTO? _selectedCategory;
        private bool _isEditMode = false;
        private CategoryDTO? _lastAddedCategory = null; // Track the last added category

        public CategoryManagementWindow()
        {
            InitializeComponent();
            _categoryService = new CategoryService();
            LoadCategories();
            ClearForm();
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _categoryService.GetAllCategories();
                CategoryListView.ItemsSource = _categories;
                UpdateStatus($"Loaded {_categories.Count} categories");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error loading categories");
            }
        }

        private void ClearForm()
        {
            _isEditMode = false;
            _selectedCategory = null;

            NameTextBox.Clear();
            DescriptionTextBox.Clear();
            PictureTextBox.Clear();

            FormTitle.Text = "Add New Category";
            SaveButton.Content = "Save";

            // Enable/disable buttons based on mode
            ResetButton.IsEnabled = true;
            SaveButton.IsEnabled = true; // Enable Save button for adding new
            UpdateButton.Visibility = Visibility.Collapsed; // Hide Update button

            CategoryListView.SelectedItem = null;
            UpdateStatus("Ready to add new category");
        }

        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = message;
        }



        private void CategoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = CategoryListView.SelectedItem as CategoryDTO;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Please select a category to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isEditMode = true;
            FormTitle.Text = "Edit Category";

            // Populate form with selected category data
            NameTextBox.Text = _selectedCategory.Name;
            DescriptionTextBox.Text = _selectedCategory.Description ?? "";
            PictureTextBox.Text = _selectedCategory.Picture ?? "";

            // Enable/disable buttons based on mode
            ResetButton.IsEnabled = false;
            SaveButton.IsEnabled = false; // Disable Save button in edit mode
            UpdateButton.Visibility = Visibility.Visible; // Show Update button

            UpdateStatus($"Editing category: {_selectedCategory.Name} - Click Update to save changes");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Please select a category to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the category '{_selectedCategory.Name}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _categoryService.DeleteCategory(_selectedCategory.CategoryId);

                    if (success)
                    {
                        MessageBox.Show($"Category '{_selectedCategory.Name}' has been deleted successfully.",
                                       "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        string deletedCategoryName = _selectedCategory.Name;
                        LoadCategories();
                        ClearForm();
                        UpdateStatus($"Deleted category: {deletedCategoryName}");

                    }
                    else
                    {
                        MessageBox.Show("Cannot delete this category because it contains products. Please remove all products from this category first.",
                                       "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Error deleting category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateStatus("Error deleting category");
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Category name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if (NameTextBox.Text.Trim().Length < 2)
            {
                MessageBox.Show("Category name must be at least 2 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            // Validate image URL if provided
            if (!string.IsNullOrWhiteSpace(PictureTextBox.Text))
            {
                if (!PictureTextBox.Text.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !PictureTextBox.Text.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
                    !PictureTextBox.Text.StartsWith("/images/", StringComparison.OrdinalIgnoreCase))
                {
                    var result = MessageBox.Show("The image URL doesn't look like a valid URL or local path. Do you want to continue?",
                                               "URL Validation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        PictureTextBox.Focus();
                        return;
                    }
                }
            }

            var categoryDTO = new CategoryDTO
            {
                Name = NameTextBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim(),
                Picture = string.IsNullOrWhiteSpace(PictureTextBox.Text) ? null : PictureTextBox.Text.Trim()
            };

            try
            {
                bool success = false;
                string message = "";

                if (_isEditMode)
                {
                    categoryDTO.CategoryId = _selectedCategory.CategoryId;
                    success = _categoryService.UpdateCategory(categoryDTO);
                    message = success ? "Category updated successfully." : "Failed to update category. Category name might already exist.";
                }
                else
                {
                    success = _categoryService.AddCategory(categoryDTO);
                    message = success ? "Category added successfully." : "Failed to add category. Category name might already exist.";
                }

                if (success)
                {
                    MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // If this was a new category, store it for potential product addition
                    if (!_isEditMode)
                    {
                        _lastAddedCategory = categoryDTO;
                    }
                    
                    LoadCategories();
                    
                    // If this was a new category, automatically select it
                    if (!_isEditMode && _lastAddedCategory != null)
                    {
                        // Find and select the newly added category
                        var newlyAddedCategory = _categories.FirstOrDefault(c => c.Name == _lastAddedCategory.Name);
                        if (newlyAddedCategory != null)
                        {
                            CategoryListView.SelectedItem = newlyAddedCategory;
                            _selectedCategory = newlyAddedCategory;
                            
                            // Ask user if they want to add products to this category
                            var addProductResult = MessageBox.Show(
                                $"Category '{newlyAddedCategory.Name}' has been added successfully!\n\nWould you like to add products to this category now?",
                                "Add Products",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question);
                            
                            if (addProductResult == MessageBoxResult.Yes)
                            {
                                AddProductButton_Click(null, null);
                            }
                        }
                    }
                    else
                    {
                        ClearForm();
                    }
                    
                    UpdateStatus(_isEditMode ? "Category updated" : "Category added");
                }
                else
                {
                    MessageBox.Show(message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error saving category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error saving category");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Same logic as SaveButton_Click but specifically for update
            if (_selectedCategory == null)
            {
                MessageBox.Show("No category selected for update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validation
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Category name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if (NameTextBox.Text.Trim().Length < 2)
            {
                MessageBox.Show("Category name must be at least 2 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            var categoryDTO = new CategoryDTO
            {
                CategoryId = _selectedCategory.CategoryId,
                Name = NameTextBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim(),
                Picture = string.IsNullOrWhiteSpace(PictureTextBox.Text) ? null : PictureTextBox.Text.Trim()
            };

            try
            {
                bool success = _categoryService.UpdateCategory(categoryDTO);

                if (success)
                {
                    MessageBox.Show("Category updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCategories();
                    ClearForm();
                    UpdateStatus("Category updated");
                }
                else
                {
                    MessageBox.Show("Failed to update category. Category name might already exist.", "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error updating category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error updating category");
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            NameTextBox.Focus(); // Focus vào textbox đầu tiên
            UpdateStatus("Ready to add new category - Enter category name");
        }

        private void SuggestImageButton_Click(object sender, RoutedEventArgs e)
        {
            var imageUrls = new[]
            {
                "https://picsum.photos/200/200?random=1",
                "https://picsum.photos/200/200?random=2",
                "https://picsum.photos/200/200?random=3",
                "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=200&h=200&fit=crop",
                "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=200&h=200&fit=crop",
                "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=200&h=200&fit=crop"
            };

            var random = new Random();
            var randomUrl = imageUrls[random.Next(imageUrls.Length)];
            PictureTextBox.Text = randomUrl;
            UpdateStatus("Suggested image URL added");
        }

        // Preview image when URL changes
        private void PictureTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PictureTextBox.Text))
            {
                UpdateStatus("Typing...");
            }
            else
            {
                UpdateStatus("Ready");
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Please select a category first to add products to.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Create and show ProductManagementWindow with the selected category
                var productWindow = new ProductManagementWindow(_selectedCategory);
                productWindow.Show();
                this.Close(); // Close the category management window
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error opening Product Management: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error opening Product Management");
            }
        }

        private void BackToDashboardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create and show AdminMainWindow
                var adminMainWindow = new AdminMainWindow();
                adminMainWindow.Show();
                this.Close(); // Close the current category management window
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error opening Admin Dashboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Error opening Admin Dashboard");
            }
        }
    }
}