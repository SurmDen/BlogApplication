function setSideBarHeight(){

    let side = document.querySelector('.side-bar');

    if(document.documentElement.offsetWidth > 805){
        

        let workHeigth = document.querySelector('section').clientHeight;

        side.style.height = workHeigth - document.querySelector('.header').clientHeight + 'px';
    }
    else{
        
        side.style.height = 'fit-content'
    }
}

setSideBarHeight();

window.addEventListener('resize', () =>{
    setSideBarHeight();
});

let catInput = document.querySelector('.cat-form input');

let catButton = document.querySelector('.cat-form button');

let disabled = true;

catInput.addEventListener('focus', (e)=>{

    catButton.disabled = false;

    catInput.style.border = '2px solid rgb(31, 122, 201)';

    if(e.target.value === 'введите название категории'){
        e.target.value = ''
    }
});

catInput.addEventListener('blur', (e)=>{

    catInput.style.border = '2px solid rgb(110, 110, 115)';

    if(e.target.value === ''){
        e.target.value = 'введите название категории'
    }
});

catInput.addEventListener('change', (e)=>{

    catButton.disabled = false;

    if(e.target.value === '' || e.target.value === 'введите название категории'){
        disabled = true;
    }
    else{
        disabled = false;
    }
});

catButton.addEventListener('mousedown', ()=>{
    if(disabled){

        catInput.style.border = '2px solid red'

        catButton.disabled = true;

        showErrorMessage("Введите название категории")
        
    }
});

let errorBox = document.querySelector('.error-box');

let errorButton = document.querySelector('.error-btn');

errorButton.addEventListener('click', () => {
    errorBox.style.display = 'none';
});
