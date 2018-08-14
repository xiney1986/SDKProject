using System;
 
/**
 * 活动商品对象
 * @author hzh
 * */
public class NoticeActiveGoods : Goods
{

	//[1000,[[400000000商品id,1全服已购买次数]],[[[consume_rmb类型,200活动期间消耗]暂时只有一个],[[[role_times,400000000],1]个人消耗次数,...]]]
	//全服已购买次数
	public int serverBuyCount;
	//个人已购买次数
	public int roleBuyCount;
	//timeID
	public int timeID;

	public NoticeActiveGoods (int sid) : base(sid)
	{
	}

	public int getServerCountCanBuy ()
	{
		return sample.serverMaxCount - serverBuyCount;
	}

	public int getRoleCountCanBuy ()
	{
		return sample.maxBuyCount - roleBuyCount;
	}

}  