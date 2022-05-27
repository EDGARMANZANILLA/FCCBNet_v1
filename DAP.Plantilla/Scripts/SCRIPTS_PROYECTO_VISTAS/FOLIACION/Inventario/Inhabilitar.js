function DibujarTablaDetalleInhabilitados() {
    $("#divTablaDetalleInhabilitados").append(

        "<table id='tblDetalleInhabilitados' class='table table-striped table-bordered table-hover' cellspacing='0' style='width:100%'>" +
        " <caption class='text-uppercase'>Detalle general de formas de pago inhabilitadas </caption>"
        + "<thead class='tabla'>" +

        "<tr>" +

        "<th>Banco </th>" +
        "<th>Cuenta</th>" +
        "<th>Num. Orden</th>" +
        "<th>Num. Contenedor</th>" +
        "<th>Folio Inicial</th>" +
        "<th>FolioFinal</th>" +
        "<th>Total del Contenedor</th>" +
        "<th>Formas Disponibles Actuales </th>" +
        "<th>Formas Inhabilitadas</th>" +
        "<th>Fecha de Alta</th>" +

        "</tr>" +
        "</thead>" +

        "</table>"
    );
};

let tablaCargados;
function PintarTablaDetalleInhabilitados(datos) {

    tablaCargados = $('#tblDetalleInhabilitados').DataTable({
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
            { "data": "FormasInhabilitadas" },
            { "data": "FechaAlta" }

        ],
        "order": [[1, 'asc']]
    })
};




