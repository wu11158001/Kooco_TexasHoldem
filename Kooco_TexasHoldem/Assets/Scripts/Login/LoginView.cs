using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thirdweb.Redcode.Awaiting;
using System.Numerics;
using Thirdweb;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class LoginView : MonoBehaviour, IPointerClickHandler
{
    [Header("切換/版本")]
    [SerializeField]
    Text Vrsion_Txt;
    [SerializeField]
    Toggle Wallet_Tog, Mobile_Tog;
    [SerializeField]
    GameObject Wallet_Obj, Mobile_Obj;

    [Header("錢包連接頁面")]
    [SerializeField]
    GameObject SelectWalletPage_Obj;
    [SerializeField]
    Button Metamask_Btn, Trust_Btn, Binance_Btn, OKX_Btn, Coinbase_Btn;
    [SerializeField]
    TMP_Text SignUp_TmpTxt;

    [Header("錢包連接_連接中頁面")]
    [SerializeField]
    GameObject WalletLoadingPage_Obj, ConnectingWallet_Obj, Connecting_Obj, RetryConnectWallet_Obj;
    [SerializeField]
    Button BackToSelectWallet_Btn, RetryConnectWallet_Btn;
    [SerializeField]
    Text ConnectionTitle_Txt, Connecting_Txt, ErrorConnect_Txt;
    [SerializeField]
    Image ConnectingLogo_Img;
    [SerializeField]
    List<Image> EffectPointList;

    [Header("錢包連接_簡訊認證頁面")]
    [SerializeField]
    GameObject SMSVerificationPage_Obj;
    [SerializeField]
    Button SMSOTPSend_Btn, SMSOTPSubmit_Btn;
    [SerializeField]
    InputField SMSMobileNumber_If, SMSOTP_If;
    [SerializeField]
    Dropdown SMSMobileNumber_Dd;
    [SerializeField]
    Text SMSMobileNumberError_Txt, SMSCodeError_Txt;

    [Header("手機登入")]
    [SerializeField]
    GameObject MobileSignIn_Obj, MobileSiginPage_Obj;
    [SerializeField]
    Button SignIn_Btn, Register_Btn, SignInPasswordEye_Btn;
    [SerializeField]
    Dropdown SignInNumber_Dd;
    [SerializeField]
    InputField SignInNumber_If, SignInPassword_If;
    [SerializeField]
    Text MobileTitle_Txt, MobileTip_Txt, MobileSignInError_Txt, SignInNumberError_Txt;
    [SerializeField]
    TMP_Text ForgotPassword_TmpTxt;
    [SerializeField]
    Toggle RememberMe_Tog;

    [Header("手機註冊")]
    [SerializeField]
    GameObject RegisterPage_Obj;
    [SerializeField]
    TMP_Text Privacy_TmpTxt;
    [SerializeField]
    Text RegisterNumberError_Txt, RegisterCodeError_Txt, RegisterPasswordError_Txt, RegisterPrivacyError_Txt;
    [SerializeField]
    Button RegisterOTPSend_Btn, RegisterPasswordEye_Btn, RegisterSubmit_Btn, RegisterSuccSignin_Btn, RegisterSuccessfulCancel_Btn;
    [SerializeField]
    InputField RegisterNumber_If, RegisterOTP_If, RegisterPassword_If;
    [SerializeField]
    Dropdown RegisterNumber_Dd;
    [SerializeField]
    Toggle Privacy_Tog;

    [Header("手機注冊密碼檢查")]
    [SerializeField]
    GameObject RegisterCheckPassword_Obj;
    [SerializeField]
    Image RegisterCheckPassword1_Img, RegisterCheckPassword2_Img, RegisterCheckPassword3_Img;
    [SerializeField]
    Text RegisterCheckPassword1_Txt, RegisterCheckPassword2_Txt, RegisterCheckPassword3_Txt;

    [Header("註冊成功")]
    [SerializeField]
    GameObject RegisterSucce_Obj;
    [SerializeField]
    Text RegisterSuccTip_Txt;

    [Header("忘記密碼")]
    [SerializeField]
    GameObject LostPassword_Obj;
    [SerializeField]
    InputField LostPswNumber_If, LostPswOTP_If, LosrPswPassword_If;
    [SerializeField]
    Button BackToMobileSignIn_Btn, LostPswPasswordEye_Btn, LostPswOTPSend_Btn, LostPswSubmit_Btn;
    [SerializeField]
    Text LostPswNumberError_Txt, LostPswCodeError_Txt, LostPswPasswordError_Txt;
    [SerializeField]
    Dropdown LostPswNumber_Dd;

    [Header("忘記密碼密碼檢查")]
    [SerializeField]
    GameObject LostPswCheckPassword_Obj;
    [SerializeField]
    Image LostPswCheckPassword1_Img, LostPswCheckPassword2_Img, LostPswCheckPassword3_Img;
    [SerializeField]
    Text LostPswCheckPassword1_Txt, LostPswCheckPassword2_Txt, LostPswCheckPassword3_Txt;
    [SerializeField]

    const string LocalPhoneNumber = "AsiaPoker_PhoneNumber";        //本地紀錄_手機號
    const string LocalPaswword = "AsiaPoker_Password";              //本地紀錄_密碼
    const int ErrorWalletConnectTime = 30;                          //判定連接失敗等待時間

    ChainData _currentChainData;                                    //當前連接練
    string _address;                                                //錢包地址

    Coroutine connectionEffectCoroutine;                            //連接錢包效果
    DateTime startConnectTime;                                      //開始連接錢包時間
    bool isShowPassword;                                            //是否顯示密碼
    bool isRegisterPasswordCorrect;                                 //是否手機注冊密碼正確
    bool isLostPswPasswordCorrect;                                  //是否忘記密碼密碼正確
    string recodePhoneNumber;                                       //紀錄的手機號
    string recodePassword;                                          //紀錄的密碼

    List<InputField> currIfList = new List<InputField>();           //當前可切換InputFild
    UnityAction KybordEnterAction;                                  //Enter鍵執行方法

    /// <summary>
    /// 紀錄當前連接錢包資料
    /// </summary>
    private RecordConnect recordConnect;
    public class RecordConnect
    {
        public string WalletProviderStr;
        public WalletEnum TheWalletEnum;
    }

    private void Awake()
    {
        recordConnect = new RecordConnect();
        ListenerEvent();
    }

    /// <summary>
    /// 事件聆聽
    /// </summary>
    private void ListenerEvent()
    {
        #region 頁面切換

        //錢包Toggle
        Wallet_Tog.onValueChanged.AddListener((isOn) =>
        {
            Wallet_Obj.SetActive(isOn);
            Mobile_Obj.SetActive(!isOn);

            isShowPassword = false;
            StringUtils.InitPasswordContent(SignInPasswordEye_Btn.image,
                                            SignInPassword_If);
            StringUtils.InitPasswordContent(RegisterPasswordEye_Btn.image,
                                            RegisterPassword_If);
            StringUtils.InitPasswordContent(LostPswPasswordEye_Btn.image,
                                            LosrPswPassword_If);

            if (isOn)
            {
                //錢包登入
                OnSwlwctWalletInit();
                OnWalletDisconnect();
            }
            else
            {
                //手機登入
                OnMobileSignInInit();
            }            
        });

        //返回選擇錢包
        BackToSelectWallet_Btn.onClick.AddListener(() =>
        {            
            StopCoroutine(connectionEffectCoroutine);
            OnSwlwctWalletInit();
            OnWalletDisconnect();
        });

        #endregion

        #region 錢包連接

        //MetaMask連接
        Metamask_Btn.onClick.AddListener(() =>
        {
            StartConnect("Metamask", WalletEnum.Metamask);
        });

        //Trust連接
        Trust_Btn.onClick.AddListener(() =>
        {
            StartConnect("Metamask", WalletEnum.TrustWallet);
        });

        //OKX連接
        OKX_Btn.onClick.AddListener(() =>
        {
            StartConnect("Metamask", WalletEnum.OKX);
        });

        //Binance連接
        Binance_Btn.onClick.AddListener(() =>
        {
            if (DataManager.IsMobilePlatform)
            {
                StartConnect("Metamask", WalletEnum.Binance);
            }
            else
            {
                StartConnect("WalletConnect", WalletEnum.Binance);
            }                

            InvokeRepeating(nameof(TryBinanceConnect), 8, 3);
        });

        //Coonbase連接
        Coinbase_Btn.onClick.AddListener(() =>
        {
            StartConnect("Coinbase", WalletEnum.Coinbase);
        });

        //重新嘗試連接
        RetryConnectWallet_Btn.onClick.AddListener(() =>
        {
            StartConnect(recordConnect.WalletProviderStr, recordConnect.TheWalletEnum);            
        });

        #endregion

        #region 簡訊認證

        //發送獲取驗證碼
        SMSOTPSend_Btn.onClick.AddListener(() =>
        {
            string phone = StringUtils.GetPhoneAddCode(SMSMobileNumber_Dd, SMSMobileNumber_If.text);
            Debug.Log($"Send Code:{phone}");

            SMSMobileNumberError_Txt.text = "";
            SMSCodeError_Txt.text = "";
            SMSOTP_If.text = "";
        });

        //簡訊OTP提交
        SMSOTPSubmit_Btn.onClick.AddListener(() =>
        {
            SMSOTPSubmitAction();
        });

        #endregion

        #region 手機登入

        //手機登入提交
        SignIn_Btn.onClick.AddListener(() =>
        {
            MobileSignInSubmit();
        });

        //手機登入密碼顯示
        SignInPasswordEye_Btn.onClick.AddListener(() =>
        {
            PasswordShowBtnClick(SignInPasswordEye_Btn.image,
                                 SignInPassword_If);
        });

        #endregion

        #region 手機注冊

        //手機註冊
        Register_Btn.onClick.AddListener(() =>
        {
            MobileTitle_Txt.text = "REGISTER";
            MobileTip_Txt.text = "You Will Receive A New Account.";

            RegisterNumberError_Txt.text = "";
            RegisterCodeError_Txt.text = "";
            RegisterPasswordError_Txt.text = "";
            RegisterPrivacyError_Txt.text = "";

            Privacy_TmpTxt.text = $"Agree To The <color=#79E84B><link=Terms><u>Terms</u></link></color> & <color=#79E84B><link=Privacy Policy><u>Privacy Policy</u></link></color>";
            MobileSiginPage_Obj.SetActive(false);
            RegisterPage_Obj.SetActive(true);
            RegisterCheckPassword_Obj.SetActive(false);

            //設定TAB切換與Enter提交方法
            RegisterNumber_If.Select();
            currIfList = new List<InputField>()
            {
                RegisterNumber_If,
                RegisterOTP_If,
                RegisterPassword_If,
            };
            KybordEnterAction = MobileRegisterSubmit;
        });

        //手機注冊發送獲取OTPCode
        RegisterOTPSend_Btn.onClick.AddListener(() =>
        {
            if (!StringUtils.CheckPhoneNumber(RegisterNumber_If.text))
            {
                RegisterNumberError_Txt.text = "User Name Entered Incorrectly, Please Try Again.";
            }
            else
            {
                RegisterNumberError_Txt.text = "";
                string phone = StringUtils.GetPhoneAddCode(RegisterNumber_Dd, RegisterNumber_If.text);
                Debug.Log($"Register Send Code:{phone}");
            }
        });

        //手機注冊密碼輸入
        RegisterPassword_If.onValueChanged.AddListener((value) =>
        {
            RegisterCheckPassword_Obj.SetActive(value.Length > 0);

            RegisterPasswordError_Txt.text = "";

            RegisterCheckPassword1_Txt.text = "Enter New Password.";
            RegisterCheckPassword2_Txt.text = "Allowed Characters: Alphanumeric A-Z, 0-9";
            RegisterCheckPassword3_Txt.text = "At Least 8 Chars.";

            bool check1 = CnahgeCheckPasswordIcon(true, RegisterCheckPassword1_Img);
            bool check2 = CnahgeCheckPasswordIcon(StringUtils.IsAlphaNumeric(RegisterPassword_If.text), RegisterCheckPassword2_Img);
            bool check3 = CnahgeCheckPasswordIcon(RegisterPassword_If.text.Length >= 8, RegisterCheckPassword3_Img);
            isRegisterPasswordCorrect = check1 && check2 && check3;
        });

        //手機注冊密碼顯示
        RegisterPasswordEye_Btn.onClick.AddListener(() =>
        {
            PasswordShowBtnClick(RegisterPasswordEye_Btn.image,
                                 RegisterPassword_If);
        });

        //手機注冊提交
        RegisterSubmit_Btn.onClick.AddListener(() =>
        {
            MobileRegisterSubmit();
        });

        //註冊成功登入
        RegisterSuccSignin_Btn.onClick.AddListener(() =>
        {
            RegisterSuccessSignIn();
        });

        //註冊成功登入取消按鈕
        RegisterSuccessfulCancel_Btn.onClick.AddListener(() =>
        {
            OnMobileSignInInit();
        });

        #endregion

        #region 忘記密碼

        //返回手機登入
        BackToMobileSignIn_Btn.onClick.AddListener(() =>
        {
            OnMobileSignInInit();
        });

        //忘記密碼密碼顯示
        LostPswPasswordEye_Btn.onClick.AddListener(() =>
        {
            PasswordShowBtnClick(LostPswPasswordEye_Btn.image,
                                 LosrPswPassword_If);
        });

        //忘記密碼發送獲取OTPCode
        LostPswOTPSend_Btn.onClick.AddListener(() =>
        {
            if (!StringUtils.CheckPhoneNumber(LostPswNumber_If.text))
            {
                LostPswNumberError_Txt.text = "User Name Entered Incorrectly, Please Try Again.";
            }
            else
            {
                LostPswNumberError_Txt.text = "";
                string phone = StringUtils.GetPhoneAddCode(LostPswNumber_Dd, LostPswNumber_If.text);
                Debug.Log($"Lost Password Send Code:{phone}");
            }
        });

        //忘記密碼密碼輸入
        LosrPswPassword_If.onValueChanged.AddListener((value) =>
        {
            LostPswCheckPassword_Obj.SetActive(value.Length > 0);

            LostPswPasswordError_Txt.text = "";

            LostPswCheckPassword1_Txt.text = "Enter New Password.";
            LostPswCheckPassword2_Txt.text = "Allowed Characters: Alphanumeric A-Z, 0-9";
            LostPswCheckPassword3_Txt.text = "At Least 8 Chars.";

            bool check1 = CnahgeCheckPasswordIcon(true, LostPswCheckPassword1_Img);
            bool check2 = CnahgeCheckPasswordIcon(StringUtils.IsAlphaNumeric(LosrPswPassword_If.text), LostPswCheckPassword2_Img);
            bool check3 = CnahgeCheckPasswordIcon(LosrPswPassword_If.text.Length >= 8, LostPswCheckPassword3_Img);
            isLostPswPasswordCorrect = check1 && check2 && check3;
        });

        //忘記密碼提交
        LostPswSubmit_Btn.onClick.AddListener(() =>
        {
            LostPswSubmit();            
        });

        #endregion
    }

    private void Start()
    {
        _currentChainData = ThirdwebManager.Instance.supportedChains.Find(x => x.identifier == ThirdwebManager.Instance.activeChain);

        SignUp_TmpTxt.text = $"Don't Have An Account? <color=#79E84B><link=Sign Up Here!><u>Sign Up Here!</u></link></color>";
        ForgotPassword_TmpTxt.text = $"<color=#79E84B><link=Forgot Password?><u>Forgot Password?</u></link></color>";
        SMSMobileNumberError_Txt.text = "";
        SMSCodeError_Txt.text = "";
        Vrsion_Txt.text = Entry.Instance.version;
        Wallet_Tog.isOn = true;
        OnSwlwctWalletInit();

        //獲取本地紀錄
        recodePhoneNumber = PlayerPrefs.GetString(LocalPhoneNumber);
        recodePassword = PlayerPrefs.GetString(LocalPaswword);

        ///自動連接Coinbase
        if (!DataManager.IsNotFirstInLogin &&
            DataManager.IsInCoinbase)
        {
            Debug.Log("Aoto Connect Coinbase");
            StartConnect("Coinbase", WalletEnum.Coinbase);
        }

        DataManager.IsNotFirstInLogin = true;
    }

    private void Update()
    {
        //連接錢包過久判定失敗
        if (Connecting_Obj.activeSelf &&
            (DateTime.Now - startConnectTime).TotalSeconds >= ErrorWalletConnectTime)
        {
            ErrorWalletConnect();
        }
        
        //當前輸入框切換
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            for (int i = 0; i < currIfList.Count; i++)
            {
                if (currIfList[i].isFocused)
                {
                    int next = i + 1 >= currIfList.Count ?
                               0 :
                               i + 1;
                    currIfList[next].Select();
                }
            }            
        }

        //執行Enter提交方法
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            KybordEnterAction?.Invoke();
        }

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.SetString(LocalPhoneNumber, "");
            PlayerPrefs.SetString(LocalPaswword, "");
        }

