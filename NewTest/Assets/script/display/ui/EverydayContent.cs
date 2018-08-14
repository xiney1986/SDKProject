using UnityEngine;
using System.Collections;

public class EverydayContent : MonoBase {

	public LoginAwardContent awardContent;
	public UILabel unOpenLabel;
	/** 正在领取的登陆奖励 */
	TotalLogin receiveTotalLogin;
	//初始化容器数据
	public void init(WindowBase win){
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
			if(TotalLoginManagerment.Instance.NeweverydayState){
				receiveTotalLogin = TotalLoginManagerment.Instance.getNewTotalLoginBySid (receivePrizeSid);
				if(receiveTotalLogin.isloginn!=1&&receiveTotalLogin.isAward!=0){
					MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0204"));
					return;
				}
			}else{
				receiveTotalLogin = TotalLoginManagerment.Instance.getTotalLoginBySid (receivePrizeSid);
				if (TotalLoginManagerment.Instance.getTotalDay () < receiveTotalLogin.totalDays) {
					MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0204"));
					return;
				}
			}
			string str = "";
			if (StorageManagerment.Instance.checkStoreFull (receiveTotalLogin.prizes, out str) == true) {
				MessageWindow.ShowAlert (str + "," + LanguageConfigManager.Instance.getLanguage ("s0203"));
				return;
			}
			sendReceiveFPort (receivePrizeSid);
		}
		if(gameObj.name=="shareButton"){
			UiManager.Instance.openDialogWindow<OneKeyShareWindow>((win) => { win.initWin(); });
		}
	}
	//得到天天送奖励数据或新天天送数据
	TotalLogin[] SortLoginAward ()
	{
		if(TotalLoginManagerment.Instance.NeweverydayState){
			TotalLogin[] awards= TotalLoginManagerment.Instance.getNewDayDate();
			return awards;
		}else{
			TotalLogin[] awards = TotalLoginManagerment.Instance.getAvailableArray ();
			return awards;
		}


	}
	//发送领取通信
	private void sendReceiveFPort (int receivePrizeSid)
	{
		TotalLoginPrizesFPort tpf = FPortManager.Instance.getFPort ("TotalLoginPrizesFPort") as TotalLoginPrizesFPort;
		tpf.getPrize (receivePrizeSid, receiveBack,TotalLoginManagerment.Instance.NeweverydayState);
	}
	private void receiveBack ()
	{
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
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0205") + "," + LanguageConfigManager.Instance.getLanguage ("s0418"),false);
				win.dialogCloseUnlockUI=false;
			});
		} else {
            //UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
            //    win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0205"),false);
            //    win.dialogCloseUnlockUI=false;
            //});
		}

        StartCoroutine(Utils.DelayRun(() => {
            UpdateUI();
            //MaskWindow.UnlockUI();
        }, 0.5f));

        UiManager.Instance.createPrizeMessageLintWindow(ps);
        
	}
	//更新页面
	public void UpdateUI(){
		TotalLogin[] awards = SortLoginAward ();
		if(TotalLoginManagerment.Instance.NeweverydayState){
			awardContent.reLoad (awards,TotalLoginManagerment.NEWEVERYDAY);
		}else{
			awardContent.reLoad (awards,TotalLoginManagerment.EVERYDAY);
		}

	}
}
