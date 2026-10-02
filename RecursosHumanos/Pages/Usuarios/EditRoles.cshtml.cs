using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Models;

namespace RecursosHumanos.Pages.Usuarios
{
    public class EditRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditRolesModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public UserRolesViewModel ModelData { get; set; } = new UserRolesViewModel();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ModelData.UserId = user.Id;
            ModelData.Email = user.Email ?? string.Empty;

            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                ModelData.Roles.Add(new RoleSelection { RoleName = role ?? string.Empty, Selected = userRoles.Contains(role) });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(ModelData.UserId);
            if (user == null) return NotFound();

            var selected = ModelData.Roles.Where(r => r.Selected).Select(r => r.RoleName).ToList();
            var current = await _userManager.GetRolesAsync(user);

            var toAdd = selected.Except(current);
            var toRemove = current.Except(selected);

            if (toAdd.Any()) await _userManager.AddToRolesAsync(user, toAdd);
            if (toRemove.Any()) await _userManager.RemoveFromRolesAsync(user, toRemove);

            return RedirectToPage("Index");
        }
    }
}
