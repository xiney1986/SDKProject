using UnityEngine;
using System.Collections;

public class GuildApplyItem : MonoBehaviour
{
	/** 公会等级 */
	public UILabel level; 
	/** 公会成员数量 */
	public UILabel member;
	/** 公会名字 */
	public UILabel guildName;
	/** 申请加入按钮 */
	public ButtonGuildApply button;
	/** 数据 */
	private GuildRankInfo guild;

	public void updateActive (GuildRankInfo guild)
	{
		this.guild = guild; 
		level.text = "Lv." + guild.level;
		member.text = guild.membership + "/" + guild.membershipMax;
		guildName.text = guild.name;
		button.initInfo(guild);

	}
}
