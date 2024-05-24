mergeInto(LibraryManager.library, {
    
    //重新整理頁面
    JS_Reload: function() {
        window.location.reload();
    },

    //斷開連接
    JS_WindowDisconnect: function() {
        if (window.ethereum && window.ethereum.disconnect) {
            window.ethereum.disconnect();
            console.log('錢包斷開連接');
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

    // 電腦網頁連接錢包
    JS_ConnectWalletFromWindow: function (walletIndex) {        

        //未安裝
        function OnNotInstall(loadUrl){
            window.unityInstance.SendMessage('LoginView', 'WindowConnectFail');
            window.open(loadUrl); 
        }

        //錢包連接
        async function OnWalletConnect(){
            try{
                await window.ethereum.request({ method: 'eth_requestAccounts' });

                // 請求獲取錢包地址和 ETH 餘額
                try {
                    const accounts = await web3.eth.getAccounts();
                    const address = accounts[0];
                    const balance = await web3.eth.getBalance(address);
                    const ethBalance = web3.utils.fromWei(balance, 'ether');
                    
                    console.log('連接錢包地址:', address);
                    console.log('錢包 ETH 餘額:', ethBalance);
                    window.unityInstance.SendMessage('LoginView', 'SetAddress', address);
                    window.unityInstance.SendMessage('LoginView', 'SetEthBlance', ethBalance);

                    //回傳連接成功
                    window.unityInstance.SendMessage('LoginView', 'WindowConnectSuccess'); 

                } catch (e) {

                    console.error('無法獲取錢包地址和 ETH 餘額: ' + e.message);
                    window.unityInstance.SendMessage('LoginView', 'WindowConnectFail');
                }
            }catch(e) {
                if (e.code === 4001) {
                        console.log("用戶不同意授權!");
                }
                else {
                    console.error('連接錢包發生錯誤:', e);
                } 
                
                window.unityInstance.SendMessage('LoginView', 'WindowConnectFail');
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
                //myUnityInstance.SendMessage('WalletManager', 'WindowConnectSuccess', signature);                
            } catch (e) {
                console.error('簽名失敗:' + e);
                //myUnityInstance.SendMessage('WalletManager', 'WindowConnectFail');
            }
        }

        //主邏輯
        window.web3 = new Web3(window.ethereum);
        if (window.web3 && window.web3.disconnect) {
            window.web3.disconnect();
        }
        console.log('連線至index:' + walletIndex);
        
        switch(walletIndex){
            //MetaMask
            case 0:
                if (window.web3) {
                        OnWalletConnect();
                } else {
                    // 未安裝 MetaMask
                    OnNotInstall("https://metamask.io/download/")
                }
                break;

            //Trust
            case 1:
                if (window.web3) {
                        OnWalletConnect();
                } else {
                    // 未安裝 Trust
                    OnNotInstall("https://trustwallet.com/");                    
                }
                break;

            //OKX
            case 2:
                if (typeof window.okxwallet !== 'undefined') {
                        OnWalletConnect();
                } else {
                    // 未安裝 OKX
                    console.log('未安裝 OKX');
                    OnNotInstall("https://chromewebstore.google.com/detail/okx-web3-%E9%8C%A2%E5%8C%85/mcohilncbfahbmgdjkbpemcciiolgcge?pli=1");
                }
                break;    
        }
    },
});