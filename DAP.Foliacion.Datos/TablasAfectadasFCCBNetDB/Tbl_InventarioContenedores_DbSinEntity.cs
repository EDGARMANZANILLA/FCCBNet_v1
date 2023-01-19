using DAP.Foliacion.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Datos.TablasAfectadasFCCBNetDB
{
    public class Tbl_InventarioContenedores_DbSinEntity
    {


        public static List<int> ObtenerContenedoresEnRecuperarFolios(string condicionACumplir)
        {
            List<int> contenedoresHayados = new List<int>();
            string consulta = "SELECT distinct( IdContenedor)  FROM [FCCBNetDB].[dbo].[Tbl_InventarioDetalle] where Activo = 1 and  id in ( " + condicionACumplir + " ) ";
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consulta, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        contenedoresHayados.Add(Convert.ToInt32(reader[0].ToString().Trim()));
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "Tbl_InventarioContenedores_DbSinEntity";
                NuevaExcepcion.Metodo = "ObtenerContenedoresEnRecuperarFolios";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "Error al consultar " + consulta;
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);
            }

            return contenedoresHayados;
        }


        public static int ActualizarFormasDisponiblesXContenedor(int FormasRecuperadas, int idContenedor)
        {
            string queryActualizaInterfacesSQL = " update FCCBNetDB.dbo.Tbl_InventarioContenedores set  FormasFoliadas = FormasFoliadas - " + FormasRecuperadas + " , FormasDisponiblesActuales = FormasDisponiblesActuales + " + FormasRecuperadas + " , activo = 1 where  id =  " + idContenedor + " ";
            int contenedorActualizado = 0;
            try
            {

                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();

                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(queryActualizaInterfacesSQL, connection);
                    contenedorActualizado += command.ExecuteNonQuery();

                    connection.Close();
                }

            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "Tbl_InventarioContenedores_DbSinEntity";
                NuevaExcepcion.Metodo = "ActualizarFormasDisponiblesXContenedor";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo ejecutar || " + queryActualizaInterfacesSQL;
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return contenedorActualizado;
        }



    }
}
