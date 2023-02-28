var select_instrument, $select_instrument;
var select_strategy, $select_strategy;

$(window).on("load", function () {
    select_instrument = $select_instrument[0] == undefined ? null : $select_instrument[0].selectize;
    select_strategy = $select_strategy[0] == undefined ? null : $select_strategy[0].selectize;
});

$select_instrument = $("#Instruments").selectize({
    onChange: function (value) {
        // set initial display after any change
        initializeView();

        if (!value.length) return;
    }
})

$select_strategy = $("#Strategies").selectize({
    onChange: function (value) {
        // set initial display after any change
        initializeView();

        if (!value.length) return;
    }
})

$(function () {
    $("#startdatepicker").datepicker();
});
$(function () {
    $("#enddatepicker").datepicker();
});

var table = new Tabulator("#tabulator_table", {});
console.log("Inside script");

$(document).ready(function () {
    // Bind an event handler to the form's submit event
    $("#backtest-form").submit(function (event) {
        // Stop the form from submitting normally
        event.preventDefault();

        // Get the form data
        var form = $(this);
        var data = form.serialize();

        // Send the Ajax Request
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: data,
            success: function (result) {
                // Update the backtest-response div with the response
                $("#backtest-response").html(result);
            },
            error: function (xhr, status, error) {
                // Handle any errors
                console.log("Error:", error)
            }
        });
    });
});