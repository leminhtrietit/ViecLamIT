using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViecLamIT.Domain.Entities;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Province { get; set; } // Tỉnh/Thành phố
    public string? Ward { get; set; }
    public string? StreetAddress { get; set; } // Số nhà, tên đường
    public string? LogoUrl { get; set; }

    public string? Website { get; set; }
    public string? Description { get; set; }
    public string? CompanySize { get; set; } // Quy mô công ty

  
    [Display(Name = "Mã số thuế")]
    public string? TaxCode { get; set; }

    [Display(Name = "Loại hình kinh doanh")]
    public string? BusinessType { get; set; } // Ví dụ: "Doanh nghiệp", "Hộ kinh doanh"

    public bool IsVerified { get; set; } = false; // Mặc định là false (chưa xác thực)

    // Thêm trường để theo dõi trạng thái yêu cầu xác thực
    // "NotRequested", "Pending", "Verified"
    public string VerificationStatus { get; set; } = "NotRequested";

    // Mối quan hệ: Một công ty thuộc về một Nhà tuyển dụng
    public string EmployerId { get; set; } = string.Empty;
    [ForeignKey("EmployerId")]
    public virtual ApplicationUser Employer { get; set; }

    // Mối quan hệ: Một công ty có nhiều tin tuyển dụng
    public virtual ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
}