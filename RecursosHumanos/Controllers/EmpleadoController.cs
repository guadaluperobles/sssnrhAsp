using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using System.Data;
using System.Text.RegularExpressions;

namespace RecursosHumanos.Controllers {
    public class EmpleadoController : Controller {
        private readonly ConeccionService _coneccionService;
        public EmpleadoController(ConeccionService coneccionService, IConfiguration configuration) {
            _coneccionService = coneccionService;
        }
        // GET: EmpleadoController
        public ActionResult Index() {
            return View();
        }

        // GET: EmpleadoController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: EmpleadoController/Create
        public ActionResult Create() {
            return View();
        }
        public ActionResult LayoutSERICA() {
            ViewBag.Mensaje = "Consulta de empleados";
            ViewBag.Ejercicio = 2026;
            ViewBag.Quincena = 17;
            ViewBag.Activo = true;

            string buscar = " AND (SUBSTRING(e.MeClvPag, 5, 3) = '610') ";
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
                string MeNoIssste = string.IsNullOrWhiteSpace(row["MeNoIssste"]?.ToString()) ?  "0000000"  : row["MeNoIssste"].ToString();

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

        // GET: EmpleadoController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: EmpleadoController/Edit/5
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

        // GET: EmpleadoController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: EmpleadoController/Delete/5
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
