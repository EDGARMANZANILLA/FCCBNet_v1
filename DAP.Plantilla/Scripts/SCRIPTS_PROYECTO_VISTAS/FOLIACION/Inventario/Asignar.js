
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

    MensajeCargando();
    axios.post('/Inventario/CrearTablaInhabilitadosOAsignacion', {
        IdCuentaBancaria: IdBanco
    })
    .then(function (response) {

            $("#divTablaDetalleAsignados").empty();
            DibujarTablaDetalleAsignados();
            PintarConsultasDetalleAsignados(response.data);
            OcultarMensajeCargando();

    })
        .catch(function (error) {
            MensajeErrorSweet('No se pudo verificar la informacion solicitada intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema', error);
            OcultarMensajeCargando();
    });


}











function VerificarContenedoresAsignaciones() {
    let numPersonal = document.getElementById("SeleccionarPersonalContenedorAsignacion").value
    let numOrdenAsignacion = document.getElementById("SeleccionOrdenAsignacion").value

    if (numPersonal != "" && numOrdenAsignacion != "") {
        document.getElementById("selecionPersonalOrderAsignacion").style.display = "none";
        document.getElementById("contenedorAsignacion").style.display = "block";



        let IdBancoAsignacion = document.getElementById("IdCuentaBancariaAsignar").innerHTML;


        MensajeCargando();
        axios.post('/Inventario/ObtenerNumerosContenedores', {
            IdBanco:  IdBancoAsignacion ,
            OrdenSeleccionada: numOrdenAsignacion
        })
            .then(function (response) {
               // console.log(response.data);
                $("#SeleccionContenedorAsignacion").empty();

                let tamanioLista = response.data.length;
                if (tamanioLista > 0) {
                    let selector = document.getElementById("SeleccionContenedorAsignacion");

                    for (let i = 0; i < tamanioLista; i++) {
                        let opcion = document.createElement("option");
                        opcion.value = response.data[i].Llave;
                        opcion.text = response.data[i].Valor;
                        selector.add(opcion);
                    }
                }
                OcultarMensajeCargando();
            })
            .catch(function (error) {
                MensajeInformacionSweet("No se pudo cargar los numeros de orden intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema", error);
                OcultarMensajeCargando();
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
                    axios.post('/Inventario/VerificarDisponibilidadFolios', {
                        IdCuentaBancaria: Idbanco,
                        FolioInicial: FInicial,
                        FolioFinal: FFinal
                    })
                    .then(function (response) {
                        OcultarMensajeCargando();

                        if (response.data.length > 0) {
                            $("#divTablaFoliosAsignados").empty();
                            DibujarTablaFoliosAsignados();
                            PintarTablaFoliosAsignados(response.data);
                            $('#ErrorEnFormasPagoAsignacion').modal('show');

                        }
                        else
                        {

                            let nombrePersonal = document.getElementById('SeleccionarPersonal').options[document.getElementById('SeleccionarPersonal').selectedIndex].text;

                            Swal.fire({
                                title: '¿Se asignara las formas de pago del rango ' + FInicial + ' al ' + FFinal + ' al empleado : ' + nombrePersonal + ' ; esta seguro de esto?',
                                text: 'Dichos folios ya no se contemplaran en el inventario',
                                icon: 'warning',
                                showCancelButton: true,
                                confirmButtonColor: '#3085d6',
                                cancelButtonColor: '#d33',
                                confirmButtonText: 'Si, Asignar!',
                                cancelButtonText: 'No, Asignar!',
                                footer: '<a href="#">Contactar al desarrollador?</a>'
                            }).then((result) => {
                                if (result.isConfirmed) {

                                    MensajeCargando();
                                    axios.post('/Inventario/AsignarRango', {
                                        IdPersonal: IdNumPersonal,
                                        IdCuentaBancaria: Idbanco,
                                        FolioInicial: FInicial ,
                                        FolioFinal: FFinal
                                    })
                                        .then(function (response2) {

                                            MensajeCorrectoConRecargaPagina("Se asignaron " + response2.data + " formas de pago al empleado : " + nombrePersonal);
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
                        OcultarMensajeCargando();
                        MensajeInformacionSweet('No se pudo verificar la disponibilidad de folios intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema', error);
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


    MensajeCargando();
    axios.post('/Inventario/VerificarDisponibilidadContenedor', {
        IdContenedor: numConteSelecAsignado
    })
    .then(function (response) {


        OcultarMensajeCargando();
        if (response.data.bandera) {

            $("#divTablaFoliosAsignados").empty();
            DibujarTablaFoliosAsignados();
            PintarTablaFoliosAsignados(response.data.TotalFoliosNoDisponibles);
            $('#ErrorEnFormasPagoAsignacion').modal('show');

        } else {

            let IdNumPersonalAsignar = document.getElementById('SeleccionarPersonalContenedorAsignacion');
            let nombrePersonalConte = IdNumPersonalAsignar.options[IdNumPersonalAsignar.selectedIndex].text;

            
            Swal.fire({
                title: '¿Se asignaran todas las formas de pago del contenedor al empleado : ' + nombrePersonalConte + ' , esta seguro de esto?',
                text: 'Esto realizara cambios al stock disponible de cheques',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Si, Continuar!',
                cancelButtonText: 'No, cancelar!',
                footer: '<a href="#">Contactar al desarrollador?</a>'
            }).then((result) => {
                if (result.isConfirmed) {
                    MensajeCargando();
                    axios.post('/Inventario/AsignarContenedor', {
                        IdPersonal: IdNumPersonalAsignar.value,
                        IdContenedor: numConteSelecAsignado
                    })
                        .then(function (response) {
                            MensajeCorrectoConRecargaPagina("Se asignaron " + response.data + " formas de pago a la chequera a cargo del empleado " + nombrePersonalConte);
                            OcultarMensajeCargando();

                        })
                        .catch(function (error) {
                            OcultarMensajeCargando();
                            MensajeErrorSweet("Intente de nuevo mas tarde, lamentamos la demora", error);

                        });

                }
            })

        }


    })
    .catch(function (error) {
            OcultarMensajeCargando();
            MensajeInformacionSweet("No se pudo verificar la disponibilidad de folios en el contenedor, intentelo de nuevo mas tarde o pongase en contacto con el administrador del sistema" , error );
    });






}

