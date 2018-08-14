using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会审批容器
 * @author 汤琦
 * */
public class GuildAppContent : dynamicContent
{
	private List<GuildApprovalInfo> appList;
	//没有成员的提示
	public UILabel noPerson;
	public void	reLoad ()
	{
		cleanAll();
		appList = GuildManagerment.Instance.getGuildApprovalList();
		if(appList == null||appList.Count<1){
			noPerson.gameObject.SetActive(true);
			return;
		}
		noPerson.gameObject.SetActive(false);	
		 base.reLoad (appList.Count);
	}
	public override void  updateItem (GameObject item, int index)
	{
		GuildAppItem button = item.GetComponent<GuildAppItem> ();				 
		button.updateInfo (appList [index],fatherWindow as GuildAppWindow);
		
	}
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as GuildAppWindow).guildAppItem);
		} 
		nodeList [i].name = StringKit. intToFixString (i + 1);
		GuildAppItem button = nodeList [i].GetComponent<GuildAppItem> ();
		button.updateInfo (appList [i],fatherWindow as GuildAppWindow);
	}

}
