function formatDate(dateString) {

    if (!dateString)
        return "-";

    let date = new Date(dateString);

    let day = String(date.getDate()).padStart(2, '0');

    let month = String(date.getMonth() + 1).padStart(2, '0');

    let year = date.getFullYear();

    return `${day}-${month}-${year}`;
}
$(document).on('submit', '#frmLeave', function (e) {
    e.preventDefault();
    var form = $(this);

    var formData = new FormData(this);

    $.ajax({
        url: "/Leave/LeaveHistory",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            // Handle the response from the server
            if (response.success) {
                bootstrap.Modal.getInstance(document.getElementById("leaveModal")).hide();
                toastr.success(response.message);

                $("#frmLeave")[0].reset();

                LoadLeaveHistory();

            } else {
                toastr.error(response.message);

                $(".modal-content").html(response);

                $.validator.unobtrusive.parse("#frmLeave");

            }
        },
    });
});
function LoadLeaveHistory(searchValue = '') {
    $('#tblLeave').DataTable(
        {
            processing: true,
            serverSide: true,
            searching: true,
            ordering: false,          // We'll add sorting later
            responsive: true,
            destroy: true,
            pageLength: 10,
            lengthMenu: [
                [10, 25, 50, 100],
                [10, 25, 50, 100]
            ],
            pagingType: "simple_numbers",

            ajax: {
                url: '/Leave/GetLeaveList',
                type: 'POST',
                contentType: "application/json",

                data: function (d) {
                    d.search.value = searchValue;
                    return JSON.stringify(d);

                },

                dataSrc: function (json) {

                    return json.data;

                }

            },
            columnDefs: [

                {
                    className: "text-center",
                    targets: "_all"
                }

            ],
            columns: [

                {
                    data: null,
                    render: function (data, type, row, meta) {
                        return meta.settings._iDisplayStart + meta.row + 1;
                    }
                },

                {
                    data: "startDate",
                    render: function (data) {
                        return formatDate(data);
                    }
                },

                {
                    data: "endDate",
                    render: function (data) {
                        return formatDate(data);
                    }
                },

                {
                    data: "totalDays"
                },

                {
                    data: "leaveTypeName"
                },

                {
                    data: "leaveStatusName",
                    defaultContent: "-",
                    render: function (data) {

                        if (data == "Pending")
                            return '<span class="badge bg-warning">Pending</span>';

                        if (data == "Approved")
                            return '<span class="badge bg-success">Approved</span>';

                        if (data == "Rejected")
                            return '<span class="badge bg-danger">Rejected</span>';

                    }
                },

                {
                    data: "reason"
                },

                {
                    data: "hrComment",
                    defaultContent: "-"
                },

                {
                    data: "attachedFileName",
                    render: function (data) {

                        if (!data)
                            return "-";

                        return `<a href="/Attachments/${data}" target="_blank">View</a>`;
                    }
                },
                //{
                //    data: null,
                //    orderable: false,
                //    render: function (data, type, row, meta) {

                //        console.log("Render called");
                //        console.log(data);
                //        console.log(row);

                //        return "Test";
                //    }
                //}
                {
                    data: "leaveId",
                    orderable: false,
                    render: function (data, type, row) {
                        var Editdisable = "";
                        console.log(row.leaveStatusName);
                        if (row.leaveStatusName === 'Approved' || row.leaveStatusName === 'Rejected') {
                            Editdisable = 'disabled'
                        }
                        return `<button class="btn btn-sm btn-outline-primary editLeave ${Editdisable}" data-id="${data}">Edit</button>`;
                    }

                }
            ],
            language: {

                processing: "Loading...",
                paginate: {
                    previous: "← Previous",
                    next: "Next →"
                },
                emptyTable: "No leave history found.",
                searchPlaceholder: "Search leave..."
            }


        }
    );
}

$(document).on("click", ".editLeave", function () {

    let leaveId = $(this).data("id");

    $.ajax({

        url: "/Leave/GetLeave",

        type: "GET",

        data: {
            leaveId: leaveId
        },

        success: function (response) {

            $("#LeaveId").val(response.leaveId);

            $("#LeaveType").val(response.leaveType);

            $("#StartDate").val(response.startDate.substring(0, 10));

            $("#EndDate").val(response.endDate.substring(0, 10));

            $("#Reason").val(response.reason);

            var modal = new bootstrap.Modal($("#leaveModal"));

            modal.show();

        }

    });

});

$("#btnApplyLeave").click(function () {

    $(".modal-content").load("/Leave/GetEmptyLeaveForm", function () {

        var modal = new bootstrap.Modal(document.getElementById("leaveModal"));

        modal.show();

    });

});

$(document).on('click', '#Rejected', function (e) {

    $('#cnr-tblEmployeesLeave').removeClass('d-none');
    LoadLeaveHistory('Rejected');
});
$(document).on('click', '#Approved', function (e) {

    $('#cnr-tblEmployeesLeave').removeClass('d-none');
    LoadLeaveHistory('Approved');
});
$(document).on('click', '#Pending', function (e) {

    $('#cnr-tblEmployeesLeave').removeClass('d-none');
    LoadLeaveHistory('Pending');
});