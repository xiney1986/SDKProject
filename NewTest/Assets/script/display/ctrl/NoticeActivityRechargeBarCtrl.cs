using UnityEngine;
using System.Collections;

public class NoticeActivityRechargeBarCtrl : MonoBase
{

	public NoticeActivityRechargeContent fatherWindow;
	public ButtonNARechargeReceive receiveButton;
	public Recharge recharge;
	public RechargeSample res;
	public UILabel costValue;
	public UILabel timeLabel;
	public UISprite costIcon;
	public UISprite background;
	public UILabel titleLabel;
	public UILabel rechargeExplain;
	public UILabel rechargeValue;
	public UILabel costExplain;
    public UILabel oneTipLabel;
	public GameObject showAwardPos;
	public GameObject goodsViewPre;
	bool isActive = false;
	private Notice notice;
	private NoticeSample sample;
	private GoodsView[] awardButtons;

	private string parseDesc (string desc, int condition) {
		if (string.IsNullOrEmpty (desc))
			return "";
		return desc.Replace ("%1", condition.ToString ());
	}
	public void updateItem (Recharge re, NoticeSample noticeSample, Notice notice)
	{
		recharge = re;
		res = re.getRechargeSample ();
		this.notice = notice;
		this.sample = noticeSample;
		showTime (); 
		receiveButton.fatherWindow = fatherWindow.win;
		receiveButton.updateButton (re);
		receiveButton.content = fatherWindow;
		setItemText();
		if (awardButtons == null) {
			awardButtons = new GoodsView[4];
			for (int i = 0; i < awardButtons.Length; i++) {
				awardButtons [i] = NGUITools.AddChild (showAwardPos, goodsViewPre).GetComponent<GoodsView> ();
				awardButtons [i].transform.localPosition = new Vector3 (i * 120, 0, 0);
				awardButtons [i].fatherWindow = fatherWindow.win;
				awardButtons [i].gameObject.SetActive (false);
			}

			//显示充值奖励内容 位移差X=120
			for (int i = 0; i < res.prizes.Length && i < 4; i++) {
				awardButtons [i].gameObject.SetActive (true);
				awardButtons [i].init (res.prizes [i]);
			}
		} else {
			for (int i = 0; i < awardButtons.Length; i++) {
				awardButtons [i].gameObject.SetActive (false);
			}
			for (int i = 0; i < res.prizes.Length && i < 4; i++) {
				awardButtons [i].gameObject.SetActive (true);
				awardButtons [i].init (res.prizes [i]);
			}
		}
	}

