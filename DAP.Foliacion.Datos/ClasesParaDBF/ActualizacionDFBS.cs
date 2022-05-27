using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPDC;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAP.Foliacion.Datos.ClasesParaDBF
{

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////     ACTUALIZA UN REGISTRO EN UN DBF       //////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public class ActualizacionDFBS
    {
       

        /// <summary>
        /// Metodo para actualizar una tabla en especifico pasando la ruta y el nombre del archivo, 
        /// </summary>
        /// <param name="RutaPath"></param>
        /// <param name="NombreArchivo"></param>
        /// <returns></returns>
        public static string ActualizarDBF_Sagitari_A_N(string RutaPath, string NombreArchivo, List<ResumenPersonalAFoliarDTO> ResumenPersonalFoliar)
         {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


            string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+RutaPath+";Extended Properties=dBASE 5.0;";

            int ElementosModificados = 0;

            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    con.Open();

                    foreach (ResumenPersonalAFoliarDTO nuevoRegistro in ResumenPersonalFoliar)
                    {
                       

                        string ExecutarQuery = "UPDATE ["+NombreArchivo+"] SET Num_che = '"+nuevoRegistro.NumChe+"', Banco_x = '"+nuevoRegistro.BancoX+"', Cuenta_x = '"+nuevoRegistro.CuentaX+"' , Observa = '"+nuevoRegistro.Observa+"' WHERE NUM = '"+nuevoRegistro.CadenaNumEmpleado+"' and RFC = '"+nuevoRegistro.RFC+"' and LIQUIDO = "+nuevoRegistro.Liquido+" and DELEG = '"+nuevoRegistro.Delegacion+"' ";
                    
                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);

                        int modificado = cmd.ExecuteNonQuery();

                        if (modificado > 0)
                        {
                            ElementosModificados += modificado;
                        }


                    }
                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();

                Debug.WriteLine("Ocurrio un error :" + E.Message.ToString());
            }
            return Convert.ToString(ElementosModificados);
        }




        public static string ActualizarDBF_Pagomatico_Prueba_NO_UTILIZABLE(string RutaPath, string NombreArchivo, List<ResumenPersonalAFoliarDTO> ResumenPersonalFoliar, bool EsPena)
        {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


           // string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + RutaPath +";Extended Properties=dBASE 5.0;";
           
            string constr = "Provider=VFPOLEDB.1 ;Data Source=" + RutaPath + "";
            int ElementosModificados = 0;

            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    con.Open();

                    foreach (ResumenPersonalAFoliarDTO nuevoRegistro in ResumenPersonalFoliar)
                    {
                        //if (nuevoRegistro.CadenaNumEmpleado == "22125") 
                        //{
                        //    int a  = 0;
                        //}

                        string ExecutarQuery = "";
                        if (EsPena)
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "'  and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' and Benef = '" + nuevoRegistro.NumBeneficiario + "' ";

                        }
                        else
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "' and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' ";

                        }



                        //string ExecutarQuery = "UPDATE ["+NombreArchivo+"] SET [Num_che = '"+nuevoRegistro.NumChe+"', Banco_x = '"+nuevoRegistro.BancoX+"', Cuenta_x = '"+nuevoRegistro.CuentaX+"' , Observa = '"+nuevoRegistro.Observa+"' WHERE NUM = '"+nuevoRegistro.CadenaNumEmpleado+"' and RFC = '"+nuevoRegistro.RFC+"' and LIQUIDO = "+nuevoRegistro.Liquido+" and DELEG = '"+nuevoRegistro.Delegacion+"' ";

                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        //var a2 = cmd.Parameters.AddWithValue("@RFC", nuevoRegistro.RFC);
                        int modificado = cmd.ExecuteNonQuery();

                        if (modificado > 0)
                        {
                            ElementosModificados += modificado;
                        }


                    }
                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();

                Debug.WriteLine("Ocurrio un error :" + E.Message.ToString());
            }
            return Convert.ToString(ElementosModificados);
        }










        public static async Task<string> ActualizarDBF_Pagomaticos(string RutaPath, string NombreArchivo, List<ResumenPersonalAFoliarDTO> ResumenPersonalFoliar, bool EsPena)
        {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


            // string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+RutaPath+";Extended Properties=dBASE 5.0;";
            string constr = "Provider=VFPOLEDB.1; Data Source=" + RutaPath + "";
            List<Task> tareas = new List<Task>();
            int ElementosModificados = 0;

            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {



                    await con.OpenAsync(); 
                    foreach (ResumenPersonalAFoliarDTO nuevoRegistro in ResumenPersonalFoliar)
                    {
                        

                        string ExecutarQuery = "";
                        if (EsPena)
                        {
                            ExecutarQuery = "UPDATE ["+NombreArchivo+"] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "' and RFC = '" + nuevoRegistro.RFC + "' and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' and Benef = '" + nuevoRegistro.NumBeneficiario + "' ";

                        }
                        else
                        {
                            ExecutarQuery = "UPDATE ["+NombreArchivo+"] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "' and RFC = '" + nuevoRegistro.RFC + "' and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' ";

                        }

                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        //var a2 = cmd.Parameters.AddWithValue("@RFC", nuevoRegistro.RFC);
                        int modificado = await cmd.ExecuteNonQueryAsync();

                        if (modificado == 1)
                        {
                            ElementosModificados += modificado;
                        }

                        //tareas.Add(new OleDbCommand(ExecutarQuery, con).ExecuteNonQueryAsync());

                    }




                      //await Task.WhenAll(tareas);

                    con.Close();

                  
                    //return Convert.ToString(tareas.Count());
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
               return E.Message.ToString();

                //Debug.WriteLine("Ocurrio un error :" + E.Message.ToString());
            }
           return Convert.ToString(ElementosModificados);
        }


        public static async Task<string> ActualizarDBF_Cheques(string RutaPath, string NombreArchivo, List<ResumenPersonalAFoliarDTO> ResumenPersonalFoliar, bool EsPena)
        {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


            // string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+RutaPath+";Extended Properties=dBASE 5.0;";
            string constr = "Provider=VFPOLEDB.1 ;Data Source=" + RutaPath + "";
            List<Task> tareas = new List<Task>();
            

            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {



                    await con.OpenAsync();
                    foreach (ResumenPersonalAFoliarDTO nuevoRegistro in ResumenPersonalFoliar)
                    {
                        string ExecutarQuery = "";
                        if (EsPena)
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "' and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' and Benef = '" + nuevoRegistro.NumBeneficiario + "' ";

                        }
                        else
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "' and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' ";

                        }

                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        //var a2 = cmd.Parameters.AddWithValue("@RFC", nuevoRegistro.RFC);
                        int modificado = await cmd.ExecuteNonQueryAsync();

                        if (modificado == 1)
                        {
                            totalActualizado += modificado;
                        }

                       // tareas.Add(new OleDbCommand(ExecutarQuery, con).ExecuteNonQueryAsync());

                    }

                   // await Task.WhenAll(tareas);

                     con.Close();


                   // return Convert.ToString(tareas.Count());
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();

                //Debug.WriteLine("Ocurrio un error :" + E.Message.ToString());
            }
            return Convert.ToString( totalActualizado);
        }


        public static string SuspenderPagomatico(string RutaPath, string NombreArchivo, Tbl_Pagos nuevoRegistro, bool EsPena, string CadenaNumEmpleado)
        {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


            // string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+RutaPath+";Extended Properties=dBASE 5.0;";
            string constr = "Provider=VFPOLEDB.1 ;Data Source=" + RutaPath + "";
            List<Task> tareas = new List<Task>();


            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    con.Open();
                   
                        string ExecutarQuery = "";
                        if (EsPena)
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '11111111',  Observa = 'TALON POR CHEQUE' , TALONXCH = 1 WHERE NUM = '" + CadenaNumEmpleado + "' and LIQUIDO = " + nuevoRegistro.ImporteLiquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' and Benef = '" + nuevoRegistro.NumBeneficiario + "' ";
                        }
                        else
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '11111111',  Observa = 'TALON POR CHEQUE' , TALONXCH = 1  WHERE NUM = '" + CadenaNumEmpleado + "' and LIQUIDO = " + nuevoRegistro.ImporteLiquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' ";
                        }

                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        int modificado = cmd.ExecuteNonQuery();

                        if (modificado == 1)
                        {
                            totalActualizado += modificado;
                        }

                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();

            }
            return Convert.ToString(totalActualizado);
        }

        public static string ReponerFormaPago(string RutaPath, string ExecutarQuery)
        {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


            // string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+RutaPath+";Extended Properties=dBASE 5.0;";
            string constr = "Provider=VFPOLEDB.1 ;Data Source=" + RutaPath + "";
            List<Task> tareas = new List<Task>();


            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                    int modificado = cmd.ExecuteNonQuery();

                    if (modificado == 1)
                    {
                        totalActualizado += modificado;
                    }

                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();

            }
            return Convert.ToString(totalActualizado);
        }



        /**********************************************************************************************************************************************************************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        /*************************************************************************************          INSERTAR DATOS PARA IPD e IPDcompensado EN DBF          ***************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        public static string LLenarIPD(string RutaPath, string NombreArchivo, List<IPDDTO> CargarIpd)
        {
            string constr = "Provider=VFPOLEDB.1 ;Data Source="+RutaPath+"";

            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                     con.Open();
                    foreach (IPDDTO nuevoRegistro in CargarIpd)
                    {
                        string ExecutarQuery = "INSERT INTO [" + NombreArchivo + "]  (referencia , tiponom , cve_presup, cvegto , cvepd , monto , tipoclave , adicional , partida , num , nombre , num_che , foliocfdi , deleg , idctabanca , idbanco , pagomat , tipo_pagom , numtarjeta , orden , quincena , nomalpha , fecha , cvegasto , cla_pto) VALUES ('"+nuevoRegistro.Referencia+"' , '"+nuevoRegistro.TipoNom+"' , '"+nuevoRegistro.Cve_presup+"' , '"+nuevoRegistro.Cvegto+"' , '"+nuevoRegistro.Cvepd+"' , "+nuevoRegistro.Monto+" , '"+nuevoRegistro.Tipoclave+"' , '"+nuevoRegistro.Adicional+"' , '"+nuevoRegistro.Partida+"' , '"+nuevoRegistro.Num+"' , '"+nuevoRegistro.Nombre+"' , '"+nuevoRegistro.Num_che+"' , "+nuevoRegistro.Foliocdfi+" , '"+nuevoRegistro.Deleg+"' , "+nuevoRegistro.Idctabanca+" , "+nuevoRegistro.IdBanco+", "+nuevoRegistro.Pagomat+" , '"+nuevoRegistro.Tipo_pagom+"'  ,  '"+nuevoRegistro.Numtarjeta+"' , "+nuevoRegistro.Orden+" , '"+nuevoRegistro.Quincena+"' , '"+nuevoRegistro.Nomalpha+"' , '"+nuevoRegistro.Fecha+"' , '"+nuevoRegistro.Cvegasto+"' , '"+nuevoRegistro.Cla_pto+"')";

                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        int modificado =  cmd.ExecuteNonQuery();

                        if (modificado == 1)
                        {
                            totalActualizado += modificado;
                        }

                    }
                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();
            }
            return Convert.ToString(totalActualizado);
        }



        /**********************************************************************************************************************************************************************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        /*************************************************************************************          INSERTAR DATOS PARA IPDcompensado EN DBF          ***************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        public static string LLenarIPDCompensado(string RutaPath, string NombreArchivo, List<IPDCDTO> CargarIpd)
        {
            string constr = "Provider=VFPOLEDB.1 ;Data Source=" + RutaPath + "";

            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    con.Open();
                    foreach (IPDCDTO nuevoRegistro in CargarIpd)
                    {
                        string ExecutarQuery = "INSERT INTO ["+NombreArchivo+"]  ( tiponom , cve_presup, cvegto ,  monto , tipoclave , num_che , cvereal, cvecompen , fecha , idctabanca , idbanco ,  num , nomalpha , quincena , adicional , cla_pto , ldf_6d ) VALUES ('"+nuevoRegistro.TipoNom+"' , '"+nuevoRegistro.Cve_presup+"' , '"+nuevoRegistro.CveGto+"' , "+nuevoRegistro.Monto+" , '"+nuevoRegistro.TipoClave+"' , '" + nuevoRegistro.Num_che + "' , '"+nuevoRegistro.CveReal+"', '"+nuevoRegistro.CveCompen+"' , '"+nuevoRegistro.fecha+"' , "+nuevoRegistro.IdctaBanca+" , "+nuevoRegistro.IdBanco+" , '"+nuevoRegistro.Num+"' , '"+nuevoRegistro.NomAlpha+"' , '"+nuevoRegistro.Quincena+"' , '"+nuevoRegistro.Adicional+"' , '"+nuevoRegistro.Cla_pto+"' , "+nuevoRegistro.Ldf_6d+")";

                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        int modificado = cmd.ExecuteNonQuery();

                        if (modificado == 1)
                        {
                            totalActualizado += modificado;
                        }

                    }
                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();
            }
            return Convert.ToString(totalActualizado);
        }



        /**********************************************************************************************************************************************************************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        /*************************************************************************************          Limpiar un registro de la base EN DBF para limpiar campos           ***************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        /**********************************************************************************************************************************************************************************************************************************************/
        public static string LimpiarUnRegitroCamposFoliacionBaseDBF(string RutaPath, string NombreArchivo, bool EsPena, string numEmpleado5Digitos , decimal ImporteLiquido , string Delegacion , string NumBeneficiario)
        {
            string constr = "Provider=VFPOLEDB.1 ;Data Source=" + RutaPath + "";

            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    string ExecutarQuery = "";
                    if (EsPena)
                    {
                        ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET NUM_CHE = '' , BANCO_X = '' , CUENTA_X = '' , OBSERVA = ''  WHERE NUM = '"+numEmpleado5Digitos + "' and LIQUIDO = "+ImporteLiquido+" and DELEG = '"+Delegacion+"' and Benef = '"+NumBeneficiario+"' ";
                    }
                    else
                    {
                        ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET NUM_CHE = '' , BANCO_X = '' , CUENTA_X = '' , OBSERVA = ''   WHERE NUM = '"+numEmpleado5Digitos+"' and LIQUIDO = "+ImporteLiquido+" and DELEG = '"+Delegacion+"' ";
                    }


                    con.Open();
                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        int modificado = cmd.ExecuteNonQuery();

                        if (modificado == 1)
                        {
                            totalActualizado += modificado;
                        }

                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();
            }
            return Convert.ToString(totalActualizado);
        }




    }


}
