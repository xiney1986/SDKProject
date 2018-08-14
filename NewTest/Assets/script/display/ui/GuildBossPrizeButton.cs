using UnityEngine;
using System.Collections;

public class GuildBossPrizeButton : ButtonBase
{
	private GuildBossPrizeSample prize;
	public UISprite qualityBg;
	public UITexture boxIcon;//宝箱图
	public UILabel num;
	public Prop prizeProp;
	public void initInfo(GuildBossPrizeSample prize)
	{
		this.prize = prize;
		prizeProp = PropManagerment.Instance.createProp (this.prize.prizeSid,this.prize.prizeSum);
		initPrize();
	}

	private void initPrize()
	{
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prizeProp.getIconId (),boxIcon);
		qualityBg.spriteName = QualityManagerment.qualityIDToIconSpriteName(prizeProp.getQualityId());
		num.text = "X" + prize.prizeSum.ToString();
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
			win.Initialize (prizeProp);
		});
	}


	private string getPrizeName()
	{
		switch (prize.prizeType) {
		case PrizeType.PRIZE_BEAST:
			Card beast = CardManagerment.Instance.createCard (prize.prizeSid);
			return beast.getName();
		case PrizeType.PRIZE_CARD:
			Card card = CardManagerment.Instance.createCard (prize.prizeSid);
			return card.getName();
		case PrizeType.PRIZE_EQUIPMENT:
			Equip equip = EquipManagerment.Instance.createEquip (prize.prizeSid);
			return equip.getName();
		case PrizeType.PRIZE_MONEY:
			return "";
		case PrizeType.PRIZE_PROP:
			Prop prop = PropManagerment.Instance.createProp (prize.prizeSid);
			return prop.getName() + "X" + prize.prizeSum;
		case PrizeType.PRIZE_RMB:
			return "";
		default:
			return "";
		}
	}
}
