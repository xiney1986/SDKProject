using UnityEngine;
using System.Collections;

public class PvpPrizeModule : MonoBehaviour
{
	/** 连胜名 */
	public UILabel winStreakName;
	/** 奖励模版 */
	public PvpPrizeSample sample;
	/** 父窗口 */
	private PvpPrizeWindow win;
	/** 容器 */
	public UIGrid goodsOffset;
	/** 奖励显示预制 */
	public GameObject goodsView;
	
	public void initialize (PvpPrizeSample _sample, WindowBase _win)
	{
		win = _win as PvpPrizeWindow;
		updateSample (_sample);
	}
	
	public void updateSample (PvpPrizeSample _sample)
	{
		if (_sample == null) {
			return;
		} else {
			sample = _sample;
			winStreakName.text = sample.des.ToString ();
			creatPrize ();
		}
	}
	
	private void creatPrize ()
	{
		UIUtils.M_removeAllChildren (goodsOffset.gameObject);
		foreach (PrizeSample p in sample.item) {
			GameObject go = NGUITools.AddChild (goodsOffset.gameObject, goodsView);
			GoodsView view = go.GetComponent<GoodsView> ();
			view.setFatherWindow (win);
			view.init (p);
			goodsOffset.Reposition ();
		}
	}
	

}
