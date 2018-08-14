using UnityEngine;
using System.Collections;
using System.Text;

/**
 * 信息提示窗口
 * @author 汤琦
 * */
public enum MessageAlignType
{
	left, //左对齐
	right,//右对齐
	center//居中
}
public class MessageWindow : WindowBase
{
	//按钮位置
	public Transform pointL;//左
	public Transform pointR;//右
	public Transform pointM;//中
	public ButtonBase button1;
	public ButtonBase button2;
	public ButtonBase button3;
	public UILabel button3Label;
	public UILabel content;//信息内容
	public GameObject closeButton;
	private float time;
	private bool isSystemMsg;
	public MessageHandle msg;
	CallBackMsg callback;
	bool showCloseButton = false;
	
	public override void  OnAwake ()
	{
		msg = new MessageHandle ();
	}

	protected override void begin ()
	{
		base.begin ();
		
		//下面解决连续的对话框弹出无法回调问题
		GameManager.Instance.setMsgCallback (callback);
		MaskWindow.UnlockUI ();
	}

	public void initWindow (int buttonNum, string button1Name, string button2Name, string content, CallBackMsg call, bool isSystemMsg)
	{
		callback = call;
		initButton (buttonNum, button1Name, button2Name);
		SetAlignType (MessageAlignType.center);
		initInformation (content);
		this.isSystemMsg = isSystemMsg;
		if (this.isSystemMsg)
			gameObject.GetComponent<UIPanel> ().depth = 10000;

	}
	public void initWindow (int buttonNum, string button1Name, string button2Name, string content, CallBackMsg call)
	{
		callback = call;
		initButton (buttonNum, button1Name, button2Name);
		SetAlignType (MessageAlignType.center);
		initInformation (content);
	}
	public void initWindow (int buttonNum, string button1Name, string button2Name, string content, CallBackMsg call,MessageAlignType alignType)
	{
		callback = call;
		initButton (buttonNum, button1Name, button2Name);
		SetAlignType (alignType);
		initInformation (content);
	}
	public void initWindow (int buttonNum, string button1Name, string button2Name, string content,bool showCloseButton, CallBackMsg call,MessageAlignType alignType)
	{
		this.showCloseButton = showCloseButton;
		closeButton.SetActive(false);
		if(showCloseButton)closeButton.SetActive(true);
		callback = call;
		initButton (buttonNum, button1Name, button2Name);
		SetAlignType (alignType);
		initInformation (content);
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
 
		//这里不用协程等待的话，回调里又有对话框调用就出错
		GameManager.Instance.StartCoroutine (GameManager.Instance.DoMsgCallback (msg));
		button1.gameObject.SetActive (false);
		button2.gameObject.SetActive (false);
//		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "button_1") 
        {
			msg.buttonID = MessageHandle.BUTTON_LEFT;
			msg.msgEvent = msg_event.dialogCancel;
		} 
        else if (gameObj.name == "button3") 
        { 
			if (GuideManager.Instance.isEqualStep (22011000)) 
            {
				GuideManager.Instance.doGuide ();
				GuideManager.Instance.guideEvent (); 
			}
			msg.buttonID = MessageHandle.BUTTON_MIDDLE;
			msg.msgEvent = msg_event.dialogOK;
		} 
        else if (gameObj.name == "close") 
        {
			msg.buttonID = MessageHandle.BUTTON_MIDDLE;
			msg.msgEvent = msg_event.dialogCancel;
		}
		else 
        {
			msg.buttonID = MessageHandle.BUTTON_RIGHT;
			msg.msgEvent = msg_event.dialogOK;
		}
		finishWindow ();
		
	}
	
	private void initButton (int buttonNum, string button1Name, string button2Name)
	{
		if (buttonNum == 1) {
			button1.gameObject.SetActive (true);
			button2.gameObject.SetActive (false);
			button3.gameObject.SetActive (false);
			button3Label.gameObject.SetActive (false);
			button1.transform.position = pointM.transform.position;
			button1.textLabel.text = button1Name;
		} else if (buttonNum == 3) {
			button1.gameObject.SetActive (false);
			button2.gameObject.SetActive (false);
			button3.gameObject.SetActive (true);
			button3Label.gameObject.SetActive (true);
		} else {
			button1.gameObject.SetActive (true);
			button2.gameObject.SetActive (true);
			button3.gameObject.SetActive (false);
			button3Label.gameObject.SetActive (false);
			button1.transform.position = pointL.transform.position;
			button2.transform.position = pointR.transform.position;
			button1.textLabel.text = button1Name;
			button2.textLabel.text = button2Name;
		}
	}

