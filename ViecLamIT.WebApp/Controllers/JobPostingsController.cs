using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViecLamIT.Data;
using ViecLamIT.Domain.Entities;
using ViecLamIT.WebApp.ViewModels;

namespace ViecLamIT.WebApp.Controllers
{
    [Authorize]
    public class JobPostingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobPostingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: JobPostings/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.EmployerId == currentUser.Id);
            if (company == null) return RedirectToAction("Create", "Companies");
            if (!company.IsVerified) return View("CompanyNotVerified");
            return View(new JobPostingCreateViewModel());
        }

        // POST: JobPostings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobPostingCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.EmployerId == currentUser.Id);
                if (company == null || !company.IsVerified) return Forbid();

                var jobPosting = new JobPosting
                {
                    Title = viewModel.Title,
                    Category = viewModel.Category,
                    Specialization = viewModel.Specialization,
                    Description = viewModel.Description,
                    Requirements = viewModel.Requirements,
                    Benefits = viewModel.Benefits,
                    SalaryFrom = viewModel.SalaryFrom,
                    SalaryTo = viewModel.SalaryTo,
                    JobLevel = viewModel.JobLevel,
                    JobType = viewModel.JobType,
                    ExperienceYears = viewModel.ExperienceYears,
                    ExpiredDate = viewModel.ExpiredDate,
                    CompanyId = company.Id,
                    CreatedDate = DateTime.Now,
                    Status = "Pending"
                };

                _context.Add(jobPosting);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đăng tin thành công! Tin của bạn đang chờ Admin duyệt.";
                return RedirectToAction("Index", "Employer");
            }
            return View(viewModel);
        }


        // GET: JobPostings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Sửa lại query để tải cả thông tin Company
            var jobPosting = await _context.JobPostings
                                         .Include(j => j.Company)
                                         .FirstOrDefaultAsync(j => j.Id == id);

            if (jobPosting == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (jobPosting.Company?.EmployerId != currentUser.Id) return Forbid();

            if (jobPosting.Status != "Active" && jobPosting.Status != "Paused")
            {
                return Forbid("Bạn không thể sửa tin đang chờ duyệt hoặc đã bị từ chối.");
            }

            var viewModel = new JobPostingEditViewModel
            {
                Id = jobPosting.Id,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                Requirements = jobPosting.Requirements,
                Benefits = jobPosting.Benefits,
                SalaryFrom = jobPosting.SalaryFrom,
                SalaryTo = jobPosting.SalaryTo,
                JobLevel = jobPosting.JobLevel,
                JobType = jobPosting.JobType,
                ExperienceYears = jobPosting.ExperienceYears,
                ExpiredDate = jobPosting.ExpiredDate,
                Status = jobPosting.Status
            };

            // Gửi địa chỉ công ty ra View để hiển thị
            ViewData["CompanyAddress"] = $"{jobPosting.Company.StreetAddress}, {jobPosting.Company.Ward}, {jobPosting.Company.Province}";
            return View(viewModel);
        }

        // POST: JobPostings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobPostingEditViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            // Xóa thuộc tính Location khỏi ModelState vì nó không tồn tại trong ViewModel
            ModelState.Remove("Location");

            if (ModelState.IsValid)
            {
                var jobPostingToUpdate = await _context.JobPostings
                                                     .Include(j => j.Company)
                                                     .FirstOrDefaultAsync(j => j.Id == id);

                if (jobPostingToUpdate == null) return NotFound();

                var currentUser = await _userManager.GetUserAsync(User);
                if (jobPostingToUpdate.Company?.EmployerId != currentUser.Id) return Forbid();

                // Cập nhật thủ công từ ViewModel vào Entity
                jobPostingToUpdate.Title = viewModel.Title;
                jobPostingToUpdate.Description = viewModel.Description;
                jobPostingToUpdate.Requirements = viewModel.Requirements;
                jobPostingToUpdate.Benefits = viewModel.Benefits;
                jobPostingToUpdate.SalaryFrom = viewModel.SalaryFrom;
                jobPostingToUpdate.SalaryTo = viewModel.SalaryTo;
                jobPostingToUpdate.JobLevel = viewModel.JobLevel;
                jobPostingToUpdate.JobType = viewModel.JobType;
                jobPostingToUpdate.ExperienceYears = viewModel.ExperienceYears;
                jobPostingToUpdate.ExpiredDate = viewModel.ExpiredDate;
                jobPostingToUpdate.Status = viewModel.Status;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.JobPostings.Any(e => e.Id == jobPostingToUpdate.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Index", "Employer");
            }

            // Nếu có lỗi, cần tải lại địa chỉ để hiển thị lại form
            var originalJob = await _context.JobPostings.AsNoTracking().Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == id);
            ViewData["CompanyAddress"] = $"{originalJob.Company.StreetAddress}, {originalJob.Company.Ward}, {originalJob.Company.Province}";
            return View(viewModel);
        }



        // GET: JobPostings/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings
                .Include(j => j.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            return View(jobPosting);
        }

        // POST: JobPostings/Apply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply([FromForm] int jobId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.UserType != "JobSeeker")
            {
                TempData["ErrorMessage"] = "Chỉ có tài khoản Ứng viên mới có thể ứng tuyển.";
                return RedirectToAction("Details", new { id = jobId });
            }

            bool alreadyApplied = await _context.JobApplications
                                                .AnyAsync(a => a.JobPostingId == jobId && a.ApplicantId == currentUser.Id);

            if (alreadyApplied)
            {
                TempData["SuccessMessage"] = "Bạn đã ứng tuyển công việc này rồi.";
                return RedirectToAction("Details", new { id = jobId });
            }

            var application = new JobApplication
            {
                JobPostingId = jobId,
                ApplicantId = currentUser.Id,
                ApplicationDate = DateTime.Now,
                Status = "Chờ duyệt"
            };

            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ứng tuyển thành công!";
            return RedirectToAction("Details", new { id = jobId });
        }

        // GET: JobPostings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings
                .Include(j => j.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (jobPosting.Company.EmployerId != currentUser.Id)
            {
                return Forbid();
            }

            return View(jobPosting);
        }

        // POST: JobPostings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting != null)
            {
                _context.JobPostings.Remove(jobPosting);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Employer");
        }
    }
}
