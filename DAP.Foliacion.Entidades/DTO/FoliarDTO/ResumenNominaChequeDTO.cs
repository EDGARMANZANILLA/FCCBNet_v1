using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class ResumenNominaChequeDTO
    {
        //El idVirtual solo sirve como contador al momento de mostrarlo en pantalla
        public int IdVirtual { get; set; }
        public int IdDelegacion { get; set; }
        //El grupo de foliacion {0} indica que son de la general y descentralizados y el grupo {1} son para todas las demas nominas
        public int GrupoFoliacion { get; set; }
        public string Coment { get; set; }
        public int IdNomina { get; set; }
        public string NombreDelegacion { get; set; }
        public int Sindicato { get; set; }
        public int Confianza { get; set; }
        public int Otros { get; set; }
        public bool EstaFoliadoCorrectamente { get; set; }

    }
}
