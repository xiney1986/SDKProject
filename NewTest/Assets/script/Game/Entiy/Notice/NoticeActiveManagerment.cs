using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * 公告活动管理器
 * @author hzh
 * */
public class NoticeActiveManagerment
{

    public const int TYPEOF_CONSUME_REBATE = 1;//限时抢购
    public const int TYPEOF_RECHARGE = 2;//充值
    public const int TYPEOF_CONSUME = 3;//消费
    public const int TYPEOF_EXCHANGE = 4;//兑换
    public const int TYPEOF_DOUBLE_RMB = 5;//双倍充值
    public const int TYPEOF_EQUIP_REMAKE = 6;// 装备提示
    public const int TYPEOF_LIMIT_COLLECT = 7;//限时收集

    //活动sid为key
    IntKeyHashMap activeInfo = new IntKeyHashMap();

    public NoticeActiveManagerment()
    {
    }

    public object getActiveInfoBySid(int activeID)
    {
        return activeInfo.get(activeID);
    }

    public void putActiveInfo(int activeID, object obj)
    {
        activeInfo.put(activeID, obj);
    }

    public static NoticeActiveManagerment Instance
    {
        get { return SingleManager.Instance.getObj("NoticeActiveManagerment") as NoticeActiveManagerment; }
    }

    //[40,1,[[[40000002,1]],[[[consume_rmb,6701]],[[[role_times,40000002],1]]]]]
    //[40,1,not_open|no_sid]
    public void parseInfo(ErlArray arr)
    {
        int activeID = StringKit.toInt(arr.Value[0].getValueString());//活动id
        int activeType = StringKit.toInt(arr.Value[1].getValueString());//活动类型
        ErlArray temp = arr.Value[2] as ErlArray;
        int num;
        if (activeType == TYPEOF_CONSUME_REBATE)
        {
            temp = arr.Value[2] as ErlArray;
            if (temp == null || temp.Value.Length < 1)
                initConsumeRebateGoods(activeID);
            else
                initConsumeRebateGoods(activeID, temp.Value[0] as ErlArray, temp.Value[1] as ErlArray);
        }
        else if (activeType == TYPEOF_RECHARGE || activeType == TYPEOF_CONSUME)
        {
            temp = arr.Value[2] as ErlArray;
            if (temp == null || temp.Value.Length < 1)
                initNewRecharge(activeID);//充值小活动
            else
            {
                num = StringKit.toInt(temp.Value[1].getValueString());
                ErlArray ea = (temp.Value[0] as ErlArray).Value[0] as ErlArray;
                initNewRecharge(activeID, StringKit.toInt(ea.Value[1].getValueString()), num);
            }
        }
        else if (activeType == TYPEOF_EXCHANGE)
        {
            temp = arr.Value[2] as ErlArray;
            if (temp == null || temp.Value.Length < 1)
                initNewExchange(activeID);
            else
            {
                initNewExchange(activeID, temp.Value[0] as ErlArray, temp.Value[1] as ErlArray);
            }
        }
        else if (activeType == TYPEOF_DOUBLE_RMB)
        {
            //[55,5,1|not open]
            if (arr.Value[2].getValueString() == "1") 
                DoubleRMBManagement.Instance.isEnd = true; 
            initDoubleRmb(activeID, arr.Value[2].getValueString() == "1");
        }
        else if (activeType == TYPEOF_EQUIP_REMAKE)
        {
            activeInfo.put(activeID, new DoubleRMBInfo(activeID, arr.Value[2].getValueString() == "1"));
        }
        else if (activeType == TYPEOF_LIMIT_COLLECT)
        {
            LimitCollectSample sample = LimitCollectSampleManager.Instance.getSampleBySid(activeID);
            if(sample!=null)sample.clearCollected();
            if (temp != null)
                initLimitCollectData(activeID, temp);
          
        }
    }


