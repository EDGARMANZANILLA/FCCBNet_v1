using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Datos;
using DAP.Foliacion.Datos.ClasesParaDBF;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPD;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPDC;

namespace DAP.Foliacion.Negocios
{
    public class CrearReferencia_CanceladosNegocios
    {

        public static List<CrearReferenciaDTO> ObtenerReferenciasAnioActual(int anioActual)
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);

            int anioAnterior = anioActual - 1;
            List<Tbl_Referencias_Cancelaciones> registrosReferenciasObtenidos = new List<Tbl_Referencias_Cancelaciones>();
            registrosReferenciasObtenidos = repositorio.ObtenerPorFiltro(x => x.Anio >= anioAnterior && x.Activo == true).OrderBy(x => x.Numero_Referencia).ToList();

            List<CrearReferenciaDTO> referenciasEncontradas = new List<CrearReferenciaDTO>();

            if (registrosReferenciasObtenidos.Count > 0)
            {
                int iterador = 0;
                foreach (Tbl_Referencias_Cancelaciones nuevaReferencia in registrosReferenciasObtenidos)
                {

                    CrearReferenciaDTO nuevaReferenciaDeCancelado = new CrearReferenciaDTO();

                    nuevaReferenciaDeCancelado.Id = nuevaReferencia.Id;
                    nuevaReferenciaDeCancelado.Id_Iterador = ++iterador;

                    nuevaReferenciaDeCancelado.Anio = nuevaReferencia.Anio;
                    nuevaReferenciaDeCancelado.NReferencia = Convert.ToString( nuevaReferencia.NReferencia );
                    nuevaReferenciaDeCancelado.Numero_Referencia = nuevaReferencia.Numero_Referencia;
                    nuevaReferenciaDeCancelado.Fecha_Creacion = nuevaReferencia.Fecha_Creacion.ToString("MM/dd/yyyy");
                    nuevaReferenciaDeCancelado.Creado_Por = nuevaReferencia.Creado_Por;
                    nuevaReferenciaDeCancelado.FormasPagoCargadas = nuevaReferencia.FormasPagoDentroReferencia;
                    nuevaReferenciaDeCancelado.EsCancelado = nuevaReferencia.EsCancelado;
                    nuevaReferenciaDeCancelado.Activo = nuevaReferencia.Activo;

                    referenciasEncontradas.Add(nuevaReferenciaDeCancelado);
                }
            }

