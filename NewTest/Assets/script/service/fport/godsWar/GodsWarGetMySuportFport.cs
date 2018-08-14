using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * 获取我的支持信息
 * @author gc
 * */
public class GodsWarGetMySuportFport : BaseFPort
{
    CallBack callback;

    public void access (CallBack callback)
	{   
        this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_GETMYSUPORTINFO);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlType str = message.getValue ("msg") as ErlType;
		if(str is ErlArray)
		{
			ErlArray arr = str as ErlArray;
			List<GodsWarMySuportInfo> list = new List<GodsWarMySuportInfo>();
			if(arr.Value.Length!=0)
			{
				for (int i = 0; i < arr.Value.Length; i++) {
					ErlArray tmp = arr.Value[i] as ErlArray;
					GodsWarMySuportInfo user = new GodsWarMySuportInfo();
					user.bytesRead(tmp);
					list.Add(user);
				}

			}
			if(list.Count>0)
				GodsWarManagerment.Instance.mySuportInfo = list;
	
		}
        if (callback != null)
        {
			callback();
			MaskWindow.UnlockUI();
        }
	}

}
