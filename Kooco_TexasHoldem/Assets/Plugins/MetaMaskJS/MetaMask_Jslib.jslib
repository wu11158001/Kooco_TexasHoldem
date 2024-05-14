mergeInto(LibraryManager.library, {

    //重新整理頁面
    Reload: function() {
        window.location.reload();
    },

    //檢測平台
    IsMobilePlatform: function() {
        return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    },

    //斷開連接
    DisconnectFromMetaMask: function() {
        if (window.ethereum && window.ethereum.disconnect) {
            window.ethereum.disconnect();
        }
    },

    //撤銷權限
    RevokePermissions: function() {
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

    //電腦網頁連接MetaMask
    WindowsConnectAndSignMetaMask: function() {
        // 連接 MetaMask
        async function ConncetMetamask() {            
            if (window.ethereum) {
                //最新版本的以太坊 dApp 瀏覽器
                window.web3 = new Web3(window.ethereum);
                try {
                    await window.ethereum.request({ method: 'eth_requestAccounts' });
                    console.log('連接最新版本的以太坊 dApp 瀏覽器');
                } catch (error) {
                    console.error('用戶取消連接MetaMask');
                    myUnityInstance.SendMessage('MetaMaskManager', 'WindowConnectFail');
                    return;
                }
            } else if (window.web3) {
                //覽器環境支援舊版的 Web3.js
                window.web3 = new Web3(web3.currentProvider);
                console.log('連接支援舊版的 Web3.js瀏覽器');
            } else {
                //未安裝MetaMask
                console.log('未安裝MetaMask');
                window.open("https://metamask.io/download/");
                return;
            }

            //錢包地址
            var accounts = await web3.eth.getAccounts();
            var address = accounts[0];
            console.log('錢包地址:' + address);            
            //ETH餘額
            var balance = await web3.eth.getBalance(address);
            var ethBalance = web3.utils.fromWei(balance, 'ether');
            console.log('ETH餘額:' + ethBalance);
            
            myUnityInstance.SendMessage('MetaMaskManager', 'SetAddress', address);
            myUnityInstance.SendMessage('MetaMaskManager', 'SetEthBlance', ethBalance);                     
        }

        // 請求 MetaMask 簽名
        async function SignMessage() {
            try {
                // 獲取用戶的帳戶信息
                const accounts = await window.ethereum.request({ method: 'eth_accounts' });
                const from = accounts[0];

                // 要簽名的消息
                const message = 'Web Sign Test Info';

                // 使用 MetaMask 簽名消息
                const signature = await window.ethereum.request({
                    method: 'personal_sign',
                    params: [message, from]
                });

                //簽名結果
                console.log('簽名結果:', signature);
                myUnityInstance.SendMessage('MetaMaskManager', 'WindowConnectSuccess', signature);                
            } catch (error) {
                console.error('User Sign disagrees!!!' + error);
                myUnityInstance.SendMessage('MetaMaskManager', 'WindowConnectFail');
            }
        }

        //主邏輯
        async function main() {
            await ConncetMetamask();
            await SignMessage();  
        }

        main();
    },
});