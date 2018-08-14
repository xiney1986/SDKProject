using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 已有公会主界面 公会节点显示
 * @author 汤琦
 * */
public class GuildMainGuildContent : MonoBase {

	/* gameobj fields */
	/** 公会实体 */
	private Guild guild;
	/** 公会名 */
	public UILabel guildName;
	/** 公会等级 */
	public UILabel guildGrade;
	/** 公会成员数 */
	public UILabel memberNum;
	/** 活跃度 */
	public UILabel liveness;
	/** 我的职位 */
	public UILabel job;
	/** 我的贡献 */
	public UILabel contribution;
	/** 公告 */
	public UILabel notice;
	/** 公会消息 */
	public GameObject msgItem;

	/* fields */
	/** 2条公会消息列表 */
	public UITextList textList;
	/** 15条公会消息列表 */
	public UITextList multiTextList;

	/* methods */
	public void initInfo () {
		guild = GuildManagerment.Instance.getGuild ();
		InitData ();
		updateInfo ();
	}
	private void InitData () {
		if (textList == null || multiTextList == null)
			return;
		textList.Clear ();
		multiTextList.Clear ();
		List<GuildMsg> list = guild.msgs;
		if (list != null) {
			for (int i = 0; i < list.Count; i++) {
				updateMsg (list [i]);
			}
		}
	}
	private void updateMsg (GuildMsg msg) {
		if (textList == null || multiTextList == null)
			return;
		string content = msg.content;
		string[] strs = content.Split (' ');
		string str = "[FFB84D]" + strs [0] + "_" + strs [1] + "　"+ "[FFFFFF]" + strs [2];
		textList.Add (str);
		multiTextList.Add (str);
	}
	public GameObject createView (object o, GameObject view) {
		GuildMsg msg = o as GuildMsg;
		GameObject obj = NGUITools.AddChild (gameObject, msgItem);
		GuildMsgItem item = obj.GetComponent<GuildMsgItem> ();
		item.initUI (msg);
		return obj;
	}
	public void updateInfo () {
		if (guild != null) {
			guildName.text = guild.name;
			guildGrade.text= "Lv."+guild.level;
			memberNum.text=guild.membership + "/" + guild.membershipMax;
			liveness.text = guild.livenessing + "/" + guild.livenessed;
			job.text = GuildJobType.getJobName (guild.job);
			contribution.text = guild.contributioning + "/" + guild.contributioned;
			if (guild.notice == "") {
				if (guild.job == GuildJobType.JOB_PRESIDENT || guild.job == GuildJobType.JOB_VICE_PRESIDENT) {
					notice.text = "[f0a0a0]" + LanguageConfigManager.Instance.getLanguage ("Guild_93");
				}
				else
					notice.text = "";
			}
			else {
				notice.text = guild.notice;
			}
		}
	}
}