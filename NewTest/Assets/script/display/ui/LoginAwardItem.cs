using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LoginAwardItem : MonoBehaviour
{
	/** 登陆奖励集--节省消耗不通过配置动态读取 */
	public ButtonTotalLoginAward[] awardButtons;
	/** 是否为大奖图标 */
	//public UISprite goodSprite;
	/** 是否已领取图标 */
	public UISprite receivedSprite;
	/** 领取奖励按钮 */
	public ButtonBase awardButton;
	/** 分享奖励按钮 */
	public ButtonBase shareButton;
	/** 条目标题 */
	public UILabel todayTitle;
	public UILabel weekTitle;
	/** 条目标题底框 */
	public GameObject backGroundyellow;
	/** 条目标题底框 */
	public GameObject backGroundblue;
	/**什么字的精灵 */
	public UISprite titleday;
	/**新天天送 */
	public UILabel titlenewLabel;
	public UISprite titleweek;
	public UILabel titleholiday;
	public UILabel titleholi;
	public GameObject redWeekly;
	public GameObject redday;
	public GameObject starSoulPoint;
	public GameObject move;
	/**红花精灵 */
	public UISprite titleredGround;

	public void updateAwardItem (TotalLogin loginAward,WindowBase fatherWindow,int type)
	{
		ButtonTotalLoginAward button;
		for (int i=0; i<awardButtons.Length; i++) {
			button=awardButtons[i];
			button.gameObject.SetActive(false);
		}
		int count=loginAward.prizes.Length;
		if(count>awardButtons.Length) count=awardButtons.Length;
		for (int i = 0; i < count; i++) 
		{
			button=awardButtons[i];
			button.gameObject.SetActive(true);
			button.cleanData();
			button.updateButton (loginAward.prizes [i]);
			button.setFatherWindow(fatherWindow);
		}
        awardButton.textLabel.text = (LanguageConfigManager.Instance.getLanguage("s0309"));
		//天天送
		if(type==TotalLoginManagerment.EVERYDAY){
			if(TotalLoginManagerment.Instance.getTotalDay()<loginAward.totalDays)
			{
				awardButton.gameObject.SetActive(false);
				/**分享功能**/
				shareButton.gameObject.SetActive(false);
			}
			else{
				awardButton.gameObject.SetActive(true);
				/**分享功能**/
				shareButton.gameObject.SetActive(true);
				shareButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("share04");
			}
		}else if(type==TotalLoginManagerment.WEEKLY){//周末送
			DateTime timen=TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
			int today=(int)timen.DayOfWeek;
			if(today==0)today=7;
			if(loginAward.week>today||loginAward.isloginn!=1){
				awardButton.gameObject.SetActive(false);
			}else{
				awardButton.gameObject.SetActive(true);
				if(loginAward.isAward==1){
					awardButton.disableButton(true);
					awardButton.textLabel.text=(LanguageConfigManager.Instance.getLanguage("total_alwayscan"));
				}
			}
		}else if(type==TotalLoginManagerment.HOLIDAY){
			if(loginAward.day>TotalLoginManagerment.Instance.getTotalHoliday()){
				awardButton.gameObject.SetActive(false);
			}else{
				awardButton.gameObject.SetActive(true);
				if(loginAward.isAward==1){
					awardButton.disableButton(true);
					awardButton.textLabel.text=(LanguageConfigManager.Instance.getLanguage("total_alwayscan"));
				}
			}
		}else if(type==TotalLoginManagerment.NEWEVERYDAY){
			DateTime timen=TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
			int today=(int)timen.DayOfWeek;
			if(today==0)today=7;
			if(loginAward.totalDays>today||loginAward.isloginn!=1){
				awardButton.gameObject.SetActive(false);
			}else{
				awardButton.gameObject.SetActive(true);
				if(loginAward.isAward==1){
					awardButton.disableButton(true);
					awardButton.textLabel.text=(LanguageConfigManager.Instance.getLanguage("total_alwayscan"));
				}
			}
		}
		awardButton.setFatherWindow (fatherWindow);
		/**分享功能**/
		shareButton.setFatherWindow (fatherWindow);
		awardButton.name = "awardButton:"+loginAward.prizeSid;
		if(loginAward.rewardType==TotalLoginManagerment.BIGPRIZE)
		{
			//backGround.spriteName="bar_blue_new";
			backGroundyellow.SetActive(false);
			backGroundblue.SetActive(true);
		}
		else
		{
			backGroundyellow.SetActive(true);
			backGroundblue.SetActive(false);
		}
		if(type==TotalLoginManagerment.EVERYDAY){
			move.transform.localPosition = new Vector3(0,0,0);
			redday.SetActive(true);
			redWeekly.SetActive(false);
			titleday.gameObject.SetActive(true);
			titlenewLabel.gameObject.SetActive(false);
			titleweek.gameObject.SetActive(false);
			titleholiday.gameObject.SetActive(false);
			todayTitle.gameObject.SetActive(true);
			todayTitle.text = loginAward.totalDays.ToString();
		}else if(type==TotalLoginManagerment.NEWEVERYDAY){
			move.transform.localPosition = new Vector3(0,0,0);
			redday.SetActive(false);
			redWeekly.SetActive(false);
			titleday.gameObject.SetActive(false);
			titleweek.gameObject.SetActive(false);
			titleholiday.gameObject.SetActive(false);
			todayTitle.gameObject.SetActive(false);
			titlenewLabel.gameObject.SetActive(true);
			string str=getTime(loginAward.totalDays);
			titlenewLabel.text=LanguageConfigManager.Instance.getLanguage("total_newday",str);
		}else if(type==TotalLoginManagerment.WEEKLY){
			move.transform.localPosition = new Vector3(0,0,0);
			titleday.gameObject.SetActive(false);
			titleweek.gameObject.SetActive(true);
			titlenewLabel.gameObject.SetActive(false);
			titleholiday.gameObject.SetActive(false);
			weekTitle.gameObject.SetActive(true);
			weekTitle.text =loginAward.single.ToString();
			redday.SetActive(false);
			redWeekly.SetActive(true);

		}else{
			move.transform.localPosition = new Vector3(28,0,0);
			redday.SetActive(true);
			titlenewLabel.gameObject.SetActive(false);
			redWeekly.SetActive(false);
			titleday.gameObject.SetActive(false);
			titleweek.gameObject.SetActive(false);
			titleholiday.gameObject.SetActive(true);
			titleholiday.text=loginAward.holidayName+LanguageConfigManager.Instance.getLanguage("total_dayy");
			todayTitle.gameObject.SetActive(true);
			todayTitle.text = loginAward.day.ToString();
		}
			
	}
	private string getTime(int day){
		string str="";
		switch(day){
		case 1:
			str=LanguageConfigManager.Instance.getLanguage("total_1");
			break;
		case 2:
			str=LanguageConfigManager.Instance.getLanguage("total_2");
			break;
		case 3:
			str=LanguageConfigManager.Instance.getLanguage("total_3");
			break;
		case 4:
			str=LanguageConfigManager.Instance.getLanguage("total_4");
			break;
		case 5:
			str=LanguageConfigManager.Instance.getLanguage("total_5");
			break;
		case 6:
			str=LanguageConfigManager.Instance.getLanguage("total_6");
			break;
		case 7:
			str=LanguageConfigManager.Instance.getLanguage("total_0");
			break;
		}
		return str;
	}

}