mergeInto(LibraryManager.library, {
    
    // 電腦網頁連接OKX
    OKXConnectAndSignFormWindow: function () {
        // 連接 OKX
        async function OnConnect() {
            console.log('準備連接OKX');

            try {
                if (typeof window.okxwallet !== 'undefined') {
                        window.web3 = new Web3(window.ethereum);
                        await window.ethereum.request({ method: 'eth_requestAccounts' });
                        console.log('OKX 連接以太坊瀏覽器');
                } else {
                    // 未安裝 OKX
                    console.log('未安裝 OKX');
                    myUnityInstance.SendMessage('WalletManager', 'WindowConnectFail');
                    window.open("https://chromewebstore.google.com/detail/okx-web3-%E9%8C%A2%E5%8C%85/mcohilncbfahbmgdjkbpemcciiolgcge?pli=1");
                    return;
                }

                // 請求獲取錢包地址和 ETH 餘額
                try {
                    const accounts = await web3.eth.getAccounts();
                    if (accounts.length === 0) {
                        throw new Error('未找到錢包地址');
                    }
                    const address = accounts[0];
                    const balance = await web3.eth.getBalance(address);
                    const ethBalance = web3.utils.fromWei(balance, 'ether');
                    
                    console.log('錢包地址:', address);
                    console.log('ETH 餘額:', ethBalance);
                    myUnityInstance.SendMessage('WalletManager', 'SetAddress', address);
                    myUnityInstance.SendMessage('WalletManager', 'SetEthBlance', ethBalance);

                    //請求簽名
                    await OnSignature();
                    
                } catch (error) {
                    if (e.code === 4001) {
                        console.error("用戶不同意授權!");
                    }
                    else {
                        console.error('無法獲取錢包地址和 ETH 餘額: ' + error.message);
                    }                    
                }
            } catch (error) {
                console.error('連接OKX時發生錯誤:', error);
                myUnityInstance.SendMessage('WalletManager', 'WindowConnectFail');
            }
        }

        // 請求簽名
        async function OnSignature() {
            try {
                // 獲取用戶的帳戶信息
                const accounts = await window.ethereum.request({ method: 'eth_accounts' });
                const from = accounts[0];

                // 要簽名的消息
                const message = "Sign Message";

                // 使用簽名消息
                const signature = await window.ethereum.request({
                    method: 'personal_sign',
                    params: [message, from]
                });

                //簽名結果
                console.log('簽名結果:', signature);
                myUnityInstance.SendMessage('WalletManager', 'WindowConnectSuccess', signature);                
            } catch (error) {
                console.error('簽名失敗:' + error);
                myUnityInstance.SendMessage('WalletManager', 'WindowConnectFail');
            }
        }

        // 主邏輯
        async function main() {
            await OnConnect();
        }

        main();
    },

    // 移動平台連接OKX
    OKXConnectAndSignFormMobile: function() {   

        async function OnConnect(){
            if (typeof window.okxwallet !== 'undefined') {
                const dappUrl = "https://wu11158001.github.io/kooco_Self/kooco_Holdem_Demo/index.html";
                const encodedDappUrl = encodeURIComponent(dappUrl);
                const deepLink = "okx://wallet/dapp/url?dappUrl=" + encodedDappUrl;
                const encodedUrl = "https://www.okx.com/download?deeplink=" + encodeURIComponent(deepLink);
                //重定向到 OKX 钱包的深度链接
                window.location.href = encodedUrl;
            }
            else {
                // 未安裝 OKX
                window.open("https://www.okx.com/download");
                return;
            }        
        }
        
        OnConnect();
    },
});