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
    }
} 