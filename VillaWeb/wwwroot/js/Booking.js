var dataTable;

$(function () {
    
    loadDataTable();
})

function loadDataTable() {
    const url = new URLSearchParams(window.location.search);
    var status = url.get('status');
    dataTable = $('#tblBookings').DataTable({
        "ajax": { url: '/Booking/GetAllBookings?status='+ status},
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "10%" },
            { "data": "phone", "width": "10%" },
            { "data": "email", "width": "15%" },
            { "data": "status", "width": "10%" },
            { "data": "checkInDate", "width": "10%" },
            { "data": "nights", "width": "10%" },
            { "data": "totalCost", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group">
                                <a href="/Booking/Details?Id=${data}" class='btn btn-outline-warning mx-2'>
                                   <i class="bi bi-pencil-square"></i> Details
                                </a>
                            </div>`
                }
            }
        ],

    });

}
