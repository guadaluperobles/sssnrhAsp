using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.Controllers {
    public class TransparenciaVIIController : Controller {
        // GET: TransparenciaVIIController
        public ActionResult Index() {
            return View();
        }

        // GET: TransparenciaVIIController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: TransparenciaVIIController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: TransparenciaVIIController/Create
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

        // GET: TransparenciaVIIController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: TransparenciaVIIController/Edit/5
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

        // GET: TransparenciaVIIController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: TransparenciaVIIController/Delete/5
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