using UnityEngine;
using System.Collections;
using System.Text;


public class VipInfoWindow: WindowBase
{
	public UISprite vipIcon;
	public UILabel vipLv;
	public UILabel value;
	public UILabel vipInfo;
	public barCtrl VipBar;
	public UILabel needMoney;

	public void initInfo(EnumSweep type)
	{
		int vipMinLevel = 0;
		if (type == EnumSweep.fuben) {
			vipMinLevel = SweepConfigManager.Instance.skipStoryVipMinLevel;
            vipInfo.text = LanguageConfigManager.Instance.getLanguage("s0155lll1", vipMinLevel.ToString());
		}
		else if (type == EnumSweep.boss) {
			vipMinLevel = SweepConfigManager.Instance.skipBossVipMinLevel;
            vipInfo.text = LanguageConfigManager.Instance.getLanguage("s0155lll6", vipMinLevel.ToString());
		}
		//vipInfo.text=LanguageConfigManager.Instance.getLanguage("s0155lll1",vipMinLevel.ToString());
		int viplvv=UserManager.Instance.self.getVipLevel();
		if (viplvv==0){
			vipIcon.gameObject.SetActive(false);
			vipLv.gameObject.SetActive(true);
		}else{
			vipIcon.gameObject.SetActive(true);
			vipLv.gameObject.SetActive(false);
			vipIcon.spriteName = "vip" + UserManager.Instance.self.getVipLevel().ToString();
		}
		float exp1 = (float)UserManager.Instance.self.getVipEXP ();
		float exp2 = (float)UserManager.Instance.self.getVipEXPDown (vipMinLevel);
		VipBar.updateValue (exp1, exp2);
		value.text=exp1.ToString()+"/"+exp2.ToString();
		needMoney.text= LanguageConfigManager.Instance.getLanguage("s0155lll2",getNeedVipRMB(type).ToString(),vipMinLevel.ToString());   //getNeedVipRMB(type).ToString();
		MaskWindow.UnlockUI();
	}
	/// <summary>
	/// 返回还需要升级的元宝
	/// </summary>
	/// <returns>The need vip lv.</returns>
	private int getNeedVipRMB(EnumSweep type){
		int vipMinLevel = 0;
		if (type == EnumSweep.fuben) {
			vipMinLevel = SweepConfigManager.Instance.skipStoryVipMinLevel;
		}
		else if (type == EnumSweep.boss) {
			vipMinLevel = SweepConfigManager.Instance.skipBossVipMinLevel;
		}
		//float exp = (float)UserManager.Instance.self.getVipEXP () - (float)UserManager.Instance.self.getVipEXPDown ();
		//float expNeed = (float)UserManager.Instance.self.getVipEXPUp (vipMinLevel) - (float)UserManager.Instance.self.getVipEXPDown (vipMinLevel);
		return (int)((float)UserManager.Instance.self.getVipEXPDown (vipMinLevel)-(float)UserManager.Instance.self.getVipEXP ());
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{   
		base.buttonEventBase (gameObj);
		if(gameObj.name=="close"){
			finishWindow();
		}else if(gameObj.name=="button_1"){
			finishWindow();
			UiManager.Instance.openWindow<rechargeWindow> (); 
		}
	}

	

}
