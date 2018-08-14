using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuyTimesTipWindow : WindowBase
{
	public CallBack<bool> callback;
	public UILabel tipLabel;
	
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 初始化提示内容
	/// </summary>
	public void initialize(string str, CallBack<bool> call){
		this.tipLabel.text = str;
		this.callback = call;
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {
			if(callback !=null)
				callback (true);
            TeamPrepareWindow win = UiManager.Instance.getWindow<TeamPrepareWindow>();
            if (win != null)
                win.isReast = true;

		}
		finishWindow ();
	}
}
