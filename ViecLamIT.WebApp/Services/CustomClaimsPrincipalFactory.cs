// File: ViecLamIT.WebApp/Services/CustomClaimsPrincipalFactory.cs
namespace ViecLamIT.WebApp.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using ViecLamIT.Domain.Entities;

// Lớp này kế thừa từ lớp gốc và nhiệm vụ của nó là thêm các claim tùy chỉnh
public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public CustomClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    // Ghi đè phương thức tạo claims
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        // Gọi phương thức gốc để lấy các claim cơ bản (như SecurityStamp)
        var identity = await base.GenerateClaimsAsync(user);

        // Thêm các claim quan trọng mà hệ thống có thể đã bỏ sót
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
        identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
        identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

        // Bạn có thể thêm các claim tùy chỉnh khác ở đây nếu cần
        identity.AddClaim(new Claim("UserType", user.UserType));

        return identity;
    }
}
