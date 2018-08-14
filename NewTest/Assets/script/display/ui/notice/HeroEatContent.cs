
using System;
using UnityEngine;

public class HeroEatContent:MonoBase
{
	public ButtonHeroEat buttonEat;
	private Timer timer;
	private int[] heroEatInfo;
	[HideInInspector]
	public WindowBase
		win;//活动窗口

	public void initContent (WindowBase win)
	{
		this.win = win;
		buttonEat.content = this;
		buttonEat.fatherWindow = win;
		updateTime ();
		startTimer ();
	}

	private void startTimer ()
	{
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
		timer.addOnTimer (updateTime);
		timer.start ();
	}

	private void updateTime ()
	{
		heroEatInfo = NoticeManagerment.Instance.getHeroEatInfo ();
		//防止切换活动，timer没来得及关闭
		if (buttonEat != null) {
			int now = ServerTimeKit.getSecondTime ();
			if (heroEatInfo != null && heroEatInfo [3] == 0 && heroEatInfo [1] < now && now < heroEatInfo [2]) {
				buttonEat.disableButton (false);
			} else if (heroEatInfo != null && heroEatInfo [3] > 0 && heroEatInfo [1] < now && now < heroEatInfo [2]) {
				buttonEat.disableButton (true);
				buttonEat.textLabel.text = LanguageConfigManager.Instance.getLanguage ("heroEatContent05");
			} else {
				buttonEat.disableButton (true);
				//当前时间超过结束时间，重新向后台取数据
				if (heroEatInfo != null && now > heroEatInfo [2]) {
					NoticetHeroEatInfoFPort fport = FPortManager.Instance.getFPort ("NoticetHeroEatInfoFPort") as NoticetHeroEatInfoFPort;
					fport.access (() => {
						heroEatInfo = NoticeManagerment.Instance.getHeroEatInfo ();
					});
				}
			}
		} else {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
		}
	}

}

