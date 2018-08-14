using UnityEngine;
using System.Collections;

public class ButtonBackRechargeReceive : ButtonBase
{
	[HideInInspector]
	public BackRechargeContent
		content;
	public BackRecharge recharge;

	RechargeSample sample;
	
	public void updateButton (BackRecharge recharge)
	{
		this.recharge = recharge;
		
	}
	
	public override void DoClickEvent ()
	{
		string str = LanguageConfigManager.Instance.getLanguage ("s0204");
		base.DoClickEvent ();
		sample = recharge.getRechargeSample();
		if (recharge != null && !StorageManagerment.Instance.checkStoreFull (sample.prizes, out str)) {
//			if (recharge.GetType () == typeof(Recharge)) {
//				NoticeGetActiveAwardFPort fport = FPortManager.Instance.getFPort ("NoticeGetActiveAwardFPort") as NoticeGetActiveAwardFPort;
//				fport.access (recharge.sid, (b)=>{
//					if(b) {
//						recharge.addCount(1);
//						bool isOpenHeroRoad =  HeroRoadManagerment.Instance.isOpenHeroRoad(sample.prizes);
//                        UiManager.Instance.createPrizeMessageLintWindow(sample.prizes);
//						content.updateWindow(isOpenHeroRoad);
//					} else {
//						UiManager.Instance.createMessageLintWindow (Language ("s0203"));
//					}
//				});
//			} else if (recharge.GetType () == typeof(NewRecharge)) {
//				NoticeGetActiveAwardFPort fport = FPortManager.Instance.getFPort<NoticeGetActiveAwardFPort> () as NoticeGetActiveAwardFPort;
//				fport.access (recharge.sid, (bl) => {
//					if (bl) {
//						recharge.modifyRecharge(1,1);
//						bool isOpenHeroRoad =  HeroRoadManagerment.Instance.isOpenHeroRoad(sample.prizes);
//                        UiManager.Instance.createPrizeMessageLintWindow(sample.prizes);
//						content.updateWindow (isOpenHeroRoad);
//					} else {
//						UiManager.Instance.createMessageLintWindow (Language ("s0203"));
//					}
//				});
//			}
			BackPrizeSendRechargeFPort fport = FPortManager.Instance.getFPort ("BackPrizeSendRechargeFPort") as BackPrizeSendRechargeFPort;
			fport.access(recharge.sid,sendCallBack);

		} else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, str + "," + LanguageConfigManager.Instance.getLanguage ("s0203"), null);
			});
		}
	}

	public void sendCallBack()
	{
		disableButton (true); 
		recharge.state = BackRechargeState.recevied;
		BackPrizeRechargeInfo.Instance.receviedCount++;
		UiManager.Instance.createPrizeMessageLintWindow(sample.prizes);
		content.updateWindow(false);

		// 英雄之章//
		for(int i=0;i<sample.prizes.Length;i++)
		{
			if(sample.prizes[i].type == PrizeType.PRIZE_CARD)
			{
				Card card = CardManagerment.Instance.createCard(sample.prizes[i].pSid);
				if(card != null)
				{
					if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed(card)) {
						StartCoroutine(Utils.DelayRun(() => {
							UiManager.Instance.openDialogWindow<TextTipWindow>((win) => {
								win.init(LanguageConfigManager.Instance.getLanguage("s0418"), 0.8f);
							});
						},0.7f));
					}
				}
			}
		}
	}
	
//	private bool isComplete ()
//	{
//
//		return recharge.isComplete ();
//	}
	
 
	
	
	
	
}
