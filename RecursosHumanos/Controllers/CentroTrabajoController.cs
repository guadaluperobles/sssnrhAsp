using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;

namespace RecursosHumanos.Controllers {
    public class CentroTrabajoController : Controller {

        private readonly ApplicationDbContext _context;
        private readonly Global _global;
        public CentroTrabajoController(ApplicationDbContext context, Global global) {
            _context = context;
            _global = global;
        }
        // GET: CentroTrabajoController
        public async Task<IActionResult> Index() {
            var validaciones = await _context.Consulta.FirstOrDefaultAsync(x => x.Nombre == "ValidarCentrosTrabajos");

            var Resultado = _global.ConsultaGeneral(validaciones.Sql);

            return View(Resultado);
        }

        // GET: CentroTrabajoController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: CentroTrabajoController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: CentroTrabajoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }

        // GET: CentroTrabajoController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: CentroTrabajoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }

        // GET: CentroTrabajoController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: CentroTrabajoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }
    }
}
