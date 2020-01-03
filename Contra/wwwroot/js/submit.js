
var coauthors, inputCoauthors;
var title, inputTitle;
var image, inputImage;
var content;
var submitStep = 1;
var currentFormat;

document.addEventListener("DOMContentLoaded", () => {
    coauthors = document.getElementById("coauthors");
    title = document.getElementById("title");
    image = document.getElementById("thumbnail-img");
    content = document.getElementById("content");
});

function setPreviewVariables() {
    inputCoauthors = document.getElementById(currentFormat + "-input-coauthors");
    inputTitle = document.getElementById(currentFormat + "-input-title");
    inputImage = document.getElementById(currentFormat + "-input-img");
}

function selectFormat(format) {
    anime.timeline({
        duration: 1000,
        easing: 'easeOutExpo'
    }).add({
        targets: '#select-format',
        height: "0px",
        opacity: 0
    }).add({
        targets: '.format',
        delay: -1000,
        opacity: 1
    });

    currentFormat = format;
    if (format === 0)
        document.getElementById("select-article").style.display = "block";
    else if (format === 1)
        document.getElementById("select-event").style.display = "block";
    else if (format === 2)
        document.getElementById("select-newsbeat").style.display = "block";

    document.getElementById("input-articleType").value = format;
    setPreviewVariables();
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

    if (currentFormat === 0)
        document.getElementById("select-article").style.display = "none";
    else if (currentFormat === 1)
        document.getElementById("select-event").style.display = "none";
    else if (currentFormat === 2)
        document.getElementById("select-newsbeat").style.display = "none";

    submitResetSteps();
}

function toggleGroup(sender, group) {
    if (sender.checked)
        document.getElementById("inputGroup-" + group).style.display = "unset";
    else
        document.getElementById("inputGroup-" + group).style.display = "none";
}

function submitResetSteps() {
    var toShow = document.getElementById(currentFormat + "-step-1");
    var shown = document.getElementById(currentFormat + "-step-" + submitStep);
    toShow.style.display = "unset";
    shown.style.display = "none";

    document.getElementById(currentFormat + "-prevButton").classList = "btn btn-outline-dark disabled";
    document.getElementById(currentFormat + "-nextButton").classList = "btn btn-outline-info";
    document.getElementById("preview").style.display = "none";

    submitStep = 1;
}

function submitUndoStep() {
    if (submitStep > 1) submitStep--;
    else submitStep = 1;

    var toShow = document.getElementById(currentFormat + "-step-" + submitStep);
    var shown = document.getElementById(currentFormat + "-step-" + (submitStep + 1));
    toShow.style.display = "unset";
    shown.style.display = "none";

    if (submitStep === 1) {
        document.getElementById(currentFormat + "-prevButton").classList = "btn btn-outline-dark disabled";
    }
    document.getElementById(currentFormat + "-nextButton").classList = "btn btn-outline-info";
    document.getElementById("preview").style.display = "none";

    updateLivePreview();
}

function submitNextStep() {
    if (submitStep < 4) submitStep++;
    else submitStep = 4;

    var toShow = document.getElementById(currentFormat + "-step-" + submitStep);
    var shown = document.getElementById(currentFormat + "-step-" + (submitStep - 1));
    toShow.style.display = "unset";
    shown.style.display = "none";

    if (submitStep === 4) {
        document.getElementById("preview").style.display = "unset";
        document.getElementById(currentFormat + "-nextButton").classList = "btn btn-outline-dark disabled";
        content.innerHTML = $("#summernote").summernote('code');
    }
    document.getElementById(currentFormat + "-prevButton").classList = "btn btn-outline-info";

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
        image.src = "../img/img05.jpg";

    content.innerHTML = $("#" + currentFormat + "-content").summernote('code');
}
