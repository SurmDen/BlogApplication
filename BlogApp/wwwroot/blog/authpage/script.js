const createQRCodeRequestURL = 'https://api.bot-market.com/v1/auth/code/create';
const getTokensByCodeRequestURL = 'https://api.bot-market.com/v1/auth/code';
const telegramBotURL = 'https://t.me/WELCOME_MARKET_BOT';

const searchParams = new URLSearchParams(window.location.search);
const redirectPath = searchParams.get('redirect') ?? '/blog/main/en';

async function getAsync(url){

    const token = localStorage.getItem('access_token');
  
    let response = await fetch(url,
        {
            method:"GET",
            headers:{
                "Content-Type":"application/json",
                "Authorization" : token === null ? "unauthorized" : "Bearer " + token 
            },
            mode:"cors",
            
    });
  
    return response;
  };
  
  async function setDataAsync(url = "", data = {}, method = ""){
  
    const token = localStorage.getItem('access_token');
  
    let response = await fetch(url, {
        method: method,
        headers:{
            "Content-Type":"application/json",
            "Authorization" : token === null ? "unauthorized" : "Bearer " + token 
        },
        mode:"cors",
        body:JSON.stringify(data)
    });
  
    return response;
  };
  
  async function setDataJsonAsync(url = "", data = {}, method = ""){
  
    const token = localStorage.getItem('access_token');
  
    let response = await fetch(url, {
        method: method,
        headers:{
            "Content-Type":"application/json",
            "Authorization" : token === null ? "unauthorized" : "Bearer " + token 
        },
        body: data !== null ? JSON.stringify(data) : ''
    });

    return response;
};

localStorage.removeItem('access_token');
localStorage.removeItem('refresh_token');
localStorage.removeItem('bot_user');

getAsync("/blog/api/botuser/v1/storage/remove");

async function registerByQRCodeAsync(){

    document.querySelector('.spinner-container').style.display = 'block';
    document.querySelector('.code-error').style.display = 'none';

    let response = null;

    try {
        response = await setDataJsonAsync(createQRCodeRequestURL, null, 'POST');
    } catch (error) {
        document.querySelector('.spinner-container').style.display = 'none';
        document.querySelector('.code-error').style.display = 'block';
    }

    if (response?.ok) {

        let codeString = await response.json();

        if (codeString.code !== '') {

            const telegramBotFullRef = `${telegramBotURL}?start=a_${codeString.code}`;

            document.querySelector('.bot-auth-btn a').setAttribute('href', telegramBotFullRef);

            generateQRCodeImage(telegramBotFullRef);

            document.querySelector('.spinner-container').style.display = 'none';

            console.log("start listening");

            let tokenData = null;
            let sentRequestCouner = 0;

            while (tokenData === null && sentRequestCouner < 100) {

                await waitFor(5);
                
                let tokenDataRespose = await setDataJsonAsync(`${getTokensByCodeRequestURL}?code=${codeString.code}`, null, 'POST');

                if (tokenDataRespose.ok) {
                    tokenData = await tokenDataRespose.json();
                }

                sentRequestCouner++;
            }

            if (tokenData !== null) {
                
                localStorage.setItem('access_token', tokenData.accessToken);
                localStorage.setItem('refresh_token', tokenData.refreshToken);

                window.location.href = redirectPath;
            }
            else{
                let telegramQRCodeImage = document.querySelector('.code');

                if (telegramQRCodeImage) {
                    telegramQRCodeImage.remove();
                }

                document.querySelector('.bot-auth-btn a').removeAttribute('href');
            }
        }
        else{
            document.querySelector('.spinner-container').style.display = 'none';
            document.querySelector('.code-error').style.display = 'block';
        }
    }
    else{
        document.querySelector('.spinner-container').style.display = 'none';
        document.querySelector('.code-error').style.display = 'block';
    }
}

async function waitFor(sec = 0){
    return new Promise((res) =>{
        setTimeout(() =>{
            res();
        }, sec * 1000)
    })
}

async function generateQRCodeImage(code){
    QRCode.toDataURL(code, function(err, url) {
        if (err) {
          console.error(err);
          return;
        }
        
        let codeImage = document.createElement('img');
        codeImage.classList.add('code');
        codeImage.src = url;

        document.querySelector('.code-data').insertAdjacentElement('afterbegin', codeImage);
    });
}

registerByQRCodeAsync();

document.querySelector('.refresh-code-btn').addEventListener('click', () =>{
    registerByQRCodeAsync();
});