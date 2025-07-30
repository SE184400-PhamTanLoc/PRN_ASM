using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repository
{
    public class OrderRepository
    {
        private readonly FuminiTikiSystemContext _context;

        public OrderRepository()
        {
            _context = new FuminiTikiSystemContext();
        }

        public void CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public List<Order> GetOrdersByCustomer(int customerId)
        {
            return _context.Orders
                .Include(o => o.Products)
                .Where(o => o.CustomerId == customerId)
                .ToList();
        }
    }
}
