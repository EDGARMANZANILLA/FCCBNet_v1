using DAP.Foliacion.Entidades.DTO.PermisosLoginDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.PermisosModels
{
    public class PermisosUsuarioModel
    {
        public bool MoodDesarrollador { get; set; }
        public string NombreEmpleado { get; set; }
        public string Puesto { get; set; }

        public List<ModulosPermitidosActivosDTO> Foliacion { get; set; }
        public List<ModulosPermitidosActivosDTO> Cancelacion { get; set; }
        public List<ModulosPermitidosActivosDTO> Configuracion { get; set; }

    }

    public static class PermisosUsuarioEstadoStatico
    {
        public static bool MoodDesarrollador { get; set; }
        public static string NombreEmpleado { get; set; }
        public static string Puesto { get; set; }
        public static List<ModulosPermitidosActivosDTO> Foliacion { get; set;} 
        public static List<ModulosPermitidosActivosDTO> Cancelacion { get; set;} 
        public static List<ModulosPermitidosActivosDTO> Configuracion { get; set;} 
    }




    public class GenerarEstadoPermisosModelo
    {
        //public static PermisosUsuarioModel PermisosEstablecidos() 
        //{
        //    PermisosUsuarioModel nueva = new PermisosUsuarioModel();
        //    nueva.MoodDesarrollador = PermisosUsuarioEstadoStatico.MoodDesarrollador;
        //    nueva.NombreEmpleado = PermisosUsuarioEstadoStatico.NombreEmpleado;
        //    nueva.Puesto = PermisosUsuarioEstadoStatico.Puesto;
        //    nueva.Foliacion = PermisosUsuarioEstadoStatico.Foliacion;
        //    nueva.Cancelacion = PermisosUsuarioEstadoStatico.Cancelacion;
        //    nueva.Configuracion = PermisosUsuarioEstadoStatico.Configuracion;

        //    return nueva;
        //}


    }
}