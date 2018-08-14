using UnityEngine;
using System.Collections;

/**
 * 诸神战决赛奖励排行
 * @author gc
 * */
public class GodsWarFinalRankWindow : WindowBase {
	public TapContentBase tapBase;
	public GodsWarFinalRankAwardContent bronzeContent;
	public GodsWarFinalRankAwardContent silverContent;
	public GodsWarFinalRankAwardContent goldContent;
	public UILabel myRank;
	private int index = int.MaxValue;

	public GameObject rankitemPrefab;
	public GameObject goodsViewPrefab;
	private int big_id;

	protected override void begin () {
		base.begin ();
	    UiManager.Instance.godsWarFinalRankWindow = this;
		//tapBase.changeTapPage (tapBase.tapButtonList [1]);
		initContent();
		myRank.text = LanguageConfigManager.Instance.getLanguage("godsWar_116",GodsWarManagerment.Instance.getMyRankInfo()+"("+GodsWarManagerment.Instance.getMyGroupInfo()+")");
		MaskWindow.UnlockUI ();
	}
	public void initWindow (int big_id) {
		this.big_id = big_id;
	}
	public override void tapButtonEventBase (GameObject gameObj, bool enable) {
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "buttonBronze" && enable == true) {
			if (index != 1) {
				index = 1;
				bronzeContent.init (GodsWarManagerment.TYPE_BRONZE,this);
			}
		}
		else if (gameObj.name == "buttonSilver" && enable == true) {
			if (index != 2) {
				index = 2;
				silverContent.init (GodsWarManagerment.TYPE_SILVER,this);
			}
		}
		else if (gameObj.name == "buttonGold" && enable == true) {
			if (index != 3) {
				index = 3;
				goldContent.init (GodsWarManagerment.TYPE_GOLD,this);
			}
		}
	}

	public void initContent()
	{
		switch (GodsWarManagerment.Instance.getType()) {
		case 110:
			tapBase.changeTapPage (tapBase.tapButtonList [0]);
			//bronzeContent.initContent (GodsWarManagerment.Instance.getType(),this);
			break;
		case 111:
			tapBase.changeTapPage (tapBase.tapButtonList [1]);
			//silverContent.initContent (GodsWarManagerment.Instance.getType(),this);
			break;
		case 112:
			tapBase.changeTapPage (tapBase.tapButtonList [2]);
			//goldContent.initContent (GodsWarManagerment.Instance.getType(),this);
			break;
		default:
			break;
		}
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
    void OnDestroy()
    {
        UiManager.Instance.godsWarFinalRankWindow = null;
    }
}