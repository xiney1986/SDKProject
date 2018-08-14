using UnityEngine;
using System.Collections;

public class ButtonPvpInfo : ButtonBase
{
	/** 头像 */
	public UITexture headIcon;
   	//General Property
	/** 等级 */
	public UILabel level;
	/** 女神等级 */
	public UILabel beastLv;
	/** 战斗力 */
    public UILabel combat;
	/** 战斗力背景 */
    public UISprite combatBg;
	/** 女神  */
	public UITexture beast;
	/** 没有女神  */
	public UITexture noBeast;
	public GameObject formationRoot;
	/** 经验条 */
	public barCtrl expbar;
	/** 经验文字 */
	public UILabel expLabel;
	/** vip图标 */
	public UISprite vipIco;
	public GameObject tenFormationRoot;


	//Normal PvpPlayerInfo
	public GameObject root_nomal;
	/** 公会名 */
	public UILabel guildName; 
	/** 星座名 */
	public UILabel starLabel;
	/** 星座图标 */
	public UISprite starIco;
	/** 玩家名 */
	public UILabel playerName;
	/** 性别图标 */
	public UISprite sexSprite;


	//Ladder PvpPlayerInfo
	public GameObject root_ladders;
	public UILabel label_ladders_rank;
	public UILabel label_ladders_playerName;
	public UISprite ladders_sexSprite;
	public UILabel label_ladders_playerScore;
	public UISprite sprite_ladders_playerTitle;
	public UILabel label_applyHelpCount;

	public void initInfo (PvpOppInfo opp,WindowBase fawin)
	{
		if (playerName != null) playerName.text = opp.name;
		
		if (level != null) level.text = "Lv." + EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_USER_EXP, opp.exp, 0);
		if (combat != null) {
			if (fawin.name == "PvpPlayerWindow") {
				combat.text = opp.combat.ToString();
				if (combatBg != null) combatBg.spriteName = "mainCombat";
			} else if (fawin.name == "MassPlayerWindow") {
				combat.text = opp.allCombat.ToString();
				if (combatBg != null) combatBg.spriteName = "allCombat";
			} else {
				combat.text = LanguageConfigManager.Instance.getLanguage ("sl0369") + opp.combat.ToString();
				if (combatBg != null) combatBg.spriteName = "power";
			}
		}
		if (opp.sdkInfo != null)
			ResourcesManager.Instance.LoadAssetBundleTexture (opp.sdkInfo.face, headIcon);
		else
			ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (opp.headIcon), headIcon);

		if (starLabel != null) starLabel.text = HoroscopesManager.Instance.getStarByType (opp.star).getName ();
		if (starIco != null) starIco.spriteName = HoroscopesManager.Instance.getStarByType (opp.star).getSpriteName ();
		
		if (StringKit.toInt (level.text.Split ('.') [1]) == 0) {
			if (expbar != null) expbar.updateValue (opp.exp, 0);
			if (expLabel != null) expLabel.text = "0/0";
		} else {
			int tempLevel = StringKit.toInt (level.text.Split ('.') [1]);
			long expDown = EXPSampleManager.Instance.getEXPDown(EXPSampleManager.SID_USER_EXP,tempLevel);
			long expUp = EXPSampleManager.Instance.getEXPUp(EXPSampleManager.SID_USER_EXP,tempLevel);
			if (expLabel != null) expLabel.text = (opp.exp - expDown) + "/" + (expUp -expDown);
			if (expbar != null) expbar.updateValue(opp.exp-expDown,expUp-expDown);
		}
		if (vipIco != null) {
			if (opp.vipLv > 0) {
				vipIco.spriteName = "vip" + opp.vipLv;
				vipIco.gameObject.SetActive (true);
			} else {
				vipIco.gameObject.SetActive (false);
			}
		}

		if (opp.beastSid != 0) {
			CardSample cs = CardSampleManager.Instance.getRoleSampleBySid (opp.beastSid);
//			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + cs.imageID + "_head", beast);
			if(noBeast != null)
				noBeast.gameObject.SetActive(false);
			beast.gameObject.SetActive(true);
            if(CommandConfigManager.Instance.getNvShenClothType() == 0)
			    ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + cs.imageID +"c", beast);
            else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + cs.imageID, beast);
			if (beastLv != null) beastLv.text = "Lv." + EXPSampleManager.Instance.getLevel (cs.levelId, opp.beastExp, 0).ToString ();
		} else {
			if(noBeast != null)
				noBeast.gameObject.SetActive(true);
			beast.gameObject.SetActive(false);
		}
	}
}
