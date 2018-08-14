using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 诸神战小组排行
 * @author gc
 * */
public class LastBattleRankWindow : WindowBase {

	/** 我的当前积分 */
	public UILabel myIntergalLabel;
	public LastBattleRankContent lastBattleContent;
	public GameObject btnRule;
	public GameObject rankitemPrefab;
	public GameObject rankContent;
	public GameObject closeRankRulePanelBtn;
	public GameObject descTmp;
	public UIGrid descGrid;
	public GameObject rulePanel;
	public Transform descParent;

	protected override void begin ()
	{
		base.begin ();
//		int rank = GodsWarManagerment.Instance.self.rank;
//		myIntergalLabel.text = LanguageConfigManager.Instance.getLanguage("godsWar_83",rank==-1?LanguageConfigManager.Instance.getLanguage("godsWar_84"):rank.ToString());
		RankFPort fport = FPortManager.Instance.getFPort ("RankFPort") as RankFPort;
		fport.access (RankManagerment.TYPE_LASTBATTLE,()=>{
			lastBattleContent.initContent();
			updateMyRank();
		});

		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close")
		{
			finishWindow ();
		}
		// 点击查看奖励规则//
		else if(gameObj == btnRule)
		{
			rankContent.SetActive(false);
			if(descParent.childCount <= 1)
			{
				initRankRulePanel();
			}
			rulePanel.SetActive(true);
		}
		// 关闭奖励规则界面//
		else if(gameObj == closeRankRulePanelBtn)
		{
			rankContent.SetActive(true);
			rulePanel.SetActive(false);
		}
	}

	void updateMyRank()
	{
		//LastBattle_MyRank
		// 未上榜//
		if(LastBattleManagement.Instance.myRank == -1)
		{
			myIntergalLabel.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_MyRank"),LanguageConfigManager.Instance.getLanguage("LastBattle_NotOnRank"));
		}
		else
		{
			myIntergalLabel.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_MyRank"),LastBattleManagement.Instance.myRank.ToString());
		}
	}

	void initRankRulePanel()
	{
		List<LastBattleRankPrizeDesc> descs = LastBattleRankPrizeDescConfigManager.Instance.descList;
		GameObject descObj;
		if(descs != null && descs.Count > 0)
		{
			for(int i=0;i<descs.Count;i++)
			{
				descObj = GameObject.Instantiate(descTmp) as GameObject;
				descObj.transform.parent = descParent;
				descObj.transform.localPosition = Vector3.zero;
				descObj.transform.localScale = Vector3.one;
				descObj.transform.FindChild("desc1").gameObject.GetComponent<UILabel>().text = descs[i].rankName;
				descObj.transform.FindChild("desc2").gameObject.GetComponent<UILabel>().text = descs[i].prizeDesc;
				descObj.SetActive(true);
			}
			descGrid.repositionNow = true;
		}
	}

}