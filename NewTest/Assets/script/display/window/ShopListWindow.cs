using UnityEngine;
using System.Collections;

/// <summary>
/// 商城列表主界面
/// </summary>
public class ShopListWindow : WindowBase {
	/**fileds */
	/**列表容器 */
	public ContentShopList content;

	/*method */
	protected override void begin ()
	{
		base.begin ();
		updateList ();
		if (GuideManager.Instance.isEqualStep (132002000)) {
			GuideManager.Instance.doGuide ();
		}
		MaskWindow.UnlockUI ();
	}
	
	
	protected override void DoEnable ()
	{
		base.DoEnable ();
	}
	public void updateList ()
	{
		content.reLoad ();
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		} 
	}

}
