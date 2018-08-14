using UnityEngine;
using System.Collections;

/**
 * 公会学院技能升级按钮
 * @author 汤琦
 * */
public class GuildCollegeSkillUpButton : ButtonBase
{
	private string uid;//公会技能uid

	public void initInfo(string uid)
	{
		this.uid = uid;
	}


	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(GuildManagerment.Instance.isUpLearnGuildSkill(uid))
		{
			GuildLearnSkillFPort fport = FPortManager.Instance.getFPort("GuildLearnSkillFPort") as GuildLearnSkillFPort;
			fport.access(uid,learnBack);
		}

	}

	private void learnBack()
	{
		(fatherWindow as GuildCollegeWindow).collegeContent.updateItemByID(transform.parent.gameObject,uid);
	}
}
