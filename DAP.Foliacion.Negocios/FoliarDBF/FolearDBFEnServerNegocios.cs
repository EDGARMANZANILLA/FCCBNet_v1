using DAP.Foliacion.Datos.ClasesParaDBF;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Negocios.FoliarDBF
{
    public class FolearDBFEnServerNegocios
    {
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        //******************************************************* PERMISOS PARA ACCEDER A LOS ARCHIVOS DE UN SERVER  *********************************************************//
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        /* PERMISOS  */
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);



        public static string SuspenderPagoEnRutaDBF(DatosCompletosBitacoraDTO datosNominaCompleto , Tbl_Pagos suspenderPago , string CadenaNumEmpleado)
        {
            WindowsImpersonationContext impersonationContext = null;
            IntPtr userHandle = IntPtr.Zero;
            const int LOGON_TYPE_NEW_CREDENTIALS = 9;
            const int LOGON32_PROVIDER_WINNT50 = 3;
            // string domain = @"\\172.19.3.171\";
            // string user = "finanzas" + @"\" + "diego.ruz";
            // string password = "Analista101";
            string domain = @"\\172.19.3.173\";
            string user = "Administrador";
            string password = "Procesosnomina1";

            if (domain == "")
                domain = System.Environment.MachineName;
            // Llame a LogonUser para obtener un token para el usuario
            bool loggedOn = LogonUser(user,
                                        domain,
                                        password,
                                        LOGON_TYPE_NEW_CREDENTIALS,
                                        LOGON32_PROVIDER_WINNT50,
                                        ref userHandle);

            if (!loggedOn)
            {
                return "No se obtuvo permiso al Servidor";

            }

            string resultado_ActualizacionDBF;
            using (new Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
            {

                //string pathBasesServidor47 = @"\\172.19.3.173\f2\";
                string pathBasesServidor47 = @"\\172.19.3.173\";



                string letraRuta = datosNominaCompleto.Ruta.Substring(0, 2);

                //Si esta en Modo debug entrara
                //if (System.Diagnostics.Debugger.IsAttached)
                //{
                if (letraRuta.ToUpper() == "F:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "F2";

                }
                else if (letraRuta.ToUpper() == "J:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "J2";
                }
                //}
                //else
                //{
                //    if (letraRuta.ToUpper() == "F:")
                //    {
                //        pathBasesServidor47 = pathBasesServidor47 + "F";

                //    }
                //    else if (letraRuta.ToUpper() == "J:")
                //    {
                //        pathBasesServidor47 = pathBasesServidor47 + "J";
                //    }

                //}
                datosNominaCompleto.Ruta = datosNominaCompleto.Ruta.Replace("" + letraRuta + "", ""); // \SAGITARI\AYUDAS\ARCHIVOS\            
                datosNominaCompleto.Ruta = pathBasesServidor47 + datosNominaCompleto.Ruta; //\\172.19.3.173\F\SAGITARI\AYUDAS\ARCHIVOS\


                resultado_ActualizacionDBF = ActualizacionDFBS.SuspenderPagomatico(datosNominaCompleto.Ruta, datosNominaCompleto.RutaNomina, suspenderPago, datosNominaCompleto.EsPenA, CadenaNumEmpleado);
               // resultado_ActualizacionDBF = NuevaActualizacionDFBS.SuspenderPagomatico(datosNominaCompleto.Ruta, datosNominaCompleto.RutaNomina, suspenderPago, datosNominaCompleto.EsPenA, CadenaNumEmpleado);
              
            }

            return resultado_ActualizacionDBF;
        }


        public static string ReponerPagoEnRutaDBF(string rutaRedServidor, string ExecutarQuery)
        {
            WindowsImpersonationContext impersonationContext = null;
            IntPtr userHandle = IntPtr.Zero;
            const int LOGON_TYPE_NEW_CREDENTIALS = 9;
            const int LOGON32_PROVIDER_WINNT50 = 3;
            // string domain = @"\\172.19.3.171\";
            // string user = "finanzas" + @"\" + "diego.ruz";
            // string password = "Analista101";
            string domain = @"\\172.19.3.173\";
            string user = "Administrador";
            string password = "Procesosnomina1";

            if (domain == "")
                domain = System.Environment.MachineName;
            // Llame a LogonUser para obtener un token para el usuario
            bool loggedOn = LogonUser(user,
                                        domain,
                                        password,
                                        LOGON_TYPE_NEW_CREDENTIALS,
                                        LOGON32_PROVIDER_WINNT50,
                                        ref userHandle);

            if (!loggedOn)
            {
                return "No se obtuvo permiso al Servidor";
            }

            string resultado_ActualizacionDBF;
            using (new Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
            {

                //string pathBasesServidor47 = @"\\172.19.3.173\f2\";
                string pathBasesServidor47 = @"\\172.19.3.173\";



                string letraRuta = rutaRedServidor.Substring(0, 2);

                //Si esta en Modo debug entrara
                //if (System.Diagnostics.Debugger.IsAttached)
                //{
                if (letraRuta.ToUpper() == "F:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "F2";

                }
                else if (letraRuta.ToUpper() == "J:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "J2";
                }
                //}
                //else
                //{
                //    if (letraRuta.ToUpper() == "F:")
                //    {
                //        pathBasesServidor47 = pathBasesServidor47 + "F";

                //    }
                //    else if (letraRuta.ToUpper() == "J:")
                //    {
                //        pathBasesServidor47 = pathBasesServidor47 + "J";
                //    }

                //}
                rutaRedServidor = rutaRedServidor.Replace("" + letraRuta + "", ""); // \SAGITARI\AYUDAS\ARCHIVOS\            
                rutaRedServidor = pathBasesServidor47 + rutaRedServidor; //\\172.19.3.173\F\SAGITARI\AYUDAS\ARCHIVOS\


                resultado_ActualizacionDBF = ActualizacionDFBS.ReponerFormaPago(rutaRedServidor, ExecutarQuery);
              //  resultado_ActualizacionDBF = NuevaActualizacionDFBS.ReponerFormaPago(rutaRedServidor, ExecutarQuery);

            }

            return resultado_ActualizacionDBF;
        }


        /** Limpia la base en DBF porque se trata de recuperar el folio con el que se folio pero estaba mal, de esta manera se refoliara **/
        public static string LimpiarRegitroBaseDBF(string pathRuta, string numeroEmpleado5Digitos, DatosCompletosBitacoraDTO datosCompletosNomina, Tbl_Pagos pagoEncontrado)
        {
            string resultado_ActualizacionDBF = "";
            /******************************************************************************/
            //Foliar en DBF

            int numeroRegistrosActualizados_BaseDBF = 0;
            int numeroRegistrosActualizados_AlPHA = 0;
            int registrosInsertadosOActualizados_Foliacion = 0;

            WindowsImpersonationContext impersonationContext = null;
            IntPtr userHandle = IntPtr.Zero;
            const int LOGON_TYPE_NEW_CREDENTIALS = 9;
            const int LOGON32_PROVIDER_WINNT50 = 3;
            // string domain = @"\\172.19.3.171\";
            // string user = "finanzas" + @"\" + "diego.ruz";
            // string password = "Analista101";
            string domain = @"\\172.19.3.173\";
            string user = "Administrador";
            string password = "Procesosnomina1";

            if (domain == "")
                domain = System.Environment.MachineName;
            // Llame a LogonUser para obtener un token para el usuario
            bool loggedOn = LogonUser(user,
                                        domain,
                                        password,
                                        LOGON_TYPE_NEW_CREDENTIALS,
                                        LOGON32_PROVIDER_WINNT50,
                                        ref userHandle);

            if (!loggedOn)
            {
                return "No se obtuvo permiso al Servidor";
            }




            using (new Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
            {
                //string pathBasesServidor47 = @"\\172.19.3.173\f2\";
               // string pathBasesServidor47 = domain;
                string pathBasesServidor47 = @"\\172.19.3.173\";

                string letraRuta = pathRuta.Substring(0, 2);



                //Si esta en Modo debug entrara
                //if (System.Diagnostics.Debugger.IsAttached)
                //{
                if (letraRuta.ToUpper() == "F:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "F2";

                }
                else if (letraRuta.ToUpper() == "J:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "J2";
                }
                //}
                //else
                //{
                //    if (letraRuta.ToUpper() == "F:")
                //    {
                //        pathBasesServidor47 = pathBasesServidor47 + "F";

                //    }
                //    else if (letraRuta.ToUpper() == "J:")
                //    {
                //        pathBasesServidor47 = pathBasesServidor47 + "J";
                //    }

                //}

                //Cuando termina esta if queda algo como  @"\\172.19.3.173\F";
                pathRuta = pathRuta.Replace("" + letraRuta + "", ""); // \SAGITARI\AYUDAS\ARCHIVOS\            
                pathRuta = pathBasesServidor47 + pathRuta; //\\172.19.3.173\F\SAGITARI\AYUDAS\ARCHIVOS\





                 resultado_ActualizacionDBF = ActualizacionDFBS.LimpiarUnRegitroCamposFoliacionBaseDBF(pathRuta /*datosCompletosNomina.Ruta*/, datosCompletosNomina.RutaNomina /*NombreArchivo*/, datosCompletosNomina.EsPenA, numeroEmpleado5Digitos, pagoEncontrado.ImporteLiquido, pagoEncontrado.Delegacion, pagoEncontrado.NumBeneficiario);

                if (resultado_ActualizacionDBF.Contains("Cannot open file"))
                {
                    pathRuta = pathRuta.Replace("" + pathBasesServidor47 + "", "***.**.**.**");
                   
                    return "LA BASE : || " + pathRuta + datosCompletosNomina.RutaNomina + " || SE ENCUENTRA ABIERTA EN OTRA PC";
                   
                }

            }

            return resultado_ActualizacionDBF;
        }


        public static AlertasAlFolearPagomaticosDTO ValidaExistenciaONoEsteAbierta(string contextoDBF , DatosCompletosBitacoraDTO datosCompletosNomina)
        {

            AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();

            if (contextoDBF.Contains("Cannot open file"))
            {
                nuevaAlerta.IdAtencion = 4;
                nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                nuevaAlerta.Detalle = "LA BASE : || " + datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina + " || SE ENCUENTRA ABIERTA EN OTRA PC";
                nuevaAlerta.Solucion = "CIERRE LA BASE E INTENTE FOLIAR DE NUEVO";
                return nuevaAlerta;
            }
            else if (contextoDBF.Contains("does not exist."))
            {
                nuevaAlerta.IdAtencion = 4;
                nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                nuevaAlerta.Detalle = "LA BASE : || " + datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina + " || NO EXISTE EN EL SERVER ";
                nuevaAlerta.Solucion = "COMUNIQUESE CON EL DESARROLLADOR INMEDIATAMENTE";
                return nuevaAlerta;
            }
            else if (contextoDBF.Contains("Invalid path or file name."))
            {
                nuevaAlerta.IdAtencion = 4;
                nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                nuevaAlerta.Detalle = "LA BASE : || " + datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina + " || RUTA INVALIDA DE LA DBF DENTRO DEL SERVIDOR ";
                nuevaAlerta.Solucion = "COMUNIQUESE CON EL DESARROLLADOR INMEDIATAMENTE";
                return nuevaAlerta;
            }

            if (nuevaAlerta.IdAtencion > 0)
            {
                return nuevaAlerta;
            }
            else 
            {
                return null;
            }

        }






        public static AlertasAlFolearPagomaticosDTO BaseDBF_CruceroParaConexionServer47DBF(int OpcionARedireccionar, DatosCompletosBitacoraDTO datosNominaCompleto, List<ResumenPersonalAFoliarDTO> resumenPersonalAFoliar, string Condicion )
        {
            /************************************************************************/
            /**   1 == FOLIACION PARA AMBAS FORMAS DE PAGO (PAGAMATICO Y CHEQUES)  **/
            /**   2 == **/
            /**   3 == **/
            /**   4 == **/
            /**   5 == **/
            /**   6 == LIMPIEZA DE CAMPOS DBF CONDICIONADOS CUANDO LA FOLIACION DEL PUNTO 1 NO CUMPLE CON EL ESTANDART PARA AMBAS FORMAS DE PAGO**/
            /**   7 == **/
            /**   8 == **/
            /**   200 == TODO SALIO CORRECTO Y SIN NINGUN ERROE EN LA BASE **/
            /***********************************************************************/
            AlertasAlFolearPagomaticosDTO adver = new AlertasAlFolearPagomaticosDTO();

            WindowsImpersonationContext impersonationContext = null;
            IntPtr userHandle = IntPtr.Zero;
            const int LOGON_TYPE_NEW_CREDENTIALS = 9;
            const int LOGON32_PROVIDER_WINNT50 = 3;
            string domain = @"\\172.19.3.173\";
            string user = "Administrador";
            string password = "Procesosnomina1";

            if (domain == "")
                domain = System.Environment.MachineName;
            // Llame a LogonUser para obtener un token para el usuario
            bool loggedOn = LogonUser(user,
                                        domain,
                                        password,
                                        LOGON_TYPE_NEW_CREDENTIALS,
                                        LOGON32_PROVIDER_WINNT50,
                                        ref userHandle);

            if (!loggedOn)
            {
                adver.IdAtencion = 4;
                adver.Id_Nom = Convert.ToString(datosNominaCompleto.Id_nom);
                adver.Detalle = "NO SE OBTUVO PERMISO AL SERVIDOR";
                adver.Solucion = "COMUNIQUESE CON EL DESARROLLADOR INMEDIATAMENTE";
                return adver;
            }

            string resultado_OperacionDBF;
            using (new Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
            {
                string pathBasesServidor47 = @"\\172.19.3.173\";

                string letraRuta = datosNominaCompleto.Ruta.Substring(0, 2);
                if (letraRuta.ToUpper() == "F:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "F2";

                }
                else if (letraRuta.ToUpper() == "J:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "J2";
                }

                datosNominaCompleto.Ruta = datosNominaCompleto.Ruta.Replace("" + letraRuta + "", ""); // \SAGITARI\AYUDAS\ARCHIVOS\            
                datosNominaCompleto.Ruta = pathBasesServidor47 + datosNominaCompleto.Ruta; //\\172.19.3.173\F\SAGITARI\AYUDAS\ARCHIVOS\



                //  CRUCERO DE REDIRECCIONAMIENTO HACIA EL METODO CORRECTO AL QUE DEBEN IR LOS DATOS  //
                switch (OpcionARedireccionar)
                {
                    case 1:
                        /*****************************************************************************************************************************************************************************************************************************/
                        /***********************************************************        LOGICA DE DIRRECCIONAMIENTO HACIA UN TRABAJO EN ESPECIFICO DE UNA DBF       ******************************************************************************/
                        /*****************************************************************************************************************************************************************************************************************************/
                        /**     FOLIACION PAGOMATICO O FOLICION CON CHEQUES     =>  FUNCIONA GRACIAS A QUE REALMENTE SOLO SE ACTUALIZAN LOS MISMOS CAMPOS PARA AMBOS FORMAS DE PAGO             **/
                        /**     SOLO CAMBIAN LOS DATOS PERO ESO ESTA EN EL NEGOCION DE CADA METODO DE ACUERDO A LA FORMA DE PAGO                                                                **/
                        resultado_OperacionDBF = ActualizacionDFBS.FoliarBaseEnDBFTodasFormasPago(datosNominaCompleto.Ruta, datosNominaCompleto.RutaNomina, resumenPersonalAFoliar, datosNominaCompleto.EsPenA);
                        AlertasAlFolearPagomaticosDTO alertaEncontrada = FolearDBFEnServerNegocios.ValidaExistenciaONoEsteAbierta(resultado_OperacionDBF, datosNominaCompleto);
                        if (alertaEncontrada != null)
                        {
                            return alertaEncontrada;
                        }


                        adver.IdAtencion = 200;
                        adver.NumeroRegistrosActualizados = Convert.ToInt32(resultado_OperacionDBF);
                        return adver;
                        

                    case 2:
                        // number = "Two";
                        break; 
                    case 3:
                        //number = "three";
                        break; 
                    case 4:
                        //number = "Four";
                        break; 
                    case 5:
                        //number = "Five";
                        break; 
                    case 6:
                        resultado_OperacionDBF = ActualizacionDFBS.LimpiarBaseDBF_IncumplimientoCalidadFoliacion(datosNominaCompleto.Ruta, datosNominaCompleto.RutaNomina, Condicion);
                        AlertasAlFolearPagomaticosDTO alertaEncontradaLimpieza = FolearDBFEnServerNegocios.ValidaExistenciaONoEsteAbierta(resultado_OperacionDBF, datosNominaCompleto);
                        if (alertaEncontradaLimpieza != null)
                        {
                            //Si existe un error esque no se termino de limpiar la base con condicion correctamente por ende solo se cambia el texto que se le informara al usuario 
                            alertaEncontradaLimpieza.Detalle += alertaEncontradaLimpieza.Detalle + " || HUBO UN ERROR EN FOLIACION Y LA BASE NO SE PUDO TERMINAR DE LIMPIAR";
                            alertaEncontradaLimpieza.Solucion += alertaEncontradaLimpieza.Solucion + " || INTENTE BLANQUEAR LA BASE MANUALMENTE Y REPITA EL PROCESO DE FOLIACION";

                            return alertaEncontradaLimpieza;
                        }


                        adver.IdAtencion = 200;
                        adver.NumeroRegistrosActualizados = Convert.ToInt32(resultado_OperacionDBF);
                        return adver;

                    case 7:
                        //number = "Seven";
                        break; 
                    case 8:
                        //number = "eight";
                        break; 
                    case 9:
                        //number = "nine";
                        break; 
                    case 10:
                        //number = "Ten";
                        break;
                }

            }

            return adver;
        }











        public static AlertasAlFolearPagomaticosDTO ModificacionDBF_SuspenderReponer_YLimpiarUnRegistro(int OpcionARedireccionar, DatosCompletosBitacoraDTO datosNominaCompleto, string CadenaNumEmpleado,  Tbl_Pagos PagoAModificar , string ReposicionNuevoFolio)
        {
            /************************************************************************/
            /**   1 ==  SUSPENDE UNA DISPERCION   **/
            /**   2 ==  REPONER UNA FORMA DE PAGO (ES UN CAMBIO DE FOLIO YA QUE CONTINUA SIENDO DEL MISMO BANCO )    **/
            /**   3 ==  LIMPIAR UN REGISTROS DE LA DBF BAJO CIERTAS CONDICIONES **/
            /***********************************************************************/

            AlertasAlFolearPagomaticosDTO advertencia = new AlertasAlFolearPagomaticosDTO();
            WindowsImpersonationContext impersonationContext = null;
            IntPtr userHandle = IntPtr.Zero;
            const int LOGON_TYPE_NEW_CREDENTIALS = 9;
            const int LOGON32_PROVIDER_WINNT50 = 3;
            // string domain = @"\\172.19.3.171\";
            // string user = "finanzas" + @"\" + "diego.ruz";
            // string password = "Analista101";
            string domain = @"\\172.19.3.173\";
            string user = "Administrador";
            string password = "Procesosnomina1";

            if (domain == "")
                domain = System.Environment.MachineName;
            // Llame a LogonUser para obtener un token para el usuario
            bool loggedOn = LogonUser(user,
                                        domain,
                                        password,
                                        LOGON_TYPE_NEW_CREDENTIALS,
                                        LOGON32_PROVIDER_WINNT50,
                                        ref userHandle);

            if (!loggedOn)
            {
                advertencia.IdAtencion = 4;
                advertencia.Id_Nom = Convert.ToString(datosNominaCompleto.Id_nom);
                advertencia.Detalle = "NO SE OBTUVO PERMISO AL SERVIDOR";
                advertencia.Solucion = "COMUNIQUESE CON EL DESARROLLADOR INMEDIATAMENTE";
                return advertencia;
            }


            string resultado_ActualizacionDBF= "0";
            using (new Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
            {

                //string pathBasesServidor47 = @"\\172.19.3.173\f2\";
                string pathBasesServidor47 = @"\\172.19.3.173\";



                string letraRuta = datosNominaCompleto.Ruta.Substring(0, 2);

                if (letraRuta.ToUpper() == "F:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "F2";

                }
                else if (letraRuta.ToUpper() == "J:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "J2";
                }
           
                datosNominaCompleto.Ruta = datosNominaCompleto.Ruta.Replace("" + letraRuta + "", ""); // \SAGITARI\AYUDAS\ARCHIVOS\            
                datosNominaCompleto.Ruta = pathBasesServidor47 + datosNominaCompleto.Ruta; //\\172.19.3.173\F\SAGITARI\AYUDAS\ARCHIVOS\



                //  CRUCERO DE REDIRECCIONAMIENTO HACIA EL METODO CORRECTO AL QUE DEBEN IR LOS DATOS  //
                switch (OpcionARedireccionar)
                {
                    /*****************************************************************************************************************************************************************************************************************************/
                    /***********************************************************        LOGICA DE DIRRECCIONAMIENTO HACIA UN TRABAJO EN ESPECIFICO DE UNA DBF       ******************************************************************************/
                    /*****************************************************************************************************************************************************************************************************************************/
                    case 1:
                        /**     SUSPENDER UNA DISPERCION      **/
                        resultado_ActualizacionDBF = ActualizacionDFBS.SuspenderPagomatico(datosNominaCompleto.Ruta, datosNominaCompleto.RutaNomina, PagoAModificar, datosNominaCompleto.EsPenA, CadenaNumEmpleado);
                        advertencia = FolearDBFEnServerNegocios.ValidaExistenciaONoEsteAbierta(resultado_ActualizacionDBF, datosNominaCompleto);
                        break;
                    case 2:
                        /**     REPONER UNA FORMA DE PAGO (ES UN CAMBIO DE FOLIO YA QUE CONTINUA SIENDO DEL MISMO BANCO )      **/
                        resultado_ActualizacionDBF = ActualizacionDFBS.ReponerCheque_MEJORADO(datosNominaCompleto.Ruta, datosNominaCompleto.RutaNomina, PagoAModificar, datosNominaCompleto.EsPenA, CadenaNumEmpleado, ReposicionNuevoFolio);
                        advertencia = FolearDBFEnServerNegocios.ValidaExistenciaONoEsteAbierta(resultado_ActualizacionDBF, datosNominaCompleto);
                        break;
                    case 3:
                        /**     LIMPIAR UN REGISTROS DE LA DBF BAJO CIERTAS CONDICIONES       **/
                        resultado_ActualizacionDBF = ActualizacionDFBS.LimpiarUnRegitroCamposFoliacionBaseDBF(datosNominaCompleto.Ruta, datosNominaCompleto.RutaNomina /*NombreArchivo*/, datosNominaCompleto.EsPenA, CadenaNumEmpleado, PagoAModificar.ImporteLiquido, PagoAModificar.Delegacion, PagoAModificar.NumBeneficiario);
                        advertencia = FolearDBFEnServerNegocios.ValidaExistenciaONoEsteAbierta(resultado_ActualizacionDBF, datosNominaCompleto);
                        break;
                }

            }


            if (advertencia != null)
            {
                return advertencia;
            }

            advertencia = new AlertasAlFolearPagomaticosDTO();
            advertencia.IdAtencion = 200;
            advertencia.NumeroRegistrosActualizados = Convert.ToInt32(resultado_ActualizacionDBF);
            return advertencia;
        }

    }
}
