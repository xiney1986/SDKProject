using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentNoticeActivity : dynamicContent
{ 
	List<Exchange> exchangeList;
	List<Recharge> rechargeList;
	List<NoticeActiveGoods> consumeRebateList;
	private Notice notice;
	private NoticeSample sample;
	public Timer timer;//计时器
	private WindowBase win;
	private MonoBase activityBase;
	
	public void initContent (Notice notice, WindowBase win, MonoBase activityBase)
	{
		this.win = win;
		this.notice = notice;
		this.activityBase = activityBase;
		sample = NoticeSampleManager.Instance.getNoticeSampleBySid (notice.sid);
		if (sample.type == NoticeType.EXCHANGENOTICE) {
			exchangeList = ExchangeManagerment.Instance.getCanUseExchanges ((sample.content as SidNoticeContent).sids, sample.type, false);
			clearExchangeOver ();
		} else if (sample.type == NoticeType.TOPUPNOTICE || sample.type == NoticeType.COSTNOTICE || sample.type == NoticeType.TIME_RECHARGE) {
			rechargeList = RechargeManagerment.Instance.getCanUseRecharges ((sample.content as SidNoticeContent).sids);
			clearRechargeOver ();
		} else if (sample.type == NoticeType.CONSUME_REBATE) {
			consumeRebateList = NoticeActiveManagerment.Instance.getGoodsList ((sample.content as NewExchangeNoticeContent).actives);
		} else if (sample.type == NoticeType.NEW_RECHARGE || sample.type == NoticeType.NEW_CONSUME) {
			rechargeList = NoticeActiveManagerment.Instance.getRechargeList ((sample.content as SidNoticeContent).sids);
		} else if (sample.type == NoticeType.NEW_EXCHANGE) {
			exchangeList = NoticeActiveManagerment.Instance.getExchangeList ((sample.content as NewExchangeNoticeContent).actives);
        } else if (sample.type == NoticeType.ONE_MANY_RECHARGE)
        {
            rechargeList = RechargeManagerment.Instance.getOneManyRecharges((sample.content as SidNoticeContent).sids);
            clearRechargeOver();
        }
		startTime ();
	}

	private void clearExchangeOver ()
	{
		Exchange ex;
		int now = ServerTimeKit.getSecondTime ();
		for (int i=0; i<exchangeList.Count; i++) {
			ex = exchangeList [i];
			int[] time = NoticeManagerment.Instance.getExchangeTime (ex.sid);
			if (time != null && time.Length == 1 && now > time [0]) {
				exchangeList.RemoveAt (i);
				i--;
			}
		}
	}

	private void clearRechargeOver ()
	{
		Recharge rg;
		int now = ServerTimeKit.getSecondTime ();
		for (int i=0; i<rechargeList.Count; i++) {
			rg = rechargeList [i];
			if (rg.checkTimeOut (now)) {
				rechargeList.RemoveAt (i);
				i--;
			}
		}
	}
	public void Initialize ()
	{
		if (sample.type == NoticeType.EXCHANGENOTICE || sample.type == NoticeType.NEW_EXCHANGE) {
			//vip兑换特殊处理
			if (notice.getSample ().sid == 4)
				PlayerPrefs.SetString (PlayerPrefsComm.VIP_EXCHANGE_TIP, UserManager.Instance.self.uid);
			clearExchangeOver ();
			if (exchangeList == null)
				return;
			reLoad (exchangeList.Count);
			
		} else if (sample.type == NoticeType.TOPUPNOTICE || sample.type == NoticeType.COSTNOTICE || sample.type == NoticeType.TIME_RECHARGE ||
			sample.type == NoticeType.NEW_RECHARGE || sample.type == NoticeType.NEW_CONSUME || sample.type == NoticeType.ONE_MANY_RECHARGE) {
			if (rechargeList == null || rechargeList.Count < 1)
				return;
			// 累计充值、消费//
			if(rechargeList[0].getRechargeSample().reType == RechargeSample.RECHARGES)
			{
				reLoad (rechargeList.Count,getRechargeAwardIndex());
			}
			else 
			{
				reLoad (rechargeList.Count);
			}
			//reLoad (rechargeList.Count);
		} else if (sample.type == NoticeType.CONSUME_REBATE) {
			reLoad (consumeRebateList.Count);
		}
	}

	private void startTime ()
	{
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updateTime);
		timer.start ();
	}

	//更新所有加载项目的时间
	public void updateTime ()
	{
		if (sample.type == NoticeType.EXCHANGENOTICE || sample.type == NoticeType.NEW_EXCHANGE) {
			for (int i=0; i<nodeList.Count; i++) {
				if (nodeList [i] == null)
					continue;
				NoticeActivityExchangeBarCtrl button = nodeList [i].GetComponent<NoticeActivityExchangeBarCtrl> ();
				button.showTime ();
			}
		} else if (sample.type == NoticeType.TOPUPNOTICE || sample.type == NoticeType.COSTNOTICE || sample.type == NoticeType.TIME_RECHARGE ||
			sample.type == NoticeType.NEW_RECHARGE || sample.type == NoticeType.NEW_CONSUME) {
			for (int i=0; i<nodeList.Count; i++) {
				if (nodeList [i] == null)
					continue;
				NoticeActivityRechargeBarCtrl button = nodeList [i].GetComponent<NoticeActivityRechargeBarCtrl> ();
				button.showTime ();
			}
		} else if (sample.type == NoticeType.CONSUME_REBATE) {
			//消费返利条目暂时没有时间限制
		} 
		
	}

	public void reLoad ()
	{
		if (sample.type == NoticeType.EXCHANGENOTICE) {
			exchangeList = ExchangeManagerment.Instance.getCanUseExchanges ((sample.content as SidNoticeContent).sids, sample.type, false);
			if (exchangeList == null || exchangeList.Count <= 0) {
				cleanAll ();
				//无内容退出
				return;
			}
			clearExchangeOver ();
			base.reLoad (exchangeList.Count);
		} else if (sample.type == NoticeType.TOPUPNOTICE || sample.type == NoticeType.COSTNOTICE || sample.type == NoticeType.TIME_RECHARGE || sample.type == NoticeType.ONE_MANY_RECHARGE) {
			rechargeList = RechargeManagerment.Instance.getCanUseRecharges ((sample.content as SidNoticeContent).sids);
			clearRechargeOver ();
			base.reLoad (rechargeList.Count);
		} else if (sample.type == NoticeType.CONSUME_REBATE) {
			base.reLoad (consumeRebateList.Count);
		} else if (sample.type == NoticeType.NEW_RECHARGE || sample.type == NoticeType.NEW_CONSUME) {
			rechargeList = NoticeActiveManagerment.Instance.getRechargeList ((sample.content as SidNoticeContent).sids);
			base.reLoad (rechargeList.Count,getRechargeAwardIndex());
		} else if (sample.type == NoticeType.NEW_EXCHANGE) {
			exchangeList = NoticeActiveManagerment.Instance.getExchangeList ((sample.content as NewExchangeNoticeContent).actives);
			base.reLoad (exchangeList.Count);
		}
	}
	
	public override void updateItem (GameObject item, int index)
	{
		if (sample.type == NoticeType.EXCHANGENOTICE || sample.type == NoticeType.NEW_EXCHANGE) {
			NoticeActivityExchangeBarCtrl ctrl = item.GetComponent<NoticeActivityExchangeBarCtrl> ();
			ctrl.updateItem (exchangeList [index]);
		} else if (sample.type == NoticeType.TOPUPNOTICE || sample.type == NoticeType.COSTNOTICE || sample.type == NoticeType.TIME_RECHARGE ||
			sample.type == NoticeType.NEW_RECHARGE || sample.type == NoticeType.NEW_CONSUME) {
			NoticeActivityRechargeBarCtrl ctrl = item.GetComponent<NoticeActivityRechargeBarCtrl> ();
			ctrl.updateItem (rechargeList [index], sample,notice);
		} else if (sample.type == NoticeType.CONSUME_REBATE) {
			NoticeActivityShopBarCtrl ctrl = item.GetComponent<NoticeActivityShopBarCtrl> ();
			ctrl.updateItem (consumeRebateList [index], sample);
        }
        else if (sample.type == NoticeType.ONE_MANY_RECHARGE){
            NoticeOneManyRechargeItem itemCtrl = item.GetComponent<NoticeOneManyRechargeItem>();
            itemCtrl.updateItem(rechargeList[index], sample, notice);
        }
		 
	}

	public override void initButton (int  i)
	{
		if (sample.type == NoticeType.EXCHANGENOTICE || sample.type == NoticeType.NEW_EXCHANGE) {
			if (nodeList [i] == null) {
				nodeList [i] = NGUITools.AddChild (gameObject, (activityBase as NoticeActivityExchangeContent).NoticeActivityExchangePrefab);
			}
			nodeList [i].name = StringKit.intToFixString (i + 1);
			NoticeActivityExchangeBarCtrl ctrl = nodeList [i].GetComponent<NoticeActivityExchangeBarCtrl> ();
			ctrl.fatherWindow = activityBase as NoticeActivityExchangeContent;
			ctrl.updateItem (exchangeList [i]);
				
		} else if (sample.type == NoticeType.TOPUPNOTICE || sample.type == NoticeType.COSTNOTICE || sample.type == NoticeType.TIME_RECHARGE ||
			sample.type == NoticeType.NEW_RECHARGE || sample.type == NoticeType.NEW_CONSUME) {
			if (nodeList [i] == null) {
				nodeList [i] = NGUITools.AddChild (gameObject, (activityBase as NoticeActivityRechargeContent).NoticeActivityRechargePrefab);
			}
 
			nodeList [i].name = StringKit.intToFixString (i + 1);
			NoticeActivityRechargeBarCtrl ctrl = nodeList [i].GetComponent<NoticeActivityRechargeBarCtrl> ();
			ctrl.fatherWindow = activityBase as NoticeActivityRechargeContent;
			ctrl.updateItem (rechargeList [i], sample,notice);
		} else if (sample.type == NoticeType.CONSUME_REBATE) {
			if (nodeList [i] == null) {
				nodeList [i] = NGUITools.AddChild (gameObject, (activityBase as NoticeConsumeRebateContent).noticeActivityShopPrefab);
			}
			nodeList [i].name = StringKit.intToFixString (i + 1);
			NoticeActivityShopBarCtrl ctrl = nodeList [i].GetComponent<NoticeActivityShopBarCtrl> ();
			ctrl.fatherContent = activityBase as NoticeConsumeRebateContent;
			ctrl.updateItem (consumeRebateList [i], sample);
        } else if (sample.type == NoticeType.ONE_MANY_RECHARGE){
            if (nodeList[i] == null)
            {
                nodeList[i] = NGUITools.AddChild(gameObject, (activityBase as NoticeOneManyRechargeContent).NoticeOneManyRechargePrefab);
            }
            nodeList[i].name = StringKit.intToFixString(i + 1);
            NoticeOneManyRechargeItem ctrl = nodeList[i].GetComponent<NoticeOneManyRechargeItem>();
            ctrl.fatherWindow = activityBase as NoticeOneManyRechargeContent;
            ctrl.updateItem(rechargeList[i], sample, notice);
        }
	}
	
	//转换时间格式 单位:秒  
	private string timeTransform (double time)
	{  
		int days = (int)(time / (3600 * 24));
		string dStr = "";
		if (days != 0)
			dStr = days + LanguageConfigManager.Instance.getLanguage ("s0018");
		
		int hours = (int)(time % (3600 * 24) / 3600);
		string hStr = "";
		if (hours != 0)
			hStr = hours + LanguageConfigManager.Instance.getLanguage ("s0019");
		
		int minutes = (int)(time % (3600 * 24) % 3600 / 60);
		string mStr = "";
		if (minutes != 0)
			mStr = minutes + LanguageConfigManager.Instance.getLanguage ("s0020");
		
		int seconds = (int)(time % (3600 * 24) % 3600 % 60);
		string sStr = "";
		if (seconds != 0)
			sStr = seconds + LanguageConfigManager.Instance.getLanguage ("s0021");
		
		return dStr + hStr + mStr + sStr;
	}

	// 得到累计充值、消费可领取的下标//
	public int getRechargeAwardIndex()
	{
		for(int i= 0;i<rechargeList.Count;i++)
		{
			if(rechargeList[i].isComplete () && rechargeList[i].isRecharge())
				return i;
		}
		return 0;
	}

	// 得到vip可兑换下标//
	public int getVipExchange()
	{
		for(int i=0;i<exchangeList.Count;i++)
		{
			bool isActive = (exchangeList[i].getStartTime () == 0 && exchangeList[i].getEndTime () == 0)||(exchangeList[i].getStartTime () == 0 && exchangeList[i].getEndTime () > 0 && ServerTimeKit.getSecondTime () < exchangeList[i].getEndTime ())
				||(exchangeList[i].getStartTime () > 0 && exchangeList[i].getEndTime () == 0 && ServerTimeKit.getSecondTime () > exchangeList[i].getStartTime ())
					||(exchangeList[i].getStartTime () > 0 && exchangeList[i].getEndTime () > 0 && ServerTimeKit.getSecondTime () > exchangeList[i].getStartTime () && ServerTimeKit.getSecondTime () < exchangeList[i].getEndTime ());

			bool conditions = !ExchangeManagerment.Instance.isCheckConditions (exchangeList[i].getExchangeSample()) || (exchangeList[i].getExchangeSample().times != 0 && exchangeList[i].getNum () >= exchangeList[i].getExchangeSample().times);

			bool premises = !ExchangeManagerment.Instance.isCheckPremises (exchangeList[i].getExchangeSample());

			if(isActive || !conditions || !premises)
				return i;
		}
		return 0;
	}
}
