using UnityEngine;
using System.Collections;

public class GoddessTrainingInit : BaseFPort {

	CallBack<int> callback;
	
	
	public void access(CallBack<int > callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage(FrontPort.CARDTRAINING_INIT);
		message.addValue("type",new ErlInt(2));
		access(message);
	}
	public override void read(ErlKVMessage message)
	{
		ErlArray arr = message.Value[1] as ErlArray;
		if (arr == null) return;
		
		for (int i = 0; i < arr.Value.Length; i++)
		{
			ErlArray item = arr.Value[i] as ErlArray;
			int index = StringKit.toInt(item.Value[0].getValueString());
			int time = StringKit.toInt(item.Value[1].getValueString());
			GuideManager.Instance.goddessTranningTime=time;
			callback(time);
		}
		if(arr.Value.Length==0)callback(0);
		
	}
}
