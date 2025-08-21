let loader  = document.querySelector('.load-box');

let selectCat = document.querySelector('.format');

let selectedCatId = 0;

let user_Id = document.querySelector('.user-id').value;

let moveButtonsWithListeners = [];

function setEventListenersForMovingButtons() {

    let moveUpButtons = document.querySelectorAll('.move-btn-up');

    let moveDownButtons = document.querySelectorAll('.move-btn-down');

    Array.from(moveUpButtons).forEach(b => {

        if (!moveButtonsWithListeners.includes(b)) {
            b.addEventListener('click', () => {

                console.log("up started")

                let parentElement = b.parentElement;

                let neededBlock = parentElement.parentElement;

                if (neededBlock.classList.contains('section-group')) {

                    let neibour = neededBlock.previousElementSibling;

                    if (neibour.classList.contains('section-group')) {

                        neededBlock.insertAdjacentElement('afterend', neibour);

                    }
                }
                else if (neededBlock.classList.contains('subsection-group')) {

                    let neibour = neededBlock.previousElementSibling;

                    if (neibour.classList.contains('subsection-group')) {

                        neededBlock.insertAdjacentElement('afterend', neibour);

                    }
                }
                else if (neededBlock.classList.contains('paragraph-group')) {

                    let neibour = neededBlock.previousElementSibling;

                    if (neibour.classList.contains('paragraph-group')) {

                        neededBlock.insertAdjacentElement('afterend', neibour);

                    }
                }
            });

            moveButtonsWithListeners.push(b);
        }

    });


    moveDownButtons.forEach(b => {

        if (!moveButtonsWithListeners.includes(b)) {
            b.addEventListener('click', () => {

                let parentElement = b.parentElement;

                let neededBlock = parentElement.parentElement;

                if (neededBlock.classList.contains('section-group')) {

                    let neibour = neededBlock.nextElementSibling;

                    if (neibour.classList.contains('section-group')) {

                        neededBlock.insertAdjacentElement('beforebegin', neibour);

                    }
                }
                else if (neededBlock.classList.contains('subsection-group')) {

                    let neibour = neededBlock.nextElementSibling;

                    if (neibour.classList.contains('subsection-group')) {

                        neededBlock.insertAdjacentElement('beforebegin', neibour);

                    }
                }
                else if (neededBlock.classList.contains('paragraph-group')) {

                    let neibour = neededBlock.nextElementSibling;

                    if (neibour.classList.contains('paragraph-group')) {

                        neededBlock.insertAdjacentElement('beforebegin', neibour);

                    }
                }
            });

            moveButtonsWithListeners.push(b);
        }

    });

}

setEventListenersForMovingButtons();

selectCat.addEventListener('change', ()=>{
    selectedCatId = parseInt(selectCat.value);
    console.log(selectCat.value)
});

// function to get base64 string from file
function fileToBase64(fileinput) {

    return new Promise((resolve) =>{
        let fileReader = new FileReader();

        fileReader.readAsDataURL(fileinput);

        fileReader.onload = () => resolve(fileReader.result.split(',')[1]);
    })
}

async function sendDataToServer(blogData){

    let blogJsonData = JSON.stringify(blogData);

    loader.style.display = 'flex';

    let request = await fetch("/blog/api/create",{
        method: "POST",
        mode: 'cors',
        headers:{
            'Content-Type': 'application/json'
        },
        body: blogJsonData

    }).then(null, error => {loader.style.display = 'none'; showErrorMessage('Не удалось отправить запрос, возможно сервер на данный момент не работает')});

    if(request.ok){
        loader.style.display = 'none';

        showErrorMessage('Пост успешно создан!');
    }
    else{

        loader.style.display = 'none';

        showErrorMessage('На сервере произошла ошибка. Проверьте правильность заполнения полей в форме, а также стабильность подключения к сети')
    }

}

let publishButton = document.querySelector('.publish-btn');

publishButton.addEventListener('click', generateBlogAndSendToServer);

function validateInputs(){

    let isValid = true;

    let inputs = document.querySelectorAll('.input-group input');

    //let areas = document.querySelectorAll('.area-group textarea');

    inputs.forEach(i =>{

        i.addEventListener('focus', ()=>{
            i.style.boxShadow = '0 0 10px rgb(49, 44, 44)';
        });

        if(i.value === ''){
            i.style.boxShadow = '0 0 8px red';

            isValid = false;
        }
    });

    //areas.forEach(i =>{

    //    i.addEventListener('focus', ()=>{
    //        i.style.boxShadow = '0 0 10px rgb(49, 44, 44)';
    //    });

    //    if(i.value === ''){
    //        i.style.boxShadow = '0 0 8px red';

    //        isValid = false;
    //    }
    //});

    return isValid;
}


