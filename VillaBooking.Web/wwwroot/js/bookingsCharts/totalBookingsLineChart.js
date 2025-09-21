$(document).ready(function () {
    loadTotalBookingsLineChart();
});

function loadTotalBookingsLineChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetUsersAndBookingsLineChartData",
        type: "GET",
        dataType: "json",
        success: function (data) {
            loadLineChart("newUsersBookingsLineChart", data);
            $(".chart-spinner").hide();
        }
    });
}

function loadLineChart(id, data) {
    var chartColors = getChartColorsArray(id);
    var options = {
        series: data.series,
        colors: chartColors,
        chart: {
            height: 350,
            type: "line"
        },
        stroke: {
            curve: "smooth",
            width:2
        },
        markers: {
            size: 3,
            strockWidth:0,
            hover: {
                size:7
            }
        },
        xaxis: {
            categories: data.categories,
            labels: {
                style: {
                    colors: "#fff"
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: "#fff"
                }
            }
        },
        legend: {
            labels: {
                colors: "#fff"
            }
        }
    }
    var chart = new ApexCharts(document.querySelector("#" + id,), options);
    chart.render();
}