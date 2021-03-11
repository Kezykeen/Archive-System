
async function getPartialView(url) {
    
    let resp = await fetch(url)
    console.log(resp)
    let textResp = resp.status === 200 ? await resp.text() :"<div class='modal-header'> <p class'text-danger'>Some error occured<p> </div>";

    var modalBody = document.getElementById("modalBody")
    modalBody.innerHTML = textResp;
    $("#modal").modal("show");

    document.getElementById("manageRole").addEventListener("submit", async (e) => {
        e.preventDefault();
        addOrEditRole(url);
        return
    })
    
   
    
}

async function addOrEditRole(url) {
    let verificationToken = document.getElementsByName("__RequestVerificationToken")[0].value
    let name = document.getElementById("role-name").value;
    let newName = document.getElementById("role-newName").value;
    console.log("name:",name,"new Name:",newName)
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

async function postData(url, name, newName, token) {
    console.log(url, name, newName, token)
    let resp = await fetch(url, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            __RequestVerificationToken: token,
        },
        body: JSON.stringify({
            Name: name ? name: "" ,
            NewName:newName
        })
    })
    return resp
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