using UnityEngine;
using System.Collections;

public class LadderHeItem : MonoBehaviour {

	public UILabel rangeLevel;
	public UILabel prizeLevel;
	private LadderHegeMoney ladderhe;
	public UILabel rmbLabel;

	public void initData(string level,LadderHegeMoney ladder)
	{
		this.ladderhe = ladder;
		if (ladderhe.startLevel != 0) {
			rangeLevel.text = LanguageConfigManager.Instance.getLanguage ("ladderruleprize2",ladderhe.startLevel.ToString(),ladderhe.endLevel.ToString());
		} else {
			rangeLevel.text = LanguageConfigManager.Instance.getLanguage ("laddderruleprize3",level);
		}
		prizeLevel.text = LanguageConfigManager.Instance.getLanguage("ladderpoints")+ this.ladderhe.levelPoint.ToString();
		rmbLabel.text = ladder.rmbPrize.ToString ();
	}

}
