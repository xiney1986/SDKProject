using UnityEngine;
using System.Collections;

public class GoddessAstrolabeInfoWindow : WindowBase {

	public UILabel[] labelA;//0已激活，1总数，2生命，3攻击，4防御，5魔法，6敏捷
	public UILabel[] labelB;//0前排，1中排，2后排，3PVE
	public UILabel[] labelC;//0卡片，1装备，2行动力，3好友

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
	}

	public void initUI()
	{
		GoddessAstrolabeInfo info = GoddessAstrolabeManagerment.Instance.getGoddessAstrolabeServerInfo();

		int num = GoddessAstrolabeManagerment.Instance.getOpenStars() == null ? 0 : GoddessAstrolabeManagerment.Instance.getOpenStars().Length;
		labelA[0].text = num + "/";
		labelA[1].text = GoddessAstrolabeManagerment.Instance.getGoddessAstrolabeFrontInfo().Count + "";

		int hp = info.allAddEffectInteger == null ? 0 : info.allAddEffectInteger.hp;
		float hpper = info.allAddEffectNumber == null ? 0 : info.allAddEffectNumber.perHp;
		labelA[2].text = LanguageConfigManager.Instance.getLanguage("star01",hp.ToString(),hpper.ToString());

		int att = info.allAddEffectInteger == null ? 0 : info.allAddEffectInteger.attack;
		float attPer = info.allAddEffectNumber == null ? 0 : info.allAddEffectNumber.perAttack;
		labelA[3].text = LanguageConfigManager.Instance.getLanguage("star02",att.ToString(),attPer.ToString());

		int def = info.allAddEffectInteger == null ? 0 : info.allAddEffectInteger.defecse;
		float defPer = info.allAddEffectNumber == null ? 0 : info.allAddEffectNumber.perDefecse;
		labelA[4].text = LanguageConfigManager.Instance.getLanguage("star03",def.ToString(),defPer.ToString());

		int mag = info.allAddEffectInteger == null ? 0 : info.allAddEffectInteger.magic;
		float magPer = info.allAddEffectNumber == null ? 0 : info.allAddEffectNumber.perMagic;
		labelA[5].text = LanguageConfigManager.Instance.getLanguage("star04",mag.ToString(),magPer.ToString());

		int agi = info.allAddEffectInteger == null ? 0 : info.allAddEffectInteger.agile;
		float agiPer = info.allAddEffectNumber == null ? 0 : info.allAddEffectNumber.perAgile;
		labelA[6].text = LanguageConfigManager.Instance.getLanguage("star05",agi.ToString(),agiPer.ToString());

		int defTeam = info.frontAddEffectInteger == null ? 0 : info.frontAddEffectInteger.defecse;
		float defPerTeam = info.frontAddEffectNumber == null ? 0 : info.frontAddEffectNumber.perDefecse;
		labelB[0].text = LanguageConfigManager.Instance.getLanguage("star03",defTeam.ToString(),defPerTeam.ToString());

		int magTeam = info.middleAddEffectInteger == null ? 0 : info.middleAddEffectInteger.magic;
		float magPerTeam = info.middleAddEffectNumber == null ? 0 : info.middleAddEffectNumber.perMagic;
		labelB[1].text = LanguageConfigManager.Instance.getLanguage("star04",magTeam.ToString(),magPerTeam.ToString());

		int attTeam = info.behindAddEffectInteger == null ? 0 : info.behindAddEffectInteger.attack;
		float attPerTeam = info.behindAddEffectNumber == null ? 0 : info.behindAddEffectNumber.perAttack;
		labelB[2].text = LanguageConfigManager.Instance.getLanguage("star02",attTeam.ToString(),attPerTeam.ToString());

		labelB[3].text = info.addPveAttr + "%";

		labelC[0].text = info.addCardStorage + "";
		labelC[1].text = info.addEquipStorage + "";
		labelC[2].text = info.addPveUse + "";
		labelC[3].text = info.addFriend + "";
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "close") { 
			finishWindow();
		}
	}

}
