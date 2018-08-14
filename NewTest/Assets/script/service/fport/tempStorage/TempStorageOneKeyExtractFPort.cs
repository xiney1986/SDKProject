using UnityEngine;
using System.Collections;

/**
 * 临时仓库一键提取接口
 * @author 汤琦
 * */
public class TempStorageOneKeyExtractFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TEMP_STORAGE_ONEKEYEXTRACT);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value; 
		if(str == "success")
		{
			callback();
		}
	}
}
