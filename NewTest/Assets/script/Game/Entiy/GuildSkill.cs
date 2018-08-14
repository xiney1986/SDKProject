using UnityEngine;
using System.Collections;

/**
 * 公告技能实体类
 * @author 汤琦
 * */
public class GuildSkill 
{
	public GuildSkill (string sid,int level)
	{
		this.sid = sid;
		this.level = level;
	} 
	public string sid;//sid
	public int level = 0;//等级
	public string iconskillName;
	//技能描述(根据等级获得不同描述)
	public string getSkillIconName()
	{
		string name = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid(StringKit.toInt(sid)).icon;
		return name;
	}
	public string getDescribe ()
	{
		string desc = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid(StringKit.toInt(sid)).desc;
		AttrChangeSample[] change = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid(StringKit.toInt(sid)).attr;
		return DescribeManagerment.getDescribe (desc, level, change); 
	}

	public string getDescribeByLv (int lv)
	{
		string desc = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid(StringKit.toInt(sid)).desc;
		AttrChangeSample[] change = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid(StringKit.toInt(sid)).attr;
		return DescribeManagerment.getDescribe (desc, lv, change); 
	}
}
