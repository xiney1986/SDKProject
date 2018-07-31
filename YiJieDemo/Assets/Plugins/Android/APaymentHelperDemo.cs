using UnityEngine;
using System.Collections;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System;
using System.IO;  
using System.Net;
using Common;
using System.Globalization;

public class APaymentHelperDemo : MonoBehaviour
{

	//自建服务器 测服
    //	public static string CP_LOGIN_CHECK_URL = "http://testomsdk.xiaobalei.com:5555/cp/user/paylog/checkLoginZijian";
	//自建服务器 正服 
	//  public static string CP_LOGIN_CHECK_URL = "http://testomsdk.xiaobalei.com:5555/cp/user/paylog/checkLoginZijianP";
	//test
  //public static string CP_LOGIN_CHECK_URL = "http://testomsdk.xiaobalei.com:5555/cp/user/paylog/checkLogin";
	//release 
	public static string CP_LOGIN_CHECK_URL = "http://testomsdk.xiaobalei.com:5555/cp/user/paylog/checkLoginP";
	public static string CP_PAY_CHECK_URL = "http://testomsdk.xiaobalei.com:5555/cp/user/paylog/get?orderId=";
	public static string CP_PAY_SYNC_URL = "http://testomsdk.xiaobalei.com:5555/cp/user/paylog/sync";
	public static string PAYTYPE_PAY = "pay";
	public static string PAYTYPE_CHARGE = "charge";
	public static string PAYTYPE_PAY_EXTEND = "payextend";
	public static string PAYTYPE_ROLE = "setData";
	string goodName = "";
	string keyName = "";
	int money = 0;
	string desc = "";
	string paytype = "";
	string	setDataType = "";
	Rect windowRect = new Rect (20, 20, 400, 600);
	string str = "MainCamera";
	public SFOnlineUser user;
	public static Boolean bLogined = false;
	public Boolean isQuerying = false;
	public Boolean isDebug = false;
	public Boolean isPause = false;
	public Boolean isFocus = false;
	public ArrayList orderIds = new ArrayList ();
	struct OderId
	{
		public int retry;
		public string orderId;
	}
	public GUISkin guiSkin;

	
	/**
	 * exit接口用于系统全局退出
	 * @param context      上下文Activity
	 * @param gameObject   游戏场景中的对象
	 * @param listener     退出的监听函数，隶属于gameObject对象的运行时脚本的方法名称，该方法会在收到退出通知后触发
	 * */
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void exit (IntPtr context, string gameObject, string listener);

	/**
	 * onCreate_listener接口用于初始化回调
	 * @param context      上下文Activity
	 * @param gameObject   游戏场景中的对象
	 * @param listener     退出的监听函数，隶属于gameObject对象的运行时脚本的方法名称，该方法会在收到退出通知后触发
	 * */
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void onCreate_listener (IntPtr context, string gameObject, string listener);
	
	/**
	 * login接口用于SDK登陆
	 * @param context      上下文Activity
	 * @param customParams 自定义参数
	 * */
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void login (IntPtr context, string customParams);
	
	/**
	 * logout接口用于SDK登出
	 * @param context      上下文Activity
	 * @param customParams 自定义参数
	 * */
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void logout (IntPtr context, string customParams);
	
	/**
	 * charge接口用于用户触发非定额计费
	 * @param context      上下文Activity
	 * @param gameObject   游戏场景中的对象
	 * @param itemName     虚拟货币名称
	 * @param unitPrice    游戏道具单位价格，单位-分
	 * @param count        商品或道具数量
	 * @param callBackInfo 由游戏开发者定义传入的字符串，会与支付结果一同发送给游戏服务器，
	 *                     游戏服务器可通过该字段判断交易的详细内容（金额角色等）
	 * @param callBackUrl  将支付结果通知给游戏服务器时的通知地址url，交易结束后，
	 * 					   系统会向该url发送http请求，通知交易的结果金额callbackInfo等信息
	 * @param payResultListener  支付监听接口，隶属于gameObject对象的运行时脚本的方法名称，该方法会在收到通知后触发
	 * */
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void charge (IntPtr context, string gameObject, string itemName, int unitPrice,
            int count, string callBackInfo, string callBackUrl, string payResultListener);
	
