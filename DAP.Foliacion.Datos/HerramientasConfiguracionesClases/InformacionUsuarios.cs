using DAP.Foliacion.Entidades.DTO.HerramientasConfiguracionesDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Datos.HerramientasConfiguracionesClases
{
    public class InformacionUsuarios
    {

        public static List<UsuariosNoRegistradoDTO> ObtenerUsuariosNoRegitradosEnSistemaWeb()
        {
            List<UsuariosNoRegistradoDTO> usuariosSinRegistro = new List<UsuariosNoRegistradoDTO>();
            try
            {
                //string query = "select id, numEmpleado , username from nomina.dbo.nom_cat_users where id not in (select IdUsuarioAlpha_nomina_nom_cat_user from LoginCentralizado.dbo.tbl_UsuariosWeb) and status = 1 ";
                string query = "select  users.numEmpleado , users.username ,  activo.nomb  from nomina.dbo.nom_cat_users users inner join nomina.dbo.vwResumenActivosN as activo on users.numEmpleado  = activo.num where users.id not in ( select IdTblUsers from FCCBNetDB.dbo.Login_Usuarios) and users.status = 1 order by numEmpleado";
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(query, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        UsuariosNoRegistradoDTO nuevoUsuarioNoRegistrado = new UsuariosNoRegistradoDTO();
                        nuevoUsuarioNoRegistrado.Num = reader[0].ToString().Trim();
                        nuevoUsuarioNoRegistrado.MostrarTexto = reader[1].ToString().Trim() + " || " + reader[2].ToString().Trim(); ;
                        usuariosSinRegistro.Add(nuevoUsuarioNoRegistrado);
                    }
                }
            }
            catch (Exception E)
            {
                string a = E.Message;
              
            }
            return usuariosSinRegistro;
        }


        public static int ObtenerIdUsuarioAlphaPorNumeroEmpleado(string numeroEmpleado)
        {
            try
            {
                string query = "select id  from nomina.dbo.nom_cat_users where numEmpleado = '" + numeroEmpleado + "' and status = 1 ";
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(query, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                }
            }
            catch (Exception E)
            {
                string a = E.Message;
          
            }
            return 00000;
        }



    }
}
