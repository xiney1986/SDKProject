using UnityEngine;
using System.Collections;
using System;

/**
 * 邮件组件
 * @author 汤琦
 * */
public class ButtonMailItem : ButtonBase 
{
	public UISprite readIcon;//邮件是否读图标
	public UISprite neckIcon;//附件是否取图标
	public UILabel sendMailName;//发送邮件人名字
	public UILabel mailName;//邮件标题
	public UILabel receiveMailTime;//收件时间
	public UILabel surplusTime;//剩余时间
	public MailWindow win;
	public Mail mail;
	private const string READ = "mail_readed";//已读
	private const string UNREAD = "mail_noopen";//未读
	private const int SEVENDAY = 7 * 24 * 3600;//默认剩余时间初始为7天
	private const string NECK = "mail_attachment";//有附件

	private Timer timer;

	public  void initialize (Mail _mail)
	{
		win = fatherWindow as MailWindow;
		updateMail(_mail);
//		addTimer();
	}

	public void addTimer()
	{
		int currentTime = ServerTimeKit.getSecondTime();
		int mailTime = mail.etime - currentTime;
		if(mailTime > 0)
		{
			int hours = (int)(mailTime / 3600);
			int minutes = (int)(mailTime % 3600 / 60);
			int seconds = (int)(mailTime % 3600 % 60);

			if (hours >= 24) {
//				timer = TimerManager.Instance.getTimer (UserManager.TIMER_MINUTE30);
				//时间太长，没必要监控了
			}
			else if (hours < 24 && minutes >= 11) {
//				timer = TimerManager.Instance.getTimer (UserManager.TIMER_MINUTE5);
				//时间太长，没必要监控了
			} 
			else if (minutes < 11 && minutes >= 1) {
				timer = TimerManager.Instance.getTimer (UserManager.TIMER_MINUTE);
			}
			else {
				timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
			}

			if(timer != null) {
				timer.addOnTimer (updateTime);
				timer.start ();
			}
		}
	}

	public override void DoDisable ()
	{
		if(timer != null)
			timer.stop();
		base.DoDisable ();
	}

	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		int currentTime = ServerTimeKit.getSecondTime();
		int mailTime = mail.etime - currentTime;
		if(mailTime > 0) {
			ReadMailFPort fport = FPortManager.Instance.getFPort("ReadMailFPort") as ReadMailFPort;
			fport.access(mail.uid,readMailBack);

		}
		else
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("MailTimeOver"),null);
		}
	}
	
	private void readMailBack()
	{
		if(timer != null)
			timer.stop();
		UiManager.Instance.openWindow<MailInfoWindow>((win)=>{
			win.init(mail,mail.etime);
			win.initButtonMailItem(this);
		});
	}
	
	public void updateMail (Mail newMail)
	{
		if(newMail == null)
			return;
		else
		{
			mail = newMail;

			if(isRead(mail.status)||mail.hasRead)
			{
				readIcon.spriteName = READ;
			}
			else 
			{
				readIcon.spriteName = UNREAD;
			}

			if(mail.annex != null && mail.status != 2)
			{

				neckIcon.gameObject.SetActive (true);
			}
			else
			{

				neckIcon.gameObject.SetActive (false);
			}
			sendMailName.text = LanguageConfigManager.Instance.getLanguage("s0124");
			mailName.text = mail.theme;
			receiveMailTime.text = receiveTimeForm(mail.stime);
			updateTime();
			
		}
	}

	public void updateTime()
	{
		int currentTime = ServerTimeKit.getSecondTime();
		int mailTime = mail.etime - currentTime;
		if(mailTime > 0)
		{
			string str = "";
			int hours = (int)(mailTime / 3600);
			int minutes = (int)(mailTime % 3600 / 60);
			int seconds = (int)(mailTime % 3600 % 60);
			if (hours >= 24) {
				str = LanguageConfigManager.Instance.getLanguage ("s0109", (hours / 24).ToString ()) + LanguageConfigManager.Instance.getLanguage ("s0018");
			}
			else if (hours < 24 && hours >= 1) {
				str = LanguageConfigManager.Instance.getLanguage ("s0109", hours.ToString ()) + LanguageConfigManager.Instance.getLanguage ("s0019");
			} 
			else if (hours < 1 && minutes >= 1) {
				str = LanguageConfigManager.Instance.getLanguage ("s0109", minutes.ToString ()) + LanguageConfigManager.Instance.getLanguage ("s0020");
			}
			else {
				str = LanguageConfigManager.Instance.getLanguage ("s0109", seconds.ToString ()) + LanguageConfigManager.Instance.getLanguage ("s0021");
			}
			
			surplusTime.text = str;
		}
		else
		{
			surplusTime.text = LanguageConfigManager.Instance.getLanguage("s0261");
			MailManagerment.Instance.addNeedDelUids(mail.uid);
			win.changeButton(true);
			if(timer != null)
				timer.stop();
		}
	}

	//收件时间格式
	private string receiveTimeForm (int time)
	{
		DateTime dTime = TimeKit.getDateTimeMin (time);
		
		string month = "";
		string day = dTime.Day.ToString();
		if(dTime.Month < 10)
		{
			month = "0"+dTime.Month;
		}
		else
		{
			month = dTime.Month.ToString();
		}
		
		return  LanguageConfigManager.Instance.getLanguage("s0125",month.ToString(),day.ToString());
	}
	
	private bool isRead(int state)
	{
		if(state == 1||state == 2)
			return true;
		else
			return false;
	}
}
