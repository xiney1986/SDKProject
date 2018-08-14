using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
 
/**
 * 金钱,经验等非随时间变化而变化的资源更新服务
 * @author longlingquan
 * */
public class UpdateService:BaseFPort
{

	public UpdateService ()
	{
		
	}
	
	public override void read (ErlKVMessage message)
	{  
		if (message.getValue ("money") != null) {
			int m = StringKit.toInt ((message.getValue ("money") as ErlType).getValueString ());
			UserManager.Instance.self.updateMoney (m);
			IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_BEAST);
			IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
		}
		/** 公会战行动值 */
		else if(message.getValue("power")!=null){
			int addPower  =   StringKit.toInt ((message.getValue ("power") as ErlType).getValueString ());
			string des  =LanguageConfigManager.Instance.getLanguage("GuildArea_51",addPower.ToString());
			UiManager.Instance.createMessageLintWindowNotUnLuck(des);
			UserManager.Instance.self.guildFightPower = Mathf.Min (UserManager.Instance.self.guildFightPower + addPower,
			                                                       UserManager.Instance.self.guildFightPowerMax);
		}
		else if (message.getValue ("rmb") != null) {
			int m = StringKit.toInt ((message.getValue ("rmb") as ErlType).getValueString ());
            if (UiManager.Instance.rechargeWWindow != null) {
                UiManager.Instance.rechargeWWindow.updateRMB();
            }
			UserManager.Instance.self.updateRMB (m);
		} else if (message.getValue ("merit") != null) {
			int m = StringKit.toInt ((message.getValue ("merit") as ErlType).getValueString ());
			UserManager.Instance.self.merit = m;
		} else if (message.getValue ("contribution") != null) {
			int m = StringKit.toInt ((message.getValue ("contribution") as ErlType).getValueString ());
			GuildManagerment.Instance.updateContrition (m);
		} else if (message.getValue ("exp") != null) {
			long m = StringKit.toLong ((message.getValue ("exp") as ErlType).getValueString ());
			UserManager.Instance.self.updateExp (m);
		} else if (message.getValue ("vip_exp") != null) {
			long m = StringKit.toLong ((message.getValue ("vip_exp") as ErlType).getValueString ());
			UserManager.Instance.self.updateVipExp (m);
		} else if (message.getValue ("max_pve") != null) {
			int m = StringKit.toInt ((message.getValue ("max_pve") as ErlType).getValueString ());
			UserManager.Instance.self.setPvEPointMax (m);
		} else if (message.getValue ("star") != null) {
			int m = StringKit.toInt ((message.getValue ("star") as ErlType).getValueString ());
			UserManager.Instance.self.updateStarSum (m);
		} else if (message.getValue ("friend_size") != null) {
			int m = StringKit.toInt ((message.getValue ("friend_size") as ErlType).getValueString ());
			FriendsManagerment.Instance.getFriends ().updateMaxSize (m);
		} else if (message.getValue ("card") != null) {
			ErlArray er = message.getValue ("card") as ErlArray;
			string uid = er.Value [0].getValueString ();
			StorageManagerment.Instance.updateCard (uid, er);
		} else if (message.getValue ("star_score") != null) {
			int m = StringKit.toInt ((message.getValue ("star_score") as ErlType).getValueString ());
			GoddessAstrolabeManagerment.Instance.setStarScore(m);
		}
		else if (message.getValue ("honor") != null) {
			int m = StringKit.toInt ((message.getValue ("honor") as ErlType).getValueString ());
			UserManager.Instance.self.honor = m;
		}
		else if (message.getValue ("front") != null) {
			string content = (message.getValue ("front") as ErlType).getValueString ();
			NoticeWindow noticeWindow = UiManager.Instance.noticeWindow;
			if (noticeWindow != null) {
				UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0463"));
					NoticeSampleManager.Instance.loadNoticeSample (content);
					noticeWindow.NextFrameInitTopButton (true, 0);
				});
			} else {
				NoticeSampleManager.Instance.loadNoticeSample (content);
			}
		} else if (message.getValue ("month_card") != null) {
			ErlType msg = message.getValue ("month_card") as ErlType;
			if (msg is ErlArray) {
				ErlArray parameters = msg as ErlArray;

				ErlArray receiveTime = parameters.Value [0] as ErlArray;
				int canReceiveTime = StringKit.toInt (parameters.Value [0].getValueString ());
				DateTime time = TimeKit.getDateTimeMin (canReceiveTime);
				int receiveYear = time.Year;
				int receiveMonth = time.Month;
				int receiveDay = time.Day;
				NoticeManagerment.Instance.monthCardDueDate = new int[3] {
					receiveYear,
					receiveMonth,
					receiveDay
				};
				NoticeManagerment.Instance.monthCardDueSeconds = canReceiveTime;

				int canReceiveEnable = StringKit.toInt (parameters.Value [1].getValueString ());
				NoticeManagerment.Instance.monthCardDayRewardEnable = canReceiveEnable == 1;

				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("monthCardBuySuccessTip"));
			}
		}else if (message.getValue("cash_double")!=null)
		{
		    if (UiManager.Instance.rechargeWWindow!=null)
		    {
		        UiManager.Instance.rechargeWWindow.updateRMB();
		    }
		}
        else if (message.getValue ("cash_first") != null) {
			RechargeManagerment.Instance.canFirst = false;
			if(UiManager.Instance.mainWindow!=null)
				UiManager.Instance.mainWindow.updateOneRmb();
		}else if(message.getValue("weekend")!=null){
			ErlType msg=message.getValue("weekend") as ErlType;
			int flag=StringKit.toInt(msg.getValueString());
			if((flag==0&&!TotalLoginManagerment.Instance.WeeklyState)||(flag==1&&TotalLoginManagerment.Instance.WeeklyState)){
				TotalLoginManagerment.Instance.WeeklyState=flag==0;
				if(UiManager.Instance.getWindow<TotalLoginWindow>()!=null){
					if(flag==1&&!TotalLoginManagerment.Instance.HolidayState&&!TotalLoginManagerment.Instance.EverydayState){
						if(UiManager.Instance.getWindow<TotalLoginWindow> ().IsFirstBoot){
							UiManager.Instance.getWindow<TotalLoginWindow>().finishWindow();
							UiManager.Instance.openWindow<MainWindow>();
						}
						UiManager.Instance.getWindow<TotalLoginWindow>().finishWindow();
					}else{
						bool fl=UiManager.Instance.getWindow<TotalLoginWindow>().IsFirstBoot;
						UiManager.Instance.getWindow<TotalLoginWindow>().destoryWindow();
						if(fl){
							WeeklyAwardFPort fport=FPortManager.Instance.getFPort<WeeklyAwardFPort> ();
							fport.access(openWindd);
						}else{
							UiManager.Instance.openWindow<TotalLoginWindow>((win) => {
								win.Initialize ();
							});
						}
					}
				}else if(UiManager.Instance.getWindow<MainWindow>()!=null&&UiManager.Instance.getWindow<MainWindow>().gameObject.activeSelf){
					UiManager.Instance.getWindow<MainWindow>().totalPrizeButton.gameObject.SetActive(TotalLoginManagerment.Instance.isShowPrize ());
				}
			}
		}else if(message.getValue("festival")!=null){
			ErlType msg=message.getValue("festival") as ErlType;
			int flag=StringKit.toInt(msg.getValueString());
			if ((flag == 0 && !TotalLoginManagerment.Instance.HolidayState) || (flag == 1 && TotalLoginManagerment.Instance.HolidayState)) {
				TotalLoginManagerment.Instance.HolidayState=flag==0;
				if(UiManager.Instance.getWindow<TotalLoginWindow>()!=null){
					if(!TotalLoginManagerment.Instance.WeeklyState&&flag==1&&!TotalLoginManagerment.Instance.EverydayState){
						if(UiManager.Instance.getWindow<TotalLoginWindow> ().IsFirstBoot){
							UiManager.Instance.getWindow<TotalLoginWindow>().finishWindow();
							UiManager.Instance.openWindow<MainWindow>();
						}
						UiManager.Instance.getWindow<TotalLoginWindow>().finishWindow();
					}else{
						bool fl=UiManager.Instance.getWindow<TotalLoginWindow>().IsFirstBoot;
						UiManager.Instance.getWindow<TotalLoginWindow>().destoryWindow();
						if(fl){
							HolidayAwardFPort fport=FPortManager.Instance.getFPort<HolidayAwardFPort>();
							fport.access(TotalLoginManagerment.Instance.getHolidayActionsTate(),openWindd);
						}else{
							UiManager.Instance.openWindow<TotalLoginWindow>((win) => {
								win.Initialize ();
							});
						}
					}
				}else if(UiManager.Instance.getWindow<MainWindow>()!=null&&UiManager.Instance.getWindow<MainWindow>().gameObject.activeSelf){
					UiManager.Instance.getWindow<MainWindow>().totalPrizeButton.gameObject.SetActive(TotalLoginManagerment.Instance.isShowPrize ());
				}
			}
		}
		else if (message.getValue ("login_award") != null) {
			ErlType msg	= message.getValue ("login_award") as ErlType;
			int flag= StringKit.toInt (msg.getValueString ());
			if ((flag == 0 && !TotalLoginManagerment.Instance.EverydayState) || (flag == 1 && TotalLoginManagerment.Instance.EverydayState)) {
				TotalLoginManagerment.Instance.EverydayState=flag==0;
				if (UiManager.Instance.getWindow<TotalLoginWindow> () != null) {
					if(!TotalLoginManagerment.Instance.WeeklyState&&!TotalLoginManagerment.Instance.HolidayState&&flag==1){
						if(UiManager.Instance.getWindow<TotalLoginWindow> ().IsFirstBoot){
							UiManager.Instance.getWindow<TotalLoginWindow>().finishWindow();
							UiManager.Instance.openWindow<MainWindow>();
						}
						UiManager.Instance.getWindow<TotalLoginWindow>().finishWindow();
					}else{
						bool fl=UiManager.Instance.getWindow<TotalLoginWindow> ().IsFirstBoot;
						UiManager.Instance.getWindow<TotalLoginWindow> ().destoryWindow ();
						if (fl) {
							TotalLoginFPort fport=FPortManager.Instance.getFPort<TotalLoginFPort> ();
							fport.access (openWindd);
						}
						else {
							TotalLoginFPort fport=FPortManager.Instance.getFPort<TotalLoginFPort> ();
							fport.access (openWindd);
						}
					}
				}else if(UiManager.Instance.getWindow<MainWindow>()!=null&&UiManager.Instance.getWindow<MainWindow>().gameObject.activeSelf){
					UiManager.Instance.getWindow<MainWindow>().totalPrizeButton.gameObject.SetActive(TotalLoginManagerment.Instance.isShowPrize ());
				}
			}
		}
		else if(message.getValue("active_limit") !=null){
			ErlArray msg = message.getValue ("active_limit") as ErlArray;
			ErlArray integral = msg.Value[1] as ErlArray;
			if(integral.Value.Length > 0){
				for(int i=0;i<integral.Value.Length;i++){
					NoticeLimitAwardInfo tmp = new NoticeLimitAwardInfo();
					tmp.sid = StringKit.toInt(msg.Value[0].getValueString());
					tmp.integral = StringKit.toInt(integral.Value[i].getValueString());
					NoticeManagerment.Instance.addNoticeLimitInfo(tmp);
//					NoticeManagerment.Instance.addNoticeLimit(StringKit.toInt(msg.Value[0].ToString()));
//					NoticeManagerment.Instance.addNoticeLimit(StringKit.toInt(integral.Value[i].ToString()));
				}
			}
//			NoticeManagerment.Instance.noticeLimit.Add(StringKit.toInt(msg.Value[0].ToString()));
//			NoticeManagerment.Instance.noticeLimit.Add(StringKit.toInt(msg.Value[1].ToString()));//【5，10，20】
		}

		else if(message.getValue("update_task") !=null)// 更新七日狂欢状态//
		{
			SevenDaysHappyMisson misson;
			ErlArray missonInfo;
			ErlArray progressInfo;
			ErlArray msg = message.getValue ("update_task") as ErlArray;
			for(int i=0;i<msg.Value.Length;i++)
			{
				missonInfo = msg.Value[i] as ErlArray;
				int missonID = StringKit.toInt(missonInfo.Value[0].getValueString());// 任务id//
				progressInfo = missonInfo.Value[2] as ErlArray;// 任务进度//
				if(SevenDaysHappyManagement.Instance.getSevenDaysHappySampleDic().Count > 0)
				{
					misson = SevenDaysHappyManagement.Instance.getMissonByMissonID(missonID);
					updateSevenDaysHappy(misson,missonInfo,progressInfo);
				}
//				else
//				{
//					SevenDaysHappyInfoFPort fport = FPortManager.Instance.getFPort ("SevenDaysHappyInfoFPort") as SevenDaysHappyInfoFPort;
//					fport.SevenDaysHappInfoAccess(()=>{
//						misson = SevenDaysHappyManagement.Instance.getMissonByMissonID(missonID);
//						updateSevenDaysHappy(misson,missonInfo,progressInfo);
//					});
//					Debug.Log("update service report.............");
//				}
			}
			// 排序//
			SevenDaysHappyManagement.Instance.sortMisson();
		}

		// 刷排行榜//
		else if(message.getValue("update_ranklist") !=null)
		{
			ErlType msg = message.getValue("update_ranklist") as ErlType;
			// 排行榜类型//
			int rankType = StringKit.toInt(msg.getValueString());
			cleanRankByType(rankType);
			// 如果是恶魔挑战排行榜//
			if(rankType == RankManagerment.TYPE_BOSSDAMAGE)
			{
				RankManagerment.Instance.updateRankItemTotalDamage = true;
			}
		}
	}

	// 根据排行榜类型清理排行榜//
	private void cleanRankByType(int rankType)
	{
		if(RankManagerment.Instance.nextTime.ContainsKey (rankType))
		{
			RankManagerment.Instance.nextTime.Remove(rankType);
		}
	}

	private void updateSevenDaysHappy(SevenDaysHappyMisson misson,ErlArray missonInfo,ErlArray progressInfo)
	{
		if(misson != null)
		{
			int missonState = StringKit.toInt(missonInfo.Value[1].getValueString());// 任务状态//
			// 更新状态//
			if(missonState == 0)// 进行中//
			{
				misson.missonState = SevenDaysHappyMissonState.Doing;
			}
			else if(missonState == 1)// 已经完成,还没有领奖//
			{
				misson.missonState = SevenDaysHappyMissonState.Completed;
				if(misson.dayID <= SevenDaysHappyManagement.Instance.getDayIndex())
				{
					SevenDaysHappyManagement.Instance.canReceviedCount++;
				}
				if(!SevenDaysHappyManagement.Instance.dayIDAndCount.ContainsKey(misson.dayID))
				{
					SevenDaysHappyManagement.Instance.dayIDAndCount.Add(misson.dayID,1);
				}
				else
				{
					SevenDaysHappyManagement.Instance.dayIDAndCount[misson.dayID]++;
				}
			}
			else if(missonState == 2)// 已经领取奖励//
			{
				misson.missonState = SevenDaysHappyMissonState.Recevied;
			}
			// 更新进度//
			if(progressInfo.Value.Length > 0 )
			{
				for(int j=0;j<progressInfo.Value.Length;j++)
				{
					misson.missonProgress[j] = StringKit.toInt(progressInfo.Value[j].getValueString());
				}
			}
		}
	}

	/**通信完成回调方法 */
	private void openWindd(){
		UiManager.Instance.openWindow<TotalLoginWindow>((win) => {
			win.Initialize (true);
		});
	}
	/** 更新公告测试方法 */
	private string updateNoticeTest (string contentMessage)
	{
		string[] texts = contentMessage.Split ('\n');
		ArrayList list = new ArrayList ();
		for (int i=0; i<texts.Length; i++) {
			list.Add (texts [i]);
		}
		int randomCount = UnityEngine.Random.Range (list.Count - 3, list.Count);
		string content = "";
		for (int i=0; i<randomCount; i++) {
			int randomv = UnityEngine.Random.Range (0, list.Count);
			content += list [randomv].ToString () + "\n";
			list.RemoveAt (randomv);
		}
		return content;
	}
}
