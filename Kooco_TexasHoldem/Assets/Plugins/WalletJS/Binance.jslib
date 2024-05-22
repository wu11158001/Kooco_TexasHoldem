mergeInto(LibraryManager.library, {

    BinanceConnectAndSign: function(){
        const clientId = 'Yuq7HuqpOf8cUCnnEckfOkSNCRAcZKZVM7ula4OEZBZgiJdW2PObFZM5s84qLs1K';
        const redirectUri = encodeURIComponent('https://wu11158001.github.io/kooco_Holdem_Self/kooco_Holdem_Demo/index.html');
        const state = '377f36a4557ab5935b36&'; // 建議生成隨機值以防止CSRF攻擊
        const scope = encodeURIComponent('user:email,user:address');

        const authUrl = `https://accounts.binance.com/en/oauth/authorize?response_type=code&client_id=${clientId}&redirect_uri=${redirectUri}&state=${state}&scope=${scope}`;

        window.location.href = authUrl;
    },
});
