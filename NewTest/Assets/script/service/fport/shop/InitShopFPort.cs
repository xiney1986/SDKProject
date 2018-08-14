using System;
 
/**
 * 初始化商店购买限制信息
 * @author huangzhenghan
 * */
public class InitShopFPort:BaseFPort
{
	CallBack callback;

	public InitShopFPort ()
	{
		
	}
	 
	public void access (CallBack callback)
	{ 
		this.callback=callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.SHOP_INIT);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		string str = (message.getValue ("msg") as ErlAtom).Value; 
		if(str=="ok")
		{
			ShopManagerment.Instance.updateShop(message.getValue ("info") as ErlArray);
			callback();
		}
	}
}