            return referenciasEncontradas;
        }


        public static int CrearNuevaReferenciaCancelados(string nuevoNumero)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
            int anioActual = DateTime.Now.Year;



            Tbl_Referencias_Cancelaciones existeReferenciaEnAnio = repositorio.Obtener(x => x.Anio == DateTime.Now.Year && x.Activo == true);
            int idAgregado = 0;
            Tbl_Referencias_Cancelaciones nuevaReferencia = new Tbl_Referencias_Cancelaciones();
            if (existeReferenciaEnAnio == null)
            {
                nuevaReferencia.NReferencia = 1;
                nuevaReferencia.Anio = DateTime.Now.Year;
                nuevaReferencia.Numero_Referencia = nuevoNumero;
                nuevaReferencia.Fecha_Creacion = DateTime.Now.Date;
                nuevaReferencia.FormasPagoDentroReferencia = 0;
                nuevaReferencia.Creado_Por = "**********";
                nuevaReferencia.EsCancelado = false;
                nuevaReferencia.Activo = true;
                idAgregado = repositorio.Agregar(nuevaReferencia).Id;

            }
            else 
            {
                int nReferencia = repositorio.ObtenerPorFiltro(x => x.Anio == DateTime.Now.Year && x.Activo == true).OrderByDescending(x => x.NReferencia).Select(x => x.NReferencia).FirstOrDefault();

                nuevaReferencia.NReferencia = nReferencia +1;


                Tbl_Referencias_Cancelaciones nuevoNumeroRefenciaCanceladoExiste2 = repositorio.Obtener(x => x.Anio == DateTime.Now.Year && x.Numero_Referencia == nuevoNumero && x.Activo == true);
             
                if (nuevoNumeroRefenciaCanceladoExiste2 == null)
                {
                    nuevaReferencia.Anio = DateTime.Now.Year;
                    nuevaReferencia.Numero_Referencia = nuevoNumero;
                    nuevaReferencia.Fecha_Creacion = DateTime.Now.Date;
                    nuevaReferencia.FormasPagoDentroReferencia = 0;
                    nuevaReferencia.Creado_Por = "**********";
                    nuevaReferencia.EsCancelado = false;
                    nuevaReferencia.Activo = true;
                    idAgregado = repositorio.Agregar(nuevaReferencia).Id;

                }
            }








            //Tbl_Referencias_Cancelaciones nuevoNumeroRefenciaCanceladoExiste = repositorio.Obtener(x => x.Anio == DateTime.Now.Year && x.Numero_Referencia == nuevoNumero && x.Activo == true);
          
            //if (nuevoNumeroRefenciaCanceladoExiste == null)
            //{
            //    Tbl_Referencias_Cancelaciones nuevaReferencia = new Tbl_Referencias_Cancelaciones();
            //    nuevaReferencia.Anio = DateTime.Now.Year;
            //    nuevaReferencia.Numero_Referencia = "CC" + nuevoNumero;
            //    nuevaReferencia.Fecha_Creacion = DateTime.Now.Date;
            //    nuevaReferencia.FormasPagoDentroReferencia = 0;
            //    nuevaReferencia.Creado_Por = "**********";
            //    nuevaReferencia.EsCancelado = false;
            //    nuevaReferencia.Activo = true;
            //    idAgregado = repositorio.Agregar(nuevaReferencia).Id;

            //}

            return idAgregado;
        }


        public static int InactivarReferenciaCancelados(int InactivarIdReferenciaCancelados)
        {
            int cantidadRegistrosModificados = 0;
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
            var repositorioTbl_Pagos = new Repositorio<Tbl_Pagos>(transaccion);

            Tbl_Referencias_Cancelaciones registroReferenciaEncontrado = null;

            List<Tbl_Pagos> registrosChequesObtenidos = repositorioTbl_Pagos.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == InactivarIdReferenciaCancelados && x.Activo == true).ToList();
            int numeroVerificacionCancelado = 0;
            if (registrosChequesObtenidos.Count != 0)
            {
                foreach (Tbl_Pagos nuevoChequeObtenido in registrosChequesObtenidos)
                {
                    //deberia devolver 8 si fue removido un cheque de la referencia de cancelacion
                    numeroVerificacionCancelado = BuscadorChequeNegocios.RevocarCheque_ReferenciaCancelado(nuevoChequeObtenido.Id);

                    if (numeroVerificacionCancelado == 8)
                    {
                        cantidadRegistrosModificados += 1;
                    }
                }

                if (registrosChequesObtenidos.Count == cantidadRegistrosModificados)
                {
                    registroReferenciaEncontrado = repositorio.Obtener(x => x.Id == InactivarIdReferenciaCancelados && x.Activo == true);
                    registroReferenciaEncontrado.Activo = false;
                    repositorio.Modificar(registroReferenciaEncontrado);

                }

            }
            else if (registrosChequesObtenidos.Count == 0)
            {
                registroReferenciaEncontrado = repositorio.Obtener(x => x.Id == InactivarIdReferenciaCancelados && x.Activo == true);
                registroReferenciaEncontrado.Activo = false;
                repositorio.Modificar(registroReferenciaEncontrado);

            }

            return cantidadRegistrosModificados;
        }





        /***********************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************/
        /***********************************        Metodos para Ver detalles o anular cancelacion desde modal       ***********************************************************/
        public static List<DetallesRegistrosDentroReferenciaDTO> ObtenerDetallesDentroIdReferencia(int Id)
        {
            var transaccion = new Transaccion();
            var repositorioTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
            List<Tbl_Pagos> registrosEncontrados = repositorioTblPagos.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == Id && x.Activo == true).ToList();

            List<DetallesRegistrosDentroReferenciaDTO> detallesEncontrados = new List<DetallesRegistrosDentroReferenciaDTO>();
            int iterador = 0;
            foreach (Tbl_Pagos nuevoRegistro in registrosEncontrados)
            {
                DetallesRegistrosDentroReferenciaDTO nuevoDetalle = new DetallesRegistrosDentroReferenciaDTO();
                nuevoDetalle.IdVirtual = iterador += 1;
                nuevoDetalle.Id = nuevoRegistro.Id;
                nuevoDetalle.EsPenA = nuevoRegistro.EsPenA.ToString();
                nuevoDetalle.Deleg = nuevoRegistro.Delegacion;
                nuevoDetalle.Num = nuevoRegistro.NumEmpleado.ToString();
                nuevoDetalle.Beneficiario = nuevoRegistro.EsPenA == true ? nuevoRegistro.BeneficiarioPenA : nuevoRegistro.NombreEmpleado;
                nuevoDetalle.FolioCheque = nuevoRegistro.FolioCheque;
                nuevoDetalle.Liquido = nuevoRegistro.ImporteLiquido;
                nuevoDetalle.Id_Nom = nuevoRegistro.Id_nom;
                nuevoDetalle.Nomina = nuevoRegistro.Nomina;
                nuevoDetalle.Quincena = nuevoRegistro.Quincena;
                nuevoDetalle.Referencia = nuevoRegistro.ReferenciaBitacora;
                nuevoDetalle.CuentaPagadora = nuevoRegistro.Tbl_CuentasBancarias.Cuenta;
                nuevoDetalle.BancoPagador = nuevoRegistro.Tbl_CuentasBancarias.NombreBanco;
                detallesEncontrados.Add(nuevoDetalle);
            }
            return detallesEncontrados;
        }


        public static bool AnularCancelacion(int IdPago)
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Pagos>(transaccion);
            var repoCancelacion = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
            var repoSeguimiento = new Repositorio<Tbl_SeguimientoHistoricoFormas_Pagos>(transaccion);


            Tbl_Pagos pagoEncontrado = repositorio.Obtener(x => x.Id == IdPago && x.IdCat_EstadoPago_Pagos != 5 && x.Activo == true);

            bool bandera = false;
            if (pagoEncontrado != null)
            {
                Tbl_Referencias_Cancelaciones referenciaEncontrada = repoCancelacion.Obtener(x => x.Id == pagoEncontrado.IdTbl_Referencias_Cancelaciones);
                referenciaEncontrada.FormasPagoDentroReferencia -= 1;
                referenciaEncontrada.FechaActualizacion_AgregoOQuitoFormas = DateTime.Now;
                repoCancelacion.Modificar_Transaccionadamente(referenciaEncontrada);

                Tbl_SeguimientoHistoricoFormas_Pagos nuevoSeguimiento = new Tbl_SeguimientoHistoricoFormas_Pagos();
                nuevoSeguimiento.IdTbl_Pagos = pagoEncontrado.Id;
                nuevoSeguimiento.FechaCambio = DateTime.Now;
                nuevoSeguimiento.ChequeAnterior = pagoEncontrado.FolioCheque;
                nuevoSeguimiento.MotivoRefoliacion = "Se removio de la referencia " + referenciaEncontrada.Numero_Referencia;
                nuevoSeguimiento.EsCancelado = false;
                nuevoSeguimiento.ReferenciaCancelado = null;
                nuevoSeguimiento.IdCat_EstadoCancelado = null;
                nuevoSeguimiento.Activo = true;

                Tbl_SeguimientoHistoricoFormas_Pagos seguimientoGuardado = repoSeguimiento.Agregar_Transaccionadamente(nuevoSeguimiento);
                if (seguimientoGuardado.IdTbl_Pagos > 1)
                {
                    pagoEncontrado.IdTbl_Referencias_Cancelaciones = null;
                    repositorio.Modificar_Transaccionadamente(pagoEncontrado);

                    transaccion.GuardarCambios();
                    bandera = true;
                }
            }

            return bandera;
        }



        /***********************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************/
        /***********************************        Metodos para desactivar una referencia de cancelacion            ***********************************************************/
        public static int ObtenerTotalChequesEnReferenciaCancelacion(int Id)
        {
            var transaccion = new Transaccion();
            var repositorioTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
            return repositorioTblPagos.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == Id && x.Activo).Count();
        }




        /***********************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************/
        /***********************************        Metodos para Finalizar una referencia de cancelacion            ***********************************************************/
        public static int FinalizarIdReferenciaCancelacion(int IdReferenciaCancelado, string FolioDocumento, byte[] ArchivoBLOB)
        {
            var transaccion = new Transaccion();
            var repoReferenciaCancelaciones = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
            var repositorioTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
            List<Tbl_Pagos> registroEncontradosCheques = repositorioTblPagos.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == IdReferenciaCancelado && x.Activo).ToList();

            int cambioEstatusCheque = 0;
            foreach (Tbl_Pagos cambiarEstadoPago in registroEncontradosCheques)
            {
                cambiarEstadoPago.IdCat_EstadoPago_Pagos = 5;
                Tbl_Pagos estatusCambiado = repositorioTblPagos.Modificar_Transaccionadamente(cambiarEstadoPago);

                if (estatusCambiado.Id > 0)
                {
                    cambioEstatusCheque += 1;
                }
            }


            Tbl_Referencias_Cancelaciones finalizarReferencia = repoReferenciaCancelaciones.Obtener(x => x.Id == IdReferenciaCancelado && x.Activo == true);

            if (cambioEstatusCheque == finalizarReferencia.FormasPagoDentroReferencia)
            {
                finalizarReferencia.EsCancelado = true;
                finalizarReferencia.FolioDocumento = FolioDocumento;
                finalizarReferencia.ArchivoSustento = ArchivoBLOB;
                Tbl_Referencias_Cancelaciones referenciaGuardada = repoReferenciaCancelaciones.Modificar(finalizarReferencia);

                if (referenciaGuardada.Id > 0)
                {
                    transaccion.GuardarCambios();
                }
            }

            return cambioEstatusCheque;
        }






        /**********************************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************************/
        /***********************************        Metodo para el previsualizar el documento oficial que avala el termino de cancelacion de una referencia          ***********************************************************/
        public static byte[] ObtenerBytesDocumentoXIdReferencia(int IdReferenciaCancelado)
        {
            var transaccion = new Transaccion();
            var repoReferenciaCancelaciones = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);

            return repoReferenciaCancelaciones.Obtener(x => x.Id == IdReferenciaCancelado && x.Activo).ArchivoSustento;

        }





        /******************************************************************************************************************************************************************/
        /*****************************************      GENERA EL IPD Y LO GUARDA EN SQL PARA SU FUTURA CONSULTA        ***************************************************/
        /******************************************************************************************************************************************************************/
        /***    VALIDA SI ESTAN ACTUALIZADOS LOS DATOS DEL IPD    ***/
        public static bool EstaActualizadoIpdGuardado(int IdReferecniaCancelacion) 
        {
            bool estaActualizoIPD = false;

            var transaccion = new Transaccion();
            var repoTblCancelaciones = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
           // var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
            Tbl_Referencias_Cancelaciones referenciacancelacion = repoTblCancelaciones.Obtener(x => x.Id == IdReferecniaCancelacion && x.Activo == true);

            //int numeroPagosEncontrados = repoTblPagos.ObtenerPorFiltro(x =>  x.IdTbl_Referencias_Cancelaciones == IdReferecniaCancelacion && x.Nomina != "08" && x.Activo == true).Count();


            if (referenciacancelacion.FechaCreacionIPD != null) 
            {
                if ( referenciacancelacion.FechaCreacionIPD >= referenciacancelacion.FechaActualizacion_AgregoOQuitoFormas  /*&& referenciacancelacion.FormasPagoDentroReferencia == referenciacancelacion.TotalFormasIPD*/) 
                {
                    estaActualizoIPD = true;
                }
            }
            return estaActualizoIPD;
        }

        /***    LIMPIA LOS DATOS DEL IPD  DE UNA REFERENCIA   ***/
        public static bool BorrarIPDGuardadoDeIdReferencia(int IdReferecniaCancelacion) 
        {
            bool seBorroIpdCorrectamente = false;
            var transaccion = new Transaccion();
            var repoIPD = new Repositorio<Cancelaciones_InterfazPercepcionesDeducciones_IPD>(transaccion);
            List<Cancelaciones_InterfazPercepcionesDeducciones_IPD> filasEncontradasEnReferencia = repoIPD.ObtenerPorFiltro(x => x.IdReferenciaCancelados == IdReferecniaCancelacion).ToList();
            int filasRemovidas = repoIPD.Remover_EntidadesMasivamente(filasEncontradasEnReferencia);

            if (filasEncontradasEnReferencia.Count == filasRemovidas) 
            {
                seBorroIpdCorrectamente = true;
            }
            return seBorroIpdCorrectamente;
        }

        /***    GENERA EL IPD DE UNA REFERENCIA EN UNA TABLA DE SQL  ***/
        public static bool GeneraYGuardaInterfaceIPDxAnio( int anioSeleccionado, int IdReferecniaCancelacion)
        {
            bool bandera = false;
          //  List<IPDDTO> PagosACargar = new List<IPDDTO>();
            List<Cancelaciones_InterfazPercepcionesDeducciones_IPD> PagosACargar = new List<Cancelaciones_InterfazPercepcionesDeducciones_IPD>();
            int cantidadDeDatosAGuardar = 0;


            /** estructura para obtener una lista con la que se rellena elipd.dbf **/
            var transaccion = new Transaccion();
            var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
            var repoTblCancelaciones = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
            Tbl_Referencias_Cancelaciones referenciacancelacion = repoTblCancelaciones.Obtener(x => x.Id == IdReferecniaCancelacion && x.Activo == true);

            List<Tbl_Pagos> pagosEncontrados = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioSeleccionado && x.IdTbl_Referencias_Cancelaciones == IdReferecniaCancelacion && x.Nomina != "08" && x.Activo == true).ToList();


                foreach (Tbl_Pagos pago in pagosEncontrados)
                {
                    string visitaAnioInterfas = Negocios.FoliarNegocios.ObtenerCadenaAnioInterface(pago.Anio);
                    DatosGeneralesIPD_DTO datosGeneralesEncontrados = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosGeneralesxIdNom_IPD(pago.Id_nom, visitaAnioInterfas);


                    if (datosGeneralesEncontrados != null)
                    {
                        string num5Digitos = Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(pago.NumEmpleado);
                        DatosAnIPD_DTO datosAn = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosAN_IPD(visitaAnioInterfas, datosGeneralesEncontrados.AN, num5Digitos);

                        if (string.IsNullOrEmpty(datosAn.Cla_Pto) && pago.Nomina.Equals("03"))
                        {
                            datosAn.Cla_Pto = "AY001";
                        }

                        if (datosAn != null)
                        {
                            //Obtener Percepciones y Deducciones
                            List<DatosApercepcionesIPD_DTO> percepciones = CrearReferencia_CanceladosDbSinEntity.ObtenerPercepciones_IPD(visitaAnioInterfas, datosGeneralesEncontrados.AP, num5Digitos);
                            bool contieneCampoBenef_AD = CrearReferencia_CanceladosDbSinEntity.ContieneCampoBenefAD_IPD(visitaAnioInterfas, datosGeneralesEncontrados.AD, num5Digitos);
                            List<DatosAdeducionesIPD_DTO> deducciones = CrearReferencia_CanceladosDbSinEntity.ObtenerDeducciones_IPD(visitaAnioInterfas , datosGeneralesEncontrados.AD, num5Digitos, contieneCampoBenef_AD);


                            foreach (DatosApercepcionesIPD_DTO nuevaPercepcion in percepciones)
                            {
                                cantidadDeDatosAGuardar += 1;
                                // IPDDTO nuevopPago = new IPDDTO();
                                Cancelaciones_InterfazPercepcionesDeducciones_IPD nuevopPago = new Cancelaciones_InterfazPercepcionesDeducciones_IPD();
                                nuevopPago.IdReferenciaCancelados = referenciacancelacion.Id;
                                nuevopPago.AnioEnReferencia = anioSeleccionado;
                                nuevopPago.IdBancoPagadorFCCBNetDb = pago.IdTbl_CuentaBancaria_BancoPagador;
                                nuevopPago.Referencia = referenciacancelacion.Numero_Referencia;
                                nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                                nuevopPago.Cve_presup = datosAn.Cve_Presup;
                                nuevopPago.Cvegto = nuevaPercepcion.cvegasto;
                                nuevopPago.Cvepd = nuevaPercepcion.Cla_perc;

                                if (nuevaPercepcion.Monto > 0)
                                {
                                    nuevopPago.MontoPositivo = nuevaPercepcion.Monto;
                                }
                                else 
                                {
                                    nuevopPago.MontoNegativo = nuevaPercepcion.Monto;
                                }
                                nuevopPago.Tipoclave = "PP";
                                nuevopPago.Adicional = pago.Adicional;
                                nuevopPago.Partida = pago.Partida;
                                nuevopPago.Num = num5Digitos;
                                nuevopPago.Nombre = pago.NombreEmpleado;
                                nuevopPago.Num_che = Convert.ToString(pago.FolioCheque);
                                nuevopPago.Foliocdfi = pago.FolioCFDI != null ? Convert.ToInt32(pago.FolioCFDI) : 0;
                                nuevopPago.Deleg = pago.Delegacion;
                                nuevopPago.Idctabanca = pago.Tbl_CuentasBancarias.IdctabancaIPD != null ? Convert.ToInt32(pago.Tbl_CuentasBancarias.IdctabancaIPD) : 0;
                                nuevopPago.IdBanco = pago.Tbl_CuentasBancarias.IdBancoIPD != null ? Convert.ToInt32(pago.Tbl_CuentasBancarias.IdBancoIPD) : 0;
                                nuevopPago.Pagomat = ".F.";
                                nuevopPago.Tipo_pagom = "";
                                nuevopPago.Numtarjeta = "";
                                nuevopPago.Orden = 1;
                                nuevopPago.Quincena = Convert.ToString(pago.Quincena);
                                nuevopPago.Nomalpha = datosGeneralesEncontrados.NominaAlpha;
                                nuevopPago.Fecha = "";
                                nuevopPago.Cvegasto = nuevaPercepcion.cvegasto;
                                nuevopPago.Cla_pto = datosAn.Cla_Pto;

                                PagosACargar.Add(nuevopPago);
                            }

                            foreach (DatosAdeducionesIPD_DTO nuevaDeduccion in deducciones)
                            {
                                cantidadDeDatosAGuardar += 1;
                            //IPDDTO nuevopPago = new IPDDTO();
                            Cancelaciones_InterfazPercepcionesDeducciones_IPD nuevopPago = new Cancelaciones_InterfazPercepcionesDeducciones_IPD();
                            nuevopPago.IdReferenciaCancelados = referenciacancelacion.Id;
                            nuevopPago.AnioEnReferencia = anioSeleccionado;
                            nuevopPago.IdBancoPagadorFCCBNetDb = pago.IdTbl_CuentaBancaria_BancoPagador;
                            nuevopPago.Referencia = referenciacancelacion.Numero_Referencia;
                                nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                                nuevopPago.Cve_presup = datosAn.Cve_Presup;
                                nuevopPago.Cvegto = nuevaDeduccion.Cvegasto;
                                nuevopPago.Cvepd = nuevaDeduccion.Cla_dedu;

                                if (nuevaDeduccion.Monto > 0)
                                {
                                    nuevopPago.MontoPositivo = nuevaDeduccion.Monto;
                                }
                                else
                                {
                                    nuevopPago.MontoNegativo = nuevaDeduccion.Monto;
                                }
                                nuevopPago.Tipoclave = "DD";
                                nuevopPago.Adicional = pago.Adicional;
                                nuevopPago.Partida = pago.Partida;
                                nuevopPago.Num = num5Digitos;
                                nuevopPago.Nombre = pago.NombreEmpleado;
                                nuevopPago.Num_che = Convert.ToString(pago.FolioCheque);
                                nuevopPago.Foliocdfi = pago.FolioCFDI != null ? Convert.ToInt32(pago.FolioCFDI) : 0;
                                nuevopPago.Deleg = pago.Delegacion;
                                nuevopPago.Idctabanca = pago.Tbl_CuentasBancarias.IdctabancaIPD != null ? Convert.ToInt32(pago.Tbl_CuentasBancarias.IdctabancaIPD) : 0;
                                nuevopPago.IdBanco = pago.Tbl_CuentasBancarias.IdBancoIPD != null ? Convert.ToInt32(pago.Tbl_CuentasBancarias.IdBancoIPD) : 0;
                                nuevopPago.Pagomat = ".F.";
                                nuevopPago.Tipo_pagom = "";
                                nuevopPago.Numtarjeta = "";
                                nuevopPago.Orden = 2;
                                nuevopPago.Quincena = Convert.ToString(pago.Quincena);
                                nuevopPago.Nomalpha = datosGeneralesEncontrados.NominaAlpha;
                                nuevopPago.Fecha = "";
                                nuevopPago.Cvegasto = nuevaDeduccion.Cvegasto;
                                nuevopPago.Cla_pto = datosAn.Cla_Pto;

                                PagosACargar.Add(nuevopPago);
                            }


                            //COUTA PATRONEL DEL ISSTE 
                            //LAS COUTAS PATRONALES NO INCLUYEN AL ISSTE YA QUE NO SE PUEDEN RECUPERAR ESOS IMPORTES PAGADOS A ELLOS ASI QUE PARA QUE NO ENTRE  SE IGUALA A "0" PARA NO AFECTAR EN UN FUTURO EL USO DE EL METODO QUE CREA EL IPD
                            datosAn.Patronal_ISSTE = 0;
                            if (datosAn.Patronal_ISSTE > 0)
                            {
                            //    cantidadDeDatosAGuardar += 1;
                            ////IPDDTO nuevopPago = new IPDDTO();
                            //Cancelaciones_InterfazPercepcionesDeducciones_IPD nuevopPago = new Cancelaciones_InterfazPercepcionesDeducciones_IPD();
                            //nuevopPago.IdReferenciaCancelados = referenciacancelacion.Id;
                            //nuevopPago.AnioEnReferencia = anioSeleccionado;
                            //nuevopPago.Referencia = referenciacancelacion.Numero_Referencia;
                            //    nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                            //    nuevopPago.Cve_presup = datosAn.Cve_Presup;
                            //    nuevopPago.Cvegto = "1411";
                            //    nuevopPago.Cvepd = "";
                            //    nuevopPago.Monto = datosAn.Patronal_ISSTE;
                            //    nuevopPago.Tipoclave = "CP";
                            //    nuevopPago.Adicional = pago.Adicional;
                            //    nuevopPago.Partida = pago.Partida;
                            //    nuevopPago.Num = num5Digitos;
                            //    nuevopPago.Nombre = pago.NombreEmpleado;
                            //    nuevopPago.Num_che = Convert.ToString(pago.FolioCheque);
                            //    nuevopPago.Foliocdfi = pago.FolioCFDI != null ? Convert.ToInt32(pago.FolioCFDI) : 0;
                            //    nuevopPago.Deleg = pago.Delegacion;
                            //    nuevopPago.Idctabanca = pago.Tbl_CuentasBancarias.IdctabancaIPD != null ? Convert.ToInt32(pago.Tbl_CuentasBancarias.IdctabancaIPD) : 0;
                            //    nuevopPago.IdBanco = pago.Tbl_CuentasBancarias.IdBancoIPD != null ? Convert.ToInt32(pago.Tbl_CuentasBancarias.IdBancoIPD) : 0;
                            //    nuevopPago.Pagomat = ".F.";
                            //    nuevopPago.Tipo_pagom = "";
                            //    nuevopPago.Numtarjeta = "";
                            //    nuevopPago.Orden = 3;
                            //    nuevopPago.Quincena = Convert.ToString(pago.Quincena);
                            //    nuevopPago.Nomalpha = datosGeneralesEncontrados.NominaAlpha;
                            //    nuevopPago.Fecha = "";
                            //    nuevopPago.Cvegasto = "1411";
                            //    nuevopPago.Cla_pto = datosAn.Cla_Pto;

                            //    PagosACargar.Add(nuevopPago);
                            }


                            //Couta patronal del ISSSTECAM
                            if (datosAn.Patronal_ISSSTECAM > 0)
                            {
                                cantidadDeDatosAGuardar += 1;
                            //IPDDTO nuevopPago = new IPDDTO();
                            Cancelaciones_InterfazPercepcionesDeducciones_IPD nuevopPago = new Cancelaciones_InterfazPercepcionesDeducciones_IPD();
                            nuevopPago.IdReferenciaCancelados = referenciacancelacion.Id;
                            nuevopPago.AnioEnReferencia = anioSeleccionado;
                            nuevopPago.IdBancoPagadorFCCBNetDb = pago.IdTbl_CuentaBancaria_BancoPagador;
                            nuevopPago.Referencia = referenciacancelacion.Numero_Referencia;
                                nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                                nuevopPago.Cve_presup = datosAn.Cve_Presup;
                                nuevopPago.Cvegto = "1413";
                                nuevopPago.Cvepd = "";

                                if (datosAn.Patronal_ISSSTECAM > 0)
                                {
                                    nuevopPago.MontoPositivo = datosAn.Patronal_ISSSTECAM;
                                }
                                else
                                {
                                    nuevopPago.MontoNegativo = datosAn.Patronal_ISSSTECAM;
                                }
                                nuevopPago.Tipoclave = "CP";
                                nuevopPago.Adicional = pago.Adicional;
                                nuevopPago.Partida = pago.Partida;
                                nuevopPago.Num = num5Digitos;
                                nuevopPago.Nombre = pago.NombreEmpleado;
                                nuevopPago.Num_che = Convert.ToString(pago.FolioCheque);
                                nuevopPago.Foliocdfi = pago.FolioCFDI != null ? Convert.ToInt32(pago.FolioCFDI) : 0;
                                nuevopPago.Deleg = pago.Delegacion;
                                nuevopPago.Idctabanca = pago.Tbl_CuentasBancarias.IdctabancaIPD != null ? Convert.ToInt32(pago.Tbl_CuentasBancarias.IdctabancaIPD) : 0;
                                nuevopPago.IdBanco = pago.Tbl_CuentasBancarias.IdBancoIPD != null ? Convert.ToInt32(pago.Tbl_CuentasBancarias.IdBancoIPD) : 0;
                                nuevopPago.Pagomat = ".F.";
                                nuevopPago.Tipo_pagom = "";
                                nuevopPago.Numtarjeta = "";
                                nuevopPago.Orden = 3;
                                nuevopPago.Quincena = Convert.ToString(pago.Quincena);
                                nuevopPago.Nomalpha = datosGeneralesEncontrados.NominaAlpha;
                                nuevopPago.Fecha = "";
                                nuevopPago.Cvegasto = "1413";
                                nuevopPago.Cla_pto = datosAn.Cla_Pto;

                                PagosACargar.Add(nuevopPago);
                            }

                        }

                    }

                }

            /** FIn de estructura **/

            var repoCancelacionesIPD = new Repositorio<Cancelaciones_InterfazPercepcionesDeducciones_IPD>(transaccion);
            int filasAgregadas = repoCancelacionesIPD.Agregar_EntidadesMasivamente(PagosACargar);
            
            if (filasAgregadas == cantidadDeDatosAGuardar)
            {
                bandera = true;
                referenciacancelacion.FechaCreacionIPD = DateTime.Now;
                referenciacancelacion.TotalFormasIPD = pagosEncontrados.Count;
                repoTblCancelaciones.Modificar(referenciacancelacion);
            }

            return bandera;
        }

        /***    LLENA UNA DBF CON LOS DATOS DE UNA REFERENCIA EN .DBF PARA DESCARGARSELA AL CLIENTE  ***/
        public static bool GeneralNuevoIPDxAnio_2023(string pathOrigen, string pathDestino, string nombreArchivoDBFAnual, int IdReferenciaCancelados)
        {
            bool bandera = false;
            List<IPDDTO> PagosACargar = new List<IPDDTO>();

            if (File.Exists(pathOrigen))
            {
                if (File.Exists(pathDestino))
                {
                    File.Delete(pathDestino);
                }

                File.Copy(pathOrigen, pathDestino);
            }

            if (File.Exists(pathDestino))
            {
                /***        OBTIENE UNA LISTA DE IPD LISTRADOA PARA DESPUES EL LLENADO DEL IPD.DBF Y PODER DESCARCARCELO AL CLIENTE        ***/
                var transaccion = new Transaccion();
                var repoIpd = new Repositorio<Cancelaciones_InterfazPercepcionesDeducciones_IPD>(transaccion);

                List<Cancelaciones_InterfazPercepcionesDeducciones_IPD> resumenIpd = repoIpd.ObtenerPorFiltro(x => x.IdReferenciaCancelados == IdReferenciaCancelados).ToList();

                foreach (Cancelaciones_InterfazPercepcionesDeducciones_IPD nuevoRegitroIpd in resumenIpd)
                {
                    IPDDTO nuevoIpdDTO = new IPDDTO();
                    nuevoIpdDTO.TipoNom = nuevoRegitroIpd.TipoNom;
                    nuevoIpdDTO.Cve_presup = nuevoRegitroIpd.Cve_presup;
                    nuevoIpdDTO.Cvegto = nuevoRegitroIpd.Cvegasto;
                    nuevoIpdDTO.Cvepd = nuevoRegitroIpd.Cvepd;
                    nuevoIpdDTO.Monto = nuevoRegitroIpd.MontoPositivo == null ? Convert.ToDecimal(nuevoRegitroIpd.MontoNegativo) : Convert.ToDecimal(nuevoRegitroIpd.MontoPositivo);
                    nuevoIpdDTO.Tipoclave = nuevoRegitroIpd.Tipoclave;
                    nuevoIpdDTO.Adicional = nuevoRegitroIpd.Adicional;
                    nuevoIpdDTO.Partida = nuevoRegitroIpd.Partida;
                    nuevoIpdDTO.Num = nuevoRegitroIpd.Num;
                    nuevoIpdDTO.Nombre = nuevoRegitroIpd.Nombre;
                    nuevoIpdDTO.Num_che = nuevoRegitroIpd.Num_che;
                    nuevoIpdDTO.Foliocdfi = Convert.ToInt32(nuevoRegitroIpd.Foliocdfi);
                    nuevoIpdDTO.Deleg = nuevoRegitroIpd.Deleg;
                    nuevoIpdDTO.Idctabanca = Convert.ToInt32(nuevoRegitroIpd.Idctabanca);
                    nuevoIpdDTO.IdBanco = Convert.ToInt32(nuevoRegitroIpd.IdBanco);
                    nuevoIpdDTO.Pagomat = nuevoRegitroIpd.Pagomat;
                    nuevoIpdDTO.Tipo_pagom = nuevoRegitroIpd.Tipo_pagom;
                    nuevoIpdDTO.Numtarjeta = nuevoRegitroIpd.Numtarjeta;
                    nuevoIpdDTO.Orden = Convert.ToInt32(nuevoRegitroIpd.Orden);
                    nuevoIpdDTO.Quincena = nuevoRegitroIpd.Quincena;
                    nuevoIpdDTO.Nomalpha = nuevoRegitroIpd.Nomalpha;
                    nuevoIpdDTO.Fecha = nuevoRegitroIpd.Fecha;
                    nuevoIpdDTO.Cvegasto = nuevoRegitroIpd.Cvegasto;
                    nuevoIpdDTO.Cla_pto = nuevoRegitroIpd.Cvegasto;

                    PagosACargar.Add(nuevoIpdDTO);
                }

                /** FIn de estructura **/
            }


            string respuestaRellado = ActualizacionDFBS.LLenarIPD(pathDestino, nombreArchivoDBFAnual, PagosACargar);

            if (Convert.ToInt32(respuestaRellado) == PagosACargar.Count)
            {
                bandera = true;
            }

            return bandera;
        }



        /******************************************************************************************************************************************************************/
        /*****************************************      VALIDA , GENERA Y GUARDA EL IPD COMPENSADO EN SQL PARA SU FUTURA CONSULTA        ***************************************************/
        /******************************************************************************************************************************************************************/
        /***    VALIDA SI ESTAN ACTUALIZADOS LOS DATOS DEL IPD COMPENSADO   ***/
        public static bool EstaActualizadoIpdCompensadoGuardado(int IdReferecniaCancelacion)
        {
            bool estaActualizoIPDC = false;

            var transaccion = new Transaccion();
            var repoTblCancelaciones = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
            Tbl_Referencias_Cancelaciones referenciacancelacion = repoTblCancelaciones.Obtener(x => x.Id == IdReferecniaCancelacion && x.Activo == true);

            if (referenciacancelacion.FechaCreacionIPDC != null)
            {
                if (referenciacancelacion.FechaCreacionIPDC >= referenciacancelacion.FechaActualizacion_AgregoOQuitoFormas)
                {
                    estaActualizoIPDC = true;
                }
            }

            return estaActualizoIPDC;
        }

        /***    LIMPIA LOS DATOS DEL IPD COMPENSADO DE UNA REFERENCIA   ***/
        public static bool BorrarIPDCompensadoGuardadoDeIdReferencia(int IdReferecniaCancelacion)
        {
            bool seBorroIpdCompensadoCorrectamente = false;
            var transaccion = new Transaccion();
            var repoIPD = new Repositorio<Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC>(transaccion);
            List<Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC> filasEncontradasEnReferencia = repoIPD.ObtenerPorFiltro(x => x.IdReferenciaCancelados == IdReferecniaCancelacion).ToList();
            int filasRemovidas = repoIPD.Remover_EntidadesMasivamente(filasEncontradasEnReferencia);

            if (filasEncontradasEnReferencia.Count == filasRemovidas)
            {
                seBorroIpdCompensadoCorrectamente = true;
            }
            return seBorroIpdCompensadoCorrectamente;
        }

        /***    GENERA EL IPD COMPENSADO DE UNA REFERENCIA EN UNA TABLA DE SQL  ***/
        public static bool GeneraYGuardaInterfaceIPDCompensadoxAnio(int anioSeleccionado ,  int IdReferecniaCancelacion)
        {
            bool bandera = false;
            int cantidadDeDatosAGuardar = 0;
            List<Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC> registrosCompensados = new List<Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC>();
            
            var transaccion = new Transaccion();
            var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
            var repoTblCancelaciones = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
            Tbl_Referencias_Cancelaciones referenciacancelacion = repoTblCancelaciones.Obtener(x => x.Id == IdReferecniaCancelacion && x.Activo == true);
            List<Tbl_Pagos> pagosEncontrados = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioSeleccionado && x.IdTbl_Referencias_Cancelaciones == IdReferecniaCancelacion && x.Nomina != "08" && x.Activo == true /*&& x.FolioCheque == 152336*/).ToList();
                
                int lDF_6M = 0;
            foreach (Tbl_Pagos pago in pagosEncontrados)
            {
                /*** encuentra un cheque en particular ***/
                //if (pago.FolioCheque == 22528)
                //{
                //    bool detenerse = true;
                //}



                string visitaAnioInterfas = Negocios.FoliarNegocios.ObtenerCadenaAnioInterface(pago.Anio);
                DatosGeneralesIPD_DTO datosGeneralesEncontrados = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosGeneralesxIdNom_IPD(pago.Id_nom, visitaAnioInterfas);
                if (datosGeneralesEncontrados != null)
                {
                    string num5Digitos = Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(pago.NumEmpleado);
                    DatosAnIPD_DTO datosAn = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosAN_IPD(visitaAnioInterfas, datosGeneralesEncontrados.AN, num5Digitos);

                    /***     EN EL COMPENSADO NO SE USAN LAS COUTAS PATRONALES ASI QUE LAS VOLVIMOS "0" PARA NO TENER QUE RELEER TODO EL CODIGO ***/
                    datosAn.Patronal_ISSSTECAM = 0;
                    datosAn.Patronal_ISSTE = 0;

                    lDF_6M = CrearReferencia_CanceladosDbSinEntity.ObtenerLDF6DxPuestoTrabajo(datosAn.Cla_Pto);


                    if (datosAn != null)
                    {
                        /***    SE CONSULTAN EL A-P Y A-D PARA OBTENER LAS PERCEPCIONES Y DEDUCCIONES DEL PAGO    ***/
                        List<DatosApercepcionesIPD_DTO> percepciones = CrearReferencia_CanceladosDbSinEntity.ObtenerPercepciones_IPD(visitaAnioInterfas, datosGeneralesEncontrados.AP, num5Digitos);
                        bool contieneCampoBenef_AD = CrearReferencia_CanceladosDbSinEntity.ContieneCampoBenefAD_IPD(visitaAnioInterfas, datosGeneralesEncontrados.AD, num5Digitos);
                        List<DatosAdeducionesIPD_DTO> deducciones = CrearReferencia_CanceladosDbSinEntity.ObtenerDeducciones_IPD(visitaAnioInterfas, datosGeneralesEncontrados.AD, num5Digitos , contieneCampoBenef_AD);

                        /*****************************************************************************************************************************************************************************************/
                        /*******************************************        SE CONSIDERA PARA NO INCLUIR LAS FALTAS O RETARDOS DENTRO DEL COMPENSADO        ******************************************************/
                        /*****************************************************************************************************************************************************************************************/
                        bool contieneFaltasRetardos = false;
                        decimal DeduccionFaltasYRetardos = 0.0M;
                        if (deducciones.Select(x => x.Cla_dedu).Contains("38") || deducciones.Select(x => x.Cla_dedu).Contains("37"))
                        {
                            contieneFaltasRetardos = true;

                            List<DatosAdeducionesIPD_DTO> deduccionesPorFaltasRetardos = deducciones.Where(x => x.Cla_dedu.Trim().Equals("38") || x.Cla_dedu.Trim().Equals("37")).ToList();

                            foreach (DatosAdeducionesIPD_DTO nuevaFaltaRetardo in deduccionesPorFaltasRetardos)
                            {

                                DeduccionFaltasYRetardos += nuevaFaltaRetardo.Monto;
                                deducciones.Remove(nuevaFaltaRetardo);
                            }
                        }
                        /*****************************************************************************************************************************************************************************************/
                        /*****************************************************************************************************************************************************************************************/
                        /*****************************************************************************************************************************************************************************************/

                        /***    SIN EMBARGO AUNQUE NO SE TOMEN EN CUENTA EN EL COMPENSADO SI ES DESCONTADO DEL SUELDO DEL TRABAJADOR    ***/
                        decimal saldo = percepciones.FirstOrDefault().Monto - DeduccionFaltasYRetardos;
                        decimal montoDDACompensar = 0M;

                        decimal saldodedu = 0M;
                        int iteradorDD = 0;
                        int iteradorPP = 0;
                        int totalDeducciones = deducciones.Count();
                        int totalPercepciones = percepciones.Count();

                        
                        DatosApercepcionesIPD_DTO pp = percepciones.Skip(iteradorPP).FirstOrDefault();
                        DatosAdeducionesIPD_DTO dedu = deducciones.Skip(iteradorDD).FirstOrDefault();

                        montoDDACompensar = saldo;

                        
                        /*** EL PRIMER WHILE HABLA SOBRE LAS DEDUDCCIONES Y COMO SE LE VAN QUITANDO A LAS PERCEPCIONES HASTA QUE SE ACABEN LAS DEDUCCIONES  ***/
                        Tbl_Pagos pagoPena = null;
                        while (totalDeducciones > iteradorDD)
                        {
                            
                            if (dedu.Cla_dedu == "25")
                            {
                                /***     SE BUSCA EL ID DE LA PENSION SELECCIONADA EN LA DEDUCCIONES REALES SIN COMPENSAR PARA SABER SI EL MONTO ES EL REAL DE LA DEDUCCION CARGADA O YA INICIO EL PROCESO DE CANCELACION ***/
                                /***    SI YA SE INICIO EL PROCESO DE CANCELACION NO DEBERIA PODER BUSCAR UNA PENSION YA QUE AUN NO ACABA DE COMPENSAR LA QUE SE A SELECCIONADA ***/
                                /***    SOLO CUANDO EL MONTO DE LA DEDUCCION QUE SE COMPENSARA ES IGUAL A LA REAL SE PUEDE DECIR QUE AUN SE INICIARA A COMPENSAR Y DEBERIAMOS BUSCAR EL PAGO CORRESPONDIENTE A ESA PENSIO ***/
                                DatosAdeducionesIPD_DTO deduccionRealSinCompensar = deducciones.Where(x => x.Cla_dedu.Equals("25") && x.IdVirtualDeduccion == dedu.IdVirtualDeduccion).FirstOrDefault();
                                if (dedu.Monto == deduccionRealSinCompensar.MontoReal)
                                {
                                    /***    SI CONTIENE EL CAMPO BENEF EL BENEFICIARIO DE LA PENSIO SE FILTRA POR EL YA QUE PUDIERA SER QUE MAS DE 1 BENEFICIARIO TENGA EL MISMO LIQUIDO DE PENSION     ***/
                                    /***    SI CONTIENE EL CAMPO BENEF SE VALIDA QUE NO VENGA VACIO POR QUE PROBABLEMENTE FUE UNA ADICIONAL Y HASTA ANTES DEL LA QUINCENA 2306 ANGELINA NO LLENABA ESE CAMPO    ***/
                                    if (contieneCampoBenef_AD && !string.IsNullOrEmpty(dedu.BENEF))
                                    {
                                        pagoPena = repoTblPagos.Obtener(x => x.Quincena == pago.Quincena && x.NumEmpleado == pago.NumEmpleado && x.EsPenA == true && x.ImporteLiquido == dedu.Monto && x.NumBeneficiario == dedu.BENEF && x.Activo == true);
                                    }
                                    else
                                    {
                                        pagoPena = repoTblPagos.Obtener(x => x.Quincena == pago.Quincena && x.NumEmpleado == pago.NumEmpleado && x.EsPenA == true && x.ImporteLiquido == dedu.Monto && x.Activo == true);
                                    }

                                }

                            }

                            if (montoDDACompensar <= saldo)
                            {
                                Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC nuevoRegistro = new Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC();
                                nuevoRegistro.IdReferenciaCancelados = referenciacancelacion.Id;
                                nuevoRegistro.AnioEnReferencia = anioSeleccionado;
                                nuevoRegistro.TipoNom = datosGeneralesEncontrados.TipoNomina;
                                nuevoRegistro.Cve_presup = datosAn.Cve_Presup;
                                nuevoRegistro.CveGto = pp.cvegasto;
                                nuevoRegistro.Monto = dedu.Monto;
                                nuevoRegistro.TipoClave = "DD";
                                nuevoRegistro.Num_che = Convert.ToString(pago.FolioCheque);
                                nuevoRegistro.CveReal = dedu.Cla_dedu;
                                nuevoRegistro.CveCompen = pp.Cla_perc;
                                nuevoRegistro.fecha = "";
                                nuevoRegistro.Num = num5Digitos;
                                nuevoRegistro.NomAlpha = "";
                                nuevoRegistro.Quincena = Convert.ToString(pago.Quincena);
                                nuevoRegistro.Adicional = Convert.ToString(pago.Adicional);
                                nuevoRegistro.Cla_pto = datosAn.Cla_Pto;
                                nuevoRegistro.Ldf_6d = lDF_6M;


                                if (dedu.Cla_dedu == "25")
                                {
                                    if (pagoPena != null)
                                    {
                                        nuevoRegistro.IdctaBanca = Convert.ToInt32(pagoPena.Tbl_CuentasBancarias.IdctabancaIPD);
                                        nuevoRegistro.IdBanco = Convert.ToInt32(pagoPena.Tbl_CuentasBancarias.IdBancoIPD);
                                    }
                                    else
                                    {
                                        nuevoRegistro.IdctaBanca = 0;
                                        nuevoRegistro.IdBanco = 0;
                                    }
                                }
                                else
                                {
                                    nuevoRegistro.IdctaBanca = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdctabancaIPD);
                                    nuevoRegistro.IdBanco = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdBancoIPD);
                                }

                                /*** SE SUMA EL DATO A GUARDAR Y SE CARGA EN UNA LISTA PARA SU POSTERIOR GUARDADO   ***/
                                ++cantidadDeDatosAGuardar;
                                registrosCompensados.Add(nuevoRegistro);


                                /***   RESTA EL MONTO DE LA DEDUCCION AL SALDO DE LA PERCEPCION INICIAL   ***/ 
                                saldo -= dedu.Monto;

                                /***    ITERA A LA SIGUIENTE DEDUCCION Y SI AUN QUEDAN ALGUNA DEDUCTIVA PASA A SER PARTE DEL MONTO A COMPENSAR    ***/
                                iteradorDD += 1;
                                dedu = deducciones.Skip(iteradorDD).FirstOrDefault();

                                if (dedu != null)
                                {
                                    montoDDACompensar = dedu.Monto;
                                }
                            }
                            else
                            {
                                Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC nuevoRegistro = new Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC();
                                nuevoRegistro.IdReferenciaCancelados = referenciacancelacion.Id;
                                nuevoRegistro.AnioEnReferencia = anioSeleccionado;
                                nuevoRegistro.TipoNom = datosGeneralesEncontrados.TipoNomina;
                                nuevoRegistro.Cve_presup = datosAn.Cve_Presup;
                                nuevoRegistro.CveGto = pp.cvegasto;
                                nuevoRegistro.Monto = saldo;
                                nuevoRegistro.TipoClave = "DD";
                                nuevoRegistro.Num_che = Convert.ToString(pago.FolioCheque);
                                nuevoRegistro.CveReal = dedu.Cla_dedu;
                                nuevoRegistro.CveCompen = pp.Cla_perc;
                                nuevoRegistro.fecha = "";
                                nuevoRegistro.Num = num5Digitos;
                                nuevoRegistro.NomAlpha = "";
                                nuevoRegistro.Quincena = Convert.ToString(pago.Quincena);
                                nuevoRegistro.Adicional = Convert.ToString(pago.Adicional);
                                nuevoRegistro.Cla_pto = datosAn.Cla_Pto;
                                nuevoRegistro.Ldf_6d = lDF_6M;
                                
                                
                                if (dedu.Cla_dedu == "25")
                                {
                                    if (pagoPena != null)
                                    {
                                        nuevoRegistro.IdctaBanca = Convert.ToInt32(pagoPena.Tbl_CuentasBancarias.IdctabancaIPD);
                                        nuevoRegistro.IdBanco = Convert.ToInt32(pagoPena.Tbl_CuentasBancarias.IdBancoIPD);
                                    }
                                    else
                                    {
                                        nuevoRegistro.IdctaBanca = 0;
                                        nuevoRegistro.IdBanco = 0;
                                    }
                                }
                                else
                                {
                                    nuevoRegistro.IdctaBanca = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdctabancaIPD);
                                    nuevoRegistro.IdBanco = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdBancoIPD);
                                }

                                /*** SE SUMA EL DATO A GUARDAR Y SE CARGA EN UNA LISTA PARA SU POSTERIOR GUARDADO   ***/
                                ++cantidadDeDatosAGuardar;
                                registrosCompensados.Add(nuevoRegistro);


                                /***    SE RESTA EL SALDO AL MONTO PORQUE PUEDE SER QUE LA DEDUCTIVA NO SE PUDO COMPENSAR COMPLETAMENTE EN LA PERCEPCION INICIAR CARGADA Y SE DEBE TOMAR LA SIGUIENTE PERCEPCION PARA INTENTAR VOLVER A COMPENSAR   ***/
                                montoDDACompensar = dedu.Monto - saldo;
                                saldo = dedu.Monto - saldo;

                                if (saldo > 0)
                                {
                                    iteradorPP += 1;
                                    pp = percepciones.Skip(iteradorPP).FirstOrDefault();
                                    montoDDACompensar = saldo;
                                    dedu.Monto = montoDDACompensar;

                                    saldo = pp.Monto;
                                }
                            }


                            /***    AGREGA LA PERCEPCION RESTANTE QUE QUEDA DESPUES DE HABER COMPENSADO LAS DEDUCTIVAS  ***/
                            if (saldo > 0 && totalDeducciones == iteradorDD)
                            {
                                //agrega el sueldo restante al sueldo  para que aparesca 01 => 01
                                Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC nuevoRegistro = new Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC();
                                nuevoRegistro.IdReferenciaCancelados = referenciacancelacion.Id;
                                nuevoRegistro.AnioEnReferencia = anioSeleccionado;
                                nuevoRegistro.TipoNom = datosGeneralesEncontrados.TipoNomina;
                                nuevoRegistro.Cve_presup = datosAn.Cve_Presup;
                                nuevoRegistro.CveGto = pp.cvegasto;
                                nuevoRegistro.Monto = saldo;
                                nuevoRegistro.TipoClave = "PP";
                                nuevoRegistro.Num_che = Convert.ToString(pago.FolioCheque);
                                nuevoRegistro.CveReal = pp.Cla_perc;
                                nuevoRegistro.CveCompen = pp.Cla_perc;
                                nuevoRegistro.fecha = "";
                                nuevoRegistro.IdctaBanca = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdctabancaIPD);
                                nuevoRegistro.IdBanco = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdBancoIPD);
                                nuevoRegistro.Num = num5Digitos;
                                nuevoRegistro.NomAlpha = "";
                                nuevoRegistro.Quincena = Convert.ToString(pago.Quincena);
                                nuevoRegistro.Adicional = Convert.ToString(pago.Adicional);
                                nuevoRegistro.Cla_pto = datosAn.Cla_Pto;
                                nuevoRegistro.Ldf_6d = lDF_6M;

                                /*** SE SUMA EL DATO A GUARDAR Y SE CARGA EN UNA LISTA PARA SU POSTERIOR GUARDADO   ***/
                                ++cantidadDeDatosAGuardar;
                                registrosCompensados.Add(nuevoRegistro);

                                iteradorPP += 1;
                                pp = percepciones.Skip(iteradorPP).FirstOrDefault();
                            }




                        }

                        /***    EL SEGUNDO WHILE HABLA SOBRE LAS PERCEPCIONES YA QUE CUANDO NO HAY PERCEPCIONES YA NO HAY NADA QUE COMPENSAR Y SOLO DE DEBEN PONER TAL CUAL SE ENCONTRARON LAS PERCEPCIONES     ***/
                        while (totalPercepciones > iteradorPP)
                        {
                            //Agrega las demas percepciones al compensado 
                            Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC nuevoRegistro = new Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC();
                            nuevoRegistro.IdReferenciaCancelados = referenciacancelacion.Id;
                            nuevoRegistro.AnioEnReferencia = anioSeleccionado;
                            nuevoRegistro.TipoNom = datosGeneralesEncontrados.TipoNomina;
                            nuevoRegistro.Cve_presup = datosAn.Cve_Presup;
                            nuevoRegistro.CveGto = pp.cvegasto;
                            nuevoRegistro.Monto = pp.Monto;
                            nuevoRegistro.TipoClave = "PP";
                            nuevoRegistro.Num_che = Convert.ToString(pago.FolioCheque);
                            nuevoRegistro.CveReal = pp.Cla_perc;
                            nuevoRegistro.CveCompen = pp.Cla_perc;
                            nuevoRegistro.fecha = "";
                            nuevoRegistro.IdctaBanca = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdctabancaIPD);
                            nuevoRegistro.IdBanco = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdBancoIPD);
                            nuevoRegistro.Num = num5Digitos;
                            nuevoRegistro.NomAlpha = "";
                            nuevoRegistro.Quincena = Convert.ToString(pago.Quincena);
                            nuevoRegistro.Adicional = Convert.ToString(pago.Adicional);
                            nuevoRegistro.Cla_pto = datosAn.Cla_Pto;
                            nuevoRegistro.Ldf_6d = lDF_6M;


                            /*** SE SUMA EL DATO A GUARDAR Y SE CARGA EN UNA LISTA PARA SU POSTERIOR GUARDADO   ***/
                            ++cantidadDeDatosAGuardar;
                            registrosCompensados.Add(nuevoRegistro);

                            iteradorPP += 1;
                            pp = percepciones.Skip(iteradorPP).FirstOrDefault();
                        }
                    }
                }

            }

            /** FIn de estructura **/

            var repoIPDCompensaciones = new Repositorio<Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC>(transaccion);
            int filasAgregadas = repoIPDCompensaciones.Agregar_EntidadesMasivamente(registrosCompensados);

            if (filasAgregadas == cantidadDeDatosAGuardar)
            {
                bandera = true;
                referenciacancelacion.FechaCreacionIPDC = DateTime.Now;
                referenciacancelacion.TotalFormasIPDC = pagosEncontrados.Count;
                repoTblCancelaciones.Modificar(referenciacancelacion);
            }

            return bandera;
        }

        /***     LLENA UNA DBF DEL COMPENSADO CON LOS DATOS DE UNA REFERENCIA EN .DBF PARA DESCARGARSELA AL CLIENTE     ***/
        public static bool GeneralNuevoIPDCompensadoxAnio_2023( string pathOrigen, string pathDestino, string nombreArchivoDBFAnual, int IdReferecniaCancelacion)
        {
            bool bandera = false;
            List<IPDCDTO> registrosCompensados = new List<IPDCDTO>();
            List<string> numChequeConErrores = new List<string>();
            int cantidadDeDatosAGuardar = 0;


            if (File.Exists(pathOrigen))
            {
                if (File.Exists(pathDestino))
                {
                    File.Delete(pathDestino);
                }

                File.Copy(pathOrigen, pathDestino);
            }

            if (File.Exists(pathDestino))
            {
                /***        OBTIENE UNA LISTA DE IPD LISTRADOA PARA DESPUES EL LLENADO DEL IPD.DBF Y PODER DESCARCARCELO AL CLIENTE        ***/
                var transaccion = new Transaccion();
                var repoIpdCompensado = new Repositorio<Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC>(transaccion);
                List<Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC> resumenIpdCompensado = repoIpdCompensado.ObtenerPorFiltro(x => x.IdReferenciaCancelados == IdReferecniaCancelacion).ToList();

                foreach (Cancelaciones_InterfazPercepcionesDeduccionesCompensados_IPDC nuevoRegitroIpdCompensado in resumenIpdCompensado)
                {
                    IPDCDTO nuevoRegistro = new IPDCDTO();
                    nuevoRegistro.TipoNom = nuevoRegitroIpdCompensado.TipoNom;
                    nuevoRegistro.Cve_presup = nuevoRegitroIpdCompensado.Cve_presup;
                    nuevoRegistro.CveGto = nuevoRegitroIpdCompensado.CveGto;
                    nuevoRegistro.Monto = Convert.ToDecimal( nuevoRegitroIpdCompensado.Monto);
                    nuevoRegistro.TipoClave = nuevoRegitroIpdCompensado.TipoClave;
                    nuevoRegistro.Num_che = nuevoRegitroIpdCompensado.Num_che;
                    nuevoRegistro.CveReal = nuevoRegitroIpdCompensado.CveReal;
                    nuevoRegistro.CveCompen = nuevoRegitroIpdCompensado.CveCompen;
                    nuevoRegistro.fecha = nuevoRegitroIpdCompensado.fecha;
                    nuevoRegistro.IdctaBanca = Convert.ToInt32( nuevoRegitroIpdCompensado.IdctaBanca );
                    nuevoRegistro.IdBanco = Convert.ToInt32( nuevoRegitroIpdCompensado.IdBanco );
                    nuevoRegistro.Num = nuevoRegitroIpdCompensado.Num;
                    nuevoRegistro.NomAlpha = nuevoRegitroIpdCompensado.NomAlpha;
                    nuevoRegistro.Quincena = nuevoRegitroIpdCompensado.Quincena;
                    nuevoRegistro.Adicional = nuevoRegitroIpdCompensado.Adicional;
                    nuevoRegistro.Cla_pto = nuevoRegitroIpdCompensado.Cla_pto;
                    nuevoRegistro.Ldf_6d = Convert.ToInt32( nuevoRegitroIpdCompensado.Ldf_6d);

                    registrosCompensados.Add(nuevoRegistro);

                }
            }

            string respuestaRellado = ActualizacionDFBS.LLenarIPDCompensado(pathDestino, nombreArchivoDBFAnual, registrosCompensados);

            if (Convert.ToInt32(respuestaRellado) == registrosCompensados.Count)
            {
                bandera = true;
            }
            return bandera;
        }


        /******************************************************************************************************************************************************************/
        /******************************************************************************************************************************************************************/
        /******************************************************************************************************************************************************************/



        public static byte[] ObtenerLogErrorParaUsuario(string path)
        {
            return File.ReadAllBytes(path);
        }



        public static string ObtenerNombreReferecnia(int IdReferenciaCancel)
        {
            var transaccion = new Transaccion();
            var repoReferenciaCancelaciones = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);

            return repoReferenciaCancelaciones.Obtener(x => x.Id == IdReferenciaCancel && x.Activo).Numero_Referencia;

        }



        /************************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************************/
        /*************************************                                            REPORTES DE CHEQUES CANCELADOS                                     ********************************************************/
        /************************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************************/

        public static List<Tbl_Pagos> ObtenerListaPagosDentroReferencia(int IdReferenciaCancel)
        {
            var transaccion = new Transaccion();
            var pagosEncontrados = new Repositorio<Tbl_Pagos>(transaccion);

            return pagosEncontrados.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == IdReferenciaCancel && x.Activo == true).ToList();
        }

        public static Tbl_Referencias_Cancelaciones ObtenerDatosIdReferenciaCancelacion(int IdReferenciaCancel)
        {
            var transaccion = new Transaccion();
            var pagosEncontrados = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
            return pagosEncontrados.Obtener(x => x.Id == IdReferenciaCancel && x.Activo == true);
        }

        public static List<int> ObtenerListaAniosDentroReferencia(int IdReferenciaCancel)
        {
            var transaccion = new Transaccion();
            var pagosEncontrados = new Repositorio<Tbl_Pagos>(transaccion);

            return pagosEncontrados.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == IdReferenciaCancel && x.Activo == true /*&& x.Anio == 2021*/ ).Select(x => x.Anio).Distinct().ToList();
        }
        public static List<int> ObtenerListaAniosDentroReferenciaIPD(int IdReferenciaCancel)
        {
            //El IPD NO TIENE COMTEMPLADO LA NOMINA 08 QUE ES ACTUALMENTE EN EL 2023 LA DE PENSION ALIMENTICIA
            var transaccion = new Transaccion();
            var pagosEncontrados = new Repositorio<Tbl_Pagos>(transaccion);

            return pagosEncontrados.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == IdReferenciaCancel && x.Nomina != "08" && x.EsPenA == false && x.Activo == true).Select(x => x.Anio).Distinct().ToList();
        }



        /******************************************************************** Filtro para obtener la o las consultas segun los reportes a consultar **************************************************************************/
        public static List<string> ObtenerListaConsultasXTipoReporteNombreReporte(string TipoReporte, string NombreReporte)
        {
            var transaccion = new Transaccion();
            var consultafiltradaEncontrados = new Repositorio<cat_ReportesCCancelados>(transaccion);
            return consultafiltradaEncontrados.ObtenerPorFiltro(x => x.TipoReporte == TipoReporte.Trim().ToUpper() && x.NombreReporte == NombreReporte).Select(x => x.Consulta).ToList();
        }

        public static string ObtenerUnicaConsultaXTipoReporteNombreReporte(string TipoReporte, string NombreReporte)
        {
            var transaccion = new Transaccion();
            var consultafiltradaEncontrados = new Repositorio<cat_ReportesCCancelados>(transaccion);
            return consultafiltradaEncontrados.Obtener(x => x.TipoReporte == TipoReporte.Trim().ToUpper() && x.NombreReporte == NombreReporte).Consulta;
        }


        /***********************************************************************************************************************************************************************/
        /******************************************************************** REPORTES INICIALES********************************************************************************/
        /****   ---- Obtener Registros para NominaAnual ---- ****/
        public static List<NominasAnualesDTO> ObtenerRegistrosNominaAnual(int IdReferenciaCancelacion , int  Anio) 
        {
            List<string> consultas = ObtenerListaConsultasXTipoReporteNombreReporte("INICIAL", "NominaAnual");

            List<string> nuevasConsultas = new List<string>();
            foreach (string query in consultas)
            {
                string nuevoQuery = "";
                nuevoQuery = query.Replace("[NombreDB]", ObtenerConexionesDB.ObtenerNombreDBValidacionFoliosDeploy());
                nuevoQuery = nuevoQuery.Replace("[IdReferencia]", ""+IdReferenciaCancelacion+"");
                nuevoQuery = nuevoQuery.Replace("[ANIO]", "" + Anio + "");
                nuevoQuery = nuevoQuery.Replace("[PartidaCAPAE]", CrearReferencia_CanceladosDbSinEntity.ObtenerPartidaCapae(Anio));

                nuevoQuery = nuevoQuery.Replace("\r", "");
                nuevoQuery = nuevoQuery.Replace("\n", "");
                nuevoQuery = nuevoQuery.Replace("\t", "");

                nuevasConsultas.Add(nuevoQuery);
            }

            return CrearReferencia_CanceladosDbSinEntity.ObtenerRegistrosNominaAnual(nuevasConsultas);
        }

        /****   ---- Obtener Registros paRA REPORTE DE INTERFACES DE CHEQUES CANCELADOS POR CUENTA BANCARIA --- ****/
        public static List<CuentasBancariasAnualesDTO> ObtenerRegistrosBancosAnual(int IdReferenciaCancelacion, int Anio)
        {
            List<string> consultas = ObtenerListaConsultasXTipoReporteNombreReporte("INICIAL", "CuentaBancariaAnual");

            List<string> nuevasConsultas = new List<string>();
            foreach (string query in consultas)
            {
                string nuevoQuery = "";
                nuevoQuery = query.Replace("[NombreDB]", ObtenerConexionesDB.ObtenerNombreDBValidacionFoliosDeploy());
                nuevoQuery = nuevoQuery.Replace("[IdReferencia]", ""+IdReferenciaCancelacion+"");
                nuevoQuery = nuevoQuery.Replace("[ANIO]", ""+Anio+"");
                nuevoQuery = nuevoQuery.Replace("[PartidaCAPAE]", CrearReferencia_CanceladosDbSinEntity.ObtenerPartidaCapae(Anio));

                nuevoQuery = nuevoQuery.Replace("\r", "");
                nuevoQuery = nuevoQuery.Replace("\n", "");
                nuevoQuery = nuevoQuery.Replace("\t", "");

                nuevasConsultas.Add(nuevoQuery);
            }

            return CrearReferencia_CanceladosDbSinEntity.ObtenerRegistrosCuentaBancariaAnual(nuevasConsultas);
        }

        /****   ---- Obtener Registros para REPORTE DE PENSION ALIMENTICIA DE CHEQUES CANCELADOS --- ****/
        public static List<PensionAlimenticiaDTO> ObtenerRegistrosPensionAlimenticia(int IdReferenciaCancelacion, List<int> Anios)
        {
            string consultaObtenida = ObtenerUnicaConsultaXTipoReporteNombreReporte("INICIAL", "PensionAlimenticia");

            List<string> nuevasConsultas = new List<string>();
            foreach (int item in Anios)
            {
                string anioParaRamoYUnidad = "";
                string soloAnio = ""+item+"";
                if (DateTime.Now.Year != item)
                {
                    anioParaRamoYUnidad = "_"+item+" ";
                  
                }

                string nuevoQuery = "";
                nuevoQuery = consultaObtenida.Replace("[ANIORamoUnidad]", ""+anioParaRamoYUnidad+"");
                nuevoQuery = nuevoQuery.Replace("[ANIO]", ""+soloAnio+"");
                nuevoQuery = nuevoQuery.Replace("[IdReferencia]", ""+IdReferenciaCancelacion+"");
                nuevoQuery = nuevoQuery.Replace("[NombreDB]", ObtenerConexionesDB.ObtenerNombreDBValidacionFoliosDeploy());
                nuevoQuery = nuevoQuery.Replace("\r", "");
                nuevoQuery = nuevoQuery.Replace("\n", "");
                nuevoQuery = nuevoQuery.Replace("\t", "");

                nuevasConsultas.Add(nuevoQuery);
            }

            return CrearReferencia_CanceladosDbSinEntity.ObtenerRegistrosPensionAlimenticia(nuevasConsultas);
        }

        public static void VerificarNumeroArchivosEnDirectorio(string pathAVerificar) 
        {
            string[] dirs = Directory.GetFiles(pathAVerificar, "*.PDF");

            if (dirs.Length == 0) 
            {
                TextWriter Escribir = new StreamWriter(pathAVerificar+"/LEER_INFO.txt");
                
                Escribir.WriteLine("NO EXISTEN REGISTROS DE PENSION ALIMENTICIA PARA NINGUN AÑIO - 'CONTINUE CON LO SIGUIENTE EN PAZ' ");

                Escribir.Close();

            }

        }

     





        #region GENERA UN NUEVO REPORTE TGCxNOMINA DE TOTALES GENERALES POR CONCEPTO DE NOMINA DE UNA REFERENCIA DE CANCELACION DE CHEQUES CANCELADOS
        /******************************************************************************************************************************************/
        /**********    FILTRO PARA OBTENER LAS CONSULTAS QUE SE DEBEN APLICAR PARA EL REPORTE DE TGCXNOMINA DE CHEQUES CANCELADOS       ***********/
        /******************************************************************************************************************************************/
        public static List<string> ObtenerConsultasReporteTGCxNomina_CuotasPatronales_IPD(int IdReferenciaCancelacion, int Anio, string TipoReporte, string NombreReporte)
        {
            var transaccion = new Transaccion();
            var repoCatReportesCancelados = new Repositorio<cat_ReportesCCancelados>(transaccion);
            List<string> consultasObtenidas = repoCatReportesCancelados.ObtenerPorFiltro(x => x.TipoReporte == TipoReporte && x.NombreReporte == NombreReporte).Select(x => x.Consulta).ToList();


            string visitaCatxAnio = FoliarNegocios.ObtenerCadenaAnioInterface(Anio);
            visitaCatxAnio = !string.IsNullOrWhiteSpace(visitaCatxAnio) ? "_" + visitaCatxAnio : visitaCatxAnio;


            List<string> nuevasConsultas = new List<string>();
            foreach (string query in consultasObtenidas)
            {
                string nuevoQuery = "";
                nuevoQuery = query.Replace("[Var_Anio]", "" + visitaCatxAnio + "");
                nuevoQuery = nuevoQuery.Replace("[Var_IdReferenciaCancelados]", "" + IdReferenciaCancelacion + "");
                nuevoQuery = nuevoQuery.Replace("[Var_AnioEnReferencia]", "" + Anio + "");
                /***     PartidaCAPAE    ***/
                nuevoQuery = nuevoQuery.Replace("[Var_CAPAE]", CrearReferencia_CanceladosDbSinEntity.ObtenerPartidaCapae(Anio));

                nuevoQuery = nuevoQuery.Replace("\r", "");
                nuevoQuery = nuevoQuery.Replace("\n", "");
                nuevoQuery = nuevoQuery.Replace("\t", "");

                //if(query.Contains("NOMINAS DE CAPAE"))
                nuevasConsultas.Add(nuevoQuery);
            }
            return nuevasConsultas;
        }


        public static string ObtenerConsultasUnicaCorrecta_TGCxPP_CuotasPatronales_IPD(string consultaTGCxPPoCP,  int IdReferenciaCancelacion, int Anio)
        {
            string visitaCatxAnio = FoliarNegocios.ObtenerCadenaAnioInterface(Anio);
            visitaCatxAnio = !string.IsNullOrWhiteSpace(visitaCatxAnio) ? "_" + visitaCatxAnio : visitaCatxAnio;

            string nuevoQuery = "";
                nuevoQuery = consultaTGCxPPoCP.Replace("[Var_Anio]", "" + visitaCatxAnio + "");
                nuevoQuery = nuevoQuery.Replace("[Var_IdReferenciaCancelados]", "" + IdReferenciaCancelacion + "");
                nuevoQuery = nuevoQuery.Replace("[Var_AnioEnReferencia]", "" + Anio + "");
                /***     PartidaCAPAE    ***/
                nuevoQuery = nuevoQuery.Replace("[Var_CAPAE]", CrearReferencia_CanceladosDbSinEntity.ObtenerPartidaCapae(Anio));

                nuevoQuery = nuevoQuery.Replace("\r", "");
                nuevoQuery = nuevoQuery.Replace("\n", "");
                nuevoQuery = nuevoQuery.Replace("\t", "");

                //if(query.Contains("NOMINAS DE CAPAE"))
            return nuevoQuery;
        }

        /******************************************************************************************************************************************/
        /**********               OBTIENE LOS DETALLES EN DTO PARA SU INSERSION EN EL DATASET Y DESCARGAR AL CLIENTE                    ***********/
        /******************************************************************************************************************************************/
        public static List<RegistrosTGCxNominaDTO> TotalesGeneralesXConcepto(int IdReferenciaCancelacion, int Anio, string nuevaConsulta)
        {
            /*************************************************************************************************************************************************************/
            /*******************************************************    DescargarTGCxNomina_2023   ***********************************************************************/
            /*************************************************************************************************************************************************************/
            /***   2-.EN EL TEMA DE LLENADO DE DATOS SE VUELVE UN POCO MAS SIMPLE POR QUE LA IDEA ERA TRATARLO COMO UNA MATRIZ PERO SE VUELVE UN POBLEMA               ***/
            /***     YA QUE LA MATRIZ TENDRIA QUE SER DINAMICA PARA AMBOS SEGMENTOS Y ESTO GENERARIA UN PROBLEMA DE BUSQUEDA Y CORRELACIONES POR ESO DECIDI USAR       ***/
            /***     LISTAS DINAMICAS, LAS CUALES SE CARGAN INICIALMENTE EN UN MODELO DEFINIDO CON TODOS LOS CAMPOS DE LOS N REPORTES                                  ***/
            /***     EN EL 1ER PASO SE OBTIENE EL DETALLE DE LA INFORMACION DE LA DB DE ACUERDO DE LA REFERENCIA DE CANCELACION Y SE CALCULA Y AGREGA UN RESUMN        ***/
            /***     EN EL 2DO PASO SE CALCULA LOS RESUMEN TOTALES DE LAS PP Y DD DE CADA RAMO Y EL RESUMEN Y SE ACTUALIZADA LA LISTA DE ACUERDO A CADA RAMO           ***/
            /***     EN EL 3ER PASO SE AGRUPAN Y SE CARGAN AL DATA SET                                                                                                 ***/
            /*************************************************************************************************************************************************************/
            /*******************************************************    DescargarTGCxNomina_2023   ***********************************************************************/
            /*************************************************************************************************************************************************************/
            List<RegistrosTGCxNominaDTO> resumenGeneralTotalesCvePD = new List<RegistrosTGCxNominaDTO>();


            /*** 2DO PASO=> EN ESTE PASO SE OBTIENE EL PUNTO 2, BASICAMENTE ES OBTENER EL DETALLE DE LA CONSULTA Y SACARLE SU RESUMEN DE CLAVES DE PP Y DD  ***/
            List<RegistrosTGCxNominaDTO> detallesTGCNominaObtenidos = ObtenerTotalesGeneralesXConceptoIncluyeResumen(IdReferenciaCancelacion, Anio, nuevaConsulta);

            /***************************************************/
            /***    AGREGA LOS TOTALES A CADA PARTIDA        ***/
            /***************************************************/
            var detallesAgrupados = detallesTGCNominaObtenidos.GroupBy(x => x.Partida).OrderBy(x => x.Key);
            foreach (var nuevoGrupo in detallesAgrupados)
            {
                decimal sumaPositiva_PP = nuevoGrupo.Sum(x => x.PP_SumatoriaPositiva);
                decimal SumaNegativa_PP = nuevoGrupo.Sum(x => x.PP_SumatoriaNegativa);

                decimal sumaPositiva_DD = nuevoGrupo.Sum(x => x.DD_SumatoriaPositiva);
                decimal SumaNegativa_DD = nuevoGrupo.Sum(x => x.DD_SumatoriaNegativa);

                decimal liquidoTotal = (sumaPositiva_PP + SumaNegativa_DD) - (sumaPositiva_DD + SumaNegativa_PP);


                foreach (RegistrosTGCxNominaDTO nuevodetalle in nuevoGrupo)
                {
                    /************************************/
                    /*********      Totales       *******/
                    /************************************/
                    nuevodetalle.PP_TotalPositivo = sumaPositiva_PP;
                    nuevodetalle.PP_TotalNegativo = SumaNegativa_PP;

                    nuevodetalle.DD_TotalPositivo = sumaPositiva_DD;
                    nuevodetalle.DD_TotalNegativo = SumaNegativa_DD;

                    nuevodetalle.Liquido = liquidoTotal;
                }

            }

            foreach (RegistrosTGCxNominaDTO nuevoResumenConcepto in resumenGeneralTotalesCvePD)
            {
                detallesTGCNominaObtenidos.Add(nuevoResumenConcepto);
            }


            List<RegistrosTGCxNominaDTO> agrupadoCorrecto = detallesTGCNominaObtenidos.OrderBy(x => x.Partida).ToList();

            return agrupadoCorrecto;
        }


        /******************************************************************************************************************************************/
        /**********                             REALIZA LA CONSULTA Y SACA EL RESUMEN DE LOS TOTALESGENERALESXCONCEPTO                                     ***********/
        /******************************************************************************************************************************************/
        public static List<RegistrosTGCxNominaDTO> ObtenerTotalesGeneralesXConceptoIncluyeResumen(int IdReferenciaCancelacion, int Anio, string nuevaConsulta)
        {
            /*** 2DO PASO=> EN ESTE PASO SE OBTIENE EL PUNTO 2, BASICAMENTE ES OBTENER EL DETALLE DE LA CONSULTA Y SACARLE SU RESUMEN DE CLAVES DE PP Y DD  ***/

            List<RegistrosTGCxNominaDTO> resumenGeneralTotalesCvePD = new List<RegistrosTGCxNominaDTO>();
            List<RegistrosTGCxNominaDTO> detallesTGCNominaObtenidos = Datos.TablasAfectadasFCCBNetDB.Cancelaciones_InterfazPercepcionesDeducciones_IPD_DbSinEntity.LeerConsultaReporteTGCNomina(nuevaConsulta);

            var detallesAgrupados_PP = detallesTGCNominaObtenidos.Where(x => x.PP_CvePD != null).GroupBy(x => new { x.pp_DescripcionCvePD, x.PP_CvePD, x.PP_TipoClave }).OrderBy(x => x.Key.PP_CvePD);
            foreach (var nuevoGrupo in detallesAgrupados_PP)
            {

                RegistrosTGCxNominaDTO nuevoResumenCvePD_PP = new RegistrosTGCxNominaDTO();
                nuevoResumenCvePD_PP.Ramo = "RESUMEN GENERAL TOTALES";
                nuevoResumenCvePD_PP.Partida = "00-00";


                /******************************/
                /***    DATOS PERCEPCIONES  ***/
                /******************************/
                nuevoResumenCvePD_PP.PP_TipoClave = nuevoGrupo.Key.PP_TipoClave;
                nuevoResumenCvePD_PP.PP_Cantidad = nuevoGrupo.Sum(x => x.PP_Cantidad);
                nuevoResumenCvePD_PP.PP_CvePD = nuevoGrupo.Key.PP_CvePD;
                nuevoResumenCvePD_PP.pp_DescripcionCvePD = nuevoGrupo.Key.pp_DescripcionCvePD;
                nuevoResumenCvePD_PP.PP_SumatoriaPositiva = nuevoGrupo.Sum(x => x.PP_SumatoriaPositiva);

                /***    si contiene una percepcion negativa se debe de mostrar positiva aunque en la practiva se sabe que se debe de netear (tecnicamente nunca debe de haber datos en la siguiente propiedad )    ***/
                nuevoResumenCvePD_PP.PP_SumatoriaNegativa = nuevoGrupo.Sum(x => x.PP_SumatoriaNegativa);

                resumenGeneralTotalesCvePD.Add(nuevoResumenCvePD_PP);
            }


            var detallesAgrupados_DD = detallesTGCNominaObtenidos.Where(x => x.DD_CvePD != null).GroupBy(x => new { x.DD_DescripcionCvePD, x.DD_CvePD, x.DD_TipoClave }).OrderBy(x => x.Key.DD_CvePD);
            foreach (var nuevoGrupo in detallesAgrupados_DD)
            {
                RegistrosTGCxNominaDTO nuevoResumenCvePD_DD = new RegistrosTGCxNominaDTO();
                nuevoResumenCvePD_DD.Ramo = "RESUMEN GENERAL TOTALES";
                nuevoResumenCvePD_DD.Partida = "00-00";

                /******************************/
                /***    DATOS DEDUCCIONES  ***/
                /******************************/
                nuevoResumenCvePD_DD.DD_TipoClave = nuevoGrupo.Key.DD_TipoClave;
                nuevoResumenCvePD_DD.DD_Cantidad = nuevoGrupo.Sum(x => x.DD_Cantidad);
                nuevoResumenCvePD_DD.DD_CvePD = nuevoGrupo.Key.DD_CvePD;
                nuevoResumenCvePD_DD.DD_DescripcionCvePD = nuevoGrupo.Key.DD_DescripcionCvePD;
                nuevoResumenCvePD_DD.DD_SumatoriaPositiva = nuevoGrupo.Sum(x => x.DD_SumatoriaPositiva);

                /***    si contiene una deduccion negativa se debe de mostrar aunque en la practiva se sabe que se debe de sumar    ***/
                nuevoResumenCvePD_DD.DD_SumatoriaNegativa = nuevoGrupo.Sum(x => x.DD_SumatoriaNegativa);
                resumenGeneralTotalesCvePD.Add(nuevoResumenCvePD_DD);
            }

            /*** TERMINA AGREGANDO EL RESUMEN GENERADO AL RESUMEN ***/
            foreach (RegistrosTGCxNominaDTO nuevoResumenConcepto in resumenGeneralTotalesCvePD)
            {
                detallesTGCNominaObtenidos.Add(nuevoResumenConcepto);
            }

            return detallesTGCNominaObtenidos;
        }

        #endregion




        #region GENERA EL  REPORTE DE TGC X CUENTAS BANCARIAS DE UNA REFERENCIA DE CANCELACION DE CHEQUES

        public static List<int> ObtenerIdsCuentasBancariasEnReferenciaCancelacionXAnio(int IdReferenciaCancelacion, int Anio)
        {
            var transaccion = new Transaccion();
            var repoTblIPD = new Repositorio<Cancelaciones_InterfazPercepcionesDeducciones_IPD>(transaccion);

            List<int> idsCuentasBancariasEncontrados = repoTblIPD.ObtenerPorFiltro(x => x.IdReferenciaCancelados == IdReferenciaCancelacion && x.AnioEnReferencia == Anio).Select(x => x.IdBancoPagadorFCCBNetDb).Distinct().ToList();
          
            return idsCuentasBancariasEncontrados;
        }

     

        public static string ObtenerConsultasReporteTGCxCuentaBancaria_IPD(int IdReferenciaCancelacion, int Anio, string TipoReporte, string NombreReporte, int idBancoPagador)
        {
            var transaccion = new Transaccion();
            var repoCatReportesCancelados = new Repositorio<cat_ReportesCCancelados>(transaccion);
            var repoTblCuentasBancarias = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            string consultaObtenida = repoCatReportesCancelados.Obtener(x => x.TipoReporte == TipoReporte && x.NombreReporte == NombreReporte).Consulta;

            Tbl_CuentasBancarias cuentaSeleccionada = repoTblCuentasBancarias.Obtener(x => x.Id == idBancoPagador);
            string nombrecuentaBancaria = cuentaSeleccionada.NombreBanco + " [ " + cuentaSeleccionada.Cuenta + " ] ";

                string nuevoQuery = consultaObtenida.Replace("[Var_NombreCuentaBancaria]", ""+ nombrecuentaBancaria + "");
                nuevoQuery = nuevoQuery.Replace("[Var_IdReferenciaCancelados]", ""+IdReferenciaCancelacion+"");
                nuevoQuery = nuevoQuery.Replace("[Var_AnioEnReferencia]", "" + Anio + "");
                nuevoQuery = nuevoQuery.Replace("[Var_Idctabanca]", "" + cuentaSeleccionada.IdctabancaIPD + "");
                nuevoQuery = nuevoQuery.Replace("[Var_IdBanco]", "" + cuentaSeleccionada.IdBancoIPD + "");
            
                nuevoQuery = nuevoQuery.Replace("\r", "");
                nuevoQuery = nuevoQuery.Replace("\n", "");
                nuevoQuery = nuevoQuery.Replace("\t", "");

            return nuevoQuery;
        }

        /******************************************************************************************************************************************/
        /**********               OBTIENE LOS DETALLES EN DTO PARA SU INSERSION EN EL DATASET Y DESCARGAR AL CLIENTE                    ***********/
        /******************************************************************************************************************************************/
        public static List<RegistrosTGCxCuentaBancariaDTO> TotalesGeneralesXCuentaBancaria(List<string> ConsultasTGCxCuentaBancaria)
        {
            List<RegistrosTGCxCuentaBancariaDTO> resumenDeTodosLosBancosEntontrados = new List<RegistrosTGCxCuentaBancariaDTO>();


            /*** EXECUTA LA CONSULTA Y DEVUELVE LOS REGISDTROS RESULTANTES DE LA CONSULTA ***/
            foreach (string nuevaConsulta in ConsultasTGCxCuentaBancaria) 
            {
                List<RegistrosTGCxCuentaBancariaDTO> detallesObtenidosTGCxCuentaBancarias = Datos.TablasAfectadasFCCBNetDB.Cancelaciones_InterfazPercepcionesDeducciones_IPD_DbSinEntity.LeerConsultaReporteTGCxCuentaBancaria(nuevaConsulta);


                /***************************************************/
                /***         AGREGA LOS TOTALES                  ***/
                /***************************************************/

                decimal sumaPositiva_PP = detallesObtenidosTGCxCuentaBancarias.Sum(x => x.PP_SumatoriaPositiva);
                decimal SumaNegativa_PP = detallesObtenidosTGCxCuentaBancarias.Sum(x => x.PP_SumatoriaNegativa);

                decimal sumaPositiva_DD = detallesObtenidosTGCxCuentaBancarias.Sum(x => x.DD_SumatoriaPositiva);
                decimal SumaNegativa_DD = detallesObtenidosTGCxCuentaBancarias.Sum(x => x.DD_SumatoriaNegativa);

                decimal liquidoTotal = (sumaPositiva_PP + SumaNegativa_DD) - (sumaPositiva_DD + SumaNegativa_PP);


                foreach (RegistrosTGCxCuentaBancariaDTO nuevodetalle in detallesObtenidosTGCxCuentaBancarias)
                {
                    /************************************/
                    /*********      Totales       *******/
                    /************************************/
                    nuevodetalle.PP_TotalPositivo = sumaPositiva_PP;
                    nuevodetalle.PP_TotalNegativo = SumaNegativa_PP;

                    nuevodetalle.DD_TotalPositivo = sumaPositiva_DD;
                    nuevodetalle.DD_TotalNegativo = SumaNegativa_DD;

                    nuevodetalle.Liquido = liquidoTotal;

                    resumenDeTodosLosBancosEntontrados.Add(nuevodetalle);
                }

            }

            return resumenDeTodosLosBancosEntontrados;
        }



        public static List<RegistrosTGCxCuentaBancariaDTO> ResumenTGCxCuentaBancaria(int IdReferenciaCancelacion, int Anio) 
        {
            List<RegistrosTGCxCuentaBancariaDTO> ResumenTGCxCB = new List<RegistrosTGCxCuentaBancariaDTO>();
            List<int> idsBancosEnReferencia = CrearReferencia_CanceladosNegocios.ObtenerIdsCuentasBancariasEnReferenciaCancelacionXAnio(IdReferenciaCancelacion, Anio);
            List<string> consultasTGCxCuentaBancaria = new List<string>();
            foreach (int nuevoIdBancoEncontrado in idsBancosEnReferencia)
            {
                /*** OBTIENE LA CONSULTA QUE LE CORRESPONDEN AL ID DEL BANCO SELECCIONADO  ***/
                consultasTGCxCuentaBancaria.Add(CrearReferencia_CanceladosNegocios.ObtenerConsultasReporteTGCxCuentaBancaria_IPD(IdReferenciaCancelacion, Anio, "IPD", "TGCxCUENTABANCARIA", nuevoIdBancoEncontrado));
            }

            List<RegistrosTGCxCuentaBancariaDTO> resumenDeTodosLosBancosEntontrados = CrearReferencia_CanceladosNegocios.TotalesGeneralesXCuentaBancaria(consultasTGCxCuentaBancaria).OrderBy(x => x.NombreCuentaBancaria).ToList();

            return resumenDeTodosLosBancosEntontrados;
        }



        #endregion

        #region GENERA EL REPORTE DE CUOTAS PATRONALES POR NOMINA Y POR AÑO
        public static List<RegistrosCuotasPatronalesDTO> ObtenerDetallesCuotasPatronales(string consulta) 
        {
            List<RegistrosCuotasPatronalesDTO> coutasPatronalesNomina = Datos.TablasAfectadasFCCBNetDB.Cancelaciones_InterfazPercepcionesDeducciones_IPD_DbSinEntity.LeerConsultaReporteCoutasPatronales(consulta);

            var cpAgrupadosRamo = coutasPatronalesNomina.GroupBy(x => x.NumeroRamo).ToList();

            foreach (var gruposeleccionado in cpAgrupadosRamo) 
            {
                foreach (RegistrosCuotasPatronalesDTO cp in gruposeleccionado) 
                {
                    cp.TotalRamo_Cantidad = gruposeleccionado.Sum(x => x.Cantidad);
                    cp.TotalRamo_Monto = gruposeleccionado.Sum(x => x.MontoPositivo);
                }
            }
            return coutasPatronalesNomina;
        }

        #endregion

        #region GENERA LOS DATOS PARA EL RESUMEN DE CHEQUES CANCELADOS (PERCEPCIONES Y CUOTAS PATRONALES )
        public static List<ResumenGeneralXNominaDTO> ResumenPercepcionesCuotasPatronalesXAnio(int IdReferencia, int anioSeleccionado)
        {
            List<ResumenGeneralXNominaDTO> ResumenGeneralPercepcionesCuotasPatronales = new List<ResumenGeneralXNominaDTO>();

            var transaccion = new Transaccion();
            var repoCatReportesCancelados = new Repositorio<cat_ReportesCCancelados>(transaccion);
            List<cat_ReportesCCancelados> consultasTGCxNominaObtenidas = repoCatReportesCancelados.ObtenerPorFiltro(x => x.TipoReporte == "IPD" && x.NombreReporte == "TGCNomina").OrderBy(x => x.Nomina).ToList();

            int iterador = 0;
            foreach(cat_ReportesCCancelados nuevaConsultaTGCxNom in consultasTGCxNominaObtenidas)
            {
                ++ iterador;

                /***    OBTENER TGCxNOMINA  ***/
                string consultaTGCxNomina = ObtenerConsultasUnicaCorrecta_TGCxPP_CuotasPatronales_IPD(nuevaConsultaTGCxNom.Consulta, IdReferencia, anioSeleccionado);
                List<RegistrosTGCxNominaDTO> tGCxNominaEncontradosNomina = TotalesGeneralesXConcepto(IdReferencia, anioSeleccionado, consultaTGCxNomina).Where(x => x.PP_TipoClave == "PP" && x.Partida != "00-00").ToList();

                if(tGCxNominaEncontradosNomina != null)
                {
                    /***  OBTIENE LA CONSULTA DE CUOTAS PATRONALES ASOCIADO A LA NOMINA VISITADA EN TGCxNOMINA  ***/
                    string consultaCuotasPatronalesDB = repoCatReportesCancelados.Obtener(x => x.TipoReporte == "IPD" && x.NombreReporte == "CuotasPatronales" && x.Nomina.Equals(nuevaConsultaTGCxNom.Nomina.Trim())).Consulta;
                    string consultaTGCxCP = ObtenerConsultasUnicaCorrecta_TGCxPP_CuotasPatronales_IPD(consultaCuotasPatronalesDB, IdReferencia, anioSeleccionado);

                    List<RegistrosCuotasPatronalesDTO> cuotasPatronalesEncontradosNomina = ObtenerDetallesCuotasPatronales(consultaTGCxCP);


                    ResumenGeneralXNominaDTO nuevoRegistroResumenGeneralPpCp = new ResumenGeneralXNominaDTO();
                    nuevoRegistroResumenGeneralPpCp.IdVirtual = iterador;
                    nuevoRegistroResumenGeneralPpCp.NombreDescripcionNomina = tGCxNominaEncontradosNomina.FirstOrDefault().NombreNomina.ToUpper().Trim();
                    nuevoRegistroResumenGeneralPpCp.PP_Regs = tGCxNominaEncontradosNomina.Sum(x => x.PP_Cantidad);
                    nuevoRegistroResumenGeneralPpCp.PP_Monto = tGCxNominaEncontradosNomina.Sum(x => x.PP_SumatoriaPositiva);
                    nuevoRegistroResumenGeneralPpCp.CP_Regs = cuotasPatronalesEncontradosNomina.Sum(x => x.Cantidad);
                    nuevoRegistroResumenGeneralPpCp.CP_Monto = cuotasPatronalesEncontradosNomina.Sum(x => x.MontoPositivo );

                    nuevoRegistroResumenGeneralPpCp.Total_Regs  = nuevoRegistroResumenGeneralPpCp.PP_Regs + nuevoRegistroResumenGeneralPpCp.CP_Regs;
                    nuevoRegistroResumenGeneralPpCp.Total_Monto = nuevoRegistroResumenGeneralPpCp.PP_Monto + nuevoRegistroResumenGeneralPpCp.CP_Monto;

                    ResumenGeneralPercepcionesCuotasPatronales.Add(nuevoRegistroResumenGeneralPpCp);
                }
            }

            return ResumenGeneralPercepcionesCuotasPatronales;
        }

        #endregion

    }
}

