using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades
{
    public class ConsultasSQLOtrasNominasConCheques
    {
        public static List<string> ObtenerConsultas_TotalesXOtrasNominas(string An , int AnioInterface) 
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            List<string> consultasPrediseneadas = new List<string>();
            /*Campeche */
            consultasPrediseneadas.Add ("select '', '00' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in('00', '01', '02', '08', '09', '10', '12', '13', '14', '15', '16')");

            /*Champoton*/
            consultasPrediseneadas.Add("select '', '03' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." +An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03");

            /*Escarcega y candelaria*/
            consultasPrediseneadas.Add("select '', '04' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('04', '11')");

            /*Calkini*/
            consultasPrediseneadas.Add("select '', '05' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05");

            /*Hecelchakan*/
            consultasPrediseneadas.Add("select '', '06' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg = 06");

            /*Hopelchen*/
            consultasPrediseneadas.Add("select '', '07' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg = 07");

            return consultasPrediseneadas;
        }













        /// <summary>
        /// Obtiene una cadena de una consulta para delegacion de una nomina en espefico filtrado 
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delegacion seleccionada</param>
        /// <returns>Regresa una consulta  </returns>
        public string ObtenerConsultaConOrdenamientoFormasDePago(int Delegacion, string An )
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:
                        /*Campeche*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' ) order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 3:
                       /*Champoton 03 */
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 5:
                     /*Calkini 5 */
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    
                    break;
                case 6:
                    /*Hecelchakan 6 */
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
                    
                    break;
                case 7:
                     /*Hopelchen 7 */
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                    
                    break;

            }
            return cadenaConsulta;
        }





        /// <summary>
        /// Obtiene una consulta de delegacion seleccionada de una nomina QUE NO ES GENERAL, DESCENTRALIZADA Y PENSION ALIMENTICA 
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delegacion seleccionada</param>
        /// <returns>Regresa una consulta con la que se deben de foliar TODAS LAS NOMINAS EXCEPTO LAS NOMINAS (GENERAL, DESCENTRALIZADA Y PENSION ALIMENTICA) </returns>
        public string ObtenerConsultaConOrdenamientoFormasDePagoFoliar(int Delegacion, string An)
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:
                    /*Campeche*/
                    cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , partida , FolioCFDI   from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' ) order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 3:
                    /*Champoton 03 */
                    cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , partida , FolioCFDI  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                    cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , partida , FolioCFDI  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 5:
                    /*Calkini 5 */
                    cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , partida , FolioCFDI  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";


                    break;
                case 6:
                    /*Hecelchakan 6 */
                    cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , partida , FolioCFDI  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 7:
                    /*Hopelchen 7 */
                    cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , partida , FolioCFDI  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";

                    break;

            }
            return cadenaConsulta;
        }



        /// <summary>
        /// Obtiene EL TOTAL DE REGISTROS A FOLIAR De una nomina QUE NO ES GENERAL, DESCENTRALIZADA Y PENSION ALIMENTICA 
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delegacion seleccionada</param>
        /// <returns>Regresa una consulta con la que se deben de foliar TODAS LAS NOMINAS EXCEPTO LAS NOMINAS (GENERAL, DESCENTRALIZADA Y PENSION ALIMENTICA) </returns>
        public string ObtenerTotalRegistrosDePagoFoliarOtrasNominas(int Delegacion, string An)
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:
                    /*Campeche*/
                    cadenaConsulta = " select count(*)  from interfaces.dbo." + An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' ) ";

                    break;
                case 3:
                    /*Champoton 03 */
                    cadenaConsulta = " select  count(*) from interfaces.dbo." + An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg = 03 ";

                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                    cadenaConsulta = " select count(*) from interfaces.dbo." + An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in ('04', '11') ";

                    break;
                case 5:
                    /*Calkini 5 */
                    cadenaConsulta = " select count(*) from interfaces.dbo." + An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 ";


                    break;
                case 6:
                    /*Hecelchakan 6 */
                    cadenaConsulta = " select count(*) from interfaces.dbo." + An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06 ";

                    break;
                case 7:
                    /*Hopelchen 7 */
                    cadenaConsulta = " select count(*) from interfaces.dbo." + An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07 ";

                    break;

            }
            return cadenaConsulta;
        }






        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//





        //Consultas para la nomina de PENCION ALIMENTICIA

        public static List<string> ObtenerConsultas_TotalesXPencionAlimenticia(string An ,int AnioInterface) 
        {
            List<string> consultaTotales = new List<string>();


            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            /*Consultar_TOTALES*/
            /*Campeche*/
            consultaTotales.Add("select '', '00' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00', '01', '02', '08', '09', '10', '12', '13', '14', '15', '16')");

            /*Champoton*/
            consultaTotales.Add("select '', '03' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03");

            /*Escarcega y candelaria*/
            consultaTotales.Add("select '', '04' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('04', '11')");

            /*Calkini*/
            consultaTotales.Add("select '', '05' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05");

            /*Hecelchakan*/
            consultaTotales.Add("select '', '06' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06");

            /*Hopelchen*/
           consultaTotales.Add("select '', '07' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07 ");



            return consultaTotales;
        }




        public static string ObtenerCadenaConsulta_GenericaOtrasNomina(string An, int AnioInterface, string DelegacionesIncluidas)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            /*Universo de los que deben de estar Foliados*/
            //string universoDatos = "select NUM   from interfaces2021.dbo.ANSE2120000489  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('05')";
          
           string  universoDatos = "select NUM   from interfaces" + Anio + ".dbo." + An + "  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in " + DelegacionesIncluidas + " ";
           

            return "SELECT COUNT(*) FROM interfaces" + Anio + ".dbo." + An + " WHERE TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and NUM_CHE = '' AND  banco_x = '' and cuenta_x = '' AND Observa = '' AND NUM IN (" + universoDatos + ")";
        }






        public static string ObtenerCadenaConsultaReportePDF_GenericOtrasNominas(string An, int AnioInterface, string DelegacionesIncluidas, bool EsPena)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            /*Universo de los que deben de estar Foliados*/
            //string universoDatos = "select NUM   from interfaces2021.dbo.ANSE2120000489  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('05')";
            string universoDatos;
            if (EsPena) 
            {
                 universoDatos = "select Substring(PARTIDA,2,5) , NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X    from interfaces" + Anio + ".dbo." + An + "  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in " + DelegacionesIncluidas + " order by JUZGADO, NOMBRE ";
            }
            else 
            {
                universoDatos = "select Substring(PARTIDA,2,5) , NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X    from interfaces" + Anio + ".dbo." + An + "  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in " + DelegacionesIncluidas + " order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }


            return universoDatos;
        }


        public static string ObtenerCadenaConsultaReportePDF_GenericOtrasNominas_PartidaCompleta(string An, int AnioInterface, string DelegacionesIncluidas, bool EsPena)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            /*Universo de los que deben de estar Foliados*/
            //string universoDatos = "select NUM   from interfaces2021.dbo.ANSE2120000489  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('05')";
            string universoDatos;
            if (EsPena)
            {
                universoDatos = "select PARTIDA, NUM, Inter.NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X , RFC,  BENEF      from interfaces" + Anio + ".dbo." + An + " as Inter where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in " + DelegacionesIncluidas + " order by JUZGADO, Inter.NOMBRE ";
            }
            else
            {
                universoDatos = "select PARTIDA, NUM, Inter.NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X , RFC  from interfaces" + Anio + ".dbo." + An + " as Inter  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in " + DelegacionesIncluidas + " order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), Inter.NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }


            return universoDatos;
        }




















        /// <summary>
        /// Obtiene una cadena de una consulta para delegacion de una nomina con PENSION ALIMENTICIA PARA REVISION (Como quedaria ya foliada)
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delegacion seleccionada</param>
        /// <returns>Regresa una consulta  </returns>
        public string ObtenerConsultaConOrdenamientoFormasDePagoPensionAlimenticia(int Delegacion, string An)
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:
                    /*Campeche*/
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )  order by JUZGADO, NOMBRE";

                    break;
                case 3:
                    /*Champoton 03 */
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03  order by JUZGADO, NOMBRE";

                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('04' , '11' )  order by JUZGADO, NOMBRE";

                    break;
                case 5:
                    /*Calkini 5 */
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 order by JUZGADO, NOMBRE";


                    break;
                case 6:
                    /*Hecelchakan 6 */
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06  order by JUZGADO, NOMBRE";

                    break;
                case 7:
                    /*Hopelchen 7 */
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07  order by JUZGADO, NOMBRE";

                    break;

            }
            return cadenaConsulta;
        }



        /// <summary>
        /// Obtiene una cadena de una consulta para una delegacion de nomina con PENSION ALIMENTICIA PARA FOLIAR
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delegacion seleccionada</param>
        /// <returns>Regresa una consulta con la que se debe de foliar una NOMINA DE PENSION ALIMENTICIA  </returns>
        public string ObtenerConsultaConOrdenamientoFormasDePagoPensionAlimenticiaFoliar(int Delegacion, string An)
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:
                    /*Campeche*/
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , partida , FolioCFDI  ,  BENEF 'NumBeneficiario'  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )  order by JUZGADO, NOMBRE";

                    break;
                case 3:
                    /*Champoton 03 */
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG  , partida , FolioCFDI , BENEF 'NumBeneficiario'  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03  order by JUZGADO, NOMBRE";

                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , partida , FolioCFDI  , BENEF 'NumBeneficiario'  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('04' , '11' )  order by JUZGADO, NOMBRE";

                    break;
                case 5:
                    /*Calkini 5 */
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO,  DELEG , partida , FolioCFDI , BENEF 'NumBeneficiario'  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 order by JUZGADO, NOMBRE";


                    break;
                case 6:
                    /*Hecelchakan 6 */
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO, DELEG , partida , FolioCFDI ,  BENEF 'NumBeneficiario'  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06  order by JUZGADO, NOMBRE";

                    break;
                case 7:
                    /*Hopelchen 7 */
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO, DELEG , partida , FolioCFDI , BENEF 'NumBeneficiario'  from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07  order by JUZGADO, NOMBRE";

                    break;

            }
            return cadenaConsulta;
        }







        public string ObtenerTotalRegistrosPensionAlimenticiaFoliar(int Delegacion, string An)
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:
                    /*Campeche*/
                    cadenaConsulta = "select  count(*)  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )  ";

                    break;
                case 3:
                    /*Champoton 03 */
                    cadenaConsulta = "select  count(*)  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03 ";

                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                    cadenaConsulta = "select  count(*)   from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('04' , '11' )  ";

                    break;
                case 5:
                    /*Calkini 5 */
                    cadenaConsulta = "select  count(*)  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 ";


                    break;
                case 6:
                    /*Hecelchakan 6 */
                    cadenaConsulta = "select  count(*)  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06 ";

                    break;
                case 7:
                    /*Hopelchen 7 */
                    cadenaConsulta = "select  count(*)  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07 ";

                    break;

            }
            return cadenaConsulta;
        }

        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//


    }
}
