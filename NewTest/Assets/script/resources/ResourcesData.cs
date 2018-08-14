using UnityEngine;
using System.Collections;

public enum ResourcesData_type
{
	AssetBundleRes,
	normalRes,	
}

public class ResourcesData
{
	public string ResourcesPath;//完整路径，包含了手机的系统路径
	public string ResourcesName;//相对路经
	public AssetBundle ResourcesBundle;//资源包
	public ResourcesData_type ResType = ResourcesData_type.normalRes;
	float _size;
	
	public float  size {
		get {
			return _size;
		}
		set {
			_size = (float)value / 1024f;
		}
	}


 
	public void LoadAll ()
	{
		if (ResType == ResourcesData_type.normalRes)
			return;
//		MemoryData = ResourcesBundle.LoadAll ();
	//	Debug.LogWarning(ResourcesName);
//		foreach(Object each in MemoryData)
//		{
//			if(each.name=="2050")
//				Debug.LogWarning("XXXXXXXXXXXXXXXXXXXXXXX");
//		}
	}
	

}
