using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO
{
  public  class InventarioContenedoresDTO
  {

        public string Banco { get; set; }
        public string Cuenta { get; set; }
        public string Orden { get; set; }
        public int Contenedor { get; set; }
        public string FolioInicial { get; set; }
        public string FolioFinal { get; set; }
        public int FormasTotalesContenedor { get; set; }
        public int FormasDisponiblesActuales { get; set; }
        public Nullable<int> FormasInhabilitadas { get; set; }
        public Nullable<int> FormasAsignadas { get; set; }
        public string FechaAlta { get; set; }


    }
}
