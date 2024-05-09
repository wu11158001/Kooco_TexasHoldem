mergeInto(LibraryManager.library, {

        TestFunc: async function(libraryManagerPtr) {
            try {
                // 检查 ethereum 对象是否已定义
                if (typeof ethereum !== 'undefined') {
                    // 创建 Web3 实例并使用 ethereum 对象
                    window.web3 = new Web3(ethereum);
                    // 请求用户授权
                    const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
                    var address = accounts[0];
                    console.log('钱包连接成功' + address);
                    SendMessage('Entry', 'HtmlDebug', "Phone Wallet is LoginSuccess");
                    SendMessage('LoginView', 'LoginSuccess', address);
                } else {
                    throw new Error('ethereum 对象不存在');
                }
            } catch (error) {
                console.error("钱包连接失败:", error);
                SendMessage('Entry', 'HtmlDebug', "Error!!!!:" + error);
            }
        },


    //登入MetaMask(Unity呼叫)
    LoginMetaMask: async function(libraryManagerPtr) {

        // 是否在移動平台
        function isMobileDevice() {
            return (typeof window.orientation !== "undefined") || (navigator.userAgent.indexOf('IEMobile') !== -1);
        }

        // 判斷移動平台
        function getMobileOperatingSystem() {
            var userAgent = navigator.userAgent || navigator.vendor || window.opera;
            if (/android/i.test(userAgent)) {
                return "Android";
            }
            if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
                return "iOS";
            }
            return "unknown";
        }

        // 检测应用程序是否安装（Android）
        async function isAppInstalled(platform) {
            if (platform === 'Android') {
                var intentURI = "metamask://";
                window.location.href = intentURI;
                setTimeout(function() {
                    if (window.location.href === "about:blank") {
                        SendMessage('Entry', 'HtmlDebug', "Android is Not AppInstalled");
                    } else {
                        SendMessage('Entry', 'HtmlDebug', "Android is AppInstalled");                            
                    }
                }, 1000); // 假设超时时间为 1 秒
            }
        }

        // 連接 MetaMask
        function connectToMetaMask() {
            if (window.ethereum) {
                window.ethereum.request({ method: 'eth_requestAccounts' }).then(accounts => {
                    var address = accounts[0];
                    console.log('钱包连接成功' + address);
                    SendMessage('Entry', 'HtmlDebug', "Wallet is LoginSuccess");
                    SendMessage('LoginView', 'LoginSuccess', address);
                }).catch(error => {
                    console.error("钱包连接失败:", error);
                    SendMessage('Entry', 'HtmlDebug', "Wallet is Not LoginSuccess");
                });
            } else {
                window.open("https://metamask.io/download/");
            }
        }

        //移動端連接
        async function phoneConnectToMetaMask() {
            try {
                // 检查 ethereum 对象是否已定义
                if (typeof ethereum !== 'undefined') {
                    // 创建 Web3 实例并使用 ethereum 对象
                    window.web3 = new Web3(ethereum);
                    // 请求用户授权
                    const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
                    var address = accounts[0];
                    console.log('钱包连接成功' + address);
                    SendMessage('Entry', 'HtmlDebug', "Phone Wallet is LoginSuccess");
                    SendMessage('LoginView', 'LoginSuccess', address);
                } else {
                    SendMessage('Entry', 'HtmlDebug', "ethereum not!!!!");
                }
            } catch (error) {
                console.error("钱包连接失败:", error);
                SendMessage('Entry', 'HtmlDebug', "Error!!!!:" + error);
            }
        }

        //主邏輯
        async function main() {
            if (isMobileDevice()) {
                var platform = getMobileOperatingSystem();
                if (platform === 'Android') {
                    try {
                        await isAppInstalled(platform);
                        setTimeout(async function() {
                            SendMessage('Entry', 'HtmlDebug', "pre phone connect!!!");
                            await ethereum.enable();
                        }, 30000); // 30 秒後執行
                    } catch (error) {
                        // MetaMask未安装或超时
                        SendMessage('Entry', 'HtmlDebug', "metamasko Time out" + error);
                    }
                } else if (platform === 'iOS') {
                    // Add iOS specific logic here
                }
            } else {
                connectToMetaMask();
            }
        }

        await main();
    },
});