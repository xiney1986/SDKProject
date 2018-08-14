using UnityEngine;
using System.Collections;

/**
 * 公会成员分页项
 * @author 汤琦
 * */
public class GuildMemberItem : MonoBase
{
	public UILabel playerName;//玩家名字
	public UILabel contribution;//贡献
	public UILabel state;//状态，在线，多久登录
	public UILabel job;//职位
	public UISprite vipIco;//vip图标
	public ButtonGuildManager button;
	public UISprite point;
	public UILabel level;//等级
	private GuildMember member;
	private WindowBase win;
	public UITexture icon;
	public GuildMemberHeadButton headButton;
	private Guild myGuild;

	public void initInfo (GuildMember member, WindowBase win, Guild myGuild)
	{
		this.win = win;
		this.member = member;
		this.myGuild = myGuild;
		updateInfo ();
	}

	private void updateInfo ()
	{

		if (member == null)
			return;
		else {
			button.gameObject.SetActive (false);
			button.textLabel.text = LanguageConfigManager.Instance.getLanguage ("guildMain07"); 
			job.text = GuildJobType.getJobName (member.job);
			playerName.text = member.name;
			if (level != null) {
				level.text ="Lv "+ member.level.ToString();;
			}
			contribution.text = member.contributioning.ToString ();
			state.text = GuildManagerment.Instance.timeTransform (member.lastLogin, member.lastLogout);
			if (point != null) {
				if(member.lastLogin >= member.lastLogout)
					point.spriteName = "point_online";
				else
					point.spriteName = "point_outline";
			}
			if (headButton != null) {
				headButton.fatherWindow = win;
				headButton.initInfo (member.uid);
			}
			if (member.vipLevel > 0) {
				vipIco.spriteName = "vip" + member.vipLevel;
				vipIco.gameObject.SetActive (true);
			} else {
				vipIco.gameObject.SetActive (false);
			}
			button.fatherWindow = win;
			button.member = member;
			ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (member.icon), icon);

			if (button.fatherWindow is GuildInfoWindow || member.uid == UserManager.Instance.self.uid) {
				button.gameObject.SetActive (false);
				return;
			}

			if (member.job == GuildJobType.JOB_PRESIDENT) {
				button.gameObject.SetActive (false);
			}

			if (myGuild.job != GuildJobType.JOB_OFFICER && myGuild.job != GuildJobType.JOB_COMMON) {
				if(myGuild.job==GuildJobType.JOB_VICE_PRESIDENT&&job.text==LanguageConfigManager.Instance.getLanguage("Guild_13"))
					button.gameObject.SetActive (false);
				else
					button.gameObject.SetActive(true);
			}
			//弹劾
			if (GuildManagerment.Instance.getGuild () != null && GuildManagerment.Instance.getGuild ().uid == myGuild.uid && member.job == GuildJobType.JOB_PRESIDENT) {
				if (GuildManagerment.Instance.isImpeach ()) {
					button.gameObject.SetActive (true);
					button.textLabel.text = LanguageConfigManager.Instance.getLanguage ("Guild_38");
				}
			}
		}
	}

}