    //serverInfo=[[400000000商品id,1全服已购买次数]],userInfo=[[[consume_rmb类型,200活动期间消耗]暂时只有一个],[[[role_times,400000000],1]个人消耗次数,...]]
    private void initConsumeRebateGoods(int activeID, ErlArray serverInfo, ErlArray userInfo)
    {
        NoticeActiveServerInfo consumeInfo = activeInfo.get(activeID) as NoticeActiveServerInfo;
        if (consumeInfo == null)
        {
            consumeInfo = new NoticeActiveServerInfo();
            activeInfo.put(activeID, consumeInfo);
        }
        ErlArray temp, temp1;
        NoticeActiveGoods goods;
        int sid;
        for (int i = 0; i < serverInfo.Value.Length; i++)
        {
            temp = serverInfo.Value[i] as ErlArray;
            sid = StringKit.toInt(temp.Value[0].getValueString());
            goods = consumeInfo.goodsList.get(sid) as NoticeActiveGoods;
            if (goods == null)
            {
                goods = new NoticeActiveGoods(sid);
                goods.timeID = activeID;
                consumeInfo.goodsList.put(sid, goods);
            }
            goods.serverBuyCount = StringKit.toInt(temp.Value[1].getValueString());
        }
        if (userInfo.Value.Length > 0)
        {
            temp = (userInfo.Value[0] as ErlArray);
            if (temp.Value.Length > 0)
                consumeInfo.consumeValue = StringKit.toInt((temp.Value[0] as ErlArray).Value[1].getValueString()); //兑换rmb消耗入门限制
            temp = userInfo.Value[1] as ErlArray;
            for (int i = 0; i < temp.Value.Length; i++)
            {
                temp1 = temp.Value[i] as ErlArray;
                sid = StringKit.toInt((temp1.Value[0] as ErlArray).Value[1].getValueString());
                goods = consumeInfo.goodsList.get(sid) as NoticeActiveGoods;
                if (goods == null)
                {
                    goods = new NoticeActiveGoods(sid);
                    goods.timeID = activeID;
                    consumeInfo.goodsList.put(sid, goods);
                }
                goods.roleBuyCount = StringKit.toInt(temp1.Value[1].getValueString());
            }
        }
    }

    private void initConsumeRebateGoods(int activeID)
    {
        activeInfo.put(activeID, new NoticeActiveServerInfo());
    }

    public List<NoticeActiveGoods> getGoodsList(NoticeActiveAndSid[] actives)
    {
        List<NoticeActiveGoods> list = new List<NoticeActiveGoods>();
        NoticeActiveServerInfo serverInfo;
        NoticeActiveGoods goods;
        foreach (NoticeActiveAndSid active in actives)
        {
            serverInfo = activeInfo.get(active.activeID) as NoticeActiveServerInfo;
            foreach (int sid in active.exchangeSids)
            {
                goods = serverInfo.goodsList.get(sid) as NoticeActiveGoods;
                if (goods == null)
                {
                    goods = new NoticeActiveGoods(sid);
                    goods.timeID = active.activeID;
                    serverInfo.goodsList.put(sid, goods);
                }
                list.Add(goods);
            }
        }
        return list;
    }

    //[[[active_cash,50],50|2|50|'首充奖励'|1|0|0|1,0,100#5,11442,1#4,31004,1],...]
    public void parseGMDetailInfo(ErlArray arr)
    {
        ErlArray temp;
        string dataStr, type;
        int sid;
        for (int i = 0; i < arr.Value.Length; i++)
        {
            //[[active_cash,50],50|2|50|'首充奖励'|1|0|0|1,0,100#5,11442,1#4,31004,1]
            temp = arr.Value[i] as ErlArray;
            type = (temp.Value[0] as ErlArray).Value[0].getValueString();
            sid = StringKit.toInt((temp.Value[0] as ErlArray).Value[1].getValueString());
            dataStr = temp.Value[1].getValueString();
            if (type == "active_cash")
            {
                RechargeSampleManager.Instance.updataSample(sid, dataStr);
            }
            else if (type == "active_time")
            {
                TimeConfigManager.Instance.updataSample(sid, dataStr);
            }
            else if (type == "exchange_list")
            {
                ExchangeSampleManager.Instance.updataSample(sid, dataStr);
            }
            else if (type == "active_consume")
            {
                RechargeSampleManager.Instance.updataSample(sid, dataStr);
            }
            else if (type == "active_double_cash")
            {
                NoticeSampleManager.Instance.updataSample(sid, dataStr);
            }
            else if (type == "shop_list")
            {
                GoodsSampleManager.Instance.updataSample(sid, dataStr);
            }
            else if (type == "active_switch")
            {
                NoticeManagerment.Instance.updateCloseNoticeList(sid, dataStr == "0");
            }
        }
    }

