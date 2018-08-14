using UnityEngine;
using System.Collections;

public class NoticeActivityExchangeBarCtrl : MonoBase
{ 
	public NoticeActivityExchangeContent fatherWindow;
	public GameObject awardShowPos;
	public GameObject goodsShowPos;
	public GameObject goodsViewPre;
	public ButtonExchangeReceive  receiveButton;
	public ButtonExchangeCondition  conditionInfoButton;
	public Exchange exchange;
	public UILabel countLabel;
	public UILabel timeLabel;
	public Transform spriteArrow;
	bool isActive = false;
	bool conditions = false;
	bool premises = false;
	private GoodsView[] costButtons;
	private GoodsView awardButton;

	public void updateItem (Exchange exc)
	{
		exchange = exc;
		ExchangeSample sample = exc.getExchangeSample ();
		receiveButton.fatherWindow = fatherWindow.win;
		receiveButton.updateButton (exc);
		
		//前提条件是否达成
		if (!ExchangeManagerment.Instance.isCheckPremises (sample)) {
			
			//显示没完成的第一个条件
			conditionInfoButton.gameObject.SetActive (true);
			conditionInfoButton.fatherWindow = fatherWindow.win;
			conditionInfoButton.fatherBar = this;
			conditionInfoButton.textLabel.text = ExchangeManagerment.Instance.checkPremises (sample); 	
			premises = false;
		} else {
			conditionInfoButton.gameObject.SetActive (false);
			premises = true;
		}
		
		//兑换条件是否达成
		if (!ExchangeManagerment.Instance.isCheckConditions (sample) || (sample.times != 0 && exchange.getNum () >= sample.times)) {
			conditions = false;
		} else {
			conditions = true;
		}
		int activeCostButtonNum = 0;
		if (costButtons == null) {
			costButtons = new GoodsView[3];
			for (int i = 0; i < costButtons.Length; i++) {
				costButtons [i] = NGUITools.AddChild (goodsShowPos, goodsViewPre).GetComponent<GoodsView> (); 
				costButtons [i].transform.localPosition = new Vector3 (i * 120, 0, 0);
				costButtons [i].fatherWindow = fatherWindow.win;
				costButtons [i].gameObject.SetActive (false);
			}
			//显示兑换条件内容
			ExchangeCondition exCon;
			for (int i = 0; i < sample.conditions[0].Length && i < 3; i++) {
				exCon = sample.conditions [0][i];
				costButtons [i].gameObject.SetActive (true);
				costButtons [i].init (exCon.costType, exCon.costSid, exCon.num);
				activeCostButtonNum++;
			}
		} else {
			for (int i = 0; i < costButtons.Length; i++) {
				costButtons [i].gameObject.SetActive (false);
			}
			ExchangeCondition exCon;
			for (int i = 0; i < sample.conditions[0].Length && i < 3; i++) {
				exCon = sample.conditions[0] [i];
				costButtons [i].gameObject.SetActive (true);
				costButtons[i].clean();
				costButtons [i].init (exCon.costType, exCon.costSid, exCon.num);
				activeCostButtonNum++;
			}
		}
		//显示兑换目标物品
		if (awardButton == null) {
			awardButton = NGUITools.AddChild (awardShowPos, goodsViewPre).GetComponent<GoodsView> ();
			awardButton.transform.localPosition = Vector3.zero;
			awardButton.fatherWindow = fatherWindow.win;
			awardButton.init (sample.type, sample.exchangeSid, sample.num);
		} else {
			awardButton.init (sample.type, sample.exchangeSid, sample.num);
		}
		if (activeCostButtonNum > 0) {
			activeCostButtonNum=activeCostButtonNum>costButtons.Length?costButtons.Length:activeCostButtonNum;
			awardShowPos.transform.localPosition = new Vector3((activeCostButtonNum-1)*120,awardShowPos.transform.localPosition.y,awardShowPos.transform.localPosition.z);
			spriteArrow.localPosition = new Vector3(-100+(activeCostButtonNum-1)*120,spriteArrow.localPosition.y,spriteArrow.localPosition.z);
		}
		if (exc.getLastNum () == int.MaxValue)
			countLabel.text = LanguageConfigManager.Instance.getLanguage ("s0134") + "(" + Language ("exchange04") + ")";
		else
			countLabel.text = LanguageConfigManager.Instance.getLanguage ("s0134") + "(" + exc.getLastNum () + "/" + exc.getExchangeSample ().times + ")";
		showTime ();
	}

	void changeButton ()
	{
		if (receiveButton == null)
			return;
		ExchangeSample exchangeSample = exchange.getExchangeSample ();
		if (exchangeSample.times != 0 && exchange.getNum () >= exchangeSample.times)
			receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
		if (isActive == false || conditions == false || premises == false) {
			receiveButton.disableButton (true);
		} else {
			receiveButton.disableButton (false);
		}
		
	}

	public void showTime ()
	{
		int time = ServerTimeKit.getSecondTime ();
		if (exchange.getStartTime () == 0 && exchange.getEndTime () == 0) {
			timeLabel.text = "";
			isActive = true;
			changeButton ();
			return;
		}
		//差多久开始
		if (exchange.getStartTime () > time && exchange.getStartTime () > 0) {	
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0137", TimeKit.dateToFormat (exchange.getStartTime (), LanguageConfigManager.Instance.getLanguage ("notice05")));
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
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0135", TimeKit.dateToFormat (exchange.getEndTime (), LanguageConfigManager.Instance.getLanguage ("notice05")));	
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
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0135", TimeKit.dateToFormat (exchange.getEndTime (), LanguageConfigManager.Instance.getLanguage ("notice05")));	
			isActive = true;
			changeButton ();
			return;
		}
	}
}
