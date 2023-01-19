/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/*************************************           VERIFICAR QUE TODO LOS CHEQUES CANCELADOS ESTEN JALANDO        ******************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */
/******************************************************************************************************************************************* */



function EliminarNumeroReferencia(IdRegistroAInactivar, NumeroReferencia, Anio) {




    Swal.fire({
        title: '¿DESEA ELIMINAR LA REFERENCIA # ' + NumeroReferencia + ' DEL AÑO ' + Anio + ' ?',
        text: "¡Si elimina una referencia todas las formas de pago cargadas a esta misma, se saldran de la referencia y tendria que volverlas a cargar una a una!",
        icon: 'warning',

        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#17a2b8',
        confirmButtonText: 'Aceptar',
        cancelButtonText: `Cancelar `

    }).then((result) => {
        if (result.isConfirmed) {

            Swal.fire({
                title: '¿ESTA SEGURO DE ELIMINAR LA REFERENCIA?',
                text: "¡Se guardara un registro de este movimiento y podria no revertirse!",
                icon: 'warning',

                showCancelButton: true,
                confirmButtonColor: '#28a745',
                cancelButtonColor: '#17a2b8',
                confirmButtonText: 'Eliminar Referencia',
                cancelButtonText: `Cancelar `

            }).then((result) => {
                if (result.isConfirmed) {

                    MensajeCargando();
                    axios.post('/CrearReferencia_Cancelados/InactivarReferenciaCancelado', {
                        IdReferenciaCancelacion: IdRegistroAInactivar
                    })
                        .then(function (response) {
                            if (response.data.bandera) {
                                MensajeCorrectoConRecargaPagina("Se a eliminado exitosamente la referencia #" + NumeroReferencia + " seleccionada y se han revocado " + response.data.respuestaServer + " registros cargados dentro de la referencia");
                            } else {
                                MensajeErrorSweet("No se pudo eliminar la referencia intente mas tarde o contacte al desarrollador");
                            }
                            OcultarMensajeCargando();
                    })
                        .catch(function (error) {
                            MensajeErrorSweet("Ocurrio un error intente de nuevo ", error)
                            OcultarMensajeCargando();
                    });



                    //let eliminarIdReferencia = "{'IdReferenciaCancelacion':'" + IdRegistroAInactivar + "'}";


                    //$.ajax({
                    //    url: '/CrearReferencia_Cancelados/InactivarReferenciaCancelado',
                    //    data: eliminarIdReferencia,
                    //    type: "POST",
                    //    contentType: "application/json; charset=utf-8",
                    //    success: function (response) {

                    //        if (response.bandera) {
                    //            MensajeCorrectoConRecargaPagina("Se a eliminado exitosamente la referencia #" + NumeroReferencia + " seleccionada y se han revocado " + response.respuestaServer + " registros cargados dentro de la referencia");
                    //        } else {
                    //            MensajeErrorSweet("No se pudo eliminar la referencia intente mas tarde o contacte al desarrollador");
                    //        }
                    //        OcultarMensajeCargando();

                    //    }, error: function (jqXHR, textStatus) {
                    //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
                    //        OcultarMensajeCargando();
                    //    }
                    //});



                } else if (result.dismiss) {
                    MensajeInformacionSweet("No se realizara ningún cambio");

                }



            })





        } else if (result.dismiss) {
            MensajeInformacionSweet("No se realizara ningún cambio");

        }



    })




}





/**********************************************************************************************************************************************************************/
/**********************************************************************************************************************************************************************/
/********************************************       funciones para el modal de finalizar una referencia *******************************************************/
let idRefereciaSeleccionada;
function AbrirFinalizaReferencia(IdReferecia) {


    idRefereciaSeleccionada = null;
    idRefereciaSeleccionada = IdReferecia;

    document.querySelector(".ajax-upload-dragdrop").style = "width: 411px;";
    $('#FinalizarReferencia').modal('show')
}



function EnviarArchivo() {
    if ($("#GuardarFolio").length > 0) {
        // hacer algo aquí si el elemento existe
        let folioAGuardar = document.getElementById("GuardarFolio").value;

        if (folioAGuardar != "") {
            //Se Envia al servidor
            ControlUploader.startUpload();

        } else {
            MensajeErrorSweet("Introduzca el folio del oficio de cancelacion");
        }

    } else {
        MensajeWarningSweet("Cargue el PDF o arastre y suelte donde se indica");
    }


}



