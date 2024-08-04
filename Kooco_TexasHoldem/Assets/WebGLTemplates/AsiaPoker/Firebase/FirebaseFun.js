// Firebase初始化
function initializeFirebase() {
    // Firebase 配置
    const firebaseConfig = {
        apiKey: "AIzaSyDbgc-2YxTTzwZdmKCG57Fz6dFK9LCPAxY",
        authDomain: "asia-poker-5959b.firebaseapp.com",
        databaseURL: "https://asia-poker-5959b-default-rtdb.firebaseio.com",
        projectId: "asia-poker-5959b",
        storageBucket: "asia-poker-5959b.appspot.com",
        messagingSenderId: "320309465715",
        appId: "1:320309465715:web:20b2b12ee48ffb7929d006"
    };

    // 初始化 Firebase
    const app = firebase.initializeApp(firebaseConfig);
    const db = firebase.firestore();
    const auth = firebase.auth();
    auth.useDeviceLanguage();
}

// 設置Recaptcha驗證監聽
function setupRecaptchaVerifier() {
    window.recaptchaVerifier = new firebase.auth.RecaptchaVerifier('recaptcha-button', {
        'size': 'invisible',
        'callback': (response) => {
            console.log('reCAPTCHA solved');
            signInWithPhoneNumber();
        }
    });

    window.recaptchaVerifier.render().then((widgetId) => {
        window.recaptchaWidgetId = widgetId;
    });
}

// 發送OTP
function signInWithPhoneNumber() {
    console.log("Send OTP To:" + window.currPhoneNumber);
    var appVerifier = window.recaptchaVerifier;
    firebase.auth().signInWithPhoneNumber(window.currPhoneNumber, appVerifier)
        .then((confirmationResult) => {
            window.confirmationResult = confirmationResult;
            console.log('SMS sent!!!');
        })
        .catch((error) => {
            console.error('Error during signInWithPhoneNumber:', error);
        });
}

// 驗證OTP 
function verifyCode(code, type) {
    window.confirmationResult.confirm(code).then((result) => {
        console.log("User signed in successfully!!!");
        const user = result.user;

        switch(type)
        {
            //錢包登入
            case "Wallet":
                window.unityInstance.SendMessage("LoginView", "OnWalletOTPSuccess");
                break;

            //手機註冊
            case "Register":
                window.unityInstance.SendMessage("LoginView", "OnRegisterOTPSuccess");
                break;
            
            //忘記密碼
            case "LostPsw":
                window.unityInstance.SendMessage("LoginView", "OnLostPswOTPSuccess");
                break;
        }
      }).catch((error) => {
        console.log("Verify Code Error : " + error);

        switch(type)
        {
            //錢包登入
            case "Wallet":
                window.unityInstance.SendMessage("LoginView", "OnWalletLoginOTPCodeError");
                break;

            //手機註冊
            case "Register":
                window.unityInstance.SendMessage("LoginView", "OnRegisterOTPCodeError");
                break;
            
            //忘記密碼
            case "LostPsw":
                window.unityInstance.SendMessage("LoginView", "OnLostPswOTPCodeError");
                break;
        }
      });
}

/*
// 觸發Recaptcha驗證
function triggerRecaptcha(phoneNumber) {
    window.currPhoneNumber = phoneNumber;
    console.log('Phone number set for reCAPTCHA:', phoneNumber);
    document.getElementById('recaptcha-button').click();
}

// 寫入資料
function writeDate(jsonData) {

    firebase.database().ref(refPath).set(jsonData, (error) => {
        if (error) {
            console.error("The write failed... : " + error);
        } else {
            console.log("Data saved successfully!");
        }
    });
}

// 修改與擴充資料
function updateDate(jsonData) {
    firebase.database().ref(refPath).update(jsonData, (error) => {
        if (error) {
            console.error("The update failed... : " + error);
        } else {
            console.log("Data updated successfully!");
        }
    });
}
*/
/*function writeLoginData(phoneNumber, password) {    
    firebase.database().ref('phone/' + phoneNumber).set({
        phoneNumber: phoneNumber,
        password: password
    }, (error) => {
        if (error) {
            console.error("The write failed... : " + error);
        } else {
            console.log("Data saved successfully!");
        }
    });
}*/

//當文檔加載完成時
document.addEventListener('DOMContentLoaded', (event) => {
    initializeFirebase();
});
