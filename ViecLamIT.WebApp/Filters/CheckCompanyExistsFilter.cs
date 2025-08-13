using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq; // Thêm để dùng .Any()
using System.Security.Claims;
using ViecLamIT.Data;
using ViecLamIT.Domain.Entities;

namespace ViecLamIT.WebApp.Filters
{
    public class CheckCompanyExistsFilter : IAsyncActionFilter
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckCompanyExistsFilter(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            // Chỉ kiểm tra nếu người dùng đã đăng nhập
            if (user.Identity.IsAuthenticated)
            {
                // Lấy UserType từ Claims (đáng tin cậy hơn IsInRole ngay sau khi đăng ký)
                var userTypeClaim = user.Claims.FirstOrDefault(c => c.Type == "UserType");

                if (userTypeClaim != null && userTypeClaim.Value == "Employer")
                {
                    var userId = _userManager.GetUserId(user);
                    var hasCompany = _context.Companies.Any(c => c.EmployerId == userId);

                    // Nếu là Employer mà chưa có công ty
                    if (!hasCompany)
                    {
                        // Tránh vòng lặp vô hạn nếu đang ở trang tạo công ty hoặc các trang Identity
                        var controllerName = context.Controller.GetType().Name;
                        if (controllerName != "CompaniesController" && !context.HttpContext.Request.Path.ToString().Contains("/Identity/"))
                        {
                            context.Result = new RedirectToActionResult("Create", "Companies", null);
                            return; // Dừng lại và chuyển hướng
                        }
                    }
                }
            }

            await next(); // Nếu mọi thứ ổn, cho phép tiếp tục
        }
    }
}