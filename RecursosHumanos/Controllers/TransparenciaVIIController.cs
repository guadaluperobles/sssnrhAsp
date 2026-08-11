using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using RecursosHumanos.ViewModel;

namespace RecursosHumanos.Controllers {
    public class TransparenciaVIIController : Controller {

        private readonly ApplicationDbContext _context;
        private readonly Global _global;
        public TransparenciaVIIController(ApplicationDbContext context, Global global) {
            _context = context;
            _global = global;
        }

        // GET: TransparenciaVIIController
        public async Task<IActionResult> Index() {
            var Consulta = await _context.Consulta.FirstOrDefaultAsync(x => x.Nombre == "TotalPercepcionesEmpleados");
            if (Consulta == null)
                return View();

            int anio = DateTime.Now.Year;
            int trimestre = ((DateTime.Now.Month - 1) / 3) + 1; 

            var consultaLocal    = $" DECLARE @Anio			    INT = {anio}; ";
            consultaLocal       += $" DECLARE @Trimestre		INT = {trimestre}; ";

            var Resultado = _global.ConsultaGeneral(consultaLocal + Consulta.Sql);

            
            var Model = new ConsultaTrimestralViewModel {
                Ejercicio = anio,
                Trimestre = trimestre,
                Models = Resultado
            };
            return View(Model);
        }

        // GET: TransparenciaVIIController
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index() {

            var Consulta = await _context.Consulta.FirstOrDefaultAsync(x => x.Nombre == "TotalPercepcionesEmpleados");
            if (Consulta == null)
                return View();

            var Resultado = _global.ConsultaGeneral(Consulta.Sql);

            return View(Resultado);
        }*/

        // GET: TransparenciaVIIController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: TransparenciaVIIController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: TransparenciaVIIController/Create
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

        // GET: TransparenciaVIIController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: TransparenciaVIIController/Edit/5
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

        // GET: TransparenciaVIIController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: TransparenciaVIIController/Delete/5
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