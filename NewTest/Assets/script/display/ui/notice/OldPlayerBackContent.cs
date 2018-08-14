using UnityEngine;
using System.Collections;

public class OldPlayerBackContent : MonoBase
{
	WindowBase fatherWin;
	public BackPrizeContent backPrizeContent;
	public UILabel timeValue;
	int endTimes;
	NoticeTopButton button;


	public void initContent(Notice notice,WindowBase win,NoticeTopButton button)
	{
		this.button = button;
		this.fatherWin = win;
		endTimes = BackPrizeLoginInfo.Instance.endTimes;
		backPrizeContent.reLoad(fatherWin,this.button);
	}

	void Update()
	{
		if((endTimes - ServerTimeKit.getSecondTime()) <= 0)// 双倍经验时间已结束//
		{
			timeValue.text = LanguageConfigManager.Instance.getLanguage("Arena73");
		}
		else
		{
			timeValue.text = TimeKit.timeTransform ((endTimes - ServerTimeKit.getSecondTime())*1000.0d);
		}
	}
}
