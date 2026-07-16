using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecursosHumanos.Data;
using System.Data;

namespace RecursosHumanos.Controllers
{
    public class ReciboNominaController : Controller
    {

        private readonly ConeccionService _coneccionService;

        public ReciboNominaController(ConeccionService coneccionService) {
            _coneccionService = coneccionService;
        }
        // GET: ReciboNominaController
        public ActionResult Index()
        {
            DataTable BasesDatos = _coneccionService.CargarBasesDatosOperativas();

            return View(BasesDatos);
        }

        // GET: ReciboNominaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReciboNominaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReciboNominaController/Create
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

        // GET: ReciboNominaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReciboNominaController/Edit/5
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

        // GET: ReciboNominaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReciboNominaController/Delete/5
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
