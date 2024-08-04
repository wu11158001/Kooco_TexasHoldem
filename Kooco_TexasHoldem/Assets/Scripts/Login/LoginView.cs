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
    TextMeshProUGUI Vrsion_Txt;
    [SerializeField]
    Toggle Wallet_Tog, Mobile_Tog;
    [SerializeField]
    TextMeshProUGUI WalletTog_Txt, MobileTog_Txt;
    [SerializeField]
    GameObject Wallet_Obj, Mobile_Obj;

    [Header("錢包連接頁面")]
    [SerializeField]
    GameObject SelectWalletPage_Obj;
    [SerializeField]
    Button Metamask_Btn, Trust_Btn, Binance_Btn, OKX_Btn, Coinbase_Btn;
    [SerializeField]
    TextMeshProUGUI SelectWalletTitle_Txt, SelectWalletTip_Txt;
    [SerializeField]
    List<TextMeshProUGUI> ConnectTip_TxtList;
    [SerializeField]
    TMP_Text SignUp_Txt;

    [Header("錢包連接_連接中頁面")]
    [SerializeField]
    GameObject WalletLoadingPage_Obj, ConnectingWallet_Obj, Connecting_Obj, RetryConnectWallet_Obj;
    [SerializeField]
    Button BackToSelectWallet_Btn, RetryConnectWallet_Btn;
    [SerializeField]
    TextMeshProUGUI ConnectionTitle_Txt, Connecting_Txt, ErrorConnect_Txt, RetryConnectWalletBtn_Txt;
    [SerializeField]
    Image ConnectingLogo_Img;
    [SerializeField]
    List<Image> EffectPointList;
    [SerializeField]
    TMP_Text DownloadWallet_Txt;

    [Header("錢包連接_簡訊認證頁面")]
    [SerializeField]
    GameObject SMSVerificationPage_Obj;
    [SerializeField]
    Button SMSOTPSend_Btn, SMSOTPSubmit_Btn;
    [SerializeField]
    TMP_InputField SMSMobileNumber_If, SMSOTP_If;
    [SerializeField]
    TMP_Dropdown SMSMobileNumber_Dd;
    [SerializeField]
    TextMeshProUGUI SMSMobileNumberError_Txt, SMSCodeError_Txt, 
                    SMSMobileNumber_Txt, SMSMobileNumberIf_Placeholder, 
                    SMSOTPCode_Txt, SMSOTPIf_Placeholder, SMSOTPSendBtn_Txt,
                    SMSOTPSubmitBtn_Txt;

    [Header("手機登入")]
    [SerializeField]
    GameObject MobileSignIn_Obj, MobileSiginPage_Obj;
    [SerializeField]
    Button SignIn_Btn, Register_Btn, SignInPasswordEye_Btn;
    [SerializeField]
    TMP_Dropdown SignInNumber_Dd;
    [SerializeField]
    TMP_InputField SignInNumber_If, SignInPassword_If;
    [SerializeField]
    Toggle RememberMe_Tog;
    [SerializeField]
    TMP_Text ForgotPassword_TmpTxt;
    [SerializeField]
    TextMeshProUGUI MobileTitle_Txt, MobileTip_Txt, MobileSignInError_Txt, SignInNumberError_Txt,
                    SignInMobileNumber_Txt, SignInNumberIf_Placeholder,
                    SignInPassword_Txt, SignInPasswordIf_Placeholder,
                    RememberMeTog_Txt, SignInBtn_Txt, RegisterBtn_Txt;


    [Header("手機註冊")]
    [SerializeField]
    GameObject RegisterPage_Obj;
    [SerializeField]
    TextMeshProUGUI RegisterNumberError_Txt, RegisterCodeError_Txt, RegisterPasswordError_Txt, RegisterPrivacyError_Txt;
    [SerializeField]
    Button RegisterOTPSend_Btn, RegisterPasswordEye_Btn, RegisterSubmit_Btn, RegisterSuccSignin_Btn, RegisterSuccessfulCancel_Btn;
    [SerializeField]
    TMP_InputField RegisterNumber_If, RegisterOTP_If, RegisterPassword_If;
    [SerializeField]
    TMP_Dropdown RegisterNumber_Dd;
    [SerializeField]
    Toggle Privacy_Tog;
    [SerializeField]
    TMP_Text Privacy_TmpTxt;
    [SerializeField]
    TextMeshProUGUI RegisterNumber_Txt, RegisterNumberIf_Placeholder,
                    RegisterCode_Txt, RegisterOTPIf_Placeholder, RegisterOTPSendBtn_Txt,
                    RegisterPassword_Txt, RegisterPasswordIf_Placeholder,
                    RegisterSubmitBtn_Txt;

    [Header("手機注冊密碼檢查")]
    [SerializeField]
    GameObject RegisterCheckPassword_Obj;
    [SerializeField]
    Image RegisterCheckPassword1_Img, RegisterCheckPassword2_Img, RegisterCheckPassword3_Img;
    [SerializeField]
    TextMeshProUGUI RegisterCheckPassword1_Txt, RegisterCheckPassword2_Txt, RegisterCheckPassword3_Txt;

    [Header("註冊成功")]
    [SerializeField]
    GameObject RegisterSucce_Obj;
    [SerializeField]
    TextMeshProUGUI RegisterSuccTitle_Txt, RegisterSuccTip_Txt, RegisterSuccSigninBtn_Txt;

    [Header("忘記密碼")]
    [SerializeField]
    GameObject LostPassword_Obj;
    [SerializeField]
    TMP_InputField LostPswNumber_If, LostPswOTP_If, LosrPswPassword_If;
    [SerializeField]
    Button BackToMobileSignIn_Btn, LostPswPasswordEye_Btn, LostPswOTPSend_Btn, LostPswSubmit_Btn;
    [SerializeField]
    TMP_Dropdown LostPswNumber_Dd;
    [SerializeField]
    TextMeshProUGUI LostPasswordTitle_Txt, LostPswNumber_Txt, LostPswNumberIf_Placeholder,
                    LostPswCode_Txt, LostPswOTPIf_Placeholder, LostPswOTPSendBtn_Txt,
                    LostPswPassword_Txt, LosrPswPasswordIf_Placeholder,
                    LostPswSubmitBtn_Txt,
                    LostPswNumberError_Txt, LostPswCodeError_Txt, LostPswPasswordError_Txt;

    [Header("忘記密碼密碼檢查")]
    [SerializeField]
    GameObject LostPswCheckPassword_Obj;
    [SerializeField]
    Image LostPswCheckPassword1_Img, LostPswCheckPassword2_Img, LostPswCheckPassword3_Img;
    [SerializeField]
    TextMeshProUGUI LostPswCheckPassword1_Txt, LostPswCheckPassword2_Txt, LostPswCheckPassword3_Txt;

    [Header("隱私政策物件")]
    [SerializeField]
    GameObject Privacy_Obj;
    [SerializeField]
    Button PrivacyConfirm_Btn;
    [SerializeField]
    TextMeshProUGUI PrivacyTitle_Txt, PrivacyContent_Txt,
                    PrivacyConfirmBtn_Txt;

    const int ErrorWalletConnectTime = 30;                                      //判定連接失敗等待時間
    const int codeCountDownTime = 60;                                           //發送OTP倒數時間

    const int InviteCodeLength = 11;                                            //邀請碼長度
    const int UserIdLength = 11;                                                //用戶Id長度

    const string LocalCountryCodeIndex = "AsiaPoker_CountryCodeIndex";          //本地紀錄_國碼編號
    const string LocalPhoneNumber = "AsiaPoker_PhoneNumber";                    //本地紀錄_手機號
    const string LocalPaswword = "AsiaPoker_Password";                          //本地紀錄_密碼
    const string LocalCodeStartTime = "AsiaPoker_CodeStartTime";                //本地紀錄_OTP發送時間

    int recodeCountryCodeIndex;                                                 //紀錄的國碼編號
    string recodePhoneNumber;                                                   //紀錄的手機號
    string recodePassword;                                                      //紀錄的密碼

    ChainData _currentChainData;                                                //當前連接練
    string _address;                                                            //錢包地址

    Coroutine connectionEffectCoroutine;                                        //連接錢包效果
    DateTime startConnectTime;                                                  //開始連接錢包時間
    bool isShowPassword;                                                        //是否顯示密碼
    bool isClickSignUpHere;                                                     //是否點擊註冊
    bool isRegisterPasswordCorrect;                                             //是否手機注冊密碼正確
    bool isLostPswPasswordCorrect;                                              //是否忘記密碼密碼正確
    DateTime codeStartTime;                                                     //發送OTP倒數開始時間
    WalletEnum currConnectingWallet;                                            //當前連接錢包

    string currVerifyPhoneNumber;                                               //當前驗證手機號
    string currVerifyPsw;                                                       //當前驗證密碼
    string currVerifyCode;                                                      //當前驗證OTP碼
    string currInviteCode;                                                      //當前驗證碼
    string currUserId;                                                          //當前UserId

    bool isStartCheckData;                                                      //是否開始檢測資料
    bool isGetInviteCode;                                                       //是否已取得邀請碼
    bool isGetUserId;                                                           //是否已取得UserId
    UnityAction checkDataCallbackFunc;                                           //檢測資料完成執行方法

    List<TMP_InputField> currIfList = new List<TMP_InputField>();               //當前可切換InputFild
    UnityAction KybordEnterAction;                                              //Enter鍵執行方法

    /// <summary>
    /// 紀錄當前連接錢包資料
    /// </summary>
    private RecordConnect recordConnect;
    public class RecordConnect
    {
        public string WalletProviderStr;
        public WalletEnum TheWalletEnum;
    }

    /// <summary>
    /// 更新文本翻譯
    /// </summary>
    private void UpdateLanguage()
    {
        #region 錢包連接頁面

        WalletTog_Txt.text = LanguageManager.Instance.GetText("Wallet");
        MobileTog_Txt.text = LanguageManager.Instance.GetText("Mobile");
        SelectWalletTitle_Txt.text = LanguageManager.Instance.GetText("SIGN IN");
        SelectWalletTip_Txt.text = LanguageManager.Instance.GetText("Please, Sign In With Your Wallet.");
        foreach (var item in ConnectTip_TxtList)
        {
            item.text = LanguageManager.Instance.GetText("Connect Using Browser Wallet");
        }
        SignUp_Txt.text = LanguageManager.Instance.GetText("Don't Have An Account? <color=#79E84B><link=Sign Up Here!><u>Sign Up Here!</u></link></color>");

        #endregion

        #region 錢包連接中頁面

        DownloadWallet_Txt.text = LanguageManager.Instance.GetText("If you have not downloaded the wallet, please download it first. <color=#79E84B><link=DownloadWallet><u>Download Here!</u></link></color>");
        RetryConnectWalletBtn_Txt.text = LanguageManager.Instance.GetText("Retry");

        #endregion

        #region 錢包簡訊認證頁面

        SMSMobileNumber_Txt.text = LanguageManager.Instance.GetText("MobileNumber");
        SMSMobileNumberIf_Placeholder.text = LanguageManager.Instance.GetText("Your Phone Number");
        SMSOTPCode_Txt.text = LanguageManager.Instance.GetText("OTP Code");
        SMSOTPIf_Placeholder.text = LanguageManager.Instance.GetText("Please Enter The OTP Code");
        SMSOTPSubmitBtn_Txt.text = LanguageManager.Instance.GetText("SUBMIT");

        #endregion

        #region 手機登入

        SignInMobileNumber_Txt.text = LanguageManager.Instance.GetText("MobileNumber");
        SignInNumberIf_Placeholder.text = LanguageManager.Instance.GetText("Your Phone Number");
        SignInPassword_Txt.text = LanguageManager.Instance.GetText("Password");
        SignInPasswordIf_Placeholder.text = LanguageManager.Instance.GetText("Please Enter Here");
        RememberMeTog_Txt.text = LanguageManager.Instance.GetText("Remember Me");
        SignInBtn_Txt.text = LanguageManager.Instance.GetText("SIGN IN");
        RegisterBtn_Txt.text = LanguageManager.Instance.GetText("REGISTER");
        ForgotPassword_TmpTxt.text = LanguageManager.Instance.GetText("<color=#79E84B><link=Forgot Password?><u>Forgot Password?</u></link></color>");

        #endregion

        #region 手機註冊

        RegisterNumber_Txt.text = LanguageManager.Instance.GetText("MobileNumber");
        RegisterNumberIf_Placeholder.text = LanguageManager.Instance.GetText("Your Phone Number");
        RegisterCode_Txt.text = LanguageManager.Instance.GetText("OTP Code");
        RegisterOTPIf_Placeholder.text = LanguageManager.Instance.GetText("Please Enter The OTP Code");
        RegisterPassword_Txt.text = LanguageManager.Instance.GetText("Password");
        RegisterPasswordIf_Placeholder.text = LanguageManager.Instance.GetText("Please Enter Here");
        RegisterCheckPassword1_Txt.text = LanguageManager.Instance.GetText("Enter New Password.");
        RegisterCheckPassword2_Txt.text = LanguageManager.Instance.GetText("Allowed Characters: Alphanumeric A-Z, 0-9.");
        RegisterCheckPassword3_Txt.text = LanguageManager.Instance.GetText("At Least 8 Chars.");
        Privacy_TmpTxt.text = LanguageManager.Instance.GetText("Agree To The <color=#79E84B><link=Terms><u>Terms</u></link></color> & <color=#79E84B><link=Privacy Policy><u>Privacy Policy</u></link></color>");
        RegisterSubmitBtn_Txt.text = LanguageManager.Instance.GetText("SUBMIT");

        #endregion

        #region 註冊成功

        RegisterSuccTitle_Txt.text = LanguageManager.Instance.GetText("Sign Up");
        RegisterSuccTip_Txt.text = LanguageManager.Instance.GetText("<size=14><color=#FFFFFF>Registration Successful</color></size>\n<size=12><color=#C6C2C2>Account Successfully Created</color></size>");
        RegisterSuccSigninBtn_Txt.text = LanguageManager.Instance.GetText("SIGN IN");

        #endregion

        #region 忘記密碼

        LostPasswordTitle_Txt.text = LanguageManager.Instance.GetText("Lost Password");
        LostPswNumber_Txt.text = LanguageManager.Instance.GetText("MobileNumber");
        LostPswNumberIf_Placeholder.text = LanguageManager.Instance.GetText("Your Phone Number");
        LostPswCode_Txt.text = LanguageManager.Instance.GetText("OTP Code");
        LostPswOTPIf_Placeholder.text = LanguageManager.Instance.GetText("Please Enter The OTP Code");
        LostPswPassword_Txt.text = LanguageManager.Instance.GetText("Reset Password");
        LosrPswPasswordIf_Placeholder.text = LanguageManager.Instance.GetText("Please Enter Here");
        LostPswCheckPassword1_Txt.text = LanguageManager.Instance.GetText("Enter New Password.");
        LostPswCheckPassword2_Txt.text = LanguageManager.Instance.GetText("Allowed Characters: Alphanumeric A-Z, 0-9.");
        LostPswCheckPassword3_Txt.text = LanguageManager.Instance.GetText("At Least 8 Chars.");
        LostPswSubmitBtn_Txt.text = LanguageManager.Instance.GetText("SUBMIT");

        #endregion

        #region 隱私政策物件

        PrivacyConfirmBtn_Txt.text = LanguageManager.Instance.GetText("Confirm");

        #endregion
    }

    private void OnDestroy()
    {
        LanguageManager.Instance.RemoveLanguageFun(UpdateLanguage);
    }

    private void Awake()
    {
        LanguageManager.Instance.AddUpdateLanguageFunc(UpdateLanguage, gameObject);

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
                if (isClickSignUpHere)
                {
                    isClickSignUpHere = false;
                    //手機註冊
                    MobileRegisterInit();
                }
                else
                {
                    //手機登入
                    OnMobileSignInInit();
                }

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

        #region 錢包連接簡訊認證

        //發送獲取驗證碼
        SMSOTPSend_Btn.onClick.AddListener(() =>
        {
            if (!StringUtils.CheckPhoneNumber(SMSMobileNumber_If.text))
            {
                SMSMobileNumberError_Txt.text = LanguageManager.Instance.GetText("User Name Entered Incorrectly, Please Try Again.");
                return;
            }

            SMSMobileNumberError_Txt.text = "";

            Debug.Log($"Send Code:{ StringUtils.GetPhoneAddCode(SMSMobileNumber_Dd, SMSMobileNumber_If.text) }");

            SMSMobileNumberError_Txt.text = "";
            SMSCodeError_Txt.text = "";
            SMSOTP_If.text = "";

            SendOTP(StringUtils.GetPhoneAddCode(SMSMobileNumber_Dd, SMSMobileNumber_If.text));
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
            isShowPassword = !isShowPassword;
            PasswordDisplayControl(isShowPassword);
        });

        #endregion

        #region 手機注冊

        //手機註冊
        Register_Btn.onClick.AddListener(() =>
        {
            MobileRegisterInit();
        });

        //手機注冊發送獲取OTPCode
        RegisterOTPSend_Btn.onClick.AddListener(() =>
        {
            if (!StringUtils.CheckPhoneNumber(RegisterNumber_If.text))
            {
                RegisterNumberError_Txt.text = LanguageManager.Instance.GetText("User Name Entered Incorrectly, Please Try Again.");
            }
            else
            {
                RegisterNumberError_Txt.text = "";
                Debug.Log($"Register Send Code:{ StringUtils.GetPhoneAddCode(RegisterNumber_Dd, RegisterNumber_If.text) }");

                SendOTP(StringUtils.GetPhoneAddCode(RegisterNumber_Dd, RegisterNumber_If.text));
            }
        });

        //手機注冊密碼輸入
        RegisterPassword_If.onValueChanged.AddListener((value) =>
        {
            RegisterCheckPassword_Obj.SetActive(value.Length > 0);

            RegisterPasswordError_Txt.text = "";

            bool check1 = GameUtils.CnahgeCheckIcon(true, RegisterCheckPassword1_Img);
            bool check2 = GameUtils.CnahgeCheckIcon(StringUtils.IsAlphaNumeric(RegisterPassword_If.text), RegisterCheckPassword2_Img);
            bool check3 = GameUtils.CnahgeCheckIcon(RegisterPassword_If.text.Length >= 8, RegisterCheckPassword3_Img);
            isRegisterPasswordCorrect = check1 && check2 && check3;
        });

        //手機注冊密碼顯示
        RegisterPasswordEye_Btn.onClick.AddListener(() =>
        {
            isShowPassword = !isShowPassword;
            PasswordDisplayControl(isShowPassword);
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
            isShowPassword = !isShowPassword;
            PasswordDisplayControl(isShowPassword);
        });

        //忘記密碼發送獲取OTPCode
        LostPswOTPSend_Btn.onClick.AddListener(() =>
        {
            if (!StringUtils.CheckPhoneNumber(LostPswNumber_If.text))
            {
                LostPswNumberError_Txt.text = LanguageManager.Instance.GetText("User Name Entered Incorrectly, Please Try Again.");
            }
            else
            {
                LostPswNumberError_Txt.text = "";
                Debug.Log($"Lost Password Send Code:{ StringUtils.GetPhoneAddCode(LostPswNumber_Dd, LostPswNumber_If.text) }");

                SendOTP(StringUtils.GetPhoneAddCode(LostPswNumber_Dd, LostPswNumber_If.text));
            }
        });

        //忘記密碼密碼輸入
        LosrPswPassword_If.onValueChanged.AddListener((value) =>
        {
            LostPswCheckPassword_Obj.SetActive(value.Length > 0);

            LostPswPasswordError_Txt.text = "";

            bool check1 = GameUtils.CnahgeCheckIcon(value.Length > 0, LostPswCheckPassword1_Img);
            bool check2 = GameUtils.CnahgeCheckIcon(StringUtils.IsAlphaNumeric(LosrPswPassword_If.text), LostPswCheckPassword2_Img);
            bool check3 = GameUtils.CnahgeCheckIcon(LosrPswPassword_If.text.Length >= 8, LostPswCheckPassword3_Img);
            isLostPswPasswordCorrect = check1 && check2 && check3;
        });

        //忘記密碼提交
        LostPswSubmit_Btn.onClick.AddListener(() =>
        {
            LostPswSubmit();            
        });

        #endregion

        #region 隱私權政策物件

        //確認
        PrivacyConfirm_Btn.onClick.AddListener(() =>
        {
            Privacy_Obj.SetActive(false);
        });

        #endregion
    }

    private void Start()
    {
        //下拉式選單添加國碼
        Utils.SetOptionsToDropdown(SMSMobileNumber_Dd, DataManager.CountryCode);
        Utils.SetOptionsToDropdown(SignInNumber_Dd, DataManager.CountryCode);
        Utils.SetOptionsToDropdown(RegisterNumber_Dd, DataManager.CountryCode);
        Utils.SetOptionsToDropdown(LostPswNumber_Dd, DataManager.CountryCode);

        _currentChainData = ThirdwebManager.Instance.supportedChains.Find(x => x.identifier == ThirdwebManager.Instance.activeChain);

        SMSMobileNumberError_Txt.text = "";
        SMSCodeError_Txt.text = "";
        Vrsion_Txt.text = Entry.Instance.version;
        Wallet_Tog.isOn = true;
        Privacy_Obj.gameObject.SetActive(false);

        OnSwlwctWalletInit();

        //獲取本地紀錄
        recodePhoneNumber = PlayerPrefs.GetString(LocalPhoneNumber);
        recodePassword = PlayerPrefs.GetString(LocalPaswword);
        recodeCountryCodeIndex = PlayerPrefs.GetInt(LocalCountryCodeIndex);
        string recodeOtpTime = PlayerPrefs.GetString(LocalCodeStartTime);
        if (DateTime.TryParse(recodeOtpTime, out DateTime parseTime))
        {
            codeStartTime = parseTime;
        }
        else
        {
            codeStartTime = DateTime.Now.AddSeconds(-codeCountDownTime);
        }

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
        //發送OTP倒數
        float codeTime = (float)(DateTime.Now - codeStartTime).TotalSeconds;
        LostPswOTPSend_Btn.interactable = codeTime > codeCountDownTime;
        LostPswOTPSendBtn_Txt.text = codeTime > codeCountDownTime ?
                                     LanguageManager.Instance.GetText("SEND CODE") :
                                     $"{LanguageManager.Instance.GetText("RESEND")} {codeCountDownTime - (int)codeTime}";
        RegisterOTPSend_Btn.interactable = codeTime > codeCountDownTime;
        RegisterOTPSendBtn_Txt.text = codeTime > codeCountDownTime ?
                                      LanguageManager.Instance.GetText("SEND CODE") :
                                      $"{LanguageManager.Instance.GetText("RESEND")} {codeCountDownTime - (int)codeTime}";
        SMSOTPSend_Btn.interactable = codeTime > codeCountDownTime;
        SMSOTPSendBtn_Txt.text = codeTime > codeCountDownTime ?
                                 LanguageManager.Instance.GetText("SEND CODE") :
                                 $"{LanguageManager.Instance.GetText("RESEND")} {codeCountDownTime - (int)codeTime}";

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

        //開始檢測資料重複
        if (isStartCheckData)
        {
            //已取得所有資料
            if (isGetInviteCode && isGetUserId)
            {
                isStartCheckData = false;

                checkDataCallbackFunc.Invoke();
            }
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

    #region 工具類

    /// <summary>
    /// TIM_Text Link 點擊事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        //註冊
        int SignUpLinkIndex = TMP_TextUtilities.FindIntersectingLink(SignUp_Txt, Input.mousePosition, null);
        if (SignUpLinkIndex != -1)
        {
            TMP_LinkInfo linkInfo = SignUp_Txt.textInfo.linkInfo[SignUpLinkIndex];
            string linkID = linkInfo.GetLinkID();

            switch (linkID)
            {
                //註冊
                case "Sign Up Here!":
                    isClickSignUpHere = true;
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

                    isShowPassword = false;
                    PasswordDisplayControl(isShowPassword);

                    //設定TAB切換與Enter提交方法
                    if (!DataManager.IsMobilePlatform)
                    {
                        LostPswNumber_If.Select();
                        currIfList = new List<TMP_InputField>()
                        {
                            LostPswNumber_If,
                            LostPswOTP_If,
                            LosrPswPassword_If,
                        };
                        KybordEnterAction = LostPswSubmit;
                    }
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
                //條款
                case "Terms":
                    Privacy_Obj.SetActive(true);
                    PrivacyTitle_Txt.text = LanguageManager.Instance.GetText("Terms");
                    PrivacyContent_Txt.text = LanguageManager.Instance.GetText("Terms Content");
                    break;

                //隱私權政策
                case "Privacy Policy":
                    Privacy_Obj.SetActive(true);
                    PrivacyTitle_Txt.text = LanguageManager.Instance.GetText("Privacy Policy");
                    PrivacyContent_Txt.text = LanguageManager.Instance.GetText("Privacy Policy Content");
                    break;
            }
        }

        //下載錢包
        int DownloadWalletIndex = TMP_TextUtilities.FindIntersectingLink(DownloadWallet_Txt, Input.mousePosition, null);
        if (DownloadWalletIndex != -1)
        {
            TMP_LinkInfo linkInfo = DownloadWallet_Txt.textInfo.linkInfo[DownloadWalletIndex];
            string linkID = linkInfo.GetLinkID();

            switch (linkID)
            {
                //下載點選錢包APP
                case "DownloadWallet":
                    DataManager.IsOpenDownloadWallet = true;
                    JSBridgeManager.Instance.OpenDownloadWallet(currConnectingWallet);
                    break;
            }
        }
    }

    /// <summary>
    /// 讀取資料判斷是否已有資料
    /// </summary>
    /// <param name="callBackFunName">回傳方法名</param>
    /// <param name="childNode">資料節點路徑</param>
    private void JudgeDateExists(string callBackFunName, string childNode)
    {        
        ViewManager.Instance.OpenWaitingView(transform);
        JSBridgeManager.Instance.ReadDataFromFirebase($"{Entry.Instance.releaseType}/{FirebaseManager.USER_DATA_PATH}{childNode}/{currVerifyPhoneNumber}",
                                                       gameObject.name,
                                                       callBackFunName);
    }

    /// <summary>
    /// 發送OTP
    /// </summary>
    /// <param name="phoneNumber">手機號</param>
    private void SendOTP(string phoneNumber)
    {
        ViewManager.Instance.OpenTipMsgView(transform,
                                            LanguageManager.Instance.GetText("Sent OTP to SMS"));

        currVerifyPhoneNumber = phoneNumber;
        JSBridgeManager.Instance.TriggerRecaptcha($"{currVerifyPhoneNumber}");

        codeStartTime = DateTime.Now;
        string codeStartTimeStr = codeStartTime.ToString("yyyy-MM-dd HH:mm:ss");
        PlayerPrefs.SetString(LocalCodeStartTime, codeStartTimeStr);
    }

    /// <summary>
    /// 密碼顯示控制
    /// </summary>
    /// <param name="isShowPsw">是否顯示密碼</param>
    private void PasswordDisplayControl(bool isShowPsw)
    {
        //(密碼眼睛圖, 密碼輸入框)
        Dictionary<Image, TMP_InputField> pswEyesDic = new()
        {
            { SignInPasswordEye_Btn.image, SignInPassword_If },
            { RegisterPasswordEye_Btn.image, RegisterPassword_If },
            { LostPswPasswordEye_Btn.image, LosrPswPassword_If },
        };

        foreach (var item in pswEyesDic)
        {
            Sprite eye = isShowPsw ?
                         AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PasswordEyeAlbum).album[1] :
                         AssetsManager.Instance.GetAlbumAsset(AlbumEnum.PasswordEyeAlbum).album[0];
            item.Key.sprite = eye;

            item.Value.contentType = isShowPsw ?
                                     TMP_InputField.ContentType.Standard :
                                     TMP_InputField.ContentType.Password;

            string currPsw = item.Value.text;
            item.Value.text = "";
            item.Value.text = currPsw;
        }
    }

    /// <summary>
    /// 本地資料紀錄
    /// </summary>
    private void LocalDataSave() 
    {
        PlayerPrefs.SetInt(LocalCountryCodeIndex, SignInNumber_Dd.value);
        PlayerPrefs.SetString(LocalPhoneNumber, recodePhoneNumber);
        PlayerPrefs.SetString(LocalPaswword, currVerifyPsw);
    }

