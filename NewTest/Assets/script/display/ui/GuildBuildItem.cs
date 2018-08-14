using UnityEngine;
using System.Collections;

/**
 * 公会建筑窗口容器单项
 * @author 汤琦
 * */
public class GuildBuildItem : MonoBase 
{
	public UILabel level;//等级
	public UILabel desc;//描述
	public UILabel cost;//花费
	public GuildBuildButton button;//升级或建设按钮
	private GuildBuildSample build;//建筑
	public UISprite icon;
	//初始化信息
	public void initInfo(GuildBuildSample build)
	{
		this.build = build;
		updateInfo();
	}
	//更新信息
	public void updateInfo()
	{
		int buildLevel = GuildManagerment.Instance.getBuildLevel(build.sid.ToString());
		if(buildLevel >= build.levelMax)
		{
			button.disableButton(true);
		}
		else
		{
			if(GuildManagerment.Instance.getGuild().livenessing >= build.costs[buildLevel])
			{
				button.disableButton(false);
			}
			else
			{
				button.disableButton(true);
			}
		}
		if(buildLevel > 0)
		{
			button.textLabel.text = LanguageConfigManager.Instance.getLanguage("Guild_34");
		}
		else
		{
			button.textLabel.text = LanguageConfigManager.Instance.getLanguage("Guild_35");
		}
		button.callback = initInfo;
		button.initInfo(buildLevel,build.sid.ToString(),build.buildName);
//		desc.text = build.desc;
		cost.text = GuildManagerment.Instance.getBuildNeedsDesc(build.sid);
		level.text = "Lv." + buildLevel;
		icon.spriteName = getBuildIcon();
		icon.gameObject.SetActive(true);
	}

	private string getBuildIcon()
	{
//		switch(build.icon)
//		{
//		case 1:
//			return "Guild_The hall";
//		case 2:
//			return "Guild_College";
//		case 3:
//			return "Guild_Shop";
//		case 4:
//			return "Guild_Altar";
//		default:
			return "";
//		}
	}
}
