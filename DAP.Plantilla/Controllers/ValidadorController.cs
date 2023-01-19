using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DAP.Foliacion.Entidades.DTO.PermisosLoginDTO;
using DAP.Foliacion.Negocios;
using DAP.Foliacion.Plantilla.Filters;
using DAP.Plantilla.Models.PermisosModels;
using DAP.Plantilla.Models.SesionesDatosModels;
using Microsoft.Ajax.Utilities;
using NegocioLoginCentralizado;

namespace DAP.Plantilla.Controllers
{
    public class ValidadorController : Controller
    {
        // GET: Validador
        public ActionResult InicioAplicacion()
        {

            string valor = ConfigurationManager.AppSettings["MoodDeveloper"];
            bool estaEnMoodDesarrollador = Convert.ToBoolean(valor);
           // bool estaEnMoodDesarrollador = (bool)Session["moodDeveloper"];


            /******************************************************************************************************/
            /*************************     => INFORMACION DE LA SESION DEL TOKEN <=    ****************************/
            /******************************************************************************************************/
            string nombreVarTokenDescifrado = "TokenDescifrado" + ConfigurationManager.AppSettings["NombreMiProyecto"];
            NegocioLoginCentralizado.DTOs.TokenDTOs.DescifradoTokenDTO tokenDescifrado = (NegocioLoginCentralizado.DTOs.TokenDTOs.DescifradoTokenDTO)Session[""+nombreVarTokenDescifrado+""];

            if (tokenDescifrado == null  && !estaEnMoodDesarrollador)
            {
                    string direccionservervidor = System.Diagnostics.Debugger.IsAttached ? "localhost:44312" : "172.19.3.171:85";/* ruta que tendra dentro del servidor*/
                    return Redirect("http://" + direccionservervidor + "/SesionUsuario/InicioApp");
            }

            /******************************************************************************************************/
            /******  => INFORMACION DE LOS DATOS PERSONALES DEL EMPLEADO QUE ESTA LOGUEADO CORRECTAMENTE <=  ******/
            /******************************************************************************************************/
            InformacionEmpleadoSesionModel nuevonuevoUsuario = new InformacionEmpleadoSesionModel();
            nuevonuevoUsuario.NombreEmpleado = tokenDescifrado != null ? tokenDescifrado.NombreEmpleado : "Mood Developer";
            nuevonuevoUsuario.Puesto = tokenDescifrado != null ? tokenDescifrado.PuestoEmpleado : "DEVELOPER";
            nuevonuevoUsuario.MoodDesarrollador = estaEnMoodDesarrollador;
        

            if (!estaEnMoodDesarrollador)
            {
                /******************************************************************************************************/
                /***************  => INFORMACION DE LOS RECUERSOS QUE PUEDE DISPONER EL EMPLEADO <=  ******************/
                /******************************************************************************************************/
                List<ModulosPermitidosActivosDTO> permisosObtenidos = PermisosLoginNegocios.ObtenerPermisosInternos(tokenDescifrado.NumEmpleado);
              
                nuevonuevoUsuario.Foliacion = permisosObtenidos.Where(x => x.NombreRol.Equals("Foliacion")).ToList();
                nuevonuevoUsuario.Cancelacion = permisosObtenidos.Where(x => x.NombreRol.Equals("Cancelacion Cheques")).ToList();
                nuevonuevoUsuario.Configuracion = permisosObtenidos.Where(x => x.NombreRol.Equals("Configuracion")).ToList();

                string nombreVarPermisosEmpleado = "ListaPermisosWeb" + ConfigurationManager.AppSettings["NombreMiProyecto"];
                Session["" + nombreVarPermisosEmpleado + ""] = permisosObtenidos;
            }



            //********************        SE GUARDA LA SESSION DEL USUARIO        **************************************/
            string nombreVarDatosEmpleado = "UsuarioAPP";
            Session["" + nombreVarDatosEmpleado + ""] = nuevonuevoUsuario;


            
        

            // ************************************************************************************************************* //
            // EL MODELO QUE SE ENVIA ES PARA QUE EL SIDEBAR CONTENGA LOS LINK’S REFERENTE A LOS PERMISOS QUE PUEDE VISITAR //
            // ************************************************************************************************************ //
            return View();
        }


