// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Para formularios(POST)
$(document).on("submit", "form", function () {
    console.log("Submit")
    $("#loading").show();
});

//Para peticiones AJAX
$(document).ajaxStart(function () {
    $("#loading").show();
});

$(document).ajaxStop(function () {
    console.log("ajax hide")
    $("#loading").hide();
});

//Para ocultarlo al cargar la página
$(window).on("load", function () {
    console.log("Load")
    $("#loading").hide();
});

//todos los clics que provoquen una redirección
$(document).on("click", "a.loading-link", function (e) {
    /*console.log("loading-link")
    if ($(this).closest(".navbar-nav").length) {
        console.log("loading-link <-")
        return;
    }/**/
    console.log("loading-link")
    $("#loading").show();
});
$(document).ready(function () {
    $('#customTabla').DataTable({
        pageLength: 10,
        responsive: true,
        language: {
            decimal: ",",
            thousands: ".",
            processing: "Procesando...",
            search: "Buscar:",
            lengthMenu: "Mostrar _MENU_ registros",
            info: "_START_ a _END_ de _TOTAL_",
            infoEmpty: "0",
            infoFiltered: "(filtrado de _MAX_ registros)",
            loadingRecords: "Cargando...",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles en la tabla",
            paginate: {
                first: "Inicio",
                previous: "<",
                next: ">",
                last: "Fin"
            }
        }
    });
});

$(document).ready(function () {
    $('#tablaDatos').DataTable({
        pageLength: 10,
        responsive: true,
        language: {
            decimal: ",",
            thousands: ".",
            processing: "Procesando...",
            search: "Buscar:",
            lengthMenu: "Mostrar _MENU_ registros",
            info: "_START_ a _END_ de _TOTAL_",
            infoEmpty: "0",
            infoFiltered: "(filtrado de _MAX_ registros)",
            loadingRecords: "Cargando...",
            zeroRecords: "No se encontraron registros",
            emptyTable: "No hay datos disponibles en la tabla",
            paginate: {
                first: "Inicio",
                previous: "<",
                next: ">",
                last: "Fin"
            }
        }
    });
});