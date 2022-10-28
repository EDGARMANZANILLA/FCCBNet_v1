using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Datos;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Negocios.ObtenerConsultasChequesFoliarNegocios;

namespace DAP.Foliacion.Negocios.ObtenerConsultasPagomaticosFoliarNegocios
{
    public class consultasPagomaticos
    {

        public static string ConvertirListaBancosEnCondicionParaPagomaticos(List<string> bancosContenidosEnAn)
        {
            string condicion = "";
            int i = 0;
            foreach (string banco in bancosContenidosEnAn)
            {
                i += 1;

                if (i == 1)
                {
                    condicion += condicion = " "+banco+" <> '' ";
                }
                else
                {
                    condicion += condicion = " or "+banco+" <> '' ";
                }
            }
            return condicion;
        }


        public static string ConvertirListaBancosEnCondicionPara_FoliacionPagomaticos(List<Tbl_CuentasBancarias> BancosSelecionados)
        {
            string condicion = "";
            int i = 0;
            foreach (Tbl_CuentasBancarias nombreCampoBanco in BancosSelecionados)
            {
                i += 1;

                if (i == 1)
                {
                    condicion += condicion = "case when "+nombreCampoBanco.NombreCampoEn_AN+" <> '' then '"+nombreCampoBanco.Id+"' ";
                }
                else
                {
                    condicion += condicion = " when "+nombreCampoBanco.NombreCampoEn_AN+" <> '' then '" + nombreCampoBanco.Id + "' ";
                }
            }
            return condicion += condicion = "end as 'IdCuentaBancariaFoliacion'"; ;
        }



        public static string ValidaBancosExistentenEnNominaSeleccionada_FoliacionPagomaticos(List<Tbl_CuentasBancarias> BancosDisponiblesAnio, List<string> BancosContenidosEnAN)
        {
            string condicion = "";
            int i = 0;
            foreach (string Nuevobanco in BancosContenidosEnAN)
            {
                Tbl_CuentasBancarias bancoEncontradoEnCampo = BancosDisponiblesAnio.Where(x => x.NombreCampoEn_AN == Nuevobanco).FirstOrDefault();

                if (bancoEncontradoEnCampo != null) 
                {
                    i += 1;

                    if (i == 1)
                    {
                        condicion += condicion = "case when " + bancoEncontradoEnCampo.NombreCampoEn_AN + " <> '' then '" + bancoEncontradoEnCampo.Id + "' ";
                    }
                    else
                    {
                        condicion += condicion = " when " + bancoEncontradoEnCampo.NombreCampoEn_AN + " <> '' then '" + bancoEncontradoEnCampo.Id + "' ";
                    }
                }
            }
            return condicion += condicion = "end as 'IdCuentaBancariaFoliacion'"; ;
        }









        public static string ObtenerRegitrosFoliados_NominaPagomatico(string visitaAnioInterface, string AN, string condicionBancosParaSerPagomatico)
        {
            return " select count(*) from interfaces"+visitaAnioInterface+".dbo." + AN + " where (Num_che <> '' or  OBSERVA <> '' or BANCO_X <> '' or CUENTA_X <> '') and ("+condicionBancosParaSerPagomatico+") ";
        }




        public static string ObtenerRegistrosAFoliar_NominaPagomatico(string visitaAnioInterface, string AN, string condicionBancosParaSerPagomatico)
        {
            return "select count(*) from interfaces"+visitaAnioInterface+".dbo."+AN+" where "+condicionBancosParaSerPagomatico+" ";
        }



        public static string ObtenerConsultaDetalleDatosPersonalesNomina_ReportePagomatico(string visitaAnioInterface, string AN, string condicionBancosParaSerPagomatico)
        {
        
            return "SELECT Substring(PARTIDA,2,5), NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X , OBSERVA FROM interfaces" + visitaAnioInterface+".dbo."+AN+" where  "+condicionBancosParaSerPagomatico+"  order by num ";
        }



