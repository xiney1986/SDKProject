
using System;

public class SweepGetInfoFPort : BaseFPort
{ 
	private CallBack callback;	

	public void getSweep(CallBack callback)
	{  
		
		this.callback = callback;		
		ErlKVMessage message = new ErlKVMessage (FrontPort.SWEEP_GET);	
		access (message);
	}
	public void parseKVMsg (ErlKVMessage message) 
	{
		ErlList data=message.getValue("msg") as ErlList;
		if(data==null)
		{
			string msg=(message.getValue("msg") as ErlType).getValueString();
			SweepManagement.Instance.clearData();
			if (callback != null)
			{
				callback();
				callback = null;
			}
			return;
		}		
		int sweepTimes=0;
		int sweepCDTime=0;
		int sweepMissionSid=0;
		int sweepMissionLevel=0;
		int state=0;
		int startTime=0;
		int pvpCount=0;
		int arrayID = 0;
		
		int length=data.Value.Length;
		ErlArray itemArray;
		string key;
		int value;
		for(int i=0;i<length;i++)
		{
			itemArray=data.Value[i] as ErlArray;
			key=itemArray.Value[0].getValueString();
			value=StringKit.toInt(itemArray.Value[1].getValueString());
			switch(key)
			{
			case "pvp_num":
				pvpCount=value;
				break;
			case "start_time":
				startTime=value;
				break;
			case "cd":
				sweepCDTime=value;
				break;
			case "sweep_num":
				sweepTimes=value;
				break;
			case "fb_id":
				sweepMissionSid=value;
				break;
			case "fb_lv":
				sweepMissionLevel=value;
				break;
			case "state":
				state=value;
				break;
			case "array_id":
				arrayID=value;
				break;
			}
		}
		SweepManagement.Instance.initPvpNum(pvpCount);
		SweepManagement.Instance.SweepCostTime=sweepCDTime;
		SweepManagement.Instance.M_updateSweepInfo(state,sweepMissionSid,sweepMissionLevel,sweepTimes,startTime,arrayID);
	}

	public override void read (ErlKVMessage message)
	{
		parseKVMsg(message);
		if (callback != null)
		{
			callback();
			callback = null;
		}
		/*
		[{pvp_num,3},
		 {start_time,1401939264},
		 {fb_id,41001},
		 {fb_lv,1},
		 {sweep_num,3},
		 {sweep_pve,15},
		 {array_id,1},
		 {state,1}]
		*/
	
	}
}



