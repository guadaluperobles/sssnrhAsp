using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.Models {
    public class Nota : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
