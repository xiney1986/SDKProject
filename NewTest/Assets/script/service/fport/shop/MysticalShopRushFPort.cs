using UnityEngine;
using System.Collections;

public class MysticalShopRushFPort : BaseFPort {
	CallBack callback;
    private bool useRush=false;
	public MysticalShopRushFPort()
	{

	}
	public void rushGoods(CallBack callback,bool useRush)
	{
        this.useRush = useRush;
		this.callback=callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MYSTICAL_RUCH);
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		ShopManagerment.Instance.updateMysticalShop(message.getValue ("msg") as ErlList);
		if(useRush)ShopManagerment.Instance.setMyRushCount(ShopManagerment.Instance.getMyRushCount()+1);
		callback();
	}
}
