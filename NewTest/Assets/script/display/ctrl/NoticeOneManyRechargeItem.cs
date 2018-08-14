using UnityEngine;
using System.Collections;

public class NoticeOneManyRechargeItem : MonoBase
{
    public NoticeOneManyRechargeContent fatherWindow;
	public ButtonOneManyRechargeReceive receiveButton;
	public Recharge recharge;
	public RechargeSample res;
	public UILabel timeLabel;
    public UILabel alreadyTimes;
	public UISprite background;
	public UILabel titleLabel;
	public GameObject showAwardPos;
	public GameObject goodsViewPre;
	bool isActive = false;
	private Notice notice;
	private NoticeSample sample;
	private GoodsView[] awardButtons;
    private bool isEndingTip = true;

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
				awardButtons [i].transform.localPosition = new Vector3 (i * 105f, 0, 0);
                awardButtons [i].transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
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
        titleLabel.text = parseDesc(res.desc, res.condition / 10) + "(" + (recharge.getOneManyRechargeMaxNum() - recharge.count) + "/" + recharge.getOneManyRechargeMaxNum() + ")";
        alreadyTimes.gameObject.SetActive(recharge.num > 0);
        int nowMax = recharge.getOneManyRechargeMaxNum() - recharge.count;
        alreadyTimes.text = LanguageConfigManager.Instance.getLanguage("missionAward01") + ": " + (nowMax >= recharge.num ? recharge.num : nowMax);
			if (isActive) {
					if (!recharge.isOneManyRechargeComplete () && recharge.isOneManyRechargeReceived()) {
						receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
						receiveButton.disableButton (true);
					}
					else {
						receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0309");
				    }
			}
	}
	void changeButton ()
	{
		if (receiveButton == null)
			return;
		if (!isActive) {   
			receiveButton.disableButton (true);
            isEndingTip = false;
            if(recharge.num > 0)
                UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("notice37"));
		}
		else {
				receiveButton.disableButton (!recharge.isOneManyRechargeComplete ());
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
				changeButton ();
				return;
			}
			//过期移除
			if (endTime < time && endTime > 0) {
				timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0138");
				isActive = false;
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
    void Update()
    {
        if ((notice as NewRechargeNotice).isEnding() && isEndingTip)
        {
            isActive = false;
            changeButton();
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
