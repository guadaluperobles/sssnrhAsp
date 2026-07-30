using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Models;

namespace RecursosHumanos.Controllers {
    public class BuscadorController : Controller {
        // GET: BuscadorController
        public ActionResult Index() {

            var modelo = new CustomTable {
                Datos = new List<Cheque>(),
                Columnas = new List<CustomTableColumn> {
                    new CustomTableColumn {
                        Propiedad = "Ejercicio",
                        Titulo = "Num Emp"
                    },
                    new CustomTableColumn {
                        Propiedad = "Quincena",
                        Titulo = "RFC"
                    },
                    new CustomTableColumn {
                        Propiedad = "NumeroCheque" ,
                        Titulo = "CURP"
                    },
                    new CustomTableColumn {
                        Propiedad = "FechaPago" ,
                        Titulo = "Nombre"
                    },
                    new CustomTableColumn {
                        Propiedad = "ClkDet" ,
                        Titulo = "Estatus"
                    },
                }
            };
            return View(modelo);
        }

        // GET: BuscadorController/Details/5
        public ActionResult Details(int id) {
            return View();
        }
        public 
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
