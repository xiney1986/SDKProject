using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlchemyBuyTipWindow : WindowBase
{
	public UIToggle neverChoose;
	public UILabel cost;
	public CallBack callback;
	public UILabel tipLabel;
    public UISprite costIcon;
	public string useType = "lianjin";
	[HideInInspector]
	public AlchemyContent
		content;
	
	protected override void begin ()
	{
		base.begin ();
		//initCost ();
		MaskWindow.UnlockUI ();
	}

	public void initCost ()
	{
        costIcon.spriteName = "rmb";
		cost.text ="x"+ LanguageConfigManager.Instance.getLanguage ("AlchemyContent02", content.getConsume ().ToString ());
	}
	//**初始化高级猎魂提示内容*/
	public void initHountTip(int index,int num){
		useType = "liehun";
        if (index == 0)
            costIcon.spriteName = "icon_money";
        else if (index == 1)
            costIcon.spriteName = "rmb";
		tipLabel.text = LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_Hunt_Comfirm");
        cost.text = LanguageConfigManager.Instance.getLanguage("StarSoulWindow_Hunt_Tip", num.ToString(), StorageManagerment.Instance.getHuntStarSoulStorage().getFreeSize().ToString());
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {
			if (neverChoose.value){
				if(useType == "lianjin")
					NoticeManagerment.Instance.alchemyNeverTip = true;
				if(useType == "liehun")
					NoticeManagerment.Instance.huntNeverTip = true;
			}
			callback ();

		} else if (gameObj.name == "cancel") {

		}
		finishWindow ();
	}
}
