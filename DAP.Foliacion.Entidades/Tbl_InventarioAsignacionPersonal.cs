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
    
    public partial class Tbl_InventarioAsignacionPersonal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tbl_InventarioAsignacionPersonal()
        {
            this.Tbl_InventarioDetalle = new HashSet<Tbl_InventarioDetalle>();
        }
    
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public string NombrePersonal { get; set; }
        public System.DateTime FechaHabilitacion { get; set; }
        public bool Activo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_InventarioDetalle> Tbl_InventarioDetalle { get; set; }
    }
}
