// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
new tempusDominus.TempusDominus(
    document.getElementById('fechaPicker'),
    {
        localization: {
            locale: 'es'
        },
        display: {
            components: {
                calendar: true,
                date: true,
                month: true,
                year: true,
                decades: true,
                clock: false
            }
        }
    }
);

