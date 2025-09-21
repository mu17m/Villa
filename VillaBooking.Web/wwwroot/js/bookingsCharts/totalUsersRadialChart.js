$(document).ready(function () {
    loadTotalUsersRadialChart();
});

function loadTotalUsersRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetTotalUsersForChartData",
        type: "GET",
        dataType: "json",
        success: function (data) {
            document.querySelector("#spanTotalUserCount").innerHTML = data.totalCount;

            var sectionCurrentCount = document.createElement("span");
            if (data.hasRationIncreased) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i> <span>' + data.countInCurrentMonth +'</span>';
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i> <span> ' + data.countInCurrentMonth + '</span>';
            }
            document.querySelector("#sectionUserCount").append(sectionCurrentCount);
            $(".chart-spinner").hide();
            loadRedialBarChart("#totalUserRadialChart", data);
        }
    });
}
