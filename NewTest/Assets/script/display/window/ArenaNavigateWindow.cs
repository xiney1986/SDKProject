using UnityEngine;
using System.Collections;

/// <summary>
/// 竞技场主界面
/// </summary>
public class ArenaNavigateWindow : WindowBase
{
	public ContentArean content;
	public GameObject arenaButtonPrefab;
	private Timer timer;

	public override void OnStart ()
	{
		base.OnStart ();
		arenaButtonPrefab.SetActive (false);
	}
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		Transform[] childs = content.GetComponentsInChildren<Transform> ();
		ButtonArenaItem arenaItem;
		foreach (Transform item in childs) {
			arenaItem = item.gameObject.GetComponent<ButtonArenaItem> ();
			if (arenaItem != null) {
				arenaItem.OnNetResume ();
			}
		}
	}

	protected override void begin ()
	{
		base.begin ();
        if (GuideManager.Instance.isEqualStep(133002000) || GuideManager.Instance.isEqualStep(121002000) || GuideManager.Instance.isEqualStep(171001000)) ;
        {
			GuideManager.Instance.doGuide ();
		}
		if (!isAwakeformHide) {
			content.reLoad ();
		}
		startTimer ();
		MaskWindow.UnlockUI ();
	}

    protected override void DoEnable() {
        base.DoEnable();
        UiManager.Instance.backGround.switchBackGround("backGround_1");
        //UiManager.Instance.backGroundWindow.switchToDark ();
    }

	private void startTimer ()
	{
		if (timer == null)
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updateTime);
		timer.start ();
	}

	private void updateTime ()
	{
		if (content == null || content.transform.childCount < 1)
			return;
		Transform[] childs = content.GetComponentsInChildren<Transform> ();
		ButtonArenaItem arenaItem;
		foreach (Transform item in childs) {
			arenaItem = item.gameObject.GetComponent<ButtonArenaItem> ();
			if (arenaItem != null) {
				arenaItem.updateTime ();
			}
		}
	}


	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") { 
			finishWindow ();			
		} 
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}
}

