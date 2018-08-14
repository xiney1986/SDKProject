using System;
 
/**
 * 商品模板
 * @author longlingquan
 * */
public class GoodsSample:Sample
{
	public GoodsSample ()
	{
	}
	
	public string name = "";//商品名称
	public int shopType = 0;//商店 类型
	public int iconId = 0;//商品图标编号
	public int goodsType = 0;//商品具体类型(card,equip,tool...)
	public int goodsSID = 0;//商品sid
	public int goodsNum = 0;//商品数量
	public int costType = 0;//花费类型(money,rmb,tool...)
	public int costToolSid = 0;//花费道具sid
	public int[] costPrice;//花费价格（）
	public int maxBuyCount = 0;//购买最大次数
	public int cycle = 0;//循环时间(小时)
	public int startTime = 0;//开始时间(秒)
	public int endTime = 0;//结束时间(秒)
	public int showType = 0;//是否是hot or new ...
	public int order = 0;//顺序
	public int offer=0;//特价（原价)显示用
	public int serverMaxCount; //全服次数
	public int rmbCondition; //rmb消费入门限制(购买等级限制)
    public int userLevelCondition;//玩家等级限制
    public int userVipLevelCondition;//VIP等级限制
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;  
		string[] strArr = str.Split ('|');
		//checkLength (strArr.Length, 18); 
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] shopType
		this.shopType = StringKit.toInt (strArr [2]);
		//strArr[3] iconId
		this.iconId = StringKit.toInt (strArr [3]);
		//strArr[4] goodsType
		this.goodsType = StringKit.toInt (strArr [4]);
		//strArr[5] goodsSID
		this.goodsSID = StringKit.toInt (strArr [5]);
		//strArr[6] goodsNum
		this.goodsNum = StringKit.toInt (strArr [6]);
		//strArr[7] costType
		this.costType = StringKit.toInt (strArr [7]);
		//strArr[8] costToolSid
		this.costToolSid = StringKit.toInt (strArr [8]);
		//strArr[9] costPriceStringKit.toInt (strArr [9]);
		this.costPrice =parsePrice(strArr [9]);
		//strArr[10] buyNum
		this.maxBuyCount = StringKit.toInt (strArr [10]);
		//strArr[11] cycle
		this.cycle = StringKit.toInt (strArr [11]);
		//strArr[12] startTime
		this.startTime = StringKit.toInt (strArr [12]);
		//strArr[13] overTime
		this.endTime = StringKit.toInt (strArr [13]);
		//strArr[14] showType
		this.showType = StringKit.toInt (strArr [14]);
		//strArr[15] order
		this.offer = StringKit.toInt (strArr [15]);
		//strArr[16] offer
		this.order=StringKit.toInt(strArr[16]);
		this.serverMaxCount=StringKit.toInt(strArr[17]);
		this.rmbCondition=StringKit.toInt(strArr[18]);
	    if (strArr.Length > 19)
	    {
	        userLevelCondition = StringKit.toInt(strArr[19]);
            userVipLevelCondition = StringKit.toInt(strArr[20]);
	    }
	}
	private int[] parsePrice(string str){
		string[] pricess=str.Split(',');
		int[] pric=new int[pricess.Length];
		for(int i=0;i<pricess.Length;i++){
			pric[i]=StringKit.toInt(pricess[i]);
		}
		return pric;
	}
	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 

