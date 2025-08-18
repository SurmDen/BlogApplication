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
                    <textarea class="blog-desc">${currentBlog.description}</textarea>
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
                        
                        <input type="hidden" class="sec-id" value=${sec.id}>
                        <input type="hidden" class="blog-id" value=${sec.blogId}>

                        <div class="input-group">
                            <label>Название раздела</label>
                            <input type="text" value="${sec.title}" class="blog-input section-title">
                        </div>
            
                        ${Array.from(sec.subsections).map((sub, indexSub) => (
                            `<div class="subsection-group subg-${sub.id}">
                                    
                                    
                                    <input type="hidden" class="sub-id" value=${sub.id}>
                                    <input type="hidden" class="section-id" value=${sub.sectionId}>

                                    <div class="input-group">
                                        <label>Название подраздела</label>
                                        <input type="text" value="${sub.title}" class="blog-input subsection-title">
                                    </div>
                                
                                ${
                                    Array.from(sub.paragraphs).map((p, indexP) =>(
                                        `<div class="paragraph-group">
                                            <input type="hidden" class="par-id" value=${p.id}>
                                            <input type="hidden" class="subsection-id" value=${p.subsectionId}>

                                            <div class="area-group">
                                                <label>Параграф</label>
                                                <textarea class="paragraph">${p.text}</textarea>
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
                                
                            </div>`
                            ))
                        }
                        
                    </div>`
                    ))
                }
                
            </div>
            `
        
            document.querySelector('.blog-form').insertAdjacentHTML('afterbegin', blogToHtml);

            let removeButtons = Array.from(document.querySelectorAll('.remove-btn'));

            removeButtons.forEach(b => {
                let id = parseInt(b.getAttribute('id'));

                if (id === 0) {
                    b.remove();
                }
            });

            loader.style.display = 'none';
        }
        else{
            loader.style.display = 'none';

            showErrorMessage('Не удалось загрузить пост, возможно сервер на данный момент не работает')
        }
    }

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

    let request = await fetch("/blog/api/update/specific",{
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

    let blog = {
        id:parseInt(document.querySelector('.b-id').value),
        title:document.querySelector('.blog-title').value,
        description:document.querySelector('.blog-desc').value,
        imageBase64String:blogBase64Image,
        image:"",
        alias:document.querySelector('.alias').value,
        //dateOfPublish:"",
        //dateOfUpdate:"",
        sections:[],
        user:null,
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
                    text: p.querySelector('.paragraph').value,
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