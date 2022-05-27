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




    }

}
