
async function getPartialView(url, id) {
    console.log(url,id)
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
            return})
    }
    else if (url.includes("/Folder/GetEditFolderPartialView")) {
        document.getElementById("editFolder").addEventListener("submit", async (e) => {
            e.preventDefault();
            await editFolder(id);
            return})
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
        let resp = await postData("/Folder/Edit", name, accesslevelId, parentId, verificationToken, id = id)
    }
    location.reload();
}


async function createFolder(url) {
    let parentId = document.getElementById("CF-parentId").value;
    let accesslevelId = document.getElementById("CF-accessLevel").value;
    let name = document.getElementById("CF-name").value;
   
    if (!name || !accesslevelId) {
        addValidationErrors("createFolder", name, accesslevelId)
    }
    else {
        let resp = await postData(url, name, accesslevelId, parentId, id = 0)
        console.log(resp);
        if (resp.status === 200) {
            location.reload()
        }
        else {
            document.getElementById("modalBody").innerHTML = resp;
            $("#modal").modal("show");
        }
        return;    
        
    }
   
}

function updateFolderList(resp) {
    let jsonResp = JSON.parse(resp);
    let element = document.createElement("li")
    element.style = 'display: flex; justify-content: space-between; align-items: center';
    element.className = "folderlist-folder";
    element.innerHTML =
        `
                    <a href="/folders/${jsonResp.data.Id}"> ${jsonResp.data.Name}</a>
                    <a href="" class="dropdown-link" data-toggle="dropdown" style="padding:0;width: 3em;margin:0; display:flex;justify-content:center">
                         <i class="fa fa-ellipsis-v"></i>
                    </a>
                    <div  class="dropdown-menu dropdown-menu-right">
                        <a href="#" onclick="getPartialView('/Folder/GetEditPartialView',${jsonResp.data.Id})" class="dropdown-item" > Edit Folder</a>
                        <a href="#" onclick="getPartialView('/Folder/GetDeletePartialView',${jsonResp.data.Id})" class="dropdown-item" > Delete Folder</a>
                    </div>

                `;
    let folderContainer = document.getElementById("FL-folders");
    folderContainer.appendChild(element);
    closeModal();
}

function addValidationErrors(formId,name,accesslevelId) {
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
                </ul>
            </div>`
    form.prepend(validationErrorMessage)
}

async function postData(url, name, accesslevelId, parentId, token, id = 0) {
    console.log("about to post data",url,token)
    let resp = await fetch(url, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            "__RequestVerificationToken": token,
            Name: name,
            AccessLevelId: parseInt(accesslevelId),
            ParentId: parseInt(parentId),
            Id: id
            
          
        })
    })
    return resp
}