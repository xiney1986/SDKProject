using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * 获得小组排名
 * @author gc
 * */
public class GodsWarGetGroupRankInfoFport : BaseFPort
{
    CallBack callback;

    public void access (CallBack callback)
	{   
        this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_GETGROUPRANKINFO);
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
				GodsWarManagerment.Instance.myGroupRanklist = list;
		}

        if (callback != null)
        {
			callback();
			MaskWindow.UnlockUI();
        }
	}
}
