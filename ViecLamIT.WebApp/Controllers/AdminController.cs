using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViecLamIT.Data;
using ViecLamIT.Domain.Entities;
using ViecLamIT.WebApp.ViewModels; 
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace ViecLamIT.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {

            _context = context;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalCompanies = await _context.Companies.CountAsync(),
                TotalJobPostings = await _context.JobPostings.CountAsync(),
                TotalApplications = await _context.JobApplications.CountAsync()
            };
            return View(viewModel);
        }
        // GET: /Admin/ManageUsers
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }
        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                // Ngăn Admin tự xóa chính mình
                var currentAdminId = _userManager.GetUserId(User);
                if (user.Id == currentAdminId)
                {
                    // Có thể thêm một thông báo lỗi ở đây
                    TempData["ErrorMessage"] = "Bạn không thể tự xóa tài khoản của chính mình.";
                    return RedirectToAction(nameof(ManageUsers));
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa người dùng thành công.";
            }

            return RedirectToAction(nameof(ManageUsers));
        }
        // GET: Admin/EditUser/5
        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _context.Roles.ToListAsync();

            var viewModel = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType,
                Roles = userRoles,
                AllRoles = allRoles.Select(r => new SelectListItem(r.Name, r.Name)).ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserEditViewModel viewModel)
        {
            // Bỏ qua kiểm tra UserType vì chúng ta đã xóa nó khỏi form
            ModelState.Remove("UserType");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(viewModel.Id);
                if (user == null)
                {
                    return NotFound();
                }

                // Cập nhật các thông tin cơ bản
                user.FirstName = viewModel.FirstName;
                user.LastName = viewModel.LastName;
                // Cập nhật vai trò
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (viewModel.Roles != null && viewModel.Roles.Any())
                {
                    await _userManager.AddToRolesAsync(user, viewModel.Roles);

                    // TỰ ĐỘNG CẬP NHẬT UserType DỰA VÀO VAI TRÒ ĐẦU TIÊN
                    user.UserType = viewModel.Roles.First();
                }
                else
                {
                    // Nếu không có vai trò nào, có thể gán một giá trị mặc định
                    user.UserType = "JobSeeker";
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Cập nhật người dùng thành công.";
                    return RedirectToAction(nameof(ManageUsers));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            viewModel.AllRoles = await _context.Roles.Select(r => new SelectListItem(r.Name, r.Name)).ToListAsync();
            return View(viewModel);
        }
        // POST: /Admin/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Để an toàn, không cho phép Admin tự đặt lại mật khẩu của chính mình bằng chức năng này
                var currentAdminId = _userManager.GetUserId(User);
                if (user.Id == currentAdminId)
                {
                    TempData["ErrorMessage"] = "Bạn không thể tự đặt lại mật khẩu của chính mình bằng chức năng này.";
                    return RedirectToAction(nameof(ManageUsers));
                }

                // Quy trình đặt lại mật khẩu chuẩn của ASP.NET Core Identity
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, "Qa@12345");

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Đặt lại mật khẩu cho người dùng '{user.Email}' thành công. Mật khẩu mới là: Qa@12345";
                }
                else
                {
                    // Gộp các lỗi thành một chuỗi để hiển thị
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = $"Lỗi khi đặt lại mật khẩu: {errors}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        // GET: /Admin/GetJobPostingDetails/5
        public async Task<IActionResult> GetJobPostingDetails(int id)
        {
            var jobPosting = await _context.JobPostings
                                           .Include(j => j.Company)
                                           .FirstOrDefaultAsync(j => j.Id == id);
            if (jobPosting == null)
            {
                return NotFound();
            }
            return PartialView("_JobPostingDetailsPartial", jobPosting);
        }

        // POST: /Admin/ApproveJobPosting
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveJobPosting(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting != null)
            {
                jobPosting.Status = "Active"; // Chuyển trạng thái thành Active
                jobPosting.AdminComment = null; // Xóa comment cũ
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageJobPostings));
        } 

        // POST: /Admin/RejectJobPosting
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectJobPosting(int id, string rejectionReason)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting != null)
            {
                jobPosting.Status = "Rejected"; // Chuyển trạng thái thành Rejected
                jobPosting.AdminComment = rejectionReason; // Lưu lý do
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageJobPostings));
        }
        // GET: /Admin/ManageJobPostings
        public async Task<IActionResult> ManageJobPostings()
        {
            var allJobs = await _context.JobPostings
                                        .Include(j => j.Company)
                                        .OrderByDescending(j => j.CreatedDate)
                                        .ToListAsync();

            var viewModel = new ManageJobPostingsViewModel
            {
                PendingJobs = allJobs.Where(j => j.Status == "Pending").ToList(),
                ApprovedJobs = allJobs.Where(j => j.Status == "Active" || j.Status == "Paused").ToList(),
                RejectedJobs = allJobs.Where(j => j.Status == "Rejected").ToList()
            };

            return View(viewModel);
        }
        // Công ty //////////////////////////////////////////
        // GET: /Admin/ManageCompanies
        // GET: /Admin/ManageCompanies
        public async Task<IActionResult> ManageCompanies()
        {
            var companiesData = await _context.Companies
                                              .Include(c => c.JobPostings)
                                              .Select(c => new CompanyAdminViewModel
                                              {
                                                  Id = c.Id,
                                                  Name = c.Name,
                                                  // --- BẮT ĐẦU SỬA ĐỔI ---
                                                  // Đếm số tin có Status là "Active"
                                                  ApprovedJobsCount = c.JobPostings.Count(jp => jp.Status == "Active"),
                                                  // --- KẾT THÚC SỬA ĐỔI ---
                                                  TotalApplicationsCount = c.JobPostings.SelectMany(jp => jp.JobApplications).Count(),
                                                  IsVerified = c.IsVerified
                                              })
                                              .ToListAsync();

            var viewModel = new ManageCompaniesViewModel
            {
                PendingCompanies = companiesData.Where(c => !c.IsVerified).ToList(),
                VerifiedCompanies = companiesData.Where(c => c.IsVerified).ToList()
            };

            return View(viewModel);
        }




        // GET: /Admin/GetCompanyDetails/5
        public async Task<IActionResult> GetCompanyDetails(int id)
        {
            var company = await _context.Companies
                                        .Include(c => c.Employer) // Lấy thông tin người sở hữu
                                        .Include(c => c.JobPostings)
                                            .ThenInclude(jp => jp.JobApplications)
                                        .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null) return NotFound();

            var viewModel = new CompanyDetailsAdminViewModel
            {
                // Thông tin công ty
                Name = company.Name,
                Address = $"{company.StreetAddress}, {company.Ward}, {company.Province}",
                TaxCode = company.TaxCode ?? "Chưa cung cấp",
                Website = company.Website,
                Description = company.Description,

                // Thông tin người đại diện
                RepresentativeName = company.Employer?.FirstName + " " + company.Employer?.LastName,
                RepresentativeEmail = company.Employer?.Email ?? "N/A",
                RepresentativePhone = company.Employer?.PhoneNumber ?? "Chưa cung cấp",

                // --- BẮT ĐẦU SỬA ĐỔI ---
                // Thống kê hoạt động theo hệ thống Status mới
                TotalJobs = company.JobPostings.Count,
                ApprovedJobs = company.JobPostings.Count(j => j.Status == "Active"),
                RejectedJobs = company.JobPostings.Count(j => j.Status == "Rejected"),
                PendingJobs = company.JobPostings.Count(j => j.Status == "Pending"),
                // --- KẾT THÚC SỬA ĐỔI ---
                TotalApplications = company.JobPostings.SelectMany(j => j.JobApplications).Count()
            };

            return PartialView("_CompanyDetailsPartial", viewModel);
        }


        // GET: Admin/EditCompany/5
        public async Task<IActionResult> EditCompany(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            // Action này không cần sửa gì thêm, nó chỉ cần trả về đúng đối tượng company
            // Việc hiển thị 3 ô địa chỉ sẽ do View xử lý
            return View(company);
        }

        // POST: Admin/EditCompany/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompany(int id,
            // --- BẮT ĐẦU SỬA ĐỔI ---
            // Thay thế "Address" và "District" bằng các trường mới trong [Bind]
            [Bind("Id,Name,LogoUrl,Province,Ward,StreetAddress,Website,Description,CompanySize,EmployerId,IsVerified,VerificationStatus,Industry,TaxCode,BusinessType")] Company company)
        // --- KẾT THÚC SỬA ĐỔI ---
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Employer");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Companies.Any(e => e.Id == company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageCompanies));
            }
            return View(company);
        }


        [HttpPost]
        public async Task<IActionResult> VerifyCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                company.IsVerified = true;
                company.VerificationStatus = "Verified";

                // --- BẮT ĐẦU LOGIC QUAN TRỌNG ---
                // Tìm tất cả các tin đã bị tạm ẩn (Paused) của công ty này và kích hoạt lại
                var pausedJobs = await _context.JobPostings
                                             .Where(j => j.CompanyId == company.Id && j.Status == "Paused")
                                             .ToListAsync();

                foreach (var job in pausedJobs)
                {
                    job.Status = "Active";
                }
                // --- KẾT THÚC LOGIC QUAN TRỌNG ---

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageCompanies));
        }


    }
}