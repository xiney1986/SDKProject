using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 邮件信息窗口
 * @author 汤琦
 * */
public class MailInfoWindow : WindowBase
{
	public UILabel sendName;//发件人
	public UILabel title;//标题
	public UILabel content;//内容
	private Mail mail;
	public UIGrid contentMail;
	private Timer timer;//计时器
	private int mailTime = 0;
	public ButtonBase deleteButton;
	public ButtonBase extractButton;
	public GameObject goodsViewPrefab;
	private bool isDelMail = false;//删除了邮件
	private bool isGetMail = false;//领取附件
	
	protected override void begin ()
	{
		base.begin (); 
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updateTime);
		timer.start ();
		MaskWindow.UnlockUI ();
	}
	 
	private void updateTime ()
	{
		int currentTime = ServerTimeKit.getSecondTime ();
		int overTime = mailTime - currentTime;
		if (overTime <= 0) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0330"), closeWindow);
		}
	}
	
	private void closeWindow (MessageHandle msg)
	{
		MailManagerment.isUpdateMailInfo = true;
		finishWindow ();
	}

	public void init (Mail _mail, int mailTime)
	{
		deleteButton.disableButton (false);
		if (_mail.annex != null && _mail.status != 2)
			extractButton.disableButton (false);
		else
			extractButton.disableButton (true);
		mail = _mail;
		this.mailTime = mailTime;
		if (mail.status == 0) {
			MailManagerment.Instance.readMail (mail);
		}
		initInfo ();
	}

	private ButtonMailItem fromItem;

	public void initButtonMailItem (ButtonMailItem btnItem)
	{
		fromItem = btnItem;
	}
	
	private void initInfo ()
	{
		title.text = LanguageConfigManager.Instance.getLanguage ("s0126", mail.theme);
//		content.text = LanguageConfigManager.Instance.getLanguage ("s0127", mail.content);
		content.text = mail.content;
		if (mail.status == 2) {
			contentMail.gameObject.SetActive (false);
			return;
		}
		if (mail.annex != null) {
			for (int i = 0; i < mail.annex.Length; i++) {
				createGoodsView (getPrize (mail.annex [i]), i);
			}
			contentMail.repositionNow = true;
		}
	}

	private void createGoodsView (PrizeSample prizeSample, int index)
	{
		if (prizeSample == null)
			return;
		GameObject a = NGUITools.AddChild (contentMail.gameObject, goodsViewPrefab);
		a.transform.localScale = new Vector3(0.8f,0.8f,1f);
		a.name = StringKit.intToFixString (index + 1);
		GoodsView goodsButton = a.GetComponent<GoodsView> ();
		goodsButton.fatherWindow = this;
		goodsButton.onClickCallback = goodsButton.DefaultClickEvent;
		goodsButton.init (prizeSample);
	}

	private PrizeSample getPrize (Annex annex)
	{
		if (annex.exp != null)
			return annex.exp;
		else if (annex.money != null)
			return annex.money;
		else if (annex.prop != null)
			return annex.prop;
		else if (annex.pve != null)
			return annex.pve;
		else if (annex.pvp != null)
			return annex.pvp;
		else if (annex.rmb != null)
			return annex.rmb;
		else if (annex.starsoulDebris != null)
			return annex.starsoulDebris;
		else if (annex.ladder != null)
			return annex.ladder;
		else if (annex.contribution != null)
			return annex.contribution;
		else if(annex.mounts!=null){
			return annex.mounts;
		}
		else
			return null;
	}
	
	private int changeMail (int mailAnnexs)
	{
		int num = mailAnnexs / 4;
		if (num < 1)
			num = 1;
		return num;
	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
		if (timer != null)
			timer.stop ();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
		if (gameObj.name == "extract") {
			if (mail.status != 2) {
				if (MailManagerment.Instance.isMailExtract (mail)) {
					sendExtractFPort ();
					return;
				} else {
					UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
						win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, MailManagerment.Instance.getStr (), null);
					});
					return;
				}
			} else {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, LanguageConfigManager.Instance.getLanguage ("s0321"), null);
				});
				return;
			}
		}
		if (gameObj.name == "delete") {
			string msg = LanguageConfigManager.Instance.getLanguage ((mail.status != 2 && mail.annex != null) ? "s0113" : "s0110");

			UiManager.Instance.openDialogWindow<MessageWindow> ((win) =>
			{
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0040"), LanguageConfigManager.Instance.getLanguage ("s0093"), msg, sendDeleteFPort);
			});
            
			return;
		}
		if (gameObj.name == "close") {
			if (fromItem != null) {
				mail.hasRead = true;
				fromItem.updateMail (mail);
				fromItem = null;
			}
			finishWindow ();
			return;
		}
	}

	//领取附件
	private void sendExtractFPort ()
	{
		isGetMail = true;
		ExtractMailAnnexFPort fport = FPortManager.Instance.getFPort ("ExtractMailAnnexFPort") as ExtractMailAnnexFPort;
		fport.access (mail.uid, sendExtractBack);
	}
	
	private void sendExtractBack ()
	{
        //UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0202"));
		if (mail.annex != null) {
			Card c;
            List<PrizeSample> prizes = new List<PrizeSample>();
			for (int i = 0; i < mail.annex.Length; i++) {
				PrizeSample ps = getPrize (mail.annex [i]);
				if (ps.type == PrizeType.PRIZE_CARD) {
					c = CardManagerment.Instance.createCard (ps.pSid);
					for (int j = 0; j < ps.getPrizeNumByInt (); j++) {
						if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed (c))
							TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("s0418"));
					}
				}
                prizes.Add(ps);
			}
            UiManager.Instance.createPrizeMessageLintWindow(prizes.ToArray());
           
		}
		extractBack ();
	}
	
	private void extractBack ()
	{
		MailManagerment.isUpdateMailInfo = true;
		MailManagerment.Instance.extractMailByUid (mail.uid);
		finishWindow ();
	}
	
	private void deleteBack ()
	{
		MailManagerment.isUpdateMailInfo = true;
		MailManagerment.Instance.deleteMailByUid (mail.uid);
		finishWindow ();
	}

	//删除邮件
	private void sendDeleteFPort (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		isDelMail = true;
		DeleteMailFPort fport = FPortManager.Instance.getFPort ("DeleteMailFPort") as DeleteMailFPort;
		fport.access (mail.uid, deleteBack);
	}

	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		MailManagerment.isUpdateMailInfo = true;
		finishWindow ();
	}
}
