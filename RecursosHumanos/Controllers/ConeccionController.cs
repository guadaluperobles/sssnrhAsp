using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace RecursosHumanos.Controllers
{
    public class ConeccionController : Controller
    {
        // GET: ConeccionController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ConeccionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ConeccionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ConeccionController/Create
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

        // GET: ConeccionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ConeccionController/Edit/5
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

        // GET: ConeccionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ConeccionController/Delete/5
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

        public DataTable CargarBasesDatos()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("BaseDatos");
            dt.Rows.Add(DBNull.Value);

            string locCadenaConeccion = cadenaConeccion.CadenaConeccion + "Database=IESYST;";

            using (SqlConnection conexion = new SqlConnection(locCadenaConeccion))
            {
                try
                {
                    conexion.Open();

                    string consulta = cadenaConeccion.ConsultaBasesDatosOperativas;

                    SqlDataAdapter da = new SqlDataAdapter(consulta, locCadenaConeccion);

                    da.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {

                    ArchivoController.logErrors(ex, "ReciboNominaController 35");

                    Console.WriteLine("----------------------------------");
                    Console.WriteLine(locCadenaConeccion);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("----------------------------------");
                    throw;
                }
            }
        }
    }
}
