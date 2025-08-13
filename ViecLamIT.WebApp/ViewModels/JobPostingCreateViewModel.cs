using System;
using System.ComponentModel.DataAnnotations;

namespace ViecLamIT.WebApp.ViewModels
{
    public class JobPostingCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Tiêu đề tin tuyển dụng.")]
        [Display(Name = "Tiêu đề tin tuyển dụng")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Nhóm ngành.")]
        [Display(Name = "Nhóm ngành")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Chuyên ngành.")]
        [Display(Name = "Chuyên ngành / Vị trí cụ thể")]
        public string Specialization { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả công việc.")]
        [Display(Name = "Mô tả công việc")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập yêu cầu.")]
        [Display(Name = "Yêu cầu ứng viên")]
        public string Requirements { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn cấp bậc.")]
        [Display(Name = "Cấp bậc")]
        public string JobLevel { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hạn nộp hồ sơ.")]
        [DataType(DataType.Date)]
        [Display(Name = "Hạn nộp hồ sơ")]
        public DateTime ExpiredDate { get; set; }

        // Các trường không bắt buộc khác
        public string? Benefits { get; set; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public string? JobType { get; set; }
        public int? ExperienceYears { get; set; }
    }
}
