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
    console.log("Start")
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
$(document).on("click", "a", function (e) {
    console.log($(this).closest(".navbar-nav"), e.target)
    if ($(this).closest(".navbar-nav").length) {
        return;
    }
    //$("#loading").show();
});

$(document).ready(function () {
    console.log("Validacion")
    $('#tablaDatos').DataTable({
        pageLength: 25,
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