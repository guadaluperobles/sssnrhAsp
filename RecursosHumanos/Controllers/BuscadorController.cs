using GeneradorRecursosHumanos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Models;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace RecursosHumanos.Controllers {
    public class BuscadorController : Controller {
        private readonly ConeccionService _coneccionService;
        private readonly IWebHostEnvironment _env;

        public BuscadorController(ConeccionService coneccionService) {
            _coneccionService = coneccionService;
        }
        // GET: BuscadorController
        public ActionResult Index() {

            var modelo = new CustomTable {
                Datos = null,
                Columnas = new List<CustomTableColumn> {
                 new CustomTableColumn {
                    Propiedad = "ClkDet",
                    Titulo = "Num Emp"
                },
                new CustomTableColumn {
                    Propiedad = "MeRfc",
                    Titulo = "RFC"
                },
                /*new CustomTableColumn {
                    Propiedad = "NumeroCheque" ,
                    Titulo = "CURP"
                },*/
                new CustomTableColumn {
                    Propiedad = "MeNomEmp" ,
                    Titulo = "Nombre"
                },
                /*new CustomTableColumn {
                    Propiedad = "ClkDet" ,
                    Titulo = "Estatus"
                }*/
            }
            };
            //t	MeRfc	MeNomAP	MeNomAM	MeNomEmp	MeSexo	ClkEstC	MeFechIIst	MeFechIRam	MeCCont	ClkInstP	MeCtaBnco	MeHrTrab	MeEdMpi	MeVCTrab	MeCTrab	MeCTrabDist	MeTabPt	MeVPuesto	MePuesto	MeGpoPto	MeNumPto	MeVPClvPag	MeClvPag	MePtoEdo	MePtoFun	MeUAdmva	MeIMando	MeTmbc	MeNivel	MeRegimen	MeRango	MeJrnda	MePrcPt	MeEfDel	MeEfAl	ClkMov	MeNLote	MeNDocto	MeFchISit	MeFchTSit	MePDAA	MeIPal	MeIPPVAno	MeIPPVAnoA	MeICong	MeIRetr	MeIResp	MeDLabAno	MeDLabAnoA	MeDLabIst	MeDLabRam	MeDLicMed	MeDLicMedAnt	MeDLicSGS	MeDLicCGS	MeDsFlt	MeDsFltA	MeDsGdsF	MePerGr	MeDed01	MePerSB	MeSdoDI	MeNetLG	MeSdoLG	MeAcMePer	MeAcMeIsr	MeRespNPApl	MeRespMPApl	MeRespBPApl	MeCMovBP	MeIFnAn	MeTPLM	MeIndMe	MeCLABE	MeIndLS	MeSADImptoA	MePADImptoA	BaseDatos

            var model = new ConsultaEmpleadosViewModel {
                Empleados = modelo,
                Localizar = ""
            };
            return View(model);
        }

        // GET: BuscadorController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        /*
         new List<CustomTableColumn> {
                 new CustomTableColumn {
                    Propiedad = "ClkDet",
                    Titulo = "Num Emp",
                    Visible = true
                },
                new CustomTableColumn {
                    Propiedad = "MeRfc",
                    Titulo = "RFC",
                    Visible = true
                },
                /*new CustomTableColumn {
                    Propiedad = "NumeroCheque" ,
                    Titulo = "CURP"
                },* /
        new CustomTableColumn {
            Propiedad = "MeNomEmp" ,
                    Titulo = "Nombre",
                    Visible = true
                },
                /*new CustomTableColumn {
                    Propiedad = "ClkDet" ,
                    Titulo = "Estatus"
                }* /
         */
        [HttpPost]
        public IActionResult Buscar(string localizar) {

            Global global = new Global(_coneccionService);
            string consulta = $"{ConsultasModel.BuscarEmpleado} LIKE '%{localizar}%'";

            DataTable Empleados = global.ConsultaGeneral(consulta);

            var columnas = Empleados.Columns.Cast<DataColumn>()
                .Select(c => new CustomTableColumn {
                    Propiedad = c.ColumnName,
                    Titulo = c.ColumnName,
                    Visible = false
                })
                .ToList();

            columnas.First(c => c.Propiedad == "BaseDatos").Titulo = "BD";
            columnas.First(c => c.Propiedad == "BaseDatos").Visible = true;

            columnas.First(c => c.Propiedad == "ClkDet").Titulo = "Num Emp";
            columnas.First(c => c.Propiedad == "ClkDet").Visible = true;

            columnas.First(c => c.Propiedad == "NombreCompleto").Titulo = "Empleado";
            columnas.First(c => c.Propiedad == "NombreCompleto").Visible = true;

            columnas.First(c => c.Propiedad == "MeRfc").Titulo = "RFC";
            columnas.First(c => c.Propiedad == "MeRfc").Visible = true;

            var modelo = new CustomTable {
                Datos = Empleados.Rows.Cast<DataRow>(),
                Columnas = columnas
            };

            var model = new ConsultaEmpleadosViewModel {
                //BasesDatos = _coneccionService.CargarBasesDatosOperativas(),
                Empleados = modelo,
                Localizar = localizar
            };

            return View("Index", model);
        }
        // GET: BuscadorController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: BuscadorController/Create
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

        // GET: BuscadorController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: BuscadorController/Edit/5
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

        // GET: BuscadorController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: BuscadorController/Delete/5
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
/*
 UERA	GUZMAN	FRANCISCA ARLENE	M	C	20190316	20190316		27	001009768050		26029	10	2640112941	2640112941	3	7	M03004A	02	000424	6	ACRES0012201M03004A0000400072101		01	JS0400SC0401	60	123	0	0	3	7	100	20220101	20221231	1101	FMP2022	0000427	20220916			0	00	00	0	1	0	0	0	1280	1280	0	0	0	0	0	0	4	7062	240.84	0	0	3410.58	3531	0	0	0	0	0		0		14		0	0	0	IESYS_ACREDITADOS
350686	AUCL6101217BA	ABUNDIS
 */