let errorBox = document.querySelector('.error-box');

function showErrorMessage(message){
    let errorMessage = document.querySelector('.error-message');

    errorMessage.innerHTML = message;

    errorBox.style.display = 'flex';
}

let errorButton = document.querySelector('.error-btn');

errorButton.addEventListener('click', () => {
    errorBox.style.display = 'none';
});

async function generateBlogAndSendToServer(){

    // let isValid = validateInputs();

    // if(!isValid){

    //     showErrorMessage('Все поля в форме должны быть заполнены');

    //     return;
    // }

    let blogImage = document.querySelector('#add-blog-img').files[0];

    let blogBase64Image = ""

    if(blogImage !== undefined){
        blogBase64Image = await fileToBase64(blogImage);
    }

    let tagListString = "";

    let checkBoxes = document.querySelectorAll(".checkbox");

    Array.from(checkBoxes).forEach(cb => {
        if (cb.checked) {
            tagListString += `${cb.getAttribute('id')} `
        }
    });

    let blogDesc = document.querySelector('.blog-desc');

    let blog = {
        id:0,
        title: document.querySelector('.blog-title').value,
        description: blogDesc.querySelector('.ql-editor').innerHTML,
        imageBase64String:blogBase64Image,
        image:"",
        alias:"",
        //dateOfPublish:"",
        //dateOfUpdate:"",
        tags: null,
        tagList: tagListString,
        sections:[],
        user:null,
        userId:user_Id,
        language:null,
        languageId:0,
        category:null,
        categoryId:selectedCatId
    }

    let sections = Array.from(document.querySelectorAll('.section-group'));

    for(let i = 0; i < sections.length; i++){

        let s = sections[i];

        let section = {
            id:0,
            title:s.querySelector('.section-title').value,
            subsections:[],
            blog:null,
            blogId:0
        }

        blog.sections.push(section);

        let subsections = Array.from(s.querySelectorAll('.subsection-group'));

        for(let j = 0; j < subsections.length; j++){

            let sub = subsections[j];

            let subsection = {
                id:0,
                title:sub.querySelector('.subsection-title').value,
                paragraphs:[],
                section:null,
                sectionId:0
            }

            section.subsections.push(subsection);

            let paragraphs = Array.from(sub.querySelectorAll('.paragraph-group'));

            for(let k = 0; k < paragraphs.length; k++){

                let p = paragraphs[k];

                let parImage = p.querySelector('.paragraph-img');

                let parBase64Image = "";

                if(parImage.files[0] !== undefined){

                    parBase64Image = await fileToBase64(parImage.files[0]);
                }

                let paragraph = {
                    id: 0,
                    text: p.querySelector('.ql-editor').innerHTML,
                    image:"",
                    imageBase64String: parBase64Image,
                    subsection:null,
                    subsectionId:0
                }

                subsection.paragraphs.push(paragraph);
            };

        };

    };

    sendDataToServer(blog);
}

let sectionsCount = 1;
let subsectionsCount = 1;
let paragraphsCount = 1;

let AddFileInputs = Array.from(document.querySelectorAll('.add-img'));

AddFileInputs.forEach(i =>{
    i.addEventListener('change', ()=>{

        let file = i.files[0];

        let src = URL.createObjectURL(file);

        let idName = i.getAttribute('id');

        let neededImagePreview = document.querySelector(`.${idName}`);

        neededImagePreview.src = src;
    })
});

let addBlockButtons = Array.from(document.querySelectorAll('.add-block button'));

let setIdToAddBlogMethod = function (b) {

    let buttonId = b.getAttribute('id');

    addBlock(buttonId);
}

let wrappers = [];

addBlockButtons.forEach(b => {

    let wrapper = setIdToAddBlogMethod.bind(this, b);

    wrappers.push(wrapper);

    b.addEventListener('click', wrapper)
});

document.querySelectorAll('button').forEach(b =>{
    b.addEventListener('click', (e)=>{
        e.preventDefault(false);
    })
});

