using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.ViewModel;
using RecursosHumanos.Models;
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
            var model = new RecibosViewModel{
                //BasesDatos = _coneccionService.CargarBasesDatosOperativas(),
                Recibos = null,
                NumeroEmpleado = "",
                EjercicioInicio = "",
                EjercicioFin = "",
                QuincenaInicio = "",
                QuincenaFin = ""
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult Index(string numeroEmpleado, string ejercicioInicio, string ejercicioFin, string quincenaInicio, string quincenaFin) {
            DataTable recibos = new DataTable();
            var model = new RecibosViewModel {
                //BasesDatos = _coneccionService.CargarBasesDatosOperativas(),
                Recibos = null,
                NumeroEmpleado = numeroEmpleado,
                EjercicioInicio = ejercicioInicio,
                EjercicioFin = ejercicioFin,
                QuincenaInicio = quincenaInicio,
                QuincenaFin = quincenaFin
            };

            string ComplementarConsulta = " pd.PrUUID IS NOT NULL AND LTRIM(RTRIM(pd.PrUUID)) <> '' ";
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


            Global global = new Global(_coneccionService);
            model.Recibos = global.ConsultaGeneral(ConsultasModel.ConsultaCFDI + " WHERE " + ComplementarConsulta + " ORDER BY pc.PrQna DESC");

            return View(model);
        }

        [HttpPost]
        public IActionResult DescargarXML(string uuid) {
            string nombreArchivo = string.Empty;
            string xml = string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(xml);

            return File(
                bytes,
                "application/xml",
                $"{nombreArchivo}.xml");
        }
        // GET: ReciboNominaController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: ReciboNominaController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: ReciboNominaController/Create
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

        // GET: ReciboNominaController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: ReciboNominaController/Edit/5
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

        public ActionResult ReciboNomina(string numeroEmpleado, string e) {
            DataTable recibos = new DataTable();

            return View(recibos);
        }

        // GET: ReciboNominaController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: ReciboNominaController/Delete/5
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
        [HttpGet]
        public IActionResult Reporte(string UUID) {
            var datos = ObtenerDatos(UUID);

            return PartialView("_Reporte", datos);
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

            Global global = new Global(_coneccionService);
            string consulta = $"{ConsultasModel.BuscarCFDI} WHERE pd.PrUUID = '{UUID}'";
            DataTable dt = global.ConsultaGeneral(consulta);
            if (dt == null || dt.Rows.Count == 0) {
                return null;
            }
            DataRow rcb = dt.Rows[0];
            string nombreArchivo = rcb[1].ToString() + "_" + rcb[2].ToString() + "_" + rcb[5].ToString();

            var recibo = CargarCFDI(rcb[6].ToString(), rcb[2].ToString(), nombreArchivo, rcb[3].ToString(), rcb[7].ToString());
            return recibo;
        }
        public ReciboModel CargarCFDI(string vXml, string vQuincena, string vNombreArchivo, string clavePago, string nombreBaseDatos) {
            string myXML = vXml;

            XmlDocument xmlDoc = new XmlDocument();
            Global global = new Global(_coneccionService);

            int c;

            xmlDoc.Load(new System.IO.StringReader(myXML));
            ReciboModel reciboNomina = new ReciboModel();

            reciboNomina.XML = myXML;
            reciboNomina.BaseDatos = nombreBaseDatos;
            reciboNomina.NombreArchivo = vNombreArchivo;

            foreach (XmlAttribute atributo in xmlDoc.DocumentElement.Attributes) {
                switch (atributo.Name) {
                    case "Fecha":
                        reciboNomina.noComprobante = atributo.Value.Substring(0, 4) + "-" + xmlDoc.DocumentElement.GetAttribute("Folio");

                        reciboNomina.fechaEmision = atributo.Value.Replace("T", " ");
                        break;

                    case "Descuento":
                        reciboNomina.totalDeducciones = Convert.ToDecimal(atributo.Value).ToString("N2");
                        break;

                    case "Total":
                        reciboNomina.total = atributo.Value;
                        break;
                }
            }

            reciboNomina.totalPagar = Convert.ToDecimal(reciboNomina.total).ToString("N2");
            //importeLetras = "Cinco pesos"; ////Letras(total);

            for (c = 0; c < xmlDoc.DocumentElement.ChildNodes.Count; c++) {
                if (xmlDoc.DocumentElement.ChildNodes[c].Name == "cfdi:Complemento") {
                    for (int i = 0; i < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes.Count; i++) {
                        switch (xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Name) {
                            case "nomina:Nomina":
                                for (int j = 0; j < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes.Count; j++) {
                                    decimal totalGravado = Convert.ToDecimal(xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].Attributes["TotalGravado"].Value);
                                    decimal totalExento = Convert.ToDecimal(xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].Attributes["TotalExento"].Value);
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
                            switch (xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Name) {
                                case "nomina12:Nomina":
                                    for (int x = 0; x < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[1].Attributes.Count; x++) {
                                        if (xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[1].Attributes[x].Name == "NumSeguridadSocial") {
                                            string SeguridadSocial = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[1].Attributes["NumSeguridadSocial"].Value;
                                            reciboNomina.noSeguridadSocial = SeguridadSocial != "" ? SeguridadSocial : "0000000000";
                                        }
                                    }

                                    for (int x = 0; x < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[1].Attributes.Count; x++) {
                                        if (xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[1].Attributes[x].Name == "Departamento") {
                                            reciboNomina.centroDeTrabajo = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[1].Attributes["Departamento"].Value;
                                        }
                                    }
                                    reciboNomina.antiguedad = obtenerAntiguedad(xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[1].Attributes["Antigüedad"].Value);
                                    reciboNomina.CURP = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[1].Attributes["Curp"].Value;
                                    reciboNomina.noEmpleado = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[1].Attributes["NumEmpleado"].Value;
                                    reciboNomina.puesto = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[1].Attributes["Puesto"].Value;

                                    break;

                                case "nomina:Nomina":

                                    reciboNomina.noSeguridadSocial = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["NumSeguridadSocial"].Value;
                                    reciboNomina.antiguedad = obtenerAntiguedad(xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["Antiguedad"].Value);
                                    reciboNomina.CURP = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["CURP"].Value;
                                    reciboNomina.noEmpleado = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["NumEmpleado"].Value;
                                    reciboNomina.puesto = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["Puesto"].Value;
                                    reciboNomina.centroDeTrabajo = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["Departamento"].Value;

                                    break;

                                case "tfd:TimbreFiscalDigital":

                                    reciboNomina.folioFiscal = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["UUID"].Value;
                                    reciboNomina.fechaHoraCertificacion = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["FechaTimbrado"].Value;

                                    for (int j = 0; j < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes.Count; j++) {
                                        switch (xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes[j].Name) {
                                            case "Version":
                                                reciboNomina.version = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["Version"].Value;
                                                break;

                                            case "version":
                                                reciboNomina.version = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["version"].Value;
                                                break;

                                            case "RfcProvCertif":
                                                reciboNomina.rfcProveedor = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["RfcProvCertif"].Value;
                                                break;

                                            case "SelloCFD":
                                                reciboNomina.cadenaOriginalSAT = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["SelloCFD"].Value;
                                                break;

                                            case "selloCFD":
                                                reciboNomina.cadenaOriginalSAT = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["selloCFD"].Value;
                                                break;

                                            case "SelloSAT":
                                                reciboNomina.selloDigitalSAT = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["SelloSAT"].Value;
                                                break;

                                            case "selloSAT":
                                                reciboNomina.selloDigitalSAT = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["selloSAT"].Value;
                                                break;

                                            case "NoCertificadoSAT":
                                                reciboNomina.certificadoSAT = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["NoCertificadoSAT"].Value;
                                                break;

                                            case "noCertificadoSAT":
                                                reciboNomina.certificadoSAT = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Attributes["noCertificadoSAT"].Value;
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

            //ArchivoController archivo = new ArchivoController();
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
                        switch (xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].Name) {
                            case "nomina12:Nomina":
                                for (int j = 0; j < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes.Count; j++) {
                                    switch (xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].Name) {
                                        case "nomina12:Percepciones":
                                            for (int k = 0; k < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes[k];
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
                                                // PercepcionesTableAdapter.PercepcionesInsert(((DataRowView)bs.List[0])["ReciboId"], Clave, Concepto, Importe);
                                            }

                                            break;

                                        case "nomina12:Deducciones":

                                            for (int k = 0; k < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes[k];
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
                                                //DeduccionesTableAdapter.DeduccionesInsert( ((DataRowView)bs.List[0])["ReciboId"],Clave,Concepto,Importe);
                                            }

                                            break;

                                        case "nomina12:OtrosPagos":

                                            for (int k = 0; k < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes[k];

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
                                                //PercepcionesTableAdapter.PercepcionesInsert(((DataRowView)bs.List[0])["ReciboId"],Clave,Concepto, Importe);
                                            }
                                            break;
                                    }
                                }
                                break;

                            case "nomina:Nomina":
                                for (int j = 0; j < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes.Count; j++) {
                                    switch (xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].Name) {
                                        case "nomina:Percepciones":
                                            for (int k = 0; k < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes[k];

                                                Clave = nodo.Attributes["Clave"].Value;
                                                Concepto = nodo.Attributes["Concepto"].Value;
                                                decimal importe = Convert.ToDecimal(nodo.Attributes["ImporteGravado"].Value) + Convert.ToDecimal(nodo.Attributes["ImporteExento"].Value);

                                                TextImporte = importe.ToString("N2");

                                                Percepciones.Add(new Percepcion {
                                                    Clave = Clave,
                                                    Concepto = Concepto,
                                                    TextImporte = TextImporte,
                                                    Importe = importe
                                                });//PercepcionesTableAdapter.PercepcionesInsert(((DataRowView)bs.List[0])["ReciboId"],Clave,Concepto, Importe);
                                            }

                                            break;

                                        case "nomina:Deducciones":

                                            for (int k = 0; k < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes[k];

                                                Clave = nodo.Attributes["Clave"].Value;
                                                Concepto = nodo.Attributes["Concepto"].Value;
                                                decimal importe = Convert.ToDecimal(nodo.Attributes["ImporteGravado"].Value) + Convert.ToDecimal(nodo.Attributes["ImporteExento"].Value);

                                                TextImporte = importe.ToString("N2");

                                                Deducciones.Add(new DeduccionModel {
                                                    Clave = Clave,
                                                    Concepto = Concepto,
                                                    TextImporte = TextImporte,
                                                    Importe = importe
                                                });//DeduccionesTableAdapter.DeduccionesInsert(((DataRowView)bs.List[0])["ReciboId"], Clave, Concepto, Importe);
                                            }

                                            break;

                                        case "nomina:OtrosPagos":

                                            //for (int k = 0; k < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes.Count; k++) {
                                            //    XmlNode nodo = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes[k];
                                            for (int k = 0; k < xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes.Count; k++) {
                                                XmlNode nodo = xmlDoc.DocumentElement.ChildNodes[c].ChildNodes[i].ChildNodes[j].ChildNodes[k];

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
                                                //PercepcionesTableAdapter.PercepcionesInsert( ((DataRowView)bs.List[0])["ReciboId"],Clave,Concepto, Importe);
                                            }

                                            break;
                                    }
                                }

                                break;
                        }
                    }
                }
            }
            reciboNomina.Deducciones = Deducciones;
            reciboNomina.Percepciones = Percepciones;
            return reciboNomina;
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
        private static XmlNode? GetNodo(XmlDocument xmlDoc, int c, int i, int j) {
            return xmlDoc.DocumentElement?.ChildNodes[c]?.ChildNodes[i]?.ChildNodes[j];
        }
    }
}
