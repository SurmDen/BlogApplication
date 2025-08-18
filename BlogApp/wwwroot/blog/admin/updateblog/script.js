 let loader  = document.querySelector('.load-box');

function validateInputs(){

    let isValid = true;

    let inputs = document.querySelectorAll('.input-group input');

    let areas = document.querySelectorAll('.area-group textarea');

    inputs.forEach(i =>{

        i.addEventListener('focus', ()=>{
            i.style.boxShadow = '0 0 10px rgb(49, 44, 44)';
        });

        if(i.value === ''){
            i.style.boxShadow = '0 0 8px red';

            isValid = false;
        }
    });

    areas.forEach(i =>{

        i.addEventListener('focus', ()=>{
            i.style.boxShadow = '0 0 10px rgb(49, 44, 44)';
        });

        if(i.value === ''){
            i.style.boxShadow = '0 0 8px red';

            isValid = false;
        }
    });

    return isValid;
}

let errorBox = document.querySelector('.error-box');

function showErrorMessage(message){
    let errorMessage = document.querySelector('.error-message');

    errorMessage.innerHTML = message;

    errorBox.style.display = 'flex';
}

let errorButton = document.querySelector('.error-btn');

errorButton.addEventListener('click', ()=>{
    errorBox.style.display = 'none';
});

const baseQueryForGet = '/blog/api/get/';

let idFromQuery = document.querySelector('.user-id').value;

let currentBlog = null;

let sectionsCount = 1;
let subsectionsCount = 1;
let paragraphsCount = 1;

function createHTMLEditor(count) {

    console.log("EDITOR " + count)

    const quill = new Quill(`#editor${count}`, {
        theme: 'snow'
    });
}

let editorsCount = 0;

async function getBlogById(){

    loader.style.display = 'flex';

    let requestForBlog = await fetch(baseQueryForGet + idFromQuery).then(null, () =>{

        loader.style.display = 'none';

        showErrorMessage('Не удалось отправить запрос, возможно сервер на данный момент не работает');
    });

    if(requestForBlog.ok){

        currentBlog = await requestForBlog.json();

        console.log(currentBlog);

        if(currentBlog !== null){
        
            let blogToHtml = `
            <div class="blog-group g">
                <input type="hidden" class="b-id" value=${currentBlog.id}>
                <input type="hidden" class="alias" value=${currentBlog.alias}>
                <input type="hidden" class="cat-id" value=${currentBlog.categoryId}>
                <input type="hidden" class="user-id" value=${currentBlog.userId}>
                <input type="hidden" class="lan-id" value=${currentBlog.languageId}>
                <div class="input-group">
                    <label>Название поста</label>
                    <input type="text" value="${currentBlog.title}" class="blog-input blog-title">
                </div>
                <div class="area-group">
                    <label>Введение</label>
                    <div id="editor${currentBlog.id}" class="blog-desc">${currentBlog.description}</div>
                </div>
                <div class="current-image-container">
                    <img class="current-image" src="${currentBlog.image}"/>
                </div>
                <div class="img-group">
                    <div class="img-block">
                        <label for="add-blog-img">
                            <img src="/common_imgs/add_img.png" alt="">
                        </label>
                        <img src="" class="image-preview add-blog-img" alt="">
                    </div>
                    <input id="add-blog-img" accepts=".jpg, .jpeg, .png, .svg" type="file"  class="blog-input add-img">
                </div>
        
                ${Array.from(currentBlog.sections).map((sec, indexS) => (

                    `<div class="section-group sg-${sec.id}">
                            <div class="move-buttons-group">
                                <button class="move-btn move-btn-up">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                                <button class="move-btn move-btn-down">
                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                </button>
                            </div>
                        <button id="${indexS}" class="remove-btn">
                            <img src="/common_imgs/remove.png" alt="">
                        </button>
                        <input type="hidden" class="sec-id" value=${sec.id}>
                        <input type="hidden" class="blog-id" value=${sec.blogId}>

                        <div class="input-group">
                            <label>Название раздела</label>
                            <input type="text" value="${sec.title}" class="blog-input section-title">
                        </div>
            
                        ${Array.from(sec.subsections).map((sub, indexSub) => (
                            `<div class="subsection-group subg-${sub.id}">
                                    <div class="move-buttons-group">
                                        <button class="move-btn move-btn-up">
                                            <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                        </button>
                                        <button class="move-btn move-btn-down">
                                            <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                        </button>
                                    </div>
                                    <button id="${indexSub}" class="remove-btn">
                                        <img src="/common_imgs/remove.png" alt="">
                                    </button>
                                    <input type="hidden" class="sub-id" value=${sub.id}>
                                    <input type="hidden" class="section-id" value=${sub.sectionId}>

                                    <div class="input-group">
                                        <label>Название подраздела</label>
                                        <input type="text" value="${sub.title}" class="blog-input subsection-title">
                                    </div>
                                
                                ${
                                    Array.from(sub.paragraphs).map((p, indexP) =>(
                                        `<div class="paragraph-group">
                                            <div class="move-buttons-group">
                                                <button class="move-btn move-btn-up">
                                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                                </button>
                                                <button class="move-btn move-btn-down">
                                                    <img src="/blog/admin/addblog/specific/move_arrow.png" />
                                                </button>
                                            </div>
                                            <button id="${indexP}" class="remove-btn">
                                                <img src="/common_imgs/remove.png" alt="">
                                            </button>
                                            <input type="hidden" class="par-id" value=${p.id}>
                                            <input type="hidden" class="subsection-id" value=${p.subsectionId}>

                                            <div class="area-group">
                                                <label>Параграф</label>
                                                <div id="editor${p.id}" db="${p.id}" class="paragraph">${p.text}</div>
                                            </div>
                                            <div class="current-image-container">
                                                <img class="current-image" src="${p.image}"/>
                                            </div>
                                            <div class="img-group">
                                                <div class="img-block">
                                                    <label for="add-par-img-${p.id}">
                                                        <img src="/common_imgs/add_img.png" alt="">
                                                    </label>
                                                    <img src="" class="image-preview add-par-img-${p.id}" alt="">
                                                </div>
                                                <input id="add-par-img-${p.id}" accepts=".jpg, .jpeg, .png, .svg" type="file"  class="paragraph-img blog-input add-img">
                                            </div>
                                    </div>`
                                    ))
                                }
                                <div class="add-paragraph-block add-block">
                                    <button id="subg-${sub.id}">добавить параграф</button>
                                </div>
                            </div>`
                            ))
                        }
                        <div class="add-subsection-block add-block">
                            <button id="sg-${sec.id}">добавить подраздел</button>
                        </div>
                    </div>`
                    ))
                }
                <div class="add-section-block add-block">
                    <button id="g">добавить раздел</button>
                </div>
            </div>
            `

            document.querySelector('.blog-form').insertAdjacentHTML('afterbegin', blogToHtml);

            createHTMLEditor(currentBlog.id);

            let paragraphs = document.querySelectorAll('.paragraph');

            Array.from(paragraphs).forEach(p => {

                let id = parseInt(p.getAttribute('db'));

                editorsCount = id;

                createHTMLEditor(id);

            });

            editorsCount++;

            let removeButtons = Array.from(document.querySelectorAll('.remove-btn'));

            removeButtons.forEach(b => {
                let id = parseInt(b.getAttribute('id'));

                if (id === 0) {
                    b.remove();
                }
            });

            setEventListenersForMovingButtons();

            loader.style.display = 'none';
        }
        else{
            loader.style.display = 'none';

            showErrorMessage('Не удалось загрузить пост, возможно сервер на данный момент не работает')
        }
    }

}

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



