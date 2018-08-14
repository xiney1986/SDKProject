using UnityEngine;
using System.Collections;

/**
 * 公会内排行窗口
 * @author 汤琦
 * */
public class GuildInsideRankWindow : WindowBase {
	public TapContentBase tapBase;
	public GuildInConRankContent conContent;
	public GuildInDoRankContent doContent;
	public GuildInRaskRankContent rastContent;
	private int index = int.MaxValue;
	public GameObject buttonBuild;

	protected override void begin () {
		base.begin ();
		tapBase.changeTapPage (tapBase.tapButtonList [0]);
		index = 2;
		doContent.initContent ();
		MaskWindow.UnlockUI ();
	}
	public void initWindow () {
		if (index != 2) {
			index = 2;
			doContent.initContent ();
		}
	}
	public override void tapButtonEventBase (GameObject gameObj, bool enable) {
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "buttonContri" && enabled == true) {
			if (index != 1) {
				index = 1;
				conContent.initContent ();
			}
		}
		else if (gameObj.name == "buttonDonate" && enabled == true) {
			if (index != 2) {
				index = 2;
				doContent.initContent ();
			}
		}
		else if (gameObj.name == "buttonRask" && enabled == true) {
			if (index != 3) {
				index = 3;
				rastContent.initContent ();
			}
		}
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
}