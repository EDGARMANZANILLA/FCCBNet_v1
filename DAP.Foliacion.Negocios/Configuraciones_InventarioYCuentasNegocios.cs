using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Objects.SqlClient;
using DAP.Foliacion.Datos;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using DAP.Foliacion.Entidades.DTO.ConfiguracionesDTO;

namespace DAP.Foliacion.Negocios
{
    public class Configuraciones_InventarioYCuentasNegocios
    {
        //Metodos para las asignaciones
        #region  Metodos el personal de asignaciones

        public static string ObtnerNombreNumEmpleado(string NumEmpleado)
        {
            return ConsultasDBSinEntity.BuscarNombreEmpleado(NumEmpleado);
        }


        public static bool GuardarNombreEmpleadoAsignaciones(string NumeroEmpleado, string NombreEmpleado, DateTime FechaHabilitacion)
        {
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            var nombreDuplicado = repositorio.Obtener(x => x.NombrePersonal.ToUpper().Trim() == NombreEmpleado.ToUpper().Trim() && x.Activo == true);

            if (nombreDuplicado == null) {
                Tbl_InventarioAsignacionPersonal nuevoPersonal = new Tbl_InventarioAsignacionPersonal();
                nuevoPersonal.IdEmpleado = Convert.ToInt32(NumeroEmpleado);
                nuevoPersonal.NombrePersonal = NombreEmpleado;
                nuevoPersonal.FechaHabilitacion = FechaHabilitacion;
                nuevoPersonal.Activo = true;


                var personalAgregado = repositorio.Agregar(nuevoPersonal);

                if (personalAgregado.Id > 0)
                    bandera = true;
            }


            return bandera;
        }





        public static List<Configuracion_EditarPersonalDTO> ObtenerPersonalActivo()
        {
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            var personalEncontrado = repositorio.ObtenerPorFiltro(x => x.Activo == true).Select(x => new { x.Id, x.NombrePersonal }).ToList();

            List<Configuracion_EditarPersonalDTO> personalAMostrar = new List<Configuracion_EditarPersonalDTO>();

            foreach (var persona in personalEncontrado)
            {
                Configuracion_EditarPersonalDTO nuevo = new Configuracion_EditarPersonalDTO();

                nuevo.Id = persona.Id;
                nuevo.NombreEmpleado = persona.NombrePersonal;

                personalAMostrar.Add(nuevo);
            }


            return personalAMostrar;
        }

        public static Tbl_InventarioAsignacionPersonal ObtenerPersonaActivaPorId(int Id)
        {
            bool bandera = false;
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            return repositorio.Obtener(x => x.Id == Id && x.Activo == true);
        }



        public static bool GuardarEdicionNombrePersonalActivo(int Id, string NombreEditado)
        {
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            Tbl_InventarioAsignacionPersonal personalEncontrado = repositorio.Obtener(x => x.Id == Id && x.Activo == true);


            personalEncontrado.NombrePersonal = NombreEditado;

            Tbl_InventarioAsignacionPersonal personalModificado = repositorio.Modificar(personalEncontrado);

            if (personalModificado.Id == Id)
                bandera = true;


            return bandera;
        }



      


        public static bool InhabilitarPersonaPorID(int Id)
        {
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            Tbl_InventarioAsignacionPersonal personalEncontrado = repositorio.Obtener(x => x.Id == Id && x.Activo == true);


            personalEncontrado.Activo = false;

            Tbl_InventarioAsignacionPersonal personalModificado = repositorio.Modificar(personalEncontrado);

            if (personalModificado.Id == Id)
                bandera = true;


            return bandera;
        }



        #endregion



        #region Metodos de Negocios para las cuentas bancarias


