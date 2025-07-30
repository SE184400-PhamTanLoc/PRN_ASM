using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repository
{
    public class CategoryRepository
    {
        private readonly FuminiTikiSystemContext _context;
        public CategoryRepository()
        {
            _context = new FuminiTikiSystemContext();
        }
        
        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }
        
        public Category GetCategoryById(int categoryId)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
        }
        
        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
        
        public void UpdateCategory(Category category)
        {
            var existingCategory = _context.Categories.Find(category.CategoryId);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                existingCategory.Picture = category.Picture;
                _context.SaveChanges();
            }
        }
        
        public bool DeleteCategory(int categoryId)
        {
            var category = _context.Categories.Find(categoryId);
            if (category != null)
            {
                // Kiểm tra xem có sản phẩm nào thuộc category này không
                var hasProducts = _context.Products.Any(p => p.CategoryId == categoryId);
                if (hasProducts)
                {
                    return false; // Không thể xóa category có sản phẩm
                }
                
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        
        public bool CategoryExists(int categoryId)
        {
            return _context.Categories.Any(c => c.CategoryId == categoryId);
        }
        
        public bool CategoryNameExists(string name, int excludeId = 0)
        {
            return _context.Categories.Any(c => c.Name == name && c.CategoryId != excludeId);
        }
    }
} 