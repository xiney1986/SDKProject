using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarIntegralRankAwardWindow : WindowBase {
    
	/** 我的当前积分 */
	public UILabel myIntergalLabel;
	//**奖励item*/
	public GameObject itemPrefab;
	public GameObject goodsViewPrefab;
	public GodsWarIntegralRankAwardContent content;
	
	protected override void begin ()
	{
		base.begin ();
	}
	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initUI( ){
		content.init(this);
		content.fatherWindow = this;
		int rank = GodsWarManagerment.Instance.self.rank;
		myIntergalLabel.text = LanguageConfigManager.Instance.getLanguage("godsWar_83",rank==-1?LanguageConfigManager.Instance.getLanguage("godsWar_84"):rank.ToString());
		MaskWindow.UnlockUI ();
	}

    public override void buttonEventBase(GameObject gameObj)
    {
		base.buttonEventBase (gameObj);
        if (gameObj.name == "close")
        {
            finishWindow();
        } 
    }



}
