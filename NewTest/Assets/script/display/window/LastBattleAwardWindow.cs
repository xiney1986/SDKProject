using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LastBattleAwardWindow : WindowBase {

	/* gameojb fields */
	/** 奖励容器 */
	public DelegatedynamicContent awardContent;
	//  boss战最后一击奖励容器 //
	public DelegatedynamicContent finalKillAwardContent;
	/** goods预制体 */
	public GameObject goodsViewPrefab;
	/** 展示的奖品 */
	private PrizeSample[] prizes;
	//  展示boss战最后一击奖励//
	private PrizeSample[] finalKillPrizes;
	/** */
	private int[] missionList;
    private CallBack callBack;
    public UILabel score;
	private List<Award> base_awList = new List<Award>();
	private List<Award> finalKill_awList = new List<Award>();
	public GameObject finalKillLabel;
	/* methods */
	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="prizes">奖品列表</param>
	public void init(Award[] aw) {
		base_awList.Clear();
		finalKill_awList.Clear();
		for(int i=0;i<aw.Length;i++)
		{
			// 末日决战小怪奖励或boss战基础奖励//
			if(aw[i].type == AwardManagerment.LASTBATTLE_AWARD || aw[i].type == AwardManagerment.LASTBATTLE_BASE_AWARD)
			{
				base_awList.Add(aw[i]);
			}
			// 末日决战boss最后一击奖励//
			if(aw[i].type == AwardManagerment.LASTBATTLE_FINALKILL_AWARD)
			{
				finalKill_awList.Add(aw[i]);
			}
		}
		this.prizes = AllAwardViewManagerment.Instance.exchangeAwards(base_awList.ToArray());
		this.prizes = AllAwardViewManagerment.Instance.contrastToArray (this.prizes);
		this.finalKillPrizes = AllAwardViewManagerment.Instance.exchangeAwards(finalKill_awList.ToArray());
		this.finalKillPrizes = AllAwardViewManagerment.Instance.contrastToArray (this.finalKillPrizes);
		UpdateUI();
	}

    public void init(Award[] aw,CallBack callback) {
		base_awList.Clear();
		finalKill_awList.Clear();
		for(int i=0;i<aw.Length;i++)
		{
			// 末日决战小怪奖励或boss战基础奖励//
			if(aw[i].type == AwardManagerment.LASTBATTLE_AWARD || aw[i].type == AwardManagerment.LASTBATTLE_BASE_AWARD)
			{
				base_awList.Add(aw[i]);
			}
			// 末日决战boss最后一击奖励//
			if(aw[i].type == AwardManagerment.LASTBATTLE_FINALKILL_AWARD)
			{
				finalKill_awList.Add(aw[i]);
			}
		}
		this.prizes = AllAwardViewManagerment.Instance.exchangeAwards(base_awList.ToArray());
		this.prizes = AllAwardViewManagerment.Instance.contrastToArray (this.prizes);
		this.finalKillPrizes = AllAwardViewManagerment.Instance.exchangeAwards(finalKill_awList.ToArray());
		this.finalKillPrizes = AllAwardViewManagerment.Instance.contrastToArray (this.finalKillPrizes);
        this.callBack = callback;
        UpdateUI();
    }
	/** begin */
	protected override void begin () {
		base.begin ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume () {
		base.OnNetResume ();
		ScreenManager.Instance.loadScreen(1, null, GameManager.Instance.outLastBattleWindow);
		BattleManager.battleData.isLastBattleBossBattle = false;
		BattleManager.battleData.isLastBattle = false;
	}
	/** 更新UI */
	public void UpdateUI() {
		if (!isAwakeformHide) {
			// 展示基础奖励//
			if (prizes != null && prizes.Length > 0) {
				awardContent.reLoad (prizes.Length);
				awardContent.SetUpdateItemCallback (onUpdateItem);
				awardContent.SetinitCallback (initItem);
			}
			// 展示最后一击奖励//
			if(finalKillPrizes != null && finalKillPrizes.Length > 0)
			{
				//finalKillAwardContent.gameObject.SetActive(true);
				finalKillLabel.SetActive(true);
				finalKillAwardContent.reLoad (finalKillPrizes.Length);
				finalKillAwardContent.SetUpdateItemCallback (onUpdateItemForFinalKill);
				finalKillAwardContent.SetinitCallback (initItemForFinalKill);
			}
			else 
			{
				finalKillLabel.SetActive(false);
			}
		}
		score.text = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_RankScoreAdd"),LastBattleManagement.Instance.battleScore.ToString());
	}
	/** 更新条目 */
	GameObject onUpdateItem (GameObject item, int i) {
		PrizeSample prizeSample = prizes [i];
		if (item == null)  {
			item = NGUITools.AddChild (awardContent.gameObject, goodsViewPrefab);
		}
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	GameObject onUpdateItemForFinalKill (GameObject item, int i) {
		PrizeSample prizeSample = finalKillPrizes [i];
		if (item == null)  {
			item = NGUITools.AddChild (finalKillAwardContent.gameObject, goodsViewPrefab);
		}
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	/** 初始化条目 */
	GameObject initItem (int i) {
		PrizeSample prizeSample = prizes [i];
		GameObject item = NGUITools.AddChild (awardContent.gameObject, goodsViewPrefab);
		item.transform.localScale = new Vector3(0.9f,0.9f,1f);
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	GameObject initItemForFinalKill (int i) {
		PrizeSample prizeSample = finalKillPrizes [i];
		GameObject item = NGUITools.AddChild (finalKillAwardContent.gameObject, goodsViewPrefab);
		item.transform.localScale = new Vector3(0.9f,0.9f,1f);
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	/** 点击事件  */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
            if (UiManager.Instance.getWindow<LastBattleWindow>() != null) {
                if (callBack != null) {
                    callBack();
                    callBack = null;
                }
                finishWindow();
            }
		}
	}


}