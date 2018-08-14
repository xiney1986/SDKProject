using UnityEngine;
using System.Collections;

public class NoticeIntroduce : MonoBase
{
	public UILabel timeText;//活动时间
	public UILabel activityDes;//活动描述
	public ButtonBase buttonOk;

	public void showTime (string str)
	{
		timeText.text = str;
	}

	public void initInfo(Notice notice)
	{

		activityDes.text = notice.getSample ().activiteDesc;
		int time = ServerTimeKit.getSecondTime ();
		timeText.text = LanguageConfigManager.Instance.getLanguage ("s0135",timeTransform (0- time));
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
