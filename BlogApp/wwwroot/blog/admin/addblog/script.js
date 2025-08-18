function setSideBarHeight(){

    let side = document.querySelector('.side-bar');

    if(document.documentElement.offsetWidth > 805){
        

        let workHeigth = document.querySelector('.work-part').clientHeight;

        side.style.height = workHeigth - document.querySelector('.admin-short-info').clientHeight + 'px';
    }
    else{
        
        side.style.height = 'fit-content'
    }
}

setSideBarHeight();

window.addEventListener('resize', () => {
    setSideBarHeight();
});

//text editor script

//let target = null;
//let selectedText = '';
//let start = 0;
//let end = 0;
//let tagName = '';

//document.body.addEventListener('click', (e) => {

//    let tag = e.target.tagName;

//    if (tag === 'INPUT' || tag === 'TEXTAREA') {

//        if (window.getSelection().toString() !== '') {

//            selectedText = window.getSelection().toString();

//            start = e.target.selectionStart;

//            end = e.target.selectionEnd;

//            target = e.target;

//            if (selectedText.startsWith('<a') && selectedText.endsWith('</a>')) {
//                tagName = 'A';

//            }
//            else if (selectedText.startsWith('<b') && selectedText.endsWith('</b>')) {
//                tagName = 'B'
//            }
//            else if (selectedText.startsWith('<i') && selectedText.endsWith('</i>')) {
//                tagName = 'I'
//            }
//            else {
//                tagName = '';
//            }
//        }
//    }
//});

//function applyEditions(openTag, closeTag) {

//    if (target !== null && selectedText !== '') {

//        let allTextFromElement = target.value;

//        let newSelectedText = openTag + selectedText + closeTag;

//        let newText = allTextFromElement.substring(0, start) + newSelectedText + allTextFromElement.substring(end);

//        target.value = newText;
//    }
//}

//function disapplyEditions() {

//    if (tagName !== '') {

//        let container = document.createElement('div');

//        container.innerHTML = selectedText;

//        let neededTag = container.firstElementChild;

//        console.log(neededTag.textContent);
//    }
//}