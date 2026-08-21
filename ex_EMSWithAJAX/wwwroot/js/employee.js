$(document).ready(function () {
    $("#actionDropdown").change(function () {
        let selectedValue = $(this).val();
        if (selectedValue === "list") {

            loadEmployeeList();

        }
        else if (selectedValue === "add") {
            loadAddEmployeeForm();
        }
        else {
            $("#employeeContent").html("");
        }
    });

    $("#employeeContent").on("change", "#countryId", function () {
        let countryId = $(this).val();

        let stateDropdown = $("#stateId");
        let cityDropdown = $("#cityId");

        stateDropdown.empty();
        cityDropdown.empty();

        cityDropdown.append(
            '<option value="">-- Select City --</option>'
        );

        cityDropdown.prop("disabled", true);

        stateDropdown.append(
            '<option value="">-- Select State --</option>'
        );
        if (!countryId) {

            stateDropdown.prop("disabled", true);

            return;
        }

        $.ajax({

            url: "/Employee/GetStates",

            type: "GET",

            data: {
                countryId: countryId
            },

            success: function (response) {

                $.each(response, function (index, state) {

                    stateDropdown.append(
                        `<option value="${state.stateId}">
                        ${state.stateName}
                    </option>`
                    );

                });

                stateDropdown.prop("disabled", false);
            },

            error: function (xhr) {

                console.log(xhr);

                alert("Unable to load states.");

            }
        });
    });

    $("#employeeContent").on("change", "#stateId", function () {
        let stateId = $(this).val();

        let cityDropdown = $("#cityId");


        cityDropdown.empty();

        cityDropdown.append(
            '<option value="">-- Select City --</option>'
        );

        cityDropdown.prop("disabled", true);

        if (!countryId) {

            cityDropdown.prop("disabled", true);

            return;
        }

        $.ajax({

            url: "/Employee/GetCities",

            type: "GET",

            data: {
                stateId: stateId
            },

            success: function (response) {

                $.each(response, function (index, city) {

                    cityDropdown.append(
                        `<option value="${city.cityId}">
                        ${city.cityName}
                    </option>`
                    );

                });

                cityDropdown.prop("disabled", false);
            },

            error: function (xhr) {

                console.log(xhr);

                alert("Unable to load states.");

            }
        });
    });

    $("#employeeContent").on("click", "#btnSaveEmployee", function () {
        let form = $("#employeeForm");
        if (!form.valid()) {
            return
        }
        /*let formData = form.serialize();*/

        let employee = {
            name: $("#Name").val().trim(),

            gender: $("input[name='Gender']:checked").val(),

            email: $("#Email").val().trim(),

            dateOfBirth: $("#DateOfBirth").val(),

            salary: Number($("#Salary").val()),

            address: $("#Address").val().trim(),

            countryId: Number($("#countryId").val()),

            stateId: Number($("#stateId").val()),

            cityId: Number($("#cityId").val())
        };

        $.ajax({

            url: "/Employee/SaveEmployee",

            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(employee),

            success: function (response) {

                if (response.success) {

                    alert(response.message);

                    $("#actionDropdown").val("list");

                    loadEmployeeList();
                }

            },

            error: function (xhr) {

                console.log(xhr);

                if (xhr.status === 409) {

                    $("#emailDuplicateError")
                        .text(xhr.responseJSON.message);

                    $("#Email").addClass("is-invalid");

                    return;
                }

                if (xhr.status === 400) {

                    alert(xhr.responseJSON.message);

                    return;
                }

            }

        });
    });

    $("#employeeContent").on("blur","#Email",
        function () {

            checkDuplicateEmail();

        }
    );
});
function loadEmployeeList() {

    let html = `
            <div class="card">

                <div class="card-header">
                    <h5 class="mb-0">Employee List</h5>
                </div>

                <div class="card-body">

                    <table id="employeeTable"
                           class="table table-bordered table-striped">

                        <thead>
                            <tr>
                                <th>Employee Id</th>
                                <th>Name</th>
                                <th>Gender</th>
                                <th>Email</th>
                                <th>Date Of Birth</th>
                                <th>Salary</th>
                            </tr>
                        </thead>

                        <tbody>
                        </tbody>

                    </table>

                </div>

            </div>
        `;

    $("#employeeContent").html(html);

    loadEmployees();
}
function loadEmployees() {
    $.ajax({
        url: "/Employee/GetEmployeeList",
        type: "GET",
        success: function (respose) {
            //console.log(respose);
            //let rows = "";
            //$.each(respose, function (index, employee) {
            //    rows += `
            //        <tr>
            //                <td>${employee.employeeId}</td>
            //                <td>${employee.name}</td>
            //                <td>${employee.gender}</td>
            //                <td>${employee.email}</td>
            //                <td>${employee.dateOfBirth}</td>
            //                <td>${employee.salary}</td>

            //        </tr>`;
            //});

            //$("#employeeTable tbody").html(rows);
            $("#employeeTable").DataTable({
                data: respose,
                destory: true,
                columns: [
                    {
                        data: "employeeId"
                    },
                    {
                        data: "name"
                    },
                    {
                        data: "gender"
                    },
                    {
                        data: "email"
                    },
                    {
                        data: "dateOfBirth",
                        render: function (data) {
                            return formatdate(data);
                        }
                    },
                    {
                        data: "salary",
                        render: function (data) {
                            return Number(data).toFixed(2);
                        }
                    }
                ]
            });
        },
        error: function (xhr) {

            console.log(xhr);

            alert("Unable to load employees.");
        }

    });
}
function formatdate(dateValue) {
    if (!dateValue) {
        return ""
    }
    let date = new Date(dateValue);
    return date.toLocaleDateString("en-GB");
}
function loadAddEmployeeForm() {
    $.ajax({
        url: "/Employee/GetEmployeeForm",
        type: "GET",
        success: function (response) {

            $("#employeeContent").html(response);
            let form = $("#employeeForm");
            $.validator.unobtrusive.parse(form);

        },
        error: function (xhr) {

            console.log(xhr);

            alert("Unable to load employee form.");

        }
    });
}
function checkDuplicateEmail() {
    let email = $("#Email").val().trim();
    let employeeId = $("#EmployeeId").val() || 0;

    if (!email) {
        $("#emailDuplicateError").text("");
        return;
    }
    $.ajax({

        url: "/Employee/CheckEmail",

        type: "GET",

        data: {
            email: email,
            employeeId: employeeId
        },

        success: function (response) {
            console.log(response);
            if (response.exists) {

                $("#emailDuplicateError")
                    .text("Email already exists.");

                $("#Email").addClass("is-invalid");

            }
            else {

                $("#emailDuplicateError")
                    .text("");

                $("#Email").removeClass("is-invalid");

            }

        },

        error: function (xhr) {

            console.log(xhr);

        }

    });
}