



function DibujarTablaResumenInventario() {
    $("#TablaResumenInventario").append(

        "<table id='NuevaTablaInventarioActualizada'  class='table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Formas de pago disponible</caption>"
        + "<thead class='tabla'>" +

        "<tr class='text-center text-uppercase'>" +

        "<th>Banco</th>" +
        "<th>Cuenta</th>" +
        "<th>Formas de pago disponibles</th>" +
        "<th>Ultimo Folio Utilizado</th>" +

        "</tr>" +
        "</thead>" +
        "</table>"
    );
};


function PintarConsultaActualizaTablaResumenInventario(datos) {



    $('#NuevaTablaInventarioActualizada').DataTable({
        "ordering": true,
        "info": false,
        "searching": false,
        "paging": false,
        "lengthMenu": [5, 10],
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
            { "data": "NombreBanco" },
            { "data": "Cuenta" },
            { "data": "FormasDisponiblesInventario" },
            { "data": "UltimoFolioUtilizadoInventario" }

        ],
        "order": [[2, 'desc']]

    });

};







function DibujarTablaAjax() {
    $("#TablaActualizada").append(

        "<table id='NominasFoliadas' class='table table-striped table-bordered table-hover' cellspacing='0' style='width:60%'>" +
        " <caption class='text-uppercase'>Nominas Foliadas </caption>"
        + "<thead class='tabla'>" +

        "<tr>" +

        "<th>Nomina foliada</th>" +
        "<th>Revisar</th>" +
        "</tr>" +
        "</thead>" +

        "</table>"
    );
};

let tablaNominasFoliadas;
function PintarConsultas(datos) {

    tablaNominasFoliadas = $('#NominasFoliadas').DataTable({
        "ordering": false,
        "info": false,
        "searching": false,
        "paging": false,
        "lengthMenu": [10, 15],
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

            { "data": "NombreNomina" },
            {

                "render": function (data, type, row) {
                    //let da = row;
                    return '<button class="mostrarPDF btn btn-success" >Ver detalles </button>';
                }

            }


        ],
        "order": [[1, 'asc']]
    });

};


function ActualizaTablaResumenFoliar() {
    var datoEnviar = "{'Dato':' '}";

    MensajeCargando();


    axios.post('/Foliar/ActualizarTablaResumenBanco', {
        Dato:''
    })
        .then(function (response) {
            $("#TablaResumenInventario").empty();
            DibujarTablaResumenInventario();
            PintarConsultaActualizaTablaResumenInventario(response.data);

            OcultarMensajeCargando();
        })
        .catch(function (error) {
            MensajeErrorSweet('', "Error al cargar la tabla ResumenInventario" + error);
            OcultarMensajeCargando();
            
        });


 

}



/*****************************************************************************************************************************************/
/***********************    TABLA PARA VERIFICAR SI ESTAN CORRECTAMENTE FOLIADAS LAS NOMINAS        **************************************/
/*****************************************************************************************************************************************/
function DibujarTablaVerificarNominaQuincena() {
    $("#TblVerificacionNominasQuincena").append(

        "<table id='VerificacionNominasQuincenaFoliada'  class='table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>RESUMEN DE VERIFICACION </caption>"
        + "<thead class='tabla'>" +

        "<tr class='text-center text-uppercase'>" +

        "<th>No.</th>" +
        "<th>Id_Nom</th>" +
        "<th>Comentario</th>" +
        "<th>Import.</th>" +
        "<th>Reg. Total</th>" +
        "<th>No Foliados</th>" +
        "<th>Cheques</th>" +
        "<th>Pagomaticos</th>" +
        "<th>Foliacion Correcta</th>" +

        "</tr>" +
        "</thead>" +
        "</table>"
    );
};


