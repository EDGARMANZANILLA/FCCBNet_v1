
    //*********************************************************************************//
    //*********************************************************************************//
    //*********************************************************************************//
    //AL CHILE NO SE PARA QUE SIRVE ESTA FUNCION PERO AL PARECER NO HACE NADA 
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