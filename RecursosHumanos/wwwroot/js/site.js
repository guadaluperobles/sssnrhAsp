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
    $("#loading").show();
});

$(document).ready(function () {
    console.log("Validacion")
    $('#tablaDatos').DataTable({
        language: {
            url: '//cdn.datatables.net/plug-ins/2.3.4/i18n/es-MX.json'
        },
        pageLength: 25,
        responsive: true
    });
});