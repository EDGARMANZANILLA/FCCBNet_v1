using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class CuentaBancariaModel
    {
        public int Id { get; set; }
        public string NombreBanco { get; set; }
        public string Abreviatura { get; set; }
        public string Cuenta { get; set; }
        public int IdCuentaBancaria_TipoPagoCuenta { get; set; }
        public Nullable<int> IdInventario { get; set; }

    }
}