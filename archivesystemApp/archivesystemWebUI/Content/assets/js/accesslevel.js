
$("document").ready(function () {
    $("#table").DataTable();
});


function GetForm(url, id, formId) {
    $.get(url,
        { id: id },
        function (response) {
            $("#modalContent").html(response);
            $("#modal").modal("show");
            $.validator.unobtrusive.parse(formId);
        }
    )
}


// AddAccessLevel AJAX callback
function OnAddAccessSuccess(response) {
    if (response == "success") {

        $('#modal').modal('hide');

        Swal.fire({
            position: 'top-end',
            icon: 'success',
            title: 'Access Level was succesfully created!',
            showConfirmButton: false,
            timer: 2000
        });

        window.location.reload();

    } else if (response == "failure") {
        $("#modal").html(response);
        $("#modal").modal("show");
        $.validator.unobtrusive.parse("#CreateAccessLevelForm");
    }
}


// EditAccessLevel AJAX callback
function OnEditAccessSuccess(response) {
    if (response == "success") {

        $('#modal').modal('hide');

        Swal.fire({
            position: 'top-end',
            icon: 'success',
            title: 'Access Level was succesfully updated!',
            showConfirmButton: false,
            timer: 2000
        });

        window.location.reload();

    } else if (response == "failure") {

        $("#modal").html(response);
        $("#modal").modal("show");
        $.validator.unobtrusive.parse("#EditAccessLevelForm");
    }
}

// AddUserAccess AJAX callback
function OnAddUserSuccess(response) {
    if (response == "success") {

        $('#modal').modal('hide');

        Swal.fire({
            position: 'top-end',
            icon: 'success',
            title: 'Success: User has been given an Access!',
            showConfirmButton: false,
            timer: 2000
        });

        window.location.reload();

    } else if (response == "failure") {
 
        $("#modal").html(response);
        $("#modal").modal("show");
        $.validator.unobtrusive.parse("#AddUserAccessForm");
    }
}

// EditUserAccess AJAX callback
function OnEditUserSuccess(response) {
    if (response == "success") {

        $('#modal').modal('hide');

        Swal.fire({
            position: 'top-end',
            icon: 'success',
            title: 'User Access Updated!',
            showConfirmButton: false,
            timer: 2000
        });

        window.location.reload();

    } else if (response == "failure") {

        $("#modal").html(response);
        $("#modal").modal("show");
        $.validator.unobtrusive.parse("#EditUserAccessForm");
    }
}


// DeleteUserAccess AJAX callback
function OnDeleteUserSuccess(response) {
    if (response == "success") {

        $('#modal').modal('hide');

        Swal.fire({
            position: 'top-end',
            icon: 'success',
            title: 'User has been removed!',
            showConfirmButton: false,
            timer: 2000
        });

        window.location.reload();

    } else if (response == "failure") {

        $("#modal").html(response);
        $("#modal").modal("show");
    }
}

function OnFailure(error) {
    bootbox.alert("Internal server error. Please contact the Admin.");
    $("#modal").modal("hide");
}