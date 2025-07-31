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

        public void CreateOrder(Order order, List<int> productIds)
        {
            // Tạo order trước
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Cập nhật OrderId cho các products
            foreach (var productId in productIds)
            {
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    product.OrderId = order.OrderId;
                    _context.Products.Update(product);
                }
            }
            _context.SaveChanges();
        }

        public List<Order> GetOrdersByCustomer(int customerId)
        {
            return _context.Orders
                .Include(o => o.Products)
                .Where(o => o.CustomerId == customerId)
                .ToList();
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Products)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return _context.Orders
                .Include(o => o.Products)
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.OrderId == orderId);
        }

        public bool UpdateOrderStatus(int orderId, string newStatus)
        {
            try
            {
                var order = _context.Orders.Find(orderId);
                if (order == null) return false;

                order.Status = newStatus;
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
