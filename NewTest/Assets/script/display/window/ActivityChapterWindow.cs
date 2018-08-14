using UnityEngine;
using System.Collections;

//活动主界面

public class ActivityChapterWindow : WindowBase
{
	public ContentActivity content;
	public GameObject activityChapterButtonPrefab;
	protected override void begin ()
	{
		base.begin ();
		GuideManager.Instance.guideEvent ();
		
		if(!isAwakeformHide)
			content.reLoad ();
		updateTimes ();
		MaskWindow.UnlockUI ();
	}
    /// <summary>
    /// 断线重连
    /// </summary>
    public override void OnNetResume() {
        base.OnNetResume();
        FuBenInfoFPort port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
        port.info(updateTimes, ChapterType.ACTIVITY_CARD_EXP);
    }

	private void updateTimes(){
		for (int i=0; i<content.transform.childCount; i++) {
			content.transform.GetChild(i).GetComponent<ButtonActivityChapter>().updateTimes();
		}
	}
	protected override void DoEnable () {
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") { 
			finishWindow();	
		} 
	} 
}
