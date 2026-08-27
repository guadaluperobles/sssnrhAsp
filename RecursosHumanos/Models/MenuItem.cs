namespace RecursosHumanos.Models {
    public class MenuItem {
        public string Vista { get; set; }
        public string Controlador { get; set; }
        public string Accion { get; set; }
        public List<MenuItem> SubModulos { get; set; } = new();
    }
}
