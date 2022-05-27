using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.CrearReferencia_CanceladosModels
{
    public class ReferenciaCanceladoModel
    {
        public int Id { get; set; }
        public int Id_Iterador { get; set; }
        public int Anio { get; set; }
        public string Numero_Referencia { get; set; }
        public string Fecha_Creacion { get; set; }
        public string Creado_Por { get; set; }
        public int FormasPagoCargadas { get; set; }
        public bool EsCancelado { get; set; }
        public bool Activo { get; set; }


    }
}