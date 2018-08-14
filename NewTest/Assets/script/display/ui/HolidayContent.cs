using UnityEngine;
using System.Collections;

public class HolidayContent : MonoBase {

	public LoginAwardContent awardContent;
	public UILabel unOpenLabel;
	/** 正在领取的登陆奖励 */
	TotalLogin receiveTotalLogin;
	private WindowBase winn;
	//初始化容器数据
	public void init(WindowBase win){
		awardContent.fatherWindow=win;
		winn=win;
		UpdateUI();
	}
	/** tap点击事件 */
	public void tapButtonEventBase (GameObject gameObj) {
		
	}
	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {
		if (gameObj.name.StartsWith ("awardButton:")) {
			string[] strs = gameObj.name.Split (':');
			int receivePrizeSid = int.Parse (strs [1]);
			receiveTotalLogin = TotalLoginManagerment.Instance.getHolidayBySid (receivePrizeSid);
			if(receiveTotalLogin.isAward==1){
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0204"));
				return;
			}
			string str = "";
			if (StorageManagerment.Instance.checkStoreFull (receiveTotalLogin.prizes, out str) == true) {
				MessageWindow.ShowAlert (str + "," + LanguageConfigManager.Instance.getLanguage ("s0203"));
				return;
			}
			sendReceiveFPort (receiveTotalLogin.holidayAllSid,receiveTotalLogin.day);
		}
	}
	//得到天天送奖励数据
	TotalLogin[] SortLoginAward ()
	{
		TotalLogin[] awards = TotalLoginManagerment.Instance.getHolidayAwardData ();
		return awards;
	}
	//发送领取通信
	private void sendReceiveFPort (int receivePrizeSid,int day)
	{
		HolidayAwardButtonFPort tpf = FPortManager.Instance.getFPort ("HolidayAwardButtonFPort") as HolidayAwardButtonFPort;
		tpf.getPrize (receivePrizeSid, day,receiveBack);
	}
	private void receiveBack (int type)
	{
		if(type==1){
			//判断是否领取的卡片开启英雄之章
			PrizeSample[] ps = receiveTotalLogin.prizes;
			bool isNew = false;
			for (int i = 0; ps != null && i < ps.Length; i++) {
				PrizeSample prize = ps [i];
				if (prize.type == PrizeType.PRIZE_CARD) {
					Card showCard = CardManagerment.Instance.createCard (prize.pSid);
					if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed (showCard)) {
						isNew = true;
						continue;
					}
				}
			}
			if (isNew) {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0205") + "," + LanguageConfigManager.Instance.getLanguage ("s0418"));
			} else {
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0205"));
			}
			
			StartCoroutine (Utils.DelayRun(()=>{
				UpdateUI();
			},0.5f));
		}else if(type==2){
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("recharge02"));
			StartCoroutine (Utils.DelayRun(()=>{
				UpdateUI();
			},0.5f));
		}else{
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0203"));
			StartCoroutine (Utils.DelayRun(()=>{
				UpdateUI();
			},0.5f));
		}


	}
	//更新页面
	public void UpdateUI(){
		(winn as TotalLoginWindow).resetHolidayWeekShow();
		awardContent.cleanAll();
		TotalLogin[] awards = SortLoginAward ();
		if(awards==null){
			unOpenLabel.gameObject.SetActive(true);
		}else{
			unOpenLabel.gameObject.SetActive(false);
			awardContent.reLoad (awards,TotalLoginManagerment.HOLIDAY);
		}

	}
}
