using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO.RecuperarFolios
{
    public class FoliosARecuperarDTO
    {
        public string Id { get; set; }
        public int IdPago { get; set; }
        public string Anio { get; set; }

        public string Id_nom { get; set; }
        public string Nomina { get; set; }
        public string Quincena { get; set; }
        public string Delegacion { get; set; }
        public string Beneficiario { get; set; }
        public string NumEmpleado { get; set; }
        public string Liquido { get; set; }
        public string FolioCheque { get; set; }
        public int IdTbl_InventarioDetalle { get; set; }
        public string CuentaBancaria { get; set; }
    }
}
