using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 诸神之战准备界面
/// </summary>
public class GodsWarPreparWindow: WindowBase { 

	#region 常量
	private const int TYPE_6 = 6, //周六
						TYPE_7 = 7,//周日
						TYPE_1 = 1,//周一
						TYPE_2 = 2,//周二
						TYPE_3 = 3,//周三
						TYPE_4 = 4,//周四
						TYPE_5 = 5;//周五

	#endregion

	#region 变量

	private int type;//赛程类别
	private List<GodsWarPrizeSample> ArrayGodsPrizeSample;//冠军礼包
	private List<PrizeSample> listPrzieSample;//奖励列表
	private int todaySecondTime;//当天的时间
	private int week;//当天星期几

	#endregion


	#region NGUI

	public GoodsView[] prizes;//终极礼包
	public UILabel [] dayTimeLabel;//具体时间列表
	public UISprite[] checkSprite;//选中当天

	#endregion


	protected override void begin () {
		MaskWindow.UnlockUI ();
		base.begin ();
		setDateTime();
		showPrize();
		showCheckDay();
	}

	/// <summary>
	/// 初始化窗口
	/// </summary>
	public void initWindow (int type) {

		this.type = type;
	}

	/// <summary>
	/// 显示赛程
	/// </summary>
	public void showCheckDay()
	{
		setTimeLabel(week);
	}

	/// <summary>
	/// 设置时间标签
	/// </summary>
	public void setTimeLabel(int type)
	{
		int num;
		if(type==TYPE_6||type==TYPE_7)
		{
			num = type-6;;
		}
		else
			num = type+1;
		checkSprite[num].gameObject.SetActive (true);
		for(int i= num;i>=0;i--)
		{
			dayTimeLabel[i].text = getDateTime(todaySecondTime-86400*(num-i));
		}
		for(int i= num+1;i<=6;i++)
		{
			dayTimeLabel[i].text = getDateTime(todaySecondTime+86400*(i-num));
		}
	}

	/// <summary>
	/// 获取当天的日期
	/// </summary>
	public void setDateTime()
	{
		System.DateTime serverDate = ServerTimeKit.getDateTime ();
		int day = TimeKit.getWeekCHA(serverDate.DayOfWeek);
		this.week = day;
		this.todaySecondTime = ServerTimeKit.getSecondTime();
	}
	/// <summary>
	/// 获得日期
	/// </summary>
	public string getDateTime(int secondTime)
	{
		return TimeKit.dateToFormat(secondTime,LanguageConfigManager.Instance.getLanguage ("notice04"));
	}


	/// <summary>
	/// 显示终极奖励
	/// </summary>
	public void showPrize()
	{
		ArrayGodsPrizeSample = GodsWarPrizeSampleManager.Instance.getChampionPrize();
		if(ArrayGodsPrizeSample!=null)
		{
			for(int i=0;i<ArrayGodsPrizeSample.Count;i++)
			{
				listPrzieSample = ArrayGodsPrizeSample[i].item;
			}
		}
		if(listPrzieSample!=null)
		{
			for(int i =0;i<prizes.Length;i++)
			{
				if(i<listPrzieSample.Count)
				{
					prizes[i].init(listPrzieSample[i]);
					prizes[i].specialName.text = listPrzieSample[i].getPrizeName();
					prizes[i].gameObject.SetActive(true);
				}
			}
		}
	}

	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
}