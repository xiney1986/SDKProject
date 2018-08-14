using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会建筑窗口容器
 * @author 汤琦
 * */
public class GuildBuildContent : dynamicContent
{
	private List<int> buildSidList;
	
	public void	reLoad ()
	{
		buildSidList = GuildBuildSampleManager.Instance.getAllGuildBuild();
		if (buildSidList == null || buildSidList.Count <= 0)
			return;  
		 base.reLoad (buildSidList.Count);
	}
	public override void  updateItem (GameObject item, int index)
	{
		GuildBuildView button = item.GetComponent<GuildBuildView> ();	
		GuildBuildSample sample = GuildBuildSampleManager.Instance.getGuildBuildSampleBySid(buildSidList[index]);
		button.initBuild (sample.sid,true,()=>{
			(fatherWindow as GuildBuildWindow).updateInfo (sample);
			MaskWindow.UnlockUI ();
		});
	}
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as GuildBuildWindow).buildItem);
		}
		nodeList [i].name = StringKit. intToFixString (i + 1);
		GuildBuildView button = nodeList [i].GetComponent<GuildBuildView> ();
		button.transform.localScale = new Vector3(0.7f,0.7f,0.7f);
		button.fatherWindow = fatherWindow;
		GuildBuildSample sample = GuildBuildSampleManager.Instance.getGuildBuildSampleBySid(buildSidList[i]);
		button.initBuild (sample.sid,true,()=>{
			(fatherWindow as GuildBuildWindow).updateInfo (sample);
			MaskWindow.UnlockUI ();
		});
//		if (i == 0) {
//			(fatherWindow as GuildBuildWindow).init (sample);
//		}
	}

	public void updateAllItems()
	{
		for (int i=0; i<nodeList.Count; i++) {
			updateItem (nodeList [i], i);
		}
	}
}
