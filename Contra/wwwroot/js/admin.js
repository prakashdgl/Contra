var lastSender = null, lastSenderContent = null, handled = false;

function confirmAction(sender, route) {
    if (lastSender !== null && !handled)
        cancelAction();

    lastSender = sender.parentNode;
    lastSenderContent = sender.parentNode.innerHTML;

    route = "\"" + route + "\"";
    sender.parentNode.innerHTML = "<h5>Are you sure? " +
        "<a class='text-danger clickable' onclick='postToAPI("+route+")'>Yes</a>" +
        "<a class='text-info clickable' onclick='cancelAction()'>No</a></h5>";
}

function postToAPI(route) {
    request("POST", route, x => {
        lastSender.innerHTML = "<h5>" + x + "</h5>";
        handled = true;
    });
}

function cancelAction() {
    lastSender.innerHTML = lastSenderContent;
    handled = false;
}

function updateRoles(sender, id, role) {
    if (sender.checked)
        request("POST", "api/v1/user/" + id + "/ensure/" + role);
    else
        request("POST", "api/v1/user/" + id + "/enfeeble/" + role);
}

function createRole() {
    var nameEl = document.getElementById("roleName");
    lastSender = document.getElementById("message");

    request("POST", "api/v1/role/create/" + nameEl.value, x => {
        lastSender.innerHTML = "<h5>" + x + "</h5>";
        handled = true;
    });

    nameEl.value = "";
}

function toggleTag(tag) {
    var tags = document.getElementById("tags");
    if (tags.value.includes(" " + tag))
        tags.value = tags.value.replace(" " + tag, "");
    else
        tags.value += " " + tag;
}

function generateTestArticle(insight, editorial) {
    var tags = document.getElementById("tags").value;
    var route = "api/v1/generate/" + tags;

    if (insight) route += "/true";
    else route += "/false";
    if (editorial) route += "/true";
    else route += "/false";

    request("POST", route, (x) => {
        document.getElementById("suite-response").innerText = x;
    });
}

function degenerateArticles() {
    request("POST", "api/v1/degenerate", (x) => {
        document.getElementById("suite-response").innerText = x;
    });
}