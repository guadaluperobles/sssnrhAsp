namespace RecursosHumanos.Models {
    public class Periodo {
        private int _Id;
        private string _Descripcion;
        private string _Quincenas;
        private string _Inicio;
        private string _Fin;
        private string _Pago;
        private string _Mes;
        public int Id { get { return _Id; } set { _Id = value; } }
        public string Descripcion { get { return _Descripcion; } set { _Descripcion = value; } }
        public string Quincenas { get { return _Quincenas; } set { _Quincenas = value; } }
        public string Inicio { get { return _Inicio; } set { _Inicio = value; } }
        public string Fin { get { return _Fin; } set { _Fin = value; } }
        public string Pago { get { return _Pago; } set { _Pago = value; } }
        public string Mes { get { return _Mes; } set { _Mes = value; } }

        public List<Periodo> trimestres() {
            List<Periodo> Per = new List<Periodo> {
                new Periodo { Id = 1, Descripcion = "1 Trimestre (Enero, Febrero, Marzo)" ,         Quincenas = "1,2,3,4,5,6"},
                new Periodo { Id = 2, Descripcion = "2 Trimestre (Abril, Mayo, Junio)" ,            Quincenas = "7,8,9,10,11,12"},
                new Periodo { Id = 3, Descripcion = "3 Trimestre (Julio, Agosto, Septiembre)",      Quincenas = "13,14,15,16,17,18"},
                new Periodo { Id = 4, Descripcion = "4 Trimestre (Octubre, Noviembre, Diciembre)",  Quincenas = "19,20,21,22,23,24" }
            };
            return Per;
        }
        public List<Periodo> cuatrimestres() {
            List<Periodo> Per = new List<Periodo> {
                new Periodo { Id = 1, Descripcion = "1 Cuatrimestre (Enero, Febrero, Marzo, Abril)",                Quincenas = "1,2,3,4,5,6,7,8" },
                new Periodo { Id = 2, Descripcion = "2 Cuatrimestre (Mayo, Junio, Julio, Agosto)",                  Quincenas = "9,10,11,12,13,14,15,16" },
                new Periodo { Id = 3, Descripcion = "3 Cuatrimestre (Septiembre, Octubre, Noviembre, Diciembre)",   Quincenas = "17,18,19,20,21,22,23,24" }
            };
            return Per;
        }

        public List<Periodo> mensual() {

            List<Periodo> Per = new List<Periodo> {
                new Periodo { Id = 1, Descripcion = "Enero",        Quincenas = "1,2" },
                new Periodo { Id = 2, Descripcion = "Febrero",      Quincenas = "3,4" },
                new Periodo { Id = 3, Descripcion = "Marzo",        Quincenas = "5,6" },
                new Periodo { Id = 4, Descripcion = "Abril",        Quincenas = "7,8" },
                new Periodo { Id = 5, Descripcion = "Mayo",         Quincenas = "9,10" },
                new Periodo { Id = 6, Descripcion = "Junio",        Quincenas = "11,12" },
                new Periodo { Id = 7, Descripcion = "Julio",        Quincenas = "13,14" },
                new Periodo { Id = 8, Descripcion = "Agosto",       Quincenas = "15,16" },
                new Periodo { Id = 9, Descripcion = "Septiembre",   Quincenas = "17,18" },
                new Periodo { Id = 10, Descripcion = "Octubre",     Quincenas = "19,20" },
                new Periodo { Id = 11, Descripcion = "Noviembre",   Quincenas = "21,22" },
                new Periodo { Id = 12, Descripcion = "Diciembre",   Quincenas = "23,24" }
            };
            return Per;
        }
        public List<Periodo> bimestre() {

            List<Periodo> Per = new List<Periodo> {
                new Periodo { Id = 1, Descripcion = "Enero - Febrero",          Quincenas = "1,2,3,4"       },
                new Periodo { Id = 2, Descripcion = "Marzo - Abril",            Quincenas = "5,6,7,8"       },
                new Periodo { Id = 3, Descripcion = "Mayo - Junio",             Quincenas = "9,10,11,12"    },
                new Periodo { Id = 4, Descripcion = "Julio - Agosto",           Quincenas = "13,14,15,16"   },
                new Periodo { Id = 5, Descripcion = "Septiembre - Octubre",     Quincenas = "17,18,19,20"   },
                new Periodo { Id = 6, Descripcion = "Noviembre - Diciembre",    Quincenas = "21,22,23,24"   }
            };
            return Per;
        }
        public List<Periodo> quincena() {

            List<Periodo> Per = new List<Periodo> {
                new Periodo { Id = 1,  Descripcion = "Enero",        Quincenas = "1",        Inicio = "1",  Mes = "01" },
                new Periodo { Id = 2,  Descripcion = "Enero",        Quincenas = "2",        Inicio = "2",  Mes = "01" },
                new Periodo { Id = 3,  Descripcion = "Febrero",      Quincenas = "3",        Inicio = "1",  Mes = "02" },
                new Periodo { Id = 4,  Descripcion = "Febrero",      Quincenas = "4",        Inicio = "2",  Mes = "02" },
                new Periodo { Id = 5,  Descripcion = "Marzo",        Quincenas = "5",        Inicio = "1",  Mes = "03" },
                new Periodo { Id = 6,  Descripcion = "Marzo",        Quincenas = "6",        Inicio = "2",  Mes = "03" },
                new Periodo { Id = 7,  Descripcion = "Abril",        Quincenas = "7",        Inicio = "1",  Mes = "04" },
                new Periodo { Id = 8,  Descripcion = "Abril",        Quincenas = "8",        Inicio = "2",  Mes = "04" },
                new Periodo { Id = 9,  Descripcion = "Mayo",         Quincenas = "9",        Inicio = "1",  Mes = "05" },
                new Periodo { Id = 10, Descripcion = "Mayo",         Quincenas = "10",       Inicio = "2",  Mes = "05" },
                new Periodo { Id = 11, Descripcion = "Junio",        Quincenas = "11",       Inicio = "1",  Mes = "06" },
                new Periodo { Id = 12, Descripcion = "Junio",        Quincenas = "12",       Inicio = "2",  Mes = "06" },
                new Periodo { Id = 13, Descripcion = "Julio",        Quincenas = "13",       Inicio = "1",  Mes = "07" },
                new Periodo { Id = 14, Descripcion = "Julio",        Quincenas = "14",       Inicio = "2",  Mes = "07" },
                new Periodo { Id = 15, Descripcion = "Agosto",       Quincenas = "15",       Inicio = "1",  Mes = "08" },
                new Periodo { Id = 16, Descripcion = "Agosto",       Quincenas = "16",       Inicio = "2",  Mes = "08" },
                new Periodo { Id = 17, Descripcion = "Septiembre",   Quincenas = "17",       Inicio = "1",  Mes = "09" },
                new Periodo { Id = 18, Descripcion = "Septiembre",   Quincenas = "18",       Inicio = "2",  Mes = "09" },
                new Periodo { Id = 19, Descripcion = "Octubre",      Quincenas = "19",       Inicio = "1",  Mes = "10" },
                new Periodo { Id = 20, Descripcion = "Octubre",      Quincenas = "20",       Inicio = "2",  Mes = "10" },
                new Periodo { Id = 21, Descripcion = "Noviembre",    Quincenas = "21",       Inicio = "1",  Mes = "11" },
                new Periodo { Id = 22, Descripcion = "Noviembre",    Quincenas = "22",       Inicio = "2",  Mes = "11" },
                new Periodo { Id = 23, Descripcion = "Diciembre",    Quincenas = "23",       Inicio = "1",  Mes = "12" },
                new Periodo { Id = 24, Descripcion = "Diciembre",    Quincenas = "24",       Inicio = "2",  Mes = "12" }
            };
            return Per;
        }
    }
}
