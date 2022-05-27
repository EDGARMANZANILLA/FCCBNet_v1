




function DibujarTablaEsFoliada() {
    $("#TablaEsFoliada").append(

        "<table id='DetallesSiEstaFoliadaNominas'    class='table table-striped table-bordered table-hover' cellspacing='0' style='width:100%'     >" +
        " <caption class='text-uppercase'>Resumen de nominas disponibles en la quincena</caption>" +
        "<thead class='tabla'>" +

        "<tr>" +
        "<th>IdNom</th>" +
        "<th>Nomina</th>" +
        "<th>Adicional</th>" +
        "<th>Nombre nomina</th>" +
        "<th>Registros a foliar</th>" +
        "<th>Esta Foliada</th>" +
        "<th>Foliar</th>" +
        "<th>Revisar PDF</th>" +
        "</tr>" +

        "</thead>" +
        "</table>"
    );
};

function PintarResultadoEsFoliada(datos) {

    $('#DetallesSiEstaFoliadaNominas').DataTable({
        "ordering": true,
        "info": true,
        "searching": true,
        "paging": true,
        "lengthMenu": [10, 15, 30],
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


            { "data": "Id_Nom" },
            { "data": "NumeroNomina" },
            { "data": "Adicional" },
            { "data": "NombreNomina" },
            { "data": "NumeroRegistrosAFoliar" },
            {
                "data": "IdEstaFoliada",
                render: function (data, type, row) {
                    // console.log("hola :" + typeof data);
                    // console.log( data);
                    //  console.log( data.row.IdAtencion);
                    if (data == 0) {
                        return '<h4 class="text-danger text-uppercase"> <i class="fas fa-times-circle"></i>  </h4>';
                    }
                    else if (data == 1) {

                        return '<h4 class="  text-success text-uppercase"> <i class="fas fa-check"></i>  </h4>';
                    } else if (data == 2) {

                        return '<h4 class=" bg-warning btn text-uppercase text-light"  > NO HAY PAGOMATICOS POR FOLIAR  </h4>';
                    } else if (data == 3) {

                        return '<h4 class=" bg-warning btn text-uppercase text-light"  > NO SE ENCONTRO LA BASE EN ALPHA (Interfaces)  </h4>';
                    } else if (data == 4) {

                        return '<h4 class=" bg-warning btn text-uppercase text-light"  > DBF SIN PERMISOS  </h4>';
                    }
                }
            },
            {
                render: function (data, type, row) {
                    //console.log(row);
                    if (row.IdEstaFoliada == 0) {
                        return '<h4 class="bg-success btn  text-uppercase text-light" onclick="FoliarNomina(' + row.Id_Nom + ')" > Foliar </h4>';
                    }
                    return '';
                }


            },
            {
                render: function (data, type, row) {
                    if (row.IdEstaFoliada < 2) {
                        return '<h4 class="bg-success btn  text-uppercase text-light"  onclick="ImprimeNomina(' + row.Id_Nom + ')"  > <i class="fas fa-print"></i> </h4>';
                    }
                    return '';
                }

            }

        ],
        "columnDefs": [


            { className: "text-center col-1", visible: true, "targets": 0, },
            { className: "text-center col-1", visible: true, "targets": 1, },
            { className: "text-center col-2", visible: true, "targets": 2, },
            { className: "text-center col-3", visible: true, "targets": 3, },
            { className: "text-center col-1", visible: true, "targets": 4, },
            { className: "text-center col-2", visible: true, "targets": 5, },
            { className: "text-center col-1", visible: true, "targets": 6, },
            { className: "text-center col-1", visible: true, "targets": 7, }


        ],

        "order": [[5, 'desc']]


    });

};







function VerificarNominaPagomatico() {


    let VerificaIdNom = document.getElementById("SeleccionarNominaFoliar").value;
    let quincenaPagomaticoNom = document.getElementById("QuincenaFoliacion").value;
   // console.log(quincenaPagomaticoNom);


    MensajeCargando();

    let EnviaIdNom = "{'IdNom': '" + VerificaIdNom + "', 'NumeroQuincena': '" + quincenaPagomaticoNom + "'}";

    $.ajax({
        url: 'Foliar/EstaFoliadaIdNominaPagomatico',
        data: EnviaIdNom,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            //console.log(response);

            if (response.RespuestaServidor == "201") {
                $('#TablaEsFoliada').empty();
                DibujarTablaEsFoliada();
                PintarResultadoEsFoliada(response.DetalleTabla);
            } else if (response.RespuestaServidor == "500") {
                MensajeErrorSweet(response.Error);
            }



            OcultarMensajeCargando();

        },
        error: function (jqXHR, textStatus) {
            MensajeErrorSweet("Ocurrio un error intente seleccionar la nomina de nuevo" + textStatus)
            //alert('Error occured: ' + textStatus);
        }

    });

}