    //[[active_cash,50],50|2|50|'首充奖励'|1|0|0|1,0,100#5,11442,1#4,31004,1]
    public void parseGMDetailOneInfo(ErlArray arr)
    {
        string type = (arr.Value[0] as ErlArray).Value[0].getValueString();
        int sid = StringKit.toInt((arr.Value[0] as ErlArray).Value[1].getValueString());
        string dataStr = arr.Value[1].getValueString();
        if (type == "active_cash")
        {
            RechargeSampleManager.Instance.updataSample(sid, dataStr);
        }
        else if (type == "active_time")
        {
            TimeConfigManager.Instance.updataSample(sid, dataStr);
        }
        else if (type == "exchange_list")
        {
            ExchangeSampleManager.Instance.updataSample(sid, dataStr);
        }
        else if (type == "active_consume")
        {
            RechargeSampleManager.Instance.updataSample(sid, dataStr);
        }
        else if (type == "active_double_cash")
        {
            NoticeSampleManager.Instance.updataSample(sid, dataStr);
        }
        else if (type == "shop_list")
        {
            GoodsSampleManager.Instance.updataSample(sid, dataStr);
        }
        else if (type == "active_switch")
        {
            NoticeManagerment.Instance.updateCloseNoticeList(sid, dataStr == "0");
        }
    }

    //初始化新充值活动
    private void initNewRecharge(int activeID)
    {
        activeInfo.put(activeID, new NewRecharge(activeID, 0));
    }

    private void initNewRecharge(int activeID, int num, int count)
    {
        activeInfo.put(activeID, new NewRecharge(activeID, count, num));
    }

    public List<Recharge> getRechargeList(int[] sids)
    {
        List<Recharge> list = new List<Recharge>();
        ActiveTime activeTime;
        int now = ServerTimeKit.getSecondTime();
        List<int> closeSidList = NoticeManagerment.Instance.CloseNoticeSidList;
        foreach (int sid in sids)
        {
            activeTime = ActiveTime.getActiveTimeByID(sid);
            if (activeTime == null)
                continue;
            if (activeTime.getIsFinish() || activeTime.getPreShowTime() > now || (now > activeTime.getEndTime() && activeTime.getEndTime() > 0) || closeSidList.Contains(sid))
                continue;
            list.Add(activeInfo.get(sid) as Recharge);
        }
        return list;
    }