let setIdToAddBlogMethod = function (b) {

    let buttonId = b.getAttribute('id');

    addBlock(buttonId);
}

let addBlockButtons;

let wrappers = [];


getBlogById().then(() =>{

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

    addBlockButtons = Array.from(document.querySelectorAll('.add-block button'));

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
    
    window.addEventListener('resize', () =>{
        setSideBarHeight();
    })

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

    loader.style.display = 'flex';

    let blogJsonData = JSON.stringify(blogData);

    console.log(blogData);

    let request = await fetch("/blog/api/update",{
        method: "PUT",
        mode: 'cors',
        headers:{
            'Content-Type': 'application/json'
        },
        body: blogJsonData
    }).then(null, error => {loader.style.display = 'none'; showErrorMessage('Не удалось отправить запрос, возможно сервер на данный момент не работает')});

    if(request.ok){

        loader.style.display = 'none';

        showErrorMessage('Пост успешно обновлен!');
    }
    else{
        loader.style.display = 'none';

        showErrorMessage('На сервере произошла ошибка. Проверьте правильность заполнения полей в форме, а также стабильность подключения к сети')
    }

}

let publishButton = document.querySelector('.publish-btn');

publishButton.addEventListener('click', generateBlogAndSendToServer);

async function generateBlogAndSendToServer(){

    let isValid = validateInputs();

    if(!isValid){

        showErrorMessage('Все поля в форме должны быть заполнены');

        return;
    }

    let blogImage = document.querySelector('#add-blog-img').files[0];

    let blogBase64Image = ""

    if(blogImage !== undefined){
        blogBase64Image = await fileToBase64(blogImage);
    }

    let blogDesc = document.querySelector('.blog-desc');

    let blog = {
        id:parseInt(document.querySelector('.b-id').value),
        title: document.querySelector('.blog-title').value,
        description: blogDesc.querySelector('.ql-editor').innerHTML,
        imageBase64String:blogBase64Image,
        image:"",
        alias:document.querySelector('.alias').value,
        //dateOfPublish:"",
        //dateOfUpdate:"",
        sections: [],
        tagList:"",
        videoPath: currentBlog.videoPath == null ? "" : currentBlog.videoPath,
        videoName: currentBlog.videoName == null ? "" : currentBlog.videoName,
        user: null,
        userId:parseInt(document.querySelector('.user-id').value),
        language:null,
        languageId:parseInt(document.querySelector('.lan-id').value),
        category:null,
        categoryId:parseInt(document.querySelector('.cat-id').value)
    }

    let sections = Array.from(document.querySelectorAll('.section-group'));

    for(let i = 0; i < sections.length; i++){

        let s = sections[i];

        let section = {
            id:s.querySelector('.sec-id') === null ? 0 : parseInt(s.querySelector('.sec-id').value),
            title:s.querySelector('.section-title').value,
            subsections:[],
            blog:null,
            blogId:s.querySelector('.blog-id') === null ? parseInt(document.querySelector('.b-id').value) : parseInt(s.querySelector('.blog-id').value)
        }

        blog.sections.push(section);

        let subsections = Array.from(s.querySelectorAll('.subsection-group'));

        for(let j = 0; j < subsections.length; j++){

            let sub = subsections[j];

            let subsection = {
                id:sub.querySelector('.sub-id') === null ? 0 : parseInt(sub.querySelector('.sub-id').value),
                title:sub.querySelector('.subsection-title').value,
                paragraphs:[],
                section:null,
                sectionId:sub.querySelector('.section-id') === null ? 0 : parseInt(sub.querySelector('.section-id').value)
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
                    id:p.querySelector('.par-id') === null ? 0 : p.querySelector('.par-id').value,
                    text: p.querySelector('.ql-editor').innerHTML,
                    image:"",
                    imageBase64String: parBase64Image,
                    subsection:null,
                    subsectionId:p.querySelector('.subsection-id') === null ? 0 : p.querySelector('.subsection-id').value
                }

                subsection.paragraphs.push(paragraph);
            };

        };

    };

    sendDataToServer(blog);
}


