using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer;
using Humanizer.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using RecursosHumanos.ViewModel;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecursosHumanos.Controllers {
    public class NominaController : Controller {
        // GET: NominaController1
        private readonly ConeccionService _coneccionService;
        private readonly IConfiguration _configuration;
        public NominaController(ConeccionService coneccionService,  IConfiguration configuration) {
            _coneccionService = coneccionService;
            _configuration = configuration;
        }
        public ActionResult Index() {

            var modelView = new NominaViewModel {
                Models = new DataTable()
            }; 

            return View("Index", modelView);
        }

        public ActionResult ActualizarCFDIEmpleado() {

            Global global = new Global(_coneccionService);

            string ConsultaRfc = $"SELECT * FROM RespaldoCFDI";
            DataTable ResCFDIS = new DataTable();
            DataTable BasesDatos = global.BasesDatosOperativas();

            var modelView = new NominaViewModel {
                BaseDatos = "",
                Models = ResCFDIS,
                BasesDatos= BasesDatos
            };
            
            return View("ActualizaCFDI", modelView);
        }

        [HttpPost]
        public ActionResult ActualizarCFDIEmpleado(string BaseDatosConsulta = "") {

            Global global = new Global(_coneccionService);
            DataTable BasesDatos = global.BasesDatosOperativas();
            BaseDatosConsulta = BaseDatosConsulta != "" ? $" WHERE BaseDatos = '{BaseDatosConsulta}'" : "";
            string ConsultaRfc = $"SELECT * FROM RespaldoCFDI {BaseDatosConsulta }";
            DataTable ResCFDIS = global.ConsultaGeneral(ConsultaRfc, "IESYST");
            foreach (var grupo in ResCFDIS.AsEnumerable().GroupBy(x => x["BaseDatos"].ToString())) {
                string BaseDatos = grupo.Key ?? "";
                string connectionString = _configuration.GetConnectionString(BaseDatos);

                using (var conexion = new SqlConnection(connectionString)) {
                    conexion.Open();

                    foreach (DataRow ResCFDI in grupo) {
                        string UUID = Valor(ResCFDI, "PrUUID");
                        string ClkPr = Valor(ResCFDI, "ClkPr");
                        string ClkDet = Valor(ResCFDI, "ClkDet");
                        string PrXML = Valor(ResCFDI, "PrXML");
                        string PrCBB = Valor(ResCFDI, "Qr");
                        string FechaTimbrado = Valor(ResCFDI, "FechaTimbrado");
                        string FechaTimbradoXML = Valor(ResCFDI, "FechaTimbradoXML");
                        string CadenaCFDI = Valor(ResCFDI, "CadenaCfdi");
                        string BaseDatosTabla = Valor(ResCFDI, "BaseDatos");

                        string[] partes = FechaTimbrado.Split(' ');
                        string[] partesTimbrado = FechaTimbrado.Split(' ');

                        string fechaEmision = partes[0].Replace("-", "");
                        string horaEmision = partes.Length > 1 ? partes[1] : "";

                        string fechaTimbrado = partesTimbrado[0].Replace("-", "");
                        string horaTimbrado = partesTimbrado.Length > 1 ? partesTimbrado[1] : "";

                        var recibo = ReciboNominaController.CargarCFDI(PrXML, "12", "ui", "bd", "gh");

                        string sqlActualizaProductoDetalle = @"
                            UPDATE Producto_Detalle
                            SET
                                PrUUID = @UUID,
                                PrXML = @PrXML,
                                PrCBB = @PrCBB,
                                PrFchEmision = @PrFchEmision,
                                PrHraEmision = @PrHraEmision,
                                PrFchTimb = @PrFchTimb,
                                PrHraTimb = @PrHraTimb,
                                PrCadenaOrig = @PrCadenaOrig
                            WHERE ClkPr = @ClkPr AND ClkDet = @ClkDet;
                        ";

                        string sqlActualizaEmpleado = @"
                            UPDATE EMPG
                            SET
                                EMPG.MeSATCPost = @MeSATPost,
                                EMPG.MeSATNomEmp = @MeSATNomEmp
                            FROM Empleado_Generales AS EMPG
                            INNER JOIN Empleado AS EMP
                                ON EMP.ClkDet = EMPG.ClkDet
                            WHERE (EMPG.MeSATCPost IS NULL OR EMPG.MeSATCPost = '00000') AND EMP.ClkDet = @ClkDet AND EMPG.MeSATCPost <> '';
                        ";

                        using (var cmd = new SqlCommand(sqlActualizaProductoDetalle, conexion)) {
                            cmd.Parameters.AddWithValue("@UUID", UUID ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PrXML", PrXML ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PrCBB", PrCBB ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@PrFchEmision", fechaEmision);
                            cmd.Parameters.AddWithValue("@PrHraEmision", horaEmision);
                            cmd.Parameters.AddWithValue("@PrFchTimb", fechaTimbrado);
                            cmd.Parameters.AddWithValue("@PrHraTimb", horaTimbrado);
                            cmd.Parameters.AddWithValue("@PrCadenaOrig", CadenaCFDI ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ClkPr", ClkPr);
                            cmd.Parameters.AddWithValue("@ClkDet", ClkDet);

                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand(sqlActualizaEmpleado, conexion)) {
                            cmd.Parameters.AddWithValue("@MeSATPost", recibo.codigoPostal ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@MeSATNomEmp", recibo.nombre ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ClkDet", ClkDet);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            
            var modelView = new NominaViewModel {
                BaseDatos = BaseDatosConsulta,
                Models = ResCFDIS,
                BasesDatos = BasesDatos
            };

            return View("ActualizaCFDI", modelView);
        }

        public ActionResult PerDedEmpleado() {

            var modelView = new NominaViewModel {
                BaseDatos = "DEPASO2",
                Quincena = 15,
                Procesados = new DataTable()
            };
            return View(modelView);
        }

        public ActionResult CodigoPostalEmpleado() {

            Global global = new Global(_coneccionService);

            string ConsultaRfc = $"SELECT * FROM RespaldoCFDI";
            DataTable ResCFDIS = new DataTable();
            DataTable BasesDatos = global.BasesDatosOperativas();

            var modelView = new NominaViewModel {
                BaseDatos = "",
                Models = ResCFDIS,
                BasesDatos = BasesDatos
            };

            return View(modelView);
        }


        [HttpPost]
        public ActionResult CodigoPostalEmpleado(string BaseDatosConsulta = "") {
            Global global = new Global(_coneccionService);
            string baseDatos = BaseDatosConsulta;
            var sql = @$"
                SELECT
                    r.MeRfc,
                    eg.ClkDet,
                    eg.MeSATCPost AS CP_Actual,
                    '' as CP_XML,                   
                    '' as NOMBRECOMPLETO_XML,
                    r.PrXML
                FROM RespaldoCFDI r
                INNER JOIN [{baseDatos}].dbo.Empleado e ON e.MeRfc = r.MeRfc
                INNER JOIN [{baseDatos}].dbo.Empleado_Generales eg ON eg.ClkDet = e.ClkDet
                WHERE r.PrXML IS NOT NULL AND r.BaseDatos = '{baseDatos}' AND r.ClkPr = 'PRO20261500'
            ";

            DataTable EmpleadosCodigoPostal = global.ConsultaGeneral(sql, "IESYST");

            foreach (DataRow row in EmpleadosCodigoPostal.Rows) {
                string xml = row["PrXML"].ToString();
                string codigoPostal = "";
                if (!string.IsNullOrEmpty(xml)) {
                    ReciboModel Recibo = ReciboNominaController.CargarCFDI(xml, "", "", "", "");
                    row["CP_XML"] = Recibo.codigoPostal;
                    row["NOMBRECOMPLETO_XML"] = Recibo.nombre;
                }
            }

            foreach (DataRow row in EmpleadosCodigoPostal.Rows.Cast<DataRow>().ToList()) {
                string cpXml = row["CP_XML"]?.ToString().Trim() ?? "";
                string cpActual = row["CP_Actual"]?.ToString().Trim() ?? "";

                if (cpXml == cpActual) {
                    row.Delete();
                }
            }

            EmpleadosCodigoPostal.AcceptChanges();

            string sqlActualizaEmpleado = @"
                            UPDATE EMPG
                            SET
                                EMPG.MeSATCPost = @MeSATPost,
                                EMPG.MeSATNomEmp = @MeSATNomEmp
                            FROM Empleado_Generales AS EMPG
                            INNER JOIN Empleado AS EMP
                                ON EMP.ClkDet = EMPG.ClkDet
                            WHERE EMP.ClkDet = @ClkDet ;
                        ";

            string connectionString = _configuration.GetConnectionString(baseDatos);
            string Clkdets = "";

            using (var conexion = new SqlConnection(connectionString)) {
                conexion.Open();

                foreach (DataRow EmpleadoCodigoPostal in EmpleadosCodigoPostal.Rows) {
                    using (var cmd = new SqlCommand(sqlActualizaEmpleado, conexion)) {
                        cmd.Parameters.AddWithValue("@MeSATPost", EmpleadoCodigoPostal["CP_XML"] ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MeSATNomEmp", EmpleadoCodigoPostal["NOMBRECOMPLETO_XML"] ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ClkDet", EmpleadoCodigoPostal["ClkDet"]);
                        Clkdets += EmpleadoCodigoPostal["ClkDet"] + ",";
                        try {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            DataTable BasesDatos = global.BasesDatosOperativas();
            
            if (Clkdets != "") {
                var sqlEmpleadoCambiado = @$"SELECT   ClkDet, MeNoSegS, MeCurp, MeSATCPost, MeSATNomEmp, MeSATRegimen
                FROM Empleado_Generales WHERE ClkDet IN ({Clkdets.TrimEnd(',')})";


                DataTable EmpleadosCodigoPostalCambiados = global.ConsultaGeneral(sqlEmpleadoCambiado, baseDatos);

                DataTable ResCFDIS = new DataTable();

                var modelView = new NominaViewModel {
                    BaseDatos = BaseDatosConsulta,
                    Quincena = 15,
                    BasesDatos = BasesDatos,
                    Models = EmpleadosCodigoPostalCambiados
                };
                return View(modelView);
            }
            else {
                var modelView = new NominaViewModel {
                    BaseDatos = BaseDatosConsulta,
                    Quincena = 15,
                    BasesDatos = BasesDatos,
                    Models = EmpleadosCodigoPostal
                };
                return View(modelView);
            }
        }

        // POST: NominaController1/Delete/5
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
        string Valor(DataRow row, string columna) {
            if (row == null)
                return "";

            if (!row.Table.Columns.Contains(columna))
                return "";

            if (row[columna] == DBNull.Value)
                return "";

            return Convert.ToString(row[columna])?.Trim() ?? "";
        }
        public async Task<IActionResult> Importar(IFormFile archivo, string BaseDatos, int Quincena, string Producto) {
            if (archivo == null)
                return BadRequest();

            DataTable dt = ProcesarExcel(archivo);

            DataTable dtPerDed_Empleado = new DataTable();
            DataTable dtPerDed_Empleado_404 = dt.Clone();

            dtPerDed_Empleado.Columns.Add("ClkDet", typeof(Int32));
            dtPerDed_Empleado.Columns.Add("MePDTipo", typeof(Int16));
            dtPerDed_Empleado.Columns.Add("MePDClave", typeof(string));
            dtPerDed_Empleado.Columns.Add("MePDVParA", typeof(string));
            dtPerDed_Empleado.Columns.Add("MePDVImp", typeof(decimal));
            dtPerDed_Empleado.Columns.Add("MePDVVigI", typeof(string));
            dtPerDed_Empleado.Columns.Add("MePDVVenc", typeof(string));

            Global global = new Global(_coneccionService);

            foreach (DataRow row in dt.Rows) {
                int? numEmp = null;
                if (int.TryParse(row["No.Emp"]?.ToString(), out int resultado)) {
                    numEmp = resultado;
                }
                string Filiacion = row["Filiacion"].ToString();

                if (numEmp == null && Filiacion == "")
                    continue;

                string ConsultaClkDetRfc = $"SELECT ClkDet FROM Empleado WHERE ClkDet = {numEmp} AND MeRfc = '{Filiacion}'";
                string ConsultaRfc = $"SELECT ClkDet FROM Empleado WHERE MeRfc = '{Filiacion}'";


                DataTable numeroEmpleadoValidado = global.ConsultaGeneral(ConsultaClkDetRfc, BaseDatos);
                DataTable numeroEmpleado;

                if (numeroEmpleadoValidado == null || numeroEmpleadoValidado.Rows.Count == 0)
                    numeroEmpleado = global.ConsultaGeneral(ConsultaRfc, BaseDatos);

                else
                    numeroEmpleado = numeroEmpleadoValidado;

                int clkDet = 0;

                if (numeroEmpleado != null && numeroEmpleado.Rows.Count > 0) {
                    clkDet = Convert.ToInt32(numeroEmpleado.Rows[0]["ClkDet"]);
                }

                if (clkDet > 0)
                    foreach (DataColumn columna in dt.Columns) {
                        // Saltar la columna numEmp
                        if (columna.ColumnName == "ClkDet")
                            continue;

                        var valor = row[columna];

                        // Ignorar valores vacíos
                        if (valor == DBNull.Value ||
                            string.IsNullOrWhiteSpace(valor.ToString()))
                            continue;

                        DataRow nuevaFila = dtPerDed_Empleado.NewRow();
                        string TipoClave = columna.ColumnName;

                        string tipo = TipoClave.Substring(0, 1);   // "2"

                        if (tipo != "1" && tipo != "2")
                            continue;

                        string clave = TipoClave.Substring(1);

                        nuevaFila["ClkDet"] = numEmp;
                        nuevaFila["MePDTipo"] = tipo;
                        nuevaFila["MePDClave"] = clave;
                        nuevaFila["MePDVParA"] = "00";
                        nuevaFila["MePDVImp"] = Convert.ToDecimal(valor);
                        nuevaFila["MePDVVigI"] = "000000";
                        nuevaFila["MePDVVenc"] = "000000";


                        if (clkDet != 0)
                            dtPerDed_Empleado.Rows.Add(nuevaFila);
                    }
                else
                    dtPerDed_Empleado_404.ImportRow(row);
            }

            if (dtPerDed_Empleado.Rows.Count > 0) {
                
                global.ConsultaGeneral("DELETE FROM PerDed_Empleado", BaseDatos);
                
                foreach (DataRow row in dtPerDed_Empleado.Rows) {
                    Decimal Monto = Convert.ToDecimal(row["MePDVImp"]);
                    if (Monto == 0)
                        continue;

                    string MePDTipo = "1";
                    switch (row["MePDClave"]) {
                        case "3D":
                        case "37":
                        case "32":
                        case "69":
                        case "70":
                        case "75":
                        case "45":
                            MePDTipo = "3";
                            break;
                        case "17":
                        case "S5":
                            MePDTipo = "4";
                            break;
                        case "03":
                        case "AR":
                        case "AT":
                        case "AY":
                        case "BA":
                        case "CB":
                        case "CF":
                        case "EM":
                        case "FA":
                        case "FM":
                        case "FP":
                        case "I1":
                        case "PB":
                        case "RM":
                        case "S3":
                        case "90":
                            MePDTipo = "6";
                            break;
                        default:
                            MePDTipo = row["MePDTipo"].ToString();
                            break;
                    }
                            string sql = $@"
                        IF NOT EXISTS (
                                SELECT 1
                                FROM PerDed_Empleado
                                WHERE ClkDet = {Convert.ToInt32(row["ClkDet"])}
                                  AND MePDTipo = '1'
                                  AND MePDClave = '07'
                                  AND MePDVParA = '00'
                            )
                            BEGIN
                                INSERT INTO PerDed_Empleado
                                (
                                    ClkDet,
                                    MePDTipo,
                                    MePDClave,
                                    MePDVParA,
                                    MePDVImp,
                                    MePDVVigI,
                                    MePDVVenc
                                )
                                VALUES
                                (
                            {Convert.ToInt32(row["ClkDet"])},
                            '{MePDTipo}',
                            '{row["MePDClave"]}',
                            '{row["MePDVParA"]}',
                            {Monto},
                            '{row["MePDVVigI"]}',
                            '{row["MePDVVenc"]}'
                        );
                            END";
                    global.ConsultaGeneral(sql, BaseDatos);
                }
                
                string sqlEmpleadosSinMovimientos = @"
                SELECT E.ClkDet
                FROM Empleado AS E
                LEFT JOIN PerDed_Empleado AS PDE
                    ON PDE.ClkDet = E.ClkDet
                WHERE PDE.ClkDet IS NULL;"
                ;

                DataTable EmpleadosSinMovimientos = global.ConsultaGeneral(sqlEmpleadosSinMovimientos, BaseDatos);

                HashSet<string> empleadosProcesados = new();

                foreach (DataRow rowEmpleadoSinMov in EmpleadosSinMovimientos.Rows) {
                    int numeroEmpleado = Convert.ToInt32(rowEmpleadoSinMov[0].ToString());
                    string sql = $@"
                        INSERT INTO PerDed_Empleado
                        (
                            ClkDet,
                            MePDTipo,
                            MePDClave,
                            MePDVParA,
                            MePDVImp,
                            MePDVVigI,
                            MePDVVenc
                        )
                        VALUES
                        (
                            {Convert.ToInt32(rowEmpleadoSinMov["ClkDet"])},
                            '1', --MePDTipo
                            '07', --MePDClave
                            '0', --MePDVParA
                            0.01, --Monto
                            '000000', --MePDVVigI
                            '000000' --MePDVVenc
                        )";
                    string sqlte = $@"
                            IF NOT EXISTS (
                                SELECT 1
                                FROM PerDed_Empleado
                                WHERE ClkDet = {Convert.ToInt32(rowEmpleadoSinMov["ClkDet"])}
                                  AND MePDTipo = '1'
                                  AND MePDClave = '07'
                                  AND MePDVParA = '00'
                            )
                            BEGIN
                                INSERT INTO PerDed_Empleado
                                (
                                    ClkDet,
                                    MePDTipo,
                                    MePDClave,
                                    MePDVParA,
                                    MePDVImp,
                                    MePDVVigI,
                                    MePDVVenc
                                )
                                VALUES
                                (
                                    {Convert.ToInt32(rowEmpleadoSinMov["ClkDet"])},
                                    '1',
                                    '07',
                                    '00',
                                    0.01,
                                    '000000',
                                    '000000'
                                );
                            END
";
                    int clkDet = Convert.ToInt32(rowEmpleadoSinMov["ClkDet"]);

                    string clave = $"{clkDet}|1|07|00";

                    // Evitar duplicados dentro del mismo DataTable
                    if (!empleadosProcesados.Add(clave))
                        continue;

                    //global.ConsultaGeneral(sqlte, BaseDatos);
                }
            }

            var modelView = new NominaViewModel {
                BaseDatos = BaseDatos,
                Quincena = Quincena,
                Producto = $"PRO2026{Quincena}00",
                Procesados = dtPerDed_Empleado,
                EmpleadosFaltantes = dtPerDed_Empleado_404,
            };
            // Ya puedes trabajar con el DataTable
            return View("PerDedEmpleado", modelView);
        }
        public string obtenerAntiguedad(string antiguedad) {

            if (string.IsNullOrWhiteSpace(antiguedad))
                return "";

            // Formato P###W
            Match semanas = Regex.Match(antiguedad, @"^P(\d+)W$");
            if (semanas.Success) {
                int w = int.Parse(semanas.Groups[1].Value);
                int años = w / 52;
                int semanasRestantes = w % 52;

                int meses = semanasRestantes / 4;
                semanasRestantes %= 4;

                return $"{(años > 0 ? $" {años} año(s)," : $"")} {(meses > 0 ? $"{meses} mes(es)," : $"")} {(semanasRestantes > 0 ? $"{semanasRestantes} semana(s)" : $"")}";
            }

            // Formato PnYnMnD
            Match periodo = Regex.Match(
                antiguedad,
                @"^P(?:(\d+)Y)?(?:(\d+)M)?(?:(\d+)D)?$");

            if (periodo.Success) {
                List<string> partes = new List<string>();

                if (periodo.Groups[1].Success) {
                    int y = int.Parse(periodo.Groups[1].Value);
                    partes.Add($"{y} año{(y == 1 ? "" : "s")}");
                }

                if (periodo.Groups[2].Success) {
                    int m = int.Parse(periodo.Groups[2].Value);
                    partes.Add($"{m} mes{(m == 1 ? "" : "es")}");
                }

                if (periodo.Groups[3].Success) {
                    int d = int.Parse(periodo.Groups[3].Value);
                    partes.Add($"{d} día{(d == 1 ? "" : "s")}");
                }

                return string.Join(", ", partes);
            }

            return antiguedad;
        }
        [HttpPost]
        public IActionResult ProcesarSiguiente( string BaseDatos, string Producto, int Quincena, string procesadosJson) {

            Global global = new Global(_coneccionService);
            DataTable procesados = JsonToDataTable(procesadosJson);

            string vaciarTablaProducto = $"DELETE FROM PerDed_Producto WHERE ClkPr = '{Producto}'";
            global.ConsultaGeneral(vaciarTablaProducto, BaseDatos);

            var consecutivos = new Dictionary<int, int>();

            foreach (DataRow procesado in procesados.Rows) {

                int clkDet = Convert.ToInt32(procesado["ClkDet"]);
                var MePDTipo = procesado["MePDTipo"].ToString() ;
                var MePDClave = procesado["MePDClave"].ToString();
                
                if (!consecutivos.ContainsKey(clkDet))
                    consecutivos[clkDet] = 1;
                else
                    consecutivos[clkDet]++;
                
                switch (MePDTipo) {
                    case "3":
                        MePDTipo = "1";
                        break;
                    case "4":
                    case "6":
                        MePDTipo = "2";
                        break;
                }

                int consecutivo = consecutivos[clkDet];

                string sqlActualizar = @$"UPDATE PerDed_Producto
                    SET PrPDImporte =  {procesado["MePDVImp"]}
                    WHERE ClkPr = '{Producto}'
                        AND PrPDClave = '{MePDClave}'
                        AND PrPDTipo = '{MePDTipo}'
                        AND ClkDet = {procesado["ClkDet"]};" ;

                decimal importe = procesado.IsNull("MePDVImp") ? 0m : Convert.ToDecimal(procesado["MePDVImp"]);

                if (importe == 0m)
                    continue;

                string sqlInsertar = $"INSERT INTO PerDed_Producto (ClkPr, ClkDet, ClkSeqE, ClkSeq, PrPDTipo, PrPDClave, PrPDImporte, PrPDParA)" +
                                     $"VALUES ('{Producto}', {procesado["ClkDet"]}, 1, {consecutivo},  {MePDTipo}, '{MePDClave}',{procesado["MePDVImp"]}, '00')";

                    global.ConsultaGeneral(sqlInsertar, BaseDatos);
                
            }

            var modelView = new NominaViewModel {
                BaseDatos = BaseDatos,
                Quincena = Quincena,
                Producto = Producto,
                Procesados = procesados
            };
            // Ya puedes trabajar con el DataTable
            return View("PerDedEmpleado", modelView);
        }

        public DataTable ProcesarExcel(IFormFile archivo) {
            DataTable dt;


            switch (Path.GetExtension(archivo.FileName).ToLower()) {
                case ".xlsx":
                case ".xls":
                    dt = RecursosHumanos.Controllers.ArchivoController.DtLeerExcel(archivo);
                    break;

                default:
                     dt = new DataTable(); 
                    break;
            }

            if (dt.Rows.Count <= 0)
                return dt;

            DataRow filaTitulos = dt.Rows[0];

            for (int i = 0; i < dt.Columns.Count; i++) {
                string titulo = filaTitulos[i]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(titulo))
                    titulo = $"Column{i}";

                string nombreOriginal = titulo;
                int contador = 0;

                while (dt.Columns.Cast<DataColumn>()
                    .Any(c => c.ColumnName == titulo && c.Ordinal != i)) {
                    titulo = $"{nombreOriginal}_{contador++}";
                }

                dt.Columns[i].ColumnName = titulo;
            }

            dt.Rows.RemoveAt(0);

            return dt;
        }

        private DataTable JsonToDataTable(string json) {
            var datos = JsonSerializer.Deserialize<
                List<Dictionary<string, JsonElement>>
            >(json);

            DataTable dt = new DataTable();

            if (datos == null || datos.Count == 0)
                return dt;

            // Crear columnas
            foreach (var columna in datos[0].Keys) {
                dt.Columns.Add(columna);
            }

            // Crear filas
            foreach (var item in datos) {
                DataRow row = dt.NewRow();

                foreach (var campo in item) {
                    if (campo.Value.ValueKind == JsonValueKind.Null)
                        row[campo.Key] = DBNull.Value;
                    else
                        row[campo.Key] = campo.Value.ToString();
                }

                dt.Rows.Add(row);
            }

            return dt;
        }

    }

}
