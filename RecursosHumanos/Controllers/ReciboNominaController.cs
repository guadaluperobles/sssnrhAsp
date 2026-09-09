using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using RecursosHumanos.ViewModel;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace RecursosHumanos.Controllers {
    public class ReciboNominaController : Controller {
        private readonly ConeccionService _coneccionService;
        private readonly IWebHostEnvironment _env;

        public ReciboNominaController(ConeccionService coneccionService) {
            _coneccionService = coneccionService;
        }
        // GET: ReciboNominaController
        public ActionResult Index() {
            DateTime Hoy = DateTime.Now;

            int Ejercicio = DateTime.Now.Year;
            int Quincena = (DateTime.Now.Month - 1) * 2;

            var model = new RecibosViewModel{
                Recibos = null,
                NumeroEmpleado = "",
                EjercicioInicio = "",
                EjercicioFin = Ejercicio.ToString(),
                QuincenaInicio = "",
                QuincenaFin = Quincena.ToString()
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult Index(string numeroEmpleado, string ejercicioInicio, string ejercicioFin, string quincenaInicio, string quincenaFin,string soloTimbrados = "true") {
            DataTable recibos = new DataTable();
            var model = new RecibosViewModel {
                //BasesDatos = _coneccionService.CargarBasesDatosOperativas(),
                Recibos = null,
                NumeroEmpleado = numeroEmpleado,
                EjercicioInicio = ejercicioInicio,
                EjercicioFin = ejercicioFin,
                QuincenaInicio = quincenaInicio,
                QuincenaFin = quincenaFin,
    
            };

            string ComplementarConsulta = "";

            if(bool.Parse(soloTimbrados))
                ComplementarConsulta = " pd.PrUUID IS NOT NULL AND LTRIM(RTRIM(pd.PrUUID)) <> '' ";

            string anios = "";

            if (!String.IsNullOrWhiteSpace(ejercicioInicio) && String.IsNullOrWhiteSpace(ejercicioFin)) {
                if (ComplementarConsulta != "") {
                    ComplementarConsulta += " AND ";
                }
                ComplementarConsulta = " (pc.PrAno = " + ejercicioInicio + ") ";
            }

            if (!String.IsNullOrWhiteSpace(ejercicioFin)) {
                if (ComplementarConsulta != "") {
                    ComplementarConsulta += " AND ";
                }

                if (String.IsNullOrWhiteSpace(ejercicioInicio)) {
                    ComplementarConsulta += " (pc.PrAno = " + ejercicioFin + ") ";
                    anios = ejercicioFin;
                }
                else {

                    string inicio = ejercicioInicio;
                    string fin = ejercicioFin;
                    int intInicio = Convert.ToInt32(inicio);
                    int intFin = Convert.ToInt32(fin);
                    int recorrer = (intInicio - intFin);
                    int incrementar = intInicio;
                    string consultarVariosA = "";

                    if (intInicio > intFin) {
                        incrementar = intFin;
                    }

                    for (int i = 0; i <= Math.Abs(recorrer); i++) {
                        if (consultarVariosA != "") {
                            consultarVariosA += " OR ";
                        }
                        consultarVariosA += " pc.PrAno = " + (incrementar + i) + " ";
                    }

                    anios = ejercicioInicio + "-" + ejercicioFin;
                    ComplementarConsulta += " (" + consultarVariosA + ") ";
                }
            }

            if (!String.IsNullOrWhiteSpace(quincenaInicio) && String.IsNullOrWhiteSpace(quincenaFin)) {
                if (ComplementarConsulta != "") {
                    ComplementarConsulta += " AND ";
                }
                ComplementarConsulta = " (pc.PrQna = " + quincenaInicio + ") ";
            }


            if (!String.IsNullOrWhiteSpace(quincenaFin)) {
                if (ComplementarConsulta != "") {
                    ComplementarConsulta += " AND ";
                }

                if (String.IsNullOrWhiteSpace(quincenaInicio)) {
                    ComplementarConsulta += " (pc.PrQna = " + quincenaFin + ") ";
                    anios = quincenaFin;
                }
                else {
                    string inicio = quincenaInicio;
                    string fin = quincenaFin;
                    int intInicio = Convert.ToInt32(inicio);
                    int intFin = Convert.ToInt32(fin);
                    int recorrer = (intInicio - intFin);
                    int incrementar = intInicio;
                    string consultarVariosQ = "";

                    if (intInicio > intFin) {
                        incrementar = intFin;
                    }

                    for (int i = 0; i <= Math.Abs(recorrer); i++) {
                        if (consultarVariosQ != "") {
                            consultarVariosQ += " OR ";
                        }
                        consultarVariosQ += " pc.PrQna = " + (incrementar + i) + " ";
                    }

                    anios = quincenaInicio + "-" + quincenaFin;
                    ComplementarConsulta += " (" + consultarVariosQ + ") ";
                }
                if (!String.IsNullOrWhiteSpace(numeroEmpleado)) {
                    if (ComplementarConsulta != "") {
                        ComplementarConsulta += " AND ";
                    }

                    ComplementarConsulta += $" CONCAT(emp.MeRfc, CAST(pd.ClkDet AS VARCHAR)) like '%{numeroEmpleado}%'";
                }
            }

            if (!String.IsNullOrWhiteSpace(numeroEmpleado)) {
                if (ComplementarConsulta != "") {
                    ComplementarConsulta += " AND ";
                }

                ComplementarConsulta += $" CONCAT(emp.MeRfc, CAST(pd.ClkDet AS VARCHAR)) like '%{numeroEmpleado}%'";
            }
            //ConsultaRespaldoCFDI
            Global global = new Global(_coneccionService);

            DataTable recibosCFDI = global.ConsultaGeneral(ConsultasModel.ConsultaCFDI + " WHERE " + ComplementarConsulta + " ORDER BY pc.PrQna DESC");
            ComplementarConsulta = ComplementarConsulta.Replace("emp.", "").Replace("pd.", "").Replace("pc.", "");
            DataTable recibosRespaldo = global.ConsultaGeneral(ConsultasModel.ConsultaRespaldoCFDI + " WHERE " + ComplementarConsulta + " ORDER BY PrQna DESC", "IESYST");
            

            recibosCFDI.Merge(recibosRespaldo);

            if (recibosCFDI.Rows.Count > 0)
                recibosCFDI = recibosCFDI.AsEnumerable().GroupBy(row => row.Field<string>("PrUUID")).Select(g => g.First()).CopyToDataTable();

            model.Recibos = recibosCFDI;
            return View(model);
        }

        [HttpGet]
        public IActionResult Descargar(string uuid) {
            string nombreArchivo = uuid ?? string.Empty;
            string xml = string.Empty;

            string consultaXml = $"SELECT PrXML FROM Producto_Detalle WHERE (PrUUID = '{uuid}')";
            Global global = new Global(_coneccionService);
            DataTable recibosCFDI = global.ConsultaGeneral(consultaXml);

            if (recibosCFDI.Rows.Count == 0)
                return NotFound("No se encontró el CFDI.");

            if (recibosCFDI.Rows[0]["PrXML"] == DBNull.Value)
                return NotFound("El CFDI no contiene XML.");

            byte[] bytes = Encoding.UTF8.GetBytes(recibosCFDI.Rows[0]["PrXML"].ToString());
            return File( bytes, "application/xml",  $"{nombreArchivo}.xml");
        }
        [HttpPost]
        public ActionResult ReciboNomina(string numeroEmpleado, string e) {
            DataTable recibos = new DataTable();

            return View(recibos);
        }

        [HttpGet]
        public IActionResult Reporte(string UUID) {
            var datos = ObtenerDatos(UUID);

            return PartialView("_Reporte", datos);
        }
        public async Task<IActionResult> AnalizarRespaldo() {
            
            var model = new RecibosViewModel {
                Recibos = null,
            };

            Global global = new Global(_coneccionService);
            model.Recibos = global.ConsultaGeneral("SELECT * FROM RespaldoCFDI WHERE  (Rfc IS NOT NULL)", "IESYST");
            foreach (DataRow r in model.Recibos.Rows) {
                if (r.IsNull("Xml"))
                    continue;

                int Id = Convert.ToInt32(r["Id"]);
                string xml = r["Xml"].ToString();
                string producto = r["Producto"].ToString(); //PRE20261240


                if (string.IsNullOrWhiteSpace(xml))
                    continue;

                if (Id <= 0)
                    continue;

                if (string.IsNullOrWhiteSpace(producto))
                    continue;

                if (xml.StartsWith("?")) {
                    xml = xml.Substring(1);
                }


                int anio = int.Parse(producto.Substring(3, 4));
                int quincena = int.Parse(producto.Substring(7, 2));
                string BaseDatos = r["BaseDatos"].ToString();

                //https://localhost:7281/ReciboNomina/AnalizarRespaldo
                ReciboModel recibo = CargarCFDI(xml, quincena.ToString(), "", "", BaseDatos);

                string rfc = recibo.rfc;
                string FechaTimbrado = recibo.fechaEmision;
                string FechaCertificacion = recibo.fechaHoraCertificacion;
                string FechaPago = recibo.fechaDePago;

                string sql = @$"
                    UPDATE RespaldoCFDI
                    SET
                        FechaTimbrado = '{FechaTimbrado}',
                        NoCertificadoSat = '{recibo.noComprobante}',
                        Rfc = '{rfc}',
                        Periodo = {anio},
                        Quincena = {quincena},
                        FechaTimbradoXml = '{FechaPago}'
                    WHERE Id = {Id}";

                model.Recibos = global.ConsultaGeneral(sql, "IESYST");

            }
            return PartialView("Index");
        }
        public IActionResult GenerarReporte(string UUID) {
            try {
                LocalReport reporte = new LocalReport();
                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "Reportes", "rptReciboNomina.rdlc");
                if (!System.IO.File.Exists(ruta)) {
                    throw new Exception(ruta);
                }
                reporte.ReportPath = ruta;
                var resibo = ObtenerDatos(UUID);
                var resibos = new List<ReciboModel> { resibo };

                reporte.DataSources.Add(new ReportDataSource("dsReciboNomina", resibos));
                reporte.DataSources.Add(new ReportDataSource("dsPercepciones", resibo.Percepciones));
                reporte.DataSources.Add(new ReportDataSource("dsDeducciones", resibo.Deducciones));

                byte[] pdf = reporte.Render("PDF");

                return File(pdf, "application/pdf");
            }
            catch (Exception ex) {
                return Content(ex.ToString());
            }
        }
        public ReciboModel ObtenerDatos(string UUID) {

            Global global = new Global(_coneccionService);//BuscarRespaldoCFDI
            string consulta = $"{ConsultasModel.BuscarCFDI} WHERE pd.PrUUID = '{UUID}'";
            DataTable recibosCFDI = global.ConsultaGeneral(consulta);

            /**
             * Obtiene datos de la tabla de respaldos 
            string consultaR = $"{ConsultasModel.BuscarRespaldoCFDI} WHERE PrUUID = '{UUID}'";
            DataTable recibosRespaldo = global.ConsultaGeneral(consultaR, "IESYST");
            recibosCFDI.Merge(recibosRespaldo);
            
            if (recibosCFDI.Rows.Count > 0)
                recibosCFDI = recibosCFDI.AsEnumerable().GroupBy(row => row.Field<string>("PrUUID")).Select(g => g.First()).CopyToDataTable();
            /**/

            if (recibosCFDI == null || recibosCFDI.Rows.Count == 0) {
                return null;
            }
            DataRow rcb = recibosCFDI.Rows[0];
            string nombreArchivo = rcb[1].ToString() + "_" + rcb[2].ToString() + "_" + rcb[5].ToString();

            var recibo = CargarCFDI(rcb[6].ToString(), rcb[2].ToString(), nombreArchivo, rcb[3].ToString(), rcb[7].ToString());
            return recibo;
        }
        public static ReciboModel CargarCFDI(string vXml, string vQuincena, string vNombreArchivo, string clavePago, string nombreBaseDatos) {
            
            ReciboModel reciboNomina = new ReciboModel();
            XmlDocument xmlDoc = new XmlDocument();
            string myXML = vXml;
            int c;

            xmlDoc.Load(new System.IO.StringReader(myXML));

            reciboNomina.XML = myXML;
            reciboNomina.BaseDatos = nombreBaseDatos;
            reciboNomina.NombreArchivo = vNombreArchivo;

            foreach (XmlAttribute atributo in xmlDoc.DocumentElement.Attributes) {
                switch (atributo.Name) {
                    case "fecha":
                    case "Fecha":
                        reciboNomina.noComprobante = atributo.Value.Substring(0, 4) + "-" + xmlDoc.DocumentElement.GetAttribute("Folio");

                        reciboNomina.fechaEmision = atributo.Value.Replace("T", " ");
                        break;

                    case "descuento":
                    case "Descuento":
                        reciboNomina.totalDeducciones = Convert.ToDecimal(atributo.Value).ToString("N2");
                        break;

                    case "total":
                    case "Total":
                        reciboNomina.total = atributo.Value;
                        break;
                }
            }

            reciboNomina.totalPagar = Convert.ToDecimal(reciboNomina.total).ToString("N2");

            for (c = 0; c < xmlDoc.DocumentElement.ChildNodes.Count; c++) {
                if (xmlDoc.DocumentElement.ChildNodes[c]?.Name == "cfdi:Complemento") {
                    for (int i = 0; i < xmlDoc.DocumentElement.ChildNodes[c]?.ChildNodes.Count; i++) {
                        var nodoPrincipal = xmlDoc.DocumentElement?.ChildNodes[c]?.ChildNodes[i];
                        switch (nodoPrincipal.Name) {
                            case "nomina:Nomina":
                                for (int j = 0; j < nodoPrincipal.ChildNodes.Count; j++) {
                                    decimal totalGravado = Convert.ToDecimal(nodoPrincipal.ChildNodes[j]?.Attributes["TotalGravado"].Value);
                                    decimal totalExento = Convert.ToDecimal(nodoPrincipal.ChildNodes[j]?.Attributes["TotalExento"].Value);
                                    reciboNomina.totalDeducciones = (totalGravado + totalExento).ToString("N2");
                                }

                                break;
                        }
                    }
                }
            }

            reciboNomina.quincena = vQuincena;
            for (c = 0; c < xmlDoc.DocumentElement.ChildNodes.Count; c++) {
                switch (xmlDoc.DocumentElement.ChildNodes[c].Name) {
                    case "cfdi:Conceptos":

                        for (int i = 0; i < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[0].Attributes.Count; i++) {
                            switch (xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[0].Attributes[i].Name) {
                                case "ValorUnitario":
                                    reciboNomina.totalPercepciones = Convert.ToDecimal(xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[0].Attributes["ValorUnitario"].Value).ToString("N2");
                                    break;

                                case "valorUnitario":
                                    reciboNomina.totalPercepciones = Convert.ToDecimal(xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[0].Attributes["valorUnitario"].Value).ToString("N2");
                                    break;
                            }
                        }

                        break;

                    case "cfdi:Complemento":

                        reciboNomina.fechaDePago = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[0].Attributes["FechaPago"].Value;
                        reciboNomina.periodoDePago = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[0].Attributes["FechaInicialPago"].Value + " AL " + xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[0].Attributes["FechaFinalPago"].Value;

                        for (int i = 0; i < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes.Count; i++) {
                            var nodoPrincipal = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i];
                            switch (nodoPrincipal.Name) {
                                case "nomina12:Nomina":
                                    var nodo12 = xmlDoc.DocumentElement?.ChildNodes[c]?.ChildNodes[i]?.ChildNodes[1];
                                    for (int x = 0; x < nodo12.Attributes.Count; x++) {
                                        if (nodo12.Attributes[x].Name == "NumSeguridadSocial") {
                                            string SeguridadSocial = nodo12.Attributes["NumSeguridadSocial"].Value;
                                            reciboNomina.noSeguridadSocial = SeguridadSocial != "" ? SeguridadSocial : "0000000000";
                                        }
                                    }

                                    for (int x = 0; x < nodo12.Attributes.Count; x++) {
                                        if (nodo12.Attributes[x].Name == "Departamento") {
                                            reciboNomina.centroDeTrabajo = nodo12.Attributes["Departamento"].Value;
                                        }
                                    }

                                    var ant = nodo12?.Attributes["Antigüedad"]?.Value;
                                    reciboNomina.antiguedad = obtenerAntiguedad(ant);

                                    reciboNomina.CURP = nodo12.Attributes["Curp"].Value;
                                    reciboNomina.noEmpleado = nodo12.Attributes["NumEmpleado"].Value;
                                    reciboNomina.puesto = nodo12.Attributes["Puesto"].Value;

                                    break;

                                case "nomina:Nomina":
                                    var nodoNomina = xmlDoc.DocumentElement?.ChildNodes[c]?.ChildNodes[i];

                                    reciboNomina.noSeguridadSocial = nodoNomina?.Attributes["NumSeguridadSocial"].Value;

                                    var ant2 = nodoNomina?.Attributes["Antiguedad"].Value;
                                    reciboNomina.antiguedad = obtenerAntiguedad(ant2);

                                    reciboNomina.CURP = nodoNomina?.Attributes["CURP"].Value;
                                    reciboNomina.noEmpleado = nodoNomina?.Attributes["NumEmpleado"].Value;
                                    reciboNomina.puesto = nodoNomina?.Attributes["Puesto"].Value;
                                    reciboNomina.centroDeTrabajo = nodoNomina?.Attributes["Departamento"].Value;

                                    break;

                                case "tfd:TimbreFiscalDigital":
                                    var nodoTimbreFiscalDigital = xmlDoc.DocumentElement?.ChildNodes[c]?.ChildNodes[i];

                                    reciboNomina.folioFiscal = nodoTimbreFiscalDigital.Attributes["UUID"].Value;
                                    reciboNomina.fechaHoraCertificacion = nodoTimbreFiscalDigital.Attributes["FechaTimbrado"].Value;

                                    for (int j = 0; j < nodoTimbreFiscalDigital.Attributes.Count; j++) {
                                        switch (nodoTimbreFiscalDigital.Attributes[j].Name) {
                                            case "Version":
                                                reciboNomina.version = nodoTimbreFiscalDigital.Attributes["Version"].Value;
                                                break;

                                            case "version":
                                                reciboNomina.version = nodoTimbreFiscalDigital.Attributes["version"].Value;
                                                break;

                                            case "RfcProvCertif":
                                                reciboNomina.rfcProveedor = nodoTimbreFiscalDigital.Attributes["RfcProvCertif"].Value;
                                                break;

                                            case "SelloCFD":
                                                reciboNomina.cadenaOriginalSAT = nodoTimbreFiscalDigital.Attributes["SelloCFD"].Value;
                                                break;

                                            case "selloCFD":
                                                reciboNomina.cadenaOriginalSAT = nodoTimbreFiscalDigital.Attributes["selloCFD"].Value;
                                                break;

                                            case "SelloSAT":
                                                reciboNomina.selloDigitalSAT = nodoTimbreFiscalDigital.Attributes["SelloSAT"].Value;
                                                break;

                                            case "selloSAT":
                                                reciboNomina.selloDigitalSAT = nodoTimbreFiscalDigital.Attributes["selloSAT"].Value;
                                                break;

                                            case "NoCertificadoSAT":
                                                reciboNomina.certificadoSAT = nodoTimbreFiscalDigital.Attributes["NoCertificadoSAT"].Value;
                                                break;

                                            case "noCertificadoSAT":
                                                reciboNomina.certificadoSAT = nodoTimbreFiscalDigital.Attributes["noCertificadoSAT"].Value;
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }

                        break;

                    case "cfdi:Emisor":

                        switch (xmlDoc.DocumentElement.ChildNodes[c].ChildNodes.Count) {
                            case 0:
                                reciboNomina.regimenFiscal = xmlDoc.DocumentElement.ChildNodes[c].Attributes["RegimenFiscal"].Value;
                                break;

                            case 1:
                                reciboNomina.regimenFiscal = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[0].Attributes["Regimen"].Value;
                                break;

                            case 2:
                                reciboNomina.regimenFiscal = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[1].Attributes["Regimen"].Value;
                                break;
                        }

                        break;

                    case "cfdi:Receptor":

                        for (int i = 0;
                             i < xmlDoc.DocumentElement.ChildNodes[c].Attributes.Count;
                             i++) {
                            switch (xmlDoc.DocumentElement.ChildNodes[c].Attributes[i].Name) {
                                case "Nombre":
                                    reciboNomina.nombre = xmlDoc.DocumentElement.ChildNodes[c].Attributes["Nombre"].Value;
                                    break;

                                case "Rfc":
                                    reciboNomina.rfc = xmlDoc.DocumentElement.ChildNodes[c].Attributes["Rfc"].Value;
                                    break;

                                case "nombre":
                                    reciboNomina.nombre = xmlDoc.DocumentElement.ChildNodes[c].Attributes["nombre"].Value;
                                    break;

                                case "rfc":
                                    reciboNomina.rfc = xmlDoc.DocumentElement.ChildNodes[c].Attributes["rfc"].Value;
                                    break;
                            }
                        }
                        break;
                }
            }

            reciboNomina.selloDigitalCFDI = "||"
                + reciboNomina.version + "|"
                + reciboNomina.folioFiscal + "|"
                + reciboNomina.fechaHoraCertificacion + "|"
                + reciboNomina.rfcProveedor + "|"
                + reciboNomina.cadenaOriginalSAT + "|"
                + reciboNomina.certificadoSAT + "||";

            string total = reciboNomina.total ?? "0.00";
            string totalSAT = new string('0', 18 - total.Substring(0, total.Length - 3).Length) + total + "0000";
            string fe = reciboNomina.cadenaOriginalSAT.Substring(reciboNomina.cadenaOriginalSAT.Length - 8);

            reciboNomina.qrVerificador =
                "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?&id="
                + reciboNomina.folioFiscal
                + "&re=SSS970311993"
                + "&rr=" + reciboNomina.rfc
                + "&tt=" + totalSAT
                + "&fe=" + fe;

            string NombreArchivo = vNombreArchivo + "_" + reciboNomina.rfc + "_" + reciboNomina.nombre;
            reciboNomina.CodigoQR = ArchivoController.GenerarQR(reciboNomina.qrVerificador);
            reciboNomina.importeLetras = Global.Letras(reciboNomina.totalPagar);

            string Clave = "";
            string Concepto = "";
            string TextImporte = "";

            List<ConceptosModel> conceptos = new List<ConceptosModel>();
            List<Percepcion> Percepciones = new List<Percepcion>();
            List<DeduccionModel> Deducciones = new List<DeduccionModel>();

            for (c = 0; c < xmlDoc.DocumentElement.ChildNodes.Count; c++) {
                if (xmlDoc.DocumentElement.ChildNodes[c].Name == "cfdi:Complemento") {
                    for (int i = 0; i < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes.Count; i++) {
                        var nodoPrincipal = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i];
                        switch (nodoPrincipal.Name) {
                            case "nomina12:Nomina":
                                for (int j = 0; j < nodoPrincipal.ChildNodes.Count; j++) {
                                    switch (nodoPrincipal.ChildNodes[j].Name) {
                                        case "nomina12:Percepciones":
                                            for (int k = 0; k < nodoPrincipal.ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = nodoPrincipal.ChildNodes[j].ChildNodes[k];
                                                Clave = nodo.Attributes["Clave"].Value;
                                                Concepto = nodo.Attributes["Concepto"].Value;
                                                decimal importe = Convert.ToDecimal(nodo.Attributes["ImporteGravado"].Value) + Convert.ToDecimal(nodo.Attributes["ImporteExento"].Value);
                                                TextImporte = importe.ToString("N2");
                                                Percepciones.Add(new Percepcion {
                                                    Clave = Clave,
                                                    Concepto = Concepto,
                                                    TextImporte = TextImporte,
                                                    Importe = importe
                                                });
                                            }
                                            break;
                                        case "nomina12:Deducciones":

                                            for (int k = 0; k < nodoPrincipal.ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = nodoPrincipal.ChildNodes[j].ChildNodes[k];
                                                Clave = nodo.Attributes["Clave"].Value;
                                                Concepto = nodo.Attributes["Concepto"].Value;
                                                Decimal importe = Convert.ToDecimal(nodo.Attributes["Importe"].Value);
                                                TextImporte = importe.ToString("N2");
                                                Deducciones.Add(new DeduccionModel {
                                                    Clave = Clave,
                                                    Concepto = Concepto,
                                                    TextImporte = TextImporte,
                                                    Importe = importe
                                                });
                                            }
                                            break;

                                        case "nomina12:OtrosPagos":

                                            for (int k = 0; k < nodoPrincipal.ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = nodoPrincipal.ChildNodes[j].ChildNodes[k];

                                                Clave = nodo.Attributes["Clave"].Value;
                                                Concepto = nodo.Attributes["Concepto"].Value;
                                                Decimal importe = Convert.ToDecimal(nodo.Attributes["Importe"].Value);
                                                TextImporte = importe.ToString("N2");

                                                Percepciones.Add(new Percepcion {
                                                    Clave = Clave,
                                                    Concepto = Concepto,
                                                    TextImporte = TextImporte,
                                                    Importe = importe
                                                });
                                            }
                                            break;
                                    }
                                }
                                break;

                            case "nomina:Nomina":
                                for (int j = 0; j < nodoPrincipal.ChildNodes.Count; j++) {
                                    switch (nodoPrincipal.ChildNodes[j].Name) {
                                        case "nomina:Percepciones":
                                            for (int k = 0; k < nodoPrincipal.ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = nodoPrincipal.ChildNodes[j].ChildNodes[k];

                                                Clave = nodo.Attributes["Clave"].Value;
                                                Concepto = nodo.Attributes["Concepto"].Value;
                                                decimal importe = Convert.ToDecimal(nodo.Attributes["ImporteGravado"].Value) + Convert.ToDecimal(nodo.Attributes["ImporteExento"].Value);

                                                TextImporte = importe.ToString("N2");

                                                Percepciones.Add(new Percepcion {
                                                    Clave = Clave,
                                                    Concepto = Concepto,
                                                    TextImporte = TextImporte,
                                                    Importe = importe
                                                });
                                            }
                                            break;

                                        case "nomina:Deducciones":
                                            for (int k = 0; k < nodoPrincipal.ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = nodoPrincipal.ChildNodes[j].ChildNodes[k];

                                                Clave = nodo.Attributes["Clave"].Value;
                                                Concepto = nodo.Attributes["Concepto"].Value;
                                                decimal importe = Convert.ToDecimal(nodo.Attributes["ImporteGravado"].Value) + Convert.ToDecimal(nodo.Attributes["ImporteExento"].Value);

                                                TextImporte = importe.ToString("N2");

                                                Deducciones.Add(new DeduccionModel {
                                                    Clave = Clave,
                                                    Concepto = Concepto,
                                                    TextImporte = TextImporte,
                                                    Importe = importe
                                                });
                                            }
                                            break;

                                        case "nomina:OtrosPagos":
                                            for (int k = 0; k < nodoPrincipal.ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = nodoPrincipal.ChildNodes[j].ChildNodes[k];

                                                Clave = nodo.Attributes["Clave"].Value;
                                                Concepto = nodo.Attributes["Concepto"].Value;
                                                decimal importe = Convert.ToDecimal(nodo.Attributes["ImporteGravado"].Value) + Convert.ToDecimal(nodo.Attributes["ImporteExento"].Value);
                                                TextImporte = importe.ToString("N2");

                                                Percepciones.Add(new Percepcion {
                                                    Clave = Clave,
                                                    Concepto = Concepto,
                                                    TextImporte = TextImporte,
                                                    Importe = importe
                                                });
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            reciboNomina.ClavePago = clavePago;
            reciboNomina.Deducciones = Deducciones;
            reciboNomina.Percepciones = Percepciones;

            return reciboNomina;
        }
        public static string obtenerAntiguedad(string antiguedad) {

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
        private static XmlNode? GetNodo(XmlDocument xmlDoc, int c, int i, int j) {
            return xmlDoc.DocumentElement?.ChildNodes[c]?.ChildNodes[i]?.ChildNodes[j];
        }
    }
}
