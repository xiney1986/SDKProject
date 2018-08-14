using UnityEngine;
using System.Collections;

public class ButtonGoddessItem : ButtonBase {
	/*filed */
	/**女神头像 */
	public UITexture goddessHead;
	/**女神名字 */
	public UILabel goddessName;
	/**女神等级 */
	public UILabel goodessLv;
	private GoddessUnitWindow winn;
	/**选择的女神 */
	private Card selectBe;
	/**女神的UID或Sid */
	/*method */
	/// <summary>
	/// 更新每条的详细信息
	/// </summary>
	/// <param name="be">Be.</param>
	public void updateItem(Card be, GoddessUnitWindow win){
		winn=win;
		selectBe=be;
		if(be!=null){
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + be.getImageID () + "_head", goddessHead);
			goddessName.text=be.getName();
			goodessLv.text="Lv:"+be.getLevel().ToString();
		}

	}
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		winn.be=selectBe;
		winn.updateUI();
		fatherWindow.finishWindow();
	}

}
