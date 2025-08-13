using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using ViecLamIT.Domain.Entities;

namespace ViecLamIT.WebApp.Services
{
    public class DevelopmentEmailSender : IEmailSender
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DevelopmentEmailSender(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Thay vì gửi mail, phương thức này sẽ đặt lại mật khẩu trực tiếp
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, "Qa@12345");
            }
            // Chúng ta sẽ dùng TempData để thông báo ở bước sau
            await Task.CompletedTask;
        }
    }
}
