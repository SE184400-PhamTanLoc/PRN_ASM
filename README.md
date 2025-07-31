# 🛒 FU Mini Tiki System

Hệ thống quản lý cửa hàng mini Tiki được phát triển bằng WPF (Windows Presentation Foundation) và C#.

## 🚀 **Tính năng chính**

### 👤 **Phần Customer (Khách hàng)**
- **Đăng nhập/Đăng ký**: Hệ thống xác thực người dùng
- **Xem sản phẩm**: Hiển thị sản phẩm theo danh mục với hình ảnh
- **Giỏ hàng**: Thêm/xóa sản phẩm, tính tổng tiền
- **Thanh toán**: Tạo đơn hàng và xử lý thanh toán

### 👨‍💼 **Phần Admin (Quản trị viên)**
- **Dashboard**: Trang tổng quan với thống kê và điều hướng
- **Quản lý danh mục**: CRUD danh mục sản phẩm với hình ảnh
- **Quản lý sản phẩm**: CRUD sản phẩm theo danh mục
- **Quản lý đơn hàng**: Xem, cập nhật trạng thái đơn hàng

## 🛠️ **Công nghệ sử dụng**

- **Frontend**: WPF (Windows Presentation Foundation)
- **Backend**: C# (.NET 8.0)
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Architecture**: 3-Layer Architecture (DataAccess, Business, Presentation)

## 📋 **Yêu cầu hệ thống**

- Windows 10/11
- .NET 8.0 Runtime
- SQL Server (LocalDB hoặc SQL Server Express)
- Visual Studio 2022 (để phát triển)

## 🔧 **Cài đặt và chạy**

### **1. Clone repository**
```bash
git clone <repository-url>
cd Group_ASM
```

### **2. Cài đặt database**
- Mở SQL Server Management Studio hoặc Azure Data Studio
- Chạy script tạo database:

```sql
CREATE DATABASE FUMiniTikiSystem;
GO
USE FUMiniTikiSystem;
GO

-- Bảng Customers
CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL
);

-- Bảng Orders
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderAmount DECIMAL(18,2) NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    CustomerID INT NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending',
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
);

-- Bảng Categories
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Picture NVARCHAR(255),
    Description NVARCHAR(255)
);

-- Bảng Products
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(MAX),
    CategoryID INT NOT NULL,
    OrderID INT NULL,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);
```

### **3. Cấu hình connection string**
- Mở file `DataAccessLayer/Models/FuminiTikiSystemContext.cs`
- Cập nhật connection string phù hợp với SQL Server của bạn

### **4. Build và chạy**
```bash
dotnet restore
dotnet build
dotnet run --project FUMiniTikiSystem
```

## 👥 **Tài khoản mặc định**

### **Admin**
- Email: `admin@fpt.edu.vn`
- Password: `admin123`

### **Customer (tạo mới)**
- Đăng ký tài khoản mới qua giao diện đăng nhập

## 📁 **Cấu trúc project**

```
Group_ASM/
├── DataAccessLayer/           # Layer truy cập dữ liệu
│   ├── Models/               # Entity models
│   └── Repository/           # Repository pattern
├── BusinessLayer/            # Layer business logic
│   ├── DTO/                  # Data Transfer Objects
│   └── Service/              # Business services
├── FUMiniTikiSystem/         # WPF Application
│   ├── AdminWindow/          # Admin interfaces
│   ├── CustomerWindow/       # Customer interfaces
│   ├── Converters/           # WPF converters
│   └── LoginWindow.xaml      # Login interface
└── README.md
```

## 🎯 **Hướng dẫn sử dụng**

### **Đăng nhập Admin**
1. Chạy ứng dụng
2. Nhập email: `admin@fpt.edu.vn`
3. Nhập password: `admin123`
4. Chọn "Admin Login"

### **Quản lý danh mục**
1. Từ Dashboard, click "Category Management"
2. Thêm danh mục mới với hình ảnh
3. Sau khi thêm, có thể chọn "Add Product" để thêm sản phẩm

### **Quản lý sản phẩm**
1. Từ Category Management, click "Add Product"
2. Nhập thông tin sản phẩm
3. Lưu sản phẩm vào danh mục

### **Quản lý đơn hàng**
1. Từ Dashboard, click "Order Management"
2. Xem danh sách đơn hàng
3. Chọn đơn hàng để xem chi tiết
4. Cập nhật trạng thái đơn hàng

### **Đăng ký Customer**
1. Từ màn hình đăng nhập, click "Register"
2. Nhập thông tin cá nhân
3. Tạo tài khoản mới

### **Mua sản phẩm**
1. Đăng nhập với tài khoản customer
2. Xem sản phẩm theo danh mục
3. Thêm vào giỏ hàng
4. Thanh toán và tạo đơn hàng

## 🔧 **Tính năng kỹ thuật**

### **Image Handling**
- Hỗ trợ upload hình ảnh cho danh mục
- Hiển thị hình ảnh sản phẩm theo hình ảnh danh mục
- Converter để chuyển đổi string path thành ImageSource

### **Status Management**
- Quản lý trạng thái đơn hàng: Pending, Processing, Shipped, Delivered, Cancelled
- Color converter để hiển thị màu sắc theo trạng thái

### **Navigation**
- Hệ thống điều hướng giữa các window
- Back button để quay lại trang trước
- Dashboard làm trung tâm điều hướng

## 🐛 **Xử lý lỗi thường gặp**

### **Lỗi kết nối database**
- Kiểm tra SQL Server đã chạy chưa
- Kiểm tra connection string
- Đảm bảo database đã được tạo

### **Lỗi build**
- Đảm bảo .NET 8.0 đã được cài đặt
- Chạy `dotnet restore` trước khi build
- Kiểm tra các package reference

### **Lỗi hiển thị hình ảnh**
- Kiểm tra đường dẫn hình ảnh
- Đảm bảo file hình ảnh tồn tại
- Kiểm tra quyền truy cập file

## 📝 **Ghi chú phát triển**

- Sử dụng MVVM pattern cho UI
- Repository pattern cho data access
- DTO pattern cho data transfer
- Exception handling cho tất cả operations
- Null safety với nullable reference types

## 🤝 **Đóng góp**

1. Fork project
2. Tạo feature branch
3. Commit changes
4. Push to branch
5. Tạo Pull Request

## 📄 **License**

Project này được phát triển cho mục đích học tập tại FPT University.

---

**Developed by Group ASM** 🚀 