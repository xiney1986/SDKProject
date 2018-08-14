using UnityEngine;
using System.Collections;

public class LuckyRankWindow : WindowBase {

	/** (全)排行榜容器 */
	public RankContent rankAllContent;
	public GameObject mask;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI();
	}

	public void initWindow(int type, IList list, WindowBase fatherWindow)
	{
		rankAllContent.init(type,list,fatherWindow);
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if(gameObj.name == "close")
		{
			mask.gameObject.SetActive(false);
			finishWindow();
		}
	}
}