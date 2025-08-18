async function getDataAsync(url) {
    return new Promise((res, rej) => {

        let response = fetch(url);

        if (response.ok) {
            res(response.json())
        }
        else {
            rej(new Error(response.statusText));
        }
    });
}

async function setDataAsync(url, method, data) {
    return new Promise((res, rej) => {

        let response = null;

        try {
            response = fetch(url,{
                method: method,
                mode:'cors',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            res(response);
        } catch {
            rej(new Error(response.statusText));
        }
    })
}

//add comment section
let userId = document.querySelector('.user-id-input');
let blogAlias = document.querySelector('.blog-alias-input');
let langCode = document.querySelector('.lang-code-input');

let publishCommentButton = document.getElementById('publish-comment-button');
let commentTextArea = document.querySelector('.comment-area');
let commentList = document.querySelector('.comment-list');

async function createCommentAsync() {
    let data = {
        "text": commentTextArea.value,
        "blogAlias": blogAlias.value,
        "userId": parseInt(userId.value),
        "langCode": langCode.value
    }

    let response = await setDataAsync("/blog/api/comment/create", "POST", data);

    if (response.ok) {
        constructComment(data.text);
    }
}

function constructComment(commentText) {
    let commentTag = document.createElement('div');
    commentTag.setAttribute('class', 'comment user-comment');
    commentList.appendChild(commentTag);

    let commentDataTag = document.createElement('div');
    commentDataTag.setAttribute('class', 'comment-data');
    commentTag.appendChild(commentDataTag);

    let commentDataTextTag = document.createElement('div');
    commentDataTextTag.setAttribute('class', 'comment-text-data');
    commentDataTag.appendChild(commentDataTextTag);

    let commentTextSpanTag = document.createElement('span');
    commentTextSpanTag.setAttribute('class', 'comment-text-value');
    commentTextSpanTag.textContent = commentText;
    commentDataTextTag.appendChild(commentTextSpanTag)
}

if (userId.value !== '0') {
    console.log(userId.value)
    publishCommentButton.addEventListener('click', (e) => {
        e.preventDefault(false);
        console.log(e.target);
        if (userId != null) {
            createCommentAsync();
        }
    });
}


//open answer form section, close, publish answer

let showAnswerFormButtons = Array.from(document.querySelectorAll('.show-answer-form-btn'));
let answerForms = Array.from(document.querySelectorAll('.answer-form'));
let closeAnswerFormButtons = Array.from(document.querySelectorAll('.close-answer-form-btn'));
let publishAnswerButtons = Array.from(document.querySelectorAll('.publish-answer-btn'));
let answerInputs = Array.from(document.querySelectorAll('.comment-answer-input'));
let answerLists = Array.from(document.querySelectorAll('.comment-answers'));

async function publishAnswer(text, commentId) {
    let data = {
        "text": text,
        "userId": userId.value,
        "commentId": commentId,
        "langCode": langCode.value
    }

    let response = await setDataAsync("/blog/api/comment/answer/create", "POST", data);

    if (response.ok) {
        constructAnswer(text, commentId)
    }
}

function constructAnswer(answerText, commentId) {
    let commentTag = document.createElement('div');
    commentTag.setAttribute('class', 'comment user-comment');

    answerLists.forEach(al => {
        let answerListCommentId = parseInt(al.getAttribute('answer-comment-id'));

        if (answerListCommentId === commentId) {

            al.appendChild(commentTag);
        }
    })

    let commentTextSpanTag = document.createElement('span');
    commentTextSpanTag.setAttribute('class', 'comment-text-value');
    commentTextSpanTag.textContent = answerText;
    commentTag.appendChild(commentTextSpanTag);
}

publishAnswerButtons.forEach(b => {
    b.addEventListener('click', (e) => {

        e.preventDefault(false);

        const commentId = parseInt(b.getAttribute('answer-publish-button-id'));
        let text = '';

        answerInputs.forEach(i => {
            const inputCommentId = parseInt(i.getAttribute('anser-input-id'));
            if (inputCommentId === commentId) {
                text = i.value;
                publishAnswer(text, commentId);
            }
        });
    });
});

showAnswerFormButtons.forEach(b => {
    b.addEventListener('click', (e) => {

        e.preventDefault(false);

        const buttonCommentIdValue = b.getAttribute('answer-open-button-id');

        answerForms.forEach(f => {
            const formCommentIdValue = f.getAttribute('answer-form-id');

            if (formCommentIdValue === buttonCommentIdValue) {

                f.style.display = 'flex';

                return;
            }
        });
    });
});

closeAnswerFormButtons.forEach(b => {
    b.addEventListener('click', (e) => {

        e.preventDefault(false);

        const buttonCommentIdValue = b.getAttribute('answer-close-button-id');

        answerForms.forEach(f => {
            const formCommentIdValue = f.getAttribute('answer-form-id');

            if (formCommentIdValue === buttonCommentIdValue) {

                f.style.display = 'none';

                return;
            }
        });
    });
});


///translate

let translateCommentButtons = document.querySelectorAll('.ctbtn');
let translateAnswerButtons = document.querySelectorAll('.com-ans-tr-btn');

let commentTexts = document.querySelectorAll('.unique-text-value');
let answerTexts = document.querySelectorAll('.answer-text-value');

async function translateCommentTextAsync(textData, lang, commentId) {
    const data = {
        "text": textData,
        "langCode": lang,
        "targetId": commentId
    }

    let response = await setDataAsync('/blog/api/comment/translate/withsaving', 'POST', data);

    if (response.ok)
    {
        let text = response.text()

        return text;
    }
    else
    {
        throw new Error(response.statusText);
    }
}

async function translateAnswerTextAsync(textData, lang, answerId) {
    const data = {
        "text": textData,
        "langCode": lang,
        "targetId": answerId
    }

    let response = await setDataAsync('/blog/api/comment/answer/translate/withsaving', 'POST', data);

    if (response.ok) {
        let text = response.text()

        return text;
    }
    else {
        throw new Error(response.statusText);
    }
}

Array.from(translateCommentButtons).forEach(b => {
    b.addEventListener('click', () => {
       
        const comId1 = b.getAttribute('comment-id-value');

        Array.from(commentTexts).forEach(async c => {
            const comId2 = c.getAttribute('comment-id-value');

            if (comId1 === comId2) {

                const isButtonForTranslate = b.getAttribute('for-translate');

                if (isButtonForTranslate === 'true') {

                    const translateTextAttributeValue = c.getAttribute('tran-text');

                    if (translateTextAttributeValue !== '') {

                        c.textContent = translateTextAttributeValue;
                    }
                    else {
                        const origTextAttributeValue = c.getAttribute('orig-text');

                        const translatedOrigText = await translateCommentTextAsync(origTextAttributeValue, langCode.value, comId1);

                        c.setAttribute('tran-text', translatedOrigText);

                        c.textContent = translatedOrigText;
                    }

                    b.setAttribute('for-translate', 'false');

                    b.textContent = 'show original';
                }
                else {
                    const origTextAttributeValue = c.getAttribute('orig-text');

                    c.textContent = origTextAttributeValue;

                    b.setAttribute('for-translate', 'true');

                    b.textContent = 'translate';
                }

                return
            }
        });
    });
});

Array.from(translateAnswerButtons).forEach(b => {
    b.addEventListener('click', () => {

        const answerId1 = b.getAttribute('answer-id-value');

        Array.from(answerTexts).forEach(async c => {
            const answerId2 = c.getAttribute('answer-id-value');

            if (answerId1 === answerId2) {

                const isButtonForTranslate = b.getAttribute('for-translate');

                if (isButtonForTranslate === 'true') {

                    const translateTextAttributeValue = c.getAttribute('tran-text');

                    if (translateTextAttributeValue !== '') {

                        c.textContent = translateTextAttributeValue;
                    }
                    else {
                        const origTextAttributeValue = c.getAttribute('orig-text');

                        const translatedOrigText = await translateAnswerTextAsync(origTextAttributeValue, langCode.value, answerId1);

                        c.setAttribute('tran-text', translatedOrigText);

                        c.textContent = translatedOrigText;
                    }

                    b.setAttribute('for-translate', 'false');

                    b.textContent = 'show original';
                }
                else {
                    const origTextAttributeValue = c.getAttribute('orig-text');

                    c.textContent = origTextAttributeValue;

                    b.setAttribute('for-translate', 'true');

                    b.textContent = 'translate';
                }

                return
            }
        });
    });
});


