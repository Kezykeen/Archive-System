
async function getPartialView(url) {
    var modalBody = document.getElementById("modalBody")
    console.log(url)
    let resp = await fetch(url)
    console.log(resp)
    let textResp = resp.status === 200 ? await resp.text() : "Some error occured"
    modalBody.innerHTML = textResp;
    $("#modal").modal("show");

    if (url.includes("roles/add")) {
        document.getElementById("createRole").addEventListener("submit", async (e) => {
            e.preventDefault();
            createRole(url);
            return
        })
    }
    else {
        
    }
    
}

async function createRole(url) {
    let verificationToken = document.getElementsByName("__RequestVerificationToken")[0].value
    let name = document.getElementById("role-name").value;
    
    if (!name) {
        addErrors("createRole", isnullNamError = true)
    }
    else {
        let resp = await postData(url, name, verificationToken)
        
        if (resp.status === 200) {
            location.reload()
        }
        else if (resp.status === 400) {
            addErrors("createRole", isnullNamError = false, isnameAlreadyExist = true, isUnknown = false)
        }
        else {
            addErrors("createRole", isnullNamError = false, isnameAlreadyExist = false, isUnknown = true)
        }

    }

}

async function postData(url, name , token) {
    let resp = await fetch(url, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            __RequestVerificationToken: token,
        },
        body: JSON.stringify({
            Name: name,
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