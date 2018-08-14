using System; 
using UnityEngine;

/**
 * 仓库数据初始化
 * */
public class MiniStorageFPort:MiniBaseFPort
{
	public const string GOODS = "goods";//道具
	public const string TEMP = "temp";//临时
	public const string CARD = "card";//卡片
	public const string EQUIPMENT = "equip";//装备
	public const string BEAST = "beast";//召唤兽
	
	public delegate void CallBack ();

	private CallBack callback;
	
	//仓库数据初始化
	public void init (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.INIT_STORAGE);    
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}
	
	public override void read (ErlKVMessage message)
	{  
//		ErlList list = message.getValue ("msg") as ErlList; 
//		for (int i=0; i<list.Value.Length; i++) {
//			ErlArray arr = list.Value [i]as ErlArray;
//			string type = (arr.Value [0] as ErlAtom).Value;
//			ErlArray info = arr.Value [1] as ErlArray;
//			prase (type, info);
//		}
		parseKVMsg (message);
		callback ();
	}
	
	//解析单个仓库数据
	private void prase (string type, ErlArray arr)
	{ 
		StorageManagerment.Instance.updateStorageInfo (type, arr);
	}
	
} 

