using Microsoft.AspNetCore.Authorization; // Thêm để dùng [Authorize]
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // Thêm để lấy User ID
using ViecLamIT.Domain.Entities;
using ViecLamIT.WebApp.ViewModels;

namespace ViecLamIT.WebApp.Controllers
{
    [Authorize] // Bắt buộc người dùng phải đăng nhập để truy cập controller này
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        // Sửa hàm khởi tạo
        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Nếu ModelState không hợp lệ, chúng ta cần biết lý do tại sao.
                var errorList = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                // Tạo một chuỗi thông báo lỗi chi tiết
                var errorMessage = "ModelState không hợp lệ. Các lỗi bao gồm:\n";
                foreach (var error in errorList)
                {
                    if (error.Value.Any())
                    {
                        errorMessage += $"Trường '{error.Key}': {string.Join(", ", error.Value)}\n";
                    }
                }

                // Trả về một trang trắng chỉ chứa danh sách các lỗi để xem cho rõ.
                return Content(errorMessage);
            }

            // Nếu hợp lệ, tiếp tục xử lý
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Content("Lỗi nghiêm trọng: Không thể định danh người dùng.");
            }

            var company = new Company
            {
                Name = viewModel.Name,
                TaxCode = viewModel.TaxCode,
                BusinessType = viewModel.BusinessType,
                Website = viewModel.Website,
                Province = viewModel.Province,
                Ward = viewModel.Ward,
                StreetAddress = viewModel.StreetAddress,
                EmployerId = currentUser.Id,
                IsVerified = false,
                VerificationStatus = "NotRequested"
            };

            _context.Add(company);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Employer");
        }

        // GET: Companies/Edit
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.EmployerId == currentUser.Id);
            if (company == null)
            {
                // Nếu vì lý do nào đó họ chưa có công ty, chuyển hướng đi tạo
                return RedirectToAction("Create");
            }
            return View(company);
        }

        // POST: Companies/Edit
        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Company company)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var originalCompany = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == company.Id);

            // Kiểm tra quyền sở hữu
            if (originalCompany == null || originalCompany.EmployerId != currentUser.Id)
            {
                return Forbid();
            }

            // Giữ lại EmployerId để không bị ghi đè
            company.EmployerId = originalCompany.EmployerId;

            // --- BẮT ĐẦU LOGIC QUAN TRỌNG ---
            // Đặt lại trạng thái để chờ Admin duyệt lại
            company.IsVerified = false;
            company.VerificationStatus = "Pending";

            // Tìm tất cả các tin đang Active của công ty này và tạm ẩn (Paused)
            var activeJobs = await _context.JobPostings
                                         .Where(j => j.CompanyId == company.Id && j.Status == "Active")
                                         .ToListAsync();

            foreach (var job in activeJobs)
            {
                job.Status = "Paused";
            }
            // --- KẾT THÚC LOGIC QUAN TRỌNG ---

            ModelState.Remove("Employer");
            if (ModelState.IsValid)
            {
                _context.Update(company);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công! Công ty của bạn đã được đưa vào hàng chờ để Admin xác thực lại.";
                return RedirectToAction("Index", "Employer");
            }
            return View(company);
        }




    }
}