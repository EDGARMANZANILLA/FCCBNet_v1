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
    
    public partial class Cancelaciones_InterfazPercepcionesDeducciones_IPD
    {
        public int Id { get; set; }
        public int IdReferenciaCancelados { get; set; }
        public int AnioEnReferencia { get; set; }
        public int IdBancoPagadorFCCBNetDb { get; set; }
        public string Referencia { get; set; }
        public string TipoNom { get; set; }
        public string Cve_presup { get; set; }
        public string Cvegto { get; set; }
        public string Cvepd { get; set; }
        public Nullable<decimal> MontoPositivo { get; set; }
        public Nullable<decimal> MontoNegativo { get; set; }
        public string Tipoclave { get; set; }
        public string Adicional { get; set; }
        public string Partida { get; set; }
        public string Num { get; set; }
        public string Nombre { get; set; }
        public string Num_che { get; set; }
        public Nullable<int> Foliocdfi { get; set; }
        public string Deleg { get; set; }
        public Nullable<int> Idctabanca { get; set; }
        public Nullable<int> IdBanco { get; set; }
        public string Pagomat { get; set; }
        public string Tipo_pagom { get; set; }
        public string Numtarjeta { get; set; }
        public Nullable<int> Orden { get; set; }
        public string Quincena { get; set; }
        public string Nomalpha { get; set; }
        public string Fecha { get; set; }
        public string Cvegasto { get; set; }
        public string Cla_pto { get; set; }
    }
}