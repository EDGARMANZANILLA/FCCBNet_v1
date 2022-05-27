using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class ResumenPersonalAFoliarDTO
    {
        public string CadenaNumEmpleado { get; set; }
        public int NumEmpleado { get; set; }
        public string RFC { get; set; }
        public string Nombre { get; set; }
        public decimal Liquido { get; set; }
        public string NumBeneficiario { get; set; }
        public string Delegacion { get; set; }

        //Llenado de datos restantes para poder tener un update completo para formas de pago 
        public int NumChe { get; set; }
        public string BancoX { get; set; }
        public string CuentaX { get; set; }
        public string Observa  { get; set; }



        //Nuevos Campos Agregados
        public string Partida  { get; set; }
        public int FolioCFDI  { get; set; }
        public int IdBancoPagador  { get; set; }

  


    }
}
