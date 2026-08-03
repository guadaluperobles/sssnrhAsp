using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.Controllers {
    public class PlantillaSindicalizadosController : Controller {
        // GET: TransparenciaIXController
        public ActionResult Index() {
            return View();
        }

        // GET: TransparenciaIXController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: TransparenciaIXController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: TransparenciaIXController/Create
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

        // GET: TransparenciaIXController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: TransparenciaIXController/Edit/5
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

        // GET: TransparenciaIXController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: TransparenciaIXController/Delete/5
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