        public static List<String> VerificarNuevasCuentasBancarias()
        {
            List<string> cuentasEnAlpha = ConsultasDBSinEntity.ObtenerNumerosCuentasDiferentesAlpha();
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var cuentasEnAppFoliacion = repositorio.ObtenerPorFiltro(x => x.Activo == true).ToList();

            List<string> cuentasDiferentes = new List<string>();

            foreach (string cuenta in cuentasEnAlpha)
            {
                var noEncontrado = repositorio.Obtener(x => x.Cuenta.Trim() == cuenta.Trim() && x.Activo == true);
                // cuentasEnAppFoliacion.Select(y => y.Cuenta.Trim() == cuenta1.Trim());

                if (noEncontrado == null)
                {
                    cuentasDiferentes.Add(cuenta);
                }
            }

            return cuentasDiferentes;
        }


        public static List<DetallesDeCuentaDTO> ObtenerNombreCuenta(string CuentaNombre)
        {
            return ConsultasDBSinEntity.ObtenerNombresCuenta(CuentaNombre);
        }


        public static bool AgregarCuentaBancariaEInventario(string NombreCuenta, string NumeroCuenta, string Abreviatura, int TipoPago, DateTime fechaActual , bool TendraChequera)
        {
            bool bandera = false;

            var transaccion = new Transaccion();


            var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);


            if (TendraChequera)
            {

                Tbl_Inventario nuevoInventario = new Tbl_Inventario();
                nuevoInventario.FormasDisponibles = 0;
                nuevoInventario.UltimoFolioInventario = null;
                nuevoInventario.UltimoFolioUtilizado = null;
                nuevoInventario.Activo = true;

                Tbl_Inventario inventarioAgregado = repositorioInventario.Agregar(nuevoInventario);


                var repositorioCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);

                Tbl_CuentasBancarias NuevaCuenta = new Tbl_CuentasBancarias();
                NuevaCuenta.NombreBanco = NombreCuenta.ToUpper();
                NuevaCuenta.Abreviatura = Abreviatura.ToUpper();
                NuevaCuenta.Cuenta = NumeroCuenta;
                NuevaCuenta.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                NuevaCuenta.IdInventario = inventarioAgregado.Id;
                NuevaCuenta.CuentaConChequera = true;
                NuevaCuenta.FechaCreacion = fechaActual;
                NuevaCuenta.FechaBaja = null;
                NuevaCuenta.Activo = true;

                Tbl_CuentasBancarias cuentaAgregada = repositorioCuentaBancaria.Agregar(NuevaCuenta);


                if (cuentaAgregada.Id > 0)
                    bandera = true;
            }
            else
            {
 
                    var repositorioCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);

                    Tbl_CuentasBancarias NuevaCuenta = new Tbl_CuentasBancarias();
                    NuevaCuenta.NombreBanco = NombreCuenta.ToUpper();
                    NuevaCuenta.Abreviatura = Abreviatura.ToUpper();
                    NuevaCuenta.Cuenta = NumeroCuenta;
                    NuevaCuenta.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                    NuevaCuenta.IdInventario = null;
                    NuevaCuenta.FechaCreacion = fechaActual;
                    NuevaCuenta.FechaBaja = null;
                    NuevaCuenta.Activo = true;

                    Tbl_CuentasBancarias cuentaAgregada = repositorioCuentaBancaria.Agregar(NuevaCuenta);


