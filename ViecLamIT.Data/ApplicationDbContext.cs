using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ViecLamIT.Domain.Entities;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<JobApplication> JobApplications { get; set; }

    // Khai báo các bảng sẽ được tạo trong DB
    public DbSet<Company> Companies { get; set; }
    public DbSet<JobPosting> JobPostings { get; set; }
    // Thêm các DbSet cho CV, JobApplication... sau này
    public DbSet<Profile> Profiles { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Dùng để cấu hình các mối quan hệ phức tạp, khóa chính/phụ... nếu cần
        // --- THÊM ĐOẠN CẤU HÌNH NÀY ---
        // Đoạn code này chỉ định rõ "luật chơi" cho các mối quan hệ
        // để tránh lỗi "multiple cascade paths".

        // Cấu hình mối quan hệ giữa JobApplication và JobPosting
        builder.Entity<JobApplication>()
            .HasOne(ja => ja.JobPosting)
            .WithMany(jp => jp.JobApplications)
            .HasForeignKey(ja => ja.JobPostingId)
            .OnDelete(DeleteBehavior.Cascade); // Khi xóa JobPosting, xóa luôn JobApplication (Hợp lý)

        // Cấu hình mối quan hệ giữa JobApplication và ApplicationUser (Applicant)
        builder.Entity<JobApplication>()
            .HasOne(ja => ja.Applicant)
            .WithMany() // ApplicationUser không cần danh sách các đơn đã ứng tuyển
            .HasForeignKey(ja => ja.ApplicantId)
            .OnDelete(DeleteBehavior.NoAction); // QUAN TRỌNG: Khi xóa User, không tự động xóa đơn ứng tuyển
    }
}