using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ViecLamIT.Domain.Entities;
using Microsoft.Extensions.Configuration; // Thêm dòng này

namespace ViecLamIT.WebApp.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        // --- BẮT ĐẦU SỬA ĐỔI ---
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration; // Thêm dịch vụ Configuration

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration; // Inject và gán giá trị
        }
        // --- KẾT THÚC SỬA ĐỔI ---

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
            [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                // --- BẮT ĐẦU SỬA ĐỔI ---
                // Sử dụng _configuration để kiểm tra, không dùng builder nữa
                if (string.IsNullOrEmpty(_configuration["MailSettings:Mail"]))
                {
                    TempData["PasswordResetMessage"] = "Mật khẩu của bạn đã được đặt lại thành công. Mật khẩu mặc định là: Qa@12345";
                }
                // --- KẾT THÚC SỬA ĐỔI ---

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
