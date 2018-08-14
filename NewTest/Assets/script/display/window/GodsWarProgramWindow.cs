using UnityEngine;

/// <summary>
/// 诸神战赛程窗口
/// </summary>
public class GodsWarProgramWindow : WindowBase {
	/// <summary>
	/// 赛程时间label
	/// </summary>
	public UILabel[] timeLabels;
	/// <summary>
	/// 变量-赛程描述列表
	/// </summary>
	private string[] timeDesc;
	public GameObject mask;

	protected override void begin ()
	{
		base.begin ();
	    UiManager.Instance.godsWarProgramWindow = this;
		initUI();
	}
	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initUI(){
		getTimeDesc();
		getDayOfTime();
		MaskWindow.UnlockUI ();
	}

	/// <summary>
	/// 获取赛程描述
	/// </summary>
	private void getTimeDesc()
	{
		GodsWarInfoConfigManager manager = GodsWarInfoConfigManager.Instance();
		timeDesc = new string[timeLabels.Length];
		for(int i=0;i< timeLabels.Length;i++)
		{
			timeDesc[i]=manager.getSampleBySid(i+1).des;
		}
	}

	/// <summary>
	/// 获得星期
	/// </summary>
	private void getDayOfTime()
	{
		System.DateTime serverDate = ServerTimeKit.getDateTime ();
		int day = TimeKit.getWeekCHA(serverDate.DayOfWeek);
		setTimeLabel(day);
	}
	/// <summary>
	/// 设置时间标签
	/// </summary>
	public void setTimeLabel(int type)
	{
//		int num = type-1;
//		for(int i= num;i>=0;i--)
//		{
//			timeLabels[i].text = getDateTime(ServerTimeKit.getSecondTime()-86400*(num-i))+"  "+timeDesc[i];
//		}
//		for(int i= num+1;i<timeLabels.Length;i++)
//		{
//			timeLabels[i].text = getDateTime(ServerTimeKit.getSecondTime()+86400*(i-num))+"  "+timeDesc[i];
//		}
		int lastMonday = ServerTimeKit.getSecondTime()-86400*(type-1);
		for(int i= 0;i<3;i++)
		{
			timeLabels[i].text = getDateTime(lastMonday + 86400*i)+"  "+timeDesc[i];
		}
		for(int i=3;i<7;i++)
		{
			timeLabels[i].text = getDateTime(lastMonday + 86400*3)+"  "+timeDesc[i];
		}
		timeLabels[7].text = getDateTime(lastMonday + 86400*4)+"  "+timeDesc[7];
	}

	/// <summary>
	/// 获得日期
	/// </summary>
	public string getDateTime(int secondTime)
	{
		return TimeKit.dateToFormat(secondTime,LanguageConfigManager.Instance.getLanguage ("notice04"));
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
        if (gameObj.name == "ButtonClose")
        {
			mask.gameObject.SetActive(false);
            finishWindow();
        }
	}
    void OnDestroy() {
        UiManager.Instance.godsWarProgramWindow = null;
    }
}