let sections = 10000000;
let subsections = 10000000;
let paragraphs = 10000000;

function addBlock(buttonId){

    let parentElement = document.querySelector(`.${buttonId}`);

    if(parentElement.classList.contains('subsection-group')){

        let siblingBlock = document.querySelector(`.${buttonId} .add-paragraph-block`);

        paragraphs++;

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
                            <div id="editor${editorsCount}" class="paragraph"></div>
                        </div>
                        <div class="img-group">
                            <div class="img-block">
                                <label for="add-par-img-${paragraphs}">
                                    <img src="/common_imgs/add_img.png" alt="">
                                </label>
                                <img src="" class="image-preview add-par-img-${paragraphs}" alt="">
                            </div>
                            <input id="add-par-img-${paragraphs}" accepts=".jpg, .jpeg, .png, .svg" type="file"  class="paragraph-img blog-input add-img">
                        </div>
                    </div>
        `

        siblingBlock.insertAdjacentHTML('beforebegin', newParagraph);
    }
    else if(parentElement.classList.contains('section-group')){

        let siblingBlock = document.querySelector(`.${buttonId} .add-subsection-block`);

        paragraphs++;

        subsections++;

        let newSubsection = `
        <div class="subsection-group subg-${subsections}">
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
                            <div id="editor${editorsCount}" class="paragraph"></div>
                        </div>
                        <div class="img-group">
                            <div class="img-block">
                                <label for="add-par-img-${paragraphs}">
                                    <img src="/common_imgs/add_img.png" alt="">
                                </label>
                                <img src="" class="image-preview add-par-img-${paragraphs}" alt="">
                            </div>
                            <input id="add-par-img-${paragraphs}" accepts=".jpg, .jpeg, .png, .svg" type="file"  class="paragraph-img blog-input add-img">
                        </div>
                    </div>
                    <div class="add-paragraph-block add-block">
                        <button id="subg-${subsections}">добавить параграф</button>
                    </div>
                </div>
        `
        siblingBlock.insertAdjacentHTML('beforebegin', newSubsection);

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

    }
    else if(parentElement.classList.contains('blog-group')){

        let siblingBlock = document.querySelector(`.${buttonId} .add-section-block`);

        paragraphs++;

        subsections++;

        sections++;

        let newSection = `
        <div class="section-group sg-${sections}">
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

                <div class="subsection-group subg-${subsections}">
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
                            <div id="editor${editorsCount}" class="paragraph"></div>
                        </div>
                        <div class="img-group">
                            <div class="img-block">
                                <label for="add-par-img-${paragraphs}">
                                    <img src="/common_imgs/add_img.png" alt="">
                                </label>
                                <img src="" class="image-preview add-par-img-${paragraphs}" alt="">
                            </div>
                            <input id="add-par-img-${paragraphs}" accepts=".jpg, .jpeg, .png, .svg" type="file"  class="paragraph-img blog-input add-img">
                        </div>
                    </div>
                    <div class="add-paragraph-block add-block">
                        <button id="subg-${subsections}">добавить параграф</button>
                    </div>
                </div>
                <div class="add-subsection-block add-block">
                    <button id="sg-${sections}">добавить подраздел</button>
                </div>
            </div>
        `
        siblingBlock.insertAdjacentHTML('beforebegin', newSection);


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
    }

    createHTMLEditor(editorsCount);

    editorsCount++;

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

            e.preventDefault(false);
        });
    });

    setEventListenersForMovingButtons();
    
}