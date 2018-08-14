using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 宝箱内容窗口
 * @author 汤琦
 * */
public class GiftBagContentWindow : WindowBase
{
	public UILabel titleLabel;
	public GiftBagContent content;
	private Award[] award;
	private Prop prop;
	private PrizeSample[] prizes;
	private int nowNum;
	
	protected override void begin ()
	{
		base.begin ();
		titleLabel.text = LanguageConfigManager.Instance.getLanguage("s0206",prop.getName(),nowNum.ToString());
		content.initGift(conAwardToPrizeSample(),this);
		content.init(conAwardToPrizeSample().Length);
		MaskWindow.UnlockUI ();
	}
	
	public void initWindow(Award[] award,Prop prop,int now)
	{
		this.award = award;
		this.prop = prop;
		nowNum = now;
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if(gameObj.name == "confirm")
		{
			UiManager.Instance.switchWindow<StoreWindow>((win)=>{
				win.updateContent();
			});
		}
	}
	
	private PrizeSample[] conAwardToPrizeSample()
	{
		List<PrizeSample> list = new List<PrizeSample>();
		for (int i = 0; i < award.Length; i++) {
			for (int j = 0; j < award[i].props.Count; j++) {
				PrizeSample sample = new PrizeSample(PrizeType.PRIZE_PROP,award[i].props[j].sid,award[i].props[j].num);
				list.Add(sample);
			}
			for (int j = 0; j < award[i].equips.Count; j++) {
				PrizeSample sample = new PrizeSample(PrizeType.PRIZE_EQUIPMENT,award[i].equips[j].sid,1);
				list.Add(sample);
			}
			for (int j = 0; j < award[i].cards.Count; j++) {
				PrizeSample sample = new PrizeSample(PrizeType.PRIZE_CARD,award[i].cards[j].sid,1);
				list.Add(sample);
			}
            for (int j = 0; j < award[i].magicWeapons.Count;j++ ) {
                PrizeSample sample = new PrizeSample(PrizeType.PRIZE_MAGIC_WEAPON,award[i].magicWeapons[j].sid,1);
            }
			if(award[i].moneyGap > 0)
			{
				PrizeSample sample = new PrizeSample(PrizeType.PRIZE_MONEY,0,award[i].moneyGap);
				list.Add(sample);
			}
			if(award[i].rmbGap > 0)
			{
				PrizeSample sample = new PrizeSample(PrizeType.PRIZE_RMB,0,award[i].rmbGap);
				list.Add(sample);
			}
		}
		
		return list.ToArray();
	}
}
