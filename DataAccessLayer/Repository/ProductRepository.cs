using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class ProductRepository
    {
        private readonly FuminiTikiSystemContext _context;
        public ProductRepository()
        {
            _context = new FuminiTikiSystemContext();
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.Include(p => p.Category).ToList();
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToList();
        }

        public List<Product> SearchProducts(string keyword, int? categoryId = null)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(p => p.Name.Contains(keyword) || (p.Description != null && p.Description.Contains(keyword)));
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);
            return query.ToList();
        }

        public Product GetProductById(int productId)
        {
            return _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == productId);
        }

        public bool AddProduct(Product product)
        {
            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateProduct(Product product)
        {
            try
            {
                var existingProduct = _context.Products.Find(product.ProductId);
                if (existingProduct == null) return false;

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteProduct(int productId)
        {
            try
            {
                var product = _context.Products.Find(productId);
                if (product == null) return false;

                // Check if product is associated with any order
                var hasOrders = _context.Products.Any(p => p.ProductId == productId && p.OrderId != null);
                if (hasOrders) return false;

                _context.Products.Remove(product);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
