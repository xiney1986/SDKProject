using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class SdkManager : MonoBehaviour
{
    public static SdkManager INSTANCE;

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void login(IntPtr context, string customParams);

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void setLoginListener(IntPtr context, string gameObject, string listener);

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void logout(IntPtr context, string customParams);

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void exit(IntPtr context, string gameObject, string listener);

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void pay(IntPtr context, string gameObject, int unitPrice, string unitName, int count, string callBackInfo, string callBackUrl, string payResultListener);

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void charge(IntPtr context, string gameObject, string itemName, int unitPrice, int count, string callBackInfo, string callBackUrl, string payResultListener);

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void setRoleData(IntPtr context, string roleId, string roleName, string roleLevel, string zoneId, string zoneName);

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void setData(IntPtr context, string key, string value);

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void extend(IntPtr context, string data, string gameObject, string listener);

    void Awake()
    {
        INSTANCE = this;
#if UNITY_ANDROID&&!UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                setLoginListener(curActivity.GetRawObject(), "SdkManager", "LoginResult");
            }
        }
#endif
    }

    internal void CreateResult(string message)
    {
        Debug.Log("message=" + message);
        if (message == CreateStatus.SUCCESS)
        {

        }
        else
        {

        }
    }

    internal void LoginResult(string message)
    {
        Debug.Log("LoginResult=" + message);
        Dictionary<string, object> dic = MiniJSON.Json.Deserialize(message) as Dictionary<string, object>; ;
        string type = (string)dic["result"];
        if (type == LoginStatus.LOGIN_SUCCESS)
        {
            Dictionary<string, object> userInfoDic = (Dictionary<string, object>)dic["userinfo"];
            string channelid = (string)userInfoDic["channelid"];
            string channeluserid = (string)userInfoDic["channeluserid"];
            string token = (string)userInfoDic["token"];
            string productcode = (string)userInfoDic["productcode"];
            string param = "channelid=" + channelid + "&productcode=" + productcode + "&channeluserid=" + channeluserid + "&token=" + token;
            StartCoroutine(LoginServer(param));
        }
        else if (type == LoginStatus.LOGOUT)
        {

        }
        else
        {
            Login();
        }
    }

    IEnumerator LoginServer(string param)
    {
        WWW www = new WWW("http://192.168.2.141:8080?" + param);
        yield return www;
        if (www.isDone)
        {
            Debug.Log(www.text);
        }
    }

    public void Login(string customParams = null)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                login(curActivity.GetRawObject(), customParams);
            }
        }
    }

    public void Logout(string customParams = null)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                logout(curActivity.GetRawObject(), customParams);
            }
        }
    }

    internal class CreateStatus
    {
        public const string SUCCESS = "success";
        public const string FAIL = "fail";
    }

    internal class LoginStatus
    {
        public const string LOGOUT = "0";
        public const string LOGIN_SUCCESS = "1";
        public const string LOGIN_FAILED = "2";
    }
}
