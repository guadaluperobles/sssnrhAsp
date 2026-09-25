using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using System.Data;
using System.Security.Principal;

namespace RecursosHumanos.Controllers {
    [Authorize]
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
            DataTable plazasVacantesDT = new DataTable();

            if (archivo != null && archivo.Length > 0) {
                string[] plazasVacantes = await ArchivoController.LeerTxt(archivo);
                Resultado = LeerPlazasVacantes(plazasVacantes, Ejercicio, Trimestre);
            }

            ViewBag.Archivo = archivo;
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
                Resultado = LeerPlazasVacantes(plazasVacantes, Ejercicio, Trimestre);
            }

            var tablas = new Dictionary<string, DataTable>{{ "Hoja 1", ResultadosTransparenciaIX(Ejercicio, Trimestre, Resultado) }  };
            ViewBag.Archivo = archivo;
            return ArchivoController.ExportarExcel(tablas, $"LGT65_IX_{Ejercicio}_{Trimestre}");
        }
        private DataTable LeerPlazasVacantes(string[] PlazasVacantes, int Ejercicio, int Trimestre) {
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


            Periodo per = new Periodo();
            List<Periodo> periodos = per.trimestres();
            Periodo? periodoActual = periodos.FirstOrDefault(x => x.Id == Trimestre);

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
                    "01/" + periodoActual.Inicio + "/" + Ejercicio.ToString(),
                    DateTime.DaysInMonth(Ejercicio, Convert.ToInt32(periodoActual.Fin)) + "/" + periodoActual.Fin + "/" + Ejercicio.ToString(),
                    "",
                    dtTemp.Rows[0][2],
                    dtTemp.Rows[0][1],
                    tipoPersonal,
                    areaAdscripcion,
                    estatus,
                    "",
                    "",
                    "DIRECCION GENERAL DE RECURSOS HUMANOS",
                    DateTime.DaysInMonth(Ejercicio, Convert.ToInt32(periodoActual.Fin)) + "/" + periodoActual.Fin + "/" + Ejercicio.ToString(),
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

            consultaLocal = consultaLocal + ConsultasModel.ConsultaBuscarPuestos;

            DataTable resultadoConsulta = global.ConsultaGeneral(consultaLocal, "", false);

            string[] columnas ={
                                    "Ejercicio",
                                    "FechaInicio",
                                    "FechaFin",
                                    "DenominacionArea",
                                    "DenominacionPuesto",
                                    "ClavePuesto",
                                    "TipoPlaza",
                                    "AreaAdscripcion",
                                    "Estatus",
                                    "Sexo",
                                    "hipervinculo",
                                    "AreaResponsable",
                                    "FechaActualizacion",
                                    "nota"
                                };

            foreach (string columna in columnas) {
                if (!Resultado.Columns.Contains(columna)) {
                    Resultado.Columns.Add(columna);
                }
            }

            foreach (DataRow r in resultadoConsulta.Rows) {
                Resultado.Rows.Add(
                    "2026",
                    "01/" + periodoActual.Inicio + "/" + Ejercicio.ToString(),
                    DateTime.DaysInMonth(Ejercicio, Convert.ToInt32(periodoActual.Fin)) + "/" + periodoActual.Fin + "/" + Ejercicio.ToString(),
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

            Resultado = Global.Filtrar(Resultado,  r => r.Field<string>("tipoPlaza") == "Confianza" || r.Field<string>("tipoPlaza") == "Base");

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
        #region Reporte LGT65 VII
        public ActionResult TransparenciaVII() {
            int ejercicio = DateTime.Now.Year;
            int trimestre = ((DateTime.Now.Month - 1) / 3) + 1;
            Dictionary<string, DataTable> Resultado = new Dictionary<string, DataTable>();

            ResultadosTransparenciaVII(ejercicio, trimestre, Resultado);
            return View();
        }
        [HttpPost]
        public ActionResult TransparenciaVII(int Ejercicio, int Trimestre) {
            Dictionary<string, DataTable> Resultado = new Dictionary<string, DataTable>();
            ResultadosTransparenciaVII(Ejercicio, Trimestre, Resultado);
            return View("TransparenciaVII");
        }
        [HttpPost]
        public ActionResult TransparenciaVIIExcel(int Ejercicio, int Trimestre) {
            Dictionary<string, DataTable> Resultado = new Dictionary<string, DataTable>();

            ArchivoController.ExportarExcel(ResultadosTransparenciaVII(Ejercicio, Trimestre, Resultado), $"LGT65_VII_{Ejercicio}_{Trimestre}");

            return ArchivoController.ExportarExcel(ResultadosTransparenciaVII(Ejercicio, Trimestre, Resultado), $"LGT65_VII_{Ejercicio}_{Trimestre}");
        }
        private Dictionary<string, DataTable> ResultadosTransparenciaVII(int Ejercicio, int Trimestre, Dictionary<string, DataTable> Resultado = null) {
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

            resultadoPrincipal = RecorrerConsultaIIV(resultadoConsulta, Trimestre, Ejercicio);

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
                else {
                    tTabla143.Rows.Add(
                                i,
                                "n/a",
                                0.00,
                                0.00,
                                "MXN",
                                "Mensual"
                            );
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

            for (int k = 1; k < tTabla143.Rows.Count; k++) {
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

            var tablas = new Dictionary<string, DataTable> {
                { "Reporte de Formatos", rPrincipal } ,
                { "Tabla_140", tTabla140 } ,
                { "Tabla_141", tTabla141 } ,
                { "Tabla_142", tTabla142 } ,
                { "Tabla_143", tTabla143 } ,
                { "Tabla_144", tTabla144 } ,
                { "Tabla_145", tTabla145 } ,
                { "Tabla_146", tTabla146 } ,
                { "Tabla_147", tTabla147 } ,
                { "Tabla_148", tTabla148 } ,
                { "Tabla_149", tTabla149 } ,
                { "Tabla_155", tTabla155 } ,
                { "Tabla_150", tTabla150 } ,
                { "Tabla_151", tTabla151 }
            };
            return tablas;
        }

        private List<TransparenciaVIIModel> RecorrerConsultaIIV(DataTable dt, int t, int a) {
            List<TransparenciaVIIModel> DT = new List<TransparenciaVIIModel>();

            foreach (DataRow dr in dt.Rows) {
                //801634
                TransparenciaVIIModel model = new TransparenciaVIIModel();

                model.ejercicio = Convert.ToString(dr[0]);
                model.fechaInicioPeriodo = (string)dr[1];
                model.fechaFinPeriodo = (string)dr[2];
                model.tipoSujetoObligado = (string)dr[3];
                model.claveNivelPuesto = (string)dr[4];
                model.descripcionPuesto = (string)dr[5];
                model.descripcionCargo = (string)dr[6];
                model.areaAdscripcion = (string)dr[7];
                model.nombre = (string)dr[8];
                model.primerApellido = (string)dr[9];
                model.segundoApellido = (string)dr[10];
                model.sexo = (string)dr[11];
                model.montoRemuredacionMensualBruta = Convert.ToString(dr[12]);
                model.tipoMonedaBruta = (string)dr[13];
                model.montoRemuredacionMensualNeta = Convert.ToString(dr[14]);
                model.tipoMonedaNeta = (string)dr[15];
                model.tabla143 = BuscarComplemento((string)dr[32], t, a);
                model.AreaResponsable = (string)dr[29];
                model.fechaActualizacion = (string)dr[2];
                model.nota = (string)dr[20];

                DT.Add(model);
            }
            return DT;
        }

        private List<Tabla143Model> BuscarComplemento(string rfc, int t, int a) {
            Global global = new Global(_coneccionService);
            List<Tabla143Model> dt = new List<Tabla143Model>();

            string ComplementarConsulta = $"DECLARE @rfc VARCHAR(15) = '{rfc}' DECLARE @anio int= {a};  DECLARE @Trimestre int= {t}";

            DataTable dt2 = global.ConsultaGeneral(ComplementarConsulta + ConsultasModel.ConsultaBuscarComplementos, "IESYS_SYSNGFCRSP");
            if (dt2.Rows.Count > 0) {
                dt.Add(new Tabla143Model());

                dt[0].Id = "1";
                dt[0].Denominacion = "COMPENSACIÓN GARANTIZADA";
                dt[0].MontoBruto = (string)dt2.Rows[0][4].ToString();
                dt[0].MontoNeto = (string)dt2.Rows[0][5].ToString();
                dt[0].TipoMoneda = "MXN";
                dt[0].Periodicidad = "Mensual";


                Console.WriteLine(rfc);
            }
            else {
                Console.WriteLine($" nel {rfc}");
            }

            return dt;
        }
        #endregion
        #region Reporte LGT65 XL
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
            var tablas = new Dictionary<string, DataTable> { { $"LGT65_XL_{Ejercicio}_{Trimestre}", ResultadosTransparenciaXL(Ejercicio, Trimestre, Resultado) } };
            return ArchivoController.ExportarExcel(tablas, $"LGT65_XL_{Ejercicio}_{Trimestre}");
        }
        private DataTable ResultadosTransparenciaXL(int Ejercicio, int Trimestre, DataTable Resultado = null) {

            Global global = new Global(_coneccionService);
            DataTable resultadoConsulta = new DataTable();
            List<TransparenciaXLModel> lstXL = new List<TransparenciaXLModel>();
            string consultaLocal = ConsultasModel.ConsultaTransparencia;

            //AND( Historico_Movimiento.HmFchIni > 20260331)
            switch (Trimestre) {
                case 1:
                    consultaLocal += $"AND Historico_Movimiento.HmFchIni BETWEEN {Ejercicio}0101 AND {Ejercicio}0331";
                    break;
                case 2:
                    consultaLocal += $"AND Historico_Movimiento.HmFchIni BETWEEN {Ejercicio}0401 AND {Ejercicio}0631";
                    break;
                case 3:
                    consultaLocal += $"AND Historico_Movimiento.HmFchIni BETWEEN {Ejercicio}0701 AND {Ejercicio}1031";
                    break;
                case 4:
                    consultaLocal += $"AND Historico_Movimiento.HmFchIni BETWEEN {Ejercicio}1101 AND {Ejercicio}1231";
                    break;
                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }

            resultadoConsulta = global.ConsultaGeneral(consultaLocal);
            lstXL = RecorrerConsultaXL(resultadoConsulta, Trimestre, Ejercicio);

            ViewBag.Resultado = Global.ToDataTable(lstXL);

            ViewBag.Ejercicio = Ejercicio;
            ViewBag.Trimestre = Trimestre;


            return Global.ToDataTable(lstXL);
        }
        private List<TransparenciaXLModel> RecorrerConsultaXL(DataTable dt, int trimestre, int anio) {
            List<TransparenciaXLModel> XL = new List<TransparenciaXLModel>();
            string fecha = DateTime.Today.ToString("dd/MM/yyyy");
            Periodo per = new Periodo();

            List<Periodo> periodos = per.trimestres();
            Periodo? periodoActual = periodos.FirstOrDefault(x => x.Id == trimestre);


            HashSet<string> RFCs = new HashSet<string>();

            foreach (DataRow Row in dt.Rows) {

                TransparenciaXLModel model = new TransparenciaXLModel();
                string productDetalle = Row[0].ToString();

                if (!RFCs.Add(Row[2].ToString()))
                    continue;

                model.ejercicio = anio.ToString();
                model.fechaInicioPeriodo = "01/" + periodoActual.Inicio + "/" + anio.ToString();
                model.fechaFinPeriodo = DateTime.DaysInMonth(anio, Convert.ToInt32(periodoActual.Fin)) + "/" + periodoActual.Fin + "/" + anio.ToString();
                model.estatus = "";
                model.tipoJuvilacionPension = Row[9].ToString();
                model.nombre = Row[8].ToString();
                model.primerApellido = Row[6].ToString();
                model.segundoApellido = Row[7].ToString();
                model.sexo = Row[1].ToString() == "M" ? "Mujer" : "Hombre";
                model.montoPorcion = "";
                model.periodicidadMonto = "";
                model.AreaResponsable = "DIRECCION GENERAL DE RECURSOS HUMANOS";
                model.fechaActualizacion = fecha;
                model.nota = Row[10].ToString();

                XL.Add(model);
            }
            return XL;
        }
        #endregion
    }
}
