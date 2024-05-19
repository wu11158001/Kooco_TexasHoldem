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
    Disconnect: function() {
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
});