let EnviarDatos;
let ControlUploader;

function SubirArchivos() {
    //Carga el upload para subir archivos
    ControlUploader = $("#fileuploader").uploadFile({
        url: "/CrearReferencia_Cancelados/ImportarArchivo",
        dragDrop: true,
        maxFileCount: 1,
        fileName: "myfile",
        multiple: true,
        allowedTypes: "pdf",
        abortStr: "Abortar",
        cancelStr: "Cancelar",
        doneStr: "Cargado",
        deletelStr: "Eliminar",
        downloadStr: "Descargar",
        extErrorStr: "No es posible cargar esté archivo, solo está permitido subir este tipo de archivos ",
        uploadErrorStr: "Ocurrió un error al cargar el archivo. Inténtelo de nuevo.",
        dragDropStr: "<span><b>Arrastrar y soltar Archivos</b></span>",
        duplicateErrorStr: "El Archivo ya Existe",
        maxFileCountErrorStr: "No esta permitido. El máximo número de Archivos son: ",
        multiDragErrorStr: "No está permitido subir estos Archivos.",
        sizeErrorStr: "Ha sobre pasado el tamaño máximo:",
        uploadStr: "Cargar",
        showDelete: true,
        showDownload: false,
        showDone: true,
        showCancel: true,
        returnType: "json",
        sequential: true,
        sequentialCount: 1,
        showPreview: true,
        previewHeight: "100px",
        previewWidth: "100px",

        onLoad: function (obj) {
        },
        dynamicFormData: function () {

            var data = { FolioDocumento: document.getElementById("GuardarFolio").value, FinalizarIdReferencia: idRefereciaSeleccionada };
            return data;

        },
        extraHTML: function () {
            var html = "<div><b>Folio de Documento : </b> <input id='GuardarFolio' type='text' name='tags' value='' /> <br/> </div> ";
            //html += " <br/> <br/> <button  type='button' class='btn btn - success' data-dismiss='modal' onclick='EnviarArchivo()'>Enviar </button>";
            return html;
        },
        onSuccess: function (files, data, xhr, pd) {

            if (data.bandera) {
                MensajeCorrectoConRecargaPagina("El archivo " + files[0] + " fue cargado correctamente y se han cambiado de estatus " + data.respuestaServer + " Cheques a CANCELADOS")
            } else {
                MensajeErrorSweet(data.mensaje);
            }

        },
        autoSubmit: false

    });


}






$(document).ready(function () {

    document.getElementById("InputNumeroReferencia").focus();
    SubirArchivos();

    let datoObtenidoDel_InputNumeroReferencia = '';
    const CrearReferencia = document.getElementById("btnCrearReferenciaCancelados");
    CrearReferencia.addEventListener("click",
        function () {

            datoObtenidoDel_InputNumeroReferencia = document.getElementById("InputNumeroReferencia").value;


            if (datoObtenidoDel_InputNumeroReferencia != '') {





                Swal.fire({
                    title: '¿Seguro que desea crear la referencia numero ' + datoObtenidoDel_InputNumeroReferencia + ' del año en curso ?',
                    text: "¡Se guardara un registro de este movimiento y podria no revertirse!",
                    icon: 'warning',

                    showCancelButton: true,
                    confirmButtonColor: '#28a745',
                    cancelButtonColor: '#17a2b8',
                    confirmButtonText: 'Crear Referencia',
                    cancelButtonText: `Cancelar `

                }).then((result) => {
                    if (result.isConfirmed)
                    {



                        MensajeCargando();
                        axios.post('/CrearReferencia_Cancelados/CrearReferenciaCancelado', {
                            NuevoNumeroReferencia: datoObtenidoDel_InputNumeroReferencia
                        })
                            .then(function (response) {
                                if (response.data) {
                                    MensajeCorrectoConRecargaPagina("Se a creado exitosamente la nueva referencia");
                                }
                                else {
                                    MensajeErrorSweet("Cambie el numero de referencia, ya que se repite con otro activo", "No se puede crear un numero de referencia repetida en un mismo año")
                                }
                                OcultarMensajeCargando();
                        })
                            .catch(function (error) {
                                MensajeErrorSweet("Ocurrio un error intente de nuevo ", error)
                            OcultarMensajeCargando();
                        });


                        //let BuscarElementoSeleccionado = "{'BuscarElemento_id': '"+datoSeleccionadoSelect2.id+"', 'BuscarElemento_text': '"+datoSeleccionadoSelect2.text+"'}";
                        //let crearReferenciaCancelacion = "{'NuevoNumeroReferencia':'" + datoObtenidoDel_InputNumeroReferencia + "'}";



                        //$.ajax({
                        //    url: 'CrearReferencia_Cancelados/CrearReferenciaCancelado',
                        //    data: crearReferenciaCancelacion,
                        //    type: "POST",
                        //    contentType: "application/json; charset=utf-8",
                        //    success: function (response) {

                        //        if (response) {
                        //            MensajeCorrectoConRecargaPagina("Se a creado exitosamente la nueva referencia");
                        //        }
                        //        else {
                        //            MensajeErrorSweet("Cambie el numero de referencia, ya que se repite con otro activo", "No se puede crear un numero de referencia repetida en un mismo año")
                        //        }


                        //        OcultarMensajeCargando();

                        //    }, error: function (jqXHR, textStatus) {
                        //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
                        //        OcultarMensajeCargando();
                        //    }
                        //});



                    }
                })




            }
            else {
                MensajeErrorSweet("Ingrese un numero", "No se ingreso ninguna dato");
            }







        }
    );


});



