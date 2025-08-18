function setSideBarHeight() {

    let side = document.querySelector('.side-bar');

    if (document.documentElement.offsetWidth > 805) {


        let workHeigth = document.querySelector('section').clientHeight;

        side.style.height = workHeigth - document.querySelector('.header').clientHeight + 'px';
    }
    else {

        side.style.height = 'fit-content'
    }
}

setSideBarHeight();

window.addEventListener('resize', () => {
    setSideBarHeight();
});

let checkBoxes = document.querySelectorAll(".checkbox");

checkBoxes.forEach(cb => {
    cb.addEventListener('click', () => {
        let isChecked = cb.getAttribute('ischecked');

        if (isChecked === 'true') {
            cb.setAttribute('ischecked', false)
        }
        else {
            cb.setAttribute('ischecked', true)
        }

    });
});

let tagListString = "";

let tagBtn = document.querySelector('.tag-btn');

let tagListInput = document.getElementById('tag-list-string');

tagBtn.addEventListener('mousedown', () => {
    Array.from(checkBoxes).forEach(cb => {
        if (cb.getAttribute('ischecked') === 'true') {
            tagListString += `${cb.getAttribute('id')} `
        }
    });

    tagListInput.value = tagListString;
});


let videoNameInput = document.querySelector(".video-name-input");

const nameDefaultValue = "add video name"

videoNameInput.value = nameDefaultValue;

videoNameInput.addEventListener("focus", () => {
    if (videoNameInput.value === nameDefaultValue) {
        videoNameInput.value = ""
    }
});

videoNameInput.addEventListener("blur", () => {
    if (videoNameInput.value === "") {
        videoNameInput.value = nameDefaultValue
    }
});

let videoLabel = document.querySelector('.video-label');
let videoInput = document.querySelector('.video-input');

videoInput.addEventListener('change', () => {
    if (videoInput.value !== "") {
        const className = videoInput.getAttribute('id');

        let label = document.querySelector('.' + className);

        label.style.borderColor = "#07de2d";
    }
});

let youtubeInput = document.querySelector('.video-youtube-input');

const youtubeInputDefault = "https://youtube.com/embed..."

youtubeInput.value = youtubeInputDefault;

youtubeInput.style.display = 'none';

youtubeInput.addEventListener("focus", () => {
    if (youtubeInput.value === youtubeInputDefault) {
        youtubeInput.value = ""
    }
});

youtubeInput.addEventListener("blur", () => {
    if (youtubeInput.value === "") {
        youtubeInput.value = youtubeInputDefault;
    }
});

let loadVideoBtn = document.getElementById('load-video');
let addRefBtn = document.getElementById('add-ref');

loadVideoBtn.addEventListener('click', () => {
    loadVideoBtn.classList.add('swiched');
    youtubeInput.style.display = 'none'
    videoLabel.style.display = 'block'
    addRefBtn.classList.remove('swiched');
});

addRefBtn.addEventListener('click', () => {
    addRefBtn.classList.add('swiched');
    youtubeInput.style.display = 'block'
    videoLabel.style.display = 'none'
    loadVideoBtn.classList.remove('swiched');
});

let addVideoButton = document.querySelector('.add-video-button');

addVideoButton.addEventListener('mousedown', () => {
    if (youtubeInput.value === youtubeInputDefault) {
        youtubeInput.value = '';
    }
})