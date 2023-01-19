
function CargarDetallesFolios() {
    let nombreDetalleBanco = document.getElementById("DetalleBancoCuenta").innerHTML
    MensajeCargando();
  $("#table_id").DataTable({
        "dom": 'Blfrtip',
        "buttons": [
            {
                extend: 'copy',
                className: 'btn btn-primary',
                text: ' <i class="fas fa-copy"></i>  &nbsp  Copiar '

            },
            {
                extend: 'excelHtml5',
                className: 'btn btn-primary ',
                text: '<i class="fas fa-file-download"></i>  &nbsp  EXCEL',
                filename: `Detalle_Banco : ${nombreDetalleBanco}`,
                title: `CONTROL DE FOLIOS DEL BANCO : ${nombreDetalleBanco}`
            },
            {
                extend: 'csvHtml5',
                className: 'btn btn-primary ',
                text: ' <i class="fas fa-file-download"></i>  &nbsp CSV',
                filename: `Detalle_Banco : ${nombreDetalleBanco}`,
                title: `CONTROL DE FOLIOS DEL BANCO : ${nombreDetalleBanco}`
            },
            {
                extend: 'pdfHtml5',
                className: 'btn btn-primary ',
                text: ' <i class="fas fa-download"></i>  &nbsp PDF ',
                filename: `Detalle_Banco : ${nombreDetalleBanco}`,
                title: `CONTROL DE FOLIOS DEL BANCO : ${nombreDetalleBanco}`
            },
            {
                extend: 'print',
                className: 'btn btn-primary ',
                text: '<i class="fas fa-print"></i>  &nbsp Imprimir',
                filename: `Detalle_Banco : ${nombreDetalleBanco}`,
                title: `FOLIOS DEL BANCO : ${nombreDetalleBanco}`
            }
        ],
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "Paginar_CargarDetalleBanco",
            "type": "POST",
            "datatype": "json"
        },
        "pageLength": 25,
        "filter": true,
        "responsivePriority": 1,
        "data": null,
        "columns": [

            { "data": "NumeroOrden" },
            { "data": "NumeroContenedor" },
            { "data": "NumeroFolio" },
            { "data": "Incidencia" },
            { "data": "FechaIncidencia" },
            { "data": "NombreEmpleado" },
            { "data": "FechaAsignacionExterna" },
            {
                render: function (data, type, row) {
                   // console.log(row.TipoNumeroIncidencia);
                    if (row.TipoNumeroIncidencia == 1 || row.TipoNumeroIncidencia == 2) {
                        return `<button class="bg-success btn  text-uppercase text-light text-center "  onclick="RecuperarFolioDeIncidencia( ${row.IdTblDetalle} ,  ${row.NumeroFolio}, '${row.Incidencia}' )"  > <i class="fas fa-undo"></i> </button>`;
                    }
                    return '';
                }
            }

        ],
        "language":
        {
            "processing": "Procesando...",
            "lengthMenu": "Mostrar _MENU_ registros",
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "Ningún dato disponible en esta tabla",
            "infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "infoFiltered": "(filtrado de un total de _MAX_ registros)",
            "search": "Buscar por Numero de Folio:",
            "info": "Mostrando de _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "info": true,
        "searching": true,
        "paging": true,
        "lengthMenu": [25, 30, 50, 100],
        "ordering": false,
        "bDestroy": true

    });

    

    document.getElementById("CargarDetallesFolios").style.display = "none";
    OcultarMensajeCargando();
}











////Utiliza sweet de confirmacion
function RecuperarFolioDeIncidencia(recuperarIdTblDetalle, numeroFolio,  incidencia)
{
    let idDetalleARecuperar = "{'recuperarIdTblDetalle':'" + recuperarIdTblDetalle + "'}";

    if (incidencia == "null")
    {
        incidencia = 'Asignado a chequera externa';
        //console.log('ENtre');
    }


    Swal.fire({
        title: '¿ Seguro que desea recuperar el folio: "'+numeroFolio+'"  el cual se encuentra "'+incidencia+'" ?',
        text: '',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, Confirmo!',
        cancelButtonText: 'No, cancelar!',
        footer: '<a href="#">Contactar al desarrollador?</a>'
    }).then((result) => {
        if (result.isConfirmed) {

            MensajeCargando();
            axios.post('/Inventario/RecuperarFolioDeIncidencia', {
                recuperarIdTblDetalle: recuperarIdTblDetalle
            })
            .then(function (response) {

                if (response.data == true) {
                    MensajeCorrectoConRecargaPagina_UnicaParaRecuperFoliosEnVistaDetalle('El Folio ' + numeroFolio + ' que se encontraba ' + incidencia + ',  fue recuperado exitosamente');

                } else {
                    MensajeErrorSweet('Intente de nuevo mas tarde', 'No se pudo recuperar el folio seleccionado');
                }
                OcultarMensajeCargando();
            })
            .catch(function (error) {
                MensajeErrorSweet('Ocurrio un problema intente de nuevo', error);
                OcultarMensajeCargando();
            });



    

        }
    })
}











// Ejemplo de codigo que actualiza un campo de un datatable sin procesamiento en el servidor ya que cuando se procesa del lado del server, cuando se invoca el .draw() para poder 
// actualizar la vista siempre se ejecuta dentro del servidor y los datos se obtienen en la base de datos
//$(document).on("click", ".RecuperarDetalleFolioSeleccionado", function () {


//    datatableDetallesFolios.cell($(this).parents("tr"), ':eq(0)').data('123').draw();

//});









