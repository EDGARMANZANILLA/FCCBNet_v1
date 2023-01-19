using DAP.Foliacion.Datos;
using DAP.Foliacion.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Negocios.ObtenerConsultasChequesFoliarNegocios
{
    public class consultasCheques
    {

        /* DELEGACIONES "Esto puede cambiar asi que amigo programer pregunta por la relacion de bancos con delegaciones "*/
        /*  Campeche                => 00 */
        /*  Champoton               => 03 */
        /*  Escarcega y candelaria  => 04 */
        /*  Calkini                 => 05 */
        /*  Hecelchakan             => 06 */
        /*  Hopelchen               => 07 */


        /// <summary>
        /// Regresa "1" si es sindicato y "0" si es de confianza, solo aplica para las nomina GENERAL Y DESCENTRALIZADA
        /// </summary>
        /// <param name="Sindicato"></param>
        /// <returns></returns>
        public static int SindicatoOConfianza(bool Sindicato) 
        {
            return Sindicato ? 1 : 0;
        }

        public static string ConvertirListaBancosEnCondicionParaCheques(List<string> bancosContenidosEnAn)
        {
            string condicion = "";
            int i = 0;
            foreach (string banco in bancosContenidosEnAn)
            {
                i += 1;

                if (i == 1)
                {
                    condicion += condicion = " " + banco + " = '' ";
                }
                else
                {
                    condicion += condicion = " and " + banco + " = '' ";
                }
            }
            return condicion;
        }



        /*****************************************************************************************************************************************************************************************************************************/
        /**************************************************************             NOMINAS GENERAL Y DESCENTRALIZADA    *************************************************************************************************************/
        /*****************************************************************************************************************************************************************************************************************************/

        public static List<string> ObtenerConsultasTotalPersonal_ConfianzaSindicaliza(string An, int AnioInterface, List<string> bancosContenidosEnAn)
        {
            List<string> consultasListas = new List<string>();

            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }

            string condicionBancos = ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);

            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            List<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos().ToList();

            //Se deben iterar 2 veces => la primera para los de confianza  y la segunda para sindicalizados
            for (int i = 0; i < 2; i++) 
            {
                foreach (var itemsDelegacion in filtrodelegaciones)
                {
                    if (i == 0)
                    {
                        //son de confianza
                        consultasListas.Add("select '0' 'Sindicato', '" + itemsDelegacion.GrupoImpresionDelegacion + "' 'Nom_Deleg' ,count(*) 'Total' from interfaces" + Anio + ".dbo." + An + " where " + condicionBancos + " and sindicato = 0 and deleg in " + itemsDelegacion.DelegacionesIncluidas + "");
                    }
                    else 
                    {
                        //Son sindicalizados
                        consultasListas.Add("select '1' 'Sindicato', '" + itemsDelegacion.GrupoImpresionDelegacion + "' 'Nom_Deleg' ,count(*) 'Total' from interfaces" + Anio + ".dbo." + An + " where " + condicionBancos + " and sindicato = 1 and deleg in " + itemsDelegacion.DelegacionesIncluidas + "");
                    }
                }

            }
            return consultasListas;
        }
        public static string ObtenerConsultaNumerosEmpleadosEnDelegacionGENERALYDESCE(string An, string visitaAnioInterface, string CondicionBancos, string DelegacionesIncluidas, bool Sindicato)
        {
            int sindi = SindicatoOConfianza(Sindicato);
            return "select Num from  interfaces" + visitaAnioInterface + ".dbo." + An + " where  " + CondicionBancos + " and  deleg in  " + DelegacionesIncluidas + " and Sindicato =" + sindi + " ";
        }




        /************************************************************************************************************************************************************************************************************************************/
        /**************************************************************             OTRAS NOMiNAS Y PENSION ALIMENTICIA                 *****************************************************************************************************/
        /************************************************************************************************************************************************************************************************************************************/
        public static List<string> ObtenerConsultasTotalPersonal_OtrasNominasYPenA(string An, string visitaAnioInterface, string CondicionBancos)
        {
            List<string> consultasTotalPersonal = new List<string>();

            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            List<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos().ToList();

           foreach (var itemsDelegacion in filtrodelegaciones)
           {
             consultasTotalPersonal.Add("select '', '"+itemsDelegacion.GrupoImpresionDelegacion+"' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+visitaAnioInterface+".dbo."+An+" where  "+CondicionBancos+" and  deleg in  "+itemsDelegacion.DelegacionesIncluidas+" ");
           }

            return consultasTotalPersonal;
        }

        public static string ObtenerConsultaNumerosEmpleadosEnDelegacion(string An, string visitaAnioInterface, string CondicionBancos, string DelegacionesIncluidas)
        {
            return "select Num from  interfaces" + visitaAnioInterface + ".dbo." + An + " where  " + CondicionBancos + " and  deleg in  " + DelegacionesIncluidas + "";
        }
        




        /*****************************************************************************************************************************************************************************************************************************/
        /**************************************************************              OBTIENE CONSULTA PARA SABER CUANTOS REGISTROS NO ESTAN FOLIADOS DEACUERDO A LA NOMINA      ******************************************************/
        /*****************************************************************************************************************************************************************************************************************************/
        public static string ObtenerConsultaTotalRegistrosNoFoliadosxDelegacion_GeneralDescentralizada(string An, string VisitaAnioInterface, string CondicionBancos,  string DelegacionesIncluidas,  bool Sindicato)
        {
            //Devuelve "1" si es sindicalizado o "0" si es Confianza
            int sindi = SindicatoOConfianza(Sindicato);
            string  universoDatos = "select NUM from interfaces"+VisitaAnioInterface+".dbo."+An+" where  "+CondicionBancos+"  and  sindicato = "+sindi+" and deleg in "+DelegacionesIncluidas+" ";  
            return "SELECT COUNT(*) FROM interfaces"+VisitaAnioInterface+".dbo."+An+" WHERE (NUM_CHE = '' or  banco_x = '' or cuenta_x = '' or Observa = '') AND NUM IN ("+universoDatos+")";
        }


        public static string ObtenerConsultaTotalRegistrosNoFoliadosxDelegacion_OtrasNominasYPenA(string An, string visitaAnioInterface, string CondicionBancos , string DelegacionesIncluidas)
        {
            string universoDatos = "select NUM   from interfaces"+visitaAnioInterface+".dbo."+An+"  where "+CondicionBancos+"  and deleg in "+DelegacionesIncluidas+" ";
            return "SELECT COUNT(*) FROM interfaces"+visitaAnioInterface+".dbo." + An + " WHERE (NUM_CHE = '' or  banco_x = '' or cuenta_x = '' or Observa = '') AND NUM IN (" + universoDatos + ")";
        }




        /***********************************************************************************************************************************************************************************************************************************************/
        /**************************************************************              OBTIENE CONSULTA PARA SABER El DETALLE DE COMO SE ENCUENTRA LAS PERSONAS O REGISTROS EN AN DE LA NOMINA      ******************************************************/
        /***********************************************************************************************************************************************************************************************************************************************/
        public static string ObtenerConsultaDetallePersonalEnAN_GeneralDescentralizada(string An, string visitaAnioInterface , string CondicionBancos,  string DelegacionesIncluidas, bool Sindicato)
        {
            int sindi = SindicatoOConfianza(Sindicato);

            return "select Substring(PARTIDA,2,5), NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X , OBSERVA from interfaces" + visitaAnioInterface + ".dbo." + An + "  where  " + CondicionBancos + " and  sindicato = " + sindi + " and  deleg in " + DelegacionesIncluidas + " order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
        }



        public static string ObtenerConsultaDetallePersonalEnAN_OtrasNominasYPenA(string An, string VisitaAnioInterface, string CondicionBancos ,  string DelegacionesIncluidas, bool EsPena)
        {
            string universoDatos;
            if (EsPena)
            {
                universoDatos = "select Substring(PARTIDA,2,5) , NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X , OBSERVA    from interfaces" + VisitaAnioInterface + ".dbo."+An+"  where "+CondicionBancos+"  and deleg in " + DelegacionesIncluidas + " order by JUZGADO, NOMBRE ";
            }
            else
            {
                universoDatos = "select Substring(PARTIDA,2,5) , NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X , OBSERVA   from interfaces"+VisitaAnioInterface + ".dbo."+An+"  where "+CondicionBancos+"  and deleg in " + DelegacionesIncluidas + " order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }


            return universoDatos;
        }


        /*********************************************************************************************************************************************************************************************************************************************/
        /**************************************************************              OBTIENE CONSULTA PARA SABER EL ORDEN EN EL QUE SE VA A FOLIAR LA DELEGACION SELECCIONADA               **********************************************************/
        /*********************************************************************************************************************************************************************************************************************************************/
        public static string ObtenerConsultaOrdenDeFoliacionPorDelegacion_GeneralDescentralizada(string An, string VisitaAnioInterface, string CondicionBancos , string DelegacionesIncluidas, bool Sindicato )
        {
           /* Universo de los que SE VAN A FOLIAR */
           int sindi = SindicatoOConfianza(Sindicato);
           return  "select PARTIDA, NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X, RFC   from interfaces"+VisitaAnioInterface+".dbo." + An + "  where  "+CondicionBancos+"  and sindicato = "+sindi+" and deleg in "+DelegacionesIncluidas+" order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
        }


        public static string ObtenerConsultaOrdenDeFoliacionPorDelegacion_OtrasNominasYPenA(string An, string VisitaAnioInterface, string CondicionBancos, string DelegacionesIncluidas, bool EsPena)
        {
            string universoDatos;
            if (EsPena)
            {
                universoDatos = "select PARTIDA, NUM, Inter.NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X , RFC,  BENEF      from interfaces"+VisitaAnioInterface+".dbo."+An+ " as Inter where  "+CondicionBancos+"  and deleg in "+DelegacionesIncluidas+" order by JUZGADO, Inter.NOMBRE ";
            }
            else
            {
                universoDatos = "select PARTIDA, NUM, Inter.NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X , RFC              from interfaces"+VisitaAnioInterface+".dbo."+An+" as Inter  where  "+CondicionBancos+"  and deleg in "+DelegacionesIncluidas+" order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), Inter.NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }

            return universoDatos;
        }

        /*********************************************************************************************************************************************************************************************************************************************/
        /*********************************************************             AJUSTE ====>  OBTIENE CONSULTA PARA SABER EL ORDEN QUE SE TIENE QUE VOLVER A FOLIAR LA DELEGACION SELECCIONADA EN EL AJUSTE                     ***********************/
        /*********************************************************************************************************************************************************************************************************************************************/
        public static string ObtenerConsultaParaAJuste_GeneralDescentralizada(string An, string visitaAnioInterface, string CondicionBancos, string DelegacionesIncluidas , bool Sindicato)
        {
            int sindi = SindicatoOConfianza(Sindicato);
            string indexadoNomina = obtenerIndexadoDeNomina();
            return "select PARTIDA, NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X, RFC  from  interfaces" + visitaAnioInterface + ".dbo." + An + " where  (NUM_CHE ='' and CUENTA_X = '') and  " + CondicionBancos + " and sindicato = " + sindi + " and  deleg in  " + DelegacionesIncluidas+"  "+indexadoNomina+"  ";
        }

        public static string ObtenerConsultaParaAJuste_OtrasNominasYPenA(string An, string VisitaAnioInterface, string CondicionBancos, string DelegacionesIncluidas, bool EsPena)
        {
            string indexadoNomina = obtenerIndexadoDeNomina();
            string universoDatos;
            if (EsPena)
            {
                universoDatos = "select PARTIDA, NUM, Inter.NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X , RFC,  BENEF      from interfaces" + VisitaAnioInterface + ".dbo." + An + " as Inter where   (NUM_CHE ='' and CUENTA_X = '') and " + CondicionBancos + "  and deleg in " + DelegacionesIncluidas + " order by JUZGADO, Inter.NOMBRE ";
            }
            else
            {
                universoDatos = "select PARTIDA, NUM, Inter.NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X , RFC              from interfaces" + VisitaAnioInterface + ".dbo." + An + " as Inter  where  (NUM_CHE ='' and CUENTA_X = '') and " + CondicionBancos+"  and deleg in "+DelegacionesIncluidas+" "+indexadoNomina+" ";
            }

            return universoDatos;
        }



        public static string obtenerIndexadoDeNomina() 
        {
            return "order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), Inter.NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
        } 

        /*****************************************************************************************************************************************************************************************************************************************************************/
        /**************************************************************              OBTIENE CONSULTA PARA SABER QUE REGISTROS SE DEBEN DE BLANQUEAR CUANDO LA FOLIACION NO CUMPLA CON EL ESTANDART DE CALIDAD               *********************************************/
        /*****************************************************************************************************************************************************************************************************************************************************************/
        public static string ObtenerCondicionParaLimpiarRegistrosChequePorDelegacion_GeneralDescentralizada(string An, string CondicionBancos, string DelegacionesIncluidas, bool Sindicato)
        {
            // No contiene el where ya que este se define en el metodo donde se hara la limpieza de los registros bajo esta condicion
            int sindi = SindicatoOConfianza(Sindicato);
            string universoDatos = "   " + CondicionBancos + "  and sindicato = " + sindi + " and deleg in " + DelegacionesIncluidas + " ";

            return universoDatos;
        }

        public static string ObtenerCondicionParaLimpiarRegistrosChequePorDelegacion_OtrasNominasYPenA(string An, string CondicionBancos, string DelegacionesIncluidas, bool EsPena )
        {
            // No contiene el where ya que este se define en el metodo donde se hara la limpieza de los registros bajo esta condicion
            string universoDatos;
            if (EsPena)
            {
                universoDatos = "   "+CondicionBancos+"   and deleg in "+DelegacionesIncluidas+"  ";
            }
            else
            {
                universoDatos = "   "+CondicionBancos+"  and deleg in "+DelegacionesIncluidas+"  ";
            }

            return universoDatos;
        }



    }
}
