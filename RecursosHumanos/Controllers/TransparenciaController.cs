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

            int ejercicio = DateTime.Now.Year;
            int trimestre = ((DateTime.Now.Month - 1) / 3) + 1;
            DataTable Resultado = new DataTable();

            ResultadosTransparenciaIX(ejercicio, trimestre, Resultado);
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

            ResultadosTransparenciaIX(Ejercicio, Trimestre, Resultado);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> TransparenciaIXExcel(int Ejercicio, int Trimestre, IFormFile archivo) {

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

            ResultadosTransparenciaIX(Ejercicio, Trimestre, Resultado);
            return View();
        }
        public ActionResult TransparenciaVII() {
            int ejercicio = DateTime.Now.Year;
            int trimestre = ((DateTime.Now.Month - 1) / 3) + 1;
            DataTable Resultado = new DataTable();

            ResultadosTransparenciaVII(ejercicio, trimestre, Resultado);
            return View();
        }
        [HttpPost]
        public ActionResult TransparenciaVII(int Ejercicio, int Trimestre) {
            DataTable Resultado = new DataTable();
            ResultadosTransparenciaVII(Ejercicio, Trimestre, Resultado);
            return View();
        }
        [HttpPost]
        public ActionResult TransparenciaVIIExcel(int Ejercicio, int Trimestre) {
            DataTable Resultado = new DataTable();
            ResultadosTransparenciaVII(Ejercicio, Trimestre, Resultado);
            return View();
        }
        public ActionResult TransparenciaXL() {
            int ejercicio = DateTime.Now.Year;
            int trimestre = ((DateTime.Now.Month - 1) / 3) + 1;
            DataTable Resultado = new DataTable();
            ResultadosTransparenciaXL(ejercicio, trimestre, Resultado);
            return View();
        }
        [HttpPost]
        public ActionResult TransparenciaXL(int Ejercicio, int Trimestre) {
            DataTable Resultado = new DataTable();
            ResultadosTransparenciaXL(Ejercicio, Trimestre, Resultado);
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

        private void ResultadosTransparenciaIX(int Ejercicio, int Trimestre, DataTable Resultado = null, DataTable archivoPlazasVacantes = null) {
            string consultaLocal = "";

            archivoPlazasVacantes ??= new DataTable();
            Global global = new Global(_coneccionService);
            Periodo per = new Periodo();

            List<Periodo> periodos = per.trimestres();
            Periodo? periodoActual = periodos.FirstOrDefault(x => x.Id == Trimestre);

            if (Ejercicio > 0) {
                consultaLocal += $" DECLARE @Anio			INT = {Ejercicio}; ";
            }
            else {
                return;
            }
            if (Trimestre > 0) {
                consultaLocal += $" DECLARE @Trimestre	    INT = {Trimestre}; ";
            }
            else {
                return;
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

            DataTable resultadoConsulta = global.ConsultaGeneral(consultaLocal);

            foreach (DataRow r in resultadoConsulta.Rows) {

                Resultado.Rows.Add(
                    "2026",
                    periodoActual.Inicio + "/" + Ejercicio.ToString(),
                    DateTime.DaysInMonth(Ejercicio, Convert.ToInt32(periodoActual.Fin)) + "/" +periodoActual.Fin + "/" + Ejercicio.ToString(),
                    "",
                    r[4],
                    r[3],
                    r[12],
                    r[6],
                    "Ocupado",
                    r[10].ToString() == "H" ? "Hombre" : "Mujer",
                    "",
                    "DIRECCION GENERAL DE RECURSOS HUMANOS",
                    DateTime.DaysInMonth(Ejercicio, Convert.ToInt32(periodoActual.Fin)) + "/" + periodoActual.Fin + "/" + Ejercicio.ToString(),
                    ""
                    );
            }

            DataTable plazasConfianza = Global.Filtrar(Resultado, r => r.Field<string>("tipoPlaza") == "Confianza");
            DataTable plazasOcupadasConfianza = Global.Filtrar(plazasConfianza, r => r.Field<string>("Estatus") == "Ocupado");
            DataTable plazasVacantesConfianza = Global.Filtrar(plazasConfianza, r => r.Field<string>("Estatus") == "Vacante");
            DataTable plazasOcupadasConfianzaHombres = Global.Filtrar(plazasOcupadasConfianza, r => r.Field<string>("Sexo") == "Hombre");
            DataTable plazasOcupadasConfianzaMujeres = Global.Filtrar(plazasOcupadasConfianza, r => r.Field<string>("Sexo") == "Mujer");

            DataTable plazasBase = Global.Filtrar(Resultado, r => r.Field<string>("tipoPlaza") == "Base");
            DataTable plazasOcupadasBase = Global.Filtrar(plazasBase, r => r.Field<string>("Estatus") == "Ocupado");
            DataTable plazasVacantesBase = Global.Filtrar(plazasBase, r => r.Field<string>("Estatus") == "Vacante");
            DataTable plazasOcupadasBaseHombres = Global.Filtrar(plazasOcupadasBase, r => r.Field<string>("Sexo") == "Hombre");
            DataTable plazasOcupadasBaseMujeres = Global.Filtrar(plazasOcupadasBase, r => r.Field<string>("Sexo") == "Mujer");

            ViewBag.Resultado = Resultado;

            ViewBag.plazasConfianza = plazasConfianza;
            ViewBag.plazasVacantesConfianza = plazasVacantesConfianza;
            ViewBag.plazasOcupadasConfianza = plazasOcupadasConfianza;
            ViewBag.plazasOcupadasConfianzaHombres = plazasOcupadasConfianzaHombres;
            ViewBag.plazasOcupadasConfianzaMujeres = plazasOcupadasConfianzaMujeres;

            ViewBag.plazasBase = plazasBase;
            ViewBag.plazasOcupadasBase = plazasOcupadasBase;
            ViewBag.plazasVacantesBase = plazasVacantesBase;
            ViewBag.plazasOcupadasBaseHombres = plazasOcupadasBaseHombres;
            ViewBag.plazasOcupadasBaseMujeres = plazasOcupadasBaseMujeres;

            ViewBag.Ejercicio = Ejercicio;
            ViewBag.Trimestre = Trimestre;
        }
        private void ResultadosTransparenciaVII(int Ejercicio, int Trimestre, DataTable Resultado = null) {
            string consultaLocal = "";

            DataTable archivoPlazasVacantes = new DataTable();
            Global global = new Global(_coneccionService);

            if (Ejercicio > 0) {
                consultaLocal += $" DECLARE @Anio			INT = {Ejercicio}; ";
            }
            else {
                return;
            }
            if (Trimestre > 0) {
                consultaLocal += $" DECLARE @Trimestre			INT = {Trimestre}; ";
            }
            else {
                return;
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

            DataTable resultadoConsulta = global.ConsultaGeneral(consultaLocal);

            foreach (DataRow r in resultadoConsulta.Rows) {

                Resultado.Rows.Add(
                    "2026",
                    "fecha inicio",
                    "fecha fin",
                    "",
                    r[4],
                    r[3],
                    r[12],
                    r[6],
                    "Ocupado",
                    r[10],
                    "",
                    "DIRECCION GENERAL DE RECURSOS HUMANOS",
                    "fecha Actualización",
                    ""
                    );
            }

            DataTable plazasConfianza = Global.Filtrar(Resultado, r => r.Field<string>("tipoPlaza") == "Confianza");
            DataTable plazasBase = Global.Filtrar(Resultado, r => r.Field<string>("tipoPlaza") == "Base");
            DataTable plazasOcupadasBase = Global.Filtrar(plazasBase, r => r.Field<string>("Estatus") == "Ocupado");
            DataTable plazasOcupadasConfianza = Global.Filtrar(plazasConfianza, r => r.Field<string>("Estatus") == "Ocupado");
            DataTable plazasVacantesBase = Global.Filtrar(plazasBase, r => r.Field<string>("Estatus") == "Vacante");
            DataTable plazasVacantesConfianza = Global.Filtrar(plazasConfianza, r => r.Field<string>("Estatus") == "Vacante");
            DataTable plazasOcupadasConfianzaHombres = Global.Filtrar(plazasOcupadasConfianza, r => r.Field<string>("Sexo") == "H");
            DataTable plazasOcupadasConfianzaMujeres = Global.Filtrar(plazasOcupadasConfianza, r => r.Field<string>("Sexo") == "M");
            DataTable plazasOcupadasBaseHombres = Global.Filtrar(plazasOcupadasBase, r => r.Field<string>("Sexo") == "H");
            DataTable plazasOcupadasBaseMujeres = Global.Filtrar(plazasOcupadasBase, r => r.Field<string>("Sexo") == "M");

            ViewBag.Resultado = Resultado;
            ViewBag.plazasConfianza = plazasConfianza;
            ViewBag.plazasBase = plazasBase;
            ViewBag.plazasOcupadasBase = plazasOcupadasBase;
            ViewBag.plazasOcupadasConfianza = plazasOcupadasConfianza;
            ViewBag.plazasVacantesBase = plazasVacantesBase;
            ViewBag.plazasVacantesConfianza = plazasVacantesConfianza;
            ViewBag.plazasOcupadasConfianzaHombres = plazasOcupadasConfianzaHombres;
            ViewBag.plazasOcupadasConfianzaMujeres = plazasOcupadasConfianzaMujeres;
            ViewBag.plazasOcupadasBaseHombres = plazasOcupadasBaseHombres;
            ViewBag.plazasOcupadasBaseMujeres = plazasOcupadasBaseMujeres;

            ViewBag.Ejercicio = Ejercicio;
            ViewBag.Trimestre = Trimestre;
        }
        private void ResultadosTransparenciaXL(int Ejercicio, int Trimestre, DataTable Resultado = null) {
            string consultaLocal = "";

            DataTable archivoPlazasVacantes = new DataTable();
            Global global = new Global(_coneccionService);

            if (Ejercicio > 0) {
                consultaLocal += $" DECLARE @Anio			INT = {Ejercicio}; ";
            }
            else {
                return;
            }
            if (Trimestre > 0) {
                consultaLocal += $" DECLARE @Trimestre			INT = {Trimestre}; ";
            }
            else {
                return;
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

            DataTable resultadoConsulta = global.ConsultaGeneral(consultaLocal);

            foreach (DataRow r in resultadoConsulta.Rows) {

                Resultado.Rows.Add(
                    "2026",
                    "fecha inicio",
                    "fecha fin",
                    "",
                    r[4],
                    r[3],
                    r[12],
                    r[6],
                    "Ocupado",
                    r[10],
                    "",
                    "DIRECCION GENERAL DE RECURSOS HUMANOS",
                    "fecha Actualización",
                    ""
                    );
            }

            DataTable plazasConfianza = Global.Filtrar(Resultado, r => r.Field<string>("tipoPlaza") == "Confianza");
            DataTable plazasBase = Global.Filtrar(Resultado, r => r.Field<string>("tipoPlaza") == "Base");
            DataTable plazasOcupadasBase = Global.Filtrar(plazasBase, r => r.Field<string>("Estatus") == "Ocupado");
            DataTable plazasOcupadasConfianza = Global.Filtrar(plazasConfianza, r => r.Field<string>("Estatus") == "Ocupado");
            DataTable plazasVacantesBase = Global.Filtrar(plazasBase, r => r.Field<string>("Estatus") == "Vacante");
            DataTable plazasVacantesConfianza = Global.Filtrar(plazasConfianza, r => r.Field<string>("Estatus") == "Vacante");
            DataTable plazasOcupadasConfianzaHombres = Global.Filtrar(plazasOcupadasConfianza, r => r.Field<string>("Sexo") == "H");
            DataTable plazasOcupadasConfianzaMujeres = Global.Filtrar(plazasOcupadasConfianza, r => r.Field<string>("Sexo") == "M");
            DataTable plazasOcupadasBaseHombres = Global.Filtrar(plazasOcupadasBase, r => r.Field<string>("Sexo") == "H");
            DataTable plazasOcupadasBaseMujeres = Global.Filtrar(plazasOcupadasBase, r => r.Field<string>("Sexo") == "M");

            ViewBag.Resultado = Resultado;
            ViewBag.plazasConfianza = plazasConfianza;
            ViewBag.plazasBase = plazasBase;
            ViewBag.plazasOcupadasBase = plazasOcupadasBase;
            ViewBag.plazasOcupadasConfianza = plazasOcupadasConfianza;
            ViewBag.plazasVacantesBase = plazasVacantesBase;
            ViewBag.plazasVacantesConfianza = plazasVacantesConfianza;
            ViewBag.plazasOcupadasConfianzaHombres = plazasOcupadasConfianzaHombres;
            ViewBag.plazasOcupadasConfianzaMujeres = plazasOcupadasConfianzaMujeres;
            ViewBag.plazasOcupadasBaseHombres = plazasOcupadasBaseHombres;
            ViewBag.plazasOcupadasBaseMujeres = plazasOcupadasBaseMujeres;

            ViewBag.Ejercicio = Ejercicio;
            ViewBag.Trimestre = Trimestre;
        }
    }
}
