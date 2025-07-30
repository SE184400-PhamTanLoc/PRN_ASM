using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BusinessLayer.DTO;
using BusinessLayer.Service;

namespace FUMiniTikiSystem
{
    public partial class PaymentWindow : Window
    {
        private List<CartItemDTO> _cartItems;
        private int _customerId;
        private OrderService _orderService;
        private decimal _subtotal;
        private decimal _shipping = 5.00m;
        private decimal _total;

        public PaymentWindow(List<CartItemDTO> cartItems, int customerId)
        {
            InitializeComponent();
            _cartItems = cartItems;
            _customerId = customerId;
            _orderService = new OrderService();
            
            CalculateTotals();
            UpdateOrderSummary();
            
            // Set initial payment method visibility after window is loaded
            this.Loaded += (s, e) => ShowPaymentForm(null, null);
        }

        private void CalculateTotals()
        {
            _subtotal = _cartItems.Sum(item => item.Total);
            _total = _subtotal + _shipping;
            
            txtSubtotal.Text = $"${_subtotal:N2}";
            txtShipping.Text = $"${_shipping:N2}";
            txtTotal.Text = $"${_total:N2}";
        }



        private void UpdateOrderSummary()
        {
            var itemCount = _cartItems.Sum(item => item.Quantity);
            txtOrderSummary.Text = $"{itemCount} items • Total: ${_total:N2}";
        }



        private bool ValidateCreditCardPayment()
        {
            return !string.IsNullOrWhiteSpace(txtCardNumber.Text) && 
                   txtCardNumber.Text.Length == 16 &&
                   !string.IsNullOrWhiteSpace(txtCardHolderName.Text);
        }

        private string GetSelectedPaymentMethod()
        {
            if (rbCreditCard.IsChecked == true) return "Credit Card";
            if (rbBankTransfer.IsChecked == true) return "Bank Transfer";
            if (rbCashOnDelivery.IsChecked == true) return "Cash on Delivery";
            return "Unknown";
        }

        private void BtnPayNow_Click(object sender, RoutedEventArgs e)
        {
            var paymentMethod = GetSelectedPaymentMethod();
            
            if (paymentMethod == "Credit Card" && !ValidateCreditCardPayment())
                return;

            var orderDTO = new BusinessLayer.DTO.OrderDTO
            {
                CustomerId = _customerId,
                OrderAmount = _total,
                OrderDate = DateTime.Now,
                Status = paymentMethod == "Cash on Delivery" ? "Pending Payment" : "Paid",
                ProductIds = _cartItems.Select(item => item.ProductId).ToList()
            };

            _orderService.CreateOrder(orderDTO);

            MessageBox.Show("Payment successful! Your order has been placed.");
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to cancel the payment?", 
                "Cancel Payment", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void ShowPaymentForm(object sender, RoutedEventArgs e)
        {
            // Check if controls are initialized
            if (gbCreditCard == null || gbBankTransfer == null || gbCashOnDelivery == null ||
                rbCreditCard == null || rbBankTransfer == null || rbCashOnDelivery == null)
            {
                return; // Exit if controls are not yet initialized
            }

            // Hide all payment forms first
            gbCreditCard.Visibility = Visibility.Collapsed;
            gbBankTransfer.Visibility = Visibility.Collapsed;
            gbCashOnDelivery.Visibility = Visibility.Collapsed;

            // Show only the selected payment form
            if (rbCreditCard.IsChecked == true)
            {
                gbCreditCard.Visibility = Visibility.Visible;
            }
            else if (rbBankTransfer.IsChecked == true)
            {
                gbBankTransfer.Visibility = Visibility.Visible;
            }
            else if (rbCashOnDelivery.IsChecked == true)
            {
                gbCashOnDelivery.Visibility = Visibility.Visible;
            }
        }
    }
} 