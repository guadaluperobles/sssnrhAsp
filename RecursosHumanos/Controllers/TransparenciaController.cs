using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using System.Data;

namespace RecursosHumanos.Controllers {
    public class TransparenciaController : Controller {
        // GET: TransparenciaControllerprivate readonly ConeccionService _coneccionService;
    
        private readonly ConeccionService _coneccionService;
        public TransparenciaController(ConeccionService coneccionService, IConfiguration configuration) {
            _coneccionService = coneccionService;
        }
        public ActionResult Index() {
            return View();
        }
        public ActionResult TransparenciaIX() {
            string resultado = ConsultaTransparencia();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> TransparenciaIX(int Ejercicio, int Trimestre, IFormFile archivo) {

            Global global = new Global(_coneccionService);
            DataTable archivoPlazasVacantes = new DataTable();
            DataTable Conglomerado = new DataTable();
            DataTable Resultado = new DataTable();

            if (archivo != null && archivo.Length > 0) {
                string[] plazasVacantes = await ArchivoController.LeerTxt(archivo);
                var plazasVacantesDT = LeerPlazasVacantes(plazasVacantes);

                foreach (string plaza in plazasVacantes) {
                    string[] row = plaza.Split('|');

                    row = row.Select(x => x.Trim()).ToArray();

                    string puesto = row[9].Trim().Replace(" ", "");
                    string areaAdscripcion = row[17];
                    string estatus = "Vacante";
                    string tipoPersonal;

                    if (puesto.StartsWith("M")) {
                        tipoPersonal = "Base";
                    }
                    else if (puesto.StartsWith("CF")) {
                        tipoPersonal = "Confianza";
                    }
                    else {
                        tipoPersonal = "";
                    }

                    DataTable dtTemp = global.ConsultaGeneral($"DECLARE  @numero  VARCHAR(10) = '{puesto}'; " + ConsultasModel.ConsultaPuesto, "IESYS_SYSNGFSON");

                    archivoPlazasVacantes.Rows.Add(
                        puesto,
                        tipoPersonal,
                        areaAdscripcion,
                        dtTemp.Rows[0][2],
                        estatus
                    );

                    Resultado.Rows.Add(
                        "2026",
                        "fecha inicio",
                        "fecha fin",
                        "",
                        dtTemp.Rows[0][2],
                        dtTemp.Rows[0][1],
                        tipoPersonal,
                        areaAdscripcion,
                        estatus,
                        "",
                        "",
                        "DIRECCION GENERAL DE RECURSOS HUMANOS",
                        "fecha Actualización",
                        ""
                        );
                }
            }

            string consultaLocal = "";
            if (Ejercicio > 0) {
                consultaLocal += $" DECLARE @Anio			INT = {Ejercicio}; ";
            }
            if (Trimestre > 0) {
                consultaLocal += $" DECLARE @Anio			INT = {Trimestre}; ";
            }


            archivoPlazasVacantes.Columns.Add("Puesto");
            archivoPlazasVacantes.Columns.Add("Tipo");
            archivoPlazasVacantes.Columns.Add("AreaAdscripcion");
            archivoPlazasVacantes.Columns.Add("PuestoDescripcion");
            archivoPlazasVacantes.Columns.Add("estatus");


            Resultado.Columns.Add("Ejercicio");
            Resultado.Columns.Add("FechaInicio");
            Resultado.Columns.Add("FechaFin");
            Resultado.Columns.Add("DenominacionArea");
            Resultado.Columns.Add("DenominacionPuesto");
            Resultado.Columns.Add("ClavePuesto");
            Resultado.Columns.Add("TipoPlaza");
            Resultado.Columns.Add("AreaAdscripcion");
            Resultado.Columns.Add("Estatus");
            Resultado.Columns.Add("Sexo");
            Resultado.Columns.Add("hipervinculo");
            Resultado.Columns.Add("AreaResponsable");
            Resultado.Columns.Add("FechaActualizacion");
            Resultado.Columns.Add("nota");

            consultaLocal = consultaLocal + ConsultasModel.ConsultaBuscarPuestos;

            return View( );
        }
        public ActionResult TransparenciaVII() {
            string resultado = ConsultaTransparencia();
            return View();
        }
        [HttpPost]
        public ActionResult TransparenciaVII(int Ejercicio, int Trimestre) {
            string resultado = ConsultaTransparencia();
            return View();
        }
        public ActionResult TransparenciaXL() {
            string resultado = ConsultaTransparencia();
            return View();
        }
        [HttpPost]
        public ActionResult TransparenciaXL(int Ejercicio, int Trimestre) {
            string resultado = ConsultaTransparencia();
            return View();
        }

        private string ConsultaTransparencia(){
            return "";
        }

        private DataTable LeerPlazasVacantes(string[] PlazasVacantes) {
            DataTable Resultado = new DataTable();
            DataTable archivoPlazasVacantes = new DataTable();

            Global global = new Global(_coneccionService);

            Resultado.Columns.Add("Ejercicio");
            Resultado.Columns.Add("FechaInicio");
            Resultado.Columns.Add("FechaFin");
            Resultado.Columns.Add("DenominacionArea");
            Resultado.Columns.Add("DenominacionPuesto");
            Resultado.Columns.Add("ClavePuesto");
            Resultado.Columns.Add("TipoPlaza");
            Resultado.Columns.Add("AreaAdscripcion");
            Resultado.Columns.Add("Estatus");
            Resultado.Columns.Add("Sexo");
            Resultado.Columns.Add("hipervinculo");
            Resultado.Columns.Add("AreaResponsable");
            Resultado.Columns.Add("FechaActualizacion");
            Resultado.Columns.Add("nota");

            archivoPlazasVacantes.Columns.Add("Puesto");
            archivoPlazasVacantes.Columns.Add("Tipo");
            archivoPlazasVacantes.Columns.Add("AreaAdscripcion");
            archivoPlazasVacantes.Columns.Add("PuestoDescripcion");
            archivoPlazasVacantes.Columns.Add("estatus");

            foreach (string plaza in PlazasVacantes) {
                string[] row = plaza.Split('|');

                row = row.Select(x => x.Trim()).ToArray();

                string puesto = row[9].Trim().Replace(" ", "");
                string areaAdscripcion = row[17];
                string estatus = "Vacante";
                string tipoPersonal;

                DataTable dtTemp = global.ConsultaGeneral($"DECLARE  @numero  VARCHAR(10) = '{puesto}'; " + ConsultasModel.ConsultaPuesto, "IESYS_SYSNGFSON");

                if (puesto.StartsWith("M")) {
                    tipoPersonal = "Base";
                }
                else if (puesto.StartsWith("CF")) {
                    tipoPersonal = "Confianza";
                }
                else {
                    tipoPersonal = "";
                }

                archivoPlazasVacantes.Rows.Add(puesto, tipoPersonal,dtTemp.Rows[0][2],estatus);

                Resultado.Rows.Add(
                    "2026",
                    "fecha inicio",
                    "fecha fin",
                    "",
                    dtTemp.Rows[0][2],
                    dtTemp.Rows[0][1],
                    tipoPersonal,
                    areaAdscripcion,
                    estatus,
                    "",
                    "",
                    "DIRECCION GENERAL DE RECURSOS HUMANOS",
                    "fecha Actualización",
                    ""
                    );
            }

            return Resultado;
        }
    }
}
