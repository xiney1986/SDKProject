using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 扫荡经验结算窗口
 * @author 陈世惟
 * */
public class SweepExpWindow : WindowBase {

	public ExpbarCtrl userExpBar;//玩家经验
	public RoleView[] teamForRole;//主力
	public ExpbarCtrl[] teamForExpBar;//主力经验
	public RoleView[] teamSubRole;//替补
	public ExpbarCtrl[] teamSubExpBar;//替补经验
	public UILabel combatLabel;//战斗力
	public UITexture beastImage;//召唤兽图片
	public GameObject noBeastBg;//没召唤兽的情况
	public ExpbarCtrl beastExpBar;//召唤兽经验
	public UILabel beastLvLabel;//召唤兽等级
	public ButtonPvpInfo buttonInfo;//玩家信息

	private Card beast;//召唤兽仓库实例
	private Army army;//实战队伍
	private Award award;//副本奖励实体

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI();
		if (!isAwakeformHide) {
			army = SweepManagement.Instance.getSweepArmy ();
			award = SweepManagement.Instance.getSweepAward();
			updateUsr();
			updateCard();
			updateBeast();
		}
	}

	//初始化玩家信息
	public void updateUsr()
	{
		buttonInfo.playerName.text = UserManager.Instance.self.nickname;

		buttonInfo.guildName.text = UserManager.Instance.self.guildName;
		ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.self.getIconPath(), buttonInfo.headIcon);

		buttonInfo.starLabel.text = HoroscopesManager.Instance.getStarByType (UserManager.Instance.self.star).getName ();
		buttonInfo.starIco.spriteName = HoroscopesManager.Instance.getStarByType (UserManager.Instance.self.star).getSpriteName ();
		buttonInfo.starIco.alpha = 1;
		if (UserManager.Instance.self.getVipLevel() > 0) {
			buttonInfo.vipIco.spriteName = "vip" + UserManager.Instance.self.getVipLevel();
			buttonInfo.vipIco.gameObject.SetActive (true);
		} else {
			buttonInfo.vipIco.gameObject.SetActive (false);
		}
		
		if (award != null && award.playerLevelUpInfo != null) {
			buttonInfo.level.text = "Lv." + award.playerLevelUpInfo.oldLevel;
			userExpBar.init (award.playerLevelUpInfo);
			userExpBar.setLevelUpCallBack ((nowLevel,index,hasTrigger) => {
				buttonInfo.level.text = "Lv." + nowLevel;
				if(!hasTrigger)
				{
					EffectManager.Instance.CreateEffectCtrlByCache(buttonInfo.headIcon.transform,"Effect/UiEffect/levelupEffect",(passobj,effect)=>{
						effect.transform.GetChild (0).particleSystem.Play ();
					});
				}
			});
			//combatLabel.text = award.playerLevelUpInfo.oldCardCombat + "";
		} else {
			User user = UserManager.Instance.self;
			buttonInfo.level.text = "Lv." + user.getUserLevel ();
			userExpBar.updateValue (UserManager.Instance.self.getLevelExp (), UserManager.Instance.self.getLevelAllExp ());
			//combatLabel.text = ArmyManager.Instance.getTeamCombat (army.armyid) + "";
		}
	}

	//初始化卡片信息
	public void updateCard()
	{
		List<EXPAward> exps = award == null ? null : (award.exps == null ?  null : award.exps);
//		int numPlayers = 0;
//		int numSubstitute = 0;

		//获得正式队员 
		string[] players = army.players;
		for (int i=0; i<players.Length; i++) {
			teamForRole [i].hideInBattle = true;
			Card c = StorageManagerment.Instance.getRole (players [i]);
			if (c != null) {
				teamForRole [i].gameObject.SetActive (true);
				teamForRole [i].init (c, this, (RoleView view)=>{
					CardBookWindow.Show(c, CardBookWindow.OTHER,null);
				});

				//计算经验
				if (award != null && exps != null) {
					for (int j = 0; j < exps.Count; j++) {
						if (c.uid == exps[j].id) {
							teamForRole [i].level.text = "Lv." + exps[j].cardLevelUpData.levelInfo.oldLevel;
							teamForExpBar[i].init(exps[j].cardLevelUpData.levelInfo);
							teamForExpBar[i].arg1 = i;
							teamForExpBar[i].setLevelUpCallBack ((nowLevel,numPlayers,hasTrigger) => {
								teamForRole [numPlayers].level.text = "Lv." + nowLevel;
								if(!hasTrigger)
								{
									EffectManager.Instance.CreateEffectCtrlByCache(teamForRole [numPlayers].transform,"Effect/UiEffect/levelupEffect",(passobj,effect)=>{
										effect.transform.GetChild (0).particleSystem.Play ();
									});
								}
							});
						}
					}
				} else {
					teamForExpBar[i].updateValue(EXPSampleManager.Instance.getNowEXPShow (c.getEXPSid (), c.getEXP ()),
					                             EXPSampleManager.Instance.getMaxEXPShow (c.getEXPSid (), c.getEXP ()));
				}
			} else {
				teamForRole [i].card = null;
				teamForRole [i].gameObject.SetActive (false);
			}
		}
		//获得替补队员
		string[] substitute = army.alternate;
		for (int i=0; i<substitute.Length; i++) {
			teamSubRole [i].hideInBattle = true;
			Card c = StorageManagerment.Instance.getRole (substitute [i]);
			if (c != null) {
				teamSubRole [i].gameObject.SetActive (true);
				teamSubRole [i].init (c, this, (RoleView view)=>{
					CardBookWindow.Show(c, CardBookWindow.OTHER,null);
				});

				//计算经验
				if (award != null && exps != null) {
					for (int j = 0; j < exps.Count; j++) {
						if (c.uid == exps[j].id) {
							teamSubRole [i].level.text = "Lv." + exps[j].cardLevelUpData.levelInfo.oldLevel;
							teamSubExpBar[i].init(exps[j].cardLevelUpData.levelInfo);
							teamSubExpBar[i].arg1 = i;
							teamSubExpBar[i].setLevelUpCallBack ((nowLevel,numSubstitute,hasTrigger) => {
								teamSubRole [numSubstitute].level.text = "Lv." + nowLevel;
								if(!hasTrigger)
								{
									EffectManager.Instance.CreateEffectCtrlByCache(teamSubRole [numSubstitute].transform,"Effect/UiEffect/levelupEffect",(passobj,effect)=>{
										effect.transform.GetChild (0).particleSystem.Play ();
									});
								}
							});
						}
					}
				} else {
					teamSubExpBar[i].updateValue(EXPSampleManager.Instance.getNowEXPShow (c.getEXPSid (), c.getEXP ()),
					                             EXPSampleManager.Instance.getMaxEXPShow (c.getEXPSid (), c.getEXP ()));
				}
			} else {
				teamSubRole [i].card = null;
				teamSubRole [i].gameObject.SetActive (false);
			}
		}
	}

	//初始化女神信息
	public void updateBeast ()
	{
		List<EXPAward> exps = award == null ? null : (award.exps == null ?  null : award.exps);

		beast = StorageManagerment.Instance.getBeast (army.beastid);
		if (beast != null) {
			beastImage.alpha = 1;
			noBeastBg.gameObject.SetActive (false);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH ,beast,beastImage);
			beastExpBar.gameObject.SetActive (true);

			if (exps != null) {
				for (int i = 0; i < exps.Count; i++) {
					if (beast.uid == exps[i].id) {
						beastLvLabel.text = "Lv." + exps[i].cardLevelUpData.levelInfo.oldLevel;
						beastExpBar.init(exps[i].cardLevelUpData.levelInfo);
						beastExpBar.setLevelUpCallBack ((nowLevel,index,hasTrigger) => {
							beastLvLabel.text = "Lv." + nowLevel;
							if(!hasTrigger)
							{
								EffectManager.Instance.CreateEffectCtrlByCache(beastImage.transform,"Effect/UiEffect/levelupEffect",(passobj,effect)=>{
									effect.transform.GetChild (0).particleSystem.Play ();
								});
							}
						});
					}
				}
			}
		}
		else {
			beastImage.alpha = 0;
			noBeastBg.gameObject.SetActive (true);
			beastExpBar.gameObject.SetActive (false);
		}
	}

	//加载数据
	public void initWindow (Award _awards)
	{
		this.award = _awards;
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "close" || gameObj.name == "buttonNext") {

			UiManager.Instance.switchWindow<SweepAwardWindow>((win)=>{
				win.initWindow(award);
			});
		}
	}
}
