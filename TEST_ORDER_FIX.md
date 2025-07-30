# Test Order Fix - Đã Sửa Database Relationship

## ✅ **Đã sửa:**
- ✅ **OrderService.CreateOrder()** - Không tạo Products collection nữa
- ✅ **OrderRepository.CreateOrder()** - Xử lý relationship đúng cách
- ✅ **Database relationship** - Order và Product được liên kết qua OrderId

## 🔧 **Vấn đề đã sửa:**
**Lỗi cũ:** "An error occurred while saving the entity changes"
**Nguyên nhân:** Conflict giữa Order.Products collection và Product.OrderId foreign key
**Giải pháp:** Tạo Order trước, sau đó cập nhật OrderId cho Products

## 🚀 **Cách test:**

### **Bước 1: Build và chạy**
```bash
dotnet build
dotnet run
```

### **Bước 2: Test đăng ký/đăng nhập**
```
Full Name: Test User
Email: testuser@example.com
Password: Password123!
```

### **Bước 3: Test mua hàng**
1. **Thêm sản phẩm vào giỏ hàng**
2. **Click "🛒 Cart"**
3. **Click "💳 Checkout"**
4. **Chọn phương thức thanh toán**
5. **Click "Pay Now"**

## 🎯 **Kết quả mong đợi:**

### **Khi thanh toán thành công:**
- ✅ **Message:** "Payment successful! Your order has been placed."
- ✅ **CartWindow đóng**
- ✅ **Order được tạo trong database**
- ✅ **Products được liên kết với Order**

### **Khi xem Orders:**
- ✅ **Click "📋 My Orders"**
- ✅ **Hiển thị order vừa tạo**
- ✅ **Hiển thị đúng sản phẩm và tổng tiền**

## 📋 **Test cases:**

### **Test Case 1: Đơn hàng đơn giản**
```
1. Thêm "The Great Gatsby" ($12.99) vào giỏ hàng
2. Checkout với Credit Card
3. Kết quả: Order được tạo với total $17.99
```

### **Test Case 2: Đơn hàng nhiều sản phẩm**
```
1. Thêm "iPhone 15 Pro" ($999.99) vào giỏ hàng
2. Thêm "Nike Air Max 270" ($129.99) vào giỏ hàng
3. Checkout với Bank Transfer
4. Kết quả: Order được tạo với total $1134.98
```

## 🔍 **Kiểm tra database:**

### **Kiểm tra Order:**
```sql
SELECT * FROM Orders WHERE CustomerID = [your_customer_id] ORDER BY OrderDate DESC
```

### **Kiểm tra Products:**
```sql
SELECT p.*, o.OrderID 
FROM Products p 
LEFT JOIN Orders o ON p.OrderID = o.OrderID 
WHERE p.OrderID IS NOT NULL
```

## 🚨 **Nếu vẫn có lỗi:**

### **Lỗi 1: Database connection**
```
Error: Cannot connect to database
```
**Giải pháp:**
1. Kiểm tra SQL Server đang chạy
2. Kiểm tra connection string trong `appsettings.json`

### **Lỗi 2: Foreign key constraint**
```
Error: The INSERT statement conflicted with the FOREIGN KEY constraint
```
**Giải pháp:**
1. Kiểm tra CustomerId có tồn tại trong database
2. Kiểm tra ProductIds có tồn tại trong database

### **Lỗi 3: Product not found**
```
Error: Product with ID X not found
```
**Giải pháp:**
1. Sử dụng data test từ `DU_LIEU_TEST_CATEGORY_PRODUCT.md`
2. Đảm bảo products đã được thêm vào database

## 📞 **Báo cáo kết quả:**

Nếu vẫn gặp vấn đề, hãy cung cấp:
- Thông báo lỗi cụ thể
- Screenshot lỗi
- Kết quả của lệnh `dotnet build`
- Nội dung database (nếu có thể) 