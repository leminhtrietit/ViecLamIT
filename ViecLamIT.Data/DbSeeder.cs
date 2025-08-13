// File: ViecLamIT.Data/DbSeeder.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViecLamIT.Domain.Entities;

namespace ViecLamIT.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider service)
        {
            var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            var context = service.GetRequiredService<ApplicationDbContext>();

            await SeedRolesAndAdminAsync(userManager, roleManager);
            await SeedEmployersAndCompaniesAsync(userManager, context);
            await SeedJobSeekersAndProfilesAsync(userManager, context);
            await SeedJobPostingsAsync(context);
        }

        private static async Task SeedRolesAndAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // ... code của phương thức này giữ nguyên ...
            string[] roleNames = { "Admin", "Employer", "JobSeeker" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            if (await userManager.FindByEmailAsync("admin@gmail.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Admin",
                    LastName = "System",
                    UserType = "Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        private static async Task SeedEmployersAndCompaniesAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            // ... code của phương thức này giữ nguyên ...
            if (await context.Companies.AnyAsync()) return;

            var employers = new List<(ApplicationUser user, string password, Company company)>
            {
                (new ApplicationUser { UserName = "hr.fpt@gmail.com", Email = "hr.fpt@gmail.com", FirstName = "HR FPT", LastName = "Software", UserType = "Employer", EmailConfirmed = true },
                 "Employer@123",
                 new Company { Name = "FPT Software", Province = "Thành phố Hồ Chí Minh", Ward = "Phường Tân Phú", StreetAddress = "Lô T2, Đường D1, Khu Công nghệ cao", IsVerified = true, VerificationStatus = "Verified" }),
                (new ApplicationUser { UserName = "hr.vng@gmail.com", Email = "hr.vng@gmail.com", FirstName = "HR VNG", LastName = "Corporation", UserType = "Employer", EmailConfirmed = true },
                 "Employer@123",
                 new Company { Name = "VNG Corporation", Province = "Thành phố Hồ Chí Minh", Ward = "Phường 15", StreetAddress = "182 Lê Đại Hành", IsVerified = true, VerificationStatus = "Verified" })
            };

            foreach (var emp in employers)
            {
                if (await userManager.FindByEmailAsync(emp.user.Email) == null)
                {
                    var result = await userManager.CreateAsync(emp.user, emp.password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(emp.user, "Employer");
                        emp.company.EmployerId = emp.user.Id;
                        await context.Companies.AddAsync(emp.company);
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedJobSeekersAndProfilesAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            if (await context.Profiles.AnyAsync()) return;

            var seekers = new List<(ApplicationUser user, string password, Profile profile)>
             {
                 // --- BẮT ĐẦU SỬA ĐỔI ---
                 // Thay thế DesiredPosition bằng DesiredCategory và DesiredSpecialization
                 (new ApplicationUser { UserName = "van.a@gmail.com", Email = "van.a@gmail.com", FirstName = "Nguyễn Văn", LastName = "A", UserType = "JobSeeker", EmailConfirmed = true },
                  "Seeker@123",
                  new Profile {
                      DesiredCategory = "Software Engineering",
                      DesiredSpecialization = "Backend Developer",
                      Skills = "C#, ASP.NET Core, SQL Server, Azure",
                      Education = "Đại học Bách Khoa TP.HCM",
                      DesiredProvince = "Thành phố Hồ Chí Minh",
                      DesiredSalary = 25000000
                  }),
                 (new ApplicationUser { UserName = "thi.b@gmail.com", Email = "thi.b@gmail.com", FirstName = "Trần Thị", LastName = "B", UserType = "JobSeeker", EmailConfirmed = true },
                  "Seeker@123",
                  new Profile {
                      DesiredCategory = "Software Design",
                      DesiredSpecialization = "UI/UX Design",
                      Skills = "Figma, Sketch, Adobe XD",
                      Education = "Đại học Khoa học Tự nhiên TP.HCM",
                      DesiredProvince = "Thành phố Hà Nội",
                      DesiredSalary = 20000000
                  })
                 // --- KẾT THÚC SỬA ĐỔI ---
             };

            foreach (var seeker in seekers)
            {
                if (await userManager.FindByEmailAsync(seeker.user.Email) == null)
                {
                    var result = await userManager.CreateAsync(seeker.user, seeker.password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(seeker.user, "JobSeeker");
                        seeker.profile.UserId = seeker.user.Id;
                        await context.Profiles.AddAsync(seeker.profile);
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedJobPostingsAsync(ApplicationDbContext context)
        {
            // ... code của phương thức này giữ nguyên ...
            if (await context.JobPostings.AnyAsync()) return;
            var fptCompany = await context.Companies.FirstOrDefaultAsync(c => c.Name == "FPT Software");
            if (fptCompany != null)
            {
                await context.JobPostings.AddAsync(new JobPosting
                {
                    CompanyId = fptCompany.Id,
                    Title = "Lập trình viên .NET (Fresher)",
                    Category = "Software Engineering",
                    Specialization = "Backend Developer",
                    Description = "Tham gia các dự án lớn.",
                    Requirements = "Nắm vững C#.",
                    ExpiredDate = DateTime.Now.AddDays(30),
                    Status = "Active"
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
