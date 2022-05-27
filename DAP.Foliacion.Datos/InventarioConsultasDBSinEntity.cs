using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Entidades;

namespace DAP.Foliacion.Datos
{
    public class InventarioConsultasDBSinEntity
    {

        public static List<Tbl_InventarioDetalle> ObtenerRegistrosDetallesFolios(string executarQuery)
        {
            List<Tbl_InventarioDetalle> listaDetalleEncontrados = new List<Tbl_InventarioDetalle>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executarQuery, connection);
                    System.Data.SqlClient.SqlDataReader LeerDatos = command.ExecuteReader();

                    while (LeerDatos.Read())
                    {
                        Tbl_InventarioDetalle nuevoDetalle = new Tbl_InventarioDetalle();

                        nuevoDetalle.Id = LeerDatos.GetInt32(0);
                        nuevoDetalle.IdContenedor = LeerDatos.GetInt32(1);
                        nuevoDetalle.NumFolio = LeerDatos.GetInt32(2);
                        string a = LeerDatos[3].ToString().Trim();
                        if (LeerDatos[3].ToString().Trim() != "")
                        { nuevoDetalle.IdIncidencia = LeerDatos.GetInt32(3); }
                        if (LeerDatos[4].ToString().Trim() != "") { nuevoDetalle.FechaIncidencia = LeerDatos.GetDateTime(4); }
                        if (LeerDatos[5].ToString().Trim() != "") { nuevoDetalle.IdEmpleado = LeerDatos.GetInt32(5); }
                        nuevoDetalle.Activo = LeerDatos.GetBoolean(6);

                        listaDetalleEncontrados.Add(nuevoDetalle);
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "InventarioConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerRegistrosDetallesFolios";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo obtener los datos consulte exception para mas detalles ";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);
           
            }

            return listaDetalleEncontrados;
        }

    }
}
