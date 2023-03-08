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

        $.ajax({
            url: "/PracticeTrade/StopRunningPracticeTrade",
            type: "POST",
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
    var getElapsedTimeUrl = $("#getElapsedTimeUrl").val();

    // Start the interval function
    setInterval(function () {
        $.ajax({
            url: getElapsedTimeUrl,
            type: "GET",
            dataType: "json",
            success: function (data) {
                $("#elapsedTime").text(data);
            }
        });
    }, 1000);
}

function checkForUrl() {
    var getElapsedTimeUrl = $("#getElapsedTimeUrl").val();
    if (getElapsedTimeUrl) {
        startInterval();
    } else {
        setTimeout(checkForUrl, 500);
    }
}

$(document).ready(function () {
    checkForUrl();
});

//$(document).ready(function () {

//        // Get the URL of the GetElapsedTime action
//        var getElapsedTimeUrl = $("#getElapsedTimeUrl").val();

//        // Start the interval function
//        setInterval(function () {
//            $.ajax({
//                url: getElapsedTimeUrl,
//                type: "GET",
//                dataType: "json",
//                success: function (data) {
//                    $("#elapsedTime").text(data);
//                    console.log("Inside the interval function");
//                }
//            });
//        }, 1000);  
//});

//$(document).ready(function () {
//    setInterval(function () {
//        $.get("/PracticeTrade/GetElapsedTime", { model: @Html.Raw(Json.Serialize(Model))
//    }, function (data) {
//        $("#elapsedTime").text(data.elapsedTime);
//    });
//}, 1000);
//});

//// This function makes an AJAX request to the server to get the updated timer value.
//function GetTimerValueFromServer() {
//    // Create a new XMLHttpRequest object to make the AJAX request.
//    var xhr = new XMLHttpRequest();

//    // Set the URL for the AJAX request.
//    xhr.open("GET", "/PracticeTrade/GetTimerValue", true);

//    // Set the callback function to be executed when the AJAX request is complete.
//    xhr.onreadystatechange = function () {
//        // Check if the AJAX request is complete and the server has returned a response.
//        if (xhr.readyState === 4 && xhr.status === 200) {
//            // Parse the JSON response to get the elapsed time value.
//            var timerValue = JSON.parse(xhr.responseText).elapsedTime;

//            // Update the timer label on the page with the updated value.
//            UpdateTimerLabelOnPage(timerValue);
//        }
//    };

//    // Send the AJAX request to the server.
//    xhr.send();
//}

//$(document).ready(function () {
//    // Bind an event handler to the form's submit event
//    $("#practicetrade-form").submit(function (event) {
//        // Stop the form from submitting normally
//        event.preventDefault();

//        // Get the form data
//        var form = $(this);
//        var data = form.serialize();

//        // Send the Ajax Request
//        $.ajax({
//            type: form.attr('method'),
//            url: form.attr('action'),
//            data: data,
//            success: function (result) {
//                // Update the backtest-response div with the response
//                $("#backtest-response").html(result);
//            },
//            error: function (xhr, status, error) {
//                // Handle any errors
//                console.log("Error:", error)
//            }
//        });
//    });
//});

