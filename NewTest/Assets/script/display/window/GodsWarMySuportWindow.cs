using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarMySuportWindow : WindowBase 
{
	/// <summary>
	///  提示 
	/// </summary>
	public UILabel lblTips;
	public GameObject itemPrefab;
	public GameObject mask;
	List<GodsWarMySuportInfo> info;
	public GodsWarMySuportContent content;

	CallBack callback;

	protected override void begin ()
	{
		base.begin ();
	    UiManager.Instance.godsWarMySuportWindow = this;
	}

	public void initWindow(CallBack callback)
	{
		this.callback = callback;
		content.initContent(this,updateUI);
		MaskWindow.UnlockUI();
	}
	public void updateUI()
	{
		info = GodsWarManagerment.Instance.mySuportInfo;
		int num = 0;
		int cout = 0;
	    string prizeName = "";
		if(info!=null)
		{
			for (int i = 0; i < info.Count; i++) {
                //if(info[i].prizes!=null){
                //    num += info[i].prizes.getPrizeNumByInt();
                //    cout++;
                //}
                int a = GodsWarManagerment.Instance.getTypeByLocalId(info[i].localId);
                PrizeSample ps = GodsWarPrizeSampleManager.Instance.getSuportPrize()[a - 1].item[0];
			    cout++;
			    if (info[i].prizes != null)
			    {
			        prizeName = info[i].prizes.getPrizeName();
                    num += info[i].prizes.getPrizeNumByInt();
			    }
                    //ps.getPrizeNumByInt()*(1 + 2);
			}
			if(num>0)
                lblTips.text = LanguageConfigManager.Instance.getLanguage("godsWar_80", cout.ToString(), num.ToString() + prizeName);
			else
				lblTips.text = LanguageConfigManager.Instance.getLanguage("godsWar_94");
		}
		else
		{
			lblTips.text = LanguageConfigManager.Instance.getLanguage("godsWar_94");
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "button_close") {
			finishWindow();
		} 
	}
    void OnDestroy() {
        UiManager.Instance.godsWarMySuportWindow = null;
    }
	
}
