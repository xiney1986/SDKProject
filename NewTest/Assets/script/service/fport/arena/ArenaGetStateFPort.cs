using UnityEngine;
using System;

/**
 * 获取当前竞技场状态
 * @author yxl
 * */
public class ArenaGetStateFPort : BaseFPort
{

	public CallBack<int,int> callback;

	public void access (CallBack<int,int> callback)
	{   
		if (!needGetState ()) {
			callback(ArenaManager.instance.state,ArenaManager.instance.stateEndTime);
			return;
		}
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.ARENA_GET_STATE);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		int[] i = parseKVMsg (message);
		if (i != null && callback != null)
			callback (i [0], i [1]);
	}
	//解析ErlKVMessgae
	public int[] parseKVMsg (ErlKVMessage message)
	{
		ErlArray array = message.getValue ("msg") as ErlArray;
		if (array != null) {
			int[] i = new int[2];
			i [0] = StringKit.toInt (array.Value [0].getValueString ());
			i [1] = StringKit.toInt (array.Value [1].getValueString ());
			ArenaManager.instance.state = i [0];
			ArenaManager.instance.stateEndTime = i [1];
			return i;
		}
		return null;
	}

	//true表示需要向后台取数据
	private bool needGetState(){
		return ArenaManager.instance.stateEndTime < ServerTimeKit.getSecondTime ();
	}
}
