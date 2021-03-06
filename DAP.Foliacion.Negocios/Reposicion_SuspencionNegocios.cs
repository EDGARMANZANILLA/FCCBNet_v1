using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Datos;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.BuscardorChequeDTO;
using Datos;
using DAP.Foliacion.Entidades.DTO.Reposicion_SuspencionNegociosDTO;
using DAP.Foliacion.Negocios.FoliarDBF;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using System.Data.SqlClient;
using System.Data;

namespace DAP.Foliacion.Negocios
{
    public class Reposicion_SuspencionNegocios
    {

        public static List<DetallesBusqueda> ObtenerDetallesDeCheque(int IdFiltro,int BuscarDato)
        {
            //Id filtro {1} es para el pago y el {2 } Para el numero de empleado
            List<DetallesBusqueda> detalleEncontrado = new List<DetallesBusqueda>();

            var transaccion = new Transaccion();

            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);
            var repositorioEstadoPagos = new Repositorio<Cat_EstadosPago_Pagos>(transaccion);
            var repositorioTipoPago = new Repositorio<Cat_FormasPago_Pagos>(transaccion);

            List<Tbl_Pagos> chequeEncontrado = new List<Tbl_Pagos>();
            if (IdFiltro == 1)
            {
                chequeEncontrado = repositorioTBlPagos.ObtenerPorFiltro(x => x.FolioCheque == BuscarDato).ToList();

            }
            else 
            {
                chequeEncontrado = repositorioTBlPagos.ObtenerPorFiltro(x => x.NumEmpleado == BuscarDato).ToList();

            }

            foreach (Tbl_Pagos nuevoPagoEncontrado in chequeEncontrado)
            {

                DetallesBusqueda nuevoChequeEncontrado = new DetallesBusqueda();
                nuevoChequeEncontrado.IdRegistro = nuevoPagoEncontrado.Id;
                nuevoChequeEncontrado.Id_nom = nuevoPagoEncontrado.Id_nom;
                nuevoChequeEncontrado.ReferenciaBitacora = nuevoPagoEncontrado.ReferenciaBitacora;
                nuevoChequeEncontrado.Quincena = nuevoPagoEncontrado.Quincena;
                nuevoChequeEncontrado.NumEmpleado = nuevoPagoEncontrado.NumEmpleado;
                nuevoChequeEncontrado.NombreBeneficiaro = nuevoPagoEncontrado.EsPenA == true ? nuevoPagoEncontrado.BeneficiarioPenA : nuevoPagoEncontrado.NombreEmpleado;
                nuevoChequeEncontrado.NumBene = nuevoPagoEncontrado.EsPenA == true ? nuevoPagoEncontrado.NumBeneficiario : "";
                nuevoChequeEncontrado.FolioCheque = Convert.ToInt32( nuevoPagoEncontrado.FolioCheque );
                nuevoChequeEncontrado.Liquido = nuevoPagoEncontrado.ImporteLiquido;
                nuevoChequeEncontrado.EstadoCheque = nuevoPagoEncontrado.IdCat_EstadoPago_Pagos != 0 ? repositorioEstadoPagos.Obtener(x => x.Id == nuevoPagoEncontrado.IdCat_EstadoPago_Pagos).Descrip : "No Definido";
                nuevoChequeEncontrado.TipoPago = nuevoPagoEncontrado.IdCat_FormaPago_Pagos != 0 ? repositorioTipoPago.Obtener(x => x.Id == nuevoPagoEncontrado.IdCat_FormaPago_Pagos).Descrip : "No Definido";
                detalleEncontrado.Add(nuevoChequeEncontrado);
            }

            return detalleEncontrado;
        }



     




        public static DetallesRegistroDTO ObtenerDetalleCompletoIdRegistro(int IdRegistroBuscar)
        {

            var transaccion = new Transaccion();

            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);

            var repositorioCuentasBancarias = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var repositorioEstadoPagos = new Repositorio<Cat_EstadosPago_Pagos>(transaccion);
            var repoFormaPago = new Repositorio<Cat_FormasPago_Pagos>(transaccion);



