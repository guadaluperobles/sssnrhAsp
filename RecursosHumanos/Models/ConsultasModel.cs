using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.Models {
    public class ConsultasModel {
        public static string ConsultaCFDI = @"
            SELECT
                pc.PrAno as PrAno,
                RIGHT('0' + CAST(pc.PrQna AS VARCHAR), 2) AS PrQna,
                emp.MeRfc as MeRfc,               
                CONCAT(emp.MeNomEmp, ' ', emp.MeNomAP, ' ', emp.MeNomAM) AS NombreEmpleado,             
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

        public static string ConsultaTotalPercepcionesEmpleados = @" 
            DECLARE @FechaInicio    DATE;
            DECLARE @FechaFin       DATE;
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
            ; WITH Datos AS
            (
                SELECT
            --    pd.ClkPr,
            --    emp.MeCTrab,
            --    emp.MeNumPto,
            --    pc.PrQna,
            --    emp.ClkDet                                      as 'noEmp',
                pc.PrAno                                        as 'ejercicio',
                CONVERT(VARCHAR(10), @FechaInicio, 103)			as 'fechaInicioPeriodo',
                CONVERT( VARCHAR(10), @FechaFin, 103)			as 'fechaFinPeriodo',
	            'Empleada (o)'									as 'tipoSujetoObligado',
                pue.PtPtoSHCP									as 'claveNivelPuesto',
                pue.PtDsc1										as 'descripcionPuesto',
	            pue.PtDsc1  									as 'descripcionCargo',
                ct.CtDsc										as 'areaAdscripcion', 
                emp.MeNomEmp									as 'nombre',
                emp.MeNomAP										as 'primerApellido',
                emp.MeNomAm										as 'segundoApellido',
                CASE emp.MeSexo
                    WHEN 'H' THEN 'Hombre'
                    WHEN 'M' THEN 'Mujer'
                    ELSE ''
                END                                             AS 'sexo',
                pd.PrTPer										as 'montoRemuredacionMensualBruta', 
	            'MXN'											as 'tipoMonedaBruta',
                pd.PrNeto										as 'montoRemuredacionMensualNeta', 
	            'MXN'											as 'tipoMonedaNeta',
	            ''												as 'Tabla_140',
	            ''												as 'Tabla_141',
	            ''												as 'Tabla_142',
	            ''												as 'Tabla_143',
	            ''												as 'Tabla_144',
	            ''												as 'Tabla_145',
	            ''												as 'Tabla_146',
	            ''												as 'Tabla_147',
	            ''												as 'Tabla_148',
	            ''												as 'Tabla_149',
	            ''												as 'Tabla_155',
	            ''												as 'Tabla_150',
	            ''												as 'Tabla_151',
	            'DIRECCION GENERAL DE RECURSOS HUMANOS'		    as 'AreaResponsable',
	            '24/4/2026'										as 'fechaActualizacion',
	            ''      										as 'nota',
	            emp.MeRfc									    as 'rfc',
	            empgen.MeCurp								    as 'curp',
                CASE SUBSTRING(empgen.MeCurp, 11, 1)
                    WHEN 'H' THEN 'Hombre'
                    WHEN 'M' THEN 'Mujer'
                    ELSE ''
                END                                             AS 'sexoCURP',
                CASE emp.MeSexo
                    WHEN 'H' THEN 'Hombre'
                    WHEN 'M' THEN 'Mujer'
                    ELSE ''
                END                                             AS 'sexoBD',
                    ROW_NUMBER() OVER(
                        PARTITION BY emp.ClkDet
                       ORDER BY pd.PrTPer DESC
                    ) AS RN
                FROM Producto_Detalle pd
            INNER JOIN Producto_Control AS pc ON pd.ClkPr = pc.ClkPr
            INNER JOIN Centro_Trabajo AS ct ON pd.PrVCTrab = ct.ClkCtVer AND pd.PrCtrab = ct.ClkCt
            JOIN Empleado AS emp ON pd.ClkDet = emp.ClkDet
            JOIN Empleado_Generales AS empgen ON empgen.ClkDet = emp.ClkDet
            JOIN Puesto AS pue ON pue.ClkPtVer = emp.MeVPuesto AND pue.ClkPt = emp.MePuesto
                WHERE
                   pc.PrAno = @Anio
                    AND pd.ClkPr LIKE 'PRO%'
                    AND (
                        (@Trimestre = 1 AND pc.PrQna BETWEEN 1 AND 6) OR
                        ( @Trimestre = 2 AND pc.PrQna BETWEEN 7 AND 12) OR
                         ( @Trimestre = 3 AND pc.PrQna BETWEEN 13 AND 18) OR
                          ( @Trimestre = 4 AND pc.PrQna BETWEEN 19 AND 24)
                    )
            )
            SELECT *
            FROM Datos
            WHERE RN = 1";

        public static string ConsultaBuscarComplementos = @"
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

                SELECT TOP 1
                    pc.PrAno,
                    pc.PrQna,
                    pd.ClkPr,
                    pd.ClkDet,
                    pd.PrTPer,
                    pd.PrNeto,
                    emp.MeRfc
                FROM Producto_Detalle AS pd
                JOIN Empleado AS emp
                    ON pd.ClkDet = emp.ClkDet
                INNER JOIN Producto_Control AS pc
                    ON pd.ClkPr = pc.ClkPr
                WHERE emp.MeRfc = @rfc
                  AND pc.PrAno = @anio
                  AND pd.PrNmCheq IN (
                        SELECT PrNmCheq
                        FROM Producto_Detalle pd2
                        INNER JOIN Empleado emp2 ON pd2.ClkDet = emp2.ClkDet
                        INNER JOIN Producto_Control pc2 ON pd2.ClkPr = pc2.ClkPr
                        WHERE emp2.MeRfc = @rfc AND pc2.PrAno = @anio AND 
                            (pc2.PrQna BETWEEN @QnaInicio AND @QnaFin) 
                        GROUP BY PrNmCheq
                        HAVING COUNT(*) = 1
                  )
                ORDER BY pc.PrQna DESC;
        ";
        public static string ConsultaTransparencia = @"
            SELECT        
                Historico_Movimiento.ClkDet, 
                MeSexo, 
                Historico_Movimiento.HmRfcA, 
                Historico_Movimiento.HmCodMov, 
                Historico_Movimiento.HmQnaAp, 
                Historico_Movimiento.HmFchIni, 
                Empleado.MeNomAP, 
                Empleado.MeNomAM,    
                Empleado.MeNomEmp, 
                Movimiento.CvDsc 
            FROM Historico_Movimiento 
            INNER JOIN Empleado ON Historico_Movimiento.ClkDet = Empleado.ClkDet 
            INNER JOIN Movimiento ON Historico_Movimiento.HmCodMov = Movimiento.ClkMov 
            WHERE (Historico_Movimiento.HmCodMov = 1102 )
        ";
    }
}
