
$("document").ready(function () {
    $("#user-roles-table").DataTable();
});


async function addOrEditRole(url) {
    let verificationToken = document.getElementsByName("__RequestVerificationToken")[0].value
    let name = document.getElementById("role-name").value;
    let newName = document.getElementById("role-newName").value;
   
    if (!newName) {
        addErrors("manageRole", isnullNamError = true)
    }
    else {
        let resp = await postData(url, name, newName, verificationToken)
       
        if (resp.status === 200) { location.reload() }
        else if (resp.status === 400) {
            addErrors("manageRole", isnullNamError = false, isnameAlreadyExist = true, isUnknown = false)
        }
        else {
            addErrors("manageRole", isnullNamError = false, isnameAlreadyExist = false, isUnknown = true)
        }

    }

}

function addErrors(formId, isnullNamError=false,isnameAlreadyExist=false,isUnknown=false) {
    let form = document.getElementById(formId);
    let errorMessage = document.createElement("div");
    
    if (form.childNodes.length > 11) {
        form.removeChild(form.childNodes[0])
    }
    errorMessage.innerHTML =
        `<div class='validation-summary-errors'>
            <ul>
                ${ isnullNamError ? '<li>Role name field is required</li>' : ""}
                ${isnameAlreadyExist ? "<li>Role name already exist</li>" : ""} 
                ${isUnknown ? "<li>Server Error: something went wrong</li>" : ""}
            </ul>
        </div>`
   
    form.prepend(errorMessage)
}

async function confirmDelete(url) {
    let resp = await fetch(url)
    let textResp = resp.status === 200 ? await resp.text() : "<div class='modal-header'> <p class'text-danger'>Some error occured<p> </div>";
    var modalBody = document.getElementById("modalBody")
    modalBody.innerHTML = textResp;
    $("#modal").modal("show");
}

function closeModal() {
    $("#modal").modal("hide");
    return;
}

async function getPartialView(url) {
    let resp = await fetch(url)
    console.log(url,resp)
    let textResp = resp.status === 200 ? await resp.text() : "<div class='modal-header'> <p class'text-danger'>Some error occured<p> </div>";
    console.log(textResp)
    var modalBody = document.getElementById("modalBody")
    modalBody.innerHTML = textResp;
    $("#modal").modal("show");

    document.getElementById("manageRole").addEventListener("submit", async (e) => {
        e.preventDefault();
        addOrEditRole(url);
        return
    })



}

async function loadModal(url) {
    var resp = await fetch(url);
    console.log(resp)
    let textResp = await resp.text();
    var modalBody = document.getElementById("modalBody")
    modalBody.innerHTML = textResp;
    $("#modal").modal("show");
}

async function AddToRole() {
    document.addEventListener("submit", (e) => { e.preventDefault() })
    let token = document.getElementsByName("__RequestVerificationToken")[0].value;
    let roleId = document.getElementById("AUTR-roleId").value;
    let userId = document.getElementById("AUTR-userId").value;
    let userEmail = document.getElementById("AUTR-userEmail").value;
    let resp = await fetch(`/roles/AddUserToRole`, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            __RequestVerificationToken: token,
        },
        body: JSON.stringify({
            userId,
            roleId,
            userEmail
        })
    })
   
    if (resp.status == 201) {
        $('#modal').modal('hide');
        Swal.fire({
            position: 'top-end',
            icon: 'success',
            title: 'Employee successfully added to role',
            showConfirmButton: false,
            timer: 2000
        });
    }
    else if (resp.status === 400) {
        var ul = document.getElementById("AUTR-error");
        ul.innerHTML=`<li>${userEmail} is already in role</li>`
    }
    else if (resp.status === 403) {
        console.log("add to role respense", resp)
        var ul = document.getElementById("AUTR-error");
        ul.innerHTML = `<li>${userEmail} was not found in the system</li>`
    }

}
async function postData(url, name, newName, token) {
    console.log(url, name, newName, token)
    let resp = await fetch(url, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            __RequestVerificationToken: token,
        },
        body: JSON.stringify({
            Name: name ? name : "",
            NewName: newName
        })
    })
    return resp
}