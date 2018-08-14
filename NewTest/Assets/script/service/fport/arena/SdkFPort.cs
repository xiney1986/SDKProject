using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//SDK相关上传和下载

public class SdkFPort : BaseFPort
{
	CallBack<Dictionary<string, PlatFormUserInfo>> callback;
	int type;
	//向后台保存自己的sdk信息,便于其他玩家获取
	public void getSdkInfo (string uids, 	CallBack<Dictionary<string, PlatFormUserInfo>> cb)
	{   
		type=2;
		ErlKVMessage message = new ErlKVMessage (FrontPort.SDK_GET_ALL_INFO);
		message.addValue ("uids", new ErlString (uids));
		access (message);
		callback = cb;
	}

	public override void read (ErlKVMessage message)
	{ 
		if(type==1)
			return;

		ErlList list = message.getValue ("msg") as ErlList;

		Dictionary<string, PlatFormUserInfo> dictionary;
		dictionary=new Dictionary<string, PlatFormUserInfo>();

		for(int i=0;i<list.Value.Length;i++){
		ErlArray _array1 = list.Value [i] as ErlArray;

		string uid = _array1.Value [0].getValueString ();
		ErlList list2 = _array1.Value [1] as ErlList;
		PlatFormUserInfo info = new PlatFormUserInfo ("");

			if(list2==null || list2.Value==null || list2.Value.Length<2)
				continue;

		ErlArray _array2=list2.Value [0] as ErlArray;
		info.sex = _array2.Value[1].getValueString ();

		 _array2=list2.Value [1] as ErlArray;
		info.face =_array2.Value[1].getValueString ();

			dictionary.Add(uid,info);
		}
		if (callback != null) {
			callback (dictionary);
			callback = null;
		}
	}
}
