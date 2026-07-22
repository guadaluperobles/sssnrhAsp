namespace RecursosHumanos.Models {
    public class ConsultasModel {
        public static string ConsultaCFDI = @"
            SELECT
                pd.ClkPr + CAST(pd.ClkDet AS VARCHAR) AS ClkPr_ClkDet, 
                pc.PrAno, 
                RIGHT('0' + CAST(pc.PrQna AS VARCHAR), 2) AS PrQna, 
                pd.PrNeto, 
                pd.PrUUID,  
                pd.PrXML 
            FROM Producto_Detalle AS pd 
            INNER JOIN Producto_Control AS pc ON pd.ClkPr = pc.ClkPr 
            INNER JOIN Empleado AS emp ON pd.ClkDet = emp.ClkDet
";
    }
}
