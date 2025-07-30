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
    }
}
