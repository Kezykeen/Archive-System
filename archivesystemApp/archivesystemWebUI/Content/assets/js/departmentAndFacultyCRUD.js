var facultyDataTable, departmentDataTable;

$(document).ready(function() {
// BEGIN DATATABLES
    // Begin Faculty DataTable
    facultyDataTable = $("#facultyDataTable").DataTable({
        "ajax": {
            "url": "/Faculty/GetFacultyData",
            "type": "GET",
            "datatype": "json"
        },
        "order": [[1, 'asc']],
        "columns": [
            {
                "data": null,
                "searchable": false,
                "orderable": false,
                "width": "30px"
            },
            { "data": "Name" },
            {
                "data": "Id",
                render: function(data) {
                    return `<a class='btn btn-primary m-l-5 m-r-5' href='#' onclick="getAddOrEditPartialView('/Faculty/GetFacultyPartialView', ${
                        data})"><i class='fa fa-pencil m-r-5'></i>Edit</a><a class='btn btn-danger' href='#' onclick='getDeletePartialView("/Faculty/GetDeletePartialView", ${
                        data})'><i class='fa fa-trash-o m-r-5'></i>Delete</a>`;
                },
                "orderable": false,
                "width": "175px"
            }
        ]
    });

    //maintain column ordering on sorting and ordering
    facultyDataTable.on('order.dt search.dt',
        function() {
            facultyDataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function(cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();
    // End Faculty DataTable

    // Begin Department DataTable
    departmentDataTable = $("#departmentDataTable").DataTable({
        "ajax": {
            "url": "/Department/GetDepartmentData",
            "type": "GET",
            "datatype": "json"
        },
        "columnDefs": [
            {
                "searchable": false,
                "orderable": false,
                "width": "30px",
                "targets": 0
            }
        ],
        "order": [[1, 'asc']],
        "columns": [
            { "data": null },
            { "data": "Name" },
            { "data": "Faculty" },
            {
                "data": "Id",
                render: function(data) {
                    return `<a class='btn btn-primary m-l-5 m-r-5' href='#' onclick="getAddOrEditPartialView('/Department/GetDepartmentPartialView', ${
                        data})"><i class='fa fa-pencil m-r-5'></i>Edit</a><a class='btn btn-danger' href='#' onclick='getDeletePartialView("/Department/GetDeletePartialView", ${
                        data})'><i class='fa fa-trash-o m-r-5'></i>Delete</a>`;
                },
                "orderable": false,
                "width": "175px"
            }
        ]
    });

    //maintain column ordering on sorting and ordering
    departmentDataTable.on('order.dt search.dt',
        function() {
            departmentDataTable.column(0, { search: 'applied', order: 'applied' }).nodes().each(function(cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();
   
    // End Department DataTable
// END DATATABLES
});

    //Get Add or Edit View
function getAddOrEditPartialView(url, id) {
    $.get(url,
        { id: id },
        function (res) {
            $("#modalBody").html(res);
            $('#modal').modal({ backdrop: 'static', keyboard: true });
            $("#modal").modal("show");
        });
}

    //Post to Add or Edit Action
function addOrEdit(name, url) {
    var form = $('form[name=' + name + ']');
    form.validate();
    if (!form.valid()) {
        $("#editTxtName-error").addClass("field-validation-error");
        $("#editTxtName").addClass("input-validation-error");
        $("#editTxtName").keyup(function () {
            $(this).removeClass("input-validation-error");
        });
        return;
    } else {
        var data = form.serialize();
        $.post(url,
            data,
            function (response) {
                if (response === "success") {
					$("#modal").modal("hide");
                    //insert alert service
                    //reload both dataTables as both use the same ajax calls
                    facultyDataTable.ajax.reload();
                    departmentDataTable.ajax.reload();
				} else if (response === "Name already exist") {
                    $("#validationMsg").html(response);
                    $("#editTxtName").keydown(function() {
                        $("#validationMsg").html("");
                    });
                } else {
                    alert(response.responseText);
                }
            });
    }
}

    //Get Delete View
function getDeletePartialView(url, id) {
    $.get(url + "/" + id,
        { id: id },
        function (res) {
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
        success: function () {
            $("#modal").modal("hide");
            //insert alert service
            //reload both as since both use the same ajax calls
            facultyDataTable.ajax.reload();
            departmentDataTable.ajax.reload();
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
    return false;
}