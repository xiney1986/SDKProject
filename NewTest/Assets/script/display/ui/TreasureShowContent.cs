using UnityEngine;
using System.Collections;

/// <summary>
/// 奖励预览容器
/// </summary>
public class TreasureShowContent : dynamicContent {
	/**filed */
	/**物品预制体 */
	public GoodsView goodsPerfab;
	/**奖品数组 */
	private PrizeSample[] prizes;
	/**父类窗口 */
	private TreasureChestWindow winn;
	/// <summary>
	/// 开始更新星魂仓库的每一条数据
	/// </summary>
	/// <param name="item">Item.</param>
	/// <param name="index">Index.</param>
	public override void updateItem (GameObject item, int index) {
		PrizeSample prizeSample = prizes [index];
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = winn;
		button.init (prizeSample);
	}
	/***/
	public override void initButton(int i) {
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject,goodsPerfab.gameObject);
		}
		GoodsView button = nodeList [i].GetComponent<GoodsView> ();
		button.fatherWindow = winn;
		button.init (prizes [i]);
	}
	public void reload(PrizeSample[] prize,TreasureChestWindow win){
		this.prizes=prize;
		this.winn=win;
		if(prizes!=null){
			base.reLoad(prizes.Length);
		}

	}
}
