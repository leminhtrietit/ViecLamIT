using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViecLamIT.Domain.Entities
{
    public class Profile
    {
        [Key] // Đặt khóa chính là UserId để đảm bảo mỗi người chỉ có 1 hồ sơ
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [Display(Name = "Nhóm ngành mong muốn")]
        public string? DesiredCategory { get; set; }

        [Display(Name = "Chuyên ngành mong muốn")]
        public string? DesiredSpecialization { get; set; }

        [Display(Name = "Kinh nghiệm làm việc")]
        public string? WorkExperience { get; set; }

        [Display(Name = "Học vấn")]
        public string? Education { get; set; }

        [Display(Name = "Kỹ năng")]
        public string? Skills { get; set; }

        [Display(Name = "Chứng chỉ")]
        public string? Certifications { get; set; }

        [Display(Name = "Mức lương mong muốn (VND)")]
        public decimal? DesiredSalary { get; set; }

        [Display(Name = "Tỉnh/Thành phố mong muốn")]
        public string? DesiredProvince { get; set; }
    }
}