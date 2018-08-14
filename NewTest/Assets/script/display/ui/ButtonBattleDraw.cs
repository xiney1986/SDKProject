using UnityEngine;
using System.Collections;

public class ButtonBattleDraw : ButtonBase
{
	public UISprite back;
	public UISprite select;
	public UITexture icon;
	public UILabel num;
	public UILabel value;
	public PrizeSample prize;
	public int drawNum = 0;

	public void clearDate()
	{
		drawNum =0;
		value.text = "";
		select.enabled = false;
		value.gameObject.SetActive(false);
//		GetComponent<BoxCollider>().enabled = false;
	}

	public void initInfo(PrizeSample prize)
	{
		this.prize = prize;
		setCreatButton(prize);
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (prize != null) {
			clickButton (prize);
		}
	}

	private void clickButton(PrizeSample prize)
	{
		switch (prize.type) {
		case PrizeType.PRIZE_MONEY:
			break;
		case PrizeType.PRIZE_PROP:
			Prop prop = PropManagerment.Instance.createProp (prize.pSid);
			UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
				win.Initialize (prop);
			});
			break;
		case PrizeType.PRIZE_RMB:
			break;
		}
		MaskWindow.UnlockUI();
	}

	private void showBattleDrawWindow()
	{
		UiManager.Instance.openWindow<BattleDrawWindow> ();
	}

	public PrizeSample getPrize()
	{
		return prize;
	}

	//设置创建按钮信息
	private void setCreatButton (PrizeSample _prize)
	{
		if(_prize == null)
		{
			return;
		}
		else
		{
			prize = _prize;
			icon.gameObject.SetActive(false);
			back.spriteName = QualityManagerment.qualityIDToIconSpriteName(_prize.getQuality());
			switch (prize.type) {
			case PrizeType.PRIZE_MONEY:
				ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.MONEY_ICONPATH, icon);
				num.text = "x"+prize.num.ToString();
				break;
			case PrizeType.PRIZE_PROP:
				Prop prop = PropManagerment.Instance.createProp(prize.pSid);
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId (), icon);
				num.text ="x"+prize.num.ToString();
				break;
			case PrizeType.PRIZE_RMB:
				ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.RMB_ICONPATH, icon);
				num.text ="x"+prize.num.ToString();
				break;
			}
			icon.gameObject.SetActive(true);
		}
	}
}
