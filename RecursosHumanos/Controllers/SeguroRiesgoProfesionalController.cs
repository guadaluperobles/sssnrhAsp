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

            var Resultado = ProcesarConsulta(Ejercicio, Trimestre);

            var Model = new ConsultaTrimestralViewModel {
                Ejercicio = Ejercicio,
                Trimestre = Trimestre,
                Models = await Resultado
            };
            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, int Ejercicio, int Trimestre) {

            var Resultado = await ProcesarConsulta(Ejercicio, Trimestre);

            var Model = new ConsultaTrimestralViewModel {
                Ejercicio = Ejercicio,
                Trimestre = Trimestre,
                Models = Resultado
            };
            return View(Model);
        }

        // POST: CentroTrabajoController/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarExcel(ConsultaTrimestralViewModel model) {
            try {
                var Resultado = await ProcesarConsulta(model.Ejercicio, model.Trimestre);
                return ArchivoController.ExportarExcel(Resultado, $"Seguro de riesgos profesional {model.Ejercicio}/{model.Trimestre}");
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        public async Task<DataTable> ProcesarConsulta(int ejercicio, int trimestre) {
            var consultaBD = await _context.Consulta.FirstOrDefaultAsync(x => x.Nombre == "SeguroRiesgoProfecional");

            int ejercicioAnterior = ejercicio;
            int trimestreAnterior = trimestre - 1;

            if (trimestreAnterior == 0) {
                trimestreAnterior = 4;
                ejercicioAnterior--;
            }

            if (consultaBD == null) 
                throw new Exception("No se encontró la consulta SeguroRiesgoProfecional.");
            
            var consultaLocalTrimActual = $@" DECLARE @Anio			        INT = {ejercicio}; 
                                              DECLARE @Trimestre		    INT = {trimestre}; "; 

            var consultaLocalTrimAnterior = $@" DECLARE @Anio			    INT = {ejercicioAnterior}; 
                                                DECLARE @Trimestre		    INT = {trimestreAnterior}; ";
            var resultado = new DataTable();

            var resultadoTrimActual = _global.ConsultaGeneral(consultaLocalTrimActual + consultaBD.Sql);
            var resultadoTrimAnterior = _global.ConsultaGeneral(consultaLocalTrimAnterior + consultaBD.Sql);

            try {

                return resultado;
            }
            catch (Exception ex) {
                return resultado;
            }
        }
    }
}
