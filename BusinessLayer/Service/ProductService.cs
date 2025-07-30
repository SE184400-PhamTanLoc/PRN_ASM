using BusinessLayer.DTO;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Service
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        public ProductService()
        {
            _productRepository = new ProductRepository();
        }

        public List<ProductDTO> GetAllProducts()
        {
            var products = _productRepository.GetAllProducts();
            return products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                CategoryName = p.Category.Name
            }).ToList();
        }

        public List<ProductDTO> GetProductsByCategory(int categoryId)
        {
            var products = _productRepository.GetProductsByCategory(categoryId);
            return products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                CategoryName = p.Category.Name
            }).ToList();
        }

        public List<ProductDTO> SearchProducts(string keyword, int? categoryId = null)
        {
            var products = _productRepository.SearchProducts(keyword, categoryId);
            return products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                CategoryName = p.Category.Name
            }).ToList();
        }

        public List<CategoryDTO> GetAllCategories()
        {
            var repo = new CategoryRepository();
            return repo.GetAllCategories().Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                Picture = c.Picture
            }).ToList();
        }
    }
} 