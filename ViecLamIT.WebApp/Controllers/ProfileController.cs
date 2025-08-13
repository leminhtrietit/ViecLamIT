using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViecLamIT.Data;
using ViecLamIT.Domain.Entities;

namespace ViecLamIT.WebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.UserType != "JobSeeker")
            {
                return Forbid();
            }

            var profile = await _context.Profiles.FindAsync(currentUser.Id);
            if (profile == null)
            {
                profile = new Profile { UserId = currentUser.Id };
            }

            return View(profile);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Profile profileViewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || profileViewModel.UserId != currentUser.Id)
            {
                return NotFound();
            }

            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                var profileToUpdate = await _context.Profiles.FindAsync(currentUser.Id);
                if (profileToUpdate == null)
                {
                    profileToUpdate = new Profile { UserId = currentUser.Id };
                    _context.Profiles.Add(profileToUpdate);
                }

                // Cập nhật tất cả các trường từ form
                profileToUpdate.DesiredCategory = profileViewModel.DesiredCategory;
                profileToUpdate.DesiredSpecialization = profileViewModel.DesiredSpecialization;
                profileToUpdate.DesiredSalary = profileViewModel.DesiredSalary;
                profileToUpdate.DesiredProvince = profileViewModel.DesiredProvince;
                profileToUpdate.WorkExperience = profileViewModel.WorkExperience;
                profileToUpdate.Education = profileViewModel.Education;
                profileToUpdate.Skills = profileViewModel.Skills;
                profileToUpdate.Certifications = profileViewModel.Certifications;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                return RedirectToAction(nameof(Edit));
            }
            return View(profileViewModel);
        }

        // GET: Profile/Details/{userId}
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm hồ sơ của người dùng có ID tương ứng
            var profile = await _context.Profiles
                                        .Include(p => p.User) // Lấy cả thông tin User (Tên, Email)
                                        .FirstOrDefaultAsync(p => p.UserId == id);

            if (profile == null)
            {
                // Có thể người này chưa tạo hồ sơ, trả về một trang thông báo
                // Hoặc tạm thời trả về NotFound
                return NotFound();
            }

            return View(profile);
        }
    }
}