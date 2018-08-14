using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfoWindow : WindowBase {

	public UILabel labelName;
	public UILabel labelId;
	public UILabel labelStar;//星座
	public UILabel labelLevel;
	public UISprite spriteVipLevel;
	public UILabel labelUnion;
	public UILabel labelTeamCombat;
	public UILabel labelHonor;
	public UILabel labelMainEvo;
	public UILabel labelMainSur;
	public UILabel labelBeastNum;
	public UILabel labelBeastEvo;
	public UILabel labelBeastEffect;
	public UILabel labelVipExp;
	public UILabel[] labelUnionExp;//0玩家、1卡片、2幻兽
	public UILabel[] labelUnionExpName;
	/** 公会技能血量附加 */
	public UILabel unionHpValue;
	public UILabel unionHpLabel;
	/** 公会技能攻击附加 */
	public UILabel unionAttValue;
	public UILabel unionAttLabel;
	/** 公会技能防御附加 */
	public UILabel unionDefValue;
	public UILabel unionDefLabel;
	/** 公会技能魔力附加 */
	public UILabel unionMagicValue;
	public UILabel unionMagicLabel;
	/** 公会技能敏捷附加 */
	public UILabel unionAgileValue;
	public UILabel unionAgileLabel;

	public barCtrl expbar;
	public UISprite[] stars;
	public UILabel labelUserExp;

	private float time = 0;
	private const string STARING = "star";//有星星
	private const string STARED = "star_b";//没有星星
	private Knighthood knighthood;
	private LevelupInfo lvinfo;

	public GameObject[] skillsNotGet;
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "close") { 
			finishWindow();
		}
	}

	public void showUI()
	{
		expbar.updateValue (UserManager.Instance.self.getLevelExp (), UserManager.Instance.self.getLevelAllExp ());
		labelUserExp.text = EXPSampleManager.Instance.getExpBarShow(EXPSampleManager.SID_USER_EXP,UserManager.Instance.self.getEXP());
		labelName.text = UserManager.Instance.self.nickname ;
		labelId.text = " ID:" + StringKit.serverIdToFrontId(UserManager.Instance.self.uid);
		labelStar.text = HoroscopesManager.Instance.getStarByType (UserManager.Instance.self.star).getName ();
		labelLevel.text = "  Lv." + UserManager.Instance.self.getUserLevel();
		if (UserManager.Instance.self.getVipLevel() > 0) {
			spriteVipLevel.gameObject.SetActive (true);
			spriteVipLevel.spriteName = "vip" + UserManager.Instance.self.getVipLevel();
		} else {
			spriteVipLevel.gameObject.SetActive (false);
		}


		if(GuildManagerment.Instance.getGuild() != null)
			labelUnion.text = GuildManagerment.Instance.getGuild().name;
		else
			labelUnion.text = LanguageConfigManager.Instance.getLanguage("Guild_0");

		int combatA = ArmyManager.Instance.getTeamCombat (1);
		int combatB = ArmyManager.Instance.getTeamCombat (2);
		int combatC = ArmyManager.Instance.getTeamCombat (3);
		int[] combats = new int[3]{combatA,combatB,combatC};

		if(combatA >= combatB) {
			if(combatA >= combatC)
				labelTeamCombat.text = LanguageConfigManager.Instance.getLanguage("s0066") + " " + combatA;
			else
				labelTeamCombat.text = LanguageConfigManager.Instance.getLanguage("s0068") + " " + combatC;
		}
		else {
			if(combatB >= combatC)
				labelTeamCombat.text = LanguageConfigManager.Instance.getLanguage("s0067") + " " + combatB;
			else
				labelTeamCombat.text = LanguageConfigManager.Instance.getLanguage("s0068") + " " + combatC;
		}

		knighthood = KnighthoodConfigManager.Instance.getKnighthoodByGrade(UserManager.Instance.self.honorLevel);
		labelHonor.text = knighthood.kName;
		updateStar();

		Card mainCard = StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid);
		labelMainEvo.text = mainCard.getEvoLevel() + "/" + EvolutionManagerment.Instance.getMaxLevel(mainCard);
		labelMainSur.text = mainCard.getSurLevel() + "/" + SurmountManagerment.Instance.getMaxSurLevel(mainCard);

		labelBeastNum.text = BeastEvolveManagerment.Instance.num + " " + LanguageConfigManager.Instance.getLanguage("ge");
		labelBeastEvo.text = BeastEvolveManagerment.Instance.evolveNum + " " + LanguageConfigManager.Instance.getLanguage("ci");
		labelBeastEffect.text = LanguageConfigManager.Instance.getLanguage("s0376",BeastEvolveManagerment.Instance.getBestResonance().ToString());

		if(UserManager.Instance.self.getVipLevel() == 0)
			labelVipExp.text = "0%";
		else
			labelVipExp.text = (VipManagerment.Instance.getVipbyLevel(UserManager.Instance.self.getVipLevel()).privilege.expAdd * 0.0001f)*100 + "%";

		getGuildSkills();
	}
	public void updateStar()
	{
		int num = knighthood.starValue;
		for (int i = 0; i < stars.Length; i++) {
			if(i < num)
			{
				stars[i].spriteName = STARING;
			}
			else 
			{
				stars[i].spriteName = STARED;
			}
		}
	}
	///<summary>
	/// 获取公会技能
	/// </summary>
	private void getGuildSkills()
	{
		GuildManagerment guildManager=GuildManagerment.Instance;
		List<GuildSkill> guildSkills = GuildManagerment.Instance.getGuildSkill();
		
		for (int j = 5; j < 10; j++) {
			skillsNotGet [j].SetActive (true);
		}
		
		for (int i = 0; guildSkills != null && i< guildSkills.Count; i++) {
			switch (StringKit.toInt (guildSkills [i].sid)) {
			case 2:
				labelUnionExpName [1].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionExpCardLabel") + "Lv." + guildSkills [i].level.ToString ();
				labelUnionExp [1].text =guildSkills[i].getDescribeByLv(guildSkills [i].level);
				skillsNotGet [6].SetActive (false);
				break;
			case 4:
				labelUnionExpName [0].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionExpRoleLabel") + "Lv." + guildSkills [i].level.ToString ();
				labelUnionExp [0].text = guildSkills[i].getDescribeByLv(guildSkills [i].level);
				skillsNotGet [5].SetActive (false);
				break;
			case 6:
				labelUnionExpName [2].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionExpBeastLabel") + "Lv." + guildSkills [i].level.ToString ();
				labelUnionExp [2].text = guildSkills[i].getDescribeByLv(guildSkills [i].level);
				skillsNotGet [7].SetActive (false);
				break;
			case 8:
				labelUnionExpName [4].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionExpCardLabel2") + "Lv." + guildSkills [i].level.ToString ();
				labelUnionExp [4].text =  guildSkills[i].getDescribeByLv(guildSkills [i].level);
					//guildManager.getSkillAddExpPorCardPve () + "%";
				skillsNotGet [9].SetActive (false);
				break;
			case 10:
				labelUnionExpName [3].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionExpBeastLabel2") + "Lv." + guildSkills [i].level.ToString ();
				labelUnionExp [3].text = guildSkills[i].getDescribeByLv(guildSkills [i].level);
				skillsNotGet [8].SetActive (false);
				break;
			}
		}
		
		if (skillsNotGet [5].activeSelf) {
			labelUnionExpName [0].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionExpRoleLabel") + "Lv.0";
			labelUnionExp [0].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_08") + "0%";
		}
		if (skillsNotGet [6].activeSelf) {
			labelUnionExpName [1].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionExpCardLabel") + "Lv.0";
			labelUnionExp [1].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_07") + "0%";
		}
		if (skillsNotGet [7].activeSelf) {
			labelUnionExpName [2].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionExpBeastLabel") + "Lv.0";
			labelUnionExp [2].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_06") + "0%";
		}
		if (skillsNotGet [8].activeSelf) {
			labelUnionExpName [3].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionExpBeastLabel2") + "Lv.0";
			labelUnionExp [3].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_06") + "0%";
		}
		if (skillsNotGet [9].activeSelf) {
			labelUnionExpName [4].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionExpCardLabel2") + "Lv.0";
			labelUnionExp [4].text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_07") + "0%";
		}
		
		CardBaseAttribute cba=guildManager.getSkillEffect();
		unionHpLabel.text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionHpLabel") + "Lv." + guildManager.getSkillLevel ("hp");
		unionAttLabel.text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionAttLabel") + "Lv." + guildManager.getSkillLevel ("attack");
		unionDefLabel.text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionDefLabel") + "Lv." + guildManager.getSkillLevel ("defense");
		unionMagicLabel.text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionMagicLabel") + "Lv." + guildManager.getSkillLevel ("magic");
		unionAgileLabel.text = LanguageConfigManager.Instance.getLanguage ("playerInfoWindow_unionAgileLabel") + "Lv." + guildManager.getSkillLevel ("agile");
		
		if(cba!=null) {
			unionHpValue.text = LanguageConfigManager.Instance.getLanguage("playerInfoWindow_01") +  cba.hp.ToString();
			unionAttValue.text = LanguageConfigManager.Instance.getLanguage("playerInfoWindow_02") + cba.attack.ToString();
			unionDefValue.text = LanguageConfigManager.Instance.getLanguage("playerInfoWindow_03") + cba.defecse.ToString();
			unionMagicValue.text = LanguageConfigManager.Instance.getLanguage("playerInfoWindow_04") + cba.magic.ToString();
			unionAgileValue.text = LanguageConfigManager.Instance.getLanguage("playerInfoWindow_05") + cba.agile.ToString();
		} else {
			string zero=Convert.ToString(0);
			unionHpValue.text = LanguageConfigManager.Instance.getLanguage("playerInfoWindow_01") +  zero;
			unionAttValue.text = LanguageConfigManager.Instance.getLanguage("playerInfoWindow_02") + zero;
			unionDefValue.text = LanguageConfigManager.Instance.getLanguage("playerInfoWindow_03") + zero;
			unionMagicValue.text = LanguageConfigManager.Instance.getLanguage("playerInfoWindow_04") + zero;
			unionAgileValue.text = LanguageConfigManager.Instance.getLanguage("playerInfoWindow_05") + zero;
		}
		
		
		//是否显示暂未习得
		if (guildManager.getSkillLevel ("hp") == 0)
			skillsNotGet [0].SetActive (true);
		if (guildManager.getSkillLevel ("attack") == 0)
			skillsNotGet [1].SetActive (true);
		if (guildManager.getSkillLevel ("defense") == 0)
			skillsNotGet [2].SetActive (true);
		if (guildManager.getSkillLevel ("magic") == 0)
			skillsNotGet [3].SetActive (true);
		if (guildManager.getSkillLevel ("agile") == 0)
			skillsNotGet [4].SetActive (true);

	}
}
