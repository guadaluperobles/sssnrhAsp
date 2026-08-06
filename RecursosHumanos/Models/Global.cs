using Microsoft.Data.SqlClient;
using RecursosHumanos.Data;
using RecursosHumanos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursosHumanos.Model {
    public class Global {
        private readonly ConeccionService _coneccionService;

        public Global(ConeccionService coneccionService) {
            _coneccionService = coneccionService;
        }
        public static string Letras(string numero) {
            string palabras = "";
            string entero = "";
            string dec = "";
            string flag = "N";

            numero = numero.Replace(",", "");
            int num, x, y;

            if (numero.Substring(0, 1) == "-") {
                numero = numero.Substring(1);
                palabras = "menos ";
            }

            for (x = 1; x <= numero.Length; x++) {
                if (numero.Substring(0, 1) == "0") {
                    numero = numero.Substring(1).Trim();

                    if (numero.Length == 0)
                        palabras = "";
                }
                else {
                    break;
                }
            }

            // Separar entero y decimal
            for (y = 0; y < numero.Length; y++) {
                if (numero[y] == '.') {
                    flag = "S";
                }
                else {
                    if (flag == "N")
                        entero += numero[y];
                    else
                        dec += numero[y];
                }
            }

            if (dec.Length == 1)
                dec += "0";

            flag = "N";

            if (Convert.ToDecimal(numero) <= 999999999) {
                for (y = entero.Length; y >= 1; y--) {
                    num = entero.Length - (y - 1);

                    switch (y) {
                        case 3:
                        case 6:
                        case 9:
                            switch (entero[num - 1]) {
                                case '1':
                                    if (entero[num] == '0' && entero[num + 1] == '0')
                                        palabras += "cien ";
                                    else
                                        palabras += "ciento ";
                                    break;
                                case '2': palabras += "doscientos "; break;
                                case '3': palabras += "trescientos "; break;
                                case '4': palabras += "cuatrocientos "; break;
                                case '5': palabras += "quinientos "; break;
                                case '6': palabras += "seiscientos "; break;
                                case '7': palabras += "setecientos "; break;
                                case '8': palabras += "ochocientos "; break;
                                case '9': palabras += "novecientos "; break;
                            }
                            break;

                        case 2:
                        case 5:
                        case 8:
                            switch (entero[num - 1]) {
                                case '1':
                                    switch (entero[num]) {
                                        case '0': palabras += "diez "; flag = "S"; break;
                                        case '1': palabras += "once "; flag = "S"; break;
                                        case '2': palabras += "doce "; flag = "S"; break;
                                        case '3': palabras += "trece "; flag = "S"; break;
                                        case '4': palabras += "catorce "; flag = "S"; break;
                                        case '5': palabras += "quince "; flag = "S"; break;
                                        default:
                                            palabras += "dieci";
                                            flag = "N";
                                            break;
                                    }
                                    break;

                                case '2':
                                    if (entero[num] == '0') {
                                        palabras += "veinte ";
                                        flag = "S";
                                    }
                                    else {
                                        palabras += "veinti";
                                        flag = "N";
                                    }
                                    break;

                                case '3':
                                    palabras += entero[num] == '0'
                                        ? "treinta "
                                        : "treinta y ";
                                    flag = entero[num] == '0' ? "S" : "N";
                                    break;

                                case '4':
                                    palabras += entero[num] == '0'
                                        ? "cuarenta "
                                        : "cuarenta y ";
                                    flag = entero[num] == '0' ? "S" : "N";
                                    break;

                                case '5':
                                    palabras += entero[num] == '0'
                                        ? "cincuenta "
                                        : "cincuenta y ";
                                    flag = entero[num] == '0' ? "S" : "N";
                                    break;

                                case '6':
                                    palabras += entero[num] == '0'
                                        ? "sesenta "
                                        : "sesenta y ";
                                    flag = entero[num] == '0' ? "S" : "N";
                                    break;

                                case '7':
                                    palabras += entero[num] == '0'
                                        ? "setenta "
                                        : "setenta y ";
                                    flag = entero[num] == '0' ? "S" : "N";
                                    break;

                                case '8':
                                    palabras += entero[num] == '0'
                                        ? "ochenta "
                                        : "ochenta y ";
                                    flag = entero[num] == '0' ? "S" : "N";
                                    break;

                                case '9':
                                    palabras += entero[num] == '0'
                                        ? "noventa "
                                        : "noventa y ";
                                    flag = entero[num] == '0' ? "S" : "N";
                                    break;
                            }
                            break;

                        case 1:
                        case 4:
                        case 7:
                            switch (entero[num - 1]) {
                                case '1':
                                    if (flag == "N")
                                        palabras += (y == 1) ? "uno " : "un ";
                                    break;
                                case '2':
                                    if (flag == "N") palabras += "dos ";
                                    break;
                                case '3':
                                    if (flag == "N") palabras += "tres ";
                                    break;
                                case '4':
                                    if (flag == "N") palabras += "cuatro ";
                                    break;
                                case '5':
                                    if (flag == "N") palabras += "cinco ";
                                    break;
                                case '6':
                                    if (flag == "N") palabras += "seis ";
                                    break;
                                case '7':
                                    if (flag == "N") palabras += "siete ";
                                    break;
                                case '8':
                                    if (flag == "N") palabras += "ocho ";
                                    break;
                                case '9':
                                    if (flag == "N") palabras += "nueve ";
                                    break;
                            }
                            break;
                    }

                    if (y == 4) {
                        if ((entero.Length >= 6 &&
                            (entero[5] != '0' || entero[4] != '0' || entero[3] != '0'))
                            || entero.Length <= 6) {
                            palabras += "mil ";
                        }
                    }

                    if (y == 7) {
                        if (entero.Length == 7 && entero[0] == '1')
                            palabras += "millón ";
                        else
                            palabras += "millones ";
                    }
                }

                return dec != ""
                    ? (palabras + "pesos " + dec + "/100 M.N.").ToUpper()
                    : (palabras + "pesos 00/100 M.N.").ToUpper();
            }

            return "";
        }
        public static Decimal ObtenerDecimal(string tb) {
            return decimal.Parse(tb, NumberStyles.Any, new CultureInfo("es-MX"));
        }
        public static DateTime ObtenerFecha(string texto) {
            string[] formatos = {
                "dd/MM/yyyy",
                "dd/M/yyyy",
                "dd/MM/yyyy HH:mm:ss",
                "dd/M/yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm:ss.fffffff",
                "dd/M/yyyy HH:mm:ss.fffffff"
            };

            if (DateTime.TryParseExact(
                    texto,
                    formatos,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fecha)) {
                return fecha;
            }

            return DateTime.Now;
        }
        public DataTable ConsultaGeneral(string consulta, string baseDatos = "") {
            DataTable dataTable = new DataTable();
            DataTable BasesDatos = _coneccionService.CargarBasesDatosOperativas();

            if (baseDatos != "") {

                dataTable = _coneccionService.EjecutarConsulta(baseDatos, consulta);

                if (!dataTable.Columns.Contains("BaseDatos")) {
                    DataColumn col = new DataColumn("BaseDatos", typeof(string));
                    col.DefaultValue = baseDatos;
                    dataTable.Columns.Add(col);
                }
            }
            else {
                foreach (DataRow row in BasesDatos.Rows) {
                    string descripcion = row["Descripcion"].ToString();
                    baseDatos = row["BaseDatos"].ToString();

                    if (baseDatos == "" || baseDatos == "LUFEN")
                        continue;

                    DataTable dtTemp = _coneccionService.EjecutarConsulta(baseDatos, consulta);

                    if (dataTable == null) {
                        dataTable = dtTemp.Clone();
                        dataTable.Columns.Add("BaseDatos", typeof(string));
                    }

                    dtTemp.Columns.Add("BaseDatos", typeof(string));

                    foreach (DataRow r in dtTemp.Rows)
                        r["BaseDatos"] = baseDatos;

                    dataTable.Merge(dtTemp);
                }

            }
            return dataTable;
        }

        public static List<CustomTableColumn> GenerarColumnas( DataTable tabla, Dictionary<string, (string Titulo, bool PK)> columnasVisibles) {
            var columnas = tabla.Columns.Cast<DataColumn>().Select(c => new CustomTableColumn {
                    Propiedad = c.ColumnName,
                    Titulo = c.ColumnName,
                    Visible = false
                }).ToList();

            foreach (var columna in columnas) {
                if (columnasVisibles.TryGetValue(columna.Propiedad, out var cfg)) {
                    columna.Titulo = cfg.Titulo;
                    columna.Visible = true;
                    columna.Pk = cfg.PK;
                }
            }

            return columnas;
        }
    }
}
