using UnityEngine;
using System.Collections;

/// <summary>
/// 限时活动奖励条目
/// </summary>
public class LuckActivityAwardItem : MonoBehaviour {

	public GameObject content;
	public UILabel lblCondition;
	public GameObject firstAwardLogo;
	public GameObject goodsViewPrefab;
	public UILabel rinkNum;
	public GameObject complated;
	public GameObject uncomplate;
	/** 父窗口 */
	private WindowBase winn;
	/** 积分标签 */
	public  UILabel sourceNum;
	/** 积分 */
	private int mySource;

	/// <summary>
	/// 更新奖励条目
	/// </summary>
	/// <param name="tl">排行奖励</param>
	/// <param name="win">父窗口</param>
	public void updateAwardItem(RankAward tl,WindowBase win) {
		winn=win;
		updateRank(tl);
	}
	/// <summary>
	/// 更新奖励条目
	/// </summary>
	/// <param name="tl">排行奖励</param>
	/// <param name="win">父窗口</param>
	/// <param name="source">积分</param>
	public void updateAwardItem(RankAward tl,WindowBase win,int source) {
		winn=win;
		mySource=source;
		updateSource(tl);
	}
	/** 更新积分条目 */
	private void updateSource(RankAward tl) {
		resetSprite();
		if(!tl.isAward){
			uncomplate.SetActive(true);
		}else{
			complated.SetActive(true);
		}
		sourceNum.text=tl.needSource.ToString();
		Utils.DestoryChilds(content.gameObject);
		for(int i = 0; i < tl.prizes.Length; i++)
		{
			PrizeSample ps = tl.prizes[i];
			GameObject obj = NGUITools.AddChild(content.gameObject,goodsViewPrefab);
			GoodsView sc = obj.GetComponent<GoodsView>();
			sc.init(ps);
			sc.fatherWindow = winn;
			content.GetComponent<UIGrid>().repositionNow = true;
		}
	}
	private void resetSprite(){
		uncomplate.SetActive(false);
		complated.SetActive(false);
	}
	/// <summary>
	/// 更新排行条目
	/// </summary>
	/// <param name="tl">Tl.</param>
	private void updateRank(RankAward tl) {
		if(tl.rinkNum==1) {
			rinkNum.gameObject.SetActive(false);
			firstAwardLogo.gameObject.SetActive(true);
		} else{
			rinkNum.gameObject.SetActive(true);
			firstAwardLogo.gameObject.SetActive(false);
			rinkNum.text=tl.dec;
		}
		Utils.DestoryChilds(content.gameObject);
		for(int i = 0; i < tl.prizes.Length; i++) {
			PrizeSample ps = tl.prizes[i];
			GameObject obj = NGUITools.AddChild(content.gameObject,goodsViewPrefab);
			GoodsView sc = obj.GetComponent<GoodsView>();
			sc.init(ps);
			sc.fatherWindow = winn;
			content.GetComponent<UIGrid>().repositionNow = true;
		}
	}
}
