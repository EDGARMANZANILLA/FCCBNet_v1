using DAP.Foliacion.Datos;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.PermisosLoginDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Negocios
{
    public class PermisosLoginNegocios
    {
        public static List<ModulosPermitidosActivosDTO> ObtenerPermisosInternos(string numEmpleado) 
        {
            Transaccion transacion = new Transaccion();
            var repo = new Repositorio<Login_Usuarios>(transacion);
            int empleadoABuscar = Convert.ToInt32(numEmpleado);
            int idUsuarioInterno = repo.Obtener(x => x.NumEmpleado == empleadoABuscar).Id;
            bool EsUsuarioRoot = repo.Obtener(x => x.NumEmpleado == empleadoABuscar).EsRoot;

            List<ModulosPermitidosActivosDTO> modulosPermitidos = new List<ModulosPermitidosActivosDTO>();

           
         

            if (!EsUsuarioRoot)
            {
                var permisos = new Repositorio<Login_Permisos>(transacion);
                List<Login_Permisos> permisosObtenidos = permisos.ObtenerPorFiltro(x => x.IdUsuario == idUsuarioInterno && x.Activo == true).ToList();

                foreach (Login_Permisos newPermiso in permisosObtenidos)
                {
                    ModulosPermitidosActivosDTO nuevoModulo = new ModulosPermitidosActivosDTO();
                    nuevoModulo.NombreRol = newPermiso.Login_Roles.NombreRol;
                    nuevoModulo.NombreModulo = newPermiso.Login_Modulos.NombreModulo;
                    nuevoModulo.Descripcion = newPermiso.Login_Modulos.Descripcion;
                    nuevoModulo.Controlador = newPermiso.Login_Modulos.Controlador;
                    nuevoModulo.Accion = newPermiso.Login_Modulos.Accion;
                    modulosPermitidos.Add(nuevoModulo);
                }
            }
            else
            {
                //SI EL USUARIO ES ROOT
                var repoModulos = new Repositorio<Login_Modulos>(transacion);
                List<Login_Modulos> modulosActivos = repoModulos.ObtenerPorFiltro(x => x.Activo == true).ToList();

                int iterador = 0;
                foreach (Login_Modulos newModulo in modulosActivos)
                {
                    iterador += 1;
                    ModulosPermitidosActivosDTO nuevoModulo = new ModulosPermitidosActivosDTO();
                    nuevoModulo.NombreRol = ObtenerRol(iterador);
                    nuevoModulo.NombreModulo = newModulo.NombreModulo;
                    nuevoModulo.Descripcion = newModulo.Descripcion;
                    nuevoModulo.Controlador = newModulo.Controlador;
                    nuevoModulo.Accion = newModulo.Accion;
                    modulosPermitidos.Add(nuevoModulo);
                }
            }
          
            return modulosPermitidos;
        }


        //Cambiar a validacion por DB
        public static string ObtenerRol(int iterador) 
        {
            string rol = "";
            switch (iterador) 
            {
                case 1:
                    rol = "Foliacion";
                    break;
                case 2:
                    rol = "Foliacion";
                    break; 
                case 3:
                    rol = "Foliacion";
                    break;
                case 4:
                    rol = "Foliacion";
                    break; 
                case 5:
                    rol = "Cancelacion Cheques";
                    break;
                case 6:
                    rol = "Cancelacion Cheques";
                    break;
                case 7:
                    rol = "Configuracion";
                    break;
                case 8:
                    rol = "Configuracion";
                    break;
            }
            return rol;
        }


        //public static List<Login_Permisos> ObtenerListaPermisos(string numEmpleado)
        //{
        //    Transaccion transacion = new Transaccion();
        //    var repo = new Repositorio<Login_Usuarios>(transacion);
        //    int empleadoABuscar = Convert.ToInt32(numEmpleado);
        //    int idUsuarioInterno = repo.Obtener(x => x.NumEmpleado == empleadoABuscar).Id;
        //    bool EsUsuarioRoot = repo.Obtener(x => x.NumEmpleado == empleadoABuscar).EsRoot;

        //    var permisos = new Repositorio<Login_Permisos>(transacion);
        //    List<Login_Permisos> permisosObtenidos = new List<Login_Permisos>();
        //    if (!EsUsuarioRoot)
        //    {
        //        permisosObtenidos = permisos.ObtenerPorFiltro(x => x.IdUsuario == idUsuarioInterno && x.Activo == true).ToList();
        //    }
        //    else 
        //    {
        //        var modulos = new Repositorio<Login_Modulos>(transacion);
        //        var modulosDisponibles =  modulos.ObtenerPorFiltro(x => x.Activo == true).ToList();

        //        foreach (Login_Modulos newmodulo in modulosDisponibles) 
        //        {
        //            Login_Permisos newpermiso = new Login_Permisos();
        //            newpermiso.
        //        }
        //    }


        //    return permisosObtenidos;
        //}


        //public static bool EsUsuarioRoota(string numEmpleado) 
        //{
        //    Transaccion transacion = new Transaccion();
        //    var repo = new Repositorio<Login_Usuarios>(transacion);
        //    int empleadoABuscar = Convert.ToInt32(numEmpleado);
        //    int idUsuarioInterno = repo.Obtener(x => x.NumEmpleado == empleadoABuscar).Id;
            
        //    var permisos = new Repositorio<Login_Permisos>(transacion);
        //    Login_Permisos permisosObtenidos = permisos.Obtener(x => x.IdUsuario == idUsuarioInterno && x.IdModulo == idModulo && x.Activo == true);

        //    return permisosObtenidos != null ? true : false;
        //}

    }
}
