var lastSender = null, lastSenderContent = null, handled = false;

function confirmAction(sender, route) {
    if (lastSender !== null && !handled)
        cancelAction();

    lastSender = sender.parentNode;
    lastSenderContent = sender.parentNode.innerHTML;

    route = "\"" + route + "\"";
    sender.parentNode.innerHTML = "<h5>Are you sure? " +
        "<a class='text-danger clickable' onclick='postToAPI("+route+")'>Yes</a> | " +
        "<a class='text-info clickable' onclick='cancelAction()'>No</a></h5>";
}

function cancelAction() {
    lastSender.innerHTML = lastSenderContent;
    handled = false;
}

function updateRoles(sender, id, role) {
    if (sender.checked)
        postToAPI("api/v1/user/" + id + "/ensure/" + role, false);
    else
        postToAPI("api/v1/user/" + id + "/enfeeble/" + role, false);
}

function createRole() {
    var nameEl = document.getElementById("roleName");
    lastSender = document.getElementById("message");
    postToAPI("api/v1/role/create/" + nameEl.value, true);
    nameEl.value = "";
}

function postToAPI(route, updateResponseText = true) {
    var http = new XMLHttpRequest();
    http.open('POST', "https://" + window.location.host + "/" + route, true);

    http.onreadystatechange = function () {
        if (http.readyState === 4 && http.status === 200 && updateResponseText) {
            lastSender.innerHTML = "<h5>" + http.responseText + "</h5>";
            handled = true;
        }
    };

    http.send();
}