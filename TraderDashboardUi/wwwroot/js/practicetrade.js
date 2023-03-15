var select_instrument, $select_instrument;
var select_strategy, $select_strategy;
var startButtonClicked = false;

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

function GetTimerValueFromServer() {
    $.ajax({
        url: "/PracticeTrade/GetTimerValue",
        type: "GET",
        success: function (result) {
            // Parse the JSON response to get the elapsed time value.
            var timerValue = JSON.parse(result).elapsedTime;

            // Update the timer label on the page with the updated value.
            UpdateTimerLabelOnPage(timerValue);
        }
    });
}

//setinterval(function () {
//    $.ajax({
//        url: @url.action("GetElapsedTime", "PracticeTrade"),
//        type: "get",
//        success: function (data) {
//            $("#elapsedtime").text(data);
//        }
//    });
//}, 1000);

$(document).ready(function () {

    // Bind an event handler to the form's submit event
    $("#practicetrade-form").submit(function (event) {
        // Stop the form from submitting normally
        event.preventDefault();

        $("#start-button").prop('disabled', true);
        $("#stop-button").prop('disabled', false);
        console.log("Start Button Has Been Disabled");



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
                $("#practicetrade-response").html(result);
            },
            error: function (xhr, status, error) {
                // Handle any errors
                console.log("Error:", error)
            }
        });
    });
});

$(document).ready(function () {
    $("#stop-button").click(function () {

        $("#start-button").prop('disabled', false);
        $("#stop-button").prop('disabled', true);
        console.log("Inside stop button function.  Start button should be enabled");
        var viewModelData = document.getElementById("viewmodel-data").value;
        $.ajax({
            url: "/PracticeTrade/StopRunningPracticeTrade",
            type: "POST",
            data: viewModelData,
            success: function (result) {
                $("#practicetrade-response").html(result);
            },
            error: function (xhr, status, error) {
                console.log("Error:", error)
            }

        });

    });
});

function startInterval() {
    // Get the URL of the GetElapsedTime action
    var getUpdatePracticeTradeRunningUrl = $("#UpdatePracticeTradeRunningUrl").val();
    console.log(getUpdatePracticeTradeRunningUrl);

    // Start the interval function
    setInterval(function () {
        $.ajax({
            url: getUpdatePracticeTradeRunningUrl,
            type: "GET",
            dataType: "json",
            success: function (data) {
                $("#elapsedTime").text(data.elapsedTime);
                $("#mostRecentCandleDateTime").text(data.candleTime);
                $("#mostRecentCandleOpen").text(data.candleOpen);
                $("#mostRecentCandleHigh").text(data.candleHigh);
                $("#mostRecentCandleLow").text(data.candleLow);
                $("#mostRecentCandleClose").text(data.candleClose);
                $("#mostRecentCandleComplete").text(data.candleComplete);
                $("#inProgressCandleDateTime").text(data.inProgressCandleTime);
                $("#inProgressCandleOpen").text(data.inProgressCandleOpen);
                $("#inProgressCandleHigh").text(data.inProgressCandleHigh);
                $("#inProgressCandleLow").text(data.inProgressCandleLow);
                $("#inProgressCandleClose").text(data.inProgressCandleClose);
                $("#inProgressCandleComplete").text(data.inProgressCandleComplete);

            }
        });
    }, 1000);
}

function checkForUrl() {
    var getElapsedTimeUrl = $("#UpdatePracticeTradeRunningUrl").val();
    if (getElapsedTimeUrl) {
        startInterval();
    } else {
        setTimeout(checkForUrl, 500);
    }
}

$(document).ready(function () {
    checkForUrl();
});

