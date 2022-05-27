using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class SolicitudFormasPagoModel
    {
      
        public int NumeroMemo { get; set; }
        public string FechaSolicitud { get; set; }
    

    }







    //public static List<AlertasAlFolearPagomaticosDTO> FoliarChequesPorNomina(FoliarFormasPagoDTO NuevaNominaFoliar, string Observa /*, List<FoliosAFoliarInventario> chequesVerificadosFoliar  ///DESCOMENTAR PARA QUE TODO FUNCIONE*/)
    //{

    //    List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();
    //    AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();


    //    var transaccion = new Transaccion();

    //    var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

    //    Tbl_CuentasBancarias bancoEncontrado = repositorio.Obtener(x => x.Id == NuevaNominaFoliar.IdBancoPagador && x.Activo == true);

    //    var repositorioInventarioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);

    //    var repositorioContenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);

    //    var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);




    //    //Se crea una transaccion para poder actualizar o insertar en la tbl_pagos de la DB foliacion
    //    var repositorioTblPago = new Repositorio<Tbl_Pagos>(transaccion);







    //    DatosCompletosBitacoraParaChequesDTO datosCompletosObtenidos_SQL = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom_TransaccionesSQL(NuevaNominaFoliar.IdNomina);



    //    //DatosCompletosBitacoraParaChequesDTO datosCompletosObtenidos = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(NuevaNominaFoliar.IdNomina);

    //    bool EstaFoliada = false;
    //    string consultaPersonal = "";
    //    int numeroRegistrosActualizados_AlPHA = 0;
    //    int registrosInsertadosOActualizados_Foliacion = 0;

    //    int numeroQuincena = Convert.ToInt32(datosCompletosObtenidos_SQL.Quincena);
    //    List<ResumenPersonalAFoliarPorChequesDTO> resumenPersonalFoliar = null;
    //    // 1 = le pertenece a las nominas general y descentralizada
    //    // 2 = le pertenece a cualquier otra nomina que no se folea por sindicato y confianza 
    //    if (datosCompletosObtenidos_SQL.Nomina == "01" || datosCompletosObtenidos_SQL.Nomina == "02" /*NuevaNominaFoliar.GrupoNomina == 1*/)
    //    {
    //        /**********************************************************************************************************************************/
    //        /**********************************************************************************************************************************/
    //        //El grupo corresponde a las nomina de GENERAL Y DESCENTRALIZADA 
    //        //Obtener la consulta a la que corresponde la delegacion para la nomina general y descentralizada
    //        consultaPersonal = ConsultasSQLSindicatoGeneralYDesc.ObtenerConsultaSindicatoFormasDePagoGeneralYDesc(datosCompletosObtenidos_SQL.An, NuevaNominaFoliar.Delegacion, NuevaNominaFoliar.Sindicato);

    //        //ObtenerResumenDatosFormasDePagoFoliar(string ConsultaSql, int NumeroChequeInicial, string NombreBanco, bool Inhabilitado, int RangoInhabilitadoInicial, int RangoInhabilitadoFinal)
    //        resumenPersonalFoliar = FoliarConsultasDBSinEntity.ObtenerResumenDatosFormasDePagoFoliar(datosCompletosObtenidos_SQL.EsPenA, Observa, consultaPersonal, bancoEncontrado, NuevaNominaFoliar.RangoInicial, NuevaNominaFoliar.Inhabilitado, Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoInicial), Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoFinal));




    //        //ACTUALIZA EL ARCHIVO DBF EN LA RUTA ESPECIFICADA
    //        string resultadoRegitrosActualizadosDBF_Cadena = Datos.ClasesParaDBF.ActualizacionDFBS.ActualizarDBF_Sagitari_A_N(Path.GetFullPath(datosCompletosObtenidos_SQL.Ruta), datosCompletosObtenidos_SQL.RutaNomina, resumenPersonalFoliar);


    //        int a = 0;
    //        if (resultadoRegitrosActualizadosDBF_Cadena.Contains("No tiene los permisos necesarios para usar el objeto"))
    //        {
    //            nuevaAlerta.IdAtencion = 3;
    //            nuevaAlerta.SubPathRuta_Ruta = datosCompletosObtenidos_SQL.Ruta;
    //            nuevaAlerta.NombreDBF_RutaNomina = datosCompletosObtenidos_SQL.RutaNomina;
    //            Advertencias.Add(nuevaAlerta);
    //            return Advertencias;
    //        }
    //        else
    //        {
    //            int numeroRegistrosActualizados_BaseDBF = Convert.ToInt32(resultadoRegitrosActualizadosDBF_Cadena);

    //            if (resumenPersonalFoliar.Count() == numeroRegistrosActualizados_BaseDBF)
    //            {
    //                // Datos.Actua
    //                TransaccionSQL transaccionSQL = new TransaccionSQL("ConexionInterfaces");
    //                RepositorioSQL repositorioGeneral = new RepositorioSQL(transaccionSQL);
    //                transaccionSQL.IniciarTransaccion();

    //                foreach (ResumenPersonalAFoliarPorChequesDTO nuevaPersona in resumenPersonalFoliar)
    //                {
    //                    Tbl_InventarioDetalle ModificarInventarioDetalle = null;
    //                    Tbl_InventarioContenedores ModificarContenedor = null;
    //                    Tbl_Inventario ModificarInventario = null;

    //                    Tbl_Pagos nuevoPago = new Tbl_Pagos();



    //                    #region ACTUALIZA UN REGISTRO EN EL A-N DE INTERFACES ESTABBLACIDA EN BITACORA
    //                    int registroAfectado = 0;
    //                    // registroAfectado = FoliarConsultasDBSinEntity.ActualizarBaseNominaParaCheques(nuevaPersona, datosCompletosObtenidos_SQL.An, datosCompletosObtenidos_SQL.EsPenA);



    //                    string queri_ActualizaInterfaces = "UPDATE interfaces.dbo." + datosCompletosObtenidos_SQL.An + " SET Num_che = '" + nuevaPersona.NumChe + "', Banco_x = '" + nuevaPersona.BancoX + "', Cuenta_x = '" + nuevaPersona.CuentaX + "', Observa = '" + nuevaPersona.Observa + "' WHERE NUM = '" + nuevaPersona.CadenaNumEmpleado + "' and RFC = '" + nuevaPersona.RFC + "' and LIQUIDO = '" + nuevaPersona.Liquido + "' and NOMBRE = '" + nuevaPersona.Nombre + "' and DELEG = '" + nuevaPersona.Delegacion + "' ";



    //                    registroAfectado = repositorioGeneral.Ejecutar_Upadate_Delete_Insert_Transaccionado(queri_ActualizaInterfaces, transaccionSQL.GetTransaction());
    //                    #endregion

    //                    if (registroAfectado >= 1)
    //                    {
    //                        numeroRegistrosActualizados_AlPHA += registroAfectado;
    //                    }


    //                    Tbl_Pagos pagoAmodificar = null;
    //                    pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosCompletosObtenidos_SQL.Anio && x.Id_nom == datosCompletosObtenidos_SQL.Id_nom && x.IdCat_FormaPago_Pagos == 1 /*Por ser Cheque*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido);



    //                    //////// EstaFoliada = FoliarConsultasDBSinEntity.ExiteRegistroPersonaEnDBFoliacion(datosCompletosObtenidos_SQL, 1 /*Por ser cheques*/, nuevaPersona);
    //                    //Si pagoEncontrado no es null es por que ya fue foliada al menos una vez ya que existe el registro y no es necesario hacer un insert solo un Update
    //                    ///Insertar un regitro o actualizar en la DB foliacion segun se el caso 
    //                    if (pagoAmodificar != null && registroAfectado >= 1)
    //                    {
    //                        //SI ENTRA ES PORQUE YA FUE FOLIADA Y SOLO SE HARA UN UPDATE
    //                        ///// Tbl_Pagos pagoAmodificar = new Tbl_Pagos();
    //                        ////  pagoAmodificar = repositorioTblPago.Obtener(x => x.Id_nom == datosCompletosObtenidos_SQL.Id_nom && x.Quincena == numeroQuincena && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido && x.Delegacion == nuevaPersona.Delegacion);


    //                        pagoAmodificar.Id_nom = datosCompletosObtenidos_SQL.Id_nom;
    //                        pagoAmodificar.Nomina = datosCompletosObtenidos_SQL.Nomina;
    //                        pagoAmodificar.An = datosCompletosObtenidos_SQL.An;
    //                        pagoAmodificar.Adicional = datosCompletosObtenidos_SQL.Adicional;
    //                        pagoAmodificar.Anio = datosCompletosObtenidos_SQL.Anio;
    //                        pagoAmodificar.Mes = datosCompletosObtenidos_SQL.Mes;
    //                        pagoAmodificar.Quincena = numeroQuincena;
    //                        pagoAmodificar.ReferenciaBitacora = datosCompletosObtenidos_SQL.ReferenciaBitacora;
    //                        pagoAmodificar.Partida = nuevaPersona.Partida;
    //                        pagoAmodificar.Delegacion = nuevaPersona.Delegacion;
    //                        pagoAmodificar.RfcEmpleado = nuevaPersona.RFC;
    //                        pagoAmodificar.NumEmpleado = nuevaPersona.NumEmpleado;
    //                        pagoAmodificar.NombreEmpleado = nuevaPersona.Nombre;
    //                        pagoAmodificar.EsPenA = datosCompletosObtenidos_SQL.EsPenA;
    //                        pagoAmodificar.BeneficiarioPenA = null;
    //                        pagoAmodificar.NumBeneficiario = null;
    //                        pagoAmodificar.ImporteLiquido = nuevaPersona.Liquido;
    //                        pagoAmodificar.FolioCheque = nuevaPersona.NumChe;
    //                        pagoAmodificar.FolioCFDI = nuevaPersona.FolioCFDI;

    //                        pagoAmodificar.IdTbl_CuentaBancaria_BancoPagador = bancoEncontrado.Id;
    //                        pagoAmodificar.IdCat_FormaPago_Pagos = 1; //1 = cheque , 2 = Pagomatico 
    //                                                                  //1 = Transito, 2= Pagado, 3 = Precancelado , 4 No definido
    //                                                                  //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado
    //                        pagoAmodificar.IdCat_EstadoPago_Pagos = 1;


    //                        string cadenaDeIntegridad = datosCompletosObtenidos_SQL.Id_nom + " || " + datosCompletosObtenidos_SQL.Nomina + " || " + datosCompletosObtenidos_SQL.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.NumBeneficiario;
    //                        EncriptarCadena encriptar = new EncriptarCadena();
    //                        pagoAmodificar.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
    //                        pagoAmodificar.Activo = true;





    //                        //////////////////DESBLOQUEARRRRRRRRRRRRRRRRRRRRRRRRRRRRR
    //                        /// INICIA CODIGO PARA GUARDAR EL ID DEL CHEQUE COMO SE ENCUENTRA DENTRO DEL INVENTARIO
    //                        /////////// Obtiene, guarda y actualiza el inventarioDetalle del folio a usar 
    //                        /*
    //                        FoliosAFoliarInventario inventarioObtenido = chequesVerificadosFoliar.Where(x => x.Folio == nuevaPersona.NumChe).FirstOrDefault();


    //                        //el el id del numero de cheque es diferente al nuevo id de cheqe el antiguo cheque pasa a estar inhabilitado
    //                        if (pagoAmodificar.IdTbl_InventarioDetalle !=  inventarioObtenido.Id)
    //                        {
    //                            ModificarInventarioDetalle = repositorioInventarioDetalle.Obtener(x => x.Id == pagoAmodificar.IdTbl_InventarioDetalle);
    //                            ModificarInventarioDetalle.FechaIncidencia = DateTime.Now;
    //                            ModificarInventarioDetalle.IdIncidencia = 1; /// 
    //                            repositorioInventarioDetalle.Modificar(ModificarInventarioDetalle);

    //                            ModificarContenedor = repositorioContenedores.Obtener(x => x.Id == inventarioObtenido.IdContenedor);
    //                            ModificarContenedor.FormasFoliadas -= 1;
    //                            ModificarContenedor.FormasInhabilitadas += 1;
    //                            repositorioContenedores.Modificar(ModificarContenedor);



    //                        }


    //                        pagoAmodificar.IdTbl_InventarioDetalle = inventarioObtenido.Id;
    //                        //nuevoPago.IdTbl_InventarioDetalle = inventarioObtenido.Id;

    //                        if (inventarioObtenido.Id != 0)
    //                        {
    //                            ModificarInventarioDetalle = null;
    //                            ModificarContenedor = null;
    //                            ModificarInventario = null;

    //                            ModificarInventarioDetalle = repositorioInventarioDetalle.Obtener(x => x.Id == inventarioObtenido.Id);
    //                            ModificarInventarioDetalle.FechaIncidencia = DateTime.Now;
    //                            ModificarInventarioDetalle.IdIncidencia = 3;
    //                            repositorioInventarioDetalle.Modificar(ModificarInventarioDetalle);


    //                            ModificarContenedor  = repositorioContenedores.Obtener(x => x.Id == inventarioObtenido.IdContenedor);
    //                            ModificarContenedor.FormasDisponiblesActuales -= 1;
    //                            ModificarContenedor.FormasFoliadas += 1;
    //                            repositorioContenedores.Modificar(ModificarContenedor);


    //                            ModificarInventario = repositorioInventario.Obtener(x => x.Id == ModificarContenedor.IdInventario);
    //                            ModificarInventario.FormasDisponibles -= 1;
    //                            ModificarInventario.UltimoFolioUtilizado = Convert.ToString( inventarioObtenido.Folio);
    //                            repositorioInventario.Modificar(ModificarInventario);

    //                        }
    //                        /////////////////////////////////

    //                        */
    //                        /// FINALIZA CODIGO PARA GUARDAR EL ID DEL CHEQUE COMO SE ENCUENTRA DENTRO DEL INVENTARIO










    //                        Tbl_Pagos pagoModificado = repositorioTblPago.Modificar_Transaccionadamente(pagoAmodificar);

    //                        if (!string.IsNullOrEmpty(Convert.ToString(pagoModificado.FolioCheque)))
    //                        {
    //                            registrosInsertadosOActualizados_Foliacion++;
    //                            //  transaccionSQL.GuardarTransaccion();
    //                            //  transaccion.GuardarCambios();

    //                        }


    //                    }
    //                    else
    //                    {
    //                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                        //si entra es por que no esta foliada y se haran inserts 
    //                        nuevoPago.Id_nom = datosCompletosObtenidos_SQL.Id_nom;
    //                        nuevoPago.Nomina = datosCompletosObtenidos_SQL.Nomina;
    //                        nuevoPago.An = datosCompletosObtenidos_SQL.An;
    //                        nuevoPago.Adicional = datosCompletosObtenidos_SQL.Adicional;
    //                        nuevoPago.Anio = datosCompletosObtenidos_SQL.Anio;
    //                        nuevoPago.Mes = datosCompletosObtenidos_SQL.Mes;
    //                        nuevoPago.Quincena = numeroQuincena;
    //                        nuevoPago.ReferenciaBitacora = datosCompletosObtenidos_SQL.ReferenciaBitacora;
    //                        nuevoPago.Partida = nuevaPersona.Partida;
    //                        nuevoPago.Delegacion = nuevaPersona.Delegacion;
    //                        nuevoPago.RfcEmpleado = nuevaPersona.RFC;
    //                        nuevoPago.NumEmpleado = nuevaPersona.NumEmpleado;
    //                        nuevoPago.NombreEmpleado = nuevaPersona.Nombre;
    //                        nuevoPago.EsPenA = datosCompletosObtenidos_SQL.EsPenA;
    //                        nuevoPago.BeneficiarioPenA = null;
    //                        nuevoPago.NumBeneficiario = null;
    //                        nuevoPago.ImporteLiquido = nuevaPersona.Liquido;
    //                        nuevoPago.FolioCheque = nuevaPersona.NumChe;
    //                        nuevoPago.FolioCFDI = nuevaPersona.FolioCFDI;

    //                        nuevoPago.IdTbl_CuentaBancaria_BancoPagador = bancoEncontrado.Id;
    //                        nuevoPago.IdCat_FormaPago_Pagos = 1; //1 = cheque , 2 = Pagomatico 
    //                                                             //1 = Transito, 2= Pagado
    //                                                             //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado
    //                        nuevoPago.IdCat_EstadoPago_Pagos = 1;

    //                        string cadenaDeIntegridad = datosCompletosObtenidos_SQL.Id_nom + " || " + datosCompletosObtenidos_SQL.Nomina + " || " + datosCompletosObtenidos_SQL.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.NumBeneficiario;
    //                        EncriptarCadena encriptar = new EncriptarCadena();
    //                        nuevoPago.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
    //                        nuevoPago.Activo = true;



    //                        //////////////////DESBLOQUEARRRRRRRRRRRRRRRRRRRRRRRRRRRRR
    //                        /// INICIA CODIGO PARA GUARDAR EL ID DEL CHEQUE COMO SE ENCUENTRA DENTRO DEL INVENTARIO

    //                        /////////// Obtiene, guarda y actualiza el inventarioDetalle del folio a usar 
    //                        /*
    //                        FoliosAFoliarInventario inventarioObtenido = chequesVerificadosFoliar.Where(x => x.Folio == nuevaPersona.NumChe).FirstOrDefault();



    //                        nuevoPago.IdTbl_InventarioDetalle = inventarioObtenido.Id;

    //                        if (inventarioObtenido.Id != 0)
    //                        {
    //                            ModificarInventarioDetalle = repositorioInventarioDetalle.Obtener(x => x.Id == inventarioObtenido.Id);
    //                            ModificarInventarioDetalle.FechaIncidencia = DateTime.Now;
    //                            ModificarInventarioDetalle.IdIncidencia = 3;
    //                            repositorioInventarioDetalle.Modificar(ModificarInventarioDetalle);





    //                            ModificarContenedor = repositorioContenedores.Obtener(x => x.Id == inventarioObtenido.IdContenedor);
    //                            ModificarContenedor.FormasDisponiblesActuales -= 1;
    //                            ModificarContenedor.FormasFoliadas += 1;
    //                            repositorioContenedores.Modificar(ModificarContenedor);


    //                            ModificarInventario = repositorioInventario.Obtener(x => x.Id == ModificarContenedor.IdInventario);
    //                            ModificarInventario.FormasDisponibles -= 1;
    //                            ModificarInventario.UltimoFolioUtilizado = Convert.ToString(inventarioObtenido.Folio);
    //                            repositorioInventario.Modificar(ModificarInventario);

    //                        }
    //                        /////////////////////////////////
    //                        */

    //                        /// finaliza CODIGO PARA GUARDAR EL ID DEL CHEQUE COMO SE ENCUENTRA DENTRO DEL INVENTARIO


    //                        Tbl_Pagos pagoInsertado = repositorioTblPago.Agregar_Transaccionadamente(nuevoPago);


    //                        if (!string.IsNullOrEmpty(Convert.ToString(pagoInsertado.FolioCheque)))
    //                        {
    //                            registrosInsertadosOActualizados_Foliacion++;
    //                            //  transaccionSQL.GuardarTransaccion();
    //                            //  transaccion.GuardarCambios();
    //                        }

    //                    }

    //                }

    //                ///Guarda Un lote de transacciones tanto para modificar o hacer inserts  
    //                if ((registrosInsertadosOActualizados_Foliacion + numeroRegistrosActualizados_AlPHA) == (numeroRegistrosActualizados_BaseDBF * 2))
    //                {
    //                    transaccionSQL.GuardarTransaccion();
    //                    transaccion.GuardarCambios();

    //                }

    //            }
    //            else if (resumenPersonalFoliar.Count() == 0)
    //            {

    //                return Advertencias;
    //            }




    //        }



    //        //si se cumple todos los registros se actualizaron correctamente 
    //        if (numeroRegistrosActualizados_AlPHA == resumenPersonalFoliar.Count() && numeroRegistrosActualizados_AlPHA > 0 && registrosInsertadosOActualizados_Foliacion == numeroRegistrosActualizados_AlPHA)
    //        {

    //            nuevaAlerta.IdAtencion = 0;
    //            nuevaAlerta.NumeroNomina = datosCompletosObtenidos_SQL.Nomina;
    //            nuevaAlerta.NombreNomina = datosCompletosObtenidos_SQL.Coment;
    //            nuevaAlerta.Detalle = "";
    //            nuevaAlerta.Solucion = "";
    //            nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosObtenidos_SQL.Id_nom);
    //            nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados_AlPHA;
    //            nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);

    //            numeroRegistrosActualizados_AlPHA = 0;

    //            Advertencias.Add(nuevaAlerta);
    //            //return nuevaAlerta;

    //        }
    //        else if (numeroRegistrosActualizados_AlPHA != resumenPersonalFoliar.Count() && numeroRegistrosActualizados_AlPHA > 0)
    //        {
    //            //Si entra en esta condicion es por que uno o mas registros no se foliaron y se necesita refoliar toda la nomina 


    //            nuevaAlerta.IdAtencion = 2;
    //            nuevaAlerta.NumeroNomina = datosCompletosObtenidos_SQL.Nomina;
    //            nuevaAlerta.NombreNomina = datosCompletosObtenidos_SQL.Coment;
    //            nuevaAlerta.Detalle = "OCURRIO UN ERROR EN LA FOLIACION";
    //            nuevaAlerta.Solucion = "IFNN";
    //            nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosObtenidos_SQL.Id_nom);
    //            nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados_AlPHA;
    //            nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);

    //            numeroRegistrosActualizados_AlPHA = 0;

    //            Advertencias.Add(nuevaAlerta);
    //            //return nuevaAlerta;
    //        }

    //        ///////////////////////////////////////////////////////////////////}


    //    }
    //    else /*if (NuevaNominaFoliar.GrupoNomina == 2)*/
    //    {

    //        /**********************************************************************************************************************************/
    //        /**********************************************************************************************************************************/
    //        //El grupo corresponde TODAS LAS NOMINA CON EXCEPCION DEL GRUPO 1 

    //        //para las nominas que no son pension
    //        ConsultasSQLOtrasNominasConCheques crearConsultaNominasSinSindicalizados = new ConsultasSQLOtrasNominasConCheques();



    //        //OBTIENE UNA CONSULTA DEPENDIENDO DEL TIPO DE NOMINA 
    //        if (datosCompletosObtenidos_SQL.Nomina.Equals("08"))
    //        {
    //            //para las nominas que si son pension 
    //            consultaPersonal = crearConsultaNominasSinSindicalizados.ObtenerConsultaConOrdenamientoFormasDePagoPensionAlimenticiaFoliar(NuevaNominaFoliar.Delegacion, datosCompletosObtenidos_SQL.An);
    //            resumenPersonalFoliar = FoliarConsultasDBSinEntity.ObtenerResumenDatosFormasDePagoFoliar(datosCompletosObtenidos_SQL.EsPenA, Observa, consultaPersonal, bancoEncontrado, NuevaNominaFoliar.RangoInicial, NuevaNominaFoliar.Inhabilitado, Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoInicial), Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoFinal));
    //        }
    //        else
    //        {
    //            consultaPersonal = crearConsultaNominasSinSindicalizados.ObtenerConsultaConOrdenamientoFormasDePagoFoliar(NuevaNominaFoliar.Delegacion, datosCompletosObtenidos_SQL.An);
    //            resumenPersonalFoliar = FoliarConsultasDBSinEntity.ObtenerResumenDatosFormasDePagoFoliar(datosCompletosObtenidos_SQL.EsPenA, Observa, consultaPersonal, bancoEncontrado, NuevaNominaFoliar.RangoInicial, NuevaNominaFoliar.Inhabilitado, Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoInicial), Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoFinal));
    //        }




    //        //ACTUALIZA EL ARCHIVO DBF EN LA RUTA ESPECIFICADA
    //        string resultadoRegitrosActualizadosDBF_Cadena = Datos.ClasesParaDBF.ActualizacionDFBS.ActualizarDBF_Sagitari_A_N(Path.GetFullPath(datosCompletosObtenidos_SQL.Ruta), datosCompletosObtenidos_SQL.RutaNomina, resumenPersonalFoliar);


    //        int a = 0;
    //        if (resultadoRegitrosActualizadosDBF_Cadena.Contains("No tiene los permisos necesarios para usar el objeto"))
    //        {
    //            nuevaAlerta.IdAtencion = 3;
    //            nuevaAlerta.SubPathRuta_Ruta = datosCompletosObtenidos_SQL.Ruta;
    //            nuevaAlerta.NombreDBF_RutaNomina = datosCompletosObtenidos_SQL.RutaNomina;
    //            Advertencias.Add(nuevaAlerta);
    //            return Advertencias;
    //        }
    //        else
    //        {
    //            int regitrosActualizados_DBFs = Convert.ToInt32(resultadoRegitrosActualizadosDBF_Cadena);




    //            if (resumenPersonalFoliar.Count() == regitrosActualizados_DBFs)
    //            {
    //                TransaccionSQL transaccionSQL = new TransaccionSQL("ConexionInterfaces");
    //                RepositorioSQL repositorioGeneral = new RepositorioSQL(transaccionSQL);
    //                transaccionSQL.IniciarTransaccion();

    //                foreach (ResumenPersonalAFoliarPorChequesDTO nuevaPersona in resumenPersonalFoliar)
    //                {
    //                    Tbl_InventarioDetalle ModificarInventarioDetalle = null;
    //                    Tbl_InventarioContenedores ModificarContenedor = null;
    //                    Tbl_Inventario ModificarInventario = null;

    //                    Tbl_Pagos nuevoPago = new Tbl_Pagos();






    //                    #region ACTUALIZA UN REGISTRO EN EL A-N DE INTERFACES ESTABBLACIDA EN BITACORA
    //                    int registroAfectado = 0;
    //                    // registroAfectado = FoliarConsultasDBSinEntity.ActualizarBaseNominaParaCheques(nuevaPersona, datosCompletosObtenidos_SQL.An, datosCompletosObtenidos_SQL.EsPenA);

    //                    string queri_ActualizaInterfaces = "UPDATE interfaces.dbo." + datosCompletosObtenidos_SQL.An + " SET Num_che = '" + nuevaPersona.NumChe + "', Banco_x = '" + nuevaPersona.BancoX + "', Cuenta_x = '" + nuevaPersona.CuentaX + "', Observa = '" + nuevaPersona.Observa + "' WHERE NUM = '" + nuevaPersona.CadenaNumEmpleado + "' and RFC = '" + nuevaPersona.RFC + "' and LIQUIDO = '" + nuevaPersona.Liquido + "' and NOMBRE = '" + nuevaPersona.Nombre + "' and DELEG = '" + nuevaPersona.Delegacion + "' ";

    //                    registroAfectado = repositorioGeneral.Ejecutar_Upadate_Delete_Insert_Transaccionado(queri_ActualizaInterfaces, transaccionSQL.GetTransaction());
    //                    #endregion





    //                    if (registroAfectado >= 1)
    //                    {
    //                        numeroRegistrosActualizados_AlPHA += registroAfectado;
    //                    }

    //                    Tbl_Pagos pagoAmodificar = null;

    //                    if (datosCompletosObtenidos_SQL.Nomina.Equals("08"))
    //                    {
    //                        pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosCompletosObtenidos_SQL.Anio && x.Id_nom == datosCompletosObtenidos_SQL.Id_nom && x.IdCat_FormaPago_Pagos == 1 /*Por ser Cheque*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido && x.NumBeneficiario == nuevaPersona.NumBeneficiario);
    //                    }
    //                    else
    //                    {
    //                        pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosCompletosObtenidos_SQL.Anio && x.Id_nom == datosCompletosObtenidos_SQL.Id_nom && x.IdCat_FormaPago_Pagos == 1 /*Por ser Cheque*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido);
    //                    }


    //                    if (pagoAmodificar != null && registroAfectado >= 1)
    //                    {
    //                        ///ACTUALIZA UN REGITRO DENTRO DE TBL_PAGOS (Si entra es por que esta foliada y se hara solo un update)


    //                        pagoAmodificar.Id_nom = datosCompletosObtenidos_SQL.Id_nom;
    //                        pagoAmodificar.Nomina = datosCompletosObtenidos_SQL.Nomina;
    //                        pagoAmodificar.An = datosCompletosObtenidos_SQL.An;
    //                        pagoAmodificar.Adicional = datosCompletosObtenidos_SQL.Adicional;
    //                        pagoAmodificar.Anio = datosCompletosObtenidos_SQL.Anio;
    //                        pagoAmodificar.Mes = datosCompletosObtenidos_SQL.Mes;
    //                        pagoAmodificar.Quincena = numeroQuincena;
    //                        pagoAmodificar.ReferenciaBitacora = datosCompletosObtenidos_SQL.ReferenciaBitacora;
    //                        pagoAmodificar.Partida = nuevaPersona.Partida;
    //                        pagoAmodificar.Delegacion = nuevaPersona.Delegacion;
    //                        pagoAmodificar.RfcEmpleado = nuevaPersona.RFC;
    //                        pagoAmodificar.NumEmpleado = nuevaPersona.NumEmpleado;





    //                        if (datosCompletosObtenidos_SQL.EsPenA && datosCompletosObtenidos_SQL.Nomina.Equals("08"))
    //                        {
    //                            pagoAmodificar.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(nuevaPersona.CadenaNumEmpleado) /*"CAMBIADO"*/; //conectarse a funcion y buscar nombre 
    //                            pagoAmodificar.EsPenA = datosCompletosObtenidos_SQL.EsPenA;
    //                            pagoAmodificar.BeneficiarioPenA = nuevaPersona.Nombre;
    //                            pagoAmodificar.NumBeneficiario = nuevaPersona.NumBeneficiario;
    //                        }
    //                        else
    //                        {
    //                            pagoAmodificar.NombreEmpleado = nuevaPersona.Nombre;
    //                            pagoAmodificar.EsPenA = datosCompletosObtenidos_SQL.EsPenA;
    //                            pagoAmodificar.BeneficiarioPenA = null;
    //                            pagoAmodificar.NumBeneficiario = null;
    //                        }


    //                        pagoAmodificar.ImporteLiquido = nuevaPersona.Liquido;
    //                        pagoAmodificar.FolioCheque = nuevaPersona.NumChe;
    //                        pagoAmodificar.FolioCFDI = nuevaPersona.FolioCFDI;

    //                        string cadenaDeIntegridad = datosCompletosObtenidos_SQL.Id_nom + " || " + datosCompletosObtenidos_SQL.Nomina + " || " + datosCompletosObtenidos_SQL.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.NumBeneficiario;
    //                        EncriptarCadena encriptar = new EncriptarCadena();
    //                        pagoAmodificar.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);

    //                        pagoAmodificar.IdTbl_CuentaBancaria_BancoPagador = bancoEncontrado.Id;
    //                        pagoAmodificar.IdCat_FormaPago_Pagos = 1; //1 = cheque , 2 = Pagomatico 

    //                        //1 = Transito, 2= Pagado
    //                        //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado
    //                        pagoAmodificar.IdCat_EstadoPago_Pagos = 1;
    //                        pagoAmodificar.Activo = true;




    //                        /// INICIA CODIGO PARA GUARDAR EL ID DEL CHEQUE COMO SE ENCUENTRA DENTRO DEL INVENTARIO
    //                        /*
    //                        /////////// Obtiene, guarda y actualiza el inventarioDetalle del folio a usar 

    //                        FoliosAFoliarInventario inventarioObtenido = chequesVerificadosFoliar.Where(x => x.Folio == nuevaPersona.NumChe).FirstOrDefault();


    //                        //el el id del numero de cheque es diferente al nuevo id de cheqe el antiguo cheque pasa a estar inhabilitado
    //                        if (pagoAmodificar.IdTbl_InventarioDetalle != inventarioObtenido.Id)
    //                        {
    //                            ModificarInventarioDetalle = repositorioInventarioDetalle.Obtener(x => x.Id == pagoAmodificar.IdTbl_InventarioDetalle);
    //                            ModificarInventarioDetalle.FechaIncidencia = DateTime.Now;
    //                            ModificarInventarioDetalle.IdIncidencia = 1; /// 
    //                            repositorioInventarioDetalle.Modificar(ModificarInventarioDetalle);

    //                            ModificarContenedor = repositorioContenedores.Obtener(x => x.Id == inventarioObtenido.IdContenedor);
    //                            ModificarContenedor.FormasFoliadas -= 1;
    //                            ModificarContenedor.FormasInhabilitadas += 1;
    //                            repositorioContenedores.Modificar(ModificarContenedor);



    //                        }


    //                        pagoAmodificar.IdTbl_InventarioDetalle = inventarioObtenido.Id;
    //                        //nuevoPago.IdTbl_InventarioDetalle = inventarioObtenido.Id;

    //                        if (inventarioObtenido.Id != 0)
    //                        {
    //                            ModificarInventarioDetalle = null;
    //                            ModificarContenedor = null;
    //                            ModificarInventario = null;

    //                            ModificarInventarioDetalle = repositorioInventarioDetalle.Obtener(x => x.Id == inventarioObtenido.Id);
    //                            ModificarInventarioDetalle.FechaIncidencia = DateTime.Now;
    //                            ModificarInventarioDetalle.IdIncidencia = 3;
    //                            repositorioInventarioDetalle.Modificar(ModificarInventarioDetalle);


    //                            ModificarContenedor = repositorioContenedores.Obtener(x => x.Id == inventarioObtenido.IdContenedor);
    //                            ModificarContenedor.FormasDisponiblesActuales -= 1;
    //                            ModificarContenedor.FormasFoliadas += 1;
    //                            repositorioContenedores.Modificar(ModificarContenedor);


    //                            ModificarInventario = repositorioInventario.Obtener(x => x.Id == ModificarContenedor.IdInventario);
    //                            ModificarInventario.FormasDisponibles -= 1;
    //                            ModificarInventario.UltimoFolioUtilizado = Convert.ToString(inventarioObtenido.Folio);
    //                            repositorioInventario.Modificar(ModificarInventario);

    //                        }
    //                        /////////////////////////////////
    //                        */

    //                        /// FINAL CODIGO PARA GUARDAR EL ID DEL CHEQUE COMO SE ENCUENTRA DENTRO DEL INVENTARIO





    //                        Tbl_Pagos pagoModificado_OtraNomina = repositorioTblPago.Modificar_Transaccionadamente(pagoAmodificar);

    //                        if (!string.IsNullOrEmpty(Convert.ToString(pagoModificado_OtraNomina.FolioCheque)))
    //                        {
    //                            registrosInsertadosOActualizados_Foliacion++;
    //                        }



    //                    }
    //                    else
    //                    {
    //                        //REALIZA INSERCIONES A LA BASE DE DATOS FOLIACION EN LA TABLA TBL_PAGOS
    //                        //si entra es por que no esta foliada y se haran inserts 
    //                        nuevoPago.Id_nom = datosCompletosObtenidos_SQL.Id_nom;
    //                        nuevoPago.Nomina = datosCompletosObtenidos_SQL.Nomina;
    //                        nuevoPago.An = datosCompletosObtenidos_SQL.An;
    //                        nuevoPago.Adicional = datosCompletosObtenidos_SQL.Adicional;
    //                        nuevoPago.Anio = datosCompletosObtenidos_SQL.Anio;
    //                        nuevoPago.Mes = datosCompletosObtenidos_SQL.Mes;
    //                        nuevoPago.Quincena = numeroQuincena;
    //                        nuevoPago.ReferenciaBitacora = datosCompletosObtenidos_SQL.ReferenciaBitacora;
    //                        nuevoPago.Partida = nuevaPersona.Partida;
    //                        nuevoPago.Delegacion = nuevaPersona.Delegacion;
    //                        nuevoPago.RfcEmpleado = nuevaPersona.RFC;
    //                        nuevoPago.NumEmpleado = nuevaPersona.NumEmpleado;

    //                        if (datosCompletosObtenidos_SQL.EsPenA && datosCompletosObtenidos_SQL.Nomina.Equals("08"))
    //                        {
    //                            nuevoPago.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(nuevaPersona.CadenaNumEmpleado); //conectarse a funcion y buscar nombre 
    //                            nuevoPago.EsPenA = datosCompletosObtenidos_SQL.EsPenA;
    //                            nuevoPago.BeneficiarioPenA = nuevaPersona.Nombre;
    //                            nuevoPago.NumBeneficiario = nuevaPersona.NumBeneficiario;
    //                        }
    //                        else
    //                        {
    //                            nuevoPago.NombreEmpleado = nuevaPersona.Nombre;
    //                            nuevoPago.EsPenA = datosCompletosObtenidos_SQL.EsPenA;
    //                            nuevoPago.BeneficiarioPenA = null;
    //                            nuevoPago.NumBeneficiario = null;
    //                        }

    //                        nuevoPago.ImporteLiquido = nuevaPersona.Liquido;
    //                        nuevoPago.FolioCheque = nuevaPersona.NumChe;
    //                        nuevoPago.FolioCFDI = nuevaPersona.FolioCFDI;
    //                        string cadenaDeIntegridad = datosCompletosObtenidos_SQL.Id_nom + " || " + datosCompletosObtenidos_SQL.Nomina + " || " + datosCompletosObtenidos_SQL.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.NumBeneficiario;
    //                        EncriptarCadena encriptar = new EncriptarCadena();
    //                        nuevoPago.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);


    //                        nuevoPago.IdTbl_CuentaBancaria_BancoPagador = bancoEncontrado.Id;
    //                        nuevoPago.IdCat_FormaPago_Pagos = 1; //1 = cheque , 2 = Pagomatico 
    //                                                             //1 = Transito, 2= Pagado
    //                                                             //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado
    //                        nuevoPago.IdCat_EstadoPago_Pagos = 1;
    //                        nuevoPago.Activo = true;






    //                        /// INICIA CODIGO PARA GUARDAR EL ID DEL CHEQUE COMO SE ENCUENTRA DENTRO DEL INVENTARIO
    //                        /// 
    //                        /*
    //                        /////////// Obtiene, guarda y actualiza el inventarioDetalle del folio a usar 

    //                        FoliosAFoliarInventario inventarioObtenido = chequesVerificadosFoliar.Where(x => x.Folio == nuevaPersona.NumChe).FirstOrDefault();



    //                        nuevoPago.IdTbl_InventarioDetalle = inventarioObtenido.Id;

    //                        if (inventarioObtenido.Id != 0)
    //                        {
    //                            ModificarInventarioDetalle = repositorioInventarioDetalle.Obtener(x => x.Id == inventarioObtenido.Id);
    //                            ModificarInventarioDetalle.FechaIncidencia = DateTime.Now;
    //                            ModificarInventarioDetalle.IdIncidencia = 3;
    //                            repositorioInventarioDetalle.Modificar(ModificarInventarioDetalle);





    //                            ModificarContenedor = repositorioContenedores.Obtener(x => x.Id == inventarioObtenido.IdContenedor);
    //                            ModificarContenedor.FormasDisponiblesActuales -= 1;
    //                            ModificarContenedor.FormasFoliadas += 1;
    //                            repositorioContenedores.Modificar(ModificarContenedor);


    //                            ModificarInventario = repositorioInventario.Obtener(x => x.Id == ModificarContenedor.IdInventario);
    //                            ModificarInventario.FormasDisponibles -= 1;
    //                            ModificarInventario.UltimoFolioUtilizado = Convert.ToString(inventarioObtenido.Folio);
    //                            repositorioInventario.Modificar(ModificarInventario);

    //                        }
    //                        /////////////////////////////////
    //                        */

    //                        /// INICIA CODIGO PARA GUARDAR EL ID DEL CHEQUE COMO SE ENCUENTRA DENTRO DEL INVENTARIO



    //                        Tbl_Pagos pagoInsertado = repositorioTblPago.Agregar_Transaccionadamente(nuevoPago);


    //                        if (!string.IsNullOrEmpty(Convert.ToString(pagoInsertado.FolioCheque)))
    //                        {
    //                            registrosInsertadosOActualizados_Foliacion++;
    //                        }

    //                    }




    //                }

    //                if ((registrosInsertadosOActualizados_Foliacion + numeroRegistrosActualizados_AlPHA) == (regitrosActualizados_DBFs * 2))
    //                {
    //                    transaccionSQL.GuardarTransaccion();
    //                    transaccion.GuardarCambios();
    //                }

    //            }
    //        }





    //        //si se cumple todos los registros se actualizaron correctamente 
    //        if (numeroRegistrosActualizados_AlPHA == resumenPersonalFoliar.Count() && numeroRegistrosActualizados_AlPHA > 0 && registrosInsertadosOActualizados_Foliacion == numeroRegistrosActualizados_AlPHA)
    //        {

    //            nuevaAlerta.IdAtencion = 0;

    //            nuevaAlerta.NumeroNomina = datosCompletosObtenidos_SQL.Nomina;
    //            nuevaAlerta.NombreNomina = datosCompletosObtenidos_SQL.Coment;
    //            nuevaAlerta.Detalle = "";
    //            nuevaAlerta.Solucion = "";
    //            nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosObtenidos_SQL.Id_nom);
    //            nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados_AlPHA;
    //            nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);

    //            numeroRegistrosActualizados_AlPHA = 0;

    //            Advertencias.Add(nuevaAlerta);
    //            //return nuevaAlerta;

    //        }
    //        else if (numeroRegistrosActualizados_AlPHA != resumenPersonalFoliar.Count() && numeroRegistrosActualizados_AlPHA > 0)
    //        {
    //            //Si entra en esta condicion es por que uno o mas registros no se foliaron y se necesita refoliar toda la nomina 


    //            nuevaAlerta.IdAtencion = 2;

    //            nuevaAlerta.NumeroNomina = datosCompletosObtenidos_SQL.Nomina;
    //            nuevaAlerta.NombreNomina = datosCompletosObtenidos_SQL.Coment;
    //            nuevaAlerta.Detalle = "OCURRIO UN ERROR EN LA FOLIACION";
    //            nuevaAlerta.Solucion = "IFNN";
    //            nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosObtenidos_SQL.Id_nom);
    //            nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados_AlPHA;
    //            nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);

    //            numeroRegistrosActualizados_AlPHA = 0;

    //            Advertencias.Add(nuevaAlerta);
    //            //return nuevaAlerta;
    //        }


    //    }


    //    return Advertencias;
    //}



}