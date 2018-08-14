using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentBackRecharge : dynamicContent
{ 
	//List<Recharge> rechargeList;
	List<BackRecharge> rechargeList;
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
		//rechargeList = NoticeActiveManagerment.Instance.getRechargeList ((sample.content as SidNoticeContent).sids);
		if(rechargeList == null)
		{
			rechargeList = new List<BackRecharge>();
		}
		rechargeList.Clear();
		foreach (KeyValuePair<int,BackRecharge> item in BackPrizeRechargeInfo.Instance.rechargeIitems)
		{
			item.Value.num = BackPrizeRechargeInfo.Instance.rechargeNum;
			rechargeList.Add(item.Value);
		}
	}

	public void Initialize ()
	{
		if (rechargeList == null)
			return;
		reLoad (rechargeList.Count);
	}

	public void reLoad ()
	{
		if(rechargeList == null)
		{
			rechargeList = new List<BackRecharge>();
		}
		rechargeList.Clear();
		foreach (KeyValuePair<int,BackRecharge> item in BackPrizeRechargeInfo.Instance.rechargeIitems)
		{
			rechargeList.Add(item.Value);
		}
		base.reLoad (rechargeList.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		BackRechargeBarCtrl ctrl = item.GetComponent<BackRechargeBarCtrl> ();
		ctrl.updateItem (rechargeList[index],sample,notice);
	}

	public override void initButton (int  i)
	{
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, (activityBase as BackRechargeContent).backRechargePrefab);
		}
		
		nodeList [i].name = StringKit.intToFixString (i + 1);
		BackRechargeBarCtrl ctrl = nodeList [i].GetComponent<BackRechargeBarCtrl> ();
		ctrl.fatherWindow = activityBase as BackRechargeContent;
		ctrl.updateItem (rechargeList[i],sample,notice);
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
}
