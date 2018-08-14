using UnityEngine;
using System.Collections;

public class LadderRuleWindow : WindowBase {

	public UILabel myRankLable;
	public UILabel socreLabel;

	public GameObject ruleItem;

	public ContentRuleLadder content1;

	public LadderHegemoneyNotice ladderNotice;

	LadderHegeMoney []  laddermoneys;


	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();

	}

	public void initWin()
	{
		int sid = 0;
		if (fatherWindow is NoticeWindow)
		{
			ladderNotice = (fatherWindow as NoticeWindow).show.GetComponent<NoticeLadderHegeMoneyContent> ().notice;
			sid = ladderNotice.sid;
		}

		laddermoneys = LadderAwardSampleManager.Instance.getLadderHegeMoneys (sid);
		updateUI ();
	}


	private int myPrizePoint()
	{
		int rank = LadderHegeMoneyManager.Instance.myRank;
		int end = 0;

		foreach (LadderHegeMoney le in laddermoneys) {

			if (le.startLevel != 0)
			{
				if (rank >= le.startLevel && rank <= le.endLevel)
					end = le.levelPoint;
			} else {
				if (le.rangeLevel == rank.ToString())
					end =  le.levelPoint;
			}
		}

		return end;
	}


	public void updateUI()
	{

		int scource = myPrizePoint ();


		myRankLable.text = LadderHegeMoneyManager.Instance.myRank.ToString();

		if (scource > 0) {
			socreLabel .text = scource.ToString();
		} else {
			socreLabel.text = LanguageConfigManager.Instance.getLanguage("ladderRulePrize1");
		}
		content1.fatherWindow = this;
		content1.reLoad(ladderNotice.sid);
	}



	public override void buttonEventBase(GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		switch (gameObj.name) 
		{
		 case "close":
			finishWindow ();
			break;

		}
	}
}
