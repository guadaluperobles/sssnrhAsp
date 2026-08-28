using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using RecursosHumanos.ViewModel;
using System.Data;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecursosHumanos.Controllers {
    public class NominaController : Controller {
        // GET: NominaController1
        private readonly ConeccionService _coneccionService; 
        public NominaController(ConeccionService coneccionService) {
            _coneccionService = coneccionService;
        }
        public ActionResult Index() {

            var modelView = new NominaViewModel {
                BaseDatos = "DEPASO2",
                Quincena = 15,
                Procesados = new DataTable()
            };
            return View("PerDedEmpleado", modelView);
        }

        // GET: NominaController1/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: NominaController1/Create
        public ActionResult Create() {
            return View();
        }

        // POST: NominaController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }

        // GET: NominaController1/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: NominaController1/Edit/5
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

        public ActionResult PerDedEmpleado() {

            var modelView = new NominaViewModel {
                BaseDatos = "DEPASO2",
                Quincena = 15,
                Procesados = new DataTable()
            };
            return View(modelView);
        }

        // GET: NominaController1/Delete/5
        public ActionResult Delete(int id) {
            return View();
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
                    if (Monto <= 0)
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
                        )";
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
                    global.ConsultaGeneral(sql, BaseDatos);
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
        [HttpPost]
        public IActionResult ProcesarSiguiente( string BaseDatos, string Producto, int Quincena, string procesadosJson) {

            Global global = new Global(_coneccionService);
            DataTable procesados = JsonToDataTable(procesadosJson);

            foreach (DataRow procesado in procesados.Rows) {
                if ( (procesado["MePDTipo"].ToString() == "2" && procesado["MePDClave"].ToString() == "01") ||
                    (procesado["MePDTipo"].ToString() == "2" && procesado["MePDClave"].ToString() == "S2") ||
                    (procesado["MePDTipo"].ToString() == "2" && procesado["MePDClave"].ToString() == "62")) {

                    var MePDTipo = procesado["MePDClave"].ToString() == "62" ? "04" : procesado["MePDTipo"].ToString(); 
                    var MePDClave = procesado["MePDClave"].ToString();

                    string sqlActualizar = @$"UPDATE PerDed_Producto
                        SET PrPDImporte =  {procesado["MePDVImp"]}
                        WHERE ClkPr = '{Producto}'
                          AND PrPDClave = '{MePDClave}'
                          AND PrPDTipo = '{MePDTipo}'
                          AND ClkDet = {procesado["ClkDet"]};" ;

                    global.ConsultaGeneral(sqlActualizar, BaseDatos);
                }
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
