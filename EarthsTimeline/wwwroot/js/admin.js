var lastSender = null, lastSenderContent = null, handled = false;

function confirmAction(sender, route) {
    if (!(lastSender === null || lastSenderContent === null) && !handled)
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
}

function postToAPI(route) {
    var http = new XMLHttpRequest();
    http.open('POST', route, true);

    http.onreadystatechange = function () {
        if (http.readyState === 4 && http.status === 200) {
            lastSender.innerHTML = "<h5>" + http.responseText + "</h5>";
            handled = true;
        }
    };

    http.send();
}