using DAP.Foliacion.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Datos.TablasAfectadasFCCBNetDB
{
    public class Tbl_InventarioDetalle_DbSinEntity
    {

        public static int LimpiarCamposTbl_InventarioDetalleRecuperacionDeFolios(int Id_Nom, int anio, string condicionACumplir, int NumeroContenedor)
        {
            int registrosLimpiados = 0;
            try
            {

                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();

                    string queryActualizaInterfacesSQL = "  update [FCCBNetDB].[dbo].[Tbl_InventarioDetalle]   set IdIncidencia = null , FechaIncidencia = null, IdEmpleado = null  where IdContenedor = "+NumeroContenedor+" and  id in ( "+condicionACumplir+")";


                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(queryActualizaInterfacesSQL, connection);
                    registrosLimpiados += command.ExecuteNonQuery();

                    connection.Close();

                }

            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "Tbl_InventarioDetalle_DbSinEntity";
                NuevaExcepcion.Metodo = "LimpiarCamposTbl_InventarioDetalleRecuperacionDeFolios";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se Limpiaron todos los campos en la nomina  " + Id_Nom + " del anio " + anio + " || Datos que debieron Limpiarse " + condicionACumplir;
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return registrosLimpiados;
        }


    }
}
