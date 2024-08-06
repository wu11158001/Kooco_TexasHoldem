mergeInto(LibraryManager.library, {

    // 驗證OTP 
    // code = 驗證碼
    // objNamePtr = 回傳物件名
    // callbackFunPtr = 回傳方法名
    JS_FirebaseVerifyCode: function(codeStr, objNamePtr, callbackFunPtr) {
        const code = UTF8ToString(codeStr);
        const gameObjectName = UTF8ToString(objNamePtr);
        const callbackFunctionName = UTF8ToString(callbackFunPtr);

        window.confirmationResult.confirm(code).then((result) => {
            console.log("User signed in successfully!!!");
            const user = result.user;

            window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, "true");
        }).catch((error) => {
            console.log("Verify Code Error : " + error);

            window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, "false");
        });
    },

    // 開始監聽資料
    // pathPtr = 資料路徑
    // objNamePtr = 回傳物件名
    // callbackFunPtr = 回傳方法名
    JS_StartListeningForDataChanges: function(pathPtr, objNamePtr, callbackFunPtr) {
        const refPath = UTF8ToString(pathPtr);
        window.ListeningGameObjectName = UTF8ToString(objNamePtr);
        window.ListeningCallbackFunctionName = UTF8ToString(callbackFunPtr);

        var dbRef = firebase.database().ref(refPath);
        dbRef.on('value', function(snapshot) {
            var jsonData = JSON.stringify(snapshot.val());
            window.unityInstance.SendMessage(window.ListeningGameObjectName, window.ListeningCallbackFunctionName, jsonData);
        });
    },

    // 停止監聽資料
    // pathPtr = 資料路徑
    JS_StopListeningForDataChanges: function(pathPtr) {
        var dbRef = firebase.database().ref(UTF8ToString(pathPtr));
        dbRef.off('value');
    },

    // 寫入資料
    // refPathPtr = 資料路徑
    // jsonDataPtr = json資料
    JS_WriteDataFromFirebase: function(refPathPtr, jsonDataPtr, objNamePtr = null, callbackFunPtr = null) {
        const refPath = UTF8ToString(refPathPtr);
        const jsonData = UTF8ToString(jsonDataPtr);
        const data = JSON.parse(jsonData);

        let gameObjectName = null;
        let callbackFunctionName = null;
        if (objNamePtr && callbackFunPtr) {
            gameObjectName = UTF8ToString(objNamePtr);
            callbackFunctionName = UTF8ToString(callbackFunPtr);
        }

        firebase.database().ref(refPath).set(data, (error) => {
            if (error) {
                console.error("The write failed... : " + error);

                if (gameObjectName != null && callbackFunctionName != null) {
                    window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, "false");
                }
            } else {
                console.log("Data saved successfully!");

                if (gameObjectName != null && callbackFunctionName != null) {
                    window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, "true");
                }
            }
        });
    },

    // 修改與擴充資料
    // refPathPtr = 資料路徑
    // jsonDataPtr = json資料
    JS_UpdateDataFromFirebase: function(refPathPtr, jsonDataPtr) {
        const refPath = UTF8ToString(refPathPtr);
        const jsonData = UTF8ToString(jsonDataPtr);
        const data = JSON.parse(jsonData);

        firebase.database().ref(refPath).update(data, (error) => {
            if (error) {
                console.error("The update failed... : " + error);
            } else {
                console.log("Data updated successfully!");
            }
        });
    },

    // 讀取資料
    // refPathPtr = 資料路徑
    // objNamePtr = 回傳物件名
    // callbackFunPtr = 回傳方法名
    JS_ReadDataFromFirebase: function(refPathPtr, objNamePtr, callbackFunPtr) {
        const refPath = UTF8ToString(refPathPtr);
        const gameObjectName = UTF8ToString(objNamePtr);
        const callbackFunctionName = UTF8ToString(callbackFunPtr);

        firebase.database().ref(refPath).once('value').then(function(snapshot) {
            const data = snapshot.val();
            const jsonData = JSON.stringify(data);
            window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, jsonData);
        }).catch(function(error) {
            console.error("The read failed... : " + error);
            window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, JSON.stringify({ error: error.message }));
        });
    },

    // 移除資料
    // refPathPtr = 資料路徑
    // objNamePtr = 回傳物件名
    // callbackFunPtr = 回傳方法名
    JS_RemoveDataFromFirebase: function(refPathPtr, objNamePtr, callbackFunPtr) {
        const refPath = UTF8ToString(refPathPtr);
        const gameObjectName = UTF8ToString(objNamePtr);
        const callbackFunctionName = UTF8ToString(callbackFunPtr);

        firebase.database().ref(refPath).remove().then(function() {
            window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, JSON.stringify({ success: true }));
        }).catch(function(error) {
            console.error("The delete failed... : " + error);
            window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, JSON.stringify({ error: error.message }));
        });
    },

    // 檢查用戶資料是否已存在
    // keyPtr = 查詢關鍵字
    // valuePtr = 查詢的值
    // releaseTypePtr = 發布環境
    // objNamePtr = 回傳物件名
    // callbackFunPtr = 回傳方法名
    JS_CheckUserDataExist: async function(keyPtr, valuePtr, releaseTypePtr, objNamePtr, callbackFunPtr) {
        const key = UTF8ToString(keyPtr);
        const valueToSearch = UTF8ToString(valuePtr);
        const releaseType = UTF8ToString(releaseTypePtr);
        const gameObjectName = UTF8ToString(objNamePtr);
        const callbackFunctionName = UTF8ToString(callbackFunPtr);

        let foundPhoneNumber = "";

        try {
            // Fetch data from the 'releaseType/user' node
            const userRef = firebase.database().ref(releaseType + '/user');
            const snapshot = await userRef.get();

            if (snapshot.exists()) {
                const userData = snapshot.val();
                console.log("Fetched userData:", userData);  // 調試輸出

                // Check phoneUser
                if (userData.phoneUser) {
                    for (const phoneNumberKey in userData.phoneUser) {
                        const phoneNumberData = userData.phoneUser[phoneNumberKey];
                        if (phoneNumberData[key] && phoneNumberData[key] === valueToSearch) {
                            foundPhoneNumber = phoneNumberData.phoneNumber;
                            console.log("Found phoneNumber in phoneUser:", foundPhoneNumber);  // 調試輸出
                            break;
                        }
                    }
                }

                // Check walletUser
                if (userData.walletUser) {
                    for (const walletUserKey in userData.walletUser) {
                        const walletUserData = userData.walletUser[walletUserKey];
                        for (const phoneNumberKey in walletUserData) {
                            const phoneNumberData = walletUserData[phoneNumberKey];
                            if (phoneNumberData[key] && phoneNumberData[key] === valueToSearch) {
                                foundPhoneNumber = phoneNumberData.phoneNumber;
                                console.log("Found phoneNumber in walletUser:", foundPhoneNumber);  // 調試輸出
                                break;
                            }
                        }
                    }
                }
            } else {
                console.log('No data available');
            }

            // Output result
            if (foundPhoneNumber) {
                console.log(`Found phoneNumber for ${key}:`, foundPhoneNumber);
                window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, JSON.stringify({exists: "true", phoneNumber: foundPhoneNumber}));
            } else {
                console.log(`No phoneNumber found for ${key}`);
                window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, JSON.stringify({exists: "false", phoneNumber: ""}));
            }

        } catch (error) {
            console.error('Error fetching data:', error);
            window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, JSON.stringify({exists: "error", phoneNumber: "", error: error.toString()}));
        }
    },
});
