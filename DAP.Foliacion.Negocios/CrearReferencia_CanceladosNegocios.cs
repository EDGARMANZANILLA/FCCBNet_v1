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


            Tbl_Referencias_Cancelaciones nuevoNumeroRefenciaCanceladoExiste = repositorio.Obtener(x => x.Anio == DateTime.Now.Year && x.Numero_Referencia == nuevoNumero && x.Activo == true);
            int idAgregado = 0;
            if (nuevoNumeroRefenciaCanceladoExiste == null)
            {
                Tbl_Referencias_Cancelaciones nuevaReferencia = new Tbl_Referencias_Cancelaciones();
                nuevaReferencia.Anio = DateTime.Now.Year;
                nuevaReferencia.Numero_Referencia = "CC" + nuevoNumero;
                nuevaReferencia.Fecha_Creacion = DateTime.Now.Date;
                nuevaReferencia.FormasPagoDentroReferencia = 0;
                nuevaReferencia.Creado_Por = "**********";
                nuevaReferencia.EsCancelado = false;
                nuevaReferencia.Activo = true;
                idAgregado = repositorio.Agregar(nuevaReferencia).Id;

            }

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


        /***********************************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************************/
        /*************************************                       Metodos para Generar y Descargar el IPD y el IDP_Compensado                             *******************************************************/
        public static string GenerarNuevoIPD(string pathOrigen, string pathDestino, string NombreArchivo, int IdReferecniaCancelacion)
        {
            //string pathOrigen = path + "/IPDVacio.dbf";


            //string nombreArchivo = "CC20212.dbf";
            //string pathDestino = path + "/"+nombreArchivo;



            if (File.Exists(pathOrigen))
            {
                //File.Move(pathOrigen, pathDestino);
                File.Copy(pathOrigen, pathDestino);

            }



            /***********************************************************************/
            /** estructura para obtener una lista con la que se rellena elipd.dbf **/
            var transaccion = new Transaccion();
            var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);

            List<Tbl_Pagos> pagosEncontrados = repoTblPagos.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == IdReferecniaCancelacion && x.Nomina != "08" && x.Activo == true).ToList();

            List<IPDDTO> PagosACargar = new List<IPDDTO>();
            int cantidadDeDatosAGuardar = 0;
            foreach (Tbl_Pagos pago in pagosEncontrados)
            {
                string anio = "";
                if (DateTime.Now.Year != pago.Anio)
                {
                    anio = Convert.ToString(pago.Anio);
                }


                DatosGeneralesIPD_DTO datosGeneralesEncontrados = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosGeneralesxIdNom_IPD(pago.Id_nom, anio);


                if (datosGeneralesEncontrados != null)
                {
                    string num5Digitos = Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(pago.NumEmpleado);
                    DatosAnIPD_DTO datosAn = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosAN_IPD(anio, datosGeneralesEncontrados.AN, num5Digitos);


                    if (datosAn != null)
                    {
                        //Obtener Percepciones y Deducciones
                        List<DatosApercepcionesIPD_DTO> percepciones = CrearReferencia_CanceladosDbSinEntity.ObtenerPercepciones_IPD(anio, datosGeneralesEncontrados.AP, num5Digitos);
                        List<DatosAdeducionesIPD_DTO> deducciones = CrearReferencia_CanceladosDbSinEntity.ObtenerDeducciones_IPD(anio, datosGeneralesEncontrados.AD, num5Digitos);

                        foreach (DatosApercepcionesIPD_DTO nuevaPercepcion in percepciones)
                        {
                            cantidadDeDatosAGuardar += 1;
                            IPDDTO nuevopPago = new IPDDTO();
                            nuevopPago.Referencia = NombreArchivo;
                            nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                            nuevopPago.Cve_presup = datosAn.Cve_Presup;
                            nuevopPago.Cvegto = nuevaPercepcion.cvegasto;
                            nuevopPago.Cvepd = nuevaPercepcion.Cla_perc;
                            nuevopPago.Monto = nuevaPercepcion.Monto;
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
                            IPDDTO nuevopPago = new IPDDTO();
                            nuevopPago.Referencia = NombreArchivo;
                            nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                            nuevopPago.Cve_presup = datosAn.Cve_Presup;
                            nuevopPago.Cvegto = nuevaDeduccion.Cvegasto;
                            nuevopPago.Cvepd = nuevaDeduccion.Cla_dedu;
                            nuevopPago.Monto = nuevaDeduccion.Monto;
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


                        //Couta patronal del ISSTE
                        if (datosAn.Patronal_ISSTE > 0)
                        {
                            cantidadDeDatosAGuardar += 1;
                            IPDDTO nuevopPago = new IPDDTO();
                            nuevopPago.Referencia = NombreArchivo;
                            nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                            nuevopPago.Cve_presup = datosAn.Cve_Presup;
                            nuevopPago.Cvegto = "1411";
                            nuevopPago.Cvepd = "";
                            nuevopPago.Monto = datosAn.Patronal_ISSTE;
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
                            nuevopPago.Cvegasto = "1411";
                            nuevopPago.Cla_pto = datosAn.Cla_Pto;

                            PagosACargar.Add(nuevopPago);
                        }


                        //Couta patronal del ISSSTECAM
                        if (datosAn.Patronal_ISSSTECAM > 0)
                        {
                            cantidadDeDatosAGuardar += 1;
                            IPDDTO nuevopPago = new IPDDTO();
                            nuevopPago.Referencia = NombreArchivo;
                            nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                            nuevopPago.Cve_presup = datosAn.Cve_Presup;
                            nuevopPago.Cvegto = "1413";
                            nuevopPago.Cvepd = "";
                            nuevopPago.Monto = datosAn.Patronal_ISSSTECAM;
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




            string respuestaRellado = ActualizacionDFBS.LLenarIPD(pathDestino, NombreArchivo, PagosACargar);



            string respuesta = "";
            if (Convert.ToInt32(respuestaRellado) == cantidadDeDatosAGuardar)
            {
                respuesta = "CORRECTO";
            }





            return respuesta;
        }


        public static bool GeneralNuevoIPDxAnio( string pathPrincipal , string pathOrigen, string pathDestino, int anioSeleccionado, string nombreArchivoDBFAnual, string NumeroReferencia ,  int IdReferecniaCancelacion)
        {
            bool bandera = false;
            List<IPDDTO> PagosACargar = new List<IPDDTO>();
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
                /** estructura para obtener una lista con la que se rellena elipd.dbf **/
                var transaccion = new Transaccion();
                var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);

                List<Tbl_Pagos> pagosEncontrados = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioSeleccionado && x.IdTbl_Referencias_Cancelaciones == IdReferecniaCancelacion && x.Nomina != "08" && x.Activo == true).ToList();

               
                foreach (Tbl_Pagos pago in pagosEncontrados)
                {
                    string anio = "";
                    if (DateTime.Now.Year != pago.Anio)
                    {
                        anio = Convert.ToString(pago.Anio);
                    }


                    DatosGeneralesIPD_DTO datosGeneralesEncontrados = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosGeneralesxIdNom_IPD(pago.Id_nom, anio);


                    if (datosGeneralesEncontrados != null)
                    {
                        string num5Digitos = Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(pago.NumEmpleado);
                        DatosAnIPD_DTO datosAn = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosAN_IPD(anio, datosGeneralesEncontrados.AN, num5Digitos);


                        if (datosAn != null)
                        {
                            //Obtener Percepciones y Deducciones
                            List<DatosApercepcionesIPD_DTO> percepciones = CrearReferencia_CanceladosDbSinEntity.ObtenerPercepciones_IPD(anio, datosGeneralesEncontrados.AP, num5Digitos);
                            List<DatosAdeducionesIPD_DTO> deducciones = CrearReferencia_CanceladosDbSinEntity.ObtenerDeducciones_IPD(anio, datosGeneralesEncontrados.AD, num5Digitos);

                            foreach (DatosApercepcionesIPD_DTO nuevaPercepcion in percepciones)
                            {
                                cantidadDeDatosAGuardar += 1;
                                IPDDTO nuevopPago = new IPDDTO();
                                nuevopPago.Referencia = NumeroReferencia;
                                nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                                nuevopPago.Cve_presup = datosAn.Cve_Presup;
                                nuevopPago.Cvegto = nuevaPercepcion.cvegasto;
                                nuevopPago.Cvepd = nuevaPercepcion.Cla_perc;
                                nuevopPago.Monto = nuevaPercepcion.Monto;
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
                                IPDDTO nuevopPago = new IPDDTO();
                                nuevopPago.Referencia = NumeroReferencia;
                                nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                                nuevopPago.Cve_presup = datosAn.Cve_Presup;
                                nuevopPago.Cvegto = nuevaDeduccion.Cvegasto;
                                nuevopPago.Cvepd = nuevaDeduccion.Cla_dedu;
                                nuevopPago.Monto = nuevaDeduccion.Monto;
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


                            //Couta patronal del ISSTE
                            if (datosAn.Patronal_ISSTE > 0)
                            {
                                cantidadDeDatosAGuardar += 1;
                                IPDDTO nuevopPago = new IPDDTO();
                                nuevopPago.Referencia = NumeroReferencia;
                                nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                                nuevopPago.Cve_presup = datosAn.Cve_Presup;
                                nuevopPago.Cvegto = "1411";
                                nuevopPago.Cvepd = "";
                                nuevopPago.Monto = datosAn.Patronal_ISSTE;
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
                                nuevopPago.Cvegasto = "1411";
                                nuevopPago.Cla_pto = datosAn.Cla_Pto;

                                PagosACargar.Add(nuevopPago);
                            }


                            //Couta patronal del ISSSTECAM
                            if (datosAn.Patronal_ISSSTECAM > 0)
                            {
                                cantidadDeDatosAGuardar += 1;
                                IPDDTO nuevopPago = new IPDDTO();
                                nuevopPago.Referencia = NumeroReferencia;
                                nuevopPago.TipoNom = datosGeneralesEncontrados.TipoNomina;
                                nuevopPago.Cve_presup = datosAn.Cve_Presup;
                                nuevopPago.Cvegto = "1413";
                                nuevopPago.Cvepd = "";
                                nuevopPago.Monto = datosAn.Patronal_ISSSTECAM;
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
            }


            string respuestaRellado = ActualizacionDFBS.LLenarIPD(pathDestino, nombreArchivoDBFAnual, PagosACargar);

            if (Convert.ToInt32(respuestaRellado) == cantidadDeDatosAGuardar)
            {
                bandera= true;
            }

            return bandera;
        }


        public static bool GeneralNuevoIPDCxAnio(string pathParaTxtErrores, string pathOrigen, string pathDestino, int anioSeleccionado, string nombreArchivoDBFAnual, string NumeroReferencia, int IdReferecniaCancelacion)
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
                /** estructura para obtener una lista con la que se rellena elipd.dbf **/
                var transaccion = new Transaccion();
                var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);

                List<Tbl_Pagos> pagosEncontrados = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioSeleccionado && x.IdTbl_Referencias_Cancelaciones == IdReferecniaCancelacion && x.Nomina != "08" && x.Activo == true).ToList();

                int lDF_6M = 0;
                foreach (Tbl_Pagos pago in pagosEncontrados)
                {
                    string anio = "";
                    if (DateTime.Now.Year != pago.Anio)
                    {
                        anio = Convert.ToString(pago.Anio);
                    }

                    DatosGeneralesIPD_DTO datosGeneralesEncontrados = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosGeneralesxIdNom_IPD(pago.Id_nom, anio);


                    if (datosGeneralesEncontrados != null)
                    {
                        string num5Digitos = Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(pago.NumEmpleado);
                        DatosAnIPD_DTO datosAn = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosAN_IPD(anio, datosGeneralesEncontrados.AN, num5Digitos);

                        lDF_6M = CrearReferencia_CanceladosDbSinEntity.ObtenerLDF6DxPuestoTrabajo(datosAn.Cla_Pto);


                        if (datosAn != null)
                        {
                            //Obtener Percepciones y Deducciones
                            List<DatosApercepcionesIPD_DTO> percepciones = CrearReferencia_CanceladosDbSinEntity.ObtenerPercepciones_IPD(anio, datosGeneralesEncontrados.AP, num5Digitos);
                            List<DatosAdeducionesIPD_DTO> deducciones = CrearReferencia_CanceladosDbSinEntity.ObtenerDeducciones_IPD(anio, datosGeneralesEncontrados.AD, num5Digitos);
                           // registrosCompensados = new List<IPDCDTO>();


                            //decimal saldo = percepciones.Where(x => x.Cla_perc == "01").FirstOrDefault().Monto;
                            decimal saldo = percepciones.FirstOrDefault().Monto;
                            decimal montoDDACompensar = 0M;

                            decimal saldodedu = 0M;
                            int iteradorDD = 0;
                            int iteradorPP = 0;
                            int totalDeducciones = deducciones.Count();
                            int totalPercepciones = percepciones.Count();
                      

                                DatosApercepcionesIPD_DTO pp = percepciones.Skip(iteradorPP).FirstOrDefault();
                                DatosAdeducionesIPD_DTO dedu = deducciones.Skip(iteradorDD).FirstOrDefault();

                                montoDDACompensar = saldo;

                                while (totalDeducciones > iteradorDD) 
                                {
                                    Tbl_Pagos pagoPena = null;
                                    if (dedu.Cla_dedu == "25") 
                                    {
                                        pagoPena = repoTblPagos.Obtener(x => x.Quincena == pago.Quincena && x.NumEmpleado == pago.NumEmpleado && x.EsPenA == true && x.ImporteLiquido == dedu.Monto && x.Activo == true);
                                    }

                                    if (montoDDACompensar <= saldo)
                                    {
                                        IPDCDTO nuevoRegistro = new IPDCDTO();
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
                                                numChequeConErrores.Add(pago.NumEmpleado + " con folio de Cheque :" + pago.FolioCheque );
                                            }
                                        }
                                        else
                                        {
                                            nuevoRegistro.IdctaBanca =  Convert.ToInt32(pago.Tbl_CuentasBancarias.IdctabancaIPD);
                                            nuevoRegistro.IdBanco = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdBancoIPD);
                                        }


                                        registrosCompensados.Add(nuevoRegistro);

                                        iteradorDD += 1;
                                        saldo -= dedu.Monto;  
                                       
                                        dedu = deducciones.Skip(iteradorDD).FirstOrDefault();
                                        if (dedu != null) 
                                        {
                                            montoDDACompensar = dedu.Monto;
                                        }
                                    }
                                    else 
                                    {
                                        IPDCDTO nuevoRegistro = new IPDCDTO();
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
                                        registrosCompensados.Add(nuevoRegistro);




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
                                                numChequeConErrores.Add(pago.NumEmpleado + " con folio de Cheque :" + pago.FolioCheque);
                                            }
                                        }
                                        else
                                        {
                                            nuevoRegistro.IdctaBanca = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdctabancaIPD);
                                            nuevoRegistro.IdBanco = Convert.ToInt32(pago.Tbl_CuentasBancarias.IdBancoIPD);
                                        }


                                        montoDDACompensar =  dedu.Monto - saldo;
                                        saldo =  dedu.Monto - saldo;

                                        if (saldo > 0) 
                                        {
                                            iteradorPP += 1;
                                            pp = percepciones.Skip(iteradorPP).FirstOrDefault();
                                            montoDDACompensar = saldo;
                                            dedu.Monto = montoDDACompensar;

                                            saldo = pp.Monto;
                                        }
                                    }



                                    if (saldo > 0 && totalDeducciones == iteradorDD) 
                                    {
                                        //agrega el sueldo restante al sueldo  para que aparesca 01 => 01
                                        IPDCDTO nuevoRegistro = new IPDCDTO();
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
                                        registrosCompensados.Add(nuevoRegistro);

                                        iteradorPP += 1;
                                        pp = percepciones.Skip(iteradorPP).FirstOrDefault();
                                    }




                                }
                                

                                while(totalPercepciones > iteradorPP) 
                                {
                                    //Agrega las demas percepciones al compensado 
                                    IPDCDTO nuevoRegistro = new IPDCDTO();
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
                                    registrosCompensados.Add(nuevoRegistro);

                                    iteradorPP += 1;
                                    pp = percepciones.Skip(iteradorPP).FirstOrDefault();
                                }


                       
                        }
                              
                         

                    }

                }

                /** FIn de estructura **/
            }



            ///Crear un log de errores en un TXT para informar al usuario de los posibles errores con el IPD COMPENSADO
            
            if (numChequeConErrores.Count > 0)
            {
                TextWriter Escribir = new StreamWriter(pathParaTxtErrores+"/LEER_ERRORES_IPDCompensado"+anioSeleccionado+".txt");
                foreach (string error in numChequeConErrores)
                {
                    Escribir.WriteLine("Error en IdCuentaBanca e IdBanco para el D25 del empleado : " + error);
                   
                }
                Escribir.Close();
            }
            


            string respuestaRellado =  ActualizacionDFBS.LLenarIPDCompensado(pathDestino, nombreArchivoDBFAnual, registrosCompensados);

            if (Convert.ToInt32(respuestaRellado) == registrosCompensados.Count())
            {
                bandera = true;
            }

            return bandera;
        }






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



        /***********************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************************/
        /*************************************                                        Metodos para descargar Reportes                                        *******************************************************/


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

            return pagosEncontrados.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == IdReferenciaCancel && x.Activo == true).Select(x => x.Anio).Distinct().ToList();
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

        /************************************************************************************************************************************************************************/
        /*******************************************************************    REPORTES DEL IPD    *****************************************************************************/
        /************************************************************************************************************************************************************************/
        /*************************************                       Metodos para Generar y Descargar el IPD y el IDP_Compensado                             *******************************************************/
        public static List<string> ObtenerConsultasReportesIPD(int IdReferenciaCancelacion, int Anio, string TipoReporte , string NombreReporte)
        {
            var transaccion = new Transaccion();
            var repoCatReportesCancelados = new Repositorio<cat_ReportesCCancelados>(transaccion);
            List<string> consultasObtenidas = repoCatReportesCancelados.ObtenerPorFiltro(x => x.TipoReporte == TipoReporte && x.NombreReporte == NombreReporte).Select(x => x.Consulta).ToList();

            List<string> nuevasConsultas = new List<string>();
            foreach (string query in consultasObtenidas)
            {
                string nuevoQuery = "";
                nuevoQuery = query.Replace("[NombreDB]", ObtenerConexionesDB.ObtenerNombreDBValidacionFoliosDeploy());
                nuevoQuery = nuevoQuery.Replace("[IdReferencia]", "" + IdReferenciaCancelacion + "");
                nuevoQuery = nuevoQuery.Replace("[ANIO]", "" + Anio + "");
                nuevoQuery = nuevoQuery.Replace("[PartidaCAPAE]", CrearReferencia_CanceladosDbSinEntity.ObtenerPartidaCapae(Anio));

                nuevoQuery = nuevoQuery.Replace("\r", "");
                nuevoQuery = nuevoQuery.Replace("\n", "");
                nuevoQuery = nuevoQuery.Replace("\t", "");

                nuevasConsultas.Add(nuevoQuery);
            }

            return nuevasConsultas;
        }

        public static List<RegistrosTGCxNominaDTO> ObtenerRegistrosTGCxNomina(string Consulta) 
        {
            return CrearReferencia_CanceladosDbSinEntity.ObtenerRegistrosTGCxNomina(Consulta);
        }




        public static List<TotalesGeneralesPorConceptoDTO> TotalesGeneralesXConcepto( int IdReferecniaCancelacion , int Anio, List<RegistrosTGCxNominaDTO> RegitrosNomina )
        {
            List<TotalesGeneralesPorConceptoDTO> totalesGenerales = new List<TotalesGeneralesPorConceptoDTO>();
            
            ///** estructura para obtener una lista con la que se rellena elipd.dbf **/
            //var transaccion = new Transaccion();
            //var repoCatReportesCancelados = new Repositorio<cat_ReportesCCancelados>(transaccion);
    

            //List<string> listaConsultasAExecutar = repoCatReportesCancelados.ObtenerPorFiltro(x => x.TipoReporte == "IPD" && x.NombreReporte == "TGCNomina").Select(x  => x.Consulta).ToList();

            //var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
            //List<Tbl_Pagos> pagosEncontrados = repoTblPagos.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == IdReferecniaCancelacion && x.Nomina != "08" && x.Anio == Anio && x.Activo == true).ToList();

            foreach (RegistrosTGCxNominaDTO pago in RegitrosNomina)
            {
                string anio = "";
                if (DateTime.Now.Year != pago.Anio)
                {
                    anio = Convert.ToString(pago.Anio);
                }


                string ramo = CrearReferencia_CanceladosDbSinEntity.ObtenerDescripcionRamo(pago.Partida, pago.Anio);
                DatosGeneralesIPD_DTO datosGeneralesEncontrados = CrearReferencia_CanceladosDbSinEntity.ObtenerDatosGeneralesxIdNom_IPD(pago.Id_nom, anio);
                if (datosGeneralesEncontrados != null)
                {
                    string num5Digitos = Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(pago.NumEmpleado);
                    List<DescripcionPPDD_DTO> PP = CrearReferencia_CanceladosDbSinEntity.ObtenerPercepcionesParaTotalesPorConcepto(datosGeneralesEncontrados.AP, pago.Anio, num5Digitos);
                    List<DescripcionPPDD_DTO> DD = CrearReferencia_CanceladosDbSinEntity.ObtenerDeduccionesParaTotalesPorConcepto(datosGeneralesEncontrados.AD, pago.Anio, num5Digitos);


                    foreach (DescripcionPPDD_DTO descripPP in PP)
                    {
                        TotalesGeneralesPorConceptoDTO claveEncontrada = totalesGenerales.Where(x => x.Clave == descripPP.Clave && x.RU == ramo).FirstOrDefault();

                        
                        if (claveEncontrada != null)
                        {
                            if (descripPP.Monto < 0)
                            {
                                claveEncontrada.MontoNegativo += descripPP.Monto;
                            } else if (descripPP.Monto > 0)
                            {
                                claveEncontrada.MontoPositivo += descripPP.Monto;
                            }
                        }
                        else 
                        {
                            TotalesGeneralesPorConceptoDTO nuevoConceptoEncontrado = new TotalesGeneralesPorConceptoDTO();
                            nuevoConceptoEncontrado.EsPercepcion = true;
                            nuevoConceptoEncontrado.RU = ramo;
                            nuevoConceptoEncontrado.Clave = descripPP.Clave ;
                            nuevoConceptoEncontrado.Descripcion = descripPP.Descripcion;

                            if (descripPP.Monto < 0)
                            {
                                nuevoConceptoEncontrado.MontoNegativo += descripPP.Monto;
                            }
                            else if (descripPP.Monto > 0)
                            {
                                nuevoConceptoEncontrado.MontoPositivo += descripPP.Monto;
                            }
                            totalesGenerales.Add(nuevoConceptoEncontrado);
                        }

                    }

                    foreach (DescripcionPPDD_DTO descripDD in DD)
                    {
                        TotalesGeneralesPorConceptoDTO claveEncontrada = totalesGenerales.Where(x => x.Clave == descripDD.Clave && x.RU == ramo).FirstOrDefault();
                        if (claveEncontrada != null)
                        {
                            if (descripDD.Monto < 0)
                            {
                                claveEncontrada.MontoNegativo += descripDD.Monto;
                            }
                            else if (descripDD.Monto > 0)
                            {
                                claveEncontrada.MontoPositivo += descripDD.Monto;
                            }
                        }
                        else
                        {
                            TotalesGeneralesPorConceptoDTO nuevoConceptoEncontrado = new TotalesGeneralesPorConceptoDTO();
                            nuevoConceptoEncontrado.EsPercepcion = false;
                            nuevoConceptoEncontrado.RU = ramo;
                            nuevoConceptoEncontrado.Clave = descripDD.Clave;
                            nuevoConceptoEncontrado.Descripcion = descripDD.Descripcion;

                            if (descripDD.Monto < 0)
                            {
                                nuevoConceptoEncontrado.MontoNegativo += descripDD.Monto;
                            }
                            else if (descripDD.Monto > 0)
                            {
                                nuevoConceptoEncontrado.MontoPositivo += descripDD.Monto;
                            }

                            totalesGenerales.Add(nuevoConceptoEncontrado);
                        }



                    }
                }

            }
            /** FIn de estructura **/

            return totalesGenerales;
        }


    }
}

