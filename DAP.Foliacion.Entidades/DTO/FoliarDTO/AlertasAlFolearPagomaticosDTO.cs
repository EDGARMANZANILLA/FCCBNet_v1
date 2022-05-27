using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class AlertasAlFolearPagomaticosDTO
    {
        /***************************************************************************************************************************************/
        // Valor de IdAtevcion 
        //   0 == SIN FOLIAR
        //   1 == YA SE ENCUENTRA FOLIADO 
        //   2 == NO HAY PAGOMATICOS POR FOLIAR
        //   3 == NO SE ENCONTRO LA BASE EN ALPHA (Interfaces)
        //   4 == DBF SIN PERMISOS (DBF ESTA ABIERTA POR OTRA PERSONA)
        //                 
        /***************************************************************************************************************************************/
        public int IdAtencion { get; set; }
        public string NumeroNomina { get; set; }
        public string NombreNomina { get; set; }
        public string Detalle { get; set; }
       
        public int RegistrosFoliados { get; set; }

        public string Solucion { get; set; }
        public string Id_Nom { get; set; }


        //Campo extra solo para los cheques foleados
        public int UltimoFolioUsado { get; set; }

        public string SubPathRuta_Ruta { get; set; }
        public string NombreDBF_RutaNomina { get; set; }
    }
}
