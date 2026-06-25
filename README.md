# TruyenCV API Project

Dự án Web API quản lý và đọc truyện trực tuyến được xây dựng trên nền tảng **.NET 10** và **Entity Framework Core**.

---

## 🛠️ Yêu cầu hệ thống
Trước khi khởi chạy dự án, hãy đảm bảo máy tính của bạn đã cài đặt các công cụ sau:
* **.NET 10 SDK**
* **MySQL Server** (hoặc sử dụng dịch vụ Cloud MySQL như Aiven)
* **dotnet-ef CLI Tool** (dành cho việc quản lý migrations cơ sở dữ liệu)

---

## ⚙️ Hướng dẫn cấu hình môi trường

Dự án này sử dụng cơ chế nạp cấu hình bảo mật từ file `.env` cục bộ. Mọi thông tin nhạy cảm đã được loại bỏ khỏi `appsettings.json` để tránh rò rỉ dữ liệu khi đẩy lên kho lưu trữ mã nguồn (Git).

### Các bước cấu hình:

1. **Tạo file `.env`:**
   Sao chép (copy) file `.env.example` ở thư mục gốc của dự án và đổi tên thành `.env`:
   ```bash
   cp .env.example .env
   ```

2. **Cấu hình các giá trị thực tế:**
   Mở file `.env` vừa tạo và cập nhật các thông số phù hợp:

   * **`ConnectionStrings__DefaultConnection`**: Chuỗi kết nối tới cơ sở dữ liệu MySQL của bạn.
     * *Ví dụ:* `Server=localhost;Port=3306;Database=truyencv_db;Uid=root;Pwd=YourPassword;SslMode=None;`
   * **`Jwt__Secret`**: Khóa bí mật dùng để mã hóa và xác thực JWT token (yêu cầu độ dài tối thiểu 32 ký tự / 256 bits).
   * **`Jwt__Issuer`**: Tên nhà phát hành token (ví dụ: `TruyenCV_API`).
   * **`Jwt__Audience`**: Đối tượng sử dụng token (ví dụ: `TruyenCV_Client`).
   * **`Jwt__ExpiryMinutes`**: Thời gian hết hạn của Access Token (phút, mặc định `15`).
   * **`Email__*`**: Thông tin máy chủ gửi mail SMTP (được sử dụng cho các tính năng xác thực, quên mật khẩu, v.v.).

*(Lưu ý: File `.env` và thư mục `Logs/` đã được thêm vào `.gitignore` nên sẽ không bị push lên Git).*

---

## 🚀 Hướng dẫn khởi chạy dự án

Thực hiện các lệnh sau tại thư mục gốc của dự án (nơi chứa file solution `.slnx`):

### Bước 1: Khôi phục các gói thư viện (Restore NuGet Packages)
```bash
dotnet restore
```

### Bước 2: Khởi tạo/Cập nhật Cơ sở dữ liệu (Database Migrations)
Nếu bạn chưa cài đặt công cụ EF Core CLI toàn cục, hãy chạy lệnh:
```bash
dotnet tool install --global dotnet-ef
```

Sau đó, áp dụng các bản cập nhật database migrations đã có sẵn:
```bash
dotnet ef database update --project src/TruyenCV.Infrastructure --startup-project src/TruyenCV.API
```

### Bước 3: Chạy ứng dụng API Server
```bash
dotnet run --project src/TruyenCV.API
```

---

## 🔍 Kiểm tra hoạt động
Sau khi chạy ứng dụng thành công, bạn có thể kiểm tra danh sách các API và thử nghiệm trực tiếp thông qua **Swagger UI**:

* **HTTP:** [http://localhost:5062/swagger](http://localhost:5062/swagger)
* **HTTPS:** [https://localhost:7252/swagger](https://localhost:7252/swagger)

---

## 📂 Cấu trúc dự án
Dự án được thiết kế theo mô hình **Clean Architecture**:
* **`TruyenCV.Domain`**: Chứa các Entity, Value Objects, Enum và các Repository Interfaces cốt lõi.
* **`TruyenCV.Shared`**: Chứa các lớp dùng chung như DTOs, Responses (`ApiResponse`, `ErrorResponse`), Constants, Exception tùy chỉnh.
* **`TruyenCV.Application`**: Chứa Logic nghiệp vụ (Services), Validators, Behaviors (như `LoggingProxy` pipeline).
* **`TruyenCV.Infrastructure`**: Hiện thực hóa Persistence (EF Core, DbContext, Repository implementations) và cấu hình DB.
* **`TruyenCV.API`**: Lớp Web API (Controllers, Middlewares, Configurations, Program.cs) tương tác trực tiếp với Client.
