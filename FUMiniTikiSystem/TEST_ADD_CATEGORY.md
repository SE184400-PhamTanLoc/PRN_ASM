# HƯỚNG DẪN TEST CHỨC NĂNG TẠO DANH MỤC MỚI

## Bước 1: Khởi động ứng dụng
1. Build và chạy ứng dụng:
```bash
cd FUMiniTikiSystem
dotnet build
dotnet run
```

2. Đăng nhập admin:
   - Email: `admin@FUMiniTikiSystem.com`
   - Password: `@@abc123@@`

3. Mở CategoryManagementWindow

## Bước 2: Test chức năng Add New

### Test 2.1: Tạo category mới cơ bản
1. Click nút "Add New"
2. Điền thông tin:
   - **Name:** "Phụ kiện điện tử"
   - **Description:** "Các loại phụ kiện cho điện thoại và máy tính"
   - **Picture:** (để trống)
3. Click "Save"
4. **Kết quả mong đợi:**
   - Thông báo "Category added successfully"
   - Category mới xuất hiện trong danh sách
   - Form được clear

### Test 2.2: Tạo category với hình ảnh từ URL
1. Click "Add New"
2. Điền thông tin:
   - **Name:** "Máy tính bảng"
   - **Description:** "Các loại tablet và iPad"
   - **Picture:** Click nút "Suggest" để lấy URL ngẫu nhiên
3. Click "Save"
4. **Kết quả mong đợi:**
   - Category được thêm thành công
   - Hình ảnh hiển thị trong danh sách
   - Preview image hoạt động

### Test 2.3: Tạo category với local image path
1. Click "Add New"
2. Điền thông tin:
   - **Name:** "Đồng hồ thông minh"
   - **Description:** "Smartwatch và wearable devices"
   - **Picture:** "/images/smartwatch.jpg"
3. Click "Save"
4. **Kết quả mong đợi:**
   - Category được thêm thành công
   - Hình ảnh hiển thị (nếu file tồn tại)

## Bước 3: Test Validation

### Test 3.1: Validation tên trống
1. Click "Add New"
2. Để trống Name
3. Click "Save"
4. **Kết quả mong đợi:**
   - Thông báo "Category name is required"
   - Focus vào NameTextBox

### Test 3.2: Validation tên quá ngắn
1. Click "Add New"
2. Nhập Name: "A"
3. Click "Save"
4. **Kết quả mong đợi:**
   - Thông báo "Category name must be at least 2 characters long"

### Test 3.3: Validation URL không hợp lệ
1. Click "Add New"
2. Điền Name: "Test Category"
3. Điền Picture: "invalid-url"
4. Click "Save"
5. **Kết quả mong đợi:**
   - Dialog hỏi "Do you want to continue?"
   - Có thể chọn Yes để tiếp tục hoặc No để sửa

### Test 3.4: Validation tên trùng
1. Click "Add New"
2. Nhập Name: "Điện thoại" (đã tồn tại)
3. Click "Save"
4. **Kết quả mong đợi:**
   - Thông báo "Failed to add category. Category name might already exist"

## Bước 4: Test chức năng Suggest Image

### Test 4.1: Suggest URL ngẫu nhiên
1. Click "Add New"
2. Click nút "Suggest"
3. **Kết quả mong đợi:**
   - URL ngẫu nhiên được thêm vào PictureTextBox
   - Preview image hiển thị
   - Status: "Suggested image URL added"

### Test 4.2: Suggest nhiều lần
1. Click "Suggest" nhiều lần
2. **Kết quả mong đợi:**
   - URL thay đổi mỗi lần click
   - Preview image cập nhật real-time

## Bước 5: Test UI/UX

### Test 5.1: Button states
1. Khi mới mở window:
   - Add New: Enabled
   - Save: Disabled
2. Khi click "Add New":
   - Add New: Enabled
   - Save: Enabled
3. Khi click "Edit":
   - Add New: Disabled
   - Save: Enabled

### Test 5.2: Form states
1. Khi click "Add New":
   - Form được clear
   - Title: "Add New Category"
   - Save button: "Save"
2. Khi click "Edit":
   - Form được populate với dữ liệu
   - Title: "Edit Category"
   - Save button: "Update"

## Bước 6: Test Integration

### Test 6.1: Refresh ProductManagementWindow
1. Thêm category mới trong CategoryManagementWindow
2. Đóng CategoryManagementWindow
3. Kiểm tra ProductManagementWindow
4. **Kết quả mong đợi:**
   - Category mới xuất hiện trong ComboBox
   - Không cần restart ứng dụng

### Test 6.2: Database persistence
1. Thêm category mới
2. Đóng và mở lại ứng dụng
3. Kiểm tra CategoryManagementWindow
4. **Kết quả mong đợi:**
   - Category mới vẫn tồn tại trong danh sách

## Các URL hình ảnh mẫu để test:
- https://picsum.photos/200/200?random=1
- https://picsum.photos/200/200?random=2
- https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=200&h=200&fit=crop
- /images/Phone.jpg
- /images/Laptop.jpg

## Troubleshooting:
1. **Nếu không thể thêm category:**
   - Kiểm tra kết nối database
   - Kiểm tra quyền truy cập
   - Kiểm tra validation rules

2. **Nếu hình ảnh không hiển thị:**
   - Kiểm tra URL có hợp lệ không
   - Kiểm tra kết nối internet (cho URL)
   - Kiểm tra file có tồn tại không (cho local path)

3. **Nếu có lỗi UI:**
   - Clean và rebuild project
   - Kiểm tra XAML syntax
   - Kiểm tra event handlers 