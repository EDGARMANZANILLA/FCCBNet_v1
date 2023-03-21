using DAP.Foliacion.Datos;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.BuscardorChequeDTO;
using DAP.Foliacion.Entidades.DTO.Reposicion_SuspencionNegociosDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Negocios
{
    public class BuscadorChequeNegocios
    {

        public static List<ResultadoObtenidoParaSelect2> BuscarDatoPorFiltro(int TipoDeBusqueda, string BuscarDato)
        {
            //int UltimaQuincenaObtenida = 0;
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Pagos>(transaccion);
            var repositorioCuentasBancarias = new Repositorio<Tbl_CuentasBancarias>(transaccion);


            List<ResultadoObtenidoParaSelect2> registrosEncontrados = new List<ResultadoObtenidoParaSelect2>();
           // ResultadoObtenidoParaSelect2 nuevoResultado = new ResultadoObtenidoParaSelect2();

            switch (TipoDeBusqueda)
            {
                case 1:

                    try
                    {
                        // Bloque de codigo para buscar por CHEQUE
                        int numCheque = Convert.ToInt32(BuscarDato);
                      //  int numCheFinal = numCheque + 50;
                        //var resultadoChequeObtenido = repositorio.ObtenerPorFiltro(x => x.FolioCheque == numCheque).ToList();
                        ////var resultadoChequeObtenido = repositorio.ObtenerPorFiltro(x => x.Activo == true).Select(y => new { y.FolioCheque, y.EsPenA, y.NumEmpleado ,y.NombreEmpleado , y.BeneficiarioPenA }).Distinct().ToList();
                        ////var resultadoChequeFiltrado = resultadoChequeObtenido.Where(x => x.FolioCheque.ToString().Contains(BuscarDato));
                       
                        
                        var resultadoChequeFiltrado = repositorio.ObtenerPorFiltro(x => x.FolioCheque == numCheque  ).ToList();
                        
                        foreach (var resultado in resultadoChequeFiltrado)
                        {
                            Tbl_CuentasBancarias resultadoIdBanco = repositorioCuentasBancarias.Obtener(x => x.Id == resultado.IdTbl_CuentaBancaria_BancoPagador);
                            ResultadoObtenidoParaSelect2 nuevoResultadoNumEmpleado = new ResultadoObtenidoParaSelect2();
                            nuevoResultadoNumEmpleado.id = resultado.Id;
                            if (resultado.EsPenA == true)
                            {
                                nuevoResultadoNumEmpleado.text = "Folio : " + resultado.FolioCheque + " || " + resultadoIdBanco.NombreBanco + " || Liquido : "+resultado.ImporteLiquido +" || Num : " + resultado.NumEmpleado + " || Empleado : "+resultado.NombreEmpleado+" || Beneficiario PenA : " + resultado.BeneficiarioPenA;
                            }
                            else 
                            {
                                nuevoResultadoNumEmpleado.text = "Folio : " + resultado.FolioCheque + " || " + resultadoIdBanco.NombreBanco +" || Liquido : " + resultado.ImporteLiquido + " || Num : " + resultado.NumEmpleado + " || Beneficiario : " + resultado.NombreEmpleado;
                            }

                            registrosEncontrados.Add(nuevoResultadoNumEmpleado);
                        }
                    }
                    catch (Exception e)
                    {
                        return registrosEncontrados;
                    }


                    return registrosEncontrados;

                case 2:
                    // Bloque de codigo para buscar por NUMERO DE EMPLEADO 

                    try
                    {
                        int NumEmpleado = Convert.ToInt32(BuscarDato);
                        var datosNumEmpleadoEncontrado = repositorio.Obtener(x => x.NumEmpleado == NumEmpleado && x.Activo == true);

                        if (datosNumEmpleadoEncontrado != null)
                        {

                            ResultadoObtenidoParaSelect2 nuevoResultadoGuarda = new ResultadoObtenidoParaSelect2();
                            nuevoResultadoGuarda.id = datosNumEmpleadoEncontrado.NumEmpleado;
                            nuevoResultadoGuarda.text = "Num : " + datosNumEmpleadoEncontrado.NumEmpleado + " || Nombre : " + datosNumEmpleadoEncontrado.NombreEmpleado;
                            registrosEncontrados.Add(nuevoResultadoGuarda);

                        }

                        //var datosNumEmpleadoObtenidos = repositorio.ObtenerPorFiltro(x => x.Activo == true).Select(y => new { y.NumEmpleado, y.NombreEmpleado }).Distinct().ToList();

                        //var empleadoObtenido = datosNumEmpleadoObtenidos.Where(x => x.NumEmpleado == NumEmpleado).ToList();

                        //foreach (var resultado in empleadoObtenido)
                        //{
                        //    ResultadoObtenidoParaSelect2 nuevoResultadoGuarda = new ResultadoObtenidoParaSelect2();
                        //    nuevoResultadoGuarda.id = resultado.NumEmpleado;
                        //    nuevoResultadoGuarda.text = "Num : " + resultado.NumEmpleado + " || NombreEmpleado : " + resultado.NombreEmpleado;
                        //    registrosEncontrados.Add(nuevoResultadoGuarda);
                        //}
                    }
                    catch (Exception e) 
                    {
                        return registrosEncontrados;
                    }

                    return registrosEncontrados;
                 
                case 3:
                    try
                    {
                        int anioAnterior = DateTime.Now.Year - 1;
                            // Bloque de codigo para buscar por el nombre del BENEFICIARIO DEL CHEQUE
                            var beneficiariosEncontrados = repositorio.ObtenerPorFiltro(x => x.Anio >= anioAnterior &&  x.Activo == true && x.NombreEmpleado.Contains(BuscarDato)).Select(y => new { y.NumEmpleado, y.NombreEmpleado }).Distinct().ToList();               
                            foreach (var resultado in beneficiariosEncontrados)
                            {
                                ResultadoObtenidoParaSelect2 nuevoResultadoNombre = new ResultadoObtenidoParaSelect2();
                                nuevoResultadoNombre.id = resultado.NumEmpleado;
                                nuevoResultadoNombre.text = "Num : " + resultado.NumEmpleado + " || Beneficiario : " + resultado.NombreEmpleado;
                                registrosEncontrados.Add(nuevoResultadoNombre);
                            }

                            // BuscarDato en los beneficiarios de pension alimenticia
                            var beneficiariosEncontradosPenA = repositorio.ObtenerPorFiltro(x => x.Anio >= anioAnterior && x.Activo == true && x.BeneficiarioPenA.Contains(BuscarDato)).Select(y => new { y.NumEmpleado, y.BeneficiarioPenA }).Distinct().ToList();
                            foreach (var resultado in beneficiariosEncontradosPenA)
                            {
                                ResultadoObtenidoParaSelect2 nuevoResultadoNombre = new ResultadoObtenidoParaSelect2();
                                nuevoResultadoNombre.id = resultado.NumEmpleado;
                                nuevoResultadoNombre.text = "Num : "+resultado.NumEmpleado+" || BeneficiarioPenA : "+resultado.BeneficiarioPenA;
                                registrosEncontrados.Add(nuevoResultadoNombre);
                            }



                    }
                            catch (Exception e)
                    {
                        return registrosEncontrados;
                    }






            return registrosEncontrados;

                default:

                    // aun por verificar si el usuario no conoce ningun dato => funcionalidad a futuro
                    // code block
                    return registrosEncontrados;

            }

        }




        public static List<DetallesBusqueda> ObtenerDetallesDeCheque(int IdRegistroDeFolio) 
        {
            List<DetallesBusqueda> detalleEncontrado = new List<DetallesBusqueda>();

            var transaccion = new Transaccion();

            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);
            var repositorioEstadoPagos = new Repositorio<Cat_EstadosPago_Pagos>(transaccion);
            var repositorioTipoPago = new Repositorio<Cat_FormasPago_Pagos>(transaccion);

            Tbl_Pagos chequeEncontrado = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroDeFolio);

            DetallesBusqueda nuevoChequeEncontrado = new DetallesBusqueda();
            nuevoChequeEncontrado.IdRegistro = chequeEncontrado.Id;
            nuevoChequeEncontrado.Id_nom = chequeEncontrado.Id_nom ;
            nuevoChequeEncontrado.ReferenciaBitacora = chequeEncontrado.ReferenciaBitacora;
            nuevoChequeEncontrado.Quincena = chequeEncontrado.Quincena;
            nuevoChequeEncontrado.NumEmpleado = chequeEncontrado.NumEmpleado;
            nuevoChequeEncontrado.NombreBeneficiaro = chequeEncontrado.EsPenA == true ?  chequeEncontrado.BeneficiarioPenA : chequeEncontrado.NombreEmpleado;
            nuevoChequeEncontrado.NumBene = chequeEncontrado.EsPenA == true ? chequeEncontrado.NumBeneficiario : "";
           // nuevoChequeEncontrado.IdRegistro = chequeEncontrado.Id;
            nuevoChequeEncontrado.FolioCheque = chequeEncontrado.FolioCheque;
            nuevoChequeEncontrado.Liquido = chequeEncontrado.ImporteLiquido;
            nuevoChequeEncontrado.EstadoCheque = chequeEncontrado.IdCat_EstadoPago_Pagos != 0 ? repositorioEstadoPagos.Obtener(x => x.Id ==  chequeEncontrado.IdCat_EstadoPago_Pagos).Descrip : "No Definido";
            nuevoChequeEncontrado.TipoPago = chequeEncontrado.IdCat_FormaPago_Pagos != 0 ? repositorioTipoPago.Obtener(X => X.Id == chequeEncontrado.IdCat_FormaPago_Pagos).Descrip : "No Definido";


            detalleEncontrado.Add(nuevoChequeEncontrado);

            return detalleEncontrado;
        }



        public static List<DetallesBusqueda> ObtenerDetallesNumEmpleado(int NumEmpleado)
        {
            List<DetallesBusqueda> detalleEncontrado = new List<DetallesBusqueda>();

            var transaccion = new Transaccion();

            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);
            var repositorioEstadoPagos = new Repositorio<Cat_EstadosPago_Pagos>(transaccion);
            var repositorioTipoPago = new Repositorio<Cat_FormasPago_Pagos>(transaccion);

            List<Tbl_Pagos> registrosNumeroEmpleadoEncontrados = repositorioTBlPagos.ObtenerPorFiltro(x => x.NumEmpleado == NumEmpleado).ToList();

            foreach (Tbl_Pagos registroEncontrado in registrosNumeroEmpleadoEncontrados) 
            {
                DetallesBusqueda nuevoChequeEncontrado = new DetallesBusqueda();
                nuevoChequeEncontrado.IdRegistro = registroEncontrado.Id;
                nuevoChequeEncontrado.Id_nom = registroEncontrado.Id_nom;
                nuevoChequeEncontrado.ReferenciaBitacora = registroEncontrado.ReferenciaBitacora;
                nuevoChequeEncontrado.Quincena = registroEncontrado.Quincena;
                nuevoChequeEncontrado.NumEmpleado = registroEncontrado.NumEmpleado;
                nuevoChequeEncontrado.NombreBeneficiaro = registroEncontrado.EsPenA == true ?  registroEncontrado.BeneficiarioPenA : registroEncontrado.NombreEmpleado ;
                nuevoChequeEncontrado.NumBene = registroEncontrado.EsPenA == true ? registroEncontrado.NumBeneficiario : "";
               // nuevoChequeEncontrado.IdRegistro = registroEncontrado.Id;
                nuevoChequeEncontrado.FolioCheque = registroEncontrado.FolioCheque;
                nuevoChequeEncontrado.Liquido = registroEncontrado.ImporteLiquido;
                nuevoChequeEncontrado.EstadoCheque = registroEncontrado.IdCat_EstadoPago_Pagos != 0 ? repositorioEstadoPagos.Obtener(x => x.Id == registroEncontrado.IdCat_EstadoPago_Pagos).Descrip : "No Definido";
                nuevoChequeEncontrado.TipoPago = registroEncontrado.IdCat_FormaPago_Pagos != 0 ? repositorioTipoPago.Obtener(X => X.Id == registroEncontrado.IdCat_FormaPago_Pagos).Descrip : "No Definido";

                detalleEncontrado.Add(nuevoChequeEncontrado);
            }




            return detalleEncontrado;
        }


        public static DetallesRegistroDTO ObtenerDetallesIdRegistro(int IdRegistroBuscar)
        {
            
            var transaccion = new Transaccion();

            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);

            var repositorioCuentasBancarias = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var repositorioEstadoPagos = new Repositorio<Cat_EstadosPago_Pagos>(transaccion);



            Tbl_Pagos registroEncontrado = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroBuscar);

            Tbl_CuentasBancarias bancoEncontrado = repositorioCuentasBancarias.Obtener(x => x.Id == registroEncontrado.IdTbl_CuentaBancaria_BancoPagador);


            DetallesRegistroDTO nuevoDetalle = new DetallesRegistroDTO();

            nuevoDetalle.IdRegistro =registroEncontrado.Id ;
            nuevoDetalle.Id_nom = " || " + registroEncontrado.Id_nom + " || - || " + registroEncontrado.Nomina + " || - || "+registroEncontrado.Adicional+ " || ";
            nuevoDetalle.ReferenciaBitacora = registroEncontrado.ReferenciaBitacora;

            nuevoDetalle.Quincena = registroEncontrado.Quincena;
            nuevoDetalle.NumEmpleado = registroEncontrado.NumEmpleado;
            nuevoDetalle.NombreEmpleado = registroEncontrado.NombreEmpleado;

            nuevoDetalle.Delegacion = registroEncontrado.Delegacion;
            nuevoDetalle.Folio = registroEncontrado.FolioCheque;
            nuevoDetalle.Liquido = registroEncontrado.ImporteLiquido;

            nuevoDetalle.EstadoCheque = registroEncontrado.IdCat_EstadoPago_Pagos != 0 ? repositorioEstadoPagos.Obtener(x => x.Id == registroEncontrado.IdCat_EstadoPago_Pagos).Descrip : "No Definido";
            nuevoDetalle.BancoPagador = bancoEncontrado.NombreBanco;
            nuevoDetalle.CuentaPagadora = bancoEncontrado.Cuenta;

            //nuevoDetalle.EsPenA = regitroEncontrado.EsPenA;
            nuevoDetalle.EsPenA = registroEncontrado.EsPenA == null ? false : registroEncontrado.EsPenA;
            nuevoDetalle.NumBeneficiarioPenA = registroEncontrado.NumBeneficiario;
            nuevoDetalle.NombreBeneficiarioPenA = registroEncontrado.BeneficiarioPenA;

            //nuevoDetalle.EsRefoliado =;
            nuevoDetalle.EsRefoliado = registroEncontrado.TieneSeguimientoHistorico == null ? false : registroEncontrado.TieneSeguimientoHistorico;



            /*****************************************************************************************************************************************/
            //nuevoDetalle.Id_nom = regitrosNombreEmpleadoEncontrados.Id_nom;
            //nuevoDetalle.NumEmpleado = regitrosNombreEmpleadoEncontrados.NumEmpleado;
            //nuevoDetalle.Quincena = regitrosNombreEmpleadoEncontrados.Quincena;

            //nuevoDetalle.Folio = regitrosNombreEmpleadoEncontrados.FolioCheque;
            //nuevoDetalle.Liquido = regitrosNombreEmpleadoEncontrados.ImporteLiquido;
            //nuevoDetalle.NombreBeneficiaro = regitrosNombreEmpleadoEncontrados.NombreEmpleado;

            //nuevoDetalle.EstadoCheque = regitrosNombreEmpleadoEncontrados.Cat_EstadosPago_Pagos.Descrip;
            //nuevoDetalle.ReferenciaBitacora = regitrosNombreEmpleadoEncontrados.ReferenciaBitacora;
            //nuevoDetalle.BancoPagador = regitrosNombreEmpleadoEncontrados.Tbl_CuentasBancarias.NombreBanco;

         
            
            //nuevoDetalle.NombreBeneficiarioPenA = regitrosNombreEmpleadoEncontrados.BeneficiarioPenA;
            

            //nuevoDetalle.EsRefoliado = regitrosNombreEmpleadoEncontrados.EsRefoliado == null? false: regitrosNombreEmpleadoEncontrados.EsRefoliado;

            /**************************************************************************************************************************************/

 





            return nuevoDetalle;
        }




        public static Dictionary<int, String> ObtenerReferenciasDeCancelacionPorAnioActivas(int AnioActual)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);

            int anioAnterior = AnioActual - 1;
            // List< Dictionary<int, string> > diccionarioaReferenciasCanceladas = new List< Dictionary<int, string>>();
            var filtradoReferenciasObtenidos = repositorio.ObtenerPorFiltro(x => x.Anio >= anioAnterior && x.EsCancelado == false && x.Activo == true).Select(x => new { x.Id, x.Anio, x.Numero_Referencia }).OrderBy(x => x.Numero_Referencia).ToList();

            Dictionary<int, string> diccionarioReferencias_Canceladas = new Dictionary<int, string>();
            foreach (var nuevoRegistro in filtradoReferenciasObtenidos)
            {
                diccionarioReferencias_Canceladas.Add(nuevoRegistro.Id, "" + nuevoRegistro.Numero_Referencia + "");
            }

            return diccionarioReferencias_Canceladas;
        }










        public static int AgregarChequeAReferenciaCancelado(int IdReferenciaCancelado, int IdRegistroCancelar) 
        {
            int numeroError = 0;
            // 0 => /Todo Salio Bien
            // 1 => /No se puede agregar una dispercion a una referencia 
            // 2 => /No se puede sacar un cheque de una referencia si nunca se a estado en una
            // 3 => No se puede cambiar un cheque a la misma referencia
            // 4 => La referencias fue removida con exito de la referencia de cancelacion

            bool bandera = false;
            var transaccion = new Transaccion();
            //Consultar
            var repoRerenciaCancelado = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
            //Modificar
            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);
            var repoSeguimientoHistorico = new Repositorio<Tbl_SeguimientoHistoricoFormas_Pagos>(transaccion);


            //TBL_PAGOS
            Tbl_Pagos registroEncontrado = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroCancelar && x.Activo == true);
            //TBL_REFERENCIAS_CANCELADOS
            Tbl_Referencias_Cancelaciones registroFereciaEncontrado = repoRerenciaCancelado.Obtener(x => x.Id == registroEncontrado.IdTbl_Referencias_Cancelaciones && x.Activo == true);


            Tbl_SeguimientoHistoricoFormas_Pagos nuevoTracking = new Tbl_SeguimientoHistoricoFormas_Pagos();
            if (registroEncontrado.IdCat_FormaPago_Pagos == 1)
            {
                if (IdReferenciaCancelado == 0)
                {
                    //Sacar una forma de pago de una referencia
                    if (registroEncontrado.IdTbl_Referencias_Cancelaciones == null)
                    {
                        // return bandera;
                        return numeroError = 2;
                    }
                    else
                    {
                        //Actualza Tbl_Pagos
                        registroEncontrado.IdTbl_Referencias_Cancelaciones = null;

                        //Actualiza TBl_Seguimiento (TRACKING)
                        nuevoTracking.IdTbl_Pagos = registroEncontrado.Id;
                        nuevoTracking.FechaCambio = Convert.ToDateTime(DateTime.Now.Date);
                        nuevoTracking.ChequeAnterior = registroEncontrado.FolioCheque;
                        nuevoTracking.ChequeNuevo = null;
                        nuevoTracking.MotivoRefoliacion = "Se revoco de una referencia proceso de cancelacion";
                        nuevoTracking.RefoliadoPor = "*****";
                        nuevoTracking.EsCancelado = false;
                        nuevoTracking.ReferenciaCancelado = null;
                        nuevoTracking.IdCat_EstadoCancelado = null;
                        nuevoTracking.Activo = true;

                        //Actualiza TBL_ReferenciaCancelados
                        registroFereciaEncontrado.FormasPagoDentroReferencia = registroFereciaEncontrado.FormasPagoDentroReferencia - 1;

                        return numeroError = 4;
                    }
                }
                else
                {
                    if (registroEncontrado.IdTbl_Referencias_Cancelaciones == IdReferenciaCancelado)
                    {
                        // return bandera;
                        return numeroError = 3;
                    }
                    else
                    {

                        registroEncontrado.IdTbl_Referencias_Cancelaciones = IdReferenciaCancelado;


                        nuevoTracking.IdTbl_Pagos = registroEncontrado.Id;
                        nuevoTracking.FechaCambio = Convert.ToDateTime(DateTime.Now.Date);
                        nuevoTracking.ChequeAnterior = registroEncontrado.FolioCheque;
                        nuevoTracking.ChequeNuevo = null;
                        nuevoTracking.MotivoRefoliacion = "Se agrego a una referencia para el proceso de cancelacion";
                        nuevoTracking.RefoliadoPor = "*****";
                        nuevoTracking.EsCancelado = true;
                        nuevoTracking.ReferenciaCancelado = registroFereciaEncontrado.Anio + "" + registroFereciaEncontrado.Numero_Referencia;
                        nuevoTracking.IdCat_EstadoCancelado = 1;
                        nuevoTracking.Activo = true;

                        //Actualiza TBL_ReferenciaCancelados
                        registroFereciaEncontrado.FormasPagoDentroReferencia = registroFereciaEncontrado.FormasPagoDentroReferencia + 1;
                    }
                }

                Tbl_Pagos Pago_Modificado = repositorioTBlPagos.Modificar_Transaccionadamente(registroEncontrado);
                Tbl_SeguimientoHistoricoFormas_Pagos Seguimiento_modificado = repoSeguimientoHistorico.Agregar_Transaccionadamente(nuevoTracking);
                Tbl_Referencias_Cancelaciones ReferenciaCancelado_Modificado = repoRerenciaCancelado.Modificar_Transaccionadamente(registroFereciaEncontrado);
                if (Pago_Modificado.Id == Seguimiento_modificado.IdTbl_Pagos)
                {
                    transaccion.GuardarCambios();
                    //bandera = true;
                   return numeroError = 0;
                }

            }
            else 
            {
                return numeroError = 1;
            }

            return numeroError;
        }




        public static int AgregarActualizarCheque_ReferenciaCancelado(int IdReferenciaCancelado, int IdRegistroCancelar)
        {
            int numeroError = 0;
            //Mensajes de ERRORES
            // 1 => /No es un cheque sino una dispercion (APLICA PARA AGREGAR, ACTUALIZAR O REVOCAR UN CHEQUE DE LA REFERENCIA DE CANCELACION)
            // 2 => /No Existe la referencia (APLICA PARA AGREGAR, ACTUALIZAR O REVOCAR UN CHEQUE DE LA REFERENCIA DE CANCELACION)
            // 3 => No se puede cambiar un cheque a la misma referencia
            // 4 => La referencia no puede ser removida porque aun no tiene una (APLiCA SOLO PARA QUITAR UN PAGO DE UNA REFERENCIA)
            // 5 => Un cheque con folio "1111 1111" no puede ser agregado a una referencia de cancelacion ya que era una dispercion se suspendio pero aun no se le asigna un folio valido

            //Mensajes Exitosos
            // 6 => /Se agrego a una referencia exitosamente
            // 7 => /Se Cambio de referencia con Exito
            // 8 => La referencias fue removida con exito de la referencia de cancelacion (APLiCA SOLO PARA QUITAR UN PAGO DE UNA REFERENCIA)


            var transaccion = new Transaccion();
            //Modificar
            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);
            var repoRerenciaCancelado = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);

            var repoSeguimientoHistorico = new Repositorio<Tbl_SeguimientoHistoricoFormas_Pagos>(transaccion);

            

            //TBL_PAGOS
            Tbl_Pagos registroPagoEncontrado = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroCancelar && x.Activo == true);
            //TBL_REFERENCIAS_CANCELADOS
            Tbl_Referencias_Cancelaciones registroFereciaEncontrado = repoRerenciaCancelado.Obtener(x => x.Id == IdReferenciaCancelado  && x.Activo == true);

          
            //Agregar un seguimiento
            Tbl_SeguimientoHistoricoFormas_Pagos nuevoTracking = new Tbl_SeguimientoHistoricoFormas_Pagos();

            //valida que el pago sea cheque y que exista la referencia a la que se agregara
            if (registroPagoEncontrado.IdCat_FormaPago_Pagos == 1)
            {
                if (registroFereciaEncontrado != null)
                {
                    if(registroPagoEncontrado.FolioCheque == 11111111) 
                    {
                        // 5 => Un cheque con folio "1111 1111" no puede ser agregado a una referencia de cancelacion ya que era una dispercion se suspendio pero aun no se le asigna un folio valido
                        return 5;
                    }
                    //valida que el pago no tenga ninguna referencia
                    if (registroPagoEncontrado.IdTbl_Referencias_Cancelaciones == null)
                    {
                        /******************************************************************************************************************************************************************************/
                        /*******************************************************************   AGREGA DE REFERENCIA   *********************************************************************************/
                        /***************  Agrega un registro al tracking, agrega referenciaCancelado a  tbl_pago, y Actualiza La referencia de cancelacion sumando la cantidad de cheques **************/
                        /********************************************************************************************************************************************************************************/

                        //llenado del objeto para Agregar un registro del seguimiento (tracking)
                        nuevoTracking.IdTbl_Pagos = registroPagoEncontrado.Id;
                        nuevoTracking.FechaCambio = Convert.ToDateTime(DateTime.Now.Date);
                        nuevoTracking.ChequeAnterior = registroPagoEncontrado.FolioCheque;
                        nuevoTracking.ChequeNuevo = null;
                        nuevoTracking.MotivoRefoliacion = "Agregado a referencia de cancelacion " + registroFereciaEncontrado.Numero_Referencia ;
                        nuevoTracking.RefoliadoPor = null;
                        nuevoTracking.EsCancelado = false;
                        nuevoTracking.ReferenciaCancelado = null;
                        nuevoTracking.IdCat_EstadoCancelado = 1;
                        nuevoTracking.Activo = true;

                        //Actualiza id de la referencia de cancelacion a la que pertenece
                        registroPagoEncontrado.IdTbl_Referencias_Cancelaciones = IdReferenciaCancelado;
                        registroPagoEncontrado.TieneSeguimientoHistorico = true;

                        //Actualiza TBL_ReferenciaCancelados
                        registroFereciaEncontrado.FormasPagoDentroReferencia += 1;
                        registroFereciaEncontrado.FechaActualizacion_AgregoOQuitoFormas = DateTime.Now;


                        //Se agrego a una referencia con exito con exito
                        numeroError = 6;

                    }
                    else if (registroPagoEncontrado.IdTbl_Referencias_Cancelaciones != IdReferenciaCancelado)
                    {
                        /******************************************************************************************************************************************************************************/
                        /*******************************************************************   CAMBIA DE REFERENCIA   *********************************************************************************/
                        /*************** Agrega un registro al tracking, Actualiza tbl_pago con la nueva referencia y resta 1 a la ferencia donde estaba y suma 1 a donde va estar ahora **************/
                        /******************************************************************************************************************************************************************************/

                        //REALIZA UN CAMBIO DE UNA REFERENCIA HACIA OTRA REFERENCIA
                        // Se valida que el idfererenciaCancelado del pago no sea el mismo al que se quiera voler agregar y que la referencia exista

                        //Obtener la referencia de donde se encuentra cargado actualmente el pago para descontar esa forma de pago 
                        Tbl_Referencias_Cancelaciones registroFerecia_Original = repoRerenciaCancelado.Obtener(x => x.Id == registroPagoEncontrado.IdTbl_Referencias_Cancelaciones && x.Activo == true);



                        //llenado del objeto para Agregar un registro del seguimiento (tracking)
                        nuevoTracking.IdTbl_Pagos = registroPagoEncontrado.Id;
                        nuevoTracking.FechaCambio = Convert.ToDateTime(DateTime.Now.Date);
                        nuevoTracking.ChequeAnterior = registroPagoEncontrado.FolioCheque;
                        nuevoTracking.ChequeNuevo = null;
                        nuevoTracking.MotivoRefoliacion = "Cambio de referencia de cancelacion";
                        nuevoTracking.RefoliadoPor = "*****";
                        nuevoTracking.EsCancelado = true;
                        nuevoTracking.ReferenciaCancelado = Convert.ToString(  registroFereciaEncontrado.Numero_Referencia );
                        nuevoTracking.IdCat_EstadoCancelado = 1;
                        nuevoTracking.Activo = true;


                        //cambia el IdreferenciaCancelado al nuevo id
                        registroPagoEncontrado.IdTbl_Referencias_Cancelaciones = IdReferenciaCancelado;

                        //RESTA 1 FORMA DE PAGO A LA REFERENCIA ACTUAL (DONDE ESTABA) Y SE LA PASA A LA NUEVA REFERENCIA DONDE SE ESTA CARGANDO (suma 1 a donde va estar ahora)
                        registroFerecia_Original.FormasPagoDentroReferencia -= 1;
                        registroFerecia_Original.FechaActualizacion_AgregoOQuitoFormas = DateTime.Now;

                        registroFereciaEncontrado.FormasPagoDentroReferencia += 1;
                        registroFereciaEncontrado.FechaActualizacion_AgregoOQuitoFormas = DateTime.Now;

                        //Se Cambio de referencia con Exito
                        numeroError = 7;
                    }
                    else
                    {
                        //No se puede cambiar un pago a la misma referencia
                        return numeroError = 3;
                    }


                    Tbl_SeguimientoHistoricoFormas_Pagos Seguimiento_modificado = repoSeguimientoHistorico.Agregar_Transaccionadamente(nuevoTracking);
                    Tbl_Pagos Pago_Modificado = repositorioTBlPagos.Modificar_Transaccionadamente(registroPagoEncontrado);
                    Tbl_Referencias_Cancelaciones ReferenciaCancelado_Modificado = repoRerenciaCancelado.Modificar_Transaccionadamente(registroFereciaEncontrado);
                    if (Pago_Modificado.Id == Seguimiento_modificado.IdTbl_Pagos && Pago_Modificado.IdTbl_Referencias_Cancelaciones == ReferenciaCancelado_Modificado.Id)
                    {
                        transaccion.GuardarCambios();
                        return numeroError ;
                    }
                }
                else 
                {
                    return numeroError = 2;
                }
              
            }
            else
            {
                return numeroError = 1;
            }

            return numeroError;
        }


        public static int RevocarCheque_ReferenciaCancelado(int IdRegistroCancelar)
        {
            int numeroError = 0;
            //Mensajes de ERRORES
            // 1 => /No es un cheque sino una dispercion (APLICA PARA AGREGAR, ACTUALIZAR O REVOCAR UN CHEQUE DE LA REFERENCIA DE CANCELACION)
            // 2 => /No Existe la referencia (APLICA PARA AGREGAR, ACTUALIZAR O REVOCAR UN CHEQUE DE LA REFERENCIA DE CANCELACION)
            // 3 => No se puede cambiar un cheque a la misma referencia
            // 4 => La referencia no puede ser removida porque aun no tiene una (APLiCA SOLO PARA QUITAR UN PAGO DE UNA REFERENCIA)

            //Mensajes Exitosos
            // 6 => /Se agrego a una referencia exitosamente
            // 7 => /Se Cambio de referencia con Exito
            // 8 => La referencias fue removida con exito de la referencia de cancelacion (APLiCA SOLO PARA QUITAR UN PAGO DE UNA REFERENCIA)


            var transaccion = new Transaccion();
            //Modificar
            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);
            var repoRerenciaCancelado = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);

            var repoSeguimientoHistorico = new Repositorio<Tbl_SeguimientoHistoricoFormas_Pagos>(transaccion);



            //TBL_PAGOS
            Tbl_Pagos registroPagoEncontrado = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroCancelar && x.Activo == true);
            //TBL_REFERENCIAS_CANCELADOS
            Tbl_Referencias_Cancelaciones registroFereciaEncontrado_Original = repoRerenciaCancelado.Obtener(x => x.Id == registroPagoEncontrado.IdTbl_Referencias_Cancelaciones && x.Activo == true);


            //Agregar un seguimiento
            Tbl_SeguimientoHistoricoFormas_Pagos nuevoTracking = new Tbl_SeguimientoHistoricoFormas_Pagos();

            //valida que el pago sea cheque y que exista la referencia a la que se agregara
            if (registroPagoEncontrado.IdCat_FormaPago_Pagos == 1)
            {
                if (registroFereciaEncontrado_Original != null)
                {
                        /*********************************************************************************************************************************************************/
                        /***************  Agrega un registro al tracking, agrega referenciaCancelado a  tbl_pago, y Actualiza La referencia de cancelacion sumando la cantidad de cheques **************/
                        /*********************************************************************************************************************************************************/
                        nuevoTracking.IdTbl_Pagos = registroPagoEncontrado.Id;
                        nuevoTracking.FechaCambio = Convert.ToDateTime(DateTime.Now.Date);
                        nuevoTracking.ChequeAnterior = registroPagoEncontrado.FolioCheque;
                        nuevoTracking.ChequeNuevo = null;
                        nuevoTracking.MotivoRefoliacion = "Se removio de la referencia "+ registroFereciaEncontrado_Original.Numero_Referencia;
                        nuevoTracking.RefoliadoPor = null;
                        nuevoTracking.EsCancelado = false;
                        nuevoTracking.ReferenciaCancelado = null;
                        nuevoTracking.IdCat_EstadoCancelado = null;
                        nuevoTracking.Activo = true;

                        //Actualiza id de la referencia del pago a null porque dejo de estar dentro de una referencia
                        registroPagoEncontrado.IdTbl_Referencias_Cancelaciones = null;

                        //Actualiza TBL_ReferenciaCancelados descontando el pago que se revoco
                        registroFereciaEncontrado_Original.FormasPagoDentroReferencia -= 1;
                        registroFereciaEncontrado_Original.FechaActualizacion_AgregoOQuitoFormas = DateTime.Now;



                   

                    Tbl_SeguimientoHistoricoFormas_Pagos Seguimiento_modificado = repoSeguimientoHistorico.Agregar_Transaccionadamente(nuevoTracking);
                    Tbl_Pagos Pago_Modificado = repositorioTBlPagos.Modificar_Transaccionadamente(registroPagoEncontrado);
                    Tbl_Referencias_Cancelaciones ReferenciaCancelado_Modificado = repoRerenciaCancelado.Modificar_Transaccionadamente(registroFereciaEncontrado_Original);
                    if (Pago_Modificado.Id == Seguimiento_modificado.IdTbl_Pagos)
                    {
                        transaccion.GuardarCambios();
                        //Se agrego a una referencia con exito con exito
                        return numeroError = 8;
                    }
                }
                else
                {
                    return numeroError = 4;
                }

            }
            else
            {
                return numeroError = 1;
            }

            return numeroError;
        }





        public static List<HistoricoReposicionesDTO> ObtenerHistoricoReposiciones(int IdRegistro)
        {
            Transaccion nuevaTransaccion = new Transaccion();
            var repositorioReposiciones = new Repositorio<Tbl_SeguimientoHistoricoFormas_Pagos>(nuevaTransaccion);
            var repoEstadoCancelado = new Repositorio<Cat_EstadoCancelados_Pagos>(nuevaTransaccion);
            List<Tbl_SeguimientoHistoricoFormas_Pagos> historicoEncontrado = repositorioReposiciones.ObtenerPorFiltro(x => x.IdTbl_Pagos == IdRegistro && x.Activo).OrderBy(x => x.Id).ToList();


            int iterador = 1;
            List<HistoricoReposicionesDTO> historicoDTO = new List<HistoricoReposicionesDTO>();
            foreach (Tbl_SeguimientoHistoricoFormas_Pagos registro in historicoEncontrado)
            {
                HistoricoReposicionesDTO nuevoRegistroDTO = new HistoricoReposicionesDTO();
                nuevoRegistroDTO.Id = iterador++;
                nuevoRegistroDTO.FechaCambio = registro.FechaCambio.ToString("MM/dd/yyyy");
                nuevoRegistroDTO.MotivoRefoliacion = registro.MotivoRefoliacion;
                nuevoRegistroDTO.ChequeAnterior = registro.ChequeAnterior;

                nuevoRegistroDTO.ChequeNuevo = registro.ChequeNuevo == null ? "" : Convert.ToString(registro.ChequeNuevo);
                nuevoRegistroDTO.RepuestoPor = registro.RefoliadoPor;
                nuevoRegistroDTO.EsCancelado = registro.EsCancelado == true? "True" : "" ;
                nuevoRegistroDTO.ReferenciaCancelado = registro.ReferenciaCancelado;
                nuevoRegistroDTO.DescripcionCancelado = registro.IdCat_EstadoCancelado == null ? "" : repoEstadoCancelado.Obtener(x => x.Id == registro.IdCat_EstadoCancelado).Estado;
                historicoDTO.Add(nuevoRegistroDTO);

            }


            return historicoDTO;
        }



        public static string ObtenerNumeroReferenciaPago(int IdRegistro)
        {
            string referencia = "";
            Transaccion nuevaTransaccion = new Transaccion();
            var repo_TBlPago = new Repositorio<Tbl_Pagos>(nuevaTransaccion);
            var repo_ReferenciaCancelados = new Repositorio<Tbl_Referencias_Cancelaciones>(nuevaTransaccion);

            Tbl_Pagos pagoObtenido = repo_TBlPago.Obtener(x => x.Id == IdRegistro);

            Tbl_Referencias_Cancelaciones referenciaObtenida = repo_ReferenciaCancelados.Obtener(x => x.Id == pagoObtenido.IdTbl_Referencias_Cancelaciones && x.EsCancelado == false && x.Activo == true);

            if (pagoObtenido != null && referenciaObtenida != null)
            {
                referencia = referenciaObtenida.Numero_Referencia;
            }

            return referencia;
        }


    }
}