	/**
	 * pay接口用于用户触发定额计费
	 * @param context      上下文Activity
	 * @param gameObject   游戏场景中的对象
	 * @param unitPrice    游戏道具单位价格，单位-分
	 * @param unitName     虚拟货币名称
	 * @param count        商品或道具数量
	 * @param callBackInfo 由游戏开发者定义传入的字符串，会与支付结果一同发送给游戏服务器，
	 *                     游戏服务器可通过该字段判断交易的详细内容（金额角色等）
	 * @param callBackUrl  将支付结果通知给游戏服务器时的通知地址url，交易结束后，
	 * 					   系统会向该url发送http请求，通知交易的结果金额callbackInfo等信息
	 * @param payResultListener  支付监听接口，隶属于gameObject对象的运行时脚本的方法名称，该方法会在收到通知后触发
	 * */
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void pay (IntPtr context, string gameObject, int unitPrice, string unitName,
            int count, string callBackInfo, string callBackUrl, string payResultListener);
	
	/**
	 * payExtend接口用于用户触发定额计费
	 * @param context      上下文Activity
	 * @param gameObject   游戏场景中的对象
	 * @param unitPrice    游戏道具单位价格，单位-分
	 * @param unitName     虚拟货币名称
	 * @param itemCode     商品ID
	 * @param remain       商品自定义参数
	 * @param count        商品或道具数量
	 * @param callBackInfo 由游戏开发者定义传入的字符串，会与支付结果一同发送给游戏服务器，
	 *                     游戏服务器可通过该字段判断交易的详细内容（金额角色等）
	 * @param callBackUrl  将支付结果通知给游戏服务器时的通知地址url，交易结束后，
	 * 					   系统会向该url发送http请求，通知交易的结果金额callbackInfo等信息
	 * @param payResultListener  支付监听接口，隶属于gameObject对象的运行时脚本的方法名称，该方法会在收到通知后触发
	 * */
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void payExtend (IntPtr context, string gameObject, int unitPrice, string unitName,
		string itemCode, string remain, int count, string callBackInfo, string callBackUrl, string payResultListener);
	
	/**
	 * 部分渠道如UC渠道，要对游戏人物数据进行统计，而且为接入规范，调用时间：在游戏角色登录成功后调用
	 *  public static void setRoleData(Context context, String roleId，
	 *  	String roleName, String roleLevel, String zoneId, String zoneName)
	 *  
	 *  @param context   上下文Activity
	 *  @param roleId    角色唯一标识
	 *  @param roleName  角色名
	 *  @param roleLevel 角色等级
	 *  @param zoneId    区域唯一标识
	 *  @param zoneName  区域名称
     * */
    //setRoleData接口用于部分渠道如UC渠道，要对游戏人物数据进行统计，接入规范：在游戏角色登录成功后
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void setRoleData (IntPtr context, string roleId,
            string roleName, string roleLevel, string zoneId, string zoneName);
	
	//备用接口
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void setData (IntPtr context, string key, string value);
	
	/**
	 *	setLoginListener方法用于设置登陆监听
	 * 初始化SDK
	 *  @param context      上下文Activity
	 *  @param gameObject	游戏场景中的对象，SDK内部完成计费逻辑后，
	 * 						并把计费结果通过Unity的
	 * 						API(com.unity3d.player.UnityPlayer.UnitySendMessage(String gameObject,
	 * 								StringruntimeScriptMethod,Stringargs)
	 * 						通知到Unity，故游戏开发者需要指定一个游戏对象和该对象的运行脚本，用于侦听SDK的计费结果
	 * @param listener      登录的监听函数，隶属于gameObject对象的运行时脚本的方法名称，该方法会在收到通知后触发
	 */
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void setLoginListener (IntPtr context, string gameObject, string listener);

