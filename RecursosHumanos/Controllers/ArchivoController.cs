using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using QRCoder;
using RecursosHumanos.Model;
using System.Data;
using System.Drawing;
using System.Text;

namespace RecursosHumanos.Controllers {
    class ArchivoController : Controller {
        private static string rutaLog = @"C:\LogsGenerador";
        private readonly IWebHostEnvironment _env;

        public ArchivoController(IWebHostEnvironment env) {
            _env = env;
        }
        public void EscribirXMLNomina(string nombreArchivo, string textoArchivo) {
            string? carpeta = Path.GetDirectoryName(nombreArchivo);

            if (!string.IsNullOrEmpty(carpeta)) {
                Directory.CreateDirectory(carpeta);
            }

            System.IO.File.WriteAllText(nombreArchivo, textoArchivo, Encoding.UTF8);
        }

        public byte[] EscribirPDFNomina(ReciboModel row) {
            var report = new LocalReport();

            report.ReportPath = Path.Combine(
                _env.ContentRootPath,
                "Reports",
                "rptReciboNomina.rdlc");

            var recibos = new List<ReciboModel> { row };
            report.DataSources.Add(new ReportDataSource("dsReciboNomina", recibos));
            report.DataSources.Add(new ReportDataSource("dsPercepciones", row.Percepciones));
            report.DataSources.Add(new ReportDataSource("dsDeducciones", row.Deducciones));

            return report.Render("PDF");
        }

