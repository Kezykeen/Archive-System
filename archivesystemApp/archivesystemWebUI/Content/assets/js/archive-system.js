console.log("ghdkhkjldhyjkpdhykjl")
console.log("ghdkhkjldhyjkpdhykjl")
console.log("ghdkhkjldhyjkpdhykjl")
console.log("ghdkhkjldhyjkpdhykjl")


function getEditFolderPartialView(url, id) {
    $.get(url,
        { id: id },
        function (res) {
            $("#modalBody").html(res);
            $("#modal").modal("show");
        });
}