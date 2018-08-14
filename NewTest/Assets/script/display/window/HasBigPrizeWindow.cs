using UnityEngine;
using System.Collections;

public class HasBigPrizeWindow : WindowBase
{
	//按钮位置
	public UILabel totalLoginLabel;//信息内容
	public UITexture cardimage;
	
	protected override void begin ()
	{
		base.begin ();
		totalLoginLabel.text = LanguageConfigManager.Instance.getLanguage ("s0116", TotalLoginManagerment.Instance.getTotalDay ().ToString ());
		Card card = CardManagerment.Instance.createCard (TotalLoginManagerment.Instance.getTodayPrizeOnly ().prizes [0].pSid);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), cardimage);	
		cardimage.alpha=1;
		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		UiManager.Instance.switchWindow<TotalLoginWindow>((win)=>{
			win.Initialize ();
		});
	}
}
