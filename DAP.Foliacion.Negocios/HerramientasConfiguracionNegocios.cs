using DAP.Foliacion.Datos;
using DAP.Foliacion.Datos.HerramientasConfiguracionesClases;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.HerramientasConfiguracionesDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Negocios
{
    public class HerramientasConfiguracionNegocios
    {
        public static List<UsuariosNoRegistradoDTO> ObtenerUsuariosNoRegistrados()
        {
            return InformacionUsuarios.ObtenerUsuariosNoRegitradosEnSistemaWeb();
        }



        public static bool RegistrarUsuarioAlphaWeb(string numEmpleado)
        {
            bool bandera = false;
            try
            {
                int idTablaAlpha = InformacionUsuarios.ObtenerIdUsuarioAlphaPorNumeroEmpleado(numEmpleado);

                if (idTablaAlpha != 0)
                {
                    Transaccion transaccion = new Transaccion();
                    var repositorio = new Repositorio<Login_Usuarios>(transaccion);

                    var usuariariosRegistrados = repositorio.ObtenerTodos();


                    if (!usuariariosRegistrados.Select(x => x.IdTblUsers).Contains(idTablaAlpha))
                    {
                        Login_Usuarios nuevoUsuario = new Login_Usuarios();
                        nuevoUsuario.IdTblUsers = idTablaAlpha;
                        nuevoUsuario.NumEmpleado = Convert.ToInt32(numEmpleado);
                        nuevoUsuario.UltimaSesion = null;
                        nuevoUsuario.EsRoot = false;
                        nuevoUsuario.Activo = true;
                        Login_Usuarios usuarioAgredadoExitosamente = repositorio.Agregar(nuevoUsuario);

                        bandera = usuarioAgredadoExitosamente != null ? true : false;
                    }

                }

            }
            catch (Exception E)
            {

                bandera = false;
            }

            return bandera;
        }


    }
}
