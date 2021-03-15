const NODE_COUNT_WITHOUT_ERROR_MESSAGE = 11;

function addValidationErrors(formId, name, accesslevelId, isNameTaken = false) {
    let form = document.getElementById(formId);
    let validationErrorMessage = document.createElement("div");
    if (form.childNodes.length > NODE_COUNT_WITHOUT_ERROR_MESSAGE) {
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
async function getPartialView(url, id) {
    url =  `${url}?id=${id}` 
    
    var alertMessageBox = document.getElementById('alertMessageBox');
    let resp = await fetch(url)
    
    if (resp.status === 403) {
        alertMessageBox.innerHTML = "Action Restricted: Action cannot be executed on folder";
        alertMessageBox.className = 'showMessage task-failure';
        setTimeout(() => { alertMessageBox.className = ''; return; }, 3000);
        return;
    }
    else if (resp.status===500){
        alertMessageBox.innerHTML = "Server error: Action failed";
        alertMessageBox.className = 'showMessage task-failure';
        setTimeout(() => { alertMessageBox.className = ''; return; }, 3000)
        return;
    }

    await showModal(resp)
    await monitorUserActivityOnModal(url);
}

async function showModal(resp) {
    var modalBody = document.getElementById("modalBody")
    let textResp = await resp.text();
    modalBody.innerHTML = textResp;
    $("#modal").modal("show");
}

async function ConfirmMove(folderToMoveInto) {
    let copiedItem = JSON.parse(localStorage.getItem("copiedItem"));
    var moveFolderAlert = document.getElementById('alertMessageBox');
    console.log(copiedItem.time, new Date(Date.now() - new Date(copiedItem.time)).getHours())
    if (!copiedItem) {
        moveFolderAlert.innerHTML = "Error: No Item was copied";
        moveFolderAlert.className = 'showMessage task-failure';
        setTimeout(() => { moveFolderAlert.className = ''; return; }, 2000)
    }
    else {
        url = `/Folder/GetConfirmItemMovePartialView?itemName=${copiedItem.name}&currentFolder=${folderToMoveInto}`;
        let resp = await fetch(url);
        if (resp.status === 200) {
            await showModal(resp);
            document.getElementById("confirmItemMove-form").addEventListener("submit", async (e) => {
                e.preventDefault();
                let newParentFolderId = document.getElementById("paste-here").getAttribute('data-FolderId');
                await CtrlV(parseInt(newParentFolderId));
            })
        }
        else  {
            moveFolderAlert.innerHTML = "Server error: Action failed";
            moveFolderAlert.className = 'showMessage task-failure';
            setTimeout(() => { alertMessageBox.className = ''; return; }, 3000)
            return;
        }
    }
   
}

async function monitorUserActivityOnModal(url) {
    if (url.includes("folders/add")) {
        document.getElementById("createFolder").addEventListener("submit", async (e) => {
            e.preventDefault();
            await createFolder(url);
        })
    }
    else if (url.includes("/Folder/GetEditFolderPartialView")) {
        document.getElementById("editFolder").addEventListener("submit", async (e) => {
            e.preventDefault();
            await editFolder(id);
        })
    }
    else {
        document.getElementById("delFolder-no").addEventListener("click", async (e) => {
            closeModal();
        })
        document.getElementById("delFolder-form").addEventListener("submit", async (e) => {
            e.preventDefault();
            await deleteFolder();
        })

    }
}

async function deleteFolder() {
    let verificationToken = document.getElementsByName("__RequestVerificationToken")[0].value;
    let folderId = document.getElementById("delFolder-id").value;
    let parentId = document.getElementById("delFolder-parentId").value;
    console.log("reached here", folderId, parentId, "folders/delete")
    let resp = await fetch("/folders/delete", {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            __RequestVerificationToken: verificationToken,
        },
        body: JSON.stringify({
            id: parseInt(folderId),
            ParentId: parseInt(parentId)
        })
    })
    if (resp.status === 204) {
        location.reload();
    }
    else if (resp.status === 400) {
        let div = document.getElementById("delfolder-con");
        div.innerHTML =
            `<div class='validation-summary-errors'>
                <ul>
                    <li>Delete Action cannot be performed on this folder</li>
                </ul>
            </div>`
        document.getElementsByClassName("modal-title")[0].innerHTML = "";
        
    }
    return;
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

function CtrlX(itemId, itemName,itemType) {
    var copyFolderAlert = document.getElementById('alertMessageBox');
    copyFolderAlert.innerHTML = `${itemName} ${itemType=="folder"? "folder": "file"} copied`;
    copyFolderAlert.className = 'showMessage task-success';
    localStorage.setItem("copiedItem", JSON.stringify({ "id": itemId, "itemType": itemType, name: itemName, time: Date.now() }))
    setTimeout(() => { copyFolderAlert.className = ''; return; }, 2000)
}

async function CtrlV(newParentFolderId ) {
    let copiedItem = JSON.parse(localStorage.getItem("copiedItem"));
    let verificationToken = document.getElementsByName("__RequestVerificationToken")[0].value;
    var moveFolderAlert = document.getElementById('alertMessageBox');
    
    resp = await fetch("/folders/move", {
        method: "POST",
        headers: {
            __RequestVerificationToken: verificationToken,
            'content-type': "application/json"
        },
        body: JSON.stringify({
            id: copiedItem.id,
            fileType: copiedItem.itemType,
            newParentFolder: newParentFolderId
        })
    })
 
    if (resp.status === 200) {
        localStorage.removeItem("copiedItem")
        moveFolderAlert.innerHTML = `Item moved successfully`;
        moveFolderAlert.className ="showMessage task-success"
        location.reload();
    }
    else {
        moveFolderAlert.innerHTML =(
            resp.status === 403 ? `Item with name already exist in Folder` :
                resp.status === 405 ? "Warning: Cannot move a folder into itself" : "Server Error: Action Failed.");
        moveFolderAlert.className = "showMessage task-failure";
        setTimeout(() => { alertMessageBox.className = ''; return; }, 3000)
    }
}

async function VerifyAccessToken(e) {
    e.preventDefault();
    let verificationToken = document.getElementsByName("__RequestVerificationToken")[0].value;
    var accessCode = document.getElementById('EAC-code').value;
    var returnUrl = document.getElementById("EAC-returnUrl").value;
    console.log(accessCode, returnUrl)
    resp = await fetch("/Folder/VerifyAccessCode", {
        method: "POST",
        headers: {
            __RequestVerificationToken: verificationToken,
            'content-type': "application/json"
        },
        body: JSON.stringify({
            accessCode: accessCode,
            
        })
    })
    console.log(resp);
    if (resp.status === 200) {
        location.href = returnUrl;
        return;
    }
    let form = document.getElementById("EAC-form");
    let validationErrorMessage = document.createElement("div");
    
    if (form.childNodes.length > NODE_COUNT_WITHOUT_ERROR_MESSAGE) {
        form.removeChild(form.childNodes[0])
    }
    var message = resp.status === 400 ? "Access code is incorrect" : "Server Error"
    validationErrorMessage.innerHTML =
        `<div class='validation-summary-errors'>
                <ul>
                    <li>${message}</li>
                </ul>
         </div>`
    form.prepend(validationErrorMessage)



}


