using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会学院分页
 * @author 汤琦
 * */
public class GuildCollegeContent : dynamicContent
{
	public UILabel contribution;//个人贡献
	private List<string> skillIds;

	public void Initialize ()
	{
		skillIds = GuildSkillSampleManager.Instance.getAllGuildSkill();
		contribution.text = GuildManagerment.Instance.getGuild().contributioning.ToString();
		base.reLoad (skillIds.Count); 
	}
	public void reLoad()
	{
		skillIds = GuildSkillSampleManager.Instance.getAllGuildSkill();
		contribution.text = GuildManagerment.Instance.getGuild().contributioning.ToString();
		base.reLoad(skillIds.Count);
	}

	public void updateItemByID(GameObject item,string skillId)
	{
		GuildCollegeItem college = item.GetComponent<GuildCollegeItem> ();
		college.initInfo (skillId,fatherWindow as GuildCollegeWindow); 
		contribution.text = GuildManagerment.Instance.getGuild().contributioning.ToString();
	}
	public override void updateItem (GameObject item, int index)
	{
		GuildCollegeItem college = item.GetComponent<GuildCollegeItem> ();
		college.initInfo (skillIds[index],fatherWindow as GuildCollegeWindow); 
		contribution.text = GuildManagerment.Instance.getGuild().contributioning.ToString();
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as GuildCollegeWindow).guildCollegeItem);
		}
		nodeList [i].name = StringKit. intToFixString (i + 1);
		GuildCollegeItem college = nodeList [i].GetComponent<GuildCollegeItem> ();
		college.initInfo (skillIds [i],fatherWindow as GuildCollegeWindow);
	}
	void OnDisable()
	{
		cleanAll();
	}
}
