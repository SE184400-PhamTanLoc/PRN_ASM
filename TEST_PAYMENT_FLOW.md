# Test Payment Flow - Đã Sửa Namespace

## ✅ **Đã sửa:**
- ✅ **PaymentWindow.xaml.cs** - Sửa namespace từ `BusinessLayer.DTOs.OrderDTO` thành `BusinessLayer.DTO.OrderDTO`
- ✅ **Database schema** - Đã thêm cột Status vào bảng Orders

## 🔧 **Vấn đề đã sửa:**
**Lỗi cũ:** "Error placing order: An error occurred while saving the entity changes"
**Nguyên nhân:** 
1. Namespace sai trong PaymentWindow
2. Database thiếu cột Status

## 🚀 **Cách test:**

### **Bước 1: Cập nhật database**
```sql
-- Chạy trong SQL Server Management Studio
USE FUMiniTikiSystem;

-- Thêm cột Status nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'Status')
BEGIN
    ALTER TABLE Orders ADD Status NVARCHAR(50) DEFAULT 'Pending';
END
```

### **Bước 2: Build và chạy**
```bash
dotnet build
dotnet run
```

### **Bước 3: Test flow thanh toán**
1. **Đăng ký/đăng nhập**
2. **Thêm sản phẩm vào giỏ hàng**
3. **Click "🛒 Cart"**
4. **Click "💳 Checkout"**
5. **Chọn phương thức thanh toán**
6. **Click "Pay Now"**

## 🎯 **Kết quả mong đợi:**

### **Khi click Checkout trong Cart:**
- ✅ **Message:** "Opening payment window with X items..."
- ✅ **Mở PaymentWindow** với form thanh toán

### **Khi thanh toán thành công:**
- ✅ **Message:** "Payment successful! Your order has been placed."
- ✅ **CartWindow đóng**
- ✅ **Order được tạo trong database**

## 📋 **Test cases:**

### **Test Case 1: Credit Card Payment**
```
1. Thêm "iPhone 15 Pro" ($9,999.00) vào giỏ hàng
2. Click "🛒 Cart"
3. Click "💳 Checkout"
4. Chọn "Credit Card"
5. Nhập thông tin thẻ:
   - Card Number: 1234567890123456
   - Cardholder Name: Test User
   - Expiry Date: 12/25
   - CVV: 123
6. Click "Pay Now"
7. Kết quả: Order được tạo với Status = "Paid"
```

### **Test Case 2: Cash on Delivery**
```
1. Thêm "Samsung" ($80,000.00) vào giỏ hàng
2. Click "🛒 Cart"
3. Click "💳 Checkout"
4. Chọn "Cash on Delivery"
5. Click "Pay Now"
6. Kết quả: Order được tạo với Status = "Pending Payment"
```

## 🔍 **Kiểm tra database:**

### **Kiểm tra Order:**
```sql
SELECT * FROM Orders WHERE CustomerID = [your_customer_id] ORDER BY OrderDate DESC
```

### **Kết quả mong đợi:**
- OrderID: [số]
- CustomerID: [customer_id]
- OrderAmount: [tổng tiền]
- OrderDate: [ngày hiện tại]
- Status: "Paid" hoặc "Pending Payment"

## 🚨 **Nếu vẫn có lỗi:**

### **Lỗi 1: Namespace not found**
```
Error: The type or namespace name 'OrderDTO' could not be found
```
**Giải pháp:** Đã sửa namespace trong PaymentWindow.xaml.cs

### **Lỗi 2: Database column not found**
```
Error: Invalid column name 'Status'
```
**Giải pháp:** Chạy script SQL để thêm cột Status

### **Lỗi 3: PaymentWindow not found**
```
Error: Could not find type 'PaymentWindow'
```
**Giải pháp:** Kiểm tra file PaymentWindow.xaml và PaymentWindow.xaml.cs tồn tại

## 📞 **Báo cáo kết quả:**

Nếu vẫn gặp vấn đề, hãy cung cấp:
- Thông báo lỗi cụ thể
- Screenshot lỗi
- Kết quả của lệnh `dotnet build`
- Nội dung database (nếu có thể) 