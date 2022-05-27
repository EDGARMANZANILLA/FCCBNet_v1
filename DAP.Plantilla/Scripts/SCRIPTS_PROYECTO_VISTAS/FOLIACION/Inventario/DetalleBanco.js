

function CargarDetallesFolios() {
    let nombreDetalleBanco = document.getElementById("DetalleBancoCuenta").innerHTML
    $("#table_id").DataTable({
        "dom": 'Blfrtip',
        "buttons": [
            {
                extend: 'copy',
                className: 'btn btn-primary offset-3',
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
        "pageLength": 15,
        "filter": true,
        "responsivePriority": 1,
        "data": null,
        "columns": [

            { "data": "NumeroOrden" },
            { "data": "NumeroContenedor" },
            { "data": "NumeroFolio" },
            { "data": "Incidencia" },
            { "data": "NombreEmpleado" },
            { "data": "FechaIncidencia" }

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
        "lengthMenu": [15, 30, 50],
        "ordering": false

    });



    document.getElementById("CargarDetallesFolios").style.display = "none";
}