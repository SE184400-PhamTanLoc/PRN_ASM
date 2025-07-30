using BusinessLayer.DTO;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Service
{
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepository;
        
        public CategoryService()
        {
            _categoryRepository = new CategoryRepository();
        }
        
        public List<CategoryDTO> GetAllCategories()
        {
            var categories = _categoryRepository.GetAllCategories();
            return categories.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                Picture = c.Picture
            }).ToList();
        }
        
        public CategoryDTO GetCategoryById(int categoryId)
        {
            var category = _categoryRepository.GetCategoryById(categoryId);
            if (category == null) return null;
            
            return new CategoryDTO
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                Picture = category.Picture
            };
        }
        
        public bool AddCategory(CategoryDTO categoryDTO)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(categoryDTO.Name))
                return false;
                
            if (_categoryRepository.CategoryNameExists(categoryDTO.Name))
                return false;
            
            var category = new Category
            {
                Name = categoryDTO.Name.Trim(),
                Description = categoryDTO.Description?.Trim(),
                Picture = categoryDTO.Picture?.Trim()
            };
            
            try
            {
                _categoryRepository.AddCategory(category);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool UpdateCategory(CategoryDTO categoryDTO)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(categoryDTO.Name))
                return false;
                
            if (!_categoryRepository.CategoryExists(categoryDTO.CategoryId))
                return false;
                
            if (_categoryRepository.CategoryNameExists(categoryDTO.Name, categoryDTO.CategoryId))
                return false;
            
            var category = new Category
            {
                CategoryId = categoryDTO.CategoryId,
                Name = categoryDTO.Name.Trim(),
                Description = categoryDTO.Description?.Trim(),
                Picture = categoryDTO.Picture?.Trim()
            };
            
            try
            {
                _categoryRepository.UpdateCategory(category);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return false;
                
            return _categoryRepository.DeleteCategory(categoryId);
        }
        
        public bool CategoryExists(int categoryId)
        {
            return _categoryRepository.CategoryExists(categoryId);
        }
        
        public bool CategoryNameExists(string name, int excludeId = 0)
        {
            return _categoryRepository.CategoryNameExists(name, excludeId);
        }
    }
} 