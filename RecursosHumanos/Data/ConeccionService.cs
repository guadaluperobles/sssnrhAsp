using Microsoft.Data.SqlClient;
using System.Data;

namespace RecursosHumanos.Data {
    public class ConeccionService {
        private readonly IConfiguration _configuration;
        public ConeccionService(IConfiguration configuration) {
            _configuration = configuration;
        }
        public DataTable CargarBasesDatos() {
            DataTable dt = new DataTable();
            string cadena = _configuration.GetConnectionString("DefaultConnection");
            using SqlConnection conexion = new SqlConnection(cadena);
            conexion.Open();
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT name AS BaseDatos FROM sys.databases WHERE state = 0 ORDER BY name",
                conexion);

            da.Fill(dt);
            return dt;
        }
        public DataTable CargarBasesDatosOperativas() {
            DataTable dt = new DataTable();
            string cadena = _configuration.GetConnectionString("IESYST");
            using SqlConnection conexion = new SqlConnection(cadena);
            conexion.Open();
            SqlDataAdapter da = new SqlDataAdapter(@"SELECT * FROM BDOperativas WHERE NumEmpIni > 0 ORDER BY Descripcion", conexion);
            da.Fill(dt);
            return dt;
        }
        public DataTable EjecutarConsulta(string nombreConexion, string consulta) {
            string cadena = _configuration.GetConnectionString(nombreConexion);
            using SqlConnection conexion = new SqlConnection(cadena);
            conexion.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
            da.Fill(dt);
            return dt;
        }
    }
}
