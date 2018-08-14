using System;
 
/**
 * 购买商品接口
 * @author longlingquan
 * */
public class BuyGoodsFPort:BaseFPort
{
	
	private const string KEY_CARD = "card";
	private const string KEY_EQUIP = "equip";
	private const string KEY_TOOL = "goods";//后台用的是goods
	private const string KEY_BEAST = "beast";
	private const string KEY_MSG = "msg";
	private CallBack<int,int> callback;
	private int sid = 0;
	private int num = 0;
	private int type=0;

	public BuyGoodsFPort ()
	{
		
	}
	
	public void buyGoods (int sid, int num, CallBack<int,int> callback)
	{  
		this.sid = sid;
		this.num = num;
		this.callback = callback;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.SHOP_BUY);  
		message.addValue ("goods_id", new ErlInt (sid));//商品sid
		message.addValue ("num", new ErlInt (num));//商品数量
		access (message);
	}
	public void buyGoods (int sid, int num,int type, CallBack<int,int> callback)
	{  
		this.sid = sid;
		this.num = num;
		this.callback = callback;
		this.type=type;
		ErlKVMessage message = new ErlKVMessage (FrontPort.SHOP_BUY);  
		message.addValue ("goods_id", new ErlInt (sid));//商品sid
		message.addValue ("num", new ErlInt (num));//商品数量
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlArray array = message.getValue ("msg") as ErlArray;
		if (array != null) {
			StorageManagerment.Instance.parseAddStorageProps(array);
			if(type==ShopType.STARSOUL_DEBRIS){
				Goods good=new Goods(sid);
				type=0;
				StarSoulManager.Instance.setDebrisNumber(StarSoulManager.Instance.getDebrisNumber()-good.getCostPrice()*num);
			}

			if (callback != null)
				callback (sid, num);
		} else {
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("s0207"));
		}
		callback = null;
	}
} 

