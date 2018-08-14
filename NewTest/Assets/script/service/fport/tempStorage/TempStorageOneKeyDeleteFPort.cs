using UnityEngine;
using System.Collections;

/**
 * 临时仓库一键删除接口
 * @author 汤琦
 * */
public class TempStorageOneKeyDeleteFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TEMP_STORAGE_ONEKEYDELETE);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString (); 
		if (str == "success") {
			callback ();
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}
}
