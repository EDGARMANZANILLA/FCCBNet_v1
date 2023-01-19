using DAP.Foliacion.Negocios;
using DAP.Foliacion.Plantilla.Filters;
using DAP.Plantilla.Models.PermisosModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAP.Plantilla.Controllers
{
    public class HerramientasConfiguracionController : Controller
    {
        // GET: HerramientasConfiguracion

        [SessionSecurityFilter]
        public ActionResult RegistrarUsuario()
        {
            ViewBag.UsuariosNoRegistrados = HerramientasConfiguracionNegocios.ObtenerUsuariosNoRegistrados();

            // ************************************************************************************************************* //
            // EL MODELO QUE SE ENVIA ES PARA QUE EL SIDEBAR CONTENGA LOS LINK’S REFERENTE A LOS PERMISOS QUE PUEDE VISITAR //
            // ************************************************************************************************************ //
            return PartialView();
        }




        [HttpPost]
        // Registrar Usuario
        public ActionResult RegistrarUsuarioEnSistemaInterno(string numEmpleado)
        {
            return Json(HerramientasConfiguracionNegocios.RegistrarUsuarioAlphaWeb(numEmpleado), JsonRequestBehavior.AllowGet);
        }
    }
}