using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Datos.TablasAfectadasFCCBNetDB
{
    public class Cancelaciones_InterfazPercepcionesDeducciones_IPD_DbSinEntity
    {

        public static List<RegistrosTGCxNominaDTO> LeerConsultaReporteTGCNomina(string consultaNomina)
        {
            List<RegistrosTGCxNominaDTO> detallesEncontrados = new List<RegistrosTGCxNominaDTO>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consultaNomina, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        decimal SumatoriaNegativa = 0, SumatoriaPositiva = 0;
                        RegistrosTGCxNominaDTO nuevoDetalleTGC = new RegistrosTGCxNominaDTO();
                        /*******************************/
                        /***    DATOS GENERALES     ****/
                        /*******************************/
                        nuevoDetalleTGC.NombreNomina = reader[0].ToString().Trim();
                        nuevoDetalleTGC.Ramo = reader[1].ToString().Trim();
                        nuevoDetalleTGC.Partida = reader[2].ToString().Trim();


                        /***     SE OBTIENE UNA SOLA VE LOS DATOS PARA LUEGO SER ASIGNADOS DE ACUERDO SI ES UNA PP O DD     ***/
                        string DescripcionCvePD = reader[3].ToString().Trim();
                        string CvePD = reader[4].ToString().Trim();
                        string tipoClave = reader[5].ToString().Trim();
                        int cantidad = Convert.ToInt32(reader[6].ToString().Trim());
                        decimal montoObtenidoPositivo = string.IsNullOrEmpty(reader[7].ToString().Trim()) ? 0 : Convert.ToDecimal(reader[7].ToString().Trim());
                        decimal montoObtenidoNegativo = string.IsNullOrEmpty(reader[8].ToString().Trim()) ? 0 : Convert.ToDecimal(reader[8].ToString().Trim());


                        if (tipoClave.Trim().ToUpper().Equals("PP"))
                        {
                            /******************************/
                            /***    DATOS PERCEPCIONES  ***/
                            /******************************/
                            nuevoDetalleTGC.PP_TipoClave = tipoClave;
                            nuevoDetalleTGC.PP_Cantidad = cantidad;
                            nuevoDetalleTGC.PP_CvePD = CvePD;
                            nuevoDetalleTGC.pp_DescripcionCvePD = DescripcionCvePD;
                            nuevoDetalleTGC.PP_SumatoriaPositiva = montoObtenidoPositivo;

                            /***    si contiene una percepcion negativa se debe de mostrar positiva aunque en la practiva se sabe que se debe de netear (tecnicamente nunca debe de haber datos en la siguiente propiedad )    ***/
                            nuevoDetalleTGC.PP_SumatoriaNegativa = montoObtenidoNegativo;
                        }
                        else if (tipoClave.Trim().ToUpper().Equals("DD"))
                        {
                            /******************************/
                            /***    DATOS DEDUCCIONES  ***/
                            /******************************/
                            nuevoDetalleTGC.DD_TipoClave = tipoClave;
                            nuevoDetalleTGC.DD_Cantidad = cantidad;
                            nuevoDetalleTGC.DD_CvePD = CvePD;
                            nuevoDetalleTGC.DD_DescripcionCvePD = DescripcionCvePD;
                            nuevoDetalleTGC.DD_SumatoriaPositiva = montoObtenidoPositivo;

                            /***    si contiene una deduccion negativa se debe de mostrar aunque en la practiva se sabe que se debe de sumar    ***/
                            nuevoDetalleTGC.DD_SumatoriaNegativa = montoObtenidoNegativo < 0 ? montoObtenidoNegativo * -1 : SumatoriaNegativa;
                        }

                        detallesEncontrados.Add(nuevoDetalleTGC);
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "Cancelaciones_InterfazPercepcionesDeducciones_IPD_DbSinEntity";
                NuevaExcepcion.Metodo = "LeerConsultaReporteTGCNomina";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "PROBLEMA AL EJECUTAR : "+ consultaNomina;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return detallesEncontrados;
        }


        public static List<RegistrosTGCxCuentaBancariaDTO> LeerConsultaReporteTGCxCuentaBancaria(string consultaNomina)
        {
            List<RegistrosTGCxCuentaBancariaDTO> detallesEncontrados = new List<RegistrosTGCxCuentaBancariaDTO>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consultaNomina, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        
                        RegistrosTGCxCuentaBancariaDTO nuevoDetalleTGC = new RegistrosTGCxCuentaBancariaDTO();
                        /*******************************/
                        /***    DATOS GENERALES     ****/
                        /*******************************/
                        nuevoDetalleTGC.NombreCuentaBancaria = reader[0].ToString().Trim().ToUpper();


                        /***     SE OBTIENE UNA SOLA VE LOS DATOS PARA LUEGO SER ASIGNADOS DE ACUERDO SI ES UNA PP O DD     ***/
                        string DescripcionCvePD = reader[1].ToString().Trim().ToUpper();
                        string CvePD = reader[2].ToString().Trim();
                        string tipoClave = reader[3].ToString().Trim();
                        int cantidad = Convert.ToInt32(reader[4].ToString().Trim());
                        decimal montoObtenidoPositivo = string.IsNullOrEmpty(reader[5].ToString().Trim()) ? 0 : Convert.ToDecimal(reader[5].ToString().Trim());
                        decimal montoObtenidoNegativo = string.IsNullOrEmpty(reader[6].ToString().Trim()) ? 0 : Convert.ToDecimal(reader[6].ToString().Trim());


                        if (tipoClave.Trim().ToUpper().Equals("PP"))
                        {
                            /******************************/
                            /***    DATOS PERCEPCIONES  ***/
                            /******************************/
                            nuevoDetalleTGC.PP_TipoClave = tipoClave;
                            nuevoDetalleTGC.PP_Cantidad = cantidad;
                            nuevoDetalleTGC.PP_CvePD = CvePD;
                            nuevoDetalleTGC.pp_DescripcionCvePD = DescripcionCvePD;
                            nuevoDetalleTGC.PP_SumatoriaPositiva = montoObtenidoPositivo;

                            /***    si contiene una percepcion negativa se debe de mostrar positiva aunque en la practiva se sabe que se debe de netear (tecnicamente nunca debe de haber datos en la siguiente propiedad )    ***/
                            nuevoDetalleTGC.PP_SumatoriaNegativa = montoObtenidoNegativo;
                        }
                        else if (tipoClave.Trim().ToUpper().Equals("DD"))
                        {
                            /******************************/
                            /***    DATOS DEDUCCIONES  ***/
                            /******************************/
                            nuevoDetalleTGC.DD_TipoClave = tipoClave;
                            nuevoDetalleTGC.DD_Cantidad = cantidad;
                            nuevoDetalleTGC.DD_CvePD = CvePD;
                            nuevoDetalleTGC.DD_DescripcionCvePD = DescripcionCvePD;
                            nuevoDetalleTGC.DD_SumatoriaPositiva = montoObtenidoPositivo;

                            /***    si contiene una deduccion negativa se debe de mostrar aunque en la practiva se sabe que se debe de sumar    ***/
                            nuevoDetalleTGC.DD_SumatoriaNegativa = montoObtenidoNegativo < 0 ? montoObtenidoNegativo * -1 : montoObtenidoNegativo;
                        }

                        detallesEncontrados.Add(nuevoDetalleTGC);
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "Cancelaciones_InterfazPercepcionesDeducciones_IPD_DbSinEntity";
                NuevaExcepcion.Metodo = "LeerConsultaReporteTGCxCuentaBancaria";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "PROBLEMA AL EJECUTAR : " + consultaNomina;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return detallesEncontrados;
        }

        public static List<RegistrosCuotasPatronalesDTO> LeerConsultaReporteCoutasPatronales(string consultaNomina)
        {
            List<RegistrosCuotasPatronalesDTO> detallesEncontrados = new List<RegistrosCuotasPatronalesDTO>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consultaNomina, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        RegistrosCuotasPatronalesDTO nuevoCuotaPatronalEnRamo = new RegistrosCuotasPatronalesDTO();
                        nuevoCuotaPatronalEnRamo.NombreNomina = reader[0].ToString().Trim().ToUpper();
                        nuevoCuotaPatronalEnRamo.NumeroRamo = reader[1].ToString().Trim() ;
                        nuevoCuotaPatronalEnRamo.DescripRamo = reader[2].ToString().Trim();
                        nuevoCuotaPatronalEnRamo.DescripUnidad = reader[3].ToString().Trim();
                        nuevoCuotaPatronalEnRamo.Cantidad = Convert.ToInt32( reader[4].ToString().Trim());
                        nuevoCuotaPatronalEnRamo.MontoPositivo = Convert.ToDecimal( reader[5].ToString().Trim() );
                        
                        detallesEncontrados.Add(nuevoCuotaPatronalEnRamo);
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "Cancelaciones_InterfazPercepcionesDeducciones_IPD_DbSinEntity";
                NuevaExcepcion.Metodo = "LeerConsultaReporteCoutasPatronales";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "PROBLEMA AL EJECUTAR : " + consultaNomina;
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return detallesEncontrados;
        }

    }
}
