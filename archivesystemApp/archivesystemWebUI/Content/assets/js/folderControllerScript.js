

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
        })
    }
    else {
        document.getElementById("delFolder-no").addEventListener("click", async (e) => {
            closeModal
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
    console.log(resp)
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

function CtrlC(itemId, itemName,itemType) {
    var copyFolderAlert = document.getElementById('copyFolder');
    copyFolderAlert.innerHTML = `${itemName} ${itemType=="folder"? "folder": "file"} copied`;
    copyFolderAlert.className = 'showFolder task-success';
    localStorage.setItem("copiedItem", JSON.stringify({ "id": itemId, "itemType":itemType  }))
    setTimeout(() => { copyFolderAlert.className = ''; return; }, 2000)
}

async function CtrlV(newParentFolderId ) {
    let copiedItem = JSON.parse(localStorage.getItem("copiedItem"));
    let verificationToken = document.getElementsByName("__RequestVerificationToken")[0].value;
    var copyFolderAlert = document.getElementById('copyFolder');
    copyFolderAlert.className = 'showFolder task-failure';
    if (!copiedItem) {
        copyFolderAlert.innerHTML = "Error: No Item was copied";
        setTimeout(() => { copyFolderAlert.className = ''; return; }, 2000)
    }
    else {
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
        console.log(resp)
        if (resp.status === 200) {
            localStorage.removeItem("copiedItem")
            copyFolderAlert.innerHTML = `Item moved successfully`;
            copyFolderAlert.className="showFolder task-success"
            location.reload();
        }
        else {
            copyFolderAlert.innerHTML = resp.status === 403 ? `Item with name already exist in Folder` : "Server Error: Action Failed."
        }
       

    }
   
         
       
}
    
    