#endif
    }

    /// <summary>
    /// 選擇錢包畫面初始
    /// </summary>
    private void OnSwlwctWalletInit()
    {
        Wallet_Obj.SetActive(true);
        Mobile_Obj.SetActive(false);
        SelectWalletPage_Obj.SetActive(true);
        ConnectingWallet_Obj.SetActive(false);
        WalletLoadingPage_Obj.SetActive(false);
        SMSVerificationPage_Obj.SetActive(false);
    }

    /// <summary>
    /// 斷開錢包連接
    /// </summary>
    async private void OnWalletDisconnect()
    {
        bool isConnected = await ThirdwebManager.Instance.SDK.Wallet.IsConnected();
        if (isConnected)
        {
            await ThirdwebManager.Instance.SDK.Wallet.Disconnect(true);
            NFTManager.Instance.CancelUpdate();
            WalletManager.Instance.CancelCheckConnect();
            Debug.Log("Wallet Is Disconnected!");
        }
    }

    /// <summary>
    /// TIM_Text Link 點擊事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        //註冊
        int SignUpLinkIndex = TMP_TextUtilities.FindIntersectingLink(SignUp_TmpTxt, Input.mousePosition, null);
        if (SignUpLinkIndex != -1)
        {
            TMP_LinkInfo linkInfo = SignUp_TmpTxt.textInfo.linkInfo[SignUpLinkIndex];
            string linkID = linkInfo.GetLinkID();

            switch (linkID)
            {
                //註冊
                case "Sign Up Here!":
                    Mobile_Tog.isOn = true;
                    break;
            }
        }

        //忘記密碼
        int forgotPasswordLinkIndex = TMP_TextUtilities.FindIntersectingLink(ForgotPassword_TmpTxt, Input.mousePosition, null);
        if (forgotPasswordLinkIndex != -1)
        {
            TMP_LinkInfo linkInfo = ForgotPassword_TmpTxt.textInfo.linkInfo[forgotPasswordLinkIndex];
            string linkID = linkInfo.GetLinkID();

            switch (linkID)
            {
                //忘記密碼
                case "Forgot Password?":
                    MobileSignIn_Obj.SetActive(false);
                    LostPassword_Obj.SetActive(true);
                    LostPswCheckPassword_Obj.SetActive(false);

                    LostPswNumberError_Txt.text = "";
                    LostPswCodeError_Txt.text = "";
                    LostPswPasswordError_Txt.text = "";

                    //設定TAB切換與Enter提交方法
                    LostPswNumber_If.Select();
                    currIfList = new List<InputField>()
                    {
                        LostPswNumber_If,
                        LostPswOTP_If,
                        LosrPswPassword_If,
                    };
                    KybordEnterAction = LostPswSubmit;
                    break;
            }
        }

        //隱私條款
        int privacyLinkIndex = TMP_TextUtilities.FindIntersectingLink(Privacy_TmpTxt, Input.mousePosition, null);
        if (privacyLinkIndex != -1)
        {
            TMP_LinkInfo linkInfo = Privacy_TmpTxt.textInfo.linkInfo[privacyLinkIndex];
            string linkID = linkInfo.GetLinkID();

            switch (linkID)
            {
                //忘記密碼
                case "Forgot Password?":
                    Debug.Log("Clicked on Forgot Password?");
                    break;

                //條款
                case "Terms":
                    Debug.Log("Clicked on Terms");
                    break;

                //隱私權政策
                case "Privacy Policy":
                    Debug.Log("Clicked on Priacy Policy");
                    break;
            }
        }
    }

    /// <summary>
    /// 顯示密碼點擊事件
    /// </summary>
    /// <param name="img"></param>
    /// <param name="inputField"></param>
    private void PasswordShowBtnClick(Image img, InputField inputField)
    {
        isShowPassword = !isShowPassword;
        Sprite eye = isShowPassword ?
                     AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PasswordEye).album[1] :
                     AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PasswordEye).album[0];
        img.sprite = eye;

        inputField.contentType = isShowPassword ?
                                 InputField.ContentType.Standard :
                                 InputField.ContentType.Password;

        string currPsw = inputField.text;
        inputField.text = "";
        inputField.text = currPsw;
    }

    /// <summary>
    /// 更換檢查密碼圖樣
    /// </summary>
    /// <param name="isTrue"></param>
    /// <param name="img"></param>
    /// <returns></returns>
    private bool CnahgeCheckPasswordIcon(bool isTrue, Image img)
    {
        string mistakeColor = "#CF5A5A";
        string correctColor = "#87CF5A";

        img.sprite = isTrue ?
                     AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PasswordCheck).album[1] :
                     AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PasswordCheck).album[0];
        string colorStr = isTrue ?
                          correctColor :
                          mistakeColor;
        if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
        {
            img.color = color;
        }

        return isTrue;
    }

    #region 手機登入

    /// <summary>
    /// 手機登入初始
    /// </summary>
    async private void OnMobileSignInInit()
    {
        await ThirdwebManager.Instance.SDK.Wallet.Disconnect(true);

        //手機登入
        SignInNumberError_Txt.text = "";
        MobileSignInError_Txt.text = "";

        //紀錄的手機/密碼
        SignInNumber_If.text = !string.IsNullOrEmpty(recodePhoneNumber) ?
                               recodePhoneNumber :
                               "";
        SignInPassword_If.text = !string.IsNullOrEmpty(recodePassword) ?
                                 recodePassword :
                                 "";

        MobileTitle_Txt.text = "SIGN IN";
        MobileTip_Txt.text = "Please, Sign In With Your Walet.";
        MobileSignIn_Obj.SetActive(true);
        MobileSiginPage_Obj.SetActive(true);
        RegisterPage_Obj.SetActive(false);
        RegisterSucce_Obj.SetActive(false);
        LostPassword_Obj.SetActive(false);

        //設定TAB切換與Enter提交方法
        SignInNumber_If.Select();
        currIfList = new List<InputField>()
        {
            SignInNumber_If,
            SignInPassword_If,
        };
        KybordEnterAction = MobileSignInSubmit;
    }

    /// <summary>
    /// 手機登入提交
    /// </summary>
    private void MobileSignInSubmit()
    {
        if (!StringUtils.CheckPhoneNumber(SignInNumber_If.text))
        {
            SignInNumberError_Txt.text = "User Name Entered Incorrectly, Please Try Again.";
        }
        else
        {
            SignInNumberError_Txt.text = "";
            string phone = StringUtils.GetPhoneAddCode(SignInNumber_Dd, SignInNumber_If.text);
            string password = SignInPassword_If.text;
            Debug.Log($"Mobile Sign In = Phone:{phone} / Password = {password}");

            if (phone == "886-987654321" && password == "A12345678")
            {
                if (RememberMe_Tog.isOn)
                {
                    PlayerPrefs.SetString(LocalPhoneNumber, SignInNumber_If.text);
                    PlayerPrefs.SetString(LocalPaswword, SignInPassword_If.text);
                }

                LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
            }
            else
            {
                MobileSignInError_Txt.text = "Invalid Code, Please Try Again.";
                Debug.LogError("Correct is Phone:886-987654321 / Password:A12345678");
            }
        }
    }

    /// <summary>
    /// 手機註冊提交
    /// </summary>
    private void MobileRegisterSubmit()
    {
        RegisterNumberError_Txt.text = "";
        RegisterCodeError_Txt.text = "";
        RegisterPasswordError_Txt.text = "";

        bool isCorrect = true;
        if (!StringUtils.CheckPhoneNumber(RegisterNumber_If.text))
        {
            //手機號格式錯誤
            isCorrect = false;
            RegisterNumberError_Txt.text = "User Name Entered Incorrectly, Please Try Again.";
        }

        if (string.IsNullOrEmpty(RegisterOTP_If.text))
        {
            //OTP為空
            RegisterCodeError_Txt.text = "Invalid Code, Please Try Again.";
            isCorrect = false;
        }

        if (!isRegisterPasswordCorrect)
        {
            //密碼錯誤
            isCorrect = false;
            RegisterPasswordError_Txt.text = "Invalid Code, Please Try Again.";
        }

        if (!Privacy_Tog.isOn)
        {
            //隱私條款未同意
            isCorrect = false;
            RegisterPrivacyError_Txt.text = "Please Agree To The Privacy Policy.";
        }

        if (isCorrect)
        {
            //註冊成功
            string phone = StringUtils.GetPhoneAddCode(RegisterNumber_Dd, RegisterNumber_If.text);
            string code = RegisterOTP_If.text;
            string psw = RegisterPassword_If.text;
            Debug.Log($"Register Submit = Phone:{phone} / Code:{code} / Password:{psw}");

            MobileSignIn_Obj.SetActive(false);
            RegisterSucce_Obj.SetActive(true);

            RegisterSuccTip_Txt.text = $"<size=14><color=#FFFFF>Registration Successful</color></size>\n" +
                                       $"<size=12><color=#C6C2C2>Account Successfully Created</color></size>";

            //設定TAB切換與Enter提交方法
            KybordEnterAction = RegisterSuccessSignIn;
        }
    }

    /// <summary>
    /// 註冊成功登入
    /// </summary>
    private void RegisterSuccessSignIn()
    {
        LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
    }

    /// <summary>
    /// 忘記密碼提交
    /// </summary>
    private void LostPswSubmit()
    {
        LostPswNumberError_Txt.text = "";
        LostPswCodeError_Txt.text = "";
        LostPswPasswordError_Txt.text = "";

        bool isCorrect = true;
        if (!StringUtils.CheckPhoneNumber(LostPswNumber_If.text))
        {
            //手機號格式錯誤
            isCorrect = false;
            LostPswNumberError_Txt.text = "User Name Entered Incorrectly, Please Try Again.";
        }

        if (string.IsNullOrEmpty(LostPswOTP_If.text))
        {
            //OTP為空
            LostPswCodeError_Txt.text = "Invalid Code, Please Try Again.";
            isCorrect = false;
        }

        if (!isLostPswPasswordCorrect)
        {
            //密碼錯誤
            isCorrect = false;
            LostPswPasswordError_Txt.text = "Invalid Code, Please Try Again.";
        }

        if (isCorrect)
        {
            //註冊成功
            string phone = StringUtils.GetPhoneAddCode(LostPswNumber_Dd, LostPswNumber_If.text);
            string code = LostPswOTP_If.text;
            string psw = LosrPswPassword_If.text;
            Debug.Log($"Lost Password Submit = Phone:{phone} / Code:{code} / Password:{psw}");
        }
    }

    #endregion

    #region 錢包連接

    /// <summary>
    /// 嘗試連接Binance
    /// </summary>
    async public void TryBinanceConnect()
    {
        if (DataManager.IsMobilePlatform)
        {
            try
            {
                string add = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
                var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
                var balStr = $"{bal.value.ToEth()} {bal.symbol}";

                DataManager.UserWalletAddress = add;
                DataManager.UserWalletBalance = balStr;

                CancelInvoke(nameof(TryBinanceConnect));
                LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
            }
            catch (Exception)
            {
                Debug.LogError("Try Connect Fail!!!");
            }
        }
    }

    /// <summary>
    /// 連接錢包效果
    /// </summary>
    /// <returns></returns>
    private IEnumerator IConnectionEffect()
    {
        int curr = 0;
        while (true)
        {
            foreach (var point in EffectPointList)
            {
                point.color = new Color(155 / 255, 155 / 255, 155 / 255, 255);
            }

            EffectPointList[curr].color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.5f);

            curr++;
            if (curr >= EffectPointList.Count)
            {
                curr = 0;
            }
        }
    }

    /// <summary>
    /// 開始連接
    /// </summary>
    /// <param name="walletProviderStr">連接形式</param>
    /// <param name="walletEnum">連接的錢包</param>
    private void StartConnect(string walletProviderStr, WalletEnum walletEnum)
    {
        #region 開啟連接畫面

        startConnectTime = DateTime.Now;
        recordConnect.WalletProviderStr = walletProviderStr;
        recordConnect.TheWalletEnum = walletEnum;

        WalletLoadingPage_Obj.SetActive(true);
        SMSVerificationPage_Obj.SetActive(false);
        ConnectingWallet_Obj.SetActive(true);
        SelectWalletPage_Obj.SetActive(false);
        Connecting_Obj.SetActive(true);
        RetryConnectWallet_Obj.SetActive(false);

        ConnectionTitle_Txt.text = $"Log In Using {walletEnum}";
        Connecting_Txt.text = $"Load Into {walletEnum}";
        ConnectingLogo_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.WalletLogo).album[(int)walletEnum];
        connectionEffectCoroutine = StartCoroutine(IConnectionEffect());

        #endregion

        #region 錢包連接

        if (DataManager.IsMobilePlatform && 
            DataManager.IsDefaultBrowser &&
            Application.platform != RuntimePlatform.IPhonePlayer)
        {
            //在預設瀏覽器內
            JSBridgeManager.Instance.OpenNewBrowser(DataManager.LineMail, DataManager.IGIUserIdAndName);
            return;
        }
        
        var wc = new WalletConnection(provider: Enum.Parse<WalletProvider>(walletProviderStr), chainId: BigInteger.Parse(_currentChainData.chainId));
        Connect(wc);

        #endregion
    }

    /// <summary>
    /// 連接錢包
    /// </summary>
    /// <param name="wc"></param>
    async private void Connect(WalletConnection wc)
    {
#if UNITY_EDITOR

        await Task.Delay(500);
        SMSVerification();

#else

        Debug.Log("Start Connecting....");
        try
        {
            _address = await ThirdwebManager.Instance.SDK.Wallet.Connect(wc);
        }
        catch (Exception e)
        {
            ErrorWalletConnect();

            _address = null;
            Debug.LogError($"Failed to connect: {e}");
            return;
        }

        PostConnect(wc);

#endif
    }

    /// <summary>
    /// 連接錢包失敗
    /// </summary>
    private void ErrorWalletConnect()
    {
        Connecting_Obj.SetActive(false);
        RetryConnectWallet_Obj.SetActive(true);
        ErrorConnect_Txt.text = $"Error Logging Into {recordConnect.TheWalletEnum}";

        CancelInvoke(nameof(TryBinanceConnect));
        if (connectionEffectCoroutine != null)
        {
            StopCoroutine(connectionEffectCoroutine);
        }
    }

    /// <summary>
    /// 連接完成
    /// </summary>
    /// <param name="wc"></param>
    private async void PostConnect(WalletConnection wc = null)
    {
        Debug.Log($"Connected to {_address}");

        StopCoroutine(connectionEffectCoroutine);

        var addy = _address.ShortenAddress();
        DataManager.UserWalletAddress = _address;

        var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
        var balStr = $"{bal.value.ToEth()} {bal.symbol}";
        DataManager.UserWalletBalance = balStr;

        var chain = await ThirdwebManager.Instance.SDK.Wallet.GetChainId();

        Debug.Log($"Current Connect ChainID: {chain}");
        Debug.Log($"Address:{DataManager.UserWalletAddress}");
        Debug.Log($"Balance:{DataManager.UserWalletBalance}");

        NFTManager.Instance.StartHandleUpdate();
        WalletManager.Instance.StartCheckConnect();

        SMSVerification();
    }

    /// <summary>
    /// 簡訊確認
    /// </summary>
    private void SMSVerification()
    {   
        ConnectionTitle_Txt.text = "SMS Verification";
        WalletLoadingPage_Obj.SetActive(false);
        SMSVerificationPage_Obj.SetActive(true);

        //設定TAB切換與Enter提交方法
        SMSMobileNumber_If.Select();
        currIfList = new List<InputField>()
        {
            SMSMobileNumber_If,
            SMSOTP_If,
        };
        KybordEnterAction = SMSOTPSubmitAction;
    }

    /// <summary>
    /// 簡訊OTP提交
    /// </summary>
    async private void SMSOTPSubmitAction()
    {
        SMSMobileNumberError_Txt.text = "";
        SMSCodeError_Txt.text = "";

        bool isCorrect = true;
        if (!StringUtils.CheckPhoneNumber(SMSMobileNumber_If.text))
        {
            //手機號格式錯誤
            isCorrect = false;
            SMSMobileNumberError_Txt.text = "User Name Entered Incorrectly, Please Try Again.";
        }

        if (string.IsNullOrEmpty(SMSOTP_If.text))
        {
            //OTP為空
            SMSCodeError_Txt.text = "Invalid Code, Please Try Again.";
            isCorrect = false;
        }

        bool isConnect = await ThirdwebManager.Instance.SDK.Wallet.IsConnected();

        if (isCorrect && isConnect)
        {
            string code = SMSOTP_If.text;
            string phone = StringUtils.GetPhoneAddCode(SMSMobileNumber_Dd, SMSMobileNumber_If.text);
            Debug.Log($"Sign In = Phone Number : {phone} / Password: {code}");

            if (phone == "886-987654321" && code == "12345678")
            {
                LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
            }
            else
            {
                SMSCodeError_Txt.text = "Invalid Code, Please Try Again.";
                Debug.LogError("Correct is Phone:886-987654321 / Code:12345678");
            }
        }        
    }

    #endregion
}
