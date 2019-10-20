var menu, nav;
var loadTarget, targetPos, skip = 0;
var enabled = true, formatSearch = false;

document.addEventListener("DOMContentLoaded", function (event) {
    menu = document.getElementById("links");
    nav = document.getElementById("nav");

    var query = document.getElementById("query");
    if (query) {
        document.addEventListener("scroll", function () {
            loadContent(query.innerText);
        });
    }
    var search = document.getElementById("search");
    if (search) formatSearch = true;
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

function loadContent(query) {
    loadTarget = document.getElementById("loadTarget");
    if (!loadTarget || !enabled) return;
    if (formatSearch) skip = 2;

    if (document.body.scrollHeight - (window.scrollY + window.innerHeight) < 100) {
        var http = new XMLHttpRequest();
        http.open('GET', "https://" + window.location.host + "/api/v1/article/list/" + query + "/" + skip, true);
        skip++;

        http.onreadystatechange = function () {
            if (http.readyState === 4 && http.status === 200) {
                console.log(http.responseText);
                if (http.responseText === "" && enabled) {
                    enabled = false;
                    setTimeout(function () {
                        if (loadTarget.innerHTML === "")
                            document.getElementById("loadSection").style.display = "none";
                        loadTarget.innerHTML += "<div class='section-title'><h2>No more content! :(</h2></div>";
                    }, 100);
                }

                var json = JSON.parse(http.responseText);

                var i = 0;
                var html = "";
                json.forEach(obj => {
                    if (formatSearch) {
                        html += formatSearchCard(obj.id, obj.title, obj.author, obj.date, obj.summary);
                    }
                    else {
                        if (i % 2 === 0) {
                            html += "<section>" + formatCard(obj.id, obj.image,
                                obj.title, obj.summary, true, false);
                        }
                        else {
                            html += formatCard(obj.id, obj.image, obj.title,
                                obj.summary, false, true) + "</section>";
                        }
                    }
                    i++;
                });

                loadTarget.innerHTML += html;
            }
        };

        http.send();
    }
}

function formatCard(id, image, title, summary, big, encapsulate) {
    var classes = "card story";
    if (big) classes += " card-big";
    if (encapsulate) classes += " card-encapsulate";

    return "<div class='" + classes + "' onclick='redirect(" + id + ")'>" +
           "<span class='image-container'><img src='" + image + "' /></span>" +
           "<div><h2>" + title + "</h2><p>" + summary + "</p></div></div>";
}

function formatSearchCard(id, title, author, date, summary) {
    return "<div class='card card-big card-search'><div><h2>" + title +
        "</h2><p>" + author + " - " + date + "</p><hr /><p>" + summary +
        "</p><a asp-action='Article' asp-route-id=" + id + ">Read More</a></div></div>";
}