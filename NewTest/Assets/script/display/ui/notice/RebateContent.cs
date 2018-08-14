using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RebateContent : MonoBase 
{

	public ButtonBase checkRuleButton;//  查看规则按钮//
	public UILabel timeLabel;// 活动限时时间区间//
	public UILabel describLabel;// 活动描述//
	public GameObject detailPanel;// 详情面板//
	public GameObject detailInfo;// 详情内容//
	public GameObject ruleInfo;//  规则内容//
	public ButtonBase closeDetailButton;// 关闭详情面板按钮//
	public ButtonBase receiveButton;// 领取按钮//
	public UILabel costDiamondCount;// 显示消耗的钻石数量//
	public UILabel costGoldCount;// 显示消耗的金币数量//
	public UILabel getDiamondCount;// 显示返利的钻石数量//
	public UILabel getGoldCount;// 显示返利的金币数量//
	public UISprite recevieBtnBg;// 领取按钮背景//
	public UILabel costDay;// 详情里参加活动开始时间//
	public UILabel getDay;// 详情里参加活动截止时间//
	public ButtonBase closeRule;
	public UILabel ruleDes;
	public GameObject ruleDescTemp;
	public Transform ruleDescPanel;

	public List<RebateSample> diamondSample = new List<RebateSample>();
	public List<RebateSample> goldSample = new List<RebateSample>();

	Dictionary<int,RebateDayInfo> m_infos;

	private RebateDayInfo rdi;

	public WindowBase win;
	private NoticeSample ns;

	int startTime;// 活动开始时间//
	int endTime;// 活动结束时间//

	string canReceive_spriteName = "button_red2";
	string notReceive_spriteName = "button_red2_gray";

	Notice m_notice;

	public GameObject effectObj;

	TimeInfoSample tsample;

	Vector3 pos = new Vector3(-19.2f,-81);
	public GameObject costDiamond;
	public GameObject costGold;
	public GameObject getDiamond;
	public GameObject getGold;

	public void initContent(Notice notice,WindowBase win)
	{
		this.win = win;
		this.ns = notice.getSample();
		this.m_notice = notice;
		tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (ns.timeID);
		startTime = tsample.mainTimeInfo[0];
		endTime = tsample.mainTimeInfo[0] + tsample.mainTimeInfo[1];

		closeDetailButton.fatherWindow = win;
		receiveButton.fatherWindow = win;
		checkRuleButton.fatherWindow = win;
		closeRule.fatherWindow = win;

		closeDetailButton.onClickEvent = closeDetail;
		receiveButton.onClickEvent = receiveButtonClick;
		checkRuleButton.onClickEvent = checkRuleClick;
		closeRule.onClickEvent = closeRuleClick;

		//showTimeLabel();
		showDescribLabel();

		RebateInfoFPort rifp = FPortManager.Instance.getFPort ("RebateInfoFPort") as RebateInfoFPort;
//		List<int> ids = RebateSampleManager.Instance.getAllIDs();
//		diamondSample = RebateSampleManager.Instance.getDiamondSampleByIDs(ids);
//		goldSample = RebateSampleManager.Instance.getGoldSampleByIDs(ids);
		if(rifp.diamondSample == null && rifp.goldSample == null)
		{
			List<int> ids = RebateSampleManager.Instance.getAllIDs();
			rifp.setDiamondSample(RebateSampleManager.Instance.getDiamondSampleByIDs(ids));
			rifp.setGoldSample(RebateSampleManager.Instance.getGoldSampleByIDs(ids));
		}
		rifp.RebateInfoAccess(getRebateInfoCallBack);

		initeRulePanel(rifp.diamondSample,rifp.goldSample);
	}

	public void getRebateInfoCallBack()
	{
		RebateInfoFPort rifp = FPortManager.Instance.getFPort ("RebateInfoFPort") as RebateInfoFPort;
		m_infos = rifp.getInfos();
		init (m_infos);
	}

	public void init(Dictionary<int,RebateDayInfo> infos)
	{
		foreach (KeyValuePair<int,RebateDayInfo> item in infos)
		{
			initRebateItem(item.Value,item.Key);
		}
	}
	// 初始化//
	public void initRebateItem(RebateDayInfo rdi,int i)
	{
		GameObject itemObj;
		RebateItem ri;
		string objName = "item_" + i;
		itemObj = GameObject.Find(objName);
		if(itemObj != null)
		{
			itemObj.transform.FindChild("dayLabel").GetComponent<UILabel>().text = string.Format(LanguageConfigManager.Instance.getLanguage("dayth"),i.ToString());
			ri = itemObj.GetComponent<RebateItem>();
			ri.setRebateContent(this);
			ri.updateRebateItem(rdi);
			ri.fatherWindow = win;
		}
	}

	// 显示活动时间区间文本//
	public void showTimeLabel()
	{
		tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (ns.timeID);
		startTime = tsample.mainTimeInfo[0];
		endTime = tsample.mainTimeInfo[0] + tsample.mainTimeInfo[1];
		DateTime startTimeDate = TimeKit.getDateTime(startTime);
		DateTime endTimeDate = TimeKit.getDateTime(endTime);
		timeLabel.text = startTimeDate.Year + "." + startTimeDate.Month + "." + startTimeDate.Day + "--" + endTimeDate.Year + "." + endTimeDate.Month + "." + endTimeDate.Day;

		//Debug.Log(startTimeDate.Year + "." + startTimeDate.Month + "." + startTimeDate.Day + "~" + endTimeDate.Year + "." + endTimeDate.Month + "." + endTimeDate.Day);
	}
	// 显示活动描述文本//
	public void showDescribLabel()
	{
		//NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(RebateDayInfo.RebateNoticeID);
		if(m_notice != null)
		{
			NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(m_notice.sid);
			string delayTime = sample.order.ToString();
			
			describLabel.text = string.Format(ns.activiteDesc,delayTime);
		}
	}

	public void showDetailInfo()
	{
		RebateInfoFPort rifp = FPortManager.Instance.getFPort ("RebateInfoFPort") as RebateInfoFPort;
		if(rifp.diamondSample.Count > 0 && rifp.goldSample.Count <= 0)
		{
			costDiamond.transform.localPosition = pos;
			getDiamond.transform.localPosition = pos;

			costDiamond.SetActive(true);
			getDiamond.SetActive(true);
			costGold.SetActive(false);
			getGold.SetActive(false);
		}
		else if(rifp.diamondSample.Count <= 0 && rifp.goldSample.Count > 0)
		{
			costGold.transform.localPosition = pos;
			getGold.transform.localPosition = pos;

			costDiamond.SetActive(false);
			getDiamond.SetActive(false);
			costGold.SetActive(true);
			getGold.SetActive(true);
		}
		detailPanel.SetActive(true);
		detailInfo.SetActive(true);
		ruleInfo.SetActive(false);
		setDetailInfo(true);
		if(rdi.rebateState == RebateState.UN_RECEIVE)// 未领取//
		{
			if(rdi.s_rebateState == S_RebateState.COLLECTING || rdi.s_rebateState == S_RebateState.WAIT_RECEIVE)// 收集中或等待领取中//
			{
				//recevieBtnBg.spriteName = notReceive_spriteName;
				receiveButton.disableButton(true);
			}
			else if(rdi.s_rebateState == S_RebateState.CAN_RECEIVE)
			{
				//recevieBtnBg.spriteName = canReceive_spriteName;
				receiveButton.disableButton(false);
			}
		}
	}

	public void closeDetail(GameObject obj)
	{
		if(effectObj != null)
		{
			effectObj.SetActive(true);
		}
		ruleInfo.SetActive(false);
		setDetailInfo(false);
		detailInfo.SetActive(false);
		detailPanel.SetActive(false);

		rdi = null;
	}

	public void closeRuleClick(GameObject obj)
	{
		if(effectObj != null)
		{
			effectObj.SetActive(true);
		}
		ruleInfo.SetActive(false);
		setDetailInfo(false);
		detailInfo.SetActive(false);
		detailPanel.SetActive(false);
	}

	public void setDetailInfo(bool b)
	{
		string[] strArr = new string[2];
		if(b)
		{
			DateTime dt_start = TimeKit.getDateTime(rdi.startTime);
			DateTime dt_end = TimeKit.getDateTime(rdi.endTime);

			strArr[0] = dt_start.Month.ToString();
			strArr[1] = dt_start.Day.ToString();
			costDay.text = string.Format(LanguageConfigManager.Instance.getLanguage("costRecord"),strArr);

			strArr[0] = dt_end.Month.ToString();
			strArr[1] = dt_end.Day.ToString();
			getDay.text = string.Format(LanguageConfigManager.Instance.getLanguage("canRecevie"),strArr);

			costDiamondCount.text = rdi.costDiamond.ToString();
			costGoldCount.text = rdi.costGold.ToString();
			getDiamondCount.text = rdi.getDiamond.ToString();
			getGoldCount.text = rdi.getGold.ToString();
		}
		else
		{
			costDay.text = "";
			getDay.text = "";
			costDiamondCount.text = "";
			costGoldCount.text = "";
			getDiamondCount.text = "";
			getGoldCount.text = "";
		}
	}
	// 领取//
	public void receiveButtonClick(GameObject obj)
	{
		if(rdi != null && rdi.s_rebateState == S_RebateState.CAN_RECEIVE)
		{
			(FPortManager.Instance.getFPort ("RebateSendFPort") as RebateSendFPort).access (rdi.dayID, receiveCallBack);
		}

	}
	// 领取回调//
	public void receiveCallBack()
	{
		// 飘字//
		showPrize();
		RebateInfoManagement.Instance.canRecevieCount--;
		rdi.rebateState = RebateState.RECEIVED;
		rdi.s_rebateState = S_RebateState.NONE;
		m_infos[rdi.dayID] = rdi;
		RebateInfoFPort rifp = FPortManager.Instance.getFPort ("RebateInfoFPort") as RebateInfoFPort;
		rifp.setInfos(m_infos);
		updateItem(rdi.dayID);
		closeDetail(null);
	}

	public void showPrize()
	{
		if(rdi != null)
		{
			List<PrizeSample> prizes = new List<PrizeSample>();
			if(rdi.getDiamond > 0)
			{
				PrizeSample ps = new PrizeSample(PrizeType.PRIZE_RMB,0,rdi.getDiamond);
				prizes.Add(ps);
			}
			if(rdi.getGold > 0)
			{
				PrizeSample ps = new PrizeSample(PrizeType.PRIZE_MONEY,0,rdi.getGold);
				prizes.Add(ps);
			}

			if(prizes.Count > 0)
			{
				UiManager.Instance.createPrizeMessageLintWindow(prizes.ToArray());
			}
		}
	}
	// 初始化规则描述//
	public void initeRulePanel(List<RebateSample> _diamondSample,List<RebateSample> _goldSample)
	{
		if(_diamondSample.Count > 0)
		{
			createRule(_diamondSample,true);
		}

		if(_goldSample.Count > 0)
		{
			createRule(_goldSample,false);
		}
		ruleDescPanel.GetComponent<UIGrid>().repositionNow = true;
		//ruleDescPanel.GetComponent<UIGrid>().Reposition();
	}

	public void createRule(List<RebateSample> mList,bool isDiamond)
	{
		string[] _strArr = new string[2];
		string txt = "";
		for(int i=0;i<mList.Count;i++)
		{
			if(mList[i].lessRate !=0)// 小于的情况//
			{
				_strArr[0] = mList[i].compareVal.ToString();
				_strArr[1] = mList[i].lessRate + "%";
				if(isDiamond)
				{
					txt = string.Format(LanguageConfigManager.Instance.getLanguage ("costDiamondDesc_less"),_strArr);
				}
				else
				{
					txt = string.Format(LanguageConfigManager.Instance.getLanguage ("costGoldDesc_less"),_strArr);
				}
				createRuleLabel(txt);
			}
			if(mList[i].moreRate !=0)// 大于情况//
			{
				_strArr[0] = mList[i].compareVal.ToString();
				_strArr[1] = mList[i].moreRate + "%";
				if(isDiamond)
				{
					txt = string.Format(LanguageConfigManager.Instance.getLanguage ("costDiamondDesc_more"),_strArr);
				}
				else
				{
					txt = string.Format(LanguageConfigManager.Instance.getLanguage ("costGoldDesc_more"),_strArr);
				}
				createRuleLabel(txt);
			}
		}
	}
	public void createRuleLabel(string text)
	{
		GameObject mLabel = GameObject.Instantiate(ruleDescTemp) as GameObject;
		mLabel.SetActive(true);
		mLabel.transform.parent = ruleDescPanel;
		mLabel.transform.localPosition = Vector3.zero;
		mLabel.transform.localScale = Vector3.one;
		mLabel.GetComponent<UILabel>().text = text;
	}

	public void updateItem(int dayID)
	{
		string objName = "item_" + dayID;
		GameObject itemObj = GameObject.Find(objName);
		RebateItem ri;
		if(itemObj != null)
		{
			ri = itemObj.GetComponent<RebateItem>();
			ri.setRebateContent(this);
			ri.updateRebateItem(rdi);
		}
	}

	public void checkRuleClick(GameObject obj)
	{
		if(effectObj != null)
		{
			effectObj.SetActive(false);
		}
		detailPanel.SetActive(true);
		detailInfo.SetActive(false);
		ruleInfo.SetActive(true);
	}

	public RebateDayInfo getInfo()
	{
		return rdi;
	}
	public void setInfo(RebateDayInfo _rdi)
	{
		rdi = _rdi;
	}

	void Update()
	{
		if((endTime - ServerTimeKit.getSecondTime()) <= 0)// 活动已结束已结束//
		{
			timeLabel.text = LanguageConfigManager.Instance.getLanguage("godsWar_141555");
		}
		else
		{
			timeLabel.text = TimeKit.timeTransform ((endTime - ServerTimeKit.getSecondTime())*1000.0d);
		}

		if(RebateInfoManagement.Instance.loginTime == 0)
		{
			RebateInfoManagement.Instance.loginTime = ServerTimeKit.getLoginTime();
		}
		if(ServerTimeKit.getMillisTime() >= BackPrizeLoginInfo.Instance.getSecondDayTime(RebateInfoManagement.Instance.loginTime))// 跨天//
		{
			RebateInfoManagement.Instance.loginTime = ServerTimeKit.getMillisTime();

			RebateInfoFPort rif = FPortManager.Instance.getFPort ("RebateInfoFPort") as RebateInfoFPort;
			rif.RebateInfoAccess(getRebateInfoCallBack);
		}
	}
}
