using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using RecursosHumanos.ViewModel;
using System.Data;

namespace RecursosHumanos.Controllers {
    public class SeguroRiesgoProfesionalController : Controller {

        private readonly ApplicationDbContext _context;
        private readonly Global _global;
        public SeguroRiesgoProfesionalController(ApplicationDbContext context, Global global) {
            _context = context;
            _global = global;
        }
        // GET: CentroTrabajoController
        public async Task<IActionResult> Index() {
            var Consulta = await _context.Consulta.FirstOrDefaultAsync(x => x.Nombre == "SeguroRiesgoProfecional");
            int Ejercicio = DateTime.Now.Year;
            int Trimestre = ((DateTime.Now.Month - 1) / 3) + 1;

            var consultaLocal = $" DECLARE @Anio			    INT = {Ejercicio}; ";
            consultaLocal += $" DECLARE @Trimestre		        INT = {Trimestre}; ";

            var Resultado = _global.ConsultaGeneral(consultaLocal + Consulta.Sql);


            var Model = new ConsultaTrimestralViewModel {
                Ejercicio = Ejercicio,
                Trimestre = Trimestre,
                Models = Resultado
            };
            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, int Ejercicio, int Trimestre) {
            var Consulta = await _context.Consulta.FirstOrDefaultAsync(x => x.Nombre == "SeguroRiesgoProfecional");
            
            var consultaLocal = $" DECLARE @Anio			    INT = {Ejercicio}; ";
            consultaLocal +=    $" DECLARE @Trimestre		    INT = {Trimestre}; ";

            var Resultado = _global.ConsultaGeneral(consultaLocal + Consulta.Sql);


            var Model = new ConsultaTrimestralViewModel {
                Ejercicio = Ejercicio,
                Trimestre = Trimestre,
                Models = Resultado
            };
            return View(Model);
        }

        // POST: CentroTrabajoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarExcel(ConsultaTrimestralViewModel model) {
            try {

                var Consulta = await _context.Consulta.FirstOrDefaultAsync(x => x.Nombre == "SeguroRiesgoProfecional");

                var consultaLocal = $" DECLARE @Anio			    INT = {model.Ejercicio}; ";
                consultaLocal += $" DECLARE @Trimestre		    INT = {model.Trimestre}; ";

                var Resultado = _global.ConsultaGeneral(consultaLocal + Consulta.Sql);

                return ArchivoController.ExportarExcel(Resultado, $"Seguro de riesgos profesional {model.Ejercicio}/{model.Trimestre}");
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
