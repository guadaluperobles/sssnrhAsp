namespace RecursosHumanos.Models {
    public class LayoutSERICA {
        private string? _RFC;
        private string? _CURP;
        private string? _NombreEmpleado;
        private string? _PrimerApellido;
        private string? _SegundoApellido;
        private string? _SeguridadSocial;
        private string? _Pagaduria;
        private string? _NumeroISSSTE;
        private string? _NumeroPrestamo;
        private string? _Plazo;
        private string? _QuincenaInicial;
        private string? _AnioInicial;
        private string? _QuincenaFinal;
        private string? _AnioFinal;
        private string? _QuincenaEnvio;
        private string? _AnioEnvio;

        public string RFC { get { return _RFC; } set { _RFC = value; } }
        public string CURP { get { return _CURP; } set { _CURP = value; } }
        public string NombreEmpleado { get { return _NombreEmpleado; } set { _NombreEmpleado = value; } }
        public string PrimerApellido { get { return _PrimerApellido; } set { _PrimerApellido = value; } }
        public string SegundoApellido { get { return _SegundoApellido; } set { _SegundoApellido = value; } }
        public string SeguridadSocial { get { return _SeguridadSocial; } set { _SeguridadSocial = value; } }
        public string Pagaduria { get { return _Pagaduria; } set { _Pagaduria = value; } }
        public string NumeroISSSTE { get { return _NumeroISSSTE; } set { _NumeroISSSTE = value; } }
        public string NumeroPrestamo { get { return _NumeroPrestamo; } set { _NumeroPrestamo = value; } }
        public string? Plazo { get { return _Plazo; } set { _Plazo = value; } }
        public string? QuincenaInicial { get { return _QuincenaInicial; } set { _QuincenaInicial = value; } }
        public string? AnioInicial { get { return _AnioInicial; } set { _AnioInicial = value; } }
        public string? QuincenaFinal { get { return _QuincenaFinal; } set { _QuincenaFinal = value; } }
        public string? AnioFinal { get { return _AnioFinal; } set { _AnioFinal = value; } }
        public string? QuincenaEnvio { get { return _QuincenaEnvio; } set { _QuincenaEnvio = value; } }
        public string? AnioEnvio { get { return _AnioEnvio; } set { _AnioEnvio = value; } }
    }
}