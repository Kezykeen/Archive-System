console.log("ghdkhkjldhyjkpdhykjl")

document.addEventListener("click", (e) => {
    console.log(e.target.id)
    if (e.target.id.includes("roleId")) {
        
       let  id = e.target.id;
        let divId = id.split( "//")[1];
        divId = "divRole-" + divId;
        document.getElementById(divId).style.display = "block";
    }
    
}
)

let tableBody=document.getElementById("userRoles-body")
tableBody.addEventListener("click", (e) => {
    let id = e.target.id;
    let divId = id.split("//")[1];
    divId = "divRole-" + divId;
    if (e.target.id.includes("btnDelYes")) {
        fetch("https://localhost:44322/roles/delete", {
            method="POST",
            body: JSON.stringify({
                "roleId": id.split("//")[1],

            })
        })
    }

    if (e.target.id.includes("btnDel")) {
        document.getElementById(divId).style.display = "none";
    }
})


