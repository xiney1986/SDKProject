using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using quicksdk;

#if QUICK
public class SdkManager : QuickSDKListener
#else
public class SdkManager : MonoBehaviour
#endif
{
    public static SdkManager INSTANCE;

    public static List<json_Goods> jsonGoodsList;

    public static string URL;

    public static string APP_VERSION;

    public static string SERVERLIST_VERSION;

    public static string RESOURCES_VERSION;

    public static string CDN;

    public static string UIN;

    public bool IS_SDK;

#if UNITY_ANDROID && !UNITY_EDITOR
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
#endif

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        INSTANCE = this;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IS_SDK)
        {
#if YIJIE
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    setLoginListener(curActivity.GetRawObject(), "SdkManager", "LoginResult");
                }
            }
#elif QUICK
            QuickSDK.getInstance().setListener(this);
#endif
        }
#endif
    }

    internal void CreateResult(string message)
    {
        if (message == CreateStatus.SUCCESS)
        {

        }
        else
        {

        }
    }

    internal void LoginResult(string message)
    {
        Dictionary<string, object> dic = MiniJSON.Json.Deserialize(message) as Dictionary<string, object>; ;
        string type = (string)dic["result"];
        if (type == LoginStatus.LOGIN_SUCCESS)
        {
            Dictionary<string, object> userInfoDic = (Dictionary<string, object>)dic["userinfo"];
            string channelid = (string)userInfoDic["channelid"];
            string channeluserid = (string)userInfoDic["channeluserid"];
            string token = (string)userInfoDic["token"];
            string productcode = (string)userInfoDic["productcode"];
            UIN = channeluserid;
            URL = "URL?uin=" + channeluserid + "&sdk=" + channelid + "&app=" + productcode + "&sess=" + token;
        }
        else if (type == LoginStatus.LOGOUT)
        {

        }
        else
        {
            Login();
        }
    }

    internal void ExitResult(string message)
    {
        Dictionary<string, object> dic = MiniJSON.Json.Deserialize(message) as Dictionary<string, object>; ;
        string type = (string)dic["result"];
        string data = (string)dic["data"];
        if (ExitStatus.SDKEXIT == type)
        {
            //SDK退出
            if (data.Equals("true"))
            {
                Application.Quit();
            }
            else if (data.Equals("false"))
            {

            }
        }
        else if (ExitStatus.SDKEXIT_NO_PROVIDE == type)
        {
            UiManager.Instance.openDialogWindow<ExitWindow>();
        }
    }

    public void Login(string customParams = null)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IS_SDK)
        {
#if YIJIE
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    login(curActivity.GetRawObject(), customParams);
                }
            }
#elif QUICK
            QuickSDK.getInstance().login();
#endif
        }
#endif
    }

    public void Logout(string customParams = null)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IS_SDK)
        {
#if YIJIE
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    logout(curActivity.GetRawObject(), customParams);
                }
            }
#elif QUICK
            QuickSDK.getInstance().logout();
#endif
        }
#endif
    }

    public void SetRoleData(string roleId, string roleName, string roleLevel, string zoneId, string zoneName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IS_SDK)
        {
#if YIJIE
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    setRoleData(curActivity.GetRawObject(), roleId, roleName, roleLevel, zoneId, zoneName);
                }
            }
#endif
        }
#endif
    }

    public void SetData(string actionName, Dictionary<string, string> dic)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IS_SDK)
        {
#if YIJIE
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    setData(curActivity.GetRawObject(), actionName, MiniJSON.Json.Serialize(dic));
                }
            }
#endif
        }
#endif
    }

    public void CreateRole(User user)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IS_SDK)
        {
#if QUICK
           	QuickSDK.getInstance ().createRole(user.ToRoleInfo());
#endif
        }
#endif
    }

    public void EnterGame()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IS_SDK)
        {
#if QUICK
           	QuickSDK.getInstance ().enterGame(UserManager.Instance.self.ToRoleInfo());
#endif
        }
#endif
    }

    public void UpdateRole()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IS_SDK)
        {
#if QUICK
           	QuickSDK.getInstance ().updateRole(UserManager.Instance.self.ToRoleInfo());
#endif
        }
#endif
    }

    public void Exit()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
#if YIJIE
        if (IS_SDK)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    exit(curActivity.GetRawObject(), "SdkManager", "ExitResult");
                }
            }
        }
        else
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("result","1");
            dic.Add("data","false");
            ExitResult(MiniJSON.Json.Serialize(dic));
        }
#elif QUICK
        if (IS_SDK)
        {
            if (QuickSDK.getInstance().isChannelHasExitDialog())
            {
                QuickSDK.getInstance().exit();
            }
            else
            {
                UiManager.Instance.openDialogWindow<ExitWindow>();
            }
        }
        else
        {
            Application.Quit();
        }
#endif
#endif
    }

    public void Pay(string goodsId, int money)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IS_SDK)
        {
#if YIJIE
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    pay(curActivity.GetRawObject(), "SdkManager", money, goodsId, 1, ServerManagerment.Instance.lastServer.sid + "|" + goodsId + "|" + SdkManager.UIN, ServerManagerment.Instance.lastServer.payUrl, "PayResult");
                }
            }
