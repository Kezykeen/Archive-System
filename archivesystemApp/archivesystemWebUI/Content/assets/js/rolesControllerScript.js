
async function getPartialView(url) {
    
    var modalBody = document.getElementById("modalBody")
    let resp = await fetch(url)
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
    
}

async function createRole(url) {
    let verificationToken = document.getElementsByName("__RequestVerificationToken")[0].value
    let name = document.getElementById("role-name").value;

    if (!name) {
        addErrors("createRole")
    }
    else {
        let resp = await postData(url, name ,verificationToken)
        if (resp.status === 200) {
            location.reload()
        }
        else {
            addErrors("createRole")
            $("#modal").modal("show");
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
function addErrors(formId, known=true) {
    let form = document.getElementById(formId);
    let errorMessage = document.createElement("div");
    if (form.childNodes.length > 11) {
        form.removeChild(form.childNodes[0])
    }
    if (known) {
        errorMessage.innerHTML =
            `<div class='validation-summary-errors'>
                <ul>
                    <li>Role name field is required</li>
                </ul>
            </div>`
    }
    else {
        errorMessage.innerHTML =
            `<div class='validation-summary-errors'>
                <ul>
                    <li>Some error occured/li>
                </ul>
            </div>`
    }
   
    form.prepend(errorMessage)
}