using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Models;

public class UsuariosController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public UsuariosController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    // GET: Usuarios
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    // GET: Usuarios/EditRoles/{id}
    public async Task<IActionResult> EditRoles(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var model = new UserRolesViewModel { UserId = user.Id, Email = user.Email ?? string.Empty };
        var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            model.Roles.Add(new RoleSelection { RoleName = role ?? string.Empty, Selected = userRoles.Contains(role) });
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoles(UserRolesViewModel model)
    {
        if (model == null) return BadRequest();
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        var selectedRoles = model.Roles.Where(r => r.Selected).Select(r => r.RoleName).ToList();
        var userRoles = await _userManager.GetRolesAsync(user);

        var rolesToAdd = selectedRoles.Except(userRoles);
        var rolesToRemove = userRoles.Except(selectedRoles);

        if (rolesToAdd.Any()) await _userManager.AddToRolesAsync(user, rolesToAdd);
        if (rolesToRemove.Any()) await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

        return RedirectToAction(nameof(Index));
    }

    // GET: Usuarios/EditPermisos/{id}
    public async Task<IActionResult> EditPermisos(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var permisos = await _context.PermisoVistaModel.ToListAsync();
        var asignados = await _context.UsuarioPermiso.Where(up => up.UserId == id && up.Eliminado == null).Select(up => up.PermisoId).ToListAsync();

        var model = new UserPermisosViewModel { UserId = user.Id, Email = user.Email ?? string.Empty };
        foreach (var p in permisos)
        {
            model.Permisos.Add(new PermisoSelection { Id = p.Id ?? 0, Nombre = p.Nombre ?? string.Empty, Selected = asignados.Contains(p.Id) });
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPermisos(UserPermisosViewModel model)
    {
        if (model == null) return BadRequest();
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        var existentes = await _context.UsuarioPermiso.Where(up => up.UserId == model.UserId).ToListAsync();

        // Eliminar relaciones que ya no están seleccionadas
        var seleccionados = model.Permisos.Where(p => p.Selected).Select(p => p.Id).ToHashSet();
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

        // Agregar nuevas asignaciones restantes en seleccionados
        foreach (var nuevoId in seleccionados)
        {
            _context.UsuarioPermiso.Add(new UsuarioPermiso { UserId = model.UserId, PermisoId = nuevoId });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
