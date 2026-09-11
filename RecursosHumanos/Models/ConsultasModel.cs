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

        /**
         * Transparencia
         */
        public static string ConsultaPuesto = @"
        SELECT TOP 1 ClkPt, PtPtoSHCP, PtDsc1 FROM Puesto 
        WHERE ClkPt like @numero OR PtPtoSHCP like @numero
        ORDER BY ClkPtVer DESC ";

        public static string ConsultaBuscarPuestos = @"
            DECLARE @FechaInicio    DATE;
            DECLARE @FechaFin       DATE;
            DECLARE @quincena       VARCHAR(20);
 
            SET @FechaInicio =
                CASE @Trimestre
                    WHEN 1 THEN DATEFROMPARTS( @Anio,1,1)
                    WHEN 2 THEN DATEFROMPARTS( @Anio,4,1)
                    WHEN 3 THEN DATEFROMPARTS( @Anio,7,1)
                    WHEN 4 THEN DATEFROMPARTS( @Anio,10,1)
                END;
    
            SET @FechaFin =
                CASE @Trimestre
                    WHEN 1 THEN DATEFROMPARTS( @Anio,3,31)
                    WHEN 2 THEN DATEFROMPARTS( @Anio,6,30)
                    WHEN 3 THEN DATEFROMPARTS( @Anio,9,30)
                    WHEN 4 THEN DATEFROMPARTS( @Anio,12,31)
                END;
    
            DECLARE @QnaInicio SMALLINT;
            DECLARE @QnaFin SMALLINT;

            SET @QnaInicio =
                CASE @Trimestre
                    WHEN 1 THEN 1
                    WHEN 2 THEN 7
                    WHEN 3 THEN 13
                    WHEN 4 THEN 19
                END;

            SET @QnaFin =
                CASE @Trimestre
                    WHEN 1 THEN 6
                    WHEN 2 THEN 12
                    WHEN 3 THEN 18
                    WHEN 4 THEN 24
                END;

            SELECT DISTINCT 
                emp.ClkDet,   
                emp.MeRfc, 
                emp.MePuesto as Puesto, 
                pue.PtPtoSHCP as PuestoSHCP,
                pue.PtDsc1 as NombrePuesto,  
                ct.ClkCt as CentroTrabajo, 
                ct.CtDsc as DescripcionCentroTrabajo, 
                emp.MeNomEmp,
                emp.MeNomAP,
                emp.MeNomAM, 
                emp.MeSexo,
                CASE SUBSTRING(CAST(pue.PtIbc AS VARCHAR(2)), 1, 1)
                    WHEN '1' THEN 'Presupuestal'
                    WHEN '2' THEN 'Eventual'
                    WHEN '3' THEN 'Lista de raya'
                    WHEN '4' THEN 'Medica'
                END AS PtIbc_1, 
                CASE SUBSTRING(CAST(pue.PtIbc AS VARCHAR(2)), 2, 1)
                    WHEN '1' THEN 'Base'
                    WHEN '2' THEN 'Confianza'
                    WHEN '3' THEN 'Honorarios'  
                    WHEN '4' THEN 'Medico residente'  
                    WHEN '5' THEN 'Medico interno de pregrado' 
                    WHEN '6' THEN 'Pasante en servicio'
                END AS PtIbc_2,
                CASE CAST(pue.PtIVac AS VARCHAR(2))
                    WHEN '0' THEN 'No'
                    WHEN '1' THEN 'Si'
                END AS vacante,
                CASE CAST(pue.PtIVac AS VARCHAR(2))
                    WHEN '0' THEN 'Activo'
                    WHEN '9' THEN 'Baja'
                END AS PtIVac
            FROM Producto_Detalle as pd
            INNER JOIN Producto_Control ON pd.ClkPr = Producto_Control.ClkPr
            JOIN Empleado AS emp ON pd.ClkDet = emp.ClkDet
            JOIN Puesto AS pue ON pue.ClkPtVer = emp.MeVPuesto AND pue.ClkPt = emp.MePuesto
            INNER JOIN Centro_Trabajo as ct ON emp.MeVCTrab = ct.ClkCtVer AND emp.MeCTrab = ct.ClkCt
            WHERE 
                (Producto_Control.PrAno = @anio) AND 
                (Producto_Control.PrQna BETWEEN @QnaInicio AND @QnaFin) AND 
                (SUBSTRING(pd.ClkPr, 1, 2) = 'PR') AND 
                SUBSTRING(pd.prclvpag,5,3)<>'610'
        ";

    }
}
