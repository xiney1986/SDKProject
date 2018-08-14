using UnityEngine;
using System.Collections;

/// <summary>
/// 废弃的类
/// </summary>
public class PracticeWindow : WindowBase
{
	public ContentPracticeChoose content;//容器
	public UILabel ruleDescript;//副本描述
	public UILabel warCountLabel;//次数
	public GameObject practiceBarPrefab;
	 
	int[] missionList;//副本sid集


	protected override void begin ()
	{
		base.begin ();
		content.Initialize (missionList);
		MaskWindow.UnlockUI ();
	}
	public override void OnOverAnimComplete ()
	{
		base.OnOverAnimComplete ();
		//切换后清空容器内容
		content.cleanAll();
		
	}
	protected override void DoEnable ()
	{
		base.DoEnable ();
		refreshData ();

	}
	public void refreshData ()
	{
		showInfo ();
	}
	
	public void showInfo ()
	{
		Chapter 	chapter=FuBenManagerment.Instance.getPracticeChapter ();
			warCountLabel.text = LanguageConfigManager.Instance.getLanguage ("s0363") + ":" + chapter.getNum () + "/" + chapter.getMaxNum ();
			missionList = FuBenManagerment.Instance.getAllShowMissions (chapter.sid);
			ruleDescript.text = LanguageConfigManager.Instance.getLanguage ("s0364");

	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow();
		}
	}
}
