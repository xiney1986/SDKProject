using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoticeLadderHegeMoneyContent : MonoBase {

	public ContentLadderHegoMoney content;
	public MessageHandle tmpMsg;
	public LadderHegemoneyNotice notice;
	private LadderHegemoneyActiveNotice activeNotice;
	public GameObject laddderHePrefab;
	//public UILabel desLabel;
	public UILabel timeLabel;
	public UILabel ladderdescLabel;
	public UISprite title1;
	public UILabel myPoint;

	public ButtonRuleShow buttonshow;
	public ButtonChallenge buttonChallenge;


	[HideInInspector]
	public WindowBase win;//活动窗口

	private bool isInit = false;
	
	public void initContent (Notice notice, WindowBase win)
	{
		this.notice = notice as LadderHegemoneyNotice;
		initNoticeTime (this.notice);
		if (!isInit) {
			isInit = true;
		} else {
			return;
		}

		this.win = win;
		setFather (win);
		LadderHegeMoneyManager.Instance.initMsg (notice.sid,()=>{updateInfo();});
	}

	public void initNoticeTime(Notice noticec)
	{
		this.notice = noticec as LadderHegemoneyNotice;

		if (notice.isTimeLimit ()) {

			int[] time = this.notice.getShowTimeLimit ();
			
			if(time == null){
				timeLabel.gameObject.SetActive(true);
				timeLabel.text = LanguageConfigManager.Instance.getLanguage("s0138");
				return;
			}
			timeLabel.gameObject.SetActive(true);
			
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("notice02", TimeKit.dateToFormat (time [0], LanguageConfigManager.Instance.getLanguage ("notice04")),
			                                                             TimeKit.dateToFormat (time [1] - 1, LanguageConfigManager.Instance.getLanguage ("notice04")));

		} else {
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("notice03");
		}
	}


	public void initActiveNotice(Notice notice)
	{
		this.activeNotice = notice as LadderHegemoneyActiveNotice;
		buttonChallenge.ladderNotice = this.activeNotice;

		if (this.activeNotice == null) {
			ladderdescLabel.text = LanguageConfigManager.Instance.getLanguage("ladderruleprize5");
			return;
		}

		if (activeNotice.isTimeLimit ()) {
			int[] time = activeNotice.getShowTimeLimit ();

			if(time == null){
				ladderdescLabel.gameObject.SetActive(true);
				ladderdescLabel.text = LanguageConfigManager.Instance.getLanguage("s0140");
				return;
			}
			ladderdescLabel.gameObject.SetActive(true);
		
			ladderdescLabel.text = LanguageConfigManager.Instance.getLanguage("ladderruleprize4",TimeKit.dateToFormat (time [0], LanguageConfigManager.Instance.getLanguage ("notice04")),
			                                                                  TimeKit.dateToFormat (time [1] - 1, LanguageConfigManager.Instance.getLanguage ("notice04")));
		} else {
			ladderdescLabel.text = LanguageConfigManager.Instance.getLanguage ("notice03");
	    }
	}

	public void OnEnable()
	{
		if ( this.win != null
		    && isInit) {
			setFather (win);
			LadderHegeMoneyManager.Instance.initMsg (()=>{updateInfo();});
		}
	}


	public void setFather(WindowBase father)
	{


		buttonshow.fatherWindow = father;
		buttonChallenge.fatherWindow = father;
	}

	public void updatePoint()
	{
		myPoint.text = LadderHegeMoneyManager.Instance.myPort.ToString();
	}

	void updateInfo()
	{
		ArrayList array = LadderHegeMoneyManager.Instance.GoodsList;
		updatePoint ();
		content.Initialize (array, ContentShopGoods.LADDER_SHOP, null,win,this);
		content.reLoad (array.Count);
	}


	private void initDesTime ()
	{
		//2014.7.17 added
		if(LanguageConfigManager.Instance.getLanguage ("notice19") == notice.getSample ().activiteDesc)
			title1.spriteName = "vip";
		else title1.spriteName = "notice_xs";
		//desLabel.text = notice.getSample ().activiteDesc;
		if (notice.isTimeLimit ()) {
			int[] time = notice.getShowTimeLimit ();
			if (time == null) {
				timeLabel.text = LanguageConfigManager.Instance.getLanguage ("s0138");
				return;
			} else {
				timeLabel.text = LanguageConfigManager.Instance.getLanguage ("notice02", TimeKit.dateToFormat (time [0], LanguageConfigManager.Instance.getLanguage ("notice04")),
				                                                             TimeKit.dateToFormat (time [1] - 1, LanguageConfigManager.Instance.getLanguage ("notice04")));
			}
		} else {
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("notice03");
          }
	}
	
	public void onReceive ()
	{

	}
	
	//兑换回调
	public void receiveGift (MessageHandle msg)
	{
		
		if (msg.msgEvent == msg_event.dialogOK) {
			ExchangeSample sample = (msg.msgInfo as Exchange).getExchangeSample ();
			
			string need = "";
			if(sample.conditions[0].Length>0){
				for (int i=0; i<sample.conditions[0].Length; i++) {
					need += sample.conditions [0][i].getName () + "x" + msg.msgNum * sample.conditions[0] [i].num + "\n";
				}
			}
			
			
			string target = sample.getExhangeItemName () + "x " + msg.msgNum;
			string info = LanguageConfigManager.Instance.getLanguage ("s0130", need, target);
			
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.dialogCloseUnlockUI=false;
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), info, sureExchange);
			});
			tmpMsg = msg;
		}
		
	}
	
	void sureExchange (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			
			ExchangeFPort exf = FPortManager.Instance.getFPort ("ExchangeFPort") as ExchangeFPort;
			exf.exchange ((tmpMsg.msgInfo as Exchange).sid, tmpMsg.msgNum, receiveOK);
			
		} else {
			tmpMsg = null;
			MaskWindow.UnlockUI();
		}
		
	}
	
	public void receiveOK (int sid, int num)
	{
		if (notice.getSample ().type == NoticeType.NEW_EXCHANGE) {
			NoticeActiveManagerment.Instance.updateExchange ((tmpMsg.msgInfo as NewExchange).timeID, sid, num);
		} else
			ExchangeManagerment.Instance.addExchange (sid, num);
		exchangeFinish ();
		UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
			win.dialogCloseUnlockUI=false;
			win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0132"),false);
			MaskWindow.UnlockUI();
		});
	}
	
	public void exchangeFinish ()
	{
		float y = content.transform.localPosition.y;
		//content.initContent (notice, win, this);
		//content.reLoad ();
		StartCoroutine(Utils.DelayRunNextFrame(()=>{content.jumpToPos(y);
		}));	
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