	/**
	 *	extend扩展接口
	 * 扩展接口
	 *  @param context      上下文Activity
	 * @param data          data
	 *  @param gameObject	游戏场景中的对象，
	 * @param listener      扩展的监听函数，隶属于gameObject对象的运行时脚本的方法名称，该方法会在收到通知后触发
	 */
	[DllImport("gangaOnlineUnityHelper")]
	private static extern void extend (IntPtr context, string data,string gameObject, string listener);
	
	/**
	 *	isMusicEnabled方法用于判断SDK是否需要打开游戏声音，目前只有移动基地需要此接口，
	 *  游戏开发者需要根据该返回值，设定游戏背景音乐是否开启
	 *
	 *  @param context      上下文Activity
	 */
	[DllImport("gangaOnlineUnityHelper")]
	private static extern Boolean isMusicEnabled (IntPtr context);
	
	GUI.WindowFunction windowFunction;

	void OnGUI ()
	{
		if (guiSkin) {   
			GUI.skin = guiSkin;
		} 
		windowRect = GUI.Window (0, windowRect, DoMyWindow, str);

	}
	
	string createQueryURL (string orderId)
	{
		if (user == null) {
			return null;
		}
		StringBuilder builder = new StringBuilder ();
		builder.Append (CP_PAY_CHECK_URL);
		builder.Append ("?orderId=");
		builder.Append (orderId);
		return builder.ToString ();
	}
	
	
	//bool startcheck;  


