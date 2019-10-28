var menu, nav;
var loadTarget, targetPos, skip = 0;
var enabled = true, formatSearch = false;
var submitStep = 1;

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
    if (search) { formatSearch = true; skip = 2; }

    window.addEventListener("resize", function () {
        onResize();
    });
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

    if (document.body.scrollHeight - (window.scrollY + window.innerHeight) < 200) {
        var http = new XMLHttpRequest();
        http.open('GET', "https://" + window.location.host + "/api/v1/article/list/" + query + "/" + skip, true);
        skip++;

        http.onreadystatechange = function () {
            if (http.readyState === 4 && http.status === 200) {
                if (http.responseText === "" && enabled) {
                    enabled = false;
                    setTimeout(function () {
                        if (loadTarget.innerHTML === "")
                            document.getElementById("loadSection").style.display = "none";
                        loadTarget.innerHTML += "<div class='section-title text-center'><h2>No more content! :(</h2></div>";
                    }, 100);
                }

                var json = JSON.parse(http.responseText);

                var i = 0;
                var html = "";
                json.forEach(obj => {
                    if (formatSearch) {
                        html += formatSearchCard(obj.id, obj.image, obj.title, obj.author, obj.date, obj.summary);
                    }
                    else {
                        var biggify = true;
                        if (i % 2 === 0) {
                            if (i % 4 === 2) biggify = !biggify;
                            html += "<section>" + formatCard(obj.id, obj.image,
                                obj.title, obj.summary, !biggify, biggify);
                        }
                        else {
                            if (i % 4 === 3) biggify = !biggify;
                            html += formatCard(obj.id, obj.image, obj.title,
                                obj.summary, biggify, !biggify) + "</section>";
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

function formatSearchCard(id, image, title, author, date, summary) {
    return "<div class='card card-big card-search'><span class='image-container'>" +
        "<img src='" + image + "' /></span><div><h2>" + title +
        "</h2><p>" + author + " - " + date + "</p><hr /><p>" + summary +
        "</p><a href='article/" + id + "'>Read More</a></div></div>";
}

function submitUndoStep() {
    if (submitStep > 1) submitStep--;
    else submitStep = 1;

    var toShow = document.getElementById("step-" + submitStep);
    var shown = document.getElementById("step-" + (submitStep + 1));
    toShow.style.display = "unset";
    shown.style.display = "none";

    if (submitStep === 1) document.getElementById("prevButton").classList = "btn btn-outline-dark disabled";
    document.getElementById("submit-wrapper").style.flexDirection = "row";
    document.getElementById("nextButton").classList = "btn btn-outline-info";
}

function submitNextStep() {
    if (submitStep < 3) submitStep++;
    else submitStep = 3;

    var toShow = document.getElementById("step-" + submitStep);
    var shown = document.getElementById("step-" + (submitStep - 1));
    toShow.style.display = "unset";
    shown.style.display = "none";

    if (submitStep === 3) {
        document.getElementById("nextButton").classList = "btn btn-outline-dark disabled";
        document.getElementById("submit-wrapper").style.flexDirection = "column";
    }
    document.getElementById("prevButton").classList = "btn btn-outline-info";
}