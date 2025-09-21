﻿$(document).ready(function () {
    loadTotalRevenuesRadialChart();
});

function loadTotalRevenuesRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetTotalRevenuesForChartData",
        type: "GET",
        dataType: "json",
        success: function (data) {
            document.querySelector("#spanTotalRevenueCount").innerHTML = data.totalCount;

            var sectionCurrentCount = document.createElement("span");
            if (data.hasRationIncreased) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i> <span>' + data.countInCurrentMonth +'</span>';
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i> <span> ' + data.countInCurrentMonth + '</span>';
            }
            document.querySelector("#sectionRevenueCount").append(sectionCurrentCount);
            $(".chart-spinner").hide();
            loadRedialBarChart("#totalRevenueRadialChart", data);
        }
    });
}
