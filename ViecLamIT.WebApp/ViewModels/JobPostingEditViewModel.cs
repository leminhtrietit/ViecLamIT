using System;
using System.ComponentModel.DataAnnotations;

namespace ViecLamIT.WebApp.ViewModels
{
    public class JobPostingEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chức danh.")]
        [Display(Name = "Chức danh")]
        public string Title { get; set; }

        [Display(Name = "Mô tả công việc")]
        public string? Description { get; set; }

        [Display(Name = "Yêu cầu ứng viên")]
        public string? Requirements { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Nhóm ngành.")]
        [Display(Name = "Nhóm ngành")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Chuyên ngành.")]
        [Display(Name = "Chuyên ngành / Vị trí cụ thể")]
        public string Specialization { get; set; }

        [Display(Name = "Phúc lợi")]
        public string? Benefits { get; set; }

        [Display(Name = "Mức lương từ")]
        public decimal? SalaryFrom { get; set; }

        [Display(Name = "Mức lương đến")]
        public decimal? SalaryTo { get; set; }

        [Required]
        [Display(Name = "Cấp bậc")]
        public string JobLevel { get; set; }

        [Required]
        [Display(Name = "Loại hình công việc")]
        public string JobType { get; set; }

        [Display(Name = "Số năm kinh nghiệm")]
        public int? ExperienceYears { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Hạn nộp hồ sơ")]
        public DateTime ExpiredDate { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; }
    }
}