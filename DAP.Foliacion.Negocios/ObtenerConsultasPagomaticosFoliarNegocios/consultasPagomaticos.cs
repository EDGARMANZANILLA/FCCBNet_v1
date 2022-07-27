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



        public static string ValidaBancosExistentenEnNominaSeleccionada_FoliacionPagomaticos(List<Tbl_CuentasBancarias> BancosSelecionados, List<string> BancosContenidosEnAN)
        {
            string condicion = "";
            int i = 0;
            foreach (string Nuevobanco in BancosContenidosEnAN)
            {
                Tbl_CuentasBancarias bancoEncontradoEnCampo = BancosSelecionados.Where(x => x.NombreCampoEn_AN == Nuevobanco).FirstOrDefault();

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









        public static string ObtenerRegitrosFoliados_NominaPagomatico(string AN, int AnioInterface)
        {
                List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN( AN, AnioInterface);

                string condicionDeBancos = ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);

                string query = "";
                    if (AnioInterface == Convert.ToInt32(DateTime.Now.Year))
                    {
                        query = "select count(*) from interfaces.dbo."+AN+ " where (Num_che <> '' or  OBSERVA <> '' or BANCO_X <> '' or CUENTA_X <> '') and ("+condicionDeBancos+")";
                    }
                    else
                    {
                        query = " select count(*) from interfaces"+AnioInterface+".dbo."+AN+ " where (Num_che <> '' or  OBSERVA <> '' or BANCO_X <> '' or CUENTA_X <> '') and ("+condicionDeBancos+") ";
                    }
                
            return query;
        }




        public static string ObtenerRegistrosAFoliar_NominaPagomatico(string AN, int AnioInterface)
        {
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(AN, AnioInterface);
            string condicionDeBancos = ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);
            string query = "";

                    if (AnioInterface == Convert.ToInt32(DateTime.Now.Year))
                    {
                        query = " select count(*) from interfaces.dbo." +AN+" where "+condicionDeBancos+" ";
                    }
                    else
                    {
                        query = "select count(*) from interfaces" + AnioInterface + ".dbo."+AN+" where "+condicionDeBancos+" ";
                    }
            
            return query;
        }



        public static string ObtenerConsultaDetalleDatosPersonalesNomina_ReportePagomatico(string AN, int AnioInterface)
        {
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(AN, AnioInterface);
            string condicionDeBancos = ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);
            string query = "";


            if (AnioInterface == Convert.ToInt32(DateTime.Now.Year))
            {
                query = "SELECT Substring(PARTIDA,2,5), NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X  FROM interfaces.dbo." + AN + " where " + condicionDeBancos + " ";
            }
            else
            {
                query = "SELECT Substring(PARTIDA,2,5), NUM, NOMBRE, DELEG, NUM_CHE, LIQUIDO, BANCO_X, CUENTA_X  FROM interfaces" + AnioInterface + ".dbo." + AN + " where " + condicionDeBancos + " ";
            }

            return query;

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



    }
}

