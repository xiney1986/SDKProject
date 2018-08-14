using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 *得到许愿信息
 * */
public class FestivalWishFPort : BaseFPort
{
    CallBack callback;
  
    public void access (string sids, CallBack callback)
	{   
        this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_FESTIVALWISH_INFO);
		message.addValue("sid",new ErlString(sids));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlArray array = message.getValue ("msg") as ErlArray;

		if(array != null)
		{
			//通信成功之后清楚老数据
			FestivalWishManagerment.Instance.festivalWishs.Clear();
			for(int i=0;i<array.Value.Length;i++)
			{
				ErlArray _array = array.Value[i] as ErlArray;
				if(_array!=null)
				{
					int poss = 0;
					FestivalWish wish = new FestivalWish();
					wish.sid = StringKit.toInt(_array.Value [poss++].getValueString());
					wish.currentWishNum = StringKit.toInt(_array.Value [poss++].getValueString());
					wish.state = StringKit.toInt(_array.Value [poss++].getValueString());
					wish.endTime = StringKit.toInt(_array.Value [poss++].getValueString());
					wish.sample = FestivalWishSampleManager.Instance.getFestivalWishSampleBySid(wish.sid);
					if(wish.endTime!=-1&&wish.endTime!=0)
						FestivalWishManagerment.Instance.festivalWishs.Add(wish);
				}
			}
		}

        if (callback != null)
        {
            callback();
			callback = null;
        }
	}
}