/******************************************************************************************************************************************************************************************/
/******************************************************************************************************************************************************************************************/
/********************************************************             funciones para el modal DetalleReferencia                      ******************************************************/

function DibujarTablaDetalleReferencia(referecnia) {
    $("#divTablaDetalleReferencia").append(

        "<table id='tblDetalleReferencia'    class='table table-striped table-bordered table-hover text-center' cellspacing='0' style='width:100%'     >" +
        " <caption class='text-uppercase'>Resumen de formas de pago cargadas dentro de la referencia : '" + referecnia + "'</caption>" +
        "<thead class='tabla'>" +

        "<tr>" +
        "<th>Id</th>" +
        "<th>EsPenA</th>" +
        "<th>Deleg</th>" +
        "<th>Id_Nom</th>" +
        "<th>Referencia</th>" +
        "<th>Nomina</th>" +
        "<th>Quincena</th>" +
        "<th>Num</th>" +
        "<th>Beneficiario</th>" +
        "<th>Num_Che</th>" +
        "<th>Liquido</th>" +
        "<th>Cuenta Pagadora</th>" +
        "<th>Banco  Pagador</th>" +
        "<th>Quitar de Referencia </th>" +
        "</tr>" +
        "</thead>" +

        "<tfoot>" +
        "<tr class='tablaFiltro'>" +
        "<th></th>" +
        "<th></th>" +
        "<th></th>" +
        "<th></th>" +
        "<th></th>" +
        "<th class='Filtro col-1'>Nomina</th>" +
        "<th class='Filtro col-1'>Quincena</th>" +
        "<th class='Filtro col-2'></th>" +
        "<th class='Filtro col-3'>Nomina</th>" +
        "<th class='Filtro col-2'>Num_che</th>" +
        "<th class='Filtro col-2'>Liquido</th>" +
        "<th class='Filtro col-1'>Cuenta Pagadora</th>" +
        "<th></th>" +
        "<th></th>" +
        "</tr>" +
        "</tfoot>" +

        "</table>"
    );
};

