using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会祭坛容器
 * @author 汤琦
 * */
public class GuildAltarContent : MonoBehaviour
{
	public UITexture bossIcon;//BOSS图片
	public UILabel bossName;//BOSS名
	public UILabel valueSum;//公会伤害总值
	public UILabel myValue;//自己的伤害总值
	public GuildInRankItem[] items;//排名项
	public UILabel countLabel;
	public GameObject prompt;//提示
	public ButtonBase bossButton;
	public ButtonBase weakNessButton;//查看弱点按钮
	public UILabel fightTeamNameLabel;

	void OnEnable()
	{
		fightTeamNameLabel.text=ArmyManager.Instance.getActiveArmyName ();
	}
	public void initInfo()
	{ 
		countLabel.text = LanguageConfigManager.Instance.getLanguage("Guild_98");
		if((ServerTimeKit.getDateTime().Hour >= 0 && ServerTimeKit.getDateTime().Second > 0) && (ServerTimeKit.getDateTime().Hour < 6 && ServerTimeKit.getDateTime().Second <= 59))
		{
			prompt.gameObject.SetActive(true);
			weakNessButton.disableButton(true);
			bossButton.disableButton(true);
			return;
		}
		else
		{
			prompt.gameObject.SetActive(false);
			weakNessButton.disableButton(false);
		}

		if(GuildManagerment.Instance.getGuildAltar() == null || 3 - GuildManagerment.Instance.getGuildAltar().count <= 0)
		{
			bossButton.disableButton(true);
			countLabel.text = LanguageConfigManager.Instance.getLanguage("Guild_81");
		}
		else
		{
			bossButton.disableButton(false);
			countLabel.text =LanguageConfigManager.Instance.getLanguage("Guild_98")+"("+ (3 - GuildManagerment.Instance.getGuildAltar().count) + "/" + 3+")";
		}


		if(GuildManagerment.Instance.getGuildAltar() == null)
		{
			weakNessButton.disableButton(true);
			return;
		}
		else
		{
			weakNessButton.disableButton(false);
		}

		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + GuildManagerment.Instance.getGuildBossIcon(), bossIcon);
		bossName.text = GuildManagerment.Instance.getGuildBossName();
		valueSum.text = GuildManagerment.Instance.getGuildAltar().hurtSum.ToString();
		myValue.text = GuildManagerment.Instance.getMyHurt().ToString();
		List<GuildAltarRank> list = GuildManagerment.Instance.filterRank();
		for (int i = 0; i < list.Count; i++) {
			items[i].names.text = list[i].playerName;
			items[i].values.text = list[i].hurtValue.ToString();
			items[i].gameObject.SetActive(true);
		}
	}

}
