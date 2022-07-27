using AutoMapper;
using DAP.Foliacion.Entidades.DTO.BuscardorChequeDTO;
using DAP.Foliacion.Negocios;
using DAP.Plantilla.Models.BuscardorChequeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAP.Plantilla.Controllers
{
    public class EncontrarChequeController : Controller
    {
        // GET: BuscadorCheque
        public ActionResult Index()
        {
            return View();
        }







        [HttpPost]
        //devuelve la vista parcial de DetallesInformativosCheques es la vista que se abre al picar "Ver Detalles"
        public ActionResult DetallesInformativosCheques(int IdRegistroBuscar)
        {

            try
            {

                //DetallesInformativosChequeModel nuevoDetalle = Mapper.Map<DetallesRegistroDTO, DetallesInformativosChequeModel>(BuscadorChequeNegocios.ObtenerDetallesIdRegistro(IdRegistroBuscar));

                DetallesInformativosChequeModel nuevoDetalle = Mapper.Map<DetallesRegistroDTO, DetallesInformativosChequeModel>(Reposicion_SuspencionNegocios.ObtenerDetalleCompletoIdRegistro(IdRegistroBuscar));
                ViewBag.diccionarioReferenciaCancelacion = BuscadorChequeNegocios.ObtenerReferenciasDeCancelacionPorAnioActivas(DateTime.Now.Year);




                /***********************************************************************************/

                if (nuevoDetalle.IdRegistro == IdRegistroBuscar)
                {
                    return PartialView(nuevoDetalle);
                }

            }
            catch (Exception E)
            {
                return Json(new
                {
                    RespuestaServidor = "500",
                    MensajeError = "Ocurrio un error verifique que la quincena sea correcta"
                });


            }

            return Json(new
            {
                RespuestaServidor = "500",
                MensajeError = "No se encuentran cargadas las nominas de la quincena seleccionada"
            });

        }




        public ActionResult ObtenerFiltradoDatos(int TipoDeBusqueda, string BuscarDato)
        {
            //TipoDeBusqueda
            // 1 => CHEQUE
            // 2 => NUMERO DE EMPLEADO
            // 3 => NOMBRE BENEFICIARIO

            //BuscadorChequeNegocios.BuscarDatoPorFiltro( TipoDeBusqueda, BuscarDato);

            // List<ProgramaDTO> listado = Mapper.Map<IEnumerable<Programas>, List<ProgramaDTO>>(ProgramasNegocios.ObtenerActivosDelAnio(DateTime.Now.Year));

            List<ElementosBuscador> elementosEncontrados2 = Mapper.Map<List<ResultadoObtenidoParaSelect2>, List<ElementosBuscador>>(Foliacion.Negocios.BuscadorChequeNegocios.BuscarDatoPorFiltro(TipoDeBusqueda, BuscarDato).ToList());
            //List <ElementosBuscador> elementosEncontrados = new List<ElementosBuscador>();



            //ElementosBuscador nuevoElementosEncontrado = new ElementosBuscador();
            //nuevoElementosEncontrado.id = 1;
            //nuevoElementosEncontrado.text = "Edgar";
            //elementosEncontrados.Add(nuevoElementosEncontrado);


            //ElementosBuscador nuevoElementosEncontrado2 = new ElementosBuscador();
            //nuevoElementosEncontrado2.id = 2;
            //nuevoElementosEncontrado2.text = "TILIN";
            //elementosEncontrados.Add(nuevoElementosEncontrado2);



            return Json(elementosEncontrados2, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Buscar(ElementosBuscador BuscarElemento /*int BuscarElemento_id, string BuscarElemento_text*/)
        {


            List<DetallesBusquedaModels> detallesRegistrosEncontrados = new List<DetallesBusquedaModels>();
            /****/

            if (BuscarElemento != null)
            {
                //tipoDeBusqueda
                // 1 => CHEQUE
                // 2 => NUMERO DE EMPLEADO
                // 3 => NOMBRE BENEFICIARIO
                switch (BuscarElemento.tipoBusqueda)
                {
                    case 1:
                        //su Id contiene el id de la tabla donde esta localizado el registro
                        detallesRegistrosEncontrados = Mapper.Map<List<DetallesBusqueda>, List<DetallesBusquedaModels>>(BuscadorChequeNegocios.ObtenerDetallesDeCheque(BuscarElemento.id));


                        break;
                    case 2:
                        //su id trae el numero de empleado que se esta buscando
                        detallesRegistrosEncontrados = Mapper.Map<List<DetallesBusqueda>, List<DetallesBusquedaModels>>(BuscadorChequeNegocios.ObtenerDetallesNumEmpleado(BuscarElemento.id));
                        break;
                    case 3:
                        //Su id trae el numero de empleado 
                        detallesRegistrosEncontrados = Mapper.Map<List<DetallesBusqueda>, List<DetallesBusquedaModels>>(BuscadorChequeNegocios.ObtenerDetallesNumEmpleado(BuscarElemento.id));
                        break;
                    default:
                        // code block
                        break;
                }

            }
            else
            {
                return Json(new
                {
                    RespuestaServidor = 400,
                    MensajeError = "No se selecciono un elemento"
                });
            }



            return Json(new
            {
                RespuestaServidor = 201,
                RegistrosEncontrados = detallesRegistrosEncontrados,
                MensajeError = "No se encuentra el folio buscado"
            });
        }



        public ActionResult AgregarRemover_IdFormaPagoAReferenciaCancelado(int IdReferenciaCancelado , int IdRegistroCancelar) 
        {
            //Mensajes de ERRORES
            // 1 => /No es un cheque sino una dispercion (APLICA PARA AGREGAR, ACTUALIZAR O REVOCAR UN CHEQUE DE LA REFERENCIA DE CANCELACION)
            // 2 => /No Existe la referencia (APLICA PARA AGREGAR, ACTUALIZAR O REVOCAR UN CHEQUE DE LA REFERENCIA DE CANCELACION)
            // 3 => No se puede cambiar un cheque a la misma referencia
            // 4 => La referencia no puede ser removida porque aun no tiene una (APLiCA SOLO PARA QUITAR UN PAGO DE UNA REFERENCIA)

            //Mensajes Exitosos
            // 6 => /Se agrego a una referencia exitosamente
            // 7 => /Se Cambio de referencia con Exito
            // 8 => La referencias fue removida con exito de la referencia de cancelacion (APLiCA SOLO PARA QUITAR UN PAGO DE UNA REFERENCIA)


            string mensaje;
            string solucion = "";
            int errorRecibido = 0;

            if (IdReferenciaCancelado != 0)
            {
                errorRecibido= BuscadorChequeNegocios.AgregarActualizarCheque_ReferenciaCancelado( IdReferenciaCancelado,  IdRegistroCancelar);
            }
            else if( IdReferenciaCancelado == 0) 
            {
                errorRecibido = BuscadorChequeNegocios.RevocarCheque_ReferenciaCancelado( IdRegistroCancelar);
            }
                    // BuscadorChequeNegocios.AgregarChequeAReferenciaCancelado(IdReferenciaCancelado, IdRegistroCancelar);

            switch (errorRecibido)
            {
                //Mensajes de error
                case 1:
                    mensaje = " No se puede agregar una dispercion a una referencia ";
                    solucion = "Solicite una suspencion de la forma de pago";
                    break;
                case 2:
                    mensaje = "No Existe la referencia a la que desea cargar la forma de pago";
                    solucion = "Cree una referencia";
                    break;
                case 3:
                    mensaje = "No se puede cambiar un cheque a la misma referencia";
                    solucion = "Cambie la referencia a una diferente de la ingresada";
                    break;
                case 4:
                    mensaje = "No se puede revocar una forma de pago de una referencia, si nunca se a estado en una ";
                    solucion = "Asegurese que la forma de pago se encuentre cargada en una referencia";
                    break;
                
                //Mensajes Exitosos
                case 6:
                    mensaje = "se a ingresado la forma de pago a la referencia seleccionada correctamente";
                    break;
                case 7:
                    mensaje = "se cambio la referencia correctamente";
                    break;
                case 8:
                    mensaje = "La referencia fue removida exitosamente "; ;
                    break;
              

                default:
                    mensaje = "La peticion no fue procesada exitodamente";
                    solucion = "Reintente de nuevo mas tarde";
                    break;
            }

            return Json(new
            {
                NumeroMensaje = errorRecibido,
                Mensaje = mensaje,
                Solucion = solucion
            });
        }






        public JsonResult BuscarHistorico(int IdRegistro)
        {
            var listaHistorico = BuscadorChequeNegocios.ObtenerHistoricoReposiciones(IdRegistro).OrderByDescending(y => y.Id);

            if (listaHistorico.Count() >= 1)
            {
                return Json(new
                {
                    RespuestaServidor = 200,
                    Data = listaHistorico
                });
            }
            else
            {
                return Json(new
                {
                    Error = "'No se encontro ningun historico para la forma de pago seleccionada'",
                    Solucion = "Intente de nuevo por favor o contecte con el desarrollador"
                });
            }



        }



        /********************************************************************************************************************************************************************************************/
        /********************************************************************************************************************************************************************************************/
        /********************************************************       Verifica si la forma de pago ya esta cargada a una referencia de cancelacion        ******************************************/
        public JsonResult TieneReferenciaIdFormaPago(int IdRegistro)
        {
            string referenciaObtenida = BuscadorChequeNegocios.ObtenerNumeroReferenciaPago(IdRegistro);

            return Json(new
            {
                Referencia = referenciaObtenida

            });

        }



    }
}