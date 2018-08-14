using System;
using UnityEngine;
using System.Collections;

/**
 * 抽奖图标节点按钮
 * */
public class ButtonLuckyDraw : ButtonBase
{
	public GameObject tipNumObj;
	public UILabel tipNumLabel;//提示可以召唤次数
	public UILabel timeLabel;//时间
	public UILabel bottomLabel;//当前货币量
	public UILabel costLabel;//花费
	public UITexture bgIcon;//广告图
	public UISprite costIcon;//消耗图标
	public UISprite havaIcon;//所拥有相应消耗的总量的图标
	public UILabel freeLabel;//本次免费
	
	public LuckyDraw luckyDraw;
	
	public void updateLuckyDraw (LuckyDraw luckyDraw)
	{
		this.luckyDraw = luckyDraw; 
		//根据不同的类型显示不同的文字
		/*
		if (luckyDraw.getDrawNum () > 0)
			numLabel.text = (luckyDraw.getDrawNum () - luckyDraw.getCostDrawNum ()) + "/" + luckyDraw.getDrawNum ();
		else
			numLabel.text = "0"; 
			*/
		//
		
//		if (luckyDraw.getTimeInfo () != "")
//			timeLabel.text = luckyDraw.getTimeInfo ();
//		else
//			timeLabel.transform.parent.gameObject.SetActive (false); 

		showFreeBg ();
		bottomLabel.text = "x " + luckyDraw.getShowCostNum ();
		
		costLabel.text = luckyDraw.getCostNumInfo ();
		LuckyCostIcon.setToolCostIconName (luckyDraw.ways [0], costIcon);
		LuckyCostIcon.setToolCostIconName (luckyDraw.ways [0], havaIcon);
		ResourcesManager.Instance.LoadAssetBundleTexture ("texture/luckydraw/luckyPic_" + luckyDraw.getIconId (), bgIcon);//+ goods.getIconId (), itemIcon);

		//jordenwu 提示可以购买次数
		//拥有对应
		//是否有免费的
//		if(luckyDraw.sid==81001){
//			//钻石召唤 不提示次数
//			tipNumObj.SetActive(false);
//		}else{

		bool ishavafree = luckyDraw.getFreeNum () > 0 ? true : false;
		if (ishavafree) {
			//直接提示免费次数
			int num = luckyDraw.getFreeNum ();
			if (num <= 0) {
				tipNumObj.SetActive (false);
			} else {
				tipNumObj.SetActive (true);
				tipNumLabel.text = luckyDraw.getFreeNum ().ToString ();
			}
		} else {
			//提示可以购买次数
			int own = luckyDraw.getShowCostNum ();
			int needCost = luckyDraw.getCostNum ();
			int num = (own / needCost);
			if (num <= 0) {
				tipNumObj.SetActive (false);
			} else {
				tipNumObj.SetActive (true);
				tipNumLabel.text = num.ToString ();
			}
		}
//		}
	}

	void FixedUpdate ()
	{  
		if (luckyDraw.getTimeInfo () != "")
			timeLabel.text = luckyDraw.getTimeInfo ();
		else if (luckyDraw.getNextFreeTime () != -1) {
			if (luckyDraw.canFreeCD ())
				showFreeBg ();
			else
				timeLabel.text = Language ("s0585") + TimeKit.timeTransform ((luckyDraw.getNextFreeTime () - ServerTimeKit.getSecondTime ()) * 1000);
		}
		else
			timeLabel.text = "";

	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		int noticeSid=luckyDraw.getNoticeSid ();
		if (noticeSid!=0) {
			NoticeSample tmp = NoticeSampleManager.Instance.getNoticeSampleBySid(noticeSid);
			Notice notice=NoticeManagerment.Instance.getValidNoticeBySid(noticeSid,tmp.entranceId);
			if (notice != null) { // 打开对应的活动界面
				UiManager.Instance.openWindow<NoticeWindow> ((win) => {
					win.entranceId = tmp.entranceId;
					win.updateSelectButton(notice.sid);
				});
			} else {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0171"));
				return;
			}
		} else { //打开抽奖二级界面 
			if (!luckyDraw.isTimeStart ()) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0054"));
				return;
			}
			GuideManager.Instance.doGuide (); 
			UiManager.Instance.openWindow<LuckyDrawDetailWindow> ((win) => {
				win.setLuckyDraw (luckyDraw); 
			});
		}
		(fatherWindow as LuckyDrawWindow).content.cleanAll ();
	}
	//根据类型获得图标名
	public string getIconName (int type)
	{
		string name = string.Empty;
		switch (type) {
		case PrizeType.PRIZE_RMB:
			name = "";
			break;
		case PrizeType.PRIZE_MONEY:
			name = "";
			break;
		case PrizeType.PRIZE_PROP:
			name = "";
			break;
		}
		return name;
	}

	void showFreeBg(){
		if (luckyDraw.canFreeCD ()) {
			freeLabel.text = Language ("s0026");
			freeLabel.gameObject.SetActive(true);
			costIcon.transform.parent.gameObject.SetActive (false);
			timeLabel.text = String.Empty;
		} else {
			freeLabel.gameObject.SetActive(false);
			costIcon.transform.parent.gameObject.SetActive (true);
		}
	}
}
