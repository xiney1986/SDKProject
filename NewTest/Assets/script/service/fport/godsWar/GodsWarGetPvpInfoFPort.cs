using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 获取对战点位信息
 * @author gc
 * */
public class GodsWarGetPvpInfoFPort : BaseFPort
{
	CallBack callback;
  
	public void access (int type,int index,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_GETIFINALPOINT_INFO);
		message.addValue("big_id",new ErlInt(type));
		message.addValue("yu_ming",new ErlInt(index));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		ErlType erl = message.getValue("msg") as ErlType;
		if(erl is ErlArray)
		{
			GodsWarFinalPoint user;
			List<GodsWarFinalPoint> infos = new List<GodsWarFinalPoint>();
			ErlArray erlarry = erl as ErlArray;
			int currentPostion=0;//当前淘汰赛的位置
			for(int i=0;i<erlarry.Value.Length;i++)
			{
				ErlArray aa = erlarry.Value[i] as ErlArray;
				if(aa.Value.Length!=0)
				{
					currentPostion++;
					for (int j = 0; j < aa.Value.Length; j++) {
						user = new GodsWarFinalPoint();
						user.bytesRead(aa.Value[j] as ErlArray);
						if(user.localID!=0)
							infos.Add(user);
					}
				}

			}
			GodsWarManagerment.Instance.finalRound = currentPostion;
			if(infos!=null)
				GodsWarManagerment.Instance.godsWarFinalPoints = infos;
			if(callback!=null)
				callback();
		}
		else
		{
			MessageWindow.ShowAlert(erl.getValueString());
			if(callback!=null)
				callback=null;
		}

	}

}
