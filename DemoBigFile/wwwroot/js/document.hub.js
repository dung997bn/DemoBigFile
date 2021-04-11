"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/documentHub").build();

connection.start().then(function () {
    console.log("SignalR connected!")
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ShowProgress", function (rowCount) {
    if (rowCount) {
        var progressBar = document.getElementById("progress-bar")
        progressBar.style.display = "block"

        var myBar = document.getElementById("myBar");
        myBar.style.width = "1%";
    }
});


connection.on("UpdateProgress", function (rowCount, current) {
    console.log(rowCount, current)
    var myBar = document.getElementById("myBar");
    var pecent = Math.floor((current / rowCount) * 100)
    console.log(pecent)
    myBar.style.width = pecent + "%";
});


connection.on("StartBulkCopy", function () {
    var progressBar = document.getElementById("progress-bar-bulk-copy")
    progressBar.style.display = "block"

    var headerText = document.getElementById("status-bulk-copy");
    headerText.innerText = "Starting Bulk Copy"

});

connection.on("CompleteConvertToDataTable", function () {
    var headerText = document.getElementById("status-bulk-copy");
    headerText.innerText = "Completed Convert To DataTable"
});

connection.on("CompleteWriteToServer", function () {
    var headerText = document.getElementById("status-bulk-copy");
    headerText.innerText = "Completed Write To Server (Donation Master)"
});


connection.on("CompleteImport", function () {
    var headerText = document.getElementById("status-bulk-copy");
    headerText.innerText = "Completed Import Data!"
});