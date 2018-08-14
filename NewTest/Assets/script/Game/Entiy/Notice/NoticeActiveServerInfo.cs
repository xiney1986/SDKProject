using System;
using System.Collections.Generic;
 
/**
 * 限时抢购服务器数据
 * @author hzh
 * */
public class NoticeActiveServerInfo
{

	//[1000,[[400000000商品id,1全服已购买次数]],[[[consume_rmb类型,200活动期间消耗]暂时只有一个],[[[role_times,400000000],1]个人消耗次数,...]]]
	//已消耗多少
	public int consumeValue;
	//活动商品信息
	public IntKeyHashMap goodsList = new IntKeyHashMap();


}  