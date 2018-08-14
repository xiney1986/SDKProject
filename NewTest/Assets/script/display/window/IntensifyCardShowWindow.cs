using UnityEngine;
using System.Collections;

/**
 * 通用强化预览窗口
 * @author 陈世惟
 * */
public class IntensifyCardShowWindow : WindowBase {

	public CardAttrItem oldRole;
	public CardAttrItem newRole;

	CallBack callBack;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI();
	}

	/// <summary>
	/// 初始化卡片预览信息
	/// </summary>
	/// <param name="oldCard">旧卡.</param>
	/// <param name="newCard">新卡.</param>
	public void initUI (Card oldCard, Card newCard, CallBack _callBack)
	{
		this.callBack = _callBack;
		oldRole.initUI (oldCard,Colors.WHITE);
		newRole.initUI (newCard,Colors.GREEN);
	}

	/// <summary>
	/// 打开预览窗口方法
	/// </summary>
	/// <param name="oldCard">旧卡.</param>
	/// <param name="newCard">新卡.</param>
	public static void Show (Card oldCard, Card newCard, CallBack _callBack)
	{
		UiManager.Instance.openWindow<IntensifyCardShowWindow> ((win)=>{
			win.initUI (oldCard, newCard, _callBack);
		});
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "okButton") {
			finishWindow ();
			if (callBack != null) {
				callBack ();
			}
		}
		else if (gameObj.name == "close") {
			finishWindow ();
		}
	}
}
