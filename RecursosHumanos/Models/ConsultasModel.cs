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
--MeCTrabDist
        ";

        public static string BuscarEmpleado = @"
            SELECT 
                *, 
                CONCAT(MeNomAP, ' ', MeNomAM, ' ',MeNomEmp ) as NombreCompleto 
            FROM Empleado 
            INNER JOIN Empleado_Generales ON Empleado.ClkDet = Empleado_Generales.ClkDet
            INNER JOIN Centro_Trabajo ON Empleado.MeVCTrab = Centro_Trabajo.ClkCtVer AND Empleado.MeCTrab = Centro_Trabajo.ClkCt 
            INNER JOIN Puesto ON Empleado.MeVPuesto = Puesto.ClkPtVer AND Empleado.MePuesto = Puesto.ClkPt
            WHERE CONCAT(MeRfc, CAST(Empleado.ClkDet AS VARCHAR), MeNomAP, MeNomAM, MeNomEmp, MeNomEmp, MeNomAP, MeNomAM) 
        ";

        public static string BuscarCFDI = @"
         SELECT 
             pd.ClkPr + CAST(pd.ClkDet AS VARCHAR) AS ClkPr_ClkDet, 
             pc.PrAno, 
             RIGHT('0' + CAST(pc.PrQna AS VARCHAR), 2) AS PrQna, 
             pd.PrClvPag, 
             pd.PrNeto, 
             pd.PrUUID,  
         pd.PrXML
         FROM Producto_Detalle AS pd
         INNER JOIN Producto_Control AS pc ON pd.ClkPr = pc.ClkPr
";
    }
}