    //初始化新兑换活动 serverInfo=[[400000000商品id,1全服已购买次数]],userInfo=[[[consume_rmb类型,200活动期间消耗]暂时只有一个],[[[role_times,400000000],1]个人消耗次数,...]]
    private void initNewExchange(int activeID, ErlArray serverInfo, ErlArray userInfo)
    {
        NoticeActiveServerInfo exchangeInfo = activeInfo.get(activeID) as NoticeActiveServerInfo;
        if (exchangeInfo == null)
        {
            exchangeInfo = new NoticeActiveServerInfo();
            activeInfo.put(activeID, exchangeInfo);
        }
        ErlArray temp, temp1;
        NewExchange exchange;
        int sid;
        for (int i = 0; i < serverInfo.Value.Length; i++)
        {
            //暂时不考虑全服数据
        }
        if (userInfo.Value.Length > 0)
        {
            temp = (userInfo.Value[0] as ErlArray);
            if (temp.Value.Length > 0)
                exchangeInfo.consumeValue = StringKit.toInt((temp.Value[0] as ErlArray).Value[1].getValueString()); //兑换rmb消耗入门限制
            temp = userInfo.Value[1] as ErlArray;
            for (int i = 0; i < temp.Value.Length; i++)
            {
                temp1 = temp.Value[i] as ErlArray;
                sid = StringKit.toInt((temp1.Value[0] as ErlArray).Value[1].getValueString());
                exchange = exchangeInfo.goodsList.get(sid) as NewExchange;
                if (exchange == null)
                {
                    exchange = new NewExchange(sid, 0);
                    exchange.timeID = activeID;
                    exchangeInfo.goodsList.put(sid, exchange);
                }
                exchange.setNum(StringKit.toInt(temp1.Value[1].getValueString()));
            }
        }
    }

    private void initNewExchange(int activeID)
    {
        activeInfo.put(activeID, new NoticeActiveServerInfo());
    }

    //显示样本
    public List<Exchange> getExchangeList(NoticeActiveAndSid[] actives)
    {
        List<Exchange> list = new List<Exchange>();
        NoticeActiveServerInfo serverInfo;
        NewExchange exchange;
        ActiveTime activeTime;
        List<int> closeSidList = NoticeManagerment.Instance.CloseNoticeSidList;
        int now = ServerTimeKit.getSecondTime();
        foreach (NoticeActiveAndSid active in actives)
        {
            activeTime = ActiveTime.getActiveTimeByID(active.activeID);
            if (activeTime.getIsFinish() || activeTime.getPreShowTime() > now || now > activeTime.getEndTime())
                continue;
            serverInfo = activeInfo.get(active.activeID) as NoticeActiveServerInfo;
            foreach (int sid in active.exchangeSids)
            {
                if (closeSidList.Contains(sid)) continue;
                exchange = serverInfo.goodsList.get(sid) as NewExchange;
                if (exchange == null)
                {
                    exchange = new NewExchange(sid, 0);
                    exchange.timeID = active.activeID;
                    serverInfo.goodsList.put(sid, exchange);
                }
                list.Add(exchange);
            }
        }
        return list;
    }

    public void updateExchange(int activeID, int sid, int num)
    {
        NoticeActiveServerInfo serverInfo = activeInfo.get(activeID) as NoticeActiveServerInfo;
        if (serverInfo == null)
            return;
        else
        {
            foreach (NewExchange exchange in serverInfo.goodsList.valueArray())
            {
                if (exchange.sid == sid)
                {
                    exchange.addNum(num);
                    ExchangeManagerment.Instance.setCompleteSid(sid);
                    return;
                }
            }
        }
    }

    private void initDoubleRmb(int activeID, bool state)
    {
        DoubleRMBManagement.Instance.IsRecharge = state;
        activeInfo.put(activeID, new DoubleRMBInfo(activeID, state));
    }


    private void initLimitCollectData(int activeID, ErlArray array)
    {
        LimitCollectSample sample = LimitCollectSampleManager.Instance.getSampleBySid(activeID);
        if (array.Value.Length == 0)
        {
            sample.isReceived = false;
        }
        else
        {
            ErlArray tempArray = array.Value[0] as ErlArray;
            for (int j = 0; j < tempArray.Value.Length; j++)
            {
                ErlArray conditionArray = tempArray.Value[j] as ErlArray;
                int sid = StringKit.toInt(conditionArray.Value[0].getValueString());
                int needNum = StringKit.toInt(conditionArray.Value[1].getValueString());
                sample.setCollected(sid, needNum);
            }
            bool received = StringKit.toInt(array.Value[1].getValueString()) == 1;
            sample.isReceived = received;
        }
        activeInfo.put(activeID, sample);
    }
}