	void DoMyWindow (int windowID)
	{
		if (GUI.Button (new Rect (10, 30, 100, 60), "Login")) {
		

			Debug.Log ("login 0");
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				Debug.Log ("login 1");
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					Debug.Log ("login 2");
					if (bLogined) {
						return;
					}
					login (curActivity.GetRawObject (), "Login");
					
				}
			}
		}
		if (GUI.Button (new Rect (150, 30, 100, 60), "Logout")) {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					Debug.Log ("logout");
					if (!bLogined) {
						return;
					}
					logout (curActivity.GetRawObject (), "");
				}
			}
		}
		if (GUI.Button (new Rect (10, 110, 100, 60), "100Gold")) {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					if (!bLogined) {
						return;
					}
					Debug.Log ("pay");
					pay (curActivity.GetRawObject (), "Main Camera", 100, "金币", 1, "购买金币", CP_PAY_SYNC_URL, "PayResult");
				}
			}
		}
		if (GUI.Button (new Rect (150, 110, 100, 60), "200Gold")) {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					if (!bLogined) {
					    return;
					}
					
				    pay (curActivity.GetRawObject (), "Main Camera", 200, "金币", 1, "购买金币", CP_PAY_SYNC_URL, "PayResult");
				    //Debug.Log ("extend");	
					//SFJSONObject temp =new SFJSONObject();
					//temp.put("callbackcount","2");
					//temp.put("callback1","callback1");
					//temp.put("callback2","callback2");
					//string ss = temp.toInlineString();
					//SFJSONObject temp1 =new SFJSONObject();
					//temp1.put("callbackmap",ss);
					//temp1.put("extendcallback","extendCallback");
					//string ll = temp1.toString();
					//SFJSONObject shareinfo = new SFJSONObject();
					//shareinfo.put("desc", "1sdk.cn");
					//shareinfo.put("title", "易接");
					//shareinfo.put("picture", "");
					//shareinfo.put("icon", "");
					//shareinfo.put("uibg", "");
					//string qq = shareinfo.toInlineString();
					//SFJSONObject data1 = new SFJSONObject();
					//data1.put("type", "shareinfo");
					//data1.put("param", qq);
					//extend (curActivity.GetRawObject(),"data", "Main Camera", ll);


				}
			}
		}
		if (GUI.Button (new Rect (10, 190, 100, 60), "payDialog")) {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					if (!bLogined) {
						return;
					}
			        Application.LoadLevel ("payment");
				}
			}
		}
		if (GUI.Button (new Rect (150, 190, 100, 60), "ChargeDialog")) {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					if (!bLogined) {
						return;
					}
					Application.LoadLevel ("chargement");
					
				}
			}
		}
		
		if (GUI.Button (new Rect (10, 270, 240, 60), "Pay Extend")) {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					if (!bLogined) {
						return;
					}
				    Application.LoadLevel ("extendpayment");
				}
			}
		}
		
		if (GUI.Button (new Rect (10, 350, 240, 60), "Exit")) {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
				   exit(curActivity.GetRawObject (), "Main Camera", "ExitResult");
				}
			}
		}
		
		if (GUI.Button (new Rect (10, 430, 240, 60), "setData")) {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					Application.LoadLevel ("setdata");
				}
			}
		}
	}
	// Use this for initialization
	void Start ()
	{
		Debug.Log ("snowfish Start goodName:"+goodName+" money:"+money.ToString()+" desc:"+desc+" paytype:"+paytype+"keyName:"+keyName);
		windowFunction = DoMyWindow;

	}
	
	// Update is called once per frame
	void Update ()
	{
	    if(Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Escape)){
            // Application.Quit();
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
				   exit(curActivity.GetRawObject (), "Main Camera", "ExitResult");
				}
			}
        }
	}
	void extendCallback(string result){
		Debug.Log ("扩展回调："+result);
		}
	void callback1(string result){
		Debug.Log ("------------callback1=" + result);
		SFJSONObject sfjson = new SFJSONObject (result);
		string tag = (string)sfjson.get ("tag");
		string value = (string)sfjson.get ("value");
		Debug.Log ("tag:" + tag + " value:" + value);
		if (tag.Equals ("success")) {
			Debug.Log ("成功");
				} else {
			Debug.Log ("失败");
				}
	}
	//初始化回调
	void InitResult(string result){
		if(result.Equals ("success"))
		{
			Debug.Log ("成功");
		}else
		{
			Debug.Log ("失败");
		}
		
	}
	void callback2(string result){
		Debug.Log ("------------callback2=" + result);
		SFJSONObject sfjson = new SFJSONObject (result);
		string tag = (string)sfjson.get ("tag");
		string value = (string)sfjson.get ("value");
		Debug.Log ("tag:" + tag + " value:" + value);
		if (tag.Equals ("success")) {
			Debug.Log ("成功");
		} else {
			Debug.Log ("失败");
		}
	}
	// 支付监听函数
	void PayResult (string result)
	{
		Debug.Log ("------------PayResult=" + result);
		SFJSONObject sfjson = new SFJSONObject (result);
		string type = (string)sfjson.get ("result");
		string data = (string)sfjson.get ("data");
	
		if (APaymentHelper.PayResult.PAY_SUCCESS == type) {
			str = "pay result = pay success " + data;
		} else if (APaymentHelper.PayResult.PAY_FAILURE == type) {
			str = "pay result = pay failure" + data;
		} else if (APaymentHelper.PayResult.PAY_ORDER_NO == type) {
			str = "pay result = pay order No" + data; 
		}
	}
	void doLogin(){
		using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					if (bLogined) {
						return;
					}
					
					login (curActivity.GetRawObject (), "Login");
					
				}
			}

	}
    // 登陆监听函数
	void LoginResult (string result)
	{

		Debug.Log ("------------loginResult=" + result);
		SFJSONObject sfjson = new SFJSONObject (result);
		string type = (string)sfjson.get ("result");	
		string customParams = (string)sfjson.get ("customParams");
		if (APaymentHelper.LoginResult.LOGOUT == type) {
			str = "login result = logout" + customParams;
			user = null;
			if (!isDebug) {
				bLogined = false;
			}
			Invoke("doLogin", 0.2f); 
			
		} else if (APaymentHelper.LoginResult.LOGIN_SUCCESS == type) {	
			SFJSONObject userinfo = (SFJSONObject)sfjson.get ("userinfo");
			if (userinfo != null) {
				long id = long.Parse ((string)userinfo.get ("id"));
				string channelId = (string)userinfo.get ("channelid");
				string ChannelUserId = (string)userinfo.get ("channeluserid");
				string UserName = (string)userinfo.get ("username");
				string Token = (string)userinfo.get ("token");
				string ProductCode = (string)userinfo.get ("productcode");
				user = new SFOnlineUser (id, channelId, ChannelUserId,
															UserName, Token, ProductCode);
				Debug.Log ("## id:" + id + " channelId:" + channelId + " ChannelUserId:" + ChannelUserId
					+ " UserName:" + UserName + " Token:" + Token + " ProductCode:" + ProductCode);
			}
				
			str = "login result = login success" + customParams; 
			LoginCheck ();
		} else if (APaymentHelper.LoginResult.LOGIN_FAILED == type) {
			str = "login result = login failed" + customParams; 
		} 
	}
	//ExitResult 退出监听函数
	void ExitResult (string result)
	{
		Debug.Log ("------------ExitResult=" + result);
		SFJSONObject sfjson = new SFJSONObject (result);
		string type = (string)sfjson.get ("result");
		string data = (string)sfjson.get ("data");
	
		if (APaymentHelper.ExitResult.SDKEXIT == type) {
			//SDK退出
			if(data.Equals("true"))
			{
				Application.Quit();
			} 
			else if(data.Equals("false"))
			{
				
			}
		} else if (APaymentHelper.ExitResult.SDKEXIT_NO_PROVIDE == type) {
			//游戏自带退出界面
			Application.Quit();
		}
	}
	
	void Awake ()
	{
		goodName = PlayerPrefs.GetString ("goodName", "");   
		money = PlayerPrefs.GetInt ("money", 0);   
		string itemCode =  PlayerPrefs.GetString ("goodid", "");   
		string remain =  PlayerPrefs.GetString ("remain", "");   
		Debug.Log ("itemCode=" +itemCode+" remain="+remain);
		bLogined = PlayerPrefs.GetInt ("login", 0) == 0 ? false : true;
		keyName = PlayerPrefs.GetString ("keyName",""); 
		if(goodName.Equals("") || money == 0)
		{
		   desc = ""; 
		   paytype = ""; 
		} 
		else 
		{
		   desc = PlayerPrefs.GetString ("desc", ""); 
		   paytype = PlayerPrefs.GetString ("paytype", ""); 
		
		}
		if(keyName.Equals("")){
			setDataType = ""; 
		}else{
			setDataType= PlayerPrefs.GetString ("setDataType", ""); 
		}

		
		PlayerPrefs.DeleteAll();
		
		if (Application.platform == RuntimePlatform.Android) {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
				   setLoginListener(curActivity.GetRawObject (), "Main Camera", "LoginResult");
					Debug.Log ("Awake setDataType:" + setDataType);
					if (bLogined == true) 
					{
						Debug.Log ("Alogin result = login success" );
						str = "login result = login success";
						if (paytype.Equals (PAYTYPE_PAY)) {
							str = "PAYTYPE_PAY";
							pay (curActivity.GetRawObject (), "Main Camera", money, goodName, 1, desc, CP_PAY_SYNC_URL, "PayResult");
						} else if (paytype.Equals (PAYTYPE_CHARGE)) {
							str = "PAYTYPE_CHARGE";
							charge (curActivity.GetRawObject (), "Main Camera", goodName, money, 1, desc, CP_PAY_SYNC_URL, "PayResult");
						} else if(paytype.Equals (PAYTYPE_PAY_EXTEND)){
							str = "PAYTYPE_PAY_EXTEND";
							payExtend (curActivity.GetRawObject (), "Main Camera", money, goodName, itemCode, remain, 
								1, desc, CP_PAY_SYNC_URL, "PayResult"); 
						}else if(setDataType.Equals(PAYTYPE_ROLE)){
							str = "PAYTYPE_ROLE";
							SFJSONObject gameinfo = new SFJSONObject();
							gameinfo.put("roleId", "1");//当前登录的玩家角色ID，必须为数字
							gameinfo.put("roleName", "猎人");//当前登录的玩家角色名，不能为空，不能为null
							gameinfo.put("roleLevel", "100");//当前登录的玩家角色等级，必须为数字，且不能为0，若无，传入1
							gameinfo.put("zoneId", "1");//当前登录的游戏区服ID，必须为数字，且不能为0，若无，传入1
							gameinfo.put("zoneName", "阿狸一区");//当前登录的游戏区服名称，不能为空，不能为null
							gameinfo.put("balance", "0");   //用户游戏币余额，必须为数字，若无，传入0
							gameinfo.put("vip", "1");            //当前用户VIP等级，必须为数字，若无，传入1
							gameinfo.put("partyName", "无帮派");//当前角色所属帮派，不能为空，不能为null，若无，传入“无帮派”
							gameinfo.put("roleCTime", "21322222");    //单位为秒，创建角色的时间
							gameinfo.put("roleLevelMTime", "54456556");  //单位为秒，角色等级变化时间,如果没有就传-1
							setData(curActivity.GetRawObject (),keyName,gameinfo.toString());//创建新角色时调用，必接接口
						}
					}	
					Debug.Log ("user had payed str:" + str);
				
				}
			}
		}
		isPause = false;

		isFocus = false;
	}
	
	void LoginCheck ()
	{
		if (isDebug == true) {
			bLogined = true;
			return;
		}

		string url = createLoginURL ();
		Debug.Log ("LoginCheck url:" + url);
		if (url == null)
			return;
		string result = executeHttpGet (url);
		Debug.Log ("LoginCheck result:" + result);
		if (result == null || !(result == "SUCCESS")) {
			bLogined = false;

		} else {
			bLogined = true;
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {

					setRoleData (curActivity.GetRawObject (), "1", "hunter", "30", "1", "1");


					SFJSONObject gameinfo = new SFJSONObject();
					gameinfo.put("roleId", "1");//当前登录的玩家角色ID，必须为数字
					gameinfo.put("roleName", "猎人");//当前登录的玩家角色名，不能为空，不能为null
					gameinfo.put("roleLevel", "100");//当前登录的玩家角色等级，必须为数字，且不能为0，若无，传入1
					gameinfo.put("zoneId", "1");//当前登录的游戏区服ID，必须为数字，且不能为0，若无，传入1
					gameinfo.put("zoneName", "阿狸一区");//当前登录的游戏区服名称，不能为空，不能为null
					gameinfo.put("balance", "0");   //用户游戏币余额，必须为数字，若无，传入0
					gameinfo.put("vip", "1");            //当前用户VIP等级，必须为数字，若无，传入1
					gameinfo.put("partyName", "无帮派");//当前角色所属帮派，不能为空，不能为null，若无，传入“无帮派”
					gameinfo.put("roleCTime", "21322222");    //单位为秒，创建角色的时间
					gameinfo.put("roleLevelMTime", "54456556");  //单位为秒，角色等级变化时间,如果没有就传-1
					setData(curActivity.GetRawObject (),"createrole",gameinfo.toString());//创建新角色时调用，必接接口
					setData(curActivity.GetRawObject (),"levelup",gameinfo.toString());//玩家升级角色时调用，必接接口
					setData(curActivity.GetRawObject(),"enterServer",gameinfo.toString());//选择服务器进入时调用,必接接口
					setData(curActivity.GetRawObject(),"loginGameRole",gameinfo.toString());////这个接口只有接uc的时候才会用到和setRoleData一样的功能，但是两个放在一起不会出现冲突,必接接口
				}
			}
			

		}
			
	}

	private string createLoginURL ()
	{
		if (user == null) {
			return null;
		}
		StringBuilder builder = new StringBuilder ();
		builder.Append (CP_LOGIN_CHECK_URL);

		builder.Append ("?app=");
		builder.Append (ToUrlEncode (user.getProductCode ()));//(Base64.EncodeBase64(user.getProductCode()));
		builder.Append ("&sdk=");
		builder.Append (ToUrlEncode (user.getChannelId ()));//(Base64.EncodeBase64(user.getChannelId()));
		builder.Append ("&uin=");
		builder.Append (ToUrlEncode (user.getChannelUserId ()));//(Base64.EncodeBase64(user.getChannelUserId()));
		builder.Append ("&sess=");
		builder.Append (ToUrlEncode (user.getToken ()));//(Base64.EncodeBase64(user.getToken()));
		return builder.ToString ();
	}

	public void addOrderId (string orderId)
	{
		if (isDebug) {
			return;
		}
		OderId id = new OderId ();
		id.retry = 0;
		id.orderId = orderId;
		orderIds.Add (id);
		if (isQuerying)
			return;

		query ();
	}

	public void query ()
	{
		doQuery ();
	}

	public void doQuery ()
	{
		if (orderIds != null) {//orderIds != null
			OderId oderId = (OderId)orderIds [0];
			if ((object)oderId != null) {
				if (oderId.retry > 10) {
					orderIds.RemoveAt (0);
				}
				oderId = (OderId)orderIds [0];
				if (oderId.Equals (null))
					return;

				oderId.retry++;
				String str = createQueryURL (oderId.orderId);
				if (str == null) {
					String result = "SUCCESS";//executeHttpGet(str);
					if (result == null || !(result == "SUCCESS")) {
					} else {
						orderIds.RemoveAt (0);
						Debug.Log ("user had payed oderId:" + oderId.orderId);
					
					}
				}

				if (orderIds.Count != 0) {
					query ();
				}
			}
		}
	}

	public static string executeHttpGet (string str)
	{
		WebClient myWebClient = new WebClient ();  
		//myWebClient.Headers.Add("Content-Type", "multipart/form-data; ");  
		byte[] b = myWebClient.DownloadData (str);  
		return (Encoding.UTF8.GetString (b));
	}
	public string HexToStr (string mHex) // 返回十六进制代表的字符串 
	{ 
		mHex = mHex.Replace (" ", ""); 
		if (mHex.Length <= 0)
			return ""; 
		byte[] vBytes = new byte[mHex.Length / 2]; 
		for (int i = 0; i < mHex.Length; i += 2) 
			if (!byte.TryParse (mHex.Substring (i, 2), NumberStyles.HexNumber, null, out vBytes [i / 2])) 
				vBytes [i / 2] = 0; 
		return ASCIIEncoding.Default.GetString (vBytes); 
	} /* HexToStr */ 
	public string StrToHex (string mStr) //返回处理后的十六进制字符串 
	{ 
		return BitConverter.ToString (ASCIIEncoding.Default.GetBytes (mStr)).Replace ("-", ""); 
	}
	
	public static string ToUrlEncode (string strCode)
	{ 
		StringBuilder sb = new StringBuilder (); 
		byte[] byStr = System.Text.Encoding.UTF8.GetBytes (strCode); //默认是System.Text.Encoding.Default.GetBytes(str) 
		System.Text.RegularExpressions.Regex regKey = new System.Text.RegularExpressions.Regex ("^[A-Za-z0-9]+$"); 
		for (int i = 0; i < byStr.Length; i++) { 
			string strBy = Convert.ToChar (byStr [i]).ToString (); 
			if (regKey.IsMatch (strBy)) { 
				//是字母或者数字则不进行转换  
				sb.Append (strBy); 
			} else { 
				sb.Append (@"%" + Convert.ToString (byStr [i], 16)); 
			} 
		} 
		return (sb.ToString ()); 
	}
}
