var select_instrument, $select_instrument;
var select_strategy, $select_strategy;

$(window).on("load", function () {
    select_instrument = $select_instrument[0] == undefine ? null : $select_instrument[0].selectize;
    select_strategy = $select_strategy[0] == undefine ? null : $select_strategy[0].selectize;
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