        public async Task<ActionResult> ValidaToken(string token)
        {

            //ES PARA EL MOODO DESARROLLADOR SOLAMENTE
            //ES PARA QUE CUANDO ESTEMOS EN MODD DESARROLLADOR PODAMOS VER TODAS LAS OPCIONES DEL MENU
            string valor = ConfigurationManager.AppSettings["MoodDeveloper"];
            bool estaEnMoodDesarrollador = Convert.ToBoolean(valor);

            if (estaEnMoodDesarrollador ) 
            {
                return RedirectToAction("InicioAplicacion", "Validador");
            }


            /*LOGICA DEL PROYECTO*/
            string direccionservervidor = "";
            if (!string.IsNullOrEmpty(token))
            {
                NegocioLoginCentralizado.DTOs.TokenDTOs.DescifradoTokenDTO tokenDescifrado = await TokenNegocio.DescifrarToken(token);

                if (!tokenDescifrado.NumEmpleado.IsNullOrWhiteSpace())
                {

                    if (SessionSecurityFilter.horarioValidoToken(tokenDescifrado.InicioFechaToken, tokenDescifrado.ExpiracionFechaToken))
                    {
                        string nombreVarTokenDescifrado = "TokenDescifrado" + ConfigurationManager.AppSettings["NombreMiProyecto"];
                        Session["" + nombreVarTokenDescifrado + ""] = tokenDescifrado;

                        string nombreVarPermisosEmpleado = "ListaPermisosWeb"+ConfigurationManager.AppSettings["NombreMiProyecto"];
                        Session[""+ nombreVarPermisosEmpleado + ""] = null;

                        string nombreVarDatosEmpleado = "UsuarioAPP";
                        Session["" + nombreVarDatosEmpleado + ""] = null;


                        //Reenviar a la pagina principal del aplicativo 
                        return RedirectToAction("InicioAplicacion", "Validador");
                    }
                    else
                    {
                        direccionservervidor = System.Diagnostics.Debugger.IsAttached ? "localhost:44312" : "172.19.3.171:85";/* ruta que tendra dentro del servidor*/
                        return Redirect("http://" + direccionservervidor + "/SesionUsuario/InicioApp");
                    }

                    //Reenviar a la pagina principal del aplicativo 
                    return RedirectToAction("InicioAplicacion", "Validador");
                }

            }

       
            /* El localhost se uso para efectos de testing */
            direccionservervidor = System.Diagnostics.Debugger.IsAttached ? "localhost:44312" : "172.19.3.171:85";/* ruta que tendra dentro del servidor*/
            return Redirect("http://" + direccionservervidor + "/SesionUsuario/InicioApp");
        }




        public ActionResult CerrarSesion()
        {
            string nombreVarTokenDescifrado = "TokenDescifrado" + ConfigurationManager.AppSettings["NombreMiProyecto"];
            Session[""+ nombreVarTokenDescifrado + ""] = null;
            Session.Remove(""+ nombreVarTokenDescifrado + "");

            string nombreVarPermisosEmpleado = "ListaPermisosWeb" + ConfigurationManager.AppSettings["NombreMiProyecto"];
            Session[""+ nombreVarPermisosEmpleado + ""] = null;
            Session.Remove(""+ nombreVarPermisosEmpleado + "");

            string nombreVarDatosEmpleado = "UsuarioAPP";
            Session["" + nombreVarDatosEmpleado + ""] = null;
            Session.Remove("" + nombreVarDatosEmpleado + "");

            HttpContext.Session.Abandon();

            string direccionservervidor = System.Diagnostics.Debugger.IsAttached ? "localhost:44312" : "172.19.3.171:85";/* ruta que tendra dentro del servidor*/
            return Redirect("http://" + direccionservervidor + "/SesionUsuario/InicioApp");
        }
    }
}