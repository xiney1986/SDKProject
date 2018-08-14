using UnityEngine;
using System.Collections;

public class BeastSummonShowWindow : WindowBase {

	public UILabel UI_ConsumeMoney;
	public UILabel UI_Condition;
	public UILabel UI_haveMoney;
	public ButtonExchange[] UI_ConsumeGoods;
	public ButtonBase UI_OkBtn;

	public UILabel[] oldMsgLabel;//0生命，1攻击，2防御，3魔法，4敏捷，5等级上限，6共鸣，7战力
	public UILabel[] newMsgLabel;//0生命，1攻击，2防御，3魔法，4敏捷，5等级上限，6共鸣，7战力
	public UISprite oldQuality;
	public UISprite newQuality;
	public ButtonSkill oldMainSkill;//主技能按钮
	public UILabel oldMainSkillLv;
	public ButtonSkill newMainSkill;//主技能按钮
	public UILabel newMainSkillLv;
	public ButtonSkill oldFeatures;//特性
	public ButtonSkill newFeatures;//特性

	private BeastEvolve selectedEvolve;
	private Card oldCard;
	private Card newCard;
	private long exp;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	
	public void Initialize(BeastEvolve _selectedEvolve,long _exp)
	{
		selectedEvolve = _selectedEvolve;
		exp = _exp;
		oldCard = selectedEvolve.getBeast();
		if(!selectedEvolve.isEndBeast()) {
			newCard = selectedEvolve.getNextBeast();
			newCard.updateExp(oldCard.getEXP());
		}
		else
			newCard = oldCard;
		
		showOldInfo();
		if(newCard != null)
			showNewInfo();
		showCondition ();
	}


	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
		if (gameObj.name == "okButton") {
			UiManager.Instance.switchWindow<BeastSummonWindow>((win)=>{
				win.Initialize (selectedEvolve);
				win.oldCard = oldCard.Clone() as Card;
				win.newCard = newCard;
				win.exp = exp;
				win.clickSummon ();
			});
		}
		
