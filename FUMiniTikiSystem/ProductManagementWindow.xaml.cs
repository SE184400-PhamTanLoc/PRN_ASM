using BusinessLayer.Service;
using BusinessLayer.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FUMiniTikiSystem
{
    public partial class ProductManagementWindow : Window
    {
        private readonly ProductService _productService;
        private List<CategoryDTO> _categories;
        private int? _selectedCategoryId = null;

        public ProductManagementWindow()
        {
            InitializeComponent();
            _productService = new ProductService();
            LoadCategories();
            LoadProducts();
        }

        private void LoadCategories()
        {
            _categories = _productService.GetAllCategories();
            CategoryComboBox.Items.Clear();

            // Thêm lựa chọn "All"
            CategoryComboBox.Items.Add("All");

            // Thêm các CategoryDTO vào ComboBox
            foreach (var cat in _categories)
                CategoryComboBox.Items.Add(cat);

            // Hiển thị tên danh mục
            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedIndex = 0;
        }

        private void LoadProducts()
        {
            string keyword = SearchBox.Text.Trim();
            List<ProductDTO> products;

            if (_selectedCategoryId.HasValue)
                products = _productService.SearchProducts(keyword, _selectedCategoryId);
            else
                products = _productService.SearchProducts(keyword);

            ProductList.ItemsSource = products;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadProducts();
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedIndex == 0)
            {
                _selectedCategoryId = null;
            }
            else
            {
                var selectedCategory = CategoryComboBox.SelectedItem as CategoryDTO;
                _selectedCategoryId = selectedCategory?.CategoryId;
            }

            LoadProducts();
        }
    }
}