let tableDetallesReferecniaCancelacion;
function PintarResultadoDetalleReferencia(datos) {
    //agregar inputs donde tenga la clase filtro

    $('#tblDetalleReferencia').find(".Filtro").each(function () {
        var title = $(this).text();
        // $(this).html(' <input type="text" style="width:100%" onkeypress="return event.keyCode != 13;"  class="form-control input-sm" placeholder="' + title + '" />   ');
        $(this).html(' <div class="inner-addon right-addon">   <i class="glyphicon fas fa-search-dollar"></i> <input type="text"  onkeypress="return event.keyCode != 13;"  class="form-control " placeholder="" /> </div >  ');
    });
    tableDetallesReferecniaCancelacion = $('#tblDetalleReferencia').DataTable({
        // "fixedHeader": true,
        "dom": 'Blfrtip',
        "buttons": [

            {
                extend: 'excelHtml5',
                className: 'btn btn-info',
                text: '<i class="fas fa-file-download"></i>  &nbsp  EXCEL',
                filename: `CancelarFoliosFiltro_Exel`,
                title: `CONTROL DE FOLIOS A CANCELAR`
            },
            {
                extend: 'print',
                customize: function (win) {
                    $(win.document.body)
                        .css('font-size', '12pt')


                    $(win.document.body).find('table')
                        .addClass('compact')
                        .css('font-size', 'inherit');
                },
                className: 'btn btn-info',
                text: '<i class="fas fa-print"></i>  &nbsp Imprimir',
                filename: `CancelarFoliosFiltro_Web`,
                title: `FOLIOS DEL BANCO A CANCELAR`
            }
        ],
        "ordering": false,
        "info": true,
        "searching": true,
        "paging": true,
        "lengthMenu": [10, 20, 50, 100],
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
            { "data": "EsPenA" },
            { "data": "Deleg" },
            { "data": "Id_Nom" },
            { "data": "Referencia" },
            { "data": "Nomina" },
            { "data": "Quincena" },
            { "data": "Num" },
            { "data": "Beneficiario" },
            { "data": "FolioCheque" },
            { "data": "Liquido" },
            { "data": "CuentaPagadora" },
            { "data": "BancoPagador" },
            {
                render: function (data, type, row) {

                    return '<button class="bg-success btn  text-uppercase text-light text-center AnularIdDeReferencia"  > <i class="fas fa-undo"></i> </button>';

                }

            }

        ],
        "order": [[0, 'desc']]


    }).columns().every(function () {
        var that = this;
        $('input', this.footer()).on('keyup change', function (e) {
            var keyCode = e.keyCode || e.which;
            if (that.search() !== this.value) {
                that
                    .search(this.value)
                    .draw();
            }
        });
    });
    //Agrega los filtros dentro del thead de la tabla
    $(".dataTable").find("tfoot tr").appendTo($(".dataTable").find("thead"));






};


function AbrirDetalleReferencia(IdReferencia, NumeroReferencia) {
   

    MensajeCargando();
    axios.post('/CrearReferencia_Cancelados/ObtenerDetalleReferenciasParaModal', {
        IdReferencia: IdReferencia
    })
        .then(function (response) {

            if (response.data.bandera) {
                $("#divTablaDetalleReferencia").empty();
                DibujarTablaDetalleReferencia(NumeroReferencia);
                PintarResultadoDetalleReferencia(response.data.respuestaServer);

                document.getElementById("NumeroReferencia").innerHTML = "DETALLE DE LA REFERENCIA #" + NumeroReferencia + "";
                document.getElementById("IdReferenciaCanceladoSelecionado").innerHTML = IdReferencia;
                document.getElementById("NombreReferenciaCancelado").innerHTML = NumeroReferencia;

                document.querySelector(".dt-buttons").style = "text-align: right;";
                $('#DetalleReferencia').modal('show')
                
            } else {
                MensajeErrorSweet("Aun no existen registros dentro de la referencia seleccionada");
            }
            OcultarMensajeCargando();
        })
        .catch(function (error) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo ", error)
            OcultarMensajeCargando();
        });

    //let EnviarDatos = "{'IdReferencia': '" + IdReferencia + "'}";
    //$.ajax({
    //    url: '/CrearReferencia_Cancelados/ObtenerDetalleReferenciasParaModal',
    //    data: EnviarDatos,
    //    type: "POST",
    //    contentType: "application/json; charset=utf-8",
    //    success: function (response) {

    //        if (response.bandera) {
    //            $("#divTablaDetalleReferencia").empty();
    //            DibujarTablaDetalleReferencia(NumeroReferencia);
    //            PintarResultadoDetalleReferencia(response.respuestaServer);

    //            document.getElementById("NumeroReferencia").innerHTML = "DETALLE DE LA REFERENCIA #" + NumeroReferencia + "";
    //            document.getElementById("IdReferenciaCanceladoSelecionado").innerHTML = IdReferencia;
    //            document.getElementById("NombreReferenciaCancelado").innerHTML = NumeroReferencia;

    //            document.querySelector(".dt-buttons").style = "text-align: right;";
    //            $('#DetalleReferencia').modal('show')
    //        } else {
    //            MensajeErrorSweet("Aun no existen registros dentro de la referencia seleccionada");
    //        }




    //        OcultarMensajeCargando();

    //    }, error: function (jqXHR, textStatus) {
    //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
    //        OcultarMensajeCargando();
    //    }
    //});

}

