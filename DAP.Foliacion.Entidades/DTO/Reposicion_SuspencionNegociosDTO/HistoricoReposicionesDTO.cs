using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.Reposicion_SuspencionNegociosDTO
{
    public class HistoricoReposicionesDTO
    {
        public int Id { get; set; }
        public string FechaCambio { get; set; }
        public string MotivoRefoliacion { get; set; }
        public int ChequeAnterior { get; set; }

        public string ChequeNuevo { get; set; }
        public string RepuestoPor { get; set; }
        public string EsCancelado { get; set; }
        public string ReferenciaCancelado { get; set; }
        public string DescripcionCancelado { get; set; }
    }
}
