using BusinessLayer.DTO;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Service
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepo;

        public OrderService()
        {
            _orderRepo = new OrderRepository(); // Gọi từ Repository
        }

        public List<OrderDTO> GetOrdersByCustomer(int customerId)
        {
            var orders = _orderRepo.GetOrdersByCustomer(customerId);
            return orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                OrderAmount = o.OrderAmount,
                OrderDate = o.OrderDate,
                CustomerId = o.CustomerId,
                Status = o.Status,
                ProductIds = o.Products.Select(p => p.ProductId).ToList()
            }).ToList();
        }

        public void CreateOrder(OrderDTO dto)
        {
            var order = new Order
            {
                OrderAmount = dto.OrderAmount,
                OrderDate = dto.OrderDate ?? DateTime.Now,
                CustomerId = dto.CustomerId,
                Status = dto.Status ?? "Pending"
            };

            _orderRepo.CreateOrder(order, dto.ProductIds);
        }

        public List<OrderDTO> GetAllOrders()
        {
            var orders = _orderRepo.GetAllOrders();
            return orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                OrderAmount = o.OrderAmount,
                OrderDate = o.OrderDate,
                CustomerId = o.CustomerId,
                Status = o.Status,
                ProductIds = o.Products?.Select(p => p.ProductId).ToList() ?? new List<int>(),
                CustomerName = o.Customer?.Name ?? "Unknown",
                CustomerEmail = o.Customer?.Email ?? "Unknown",
                ProductCount = o.Products?.Count ?? 0
            }).ToList();
        }

        public bool UpdateOrderStatus(int orderId, string newStatus)
        {
            return _orderRepo.UpdateOrderStatus(orderId, newStatus);
        }

        public OrderDTO GetOrderById(int orderId)
        {
            var order = _orderRepo.GetOrderById(orderId);
            if (order == null) return null;

            return new OrderDTO
            {
                OrderId = order.OrderId,
                OrderAmount = order.OrderAmount,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                Status = order.Status,
                ProductIds = order.Products?.Select(p => p.ProductId).ToList() ?? new List<int>()
            };
        }
    }
}
