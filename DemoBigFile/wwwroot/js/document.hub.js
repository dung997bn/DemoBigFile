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