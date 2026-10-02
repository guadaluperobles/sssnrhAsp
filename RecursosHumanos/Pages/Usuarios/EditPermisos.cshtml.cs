using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Models;

namespace RecursosHumanos.Pages.Usuarios
{
    public class EditPermisosModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EditPermisosModel(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public UserPermisosViewModel ModelData { get; set; } = new UserPermisosViewModel();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ModelData.UserId = user.Id;
            ModelData.Email = user.Email ?? string.Empty;

            var permisos = await _context.PermisoVistaModel.ToListAsync();
            var asignados = await _context.UsuarioPermiso.Where(up => up.UserId == id && up.Eliminado == null).Select(up => up.PermisoId).ToListAsync();

            foreach (var p in permisos)
            {
                ModelData.Permisos.Add(new PermisoSelection { Id = p.Id ?? 0, Nombre = p.Nombre ?? string.Empty, Selected = asignados.Contains(p.Id) });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(ModelData.UserId)) return BadRequest();
            var user = await _userManager.FindByIdAsync(ModelData.UserId);
            if (user == null) return NotFound();

            var existentes = await _context.UsuarioPermiso.Where(up => up.UserId == ModelData.UserId).ToListAsync();
            var seleccionados = ModelData.Permisos.Where(p => p.Selected).Select(p => p.Id).ToHashSet();

            foreach (var ex in existentes)
            {
                if (!seleccionados.Contains(ex.PermisoId ?? 0))
                {
                    _context.UsuarioPermiso.Remove(ex);
                }
                else
                {
                    seleccionados.Remove(ex.PermisoId ?? 0);
                }
            }

            foreach (var nuevoId in seleccionados)
            {
                _context.UsuarioPermiso.Add(new UsuarioPermiso { UserId = ModelData.UserId, PermisoId = nuevoId });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
