using System.Collections;

/**
 * 神秘商店端口
 * @author huangzhenghan
 * */
public class MysticalShopFPort : BaseFPort {
	CallBack callback;
	public MysticalShopFPort ()
	{
		
	}
	
	public void access (CallBack callback)
	{ 
		this.callback=callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MYSTICAL_SHOP_INIT);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 

		ShopManagerment.Instance.updateMysticalShop(message.getValue ("msg") as ErlList);
		ShopManagerment.Instance.setMyRushCount(StringKit.toInt ((message.getValue ("count") as ErlType).getValueString ()));
		callback();
		

	}


}
