using DAP.Foliacion.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Datos
{
    public class Reposicion_SuspencionDBSinORM
    {

        //formaPagoEncontrada.An, formaPagoEncontrada.NumEmpleado (EL NUMERO DEL EMPLEADO DEBERIA VENIR A 5 DIGITOS)
        public static int SuspenderDispercionEnSql_transaccionado(string Query) 
        {
            int filaActualizada = 0;
            
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
            {
                connection.Open();
                SqlTransaction transactionSQL;
                SqlCommand command = connection.CreateCommand();

                transactionSQL = connection.BeginTransaction();
                try
                {
                    command.Connection = connection;
                    command.Transaction = transactionSQL;
                    command.CommandType = CommandType.Text;
                    command.CommandText = Query;
                    filaActualizada += command.ExecuteNonQuery();

                    if (filaActualizada == 1)
                    {
                        transactionSQL.Commit();
                    }
                    else 
                    {
                        transactionSQL.Rollback();
                    }

                }
                catch (Exception E)
                {

                    try
                    {
                        transactionSQL.Rollback();
                    }
                    catch (Exception ex2)
                    {

                        // Este bloque catch manejará cualquier error que pueda haber ocurrido
                        // en el servidor que haría que la reversión fallara, como
                        // una conexión cerrada.

                        var transaccion2 = new Transaccion();

                        var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion2);

                        LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                        NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                        NuevaExcepcion.Metodo = "SuspenderDispercionEnSql_transaccionado";
                        NuevaExcepcion.Usuario = null;
                        NuevaExcepcion.Excepcion = E.Message + " || " + ex2.Message;
                        NuevaExcepcion.Comentario = "Rollback Exception  : " + ex2.GetType();
                        NuevaExcepcion.Fecha = DateTime.Now;

                        repositorio.Agregar(NuevaExcepcion);
                    }

                }

            }

            return filaActualizada;
        }








        public static int ReponerFormaPago(string ExecutaQuery)
        {
            int filaActualizada = 0;
            try
            {
                //Se conecta a interfaces por que solo se puede suspender una dispercion en una quincena actual por lo que siempre se trabaja con la interfas del anio actual 
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(ExecutaQuery, connection);
                    filaActualizada = command.ExecuteNonQuery();
                    connection.Close();
                }

            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "Reposicion_SuspencionDBSinORM";
                NuevaExcepcion.Metodo = "ReponerFormaPago";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo hacer el UPDATE";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }
            return filaActualizada;
        }







    }
}
