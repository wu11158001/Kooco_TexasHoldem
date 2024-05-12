mergeInto(LibraryManager.library, {

    //檢測平台
    IsMobilePlatform: function(libraryManagerPtr) {
        return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    },

    //電腦網頁登入MetaMask(Unity呼叫)
    WindowsLoginMetaMask: function(libraryManagerPtr) {

        // 連接 MetaMask
        async function ConncetMetamask() {
            
            if (window.ethereum) {
                //最新版本的以太坊 dApp 瀏覽器
                window.web3 = new Web3(window.ethereum);
                try {
                    await window.ethereum.request({ method: 'eth_requestAccounts' });
                    console.log('連接最新版本的以太坊 dApp 瀏覽器');
                } catch (error) {
                    console.error('連接最新版本的以太坊 dApp 瀏覽器錯誤:' + error);
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
            
            myUnityInstance.SendMessage('MetaMaskConnect', 'GetAddress', address);
            myUnityInstance.SendMessage('MetaMaskConnect', 'GetEthBlance', ethBalance);
            myUnityInstance.SendMessage('MetaMaskConnect', 'WindowConnectSuccess');
        }

        //主邏輯
        async function main() {
            await ConncetMetamask();
        }

        main();
    },
});