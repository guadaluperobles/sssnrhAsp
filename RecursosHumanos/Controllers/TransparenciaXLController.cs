using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.Controllers {
    public class TransparenciaXLController : Controller {
        // GET: TransparenciaXLController
        public ActionResult Index() {
            return View();
        }

        // GET: TransparenciaXLController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: TransparenciaXLController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: TransparenciaXLController/Create
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

        // GET: TransparenciaXLController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: TransparenciaXLController/Edit/5
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

        // GET: TransparenciaXLController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: TransparenciaXLController/Delete/5
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
