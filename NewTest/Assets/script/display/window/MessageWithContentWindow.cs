using UnityEngine;
using System.Collections;
using System.Text;

/// <summary>
/// 带动态容器的消息窗口
/// </summary>
public class MessageWithContentWindow : WindowBase
{
	public ButtonBase button1;
	public ButtonBase button2;
	public UILabel content;//信息内容
	public UISprite contentBg;//信息背景
	private float time;
	private bool isSystemMsg;
	public MessageHandle msg;
	public GameObject cardPrefab;
	/**卡片容器*/
	public MessageWithContentCardContent cardContent;
	/**卡片链表*/
	ArrayList cardList = null;
	CallBackMsg callback;

	public override void  OnAwake ()
	{
		msg = new MessageHandle ();
	}
	
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	public void initWindow (string content,ArrayList cardList,CallBackMsg back){
		callback = back;
		cardContent.reLoad (cardList);
	}

	/// <summary>
	/// 更新卡片容器
	/// </summary>
	public void updateCardContent ()
	{
		cardContent.reLoad (cardList);
	}
	public override void DoDisable ()
	{
		base.DoDisable ();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonCancel") {
			destroyAllCards();
			finishWindow ();
			MaskWindow.UnlockUI ();
		}
		else if (gameObj.name == "buttonComfirm") {
			msg.buttonID = MessageHandle.BUTTON_RIGHT;
			msg.msgEvent = msg_event.dialogOK;
			StartCoroutine (Utils.DelayRun (() => {
			}, 1.3f));
			if(callback != null)
				callback(msg);
			destroyAllCards();
			finishWindow();
		}
		
	}
	///<summary>
	/// 销毁所有卡片
	/// </summary>
	private void destroyAllCards()
	{
		if(cardList != null)
			cardList.RemoveRange(0,cardList.Count);
	}
}
