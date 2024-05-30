mergeInto(LibraryManager.library, {

    //本地頁面跳轉
    JS_LocationHref: function(url){
        window.location.href = UTF8ToString(url);
    },

    //關閉頁面
    JS_WindowClose: function(){
        window.open("","_self").close();
    },

    //重新整理頁面
    JS_Reload: function() {
        window.location.reload();
    },

    //斷開連接
    JS_WindowDisconnect: function() {
        if (typeof window.ethereum !== 'undefined') {
            if (window.ethereum.isConnected()) {
                window.ethereum
                    .request({ method: 'eth_requestAccounts' })
                    .then(() => {
                        window.ethereum.disconnect();
                        console.log("錢包连接已断开");
                    })
                    .catch((e) => {
                        console.error(e);
                    });
            } 
        }
    },

    //撤銷權限
    JS_RevokePermissions: function() {
        async function Revoke() {
            try {
                await window.ethereum.request({
                    method: 'wallet_revokePermissions',
                    params: [{ eth_accounts: {}, },],
                });
                console.log('權限已移除');
            } catch (error) {
                console.error('權限移除錯誤:' + error);
            }
        }

        Revoke();        
    },

    //獲取瀏覽器訊息
    JS_GetBrowserInfo: function(){
        window.unityInstance.SendMessage('Entry', 'HtmlDebug', 'GetBrowserInfo');
        const userAgent = navigator.userAgent;
        const appName = navigator.appName;
        const appVersion = navigator.appVersion;
        const platform = navigator.platform;
        const language = navigator.language;

        window.unityInstance.SendMessage('Entry', 'HtmlDebug', 'userAgent:'+userAgent);
        window.unityInstance.SendMessage('Entry', 'HtmlDebug', 'appName:'+appName);
        window.unityInstance.SendMessage('Entry', 'HtmlDebug', 'appVersion:'+appVersion);
        window.unityInstance.SendMessage('Entry', 'HtmlDebug', 'platform:'+platform);
        window.unityInstance.SendMessage('Entry', 'HtmlDebug', 'language:'+language);
        
        //const userAgent = navigator.userAgent;
        let browserName, fullVersion;

        if (userAgent.indexOf("Chrome") > -1) {
            browserName = "Chrome";
            fullVersion = userAgent.substring(userAgent.indexOf("Chrome") + 7);
            fullVersion = fullVersion.substring(0, fullVersion.indexOf(" "));
        } else if (userAgent.indexOf("Safari") > -1) {
            browserName = "Safari";
            fullVersion = userAgent.substring(userAgent.indexOf("Version") + 8);
            fullVersion = fullVersion.substring(0, fullVersion.indexOf(" "));
        } else if (userAgent.indexOf("Firefox") > -1) {
            browserName = "Firefox";
            fullVersion = userAgent.substring(userAgent.indexOf("Firefox") + 8);
        } else if (userAgent.indexOf("MSIE") > -1 || !!document.documentMode) {
            browserName = "IE";
            fullVersion = userAgent.substring(userAgent.indexOf("MSIE") + 5);
            fullVersion = fullVersion.substring(0, fullVersion.indexOf(" "));
        } else {
            browserName = "Unknown";
            fullVersion = "Unknown";
        }      
    },

    //離開預設瀏覽器開啟chrome瀏覽器
    JS_OpenNewBrowser: function(mailStr){
        const mail = UTF8ToString(mailStr);
        const targetUrl = `${window.callbackUrl}?linemail=${encodeURIComponent(mail)}`;
        const intentUrl = `intent://${targetUrl.replace(/^https?:\/\//, '')}#Intent;scheme=http;package=com.android.chrome;end;`;
        window.location.href = intentUrl;
    },
});