function PintarResumenVerificacionNominasQuincena(datos, quincena) {

    $('#VerificacionNominasQuincenaFoliada').DataTable({
        "dom": 'Blfrtip',
        "buttons": [
            {
                extend: 'excel',
                className: 'btn btn-primary ',
                text: '<i class="fas fa-file-download"></i>  &nbsp  EXCEL',
                filename: `Detalle_Nominas_EXEL`,
                title: `VERIFICACION DE FOLIACION, QUINCENA NO : ${quincena} `
            },
            {
                extend: 'pdf',
                className: 'btn btn-primary ',
                text: ' <i class="fas fa-download"></i>  &nbsp PDF ',
                filename: `Detalle_Nominas_PDF`,
                title: `VERIFICACION DE FOLIACION, QUINCENA NO : ${quincena} `
            }
        ],
        "bDestroy": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "paging": true,
        "lengthMenu": [15, 20, 40],
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
            { "data": "Id" },
            { "data": "Id_Nom" },
            { "data": "Comentario" },
            {
                data: "EstaImportado",
                render: function (data, type, row) {
                    //booleano que dice si estan todos los registros foliados por delegacion o No
                    if (data) {
                        return '<h4 class="  text-success text-uppercase"> SI </h4>';
                    } else {
                        return '<button class=" btn-danger btn text-uppercase text-light"  onclick="BaseNoImportada()" > Base no importada </button>';
                    }
                }

            },
            { "data": "TotalRegistros" },
            { "data": "RegistrosNoFoliados" },
            { "data": "Cheques" },
            { "data": "Pagomaticos" },
            {
                data: "EstaFoliadoCorrectamente",
                render: function (data, type, row) {
                    //booleano que dice si estan todos los registros foliados por delegacion o No
                    if (data) {
                        return '<h4 class="  text-success text-uppercase"> <i class="fas fa-check"></i>  </h4>';
                    }
                    return '<h4 class=" text-danger   text-uppercase "> <i class="fas fa-times-circle"></i>  </h4>';
                }

            }

        ],
        "order": [[0, 'asc']]

    });

};


function BaseNoImportada() {
    MensajeErrorSweet('Informe al personal encargado de subir las bases ', 'NO SE IMPORTO LA BASE DBF HACIA SQL');
}

/************************************************************************************/
/************************************************************************************/


/*******************************/
//Metodos Modificados
function RetornarVistaParcial() {

    let quincena = document.getElementById("QuincenaFoliacion").value;
    let tipoFoliacion = document.getElementById("SeleccionarTipoFoliacion").value;
 

    if (tipoFoliacion != "" && quincena != "") {
        //devuelve la vista parcial de pagomatico para el nuevo render
        if (tipoFoliacion == 1) {
            MensajeCargando();
            axios.post('/Foliar/FoliarXPagomatico', {
                NumeroQuincena: quincena
            })
                .then(function (response) {


                    if (response.data.RespuestaServidor === "500") {
                        MensajeErrorSweet(response.data.MensajeError);
                        $('#RenderVistaParcial').html('');
                    } else {
                        $('#RenderVistaParcial').html('');
                        $('#RenderVistaParcial').html(response.data);
                        document.getElementById("SeleccionVistas").style.display = "none";
                        document.getElementById("VistaPagomatico").style.display = "block";

                    }

                    OcultarMensajeCargando();
                })
                .catch(function (error) {
                    MensajeErrorSweet('', "Error en vista parcial pagomatico " + error);
                    OcultarMensajeCargando();
                });





        } else if (tipoFoliacion == 2) {


            MensajeCargando();
            axios.post('/Foliar/FoliarXFormasPago', {
                Quincena: quincena
            })
                .then(function (response) {


                    if (response.data.RespuestaServidor === "500") {
                        MensajeErrorSweet(response.data.MensajeError);
                        $('#RenderVistaParcial').html('');
                    } else {
                        $('#RenderVistaParcial').html('');
                        $('#RenderVistaParcial').html(response.data);
                        document.getElementById("SeleccionVistas").style.display = "none";
                        document.getElementById("VistaFormasPago").style.display = "block";
                    }


                    OcultarMensajeCargando();
                })
                .catch(function (error) {
                    MensajeErrorSweet('', "Error en vista parcial" + error)
                    OcultarMensajeCargando();

                });


        } else if (tipoFoliacion == 3)
        {
            
            MensajeCargando();
            axios.post('/Foliar/VerificarNominasEstanFoliadasEnQuincena', {
                Quincena: quincena
            })
                .then(function (response) {

                    $('#TblVerificacionNominasQuincena').empty();
                    DibujarTablaVerificarNominaQuincena();
                    PintarResumenVerificacionNominasQuincena(response.data, quincena);
                    document.getElementById("NombreCabezeraModalVerificacion").innerText = 'Resultado de la verificacion de nominas de la quincena ' + quincena;
                    $('#VerificarNominasQuincena').modal('show');
                    //console.log(response.data);


                    OcultarMensajeCargando();
                })
                .catch(function (error) {
                    MensajeErrorSweet('', "Error en vista parcial" + error)
                    OcultarMensajeCargando();

                });


      
        }

     



    } else {

        MensajeInformacionSweet("Asegurese de ingresar un numero de quincena y seleccionar una opcion para continuar");

    }


}




function RegresarIndexFoliacionDesdePagomatico() {
    document.getElementById("VistaPagomatico").style.display = "none";
    document.getElementById("SeleccionVistas").style.display = "block";
}



function RegresarIndexFoliacionDesdeFormaPago() {
    document.getElementById("VistaFormasPago").style.display = "none";
    document.getElementById("SeleccionVistas").style.display = "block";
}










$(document).ready(function () {

    ActualizaTablaResumenFoliar();

});

