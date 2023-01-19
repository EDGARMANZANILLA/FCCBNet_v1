using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class VerificarFoliacionNominasQuincenaDTO
    {
        public int Id { get; set; }
        public int Id_Nom { get; set; }
        public string Comentario { get; set; }
        public bool EstaImportado { get; set; }
        public int TotalRegistros { get; set; }
        public int RegistrosNoFoliados { get; set; }
        public int Cheques { get; set; }
        public int Pagomaticos { get; set; }
        public bool EstaFoliadoCorrectamente { get; set; }
    }
}