function DibujarTablaFoliosInvalidos() {
    $("#divTablaFoliosInvalidos").append(

        "<table id='tblDetalleFoliosInvalidos' class='table table-striped table-bordered table-hover' cellspacing='0' style='width:100%'>" +
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

function PintarTablaFoliosInvalidos(datos) {
    $('#tblDetalleFoliosInvalidos').DataTable({
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





//Funcionalidad de los primeros botones de pinten o se despinten segun la opcion seleccionada
function InhabilitarFolios() {
    $("#divTablaSubir").empty();
    let InhabilitarFolios = document.getElementById('InhabilitarFolios').classList.remove('btn-outline-primary');
    InhabilitarFolios = document.getElementById('InhabilitarFolios').classList.add('btn-primary');

    let inhabilitarContenedor = document.getElementById('InhabilitarContenedor').classList.remove('btn-primary');
    inhabilitarContenedor = document.getElementById('InhabilitarContenedor').classList.add('btn-outline-primary');

    let seccionDetalle = document.getElementById('DetalleInhabilitado').classList.remove('btn-primary');
    seccionDetalle = document.getElementById('DetalleInhabilitado').classList.add('btn-outline-primary');


    document.getElementById("DetalleInhabilitacion").style.display = "none";
    document.getElementById("InhabilitacionContenedor").style.display = "none";


    document.getElementById("IFolios").style.display = "block";

}

function InhabilitarContenedor() {

    $("#divTablaSubir").empty();

    let inhabilitarContenedor = document.getElementById('InhabilitarContenedor').classList.remove('btn-outline-primary');
    inhabilitarContenedor = document.getElementById('InhabilitarContenedor').classList.add('btn-primary');

    let seccionDetalle = document.getElementById('DetalleInhabilitado').classList.remove('btn-primary');
    seccionDetalle = document.getElementById('DetalleInhabilitado').classList.add('btn-outline-primary');

    let InhabilitarFolios = document.getElementById('InhabilitarFolios').classList.remove('btn-primary');
    InhabilitarFolios = document.getElementById('InhabilitarFolios').classList.add('btn-outline-primary');

    document.getElementById("IFolios").style.display = "none";
    document.getElementById("DetalleInhabilitacion").style.display = "none";
    document.getElementById("InhabilitacionContenedor").style.display = "block";

    //deshabilita el boton para enviar datos al server por contenedor y solo envia los ihabilitados
}

function VerificaDetalleInhabilitado() {

    let detalle = document.getElementById('DetalleInhabilitado').classList.remove('btn-outline-primary');
    detalle = document.getElementById('DetalleInhabilitado').classList.add('btn-primary');

    let inhabilitarContenedor = document.getElementById('InhabilitarContenedor').classList.remove('btn-primary');
    inhabilitarContenedor = document.getElementById('InhabilitarContenedor').classList.add('btn-outline-primary');

    let InhabilitarFolios = document.getElementById('InhabilitarFolios').classList.remove('btn-primary');
    InhabilitarFolios = document.getElementById('InhabilitarFolios').classList.add('btn-outline-primary');


    document.getElementById("IFolios").style.display = "none";
    document.getElementById("InhabilitacionContenedor").style.display = "none";
    document.getElementById("DetalleInhabilitacion").style.display = "block";


    let IdCuentaBancaria = document.getElementById("IdCuentaBancariaInhabilitar").innerHTML;

   // console.log(IdCuentaBancaria);

    //  $("#divTablaSubir").empty();
    //  DibujarTablaDetalleInhabilitados();


    let verificarFolios = "{'IdCuentaBancaria':'" + IdCuentaBancaria + "'}";

    MensajeCargando();
    $.ajax({
        url: 'CrearTablaInhabilitadosOAsignacion',
        data: verificarFolios,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
          //  console.log(msg);
            //console.log("msg", msg.length);

            $("#divTablaDetalleInhabilitados").empty();
            DibujarTablaDetalleInhabilitados();
            PintarTablaDetalleInhabilitados(msg)
            // PintarConsultas(msg);

            OcultarMensajeCargando();

        },
        error: function (msg) {
            OcultarMensajeCargando();

            MensajeInformacionSweet("No se pudo verificar la disponibilidad de folios intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema");
        }
    });



}
//Termina funcionalidad de pintar botones segun opcion seleccionada


function regresarPrimeraOpcion()
{
    document.getElementById("MasOpciones").style.display = "none";
    document.getElementById("PrimeraOpcion_Orden").style.display = "block";
}








let FFinal = null;
let FInicial = null;
function ValidarFolios() {
    let Idbanco = document.getElementById("IdCuentaBancariaInhabilitar").innerHTML;

    FInicial = document.getElementById('FolioInicial').value;
    FFinal = document.getElementById('FolioFinal').value;

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
                            $("#divTablaFoliosInvalidos").empty();
                            DibujarTablaFoliosInvalidos();
                            PintarTablaFoliosInvalidos(msg);
                            $('#ErrorEnFormasPago').modal('show');
                        } else {
                            Swal.fire({
                                title: '¿Se inhabilitara desde el rango ' + FInicial + ' al ' + FFinal + ' las formas de pago, esta seguro de esto?',
                                showDenyButton: true,
                                showCancelButton: true,
                                confirmButtonText: 'Inhabilitar Rango',
                                denyButtonText: `No hacer nada`,
                                cancelButtonText: `Cancelar`,
                            }).then((result) => {
                                /* Read more about isConfirmed, isDenied below */
                                if (result.isConfirmed) {
                                    MensajeCargando();
                                    $.ajax({
                                        url: 'InhabilitarRango',
                                        data: verificarFolios,
                                        type: "POST",
                                        contentType: "application/json; charset=utf-8",

                                        success: function (renposehttp) {
                                            OcultarMensajeCargando();

                                            MensajeCorrectoConRecargaPagina("Se inhabilitaron " + renposehttp + " formas de pago");
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

    if (FInicial == "") {
        MensajeWarningSweet('Introduzca al menos un folio');
    }



}

function VerificarNumeroContenedores() {
    let OrdenSeleccionada = document.getElementById("SeleccionOrden").value;
    let IdBanco = document.getElementById("IdCuentaBancariaInhabilitar").innerHTML;


    let DatosEnviar = "{'IdBanco': '" + IdBanco + "' ,'OrdenSeleccionada': '" + OrdenSeleccionada + "' }";
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



            $("#SeleccionContenedor").empty();

            let tamanioLista = response.length;
            if (tamanioLista > 0) {
                let selector = document.getElementById("SeleccionContenedor");



                for (let i = 0; i < tamanioLista; i++) {
                    let opcion = document.createElement("option");
                    opcion.value = response[i].Llave;
                    opcion.text = response[i].Valor;
                    selector.add(opcion);
                }
            }

            document.getElementById("PrimeraOpcion_Orden").style.display = "none";
            document.getElementById("MasOpciones").style.display = "block";


        },
        error: function (msg) {
            MensajeInformacionSweet("No se pudo cargar los numeros de orden intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema");
        }
    });
}

function ValidarContenedor() {
    MensajeCargando();

    let numeroContenedorSelecionado = document.getElementById("SeleccionContenedor").value;

    let verificarContenedor = "{'IdContenedor':'" + numeroContenedorSelecionado + "'}";
   // console.log(verificarContenedor);

    $.ajax({
        url: 'VerificarDisponibilidadContenedor',
        data: verificarContenedor,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            OcultarMensajeCargando();
            if (response.bandera) {
                $("#divTablaFoliosInvalidos").empty();
                DibujarTablaFoliosInvalidos();
                PintarTablaFoliosInvalidos(response.TotalFoliosNoDisponibles);
                $('#ErrorEnFormasPago').modal('show');
            } else {
                Swal.fire({
                    title: '¿Se inhabilitaran todas las formas de pago del contenedor seleccionado, esta seguro de esto?',
                    showDenyButton: true,
                    showCancelButton: true,
                    confirmButtonText: 'Inhabilitar',
                    denyButtonText: `No hacer nada`,
                    cancelButtonText: `Cancelar`,
                }).then((result) => {
                    /* Read more about isConfirmed, isDenied below */
                    if (result.isConfirmed) {
                        MensajeCargando();
                        $.ajax({
                            url: 'InhabilitarContenedor',
                            data: verificarContenedor,
                            type: "POST",
                            contentType: "application/json; charset=utf-8",

                            success: function (renposehttp) {
                                OcultarMensajeCargando();

                                MensajeCorrectoConRecargaPagina("Se inhabilitaron " + renposehttp + " formas de pago")
                                //MensajeCorrectoSweet("Se inhabil22itaron " + renposehttp + " formas de pago");

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


        },
        error: function (msg) {
            OcultarMensajeCargando();
            MensajeInformacionSweet("No se pudo verificar la disponibilidad de folios intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema");
        }
    });

}






