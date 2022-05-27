
//metodos para pintar tabla detalle con ajax
function DibujarTablaDetalleAsignados() {
    $("#divTablaDetalleAsignados").append(

        "<table id='AsignarFormas' class='table table-striped table-bordered table-hover' cellspacing='0' style='width:100%'>" +
        " <caption class='text-uppercase'>Detalle general de formas de pago asignadas </caption>"
        + "<thead class='tabla'>" +

        "<tr>" +

        "<th>Banco </th>" +
        "<th>Cuenta</th>" +
        "<th>Orden</th>" +
        "<th>Contenedor</th>" +
        "<th>Folio Inicial</th>" +
        "<th>FolioFinal</th>" +
        "<th>Total del Contenedor</th>" +
        "<th>Formas Disponibles Actuales </th>" +
        "<th>Formas Asignadas</th>" +
        "<th>Fecha de Alta</th>" +

        "</tr>" +
        "</thead>" +
        "</table>"
    );
};
function PintarConsultasDetalleAsignados(datos) {

    $('#AsignarFormas').DataTable({
        "language":
        {
            "processing": "Procesando...",
            "lengthMenu": "Mostrar _MENU_ registros",
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "Ningún dato disponible en esta tabla",
            "infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "infoFiltered": "(filtrado de un total de _MAX_ registros)",
            "search": "Buscar:",
            "info": "Mostrando de _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "data": datos,
        "columns": [

            { "data": "Banco" },
            { "data": "Cuenta" },
            { "data": "Orden" },
            { "data": "Contenedor" },
            { "data": "FolioInicial" },
            { "data": "FolioFinal" },
            { "data": "FormasTotalesContenedor" },
            { "data": "FormasDisponiblesActuales" },
            { "data": "FormasAsignadas" },
            { "data": "FechaAlta" }

        ],
        "order": [[1, 'asc']]
    })
};


//Folios con problemas 
function DibujarTablaFoliosAsignados() {
    $("#divTablaFoliosAsignados").append(

        "<table id='tblDetalleFoliosAsignados' class='table table-striped table-bordered table-hover' cellspacing='0' style='width:100%'>" +
        " <caption class='text-uppercase'>Detalle de Folios Invalidos </caption>"
        + "<thead class='tabla'>" +
        "<tr>" +
        "<th>IdVirtual</th>" +
        "<th>NumFolio</th>" +
        "<th>Incidencia</th>" +
        "</tr>" +
        "</thead>" +
        "</table>"
    );
};
function PintarTablaFoliosAsignados(datos) {
    $('#tblDetalleFoliosAsignados').DataTable({
        "language":
        {
            "processing": "Procesando...",
            "lengthMenu": "Mostrar _MENU_ registros",
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "Ningún dato disponible en esta tabla",
            "infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "infoFiltered": "(filtrado de un total de _MAX_ registros)",
            "search": "Buscar:",
            "info": "Mostrando de _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "data": datos,
        "columns": [

            { "data": "IdVirtual" },
            { "data": "NumFolio" },
            { "data": "Incidencia" }

        ],
        "columnDefs": [


            { className: "text-center col-3", visible: true, "targets": 0, },
            { className: "text-center col-3", visible: true, "targets": 1, },
            { className: "text-center col-6", visible: true, "targets": 2, }

        ],
        "order": [[1, 'asc']]
    })
};



function AsignarFolios() {
    let InhabilitarFolios = document.getElementById('AsignarFolios').classList.remove('btn-outline-primary');
    InhabilitarFolios = document.getElementById('AsignarFolios').classList.add('btn-primary');

    let inhabilitarContenedor = document.getElementById('AsignarContenedor').classList.remove('btn-primary');
    inhabilitarContenedor = document.getElementById('AsignarContenedor').classList.add('btn-outline-primary');

    let seccionDetalle = document.getElementById('DetalleAsignados').classList.remove('btn-primary');
    seccionDetalle = document.getElementById('DetalleAsignados').classList.add('btn-outline-primary');

    document.getElementById("AsignacionContenedor").style.display = "none";
    document.getElementById("SeccionFoliosAsignados").style.display = "none";
    document.getElementById("IFolios").style.display = "block";


    document.getElementById("GuardarfoliosContenedorAsignado").style.display = "none";
    document.getElementById("GuardarAsignados").style.display = "block";

}

function AsignarContenedor() {
    let inhabilitarContenedor = document.getElementById('AsignarContenedor').classList.remove('btn-outline-primary');
    inhabilitarContenedor = document.getElementById('AsignarContenedor').classList.add('btn-primary');

    let seccionDetalle = document.getElementById('DetalleAsignados').classList.remove('btn-primary');
    seccionDetalle = document.getElementById('DetalleAsignados').classList.add('btn-outline-primary');

    let InhabilitarFolios = document.getElementById('AsignarFolios').classList.remove('btn-primary');
    InhabilitarFolios = document.getElementById('AsignarFolios').classList.add('btn-outline-primary');

    document.getElementById("IFolios").style.display = "none";
    document.getElementById("SeccionFoliosAsignados").style.display = "none";
    document.getElementById("AsignacionContenedor").style.display = "block";


    document.getElementById("GuardarAsignados").style.display = "none";
    document.getElementById("GuardarfoliosContenedorAsignado").style.display = "block";

}

function Detalle() {

    let detalle = document.getElementById('DetalleAsignados').classList.remove('btn-outline-primary');
    detalle = document.getElementById('DetalleAsignados').classList.add('btn-primary');

    let inhabilitarContenedor = document.getElementById('AsignarContenedor').classList.remove('btn-primary');
    inhabilitarContenedor = document.getElementById('AsignarContenedor').classList.add('btn-outline-primary');

    let InhabilitarFolios = document.getElementById('AsignarFolios').classList.remove('btn-primary');
    InhabilitarFolios = document.getElementById('AsignarFolios').classList.add('btn-outline-primary');

    document.getElementById("AsignacionContenedor").style.display = "none";
    document.getElementById("IFolios").style.display = "none";
    document.getElementById("SeccionFoliosAsignados").style.display = "block";




    let IdBanco = document.getElementById("IdCuentaBancariaAsignar").innerHTML;

    let idCuentaBancaria = "{'IdCuentaBancaria':'" + IdBanco + "'}";
    MensajeCargando();
    $.ajax({
        url: 'CrearTablaInhabilitadosOAsignacion',
        data: idCuentaBancaria,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
            // console.log("msg", msg);
            //  console.log("msg", msg.length);

            $("#divTablaDetalleAsignados").empty();
            DibujarTablaDetalleAsignados();
            PintarConsultasDetalleAsignados(msg);
            OcultarMensajeCargando();
        },
        error: function (msg) {
            OcultarMensajeCargando();
            MensajeErrorSweet('No se pudo verificar la informacion solicitada intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema');
        }
    });

}











function VerificarContenedoresAsignaciones() {
    let numPersonal = document.getElementById("SeleccionarPersonalContenedorAsignacion").value
    let numOrdenAsignacion = document.getElementById("SeleccionOrdenAsignacion").value

    if (numPersonal != "" && numOrdenAsignacion != "") {
        document.getElementById("selecionPersonalOrderAsignacion").style.display = "none";
        document.getElementById("contenedorAsignacion").style.display = "block";



        let IdBancoAsignacion = document.getElementById("IdCuentaBancariaAsignar").innerHTML;


        let DatosEnviar = "{'IdBanco': '" + IdBancoAsignacion + "' ,'OrdenSeleccionada': '" + numOrdenAsignacion + "' }";
        //console.log(DatosEnviar);

        // console.log(DatosEnviar);
        $.ajax({
            url: 'ObtenerNumerosContenedores',
            data: DatosEnviar,
            type: "POST",
            contentType: "application/json; charset=utf-8",

            success: function (response) {
                //console.log("msg", response);

                //console.log("msg", response[1].Llave);
                //console.log("msg", response[1].Valor);



                $("#SeleccionContenedorAsignacion").empty();

                let tamanioLista = response.length;
                if (tamanioLista > 0) {
                    let selector = document.getElementById("SeleccionContenedorAsignacion");




                    for (let i = 0; i < tamanioLista; i++) {
                        let opcion = document.createElement("option");
                        opcion.value = response[i].Llave;
                        opcion.text = response[i].Valor;
                        selector.add(opcion);
                    }
                }


            },
            error: function (msg) {
                MensajeInformacionSweet("No se pudo cargar los numeros de orden intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema");
            }
        });







    } else {
        MensajeInformacionSweet("Asegurese de elegir una opcion en todos los campos");
    }
}

function regresarSeleccion() {
    document.getElementById("contenedorAsignacion").style.display = "none";
    document.getElementById("selecionPersonalOrderAsignacion").style.display = "block";

}


function ValidarRangoFoliosAsignaciones() {
    let Idbanco = document.getElementById("IdCuentaBancariaAsignar").innerHTML;

    let FInicial = document.getElementById('AsignarFolioInicial').value;
    let FFinal = document.getElementById('AsignarFolioFinal').value;

    let IdNumPersonal = document.getElementById('SeleccionarPersonal').value;
    console.log(IdNumPersonal)
    if (IdNumPersonal != "") {

        if (FInicial != "") {


            if (FFinal != "") {


                if (parseInt(FFinal) > parseInt(FInicial) || parseInt(FInicial) == parseInt(FFinal)) {
                    //Caso en donde el folio Final es mayor al inicial o iguales
                    // console.log(FFinal);
                    //console.log(FInicial);
                    MensajeCargando();

                    let verificarFolios = "{'IdCuentaBancaria':'" + Idbanco + "','FolioInicial':'" + FInicial + "','FolioFinal':'" + FFinal + "'}";

                    //console.log(verificarFolios);

                    $.ajax({
                        url: 'VerificarDisponibilidadFolios',
                        data: verificarFolios,
                        type: "POST",
                        contentType: "application/json; charset=utf-8",

                        success: function (msg) {
                            //console.log("msg", msg);

                            if (msg.length > 0) {
                                $("#divTablaFoliosAsignados").empty();
                                DibujarTablaFoliosAsignados();
                                PintarTablaFoliosAsignados(msg);
                                $('#ErrorEnFormasPagoAsignacion').modal('show');
                            } else {

                                let nombrePersonal = document.getElementById('SeleccionarPersonal').options[document.getElementById('SeleccionarPersonal').selectedIndex].text;
                                // console.log(nombrePersonal)
                                Swal.fire({
                                    title: '¿Se asignara las formas de pago del rango ' + FInicial + ' al ' + FFinal + ' al empleado : ' + nombrePersonal + ' ; esta seguro de esto?',
                                    showDenyButton: true,
                                    showCancelButton: true,
                                    confirmButtonText: 'Asignar Rango',
                                    denyButtonText: `No hacer nada`,
                                    cancelButtonText: `Cancelar`,
                                }).then((result) => {
                                    /* Read more about isConfirmed, isDenied below */
                                    if (result.isConfirmed) {
                                        MensajeCargando();

                                        let asignar = "{'IdPersonal':'" + IdNumPersonal + "','IdCuentaBancaria':'" + Idbanco + "','FolioInicial':'" + FInicial + "','FolioFinal':'" + FFinal + "'}";

                                        $.ajax({
                                            url: 'AsignarRango',
                                            data: asignar,
                                            type: "POST",
                                            contentType: "application/json; charset=utf-8",
                                            success: function (renposehttp) {
                                                OcultarMensajeCargando();

                                                MensajeCorrectoConRecargaPagina("Se asignaron " + renposehttp + " formas de pago al empleado : " + nombrePersonal);
                                                // MensajeCorrectoSweet("Se inhabilitaron " + renposehttp + " formas de pago");

                                            },
                                            error: function (renposehttp) {
                                                OcultarMensajeCargando();
                                                MensajeInformacionSweet(renposehttp, "Intente de nuevo mas tarde, lamentamos la demora");
                                            }
                                        });



                                        /*Swal.fire('Saved!', '', 'success')*/
                                    } else if (result.isDenied) {

                                        MensajeInformacionSweet("No se guardara ningun cambio")
                                        /* Swal.fire('Changes are not saved', '', 'info')*/
                                    }
                                })

                            }


                            OcultarMensajeCargando();
                        },
                        error: function (msg) {
                            OcultarMensajeCargando();
                            MensajeInformacionSweet('No se pudo verificar la disponibilidad de folios intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema');
                        }
                    });

                } else {

                    MensajeWarningSweet('Introduzca un Folio Final mayor al inicial');
                }

            } else {


                MensajeWarningSweet('Introduzca un Folio Final igual o mayor al Folio Inicial');
            }

        }

    } else {
        MensajeErrorSweet("seleccione una persona para asignar las formas de pago");
    }




}



function ValidarContenedorAsignacion() {


    let numConteSelecAsignado = document.getElementById("SeleccionContenedorAsignacion").value;

    let verificarContenedor = "{'IdContenedor':'" + numConteSelecAsignado + "'}";



    MensajeCargando();
    $.ajax({
        url: 'VerificarDisponibilidadContenedor',
        data: verificarContenedor,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            OcultarMensajeCargando();
            if (response.bandera) {

                $("#divTablaFoliosAsignados").empty();
                DibujarTablaFoliosAsignados();
                PintarTablaFoliosAsignados(response);
                $('#ErrorEnFormasPagoAsignacion').modal('show');

            } else {

                let IdNumPersonalAsignar = document.getElementById('SeleccionarPersonalContenedorAsignacion');
                let nombrePersonalConte = IdNumPersonalAsignar.options[IdNumPersonalAsignar.selectedIndex].text;

                let enviarAsignacion = "{'IdPersonal':'" + IdNumPersonalAsignar.value + "', 'IdContenedor':'" + numConteSelecAsignado + "'}";

                Swal.fire({
                    title: '¿Se asignaran todas las formas de pago del contenedor al empleado : ' + nombrePersonalConte + ' , esta seguro de esto?',
                    showDenyButton: true,
                    showCancelButton: true,
                    confirmButtonText: 'Asignar',
                    denyButtonText: `No hacer nada`,
                    cancelButtonText: `Cancelar`,
                }).then((result) => {
                    /* Read more about isConfirmed, isDenied below */
                    if (result.isConfirmed) {
                        MensajeCargando();
                        $.ajax({
                            url: 'AsignarContenedor',
                            data: enviarAsignacion,
                            type: "POST",
                            contentType: "application/json; charset=utf-8",

                            success: function (renposehttp) {
                                OcultarMensajeCargando();

                                MensajeCorrectoConRecargaPagina("Se asignaron " + renposehttp + " formas de pago a la chequera a cargo del empleado " + nombrePersonalConte)

                            },
                            error: function (renposehttp) {
                                OcultarMensajeCargando();
                                MensajeInformacionSweet(renposehttp, "Intente de nuevo mas tarde, lamentamos la demora");
                            }
                        });


                    } else if (result.isDenied) {

                        MensajeInformacionSweet("No se guardara ningun cambio")
                        /* Swal.fire('Changes are not saved', '', 'info')*/
                    }
                })

            }


        },
        error: function (msg) {
            OcultarMensajeCargando();
            MensajeInformacionSweet("No se pudo verificar la disponibilidad de folios intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema");
        }
    });

}

