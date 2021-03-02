

async function getPartialView(url, id) {
    url =  `${url}?id=${id}` 
    var modalBody = document.getElementById("modalBody")
    let resp = await fetch(url)
    let textResp = resp.status === 200 ? await resp.text() : "Some error occured"
    modalBody.innerHTML = textResp;
    $("#modal").modal("show");

    if (url.includes("folders/add")) {
        document.getElementById("createFolder").addEventListener("submit", async (e) => {
            e.preventDefault();
            await createFolder(url);
            return
        })
    }
    else if (url.includes("/Folder/GetEditFolderPartialView")) {
        document.getElementById("editFolder").addEventListener("submit", async (e) => {
            e.preventDefault();
            await editFolder(id);
            return
        })
    }
    else {
        document.getElementById("delFolder-no").addEventListener("click", async (e) => {
            closeModal();
            return
        })
    }
}

function closeModal() {
    $("#modal").modal("hide");
}

async function editFolder(id) {
    let verificationToken = document.getElementsByName("__RequestVerificationToken")[0].value
    let accesslevelId = document.getElementById("EF-accessLevel").value;
    let name = document.getElementById("EF-name").value;
    let parentId = document.getElementById("EF-parentId").value;
    if (!name || !accesslevelId) {
        addValidationErrors("editFolder", name, accesslevelId)
    }
    else {
        await postData("/Folder/Edit", name, accesslevelId, parentId, verificationToken, id = id)
        location.reload();
    }
    }
   

async function createFolder(url) {
    let verificationToken = document.getElementsByName("__RequestVerificationToken")[0].value
    let parentId = document.getElementById("CF-parentId").value;
    let accesslevelId = document.getElementById("CF-accessLevel").value;
    let name = document.getElementById("CF-name").value;
    
    if (!name || !accesslevelId) {
        addValidationErrors("createFolder", name, accesslevelId)
    }
    else {
        let resp = await postData(url, name, accesslevelId, parentId, verificationToken, id = 0)
        console.log(resp.status === 400)
        console.log(resp)
        if (resp.status === 400) {
            addValidationErrors("createFolder", name, accesslevelId, isNameTaken = true)
        }
        else {
            location.reload()
        } 
        
    }
   
}


function addValidationErrors(formId, name, accesslevelId, isNameTaken=false) {
    let form = document.getElementById(formId);
    let validationErrorMessage = document.createElement("div");
    if (form.childNodes.length > 11) {
        form.removeChild(form.childNodes[0])
    }
    validationErrorMessage.innerHTML =
        `<div class='validation-summary-errors'>
                <ul>
                    ${!name ? '<li>Folder name field is required</li>' : ''}
                    ${!accesslevelId ? '<li>Access level field is required</li>' : ''}
                    ${isNameTaken ? `<li>${name} already exist in folder</li>` : ''}
                </ul>
            </div>`
    form.prepend(validationErrorMessage)
}

async function postData(url, name, accesslevelId, parentId, token, id = 0) {
    let resp = await fetch(url, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            __RequestVerificationToken: token,
        },
        body: JSON.stringify({
            Name: name,
            AccessLevelId: parseInt(accesslevelId),
            ParentId: parseInt(parentId),
            Id: id
        })
    })
    return resp
}

