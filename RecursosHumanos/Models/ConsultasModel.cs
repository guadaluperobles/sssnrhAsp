namespace RecursosHumanos.Models {
    public class ConsultasModel {
        public static string ConsultaCFDI = @"
            SELECT
                pc.PrAno,
                RIGHT('0' + CAST(pc.PrQna AS VARCHAR), 2) AS PrQna,
                pd.ClkPr,
                pd.PrNeto,
                pd.PrUUID,
                pd.PrXML,
                CONCAT(emp.MeRfc, CAST(pd.ClkDet AS VARCHAR)) AS busqueda
            FROM Producto_Detalle AS pd
            INNER JOIN Producto_Control AS pc ON pd.ClkPr = pc.ClkPr
            INNER JOIN Empleado AS emp  ON pd.ClkDet = emp.ClkDet
";
    }
}
