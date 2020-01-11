
var coauthors, inputCoauthors;
var title, inputTitle;
var image, inputImage;
var content;
var submitStep = 1;
var currentFormat;

var simplemde;

document.addEventListener("DOMContentLoaded", () => {
    coauthors = document.getElementById("coauthors");
    title = document.getElementById("title");
    image = document.getElementById("thumbnail-img");
    content = document.getElementById("preview-content");

    inputCoauthors = document.getElementById("input-coauthors");
    inputTitle = document.getElementById("input-title");
    inputImage = document.getElementById("input-img");

    simplemde = new SimpleMDE();
});

function selectFormat(format) {
    anime.timeline({
        duration: 1000,
        easing: 'easeOutExpo'
    }).add({
        targets: '#select-format',
        height: "0px",
        opacity: 0
    });

    currentFormat = format;
    document.getElementById("submit-form").style.display = "unset";

    document.getElementById("input-articleType").value = format;
}

function reselectFormat() {
    anime.timeline({
        duration: 1000,
        easing: 'easeOutExpo'
    }).add({
        targets: '#select-format',
        height: "100%",
        opacity: 1
    });

    document.getElementById("submit-form").style.display = "none";

    submitResetSteps();
}

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
    if (submitStep < 4) submitStep++;
    else submitStep = 4;

    var toShow = document.getElementById("step-" + submitStep);
    var shown = document.getElementById("step-" + (submitStep - 1));
    toShow.style.display = "unset";
    shown.style.display = "none";

    if (submitStep === 4) {
        document.getElementById("preview").style.display = "unset";
        document.getElementById("nextButton").classList = "btn btn-outline-dark disabled";
        content.innerHTML = simplemde.value();
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
        image.src = "/img/img05.jpg";

    document.getElementById("Content").value = DOMPurify.sanitize(marked(simplemde.value()));
    content.innerHTML = DOMPurify.sanitize(marked(simplemde.value()));
}
