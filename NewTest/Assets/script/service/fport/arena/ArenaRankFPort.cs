using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获取海选信息
 * @author yxl
 * */
public class ArenaRankFPort : BaseFPort
{
    CallBack<List<RankItemMoney>> callback;
  
    public void access (CallBack<List<RankItemMoney>> callback,int team,int count)
	{   
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_RANK);
        message.addValue("team",new ErlString(team.ToString()));
        message.addValue("index",new ErlString(count.ToString()));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlArray array = message.getValue ("msg") as ErlArray;
        if (array != null)
        {
            List<RankItemMoney> list = new List<RankItemMoney>();
            for(int i = 0; i < array.Value.Length; i++)
            {
                ErlArray ea = array.Value[i] as ErlArray;
                RankItemMoney item = new RankItemMoney();
                item.uid = ea.Value[0].getValueString();
                item.name = ea.Value[1].getValueString();
                item.vipLevel = StringKit.toInt(ea.Value[2].getValueString());
                item.money = StringKit.toInt(ea.Value[3].getValueString());
//                if(item.vipLevel > 0)
//                    item.name = "[VIP"+item.vipLevel+"]" + item.name;
                list.Add(item);
            }

            if(callback != null)
            {
                callback(list);
            }
        }

	}
}
