using UnityEngine;
using System.Collections;

/**
 * 公会Boss奖励窗口
 * */
public class GuildBossRaskAwardWindow : WindowBase
{

	public TapContentBase tapBase;
	public GuildHurtValuePrizeContent hurtContent;
	public GuildBossRaskRankcontent rastContent;
	private int index = int.MaxValue;

	protected override void begin ()
	{
		base.begin ();
		tapBase.changeTapPage (tapBase.tapButtonList [0]);
		index = 2;
		MaskWindow.UnlockUI ();
	}

	public void initWindow ()
	{
		rastContent.initContent ();
	}

	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "buttonHurt" && enabled == true) {
			if(index!=1)
			{
				hurtContent.initContent ();
				rastContent.gameObject.SetActive(false);
				hurtContent.gameObject.SetActive(true);
			}
		} else if (gameObj.name == "buttonRask" && enabled == true) {
			if(index!=2)
			{
				rastContent.initContent ();
				hurtContent.gameObject.SetActive(false);
				rastContent.gameObject.SetActive(true);
			}
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
}