using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RecursosHumanos.Pages.Usuarios
{
    public class DetailsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IdentityUser? UserItem { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            UserItem = await _userManager.FindByIdAsync(id);
            if (UserItem == null) return NotFound();
            return Page();
        }
    }
}
