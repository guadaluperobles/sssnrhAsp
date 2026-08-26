using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.Models {
    public class ConsultasModel {
        public static string ConsultaCFDI = @"
            SELECT
                pc.PrAno as PrAno,
                RIGHT('0' + CAST(pc.PrQna AS VARCHAR), 2) AS PrQna,
                pd.ClkPr as ClkPr,
                pd.PrNeto as PrNeto,
                pd.PrUUID as PrUUID,
                pd.PrXML as PrXML,
                CONCAT(emp.MeRfc, CAST(pd.ClkDet AS VARCHAR)) AS busqueda
            FROM Producto_Detalle AS pd
            INNER JOIN Producto_Control AS pc ON pd.ClkPr = pc.ClkPr
            INNER JOIN Empleado AS emp  ON pd.ClkDet = emp.ClkDet
        "; 
        
        public static string ConsultaRespaldoCFDI = @"
            SELECT
                CAST(PrAno AS SMALLINT) AS PrAno,
                CAST(PrQna AS VARCHAR(10)) AS PrQna,
                ClkPr,
                CAST(0.00 AS FLOAT) AS PrNeto,
                PrUUID,
                PrXML,
                CONCAT(MeRfc, CAST(ClkDet AS VARCHAR)) AS busqueda
            FROM RespaldoCFDI
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
             pc.PrAno as PrAno, 
             RIGHT('0' + CAST(pc.PrQna AS VARCHAR), 2) AS PrQna, 
             pd.PrClvPag as PrClvPag, 
             pd.PrNeto as PrClvPag, 
             pd.PrUUID as PrClvPag,  
         pd.PrXML
         FROM Producto_Detalle AS pd
         INNER JOIN Producto_Control AS pc ON pd.ClkPr = pc.ClkPr
"; 
        public static string BuscarRespaldoCFDI = @"
         SELECT 
             ClkPr + CAST(ClkDet AS VARCHAR) AS ClkPr_ClkDet, 
             CAST(PrAno AS SMALLINT) AS PrAno,
             CAST(PrQna AS VARCHAR(10)) AS PrQna,
             '' as PrClvPag, 
             CAST(0.00 AS FLOAT) AS PrNeto,
             PrUUID,  
             PrXML
         FROM RespaldoCFDI 
";
        public static string ValidarClkdetRfc = @"
            SELECT ClkDet FROM Empleado WHERE ClkDet = @clkdet AND MeRfc = @rfc
        "; 
        
        public static string BuscarClkdetRfc = @"
            SELECT ClkDet FROM Empleado WHERE  MeRfc = @rfc
        ";

    }
}
