﻿$(document).ready(function () {
    loadTotalCustomerBookingsChart();
});

function loadTotalCustomerBookingsChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetTotalBookingPieChart",
        type: "GET",
        dataType: "json",
        success: function (data) {
            loadPieChart("#customerBookingsPieChart", data);
            $(".chart-spinner").hide();
        }
    });
}

function loadPieChart(id, data) {
    var chartColors = getChartColorsArray(id);
    var options = {
        series: data.series,
        labels: data.labels,
        colors: chartColors,
        chart: {
            type: "pie",
            width: 350
        },
        stroke: {
            show: false
        },
        legend: {
            position: 'bottom',
            horizontalAlign: 'left',
            labels: {
                colors: "#fff",
                useSeriesColors:true
            }
        }
    }
    var chart = new ApexCharts(document.querySelector(id,), options);
    chart.render();
}