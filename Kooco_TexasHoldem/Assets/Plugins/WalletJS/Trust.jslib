mergeInto(LibraryManager.library, {

    // 電腦網頁連接TrustWallet
    TrustConnectAndSignFormWindows: function () {
        // 連接 TrustWallet
        async function TrustWalletConnect() {
            console.log('準備連接TrustWallet');

            try {
                if (window.ethereum && window.ethereum.isTrust) {
                        window.web3 = new Web3(window.ethereum);
                        await window.ethereum.request({ method: 'eth_requestAccounts' });
                        console.log('Trust 連接以太坊 dApp 瀏覽器');
                } else {
                    // 未安裝 Trust
                    console.log('未安裝 Trust');
                    window.open("https://trustwallet.com/");
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
                    await SignMessage();
                    
                } catch (error) {
                    throw new Error('無法獲取錢包地址和 ETH 餘額: ' + error.message);
                }
            } catch (error) {
                console.error('連接TrustWallet時發生錯誤:', error);
                myUnityInstance.SendMessage('WalletManager', 'WindowConnectFail');
            }
        }

        // 請求簽名
        async function SignMessage() {
            try {
                // 獲取用戶的帳戶信息
                const accounts = await window.ethereum.request({ method: 'eth_accounts' });
                const from = accounts[0];

                // 要簽名的消息
                const message = 'Trust Web Sign Test Info';

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
            await TrustWalletConnect();
        }

        main();
    },
    

    // 移動平台連接TrustWallet
    TrustConnectAndSignFormMobile: function() {
        window.location.href = "trust://";
    },
});