using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ViecLamIT.Data;
using ViecLamIT.Domain.Entities;
using ViecLamIT.WebApp.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using ViecLamIT.WebApp.Settings;


var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký DbContext (Giữ nguyên)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Đăng ký Identity một cách tường minh (PHẦN THAY ĐỔI QUAN TRỌNG)
// Thay vì dùng AddDefaultIdentity, chúng ta sẽ cấu hình từng phần
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    // Bạn có thể thêm các cấu hình khác về mật khẩu, tài khoản ở đây
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>();

// 3. Cấu hình lại cách ứng dụng xử lý Cookie để nó luôn hoạt động với Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    // Đường dẫn đến trang đăng nhập khi người dùng chưa xác thực
    options.LoginPath = $"/Identity/Account/Login";
    // Đường dẫn đến trang đăng xuất
    options.LogoutPath = $"/Identity/Account/Logout";
    // Đường dẫn đến trang báo lỗi "Cấm truy cập"
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


// Thêm các dịch vụ cần thiết
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<ViecLamIT.WebApp.Filters.CheckCompanyExistsFilter>();

var mailSettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettings);

// Logic chuyển đổi thông minh
if (string.IsNullOrEmpty(mailSettings["Mail"]))
{
    // Nếu không có cấu hình email, dùng "Người Đưa Thư Giả"
    builder.Services.AddTransient<IEmailSender, DevelopmentEmailSender>();
}
else
{
    // Nếu có, dùng "Người Đưa Thư Thật"
    builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

// Bật Authentication và Authorization (quan trọng)
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
// --- BẮT ĐẦU THÊM MỚI ---
// Cấu hình để chạy DbSeeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbSeeder.SeedDataAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during seeding");
    }
}
// --- KẾT THÚC THÊM MỚI ---

app.Run();