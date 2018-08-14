using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankLadderContent : dynamicContent
{
	public RankWindow window;
	public GameObject itemPrefab;
	private List<PvpOppInfo> data;

	public void reLoad (List<PvpOppInfo> _data)
	{
		data = _data;
		base.reLoad (data.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		RankLadderItemView rankItem = item.GetComponent<RankLadderItemView> ();
		rankItem.M_update(data[index],index+1);
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject,itemPrefab);
			nodeList [i].SetActive(true);
			nodeList [i].name=StringKit.intToFixString(i+1);
			RankLadderItemView view = nodeList [i].GetComponent<RankLadderItemView> ();				
			view.M_update(data[i],i+1);
			view.setFatherWindow(this.fatherWindow);
		}
	}
}

