using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.ConfiguracionesDTO
{
    public class ResumenFoliosDesinhabilitarDTO
    {

        public int Id { get; set; }
        public int Folio { get; set; }
        public string Incidencia { get; set; }
        public int IdContenedor { get; set; }
        public string FechaIncidencia { get; set; }

        public int Error { get; set; }


    }
}
