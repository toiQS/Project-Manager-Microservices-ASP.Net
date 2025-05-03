using Microsoft.Extensions.DependencyInjection;
using PM.Core.Entities;

namespace PM.Core.Infrastructure.Data
{
    public static class SeedingPosition
    {
        public static async Task Initialize(this ServiceProvider services)
        {
            var context = services.GetRequiredService<CoreDbContext>();
            if (context == null)
            {
                return;
            }

            var arrData = new List<Position>()
            {
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Product Owner",
                    Description = "Chịu trách nhiệm xác định yêu cầu và ưu tiên sản phẩm."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Project Manager",
                    Description = "Quản lý dự án tổng thể: tiến độ, ngân sách, phạm vi công việc."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Business Analyst",
                    Description = "Phân tích và làm rõ yêu cầu nghiệp vụ cho nhóm kỹ thuật."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Technical Architect",
                    Description = "Thiết kế kiến trúc hệ thống, định hướng công nghệ."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Software Developer",
                    Description = "Phát triển tính năng sản phẩm theo yêu cầu kỹ thuật."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Quality Assurance Engineer",
                    Description = "Kiểm thử phần mềm để đảm bảo chất lượng sản phẩm."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "UI/UX Designer",
                    Description = "Thiết kế giao diện và trải nghiệm người dùng."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "DevOps Engineer",
                    Description = "Quản lý hệ thống CI/CD, môi trường deploy và vận hành hệ thống."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Security Engineer",
                    Description = "Thiết kế và kiểm soát các biện pháp bảo mật hệ thống."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Scrum Master",
                    Description = "Hỗ trợ nhóm phát triển thực hiện đúng quy trình Agile/Scrum."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Data Engineer",
                    Description = "Xây dựng pipelines xử lý dữ liệu, hỗ trợ hệ thống phân tích và AI."
                },
                new Position()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Support Engineer",
                    Description = "Hỗ trợ kỹ thuật cho khách hàng và xử lý sự cố sản phẩm."
                }
            };

            if (!context.Positions.Any())
            {
                context.Positions.AddRange(arrData);
                await context.SaveChangesAsync();
            }
        }
    }
}
