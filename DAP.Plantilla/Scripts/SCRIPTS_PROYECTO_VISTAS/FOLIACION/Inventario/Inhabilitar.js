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


    MensajeCargando();
    axios.post('/Inventario/CrearTablaInhabilitadosOAsignacion', {
        IdCuentaBancaria: IdCuentaBancaria
    })
    .then(function (response) {

        $("#divTablaDetalleInhabilitados").empty();
        DibujarTablaDetalleInhabilitados();
        PintarTablaDetalleInhabilitados(response.data)
        OcultarMensajeCargando();
    })
    .catch(function (error) {
        MensajeInformacionSweet("No se pudo verificar la disponibilidad de folios intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema", error);
        OcultarMensajeCargando();
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
                axios.post('/Inventario/VerificarDisponibilidadFolios', {
                    IdCuentaBancaria: Idbanco,
                    FolioInicial: FInicial,
                    FolioFinal: FFinal
                })
                    .then(function (response) {

                        OcultarMensajeCargando();
                        if (response.data.length > 0) {
                            $("#divTablaFoliosInvalidos").empty();
                            DibujarTablaFoliosInvalidos();
                            PintarTablaFoliosInvalidos(response.data);
                            $('#ErrorEnFormasPago').modal('show');
                        } else
                        {

                            Swal.fire({
                                title: '¿Se inhabilitara desde el rango ' + FInicial + ' al ' + FFinal + ' las formas de pago, esta seguro de esto?',
                                text: 'Esto disminuira los folios disponibles para la cuenta bancaria actual ',
                                icon: 'warning',
                                showCancelButton: true,
                                confirmButtonColor: '#3085d6',
                                cancelButtonColor: '#d33',
                                confirmButtonText: 'Si, Inhabilitar Rango!',
                                cancelButtonText: 'No, Inhabilitar!',
                                footer: '<a href="#">Contactar al desarrollador?</a>'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    MensajeCargando();
                                    axios.post('/Inventario/InhabilitarRango', {
                                        IdCuentaBancaria: Idbanco,
                                        FolioInicial: FInicial,
                                        FolioFinal: FFinal
                                    })
                                        .then(function (response) {
                                            MensajeCorrectoConRecargaPagina("Se inhabilitaron " + response.data + " formas de pago");
                                            OcultarMensajeCargando();

                                        })
                                        .catch(function (error) {
                                            MensajeInformacionSweet("Intente de nuevo mas tarde, lamentamos la demora", error);
                                            OcultarMensajeCargando();
                                        });

                                }
                            })
                           
                        }
                })
                    .catch(function (error) {
                        MensajeInformacionSweet('No se pudo verificar la disponibilidad de folios intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema', error);
                        OcultarMensajeCargando();
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


    MensajeCargando();
    axios.post('/Inventario/ObtenerNumerosContenedores', {
        IdBanco: IdBanco ,
        OrdenSeleccionada: OrdenSeleccionada
    })
    .then(function (response) {

        $("#SeleccionContenedor").empty();

        let tamanioLista = response.data.length;
        if (tamanioLista > 0) {
            let selector = document.getElementById("SeleccionContenedor");

            for (let i = 0; i < tamanioLista; i++) {
                let opcion = document.createElement("option");
                opcion.value = response.data[i].Llave;
                opcion.text = response.data[i].Valor;
                selector.add(opcion);
            }
        }

        document.getElementById("PrimeraOpcion_Orden").style.display = "none";
        document.getElementById("MasOpciones").style.display = "block";
        OcultarMensajeCargando();
    })
        .catch(function (error) {
            MensajeInformacionSweet("No se pudo cargar los contenedores del numero de orden seleccionado intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema", error);
            OcultarMensajeCargando();
        });


}

function ValidarContenedor() {

    let numeroContenedorSelecionado = document.getElementById("SeleccionContenedor").value;


    MensajeCargando();
    axios.post('/Inventario/VerificarDisponibilidadContenedor', {
        IdContenedor: numeroContenedorSelecionado
    })
        .then(function (response) {

            OcultarMensajeCargando();
            if (response.data.bandera) {
                $("#divTablaFoliosInvalidos").empty();
                DibujarTablaFoliosInvalidos();
                PintarTablaFoliosInvalidos(response.data.TotalFoliosNoDisponibles);
                $('#ErrorEnFormasPago').modal('show');
            } else {

                Swal.fire({
                    title: '¿Se inhabilitaran todas las formas de pago del contenedor seleccionado, esta seguro de esto?',
                    text: 'Esto disminuira los folios disponibles para la cuenta bancaria actual ',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Si, Inhabilitar contenedor!',
                    cancelButtonText: 'No, cancelar!',
                    footer: '<a href="#">Contactar al desarrollador?</a>'
                }).then((result) => {
                    if (result.isConfirmed) {
                        MensajeCargando();
                        axios.post('/Inventario/InhabilitarContenedor', {
                            IdContenedor: numeroContenedorSelecionado
                        })
                            .then(function (response) {
                                MensajeCorrectoConRecargaPagina("Se inhabilitaron " + response.data + " formas de pago")
                                OcultarMensajeCargando();

                        })
                            .catch(function (error) {
                                MensajeInformacionSweet("Intente de nuevo mas tarde, lamentamos el inconveniente", error);
                                OcultarMensajeCargando();
                            });


                    }
                })
                
            }


        })
        .catch(function (error) {
            MensajeInformacionSweet("No se pudo verificar la disponibilidad de folios en el contenedor intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema", error);
            OcultarMensajeCargando();
        });




}






