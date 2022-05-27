





function DibujarTablaRegistros() {
    $("#TablaRegistros").append(

        "<table id='RegistrosEncontrados'  class='table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Registros Encontrados</caption>"
        + "<thead class='tabla'>" +

        "<tr class='text-center text-uppercase'>" +


        "<th>Id_Nom</th>" +
        "<th>Referencia Bitacora</th>" +
        "<th>Quincena</th>" +
        "<th>Num Empleado</th>" +
        "<th>Nombre Beneficiaro</th>" +
        "<th>Num Benef</th>" +
        "<th>Folio Cheque</th>" +
        "<th>Liquido</th>" +
        "<th>Estado Cheque</th>" +
        "<th>Tipo Pago</th>" +
        "<th> </th>" +

        "</tr>" +
        "</thead>" +
        "</table>"
    );
};

let RegistrosTabla;
function PintarRegistrosEncontrados(datos) {



    RegistrosTabla = $('#RegistrosEncontrados').DataTable({
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

            { "data": "Id_nom" },
            { "data": "ReferenciaBitacora" },
            { "data": "Quincena" },
            { "data": "NumEmpleado" },
            { "data": "NombreBeneficiaro" },
            { "data": "NumBene" },
            { "data": "FolioCheque" },
            { "data": "Liquido" },
            { "data": "EstadoCheque" },
            { "data": "TipoPago" },
            { "data": "IdRegistro" }
        ],
        "columnDefs": [

            { className: "text-center col-1", visible: true, "targets": 0, },
            { className: "text-center col-1", visible: true, "targets": 1, },
            { className: "text-center col-1", visible: true, "targets": 2, },
            { className: "text-center col-1", visible: true, "targets": 3, },
            { className: "text-center col-2", visible: true, "targets": 4, },
            { className: "text-center col-1", visible: true, "targets": 5, },
            { className: "text-center col-1", visible: true, "targets": 6, },
            { className: "text-center col-1", visible: true, "targets": 7, },
            { className: "text-center col-1", visible: true, "targets": 8, },
            { className: "text-center col-1", visible: true, "targets": 9, },
            {
                className: "text-center col-1",
                visible: true,
                "targets": [10],
                render: function (data, type, row) {
                    //console.log(typeof data);
                    //console.log( data);
                    if (data) {
                        return '<h4 class="verDetalle bg-success btn  text-uppercase text-light"  > Ver Detalle </h4>';
                    }

                }
            }

        ],
        "order": [[2, 'desc']]

    });

};






///****************************************************************************************************************************************************************************************************///
///********************************************************************   Metodos para el DetalleInformaticoCheques   *********************************************************************************///
///****************************************************************************************************************************************************************************************************///

////******Crea y pinta la tabla del modal del historico **********///
function DibujarTBLHistoricoFormaPago() {
    $("#TablaHistoricoReposiciones").append(

        "<table id='HistoricoReposiciones'  class='margenSection table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Historico  DE  SEGUIMIENTO</caption>"
        + "<thead class='tabla'>" +

        "<tr class='text-center text-uppercase'>" +


        "<th>Id</th>" +
        "<th>Fecha Cambio</th>" +
        "<th>Motivo de Registro</th>" +
        "<th>Forma de Pago </th>" +
        "<th>                       </th>" +
        "<th>Nueva Forma de Pago    </th>" +
        "<th>Suceso creado por </th>" +
        "<th>Estado Cancelacion </th>" +
        "<th>Es Cancelado </th>" +
        "<th>Referencia Cancelado </th>" +



        "</tr>" +
        "</thead>" +
        "</table>"
    );
};


function PintarDatosHistoricoFormaPago(datos) {
    $('#HistoricoReposiciones').DataTable({
        "ordering": true,
        "info": false,
        "searching": false,
        "paging": true,
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

            { "data": "Id" },
            { "data": "FechaCambio" },
            { "data": "MotivoRefoliacion" },
            { "data": "ChequeAnterior" },
            null,
            { "data": "ChequeNuevo" },
            { "data": "RepuestoPor" },
            { "data": "DescripcionCancelado" },
            { "data": "EsCancelado" },
            { "data": "ReferenciaCancelado" }
        ],
        "columnDefs": [

            { className: "text-center col-1", visible: true, "targets": 0, },
            { className: "text-center col-1", visible: true, "targets": 1, },
            { className: "text-center col-2", visible: true, "targets": 2, },
            { className: "text-center col-1", visible: true, "targets": 3, },
            {
                className: "text-center col-1",
                visible: true,
                "targets": [4],
                render: function (data, type, row) {
                    //console.log(typeof data);
                    //console.log( data);
                    //if (data) {
                    return '<i class="fas fa-exchange-alt"></i>';
                    //}

                }
            },
            { className: "text-center col-1", visible: true, "targets": 5, },
            { className: "text-center col-2", visible: true, "targets": 6, },
            { className: "text-center col-1", visible: true, "targets": 7, },
            { className: "text-center col-1", visible: true, "targets": 8, },
            { className: "text-center col-1", visible: true, "targets": 9, }


        ],
        "order": [[0, 'desc']]

    });

};


