using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using RecursosHumanos.ViewModel;
using System.Data;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace RecursosHumanos.Controllers {
    [Authorize]
    public class EmpleadoController : Controller {
        private readonly ConeccionService _coneccionService;
        public EmpleadoController(ConeccionService coneccionService, IConfiguration configuration) {
            _coneccionService = coneccionService;
        }
        // GET: EmpleadoController
        public ActionResult Index() {
            return View();
        }
        public ActionResult LayoutSERICA() {
            ViewBag.Mensaje = "Consulta de empleados";
            ViewBag.Ejercicio = 2026;
            ViewBag.Quincena = 17;
            ViewBag.Activo = true;

            string buscar = " AND (SUBSTRING(e.MeClvPag, 5, 3) <> '610') ";
            var ContenidoSERICA = Global.ToDataTable(SERICA(buscar));
            return View(ContenidoSERICA);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LayoutSERICA(string Activo) {
            bool valorActivo = Activo == "on";
            string buscar = valorActivo ? " AND (SUBSTRING(e.MeClvPag, 5, 3) <> '610') ": " AND (SUBSTRING(e.MeClvPag, 5, 3) = '610') " ;
            var ContenidoSERICA = Global.ToDataTable(SERICA(buscar));

            ViewBag.Activo = valorActivo;

            return View(ContenidoSERICA);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LayoutSERICAExcel(int Ejercicio, int Quincena, string Activo) {
            bool valorActivo = Activo == "on";
            string buscar = valorActivo ? " AND (SUBSTRING(e.MeClvPag, 5, 3) <> '610') " : " AND (SUBSTRING(e.MeClvPag, 5, 3) = '610') ";
            string nombreArchivo = $"";
            var ContenidoSERICA = Global.ToDataTable(SERICA(buscar));

            ViewBag.Ejercicio = Ejercicio;
            ViewBag.Quincena = Quincena;
            ViewBag.Activo = Activo;

            var tablas = new Dictionary<string, DataTable>{
                    { "Hoja 1", ContenidoSERICA }
                };
            return ArchivoController.ExportarExcel(tablas, $"MPI26{Quincena}{Ejercicio}");
            
        }
        public ActionResult PlantillaEmpleados() {
            ViewBag.Mensaje = "Plantilla de empleados";
            var Contenido = new DataTable();
            return View(Contenido);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlantillaEmpleados(List<string> CentroTrabajo) {
            ViewBag.Mensaje = "Plantilla de empleados";
            ViewBag.CentroTrabajo = CentroTrabajo;

            var Contenido = Plantilla(CentroTrabajo);
            return View(Contenido);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlantillaEmpleadosExcel(List<string> CentroTrabajo) {
            string nombreArchivo = $"Plantilla Empleados General";
            var Contenido = Plantilla(CentroTrabajo);

            ViewBag.CentroTrabajo = CentroTrabajo;

            var tablas = new Dictionary<string, DataTable>{
                    { "Hoja 1", Contenido }
                };
            return ArchivoController.ExportarExcel(tablas, nombreArchivo);

        }

        private List<LayoutSERICA> SERICA(string buscar) {

            List<LayoutSERICA> empleadosSERICA = new List<LayoutSERICA>();
            Global global = new Global(_coneccionService);
            string sql = @"
                SELECT 
                    eg.MeCurp, 
                    e.MeRfc, 
                    e.MeNomEmp,                     
                    e.MeNomAP, 
                    e.MeNomAM, 
                    eg.MeNoSegS, 
                    eg.MeNoIssste, 
                    hme.HmObs, 
                    pde.MePDVVenc, 
                    pde.MePDVVigI 
                FROM Empleado as e
                INNER JOIN PerDed_Empleado as pde ON e.ClkDet = pde.ClkDet
                INNER JOIN Empleado_Generales as eg ON e.ClkDet = eg.ClkDet
                INNER JOIN Historico_Movimiento as hme ON e.ClkDet = hme.ClkDet
                WHERE (e.MeIndMe = '10' OR  e.MeIndMe = '20') AND (SUBSTRING(e.MeClvPag, 5, 3) = '411') AND pde.MePDClave='03' AND (hme.HmQnaAp > 201312) AND (hme.HmCodMov = 7203) AND (hme.HmObs LIKE '% 03 del issste%')
            ";

            string sqlEmpleados = @$"
                SELECT     
                    e.ClkDet,
                    eg.MeCurp, 
                    e.MeRfc, 
                    e.MeNomEmp,                     
                    e.MeNomAP, 
                    e.MeNomAM, 
                    eg.MeNoSegS, 
                    eg.MeNoIssste, 
                    pde.MePDVVenc, 
                    pde.MePDVVigI 
                FROM Empleado as e
                INNER JOIN PerDed_Empleado as pde ON e.ClkDet = pde.ClkDet
                INNER JOIN Empleado_Generales as eg ON e.ClkDet = eg.ClkDet
                WHERE (e.MeIndMe = '10' OR e.MeIndMe = '20') {buscar} AND pde.MePDClave='03'
            ";

            string sqlProductos = @"
                SELECT TOP(1) * FROM Producto_Control
                WHERE        (ClkPr LIKE 'PRO%')
                ORDER BY PrAno DESC, PrQna DESC
            ";

            DataTable Empleados = global.ConsultaGeneral(sqlEmpleados);
            DataTable Productos = global.ConsultaGeneral(sqlProductos);

            foreach (DataRow row in Empleados.Rows) {
                string ClkDet = row["ClkDet"].ToString();
                string MeCurp = row["MeCurp"].ToString();
                string MeRfc = row["MeRfc"].ToString();
                string MeNomEmp = row["MeNomEmp"].ToString();
                string MeNomAP = row["MeNomAP"].ToString();
                string MeNomAM = row["MeNomAM"].ToString();
                string MeNoSegS = string.IsNullOrWhiteSpace(row["MeNoSegS"]?.ToString())  ?"00000000000" : row["MeNoSegS"].ToString();
                string NoIssste = row["MeNoIssste"]?.ToString();
                string MeNoIssste = string.IsNullOrWhiteSpace(NoIssste) ? "0000000": NoIssste.Length >= 7? NoIssste.Substring(NoIssste.Length - 7): NoIssste.PadLeft(7, '0');

                string MePDVVenc = row["MePDVVenc"].ToString();
                string MePDVVigI = row["MePDVVigI"].ToString();
                string BaseDatos = row["BaseDatos"].ToString();

                string sqlComentario = @$"
                    SELECT   TOP(1)     
                        ClkDet,  HmQnaAp, HmFchIni, HmFchTer, HmTabPt, HmVPuesto, HmPuesto, HmGpoPto, HmNumPto,  
                        HmTmbc, HmDias, HmHrExt, HmHrExtTr, HmObs, HmUsrCp, HmFchCp, HmHraCp, HmUsrCc, HmFchCc, 
                        HmHraCc, HmOrigen, HmIndR, HmDatCom
                    FROM  Historico_Movimiento
                    WHERE ClkDet = {ClkDet} and  (HmQnaAp > 201312) AND (substring(HmCodMov,1,1) = 7) AND ((HmObs LIKE '%PRESTAMO%') and (HmObs LIKE '%ISSSTE%'))
                    ORDER BY HmQnaAp DESC
                ";

                DataTable dtComentario = global.ConsultaGeneral(sqlComentario, BaseDatos);
                DataRow Comentario = null;

                if (dtComentario != null && dtComentario.Rows.Count > 0) {
                    Comentario = dtComentario.Rows[0];
                }
                string texto = "";

                if (Comentario != null) {
                    texto = Comentario["HmObs"]?.ToString() ?? "";
                }

                DataRow producto = Productos.AsEnumerable().FirstOrDefault(row => row["BaseDatos"]?.ToString() == BaseDatos);

                Match match = Regex.Match(texto, @"\b\d{11}\b");
                string numeroPrestamo = match.Success ? match.Value : "00000000000";

                int ejercicioInicio = Convert.ToInt32(MePDVVigI.Substring(0, 4));
                int quincenaInicio = Convert.ToInt32(MePDVVigI.Substring(4, 2));

                int Vigencia = Convert.ToInt32(MePDVVenc.Substring(0, 3));

                int ejercicioFin = Convert.ToInt32(MePDVVigI.Substring(0, 4));
                int quincenaFin = Convert.ToInt32(MePDVVigI.Substring(4, 2));

                string pagaduria = "";
                string claveSerica = "";
                string claveRamo = "";

                switch (BaseDatos) {
                    case "CONTRATOS":
                        pagaduria = "S2620";
                        claveSerica = "00326030";
                        claveRamo = "12926";
                        break;
                    case "CONTRATOS_IB":
                        pagaduria = "S2610";
                        claveSerica = "00426030";
                        claveRamo = "12926";
                        break;
                    case "IESYS_HONOFED":
                    case "FORMALIZADOS":
                    case "IESYS_SYSNGFHOMO":
                    case "IESYS_SYSNGFSON":
                        pagaduria = "14426";
                        claveSerica = "01226030";
                        claveRamo = "12926";
                        break;
                    case "HOMO_IB":
                    case "SYSNGFSON_IB":
                    case "FORMALIZADOS_IB":
                    case "HONOFED_IB":
                        pagaduria = "14426";
                        claveSerica = "41226030";
                        claveRamo = "12926";
                        break;
                    default:
                        pagaduria = "O0000";
                        claveSerica = "00000000";
                        claveRamo = "00000";
                        break;
                }

                int resultado = quincenaInicio + Vigencia;

                while (resultado > 24) {
                    resultado -= 24;
                    ejercicioFin++;
                }

                quincenaFin = resultado;


                empleadosSERICA.Add(new LayoutSERICA {
                    RFC = MeRfc,
                    CURP = MeCurp,
                    NombreEmpleado = MeNomEmp,
                    PrimerApellido = MeNomAP,
                    SegundoApellido = MeNomAM,
                    SeguridadSocial = MeNoSegS,
                    NumeroISSSTE = MeNoIssste,
                    NumeroPrestamo = numeroPrestamo,
                    Pagaduria = pagaduria,

                    Plazo = Vigencia.ToString("D3"),
                    QuincenaInicial = quincenaInicio.ToString("D2"),
                    AnioInicial = ejercicioInicio.ToString("D4"),
                    QuincenaFinal = quincenaFin.ToString("D2"),
                    AnioFinal = ejercicioFin.ToString("D4"),
                    QuincenaEnvio = Convert.ToInt32(producto["PrQna"]).ToString("D2"),
                    AnioEnvio = Convert.ToInt32(producto["PrAno"]).ToString("D4") // Asignar un valor predeterminado o calcularlo según sea necesario
                });
            }

            return empleadosSERICA;
        }
        private DataTable Plantilla(List<string> buscar = null) {
            buscar ??= [];
            List<PlantillaEmpleadoModel> dt = new List<PlantillaEmpleadoModel>();
            Global global = new Global(_coneccionService);
            string valores = string.Join(",",buscar.Select(x => $"'{x.Replace("'", "''")}'") );

            string Buscar = $" AND mectrab IN ({valores}) AND mectrabdist IN ({valores})";
            string sql = ConsultasModel.PlantillaEmpleados + Buscar;

            foreach (DataRow dr in global.ConsultaGeneral(sql).Rows) {
                string Estatus = dr[16]?.ToString() ?? "";
                string Movimiento = dr[21]?.ToString() ?? "";
                string tpuesto = dr[036]?.ToString() ?? "";

                int intDlabi = Convert.ToInt32(dr[030]?.ToString() ?? "0") / 7;
                int intDlabr = Convert.ToInt32(dr[031]?.ToString() ?? "0") / 7;

                DateTime feiinst = Global.ObtenerFecha(dr[18].ToString() ?? "");
                DateTime feiram = Global.ObtenerFecha(dr[19]?.ToString() ?? "");
                DateTime fecnac = Global.ObtenerFecha(dr[20]?.ToString() ?? "");
                DateTime fecham = Global.ObtenerFecha(dr[22]?.ToString() ?? "");

                string SueldoBruto = dr[027]?.ToString() ?? "";
                string SueldoNeto = dr[028]?.ToString() ?? "";

                decimal numeroSueldoBruto = Global.ObtenerDecimal(SueldoBruto);
                decimal numeroSueldoNeto = Global.ObtenerDecimal(SueldoNeto);

                numeroSueldoBruto = numeroSueldoBruto * 2;
                numeroSueldoNeto = numeroSueldoNeto * 2;//ObtenerFecha

                SueldoBruto = numeroSueldoBruto.ToString("F2");
                SueldoNeto = numeroSueldoNeto.ToString("F2");

                string tPto1 = "";
                string tPto2 = "";
                string tPto3 = "";

                string Estatus1 = "";
                string Estatus2 = "";

                switch (int.Parse(tpuesto[0].ToString())) {
                    case 1:
                        tPto1 = "Presupuestal";
                        break;
                    case 2:
                        tPto1 = "Eventual";
                        break;
                    case 3:
                        tPto1 = "Lista de raya";
                        break;
                    case 4:
                        tPto1 = "Medica";
                        break;
                    default:
                        tPto1 = "";
                        break;
                }

                switch (int.Parse(tpuesto[1].ToString())) {
                    case 1:
                        tPto2 = "Base";
                        break;
                    case 2:
                        tPto2 = "Confianza";
                        break;
                    case 3:
                        tPto2 = "Honorarios";
                        break;
                    case 4:
                        tPto2 = "medico residente";
                        break;
                    case 5:
                        tPto2 = "medico interno de pregrado";
                        break;
                    case 6:
                        tPto2 = "Pasante en servicio social ";
                        break;
                    default:
                        tPto2 = "";
                        break;
                }

                switch (int.Parse(tpuesto[2].ToString())) {
                    case 1:
                        tPto3 = "Propiedad";
                        break;
                    case 2:
                        tPto3 = "Interina limitada";
                        break;
                    case 3:
                        tPto3 = "Provisional";
                        break;
                    default:
                        tPto1 = "";
                        break;
                }

                switch (int.Parse(Estatus[0].ToString())) {
                    case 1:
                        Estatus1 = "Registro con proceso posterior";
                        break;
                    case 2:
                        Estatus1 = "Registro sin proceso posterior ";
                        break;
                    default:
                        Estatus1 = "";
                        break;
                }
                switch (int.Parse(Estatus[1].ToString())) {
                    case 0:
                        Estatus2 = "Activo";
                        break;
                    case 1:
                    case 2:
                        Estatus2 = "Licencia";
                        break;
                    case 3:
                        Estatus2 = "Baja temporal";
                        break;
                    case 4:
                        Estatus2 = "Baja definitiva";
                        break;
                    case 5:
                        Estatus2 = "Baja con marca de contraloria";
                        break;
                    default:
                        Estatus2 = "";
                        break;
                }

                dt.Add(new PlantillaEmpleadoModel() {
                    NumeroEmpleado = dr[0]?.ToString() ?? "",
                    RFC = dr[1]?.ToString() ?? "",
                    CURP = dr[2]?.ToString() ?? "",
                    NSS = dr[3]?.ToString() ?? "",
                    ISSTE = dr[4]?.ToString() ?? "",
                    PrimerApellido = dr[5]?.ToString() ?? "",
                    SegundoApellido = dr[6]?.ToString() ?? "",
                    Nombre = dr[7]?.ToString() ?? "",
                    NombreCompleto = dr[8]?.ToString() ?? "",
                    Puesto = dr[9]?.ToString() ?? "",
                    NumeroPuesto = dr[10]?.ToString() ?? "",
                    DescripcionPuesto = dr[11]?.ToString() ?? "",
                    CentroTrabajo = dr[12]?.ToString() ?? "",
                    DescripcionCentroTrabajo = dr[13]?.ToString() ?? "",
                    CentroDistribucion = dr[14]?.ToString() ?? "",
                    DescripcionCentroDistribucion = dr[15]?.ToString() ?? "",
                    Estatus = Estatus2 ,
                    ur = dr[17]?.ToString() ?? "",
                    feiinst = feiinst.ToString("dd/MM/yyyy"),
                    feiram = feiram.ToString("dd/MM/yyyy"),
                    fecnac = fecnac.ToString("dd/MM/yyyy"),
                    Movimiento = Movimiento,
                    fecham = fecham.ToString("dd/MM/yyyy"),
                    cvepag = dr[023]?.ToString() ?? "",
                    BaseDatos = dr[024]?.ToString() ?? "",
                    sexo = dr[025]?.ToString() ?? "",
                    hijos = dr[026]?.ToString() ?? "",
                    SueldoBruto = SueldoBruto,
                    SueldoNeto = SueldoNeto,
                    horario = dr[029]?.ToString() ?? "",
                    dlabi = dr[030]?.ToString() ?? "",
                    dlabr = dr[031]?.ToString() ?? "",
                    cluest = dr[032]?.ToString() ?? "",
                    cluesd = dr[033]?.ToString() ?? "",
                    uadmva = dr[034]?.ToString() ?? "",
                    Antiguedad = ReciboNominaController.obtenerAntiguedad($"P{intDlabi}W"),
                    //ReciboNominaController.obtenerAntiguedad($"P{intDlabi}W");
                    //ReciboNominaController.obtenerAntiguedad($"P{intDlabr}W");
                    tpuesto = tPto1 + ", " + tPto2 + ", " + tPto3,
                    InstrumentoPago = dr[037]?.ToString() ?? "",
                    CuentaBanco = dr[038]?.ToString() ?? "",
                    PuestoSHCP = dr[039]?.ToString() ?? "",
                    NivelAcademico = dr[40]?.ToString() ?? "",
                    turno = dr[41]?.ToString() ?? "",
                    Programa = dr[042]?.ToString() ?? "",
                    MeSATCPost = dr[44]?.ToString() ?? "",
                    SATNombre = dr[45]?.ToString() ?? "",
                    satdice = dr[46]?.ToString() ?? "",
                });
            }

            return Global.ToDataTable(dt);
        }

    }
}
