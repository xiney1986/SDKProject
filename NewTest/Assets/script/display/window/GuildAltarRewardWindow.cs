using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildAltarRewardWindow : WindowBase {
	/** 左箭头 */
	public UISprite leftArrow;
	/** 右箭头 */
	public UISprite rightArrow; 
	/** 条件 */
	public UILabel conditon;
	/** 活跃度增加值 */
	public UILabel guildActiveAdd;
	/** BOSS奖励列表 */
	private List<GuildBossPrizeSample> prizes = new List<GuildBossPrizeSample>();
	public GuildBossPrizeContent altarPrizeContent;

	protected override void begin ()
	{
		base.begin ();
		initWindow ();
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 初始化窗口
	/// </summary>
	public void initWindow(){
		prizes = GuildPrizeSampleManager.Instance.getPrizes ();
		updateUI (prizes [0]);
		altarPrizeContent.initInfo ();
		altarPrizeContent.callbackUpdateEach = altarPrizeContent.updateButton;
		altarPrizeContent.onCenterItem =updateActive;	
		altarPrizeContent.init ();
	}

	/// <summary>
	/// 刷新描述
	/// </summary>
	/// <param name="sample">Sample.</param>
	public void updateUI(GuildBossPrizeSample sample){
		conditon.text = LanguageConfigManager.Instance.getLanguage ("guildAltar14", sample.hurt.ToString());
		guildActiveAdd.text ="+" + sample.liveness.ToString();

	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "ButtonSure") {
			this.finishWindow();
		}
	}
	public  void updateActive (GameObject obj)
	{
		int pageNUm=StringKit.toInt(obj.name);
		
		if(pageNUm == 1)
		{
			leftArrow.gameObject.SetActive(false);
			rightArrow.gameObject.SetActive(true);
		}
		else if(pageNUm == prizes.Count)
		{
			leftArrow.gameObject.SetActive(true);
			rightArrow.gameObject.SetActive(false);
		}
		else
		{
			leftArrow.gameObject.SetActive(true);
			rightArrow.gameObject.SetActive(true);
		}
		if (pageNUm <= prizes.Count) {
			updateUI (prizes [pageNUm-1]);
		}
	}
}
