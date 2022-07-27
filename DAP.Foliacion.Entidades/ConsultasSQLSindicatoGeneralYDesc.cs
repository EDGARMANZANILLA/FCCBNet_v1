using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades
{
    public class ConsultasSQLSindicatoGeneralYDesc
    {
        //public string AnObtenido = "";
        //public ConsultasSQLSindicatoGeneralYDesc(string An)
        //{
        //    AnObtenido = An;
        //}


        /// <summary>
        /// Devuelve 6 cadenas de consultas para los de confianza y otras 6 para los sindicalizado  
        /// </summary>
        /// <returns></returns>
        public static List<string> ObtenerConsultas_TotalesXSindicato( string An, int AnioInterface, List<string> bancosContenidosEnAn )
        {

            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString( AnioInterface );
            }




            /*Para los de confianza osea NO SINDICALIZADOS*/
            /*Campeche y Otros*/
            string Campeche_Confianza =             "select '0' 'Sindicato', '00' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )";

            /*Champoton*/
            string Champoton__Confianza =           "select '0' 'Sindicato', '03' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 03";

            /*Escarcega y candelaria*/
            string EscarcegaYCandelaria_Confianza = "select '0' 'Sindicato', '04' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in ('04', '11')";

            /*Calkini*/
            string Calkini_Confianza =              "select '0' 'Sindicato', '05' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 05";

            /*Hecelchakan*/
            string Hecelchakan_Confianza =          "select '0' 'Sindicato', '06' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 06";

            /*Hopelchen*/
            string Hopelchen_Confianza =            "select '0' 'Sindicato', '07' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 07";







            /*Para los de Sindicalizados*/
            /*Campeche y Otros*/
            string Campeche_Sindicalizados =            "select '1' 'Sindicato', '00' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )";

            /*Champoton*/
            string Champoton_Sindicalizados =           "select '1' 'Sindicato', '03' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 03";

            /*Escarcega y candelaria*/
            string EscarcegaYCandelaria_Sindicalizados ="select '1' 'Sindicato','04' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in ('04', '11')";

            /*Calkini*/
            string Calkini_Sindicalizados =             "select '1' 'Sindicato','05' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 05";

            /*Hecelchakan*/
            string Hecelchakan_Sindicalizados =         "select '1' 'Sindicato','06' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 06";

            /*Hopelchen*/
            string Hopelchen_Sindicalizados =           "select '1' 'Sindicato','07' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 07";





            //consul
            List<string> consultasPreparadas = new List<string>();

            if (An != null)
            {
                consultasPreparadas.Add(Campeche_Confianza);
                consultasPreparadas.Add(Champoton__Confianza);
                consultasPreparadas.Add(EscarcegaYCandelaria_Confianza);
                consultasPreparadas.Add(Calkini_Confianza);
                consultasPreparadas.Add(Hecelchakan_Confianza);
                consultasPreparadas.Add(Hopelchen_Confianza);



                consultasPreparadas.Add(Campeche_Sindicalizados);
                consultasPreparadas.Add(Champoton_Sindicalizados);
                consultasPreparadas.Add(EscarcegaYCandelaria_Sindicalizados);
                consultasPreparadas.Add(Calkini_Sindicalizados);
                consultasPreparadas.Add(Hecelchakan_Sindicalizados);
                consultasPreparadas.Add(Hopelchen_Sindicalizados);
            }



            return consultasPreparadas;
        }
























        public static string ObtenerCadenaConsulta_XSindicato(string An , int AnioInterface, string DelegacionesIncluidas, bool Sindicato) 
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            /*Universo de los que deben de estar Foliados*/
            //string universoDatos = "select NUM   from interfaces2021.dbo.ANSE2120000489  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('05')";
            string universoDatos;
            if (Sindicato)
            {
                universoDatos = "select NUM   from interfaces" + Anio + ".dbo." + An + "  where sindicato = 1 and  TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in " + DelegacionesIncluidas + " ";
            }
            else 
            {
                universoDatos = "select NUM   from interfaces" + Anio + ".dbo." + An + "  where sindicato = 0 and  TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in " + DelegacionesIncluidas + " ";
            }

            return  "SELECT COUNT(*) FROM interfaces"+Anio+".dbo."+An+ " WHERE NUM_CHE = '' AND  banco_x = '' and cuenta_x = '' AND Observa = '' AND NUM IN (" + universoDatos+")";
        }



        public static string ObtenerCadenaConsultaReportePDF_XSindicato(string An, int AnioInterface, string DelegacionesIncluidas, bool Sindicato)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            /*Universo de los que deben de estar Foliados*/
            //string universoDatos = "select NUM   from interfaces2021.dbo.ANSE2120000489  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('05')";
            string universoDatos;
            if (Sindicato)
            {
                universoDatos = "select Substring(PARTIDA,2,5), NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X  from interfaces" + Anio+".dbo."+An+"  where sindicato = 1 and  TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in "+DelegacionesIncluidas+" order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }
            else
            {
                universoDatos = "select Substring(PARTIDA,2,5), NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X  from interfaces" + Anio+".dbo."+An+"  where sindicato = 0 and  TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in  "+DelegacionesIncluidas+" order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }

            return universoDatos;
        }

        public static string ObtenerCadenaConsultaReportePDF_XSindicato_PartidaCompleta(string An, int AnioInterface, string DelegacionesIncluidas, bool Sindicato)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            /*Universo de los que deben de estar Foliados*/
            //string universoDatos = "select NUM   from interfaces2021.dbo.ANSE2120000489  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('05')";
            string universoDatos;
            if (Sindicato)
            {
                universoDatos = "select PARTIDA, NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X, RFC   from interfaces" + Anio + ".dbo." + An + "  where sindicato = 1 and  TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in " + DelegacionesIncluidas + " order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }
            else
            {
                universoDatos = "select PARTIDA, NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X, RFC   from interfaces" + Anio + ".dbo." + An + "  where sindicato = 0 and  TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in  " + DelegacionesIncluidas + " order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }

            return universoDatos;
        }














        /// <summary>
        /// Obtiene una cadena de una consulta para una nomina en espefico filtrado por si es de sindicato y por delegacion
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delagacion seleccionada</param>
        /// <param name="Sindicato"> boleano para saber si son sindicalizados o de confianza</param>
        /// <returns></returns>
        public static string ObtenerConsultaSindicatoFormasDePago(string AnObtenido ,  int Delegacion, bool Sindicato  )
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:

                    if (Sindicato)
                    {
                        /*Para los de Sindicalizados*/
                        /*Campeche y otros*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' ) order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    }
                    else
                    {

                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Campeche*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' ) order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    }

                    break;
                case 3:

                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Champoton*/
                        cadenaConsulta= " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                     
                    }
                    else
                    {
                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Champoton*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                       
                    }


                    break;
                case 4:


                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Escarcega y candelaria*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    

                    }
                    else
                    {

                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Escarcega y candelaria*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                       
                    }
                    break;
                case 5:


                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Calkini*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";

                     
                    }
                    else
                    {

                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Calkini*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    }
                    break;
                case 6:

                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Hecelchakan*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                        

                    }
                    else
                    {
                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Hecelchakan*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
                    }

                    break;
                case 7:

                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Hopelchen*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";


                    }
                    else
                    {
                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Hopelchen*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                    }

                    break;

            }
            return cadenaConsulta;
        }







        //*******************************************************************************************************************************************************************//
        //******************************************************************************************************************************************************************//
        //*****************************************************************************************************************************************************************//
                                                                    //*** CONSULTA PARA FOLIACION DE CHEQUES PARA NOMINA GENERAL Y DESCENTRALIZADA ***//
        /// <summary>
        /// Obtiene una cadena de una consulta para una nomina en espefico filtrado por si es de sindicato y por delegacion
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delagacion seleccionada</param>
        /// <param name="Sindicato"> boleano para saber si son sindicalizados o de confianza</param>
        /// <returns></returns>
        public static string ObtenerConsultaSindicatoFormasDePagoGeneralYDesc(string AnObtenido, int Delegacion, bool Sindicato)
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:

                    if (Sindicato)
                    {
                        /*Para los de Sindicalizados*/
                        /*Campeche y otros*/
                        cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG, Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' ) order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    }
                    else
                    {

                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Campeche*/
                        cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG, Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' ) order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    }

                    break;
                case 3:

                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Champoton*/
                        cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";


                    }
                    else
                    {
                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Champoton*/
                        cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";


                    }


                    break;
                case 4:


                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Escarcega y candelaria*/
                        cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG ,  Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";



                    }
                    else
                    {

                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Escarcega y candelaria*/
                        cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";


                    }
                    break;
                case 5:


                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Calkini*/
                        cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";


                    }
                    else
                    {

                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Calkini*/
                        cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    }
                    break;
                case 6:

                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Hecelchakan*/
                        cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";



                    }
                    else
                    {
                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Hecelchakan*/
                        cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
                    }

                    break;
                case 7:

                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Hopelchen*/
                        cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";


                    }
                    else
                    {
                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Hopelchen*/
                        cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO , DELEG , Partida , FolioCFDI from interfaces.dbo." + AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                    }

                    break;

            }
            return cadenaConsulta;
        }











        //*******************************************************************************************************************************************************************//
        //******************************************************************************************************************************************************************//
        //*****************************************************************************************************************************************************************//
        //*** CONSULTA PARA OBTENER CUANTOS REGISTROS SE VAN A FOLIARPARA NOMINA GENERAL Y DESCENTRALIZADA ***//
        /// <summary>
        /// Obtiene una cadena de una consulta para una nomina en espefico filtrado por si es de sindicato y por delegacion
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delagacion seleccionada</param>
        /// <param name="Sindicato"> boleano para saber si son sindicalizados o de confianza</param>
        /// <returns></returns>
        public static string ObtenerNumeroDeRegistroFormasDePagoGeneralYDesc(string AnObtenido, int Delegacion, bool Sindicato)
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:

                    if (Sindicato)
                    {
                        /*Para los de Sindicalizados*/
                        /*Campeche y otros*/
                        cadenaConsulta = " select  count(*)  from interfaces.dbo."+AnObtenido+ " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )";

                    }
                    else
                    {

                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Campeche*/
                        cadenaConsulta = " select count(*)  from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )";

                    }

                    break;
                case 3:

                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Champoton*/
                        cadenaConsulta = " select  count(*)  from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 03 ";


                    }
                    else
                    {
                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Champoton*/
                        cadenaConsulta = " select  count(*)  from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 03 ";


                    }


                    break;
                case 4:


                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Escarcega y candelaria*/
                        cadenaConsulta = " select  count(*) from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in ('04', '11') ";



                    }
                    else
                    {

                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Escarcega y candelaria*/
                        cadenaConsulta = " select  count(*)  from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in ('04', '11') ";


                    }
                    break;
                case 5:


                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Calkini*/
                        cadenaConsulta = " select  count(*)  from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 05 ";


                    }
                    else
                    {

                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Calkini*/
                        cadenaConsulta = " select  count(*)  from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 05  ";

                    }
                    break;
                case 6:

                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Hecelchakan*/
                        cadenaConsulta = " select count(*)  from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 06  ";



                    }
                    else
                    {
                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Hecelchakan*/
                        cadenaConsulta = " select count(*)  from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 06 ";
                    }

                    break;
                case 7:

                    if (Sindicato)
                    {

                        /*Para los de Sindicalizados*/
                        /*Hopelchen*/
                        cadenaConsulta = " select count(*)  from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 07 ";


                    }
                    else
                    {
                        /*Para los de confianza osea NO SINDICALIZADOS*/
                        /*Hopelchen*/
                        cadenaConsulta = " select count(*)  from interfaces.dbo."+AnObtenido + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 07 ";
                    }

                    break;

            }
            return cadenaConsulta;
        }











    }
}
