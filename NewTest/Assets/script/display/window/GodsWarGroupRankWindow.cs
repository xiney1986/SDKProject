using UnityEngine;
using System.Collections;

/**
 * 诸神战小组排行
 * @author gc
 * */
public class GodsWarGroupRankWindow : WindowBase {

	/** 我的当前积分 */
	public UILabel myIntergalLabel;
	public GodsWarGroupRankContent content;

	public GameObject rankitemPrefab;


	protected override void begin () {
		base.begin ();
		int rank = GodsWarManagerment.Instance.self.rank;
		myIntergalLabel.text = LanguageConfigManager.Instance.getLanguage("godsWar_83",rank==-1?LanguageConfigManager.Instance.getLanguage("godsWar_84"):rank.ToString());
		content.initContent();
		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
}