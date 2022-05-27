using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class ResumenNominaDTO
    {
        public string Delegacion { get; set; }

        public string Sindicalizado { get; set; }
        public string Confianza { get; set; }
        public string Otros { get; set; }
        public bool Foliado { get; set; }
        public int Total { get; set; }
    
    }


}
