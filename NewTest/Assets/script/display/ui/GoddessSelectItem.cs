using UnityEngine;
using System.Collections;

public class GoddessSelectItem : ButtonBase {

	public UILabel lvLabel;
	public UITexture icon;
	public UISprite iconBg;
	private Card card;
    public UITexture EmptyIcon;
	private GoddessUnitWindow winn;
	public void init(GoddessUnitWindow win,Card selectCard,string index){
		card=selectCard;
		winn=win as GoddessUnitWindow;
		if(card==null)
		{
			lvLabel.gameObject.SetActive(false);
			icon.gameObject.SetActive(false);
            EmptyIcon.gameObject.SetActive(true);
            if(index!="")ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.GODDESS_EMTY + index+ "_e", EmptyIcon);
			iconBg.gameObject.SetActive(true);
			iconBg.spriteName="IconBack";
		}else{
			lvLabel.gameObject.SetActive(true);
			icon.gameObject.SetActive(true);
			iconBg.gameObject.SetActive(true);
            EmptyIcon.gameObject.SetActive(false);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.GODDESS_HEAD2 + card.getImageID()+"_h", icon);
			iconBg.spriteName= QualityManagerment.qualityIDToIconSpriteName(card.getQualityId());
			lvLabel.text ="Lv."+ card.getLevel() +"/"+card.getMaxLevel();
		}
	}
	public override void DoClickEvent ()
	{
		winn.be=card;
		winn.updateUI();
		fatherWindow.finishWindow();
	}
}
