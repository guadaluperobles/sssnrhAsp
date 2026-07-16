using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.Controllers
{
    public class TransparenciaController : Controller
    {
        // GET: TransparenciaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TransparenciaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TransparenciaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TransparenciaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TransparenciaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TransparenciaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TransparenciaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TransparenciaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
