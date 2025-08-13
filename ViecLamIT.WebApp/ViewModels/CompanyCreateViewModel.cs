using System.ComponentModel.DataAnnotations;

namespace ViecLamIT.WebApp.ViewModels
{
    public class CompanyCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên công ty.")]
        [Display(Name = "Tên công ty")]
        public string Name { get; set; }

        [Display(Name = "Link Logo công ty")]
        public string? LogoUrl { get; set; }

        [Display(Name = "Website")]
        [Url(ErrorMessage = "Địa chỉ website không hợp lệ.")]
        public string? Website { get; set; }

        [Display(Name = "Mô tả về công ty")]
        public string? Description { get; set; }

        [Display(Name = "Quy mô")]
        public string? CompanySize { get; set; }

     
        [Required(ErrorMessage = "Vui lòng nhập mã số thuế.")]
        [Display(Name = "Mã số thuế")]
        public string TaxCode { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại hình kinh doanh.")]
        [Display(Name = "Loại hình kinh doanh")]
        public string BusinessType { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố.")]
        [Display(Name = "Tỉnh/Thành phố")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Phường/Xã.")]
        [Display(Name = "Phường/Xã")]
        public string Ward { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Số nhà, tên đường.")]
        [Display(Name = "Số nhà, tên đường")]
        public string StreetAddress { get; set; }
    }
}
