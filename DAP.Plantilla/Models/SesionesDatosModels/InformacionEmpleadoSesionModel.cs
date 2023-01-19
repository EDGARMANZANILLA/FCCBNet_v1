using DAP.Foliacion.Entidades.DTO.PermisosLoginDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.SesionesDatosModels
{
    public class InformacionEmpleadoSesionModel
    {
        public string NombreEmpleado { get; set; }
        public string Puesto { get; set; }
        public bool MoodDesarrollador { get; set; }


        public List<ModulosPermitidosActivosDTO> Foliacion { get; set; }
        public List<ModulosPermitidosActivosDTO> Cancelacion { get; set; }
        public List<ModulosPermitidosActivosDTO> Configuracion { get; set; }

    }
}