#endregion

    #region 手機登入

    /// <summary>
    /// 手機登入初始
    /// </summary>
    async private void OnMobileSignInInit()
    {
        //手機登入
        MobileTitle_Txt.text = LanguageManager.Instance.GetText("SIGN IN");
        SignInNumberError_Txt.text = "";
        MobileSignInError_Txt.text = "";

        await ThirdwebManager.Instance.SDK.Wallet.Disconnect(true);

        //紀錄的國碼/手機/密碼
        SignInNumber_Dd.value = recodeCountryCodeIndex;
        SignInNumber_If.text = !string.IsNullOrEmpty(recodePhoneNumber) ?
                               recodePhoneNumber :
                               "";
        SignInPassword_If.text = !string.IsNullOrEmpty(recodePassword) ?
                                 recodePassword :
                                 "";

        MobileTip_Txt.text = LanguageManager.Instance.GetText("Please use your mobile phone number to log in.");
        MobileSignIn_Obj.SetActive(true);
        MobileSiginPage_Obj.SetActive(true);
        RegisterPage_Obj.SetActive(false);
        RegisterSucce_Obj.SetActive(false);
        LostPassword_Obj.SetActive(false);

        isShowPassword = false;
        PasswordDisplayControl(isShowPassword);

        //設定TAB切換與Enter提交方法
        if (!DataManager.IsMobilePlatform)
        {
            SignInNumber_If.Select();
            currIfList = new List<TMP_InputField>()
            {
                SignInNumber_If,
                SignInPassword_If,
            };
            KybordEnterAction = MobileSignInSubmit;
        }
    }

    /// <summary>
    /// 手機登入提交
    /// </summary>
    private void MobileSignInSubmit()
    {
#if UNITY_EDITOR
        LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
        return;
#endif


        MobileSignInError_Txt.text = "";
        SignInNumberError_Txt.text = "";

        if (!StringUtils.CheckPhoneNumber(SignInNumber_If.text))
        {
            SignInNumberError_Txt.text = LanguageManager.Instance.GetText("User Name Entered Incorrectly, Please Try Again.");
        }
        else
        {
            currVerifyPhoneNumber = StringUtils.GetPhoneAddCode(SignInNumber_Dd, SignInNumber_If.text);
            currVerifyPsw = SignInPassword_If.text;
            Debug.Log($"Mobile Sign In = Phone:{currVerifyPhoneNumber} / Password = {currVerifyPsw}");

            JudgeDateExists(nameof(JudgeMobileSignIn),
                            LoginType.phoneUser.ToString());
        }
    }

    /// <summary>
    /// 手機登入判斷
    /// </summary>
    /// <param name="jsonData">判斷資料回傳</param>
    private void JudgeMobileSignIn(string jsonData)
    {
        ViewManager.Instance.CloseWaitingView(transform);

        AccountData loginData = FirebaseManager.Instance.OnFirebaseDataRead<AccountData>(jsonData);
        if (loginData.phoneNumber != null)
        {
            if (loginData.password == currVerifyPsw)
            {
                //登入成功
                if (RememberMe_Tog.isOn)
                {
                    recodePhoneNumber = SignInNumber_If.text;
                    recodePassword = SignInPassword_If.text;
                    recodeCountryCodeIndex = SignInNumber_Dd.value;

                    //有勾選記住帳號密碼
                    LocalDataSave();
                }
                else
                {
                    //沒勾選清空資料
                    PlayerPrefs.SetInt(LocalCountryCodeIndex, 0);
                    PlayerPrefs.SetString(LocalPhoneNumber, "");
                    PlayerPrefs.SetString(LocalPaswword, "");
                }

                DataManager.UserLoginType = LoginType.phoneUser;
                OnIntoLobby();
            }
            else
            {
                MobileSignInError_Txt.text = LanguageManager.Instance.GetText("Invalid Code, Please Try Again.");
            }
        }
        else
        {
            SignInNumberError_Txt.text = LanguageManager.Instance.GetText("User Name Entered Incorrectly, Please Try Again.");
        }
    }

    #endregion

    #region 手機註冊

    /// <summary>
    /// 手機註冊初始化
    /// </summary>
    private void MobileRegisterInit()
    {
        MobileTitle_Txt.text = LanguageManager.Instance.GetText("REGISTER");
        MobileTip_Txt.text = LanguageManager.Instance.GetText("You Will Receive A New Account.");

        RegisterNumberError_Txt.text = "";
        RegisterCodeError_Txt.text = "";
        RegisterPasswordError_Txt.text = "";
        RegisterPrivacyError_Txt.text = "";

        MobileSiginPage_Obj.SetActive(false);
        RegisterPage_Obj.SetActive(true);
        RegisterCheckPassword_Obj.SetActive(false);

        isShowPassword = false;
        PasswordDisplayControl(isShowPassword);

        //設定TAB切換與Enter提交方法
        if (!DataManager.IsMobilePlatform)
        {
            RegisterNumber_If.Select();
            currIfList = new List<TMP_InputField>()
            {
                RegisterNumber_If,
                RegisterOTP_If,
                RegisterPassword_If,
            };
            KybordEnterAction = MobileRegisterSubmit;
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

        string phoneNumber = StringUtils.GetPhoneAddCode(RegisterNumber_Dd, RegisterNumber_If.text);
        string code = RegisterOTP_If.text;
        string psw = RegisterPassword_If.text;

        bool isCorrect = true;
        if (!StringUtils.CheckPhoneNumber(RegisterNumber_If.text))
        {
            //手機號格式錯誤
            isCorrect = false;
            RegisterNumberError_Txt.text = LanguageManager.Instance.GetText("User Name Entered Incorrectly, Please Try Again.");
        }

        if (string.IsNullOrEmpty(RegisterOTP_If.text))
        {
            //OTP為空
            RegisterCodeError_Txt.text = LanguageManager.Instance.GetText("Invalid Code, Please Try Again.");
            isCorrect = false;
        }

        if (!isRegisterPasswordCorrect)
        {
            //密碼錯誤
            isCorrect = false;
            RegisterPasswordError_Txt.text = LanguageManager.Instance.GetText("Invalid Code, Please Try Again.");
        }

        if (!Privacy_Tog.isOn)
        {
            //隱私條款未同意
            isCorrect = false;
            RegisterPrivacyError_Txt.text = LanguageManager.Instance.GetText("Please Agree To The Privacy Policy.");
        }

        if (phoneNumber != $"{currVerifyPhoneNumber}")
        {
            //輸入手機號與驗證手機號不符
            isCorrect = false;
            RegisterCodeError_Txt.text = LanguageManager.Instance.GetText("Invalid Code, Please Try Again.");
        }

        if (isCorrect)
        {
            //資料正確    
            Debug.Log($"Register Submit = Phone:{phoneNumber} / Code:{code} / Password:{psw}");

            currVerifyPsw = psw;
            currVerifyCode = code;

            RegisterNumberError_Txt.text = "";
            RegisterCodeError_Txt.text = "";
            RegisterPasswordError_Txt.text = "";

            //讀取資料判斷是否已有資料
            JudgeDateExists(nameof(RegisterVerifyCode),
                            LoginType.phoneUser.ToString());
        }
    }

    /// <summary>
    /// 手機註冊OTP驗證
    /// </summary>
    /// <param name="jsonData">回傳資料</param>
    private void RegisterVerifyCode(string jsonData)
    {
        AccountData loginData = FirebaseManager.Instance.OnFirebaseDataRead<AccountData>(jsonData);

        if (loginData.phoneNumber != null)
        {
            //已有相同手機號
            ViewManager.Instance.CloseWaitingView(transform);
            RegisterNumberError_Txt.text = LanguageManager.Instance.GetText("Mobile Number Has Been Used, Please Try Again.");
            return;
        }

        JSBridgeManager.Instance.FirebaseVerifyCode(currVerifyCode,
                                                    gameObject.name,
                                                    nameof(RegisterOTPVerifyCallback));
    }

    /// <summary>
    /// 手機註冊OTP驗證回傳
    /// </summary>
    /// <param name="isSuccess">回傳結果(true/false)</param>
    public void RegisterOTPVerifyCallback(string isSuccess)
    {
        ViewManager.Instance.CloseWaitingView(transform);

        if (isSuccess == "false")
        {
            //驗證失敗
            RegisterCodeError_Txt.text = LanguageManager.Instance.GetText("Invalid Code, Please Try Again.");
            return;
        }

        checkDataCallbackFunc = WritePhoneNewUser;
        SetUniqueData();
    }

    /// <summary>
    /// 寫入手機新用戶資料
    /// </summary>
    private void WritePhoneNewUser()
    {
        //註冊成功
        MobileSignIn_Obj.SetActive(false);
        RegisterSucce_Obj.SetActive(true);

        //設定TAB切換與Enter提交方法
        KybordEnterAction = RegisterSuccessSignIn;

        //記錄的資料
        recodePhoneNumber = RegisterNumber_If.text;
        recodePassword = RegisterPassword_If.text;
        recodeCountryCodeIndex = RegisterNumber_Dd.value;

        //本地資料紀錄
        LocalDataSave();

        //寫入資料
        Dictionary<string, object> dataDic = new()
        {
            { FirebaseManager.PHONE_NUMBER, currVerifyPhoneNumber},
            { FirebaseManager.PASSWORD, currVerifyPsw },
            { FirebaseManager.INVITATION_CODE, currInviteCode },
            { FirebaseManager.USER_ID, currUserId },
            { FirebaseManager.AVATAR_INDEX, 0 },
        };
        JSBridgeManager.Instance.WriteDataFromFirebase($"{Entry.Instance.releaseType}/{FirebaseManager.USER_DATA_PATH }{LoginType.phoneUser}/{currVerifyPhoneNumber}",
                                                        dataDic);
    }

    /// <summary>
    /// 註冊成功登入
    /// </summary>
    private void RegisterSuccessSignIn()
    {
        DataManager.UserLoginType = LoginType.phoneUser;
        OnIntoLobby();
    }

    #endregion

    #region 忘記密碼

    /// <summary>
    /// 忘記密碼提交
    /// </summary>
    private void LostPswSubmit()
    {
        LostPswNumberError_Txt.text = "";
        LostPswCodeError_Txt.text = "";
        LostPswPasswordError_Txt.text = "";

        string phoneNumber = StringUtils.GetPhoneAddCode(LostPswNumber_Dd, LostPswNumber_If.text);
        string code = LostPswOTP_If.text;
        string psw = LosrPswPassword_If.text;

        bool isCorrect = true;
        if (!StringUtils.CheckPhoneNumber(LostPswNumber_If.text))
        {
            //手機號格式錯誤
            isCorrect = false;
            LostPswNumberError_Txt.text = LanguageManager.Instance.GetText("User Name Entered Incorrectly, Please Try Again.");
        }

        if (string.IsNullOrEmpty(LostPswOTP_If.text))
        {
            //OTP為空
            LostPswCodeError_Txt.text = LanguageManager.Instance.GetText("Invalid Code, Please Try Again.");
            isCorrect = false;
        }

        if (!isLostPswPasswordCorrect)
        {
            //密碼錯誤
            isCorrect = false;
            LostPswPasswordError_Txt.text = LanguageManager.Instance.GetText("Invalid Code, Please Try Again.");
        }

        if (StringUtils.GetPhoneAddCode(LostPswNumber_Dd, phoneNumber) != currVerifyPhoneNumber)
        {
            //輸入框手機號與驗證手機號不符
            isCorrect = false;
        }

        if (isCorrect)
        {
            //忘記密碼提交成功
            Debug.Log($"Lost Password Submit = Phone:{StringUtils.GetPhoneAddCode(LostPswNumber_Dd, phoneNumber)} / Code:{code} / Password:{psw}");

            currVerifyPhoneNumber = phoneNumber;
            currVerifyCode = code;
            currVerifyPsw = psw;

            LostPswNumberError_Txt.text = "";
            LostPswCodeError_Txt.text = "";
            LostPswPasswordError_Txt.text = "";

            //讀取資料判斷是否已有資料
            JudgeDateExists(nameof(LostPswVerifyCode),
                            LoginType.phoneUser.ToString());
        }
    }

    /// <summary>
    /// 忘記密碼OTP驗證
    /// </summary>
    /// <param name="jsonData">判斷資料回傳</param>
    private void LostPswVerifyCode(string jsonData)
    {
        AccountData loginData = FirebaseManager.Instance.OnFirebaseDataRead<AccountData>(jsonData);

        if (loginData.phoneNumber == null)
        {
            //沒有有找到手機號
            ViewManager.Instance.OpenWaitingView(transform);
            LostPswNumberError_Txt.text = LanguageManager.Instance.GetText("User Name Entered Incorrectly, Please Try Again.");
            return;
        }

        JSBridgeManager.Instance.FirebaseVerifyCode(currVerifyCode,
                                                    gameObject.name,
                                                    nameof(LostPswOTPVerityCallback));
    }

    /// <summary>
    /// 忘記密碼OTP驗證回傳
    /// </summary>
    /// <param name="isSuccess">回傳結果(true/false)</param>
    public void LostPswOTPVerityCallback(string isSuccess)
    {
        ViewManager.Instance.CloseWaitingView(transform);

        if (isSuccess == "false")
        {
            //驗證失敗
            LostPswCodeError_Txt.text = LanguageManager.Instance.GetText("Invalid Code, Please Try Again.");
            return;
        }

        //修改資料
        Dictionary<string, object> dataDic = new()
        {
            { FirebaseManager.PHONE_NUMBER, currVerifyPhoneNumber },
            { FirebaseManager.PASSWORD, currVerifyPsw }
        };
        JSBridgeManager.Instance.UpdateDataFromFirebase($"{Entry.Instance.releaseType}/{FirebaseManager.USER_DATA_PATH}{LoginType.phoneUser}/{currVerifyPhoneNumber}",
                                                        dataDic);

        //修改本地資料
        PlayerPrefs.SetString(LocalPhoneNumber, LostPswNumber_If.text);
        PlayerPrefs.SetString(LocalPaswword, "");
        recodePhoneNumber = LostPswNumber_If.text;
        recodePassword = "";

        OnMobileSignInInit();
    }

    #endregion

    #region 錢包連接

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

        SMSMobileNumberError_Txt.text = "";
        SMSCodeError_Txt.text = "";
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
    async private void StartConnect(string walletProviderStr, WalletEnum walletEnum)
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

        ConnectionTitle_Txt.text = $"{LanguageManager.Instance.GetText("Log In Using")} {walletEnum}";
        Connecting_Txt.text = $"{LanguageManager.Instance.GetText("Load Into")} {walletEnum}";
        ConnectingLogo_Img.sprite = AssetsManager.Instance.GetAlbumAsset(AlbumEnum.WalletLogoAlbum).album[(int)walletEnum];
        connectionEffectCoroutine = StartCoroutine(IConnectionEffect());

        #endregion

        #region 錢包連接

        currConnectingWallet = walletEnum;
        DownloadWallet_Txt.gameObject.SetActive(DataManager.IsMobilePlatform);

        if (DataManager.IsMobilePlatform && 
            DataManager.IsDefaultBrowser &&
            Application.platform != RuntimePlatform.IPhonePlayer)
        {
            //在預設瀏覽器內
            JSBridgeManager.Instance.OpenNewBrowser(DataManager.LineMail, DataManager.IGIUserIdAndName);
            return;
        }

        //非移動平台
        if (!DataManager.IsMobilePlatform)
        {
            if (walletProviderStr == "Coinbase")
            {
                //Coinbase 使用 Thirdweb

                OnConnectWallet();
            }
            else
            {
                //其他錢包判斷是否有安裝錢包擴充

                if (JSBridgeManager.Instance.WindowCheckWallet(walletEnum))
                {
                    //有安裝錢包
                    OnConnectWallet();
                }
                else
                {
                    DataManager.IsOpenDownloadWallet = true;

                    if (walletEnum == WalletEnum.Coinbase)
                    {
                        await Task.Delay(2000);
                        ErrorWalletConnect();

                        OnConnectWallet();
                    }
                    else
                    {
                        ErrorWalletConnect();
                    }
                }
            }           
        }
        else
        {
            //在移動平台
            OnConnectWallet();
        }

        //連接錢包
        void OnConnectWallet()
        {
            var wc = new WalletConnection(provider: Enum.Parse<WalletProvider>(walletProviderStr), chainId: BigInteger.Parse(_currentChainData.chainId));
            Connect(wc);
        }

        #endregion
    }

    /// <summary>
    /// 連接錢包
    /// </summary>
    /// <param name="wc"></param>
    async private void Connect(WalletConnection wc)
    {
#if UNITY_EDITOR

        await Task.Delay(2000);
        OpenSMSVerificationPage();

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
        ErrorConnect_Txt.text = $"{LanguageManager.Instance.GetText("Error Logging Into")} {recordConnect.TheWalletEnum}";

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

        OpenSMSVerificationPage();
    }

    /// <summary>
    /// 開啟錢包簡訊確認頁面
    /// </summary>
    private void OpenSMSVerificationPage()
    {
        ConnectionTitle_Txt.text = LanguageManager.Instance.GetText("SMS Verification");
        WalletLoadingPage_Obj.SetActive(false);
        SMSVerificationPage_Obj.SetActive(true);

        //設定TAB切換與Enter提交方法
        if (!DataManager.IsMobilePlatform)
        {
            SMSMobileNumber_If.Select();
            currIfList = new List<TMP_InputField>()
            {
                SMSMobileNumber_If,
                SMSOTP_If,
            };
            KybordEnterAction = SMSOTPSubmitAction;
        }
    }

    /// <summary>
    /// 簡訊OTP提交
    /// </summary>
    async private void SMSOTPSubmitAction()
    {
        SMSMobileNumberError_Txt.text = "";
        SMSCodeError_Txt.text = "";

        string phoneNumber = StringUtils.GetPhoneAddCode(SMSMobileNumber_Dd, SMSMobileNumber_If.text);
        string code = SMSOTP_If.text;

        bool isCorrect = true;
        if (!StringUtils.CheckPhoneNumber(SMSMobileNumber_If.text))
        {
            //手機號格式錯誤
            isCorrect = false;
            SMSMobileNumberError_Txt.text = LanguageManager.Instance.GetText("User Name Entered Incorrectly, Please Try Again.");
        }

        if (string.IsNullOrEmpty(SMSOTP_If.text))
        {
            //OTP為空
            isCorrect = false;
        }

        if (phoneNumber != currVerifyPhoneNumber)
        {
            //輸入框手機號與驗證手機號不符
            isCorrect = false;
        }

        bool isConnect = await ThirdwebManager.Instance.SDK.Wallet.IsConnected();

        if (isCorrect && isConnect)
        {

            Debug.Log($"Sign In = Phone Number : {phoneNumber} / Password: {code}");

            currVerifyCode = code;

            SMSMobileNumberError_Txt.text = "";
            SMSCodeError_Txt.text = "";

            ViewManager.Instance.OpenWaitingView(transform);
            JSBridgeManager.Instance.FirebaseVerifyCode(currVerifyCode,
                                                        gameObject.name,
                                                        nameof(WalletOTPVerifyCallback));
        }     
        else
        {
            SMSCodeError_Txt.text = LanguageManager.Instance.GetText("Invalid Code, Please Try Again.");
        }
    }

    /// <summary>
    /// 錢包OTP驗證回傳
    /// </summary>
    /// <param name="isSuccess">回傳結果(true/false)</param>
    public void WalletOTPVerifyCallback(string isSuccess)
    {
        ViewManager.Instance.CloseWaitingView(transform);

        if (isSuccess == "false")
        {
            //驗證失敗
            SMSCodeError_Txt.text = LanguageManager.Instance.GetText("Invalid Code, Please Try Again.");
            return;
        }

        DataManager.UserLoginType = LoginType.walletUser;
        JSBridgeManager.Instance.ReadDataFromFirebase($"{Entry.Instance.releaseType}/{FirebaseManager.USER_DATA_PATH}{DataManager.UserLoginType}/{currVerifyPhoneNumber}",
                                                      gameObject.name,
                                                      nameof(CheckWalletData));
    }

    /// <summary>
    /// 檢查錢包登入資料
    /// </summary>
    public void CheckWalletData(string jsonData)
    {
        AccountData loginData = FirebaseManager.Instance.OnFirebaseDataRead<AccountData>(jsonData);

        //沒有資料
        if (string.IsNullOrEmpty(loginData.phoneNumber))
        {
            checkDataCallbackFunc = WriteWalletNewUser;
            SetUniqueData();

            return;
        }

        ViewManager.Instance.CloseWaitingView(transform);
        OnIntoLobby();
    }

    /// <summary>
    /// 寫入錢包新用戶資料
    /// </summary>
    private void WriteWalletNewUser()
    {
        //寫入資料
        Dictionary<string, object> dataDic = new()
        {
            { FirebaseManager.PHONE_NUMBER,currVerifyPhoneNumber},
            { FirebaseManager.INVITATION_CODE, currInviteCode },
            { FirebaseManager.USER_ID, currUserId },
            { FirebaseManager.AVATAR_INDEX, 0},
        };
        JSBridgeManager.Instance.WriteDataFromFirebase($"{Entry.Instance.releaseType}/{FirebaseManager.USER_DATA_PATH}{LoginType.walletUser}/{currVerifyPhoneNumber}",
                                                        dataDic,
                                                        gameObject.name,
                                                        nameof(WalletNewUerDataCallback));
    }

    /// <summary>
    /// 錢包登入新用戶寫入資料回傳
    /// </summary>
    /// <param name="isSuccess">回傳結果(true/false)</param>
    public void WalletNewUerDataCallback(string isSuccess)
    {
        OnIntoLobby();
    }

    #endregion

    #region 註冊前設置資料

    /// <summary>
    /// 設置唯一性資料
    /// </summary>
    private void SetUniqueData()
    {
        isStartCheckData = true;
        //邀請碼
        currInviteCode = StringUtils.GenerateRandomString(InviteCodeLength);
        //用戶ID
        currUserId = StringUtils.GenerateRandomString(UserIdLength);

        JSBridgeManager.Instance.CheckUserDataExist(currInviteCode,
                                                    gameObject.name,
                                                    nameof(CheckInviteCodeCallBack));

        JSBridgeManager.Instance.CheckUserDataExist(currUserId,
                                                    gameObject.name,
                                                    nameof(CheckUserIdCallBack));
    }

    /// <summary>
    /// 邀請碼重複檢測回傳
    /// </summary>
    /// <param name="isExist">是否已存在回傳結果(true/false)</param>
    public void CheckInviteCodeCallBack(string isExist)
    {
        if (isExist == "true")
        {
            currInviteCode = StringUtils.GenerateRandomString(InviteCodeLength);
            JSBridgeManager.Instance.CheckUserDataExist(currInviteCode,
                                                        gameObject.name,
                                                        nameof(CheckInviteCodeCallBack));

            return;
        }

        isGetInviteCode = true;
    }

    /// <summary>
    /// UserId重複檢測回傳
    /// </summary>
    /// <param name="isExist">是否已存在回傳結果(true/false)</param>
    public void CheckUserIdCallBack(string isExist)
    {
        if (isExist == "true")
        {
            currUserId = StringUtils.GenerateRandomString(UserIdLength);
            JSBridgeManager.Instance.CheckUserDataExist(currUserId,
                                                        gameObject.name,
                                                        nameof(CheckUserIdCallBack));

            return;
        }

        isGetUserId = true;
    }

    #endregion

    #region 進入大廳

    /// <summary>
    /// 進入大廳
    /// </summary>
    private void OnIntoLobby()
    {
        ViewManager.Instance.CloseWaitingView(transform);

        DataManager.UserLoginPhoneNumber = currVerifyPhoneNumber;
        DataManager.UserLoginPassword = currVerifyPsw;

        LoadSceneManager.Instance.LoadScene(SceneEnum.Lobby);
    }

    #endregion
}
