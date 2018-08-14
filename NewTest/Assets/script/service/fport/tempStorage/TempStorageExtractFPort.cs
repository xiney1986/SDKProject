using UnityEngine;
using System.Collections;

/**
 * 临时仓库提取接口
 * @author 汤琦
 * */
public class TempStorageExtractFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (int index, CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TEMP_STORAGE_EXTRACT);
		message.addValue ("index", new ErlInt (index));//要提取的指定索引
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value; 
		if (str == "ok") {
			callback ();
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}
}
