using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会申请列表容器
 * @author 汤琦
 * */
public class GuildApplyContent : dynamicContent
{
	private List<GuildRankInfo> guildList;
	public GameObject applyItem;

	public void	reLoad ()
	{
		guildList = GuildManagerment.Instance.getGuildList();
		if (guildList == null || guildList.Count <= 0)
			return;  
		 base.reLoad (guildList.Count);
	}
	public override void  updateItem (GameObject item, int index)
	{
		GuildApplyItem button = item.GetComponent<GuildApplyItem> ();				 
		button.updateActive (guildList [index]);
		
	}
	public override void initButton (int  i)
	{
		if (nodeList [i] == null)
			nodeList [i] = NGUITools.AddChild(gameObject,applyItem);
		
//		nodeList [i].transform.parent = transform;
//		nodeList [i].transform.localPosition = Vector3 .zero;
//		nodeList [i].transform.localScale = Vector3 .one; 
		nodeList [i].name = StringKit. intToFixString (i + 1);
		GuildApplyItem button = nodeList [i].GetComponent<GuildApplyItem> ();
		button.button.fatherWindow = fatherWindow;
		button.updateActive (guildList [i]);
	}
    /// <summary>
    /// 更新容器节点列表
    /// </summary>
    /// <param name="findGuildList"></param>
    public void updateContentNodeList(List<GuildRankInfo> findGuildList)
    {
		MaskWindow.UnlockUI ();
        if (findGuildList == null || findGuildList.Count <= 0)
        {
            guildList = new List<GuildRankInfo>();
            base.reLoad(guildList.Count);
            return;
        }
        guildList = findGuildList;
        base.reLoad(guildList.Count);
		MaskWindow.UnlockUI ();
    }
}