        /*****************************************************************************************************************************************************************************************/
        /********************************                   CONSULTA PARA LA FOLIACION DE UN NOMINA CON PAGOMATICOS                ***************************************************************/
        /*****************************************************************************************************************************************************************************************/
        public static string ObtenerConsultaDetalle_FoliacionPagomatico(string AN, int AnioInterface ,  bool EsPenA , List<Tbl_CuentasBancarias> BancosSelecionados)
        {
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(AN, AnioInterface);
            string condicionDeBancos = ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);

            string condicionDeIdCuentaBancaria = ValidaBancosExistentenEnNominaSeleccionada_FoliacionPagomaticos(BancosSelecionados, bancosContenidosEnAn);


           // string condicionDeIdCuentaBancaria = ConvertirListaBancosEnCondicionPara_FoliacionPagomaticos(BancosSelecionados);
            
            string query = "";


            if (AnioInterface == Convert.ToInt32(DateTime.Now.Year))
            {
                if (EsPenA)
                {
                    query = "select   NUM, RFC, NOMBRE, LIQUIDO, " + condicionDeIdCuentaBancaria + " , DELEG , Partida , FolioCFDI , BENEF 'BENEFICIARIO'  from interfaces.dbo." + AN + " where  " + condicionDeBancos + " ORDER BY NUM ";
                }
                else 
                {
                    query = "select   NUM, RFC, NOMBRE, LIQUIDO, " + condicionDeIdCuentaBancaria + " , DELEG , Partida , FolioCFDI  from interfaces.dbo." + AN + " where  " + condicionDeBancos + " ORDER BY NUM ";
                }
            }
            else
            {
                if (EsPenA)
                {
                    query = "select   NUM, RFC, NOMBRE, LIQUIDO, " + condicionDeIdCuentaBancaria + " , DELEG , Partida , FolioCFDI , BENEF 'BENEFICIARIO' from interfaces" + AnioInterface + ".dbo." + AN + " where  " + condicionDeBancos + "  ORDER BY NUM ";
                }
                else
                {
                    query = "select   NUM, RFC, NOMBRE, LIQUIDO, " + condicionDeIdCuentaBancaria + " , DELEG , Partida , FolioCFDI  from interfaces" + AnioInterface + ".dbo." + AN + " where  " + condicionDeBancos + "  ORDER BY NUM ";
                }
            }

            return query;

        }


        /******************************************************************************************************************************************************************************************/
        /********************************                   CONSULTA PARA LA REFOLIACION DE UN NOMINA CON PAGOMATICOS                **************************************************************/
        /******************************************************************************************************************************************************************************************/
        public static string ObtenerConsultaDetalle_ReFoliacionPagomatico(string visitaAnioInterface , string AN,  bool EsPenA, string condicionDeIdCuentaBancaria, string condicionBancosParaSerPagomatico)
        {
        //    List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(AN, AnioInterface);
        //    string condicionDeBancos = ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);

        //    string condicionDeIdCuentaBancaria = ValidaBancosExistentenEnNominaSeleccionada_FoliacionPagomaticos(BancosSelecionados, bancosContenidosEnAn);


            // string condicionDeIdCuentaBancaria = ConvertirListaBancosEnCondicionPara_FoliacionPagomaticos(BancosSelecionados);

            string query = "";

            if (EsPenA)
            {
                query = "select   NUM, RFC, NOMBRE, LIQUIDO, "+condicionDeIdCuentaBancaria+" , DELEG , Partida , FolioCFDI , BENEF 'BENEFICIARIO'  from interfaces"+visitaAnioInterface+".dbo."+AN+" where  "+condicionBancosParaSerPagomatico+" ORDER BY NUM ";
            }
            else
            {
                query = "select   NUM, RFC, NOMBRE, LIQUIDO, "+condicionDeIdCuentaBancaria+" , DELEG , Partida , FolioCFDI  from interfaces"+visitaAnioInterface+".dbo."+AN+" where  "+condicionBancosParaSerPagomatico+" ORDER BY NUM ";
            }


            return query;

        }


    }
}

