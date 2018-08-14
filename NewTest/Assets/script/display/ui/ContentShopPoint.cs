using UnityEngine;
using System.Collections;

public class ContentShopPoint : MonoBase {
	public ContentShopGoods contentGoods;
	public UILabel number;
	public ButtonBase buttonRecharge;
	public void init(WindowBase win){
		contentGoods.fatherWindow=win;
		buttonRecharge.fatherWindow=win;
		InitShopFPort fport = FPortManager.Instance.getFPort ("InitShopFPort") as InitShopFPort;		
		fport.access (getDataSuccess);
	}
	void getDataSuccess ()
	{

		updateInfo ();
		reloadShop ();
	}
	public void updateInfo ()
	{
		number.text = UserManager.Instance.self.getRMB () + "";
	}
	public void reloadShop ()
	{
		float y = contentGoods.transform.localPosition.y;
		ArrayList list = ShopManagerment.Instance.getAllRmbGoods ();
		contentGoods.Initialize (list, ContentShopGoods.RMB_SHOP, updateInfo);
		contentGoods.reLoad (list.Count);
		StartCoroutine(Utils.DelayRunNextFrame(()=>{contentGoods.jumpToPos(y);
		}));	
	}
	public void updateUI(){
		reloadShop();
		updateInfo ();
	}
	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {
		if (gameObj.name == "buttonRecharge") {  
			UiManager.Instance.openWindow<rechargeWindow> (); 
		}	
	}

}
