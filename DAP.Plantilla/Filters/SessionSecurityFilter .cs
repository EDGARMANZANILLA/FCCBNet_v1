using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using System.Web.Mvc;

namespace IntercomunicacionLogin.Filters
{
    public class SessionSecurityFilter : AuthorizeAttribute, IAuthenticationFilter
    {

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            NegocioLoginCentralizado.DTOs.TokenDTOs.DescifradoTokenDTO sessioDeUser = (NegocioLoginCentralizado.DTOs.TokenDTOs.DescifradoTokenDTO)filterContext.HttpContext.Session["User"];

            if (sessioDeUser == null)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            //else
            //{
                //Si entra en esta parte se genera un ciclo infinito, por que siempre hay un cambio 
            //    //filterContext.Result = new HttpStatusCodeResult(200);
            //}
        }




        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                // Redireccionando el usuario a la vista de Logueo del controlador Usuario
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    filterContext.Result = new RedirectResult("http://localhost:44312/");
                }
                else
                {
                    filterContext.Result = new RedirectResult("http://172.19.3.171:85/");
                }

            }
        }

    }
}