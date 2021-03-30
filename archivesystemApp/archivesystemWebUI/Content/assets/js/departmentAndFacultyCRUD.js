var facultyDataTable, departmentDataTable;
const Toast = Swal.mixin({
    toast: true,
    position: "top",
    showConfirmButton: false,
    timer: 2000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener("mouseenter", Swal.stopTimer);
        toast.addEventListener("mouseleave", Swal.resumeTimer);
    }
});

$(document).ready(function () {
    // BEGIN DATATABLES
    // Begin Faculty DataTable
    facultyDataTable = $("#facultyDataTable").DataTable({
        ajax: {
            url: "/Faculty/GetFacultyData",
            type: "GET",
            datatype: "json"
        },
        processing: true,
        autoWidth: false,
        language: {
            processing: '<i class="fa fa-spin fa-spinner"></i> Loading...'
        },
        order: [[1, "asc"]],
        columns: [
            {
                data: null,
                searchable: false,
                orderable: false,
                width: "20px"
            },
            { data: "Name" },
            {
                data: "Id",
                render: function (data) {
                    return `<a class='btn btn-sm btn-outline-primary' href='/Faculty/ViewAllDepartmentsInFaculty/${data
                        }'>View Departments</a>`;
                },
                searchable: false,
                orderable: false,
                width: "137px"
            },
            {
                data: "Id",
                render: function (data) {
                    return `<a class='m-l-5 m-r-10' href='#' data-toggle="tooltip" data-placement="bottom" title="Edit" onclick="getEditPartialView('/Faculty/GetEditFacultyPartialView', ${data
                        })"><i class='fa fa-pencil m-r-5'></i></a><a href='#' data-toggle="tooltip" data-placement="bottom" title="Delete" onclick='getDeletePartialView("/Faculty/GetDeletePartialView", ${data
                        })'><i class='fa fa-trash-o'></i></a>`;
                },
                orderable: false,
                width: "50px"
            }
        ]
    });

    //maintain column ordering on sorting and ordering
    facultyDataTable
        .on("order.dt search.dt", function () {
            facultyDataTable
                .column(0, { search: "applied", order: "applied" })
                .nodes()
                .each(function (cell, i) {
                    cell.innerHTML = i + 1;
                });
        })
        .draw();
    // End Faculty DataTable

    // Begin Department DataTable
    departmentDataTable = $("#departmentDataTable").DataTable({
        processing: true,
        autoWidth: false,
        language: {
            processing: '<i class="fa fa-spin fa-spinner"></i> Loading...'
        },
        ajax: {
            url: "/Department/GetDepartmentData",
            type: "GET",
            datatype: "json"
        },
        columnDefs: [
            {
                searchable: false,
                orderable: false,
                width: "20px",
                targets: 0
            },
            {
                searchable: false,
                orderable: false,
                width: "88px",
                targets: 3
            }
        ],
        order: [[1, "asc"]],
        columns: [
            { data: null },
            { data: "Name" },
            { data: "Faculty" },
            {
                data: "Id",
                render: function(data) {
                    return `<a class='btn btn-sm btn-outline-primary' href='/Department/ViewAllUsersInDept/${data}'>View Users</a>`;
                }
            },
            {
                data: "Id",
                render: function (data) {
                    return `<a class='m-l-5 m-r-10' href='#' data-toggle="tooltip" data-placement="bottom" title="Edit" onclick="getEditPartialView('/Department/GetEditDepartmentPartialView', ${
                        data
                        })"><i class='fa fa-pencil m-r-5'></i></a><a href='#' data-toggle="tooltip" data-placement="bottom" title="Delete" onclick='getDeletePartialView("/Department/GetDeletePartialView", ${
                        data
                        })'><i class='fa fa-trash-o'></i></a>`;
                },
                orderable: false,   
                width: "50px"
            }
        ]
    });

    //maintain column ordering on sorting and ordering
    departmentDataTable
        .on("order.dt search.dt", function () {
            departmentDataTable
                .column(0, { search: "applied", order: "applied" })
                .nodes()
                .each(function (cell, i) {
                    cell.innerHTML = i + 1;
                });
        })
        .draw();

    // End Department DataTable

    $('[data-toggle="tooltip"]').tooltip();
    // END DATATABLES
});

//Get Edit View
function getEditPartialView(url, id) {
    $.get(url, { id: id }, function (res) {
        $("#modalBody").html(res);
        $("#modal").modal({ backdrop: "static", keyboard: true });
        $("#modal").modal("show");
        $.validator.unobtrusive.parse("#UpdateForm");

        if ($("#modal .select").length > 0) {
            $("#modal .select").select2({
                minimumResultsForSearch: -1,
                width: "100%"
            });
        }
    });
}

//Get Add View
function getAddPartialView(url) {
    $.get(url,
        function(res) {
            $("#modalBody").html(res);
            $("#modal").modal({ backdrop: "static", keyboard: true });
            $("#modal").modal("show");
            $.validator.unobtrusive.parse("#AddForm");

            if ($("#modal .select").length > 0) {
                $("#modal .select").select2({
                    minimumResultsForSearch: -1,
                    width: "100%"
                });
            }
        });
}

function departmentOnSuccess(response) {
    if (response.success) {
        $("#modal").modal("hide");
        Toast.fire({
            icon: "success",
            title: "Update Successful"
        });
        departmentDataTable.ajax.reload();
    } else {
        $("#modal").modal("hide");
        Toast.fire({
            title: "Update Failed",
            icon: "error"
        });
    }
}

function departmentOnFailure() {
    $("#modal").modal("hide");
    Toast.fire({
        title: "Update Failed",
        icon: "error"
    });
}

function facultyOnSuccess(response) {
    if (response.success) {
        $("#modal").modal("hide");
        Toast.fire({
            icon: "success",
            title: "Update Successful"
        });
        facultyDataTable.ajax.reload();
    } else {
        $("#modal").modal("hide");
        Toast.fire({
            title: "Update Failed",
            icon: "error"
        });
    }
}

function facultyOnFailure() {
    $("#modal").modal("hide");
    Toast.fire({
        title: "Update Failed",
        icon: "error"
    });
}

//Get Delete View
function getDeletePartialView(url, id) {
    $.get(url + "/" + id, { id: id }, function (res) {
        $("#modalBody").html(res);
        $("#modal").modal("show");
    });
}
//Post to Delete Action
function confirmDelete(url) {
    var id = $("#txtDeleteId").val();
    $.ajax({
        url: url + "/" + id,
        type: "POST",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                $("#modal").modal("hide");
                Toast.fire({
                    icon: "success",
                    title: "Deleted Successfully"
                });
                //reload both as since both use the same ajax calls
                facultyDataTable.ajax.reload();
                departmentDataTable.ajax.reload();
            } else if (response.prohibited) {
                $("#modal").modal("hide");
                Toast.fire({
                    title: response.errorMessage,
                    icon: "error"
                });
            } else {
                $("#modal").modal("hide");
                Toast.fire({
                    title: "Delete failed",
                    icon: "error"
                });
            }
        },
        failure: function () {
            $("#modal").modal("hide");
            Toast.fire({
                title: "Delete failed",
                icon: "error"
            });
        }
    });
    return false;
}
