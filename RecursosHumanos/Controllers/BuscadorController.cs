using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Models;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using RecursosHumanos.Model;
using RecursosHumanos.ConsultaViewModel;

namespace RecursosHumanos.Controllers {
    public class BuscadorController : Controller {
        private readonly ConeccionService _coneccionService;
        private readonly IWebHostEnvironment _env;

        public BuscadorController(ConeccionService coneccionService) {
            _coneccionService = coneccionService;
        }
        // GET: BuscadorController
        public ActionResult Index() {
            var model = new EmpleadosConsulta {
                Empleados = new CustomTable {
                    Datos = null,
                },
                Localizar = ""
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Index(string localizar) {
            Global global = new Global(_coneccionService);
            string texto = (localizar ?? "").Replace(" ", "");
            string consulta = $"{ConsultasModel.BuscarEmpleado} LIKE '%{texto}%'";
            DataTable Empleados = global.ConsultaGeneral(consulta);

            var modelo = new CustomTable {
                Datos = Empleados.Rows.Cast<DataRow>(),
                Columnas = Global.GenerarColumnas(
                    Empleados,
                    new Dictionary<string, (string Titulo, bool PK)>
                    {
                        { "ClkDet", ("Num Emp", true) },
                        { "BaseDatos", ("BD", false) },
                        { "NombreCompleto", ("Empleado" , false) },
                        { "MeRfc", ("RFC", false) }
                    })
            };

            var model = new EmpleadosConsulta {
                //BasesDatos = _coneccionService.CargarBasesDatosOperativas(),
                Empleados = modelo,
                Localizar = localizar
            };

            return View("Index", model);
        }
        // GET: BuscadorController/Details/5
        public ActionResult Details(int id) {
            return View();
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