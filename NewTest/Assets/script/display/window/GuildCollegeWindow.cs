using UnityEngine;
using System.Collections;

public class GuildCollegeWindow : WindowBase {
	
	public GuildCollegeContent collegeContent;//学院
	public GameObject guildCollegeItem;
	public bool isChange;//工会信息是否需要刷新
	
	protected override void DoEnable()
	{
		//UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();
	}
	
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	public override void OnAwake () {
		base.OnAwake ();
		GuildManagerment.Instance.clearUpdateMsg ();
		if (GuildManagerment.Instance.getGuildSkill () == null) {
			GuildGetSkillFPort fport = FPortManager.Instance.getFPort ("GuildGetSkillFPort") as GuildGetSkillFPort;
			fport.access (intoCollege);
		} else {
			intoCollege ();
		}
	}

	private void openGuildCollege ()
	{
		GuildBuildLevelGetFPort fport = FPortManager.Instance.getFPort ("GuildBuildLevelGetFPort") as GuildBuildLevelGetFPort;
		fport.access (intoCollege);
	}
	public void initWindow ()
	{
		if (GuildManagerment.Instance.getGuildSkill () == null) {
			GuildGetSkillFPort fport = FPortManager.Instance.getFPort ("GuildGetSkillFPort") as GuildGetSkillFPort;
			fport.access (openGuildCollege);
		} else {
			openGuildCollege ();
		}
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} 
	}
	
	IEnumerator waitUnlockUI (float time)
	{
		yield return new WaitForSeconds (time);
		MaskWindow.UnlockUI ();
	}
	private void closeWindow ()
	{
		UiManager.Instance.switchWindow<MainWindow> ((win) => { });
	}
	private void intoCollege ()
	{
		collegeContent.reLoad ();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		GuildManagerment.Instance.clearUpdateMsg ();
		//		SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFTGUILDSHOP);
		//		sc.clearSortCondition ();
		if (MissionManager.instance != null)
		{
			MissionManager.instance.showAll ();
			MissionManager.instance.setBackGround();
		}
	}
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		initWindow ();
	}
}