                    if (cuentaAgregada.Id > 0)
                        bandera = true;
                
            }



            return bandera;
        }


        public static Dictionary<int, string> ObtenerCuentasBancariasActivas()
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var cuentasEncontradas = repositorio.ObtenerPorFiltro(x => x.Activo == true).Select(y => new { y.Id, y.NombreBanco, y.Cuenta , y.FechaCreacion }).ToList();

            Dictionary<int, string> cuentasActivas = new Dictionary<int, string>();
            foreach (var nuevoItem in cuentasEncontradas) 
            {
                cuentasActivas.Add(nuevoItem.Id, nuevoItem.FechaCreacion.Date.Year +" || "+ nuevoItem.NombreBanco+" || "+nuevoItem.Cuenta);
            }

            return cuentasActivas;
        }


        public static Tbl_CuentasBancarias ObtenerDetallesCuentaBancaria(int IdCuentaBancaria )
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);


            return repositorio.Obtener(x => x.Id == IdCuentaBancaria && x.Activo == true);
        }


        public static bool EditarCuentaBancariaActiva(int IdCuentaBancaria, string NombreCuenta, string Abreviatura, int TipoPago)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var bancoEncontrado = repositorio.Obtener(x => x.Id == IdCuentaBancaria && x.Activo == true);
            bool bandera = false;

            var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);

            //depende del tipo de pago que pase como parametro el usuario

            //si el pago es tarjeta 
            if (TipoPago == 1) 
            {
                //y el banco encontrado no ah tenido inventario solo se modifica los nombres 
                if (bancoEncontrado.IdInventario == null)
                {
                    //Nace como tarjeta y se quiere mover a cheque o (tarjeta y cheque)
                    if (TipoPago > 1)
                    {
                        Tbl_Inventario nuevoInventario = new Tbl_Inventario();
                        nuevoInventario.FormasDisponibles = 0;
                        nuevoInventario.UltimoFolioInventario = null;
                        nuevoInventario.Activo = true;

                        var registroInventarioAgregado = repositorioInventario.Agregar(nuevoInventario);

                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;
                        bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                        bancoEncontrado.IdInventario = registroInventarioAgregado.Id;

                        var entidadAgregada2 = repositorio.Modificar(bancoEncontrado);

                        if (entidadAgregada2.Id > 0)
                            bandera = true;

                        return bandera;
                    }


                    //nace como tarjeta y quiere seguir como tarjeta solo se modifica el nombre y su abreviatura
                    if (TipoPago == 1)
                    {
                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;
                      

                        var entidadAgregada = repositorio.Modificar(bancoEncontrado);
                        if (entidadAgregada.Id > 0)
                            bandera = true;

                        return bandera;
                    }

                }

                //y el banco ya tuvo inventario se inactiva su inventario 
                if (bancoEncontrado.IdInventario != null)
                {

                    //caso uno si que el tipo de pago se quiera cambiar a cheque, provenga de una cheque y que ya haya tenido cheques entonces
                    //se cambia el tipo de pago del banco y se habilita su inventario existente en Tbl_CuentasBancarias  / ya que decidio la misma opcion en la que estaba actualmente su inventario del banco
                    if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == 1)
                    {
                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;

                        var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);

                        if (entidadAgregadaCuentaBancaria.Id > 0)
                            bandera = true;

                        return bandera;
                    }


                    //caso uno si que el tipo de pago se quiera cambiar a tarjeta, provenga de un cheque y que ya haya tenido cheques entonces
                    //solo se cambia el (nombre del banco, su abreviatura si se requiere) y se activa su inventario
                    if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == 2)
                    {

                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;
                        bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;


                        
                        var InventarioModificar = repositorioInventario.Obtener(x => x.Id == bancoEncontrado.IdInventario);
                        InventarioModificar.Activo = false;
                        var registroInventarioAgregado = repositorioInventario.Modificar(InventarioModificar);
                        var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);
                        if (entidadAgregadaCuentaBancaria.Id > 0)
                            bandera = true;

                        return bandera;
                    }

                    //caso uno si que el tipo de pago se quiera cambiar a  tarjeta, provenga de (tarjeta y cheques opcion 3) y que ya haya tenido cheques entonces
                    //solo se cambia el nombre del banco, su abreviatura,tipo de pago en Tbl_CuentasBancarias y se habilita su inventario
                    if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == 3)
                    {
                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;
                        bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;


                        var InventarioModificar = repositorioInventario.Obtener(x => x.Id == bancoEncontrado.IdInventario);
                        InventarioModificar.Activo = false;
                        var registroInventarioAgregado = repositorioInventario.Modificar(InventarioModificar);
                        var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);


                        if (entidadAgregadaCuentaBancaria.Id > 0)
                            bandera = true;

                        return bandera;
                    }

                }

            }


            if (TipoPago == 2)
            {
                //nace como tarjeta y se quiere mover a cheque
                if (bancoEncontrado.IdInventario == null)
                {

                    Tbl_Inventario nuevoInventario = new Tbl_Inventario();
                    nuevoInventario.FormasDisponibles = 0;
                    nuevoInventario.UltimoFolioInventario = null;
                    nuevoInventario.Activo = true;

                    var registroInventarioAgregado = repositorioInventario.Agregar(nuevoInventario);

                    bancoEncontrado.NombreBanco = NombreCuenta;
                    bancoEncontrado.Abreviatura = Abreviatura;
                    bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                    bancoEncontrado.IdInventario = registroInventarioAgregado.Id;

                    var entidadAgregada = repositorio.Modificar(bancoEncontrado);

                    if (entidadAgregada.Id > 0)
                        bandera = true;

                    return bandera;
                }

                //y el banco ya tuvo inventario se inactiva su inventario 
                if (bancoEncontrado.IdInventario != null)
                {
                   
                    //caso uno si que el tipo de pago se quiera cambiar a cheque, provenga de una tarjeta y que ya haya tenido cheques entonces
                    //se cambia el tipo de pago del banco y se habilita su inventario existente en Tbl_CuentasBancarias
                    if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == 1)
                    {
                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;
                        bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;

                       //modifica el inventario
                        var InventarioModificar = repositorioInventario.Obtener(x => x.Id == bancoEncontrado.IdInventario);
                        InventarioModificar.Activo = true;
                        var registroInventarioAgregado = repositorioInventario.Modificar(InventarioModificar);
                        var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);

                        if (entidadAgregadaCuentaBancaria.Id > 0)
                            bandera = true;

                        return bandera;
                    }


                    //caso uno si que el tipo de pago se quiera cambiar a cheque, provenga de un cheque y que ya haya tenido cheques entonces
                    //solo se cambia el nombre del banco y su abreviatura en Tbl_CuentasBancarias / ya que decidio la misma opcion en la que estaba actualmente su inventario del banco
                    if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == 2)
                    {

                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;
                        //bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                        var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);
                            if (entidadAgregadaCuentaBancaria.Id > 0)
                                bandera = true;

                            return bandera;
                    }

                    //caso uno si que el tipo de pago se quiera cambiar a cheque, provenga de (tarjeta y cheques opcion 3)  y que ya haya tenido cheques entonces
                    //solo se cambia el nombre del banco, su abreviatura  y tipo de pago en Tbl_CuentasBancarias
                    if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == 3)
                    {
                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;
                        bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                        var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);
                        if (entidadAgregadaCuentaBancaria.Id > 0)
                            bandera = true;

                        return bandera;
                    }


                }

            }

            //si el tipo de pago es cheque o para tarjeta y cheque (ambos)
            if (TipoPago == 3)
            {
                //nace como tarjeta y se quiere cambiar a (cheque/ tarjeta)
                if (bancoEncontrado.IdInventario == null)
                {
                    Tbl_Inventario nuevoInventario = new Tbl_Inventario();
                    nuevoInventario.FormasDisponibles = 0;
                    nuevoInventario.UltimoFolioInventario = null;
                    nuevoInventario.Activo = true;
                    var registroInventarioAgregado = repositorioInventario.Agregar(nuevoInventario);

                    bancoEncontrado.NombreBanco = NombreCuenta;
                    bancoEncontrado.Abreviatura = Abreviatura;
                    bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                    bancoEncontrado.IdInventario = registroInventarioAgregado.Id;
                    var entidadAgregada = repositorio.Modificar(bancoEncontrado);

                    if (entidadAgregada.Id > 0)
                        bandera = true;

                    return bandera;
                }


                if (bancoEncontrado.IdInventario != null)
                {

                    //caso uno si que el tipo de pago se quiera cambiar a (tarjeta/cheque) y provenga de tarjeta  y que ya haya tenido cheques entonces
                    //se cambia el tipo de pago del banco (si es necesario nombre y abreviatura) y se habilita su inventario existente en Tbl_CuentasBancarias
                    if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == 1)
                    {
                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;
                        bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                        
                     
                        var InventarioModificar = repositorioInventario.Obtener(x => x.Id == bancoEncontrado.IdInventario);
                        InventarioModificar.Activo = true;

                        var registroInventarioAgregado = repositorioInventario.Modificar(InventarioModificar);


                        var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);
                        if (entidadAgregadaCuentaBancaria.Id > 0)
                            bandera = true;

                        return bandera;
                    }


                    //caso uno si que el tipo de pago se quiera cambiar a (tarjeta/cheque) , provenga de cheque y que ya haya tenido cheques entonces
                    //solo se cambia el nombre del banco y su abreviatura en Tbl_CuentasBancarias
                    if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == 2)
                    {

                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;
                        bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                        //bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                        var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);
                        if (entidadAgregadaCuentaBancaria.Id > 0)
                            bandera = true;

                        return bandera;
                    }

                    //caso uno si que el tipo de pago se quiera cambiar a (tarjeta y cheques opcion 3), provenga de (tarjeta y cheques opcion 3)  y que ya haya tenido cheques entonces
                    //solo se cambia el nombre del banco, su abreviatura en Tbl_CuentasBancarias / ya que decidio la misma opcion en la que estaba actualmente su inventario del banco
                    if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == 3)
                    {
                        bancoEncontrado.NombreBanco = NombreCuenta;
                        bancoEncontrado.Abreviatura = Abreviatura;
                        var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);
                        if (entidadAgregadaCuentaBancaria.Id > 0)
                            bandera = true;

                        return bandera;
                    }



                }


            }



            return bandera;
        }

        


        public static bool EliminarCuentaBancariaActiva(int IdCuentaBancaria, DateTime FechaBaja)
        {
            bool bandera = false;
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);
            var repositorioContenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repositorioDetalles = new Repositorio<Tbl_InventarioDetalle>(transaccion);

            var bancoEncontrado = repositorio.Obtener(x =>  x.Id == IdCuentaBancaria && x.Activo == true);
            bancoEncontrado.Activo = false;
            bancoEncontrado.FechaBaja = FechaBaja;
            var entidadModificada =repositorio.Modificar(bancoEncontrado);

            if (entidadModificada != null && bancoEncontrado.IdInventario != null) {
                var inventarioEncontrado = repositorioInventario.Obtener(x => x.Id == bancoEncontrado.IdInventario && x.Activo == true);
                inventarioEncontrado.Activo = false;
                var entidadInventarioModificada = repositorioInventario.Modificar(inventarioEncontrado);


                if (entidadInventarioModificada != null)
                {
                    var inventarioContenedores = repositorioContenedores.ObtenerPorFiltro(x => x.IdInventario == inventarioEncontrado.Id && x.Activo == true).ToList();

                    foreach (Tbl_InventarioContenedores contenedor in inventarioContenedores)
                    {
                        contenedor.Activo = false;
                        repositorioContenedores.Modificar(contenedor);


                        var detallesEncontrados = repositorioDetalles.ObtenerPorFiltro(x => x.IdContenedor == contenedor.Id && x.Activo == true).ToList();

                        foreach (Tbl_InventarioDetalle detalle in detallesEncontrados)
                        {
                            detalle.Activo = false;
                           var entidadDetalleModificada = repositorioDetalles.Modificar(detalle);

                            if (entidadInventarioModificada != null)
                                bandera = true;
                            
                        }


                    }

                }
               

            }
            
            
            if(entidadModificada != null) 
            {
                bandera = true;
            }
     

            return bandera;
        }

        #endregion



        #region Metodos de Negocios para las exepciones de formas de pago 

        public static List<string> ObtenerCuentasBancariasConPagoTarjetaEInventario()
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);


            return repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta == 1 && x.IdInventario != null && x.Activo == true).Select(y => y.Cuenta).ToList();
        }



        public static Tbl_CuentasBancarias ObtenerDetallesCuentaConPagoTarjetaEInventario(string NumeroCuenta)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);


            return repositorio.Obtener(x => x.Cuenta.Trim() == NumeroCuenta.Trim() && x.Activo == true);
        }


        public static Dictionary<int, string> ObtenerCuentasConchequeraActivas()
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);


            var seleccionados = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta == 2 || x.IdCuentaBancaria_TipoPagoCuenta == 3 && x.Activo == true).Select(y => new {y.IdInventario, y.NombreBanco, y.Cuenta }).ToList();

            Dictionary<int, string> cuentasFiltradas = new Dictionary<int, string>();

            foreach (var nuevaSeleccion in seleccionados) 
            {
                cuentasFiltradas.Add(Convert.ToInt32( nuevaSeleccion.IdInventario), nuevaSeleccion.NombreBanco +"+ [ "+nuevaSeleccion.Cuenta+" ] ");    
            }


            return cuentasFiltradas;
        }





        //*** VERIFICAR DISPONIBILIDAD DE FOLIOS INABILI ***//
        public static List<ResumenFoliosDesinhabilitarDTO> verificarFoliosInhabiltadosEnInventarioDetalle(int IdInventario, int FInicialInhabilitado, int FFinalInhabilitado)
        {
            List<ResumenFoliosDesinhabilitarDTO> listaFoliosVerificados = new List<ResumenFoliosDesinhabilitarDTO>();



            var transaccion = new Transaccion();

            //var repositorioTblBanco = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            //Tbl_CuentasBancarias bancoEncontrado = repositorioTblBanco.Obtener(x => x.Id == IdBanco && x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true);





            //obtiene los numero de folios como deben de encontrarse en la tabla inventarios
           // List<int> foliosEnordenRecuperados = FoliarConsultasDBSinEntity.ObtenerListaDefolios(FInicial, FFinal, Inhabilitado, InhabilitadoInicial, InhabilitadoFinal);





            //obtiene todos los contenedores activos del banco seleccionado
            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var contenedoresEncontrados = repositorio.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.FormasDisponiblesActuales > 0 && x.Activo);
            List<int> idsContenedoresEncontrados = new List<int>();

            foreach (Tbl_InventarioContenedores contenedor in contenedoresEncontrados)
            {
                idsContenedoresEncontrados.Add(contenedor.Id);
            }



            var repositorioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);


          //  foreach (int folioObtenido in foliosEnordenRecuperados)
           // {
                Tbl_InventarioDetalle folioEncontradoEnInventario = null;
                Tbl_InventarioDetalle folioFinalEncontradoEnInventario = null;

            foreach (int contenedor in idsContenedoresEncontrados)
                {
                    //busca el folio inicial
                    if (folioEncontradoEnInventario == null)
                    {
                        folioEncontradoEnInventario = repositorioDetalle.Obtener(x => x.IdContenedor == contenedor && x.NumFolio == FInicialInhabilitado && x.Activo == true) ;
                    }

                    //busca el folio final 
                    if (folioFinalEncontradoEnInventario == null)
                    {
                        folioFinalEncontradoEnInventario = repositorioDetalle.Obtener(x => x.IdContenedor == contenedor && x.NumFolio == FFinalInhabilitado && x.Activo == true);
                    }


                //si trae un id mayor o diferente a 0 es que existe el regsitro y por ende contiene datos 
            }


                if (folioEncontradoEnInventario != null && folioFinalEncontradoEnInventario != null)
                {

                int idFolioincial = folioEncontradoEnInventario.Id - 1;
                int idFolioFinal = folioFinalEncontradoEnInventario.Id + 1;

                    var foliosComletosEncontrados = repositorioDetalle.ObtenerPorFiltro(x => x.Id >= folioEncontradoEnInventario.Id && x.Id <= folioFinalEncontradoEnInventario.Id && x.Activo == true );

                    foreach (var folioEncontrado in foliosComletosEncontrados)
                      {
                        ResumenFoliosDesinhabilitarDTO nuevoFolioDesinhabilitar = new ResumenFoliosDesinhabilitarDTO();
                        nuevoFolioDesinhabilitar.Id = folioEncontrado.Id;
                        nuevoFolioDesinhabilitar.Folio = folioEncontrado.NumFolio;

                    if (folioEncontrado.IdIncidencia == 1 || folioEncontrado.IdIncidencia == 4)
                    {
                        nuevoFolioDesinhabilitar.Incidencia = folioEncontrado.Tbl_InventarioTipoIncidencia.Descrip_Incidencia.Trim();
                        nuevoFolioDesinhabilitar.Error = 0;
                    }
                    else 
                    {
                       
                        nuevoFolioDesinhabilitar.Incidencia = folioEncontrado.IdIncidencia == null ? "DISPONIBLE" : folioEncontrado.Tbl_InventarioTipoIncidencia.Descrip_Incidencia.Trim();
                        nuevoFolioDesinhabilitar.Error = 1;

                    }

                    nuevoFolioDesinhabilitar.IdContenedor = folioEncontrado.IdContenedor;

                    nuevoFolioDesinhabilitar.FechaIncidencia = folioEncontrado.FechaIncidencia?.ToString("dd/MM/yyyy");


                        listaFoliosVerificados.Add(nuevoFolioDesinhabilitar);
    
                    }
                  

                }
                else
                {
                    //entra si el folio inicial no fue encontrado en los contenedore lo que quieredecir que no hay un registro de ese folio por lo tnato no existe
                    return listaFoliosVerificados;
                }

         //   }
            return listaFoliosVerificados;
        }

       
        /**  Desinhabilita el folio inhabilitado y suma al contenedor y al inventario  **/
        public static bool desInhabilitarFolo(int IdDetalle, int IdContenedor, int Folio) 
        {
            bool bandera = false;

            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioDetalle>(transaccion);



            Tbl_InventarioDetalle registroDetalleFolioObtenido = repositorio.Obtener(x => x.Id == IdDetalle && x.IdContenedor == IdContenedor && x.NumFolio == Folio && x.Activo == true);
            registroDetalleFolioObtenido.IdIncidencia = null;
            registroDetalleFolioObtenido.FechaIncidencia = null;
            registroDetalleFolioObtenido.IdEmpleado = null;

            
            //repositorio.Modificar(registroFolioObtenido);



            var repositorioContenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            Tbl_InventarioContenedores registroContenedorObtenido = repositorioContenedores.Obtener(x => x.Id == registroDetalleFolioObtenido.IdContenedor && x.Activo == true);
            registroContenedorObtenido.FormasInhabilitadas -= 1;
            registroContenedorObtenido.FormasDisponiblesActuales += 1;

            //repositorioContenedores.Modificar(registroContenedorObtenido);




            var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);
            Tbl_Inventario registroInventarioObtenido = repositorioInventario.Obtener( x => x.Id == registroContenedorObtenido.IdInventario && x.Activo == true);
            registroInventarioObtenido.FormasDisponibles += 1 ;

            //repositorioInventario.Modificar(registroInventarioObtenido);




            if (registroDetalleFolioObtenido != null && registroContenedorObtenido != null && registroInventarioObtenido != null)
            {
                transaccion.GuardarCambios();
                return bandera = true;
            }
            else 
            {
                transaccion.Dispose();
                return bandera = false;
            }

            return bandera;
        }


        #endregion




 

    }
}
