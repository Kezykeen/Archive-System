﻿var facultyDataTable, departmentDataTable;
const Toast = Swal.mixin({
  toast: true,
  position: "top",
  showConfirmButton: false,
  timer: 2000,
  timerProgressBar: true,
  didOpen: (toast) => {
    toast.addEventListener("mouseenter", Swal.stopTimer);
    toast.addEventListener("mouseleave", Swal.resumeTimer);
  },
});

$(document).ready(function () {
  // BEGIN DATATABLES
  // Begin Faculty DataTable
  facultyDataTable = $("#facultyDataTable").DataTable({
    ajax: {
      url: "/Faculty/GetFacultyData",
      type: "GET",
      datatype: "json",
    },
    processing: true,
    autoWidth: false,
    language: {
      processing: "Data loading...",
    },
    order: [[1, "asc"]],
    columns: [
      {
        data: null,
        searchable: false,
        orderable: false,
        width: "20px",
      },
      { data: "Name" },
      {
        data: "Id",
        render: function (data) {
          return `<a class='btn btn-sm btn-outline-primary m-l-5 m-r-5' href='#' onclick="getAddOrEditPartialView('/Faculty/GetFacultyPartialView', ${data})"><i class='fa fa-pencil m-r-5'></i>Edit</a><a class='btn btn-sm btn-outline-danger' href='#' onclick='getDeletePartialView("/Faculty/GetDeletePartialView", ${data})'><i class='fa fa-trash-o m-r-5'></i>Delete</a>`;
        },
        orderable: false,
        width: "146px",
      },
    ],
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
      processing: '<i class="fa fa-spin fa-spinner"></i> Loading...',
    },
    ajax: {
      url: "/Department/GetDepartmentData",
      type: "GET",
      datatype: "json",
    },
    columnDefs: [
      {
        searchable: false,
        orderable: false,
        width: "20px",
        targets: 0,
      },
    ],
    order: [[1, "asc"]],
    columns: [
      { data: null },
      { data: "Name" },
      { data: "Faculty" },
      {
        data: "Id",
        render: function (data) {
          return `<a class='btn btn-sm btn-outline-primary m-l-5 m-r-5' href='#' onclick="getAddOrEditPartialView('/Department/GetDepartmentPartialView', ${data})"><i class='fa fa-pencil m-r-5'></i>Edit</a><a class='btn btn-sm btn-outline-danger' href='#' onclick='getDeletePartialView("/Department/GetDeletePartialView", ${data})'><i class='fa fa-trash-o m-r-5'></i>Delete</a>`;
        },
        orderable: false,
        width: "146px",
      },
    ],
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
  // END DATATABLES
});

//Get Add or Edit View
function getAddOrEditPartialView(url, id) {
  $.get(url, { id: id }, function (res) {
    $("#modalBody").html(res);
    $("#modal").modal({ backdrop: "static", keyboard: true });
    $("#modal").modal("show");
    $.validator.unobtrusive.parse(".addOrUpdateForm");

    if ($("#modal .select").length > 0) {
      $("#modal .select").select2({
        minimumResultsForSearch: -1,
        width: "100%",
      });
    }
  });
}

//Post to Add or Edit Action
function addOrEdit(name, url) {
  var form = $("form[name=" + name + "]");
  if (!form.valid()) {
    return;
  } else {
    var data = form.serialize();
    $.post(url, data, function (response) {
      if (response === "success") {
        $("#modal").modal("hide");
        Toast.fire({
          icon: "success",
          title: "Update Successful",
        });
        //reload both dataTables as both use the same ajax calls
        facultyDataTable.ajax.reload();
        departmentDataTable.ajax.reload();
      } else {
        $("#modal").modal("hide");
        Toast.fire({
          title: "Update Failed",
          icon: "error",
        });
      }
    });
  }
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
    success: function () {
      $("#modal").modal("hide");
      Toast.fire({
        icon: "success",
        title: "Deleted Successfully",
      });
      //reload both as since both use the same ajax calls
      facultyDataTable.ajax.reload();
      departmentDataTable.ajax.reload();
    },
    failure: function (response) {
      $("#modal").modal("hide");
      Toast.fire({
        title: "Delete failed",
        icon: "error",
      });
    },
  });
  return false;
}