function VerificarPagomaticoTodasNominas() {
    let quincenaPagomatico = document.getElementById("QuincenaFoliacion").value;
   // console.log(quincenaPagomatico);

    MensajeCargando();

    let EnviarQuincenaPagomatico = "{'NumeroQuincena': '" + quincenaPagomatico + "'}";

    $.ajax({
        url: 'Foliar/EstanFoliadasTodasNominaPagomatico',
        data: EnviarQuincenaPagomatico,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response.RespuestaServidor == "201") {
                $('#TablaEsFoliada').empty();
                DibujarTablaEsFoliada();
                PintarResultadoEsFoliada(response.DetalleTabla);

            } else if (response.RespuestaServidor == "500") {
                MensajeErrorSweet(response.Error);
            }
            OcultarMensajeCargando();

        }, error: function (jqXHR, textStatus) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
            OcultarMensajeCargando();
        }
    });

}




function CheckNomina() {
    let checkNom = document.getElementById('checkNomina').checked;

    if (checkNom) {
        document.getElementById('checkTodasNominas').checked = false;
        document.getElementById('DetalleTodasLasNominas').style.display = 'none';

        $('#TablaEsFoliada').empty();

        document.getElementById('SeleccionarNominaFoliar').style.display = "block";
        document.getElementById('VerificarNomina').style.display = "block";




    } else {
        document.getElementById('checkNomina').checked = false;


        document.getElementById('SeleccionarNominaFoliar').style.display = "none";
        document.getElementById('VerificarNomina').style.display = "none";
    }

    //console.log(checkNom)
}

function CheckTodasNominas() {
    let checkTodasNom = document.getElementById('checkTodasNominas').checked;

    if (checkTodasNom) {
        document.getElementById('checkNomina').checked = false;
        document.getElementById('SeleccionarNominaFoliar').style.display = "none";
        document.getElementById('VerificarNomina').style.display = "none";

        $('#TablaEsFoliada').empty();

        document.getElementById('DetalleTodasLasNominas').style.display = 'Block';

    } else {
        document.getElementById('checkTodasNominas').checked = false;

        document.getElementById('DetalleTodasLasNominas').style.display = 'none';
    }

}





function FoliarNomina(IdNom) {

    let quincenaParaFoliar = document.getElementById("QuincenaFoliacion").value;
    //console.log(quincenaParaFoliar);

    Swal.fire({
        title: '¿Estas seguro de querer foliar?',
        text: "",
        icon: 'warning',
        showCancelButton: true,
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, continuar!',
        cancelButtonText: 'No, cancelar!',
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {

            let EnviarRevicion = "{'IdNomina': '" + IdNom + "','NumeroQuincena': '" + quincenaParaFoliar + "'}";

            MensajeCargando();
            $.ajax({
                url: 'Foliar/FoliarPorIdNominaPagomatico',
                data: EnviarRevicion,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (response) {




                    if (response.bandera) {
                        $('#TablaEsFoliada').empty();
                        DibujarTablaEsFoliada();
                        PintarResultadoEsFoliada(response.resultadoServer.Data.DetalleTabla);
                        MensajeCorrectoSweet("La nomina se folio correctamente");

                    } else {
                        MensajeErrorSweet(response.DBFAbierta[0].Detalle, response.DBFAbierta[0].Solucion);
                    }

                    OcultarMensajeCargando();


                }, error: function (jqXHR, textStatus) {
                    OcultarMensajeCargando();
                    MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)

                }
            });



        } else if (result.dismiss === Swal.DismissReason.cancel) {

            MensajeInformacionSweet('Cancelado con exito', 'La base esta a salvo sin ninguna modificacion :)');

        }
    })


}

function ImprimeNomina(IdNom) {

    let quincenaParaImprimir = document.getElementById("QuincenaFoliacion").value;
    //console.log(quincenaParaImprimir);

    let EnviarRevicion = "{'IdNomina': '" + IdNom + "', 'Quincena': '" + quincenaParaImprimir + "'}";


    MensajeCargando();
    $.ajax({
        url: 'Foliar/RevisarReportePDFPagomaticoPorIdNomina',
        data: EnviarRevicion,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (response) {


            $("example").empty();
            /// PDFObject.embed(response, "#example");
            PDFObject.embed("data:application/pdf;base64," + response + " ", "#example");
            //PDFObject.embed("../Reportes/ReportesPDFSTemporales/RevicionNomina" + nominaSeleccionadaFoliar.value + ".pdf", "#example");
            $('#ModalPDFVisualizador').modal('show');

            OcultarMensajeCargando();
        }, error: function (jqXHR, textStatus) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus + " || "+ jqXHR)
            OcultarMensajeCargando();
        }
    });

   // OcultarMensajeCargando();

}