document.querySelectorAll('.remove-btn').forEach(btn =>{
    btn.addEventListener('click', (e)=>{

        let parentElementForDelete = btn.parentElement;

        parentElementForDelete.remove();

        e.preventDefault(false);
    });
});

let editorsCount = 2;

function createHTMLEditor(count) {

    console.log("EDITOR " + count)

    const quill = new Quill(`#editor${count}`, {
        theme: 'snow'
    });
}

function addBlock(buttonId) {

    let parentElement = document.querySelector(`.${buttonId}`);

    if (parentElement.classList.contains('subsection-group')) {

        editorsCount++;

        console.log(`subsec button ${buttonId} clicked`);

        let siblingBlock = document.querySelector(`.${buttonId} .add-paragraph-block`);

        paragraphsCount++;

        let newParagraph = `
        <div class="paragraph-group">
                    <div class="move-buttons-group">
                                <button class="move-btn move-btn-up">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                                <button class="move-btn move-btn-down">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                            </div>
                    <button class="remove-btn">
                        <img src="/common_imgs/remove.png" alt="">
                    </button>
                        <div class="area-group">
                            <label>Параграф</label>
                            <div class="paragraph" id="editor${editorsCount}"></div>
                        </div>
                        <div class="img-group">
                            <div class="img-block">
                                <label for="add-par-img-${paragraphsCount}">
                                    <img src="/common_imgs/add_img.png" alt="">
                                </label>
                                <img src="" class="image-preview add-par-img-${paragraphsCount}" alt="">
                            </div>
                            <input id="add-par-img-${paragraphsCount}" accepts=".jpg, .jpeg, .png, .svg" type="file"  class="paragraph-img blog-input add-img">
                        </div>
                    </div>
        `

        console.log(`paragr ${paragraphsCount} added`);

        siblingBlock.insertAdjacentHTML('beforebegin', newParagraph);

        setEventListenersForMovingButtons();

        createHTMLEditor(editorsCount);
    }
    else if (parentElement.classList.contains('section-group')) {

        editorsCount++;

        console.log(`sec button ${buttonId} clicked`);

        let siblingBlock = document.querySelector(`.${buttonId} .add-subsection-block`);

        paragraphsCount++;

        subsectionsCount++;

        let newSubsection = `
        <div class="subsection-group subg-${subsectionsCount}">
                    <div class="move-buttons-group">
                                <button class="move-btn move-btn-up">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                                <button class="move-btn move-btn-down">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                            </div>
                    <button class="remove-btn">
                        <img src="/common_imgs/remove.png" alt="">
                    </button>
                    <div class="input-group">
                        <label>Название подраздела</label>
                        <input type="text" class="blog-input subsection-title">
                    </div>
                    
                    <div class="paragraph-group">
                        <div class="move-buttons-group">
                                <button class="move-btn move-btn-up">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                                <button class="move-btn move-btn-down">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                            </div>
                        <div class="area-group">
                            <label>Параграф</label>
                            <div class="paragraph" id="editor${editorsCount}" ></div>
                        </div>
                        <div class="img-group">
                            <div class="img-block">
                                <label for="add-par-img-${paragraphsCount}">
                                    <img src="/common_imgs/add_img.png" alt="">
                                </label>
                                <img src="" class="image-preview add-par-img-${paragraphsCount}" alt="">
                            </div>
                            <input id="add-par-img-${paragraphsCount}" accepts=".jpg, .jpeg, .png, .svg" type="file"  class="paragraph-img blog-input add-img">
                        </div>
                    </div>
                    <div class="add-paragraph-block add-block">
                        <button id="subg-${subsectionsCount}">добавить параграф</button>
                    </div>
                </div>
        `
        siblingBlock.insertAdjacentHTML('beforebegin', newSubsection);

        setEventListenersForMovingButtons();

        //document.getElementById(`subg-${subsectionsCount}`).addEventListener('click', (e)=>{

        //    let buttonId = document.getElementById(`subg-${subsectionsCount}`).getAttribute('id');

        //    addBlock(buttonId);

        //    e.preventDefault(false);

        //});

        addBlockButtons.forEach((b, i) => {

            b.removeEventListener('click', wrappers[i]);
        });

        wrappers = [];

        addBlockButtons = Array.from(document.querySelectorAll('.add-block button'));

        addBlockButtons.forEach(b => {
            let wrapper = setIdToAddBlogMethod.bind(this, b);

            wrappers.push(wrapper);

            b.addEventListener('click', wrapper)
        });

        createHTMLEditor(editorsCount);
    }
    else if (parentElement.classList.contains('blog-group')) {

        editorsCount++;

        let siblingBlock = document.querySelector(`.${buttonId} .add-section-block`);

        paragraphsCount++;

        subsectionsCount++;

        sectionsCount++;

        let newSection = `
        <div class="section-group sg-${sectionsCount}">
                <div class="move-buttons-group">
                                <button class="move-btn move-btn-up">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                                <button class="move-btn move-btn-down">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                            </div>
                <button class="remove-btn">
                    <img src="/common_imgs/remove.png" alt="">
                </button>
                <div class="input-group">
                    <label>Название раздела</label>
                    <input type="text" class="blog-input section-title">
                </div>

                <div class="subsection-group subg-${subsectionsCount}">
                    <div class="move-buttons-group">
                                <button class="move-btn move-btn-up">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                                <button class="move-btn move-btn-down">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                            </div>
                    <div class="input-group">
                        <label>Название подраздела</label>
                        <input type="text" class="subsection-title blog-input">
                    </div>
                    
                    <div class="paragraph-group">
                        <div class="move-buttons-group">
                                <button class="move-btn move-btn-up">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                                <button class="move-btn move-btn-down">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                            </div>
                        <div class="area-group">
                            <label>Параграф</label>
                            <div class="paragraph" id="editor${editorsCount}"></div>
                        </div>
                        <div class="img-group">
                            <div class="img-block">
                                <label for="add-par-img-${paragraphsCount}">
                                    <img src="/common_imgs/add_img.png" alt="">
                                </label>
                                <img src="" class="image-preview add-par-img-${paragraphsCount}" alt="">
                            </div>
                            <input id="add-par-img-${paragraphsCount}" accepts=".jpg, .jpeg, .png, .svg" type="file"  class="paragraph-img blog-input add-img">
                        </div>
                    </div>
                    <div class="add-paragraph-block add-block">
                        <button id="subg-${subsectionsCount}">добавить параграф</button>
                    </div>
                </div>
                <div class="add-subsection-block add-block">
                    <button id="sg-${sectionsCount}">добавить подраздел</button>
                </div>
            </div>
        `
        siblingBlock.insertAdjacentHTML('beforebegin', newSection);

        setEventListenersForMovingButtons();

        console.log(`paragr ${paragraphsCount} added`);
        console.log(`subsec ${subsectionsCount} added`);
        console.log(`sec ${sectionsCount} added`);

        addBlockButtons.forEach((b, i) => {

            b.removeEventListener('click', wrappers[i]);
        });

        wrappers = [];

        addBlockButtons = Array.from(document.querySelectorAll('.add-block button'));

        addBlockButtons.forEach(b => {
            let wrapper = setIdToAddBlogMethod.bind(this, b);

            wrappers.push(wrapper);

            b.addEventListener('click', wrapper)
        });

        createHTMLEditor(editorsCount);

        //document.getElementById(`subg-${subsectionsCount}`).addEventListener('click', (e)=>{

        //    let buttonId = document.getElementById(`subg-${subsectionsCount}`).getAttribute('id');

        //    addBlock(buttonId);

        //    e.preventDefault(false);

        //});

        //document.getElementById(`sg-${sectionsCount}`).addEventListener('click', (e)=>{

        //    let buttonId = document.getElementById(`sg-${sectionsCount}`).getAttribute('id');

        //    addBlock(buttonId);

        //    e.preventDefault(false);

        //});
    }

    Array.from(document.querySelectorAll('button')).forEach(b =>{
        b.addEventListener('click', (e)=>{
            e.preventDefault(false);
        })
    });

    AddFileInputs = Array.from(document.querySelectorAll('.add-img'));

    AddFileInputs.forEach(i =>{

        i.addEventListener('change', ()=>{

            let file = i.files[0];

            let src = URL.createObjectURL(file);

            let idName = i.getAttribute('id');

            let neededImagePreview = document.querySelector(`.${idName}`);

            neededImagePreview.src = src;
        })
    });

    document.querySelectorAll('.remove-btn').forEach(btn =>{
        btn.addEventListener('click', (e)=>{
            let parentElementForDelete = btn.parentElement;
    
            parentElementForDelete.remove();

            console.log("removed");

            e.preventDefault(false);
        });
    });
}