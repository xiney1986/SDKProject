using UnityEngine;
using System.Collections;

/// <summary>
/// 天梯排名奖励窗口
/// </summary>
public class LaddersRankRewardWindow : WindowBase
{
	public GameObject goodsViewPrefab;
	public GameObject root_award;
	public ButtonBase btn_receive;
	public UISprite sprite_titleBg;
	public UILabel label_titleName;
	[HideInInspector]
	public CallBack
		closeCallback;
	private bool needRefresh = false;

	protected override void begin ()
	{
	    UiManager.Instance.laddersRankRewardWindow = this;
		M_updateView ();
		needRefresh = false;
	}
	/// <summary>
	/// 更新视图
	/// </summary>
	private void M_updateView ()
	{
		LaddersAwardInfo award = LaddersManagement.Instance.Award;
		btn_receive.disableButton (!award.canReceive);

		LaddersAwardSample sample = LaddersConfigManager.Instance.config_Award.M_getAward (award.rank);
		if (sample != null)
		{
			M_updateAwardSample (sample.samples);
		}
		LaddersTitleSample titlesample = LaddersManagement.Instance.M_getCurrentPlayerTitle();
		LaddersMedalSample medalsample = LaddersConfigManager.Instance.config_Medal.M_getMedal(award.rank);
		updateTitle (titlesample,medalsample);
	}
	/// <summary>
	/// 更新奖励视图
	/// </summary>
	/// <param name="samples">Samples.</param>
	private void M_updateAwardSample (PrizeSample[] samples)
	{
		UIUtils.M_removeAllChildren (root_award);
		for (int i = 0,length=samples.Length; i < length; i++) {
			GameObject a = NGUITools.AddChild (root_award, goodsViewPrefab);
			a.name = StringKit.intToFixString (i + 1);
			GoodsView goodsButton = a.GetComponent<GoodsView> ();
			goodsButton.fatherWindow = this;
			goodsButton.onClickCallback = goodsButton.DefaultClickEvent;
			goodsButton.init (samples [i]);
		}
		root_award.GetComponent<UIGrid> ().Reposition ();
	}
	/// <summary>
	/// 更新称号
	/// </summary>
	/// <param name="currentTitle">Current title.</param>
	/// <param name="currentMedal">Current medal.</param>
	private void updateTitle (LaddersTitleSample currentTitle,LaddersMedalSample currentMedal)
	{
		label_titleName.text = currentTitle.name;
		if (currentMedal != null) {
			if(currentMedal.minRank > 500)
				sprite_titleBg.spriteName = "medal_0";
			else
				sprite_titleBg.spriteName = "medal_" + Mathf.Min (currentMedal.index + 1, 5);
		} else {
			sprite_titleBg.spriteName = "medal_0";
		}
	}
	/// <summary>
	/// 按钮点击事件
	/// </summary>
	/// <param name="gameObj">Game object.</param>
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		string btnName = gameObj.name;
		switch (btnName) {
		case "btn_close":
			if (needRefresh) {
				LaddersWindow baseWindow = UiManager.Instance.getWindow<LaddersWindow> ();
				if (baseWindow != null) {
					baseWindow.M_updateUserInfo ();
				}
				if(closeCallback!=null)
				{
					closeCallback();
					closeCallback=null;
				}
			}
			finishWindow ();
			break;
		case "btn_receive":
			LaddersAwardFport fport = FPortManager.Instance.getFPort<LaddersAwardFport> ();
			fport.apply ((msg) => {
				if (msg == "ok") {
					needRefresh = true;
					TextTipWindow.Show (Language ("s0120"));
					btn_receive.disableButton (true);
					LaddersManagement.Instance.Award.M_clear ();
					finishWindow ();

					LaddersWindow ladderWin=UiManager.Instance.getWindow<LaddersWindow>();
					if(ladderWin!=null)
					{
						ladderWin.M_updateView();
					}
				}
			});	
			break;
		}
	}
    public override void DoDisable() {
        base.DoDisable();
        UiManager.Instance.laddersRankRewardWindow = null;
    }

}

