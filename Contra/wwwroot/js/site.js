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

(function () {
    if (document.cookie.includes("dark=true"))
        document.documentElement.classList.add("dark");
})();

function toggleNightTheme() {
    if (document.cookie.includes("dark=true")) {
        eraseCookie("dark");
        document.documentElement.classList.remove("dark");
    }
    else {
        setCookie("dark", "true", 30);
        document.documentElement.classList.add("dark");
    }
}

var menu, nav, dropdown;
document.addEventListener("DOMContentLoaded", () => {
    menu = document.getElementById("links");
    nav = document.getElementById("nav");
    dropdown = document.getElementById("account-dropdown");

    var nltargets = document.getElementsByClassName("neoload");
    for (var i = 0; i < nltargets.length; i++) {
        var el = nltargets[i];
        neoload(el, el.getAttribute("data-query"),
                    el.getAttribute("data-amount"),
                    el.getAttribute("data-type"),
                    el.getAttribute("data-loading"));
    }

    if (document.cookie.includes("compact=yes") &&
        document.getElementById("compact-compatible"))
        showCompact();

    window.addEventListener("resize", onResize);
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
    if (dropdown) {
        if (dropdown.style.display === "none" && window.innerWidth < 768) {
            dropdown.style.display = "block";
        }
        else if (window.innerWidth > 768) {
            dropdown.style.display = "none";
        }
    }
}

function toggleAccountDropdown() {
    if (dropdown.style.display !== "block") {
        dropdown.style.display = "block";
    }
    else {
        dropdown.style.display = "none";
    }
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

function neoload(target, query, amount, type, loading) {
    request("GET", "api/v1/article/list/" + query + "/" + amount, x => {
        if (!x) return;
        else {
            if (type === "mini") target.style.display = "flex";
            else target.style.display = "block";
        }

        var json = JSON.parse(x);
        
        var i = 0;
        var html = "";
        json.forEach(obj => {
            var sensitive = false, spoiler = false, pinned = false;
            if (obj.pinned === "True") pinned = true;
            if (obj.sensitive === "True") sensitive = true;
            if (obj.spoiler === "True") spoiler = true;

            var cw;
            if (type === "search") {
                cw = formatContentWarning(pinned, sensitive, spoiler, false);
                html += formatSearchCard(obj.id, obj.image, obj.title, obj.author, obj.date, obj.summary, cw);
            }
            else if (type === "block") {
                cw = formatContentWarning(pinned, sensitive, spoiler, false);
                var biggify = false;
                if (i % 2 === 0) {
                    if (i % 4 === 2) biggify = !biggify;
                    html += "<section>" + formatCard(obj.id, obj.image, obj.title, obj.summary, !biggify, biggify, cw);
                }
                else {
                    if (i % 4 === 3) biggify = !biggify;
                    html += formatCard(obj.id, obj.image, obj.title, obj.summary, biggify, !biggify, cw) + "</section>";
                }
            }
            else if (type === "mini") {
                cw = formatContentWarning(pinned, sensitive, spoiler, true);
                html += formatInsight(obj.id, obj.image, obj.title, obj.summary, cw);
            }
            i++;
        });

        setTimeout(() => {
            target.innerHTML += html;

            if (document.cookie.includes("compact=yes") &&
                document.getElementById("compact-compatible"))
                showCompact();

            if (loading) document.getElementById("loading-" + loading).style.display = "none";
        }, 100);
    });
}

function formatContentWarning(pinned, sensitive, spoiler, mini) {
    if (!(pinned && mini) && !sensitive && !spoiler)
        return "";

    var html = "<p class='content-notice";
    if (mini) html += " content-notice-inline";
    html += "'>";
    if (pinned && mini) html += "Pinned ";

    if (sensitive || spoiler) html += "CW: ";
    if (sensitive) html += "<span>Sensitive</span>";
    if (sensitive && spoiler) html += "<span>, </span>";
    if (spoiler) html += "<span>Spoiler</span>";

    return html + "</p>";
}

function formatCard(id, image, title, summary, big, encapsulate, contentWarning) {
    var classes = "card story";
    if (big) classes += " card-big";
    if (encapsulate) classes += " card-encapsulate";

    return "<a class='" + classes + "' href='/article/" + id + "'>" +
        "<span class='image-container'><img src='" + image + "' alt='" + title + " Thumbnail Image' /></span>" +
        "<div><h2>" + title + "</h2>" + contentWarning + "<p>" + summary + "</p></div></da>";
}

function formatSearchCard(id, image, title, author, date, summary, contentWarning) {
    return "<div class='card card-big card-search'><span class='image-container'>" +
        "<img src='" + image + "' alt='" + title + " Thumbnail Image' /></span><div><h2>" +
        title + "</h2><p class='compact-hidden'>" + author + " - " + date + "</p>" + contentWarning + "<hr class='compact-hidden' /><p>" +
        summary + "</p><a href='https://" + window.location.host + "/article/" + id + "'>Read More</a></div></div>";
}

function formatInsight(id, image, title, summary, contentWarning) {
    return "<a class='card card-big card-encapsulate card-insight' href='/article/" + id +
        "'><span class='image-container'><img src='" + image + "' alt='" + title +
        " Thumbnail' /></span><div><h2>" + title + "</h2>" + contentWarning + "<p>" + summary + "</p></div></a>";
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

    document.getElementById("comfortableButton").classList = "btn c-btn-info";
    document.getElementById("compactButton").classList = "btn c-btn-outline-info";
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

    document.getElementById("comfortableButton").classList = "btn c-btn-outline-info";
    document.getElementById("compactButton").classList = "btn c-btn-info";
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