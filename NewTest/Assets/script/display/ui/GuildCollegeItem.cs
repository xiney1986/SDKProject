using UnityEngine;
using System.Collections;

/**
 * 公会学院单项
 * @author 汤琦
 * */
public class GuildCollegeItem : MonoBase
{
	public UITexture icon;//图标
	public UILabel iconName;//图标名
	public UILabel need;//需求
	public UILabel desc;//提示文本
	public GuildCollegeSkillUpButton upgradeButton;//升级按钮
	public UILabel stateLabel;//状态文本
	private GuildSkillSample skillSample;//公会学院技能模板
	private GuildSkill skill;//公会技能
	private string skillName;//技能图标名
	private GuildCollegeWindow win;
	private string sid;

	public void initInfo (string sid, GuildCollegeWindow win)
	{
		this.sid = sid;
		this.win = win;
		skillSample = GuildSkillSampleManager.Instance.getGuildSkillSampleBySid (StringKit.toInt (sid));
		skill = GuildManagerment.Instance.getGuildSkillBySid (sid);
		skillName = skillSample.icon;
		updateInfo ();
	}

	private void updateInfo ()
	{
		iconName.text = skillSample.skillName;
		string path = (ResourcesManager.SKILLIMAGEPATH + skillName);
		ResourcesManager.Instance.LoadAssetBundleTexture (path, icon);

		//没有学过公会技能
		if (skill == null) {
			if (GuildManagerment.Instance.getBuildLevel (GuildManagerment.COLLEGE) >= skillSample.openLevel) {
				stateLabel.text = LanguageConfigManager.Instance.getLanguage ("Guild_44");
			} else {
				stateLabel.text = LanguageConfigManager.Instance.getLanguage ("Guild_55");
			}
			need.text = skillSample.costs [0].ToString ();
			desc.text = skillSample.getDescribe ();
		}
		//学了公会技能
		else {
			//技能需求
			if (skill.level >= GuildBuildSampleManager.Instance.getGuildBuildSampleBySid (StringKit.toInt (GuildManagerment.COLLEGE)).levelMax) {
				need.text = LanguageConfigManager.Instance.getLanguage ("Guild_89");
			}
			else {
				need.text = skillSample.costs [skill.level].ToString ();
			}
			//技能等级
			if (GuildManagerment.Instance.getBuildLevel (GuildManagerment.COLLEGE) < skillSample.openLevel) {
				stateLabel.text = LanguageConfigManager.Instance.getLanguage ("Guild_55");
				desc.text = skill.getDescribeByLv (0);
				upgradeButton.textLabel.text=LanguageConfigManager.Instance.getLanguage("Guild_34");
			}
			else {
				stateLabel.text = "LV." + skill.level + "/" + GuildManagerment.Instance.getBuildLevel (GuildManagerment.COLLEGE).ToString();
				iconName.text = skillSample.skillName;
				desc.text = skill.getDescribeByLv (skill.level);

//				if (skill.level < GuildManagerment.Instance.getBuildLevel (GuildManagerment.COLLEGE)) {
//					//stateLabel.text = "LV." + skill.level;
//					stateLabel.text = "";
//					iconName.text = skillSample.skillName+ "  LV." + skill.level + "/"  + skillSample.openLevel;
//					desc.text = skill.getDescribeByLv (skill.level);
//				} else {
//					//stateLabel.text = "LV." + GuildManagerment.Instance.getBuildLevel (GuildManagerment.COLLEGE);
//					stateLabel.text = "";
//					iconName.text = skillSample.skillName +"  LV." + GuildManagerment.Instance.getBuildLevel (GuildManagerment.COLLEGE) + "/" + skillSample.openLevel;
//					desc.text = skill.getDescribeByLv (GuildManagerment.Instance.getBuildLevel (GuildManagerment.COLLEGE));
//				}
			}

		}
		upgradeButton.fatherWindow = win;
		upgradeButton.initInfo (sid);
	}

}
