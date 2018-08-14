using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeeklyAwardContent : MonoBase {

	public LoginAwardContent awardContent;
	public UILabel unOpenLabel;
	/** 正在领取的登陆奖励 */
	TotalLogin receiveTotalLogin;
	private WindowBase winn;
	//初始化容器数据
	public void init(WindowBase win){
		winn=win;
		awardContent.fatherWindow=win;
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
			receiveTotalLogin = TotalLoginManagerment.Instance.getWeeklyButtonBySid (receivePrizeSid);
			if (receiveTotalLogin.isloginn!=1) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0204"));
				return;
			}
			string str = "";
			if (StorageManagerment.Instance.checkStoreFull (receiveTotalLogin.prizes, out str) == true) {
				MessageWindow.ShowAlert (str + "," + LanguageConfigManager.Instance.getLanguage ("s0203"));
				return;
			}
			sendReceiveFPort (receivePrizeSid);
		}
	}
	//得到周末送奖励数据
	TotalLogin[] SortLoginAward ()
	{
		TotalLogin[] awards = TotalLoginManagerment.Instance.getWeeklyAwardData ();
		return awards;
	}
	//发送领取通信
	private void sendReceiveFPort (int receivePrizeSid)
	{
		WeeklyAwardButtonFPort tpf = FPortManager.Instance.getFPort ("WeeklyAwardButtonFPort") as WeeklyAwardButtonFPort;
		tpf.getPrize (receivePrizeSid, receiveBack);
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
		DateTime timen=TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
		int totime = (int)timen.DayOfWeek;
		if(totime == 6 || totime == 0)
		{
			unOpenLabel.gameObject.SetActive(false);
			TotalLogin[] awards = SortLoginAward ();
			awardContent.reLoad (awards,TotalLoginManagerment.WEEKLY);
		}else{
			unOpenLabel.gameObject.SetActive(true);

		}

	}


}
