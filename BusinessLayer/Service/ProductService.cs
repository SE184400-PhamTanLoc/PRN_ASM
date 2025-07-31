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
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "Unknown Category",
                CategoryImage = p.Category?.Picture
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
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "Unknown Category",
                CategoryImage = p.Category?.Picture
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
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "Unknown Category",
                CategoryImage = p.Category?.Picture
            }).ToList();
        }

        public ProductDTO GetProductById(int productId)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null) return null;

            return new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "Unknown Category",
                CategoryImage = product.Category?.Picture
            };
        }

        public bool AddProduct(ProductDTO productDTO)
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Price = productDTO.Price,
                Description = productDTO.Description,
                CategoryId = productDTO.CategoryId
            };

            return _productRepository.AddProduct(product);
        }

        public bool UpdateProduct(ProductDTO productDTO)
        {
            var product = new Product
            {
                ProductId = productDTO.ProductId,
                Name = productDTO.Name,
                Price = productDTO.Price,
                Description = productDTO.Description,
                CategoryId = productDTO.CategoryId
            };

            return _productRepository.UpdateProduct(product);
        }

        public bool DeleteProduct(int productId)
        {
            return _productRepository.DeleteProduct(productId);
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