	private void initInformation (string information)
	{
		content.text = information;
	}
	
	void Update ()
	{
		if (button3Label.gameObject.activeSelf) {
			float offset = Mathf.Sin (time * 6); 
			button3Label.alpha = sin ();
		}
	}
	public  void SetAlignType(MessageAlignType alignType){
		switch (alignType) {
		case MessageAlignType.left:
			content.pivot = UIWidget.Pivot.Left;
			break;
		case MessageAlignType.right:
			content.pivot = UIWidget.Pivot.Right;
			break;
		default:
			content.pivot = UIWidget.Pivot.Center;
			break;

		}

	}

	public static void ShowAlert (string msg)
	{
		ShowAlert (msg, null);
	}

	public static void ShowAlert (string msg, CallBackMsg callback)
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, msg, callback,MessageAlignType.center);
		});
	}

	/// <summary>
	/// 显示messageWindow
	/// </summary>
	/// <param name="msg">Message.</param>
	/// <param name="alignType">对齐方式(新增参数)</param>
	public static void ShowAlert(string msg , MessageAlignType alignType){
		ShowAlert (msg, null, alignType);

	}
	/// <summary>
	/// 显示messageWindow
	/// </summary>
	/// <param name="msg">Message.</param>
	/// <param name="callback">回调函数</param>
	/// <param name="alignType">对齐方式(新增参数)</param>
	public static void ShowAlert (string msg, CallBackMsg callback,MessageAlignType alignType)
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, msg, callback ,alignType);
		});
	}

	public static void ShowConfirm (string msg, CallBackMsg callback , bool dialogCloseUnlockUI)
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), msg, callback);
			win.dialogCloseUnlockUI = dialogCloseUnlockUI;
		});
	}
	public static void ShowConfirm (string msg, CallBackMsg callback)
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), msg, callback);
		});
	}

	public static void ShowConfirm (string msg, CallBackMsg callback,MessageAlignType alignType)
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), msg, callback ,alignType);
		});
	}

	public static void ShowRecharge (string msg)
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((MessageWindow win) => {
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0315"), msg, (eventMsg) => {
				if (eventMsg.buttonID == MessageHandle.BUTTON_RIGHT) {
					UiManager.Instance.openWindow<rechargeWindow> (); 
                    MaskWindow.LockUI();
				}
			});
		});
	}

	public static void ShowRecharge (string msg,MessageAlignType alignType)
	{
		UiManager.Instance.openDialogWindow<MessageWindow> ((MessageWindow win) => {
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0315"), msg, (eventMsg) => {
				if (eventMsg.buttonID == MessageHandle.BUTTON_RIGHT) {
					UiManager.Instance.openWindow<rechargeWindow> (); 
				}
			},alignType);
		});
	}

	/// <summary>
	/// 创建消息信息数据
	/// </summary>
	/// <param name="messageList">消息列表</param>
	public static string createMessageInfo(ArrayList messageList) {
		if (messageList == null||messageList.Count==0)
			return null;
		StringBuilder messageBuilder = new StringBuilder ();
		for (int i=0; i<messageList.Count; i++) {
			if(messageBuilder.Length>0) messageBuilder.Append("\n");
			if (messageList.Count > 1) {
				messageBuilder.Append((i+1)+".");
			}
			messageBuilder.Append(messageList[i].ToString());
		}
		return messageBuilder.ToString ();
	}
	/// <summary>
	/// 递归执行消息列表提示框
	/// </summary>
	/// <param name="messageList">Message list.</param>
	public static void doCreateMessageFun(ArrayList messageList,CallBack callBack){
		if (messageList == null||callBack==null) {
			return;
		}
		if (messageList.Count > 0) {
			UiManager.Instance.createMessageWindowByTwoButton(messageList[0].ToString(),(MessageHandle msg)=>{
				messageList.RemoveAt(0);
				if(msg.buttonID == MessageHandle.BUTTON_LEFT)
				{
					MaskWindow.UnlockUI();
					return;
				}
				doCreateMessageFun(messageList,callBack);
			});
		}
		else{
			callBack();
			callBack=null;
		}
	}
}
