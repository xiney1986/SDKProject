using UnityEngine;
using System.Collections;

/**
 * 任务系统窗口
 * @author 汤琦
 * */
public class TaskWindow : WindowBase
{
	public TapContentBase tapBase;//分页按钮
	public EveryDayTask everyDay;//每日任务页
	public MainLineTask mainLine;//主线任务页
	public UILabel everyDayTaskedNum;//每日任务页完成了任务的数量
	public UILabel mainLineTaskedNum;//主线任务页完成了任务的数量
	public GameObject taskitem;
	/** 行军值经验条 */
	public barCtrl guildPveBar;
	/** 行军值 */
	public UILabel guildPveValue;
	/** 行军值介绍 */
	public UILabel guildPveDesc;
	private const int EVERYDAYTASKNUM = 4;//每日任务页任务条目数量
	private const int MAINLINETASKNUM = 4;//主线任务页任务条目数量
	private int tempNum = 0;//控制
	private Guild guild;
	private GuildFightSampleManager instance;
	private CallBack back;//窗口关闭回调
	
	protected override void begin ()
	{
		base.begin ();
		everyDayTaskedNum.text = TaskManagerment.Instance.getEveryDayTaskedNum ().ToString ();
		mainLineTaskedNum.text = TaskManagerment.Instance.getMainLineTaskedNum ().ToString ();
		guild = GuildManagerment.Instance.getGuild ();
		instance = GuildFightSampleManager.Instance();
		updateGuildPve ();
		guildPveDesc.text = Language ("task04",instance.getTaskPowerBySid (GuildFightSampleManager.TASK_POWER).ToString ());
		MaskWindow.UnlockUI ();
	}

	protected override void DoEnable () {
		base.DoEnable ();
		updateContent ();
		UiManager.Instance.backGround.switchBackGround("backGround_1");
	}

	public void updateGuildPve ()
	{
		if (guild != null && instance.isGuildFightOpen ()) {
			guildPveBar.gameObject.SetActive (true);
			int power = UserManager.Instance.self.guildFightPower;
			int powerMax = UserManager.Instance.self.guildFightPowerMax;
			guildPveBar.updateValue (power, powerMax);
			guildPveValue.text = power + "/" + powerMax;
		} else {
			guildPveBar.gameObject.SetActive (false);
		}
	}

	public void initTap (int tapIndex)
	{
		tapBase.changeTapPage (tapBase.tapButtonList [tapIndex]);
	}
	
	public void updateContent ()
	{
		if (tempNum == 0) {

			float y = everyDay.transform.localPosition.y;
			everyDay.reLoad (TaskManagerment.Instance.getEveryDayTask (UserManager.Instance.self.getUserLevel ()));
			StartCoroutine(Utils.DelayRunNextFrame(()=>{
				everyDay.jumpToPos(y);
			}));
		} else {
			float y = mainLine.transform.localPosition.y;
			mainLine.reLoad (TaskManagerment.Instance.getMainLineTask (UserManager.Instance.self.getUserLevel ()));
			StartCoroutine(Utils.DelayRunNextFrame(()=>{
				mainLine.jumpToPos(y);
			}));
		}
		everyDayTaskedNum.text = TaskManagerment.Instance.getEveryDayTaskedNum ().ToString ();
		mainLineTaskedNum.text = TaskManagerment.Instance.getMainLineTaskedNum ().ToString ();
		updateGuildPve ();
	}
	
	public void Initialize (CallBack back)
	{
		if (back != null) {
			this.back = back;
		}
		everyDayTaskedNum.text = TaskManagerment.Instance.getEveryDayTaskedNum ().ToString ();
		mainLineTaskedNum.text = TaskManagerment.Instance.getMainLineTaskedNum ().ToString ();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{ 
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") { 
			//GuideManager.Instance.doGuide ();
			finishWindow ();
			if (back != null) {
				back ();
			}
			back = null;
		}
		
	}
	
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "mainLine" && enable == true) {
			//GuideManager.Instance.doGuide ();
			mainLine.Initialize (TaskManagerment.Instance.getMainLineTask (UserManager.Instance.self.getUserLevel ()));
			tempNum = 1;
		}
		if (gameObj.name == "everyDay" && enable == true) {
			everyDay.Initialize (TaskManagerment.Instance.getEveryDayTask (UserManager.Instance.self.getUserLevel ()));
			tempNum = 0;
		}
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		updateContent ();
	}
}