	private void setItemText()
	{
		//消费
		if (sample.type == NoticeType.COSTNOTICE || sample.type == NoticeType.NEW_CONSUME) {
			rechargeExplain.gameObject.SetActive (false);
			titleLabel.text = parseDesc (res.desc, res.condition) + "(" + recharge.getLastNum () + "/" + res.count + ")";
			if (recharge.num >= res.condition) {
				costValue.gameObject.SetActive (false);
				costIcon.gameObject.SetActive (false);
				if (recharge.count >= res.count) {
					costExplain.text = LanguageConfigManager.Instance.getLanguage ("s0209");
					receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
				}
				else{
					costExplain.gameObject.SetActive (false);
					receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0309");
				}
					
			}
			else {
				receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0309");
				costValue.gameObject.SetActive (true);
				costIcon.gameObject.SetActive (true);
				costExplain.gameObject.SetActive (true);
				costValue.text = "X " + (res.condition - recharge.num);
			}
		}
		else if (sample.type == NoticeType.TOPUPNOTICE || sample.type == NoticeType.TIME_RECHARGE || sample.type == NoticeType.NEW_RECHARGE) {
			costValue.gameObject.SetActive (false);
			costExplain.gameObject.SetActive (false);
			costIcon.gameObject.SetActive (false);
			titleLabel.text = parseDesc (res.desc, res.condition / 10) + "(" + recharge.getLastNum () + "/" + res.count + ")";
			if (isActive) {
				if (res.reType == RechargeSample.RECHARGE) { // 单笔充值
					if (!recharge.isOneRecharge ()) {
						rechargeExplain.text = LanguageConfigManager.Instance.getLanguage ("s0209");
						rechargeValue.gameObject.SetActive(false);
						receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
						receiveButton.disableButton (true);
					}
					else {
						receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0309");
						rechargeExplain.text = LanguageConfigManager.Instance.getLanguage ("notice22");
						rechargeValue.gameObject.SetActive(true);
						rechargeValue.text = (res.condition / 10).ToString ()+ LanguageConfigManager.Instance.getLanguage ("notice23");
					}
				}
				else { // 累计充值
					if (!recharge.isRecharge ()) {
						rechargeExplain.text = LanguageConfigManager.Instance.getLanguage ("s0209");
						rechargeValue.gameObject.SetActive(false);
						receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
						receiveButton.disableButton (true);
					}
					else {
						int num = 0;
						if (res.condition - recharge.num <= 0) {
							num = 0;
						}
						else {
							num = res.condition - recharge.num;
						}
						receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0309");
						rechargeExplain.text = LanguageConfigManager.Instance.getLanguage ("notice24");
						rechargeValue.gameObject.SetActive(true);
						rechargeValue .text = (num / 10).ToString () + LanguageConfigManager.Instance.getLanguage ("notice23");
					}
				}
			}
		}
	}
	void changeButton ()
	{
		if (receiveButton == null)
			return;
		if (!isActive) {   
			receiveButton.disableButton (true);
		}
		else {
			if(res.reType == RechargeSample.RECHARGE) { // 单笔充值
				if(!recharge.isOneComplete () || !recharge.isOneRecharge()) {
					receiveButton.disableButton (true);
                    oneTipLabel.gameObject.SetActive(false);
				} else {
					receiveButton.disableButton (false);
                    oneTipLabel.gameObject.SetActive(true);
                    oneTipLabel.text = LanguageConfigManager.Instance.getLanguage("missionAward01") + ":" + recharge.getOneCompleteNum();
				}
			}
			else if(res.reType == RechargeSample.RECHARGES) { // 累计充值
				if(!recharge.isComplete () || !recharge.isRecharge()) {
					receiveButton.disableButton (true);
				} else {
					receiveButton.disableButton (false);
				}
			}
			else {
				receiveButton.disableButton (false);
			}
		}
	}
	
	public void showTime ()
	{
		if(notice==null)
			return;
		int time = ServerTimeKit.getSecondTime ();
		int[] limitTimes=notice.getTimeLimit ();
		//过期移除
		if (limitTimes == null) {
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0138");
			isActive = false;
			rechargeExplain.text = LanguageConfigManager.Instance.getLanguage ("s0211");
			rechargeValue.gameObject.SetActive(false);
			changeButton ();
		}
		else {
			int startTime = limitTimes [0];
			int endTime= limitTimes [1];
			//表示永久活动
			if (startTime == 0 && endTime == 0) {
				timeLabel.text = "";
				isActive = true;
				changeButton ();
				return;
			}
			//差多久开始
			if (startTime > time && startTime > 0) {	
				timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0137", TimeKit.dateToFormat (startTime, LanguageConfigManager.Instance.getLanguage ("notice05")));
				isActive = false;
				rechargeExplain.text = LanguageConfigManager.Instance.getLanguage ("s0210");
				rechargeValue.gameObject.SetActive(false);
				changeButton ();
				return;
			}
			//过期移除
			if (endTime < time && endTime > 0) {
				timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0138");
				isActive = false;
				rechargeExplain.text = LanguageConfigManager.Instance.getLanguage ("s0211");
				rechargeValue.gameObject.SetActive(false);
				changeButton ();
				return;
			}	
			if (startTime == 0 && endTime > 0 && time < endTime) {
				timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0135", TimeKit.dateToFormat (endTime, LanguageConfigManager.Instance.getLanguage ("notice05")));	
				isActive = true;
				setItemText();
				changeButton ();
				return;
			}
			if (startTime > 0 && endTime == 0 && time > startTime) {
				timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0136");
				isActive = true;
				changeButton ();
				return;
			}
			if (startTime > 0 && endTime > 0 && time > startTime && time < endTime) {
				timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0135", TimeKit.dateToFormat (endTime, LanguageConfigManager.Instance.getLanguage ("notice05")));	
				isActive = true;
				setItemText();
				changeButton ();
				return;
			}
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
}
