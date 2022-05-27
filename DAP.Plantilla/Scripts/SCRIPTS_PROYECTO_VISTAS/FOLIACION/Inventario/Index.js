


    function PintarTablaCuentasEncontradasModal(msg) {
        let ninios = $("#cuerpoTabla").children().length



        // If the <ul> element has any child nodes, remove its first child node
        if (ninios > 1) {
            $("#cuerpoTabla").children().remove();
        }


        const CUERPOTABLA = document.getElementById('cuerpoTabla');

        msg.forEach(Dato => {

            //crear un <tr>
            const TR = document.createElement("tr");
            // Creamos el <td> y se adjunta a tr
            let tdFolio = document.createElement("td");
            tdFolio.textContent = Dato; //el textContent del td es el concepto
            TR.appendChild(tdFolio);
            // el td de nómina

            CUERPOTABLA.appendChild(TR);

        });

    }



    ////$(document).ready(function () {



    ////   // let mesSeleccionado = null;
    ////   // const seleccionMes = document.getElementById("SeleccionarMes");
    ////   // seleccionMes.addEventListener("change",
    ////   //     function () {
    ////   //             //mesSeleccionado = null;
    ////   //             mesSeleccionado = this.options[seleccionMes.selectedIndex];

    ////   //         console.log(mesSeleccionado.value);

    ////   //        //let a = href = "/Inventario/GenerarReporteSolicitud?NumMemorandum=19";

    ////   //         //window.open(url);


    ////   //     }
    ////   // );

    ////   //// Descargar
    ////   // const DescargarInventario = document.getElementById("DescargarPDF");
    ////   // DescargarInventario.addEventListener("click",
    ////   //     function () {

    ////   //         //IdIncidencia = 1 es por que esta inhabilitado
    ////   //         //let mes = "{'MesSelecionado':'"+mesSeleccionado.value+"'}";

    ////   //         if (mesSeleccionado != null) {
    ////   //             //seleccionar un mes de quincena

    ////   //             // console.log(mes);
    ////   //             console.log(mesSeleccionado.value);

    ////   //             let url = "/Inventario/GenerarReporteFormasChequesExistentes";
    ////   //             document.getElementById('Descargar').setAttribute('href', url + '?MesSelecionado=' + `${mesSeleccionado.value}`);

    ////   //            //location.reload();
    ////   //            //window.location.reload(true);

    ////   //             $('#DescargarReporte').modal('hide');
    ////   //             $("#SeleccionarMes").val('0')


    ////   //         } else {
    ////   //             //no se selecciono mes

    ////   //             Swal.fire({
    ////   //                 backdrop: true,
    ////   //                 allowEnterKey: false,
    ////   //                 allowOutsideClick: false,
    ////   //                 icon: 'warning',
    ////   //                 title: 'Seleccione un mes de la quincena',
    ////   //                 text: ``
    ////   //             })
    ////   //         }


    ////   //     }
    ////   // );



        









    ////});


let mesADescargar = null;
function DescargarPDFInventario()
{
    mesADescargar = document.getElementById("SeleccionarMes").value;

    if (mesADescargar != 0) {

        let url = "/Inventario/GenerarReporteFormasChequesExistentes";
        document.getElementById('DescargarPDF').setAttribute('href', url + '?MesSelecionado=' + `${mesADescargar}`);

        //location.reload();
        //window.location.reload(true);

        $('#DescargarReporte').modal('hide');
        $("#SeleccionarMes").val('0')



    } else
    {
        MensajeWarningSweet("No se a seleccionado un mes")
    }

}