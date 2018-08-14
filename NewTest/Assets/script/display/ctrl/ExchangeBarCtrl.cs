using UnityEngine;
using System.Collections;

public class ExchangeBarCtrl : MonoBase
{
	
	public ExChangeWindow fatherWindow;
	public GameObject awardShowPos;
	public GameObject goodsShowPos;
	public GameObject goodsViewPre;
	public ButtonExchangeReceive  receiveButton;
	public ButtonExchangeCondition  conditionInfoButton;
	public Exchange exchange;
	public UILabel countLabel;
	public UILabel timeLabel;
	public UISprite timeBg;
	bool isActive = false;
	bool conditions = false;
	bool premises = false;
	Timer timer;
	private const string FIRSTOBJECT = "001";
	private GoodsView[] costButtons;
	private GoodsView awardButton;

	public void updateItem (Exchange exc)
	{
		exchange = exc;
		ExchangeSample sample = exc.getExchangeSample ();
		receiveButton.fatherWindow = fatherWindow;
		receiveButton.updateButton (exc);
		
		//前提条件是否达成
		if (!ExchangeManagerment.Instance.isCheckPremises (sample)) {
			//显示没完成的第一个条件
			conditionInfoButton.gameObject.SetActive (true);
			conditionInfoButton.fatherWindow = fatherWindow;
			conditionInfoButton.fatherBar = this;
			conditionInfoButton.textLabel.text = ExchangeManagerment.Instance.checkPremises (sample); 	
			premises = false;
			
			//	changeButton (false);
			//	return ;
		} else {
			conditionInfoButton.gameObject.SetActive (false);
			premises = true;
		}
		
		//兑换条件是否达成
		if (!ExchangeManagerment.Instance.isCheckConditions (sample)) {
			conditions = false;
		} else {
			conditions = true;
		}
		bool needChangeColor = false;
		if (costButtons == null) {
			costButtons = new GoodsView[3];
			for (int i = 0; i < costButtons.Length; i++) {
				costButtons[i] = NGUITools.AddChild (goodsShowPos, goodsViewPre).GetComponent<GoodsView> (); 
				costButtons[i].transform.localPosition = new Vector3 (i * 120, 0, 0);
				costButtons[i].fatherWindow = fatherWindow;
				costButtons[i].gameObject.SetActive(false);
			}
			//显示兑换条件内容
			ExchangeCondition exCon;
			for (int i = 0; i < sample.conditions[0].Length && i < 3; i++) {
				exCon = sample.conditions[0] [i];
				needChangeColor = ExchangeManagerment.Instance.isCheckConditions (sample,0,i);
				if(!needChangeColor)
					costButtons[i].rightBottomText.color = Color.red;
				else
					costButtons[i].rightBottomText.color = Color.white;
				costButtons[i].gameObject.SetActive(true);
				costButtons[i].init (exCon.costType, exCon.costSid, exCon.num);
			}
		} else {
			for (int i = 0; i < costButtons.Length; i++) {
				costButtons[i].gameObject.SetActive(false);
			}
			ExchangeCondition exCon;
			for (int i = 0; i < sample.conditions[0].Length && i < 3; i++) {
				exCon = sample.conditions [0][i];
				needChangeColor = ExchangeManagerment.Instance.isCheckConditions (sample,0,i);
				if(!needChangeColor)
					costButtons[i].rightBottomText.color = Color.red;
				else
					costButtons[i].rightBottomText.color = Color.white;
				costButtons[i].gameObject.SetActive(true);
				costButtons[i].init (exCon.costType, exCon.costSid, exCon.num);
			}
		}
		
		//显示兑换目标物品
		if (awardButton == null) {
			awardButton = NGUITools.AddChild (awardShowPos, goodsViewPre).GetComponent<GoodsView> ();
			awardButton.transform.localPosition = Vector3.zero;
			awardButton.fatherWindow = fatherWindow;
			awardButton.init (sample.type, sample.exchangeSid, sample.num);
		} else {
			awardButton.init (sample.type, sample.exchangeSid, sample.num);
		}

		if (exc.getExchangeSample ().times <= 0) {
			countLabel.text = "";
		} else {
			countLabel.text = LanguageConfigManager.Instance.getLanguage ("s0134") + "(" + exc.getLastNum () + "/" + exc.getExchangeSample ().times + ")";
		}
		
		showTime ();
	
		if (timer == null)
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (showTime);
		timer.start ();
		
		changeButton ();
	}

	void OnDisable ()
	{ 
		if (timer != null)
			timer.stop ();
		timer = null;
	}

	void changeButton ()
	{
		if (receiveButton == null)
			return;
		if (isActive == false || conditions == false || premises == false) {
//			receiveButton.GetComponent<BoxCollider> ().enabled = false;
//			receiveButton.spriteBg.spriteName = "button_big1_gray";
//			receiveButton.textLabel.color = Color.gray;
			receiveButton.disableButton (true);
		} else {
//			receiveButton.spriteBg.spriteName = "button_big1";
//			receiveButton.GetComponent<BoxCollider> ().enabled = true;
//			receiveButton.textLabel.color = new Color (1f, 1f, 1f, 1f);
			receiveButton.disableButton (false);
		}
	}

	void showTime ()
	{
		int time = ServerTimeKit.getSecondTime ();
		
		if (exchange.getStartTime () == 0 && exchange.getEndTime () == 0) {
			timeLabel.text = "";
			timeBg.gameObject.SetActive (false);
			isActive = true;
			changeButton ();
			return;
		}
		

		//差多久开始
		if (exchange.getStartTime () > time && exchange.getStartTime () > 0) {	
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0137", timeTransform (exchange.getStartTime () - time));
			isActive = false;
			changeButton ();
			return;
		}
		//过期移除
		if (exchange.getEndTime () < time && exchange.getEndTime () > 0) {
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0138");
			isActive = false;
			changeButton ();
			return;
		}		
		
		if (exchange.getStartTime () == 0 && exchange.getEndTime () > 0 && time < exchange.getEndTime ()) {
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0135", timeTransform (exchange.getEndTime () - time));	
			isActive = true;
			changeButton ();
			return;
		}
		
		if (exchange.getStartTime () > 0 && exchange.getEndTime () == 0 && time > exchange.getStartTime ()) {
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0136");
			isActive = true;
			changeButton ();
			return;
		}	
		
		if (exchange.getStartTime () > 0 && exchange.getEndTime () > 0 && time > exchange.getStartTime () && time < exchange.getEndTime ()) {
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0135", timeTransform (exchange.getEndTime () - time));	
			isActive = true;
			changeButton ();
			return;
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

