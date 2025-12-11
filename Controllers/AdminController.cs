using System.Linq;
using System.Threading.Tasks;
using Inventory_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var model = new System.Collections.Generic.List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(model);
        }

        // GET: /Admin/EditUserRoles/{id}
        public async Task<IActionResult> EditUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email
            };

            foreach (var role in _roleManager.Roles)
            {
                model.Roles.Add(new ManageUserRoleViewModel
                {
                    RoleName = role.Name,
                    Selected = await _userManager.IsInRoleAsync(user, role.Name)
                });
            }

            return View(model);
        }

        // POST: /Admin/EditUserRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            foreach (var roleVm in model.Roles)
            {
                if (roleVm.Selected && !currentRoles.Contains(roleVm.RoleName))
                {
                    await _userManager.AddToRoleAsync(user, roleVm.RoleName);
                }

                if (!roleVm.Selected && currentRoles.Contains(roleVm.RoleName))
                {
                    await _userManager.RemoveFromRoleAsync(user, roleVm.RoleName);
                }
            }

            return RedirectToAction(nameof(Users));
        }
    }
}
