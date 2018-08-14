using UnityEngine;
using System.Collections;

/// <summary>
/// 公会公告编辑器
/// </summary>
public class GuildNoticeEditWindow : WindowBase {

	private const int COUNTSUM = 100;//最多文字数
	/* gameobj fields */
	public UIInput input;//输入内容
	public UILabel count;//输入文字数

	/* methods */
	/** 更新输入 */
	public void updateInput () {
		input.value = GuildManagerment.Instance.getGuild ().notice;
		MaskWindow.UnlockUI ();
	}
	/***/
	void Update () {
		count.text = input.value.Length + "/" + COUNTSUM;
	}
	/** button点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow();
		}
		else if (gameObj.name == "buttonSubmit") {
			if (ShieldManagerment.Instance.isContainShield2 (input.value)) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0279"));
				return;
			}
			GuildNoticeAmendFPort fport = FPortManager.Instance.getFPort ("GuildNoticeAmendFPort") as GuildNoticeAmendFPort;
			fport.access (input.value, ()=>{
				if(fatherWindow is GuildMainWindow) {
					GuildMainWindow gmw=fatherWindow as GuildMainWindow;
					gmw.UpdateGuildContent();
					finishWindow();
				}
				else if(fatherWindow is GuildFightMainWindow){
					GuildFightMainWindow gmw=fatherWindow as GuildFightMainWindow;
					gmw.updateBaseInfo();
					finishWindow();
				}
			});
		}
	}
}