using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Entidades.DTO;

namespace DAP.Foliacion.Datos
{
    public class ConsultasDBSinEntity
    {


        public static string BuscarNombreEmpleado(string NumEmpleado)
        {
            string nombreCompletoEmpleado = null;

            
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
            {
                connection.Open();
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(" select nomina.dbo.fNombre('" + NumEmpleado + "') ", connection);
                System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    nombreCompletoEmpleado = string.IsNullOrEmpty(reader[0].ToString()) ? "Empleado no Encontrado" : reader[0].ToString()  ;

                }
            }

            return nombreCompletoEmpleado;
        }


        public static List<string> ObtenerNumerosCuentasDiferentesAlpha()
        {
            List<string> cuentasEncontradas = new List<string>();

            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
            {
                connection.Open();
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select distinct cuenta from nomina.dbo.nom_cat_bancos where status = 1", connection);
                System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    cuentasEncontradas.Add(reader[0].ToString().Trim());
                }
            }

            return cuentasEncontradas;
        }



        public static List<DetallesDeCuentaDTO> ObtenerNombresCuenta(string CuentaNombre)
        {
   
            List<DetallesDeCuentaDTO> detalleMostrar = new List<DetallesDeCuentaDTO>();

            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
            {
                connection.Open();
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select descrip, cuenta, forma_pago from nom_cat_bancos where status = 1 and cuenta = '"+CuentaNombre+"' ", connection);
                System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DetallesDeCuentaDTO nuevoDetalle = new DetallesDeCuentaDTO();

                    nuevoDetalle.descrip = reader[0].ToString().Trim();
                    nuevoDetalle.cuenta = reader[1].ToString().Trim();
                    nuevoDetalle.forma_pago = reader[2].ToString().Trim();


                    detalleMostrar.Add(nuevoDetalle);

                }
            }

            return detalleMostrar;
        }









    }
}