        public static byte[] GenerarQR(string texto) {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            QRCodeData qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap bitmap = qrCode.GetGraphic(20)) {
                using (MemoryStream ms = new MemoryStream()) {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> SubirArchivo(IFormFile archivo) {
            if (archivo == null || archivo.Length == 0) {
                return BadRequest("No se seleccionó ningún archivo.");
            }

            var carpeta = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var nombreArchivo = Guid.NewGuid() + Path.GetExtension(archivo.FileName);
            var ruta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(ruta, FileMode.Create)) {
                await archivo.CopyToAsync(stream);
            }

            return Ok(new {
                NombreOriginal = archivo.FileName,
                NombreGuardado = nombreArchivo,
                Tamaño = archivo.Length
            });
        }
        [HttpPost]
        public async Task<IActionResult> SubirArchivos(List<IFormFile> archivos) {
            foreach (var archivo in archivos) {
                if (archivo.Length > 0) {
                    var carpeta = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    var nombreArchivo = Guid.NewGuid() + Path.GetExtension(archivo.FileName);
                    var ruta = Path.Combine(carpeta, nombreArchivo);

                    using (var stream = new FileStream(ruta, FileMode.Create)) {
                        await archivo.CopyToAsync(stream);
                    }

                    return Ok(new {
                        NombreOriginal = archivo.FileName,
                        NombreGuardado = nombreArchivo,
                        Tamaño = archivo.Length
                    });
                }
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CargarArchivo(IFormFile archivo) {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Seleccione un archivo.");

            var extension = Path.GetExtension(archivo.FileName).ToLower();

            switch (extension) {
                case ".txt":
                    await LeerTxt(archivo);
                    break;

                case ".xlsx":
                case ".xls":
                    await LeerExcel(archivo);
                    break;

                default:
                    return BadRequest("Tipo de archivo no soportado.");
            }

            return Ok();
        }
        private async Task LeerTxt(IFormFile archivo) {
            using var reader = new StreamReader(archivo.OpenReadStream());

            while (!reader.EndOfStream) {
                string? linea = await reader.ReadLineAsync();

                if (!string.IsNullOrWhiteSpace(linea)) {
                    Console.WriteLine(linea);
                }
            }
        }
        private async Task LeerExcel(IFormFile archivo) {
            using var stream = archivo.OpenReadStream();

            using var reader = ExcelReaderFactory.CreateReader(stream);

            var ds = reader.AsDataSet();

            DataTable tabla = ds.Tables[0];

            foreach (DataRow fila in tabla.Rows) {
                for (int i = 0; i < tabla.Columns.Count; i++) {
                    string valor = fila[i]?.ToString() ?? "";

                    Console.Write(valor + " | ");
                }

                Console.WriteLine();
            }
        }
        public static DataTable DtLeerExcel(IFormFile archivo) {
            using var stream = archivo.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            return reader.AsDataSet().Tables[0];
        }

        public IActionResult ExportarExcel(DataTable dt, string nombreArchivo) {
            using var workbook = new XLWorkbook();
            workbook.Worksheets.Add(dt, "Datos");
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File( stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",$"{nombreArchivo}.xlsx");
        }
        /*
        public static void ExportarExcel<T>( List<T> lista, string archivo, ProgressBar pb,  Label lb) {
            Excel.Application excel = new Excel.Application( );
            Excel.Workbook libro = excel.Workbooks.Add( );
            Excel.Worksheet hoja = (Excel.Worksheet)libro.ActiveSheet;

            string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string rutaCarpeta = Path.Combine(escritorio, "Archivos Generados");
            string miCarpeta = rutaCarpeta ;

            try {
                PropertyInfo[] propiedades = typeof(T).GetProperties( );

                for (int i = 0; i < propiedades.Length; i++) {
                    hoja.Cells[1, i + 1] = propiedades[i].Name;
                }
                NotificacionModel msj = new NotificacionModel( );

                msj.Mensaje(lb, "Comienzo de excel",pb, 45);

                float p = 50;
                float j = p / lista.Count;

                for (int fila = 0; fila < lista.Count; fila++) {
                    p += j;
                    for (int col = 0; col < propiedades.Length; col++) {
                        hoja.Cells[fila + 2, col + 1] = propiedades[col].GetValue(lista[fila], null);
                    }
                    /*pb.Value = (int)p;
                    lb.Text = "procesando " + fila + " de " + lista.Count;
                    Application.DoEvents( );
                    *
                    * /                
                    msj.Mensaje(lb, $"procesando { fila } de {lista.Count}" , pb, (int)p);
                    
                    Console.WriteLine("procesando " + fila + " de " + lista.Count + " " + p + " " + j);
                }

                hoja.Columns.AutoFit( );

                if (!Directory.Exists(miCarpeta)) {
                    Directory.CreateDirectory(miCarpeta);
                }

                string rutaArchivo = miCarpeta + "\\" + archivo + ".xlsx";

                if (File.Exists(rutaArchivo)) {
                    File.Delete(rutaArchivo);
                }

                libro.SaveAs(rutaArchivo);
                libro.Close(false);
                
                msj.Mensaje(lb, $"Excel creado en la carpeta {miCarpeta}", pb, 100);

                excel.Quit( );
                Process.Start(rutaCarpeta);
            }
            catch (Exception ex){
                ArchivoController.logErrors(ex, "ArchivoController 121");

                lb.Text = "Error: " + ex.Message;
                Console.WriteLine(ex.Message + "\n " + ex.StackTrace);
                Application.DoEvents( );
                throw;
            }
        }
        public byte[] GenerarQR( string texto ) {
            QRCodeGenerator qrGenerator = new QRCodeGenerator( );

            QRCodeData qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap bitmap = qrCode.GetGraphic(20)) {
                using (MemoryStream ms = new MemoryStream( )) {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray( );
                }
            }
        }
        public static string[] LeerTexto(string rutaALeer) {
            string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string rutaCarpeta = Path.Combine(escritorio, "Carga de Archivos");
            string rutaArchivo = Path.Combine(rutaCarpeta, "E0BA0A18-36E4-4AA7-B9FC-5FBA6FF20E5F_01.txt");

            if (!File.Exists(rutaArchivo)) {
                return new string[0];
            }

            string[] contenido = File.ReadAllLines(rutaALeer);

            return contenido;
        }
        public static string[] LeerArchivoPlazasVacantes(string rutaALeer) {

            if (!File.Exists(rutaALeer)) {
                return new string[0];
            }

            string[] contenido = File.ReadAllLines(rutaALeer);

            return contenido;
        }
        public static string SubirArchivo(string nombre) {
            OpenFileDialog ofd = new OpenFileDialog( );
            try {
                string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string carpetaDestino = Path.Combine(escritorio, "Carga de Archivos");

                if (!Directory.Exists(carpetaDestino)) {
                    Directory.CreateDirectory(carpetaDestino);
                }

                if (ofd.ShowDialog( ) == DialogResult.OK) {
                    string extension = Path.GetExtension(ofd.FileName).ToLower( );

                    /*if (extension == ".zip") {
                        string carpetaZip = Path.Combine(carpetaDestino, nombre);

                        if (Directory.Exists(carpetaZip))
                            Directory.Delete(carpetaZip, true);

                        Directory.CreateDirectory(carpetaZip);

                        ZipFile.ExtractToDirectory(ofd.FileName, carpetaZip);

                        return carpetaZip;
                    }
                    else {*
        *
        * /
                        string nombreArchivo = Path.Combine(carpetaDestino, nombre + extension);
                        File.Copy(ofd.FileName, nombreArchivo, true);
                        return nombreArchivo;
                    //}
                }

                return string.Empty;
            }
            catch (Exception ex) {
                ArchivoController.logErrors(ex, "ArchivoController 192");
                throw;
            }
        }
        public string[] ObtenerArchivos() {
            string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string carpeta = Path.Combine(escritorio, "Carga de Archivos");

            if (!Directory.Exists(carpeta))
                return new string[0];

            return Directory.GetFiles(carpeta);
        }
        public static Dictionary<string, string> llenarComboBoxArchivos() {
            string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string carpeta = Path.Combine(escritorio, "Carga de Archivos");

            Dictionary<string, string> archivos = new Dictionary<string, string>( );

            if (!Directory.Exists(carpeta))
                return archivos;

            archivos.Add("Seleccione...", "");

            foreach (string archivo in Directory.GetFiles(carpeta)) {
                archivos.Add(
                    Path.GetFileNameWithoutExtension(archivo),
                    archivo
                );
            }

            return archivos;
        }
        public static DataTable LeerExcel( string rutaArchivo, Label te, ProgressBar pb ) {
             DataTable dt = new DataTable();
                NotificacionModel msj = new NotificacionModel();

                Excel.Application excelApp = null;
                Excel.Workbook workbook = null;
                Excel.Worksheet worksheet = null;
                Excel.Range range = null;

                try
                {
                    excelApp = new Excel.Application();
                    excelApp.Visible = false;
                    excelApp.DisplayAlerts = false;
                    excelApp.ScreenUpdating = false;

                    workbook = excelApp.Workbooks.Open(rutaArchivo);

                    worksheet = workbook.Worksheets.Count < 2 ? (Excel.Worksheet)workbook.Sheets[1]  : (Excel.Worksheet)workbook.Sheets[2];

                    msj.Mensaje(te, "Obteniendo rango...", pb, 10);

                    range = worksheet.UsedRange;

                    int filas = range.Rows.Count;
                    int columnas = range.Columns.Count;

                    msj.Mensaje(te,  $"Filas: {filas:N0}  Columnas: {columnas:N0}", pb, 20);

                    // Leer TODO el rango de una sola vez
                    object[,] datos = range.Value2 as object[,];

                    if (datos == null)
                        throw new Exception("No fue posible leer el rango del Excel.");

                    // Encabezados
                    for (int c = 1; c <= columnas; c++)
                    {
                        string encabezado = "";

                        if (datos[1, c] != null)
                            encabezado = datos[1, c].ToString();

                        if (String.IsNullOrWhiteSpace(encabezado))
                            encabezado = "Columna" + c;

                        dt.Columns.Add(encabezado);
                    }

                    // Datos
                    for (int f = 2; f <= filas; f++)
                    {
                        DataRow row = dt.NewRow();

                        for (int c = 1; c <= columnas; c++)
                        {
                            row[c - 1] = datos[f, c] == null  ? "" : datos[f, c].ToString();
                        }

                        dt.Rows.Add(row);

                        // Actualizar la interfaz solo cada 500 filas
                        if (f % 500 == 0)
                            msj.Mensaje(te,  $"Leyendo {f:N0} de {filas:N0}",   pb,  20);
                    }

                    msj.Mensaje(te, "Lectura finalizada", pb, 100);

                    return dt;
                }
                finally
                {
                    if (workbook != null)
                        workbook.Close(false);

                    if (excelApp != null)
                        excelApp.Quit();

                    if (range != null)
                        Marshal.ReleaseComObject(range);

                    if (worksheet != null)
                        Marshal.ReleaseComObject(worksheet);

                    if (workbook != null)
                        Marshal.ReleaseComObject(workbook);

                    if (excelApp != null)
                        Marshal.ReleaseComObject(excelApp);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
        }
        public static void logErrors( Exception ex, string ubicacion ) {
            try {

                string fecha = DateTime.Now.ToString("ddMMyyyy_HHmmss");
                rutaLog += $"\\{fecha}_errores.log";
                Directory.CreateDirectory(Path.GetDirectoryName(rutaLog));

                using (StreamWriter sw = new StreamWriter(rutaLog, true)) {
                    sw.WriteLine("====================================");
                    sw.WriteLine("Here: " + ubicacion);
                    sw.WriteLine("Fecha: " + DateTime.Now);
                    sw.WriteLine("Mensaje: " + ex.Message);
                    sw.WriteLine("StackTrace: " + ex.StackTrace);
                    sw.WriteLine("====================================");
                    sw.WriteLine( );
                }
            }
            catch {
                // Evitar que el logger genere otro error
            }
        }//*/
    }
}
