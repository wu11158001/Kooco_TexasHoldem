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

    // 檢查用戶資料是否已存在(邀請碼/UserID)
    // objNamePtr = 回傳物件名
    // callbackFunPtr = 回傳方法名
    JS_CheckUserDataExist: async function(keyPtr, objNamePtr, callbackFunPtr) {
        const key = UTF8ToString(keyPtr);
        const gameObjectName = UTF8ToString(objNamePtr);
        const callbackFunctionName = UTF8ToString(callbackFunPtr);

        const values = new Set();  // 使用 Set 來追蹤唯一值
        let hasDuplicates = false;

        try {
            // Fetch data from the 'user' node
            const userRef = firebase.database().ref('user');
            const snapshot = await userRef.get();

            if (snapshot.exists()) {
                const userData = snapshot.val();

                // Check walletUser
                if (userData.walletUser) {
                    for (const walletUserKey in userData.walletUser) {
                        const walletUserData = userData.walletUser[walletUserKey];
                        for (const phoneNumberKey in walletUserData) {
                            const phoneNumberData = walletUserData[phoneNumberKey];
                            for (const itemKey in phoneNumberData) {
                                const item = phoneNumberData[itemKey];
                                if (item[key]) {
                                    const value = item[key];
                                    if (values.has(value)) {
                                        hasDuplicates = true;
                                    }
                                    values.add(value);
                                }
                            }
                        }
                    }
                }

                // Check phoneUser
                if (userData.phoneUser) {
                    for (const phoneUserKey in userData.phoneUser) {
                        const phoneUserData = userData.phoneUser[phoneUserKey];
                        for (const phoneNumberKey in phoneUserData) {
                            const phoneNumberData = phoneUserData[phoneNumberKey];
                            for (const itemKey in phoneNumberData) {
                                const item = phoneNumberData[itemKey];
                                if (item[key]) {
                                    const value = item[key];
                                    if (values.has(value)) {
                                        hasDuplicates = true;
                                    }
                                    values.add(value);
                                }
                            }
                        }
                    }
                }
            } else {
                console.log('No data available');
            }

            // Output result
            if (hasDuplicates) {
                // 資料已存在
                console.log(`There are duplicate ${key}s.`);
                window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, "true");

            } else {
                // 資料不存在
                console.log(`No duplicate ${key}s found.`);
                window.unityInstance.SendMessage(gameObjectName, callbackFunctionName, "false");
            }

        } catch (error) {
            console.error('Error fetching data:', error);
        }
    },

    // 更新邀請碼對象的綁定邀請者
    // invitationCode = 查找的邀請碼
    // boundInviterId = 被邀請的用戶ID
    JS_UpdateBoundInviterId: function(invitationCode, boundInviterId) {
        updateBoundInviterId('walletUser', UTF8ToString(invitationCode), UTF8ToString(boundInviterId));
        updateBoundInviterId('phoneUser', UTF8ToString(invitationCode), UTF8ToString(boundInviterId));

        // 查找並更新
        function updateBoundInviterId(loginType, invitationCode, newboundInviterId) {
            const ref = firebase.database().ref(`user/${loginType}`);
            ref.orderByChild('invitationCode').equalTo(invitationCode).once('value')
                .then(snapshot => {
                    snapshot.forEach(childSnapshot => {
                        const key = childSnapshot.key;
                        const childData = childSnapshot.val();

                        if (!childData.boundInviterId) {
                            // 如果boundInviterId不存在或為空，則更新它
                            ref.child(key).update({ boundInviterId: newboundInviterId })
                                .then(() => {
                                    console.log(`Updated inviterId for ${key}`);
                                })
                                .catch(error => {
                                    console.error(`Failed to update inviterId for ${key}:`, error);
                                });
                        }
                    });
                })
                .catch(error => {
                    console.error('Error fetching data:', error);
                });
        }
    },
});
