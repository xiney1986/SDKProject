using UnityEngine;
using System.Collections.Generic;

public class ResourceConfigmanager : ConfigManager {

	private static ResourceConfigmanager instance;
	public List<string> cacheResource;//缓冲初始资源集合
	public List<string> effectResource;//特效缓冲始资源集合
    public List<string> loginResource;//登陆时需要加载的资源
    private int resourceIndex;
	public static ResourceConfigmanager Instance {
		get {
			if (instance == null)
				instance = new ResourceConfigmanager ();
			return instance;
		}
	}
	
	public ResourceConfigmanager ()
	{
		TextAsset textAsset=Resources.Load("resourceConfig") as TextAsset;
		resolveConfig(textAsset.text);
	}

	public override void parseConfig (string str)
	{
		if(str=="@end"){
           resourceIndex++;
           return;
        }
        if (resourceIndex==0) {
            if (loginResource == null)
                loginResource = new List<string>();
                loginResource.Add(str);
        }
        else if (resourceIndex==1)
		{
			if (cacheResource == null)
				cacheResource = new List<string> ();
			cacheResource.Add(str);
		}else if(resourceIndex==2){ 
			if (effectResource == null)
				effectResource = new List<string> ();
			effectResource.Add(str);
		}
	}
	public List<string> getCacheResource()
	{
		return cacheResource;
	}
	public List<string> getEffectResource()
	{
		return effectResource;
	}
    public List<string> getLoginResource() {
        return loginResource;
    }

}
