var dataTable;

$(document).ready(function() {
// BEGIN DATATABLES
	// Begin Faculty DataTable
	dataTable = $("#facultyDataTable").DataTable({
		"ajax": {
			"url": "/Faculty/GetFacultyData",
			"type": "GET",
			"datatype": "json"
		},
		"columns": [
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
	// End Faculty DataTable

	// Begin Department DataTable
	dataTable = $("#departmentDataTable").DataTable({
		"ajax": {
			"url": "/Department/GetDepartmentData",
			"type": "GET",
			"datatype": "json"
		},
		"columns": [
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
	// End Department DataTable
// END DATATABLES
});

	//Get Add or Edit View
function getAddOrEditPartialView(url, id) {
	$.get(url,
		{ id: id },
		function (res) {
			$("#modalBody").html(res);
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
					dataTable.ajax.reload();
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
			//insert alert function
			dataTable.ajax.reload();
		},
		failure: function (response) {
			alert(response.responseText);
		}
	});
	return false;
}