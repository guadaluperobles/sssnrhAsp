namespace RecursosHumanos.Models {
    public class CustomTableColumn {
        public string Propiedad { get; set; } = "";
        public string Titulo { get; set; } = "";
        public bool Visible { get; set; } = true;
        public bool Pk { get; set; } = false;
        public bool Editar { get; set; } = false;
        public bool Eliminar { get; set; } = false;
    }
}
