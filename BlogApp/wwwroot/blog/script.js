let pagButtons = document.querySelectorAll('.pag-btn');

Array.from(pagButtons).forEach(p => {
    p.addEventListener('mousedown', () => {
        document.querySelector('.pag-input').value = p.value;
    });
});

let tagPannel = document.querySelector('.tag-search-pannel');
let openTagPannelBtn = document.querySelector('.tag-search-btn');
let closeTagPannelBtn = document.querySelector('.close-tag-pannel button');
let tags = document.querySelectorAll('.tag');
let tagInput = document.querySelector('.tag-input');
let tagPostButton = document.querySelector('.tag-post-btn');

openTagPannelBtn.addEventListener('click', () => {
    tagPannel.style.display = 'block';
});

closeTagPannelBtn.addEventListener('click', () => {
    tagPannel.style.display = 'none';
})

let selectedButtonIds = [];

Array.from(tags).forEach(t => {

    t.addEventListener('click', () => {
        if (t.classList.contains('tag-active')) {

            let tagId = parseInt(t.getAttribute('id'));

            let index = selectedButtonIds.indexOf(tagId);

            selectedButtonIds.splice(index, 1);

            tagInput.value = selectedButtonIds.toString();

            t.classList.remove('tag-active');
        }
        else {
            let tagId = parseInt(t.getAttribute('id'));

            selectedButtonIds.push(tagId);

            tagInput.value = selectedButtonIds.toString();

            console.log(tagInput.value);

            t.classList.add('tag-active');
        }
    });
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
// {
//     "id": "01989f1f-87f2-75d8-8e04-e59693375fec",
//     "botId": "01983d64-899b-7892-bdb6-73f1782e85e0",
//     "user": {
//         "id": 1244789214,
//         "username": "surm_den",
//         "firstName": "Jordani Jovanovich",
//         "lastName": "",
//         "link": "@surm_den",
//         "type": "private"
//     },
//     "languageId": 1,
//     "status": {
//         "id": 1,
//         "title": "ACTIVE"
//     },
//     "createdTime": "2025-08-12 19:31:40",
//     "updatedTime": "2025-08-14 17:15:34",
//     "createdAt": 1755016300,
//     "updatedAt": 1755180934,
//     "expectation": null,
//     "attributes": [],
//     "attributesCustom": {
//         "polic": {
//             "id": "0198a544-1f6b-71c0-934c-890d0d414b0a",
//             "attribute": {
//                 "id": "01983d7b-7955-7f4a-927f-97da760a1143",
//                 "name": "polic",
//                 "title": "polic",
//                 "type": "boolean",
//                 "system": "bot_user_custom",
//                 "componentId": "01983d71-ecfd-7dce-b105-9734ce6bf72a"
//             },
//             "objectId": "01989f1f-87f2-75d8-8e04-e59693375fec",
//             "value": "1"
//         }
//     }
// }
