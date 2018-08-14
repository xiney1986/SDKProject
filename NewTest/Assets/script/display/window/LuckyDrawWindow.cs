using UnityEngine;
using System.Collections;

/**
 * 抽奖主界面
 * @author longlingquan
 * */
public class LuckyDrawWindow : WindowBase
{
	public ContentLuckyDraw content;
	public UILabel number;
	public GameObject luckyDrawBarPrefab;

	protected override void begin ()
	{
		base.begin ();
		updateList ();
		MaskWindow.UnlockUI ();
	}


	protected override void DoEnable ()
	{
		base.DoEnable ();
		number.text = UserManager.Instance.self.getRMB () + "";
	}

	public void updateList ()
	{
		content.reLoad ();
	}
	 
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		} 
	}
	 
}
