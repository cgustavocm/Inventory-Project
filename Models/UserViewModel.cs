using System.Collections.Generic;

namespace Inventory_Project.Models
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class ManageUserRoleViewModel
    {
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }

    public class EditUserRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<ManageUserRoleViewModel> Roles { get; set; } = new();
    }
}
