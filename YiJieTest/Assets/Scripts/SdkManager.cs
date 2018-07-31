using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class SdkManager : MonoBehaviour
{
    public static SdkManager INSTANCE;

    void Awake()
    {
        INSTANCE = this;
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                setLoginListener(curActivity.GetRawObject(), "Login", "LoginResult");
            }
        }
    }

    public void CreateResult(string message)
    {
        Debug.Log("CreateResult=" + message);
    }

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void login(IntPtr context, string customParams);

    [DllImport("gangaOnlineUnityHelper")]
    private static extern void setLoginListener(IntPtr context, string gameObject, string listener);

    public void Login(string customParams)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                login(curActivity.GetRawObject(), "Login");
            }
        }
    }
}
