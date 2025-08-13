using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViecLamIT.Domain.Entities;

public class JobPosting
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } // Mô tả công việc
    public string? Requirements { get; set; } // Yêu cầu

    [Display(Name = "Nhóm ngành")]
    public string? Category { get; set; }

    [Display(Name = "Chuyên ngành")]
    public string? Specialization { get; set; }
    public string? Benefits { get; set; } // Phúc lợi
    [Column(TypeName = "decimal(18, 2)")] // Thêm dòng này
    public decimal? SalaryFrom { get; set; }

    [Column(TypeName = "decimal(18, 2)")] // Thêm dòng này
    public decimal? SalaryTo { get; set; }
    public string JobLevel { get; set; } // Cấp bậc
    public string JobType { get; set; } // Loại hình công việc
    public int? ExperienceYears { get; set; } // Số năm kinh nghiệm
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ExpiredDate { get; set; } // Hạn nộp hồ sơ

    public int CompanyId { get; set; }
    [ForeignKey("CompanyId")]

    public string Status { get; set; } = "Pending";

    public string? AdminComment { get; set; }
    public virtual Company Company { get; set; }
    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}