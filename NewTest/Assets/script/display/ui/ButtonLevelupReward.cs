using UnityEngine;
using System.Collections;
/// <summary>
/// 升级奖励入口按钮
/// </summary>
public class ButtonLevelupReward : ButtonBase
{

	public GoodsView goodsButton;
	public UILabel lab_info;

	/// <summary>
	/// 初始化按钮的显示
	/// </summary>
	/// <param name="level">当前等级.</param>
	/// <param name="prize">对应的奖励.</param>
	public void init(int level,PrizeSample prize)
	{
		int curLevel=UserManager.Instance.self.getUserLevel();
		if (curLevel >= level) {
			goodsButton.openEffectsShow();
			goodsButton.linkQualityEffectPoint ();
			goodsButton.showEffectByQuality (prize.getQuality ());
		}
		lab_info.text=level.ToString();
		goodsButton.clear();
		switch (prize.type) {
		case PrizeType.PRIZE_BEAST:
			Card beast = CardManagerment.Instance.createCard (prize.pSid);
			goodsButton.init(beast);
			break;
		case PrizeType.PRIZE_CARD:
			Card card = CardManagerment.Instance.createCard (prize.pSid);
			goodsButton.init(card);
			break;
		case PrizeType.PRIZE_EQUIPMENT:
			Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);
			goodsButton.init(equip);
			break;
		case PrizeType.PRIZE_MONEY:
			PrizeSample prizeMoney = new PrizeSample(PrizeType.PRIZE_MONEY,0,prize.num);
			goodsButton.init(prizeMoney);
			break;
		case PrizeType.PRIZE_PROP:
			Prop prop = PropManagerment.Instance.createProp (prize.pSid);
			goodsButton.init(prop,prize.getPrizeNumByInt ());
			break;
		case PrizeType.PRIZE_RMB:
			PrizeSample prizeRmb = new PrizeSample(PrizeType.PRIZE_RMB,0,prize.num);
			goodsButton.init(prizeRmb);
			break;
		}
		if (curLevel < level)
			goodsButton.closeEffectsShow ();
	}
}