function HistoricoSeguimiento(idRegistro) {

    MensajeCargando();

    let HistoricoAlocalizar = "{'IdRegistro':'" + idRegistro + "'}";


    $.ajax({
        url: 'EncontrarCheque/BuscarHistorico',
        data: HistoricoAlocalizar,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (response) {



            if (response.RespuestaServidor == 200) {

                // console.log(response.Data);


                $('#TablaHistoricoReposiciones').empty();
                DibujarTBLHistoricoFormaPago();
                PintarDatosHistoricoFormaPago(response.Data);

                //OCULTAR EL MODAL DONDE SE ENCUENTRA LA VISTA PARCIAL
                $('#HistoricoSeguimientoFormaPago').modal('show');




            } else {
                MensajeErrorSweet(response.Error, response.Solucion);
            }

            OcultarMensajeCargando();

        }, error: function (jqXHR, textStatus) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
            OcultarMensajeCargando();
        }
    });

}


function AgregarFormaPagoReferenciaCancelacion(IdRegitroAReferenciaCancelado) {
    let SeleccionaReferencia = document.getElementById("SeleccionaReferenciaCancelacion");

    console.log(SeleccionaReferencia);


    if (SeleccionaReferencia.value != "") {

        MensajeCargando();

        let enviarIdReferencia = "{'IdReferenciaCancelado':'" + SeleccionaReferencia.value + "', 'IdRegistroCancelar':'" + IdRegitroAReferenciaCancelado + "'}";


        $.ajax({
            url: 'EncontrarCheque/AgregarRemover_IdFormaPagoAReferenciaCancelado',
            data: enviarIdReferencia,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response.NumeroMensaje == 6 || response.NumeroMensaje == 7 || response.NumeroMensaje == 8) {


                    MensajeCorrectoSweet(response.Mensaje);

                } else {
                    MensajeErrorSweet(response.Solucion, response.Mensaje)
                }


                $('#AgregarFormaPagoCancelacion').modal('hide');
                OcultarMensajeCargando();




            }, error: function (jqXHR, textStatus) {
                MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
                OcultarMensajeCargando();
            }
        });

        OcultarMensajeCargando();

        // console.log(SeleccionaReferencia.value + " || " + IdRegitroAReferenciaCancelado);
    } else {
        MensajeErrorSweet(" Seleccione una referencia ", " No se a elegido ninguna referencia aún ");
    }



}


function VerificaSiTieneReferencia(idRegistro) {
    let existeReferencia = document.getElementById("ExisteReferencia");

    console.log(existeReferencia);
    let tieneReferencia = "{'IdRegistro':'" + idRegistro + "'}";


    $.ajax({
        url: 'EncontrarCheque/TieneReferenciaIdFormaPago',
        data: tieneReferencia,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (response) {



            if (response.Referencia != "") {


                existeReferencia.innerHTML = 'Esta forma de pago ya se encuentra cargada dentro de la referencia ' + response.Referencia + ', cambiela si asi lo desea';
                existeReferencia.style.display = "block";
                $('#AgregarFormaPagoCancelacion').modal('show');


            } else {

                existeReferencia.style.display = "none";
                $('#AgregarFormaPagoCancelacion').modal('show');
            }

            OcultarMensajeCargando();

        }, error: function (jqXHR, textStatus) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
            OcultarMensajeCargando();
        }
    });



}

//$(document).on('hidden.bs.modal', function (event) {
//    if ($('.modal:visible').length) {
//        $('body').addClass('modal-open');
//    }
//});






///****************************************************************************************************************************************************************************************************///
///****************************************************************************************************************************************************************************************************///
///****************************************************************************************************************************************************************************************************///





