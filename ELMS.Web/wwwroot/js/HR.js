function LoadEmployeesLeave(searchValue = "") {
    $('#tblEmployeesLeave').DataTable(
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
                url: '/HR/GetEmployeesLeaveList',
                type: 'POST',
                contentType: "application/json",
                data: function (d) {
                    d.search.value = searchValue;
                    return JSON.stringify(d);
                },
                dataSrc: function (json) {
                    console.log(json.data);
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
                    data: "userName"
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
                    data: "leaveId",
                    orderable: false,
                    render: function (data, type, row) {
                        var approveDisabled = "";
                        var rejectDisabled = "";

                        if (row.leaveStatusName === "Approved") {
                            approveDisabled = "disabled";
                        }
                        if (row.leaveStatusName === "Rejected") {
                            rejectDisabled = "disabled";
                        }
                        return `<button class="btn btn-sm btn-outline-success approveLeave ${approveDisabled}" data-id="${data}">Approve</button> ` + `<button class="btn btn-sm btn-outline-danger rejectLeave ${rejectDisabled}" data-id="${data}">Reject</button>`;
                    }

                }
            ]
        }
    );
}

$(document).on('click', '#ttlleave', function (e) {

    $('#cnr-tblEmployeesLeave').removeClass('d-none');
    LoadEmployeesLeave();
});
$(document).on('click', '#ttlRejected', function (e) {

    $('#cnr-tblEmployeesLeave').removeClass('d-none');
    LoadEmployeesLeave('Rejected');
});
$(document).on('click', '#ttlApproved', function (e) {

    $('#cnr-tblEmployeesLeave').removeClass('d-none');
    LoadEmployeesLeave('Approved');
});
$(document).on('click', '#ttlPending', function (e) {

    $('#cnr-tblEmployeesLeave').removeClass('d-none');
    LoadEmployeesLeave('Pending');
});
//Approve
$(document).on('click', '.approveLeave', function (e) {
    var updateLeaveStatus = {
        LeaveId: $(this).data('id'), ActionType: 'Approved'
    }
    $.ajax({
        url: '/HR/UpdateLeaveStatus',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(updateLeaveStatus),
        success: function (response) {
            console.log(response);
            if (response.success) {
                toastr.success(response.message);
                LoadEmployeesLeave();
            }
            else {
                toastr.error(response.message);
            }
        }
    }
    );
});

//Reject

$(document).on('click', '.rejectLeave', function () {
    $('#txtleavereason').val('');
    $('#errortxtleavereason').text('');
    $('#leaveReason').data('leaveid', $(this).data('id'));
    $('#leaveReason').modal('show');
});

$('#btnfinalreject').on('click', function () {

    var reason = $('#txtleavereason').val().trim();
    var selectedLeaveId = $('#leaveReason').data('leaveid');
    $('#errortxtleavereason').text('');

    if (selectedLeaveId == 0) {
        toastr.error("Invalid leave selected.");
        return;
    }

    if (reason == "") {
        $('#errortxtleavereason').text("Please enter reject reason.");
        return;
    }

    var updateLeaveStatus = {
        LeaveId: selectedLeaveId,
        ActionType: "Rejected",
        HRComment: reason
    };

    $.ajax({
        url: '/HR/UpdateLeaveStatus',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(updateLeaveStatus),
        success: function (response) {

            if (response.success) {

                toastr.success(response.message);

                bootstrap.Modal.getInstance(document.getElementById('leaveReason')).hide();

                LoadEmployeesLeave();
            }
            else {
                toastr.error(response.message);
            }
        },
        error: function () {
            toastr.error("Something went wrong.");
        }
    });

});