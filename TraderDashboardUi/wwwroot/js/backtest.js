var select_instrument, $select_instrument;

$(window).on("load", function () {
    select_instrument = $select_instrument[0] == undefine ? null : $select_instrument[0].selectize;
});

$select_instrument = $("#Instruments").selectize({
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