using UnityEngine;
using System.Collections;

/// <summary>
/// 成长奖励单元
/// </summary>
public class GrowupAwardItem : MonoBehaviour {

	/* fields */
	public GoodsView goodsview;
	public UILabel title;
	public ButtonBase button;
	//父窗口
	private WindowBase win;

	/**method */
	/// <summary>
	/// 更新奖励条目
	/// </summary>
	/// <param name="tl">排行奖励</param>
	/// <param name="win">父窗口</param>
	public void updateAwardItem (GrowupAwardSample tl, int index, WindowBase win) {
		this.win = win;
		updateRank (tl);
		initButtonInfo (tl, index);
	}
	/// <summary>
	/// 更新排行条目
	/// </summary>
	/// <param name="tl">Tl.</param>
	private void updateRank (GrowupAwardSample tl) {
		title.text = tl.needLevel.ToString ();
		goodsview.init (tl.prize);
		goodsview.fatherWindow = win;
	}
	/// <summary>
	/// 初始化button信息
	/// </summary>
	/// <param name="tl">Tl.</param>
	public void initButtonInfo (GrowupAwardSample tl, int index) {
		button.setFatherWindow (win);
		button.name = "AwardButton";
		button.exFields = new Hashtable ();
		button.exFields.Add ("index", index);
		button.exFields.Add ("needLevel", tl.needLevel);
		if (StringKit.toInt (tl.needLevel) <= UserManager.Instance.self.getUserLevel ()) {
			button.disableButton (false);
		}
		else {
			button.disableButton (true);
		}
	}
}