using UnityEngine;
using System.Collections;

/// <summary>
/// 特惠商店容器
/// </summary>
public class ContentTeHuiShop : MonoBase {
	public ContentTeHuiGoods contentGoods;
	/**身上有的钻石数量 */
	public UILabel number;
	public void init(WindowBase win){
		contentGoods.fatherWindow=win;
		InitShopFPort fport = FPortManager.Instance.getFPort ("InitShopFPort") as InitShopFPort;		
		fport.access (getDataSuccess);
	}
	void getDataSuccess ()
	{
		updateInfo ();
		reloadShop ();
		MaskWindow.UnlockUI();
	}

	public void updateInfo ()
	{
		number.text = UserManager.Instance.self.getRMB () + "";
	}
	public void reloadShop ()
	{
		float y = contentGoods.transform.localPosition.y;
		ArrayList list = ShopManagerment.Instance.getAllTehuiGodds ();
		contentGoods.Initialize (list, ContentShopGoods.TEHUI_SHOP, updateInfo);
		contentGoods.reLoad (list.Count);
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			contentGoods.jumpToPos(y);})
		               );	
	}
	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {

	}
	public void updateUI(){
		updateInfo ();
		reloadShop ();
	}
}