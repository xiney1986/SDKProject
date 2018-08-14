using UnityEngine;
using System.Collections;

public class GuildAreaItem : ButtonBase
{
	/** 名字 */
	public UILabel name;
	/** 战争点 */
	public UILabel warNum;
	/** 排名 */
	public UILabel rank;
	/** 等级 */
	public UILabel judge;
	/** 背景 */
	public UISprite build;
	/** 小人的攻击位置 */
	public Transform[] AttackPoint;
	/** 公会预览信息 */
	private GuildAreaPreInfo data;
	public UILabel timeLabel;
    private float selfCurBol;
    private float selfMaxBol;

	private Timer timer;
	private void startTimer ()
	{
		if (timer == null)
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (updateTime);
		timer.start ();
	}

	
	private void updateTime(){
		if (data.getBackTime() <= 0) {
			timer.stop ();
			timer = null;
			timeLabel.gameObject.SetActive(false);
			timeLabel.text = "";
			(fatherWindow as GuildFightMainWindow).getFightInfo();
		} else {
			timeLabel.gameObject.SetActive(true);
			timeLabel.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_44",  data.getBackTimeString ());
		}
	}

    public void initUI(GuildAreaPreInfo data, float curBolld, float maxBolld)
	{
		this.data = data;
		if (data.uid != GuildManagerment.Instance.getGuild().uid) {
			name.color =Color.red;
		}
        selfCurBol = curBolld;
        selfMaxBol = maxBolld;
		name.text = data.name + "." + data.server;
		warNum.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_16") + data.warNum.ToString ();
		rank.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_18", data.rank.ToString());
		judge.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_19")+ data.getJudeString();
		/** 被击破 */
		if (data.state == 3) {
			build.spriteName = "broke";
			startTimer ();
		} else {
			timeLabel.gameObject.SetActive(false);
			build.spriteName = "hall_fight_" + StringKit.toInt(this.gameObject.name);
		}
	}
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (data == null ) {
			MaskWindow.UnlockUI();
			return;
		}
        if (GuildManagerment.Instance.guildFightInfo.isDead && data.uid != GuildManagerment.Instance.getGuild().uid)
        {
            UiManager.Instance.createMessageLintWindow(Language("GuildArea_67"));
            return;
        }
		/** 入会不满1天 */
		if (!GuildManagerment.Instance.isCanJoinGuildFight ()) {
			UiManager.Instance.createMessageLintWindow(Language("GuildArea_41"));
			return;
		}

		if(data.state == 3){
			UiManager.Instance.createMessageLintWindow(Language("GuildArea_45"));
			return ;
		}
	
		UiManager.Instance.openWindow<GuildAreaWindow> ((win) => {
			win.updateUI (data.uid,data.server,data.name,selfCurBol,selfMaxBol);
		});
	}


	public override void DoDisable ()
	{
		base.DoDisable ();
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}
}

