using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecursosHumanos.Controllers {
    public class MigracionController : Controller {
        // GET: MigracionController
        private readonly ConeccionService _coneccionService;
        private readonly IConfiguration _configuration;
        public MigracionController(ConeccionService coneccionService,  IConfiguration configuration) {
            _coneccionService = coneccionService;
            _configuration = configuration;
        }
        public ActionResult Index() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: MigracionController/Details/5
        public ActionResult Index(string NumeroOrigen, string NumeroDestino, string BaseDatosConsulta) {
            
            if (!int.TryParse(NumeroOrigen, out int numeroOrigen)) {
                return View("Index", "El número de origen no es válido.");
            }

            if (!int.TryParse(NumeroDestino, out int numeroDestino)) {
                return View("Index", "El número de destino no es válido.");
            }

            if (string.IsNullOrWhiteSpace(BaseDatosConsulta)) {
                return View("Index", "Debe seleccionar la base de datos.");
            }

            var bd = Global.BaseDatosToIB(BaseDatosConsulta);
            string resultado = "";

            resultado += crearMigracion(numeroOrigen, numeroDestino, bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaEmpleado(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaEmpleado_Terceros(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaEscolares(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaEstacionamiento(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaExperiencia_Lab(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaFamiliares(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaAcumuladoA_DIMM(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaFUMP(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaProducto_Detalle(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaPlantilla_Detalle(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaPerDed_Producto(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaAcumulado_Anual(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaMovimiento_Nomina(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaPerDed_AcumuladoAnual(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaPerDed_Empleado(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaPension_Alimenticia(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaSarFovissste_Extraord(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaPerDedExt_Empleado(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaEmpleado_Generales(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += moverTablaHistorico_Movimiento(bd.BaseDatosOrigen, bd.BaseDatosDestino);
            resultado += CambiarEstatus(bd.BaseDatosOrigen, bd.BaseDatosDestino);

            return View("Index", resultado);
        }
        private string crearMigracion(int NumeroOrigen, int NumeroDestino, string BaseDatosOrigen, string BaseDatosDestino) {
            DateTime fecha = DateTime.Now;
            string query = @$"INSERT INTO {BaseDatosDestino}.dbo._MigracionControl (ClkDetOrigen, BDOrigen, ClkDetDestino, FechaMigracion, Estatus)
                VALUES({NumeroOrigen}, '{BaseDatosOrigen}', {NumeroDestino}, {fecha.ToString("ddMMyyyy")})";
            string Mensaje = $"Migración creada correctamente Base de datos {BaseDatosOrigen} con ({NumeroOrigen}) a {BaseDatosDestino} con ({NumeroDestino})";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaEmpleado(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	    INSERT INTO {BaseDatosDestino}.dbo.Empleado
                SELECT
                    mc.ClkDetDestino AS ClkDet, MeRfc, MeNomAP, MeNomAM, MeNomEmp, MeSexo, ClkEstC, MeFechIIst, MeFechIRam, MeCCont, ClkInstP, 
                    MeCtaBnco, MeHrTrab, MeEdMpi, MeVCTrab, MeCTrab, MeCTrabDist, MeTabPt, MeVPuesto, MePuesto, MeGpoPto, MeNumPto, MeVPClvPag, 
                    MeClvPag, MePtoEdo, MePtoFun, MeUAdmva, MeIMando, MeTmbc, MeNivel, MeRegimen, MeRango, MeJrnda, MePrcPt, MeEfDel, MeEfAl, 
                    ClkMov, MeNLote, MeNDocto, MeFchISit, MeFchTSit, MePDAA, MeIPal, MeIPPVAno, MeIPPVAnoA, MeICong, MeIRetr, MeIResp, 
                    MeDLabAno, MeDLabAnoA, MeDLabIst, MeDLabRam, MeDLicMed, MeDLicMedAnt, MeDLicSGS, MeDLicCGS, MeDsFlt, MeDsFltA, 
                    MeDsGdsF, MePerGr, MeDed01, MePerSB, MeSdoDI, MeNetLG, MeSdoLG, MeAcMePer, MeAcMeIsr, MeRespNPApl, MeRespMPApl, 
                    MeRespBPApl, MeCMovBP, MeIFnAn, MeTPLM, MeIndMe, MeCLABE, MeIndLS, MeSADImptoA, MePADImptoA
                FROM {BaseDatosOrigen}.dbo.Empleado e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente Empleado {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaEmpleado_Terceros(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	print('Importando la tabla Empleado_Terceros')
                INSERT INTO {BaseDatosDestino}.dbo.Empleado_Terceros
                SELECT
                  mc.ClkDetDestino AS ClkDet, ClkTA, ClkPyD, MeReferencia
                FROM {BaseDatosOrigen}.dbo.Empleado_Terceros e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet  AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente Empleado_Terceros {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaEscolares(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Escolares
                SELECT
                  mc.ClkDetDestino AS ClkDet, ClkSeq, EsNME, EsNombInst, EsDirInst, EsPeriodo, EsIndGA, EsIndSDE, EsIndNCE, 
                  EsGradoAvance, EsDescHrEsc, EsDescCoE, EsCompEst, EsIndR
                FROM {BaseDatosOrigen}.dbo.Escolares e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
        
            ";

            string Mensaje = $"Migración creada correctamente Escolares {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaEstacionamiento(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Estacionamiento
                SELECT
                    mc.ClkDetDestino AS ClkDet, ClkSeq, EtTipo, EtTarjeta, EtDescV, EtNumPlaca, EtIndR
                FROM {BaseDatosOrigen}.dbo.Estacionamiento e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
  
            ";
            string Mensaje = $"Migración creada correctamente Estacionamiento {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaExperiencia_Lab(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Experiencia_Lab
                    SELECT
                      mc.ClkDetDestino AS ClkDet, ClkSeq_Exp, ElRazonSoc, ElDomicilio, ElTelefono, ElPtoDesem, ElSecPubPriv, 
                      ElSdoInic, ElSdoFin, ElJefeInm, ElPeriodo, ElSolicInf, ElIndReg
                    FROM {BaseDatosOrigen}.dbo.Experiencia_Lab e
                    INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";
            string Mensaje = $"Migración creada correctamente Experiencia_Lab {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaFamiliares(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Familiares
                    SELECT
                      mc.ClkDetDestino AS ClkDet, ClkSeq, FmNombre, FmParentesco, FmFchNac, FmPoblNac, FmSexo, FmIndDepend, 
                      FmEstC, FmIndVida, FmDirActual, FmTelef, FmOcupacion, FmBenefBeca, FmPrcntSeg, FmIndR
                    FROM {BaseDatosOrigen}.dbo.Familiares e
                    INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet  AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente Familiares {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaAcumuladoA_DIMM(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.AcumuladoA_DIMM
                SELECT Clk, mc.ClkDetDestino AS ClkDet, DIMM
                FROM {BaseDatosOrigen}.dbo.AcumuladoA_DIMM e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente AcumuladoA_DIMM {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaFUMP(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.FUMP
                SELECT
	                ClkAno, ClkNConsecutivo, mc.ClkDetDestino AS ClkDet, ClkDet_Alt, FuCodMov, FuQnaAp, FuFchIni, FuFchTer, FuRfc, FuNomEmp, FuNomAP, 
                    FuNomAM, FuSexo, FuEstC, FuNivMe, FuFchIIst, FuFchIram, FuNoSegS, FuInsPago, FuCtaBco, FuCurp, FuFchNac, FuLugNac, FuNcnldad, FuHrTrab, FuCalle, 
                    FuColFr, FuPobl, FuCPost, FuTelPar, FuVCtrab, FuCtrab, FuCTrabDist, FuVPuesto, FuPuesto, FuGpoPto, FuNumPto, FuVPClvPag, FuClvPag, FuPtoEdo, FuPtoFun, 
	                FuUAdmva, FuIMando, FuTmbc, FuNivel, FuRegimen, FuRango, FuJrnda, FuPrcPt, FuARfcO, FuANomEmp, FuANomAP, FuANomAM, FuMotB, FuFchBaj, FuAVCtrab, 
                    FuACtrab, FuACTrabDist, FuAvPuesto, FuAPuesto, FuAGpoPto, FuANumPto, FuAVPClvPag, FuAClvPag, FuAPtoEdo, FuAPtoFun, FuAUAdmva, FuAIMando, FuATmbc, FuANivel, 
                    FuARegimen, FuARango, FuAJrnda, FuAPrcPt, FuPDAA, FuSdoMen, FuTrV, FuPercep, FuObserv, FuUsrCp, FuFchCp, FuHraCp, FuUsrCc, FuFchCc, FuHraCc, FuIndR, FuNoExt, 
                    FuNoInt, FuCLABE, FuDatCom
                FROM {BaseDatosOrigen}.dbo.    FUMP e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet  AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente FUMP {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaProducto_Detalle(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Producto_Detalle
                SELECT
                    ClkPr, mc.ClkDetDestino AS ClkDet, ClkSeqE, PrCCont, PrInsPago, PrCtaBnco, 
                    PrMotC, PrEdMpi, PrVCTrab, PrCtrab, PrCTrabDist, PrTabPt, PrVPuesto, PrPuesto, PrGpoPto, PrNumPto,
                    PrVPClvPag, PrClvPag, PrPtoEdo, PrPtoFun, PrUAdmva, PrIMando, PrTmbc, PrNivel, PrRegimen, PrRango,
                    PrJrnda, PrPrcPt, PrPPag, PrTPPag, PrNmCheq, PrDvCheq, PrTPer, PrTDed, PrNeto, PrTNPPE, PrEClvPag_Ant,
                    PrNmCheq_Ant, PrStatus, PrIndR, PrVerKey, PrUUID, PrXML, PrCBB, PrFchCanx, PrHraCanx, PrFchEmision, PrHraEmision,
                    PrFchTimb, PrHraTimb, PrCadenaOrig, PrIndEC, PrCLABE
                FROM {BaseDatosOrigen}.dbo.Producto_Detalle e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente Producto_Detalle {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaPlantilla_Detalle(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Plantilla_Detalle 
                SELECT 
	                ClkGpoPto, ClkNumPto, mc.ClkDetDestino AS ClkDet, PpNoEmpP, PpNoEmpS, PpEdMpi, PpVCTrab, PpCTrab, PpCTrabDist, 
                    PpTabPt, PpVPuesto, PpPuesto, PpVPClvPag, PpClvPag, PpPtoEdo, PpPtoFun, PpUAdmva, PpIMando, PpTmbc, PpNivel, PpRegimen,
                    PpRango, PpJrnda, PpPrcPt, PpEfDel, PpEfAl, PpICrn, PpCndV, PpIndR
                FROM {BaseDatosOrigen}.dbo.Plantilla_Detalle ptINNER 
                JOIN {BaseDatosDestino}.dbo.Empleado e ON e.MeGpoPto COLLATE Modern_Spanish_CI_AS = ClkGpoPto COLLATE Modern_Spanish_CI_AS AND e.MeNumPto COLLATE Modern_Spanish_CI_AS = ClkNumPto  
                JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetDestino = e.ClkDet    AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente Plantilla_Detalle {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaPerDed_Producto(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.PerDed_Producto
                SELECT
                     ClkPr, mc.ClkDetDestino AS ClkDet, ClkSeqE, ClkSeq, PrPDTipo, PrPDClave, PrPDImporte, PrPDParA
                FROM {BaseDatosOrigen}.dbo.PerDed_Producto e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND  mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente PerDed_Producto {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaAcumulado_Anual(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Acumulado_Anual
                SELECT
	                ClkAA,  mc.ClkDetDestino AS ClkDet, AaEdMpi, AaVCTrab, AaCTrab, AaCTrabDist, AaTabPt, AaVPuesto, AaPuesto, AaGpoPto,
                    AaNumPto, AaVPClvPag, AaClvPag, AaPtoEdo, AaPtoFun, AaUAdmva, AaIMando, AaTmbc, AaNivel, AaRegimen, AaAMA, 
	                AaAMB, AaSMD, AaZona, AaNQnas, AaTPGr, AaTPEx, AaTCmp, AaTIsr, AaTCSP, AaTORein, AaTIsrOR, AaTOPer, AaTIsrOP,
                    AaTingP, AaIsrTN, AaPerB, AaIngE, AaIAcu, AaINSubs, AaISubs, AaIsrAcr, AaIsrTot, AaCsTot, AaGrav, 
	                AaExen, AaComp, AaIsr, AaCSP, AaQnas, AaIPer, AaIndR, AaTCSA, AaTPSGrav, AaTPSExen, AaTDevIsr, AaCSA, AaSecInfDecl
                FROM {BaseDatosOrigen}.dbo.Acumulado_Anual e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente Acumulado_Anual {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaMovimiento_Nomina(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Movimiento_Nomina
                SELECT
                    ClkMv, MvNLote, MvNDocto, mc.ClkDetDestino AS ClkDet, ClkDet_Alt, MvCoDMov, MvTpMovs, MvFchCp, MvFchVl, MvFchAf, 
                    MvClRech, MVCnRech, MvQnaAp, MvFchInI, MvFchTer, MvQnaIni, MvQnaTer, MvIndPr, MvRfc, MvRfcN, 
                    MvNomEmp, MvNomAP, MvNomAM, MvSexo, MvEstC, MvNivME, MvFchIIst, MvFchIRam, MvNoSegS, MvCCont, MvInsPago, MvCtaBnco,
                    MvCurp, MvFchNac, MvLugNac, MvNcnldad, MvClvTDP, MvDocPrb, MvFlDPrb, 
                    MvHrTrab, MvCalle, MvColFr, MvPobl, MvCPost, MvDomOf, MvTelPar, MvTelOf, MvNoAhisa, MvNoAsemex, MVNoIssste, MvNoPCp, 
                    MvAVCTrab, MvACtrab, MvAVPuesto, MvAPuesto, MvAGpoPto, MvANumPto, MvAVPClvPag, 
                    MvAClvPag, MvVCTrab, MvCtrab, MvCTrabDist, MvVPuesto, MvPuesto, MvGpoPto, MvNumPto, MvVPClvPag, MvClvPag, MvPtoEdo, 
                    MvPtoFun, MvUAdmva, MvTmbc, MvRango, MvJrnda, MvPDAA, MvSdoMen, MvDias, MvHrExt, 
                    MvDsGdsF, MvNDias, MvPrcn, MvDLabAno, MvDLabAnoA, MvDLabIst, MvDLabRam, MvDLicMed, MvDLicSGS, MvDLicCGS, MvTrv, MvTPLM,
                    MvUsrCp, MvFchCpLM, MvHraCp, MvUsrCc, MvFchCc, MvHraCc, MvOrigen, 
                    MvIndR, MvObs, MvNoExt, MvNoInt, MvCLABE, MvDatCom

                FROM {BaseDatosOrigen}.dbo.Movimiento_Nomina e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente Movimiento_Nomina {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaPerDed_AcumuladoAnual(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.PerDed_AcumuladoAnual
                SELECT
                    ClkAA, mc.ClkDetDestino AS ClkDet, ClkSeq, AaPDTipo, AaPDClave, AaPDImporte, AaPDGravable, AaPDExento, AaPDParA
                FROM {BaseDatosOrigen}.dbo.PerDed_AcumuladoAnual e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente PerDed_AcumuladoAnual {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaPerDed_Empleado(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.PerDed_Empleado
                SELECT
                  mc.ClkDetDestino AS ClkDet, MePDTipo, MePDClave, MePDVParA, MePDVImp, MePDVVigI, MePDVVenc
                FROM {BaseDatosOrigen}.dbo.PerDed_Empleado e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc  ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente PerDed_Empleado {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaPension_Alimenticia(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Pension_Alimenticia
                SELECT
                  mc.ClkDetDestino AS ClkDet, ClkPa, PaNomEmp, PaNomAP, PaNomAM, PaFchA, PaFchB, ClkInstP, PaCtaBnco, PaEdMpi, PaVCTrab, 
                  PaCTrab, PaTabPt, PaCodMov, PaNLote, PaNDocto, PaFchISit, PaICal, PaPrCPa, PaDImpPa, PaDVenPa, PaAImpPA1, PaAVenPa1, 
                  PaAImpPa2, PaAVenPa2, PaAImpPa3, PaAVenPa3, PaImpOPPa, PaImpRsPa, PaImpRsVn, PaOCnC, PaDIOCPa, PaAqP, PaUsrCap, PaFchCap, PaHrsCap, PaIndR, PaCLABE
                FROM {BaseDatosOrigen}.dbo.Pension_Alimenticia e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente Pension_Alimenticia {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaSarFovissste_Extraord(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.SarFovissste_Extraord
                SELECT
                  ClkSF, mc.ClkDetDestino AS ClkDet, ClkSeqE, SfeRefPr, SfeABim, SfeSdoBCotizI, SfeSdoBCotiz, SfeSdoBCotizV, SfeSdoIntegr, 
                  SfePagoConvI, SfePagoConvF, SfeDiasCotizB, SfeIndCFov, SfePagoConvS, SfeIndAhorroS,SfeImpAhorroS
                FROM {BaseDatosOrigen}.dbo.SarFovissste_Extraord e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente SarFovissste_Extraord {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaPerDedExt_Empleado(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.PerDedExt_Empleado
                SELECT
                  mc.ClkDetDestino AS ClkDet, ClkPyD, MeFDel, MeBaseAp, MeTpoCal, MePerio, MeImporte
                FROM {BaseDatosOrigen}.dbo.PerDedExt_Empleado e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente PerDedExt_Empleado {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaEmpleado_Generales(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Empleado_Generales
                SELECT 
                    mc.ClkDetDestino AS ClkDet, MeNoSegS, MeCurp, MeFchNac, ClkLN, MePoblNac, MePaisNac, ClkNac, MeClvTDP, MeDocPrb, MeFlDPrb, MeICurp, MeFch_Renapo, 
                    MeFol_Renapo, MeInd_Renapo, MeFchOE_Renapo, MeNumOE_Renapo, MeInd_Mov, MeCalle, MeColFr, MePobl, ClkCodP, MeTelPar, MeTelCel, MeBiper, MePin, MeMail, 
                    MeDomOf, MeTelOf, MeNumCredAcceso, MeDoctosPers, MeNoCartSMN, MeNoCredElect, MeNoPasaporte, MeNoLicManejo, MeTpLicManejo, MeFVLicManejo, MeNoCedProf, 
                    MeReligion, MeNombreAvAccidente, MeTelefAvAccidente, MeClkDetJI, ClkNME, MeIdiomas, MeRPNombre1, MeRPDirecc1, MeRPTelef1, MeRPNombre2, MeRPDirecc2, 
                    MeRPTelef2, MeUltimaEmpInst, MeNombreRecomendo, MeOTrab, MeMTran, MeTUtil, MeCapct, MeTCapct1, MeTCapct2, MeTCapct3, MeTCapct4, MeTHab, MeSHab, MeMRMH, 
                    MeUCFovi, MeFUFovi, MeFPto, MePFPto, MeTFunD, MeIOyET, MeComent, MeNoAhisa, MeNoAsemex, MeNoIssste, MeNoPCP, MeFchPASAR, MeFchUASAR, MeBcoAdmSAR, 
                    MePlzLocSAR, MeCTratoSAR, MeIndAfore, MeIndTPISSSTE, MeNoExt, MeNoInt, MeSATTipoContr, MeSATTipoJrnda, MeSATTipoRegim,
                    MeSATCPost, MeSATNomEmp, MeSATRegimen
                FROM     {BaseDatosOrigen}.dbo.Empleado_Generales as e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente Empleado_Generales {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string moverTablaHistorico_Movimiento(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	INSERT INTO {BaseDatosDestino}.dbo.Historico_Movimiento
                SELECT
                    mc.ClkDetDestino AS ClkDet, ClkNSeq, HmRfcA, HmCodMov, HmNLote, HmNDocto, HmQnaAp, HmFchIni, HmFchTer, HmFntReg, HmEdMpi, HmVCTrab,
                    HmCTrab, HmCTrabDist, HmTabPt, HmVPuesto, HmPuesto, HmGpoPto, HmNumPto, HmVPClvPag, HmClvPag, HmPtoEdo, 
                    HmPtoFun, HmUAdmva, HmIMando, HmTmbc, HmNivel, HmRegimen, HmRango, HmJrnda, HmPrcPt, HmEfDel, HmEfAl, HmPDAa, 
                    HmICong, HmDsGdsF, HmDLabIst, HmDLabRam, HmDLicMed, HmDLicMedAnt, HmDLicSGS, HmDLicCGS, HmDias, HmHrExt, HmHrExtTr,
                    HmTpFlt, HmNDias, HmPrcn, HmPDAA_Mov, HmTrv_Mov, HmSdoMen_Mov, HmDsGdsF_Mov, HmObs, HmUsrCp, HmFchCp, HmHraCp, HmUsrCc,
                    HmFchCc, HmHraCc, HmOrigen, HmIndR, HmDatCom 
                FROM {BaseDatosOrigen}.dbo.Historico_Movimiento e
                INNER JOIN {BaseDatosDestino}.dbo._MigracionControl mc ON mc.ClkDetOrigen = e.ClkDet  AND mc.BDOrigen = '{BaseDatosOrigen}' AND mc.Estatus = 'PENDIENTE';
            ";

            string Mensaje = $"Migración creada correctamente Historico_Movimiento {BaseDatosOrigen} a {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }
        private string CambiarEstatus(string BaseDatosOrigen, string BaseDatosDestino) {
            string query = @$"	UPDATE MC SET Estatus = 'MIGRADO'  
                FROM  {BaseDatosDestino}.dbo._MigracionControl MC
                WHERE Estatus = 'PENDIENTE'
            ";

            string Mensaje = $"Migración creada correctamente  {BaseDatosDestino}";
            return Consulta(query, BaseDatosDestino, Mensaje);
        }

        private string Consulta(string query, string bd, string mensaje) {
            try {
                Global global = new Global(_coneccionService);
                global.ConsultaGeneral(query, bd);
                return $"OK: {mensaje}\n";
            }
            catch (Exception ex) {
                return $"ERROR: {mensaje}\n{ex.Message}";
            }
        }
    }
}
