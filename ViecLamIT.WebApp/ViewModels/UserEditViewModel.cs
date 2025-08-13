using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ViecLamIT.WebApp.ViewModels
{
    public class UserEditViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }

        // Danh sách các vai trò mà người dùng này đang có
        public IList<string> Roles { get; set; }

        // Danh sách tất cả các vai trò có trong hệ thống để tạo dropdown list
        public List<SelectListItem> AllRoles { get; set; }
    }
}