using System;
using System.Collections;
using System.Collections.Generic;
 
/**
 * 抽奖管理器 维护所有抽奖信息
 * @author longlingquan
 * */
public class LuckyDrawManagerment
{
	private LuckyDraw[] array = null;

	public LuckyDrawManagerment ()
	{ 

	}
	 
	public static LuckyDrawManagerment Instance {
		get{ return SingleManager.Instance.getObj ("LuckyDrawManagerment") as LuckyDrawManagerment;}
	}
	 
	//初始化所有抽奖信息
	public void createAllLuckyDraw ()
	{
        int[] sids = LuckyDrawSampleManager.Instance.getAllLuckyDarwIds();
		array = new LuckyDraw[sids.Length];
		for (int i = 0; i < sids.Length; i++) {
			array [i] = new LuckyDraw (sids [i]);	
		}
	}
	
	//更新抽奖信息
	public void updateAllLuckyDraw (ErlArray erlarr)
	{
		if (array == null || array.Length < 1)
			createAllLuckyDraw ();
		for (int i = 0; i < erlarr.Value.Length; i++) {
			updateLuckyDraw (erlarr.Value [i] as ErlArray);
		}
	}
	
	//更新单个抽奖信息
	public void updateLuckyDraw (ErlArray erlarr)
	{ 
		if (array == null || array.Length < 1)
			throw new Exception (GetType () + "updateLuckyDraw error! array is null");
		if (erlarr == null || erlarr.Value.Length < 1)
			return;
		int _sid = StringKit.toInt (erlarr.Value [0].getValueString ()); //大条目sid
		for (int i=0; i<array.Length; i++) {
			if (array [i].sid != _sid)
				continue;
			array [i].updateFreeNum (StringKit.toInt (erlarr.Value [1].getValueString ()));//免费次数
			array [i].setNextFreeTime (StringKit.toInt (erlarr.Value [3].getValueString ()));//下次免费抽奖时间戳
			ErlArray arr = erlarr.Value [2] as ErlArray; //具体抽奖信息
			ErlArray arr2;
			if (arr != null) {
				for (int j = 0; j < array[i].ways.Length; j++) {
					for (int k=0; k<arr.Value.Length; k++) {
						arr2 = arr.Value [k] as ErlArray;
						if (arr2 != null) {
							if (array [i].ways [j].getDrawTypeId () == StringKit.toInt (arr2.Value [0].getValueString ())) {
								array [i].ways [j].updateNum (StringKit.toInt (arr2.Value [1].getValueString ()));
							}
						}
					}
				}
			}
			break; //处理后跳出
		}
	}
	
	//获得主界面抽奖信息
	public LuckyDraw[] getLuckyDrawArr ()
	{ 
		List<LuckyDraw> list = new List<LuckyDraw> ();
		int max = array.Length;
		for (int i = 0; i < max; i++) {
			LuckyDrawSample sample = LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (array [i].sid);
			bool isValid = isValidLuckyDrawSample (sample);
			if (!isValid)
				continue;
			if (array [i].isShow () && sample.drawType == 1 && sample.idsType == "1")
				list.Add (array [i]);
		}
		return list.ToArray ();
	}
	/** 是否为有效的抽奖条目 */
	public bool isValidLuckyDrawSample (LuckyDrawSample sample)
	{
		if (sample.noticeSid != 0) {
			NoticeSample tmp = NoticeSampleManager.Instance.getNoticeSampleBySid(sample.noticeSid);
			Notice notice = NoticeManagerment.Instance.getValidNoticeBySid (sample.noticeSid,tmp.entranceId);
			if (notice == null || !notice.isValid ())
				return false;
			if (notice is LuckyDrawNotice) { // 有效的活动抽奖条目,更新抽奖模板时间
				LuckyDrawNotice ldn = notice as LuckyDrawNotice;
				int noticeOpenTime = ldn.activeTime.getDetailStartTime ();
				int remainTime = noticeOpenTime - ServerTimeKit.getSecondTime ();
				if (remainTime <= 0) {
					int noticeCloseTime = ldn.activeTime.getDetailEndTime ();
					int remainCloseTime = noticeCloseTime - ServerTimeKit.getSecondTime ();
					if (remainCloseTime <= 0) {
						sample.startTime = 0;
						sample.endTime = 0;
					} else {
						sample.startTime = 0;
						sample.endTime = noticeCloseTime;
					}
				} else {
					sample.startTime = noticeOpenTime;
					sample.endTime = 0;
				}
			}
			return true;
		}
		return true;
	}

	public LuckyDraw getStarLuckyDraw ()
	{
		int max = array.Length;
		for (int i = 0; i < max; i++) {
			LuckyDrawSample sample = LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (array [i].sid);
			if (array [i].isShow () && sample.drawType == 2)
				return array [i];
		}
		return null;
	}
	
	public LuckyDraw getLuckyDrawBySid (int sid)
	{
		int max = array.Length;
		for (int i = 0; i < max; i++) {
			if (array [i].sid == sid)
				return array [i];
		}
		return null;
	}

	public bool HasFree(){
		foreach (LuckyDraw each in array) 
			if (each.canFreeCD ())
				return true;
		return false;
	}

	public void updateNextTime(int sid,int nextTime)
	{
		foreach (LuckyDraw each in array) {
			if (each.sid == sid) {
				each.updateNextFreeTime(nextTime);
				return;
			}
		}
	}
} 

