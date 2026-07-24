using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.Controllers {
    public class BuscadorController : Controller {
        // GET: BuscadorController
        public ActionResult Index() {
            return View();
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
