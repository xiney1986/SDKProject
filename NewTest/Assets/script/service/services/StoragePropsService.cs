using UnityEngine;
using System.Collections;

/**
 * 账号服务
 * @author huangzhenghan
 * */
public class StoragePropsService:BaseFPort
{
	public StoragePropsService ()
	{
		
	}

	public override void read (ErlKVMessage message)
	{ 
		//优先删除，否则增加其他物品会出现顺序不一致
		ErlArray delProps = message.getValue ("del_props") as ErlArray;
		if (delProps != null && delProps.Value.Length > 0) {
			StorageManagerment.Instance.parseDelStorageProps (delProps);
		}
		ErlArray addProps = message.getValue ("add_props") as ErlArray;
		if (addProps != null && addProps.Value.Length > 0) {
			StorageManagerment.Instance.parseAddStorageProps (addProps);
		}
		ErlArray updateProps = message.getValue ("update_props") as ErlArray;
		if (updateProps != null && updateProps.Value.Length > 0) {
			StorageManagerment.Instance.parseUpdateStorageProps (updateProps);
		}
	}
}