#endif
        }
#endif
    }

    //假的测试用
    public static void createGoodList()
    {
        jsonGoodsList = new List<json_Goods>();

        json_Goods goods = new json_Goods();
        goods.id = "1";
        goods.name = "6元购买1800钻石";
        goods.amount = "1";
        goods.rate = "1800";
        goods.desc1 = "首次充值额外获得3600钻石";
        goods.desc2 = "goodsIcon_1";
        jsonGoodsList.Add(goods);

        goods = new json_Goods();
        goods.id = "2";
        goods.name = "30元购买9000钻石";
        goods.amount = "30";
        goods.rate = "9000";
        goods.desc1 = "首次充值额外获得18000钻石";
        goods.desc2 = "goodsIcon_2";
        jsonGoodsList.Add(goods);

        goods = new json_Goods();
        goods.id = "3";
        goods.name = "98元购买29400钻石";
        goods.amount = "98";
        goods.rate = "29400";
        goods.desc1 = "首次充值额外获得58800钻石";
        goods.desc2 = "goodsIcon_2";
        jsonGoodsList.Add(goods);

        goods = new json_Goods();
        goods.id = "4";
        goods.name = "198元购买59400钻石";
        goods.amount = "198";
        goods.rate = "59400";
        goods.desc1 = "首次充值额外获得118800钻石";
        goods.desc2 = "goodsIcon_3";
        jsonGoodsList.Add(goods);

        goods = new json_Goods();
        goods.id = "5";
        goods.name = "328元购买98400钻石";
        goods.amount = "328";
        goods.rate = "98400";
        goods.desc1 = "首次充值额外获得196800钻石";
        goods.desc2 = "goodsIcon_3";
        jsonGoodsList.Add(goods);

        goods = new json_Goods();
        goods.id = "6";
        goods.name = "648元购买194400钻石";
        goods.amount = "648";
        goods.rate = "194400";
        goods.desc1 = "首次充值额外获得388800钻石";
        goods.desc2 = "goodsIcon_4";
        jsonGoodsList.Add(goods);

        goods = new json_Goods();
        goods.id = "7";
        goods.name = "奖励丰厚，欢迎选购";
        goods.amount = "30";
        goods.rate = "31";
        goods.desc1 = "购买月卡不参与首冲等充值活动";
        goods.desc2 = "goodsIcon_5";
        goods.type = 1;
        jsonGoodsList.Add(goods);

        goods = new json_Goods();
        goods.id = "8";
        goods.name = "奖励丰厚，欢迎选购";
        goods.amount = "6";
        goods.rate = "31";
        goods.desc1 = "购买周卡不参与首冲等充值活动";
        goods.desc2 = "goodsIcon_6";
        goods.type = 2;
        jsonGoodsList.Add(goods);

    }

    public IEnumerator LoadUrl(string url, Action<LoadResult> action, int timeOut = 10)
    {
        WWW www = new WWW(url);
        float time = Time.realtimeSinceStartup;
        while (true)
        {
            if (www.isDone)
            {
                string error = www.error;
                if (string.IsNullOrEmpty(error))
                {
                    LoadResult result = new LoadResult(LoadStatus.OK, www);
                    action(result);
                    yield break;
                }
                else
                {
                    LoadResult result = new LoadResult(LoadStatus.ERROR, www);
                    action(result);
                    yield break;
                }
            }
            else if (Time.realtimeSinceStartup - time >= timeOut && timeOut != 0)
            {
                LoadResult result = new LoadResult(LoadStatus.TIMEOUT, www);
                action(result);
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }

#if QUICK
    public override void onInitSuccess()
    {
    }

    public override void onInitFailed(ErrorMsg message)
    {
    }

    public override void onLoginSuccess(UserInfo userInfo)
    {
        URL = "URL?uid=" + userInfo.uid + "&channel_code=" + QuickSDK.getInstance().channelType() + "&token=" + userInfo.token;
        Debug.Log(URL);
    }

    public override void onSwitchAccountSuccess(UserInfo userInfo)
    {
    }

    public override void onLoginFailed(ErrorMsg errMsg)
    {
        Login();
    }

    public override void onLogoutSuccess()
    {
    }

    public override void onPaySuccess(PayResult payResult)
    {
    }

    public override void onPayFailed(PayResult payResult)
    {
    }

    public override void onPayCancel(PayResult payResult)
    {
    }

    public override void onExitSuccess()
    {
        QuickSDK.getInstance().exitGame();
    }
#endif

    public struct LoadResult
    {
        public int state;
        public WWW www;

        public LoadResult(int state, WWW www)
        {
            this.state = state;
            this.www = www;
        }
    }

    public class LoadStatus
    {
        public static int OK = 0;
        public static int ERROR = 1;
        public static int TIMEOUT = 2;
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

    internal class ExitStatus
    {
        /** exit success*/
        public const String SDKEXIT = "0";


        /**No Exiter Provide*/
        public const String SDKEXIT_NO_PROVIDE = "1";
    }
}
