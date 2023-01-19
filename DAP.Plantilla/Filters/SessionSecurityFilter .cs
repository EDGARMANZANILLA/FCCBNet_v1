using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using DAP.Foliacion.Entidades.DTO.PermisosLoginDTO;
using System.Configuration;

namespace DAP.Foliacion.Plantilla.Filters
{
    public class SessionSecurityFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //ES PARA QUE CUANDO ESTEMOS EN MODD DESARROLLADOR PODAMOS VER TODAS LAS OPCIONES DEL MENU
            string valor = ConfigurationManager.AppSettings["MoodDeveloper"];
            bool estaEnMoodDesarrollador = Convert.ToBoolean(valor);

            //if (estaEnMoodDesarrollador)
            //{
            //    return RedirectToAction("InicioAplicacion", "Validador");
            //}

            //bool estaEnMoodDesarrollador = (bool)filterContext.HttpContext.Session["moodDeveloper"];

            if (!estaEnMoodDesarrollador)
            {
                string nombreVarTokenDescifrado = "TokenDescifrado" + ConfigurationManager.AppSettings["NombreMiProyecto"];
                NegocioLoginCentralizado.DTOs.TokenDTOs.DescifradoTokenDTO Token = (NegocioLoginCentralizado.DTOs.TokenDTOs.DescifradoTokenDTO)filterContext.HttpContext.Session[""+ nombreVarTokenDescifrado + ""];


                bool forzarCerradoSesion = false;
                if (Token != null)
                {

                    if (horarioValidoToken(Token.InicioFechaToken, Token.ExpiracionFechaToken))
                    {
                        string nombreVarPermisosEmpleado = "ListaPermisosWeb" + ConfigurationManager.AppSettings["NombreMiProyecto"];
                        List<DAP.Foliacion.Entidades.DTO.PermisosLoginDTO.ModulosPermitidosActivosDTO> listaPermisosUsuario = (List<DAP.Foliacion.Entidades.DTO.PermisosLoginDTO.ModulosPermitidosActivosDTO>)filterContext.HttpContext.Session[""+nombreVarPermisosEmpleado+""];
                        //List<string> accionesPermitidas = (List<string>)filterContext.HttpContext.Session["ListaPermisos"];

                        string controladorAIr = Convert.ToString(filterContext.RouteData.Values["controller"]);
                        string accionAIr = Convert.ToString(filterContext.RouteData.Values["action"]);

                        var a = listaPermisosUsuario.Select(x => x.Controlador).ToList();
                        var b = listaPermisosUsuario.Select(x => x.Accion).ToList();
                        //aqui deberia ir el contexto de los vistas a donde puede llegar el aplicativo 
                        if (listaPermisosUsuario.Select(x => x.Controlador).Contains(controladorAIr.Trim()) /*&& listaPermisosUsuario.Select(x => x.Accion).Contains(accionAIr.Trim())*/ )
                        {
                            //El token esta DENTRO DEL HORARIO PERMITIDO y quiere ir a un RECURSO  PERMITIDO
                            filterContext.RouteData.Values["controller"] = controladorAIr;
                            filterContext.RouteData.Values["action"] = accionAIr;
                        }
                        else
                        {
                            //El token esta DENTRO DEL HORARIO PERMITIDO y quiere ir a un RECURSO NO PERMITIDO

                            filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                             { "controller", "Errores" },
                             { "action", "RecursoNoPermitido" }
                            });
                        }

                    }
                    else
                    {
                        //FUERA DEL HORARIO PERMITIDO  HAY QUE REGRESARLO AL LOGIN INICIAL PARA QUE SE LE SEA ASIGNADO UN NUEVO LOGIN
                        forzarCerradoSesion = true;
                    }
                }
                else
                {
                    forzarCerradoSesion = true;
                }


                if (forzarCerradoSesion)
                {
                    filterContext.Result = new RedirectToRouteResult(
                          new RouteValueDictionary
                          {
                             { "controller", "Validador" },
                             { "action", "CerrarSesion" }
                          });
                }


                /****************************************************************************/
                /****************************************************************************/
                /****************************************************************************/
                /****************************************************************************/


                //List<DAP.Foliacion.Entidades.DTO.PermisosLoginDTO.ModulosPermitidosActivosDTO> listaPermisos = (List<DAP.Foliacion.Entidades.DTO.PermisosLoginDTO.ModulosPermitidosActivosDTO>)filterContext.HttpContext.Session["ListaPermisos"];

                //var controllerName = filterContext.RouteData.Values["controller"];
                //var actionName = filterContext.RouteData.Values["action"];

                //string controlador = Convert.ToString(controllerName);
                //string accion = Convert.ToString(actionName);

                //bool tienePermisoControlador = listaPermisos.Select(x => x.Controlador).Contains(controlador);

                //if (!tienePermisoControlador)
                //{
                //    filterContext.RouteData.Values["controller"] = "Validador";
                //    filterContext.RouteData.Values["action"] = "InicioAplicacion";
                //}

            }

        }



        public static bool horarioValidoToken(DateTime InicioSesionHora, DateTime ExpiraSesionHora)
        {
            bool esHorarioValido = false;
            DateTime diaHoraActual = DateTime.Now;
            if (diaHoraActual >= InicioSesionHora && diaHoraActual <= ExpiraSesionHora)
            {
                esHorarioValido = true;
            }

            return esHorarioValido;
        }


    }



}