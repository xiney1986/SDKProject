using UnityEngine;
using System;

/**
 * 序列化单个仓库接口
 * @author 汤琦
 * */
public class SerializeStorageFPort : BaseFPort
{
	private CallBack callback;
	//public string uid = "";//受影响卡片id
	
	public SerializeStorageFPort ()
	{
		
	}
	//type仓库名字：card_storage，beast_storage，equip_storage，goods_storage，temp_storage
	public void access (string type, CallBack back)
	{
		this.callback = back;
		//this.uid = uid;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STORAGE_SERIALIZE); 
		message.addValue ("type", new ErlString (type));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlArray arr = message.getValue ("msg") as ErlArray;
		string type = (arr.Value [0] as ErlAtom).Value;
		ErlArray info = arr.Value [1] as ErlArray;
		prase (type, info);

		if (callback != null){
			callback ();
		}
	}
	
	//解析单个仓库数据
	private void prase (string type, ErlArray arr)
	{ 
		StorageManagerment.Instance.updateStorageInfo (type, arr);
	}
}
