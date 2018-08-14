using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 排行奖励容器
/// </summary>
public class RankingForActiveContent : MonoBehaviour {

	/** 类型 */
	private int openType;
	/** 容器 */
	public RankingActiveContent rankcontent;
	/** 排行榜描述 */
	public UILabel rankTextLabel;

	/** 初始化方法 */
	public void init(WindowBase win,int type){
		rankcontent.fatherWindow=win;
		openType=type;
		RankAward[] awards = SortAward ();
		rankcontent.reLoad(awards);
		updateUI();
	}
	public void updateUI() {
		updateRankText();
	}
	/** 更新排行榜文本 */
	private void updateRankText() {
		int minIntegral=0;
		RankAward rankAward=LucklyActivityAwardConfigManager.Instance.getFirstSource(openType);
		if(rankAward!=null) {
			minIntegral=rankAward.needSource;
		}
//		if(openType ==2)
		rankTextLabel.text=LanguageConfigManager.Instance.getLanguage("luckinfo",Convert.ToString(minIntegral));
	}
	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {
		MaskWindow.UnlockUI();
	}
	//得到奖励数据
	RankAward[] SortAward () {
		RankAward[] awards = LucklyActivityAwardConfigManager.Instance.getAward (openType);
		return awards;
	}
}
