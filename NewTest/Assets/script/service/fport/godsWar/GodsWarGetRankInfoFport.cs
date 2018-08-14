using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * 获得支持人数排名
 * @author gc
 * */
public class GodsWarGetRankInfoFport : BaseFPort
{
    CallBack callback;
	private int big_id;

    public void access (CallBack callback,int big_id)
	{   
        this.callback = callback;
		this.big_id = big_id;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_GETRANK);
		message.addValue("big_id", new ErlInt(big_id));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType str = message.getValue ("msg") as ErlType;
		if(str is ErlArray)
		{
			ErlArray arr = str as ErlArray;
			List<GodsWarRankUserInfo> list = new List<GodsWarRankUserInfo>();
			if(arr.Value.Length!=0)
			{
				for (int i = 0; i < arr.Value.Length; i++) {
					ErlArray tmp = arr.Value[i] as ErlArray;
					GodsWarRankUserInfo user = new GodsWarRankUserInfo();
					user.bytesRead(tmp);
					list.Add(user);
				}

			}
			if(list.Count>0)
				switch (big_id) {
					case 110:
					GodsWarManagerment.Instance.usersRankList_bronze = list;
					break;
					case 111:
					GodsWarManagerment.Instance.usersRankList_silver= list;
					break;
					case 112:
					GodsWarManagerment.Instance.usersRankList_gold= list;
					break;
					default:
					break;
			}
		}

        if (callback != null)
        {
			callback();
			MaskWindow.UnlockUI();
        }
	}
}
