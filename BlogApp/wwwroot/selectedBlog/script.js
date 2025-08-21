let test = Array.from(document.querySelectorAll('.section-title'))[0];
let navPannelContainer = document.querySelector('.mini-nav-pannel-container')
let navPannel = document.querySelector('.mini-nav-pannel');
let sideBar = document.querySelector('.side-bar');
let allSections = document.querySelectorAll('.section-title');
let allNavPannelRefs = document.querySelectorAll('.mn-section');

let allBlogImages = document.querySelectorAll('.imagination');
let hideImageButton = document.querySelector('.hide-image img');
let fullImageContainer = document.querySelector('.full-screen-image-container');
let fullImageElement = document.querySelector('.full-image');

let dataList = document.querySelectorAll('li');

if (dataList !== null) {
    console.log('not null')
    Array.from(dataList).forEach(i => {
        if (i.getAttribute('data-list') === 'bullet') {
            i.setAttribute('class', 'list-bullet')
        }
    })
}

Array.from(allBlogImages).forEach(i => {
    i.addEventListener('click', () => {

        let width = document.documentElement.clientWidth;

        let selectedImageSrc = i.getAttribute('src');

        fullImageElement.setAttribute('src', selectedImageSrc);

        fullImageContainer.style.display = 'flex';
    });
});

hideImageButton.addEventListener('click', () => {
    fullImageContainer.style.display = 'none';
});


let pos = navPannel.getAttribute('pos');

const isArabian = pos === 'l'

if (isArabian) {
    document.body.style.flexDirection = 'row-reverse';
}
else {
    document.body.style.flexDirection = 'row';
}

function displayNavPannel() {

    let width = document.documentElement.clientWidth;

    if (width > 1250) {
        if (window.scrollY >= test.offsetTop) {

            navPannelContainer.style.display = 'block';

            document.body.style.justifyContent = "flex-start";
        }
        else {

            navPannelContainer.style.display = 'none';

            document.body.style.justifyContent = "center";

        }
    }
    else {
        navPannelContainer.style.display = 'none';

        document.body.style.justifyContent = "center";

    }
}

//displayNavPannel();

let section = document.querySelector('section');

let sectionLeftWidth = parseInt(section.getBoundingClientRect().left);

window.addEventListener('resize', () => {
    //displayNavPannel();

    sectionLeftWidth = parseInt(section.getBoundingClientRect().left);
})


window.addEventListener('scroll', function () {

    let width = document.documentElement.clientWidth;


    if (width > 1250) {
        if (window.scrollY > test.offsetTop) {
            section.style.transform = `translateX(-${sectionLeftWidth}px)`;

            navPannelContainer.style.display = 'block';

            navPannel.style.transform = `translateX(-${sectionLeftWidth}px)`;

            //document.body.style.justifyContent = "flex-start";

        }
        else {
            
            section.style.transform = `translateX(0px)`

            navPannelContainer.style.display = 'none';

            navPannel.style.transform = `translateX(0px)`;

            //document.body.style.justifyContent = "center";
        }

        Array.from(allSections).forEach(s => {

            let secId = s.getAttribute('id');

            if (window.scrollY >= s.offsetTop && window.scrollY <= s.offsetTop + s.clientHeight) {

                console.log(secId)

                Array.from(allNavPannelRefs).forEach(r => {
                    let navRef = r.getAttribute('href');

                    if (navRef === '#' + secId) {
                        r.classList.add('active-mn-section');
                    }
                    else {
                        r.classList.remove('active-mn-section');
                    }
                })
            }
        });
    }
    else {
        navPannelContainer.style.display = 'none';

        document.body.style.justifyContent = "center";
    }
});

//scripts for loading telegram user

async function getJsonAsync(url) {

    const token = localStorage.getItem('access_token');

    let response = await fetch(url, {
        method:"GET",
        headers:{
            "Content-Type":"application/json",
            "Authorization" : token === null ? "unauthorized" : "Bearer " + token 
        },
        mode:"cors",
    });

    return response;
}

async function setJsonAsync(url = '', data = {}, method = '') {

    const token = localStorage.getItem('access_token');

    let response = await fetch(url, {
        method:method,
        headers:{
            "Content-Type":"application/json",
            "Authorization" : token === null ? "unauthorized" : "Bearer " + token 
        },
        mode:"cors",
        body:JSON.stringify(data)
    });

    return response;
}

async function setTelegramUserAsync(){

    let telegramUserObject = null;

    try {
        const userResponse = await getJsonAsync('https://api.bot-market.com/v1/private/bot/user/auth/get');

        if (userResponse?.ok) {
            
            telegramUserObject = await userResponse.json();

            console.log(`telegram user object: ${JSON.stringify(telegramUserObject)}`);

            document.querySelector('.login-ref').style.display = 'none';
            document.querySelector('.user-block').style.display = 'flex';

            const telegramUser = telegramUserObject.user;

            console.log(`telegram user: ${JSON.stringify(telegramUser)}`);

            let botDbUserAnswer = await getJsonAsync(`/blog/api/botuser/v1/get/telegram/${telegramUser.id}`);

            let botDbUser = await botDbUserAnswer.json();

            if (botDbUser.id === 0 || botDbUser.id === '0'){
                const createUserBotRequestObject = {
                    telegramAccountId: telegramUser.id,
                    userName: telegramUser.username,
                    firstName: telegramUser.firstName,
                    lastName: telegramUser.lastName,
                    link: telegramUser.link
                };

                botDbUserAnswer = await setJsonAsync('/blog/api/botuser/v1/create', createUserBotRequestObject, 'POST');

                botDbUser = await botDbUserAnswer.json();
            }

            localStorage.setItem('bot_user', botDbUser);

            await getJsonAsync(`/blog/api/botuser/v1/storage/set/${botDbUser.id}`);
        }
    } catch (error) {
        
    }
}

setTelegramUserAsync();