$(document).ready(function () {






    let filtroSeleccionado = null;
    const TipoFiltro = document.getElementById("TipoFiltro");
    TipoFiltro.addEventListener("change",
        function () {

            filtroSeleccionado = null;
            filtroSeleccionado = this.options[TipoFiltro.selectedIndex];
         ///   console.log(filtroSeleccionado.value);

            document.getElementById("Buscador").disabled = false;
        }
    );



    let EnviaIdNom = "{'NumeroQuincena: ''}";


    $("#Buscador").select2
        ({

            language: "es",
            width: '100%',
            ajax: {
                url: 'EncontrarCheque/ObtenerFiltradoDatos',
                type: 'GET',
                dataType: 'json',
                delay: 1000,
                data: function (data) {

                    //let query = {
                    //    nombre: data.term,
                    //    type: 'strin'
                    //}

                    return {
                        TipoDeBusqueda: filtroSeleccionado.value,
                        BuscarDato: data.term
                    };

                    // console.log(query.term);

                    // Query parameters will be ?search=[term]&type=public
                    //return query;
                    //console.log(response.term);

                },
                processResults: function (data, params) {
                    // parse the results into the format expected by Select2
                    // since we are using custom formatting functions we do not need to
                    // alter the remote JSON data, except to indicate that infinite
                    // scrolling can be used
                    ////console.log("entre TILIN")
                    ////console.log(data)
                    ////console.log(data.DataElementosEncontrados)


                    let results = [];
                    $.each(data, function (index, item) {

                        results.push({
                            id: item.id,
                            text: item.text
                        })

                    });

                    return { results };

                },
                cache: true
            },
            placeholder: "Ingrese datos a buscar...",
            allowClear: true,
            disabled: true,
            minimumInputLength: 5,
            cache: true,





        });


    let myID = 'Buscador';



    $('#select2-' + myID + '-container').addClass('seleccion2');
    $('#select2-' + myID + '-container').parent().css('height', '62px');
    $('#select2-' + myID + '-container').parent().css('border-radius', '15px');








    let datoSeleccionadoSelect2 = null;
    $('#Buscador').on('select2:select', function (e) {

        datoSeleccionadoSelect2 = null;
        datoSeleccionadoSelect2 = e.params.data;
       // console.log(datoSeleccionadoSelect2);
    });





    const Buscador = document.getElementById("btnBuscardor");
    Buscador.addEventListener("click",
        function () {

            if (filtroSeleccionado != null) {
                if (datoSeleccionadoSelect2 != null) {
                    MensajeCargando();

                    //let BuscarElementoSeleccionado = "{'BuscarElemento_id': '"+datoSeleccionadoSelect2.id+"', 'BuscarElemento_text': '"+datoSeleccionadoSelect2.text+"'}";
                    let BuscarElementoSeleccionado = "{'BuscarElemento':'" + datoSeleccionadoSelect2.i + "'}";
                    let BuscarElemento = {

                        tipoBusqueda: filtroSeleccionado.value,
                        id: datoSeleccionadoSelect2.id,
                        text: datoSeleccionadoSelect2.text,

                    };

                    $.ajax({
                        url: 'EncontrarCheque/Buscar',
                        data: JSON.stringify(BuscarElemento),
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response.RespuestaServidor == 201) {
                                MensajeCorrectoSweet("Registro encontrado satisfactoriamente");
                              //  console.log(response.RegistrosEncontrados);
                                //$('#TablaEsFoliada').empty();
                                //DibujarTablaEsFoliada();
                                //PintarResultadoEsFoliada(response.DetalleTabla);
                                $('#TablaRegistros').empty();
                                DibujarTablaRegistros();
                                PintarRegistrosEncontrados(response.RegistrosEncontrados);
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
                else {
                    MensajeErrorSweet("seleccione un dato por favor!", "No se a seleccionado ningun dato");
                }
            }
            else {
                MensajeErrorSweet("seleccione un filtro deseado!", "No se a seleccionado ningun filtro");
            }







        }
    );








    //Click para eliminar el elemento seleccionado de la tabla
    $(document).on("click", ".verDetalle", function () {



        //Decomentar para volver al funcionamiendo original 

        let datoIdbuscar = RegistrosTabla.row($(this).parents("tr")).data();

        let buscarIdRegistro = "{'IdRegistroBuscar': '" + datoIdbuscar.IdRegistro + "'}";

        MensajeCargando();
        $.ajax({
            url: 'EncontrarCheque/DetallesInformativosCheques',
            data: buscarIdRegistro,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response.RespuestaServidor === "500") {
                    MensajeErrorSweet(response.MensajeError);
                    $('#RenderPartialViewDetallesCheques').html('');
                } else {

                    $('#RenderPartialViewDetallesCheques').html('');
                    $('#RenderPartialViewDetallesCheques').html(response);
                    $('#Prueba_RenderVista').modal('show');

                }

                OcultarMensajeCargando();
            }, error: function (jqXHR, textStatus) {
                MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
                OcultarMensajeCargando();
            }
        });


    });











});
























