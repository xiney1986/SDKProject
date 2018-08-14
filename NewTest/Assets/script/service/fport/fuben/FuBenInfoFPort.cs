using System;
using System.Collections.Generic;

/**
 * 副本信息接口
 * 获得指定类型副本信息
 * @author longlingquan
 * */
public class FuBenInfoFPort:BaseFPort
{
	private const string NONE = "none";//没有过往副本记录 
	private const string INFO = "fbinfo";//有过往副本记录
	private const string KEY_INFO = "info";//副本信息 键
	private CallBack callback;

	public FuBenInfoFPort ()
	{
		
	}
	 
	public void info (CallBack callback, int chapterType)
	{
		if (needSend (chapterType)) {
			this.callback = callback;
			ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_INFO);   
			message.addValue ("type", new ErlInt (chapterType));
			access (message);
		} else {
			if (callback != null)
				callback ();
		}
	}
	public void init (CallBack callback, int chapterType)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_INFO);   
		message.addValue ("type", new ErlInt (chapterType));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{


		parseKVMsg (message);
		if (callback != null)
			callback ();
	} 

	//解析ErlKVMessgae
	public void parseKVMsg (ErlKVMessage message)
	{
		int chapterType = StringKit.toInt ((message.getValue ("type") as ErlType).getValueString ());//副本类型
		ErlArray kt = message.getValue ("kt") as ErlArray;  //活动副本
		ErlArray sids = message.getValue ("sids") as ErlArray;  //副本sid信息，不同副本不同的数据格式
		ErlType number = message.getValue ("number") as ErlType;//讨伐使用次数
		ErlType buy = message.getValue ("buy") as ErlType;//购买次数
		ErlType step = message.getValue ("pstep") as ErlType;//今日修炼最大通关 暂无用
		ErlList storyTimes = message.getValue ("lv") as ErlList;//剧情噩梦难度使用次数
		ErlType practicetime = message.getValue ("practicetime") as ErlType;//修炼副本倒计时
        //==================================爬塔
        ErlType towerState = message.getValue("isfight") as ErlType;//可否挑战
        ErlArray reGetFirstSid = message.getValue("frees") as ErlArray;//已经领取的sid列表
        ErlType reset = message.getValue("reset") as ErlType;//爬塔已经重置的次数
        ErlType lottery = message.getValue("lottery") as ErlType;//已经开宝箱的次数
        ErlType lottery_buy = message.getValue("lottery_buy") as ErlType;//已经购买的开宝箱次数
		if(practicetime!=null)
		{
			FuBenManagerment.Instance.practiceDueTime=StringKit.toInt (practicetime.getValueString ());
		}

		if (chapterType == ChapterType.STORY) {
			int[][] storySids = new int[sids.Value.Length][];
			ErlArray temp;
			for (int i=0; i<storySids.Length; i++) {
				temp = sids.Value [i] as ErlArray;
				storySids [i] = new int[3];
				storySids [i] [0] = StringKit.toInt ((temp.Value [0] as ErlType).getValueString ());//sid
				storySids [i] [1] = StringKit.toInt ((temp.Value [1] as ErlType).getValueString ());//step
				storySids [i] [2] = 0;
			}

			if (storyTimes != null && storySids != null && storySids.Length > 0) {
				int sidTemp;
				ErlArray tempErl;
				for (int i = 0; i < storyTimes.Value.Length; i++) {
					tempErl = storyTimes.Value [i] as ErlArray;
					sidTemp = StringKit.toInt ((tempErl.Value [0] as ErlType).getValueString ());//sid
					for (int j = 0; j < storySids.Length; j++) {
						if (storySids [j] [0] == sidTemp) {
							storySids [j] [2] = StringKit.toInt ((tempErl.Value [1] as ErlType).getValueString ());
						}
					}
				}
			}
			if (needSend (chapterType))
				FuBenManagerment.Instance.updateStory (storySids);
		} else if (chapterType == ChapterType.WAR) {
			int[] warSids = new int[sids.Value.Length];
			for (int i=0; i<warSids.Length; i++) {
				warSids [i] = StringKit.toInt ((sids.Value [i] as ErlType).getValueString ());
			}
			FuBenManagerment.Instance.updateWar (warSids, StringKit.toInt (number.getValueString ()), StringKit.toInt (buy.getValueString ()));
        } else if (chapterType == ChapterType.TOWER_FUBEN) {//初始化爬塔信息
            int[] towerSid = new int[sids.Value.Length];//爬塔通关mission sid
            int[] firtReadySid = new int[reGetFirstSid.Value.Length];//已经领取首次宝箱的
            for (int i = 0; i < towerSid.Length;i++ ) {
                towerSid[i] = StringKit.toInt((sids.Value[i] as ErlType).getValueString());
            }
            for (int j = 0; j < firtReadySid.Length; j++) {
                firtReadySid[j] = StringKit.toInt((reGetFirstSid.Value[j] as ErlType).getValueString());
            }
            FuBenManagerment.Instance.updateTower(towerSid, StringKit.toInt(towerState.getValueString()), firtReadySid, StringKit.toInt(reset.getValueString()), StringKit.toInt(lottery.getValueString()),StringKit.toInt(lottery_buy.getValueString()));
        } 
        else if (chapterType == ChapterType.PRACTICE) {
			int[] praSids = new int[sids.Value.Length];
			for (int i=0; i<praSids.Length; i++) {
				praSids [i] = StringKit.toInt ((sids.Value [i] as ErlType).getValueString ());
			}
			FuBenManagerment.Instance.updatePractice (praSids, StringKit.toInt (number.getValueString ()));
		} else if (chapterType == ChapterType.ACTIVITY_CARD_EXP || chapterType == ChapterType.ACTIVITY_EQUIP_EXP ||
			chapterType == ChapterType.ACTIVITY_MONEY || chapterType == ChapterType.ACTIVITY_SKILL_EXP) {
			int[][] activeSids = new int[kt.Value.Length][];
			ErlArray temp;
			for (int i=0; i<activeSids.Length; i++) {
				temp = kt.Value [i] as ErlArray;
				activeSids [i] = new int[5];
				activeSids [i] [0] = StringKit.toInt ((temp.Value [0] as ErlType).getValueString ());//类型
				activeSids [i] [1] = StringKit.toInt ((temp.Value [1] as ErlType).getValueString ());//次数
				activeSids [i] [2] = StringKit.toInt ((temp.Value [2] as ErlType).getValueString ());//最大次数
                

				ErlList timeInfo = temp.Value [3] as ErlList;
                activeSids[i][3] = StringKit.toInt((temp.Value[4] as ErlType).getValueString());//已经购买次数
				FuBenManagerment.Instance.updateActivityChapters (activeSids [i] [0], activeSids [i] [1], activeSids [i] [2],
                                                                  StringKit.toInt((temp.Value[3] as ErlType).getValueString()), activeSids[i][3]); 
				//	activeSids [i] [3] = StringKit.toInt ((temp.Value [3] as ErlType).getValueString ());//开始时间
				//	activeSids [i] [4] = StringKit.toInt ((temp.Value [4] as ErlType).getValueString ());//结束时间
			}
		
		}
	}

	//是否需要发送获取信息请求 true表示需要 活动类型暂时如此处理
	private bool needSend (int chapterType)
	{
		switch (chapterType) {
		case ChapterType.STORY:
			return FuBenManagerment.Instance.getStoryInfos () == null;
		case ChapterType.WAR:
			return FuBenManagerment.Instance.getWarInfos () == null;
		case ChapterType.PRACTICE:
			return FuBenManagerment.Instance.getPracticeInfos () == null;
            case ChapterType.TOWER_FUBEN:
            return true;
		case ChapterType.ACTIVITY_CARD_EXP:
			return true;
		case ChapterType.ACTIVITY_EQUIP_EXP:
			return true;
		case ChapterType.ACTIVITY_MONEY:
			return true;
		case ChapterType.ACTIVITY_SKILL_EXP:
			return true;
		default:
			return false;
		}
	}
} 

