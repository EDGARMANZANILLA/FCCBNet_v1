using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class NombreNominasDTO
    {
        public string Quincena { get; set; }
        public string Nomina { get; set; }
        public string Adicional { get; set; }
        public string Ruta { get; set; }
        public string RutaNomina { get; set; }
        public string Coment { get; set; }
        public string Id_nom { get; set; }
        public bool Importado { get; set; }



    }


    public class NominasReporteInicialFoliacion
    {
        public string Nomina { get; set; }
        public string Id_nom { get; set; }
        public string Coment { get; set; }
        public string Adicional { get; set; }
        public string RutaNomina { get; set; }
        public string AN { get; set; }
      
      

    }
}
