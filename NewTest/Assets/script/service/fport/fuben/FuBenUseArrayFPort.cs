using UnityEngine;
using System.Collections;

/**
 * 废了，副本队伍使用方案接口
 * @author 汤琦
 * */
public class FuBenUseArrayFPort : BaseFPort
{
	
	private CallBack callback;
	
	public void access (int u_id, CallBack back)
	{ 
		this.callback = back;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_USE_ARRAY); 
		message.addValue ("array", new ErlInt (u_id));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value; 
		if (str == FPortGlobal.FUBEN_USEARRAY_SUCCESS) {
			callback ();
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}
	
}
