var menu, nav;
var loadTarget, targetPos, skip = 0;
var formatSearch = false;

// https://stackoverflow.com/a/24103596
function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function eraseCookie(name) {
    var date = new Date();
    date.setTime(date.getTime() - 1000);
    expires = "; expires=" + date.toUTCString();
    document.cookie = name + '=' + expires + ';path=/;';
}

document.addEventListener("DOMContentLoaded", function (event) {
    menu = document.getElementById("links");
    nav = document.getElementById("nav");

    var nltargets = document.getElementsByClassName("neoload");
    for (var i = 0; i < nltargets.length; i++) {
        var el = nltargets[i];
        neoload(el,
                el.getAttribute("data-query"),
                el.getAttribute("data-amount"),
                el.getAttribute("data-type"));
    }

    var search = document.getElementById("search");
    if (search) { formatSearch = true; skip = 2; }

    var query = document.getElementById("query");
    if (query) { loadContent(query.innerText); }

    if (document.cookie.includes("compact=yes") &&
        document.getElementById("compact-compatible"))
        showCompact();

    window.addEventListener("resize", function () {
        onResize();
    });
    
    coauthors = document.getElementById("coauthors");
    inputCoauthors = document.getElementById("input-coauthors");
    title = document.getElementById("title");
    inputTitle = document.getElementById("input-title");
    image = document.getElementById("thumbnail-img");
    inputImage = document.getElementById("input-img");
    content = document.getElementById("content");
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
    if ((menu.style.height !== "" || nav.style.height !== "") && window.innerWidth > 768) {
        menu.style.display = "";
        nav.style.height = "";
    }
}

function redirect(id) {
    window.location = "/article/"+id;
}

function changePFP(id, url) {
    if (url === "reset")
        request("POST", "api/v1/account/" + id + "/picture/" + url, () => {
            request("GET", "api/v1/account/" + id + "/picture",
                (x) => document.getElementById("user-picture").src = x + "&s=512");
        });
    else {
        request("POST", "api/v1/account/" + id + "/picture/" + url);
        document.getElementById("user-picture").src = url;
        hideDialog();
    }
}

function request(method, route, callback = null) {
    var http = new XMLHttpRequest();
    http.open(method, "https://" + window.location.host + "/" + route, true);

    http.onreadystatechange = function () {
        if (http.readyState === 4 && http.status === 200)
            callback(http.responseText);
    };

    http.send();
}

function neoload(target, query, amount, type) {
    request("GET", "api/v1/article/list/" + query + "/" + amount, x => {
        var json = JSON.parse(x);

        var i = 0;
        var html = "";
        json.forEach(obj => {
            if (type === "search") {
                html += formatSearchCard(obj.id, obj.image, obj.title, obj.author, obj.date, obj.summary);
            }
            else if (type === "block") {
                var biggify = false;
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
            else if (type === "mini") {
                html += formatNewsbeat(obj.id, obj.image, obj.title, obj.summary);
            }
            i++;
        });

        target.innerHTML += html;
    });
}

function loadContent(query) {
    loadTarget = document.getElementById("loadTarget");
    if (!loadTarget) return;

    var http = new XMLHttpRequest();
    http.open('GET', "https://" + window.location.host + "/api/v1/article/list/" + query + "/8/" + skip, true);
    skip++;

    http.onreadystatechange = function () {
        if (http.readyState === 4 && http.status === 200) {
            if (http.responseText === "") {
                if (document.getElementById("loadSection") && loadTarget.innerHTML === "")
                    document.getElementById("loadSection").style.display = "none";
                document.getElementById("loadButton").style.display = "none";
                loadTarget.innerHTML += "<div class='section-title text-center'><h2>No more content! :(</h2></div>";
                return;
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

            if (document.cookie.includes("compact=yes") &&
                document.getElementById("compact-compatible"))
                showCompact();
        }
    };

    http.send();
}

function formatCard(id, image, title, summary, big, encapsulate) {
    var classes = "card story";
    if (big) classes += " card-big";
    if (encapsulate) classes += " card-encapsulate";

    return "<div class='" + classes + "' onclick='redirect(" + id + ")'>" +
           "<span class='image-container'><img src='" + image + "' alt='" + title + " Thumbnail Image' /></span>" +
           "<div><h2>" + title + "</h2><p>" + summary + "</p></div></div>";
}

function formatSearchCard(id, image, title, author, date, summary) {
    return "<div class='card card-big card-search'><span class='image-container'>" +
           "<img src='" + image + "' alt='" + title + " Thumbnail Image' /></span><div><h2>" +
           title + "</h2><p class='compact-hidden'>" + author + " - " + date + "</p><hr class='compact-hidden' /><p>" +
           summary + "</p><a href='https://" + window.location.host + "/article/" + id + "'>Read More</a></div></div>";
}

function formatNewsbeat(id, image, title, summary) {
    return "<div class='card card-big card-encapsulate card-newsbeat' onclick='redirect(" +
           id + ")'><span class='image-container'><img src='" + image + "' alt='" + title +
           " Thumbnail' /></span><div><h2>" + title + "</h2><p>" + summary + "</p></div></div>";
}

function submitUndoStep() {
    if (submitStep > 1) submitStep--;
    else submitStep = 1;

    var toShow = document.getElementById("step-" + submitStep);
    var shown = document.getElementById("step-" + (submitStep + 1));
    toShow.style.display = "unset";
    shown.style.display = "none";

    if (submitStep === 1) {
        document.getElementById("prevButton").classList = "btn btn-outline-dark disabled";
    }
    document.getElementById("nextButton").classList = "btn btn-outline-info";

    updateLivePreview();
}

function submitNextStep() {
    if (submitStep < 4) submitStep++;
    else submitStep = 4;

    var toShow = document.getElementById("step-" + submitStep);
    var shown = document.getElementById("step-" + (submitStep - 1));
    toShow.style.display = "unset";
    shown.style.display = "none";

    if (submitStep === 4) {
        document.getElementById("nextButton").classList = "btn btn-outline-dark disabled";
        content.innerHTML = $("#summernote").summernote('code');
    }
    document.getElementById("prevButton").classList = "btn btn-outline-info";

    updateLivePreview();
}

function toggleTag(tag) {
    var tags = document.getElementById("tags");
    if (tags.value.includes(tag))
        tags.value.replace(tag, "");
    else
        tags.value += " " + tag;
}

var coauthors, inputCoauthors;
var title, inputTitle;
var image, inputImage;
var content, contentUpdateTimer;
var submitStep = 1;

function updateLivePreview() {
    if (inputCoauthors.value)
        coauthors.innerText = ", " + inputCoauthors.value;
    else
        coauthors.innerText = "";

    if (inputTitle.value)
        title.innerText = inputTitle.value;
    else
        title.innerText = "Title";

    if (inputImage.value)
        image.src = inputImage.value;
    else
        image.src = "../img/img05.jpg";

    content.innerHTML = $("#summernote").summernote('code');
}

function showComfortable() {
    var elements = document.getElementsByClassName("compact-hidden");
    for (var i = 0; i < elements.length; i++) {
        elements[i].style.display = "flex";
    }

    var cards = document.getElementsByClassName("card");
    for (var j = 0; j < cards.length; j++) {
        cards[j].style.marginTop = "10px";
        cards[j].style.marginBottom = "10px";
        cards[j].style.borderRadius = "10px";
        cards[j].style.borderBottom = "none";
    }

    eraseCookie("compact");

    document.getElementById("comfortableButton").classList = "btn btn-info";
    document.getElementById("compactButton").classList = "btn btn-outline-info";
}

function showCompact() {
    var elements = document.getElementsByClassName("compact-hidden");
    for (var i = 0; i < elements.length; i++) {
        elements[i].style.display = "none";
    }

    var cards = document.getElementsByClassName("card");
    for (var j = 0; j < cards.length; j++) {
        cards[j].style.marginTop = 0;
        cards[j].style.marginBottom = 0;
        cards[j].style.borderRadius = 0;
        cards[j].style.borderBottom = "2px solid #abc";
    }

    setCookie("compact", "yes", 0);

    document.getElementById("comfortableButton").classList = "btn btn-outline-info";
    document.getElementById("compactButton").classList = "btn btn-info";
}

function setDialog(selector) {
    var dialog = document.getElementById("dialog");
    var selected = document.getElementById(selector);

    if (!dialog.style.display) dialog.style.display = "none";

    if (dialog.style.display !== "none") {
        dialog.style.display = "none";
        dialog.childNodes.forEach((value) => value.style.display = "none");
    }
    else {
        dialog.style.display = "flex";
        selected.style.display = "flex";
    }
}

function hideDialog() {
    dialog.style.display = "none";
}

function stopPropogation(event) {
    event.stopPropagation();
}