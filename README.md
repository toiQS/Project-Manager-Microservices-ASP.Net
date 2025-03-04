# 📌 PM - Project Management System

![GitHub Repo Stars](https://img.shields.io/github/stars/toiQS/PM?style=social)
![GitHub Forks](https://img.shields.io/github/forks/toiQS/PM?style=social)
![GitHub License](https://img.shields.io/github/license/toiQS/PM)

## 📝 Giới thiệu
**PM** (Project Management System) là một hệ thống quản lý dự án được xây dựng bằng **.NET**, giúp các nhóm phát triển phần mềm theo dõi công việc, quản lý tiến độ, phân công nhiệm vụ một cách hiệu quả.

🔹 **Mục tiêu dự án:**
- Cung cấp một nền tảng quản lý dự án đơn giản, dễ sử dụng.
- Áp dụng các mô hình kiến trúc hiện đại để đảm bảo khả năng mở rộng.
- Sử dụng các công nghệ mới nhất của hệ sinh thái **.NET**.

## 🚀 Công nghệ sử dụng
- **Ngôn ngữ lập trình:** C#
- **Frameworks:** ASP.NET Core, Entity Framework Core
- **Authentication:** JWT Bearer Authentication
- **API Gateway:** Ocelot
- **Database:** SQL Server
- **Containerization:** Docker
- **DevOps:** GitHub Actions

## 📥 Cài đặt & Chạy dự án
### 1️⃣ Yêu cầu hệ thống
- .NET SDK 7.0+
- SQL Server
- Docker (tùy chọn)

### 2️⃣ Clone dự án
```bash
git clone https://github.com/toiQS/PM.git
cd PM
```

### 3️⃣ Cấu hình database
- Cập nhật chuỗi kết nối trong `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=PM;Trusted_Connection=True;"
}
```
- Chạy migration
```bash
dotnet ef database update
```

### 4️⃣ Chạy ứng dụng
```bash
dotnet run
```
Ứng dụng sẽ chạy trên `http://localhost:5000`.

## 📌 Kiến trúc hệ thống
**PM** được thiết kế theo mô hình **Microservices**, áp dụng các pattern chuẩn trong phát triển phần mềm:
- **Repository & Unit of Work** – Quản lý database transaction.
- **CQRS (Command Query Responsibility Segregation)** – Tách biệt thao tác ghi và đọc dữ liệu.
- **API Gateway với Ocelot** – Điều phối request giữa các service.

### ⚙️ Mô hình tổng quan:
```
+-------------+      +----------------+      +----------------+
| Frontend UI | ---> | API Gateway    | ---> | Service X      |
+-------------+      +----------------+      +----------------+
                             |                     |
                             v                     v
                      +------------+        +------------+
                      | Database A |        | Database B |
                      +------------+        +------------+
```

## 🔥 Các tính năng chính
✅ Quản lý dự án, tasks, deadlines.
✅ Phân quyền user (Admin, Member, Viewer).
✅ Xác thực với JWT Authentication.
✅ API Gateway với Ocelot.
✅ Triển khai với Docker.

## 🤝 Đóng góp
Nếu bạn muốn đóng góp cho dự án, hãy:
1. Fork repo này.
2. Tạo một branch mới (`feature/your-feature`).
3. Gửi pull request.

## 📬 Liên hệ
📩 **Email:** nguyensieu12112002@gmail.com  
🐙 **GitHub:** [toiQS](https://github.com/toiQS)  
💼 **LinkedIn:** [NguyenQuocSieu](https://www.linkedin.com/in/nguyenquocsieu-akai)

---

