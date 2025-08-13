# 💼 ViecLamIT

**ViecLamIT** là nền tảng tuyển dụng trực tuyến chuyên biệt cho **ngành Công nghệ Thông tin**, giúp kết nối nhanh chóng giữa **ứng viên IT** và **nhà tuyển dụng**.  
Dự án được xây dựng nhằm tối ưu quy trình tìm kiếm việc làm và quản lý tin tuyển dụng, mang lại trải nghiệm thân thiện và hiệu quả.

---

## 📌 Mục Tiêu Dự Án
- Cung cấp một cổng thông tin việc làm chuyên sâu cho ngành IT.
- Hỗ trợ ứng viên tạo, quản lý hồ sơ và ứng tuyển nhanh chóng.
- Giúp nhà tuyển dụng đăng tin, quản lý ứng viên và quy trình tuyển dụng.
- Tối ưu tìm kiếm công việc theo kỹ năng, vị trí và mức lương.

---

## 🛠 Công Nghệ Sử Dụng
- **Ngôn ngữ lập trình:** C# (.NET Core / ASP.NET)
- **Cơ sở dữ liệu:** Microsoft SQL Server
- **Frontend:** Razor Pages / MVC + Bootstrap
- **Quản lý phiên & bảo mật:** ASP.NET Core Identity
- **Công cụ khác:** Entity Framework Core, LINQ, AutoMapper

---

## 🚀 Các Tính Năng Chính
### Dành cho Ứng Viên
- Đăng ký, đăng nhập, quản lý hồ sơ cá nhân.
- Tìm kiếm việc làm theo từ khóa, kỹ năng, địa điểm.
- Ứng tuyển trực tiếp và theo dõi trạng thái hồ sơ.

### Dành cho Nhà Tuyển Dụng
- Đăng tin tuyển dụng mới, chỉnh sửa và quản lý tin đã đăng.
- Xem danh sách ứng viên và hồ sơ chi tiết.
- Xác thực công ty và quản lý thông tin doanh nghiệp.

### Dành cho Quản Trị Viên
- Quản lý tài khoản người dùng và công ty.
- Kiểm duyệt tin tuyển dụng trước khi hiển thị.
- Báo cáo và thống kê hệ thống.

---

## 📂 Cấu Trúc Thư Mục
```
ViecLamIT/
 ├── Controllers/      # Xử lý logic nghiệp vụ
 ├── Models/           # Các lớp mô hình dữ liệu
 ├── Views/            # Giao diện Razor Pages/MVC
 ├── wwwroot/          # Tài nguyên tĩnh (CSS, JS, hình ảnh)
 ├── Data/             # Cấu hình kết nối DB, migrations
 └── ViecLamIT.csproj  # File cấu hình dự án
```

---

## ⚙️ Cài Đặt & Chạy Dự Án
```bash
# 1. Clone dự án
git clone https://github.com/leminhtrietit/ViecLamIT.git

# 2. Mở dự án bằng Visual Studio hoặc Rider

# 3. Cấu hình chuỗi kết nối DB trong appsettings.json

# 4. Chạy migrations để tạo cơ sở dữ liệu
dotnet ef database update

# 5. Chạy dự án
dotnet run
```

---

## 📜 License
Dự án được phân phối theo giấy phép **MIT**. Xem chi tiết tại [LICENSE](LICENSE).

---

## 👨‍💻 Tác Giả
- **Lê Minh Triết** - [GitHub](https://github.com/leminhtrietit)

---

## 🌟 Hỗ Trợ
Nếu bạn thấy dự án hữu ích, hãy ⭐ repository để ủng hộ tác giả.
