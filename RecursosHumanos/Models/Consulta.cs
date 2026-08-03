using AspNetCoreGeneratedDocument;

namespace RecursosHumanos.Models {
    public class Consulta {
        private int? _Id;
        private string _Nombre;
        private string _Sql;
        private short? _Activo;
        private DateTime _Creado;
        private DateTime? _Editado;
        private DateTime? _Eliminado;

        public int? Id { get { return _Id; } set { _Id = value; } }
        public string Nombre { get { return _Nombre; } set { _Nombre = value; } }
        public string Sql { get { return _Sql; } set { _Sql = value; } }
        public short? Activo { get { return _Activo; } set { _Activo = value; } }
        public DateTime Creado { get { return _Creado; } set { _Creado = value; } }
        public DateTime? Editado { get { return _Editado; } set { _Editado = value; } }
        public DateTime? Eliminado { get { return _Eliminado; } set { _Eliminado = value; } }
    }
}
