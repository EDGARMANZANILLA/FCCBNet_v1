//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAP.Foliacion.Entidades
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_Solicitudes
    {
        public int Id { get; set; }
        public int NumeroMemo { get; set; }
        public int IdCuentaBancaria { get; set; }
        public int Cantidad { get; set; }
        public string FolioInicial { get; set; }
        public string FolioMuestra { get; set; }
        public Nullable<decimal> UsoAproximadoMeses { get; set; }
        public System.DateTime FechaSolicitud { get; set; }
        public bool Activo { get; set; }
    
        public virtual Tbl_CuentasBancarias Tbl_CuentasBancarias { get; set; }
    }
}
