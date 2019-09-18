var menu, nav;

document.addEventListener("DOMContentLoaded", function (event) {
    menu = document.getElementById("links");
    nav = document.getElementById("nav");
    $('#summernote').summernote({ height: 300 });
});

function showMenu() {
    if (menu.style.display === "none" || menu.style.display === "") {
        menu.style.display = "flex";
        nav.style.height = "unset";
    }
    else {
        menu.style.display = "none";
        nav.style.height = "50px";
    }
}

function onResize() {
    if (menu.style.height !== "" || nav.style.height !== "") {
        menu.style.display = "";
        nav.style.height = "";
    }
}

function redirect(id) {
    window.location = "article/"+id;
}