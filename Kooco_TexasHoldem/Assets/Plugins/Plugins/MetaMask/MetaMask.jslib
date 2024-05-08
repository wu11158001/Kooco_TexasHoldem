mergeInto(LibraryManager.library, {
    // 登入 MetaMask
    LoginMetaMask: function(libraryManagerPtr) {

        var serverIp = "https://localhost:44389";
        //var serverIp = "https://8058-219-91-90-83.ngrok-free.app";

        if (window.ethereum) {
            // MetaMask 已安装
            window.ethereum.request({ method: 'eth_requestAccounts' }).then((accounts) => {
                var address = accounts[0];
                console.log('钱包连接成功' + address);

                SendMessage('LoginView', 'LoginSuccess', address);

                // 开始获取 nonce 并签名
                //getNonceAndSign(address); 
            }).catch((error) => {
                console.error("钱包连接失败:", error);
            });
        } else {
            window.open("https://metamask.io/download/");
        }

        async function getNonceAndSign(address) {
            try {
                // 从服务器获取 nonce
                const nonce = await fetchNonceFromServer(address);
                console.log("從服務器獲取 nonce 成功 :" + nonce);
                // 对 nonce 进行签名
                signNonce(nonce, address); 
            } catch (error) {
                console.error("无法获取 nonce：", error);
            }
        }

        async function signNonce(nonce, address) {
            try {
                // 使用 ethers.js 进行签名
                const provider = new ethers.providers.Web3Provider(window.ethereum);
                const signer = provider.getSigner();
                const signature = await signer.signMessage(nonce);

                // 签名成功
                console.log("签名结果：", signature);
                // 将签名结果传递给后端进行验证
                verifySignature(nonce, signature, address); 
            } catch (error) {
                // 签名失败
                console.error("签名失败：", error);
            }
        }

        function verifySignature(nonce, signature, address) {
            // 将 nonce 和 signature 传递给后端进行验证
            console.log("準備驗證簽名...");
            
            fetch(serverIp + "/MetaMask/VerifySignature", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ address: address, signature: signature }),
            }).then(response => response.json())
              .then(data => {
                  if (data.success) {
                      console.log("签名验证成功");
                  } else {
                      console.error("签名验证失败:", data.error);
                      // 在这里处理签名验证失败的情况
                  }
              })
              .catch(error => {
                  console.error("签名验证请求失败:", error);
                  // 在这里处理请求失败的情况
              });
        }

        function fetchNonceFromServer(address) {
            return fetch(serverIp + "/MetaMask/GetNonce?address=" + address)
                .then(response => response.json())
                .then(data => data.nonce)
                .catch(error => Promise.reject(error));
        }
    },

});
