using DAP.Foliacion.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Datos.TablasAfectadasFCCBNetDB
{
    public class Tbl_Pagos_DbSinEntity
    {

        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /*********************************************************         METODO PARA LIMPIAR LOS DETALLES CUANDO SE RECUPERAN FOLIOS MAL FOLIADOS         *************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        public static int LimpiarCamposTBLPagosRecuperacionDeFolios( int Id_Nom, int anio , string condicionACumplir)
        {
            int registrosLimpiados = 0;
            try
            {

                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();

                    string queryActualizaInterfacesSQL = "update FCCBNetDB.dbo.Tbl_Pagos set FolioCheque = 0, Integridad_HashMD5 = 'CHEQUE RECUPERADO EXITOSAMENTE, VUELVA A FOLEAR DE NUEVO', IdTbl_CuentaBancaria_BancoPagador = 0, IdCat_EstadoPago_Pagos = 4, IdTbl_InventarioDetalle = null " +
                                                         "where Id_nom = "+Id_Nom+" and anio = "+anio+" and Id in ( "+condicionACumplir+")";


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

                NuevaExcepcion.Clase = "Tbl_Pagos_DbSinEntity";
                NuevaExcepcion.Metodo = "LimpiarCamposTBLPagosRecuperacionDeFolios";
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
