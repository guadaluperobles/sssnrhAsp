namespace RecursosHumanos.Models {
    public class PermisoVistaModel {
        private int? _Id;
        private string _Nombre;
        private string _Controller;
        private string _Action;
        private string _Descripcion;
        private short? _Activo;
        private DateTime _Creado;
        private DateTime? _Editado;
        private DateTime? _Eliminado;

        public int? Id { get { return _Id; } set { _Id = value; } }
        public string Nombre { get { return _Nombre; } set { _Nombre = value; } }
        public string Controller { get { return _Controller; } set { _Controller = value; } }
        public string Action { get { return _Action; } set { _Action = value; } }
        public string Descripcion { get { return _Descripcion; } set { _Descripcion = value; } }
        public short? Activo { get { return _Activo; } set { _Activo = value; } }
        public DateTime Creado { get { return _Creado; } set { _Creado = value; } }
        public DateTime? Editado { get { return _Editado; } set { _Editado = value; } }
        public DateTime? Eliminado { get { return _Eliminado; } set { _Eliminado = value; } }
    }
}
