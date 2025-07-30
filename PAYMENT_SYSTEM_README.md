# Hệ thống Thanh toán và Đặt hàng - FU Mini Tiki System

## Tổng quan
Hệ thống thanh toán đã được tích hợp vào ứng dụng FU Mini Tiki System, cho phép khách hàng:
- Thêm sản phẩm vào giỏ hàng
- Thanh toán an toàn với nhiều phương thức
- Theo dõi trạng thái đơn hàng
- Xem lịch sử đơn hàng

## Các tính năng chính

### 1. Giỏ hàng (Cart)
- **Thêm sản phẩm**: Click "Add to Cart" trên sản phẩm
- **Quản lý số lượng**: Tăng/giảm số lượng trong giỏ hàng
- **Xóa sản phẩm**: Click "Remove" để xóa sản phẩm
- **Xóa toàn bộ**: Click "Clear Cart" để xóa tất cả

### 2. Thanh toán (Payment)
Hệ thống hỗ trợ 3 phương thức thanh toán:

#### 💳 Thẻ tín dụng/Ghi nợ
- Nhập số thẻ 16 chữ số
- Tên chủ thẻ
- Ngày hết hạn (MM/YY)
- Mã CVV (3-4 chữ số)
- **Validation**: Kiểm tra định dạng và độ dài

#### 🏦 Chuyển khoản ngân hàng
- Thông tin tài khoản ngân hàng
- Sử dụng Order ID làm mã tham chiếu
- Thanh toán thủ công

#### 💰 Thanh toán khi nhận hàng (COD)
- Không cần thông tin thanh toán
- Thu tiền khi giao hàng

### 3. Theo dõi đơn hàng (Order Tracking)
- **Trạng thái đơn hàng**:
  - Pending Payment: Chờ thanh toán
  - Paid: Đã thanh toán
  - Shipped: Đã gửi hàng
  - Delivered: Đã giao hàng
  - Cancelled: Đã hủy

- **Các bước theo dõi**:
  1. ✅ Đặt hàng
  2. ✅ Thanh toán
  3. ⏳ Xử lý đơn hàng
  4. ⏳ Chuẩn bị gửi hàng
  5. ⏳ Đang vận chuyển
  6. ⏳ Giao hàng
  7. ⏳ Hoàn thành

### 4. Lịch sử đơn hàng
- Xem tất cả đơn hàng đã đặt
- Sắp xếp theo ngày (mới nhất trước)
- Chi tiết đơn hàng và trạng thái
- Chức năng refresh để cập nhật

## Luồng hoạt động

### 1. Mua hàng
```
Chọn sản phẩm → Thêm vào giỏ → Xem giỏ hàng → Thanh toán → Xác nhận đơn hàng
```

### 2. Thanh toán
```
Chọn phương thức → Nhập thông tin → Xác thực → Xử lý thanh toán → Hoàn thành
```

### 3. Theo dõi
```
Xem đơn hàng → Kiểm tra trạng thái → Theo dõi vận chuyển → Nhận hàng
```

## Bảo mật

### Validation dữ liệu
- **Số thẻ**: Chỉ cho phép 16 chữ số
- **Ngày hết hạn**: Định dạng MM/YY
- **CVV**: 3-4 chữ số
- **Tên chủ thẻ**: Không được để trống

### Xử lý thanh toán
- Mô phỏng xử lý thanh toán (2 giây)
- Hiển thị trạng thái xử lý
- Thông báo kết quả thanh toán

## Giao diện người dùng

### PaymentWindow
- **Header**: Tiêu đề và tóm tắt đơn hàng
- **Payment Methods**: Chọn phương thức thanh toán
- **Payment Details**: Form nhập thông tin thanh toán
- **Order Summary**: Tóm tắt giá trị đơn hàng
- **Action Buttons**: Thanh toán hoặc Hủy

### OrdersWindow
- **Header**: Tiêu đề và số lượng đơn hàng
- **Order Cards**: Hiển thị từng đơn hàng
- **Status Badges**: Màu sắc theo trạng thái
- **Action Buttons**: Theo dõi và Xem chi tiết

## Cấu trúc code

### Files chính
- `PaymentWindow.xaml/.cs`: Cửa sổ thanh toán
- `OrdersWindow.xaml/.cs`: Cửa sổ lịch sử đơn hàng
- `CartWindow.xaml/.cs`: Cửa sổ giỏ hàng (đã cập nhật)
- `OrderService.cs`: Xử lý logic đơn hàng

### DTOs
- `OrderDTO`: Thông tin đơn hàng
- `CartItemDTO`: Thông tin sản phẩm trong giỏ

### Models
- `Order`: Model đơn hàng trong database
- `Product`: Model sản phẩm
- `Customer`: Model khách hàng

## Hướng dẫn sử dụng

### Cho khách hàng
1. **Đăng nhập** vào hệ thống
2. **Duyệt sản phẩm** và thêm vào giỏ hàng
3. **Xem giỏ hàng** và kiểm tra sản phẩm
4. **Thanh toán** với phương thức phù hợp
5. **Theo dõi đơn hàng** trong mục "My Orders"

### Cho developer
1. **PaymentWindow**: Xử lý thanh toán
2. **OrdersWindow**: Hiển thị lịch sử đơn hàng
3. **OrderService**: Logic xử lý đơn hàng
4. **Validation**: Kiểm tra dữ liệu đầu vào

## Mở rộng trong tương lai

### Tính năng có thể thêm
- Tích hợp cổng thanh toán thực tế (VNPay, Momo, etc.)
- Gửi email xác nhận đơn hàng
- Push notification cho trạng thái đơn hàng
- Đánh giá sản phẩm sau khi nhận hàng
- Hoàn tiền và đổi trả hàng

### Cải tiến kỹ thuật
- Sử dụng async/await cho xử lý thanh toán
- Thêm logging cho debug
- Unit tests cho các service
- Tối ưu performance cho database queries 