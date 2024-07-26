using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FirebaseManager : UnitySingleton<FirebaseManager>
{
    private string apiKey = "AIzaSyDdhkPzYHHJndpTV3QARMdToXXws-n89X0"; // 確保這是你的 Firebase Web API 鑰匙

    public void SendOTP(string phoneNumber)
    {
        StartCoroutine(SendVerificationCodeCoroutine(phoneNumber));
    }

    private IEnumerator SendVerificationCodeCoroutine(string phoneNumber)
    {
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendVerificationCode?key={apiKey}";
        string json = "{\"phoneNumber\":\"" + phoneNumber + "\",\"recaptchaToken\":\"RECAPTCHA_TOKEN\"}"; // 添加 recaptchaToken

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("OTP sent successfully: " + request.downloadHandler.text);
        }
    }
}
