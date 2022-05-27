using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Datos
{
    public class CrearReferencia_CanceladosDbSinEntity
    {


        public static DatosGeneralesIPD_DTO ObtenerDatosGeneralesxIdNom_IPD(int IdNom, string AnioInterfas)
        {
            string executaQuery = "select an, ap , ad , nomina 'TipoNomina' , nomina.dbo.fGetTipNominaFOX_ALPHA(nomina)'NominaAlpha' from interfaces" + AnioInterfas + ".dbo.bitacora where  id_nom = " + IdNom + "";

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executaQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        DatosGeneralesIPD_DTO datosNominasEncontrado = new DatosGeneralesIPD_DTO();

                        datosNominasEncontrado.AN = reader[0].ToString().Trim();
                        datosNominasEncontrado.AP = reader[1].ToString().Trim();
                        datosNominasEncontrado.AD = reader[2].ToString().Trim();
                        datosNominasEncontrado.TipoNomina = reader[3].ToString().Trim();
                        datosNominasEncontrado.NominaAlpha = reader[4].ToString().Trim();

                        return datosNominasEncontrado;
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDatosGeneralesxIdNom_IPD";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);

            }


            return new DatosGeneralesIPD_DTO();
        }




        /************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************/
        //****************************                   num || partida || folioCFDI || cve_Presup || Patronal_ISSTE || Patronal_ISSSTECAM || TipoPAgo || CLA_PTO           *****/
        //**********************************                            obtiene los datos de A-N que se muestran arriba de este comentario         *************************************/
        public static DatosAnIPD_DTO ObtenerDatosAN_IPD(string AnioInterfas,  string AN , string Num)
        {
            string executaQuery = "select num,  partida, foliocfdi ,  cve_presup ,p_isste  'Patronal_ISSTE' ,  p_isstecam  'Patronal_ISSSTECAM'  ,   'F'  'TipoPago' ,   CLA_PTO  from interfaces" + AnioInterfas+".dbo."+AN+" where num = '"+Num+"'";

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executaQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DatosAnIPD_DTO nuevoAnEncontrado = new DatosAnIPD_DTO();

                        nuevoAnEncontrado.Num = reader[0].ToString().Trim();
                        nuevoAnEncontrado.Partida = reader[1].ToString().Trim();
                        nuevoAnEncontrado.FolioCfdi = reader[2].ToString().Trim();
                        nuevoAnEncontrado.Cve_Presup = reader[3].ToString().Trim();
                        nuevoAnEncontrado.Patronal_ISSTE = Convert.ToDecimal( reader[4].ToString().Trim() );
                        nuevoAnEncontrado.Patronal_ISSSTECAM = Convert.ToDecimal( reader[5].ToString().Trim());
                        nuevoAnEncontrado.TipoPago = reader[6].ToString().Trim();
                        nuevoAnEncontrado.Cla_Pto = reader[7].ToString().Trim();



                        return nuevoAnEncontrado;
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDatosAN_IPD";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);

            }


            return  new DatosAnIPD_DTO();
        }




        /************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************/
       //**************************************                 obtiene las percepciones y deducciones para el IPD                *******************************/
        public static List<DatosApercepcionesIPD_DTO> ObtenerPercepciones_IPD(string AnioInterfas, string AP, string Num)
        {
            string executaQuery = "select Cla_perc, Monto_Perc , nomina.dbo.fGetCveGastoTab(cla_perc) 'cvegasto' from interfaces" + AnioInterfas+".dbo."+AP+" where num = '"+Num+ "'  order by cla_perc";

            List<DatosApercepcionesIPD_DTO> percepciones = new List<DatosApercepcionesIPD_DTO>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executaQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DatosApercepcionesIPD_DTO percepcionesEncontradas = new DatosApercepcionesIPD_DTO();
                        percepcionesEncontradas.Cla_perc = reader[0].ToString().Trim();
                        percepcionesEncontradas.Monto = Convert.ToDecimal( reader[1].ToString().Trim() );
                        percepcionesEncontradas.cvegasto = reader[2].ToString().Trim();

                        percepciones.Add(percepcionesEncontradas);
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerPercepciones_IPD";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);

            }


            return percepciones;
        }


        public static List<DatosAdeducionesIPD_DTO> ObtenerDeducciones_IPD(string AnioInterfas, string AD, string Num)
        {
            string executaQuery = "select cla_dedu , monto_dedu , nomina.dbo.fGetCveGastoTab(cla_dedu) 'cvegasto' from interfaces"+AnioInterfas+".dbo."+AD+" where num = '"+Num+ "' order by cla_dedu ";
            List<DatosAdeducionesIPD_DTO> deducciones = new List<DatosAdeducionesIPD_DTO>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executaQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DatosAdeducionesIPD_DTO deduccionesEncontradas = new DatosAdeducionesIPD_DTO();
                        deduccionesEncontradas.Cla_dedu  = reader[0].ToString().Trim();
                        deduccionesEncontradas.Monto = Convert.ToDecimal(reader[1].ToString().Trim());
                        deduccionesEncontradas.Cvegasto = reader[2].ToString().Trim();

                        deducciones.Add(deduccionesEncontradas);
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDeducciones_IPD";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return deducciones;
        }






        /************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************/
        //**************************************                                REPORTES DE CHEQUES CANCELADOS                                    *******************************/
        /************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************/
        public static string ObtenerPartidaCapae (int AnioPArtida )
        {
            string executaQuery = "SELECT top(1) part  FROM [Nomina].[dbo].[nom_cat_a_part_"+AnioPArtida+"] where descrip like '%COMISION DE AGUA POTABLE Y ALCANTARILLADO DEL ESTADO DE CAMPECHE%' order by part";
           try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executaQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return reader[0].ToString().Trim();
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerPartidaCapae";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return "";
        }

        /*****************************************************      OBTENER REGISTROS PARA NOMINA ANUAL     ********************************************************************/
        public static List<NominasAnualesDTO> ObtenerRegistrosNominaAnual(List<string> Consultas)
        {
            List<NominasAnualesDTO> registrosAnuales = new List<NominasAnualesDTO>();
            try
            {
                foreach (string executarQuery in Consultas)
                {
                    using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                    {
                        connection.Open();
                        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executarQuery, connection);
                        System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            NominasAnualesDTO nuevoRegistro = new NominasAnualesDTO();
                            nuevoRegistro.Quincena = reader[0].ToString().Trim();
                            nuevoRegistro.Cheque   = reader[1].ToString().Trim();
                            nuevoRegistro.Num      = reader[2].ToString().Trim();
                            nuevoRegistro.NombreBenefirioCheque = reader[3].ToString().Trim();
                            nuevoRegistro.Deleg    = reader[4].ToString().Trim();
                            nuevoRegistro.Liquido  = Convert.ToDecimal(reader[5].ToString().Trim());
                            nuevoRegistro.NombreNomina = reader[6].ToString().Trim();
    
                            registrosAnuales.Add(nuevoRegistro);
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerRegistrosNominaAnual";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return registrosAnuales;
        }


        /*****************************************************      OBTENER REGISTROS PARA CUENTA BANCARIA ANUAL     ********************************************************************/
        public static List<CuentasBancariasAnualesDTO> ObtenerRegistrosCuentaBancariaAnual(List<string> Consultas)
        {
            List<CuentasBancariasAnualesDTO> registrosAnuales = new List<CuentasBancariasAnualesDTO>();
            try
            {
                foreach (string executarQuery in Consultas)
                {
                    using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                    {
                        connection.Open();
                        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executarQuery, connection);
                        System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            CuentasBancariasAnualesDTO nuevoRegistro = new CuentasBancariasAnualesDTO();
                            nuevoRegistro.NombreNomina = reader[0].ToString().Trim();
                            nuevoRegistro.SumaLiquido = Convert.ToDecimal( reader[1].ToString().Trim());
                            nuevoRegistro.TotalRegistros = reader[2].ToString().Trim();
                            nuevoRegistro.NombreCuentaBanco = reader[3].ToString().Trim() + " - CTA : "+ reader[4].ToString().Trim();

                            registrosAnuales.Add(nuevoRegistro);
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerRegistrosCuentaBancariaAnual";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return registrosAnuales.OrderBy(x => x.NombreCuentaBanco).ToList();
        }

        /*****************************************************      OBTENER REGISTROS PARA CUENTA BANCARIA ANUAL     ********************************************************************/
        public static List<PensionAlimenticiaDTO> ObtenerRegistrosPensionAlimenticia(List<string> Consultas)
        {
            List<PensionAlimenticiaDTO> registrosAnuales = new List<PensionAlimenticiaDTO>();
            try
            {
                foreach (string executarQuery in Consultas)
                {
                    using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                    {
                        connection.Open();
                        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executarQuery, connection);
                        System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            PensionAlimenticiaDTO nuevoRegistro = new PensionAlimenticiaDTO();
                            nuevoRegistro.Ramo = reader[0].ToString().Trim();
                            nuevoRegistro.Unidad = reader[1].ToString().Trim();
                            nuevoRegistro.Quincena = reader[2].ToString().Trim();
                            nuevoRegistro.Num_che = reader[3].ToString().Trim();
                            nuevoRegistro.NumEmpleado = reader[4].ToString().Trim();
                            nuevoRegistro.NombreBeneficiario = reader[5].ToString().Trim();
                            nuevoRegistro.Delegacion = reader[6].ToString().Trim();
                            nuevoRegistro.Liquido = Convert.ToDecimal( reader[7].ToString().Trim());
                            
                            registrosAnuales.Add(nuevoRegistro);
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerRegistrosPensionAlimenticia";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return registrosAnuales.OrderBy(x => x.Ramo).ToList();
        }


        /************************************************************************************************************************************************************************/
        //***************************************************************                Reportes del IDP               *********************************************************/

        public static string ObtenerDescripcionRamo(string Partida, int AnioInterfaz)
        {

            string anio = "";
            if (DateTime.Now.Year != AnioInterfaz)
            {
                anio ="_"+Convert.ToString(AnioInterfaz)+"";
            }

            string executaQuery = "SELECT nomina.dbo.fGetDescripRamo"+anio+"(substring('"+Partida+"', 2, 2))'RAMO'";
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executaQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return reader[0].ToString().Trim();
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDescripcionRamo";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return "";
        }

        public static List<RegistrosTGCxNominaDTO> ObtenerRegistrosTGCxNomina(string Consulta)
        {
            List<RegistrosTGCxNominaDTO> regitrosXNomina = new List<RegistrosTGCxNominaDTO>();

            try
            {
                    using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                    {
                        connection.Open();
                        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(Consulta, connection);
                        System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            RegistrosTGCxNominaDTO nuevoRegistro = new RegistrosTGCxNominaDTO();
                            
                            nuevoRegistro.NombreNomina = reader.GetString(0);
                            nuevoRegistro.Id_nom = reader.GetInt32(1);
                            nuevoRegistro.Anio = reader.GetInt32(2);
                            nuevoRegistro.Partida = reader[3].ToString().Trim();
                            nuevoRegistro.NumEmpleado = reader.GetInt32(4);
                        
                            regitrosXNomina.Add(nuevoRegistro);
                        }
                        connection.Close();
                    }

            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerRegistrosTGCxNomina";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return regitrosXNomina;

        }


        public static List<DescripcionPPDD_DTO> ObtenerPercepcionesParaTotalesPorConcepto(string AP, int AnioInterfaz, string Num)
        {

            string anio = "";
            if (DateTime.Now.Year != AnioInterfaz)
            {
                anio =  Convert.ToString(AnioInterfaz) ;
            }

            List<DescripcionPPDD_DTO> percepciones = new List<DescripcionPPDD_DTO>();
            string executaQuery = "select  Monto_Perc , Cla_perc, nomina.dbo.fGetDescTipoPD(CLA_PERC) 'Descripcion' from interfaces" + anio+".dbo."+AP+" where num = '"+Num+"'  order by cla_perc";
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executaQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DescripcionPPDD_DTO nuevaPercepcion = new DescripcionPPDD_DTO();
                        nuevaPercepcion.Monto = Convert.ToDecimal( reader[0].ToString().Trim() );
                        nuevaPercepcion.Clave = reader[1].ToString().Trim();
                        nuevaPercepcion.Descripcion = reader[2].ToString().Trim();

                        percepciones.Add(nuevaPercepcion);
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerPercepcionesParaTotalesPorConcepto";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return percepciones;
        }

        public static List<DescripcionPPDD_DTO> ObtenerDeduccionesParaTotalesPorConcepto(string AD, int AnioInterfaz, string Num)
        {

            string anio = "";
            if (DateTime.Now.Year != AnioInterfaz)
            {
                anio = Convert.ToString(AnioInterfaz);
            }

            List<DescripcionPPDD_DTO> deducciones = new List<DescripcionPPDD_DTO>();
            string executaQuery = "select Monto_dedu , cla_dedu,nomina.dbo.fGetDescTipoPD(cla_dedu) 'Descripcion' from interfaces"+anio+".dbo."+AD+" where num = '"+Num+"' order by cla_dedu ";
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executaQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DescripcionPPDD_DTO nuevaPercepcion = new DescripcionPPDD_DTO();
                        nuevaPercepcion.Monto = Convert.ToDecimal(reader[0].ToString().Trim());
                        nuevaPercepcion.Clave = reader[1].ToString().Trim();
                        nuevaPercepcion.Descripcion = reader[2].ToString().Trim();

                        deducciones.Add(nuevaPercepcion);
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = " ObtenerDeduccionesParaTotalesPorConcepto";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return deducciones;
        }



        /*******************************************************      OBTENER LDF_6D       **********************************************************/
        public static int ObtenerLDF6DxPuestoTrabajo(string PuestoTrabajo)
        {
            string executaQuery = "select Nomina.dbo.fGetID_LDF_Normal('"+PuestoTrabajo+"')";
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executaQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return Convert.ToInt32(reader[0].ToString().Trim());
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "CrearReferencia_CanceladosDbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerLDF6DxPuestoTrabajo";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
                return 0;
            }
            return 0;
        }


    }
}