            Tbl_Pagos registroEncontrado = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroBuscar);

            Tbl_CuentasBancarias bancoEncontrado = repositorioCuentasBancarias.Obtener(x => x.Id == registroEncontrado.IdTbl_CuentaBancaria_BancoPagador);


            DetallesRegistroDTO nuevoDetalle = new DetallesRegistroDTO();

            nuevoDetalle.IdRegistro = registroEncontrado.Id ;
            nuevoDetalle.Id_nom = " || " + registroEncontrado.Id_nom + " || - || " + registroEncontrado.Nomina + " || - || "+ registroEncontrado.Adicional+ " || ";
            nuevoDetalle.ReferenciaBitacora = registroEncontrado.ReferenciaBitacora;

            nuevoDetalle.Quincena = registroEncontrado.Quincena;
            nuevoDetalle.NumEmpleado = registroEncontrado.NumEmpleado;
            nuevoDetalle.NombreEmpleado = registroEncontrado.NombreEmpleado;

            nuevoDetalle.Delegacion = registroEncontrado.Delegacion;
            nuevoDetalle.Folio = Convert.ToInt32( registroEncontrado.FolioCheque );
            nuevoDetalle.Liquido = registroEncontrado.ImporteLiquido;

            nuevoDetalle.TipoPago = repoFormaPago.Obtener(x => x.Id == registroEncontrado.IdCat_FormaPago_Pagos).Descrip;
            nuevoDetalle.EstadoCheque = registroEncontrado.IdCat_EstadoPago_Pagos != 0 ? repositorioEstadoPagos.Obtener(x => x.Id == registroEncontrado.IdCat_EstadoPago_Pagos).Descrip : "No Definido";
            nuevoDetalle.BancoPagador = bancoEncontrado.NombreBanco;
            nuevoDetalle.CuentaPagadora = bancoEncontrado.Cuenta;
            nuevoDetalle.EsSuspencion = registroEncontrado.TieneSuspensionDispersion == null ? "False" : "Verdadero";

            //nuevoDetalle.EsPenA = regitroEncontrado.EsPenA;
            nuevoDetalle.EsPenA = registroEncontrado.EsPenA == null ? false : registroEncontrado.EsPenA;
            nuevoDetalle.NumBeneficiarioPenA = registroEncontrado.NumBeneficiario;
            nuevoDetalle.NombreBeneficiarioPenA = registroEncontrado.BeneficiarioPenA;

            //TieneSeguimientoHistorico =;
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




        /// <summary>
        /// devuelve una cadena que dicta que se puede hacer con ese pago si una reposicion o suspencion
        /// El parametro FormaPago = 1 => CHEQUE y solo puede ser para reposicion
        /// El parametro FormaPago = 2 => Pagomatico y solo puede ser suspendido 
        /// </summary>
        /// <param name="IdRegistroPago">Identificador del pago que que quiere suspender o reponer</param>
        /// <returns></returns>
        public static string VerificaFormaPagoEsActivoYQueSePuedeHacer(int IdRegistroPago) 
        {

            
            //Forma de Pago
            // 1 => CHEQUE y solo puede ser para reposicion
            // 2 => Pagomatico y solo puede ser suspendido 
            string soloPuedeHacer;

            var transaccion = new Transaccion();

            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);

            Tbl_Pagos formaPagoEncontrado = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroPago && x.Activo == true);

            if (formaPagoEncontrado.IdCat_FormaPago_Pagos == 1)
            {

                //Si no tiene un IdReferenciaCancelacion
                if (formaPagoEncontrado.Tbl_Referencias_Cancelaciones != null)
                {
                    soloPuedeHacer = "REFERENCIACANCELADO";
                }
                else 
                {
                    soloPuedeHacer = "REPONER";
                }


            }
            else if (formaPagoEncontrado.IdCat_FormaPago_Pagos == 2)
            {
                soloPuedeHacer = "SUSPENDER";
            }
            else 
            {
                soloPuedeHacer = "NO_EXISTE";
            }

            return soloPuedeHacer;
        }







        //public static string SuspenderFormaPago_Borrar(int IdFormaaPago)
        //{
        //    string resultadoSuspencion = "Error";
        //    var transaccion = new Transaccion();
        //    var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);
        //    var repositorioTBlRefoliados = new Repositorio<Tbl_SeguimientoHistoricoFormas_Pagos>(transaccion);

        //    Tbl_Pagos suspenderFormaPago = repositorioTBlPagos.Obtener(x => x.Id == IdFormaaPago);

           


        //    string CadenaNumEmpleado = ObtenerNumeroEmpleadoCincoDigitos(suspenderFormaPago.NumEmpleado);

        //    DatosCompletosBitacoraDTO datosNominaCompleto = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(suspenderFormaPago.Id_nom, suspenderFormaPago.Anio);


        //    int registroSuspendidoDBF = 0;
        //    /*******************************************************************************************************************************************************/
        //    ///************************************************ ACTUALIZAR DBF EN UNA RUTA DEL SERVIDOR **********************************************************////
        //    string resultado_SuspencionDBF = FolearDBFEnServerNegocios.SuspenderPagoEnRutaDBF(datosNominaCompleto, suspenderFormaPago, CadenaNumEmpleado);
        //    if (resultado_SuspencionDBF.Contains("Cannot open file"))
        //    {
        //        return "LA BASE : || " + datosNominaCompleto.Ruta + datosNominaCompleto.RutaNomina + " || SE ENCUENTRA ABIERTA EN OTRA PC ; CIERRE LA BASE E INTENTE DE NUEVO";
        //    }
        //    else if (resultado_SuspencionDBF.Equals("1"))
        //    {
        //         registroSuspendidoDBF = Convert.ToInt32(resultado_SuspencionDBF);
        //    }
        //    else
        //    {
        //        return resultado_SuspencionDBF + " error DBF";
        //    }


        //    /********************************************************************************************************************************************************/
        //    ///*******************************************  ACTUALIZA EN ALPHA INTERFACES **************************************************************************////
        //    //Actualiza un registro suspendido en Interfaces
        //    string Anio = "";
        //    if (suspenderFormaPago.Anio != Convert.ToInt32(DateTime.Now.Year))
        //    {
        //        Anio = Convert.ToString(suspenderFormaPago.Anio);
        //    }


            
        //    int resultadoSuspendidoInterfaces = 0;
        //    if (registroSuspendidoDBF == 1) 
        //    {
        //        string query;
        //        if (datosNominaCompleto.EsPenA)
        //        {
        //            query = "UPDATE interfaces"+Anio+".dbo."+datosNominaCompleto.An+" SET NUM_CHE = '11111111', OBSERVA = 'TALON POR CHEQUE', TALONXCH = 1  WHERE NUM = '"+CadenaNumEmpleado+"' and Liquido = "+ suspenderFormaPago.ImporteLiquido+ " and deleg = "+suspenderFormaPago.Delegacion+" and BENEF = '"+suspenderFormaPago.NumBeneficiario+"'";
        //        }
        //        else
        //        {
        //            query = "UPDATE interfaces"+Anio+".dbo."+datosNominaCompleto.An+" SET NUM_CHE = '11111111', OBSERVA = 'TALON POR CHEQUE', TALONXCH = 1  WHERE NUM = '" + CadenaNumEmpleado + "' and Liquido = " + suspenderFormaPago.ImporteLiquido + " and deleg = " + suspenderFormaPago.Delegacion + "";
        //        }

        //        resultadoSuspendidoInterfaces = Reposicion_SuspencionDBSinORM.SuspenderDispercionEnSql_transaccionado(query);

        //    }



        //    //Actualizacion de la tabla de la DB de interfaces donde se encuentra el registro del empleado (Alcualmente apunta al local porque no esta en produccion)
        //    //  int registroActualizado = Reposicion_SuspencionDBSinORM.SuspenderDispercion(formaPagoEncontrada.An, NumCompleto);
        //    /****************************************************************************************************************************************************/
        //    ///***************************  GUARDA UN REGISTRO PARA EL SEGUIMIENTO DE LO QUE SUCEDIO CON ESTE PAGO ********************************************////
        //    int resultadoDbFoliacion = 0;
        //    if (resultadoSuspendidoInterfaces == 1)
        //    {
        //        try
        //        {
        //            //Crea el inicio de un nuevo trackin de formas de pago (cheques)
        //            Tbl_SeguimientoHistoricoFormas_Pagos nuevaRefoliacion = new Tbl_SeguimientoHistoricoFormas_Pagos();
        //            nuevaRefoliacion.IdTbl_Pagos = suspenderFormaPago.Id;
        //            nuevaRefoliacion.FechaCambio = DateTime.Now;
        //            nuevaRefoliacion.ChequeAnterior = Convert.ToInt32( suspenderFormaPago.FolioCheque );
        //            nuevaRefoliacion.ChequeNuevo = 11111111;
        //            nuevaRefoliacion.MotivoRefoliacion = "SUSPENCION DE DISPERSION";
        //            nuevaRefoliacion.RefoliadoPor = "*****";
        //            nuevaRefoliacion.EsCancelado = false;
        //            nuevaRefoliacion.Activo = true;
        //            Tbl_SeguimientoHistoricoFormas_Pagos trackinAgregado = repositorioTBlRefoliados.Agregar(nuevaRefoliacion);


        //            if (trackinAgregado.Id > 0)
        //            {
        //                //Edita la tabla Tbl_pagos para guardar el registro del la suspension
        //                suspenderFormaPago.FolioCheque = 11111111;
        //                suspenderFormaPago.TieneSeguimientoHistorico = true;
        //                suspenderFormaPago.IdCat_FormaPago_Pagos = 1;
        //                Tbl_Pagos pagoModificado = repositorioTBlPagos.Modificar(suspenderFormaPago);

        //                if(pagoModificado.Id > 0) 
        //                {
        //                    resultadoDbFoliacion = 1;
        //                }
        //            }

        //        }
        //        catch (Exception E)
        //        {
        //            //si entra es por que hubo algun error y se trata de forzar la suspencion 

        //            transaccion.Dispose();
                    

        //            return resultadoSuspencion = "Error";
        //        }

        //    }


        //    if ((registroSuspendidoDBF + resultadoSuspendidoInterfaces) == (resultadoDbFoliacion * 2)) 
        //    {
        //        //transaccion.GuardarCambios();
        //        return resultadoSuspencion = "Correcto";
        //    }


        //    return resultadoSuspencion;
        //}



        //public static string ReponerNuevaFormaPago_Borrar(int IdRegistroPago, int ReponerNuevoFolio, string motivoReposicion)
        //{
        //    var transaccion = new Transaccion();
        //    var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);
        //    var repositorioTBlDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
        //    var repositorioTBlContenedor = new Repositorio<Tbl_InventarioContenedores>(transaccion);
        //    var repositorioTBlInventario = new Repositorio<Tbl_Inventario>(transaccion);
           

        //    Tbl_Pagos reponerFormaPago = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroPago && x.IdCat_FormaPago_Pagos == 1 && x.Activo == true);
        //    DatosCompletosBitacoraDTO datosNominaCompleto = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(reponerFormaPago.Id_nom, reponerFormaPago.Anio);
        //    string CadenaNumEmpleado = ObtenerNumeroEmpleadoCincoDigitos(reponerFormaPago.NumEmpleado);
        //    /************************************************************************************************************************************************************/
        //    ///***************************  VERFICA QUE EL FOLIO ESTE DISPONIBLE PARA SER USADO ************************************************************************////
        //    Tbl_InventarioDetalle chequeVerificado = DisponibilidadFolioInventarioDetalle(reponerFormaPago.IdTbl_CuentaBancaria_BancoPagador, ReponerNuevoFolio);
        //    if (chequeVerificado.IdIncidencia != null )
        //    {
        //        return "El Folio :"+ReponerNuevoFolio+" no esta disponible para su uso, vaya a ver detalles para mas informacion o comuniquese con el desarrollador";
        //    }
        //    else
        //    {
        //        int registroReposicionDBF = 0;
        //        int registroReposicionDBInterfaces = 0;
        //        //string NumCompleto5Digitos = ObtenerNumeroEmpleadoCincoDigitos(formaPagoEncontrado.NumEmpleado);
        //        /******************************************************************************************************************************************************/
        //        /**********************************   MODIFICA LA BASE DBF EN EL SERVIDOR ( CAMBIA EL FOLIO)    *******************************************************/
        //        /******************************************************************************************************************************************************/
        //        string executaQuery;


        //        if (datosNominaCompleto.EsPenA)
        //        {
        //            executaQuery = "UPDATE [" + datosNominaCompleto.RutaNomina + "] SET NUM_CHE = '" + ReponerNuevoFolio + "'  WHERE NUM = '" + CadenaNumEmpleado + "' and Liquido = " + reponerFormaPago.ImporteLiquido + " and deleg = '" + reponerFormaPago.Delegacion+"' and BENEF = '" + reponerFormaPago.NumBeneficiario + "'";
        //        }
        //        else
        //        {
        //            executaQuery = "UPDATE [" + datosNominaCompleto.RutaNomina + "] SET NUM_CHE = '" + ReponerNuevoFolio + "'  WHERE NUM = '" + CadenaNumEmpleado + "' and Liquido = " + reponerFormaPago.ImporteLiquido + " and deleg = '"+reponerFormaPago.Delegacion+"'";
        //        }

        //        string resultadoReposicionDBF = FolearDBFEnServerNegocios.ReponerPagoEnRutaDBF(datosNominaCompleto.Ruta, executaQuery);

        //        if (resultadoReposicionDBF.Contains("Cannot open file"))
        //        {
        //            return "LA BASE : || " + datosNominaCompleto.Ruta + datosNominaCompleto.RutaNomina + " || SE ENCUENTRA ABIERTA EN OTRA PC ; CIERRE LA BASE E INTENTE DE NUEVO";
        //        }
        //        else if (resultadoReposicionDBF.Equals("1"))
        //        {
        //            registroReposicionDBF = Convert.ToInt32(resultadoReposicionDBF);
        //        }
        //        else
        //        {
        //            return resultadoReposicionDBF + " error DBF";
        //        }


        //        /**************************************************************************************************************************************************/
        //        /**********************************   MODIFICA EL REGISTRO EN DB INTERFACES DE ALPHA (CAMBIA EL FOLIO)  *******************************************/
        //        string Anio = "";

        //        if (reponerFormaPago.Anio != Convert.ToInt32(DateTime.Now.Year))
        //        {
        //            Anio = Convert.ToString(reponerFormaPago.Anio);
        //        }


        //        if (registroReposicionDBF == 1)
        //        {
        //            string queryInterfas = "";
        //            if (datosNominaCompleto.EsPenA)
        //            {
        //                queryInterfas = "UPDATE interfaces" + Anio + ".dbo." + datosNominaCompleto.An + " SET NUM_CHE = '" + ReponerNuevoFolio + "'  WHERE NUM = '" + CadenaNumEmpleado + "' and Liquido = " + reponerFormaPago.ImporteLiquido + " and deleg = '" + reponerFormaPago.Delegacion + "' and BENEF = '" + reponerFormaPago.NumBeneficiario + "'";
        //            }
        //            else
        //            {
        //                queryInterfas = "UPDATE interfaces" + Anio + ".dbo." + datosNominaCompleto.An + " SET NUM_CHE = '" + ReponerNuevoFolio + "'  WHERE NUM = '" + CadenaNumEmpleado + "' and Liquido = " + reponerFormaPago.ImporteLiquido + " and deleg = '" + reponerFormaPago.Delegacion + "'";
        //            }

        //            registroReposicionDBInterfaces = Reposicion_SuspencionDBSinORM.ReponerFormaPago(queryInterfas);
        //        }



        //        /****************************************************************************************************************************************************/
        //        /*************************************** GUARDA UN REGISTRO DEL MOVIMIENTO Y MODIFICA EL REGISTRO EN DB FOLIACION (CAMBIA EL FOLIO)   *******************************************/
        //        if (registroReposicionDBInterfaces == 1)
        //        {

        //            try
        //            {
        //                var repositorioTBlRefoliados = new Repositorio<Tbl_SeguimientoHistoricoFormas_Pagos>(transaccion);

        //                Tbl_SeguimientoHistoricoFormas_Pagos nuevaRefoliacion = new Tbl_SeguimientoHistoricoFormas_Pagos();
        //                nuevaRefoliacion.IdTbl_Pagos = reponerFormaPago.Id;
        //                nuevaRefoliacion.FechaCambio = DateTime.Now;
        //                nuevaRefoliacion.ChequeAnterior = Convert.ToInt32( reponerFormaPago.FolioCheque );
        //                nuevaRefoliacion.ChequeNuevo = ReponerNuevoFolio;
        //                nuevaRefoliacion.MotivoRefoliacion = motivoReposicion;
        //                nuevaRefoliacion.RefoliadoPor = "*****";
        //                nuevaRefoliacion.EsCancelado = false;
        //                nuevaRefoliacion.Activo = true;
        //                Tbl_SeguimientoHistoricoFormas_Pagos trackinAgregado = repositorioTBlRefoliados.Agregar(nuevaRefoliacion);


        //                if (trackinAgregado.Id > 0)
        //                {
        //                    /*Guardar los cheques y descontarlos del inventario*/
        //                    Tbl_InventarioDetalle folioEnInventarioEncontrado = repositorioTBlDetalle.Obtener(x => x.Id == chequeVerificado.Id);
        //                    folioEnInventarioEncontrado.IdIncidencia = 3; //3 porque ya fue foliado por primera vez
        //                    folioEnInventarioEncontrado.FechaIncidencia = DateTime.Now;
        //                    repositorioTBlDetalle.Modificar_Transaccionadamente(folioEnInventarioEncontrado);


        //                    Tbl_InventarioContenedores descontarFolioDelContenedor = repositorioTBlContenedor.Obtener(x => x.Id == folioEnInventarioEncontrado.IdContenedor);
        //                    descontarFolioDelContenedor.FormasFoliadas += 1;
        //                    descontarFolioDelContenedor.FormasDisponiblesActuales -= 1;
        //                    repositorioTBlContenedor.Modificar_Transaccionadamente(descontarFolioDelContenedor);

        //                    Tbl_Inventario descontarFolioDeInventario = repositorioTBlInventario.Obtener(x => x.Id == descontarFolioDelContenedor.IdInventario);
        //                    descontarFolioDeInventario.FormasDisponibles -= 1;
        //                    descontarFolioDeInventario.UltimoFolioUtilizado = Convert.ToString(folioEnInventarioEncontrado.NumFolio);
        //                    repositorioTBlInventario.Modificar_Transaccionadamente(descontarFolioDeInventario);





        //                    reponerFormaPago.FolioCheque = ReponerNuevoFolio;
        //                    reponerFormaPago.TieneSeguimientoHistorico = true;
        //                    reponerFormaPago.IdTbl_InventarioDetalle = chequeVerificado.Id;
        //                    Tbl_Pagos entidadGuardada = repositorioTBlPagos.Modificar(reponerFormaPago);



        //                    if (entidadGuardada.Activo)
        //                    {
        //                        transaccion.GuardarCambios();
        //                        return "CORRECTO";
        //                    }
        //                }
        //            }
        //            catch (Exception E)
        //            {

        //                return E.Message;

        //            }

        //        }

        //    }


          

        //    return "Error";
        //}



        public static string SuspenderFormaPago(int IdFormaaPago)
        {
            string resultadoSuspencion = "Error";
            var transaccion = new Transaccion();
            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);
            var repositorioTBlRefoliados = new Repositorio<Tbl_SeguimientoHistoricoFormas_Pagos>(transaccion);

            Tbl_Pagos suspenderFormaPago = repositorioTBlPagos.Obtener(x => x.Id == IdFormaaPago);


            if (suspenderFormaPago.Nomina != "08")
            {

                DatosCompletosBitacoraDTO datosNominaCompleto = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(suspenderFormaPago.Id_nom, suspenderFormaPago.Anio);
                string cadenaNumEmpleado = ObtenerNumeroEmpleadoCincoDigitos(suspenderFormaPago.NumEmpleado);

                int registroSuspendidoDBF = 0;
                int resultadoSuspendidoInterfaces = 0;
                /******************************************************************************************************************************************************/
                /**********************************   MODIFICA LA BASE DBF EN EL SERVIDOR ( CAMBIA EL FOLIO)    *******************************************************/
                /******************************************************************************************************************************************************/
                AlertasAlFolearPagomaticosDTO verificacionAlerta = FolearDBFEnServerNegocios.ModificacionDBF_SuspenderReponer_YLimpiarUnRegistro(1 /*Opncion 1 por ser una suspencion*/ , datosNominaCompleto, cadenaNumEmpleado, suspenderFormaPago, ""/* Parametro vacio por ser una suspencion no se ocupa*/);
                if (verificacionAlerta.IdAtencion == 200)
                {
                    registroSuspendidoDBF = verificacionAlerta.NumeroRegistrosActualizados;
                }
                else
                {
                    return verificacionAlerta.Detalle;
                }


                /**************************************************************************************************************************************************************/
                /**********************************************       ACTUALIZA EN ALPHA INTERFACES AN       ******************************************************************/
                /**************************************************************************************************************************************************************/
                if (registroSuspendidoDBF == 1)
                {
                    resultadoSuspendidoInterfaces = FoliarConsultasDBSinEntity.SupenderDispercionDePagomatico(datosNominaCompleto, cadenaNumEmpleado, suspenderFormaPago);
                }



                //Actualizacion de la tabla de la DB de interfaces donde se encuentra el registro del empleado (Alcualmente apunta al local porque no esta en produccion)
                //  int registroActualizado = Reposicion_SuspencionDBSinORM.SuspenderDispercion(formaPagoEncontrada.An, NumCompleto);
                /****************************************************************************************************************************************************/
                ///***************************  GUARDA UN REGISTRO PARA EL SEGUIMIENTO DE LO QUE SUCEDIO CON ESTE PAGO ********************************************////
                int resultadoDbFoliacion = 0;
                if (resultadoSuspendidoInterfaces == 1)
                {
                    try
                    {
                        //Crea el inicio de un nuevo trackin de formas de pago (cheques)
                        Tbl_SeguimientoHistoricoFormas_Pagos nuevaRefoliacion = new Tbl_SeguimientoHistoricoFormas_Pagos();
                        nuevaRefoliacion.IdTbl_Pagos = suspenderFormaPago.Id;
                        nuevaRefoliacion.FechaCambio = DateTime.Now;
                        nuevaRefoliacion.ChequeAnterior = Convert.ToInt32(suspenderFormaPago.FolioCheque);
                        nuevaRefoliacion.ChequeNuevo = 11111111;
                        nuevaRefoliacion.MotivoRefoliacion = "SUSPENCION DE DISPERSION";
                        nuevaRefoliacion.RefoliadoPor = "*****";
                        nuevaRefoliacion.EsCancelado = false;
                        nuevaRefoliacion.Activo = true;
                        Tbl_SeguimientoHistoricoFormas_Pagos trackinAgregado = repositorioTBlRefoliados.Agregar(nuevaRefoliacion);


                        if (trackinAgregado.Id > 0)
                        {
                            //Edita la tabla Tbl_pagos para guardar el registro del la suspension
                            suspenderFormaPago.FolioCheque = 11111111;
                            suspenderFormaPago.TieneSeguimientoHistorico = true;
                            suspenderFormaPago.TieneSuspensionDispersion = true;
                            suspenderFormaPago.IdCat_FormaPago_Pagos = 1;
                            Tbl_Pagos pagoModificado = repositorioTBlPagos.Modificar(suspenderFormaPago);

                            if (pagoModificado.Id > 0)
                            {
                                resultadoDbFoliacion = 1;
                            }
                        }

                    }
                    catch (Exception E)
                    {
                        //si entra es por que hubo algun error y se trata de forzar la suspencion 

                        transaccion.Dispose();


                        return resultadoSuspencion = "Error";
                    }

                }


                if ((registroSuspendidoDBF + resultadoSuspendidoInterfaces) == (resultadoDbFoliacion * 2))
                {
                    //transaccion.GuardarCambios();
                    return resultadoSuspencion = "Correcto";
                }


            }
            else 
            {
                return "No se puede suspender una dispercion de pension alimenticia";
            }

      

            return resultadoSuspencion;
        }


        public static string ReponerNuevaFormaPago(int IdRegistroPago, int ReponerNuevoFolio , string motivoReposicion )
        {
            var transaccion = new Transaccion();

            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);
            Tbl_Pagos reponerFormaPago = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroPago && x.IdCat_FormaPago_Pagos == 1 && x.Activo == true);
            
           

            var repositorioTblBanco = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            Tbl_CuentasBancarias cuentaBancariaEncontrada = repositorioTblBanco.Obtener(x => x.Id == reponerFormaPago.IdTbl_CuentaBancaria_BancoPagador && x.Activo == true);

            
            int idInventario = cuentaBancariaEncontrada != null ? Convert.ToInt32( cuentaBancariaEncontrada.IdInventario ): 0;


            var repoInventarioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            Tbl_InventarioDetalle folioEncontrado = repoInventarioDetalle.Obtener(x => x.IdInventario == idInventario && x.NumFolio == ReponerNuevoFolio);



            /************************************************************************************************************************************************************/
            ///***************************  VERFICA QUE EL FOLIO ESTE DISPONIBLE PARA SER USADO ************************************************************************////
            if (folioEncontrado == null)
            {
                return "El Folio : "+ReponerNuevoFolio + " no existe para el banco : "+cuentaBancariaEncontrada.NombreBanco+" || "+cuentaBancariaEncontrada.Cuenta+"   visite => Inventario => 'ver detalles' para mas informacion ";
            }
            else 
            {
                if (folioEncontrado.FechaIncidencia != null)
                {
                    return "El Folio : "+ReponerNuevoFolio+" no esta disponible para su uso, vaya a ver detalles para mas informacion";
                }
                else
                {
                    DatosCompletosBitacoraDTO datosNominaCompleto = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(reponerFormaPago.Id_nom, reponerFormaPago.Anio);
                    string CadenaNumEmpleado = ObtenerNumeroEmpleadoCincoDigitos(reponerFormaPago.NumEmpleado);
                    int registroReposicionDBF = 0;
                    int registroReposicionDBInterfaces = 0;
                    /******************************************************************************************************************************************************/
                    /**********************************   MODIFICA LA BASE DBF EN EL SERVIDOR ( CAMBIA EL FOLIO)    *******************************************************/
                    /******************************************************************************************************************************************************/
                    AlertasAlFolearPagomaticosDTO verificacionAlerta = FolearDBFEnServerNegocios.ModificacionDBF_SuspenderReponer_YLimpiarUnRegistro(2 /*Opncion 2 por ser una reposicion*/ , datosNominaCompleto, CadenaNumEmpleado, reponerFormaPago, Convert.ToString(ReponerNuevoFolio));

                    if (verificacionAlerta.IdAtencion == 200)
                    {
                        registroReposicionDBF = verificacionAlerta.NumeroRegistrosActualizados;
                    }
                    else
                    {
                        return verificacionAlerta.Detalle;
                    }


                    /**************************************************************************************************************************************************/
                    /**********************************   MODIFICA EL REGISTRO EN DB INTERFACES DE ALPHA (CAMBIA EL FOLIO)  *******************************************/

                    if (registroReposicionDBF == 1)
                    {
                        registroReposicionDBInterfaces = FoliarConsultasDBSinEntity.ReponerCheque(datosNominaCompleto, ReponerNuevoFolio, CadenaNumEmpleado, reponerFormaPago);

                    }



                    /****************************************************************************************************************************************************/
                    /*************************************** GUARDA UN REGISTRO DEL MOVIMIENTO Y MODIFICA EL REGISTRO EN DB FOLIACION (CAMBIA EL FOLIO)   *******************************************/
                    if (registroReposicionDBInterfaces == 1)
                    {
                        try
                        {
                            var repositorioTBlRefoliados = new Repositorio<Tbl_SeguimientoHistoricoFormas_Pagos>(transaccion);

                            Tbl_SeguimientoHistoricoFormas_Pagos nuevaRefoliacion = new Tbl_SeguimientoHistoricoFormas_Pagos();
                            nuevaRefoliacion.IdTbl_Pagos = reponerFormaPago.Id;
                            nuevaRefoliacion.FechaCambio = DateTime.Now;
                            nuevaRefoliacion.ChequeAnterior = Convert.ToInt32(reponerFormaPago.FolioCheque);
                            nuevaRefoliacion.ChequeNuevo = ReponerNuevoFolio;
                            nuevaRefoliacion.MotivoRefoliacion = motivoReposicion;
                            nuevaRefoliacion.RefoliadoPor = "*****";
                            nuevaRefoliacion.EsCancelado = false;
                            nuevaRefoliacion.Activo = true;
                            Tbl_SeguimientoHistoricoFormas_Pagos trackinAgregado = repositorioTBlRefoliados.Agregar(nuevaRefoliacion);


                            if (trackinAgregado.Id > 0)
                            {
                                /*Guardar los cheques y descontarlos del inventario*/

                                bool SeDebeDescontarFomarDePago = true;
                                if (folioEncontrado.IdIncidencia == 2)
                                {
                                    //Si la incidencia es 2 => significa que fue Asignada a un personal y por ende ya fue descontada del inventario cuando se entrego al empleado por ende no se tiene que volver a restar del inventario
                                    //al menos que se haya regresado ese folio (si se regreso se procede a recuperarlo en la vista detalles -> recupertar )
                                    SeDebeDescontarFomarDePago = false;
                                }


                                /** Modifica el registro del folio para ponerle una incidencia **/
                                folioEncontrado.IdIncidencia = 3;
                                folioEncontrado.FechaIncidencia = DateTime.Now;
                                repoInventarioDetalle.Modificar_Transaccionadamente(folioEncontrado);



                                if (SeDebeDescontarFomarDePago) 
                                {
                                    /** Resta una forma de pago disponible del contenedor **/
                                    var repoTBlContenedor = new Repositorio<Tbl_InventarioContenedores>(transaccion);
                                    Tbl_InventarioContenedores descontarFolioDelContenedor = repoTBlContenedor.Obtener(x => x.Id == folioEncontrado.IdContenedor);
                                    descontarFolioDelContenedor.FormasFoliadas += 1;
                                    descontarFolioDelContenedor.FormasDisponiblesActuales -= 1;
                                    repoTBlContenedor.Modificar_Transaccionadamente(descontarFolioDelContenedor);

                                    var repoTBlInventario = new Repositorio<Tbl_Inventario>(transaccion);
                                    Tbl_Inventario descontarFolioDeInventario = repoTBlInventario.Obtener(x => x.Id == folioEncontrado.IdInventario);
                                    descontarFolioDeInventario.FormasDisponibles -= 1;
                                    descontarFolioDeInventario.UltimoFolioUtilizado = Convert.ToString(folioEncontrado.NumFolio);
                                    repoTBlInventario.Modificar_Transaccionadamente(descontarFolioDeInventario);
                                }
                             





                                reponerFormaPago.FolioCheque = ReponerNuevoFolio;
                                reponerFormaPago.TieneSeguimientoHistorico = true;
                                reponerFormaPago.IdTbl_InventarioDetalle = folioEncontrado.Id;
                                Tbl_Pagos entidadGuardada = repositorioTBlPagos.Modificar(reponerFormaPago);

                                if (entidadGuardada.Activo)
                                {
                                    transaccion.GuardarCambios();
                                    return "CORRECTO";
                                }
                            }
                        }
                        catch (Exception E)
                        {

                            return E.Message;

                        }

                    }

                }


            }






            return "Error";
        }





        public static string ObtenerNumeroEmpleadoCincoDigitos(int NumenoEmpleado) 
        {
            string NumCompleto;
            string cadenaNumEmpleado = Convert.ToString(NumenoEmpleado);

            int caracteresTotales = cadenaNumEmpleado.Length; 
            switch (caracteresTotales)
            {
                case 1:
                    NumCompleto = "0000"+cadenaNumEmpleado;
                    break;
                case 2:
                    NumCompleto = "000"+cadenaNumEmpleado;
                    break;
                case 3:
                    NumCompleto = "00"+cadenaNumEmpleado;
                    break;
                case 4:
                    NumCompleto = "0"+cadenaNumEmpleado;
                    break;
                case 5:
                    NumCompleto = cadenaNumEmpleado;
                    break;
                default:
                    NumCompleto = cadenaNumEmpleado;
                    break;
            }
            return NumCompleto;
        }


        public static Tbl_InventarioDetalle DisponibilidadFolioInventarioDetalle(int IdBanco, int verificarFolio)
        {
            var transaccion = new Transaccion();
            var repositorioTblBanco = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            int idInventario = Convert.ToInt32(repositorioTblBanco.Obtener(x => x.Id == IdBanco && x.Activo == true).IdInventario);

            var repoInventarioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);

            Tbl_InventarioDetalle folioEncontrado = repoInventarioDetalle.Obtener(x => x.IdInventario == idInventario && x.NumFolio == verificarFolio);

            return folioEncontrado;
        }






    }
}
