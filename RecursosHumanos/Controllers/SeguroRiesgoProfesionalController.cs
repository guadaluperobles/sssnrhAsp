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


            resultado.Columns.Add("Número de Empleado");
            resultado.Columns.Add("Nombre de Empleado");
            resultado.Columns.Add("Cargo");
            resultado.Columns.Add("RFC");
            resultado.Columns.Add("CURP");
            resultado.Columns.Add("Cedula Profesional");
            resultado.Columns.Add("Nivel Tabular");
            resultado.Columns.Add("Fecha Alta");
            if (trimestre == 1) {
                resultado.Columns.Add("Quincena 01");
                resultado.Columns.Add("Quincena 02");
                resultado.Columns.Add("Quincena 03");
                resultado.Columns.Add("Quincena 04");
                resultado.Columns.Add("Quincena 05");
                resultado.Columns.Add("Quincena 06");
            }
            if (trimestre == 2) {
                resultado.Columns.Add("Quincena 07");
                resultado.Columns.Add("Quincena 08");
                resultado.Columns.Add("Quincena 09");
                resultado.Columns.Add("Quincena 10");
                resultado.Columns.Add("Quincena 11");
                resultado.Columns.Add("Quincena 12");
            }
            if (trimestre == 3) {
                resultado.Columns.Add("Quincena 13");
                resultado.Columns.Add("Quincena 14");
                resultado.Columns.Add("Quincena 15");
                resultado.Columns.Add("Quincena 16");
                resultado.Columns.Add("Quincena 17");
                resultado.Columns.Add("Quincena 18");
            }
            if (trimestre == 4) {
                resultado.Columns.Add("Quincena 19");
                resultado.Columns.Add("Quincena 20");
                resultado.Columns.Add("Quincena 21");
                resultado.Columns.Add("Quincena 22");
                resultado.Columns.Add("Quincena 23");
                resultado.Columns.Add("Quincena 24");
            }
            resultado.Columns.Add("Total");
            resultado.Columns.Add("Contratacion");
            resultado.Columns.Add("Nota");
            try {
                var bdOperativas = _global.BasesDatosOperativas();

                if (resultadoTrimActual != null || resultadoTrimActual.Rows.Count > 0)
                    foreach (DataRow Row in resultadoTrimActual.Rows) {
                        var numeroEmpleado = Row[0].ToString();
                        var nombreEmpleado = Row[1].ToString();
                        var cargo = Row[2].ToString();
                        var rfc = Row[3].ToString();
                        var curp = Row[4].ToString();
                        var cedulaProfecional = Row[5].ToString();
                        var nivelTabular = Row[6].ToString();
                        var fechaAlta = Row[7].ToString();
                        var quincena1 = Global.ObtenerDecimal(Row[8].ToString());
                        var quincena2 = Global.ObtenerDecimal(Row[9].ToString());
                        var quincena3 = Global.ObtenerDecimal(Row[10].ToString());
                        var quincena4 = Global.ObtenerDecimal(Row[11].ToString());
                        var quincena5 = Global.ObtenerDecimal(Row[12].ToString());
                        var quincena6 = Global.ObtenerDecimal(Row[13].ToString());
                        var baseDatos = Row[14].ToString();
                        var nota = "";
                        Decimal total = 0;

                        DataRow bd = bdOperativas.Select($"BaseDatos = '{baseDatos}'").FirstOrDefault();
                        string descripcionBaseDatos = bd?["Descripcion"]?.ToString() ?? "";

                        if (
                            (baseDatos == "IESYS_SYSNGFSON" || 
                            baseDatos == "IESYS_HONOFED" || 
                            baseDatos == "FORMALIZADOS" || 
                            baseDatos == "SYSNGFSON_IB" || 
                            baseDatos == "HONOFED_IB" || 
                            baseDatos == "FORMALIZADOS_IB") &&
                            nivelTabular.Substring(0, 2) != "CF"
                            ) {
                            quincena1 = quincena1 * 2;
                            quincena2 = quincena2 * 2;
                            quincena3 = quincena3 * 2;
                            quincena4 = quincena4 * 2;
                            quincena5 = quincena5 * 2;
                            quincena6 = quincena6 * 2;

                        }
                        
                        total = quincena1 + quincena2 + quincena3 + quincena4 + quincena5 + quincena6;

                        if (total <= 0)
                            nota = "BAJA";

                        if (Convert.ToUInt32(fechaAlta) >= 20250601) {
                            fechaAlta = fechaAlta;
                        }
                        else {
                            fechaAlta = "20250106";
                        }

                        resultado.Rows.Add(numeroEmpleado, nombreEmpleado, cargo, rfc, curp, cedulaProfecional, nivelTabular, Global.ObtenerFecha(fechaAlta), quincena1, quincena2, quincena3, quincena4, quincena5, quincena6, total, descripcionBaseDatos, nota );
                    }

                return resultado;
            }
            catch (Exception ex) {
                return resultado;
            }
        }
    }
}
