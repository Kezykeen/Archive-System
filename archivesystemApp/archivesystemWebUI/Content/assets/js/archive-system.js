

async function getEditFolderPartialView(url, id) {
    var modalBody = document.getElementById("modalBody")
    url = `${url}?id=${id}`;
    let resp =
        await fetch(url)
        .then(
            resp => {
                return resp.text()
            }
        );
   
    modalBody.innerHTML = resp;
    $("#modal").modal("show");
    console.log(modalBody);
   
}

async function getDeleteFolderPartialView(url, id, name) {
    var modalBody = document.getElementById("modalBody")
    url = `${url}?id=${id}&name=${name}`;
    let resp =
        await fetch(url)
            .then(
                resp => {
                    return resp.text()
                }
        );
    console.log("resp:",resp)
    modalBody.innerHTML = resp;
    $("#modal").modal("show");
    console.log(modalBody);
}
function closeModal() {
    $("#modal").modal("hide");
}