using UnityEngine;
using System.Collections;

public class TopCloseWindow : WindowBase
{

	public	UIPlayTween NormalGroupTweener;
	WindowBase parentWindow;
	CallBack closeCallBack;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void NormalGroupIn ()
	{
		NormalGroupTweener.playDirection = AnimationOrTween.Direction.Forward;
		NormalGroupTweener.Play (true);

	}
 

	//关联关闭窗口
	public void setCloseWindow (WindowBase window)
	{
		this.parentWindow = window;
	}
	//关联关闭窗口
	public WindowBase getCloseWindow ()
	{
		return parentWindow;
	}

	public void NormalGroupOut ()
	{
		NormalGroupTweener.playDirection = AnimationOrTween.Direction.Reverse;



		NormalGroupTweener.Play (true);

	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			OnClose ();
		}
	}

	public	void OnAnimFinish ()
	{
		if(NormalGroupTweener.playDirection == AnimationOrTween.Direction.Reverse && closeCallBack!=null)
		{
			closeCallBack();
			closeCallBack=null;
		}

	}

	public void setCloseCallBack(CallBack callback){
		closeCallBack = callback;
	}

	public void OnClose ()
	{

		//点击关闭后肯定动画
		NormalGroupOut ();
		parentWindow.DestroyWhenClose = true;
		parentWindow.hideWindow ();

		if (closeCallBack != null) {
			//如果有自定义回调
			return;

		} 

	}

}
