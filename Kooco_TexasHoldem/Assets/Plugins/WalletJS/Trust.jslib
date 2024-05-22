mergeInto(LibraryManager.library, {

    // 電腦網頁連接TrustWallet
    TrustConnectAndSignFormWindow: function () {
        // 連接 TrustWallet
        async function OnConnect() {
            console.log('準備連接TrustWallet');

            try {
                if (window.ethereum && window.ethereum.isTrust) {
                        window.web3 = new Web3(window.ethereum);
                        await window.ethereum.request({ method: 'eth_requestAccounts' });
                        console.log('Trust 連接以太坊瀏覽器');
                } else {
                    // 未安裝 Trust
                    console.log('未安裝 Trust');
                    myUnityInstance.SendMessage('WalletManager', 'WindowConnectFail');
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
                console.error('連接TrustWallet時發生錯誤:', error);
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
    

    // 移動平台連接TrustWallet
    TrustConnectAndSignFormMobile: function() {   

        async function OnConnect(){
            //連接
            const ethersProvider = new ethers.providers.Web3Provider(window.ethereum);
            await provider.send("eth_requestAccounts", []);
            const signer = provider.getSigner();
            
            //簽名
            signature = await signer.signMessage("Sign Message");
            //簽名結果
            console.log('簽名結果:', signature);
            myUnityInstance.SendMessage('WalletManager', 'WindowConnectSuccess', signature);       

            const balance = await provider.getBalance("ethers.eth");
            const ethBalance = ethers.utils.formatEther(balance);
        }         

        // 主邏輯
        async function main() {
            const redirectUri = '';
            const deepLink = 'https://link.trustwallet.com/open_url?coin_id=60&url=https://wu11158001.github.io/kooco_Holdem_Self/kooco_Holdem_Demo/index.html';
            window.location.href = deepLink;
            await OnConnect();
        }

        main();
    },
});