function CerrarDetalleReferencia() {
    $('#DetalleReferencia').modal('hide');
   // window.location.reload();
}

$(document).on("click", ".AnularIdDeReferencia", function () {
    //let eliminarDetoTemporalDeDataTable = tableDetallesReferecniaCancelacion.row($(this).parents("tr"));
    let pagoARemover = tableDetallesReferecniaCancelacion.row($(this).parents("tr")).data();
    

    //console.log(pagoARemover);

    Swal.fire({
        title: '¿Seguro que deseas remover esta forma de pago del proceso de cancelacion?'/*'Do you want to save the changes?'*/,
        showDenyButton: true,
        showCancelButton: false,
        confirmButtonText: 'Remover',
        denyButtonText: `No hacer nada`,
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {


            MensajeCargando();
            let eliminarDetoTemporalDeDataTable = tableDetallesReferecniaCancelacion.row($(this).parents("tr"));
            axios.post('/CrearReferencia_Cancelados/AnularCancelacion', {
                IdPago: pagoARemover.Id
            })
            .then(function (response) {

                if (response.data.bandera) {

                    eliminarDetoTemporalDeDataTable.remove().draw();

                    MensajeCorrectoSweet("Se removio con exito la forma de pago de la referencia");
                } else {
                    MensajeErrorSweet("Hubo un problema y no se pudo remover la forma de pago de la referencia");
                }
                OcultarMensajeCargando();

            })
            .catch(function (error) {
                MensajeErrorSweet("Ocurrio un error intente de nuevo ", error )
                OcultarMensajeCargando();
            });

            //   tableDetallesReferecniaCancelacion.row($(this).parents("tr")).remove().draw();
            //Peticion por ejax al server para eliminar el cheque de la referencia
            //let EnviarDatos = "{'IdPago': '" + pagoARemover.Id + "'}";

            //$.ajax({
            //    url: '/CrearReferencia_Cancelados/AnularCancelacion',
            //    data: EnviarDatos,
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    success: function (response) {

            //        if (response.bandera) {

            //            eliminarDetoTemporalDeDataTable.remove().draw();

            //            MensajeCorrectoSweet("Se removio con exito la forma de pago de la referencia");
            //        } else {
            //            MensajeErrorSweet("Hubo un problema y no se pudo remover la forma de pago de la referencia");
            //        }

            //        OcultarMensajeCargando();

            //    }, error: function (jqXHR, textStatus) {
            //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
            //        OcultarMensajeCargando();
            //    }
            //});






        } else if (result.isDenied) {

            MensajeInformacionSweet("No hare ningun cambio, Descuida!")
        }
    })





});


/******************************************************************************************************************************************************************************************/
/******************************************************************************************************************************************************************************************/
/******************************************************  funcion para mostrar la previa del PDF del documento acreditado             ******************************************************/

function AbrirVistaPreviaPDF(IdReferencia) {

    axios.post('/CrearReferencia_Cancelados/ObtenerPdfReferenciaCancelada', {
        IdReferenciaCancelado: IdReferencia
    })
    .then(function (response) {

        if (response.data.bandera) {

            $("VisorPdfCCOficial").empty();
            PDFObject.embed("data:application/pdf;base64," + response.data.respuestaServer + "", "#VisorPdfCCOficial");
                $('#VisualizadorCancelacionOficial').modal('show');
        } else {
                MensajeErrorSweet("Hubo un problema y no se a podido cargar la previa, intentelo de nuevo mas tarde");
        }

         OcultarMensajeCargando();
    })
        .catch(function (error) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo ", error )
        OcultarMensajeCargando();
    });


}



