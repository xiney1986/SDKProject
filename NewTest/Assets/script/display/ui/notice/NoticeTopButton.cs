using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoticeTopButton:ButtonBase
{
	public UITexture icon;
	public UITexture selelct;
	public GameObject tip;
	public UILabel tipLabel;
	public UISprite timeLimit;
	private Notice notice;
	private NoticeSample noticeSample;
	[HideInInspector]
	public float
		local_x;

	public Notice getNotice ()
	{
		return this.notice;
	}

	public void setNotice (Notice notice)
	{
		this.notice = notice;
		this.noticeSample = notice.getSample ();
	}

	public void init (Notice notice)
	{
		setNotice (notice);
		textLabel.text = noticeSample.name;
//		if (noticeSample.timeLimit == 1 && !timeLimit.gameObject.activeSelf)
//			timeLimit.gameObject.SetActive (true);
	}

	public void updateTime ()
	{
		if (noticeSample == null)
			return;
		if (tip == null)
			return;
		int type = noticeSample.type;
		int now = ServerTimeKit.getSecondTime ();
		int current = ServerTimeKit.getCurrentSecond ();
		if (type == NoticeType.HAPPY_TURN_SPRITE) {
			if (UserManager.Instance.self.getUserLevel () >= noticeSample.levelLimit && NoticeManagerment.Instance.turnSpriteData.num !=0){
				tip.SetActive (true);
				tipLabel.text = NoticeManagerment.Instance.turnSpriteData.num.ToString();
			}
			else if (tip.activeSelf)
				tip.SetActive (false);
		}
		else if (type == NoticeType.XIANSHI_HAPPY_TURN) {
			if (UserManager.Instance.self.getUserLevel () >= noticeSample.levelLimit && NoticeManagerment.Instance.xs_turnSpriteData.num !=0){
				tip.SetActive (true);
				tipLabel.text = NoticeManagerment.Instance.xs_turnSpriteData.num.ToString();
			}
			else if (tip.activeSelf)
				tip.SetActive (false);
		}
		else if (type == NoticeType.ALCHEMY) {
			if (!(NoticeManagerment.Instance.getAlchemyConsume () > 0)) {
				if (!tip.activeSelf)
					tip.gameObject.SetActive (true);
			}
			else if (tip.activeSelf)
				tip.gameObject.SetActive (false);
		}
		else if (type == NoticeType.HEROEAT) {
			int[] info = NoticeManagerment.Instance.getHeroEatInfo ();
			if (info != null && info [1] < now && now < info [2] && info [3] == 0) {
				if (!tip.activeSelf)
					tip.gameObject.SetActive (true);
			}
			else if (tip.activeSelf)
				tip.gameObject.SetActive (false);
		}
		else if (type == NoticeType.TOPUPNOTICE || type == NoticeType.COSTNOTICE || type == NoticeType.TIME_RECHARGE || type == NoticeType.NEW_RECHARGE || type == NoticeType.NEW_CONSUME) {
			List<Recharge> temps = RechargeManagerment.Instance.getValidRechargesByTime ((noticeSample.content as SidNoticeContent).sids, now);
			if (temps == null || temps.Count == 0) {
				tip.gameObject.SetActive (false);
			}
			else {
				tip.gameObject.SetActive (true);
				tipLabel.text = temps.Count.ToString ();
			}
		}
		else if (type == NoticeType.NEW_EXCHANGE) {
			//List<Exchange> temps = ExchangeManagerment.Instance.getValidExchangesByTime ((noticeSample.content as NewExchangeNoticeContent).actives[0].exchangeSids, noticeSample.type, now);
//			if (temps == null || temps.Count == 0) {
//				tip.gameObject.SetActive (false);
//			}else{
//				tip.gameObject.SetActive (true);
//				tipLabel.text = temps.Count.ToString ();
//			}
			int count = 0;
			for(int i=0;i<(noticeSample.content as NewExchangeNoticeContent).actives.Length;i++){
				count += (ExchangeManagerment.Instance.getValidExchangesByTime((noticeSample.content as NewExchangeNoticeContent).actives[i].exchangeSids,NoticeType.EXCHANGENOTICE,now)).Count;
			}
			if(count == 0){
				tip.gameObject.SetActive (false);
			}else{
				tip.gameObject.SetActive (false);
//				tip.gameObject.SetActive (true);
//				tipLabel.text = count.ToString();
			}
		}
		else if (type == NoticeType.EXCHANGENOTICE) {
			List<Exchange> temps = ExchangeManagerment.Instance.getValidExchangesByTime ((noticeSample.content as SidNoticeContent).sids, noticeSample.type, now);
			if (temps == null || temps.Count == 0) {
				tip.gameObject.SetActive (false);
			}
			else  if(noticeSample.sid != 4){
				tip.gameObject.SetActive (true);
				tipLabel.text = temps.Count.ToString ();
			}else 	if(noticeSample.sid == 4 ){ //vip兑换特殊处理
				if(PlayerPrefs.GetString(PlayerPrefsComm.VIP_EXCHANGE_TIP) == "ok"){
					tip.gameObject.SetActive (true);
				}
				else
					tip.gameObject.SetActive(false);
			}
		}
		else if (type == NoticeType.ONERMB) {
			int state = RechargeManagerment.Instance.getOneRmbState ();
			if (state != RechargeManagerment.ONERMB_STATE_VALID) {
				tip.gameObject.SetActive (false);
			}
			else {
				tip.gameObject.SetActive (true);
			}
		}
		else if (type == NoticeType.MONTHCARD) {
			int state = NoticeManagerment.Instance.getMonthCardRewardState ();
			if (state != NoticeManagerment.MONTHCARD_STATE_VALID) {
				tip.gameObject.SetActive (false);
			}
			else {
				tip.gameObject.SetActive (true);
			}
		}
		else if (type == NoticeType.HAPPY_SUNDAY) {
			int num = HappySundayManagerment.Instance.getCanReceiveNum ();
			tip.gameObject.SetActive (num > 0);
			if (tip.gameObject.activeSelf) {
				tipLabel.text = num.ToString ();
			}
		}
		else if (type == NoticeType.QUIZ_EXAM) {
			bool isEnbale = (notice as QuizNotice).isCanAnswer ();
			if (isEnbale) {
				tip.gameObject.SetActive (true);
			}
			else {
				tip.gameObject.SetActive (false);
			}
		}
		else if (type == NoticeType.QUIZ_SURVEY) {
			bool isEnbale = (notice as QuizNotice).isCanAnswer ();
			if (isEnbale) {
				tip.gameObject.SetActive (true);				
			}
			else {
				tip.gameObject.SetActive (false);
			}
		}
		else if (type == NoticeType.DAILY_REBATE) {
			int num= 0;
			ArrayList dailyList = TaskManagerment.Instance.getDailyRebateTask();
			for(int i=0;i<dailyList.Count;i++)
			{
				if(TaskManagerment.Instance.isComplete(dailyList[i] as Task))
					num ++;
			}
			if(num>0)
			{
				tip.gameObject.SetActive(true);
				tipLabel.text = num.ToString();
			}
			else
			{
				tip.gameObject.SetActive(false);
				tipLabel.text = "";
			}
		}
        else if (type == NoticeType.LIMIT_COLLECT) {
            int num = 0;
            foreach (int sid in (noticeSample.content as SidNoticeContent).sids)
            {
                LimitCollectSample sample = NoticeActiveManagerment.Instance.getActiveInfoBySid(sid) as LimitCollectSample;
                if ( sample != null &&sample.isCanReceive()) {
                    num++;
                }
            }
            if (num > 0)
            {
                tip.gameObject.SetActive(true);
                tipLabel.text = num.ToString();
            }
            else
            {
                tip.gameObject.SetActive(false);
                tipLabel.text = "";
            }

        }
        else if (type == NoticeType.ONE_MANY_RECHARGE)
        {
            List<Recharge> temps = RechargeManagerment.Instance.getValidRechargesByTime((noticeSample.content as SidNoticeContent).sids, now);
            if (temps == null || temps.Count == 0)
            {
                tip.gameObject.SetActive(false);
            }
            else
            {
                tip.gameObject.SetActive(true);
                tipLabel.text = temps.Count.ToString();
            }
        }
		else if(type == NoticeType.SUPERDRAW)
		{
			if(SuperDrawManagerment.Instance==null)
			{
				tip.gameObject.SetActive(false);
				return;
			}
			else
			{
				int count = SuperDrawManagerment.Instance.superDraw.canUseNum;
				if(count==0)
					tip.gameObject.SetActive(false);
				else
				{
					tip.gameObject.SetActive(true);
					tipLabel.text = count.ToString();
				}
			}
        } else if (type == NoticeType.SIGNIN) {
            SignInSample sample = SignInSampleManager.Instance.getSignInSampleBySid(StringKit.toInt(notice.sid + ""+ServerTimeKit.getCurrentMonth()));
            if(sample == null) return;
            List<int> sids = sample.daySids;
            if (!SignInManagerment.Instance.stateList.Contains(sids[ServerTimeKit.getDayOfMonth() -1])) {
                tip.gameObject.SetActive(true);
                tipLabel.text = "1";
            } else {
                tip.gameObject.SetActive(false);
            }
        } else if (type == NoticeType.SHAREDRAW) {
            if (ShareDrawManagerment.Instance.isFirstShare == 0 && ShareDrawManagerment.Instance.canDrawTimes == 0) {
                tip.gameObject.SetActive(true);
                tipLabel.text = "1";
            } else if (ShareDrawManagerment.Instance.canDrawTimes != 0 && ShareDrawManagerment.Instance.isFirstShare == 0) {
                tip.gameObject.SetActive(true);
                tipLabel.text = (ShareDrawManagerment.Instance.canDrawTimes + 1).ToString();
            } else if (ShareDrawManagerment.Instance.canDrawTimes != 0 && ShareDrawManagerment.Instance.isFirstShare != 0) {
                tip.gameObject.SetActive(true);
                tipLabel.text = ShareDrawManagerment.Instance.canDrawTimes.ToString();
            } else {
                tip.gameObject.SetActive(false);
            }
		}else if(type == NoticeType.WEEKCARD){
			if(WeekCardInfo.Instance.recevieState == WeekCardRecevieState.recevie)
			{
				tip.gameObject.SetActive(true);
				tipLabel.text = "1";
			}
			else
			{
				tip.gameObject.SetActive(false);
			}
		}else if(type == NoticeType.BACK_PRIZE){
			if(BackPrizeLoginInfo.Instance.loginDays <= BackPrizeInfoFPort.tottalLoginDays)
			{
				if(BackPrizeLoginInfo.Instance.loginDays - BackPrizeLoginInfo.Instance.receivedDays.Count > 0)
				{
					tip.gameObject.SetActive(true);
					tipLabel.text = (BackPrizeLoginInfo.Instance.loginDays - BackPrizeLoginInfo.Instance.receivedDays.Count).ToString();
				}
				else
				{
					tip.gameObject.SetActive(false);
				}
			}
			else
			{
				if(BackPrizeInfoFPort.tottalLoginDays - BackPrizeLoginInfo.Instance.receivedDays.Count > 0)
				{
					tip.gameObject.SetActive(true);
					tipLabel.text = (BackPrizeInfoFPort.tottalLoginDays - BackPrizeLoginInfo.Instance.receivedDays.Count).ToString();
				}
				else
				{
					tip.gameObject.SetActive(false);
				}
			}

		}else if(type == NoticeType.BACK_RECHARGE){
			if(BackPrizeRechargeInfo.Instance.getCanRecevieCount() - BackPrizeRechargeInfo.Instance.getReceviedCount() > 0)
			{
				tip.gameObject.SetActive(true);
				tipLabel.text = (BackPrizeRechargeInfo.Instance.getCanRecevieCount() - BackPrizeRechargeInfo.Instance.getReceviedCount()).ToString();
			}
			else
			{
				tip.gameObject.SetActive(false);
			}
		}
		else if(type == NoticeType.XIANSHI_FANLI){
			if(RebateInfoManagement.Instance.canRecevieCount > 0)
			{
				tip.gameObject.SetActive(true);
				tipLabel.text = RebateInfoManagement.Instance.canRecevieCount.ToString();
			}
			else
			{
				tip.gameObject.SetActive(false);
			}
		}
		else if(type == NoticeType.LOTTERY)
		{
			if(LotteryManagement.Instance.getLotteryCount() + LotteryManagement.Instance.selectedAwardCount > 0)
			{
				if((notice as LotteryNotice).isActivityOpen())
				{
					tip.gameObject.SetActive(true);
					tipLabel.text = (LotteryManagement.Instance.getLotteryCount() + LotteryManagement.Instance.selectedAwardCount).ToString();
				}
				else
				{
					if(LotteryManagement.Instance.selectedAwardCount > 0)
					{
						tip.gameObject.SetActive(true);
						tipLabel.text = LotteryManagement.Instance.selectedAwardCount.ToString();
					}
					else 
					{
						tip.gameObject.SetActive(false);
					}
				}
			}
			else 
			{
				tip.gameObject.SetActive(false);
			}
		}
	}
}