		if (gameObj.name == "close") {
			WindowBase win = UiManager.Instance.getWindow<GoddessWindow> ();
			if (win != null) {
				UiManager.Instance.BackToWindow<GoddessWindow> ();
			}
			else
			{
				finishWindow();
			}
		}
	}


	private void showCondition () {
		ExchangeSample sample = selectedEvolve.getExchangeBySids (selectedEvolve.getNextBeast ().sid);
		UI_Condition.text = selectedEvolve.checkPremises (selectedEvolve);

		int index = 0;

		foreach (ExchangeCondition each in sample.conditions[0]) {
			if (each.costType == PrizeType.PRIZE_MONEY) {

				UI_haveMoney.text = UserManager.Instance.self.getMoney().ToString();
				UI_ConsumeMoney.text = each.num.ToString ();

//				if(UserManager.Instance.self.getMoney()<each.num){
//					UI_ConsumeMoney.text = "[FF0000]"+each.num.ToString ();
//				}else{
//					UI_ConsumeMoney.text = each.num.ToString ();
//				}
				continue;
			}
			else {
				if (index >= UI_ConsumeGoods.Length)
					continue;
				UI_ConsumeGoods[index].updateButton (each, ButtonExchange.BEASTSUMMON);
				if(UI_ConsumeGoods[index].needValue != null)	UI_ConsumeGoods[index].needValue.text = each.num.ToString();
				ArrayList list = StorageManagerment.Instance.getPropsBySid (each.costSid);
				
				int count = 0;
				foreach (Prop tmp in list) {
					count += tmp.getNum();
				}
				if(UI_ConsumeGoods[index].haveValue != null)  UI_ConsumeGoods[index].haveValue.text =count.ToString();
				index += 1;
			} 
		}

		//前提条件是否达成
		if (!selectedEvolve.isCheckAllPremises (selectedEvolve)) {
			UI_OkBtn.disableButton (true);
			return;
		}

		//兑换条件是否达成
		if (!ExchangeManagerment.Instance.isCheckConditions (selectedEvolve.getExchangeBySids (selectedEvolve.getNextBeast().sid))) {
			UI_OkBtn.disableButton (true);
			return;
		}  
	}
	
	private void showOldInfo()
	{
		CardBaseAttribute attr = CardManagerment.Instance.getCardWholeAttr (oldCard);

		//oldMsgLabel[0].text = attr.getWholeHp () + "";
		//oldMsgLabel[1].text = attr.getWholeAtt () + "";
		//oldMsgLabel[2].text = attr.getWholeDEF () + "";
		//oldMsgLabel[3].text = attr.getWholeMAG () + "";
		//oldMsgLabel[4].text = attr.getWholeAGI () + "";
		oldMsgLabel[5].text = "Lv." + oldCard.getMaxLevel();
		oldMsgLabel[6].text = BeastEvolveManagerment.Instance.getBestResonance().ToString()+"%";
		oldMsgLabel[7].text = oldCard.getCardCombat() + "";
		oldQuality.spriteName = QualityManagerment.qualityIDToStringByBG (oldCard.getQualityId());
		
		InitOldSkill(oldCard);
	}
	
	private void showNewInfo()
	{
		CardBaseAttribute attrNew = CardManagerment.Instance.getCardWholeAttr (newCard);
		CardBaseAttribute attr = CardManagerment.Instance.getCardWholeAttr (oldCard);

		//newMsgLabel[0].text = attr.getWholeHp () + " [FF0000]+ " + (attrNew.getWholeHp() - attr.getWholeHp());
		//newMsgLabel[1].text = attr.getWholeAtt () + " [FF0000]+ " + (attrNew.getWholeAtt() - attr.getWholeAtt());
		//newMsgLabel[2].text = attr.getWholeDEF () + " [FF0000]+ " + (attrNew.getWholeDEF() - attr.getWholeDEF());
		//newMsgLabel[3].text = attr.getWholeMAG () + " [FF0000]+ " + (attrNew.getWholeMAG() - attr.getWholeMAG());
		//newMsgLabel[4].text = attr.getWholeAGI () + " [FF0000]+ " + (attrNew.getWholeAGI() - attr.getWholeAGI());
		newMsgLabel[5].text = "Lv." + newCard.getMaxLevel();
		newMsgLabel[6].text = "+"+BeastEvolveManagerment.Instance.getNextEvolveBestResonance().ToString() +"%";
		newMsgLabel[7].text =  "+"+(newCard.getCardCombat() - oldCard.getCardCombat()).ToString();
		newQuality.spriteName = QualityManagerment.qualityIDToStringByBG (newCard.getQualityId());
		
		InitNewSkill(newCard);
	}
	
	private void InitOldSkill (Card _card)
	{ 
		Skill[] mSkill = _card .getSkills ();
		int mainSkillLv = EXPSampleManager.Instance.getLevel(EXPSampleManager.SID_HALLOW_EXP,exp);
		if (mSkill == null || mSkill [0] == null)
			return;
		if(mainSkillLv == 0)
			mainSkillLv = 1;
		if(mainSkillLv >= mSkill [0].getMaxLevel())
			mainSkillLv = mSkill [0].getMaxLevel();
		oldMainSkillLv.text = "Lv." + mainSkillLv;
		string mainSkillDescript = mSkill [0].getDescribeByLv(mainSkillLv);
			
		oldMainSkill.initBeastSkill(mSkill [0], ButtonSkill.STATE_BEAST,mSkill[0].getName(),mainSkillDescript,exp,EXPSampleManager.SID_HALLOW_EXP,oldMainSkillLv.text);
		 
		oldFeatures.textLabel.text = _card .getFeatures () [1];
		//oldFeatures.skillLevel.text =(_card.getQualityId() == 1 ? "[FFFFFF]":QualityManagerment.getQualityColor(_card.getQualityId()))+ _card.getFeatures()[0]; 
        oldFeatures.skillLevel.text =_card.getFeatures()[0]; 
	}
	
	private void InitNewSkill (Card _card)
	{ 
		Skill[] mSkill = _card .getSkills ();
		int mainSkillLv = EXPSampleManager.Instance.getLevel(EXPSampleManager.SID_HALLOW_EXP,exp);
		if (mSkill == null || mSkill [0] == null)
			return;
		if(mainSkillLv == 0)
			mainSkillLv = 1;
		if(mainSkillLv >= mSkill [0].getMaxLevel())
			mainSkillLv = mSkill [0].getMaxLevel();
		newMainSkillLv.text = "Lv." + mainSkillLv;
		string mainSkillDescript = mSkill [0].getDescribeByLv(mainSkillLv);
			
		newMainSkill.initBeastSkill(mSkill [0], ButtonSkill.STATE_BEAST,mSkill[0].getName(),mainSkillDescript,exp,EXPSampleManager.SID_HALLOW_EXP,newMainSkillLv.text);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Passive", newFeatures.icon);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Passive", oldFeatures.icon);
		newFeatures.textLabel.text = _card .getFeatures () [1];
		//newFeatures.skillLevel.text = QualityManagerment.getQualityColor(_card.getQualityId())+ _card.getFeatures () [0]; 
        newFeatures.skillLevel.text = _card.getFeatures()[0]; 
	}
}
