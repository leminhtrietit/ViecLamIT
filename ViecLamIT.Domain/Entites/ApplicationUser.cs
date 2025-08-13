using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ViecLamIT.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    // Thay thế FullName bằng các trường chi tiết hơn
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string UserType { get; set; }
}