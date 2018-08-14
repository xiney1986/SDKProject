using UnityEngine;
using System.Collections;

/**
 * 公会捐献窗口
 * @author 汤琦
 * */
public class GuildDonateWindow : WindowBase {

	public UILabel[] labels;
	public GameObject[] buttons;
	private CallBack callback;
	private int consum = 0;

	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	void Start () {
		UpdateInfo ();
	}
	public void setCallBack (CallBack callback) {
		this.callback = callback;
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		GuildDonationFPort fport = FPortManager.Instance.getFPort ("GuildDonationFPort") as GuildDonationFPort;
		if (gameObj.name == "buttonMoney") {
			if (UserManager.Instance.self.getMoney () < consum) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("guild_851"));
			}
			else {
				fport.access (GuildManagerment.DONATIONMONEY, () => {
					GuildManagerment.Instance.getGuild ().todayDonateTimes++;
					UpdateInfo ();
				});
			}
		}
		else if (gameObj.name == "buttonOne") {
			fport.access (GuildManagerment.DONATIONGONE, Refresh);
		}
		else if (gameObj.name == "buttonTwo") {
			fport.access (GuildManagerment.DONATIONGTWO, Refresh);
		}
		else if (gameObj.name == "buttonThree") {
			fport.access (GuildManagerment.DONATIONGTHREE, Refresh);
		}
		else if (gameObj.name == "close") {
			finishWindow ();
		}
	}
	void Refresh () {
		Guild guild = GuildManagerment.Instance.getGuild ();
		guild.firstAward++;
		UpdateInfo();

	}
	void UpdateInfo () {
		Guild guild = GuildManagerment.Instance.getGuild ();
		GuildDonateConfigManager.GuildDonateItem[] items = GuildDonateConfigManager.Instance.getGuildDonateItems (guild.level);
		for (int i = 0; i < items.Length; i++) {
			GuildDonateConfigManager.GuildDonateItem item = items [i];
			if (i == 0) {
				string str = LanguageConfigManager.Instance.getLanguage ("s0398");
				consum = item.consume;
				str = string.Format (str, item.consume, item.maxTimesOneDay, '\n', item.activity);
				labels [i].text = str;
				GameObject obj;
				if (guild.todayDonateTimes >= item.maxTimesOneDay) {
					obj = buttons [i];
					obj.GetComponent<ButtonBase> ().disableButton (true);
				}
			}
			else {
				string str="";
				if(guild.firstAward==0){
					str = LanguageConfigManager.Instance.getLanguage ("s0397");
				}else{
					str = LanguageConfigManager.Instance.getLanguage ("s0397l01");
				}
				str = string.Format (str, item.consume, item.dedication, '\n', item.activity);
				labels [i].text = str;
			}
		}
	}
	public override void OnNetResume () {
		base.OnNetResume ();
		UpdateInfo ();
	}
}