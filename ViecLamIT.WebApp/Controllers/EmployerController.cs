using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViecLamIT.Data;
using ViecLamIT.Domain.Entities;
using ViecLamIT.WebApp.ViewModels;

namespace ViecLamIT.WebApp.Controllers
{
    [Authorize] // Bắt buộc đăng nhập
    public class EmployerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> RequestVerification()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.EmployerId == currentUser.Id);

            if (company != null && company.VerificationStatus == "NotRequested")
            {
                company.VerificationStatus = "Pending";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã gửi yêu cầu xác thực đến Admin!";
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.EmployerId == currentUser.Id);
            if (company == null) return RedirectToAction("Create", "Companies");

            var allJobs = await _context.JobPostings
                .Where(j => j.CompanyId == company.Id)
                .Include(j => j.JobApplications)
                .OrderByDescending(j => j.CreatedDate)
                .ToListAsync();

            var viewModel = new EmployerDashboardViewModel
            {
                CompanyInfo = company,
                ApprovedJobs = allJobs.Where(j => j.Status == "Active").ToList(),
                PausedJobs = allJobs.Where(j => j.Status == "Paused").ToList(), // Thêm danh sách mới
                PendingJobs = allJobs.Where(j => j.Status == "Pending").ToList(),
                RejectedJobs = allJobs.Where(j => j.Status == "Rejected").ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var jobPosting = await _context.JobPostings.Include(j => j.Company)
                                         .FirstOrDefaultAsync(j => j.Id == id);

            if (jobPosting == null || jobPosting.Company.EmployerId != currentUser.Id)
            {
                return Forbid();
            }

            if (jobPosting.Status == "Active")
            {
                jobPosting.Status = "Paused"; // Tạm ẩn
            }
            else if (jobPosting.Status == "Paused")
            {
                jobPosting.Status = "Active"; // Đăng lại
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Employer/ViewApplicants/5
        public async Task<IActionResult> ViewApplicants(int id) // id này là của JobPosting
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.UserType != "Employer")
            {
                return Forbid();
            }

            // Lấy tin tuyển dụng và đảm bảo nó thuộc về nhà tuyển dụng này
            var jobPosting = await _context.JobPostings
                .Include(j => j.JobApplications) // Lấy danh sách các đơn ứng tuyển
                    .ThenInclude(ja => ja.Applicant) // Với mỗi đơn, lấy thông tin người ứng tuyển
                .FirstOrDefaultAsync(j => j.Id == id && j.Company.EmployerId == currentUser.Id);

            if (jobPosting == null)
            {
                return NotFound(); // Không tìm thấy hoặc không có quyền xem
            }

            return View(jobPosting);
        }
    }
}