/******************************************************************************************************************************************************************************************/
/******************************************************************************************************************************************************************************************/
/******************************************************             Funciones para abrir modales para ver y descargar reportes            *************************************************/
/******************************************************************************************************************************************************************************************/
/******************************************************************************************************************************************************************************************/
function AbrirReporteInicial(urlControler)
{
    let IdSeleccionado = document.getElementById("IdReferenciaCanceladoSelecionado").innerHTML;
    $('#ReportesBasicos').modal('show');
   // console.log(urlControler);
   // console.log(IdSeleccionado);

   // console.log(urlControler + "?IdReferencia=" + IdSeleccionado);


   // href = "/CrearReferencia_Cancelados/DescargarReporteNominaAnual?IdReferencia=1
   // document.getElementById('descargaNominaAnual').setAttribute('href', urlControler +"DescargarReporteNominaAnual"+  "?IdReferencia=" + IdSeleccionado);
  //  document.getElementById('descargaCBAnual').setAttribute('href', urlControler +"DescargarReporteCuentaBancariaAnual"+  "?IdReferencia=" + IdSeleccionado);
  //  document.getElementById('descargaPenA').setAttribute('href', urlControler +"DescargarReportePensionAlimenticia"+  "?IdReferencia=" + IdSeleccionado);


    // DescargarReporteCuentaBancariaAnual


}

function AbrirReporteIPD(urlControler) {
    let IdSeleccionado = document.getElementById("IdReferenciaCanceladoSelecionado").innerHTML;

   // document.getElementById('TotalesIPDPorNomina').setAttribute('href', urlControler +"DescargarTGCxNomina"+"?IdReferencia="+IdSeleccionado);
  // document.getElementById('IPD').setAttribute('href', urlControler +"DescargarIPDPorAnio"+"?IdReferencia="+IdSeleccionado);
   // document.getElementById('TotalesIPDPorBanco').setAttribute('href', urlControler +"DescargarTGCxBanco"+"?IdReferencia="+IdSeleccionado);

    $('#ReportesIPD').modal('show');
}

function AbrirReporteIPDCOMPENSADO(urlControler) {
    let IdSeleccionado = document.getElementById("IdReferenciaCanceladoSelecionado").innerHTML;
  //  document.getElementById('DescargarIPDCxAnio').setAttribute('href', urlControler+"DescargarIPDCompensadoPorAnio"+"?IdReferencia="+IdSeleccionado);

    $('#ReportesIPD_COMPENSADO').modal('show');
}



/******************************************************************************************************************************************************************************************/
/******************************************************************************************************************************************************************************************/
/******************************************************             funciones para Descargar el IPD y funciones aun en desarrollo            ******************************************************/
/******************************************************************************************************************************************************************************************/
/******************************************************************************************************************************************************************************************/

async function exportarIPD(url) {

    let IdSeleccionado = document.getElementById("IdReferenciaCanceladoSelecionado").innerHTML;
    let nombreRefeCancelacion = document.getElementById("NombreReferenciaCancelado").innerHTML;
    console.log(IdSeleccionado)
    console.log(nombreRefeCancelacion)

    //console.log("Download")
    //console.log(url)


    // Genero una petición con axios que me regresará una archivo Blob (Binary Large Object) como respuesta
    // y una vez obtenido el archivo crea un ancla temporal el cúal su función será descargar el archivo
    // generado por medio del window.URL.createObjectURL

    MensajeCargando();

    axios({
        url: url,
        method: 'GET',
        responseType: 'blob',
        params: {
            IdReferencia: IdSeleccionado,
        }
    }).then((response) => {

       
        console.log(response.data)
       // alert("Listo");

        const url = window.URL
            .createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');

        link.href = url;
       // link.setAttribute('download', `ipd${nombreRefeCancelacion}.dbf`);
        link.setAttribute('download', `IPD_Anuales_${nombreRefeCancelacion}.Zip`);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

          

    }).catch(error => {
            console.log(error)
            //cargar(false)
    }).finally(() => {
        OcultarMensajeCargando();
    })
}


function EstamosEnDesarrollo()
{
    MensajeEstamosEnConstrucion();
}



