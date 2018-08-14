using UnityEngine;
using System.Collections;

/// <summary>
/// 神秘商店时间表容器 
/// </summary>
public class MysticlShopTimeContent : dynamicContent {
	/**预制体 */
	public TimeListPerfab timeListPerfab;
	private string[] timelist;
	public void reLoad(string[] list)
	{
		timelist=list;
		base.reLoad(list.Length);
	}
	public override void updateItem (GameObject item, int index)
	{ 
		TimeListPerfab button = item.GetComponent<TimeListPerfab> ();
		button.init (timelist [index] ); 
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null)
		{
			nodeList [i] = NGUITools.AddChild(gameObject, timeListPerfab.gameObject);
		}
		nodeList [i].name = StringKit. intToFixString (i + 1);
		TimeListPerfab button= nodeList [i].GetComponent<TimeListPerfab> ();
		button.init (timelist [i] ); 
		
	}
}
