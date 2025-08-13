using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ViecLamIT.WebApp.Models;
using ViecLamIT.WebApp.Filters;
using Microsoft.AspNetCore.Identity;
using ViecLamIT.Domain.Entities;

namespace ViecLamIT.WebApp.Controllers
{
    [ServiceFilter(typeof(CheckCompanyExistsFilter))]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString, string category, string specialization, string jobLevel, string province, int? salaryMin, bool matchCv = false)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            Profile userProfile = null;

            if (matchCv && currentUser != null && currentUser.UserType == "JobSeeker")
            {
                userProfile = await _context.Profiles.FindAsync(currentUser.Id);
                if (userProfile != null)
                {
                    category = userProfile.DesiredCategory;
                    specialization = userProfile.DesiredSpecialization;
                    province = userProfile.DesiredProvince;
                    salaryMin = (int?)(userProfile.DesiredSalary / 1000000);
                }
            }

            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentCategory"] = category;
            ViewData["CurrentSpecialization"] = specialization;
            ViewData["CurrentJobLevel"] = jobLevel;
            ViewData["CurrentProvince"] = province;
            ViewData["CurrentSalary"] = salaryMin;

            var jobPostingsQuery = _context.JobPostings.Include(j => j.Company).Where(j => j.Status == "Active");

            if (!String.IsNullOrEmpty(searchString))
                jobPostingsQuery = jobPostingsQuery.Where(j => j.Title.Contains(searchString) || j.Company.Name.Contains(searchString));
            if (!String.IsNullOrEmpty(category))
                jobPostingsQuery = jobPostingsQuery.Where(j => j.Category == category);
            if (!String.IsNullOrEmpty(specialization))
                jobPostingsQuery = jobPostingsQuery.Where(j => j.Specialization == specialization);
            if (!String.IsNullOrEmpty(jobLevel))
                jobPostingsQuery = jobPostingsQuery.Where(j => j.JobLevel == jobLevel);
            if (!String.IsNullOrEmpty(province))
                jobPostingsQuery = jobPostingsQuery.Where(j => j.Company.Province == province);
            if (salaryMin.HasValue)
            {
                decimal salaryValue = salaryMin.Value * 1000000;
                jobPostingsQuery = jobPostingsQuery.Where(j => j.SalaryFrom >= salaryValue);
            }

            ViewBag.Provinces = await _context.Companies.Where(c => c.Province != null).Select(c => c.Province).Distinct().OrderBy(p => p).ToListAsync();

            var finalResult = await jobPostingsQuery.OrderByDescending(j => j.CreatedDate).ToListAsync();
            return View(finalResult);
        }




        public IActionResult Privacy()
        {
            return View();
        }
        // GET: /Home/RandomJob
        public async Task<IActionResult> RandomJob()
        {
            // Lấy tất cả ID của các tin đang hoạt động
            var activeJobIds = await _context.JobPostings
                                             .Where(j => j.Status == "Active")
                                             .Select(j => j.Id)
                                             .ToListAsync();

            if (activeJobIds.Any())
            {
                // Chọn ngẫu nhiên một ID
                var random = new Random();
                int randomIndex = random.Next(activeJobIds.Count);
                int randomJobId = activeJobIds[randomIndex];

                // Chuyển hướng đến trang chi tiết của tin tuyển dụng đó
                return RedirectToAction("Details", "JobPostings", new { id = randomJobId });
            }

            // Nếu không có tin nào, quay lại trang chủ
            TempData["InfoMessage"] = "Hiện tại chưa có tin tuyển dụng nào để hiển thị ngẫu nhiên.";
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}