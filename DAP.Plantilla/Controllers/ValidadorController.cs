using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using NegocioLoginCentralizado;

namespace DAP.Plantilla.Controllers
{
    public class ValidadorController : Controller
    {
        // GET: Validador
        public ActionResult InicioAplicacion()
        {
            NegocioLoginCentralizado.DTOs.TokenDTOs.DescifradoTokenDTO tokenDescifrado = (NegocioLoginCentralizado.DTOs.TokenDTOs.DescifradoTokenDTO) Session["UserAPP"];

            ViewBag.NombreEmpleado = tokenDescifrado.NombreEmpleado;
            ViewBag.Puesto = tokenDescifrado.PuestoEmpleado;


            return View();
        }


        public async Task<ActionResult> ValidaToken(string token)
        {

            if (!string.IsNullOrEmpty(token))
            {
                NegocioLoginCentralizado.DTOs.TokenDTOs.DescifradoTokenDTO tokenDescifrado = await TokenNegocio.DescifrarToken(token);

                if (!tokenDescifrado.NumEmpleado.IsNullOrWhiteSpace())
                {
                    Session["UserAPP"] = tokenDescifrado;

                    //se autoriza el id e una cookie con el token 
                    FormsAuthentication.SetAuthCookie(token, false);

                    //Reenviar a la pagina principal del aplicativo 
                    //return RedirectToAction("/Home/Index");
                    return RedirectToAction("InicioAplicacion", "Validador");
                    //  return RedirectToRoute("/Home/Index");
                }

            }


            /* El localhost se uso para efectos de testing */
            string direccionservervidor = System.Diagnostics.Debugger.IsAttached ? "localhost:44312" : "172.19.3.171:85";/* ruta que tendra dentro del servidor*/

            return Redirect("http://" + direccionservervidor + "/SesionUsuario/InicioApp");
        }




        public ActionResult CerrarSesion()
        {
          //FormsAuthentication.SignOut();
            Session["UserAPP"] = null;
            string direccionservervidor = System.Diagnostics.Debugger.IsAttached ? "localhost:44312" : "172.19.3.171:85";/* ruta que tendra dentro del servidor*/
            return Redirect("http://" + direccionservervidor + "/SesionUsuario/InicioApp");
        }
    }
}