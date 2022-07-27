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




        /****************************************************************************************************************************************************************************************************************************/
        /**************************************************************               PENSION ALIMENTICIA                 *************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        public static List<string> ObtenerConsultasTotalPersonal_OtrasNominasYPenA(string An, int AnioInterface, List<string> bancosContenidosEnAn)
        {
            List<string> consultasTotalPersonal = new List<string>();


            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            string condicionBancos = ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);

            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            List<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos().ToList();

           foreach (var itemsDelegacion in filtrodelegaciones)
           {
             consultasTotalPersonal.Add("select '', '"+itemsDelegacion.GrupoImpresionDelegacion+"' 'Nom_Deleg' ,count(*) 'Total' from interfaces"+Anio+".dbo."+An+" where  "+condicionBancos+" and  deleg in  "+itemsDelegacion.DelegacionesIncluidas+" ");
           }

            return consultasTotalPersonal;
        }




        /****************************************************************************************************************************************************************************************************************************/
        /**************************************************************              OBTIENE CONSULTA PARA SABER CUANTOS REGISTROS NO ESTAN FOLIADOS DEACUERDO A LA NOMINA      ******************************************************/
        /*****************************************************************************************************************************************************************************************************************************/
        public static string ObtenerConsultaTotalRegistrosNoFoliadosxDelegacion_GeneralDescentralizada(string An, int AnioInterface, string DelegacionesIncluidas, bool Sindicato, List<string> bancosContenidosEnAn)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            string condicionBancos = ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            /*Universo de los que deben de estar Foliados*/
            //string universoDatos = "select NUM   from interfaces2021.dbo.ANSE2120000489  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('05')";
            string universoDatos;
            if (Sindicato)
            {
                universoDatos = "select NUM from interfaces" + Anio + ".dbo." + An + " where  " + condicionBancos + "  and  sindicato = 1 and deleg in " + DelegacionesIncluidas + " ";
            }
            else
            {
                universoDatos = "select NUM from interfaces" + Anio + ".dbo." + An + "  where  " + condicionBancos + "  and  sindicato = 0 and deleg in " + DelegacionesIncluidas + " ";
            }

            return "SELECT COUNT(*) FROM interfaces" + Anio + ".dbo." + An + " WHERE (NUM_CHE = '' or  banco_x = '' or cuenta_x = '' or Observa = '') AND NUM IN (" + universoDatos + ")";
        }


        public static string ObtenerConsultaTotalRegistrosNoFoliadosxDelegacion_OtrasNominasYPenA(string An, int AnioInterface, string DelegacionesIncluidas, List<string> bancosContenidosEnAn)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }

            string condicionBancos = ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            /*Universo de los que deben de estar Foliados*/
            //string universoDatos = "select NUM   from interfaces2021.dbo.ANSE2120000489  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('05')";

            string universoDatos = "select NUM   from interfaces"+Anio+".dbo."+An+"  where "+condicionBancos+"  and deleg in "+DelegacionesIncluidas+" ";


            return "SELECT COUNT(*) FROM interfaces" + Anio + ".dbo." + An + " WHERE (NUM_CHE = '' or  banco_x = '' or cuenta_x = '' or Observa = '') AND NUM IN (" + universoDatos + ")";
        }




        /****************************************************************************************************************************************************************************************************************************/
        /**************************************************************              OBTIENE CONSULTA PARA SABER El DETALLE DE COMO SE ENCUENTRA LAS PERSONAS O REGISTROS EN AN DE LA NOMINA      ******************************************************/
        /*****************************************************************************************************************************************************************************************************************************/
        public static string ObtenerConsultaDetallePersonalEnAN_GeneralDescentralizada(string An, int AnioInterface, string DelegacionesIncluidas, bool Sindicato, List<string> bancosContenidosEnAn)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }

            string condicionBancos = ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);

            /*Universo de los que deben de estar Foliados*/
            //string universoDatos = "select NUM   from interfaces2021.dbo.ANSE2120000489  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('05')";
            string universoDatos;
            if (Sindicato)
            {
                universoDatos = "select Substring(PARTIDA,2,5), NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X  from interfaces" + Anio + ".dbo." + An + "  where  "+condicionBancos+" and  sindicato = 1 and  deleg in " + DelegacionesIncluidas + " order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }
            else
            {
                universoDatos = "select Substring(PARTIDA,2,5), NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X  from interfaces" + Anio + ".dbo." + An + "  where  "+condicionBancos+" and  sindicato = 0 and  deleg in  " + DelegacionesIncluidas + " order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }

            return universoDatos;
        }



        public static string ObtenerConsultaDetallePersonalEnAN_OtrasNominasYPenA(string An, int AnioInterface, string DelegacionesIncluidas, bool EsPena, List<string> bancosContenidosEnAn)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }

            string condicionBancos = ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            string universoDatos;
            if (EsPena)
            {
                universoDatos = "select Substring(PARTIDA,2,5) , NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X    from interfaces"+Anio+".dbo."+An+"  where "+condicionBancos+"  and deleg in " + DelegacionesIncluidas + " order by JUZGADO, NOMBRE ";
            }
            else
            {
                universoDatos = "select Substring(PARTIDA,2,5) , NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X    from interfaces"+Anio+".dbo."+An+"  where "+condicionBancos+"  and deleg in " + DelegacionesIncluidas + " order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }


            return universoDatos;
        }


        /********************************************************************************************************************************************************************************************************************************/
        /**************************************************************              OBTIENE CONSULTA PARA SABER EL ORDEN EN EL QUE SE VA A FOLIAR LA DELEGACION SELECCIONADA               *********************************************/
        /********************************************************************************************************************************************************************************************************************************/
        public static string ObtenerConsultaOrdenDeFoliacionPorDelegacion_GeneralDescentralizada(string An, int AnioInterface, string DelegacionesIncluidas, bool Sindicato , List<string> bancosContenidosEnAn)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }

            string condicionBancos = ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            /* Universo de los que SE VAN A FOLIAR */
            string universoDatos;
            if (Sindicato)
            {
                universoDatos = "select PARTIDA, NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X, RFC   from interfaces" + Anio + ".dbo." + An + "  where  "+condicionBancos+"  and sindicato = 1 and deleg in "+DelegacionesIncluidas+" order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }
            else
            {
                universoDatos = "select PARTIDA, NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X, RFC   from interfaces" + Anio + ".dbo." + An + "  where  "+condicionBancos+" and sindicato = 0 and deleg in "+DelegacionesIncluidas+" order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }

            return universoDatos;
        }


        public static string ObtenerConsultaOrdenDeFoliacionPorDelegacion_OtrasNominasYPenA(string An, int AnioInterface, string DelegacionesIncluidas, bool EsPena, List<string> bancosContenidosEnAn)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            string condicionBancos = ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            /*Universo de los que deben de estar Foliados*/
            //string universoDatos = "select NUM   from interfaces2021.dbo.ANSE2120000489  where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('05')";
            string universoDatos;
            if (EsPena)
            {
                universoDatos = "select PARTIDA, NUM, Inter.NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X , RFC,  BENEF      from interfaces" + Anio + ".dbo." + An + " as Inter where  "+condicionBancos+"   and deleg in " + DelegacionesIncluidas + " order by JUZGADO, Inter.NOMBRE ";
            }
            else
            {
                universoDatos = "select PARTIDA, NUM, Inter.NOMBRE, DELEG, NUM_CHE, LIQUIDO , FolioCFDI, BANCO_X, CUENTA_X , RFC              from interfaces" + Anio + ".dbo." + An + " as Inter  where  "+condicionBancos+"  and deleg in " + DelegacionesIncluidas + " order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), Inter.NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
            }


            return universoDatos;
        }



        /*****************************************************************************************************************************************************************************************************************************************************************/
        /**************************************************************              OBTIENE CONSULTA PARA SABER QUE REGISTROS SE DEBEN DE BLANQUEAR CUANDO LA FOLIACION NO CUMPLA CON EL ESTANDART DE CALIDAD               *********************************************/
        /*****************************************************************************************************************************************************************************************************************************************************************/
        public static string ObtenerConsultaLimpiarRegistrosPorDelegacion_GeneralDescentralizada(string An, int AnioInterface, string DelegacionesIncluidas, bool Sindicato, List<string> bancosContenidosEnAn)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }

            string condicionBancos = ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            /* Universo de los que SE VAN A FOLIAR */
            string universoDatos;
            // No contiene el where ya que este se define en el metodo donde se hara la limpieza de los registros bajo esta condicion
            if (Sindicato)
            {
                universoDatos = "   "+condicionBancos+"  and sindicato = 1 and deleg in "+DelegacionesIncluidas+" ";
            }
            else
            {
                universoDatos = "   "+condicionBancos+" and sindicato = 0 and deleg in "+DelegacionesIncluidas+"  ";
            }

            return universoDatos;
        }


        public static string ObtenerConsultaLimpiarRegistrosPorDelegacion_OtrasNominasYPenA(string An, int AnioInterface, string DelegacionesIncluidas, bool EsPena, List<string> bancosContenidosEnAn)
        {
            string Anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                Anio = Convert.ToString(AnioInterface);
            }


            string condicionBancos = ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);

            // No contiene el where ya que este se define en el metodo donde se hara la limpieza de los registros bajo esta condicion
            string universoDatos;
            if (EsPena)
            {
                universoDatos = "   "+condicionBancos+"   and deleg in "+DelegacionesIncluidas+"  ";
            }
            else
            {
                universoDatos = "   "+condicionBancos+"  and deleg in "+DelegacionesIncluidas+"  ";
            }


            return universoDatos;
        }



    }
}
