using UnityEngine;
using System.Collections;

public class ButtonNARechargeReceive : ButtonBase
{
	[HideInInspector]
	public NoticeActivityRechargeContent
		content;
	public Recharge recharge;
	
	public void updateButton (Recharge recharge)
	{
		this.recharge = recharge;
		
	}
	
	public override void DoClickEvent ()
	{
		string str = LanguageConfigManager.Instance.getLanguage ("s0204");
		base.DoClickEvent ();
		RechargeSample sample = RechargeSampleManager.Instance.getRechargeSampleBySid (recharge.sid);
		if (recharge != null && !StorageManagerment.Instance.checkStoreFull (sample.prizes, out str)) {
			disableButton (true); 
			if (recharge.GetType () == typeof(Recharge)) {
				NoticeGetActiveAwardFPort fport = FPortManager.Instance.getFPort ("NoticeGetActiveAwardFPort") as NoticeGetActiveAwardFPort;
				fport.access (recharge.sid, (b)=>{
					if(b) {
						recharge.addCount(1);
						bool isOpenHeroRoad =  HeroRoadManagerment.Instance.isOpenHeroRoad(sample.prizes);
                        UiManager.Instance.createPrizeMessageLintWindow(sample.prizes);
						content.updateWindow(isOpenHeroRoad);
					} else {
						UiManager.Instance.createMessageLintWindow (Language ("s0203"));
					}
				});
			} else if (recharge.GetType () == typeof(NewRecharge)) {
				NoticeGetActiveAwardFPort fport = FPortManager.Instance.getFPort<NoticeGetActiveAwardFPort> () as NoticeGetActiveAwardFPort;
				fport.access (recharge.sid, (bl) => {
					if (bl) {
						recharge.modifyRecharge(1,1);
						bool isOpenHeroRoad =  HeroRoadManagerment.Instance.isOpenHeroRoad(sample.prizes);
                        UiManager.Instance.createPrizeMessageLintWindow(sample.prizes);
						content.updateWindow (isOpenHeroRoad);
					} else {
						UiManager.Instance.createMessageLintWindow (Language ("s0203"));
					}
				});
			}

		} else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, str + "," + LanguageConfigManager.Instance.getLanguage ("s0203"), null);
			});
		}
	}
	
	private bool isComplete ()
	{

		return recharge.isComplete ();
	}
	
 
	
	
	
	
}
