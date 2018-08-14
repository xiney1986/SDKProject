using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 竞技场决赛赛程详解
/// </summary>
public class ArenaFinalPreduceTimeWindow : WindowBase {
	/** 容器 */
	public UIGrid Content;
	/** 描述预制体 */
	public GameObject DesPrefab;
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	public void initUI(Dictionary<string,string> desDic){
		UIUtils.M_removeAllChildren (Content.gameObject);
		foreach (KeyValuePair<string,string> entry in desDic) {
			ArenaTimeItem item = NGUITools.AddChild(Content.gameObject,DesPrefab).GetComponent<ArenaTimeItem>();
			item.initUI(entry.Key,entry.Value);
		}
		Content.Reposition ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			this.finishWindow();
		}
	}
}
