using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.Models {
    public class EmpleadoModel : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
