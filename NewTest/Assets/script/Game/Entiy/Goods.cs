using System;
 
/**
 * 商品实体对象
 * @author longlingquan
 * */
public class Goods : CloneObject
{
	public int sid = 0;
	public int nowBuyNum = 0;//当前已经购买数量
	public int isBuy = 0;
	public bool isShowInMytical = false;//是否显示在神秘商店
	public int showIndex = 0;//显示排序
	protected GoodsSample sample;

	public Goods (int sid)
	{
		this.sid = sid;
		sample = GoodsSampleManager.Instance.getGoodsSampleBySid (sid);
	}
	
	//设置当前已经购买数量
	public void setNowBuyNum (int num)
	{
		this.nowBuyNum = num;
	}

	//针对天梯争霸获取不同sid天梯争霸的商品标识
	public int getLadderGoodSid
	{
		get {
			return sample.rmbCondition;
		}
	}

    /// <summary>
    /// 针对功勋商店商品购买的等级限制
    /// </summary>
    public int getLimitLevel
    {
        get {
			return sample.userLevelCondition;
		}
    }
    public int getVipLimitLevel {
        get {
            return sample.userVipLevelCondition;
        }
    }

    public void setIsShowInMytical (bool bl)
	{
		this.isShowInMytical = bl;
	}

	public bool getIsShowInMytical ()
	{
		return isShowInMytical;
	}
	//设置当前是否已经购买过
	public void setIsBuy (int num)
	{
		this.isBuy = num;
	}

	public int getIsBuy ()
	{
		return isBuy;
	}
	
	public int getNowBuyNum ()
	{
		return nowBuyNum;
	}
	
	//获得商品名字
	public string getName ()
	{
		return sample.name;
	}
	//得到折扣价
	public int getOfferNum ()
	{
		return sample.offer;
	}
	
	//获得商品数量 返回值为-1表示数量不受限制
	public int getNum ()
	{
		int buyNum = sample.maxBuyCount;
		//商品数量无限制
		if (buyNum == 0)
			return ShopManagerment.GOODS_MAX;
		return buyNum - nowBuyNum;
	}
	
	//获得商品图标
	public int getIconId ()
	{
		return sample.iconId;
	}

	//是否是卡片碎片道具
	public bool isCardScrap ()
	{
		if (getGoodsType () == PrizeType.PRIZE_PROP) {
			PropSample propTemp = PropSampleManager.Instance.getPropSampleBySid (getGoodsSid ());
			if (propTemp.type == PropType.PROP_TYPE_CARDSCRAP) {
				return true;
			}
		}
		return false;
	}
    public bool isMagicScrap() {
        if (getGoodsType() == PrizeType.PRIZE_PROP) {
            PropSample propTemp = PropSampleManager.Instance.getPropSampleBySid(getGoodsSid());
            if (propTemp.type == PropType.PROP_MAGIC_SCRAP) {
                return true;
            }
        }
        return false;
    }
	//是否是碎片道具
	public bool isScrap ()
	{
		if (getGoodsType () == PrizeType.PRIZE_PROP) {
			PropSample propTemp = PropSampleManager.Instance.getPropSampleBySid (getGoodsSid ());
			if (propTemp.type == PropType.PROP_TYPE_CARDSCRAP || propTemp.type == PropType.PROP_TYPE_EQUIPSCRAP||propTemp.type==PropType.PROP_MAGIC_SCRAP) {
				return true;
			}
		}
		return false;
	}
	
	//获得商品类型
	public int getGoodsType ()
	{
		return sample.goodsType;
	}
	
	//获得商品消费类型
	public int getCostType ()
	{
		return sample.costType;
	}
	
	//获得商品消费价格
	public int getCostPrice ()
	{
	return nowBuyNum>(sample.costPrice.Length-1)?sample.costPrice[sample.costPrice.Length-1]:sample.costPrice[nowBuyNum];
	}
    public int getCostPriceForBuyWindow(int i)
    {
        return (nowBuyNum + i) > (sample.costPrice.Length - 1) ? sample.costPrice[sample.costPrice.Length - 1] : sample.costPrice[nowBuyNum + i];
    }
	//获得商品消耗道具sid
	public int getCostToolSid ()
	{
		return sample.costToolSid;
	}
	 
	//获得商品是否显示
	public bool isShow ()
	{
		//商品数量为0
		if (getNum () == 0)
			return false;
		if (sample.endTime == 0 || sample.startTime == 0)
			return true;
		//时间到期
		if (ServerTimeKit.getSecondTime () > sample.endTime)
			return false;
		return true;
	}
	
	//获得商品显示信息
	public string getShowInfo ()
	{
		return getShowTime () + " " + getShowNum ();
	}
	
	//获得数量显示
	private string getShowNum ()
	{
		if (getNum () == ShopManagerment.GOODS_MAX)
			return "";
		else
			return  LanguageConfigManager.Instance.getLanguage ("s0042", getNum () + ""); 
	}
	
	//获得显示时间
	public string getShowTime ()
	{ 
		//商品无到期时间
		if (sample.startTime == 0 || sample.endTime == 0)
			return "";
		//商品未开卖 显示商品开卖时间   开卖后 显示到期时间
		if ((int)ServerTimeKit.getSecondTime () < sample.startTime) {
			DateTime dt = TimeKit.getDateTime (sample.startTime); 
			string str = dt.ToString ("yyyy-MM-dd HH:mm") + LanguageConfigManager.Instance.getLanguage ("s0085");
			return str;
		} else { 
			//	DateTime dt = new DateTime (GoodsSampleManager.Instance.getGoodsSampleBySid (sid).endTime * 10 * 1000);
			DateTime dt = TimeKit.getDateTime (sample.endTime); 
			string str = dt.ToString ("yyyy-MM-dd HH:mm") + LanguageConfigManager.Instance.getLanguage ("s0086");
			return str;
		} 
	}
	
	//获得购买一个商品内的数量
	public int getGoodsShowNum ()
	{
		return sample.goodsNum;
	}
	//获得最大可购买的商品数量
	public int getGoodsMaxBuyCount ()
	{
		return sample.maxBuyCount;
	}
	//获得商店类型
	public int getGoodsShopType ()
	{
		return sample.shopType;
	}
	//获得商品sid
	public int getGoodsSid ()
	{
		return sample.goodsSID;
	}
	//获得商品顺序
	public int getOrder ()
	{
		return sample.order;
	}

	public GoodsSample getSample ()
	{
		if (sample == null)
			sample = GoodsSampleManager.Instance.getGoodsSampleBySid (sid);
		return sample;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}  