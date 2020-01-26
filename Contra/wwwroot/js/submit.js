
var coauthors, inputCoauthors;
var title, inputTitle;
var image, inputImage;
var content;
var submitStep = 1, maxSteps;
var currentFormat;

var inputResponseID;

document.addEventListener("DOMContentLoaded", () => {
    coauthors = document.getElementById("coauthors");
    title = document.getElementById("title");
    image = document.getElementById("thumbnail-img");
    content = document.getElementById("preview-content");

    inputCoauthors = document.getElementById("input-coauthors");
    inputTitle = document.getElementById("input-title");
    inputImage = document.getElementById("input-img");

    maxSteps = parseInt(document.getElementById("submit-data").getAttribute("data-steps"));

    inputResponseID = document.getElementById("input-responseID");
    if (inputResponseID) {
        var id = parseInt(inputResponseID.value.split('/').pop());
        document.getElementById("ResponseId").value = id;
        monoload(document.getElementById("responseEmbedPreview"), id);
        inputResponseID.addEventListener("input", () => {
            var id = parseInt(inputResponseID.value.split('/').pop());
            if (id) {
                document.getElementById("ResponseId").value = id;
                monoload(document.getElementById("responseEmbedPreview"), id);
            }
        });
    }
});

function toggleGroup(sender, group) {
    if (sender.checked)
        document.getElementById("inputGroup-" + group).style.display = "unset";
    else
        document.getElementById("inputGroup-" + group).style.display = "none";
}

function submitResetSteps() {
    var toShow = document.getElementById("step-1");
    var shown = document.getElementById("step-" + submitStep);
    shown.style.display = "none";
    toShow.style.display = "unset";

    document.getElementById("prevButton").classList = "btn btn-outline-dark disabled";
    document.getElementById("nextButton").classList = "btn btn-outline-info";
    document.getElementById("preview").style.display = "none";

    submitStep = 1;
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
    document.getElementById("preview").style.display = "none";

    updateLivePreview();
}

function submitNextStep() {
    if (submitStep < maxSteps) submitStep++;
    else submitStep = maxSteps;

    var toShow = document.getElementById("step-" + submitStep);
    var shown = document.getElementById("step-" + (submitStep - 1));
    toShow.style.display = "unset";
    shown.style.display = "none";

    if (submitStep === maxSteps) {
        document.getElementById("preview").style.display = "unset";
        document.getElementById("nextButton").classList = "btn btn-outline-dark disabled";
        content.innerHTML = $("#content").summernote('code');
    }
    document.getElementById("prevButton").classList = "btn btn-outline-info";

    updateLivePreview();
}

function toggleTag(tag) {
    var tags = document.getElementById("tags");
    if (tags.value.includes(" " + tag))
        tags.value = tags.value.replace(" " + tag, "");
    else
        tags.value += " " + tag;
}


function updateLivePreview() {
    if (!document.getElementById("toggle-coauthors").checked)
        inputCoauthors.value = null;

    if (inputCoauthors.value)
        coauthors.innerText = ", " + inputCoauthors.value;
    else
        coauthors.innerText = "";

    if (inputTitle.value)
        title.innerText = inputTitle.value;
    else
        title.innerText = "Title";

    if (inputImage.value) {
        var reader = new FileReader();
        reader.onload = function () {
            image.src = reader.result;
        };
        reader.readAsDataURL(inputImage.files[0]);
    }
    else
        image.src = "/img/img05.jpg";

    content.innerHTML = $("#Content").summernote('code');
}
