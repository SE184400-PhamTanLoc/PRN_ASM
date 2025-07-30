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
            return _context.Products.Where(p => p.CategoryId == categoryId).ToList();
        }

        public List<Product> SearchProducts(string keyword, int? categoryId = null)
        {
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword));
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
    }
}
