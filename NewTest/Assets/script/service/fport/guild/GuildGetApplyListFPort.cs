using UnityEngine;
using System.Collections;

/**
 * 获得公会申请列表接口
 * @author 汤琦
 * */
public class GuildGetApplyListFPort : BaseFPort
{
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GET_APPLYLIST);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		if(parseKVMsg (message) && callback != null)
			callback();
	}
	//解析ErlKVMessgae
	public bool parseKVMsg (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType; 
		if(type.getValueString() == "non_mem")
			return false;
		ErlArray array = type as ErlArray;
		GuildManagerment.Instance.clearIds();
		if(array.Value.Length > 0)
		{
			
			for (int i = 0; i < array.Value.Length; i++) {
				GuildManagerment.Instance.addIds(array.Value[i].getValueString());
			}
		}
		return true;
	}
}
