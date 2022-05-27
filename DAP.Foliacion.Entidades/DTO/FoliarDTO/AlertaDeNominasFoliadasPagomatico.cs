using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class AlertaDeNominasFoliadasPagomatico
    {
        public int Id_Nom { get; set; }
        public string NumeroNomina { get; set; }
        public string NombreNomina { get; set; }
        public string Adicional { get; set; }
        public int NumeroRegistrosAFoliar { get; set; }
        public int IdEstaFoliada { get; set; }

    }
}
