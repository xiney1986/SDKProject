using UnityEngine;
using System.Collections;

/**
 * 临时仓库删除接口
 * @author 汤琦
 * */
public class TempStorageDeleteFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (int index, CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TEMP_STORAGE_DELETE);
		message.addValue ("index", new ErlInt (index));//要删除的指定索引
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value; 
		if (str == "success") {
			callback ();
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}
}
