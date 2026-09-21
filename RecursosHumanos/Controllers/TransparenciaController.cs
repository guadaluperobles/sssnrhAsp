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
        #region Reporte LGT65 IX
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
            var tablas = new Dictionary<string, DataTable>{{ "Hoja 1", ResultadosTransparenciaIX(Ejercicio, Trimestre, Resultado) }  };
            ArchivoController.ExportarExcel(tablas, $"LGT65_IX_{Ejercicio}_{Trimestre}");
            return View("TransparenciaIX");
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
        private DataTable ResultadosTransparenciaIX(int Ejercicio, int Trimestre, DataTable Resultado = null, DataTable archivoPlazasVacantes = null) {
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
                return Resultado;
            }
            if (Trimestre > 0) {
                consultaLocal += $" DECLARE @Trimestre	    INT = {Trimestre}; ";
            }
            else {
                return Resultado;
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

            return Resultado;
        }
        #endregion
        #region LGT65 VII
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

            var tablas = new Dictionary<string, DataTable> { { "Hoja 1", ResultadosTransparenciaVII(Ejercicio, Trimestre, Resultado) } };
            ArchivoController.ExportarExcel(tablas, $"LGT65_VII_{Ejercicio}_{Trimestre}");

            return View("TransparenciaVII");
        }
        private DataTable ResultadosTransparenciaVII(int Ejercicio, int Trimestre, DataTable Resultado = null) {
            string consultaLocal = "";
            Global global = new Global(_coneccionService);

            if (Ejercicio > 0) {
                consultaLocal += $" DECLARE @Anio       INT = {Ejercicio}; ";
            }
            else {
                return Resultado;
            }
            if (Trimestre > 0) {
                consultaLocal += $" DECLARE @Trimestre  INT = {Trimestre}; ";
            }
            else {
                return Resultado;
            }

            List<TransparenciaVIIModel> resultadoPrincipal = new List<TransparenciaVIIModel>();
            consultaLocal = consultaLocal + ConsultasModel.ConsultaTotalPercepcionesEmpleados;
            DataTable resultadoConsulta = global.ConsultaGeneral(consultaLocal);

            DataTable rPrincipal = new DataTable();
            DataTable tTabla140 = new DataTable();
            DataTable tTabla141 = new DataTable();
            DataTable tTabla142 = new DataTable();
            DataTable tTabla143 = new DataTable();
            DataTable tTabla144 = new DataTable();
            DataTable tTabla145 = new DataTable();
            DataTable tTabla146 = new DataTable();
            DataTable tTabla147 = new DataTable();
            DataTable tTabla148 = new DataTable();
            DataTable tTabla149 = new DataTable();
            DataTable tTabla150 = new DataTable();
            DataTable tTabla151 = new DataTable();
            DataTable tTabla155 = new DataTable();

            rPrincipal.Columns.Add("Ejercicio");
            rPrincipal.Columns.Add("FechaInicio");
            rPrincipal.Columns.Add("FechaFin");
            rPrincipal.Columns.Add("TipoSujetoObligado");
            rPrincipal.Columns.Add("ClavePuesto");
            rPrincipal.Columns.Add("DenominacionPuesto");
            rPrincipal.Columns.Add("DenominacionCargo");
            rPrincipal.Columns.Add("AreaAdcripcion");
            rPrincipal.Columns.Add("Nombre");
            rPrincipal.Columns.Add("PrimerApellido");
            rPrincipal.Columns.Add("SegundoApellido");
            rPrincipal.Columns.Add("Sexo");
            rPrincipal.Columns.Add("MontoBruto");
            rPrincipal.Columns.Add("MontoBrutoMoneda");
            rPrincipal.Columns.Add("MontoNeto");
            rPrincipal.Columns.Add("MontoNetoMoneda");
            rPrincipal.Columns.Add("Tabla140");
            rPrincipal.Columns.Add("Tabla141");
            rPrincipal.Columns.Add("Tabla142");
            rPrincipal.Columns.Add("Tabla143");
            rPrincipal.Columns.Add("Tabla144");
            rPrincipal.Columns.Add("Tabla145");
            rPrincipal.Columns.Add("Tabla146");
            rPrincipal.Columns.Add("Tabla147");
            rPrincipal.Columns.Add("Tabla148");
            rPrincipal.Columns.Add("Tabla149");
            rPrincipal.Columns.Add("Tabla155");
            rPrincipal.Columns.Add("Tabla150");
            rPrincipal.Columns.Add("Tabla151");
            rPrincipal.Columns.Add("AreaResponsable");
            rPrincipal.Columns.Add("FechaActualizacion");
            rPrincipal.Columns.Add("Nota");

            tTabla140.Columns.Add("ID");
            tTabla140.Columns.Add("Denominacion");
            tTabla140.Columns.Add("MontoBruto");
            tTabla140.Columns.Add("MontoNeto");
            tTabla140.Columns.Add("TipoMoneda");
            tTabla140.Columns.Add("Periodicidad");

            tTabla141.Columns.Add("ID");
            tTabla141.Columns.Add("Descripcion");
            tTabla141.Columns.Add("Periodicidad");

            tTabla142.Columns.Add("ID");
            tTabla142.Columns.Add("Denominacion");
            tTabla142.Columns.Add("MontoBruto");
            tTabla142.Columns.Add("MontoNeto");
            tTabla142.Columns.Add("TipoMoneda");
            tTabla142.Columns.Add("Periodicidad");

            tTabla143.Columns.Add("ID");
            tTabla143.Columns.Add("Denominacion");
            tTabla143.Columns.Add("MontoBruto");
            tTabla143.Columns.Add("MontoNeto");
            tTabla143.Columns.Add("TipoMoneda");
            tTabla143.Columns.Add("Periodicidad");

            tTabla144.Columns.Add("ID");
            tTabla144.Columns.Add("Denominacion");
            tTabla144.Columns.Add("MontoBruto");
            tTabla144.Columns.Add("MontoNeto");
            tTabla144.Columns.Add("TipoMoneda");
            tTabla144.Columns.Add("Periodicidad");

            tTabla145.Columns.Add("ID");
            tTabla145.Columns.Add("Denominacion");
            tTabla145.Columns.Add("MontoBruto");
            tTabla145.Columns.Add("MontoNeto");
            tTabla145.Columns.Add("TipoMoneda");
            tTabla145.Columns.Add("Periodicidad");

            tTabla146.Columns.Add("ID");
            tTabla146.Columns.Add("Denominacion");
            tTabla146.Columns.Add("MontoBruto");
            tTabla146.Columns.Add("MontoNeto");
            tTabla146.Columns.Add("TipoMoneda");
            tTabla146.Columns.Add("Periodicidad");

            tTabla147.Columns.Add("ID");
            tTabla147.Columns.Add("Denominacion");
            tTabla147.Columns.Add("MontoBruto");
            tTabla147.Columns.Add("MontoNeto");
            tTabla147.Columns.Add("TipoMoneda");
            tTabla147.Columns.Add("Periodicidad");

            tTabla148.Columns.Add("ID");
            tTabla148.Columns.Add("Denominacion");
            tTabla148.Columns.Add("MontoBruto");
            tTabla148.Columns.Add("MontoNeto");
            tTabla148.Columns.Add("TipoMoneda");
            tTabla148.Columns.Add("Periodicidad");

            tTabla149.Columns.Add("ID");
            tTabla149.Columns.Add("Denominacion");
            tTabla149.Columns.Add("MontoBruto");
            tTabla149.Columns.Add("MontoNeto");
            tTabla149.Columns.Add("TipoMoneda");
            tTabla149.Columns.Add("Periodicidad");

            tTabla155.Columns.Add("ID");
            tTabla155.Columns.Add("Denominacion");
            tTabla155.Columns.Add("MontoBruto");
            tTabla155.Columns.Add("MontoNeto");
            tTabla155.Columns.Add("TipoMoneda");
            tTabla155.Columns.Add("Periodicidad");

            tTabla150.Columns.Add("ID");
            tTabla150.Columns.Add("Denominacion");
            tTabla150.Columns.Add("MontoBruto");
            tTabla150.Columns.Add("MontoNeto");
            tTabla150.Columns.Add("TipoMoneda");
            tTabla150.Columns.Add("Periodicidad");

            tTabla151.Columns.Add("ID");
            tTabla151.Columns.Add("Denominacion");
            tTabla151.Columns.Add("Periodicidad");

            var i = 0;
            var iT143 = 0;

            foreach (TransparenciaVIIModel dr in resultadoPrincipal) {
                i++;

                if (dr.tabla143.Count > 0) {
                    foreach (Tabla143Model item in dr.tabla143) {
                        tTabla143.Rows.Add(
                            i,
                            item.Denominacion,
                            item.MontoBruto,
                            item.MontoNeto,
                            item.TipoMoneda,
                            item.Periodicidad
                        );
                    }
                }

                rPrincipal.Rows.Add(
                    dr.ejercicio,
                    dr.fechaInicioPeriodo,
                    dr.fechaFinPeriodo,
                    dr.tipoSujetoObligado,
                    dr.claveNivelPuesto,
                    dr.descripcionPuesto,
                    dr.descripcionCargo,
                    dr.areaAdscripcion,
                    dr.nombre,
                    dr.primerApellido,
                    dr.segundoApellido,
                    dr.sexo,
                    dr.montoRemuredacionMensualBruta,
                    dr.tipoMonedaBruta,
                    dr.montoRemuredacionMensualNeta,
                    dr.tipoMonedaNeta,
                    i,
                    i,
                    i,
                    //dr.tabla143.Count > 0 ? iT143.ToString() : "",
                    i,
                    i,
                    i,
                    i,
                    i,
                    i,
                    i,
                    i,
                    i,
                    i,
                    dr.AreaResponsable,
                    dr.fechaActualizacion,
                    dr.nota
                    );
            }

            for (int k = 0; k < tTabla143.Rows.Count; k++) {
                tTabla140.Rows.Add(k, "n/a", "0.00", "0.00", "MXN", "MENSUAL");
                tTabla141.Rows.Add(k, "n/a", "n/a");
                tTabla142.Rows.Add(k, "n/a", "0.00", "0.00", "MXN", "MENSUAL");
                tTabla144.Rows.Add(k, "n/a", "0.00", "0.00", "MXN", "MENSUAL");
                tTabla145.Rows.Add(k, "n/a", "0.00", "0.00", "MXN", "MENSUAL");
                tTabla146.Rows.Add(k, "n/a", "0.00", "0.00", "MXN", "MENSUAL");
                tTabla147.Rows.Add(k, "n/a", "0.00", "0.00", "MXN", "MENSUAL");
                tTabla148.Rows.Add(k, "n/a", "0.00", "0.00", "MXN", "MENSUAL");
                tTabla149.Rows.Add(k, "n/a", "0.00", "0.00", "MXN", "MENSUAL");
                tTabla155.Rows.Add(k, "n/a", "0.00", "0.00", "MXN", "MENSUAL");
                tTabla150.Rows.Add(k, "n/a", "0.00", "0.00", "MXN", "MENSUAL");
                tTabla151.Rows.Add(k, "n/a", "No aplica");
            }

            ViewBag.Resultado = rPrincipal;
            ViewBag.tTabla140 = tTabla140;
            ViewBag.tTabla141 = tTabla141;
            ViewBag.tTabla142 = tTabla142;
            ViewBag.tTabla143 = tTabla143;
            ViewBag.tTabla144 = tTabla144;
            ViewBag.tTabla145 = tTabla145;
            ViewBag.tTabla146 = tTabla146;
            ViewBag.tTabla147 = tTabla147;
            ViewBag.tTabla148 = tTabla148;
            ViewBag.tTabla149 = tTabla149;
            ViewBag.tTabla150 = tTabla150;
            ViewBag.tTabla151 = tTabla151;
            ViewBag.tTabla155 = tTabla155;

            ViewBag.Ejercicio = Ejercicio;
            ViewBag.Trimestre = Trimestre;

            return rPrincipal;
        }
        #endregion
        #region LGT65 XL
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
        [HttpPost]
        public ActionResult TransparenciaXLExcel(int Ejercicio, int Trimestre) {
            DataTable Resultado = new DataTable();

            var tablas = new Dictionary<string, DataTable> { { "Hoja 1", ResultadosTransparenciaVII(Ejercicio, Trimestre, Resultado) } };
            ArchivoController.ExportarExcel(tablas, $"LGT65_VII_{Ejercicio}_{Trimestre}");

            return View();
        }
        private DataTable ResultadosTransparenciaXL(int Ejercicio, int Trimestre, DataTable Resultado = null) {

            return Resultado;
        }
        #endregion
    }
}