function CheCancRPNominaAnual() {
    let IdReferencia = document.getElementById("IdReferenciaCanceladoSelecionado").innerHTML;

    //let EnviarDatos = "{'IdReferencia': '" + IdReferencia + "'}";

    MensajeCargando();
    axios.post('/CrearReferencia_Cancelados/ObtenerDetalleReferenciasParaModal', {
        IdReferencia: IdReferencia
    })
    .then(function (response) {

        if (response.data.bandera) {
            $("#divTablaDetalleReferencia").empty();
            DibujarTablaDetalleReferencia(NumeroReferencia);
            PintarResultadoDetalleReferencia(response.data.respuestaServer);

                document.getElementById("NumeroReferencia").innerHTML = "DETALLE DE LA REFERENCIA #" + NumeroReferencia + "";
                document.getElementById("IdReferenciaCanceladoSelecionado").innerHTML = IdReferencia;
                document.getElementById("NombreReferenciaCancelado").innerHTML = NumeroReferencia;
                $('#DetalleReferencia').modal('show')
        } else {
                MensajeErrorSweet("Aun no existen registros dentro de la referencia seleccionada");
        }

            OcultarMensajeCargando();

    })
        .catch(function (error) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo ", error)
        OcultarMensajeCargando();
    });



  

}

/******************************************************************************************************************************************************************************************/
/******************************************************************************************************************************************************************************************/
/******************************************************             funciones para Descargar IPDcompensado               ******************************************************/
/******************************************************************************************************************************************************************************************/
/******************************************************************************************************************************************************************************************/

async function exportarIPDCompensado(url) {

    let IdSeleccionado = document.getElementById("IdReferenciaCanceladoSelecionado").innerHTML;
    let nombreRefeCancelacion = document.getElementById("NombreReferenciaCancelado").innerHTML;
    console.log(IdSeleccionado)
    console.log(nombreRefeCancelacion)

    //console.log("Download")
    //console.log(url)


    // Genero una petición con axios que me regresará una archivo Blob (Binary Large Object) como respuesta
    // y una vez obtenido el archivo crea un ancla temporal el cúal su función será descargar el archivo
    // generado por medio del window.URL.createObjectURL

    MensajeCargando();

    axios({
        url: url,
        method: 'GET',
        responseType: 'blob',
        params: {
            IdReferencia: IdSeleccionado,
        }
    }).then((response) => {


        console.log(response.data)
        // alert("Listo");

       // const url = window.URL.createObjectURL(new Blob([response.data]));
        const url = window.URL.createObjectURL(response.data);
        const link = document.createElement('a');

        link.href = url;
        // link.setAttribute('download', `ipd${nombreRefeCancelacion}.dbf`);
        link.setAttribute('download', `IPD_COMPENSADO_Anuales_${nombreRefeCancelacion}.Zip`);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);



    }).catch(error => {
        console.log(error)
        //cargar(false)
    }).finally(() => {
        OcultarMensajeCargando();
    })
}





async function exportarZIP(url , numero) {

    let IdSeleccionado = document.getElementById("IdReferenciaCanceladoSelecionado").innerHTML;
    let nombreRefeCancelacion = document.getElementById("NombreReferenciaCancelado").innerHTML;
    //console.log(IdSeleccionado)
    //console.log(nombreRefeCancelacion)

    //console.log(numero)
    //console.log(url)

    let nombreReporte;
    switch (numero) {
        case 1:
            nombreReporte = "ReporteNominaAnualCC_"+nombreRefeCancelacion+".Zip";
            break;
        case 2:
            nombreReporte = "ReporteCuentaBancariaAnualCC_"+nombreRefeCancelacion+".Zip";  
            break;
        case 3:
            nombreReporte = "ReportePensionAlimenticiaCC_"+nombreRefeCancelacion+".Zip";
            break;
    }

    // Genero una petición con axios que me regresará una archivo Blob (Binary Large Object) como respuesta
    // y una vez obtenido el archivo crea un ancla temporal el cúal su función será descargar el archivo
    // generado por medio del window.URL.createObjectURL

    MensajeCargando();

    axios({
        url: url,
        method: 'GET',
        responseType: 'blob',
        params: {
            IdReferencia: IdSeleccionado
        }
    }).then((response) => {


        // console.log(response.data)
        // alert("Listo");

        const url = window.URL.createObjectURL(response.data);
        const link = document.createElement('a');

        link.href = url;
        // link.setAttribute('download', `ipd${nombreRefeCancelacion}.dbf`);
        link.setAttribute('download', nombreReporte);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);



    }).catch(error => {
        console.log(error)
        //cargar(false)
    }).finally(() => {
        OcultarMensajeCargando();
    })
}

