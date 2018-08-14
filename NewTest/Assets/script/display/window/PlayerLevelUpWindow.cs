using UnityEngine;
using System.Collections;
 
public class PlayerLevelUpWindow :WindowBase
{ 
	public UISprite userIcon;
	public UILabel userIconLv;
	public ExpbarCtrl expbar;
	public UILabel getMoney;
	public UILabel getExp;
	public UILabel nextLvUpNeedExp;
	public UILabel roleName;
	public Transform lvUpEffectPoint;
	User user;
	AwardDisplayCtrl awardCtrl;

	public void  Initialize (AwardDisplayCtrl ctrl)
	{
		awardCtrl = ctrl;
		user = UserManager.Instance.self;
		
		//如果奖励的值为-1 表示未获得
		if (ctrl.activeAward.awardMoney == -1)
			getMoney.text = LanguageConfigManager.Instance.getLanguage ("s0050") + 0;
		else
			getMoney.text = LanguageConfigManager.Instance.getLanguage ("s0050") + (ctrl.activeAward.moneyGap); 
		
		if (ctrl.activeAward.awardExp == -1)
			getExp.text = LanguageConfigManager.Instance.getLanguage ("s0051") + 0;
		else
			getExp.text = LanguageConfigManager.Instance.getLanguage ("s0051") + (ctrl.activeAward.expGap );		
		userIconLv.text = "Lv." + UserManager.Instance.self.getUserLevel ().ToString ();
		roleName.text = UserManager.Instance.self.nickname;

	
	}
	
	protected override void begin ()
	{
		base.begin ();
		expbar.init (awardCtrl.activeAward.playerLevelUpInfo);
		expbar.setLevelUpCallBack (levelUp);
		
		nextLvUpNeedExp.text = LanguageConfigManager.Instance.getLanguage ("s0052") + (UserManager.Instance.self.getEXPUp () - UserManager.Instance.self.getEXP ());
		MaskWindow.UnlockUI ();
	}
	
	void levelUp (int now)
	{
		EffectManager.Instance.CreateEffect (lvUpEffectPoint, "Effect/Other/player_shengji");
		userIconLv.text = "Lv." + expbar.LevelNow.ToString ();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
		
		if (gameObj.name == "buttonOK" || gameObj.name == "close") {
			//直接先显示通关奖励
			openNextWindow ();
		} 
	}
	
	private void openNextWindow ()
	{
		hideWindow ();
		awardCtrl.openNextWindow ();
	}
	
}
