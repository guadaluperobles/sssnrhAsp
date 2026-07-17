namespace RecursosHumanos.Models {
    public class ConsultasModel {
        public string ConsultaCFDI = @"
            SELECT CASE DB_NAME() 
                  WHEN 'IESYS_ACREDITADOS' THEN 'ACREDITADOS' 
                  WHEN 'IESYS_SYSNGFCRSP' THEN 'COMPLEMENTOS' 
                  WHEN 'CONTRATOS' THEN 'CONTRATOS' 
                  WHEN 'IESYS_SYSNGFSON' THEN 'FEDERAL' 
                  WHEN 'FORMALIZADOS' THEN 'FORMALIZADOS' 
                  WHEN 'IESYS_SYSNGFHOMO' THEN 'HOMOLOGADOS' 
                  WHEN 'IESYS_SYSNGFPP' THEN 'PROGRAMAS' 
                  WHEN 'IESYS_HONOFED' THEN 'REGULARIZADOS' 
              ELSE DB_NAME() END AS 
                  DescripcionBD, pd.ClkPr + CAST(pd.ClkDet AS VARCHAR) AS ClkPr_ClkDet, pc.PrAno, RIGHT('0' + CAST(pc.PrQna AS VARCHAR), 2) AS PrQna, pd.PrClvPag, pd.PrNeto, pd.PrUUID,  pd.PrXML 
              FROM Producto_Detalle AS pd 
              INNER JOIN Producto_Control AS pc ON pd.ClkPr = pc.ClkPr 
";
    }
}
