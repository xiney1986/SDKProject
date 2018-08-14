using System;
using System.Collections;

/**
 * 随机名管理器
 * @author 汤琦
 * */
public class RandomNameManagerment 
{

  
	public static RandomNameManagerment Instance {
		get{return SingleManager.Instance.getObj("RandomNameManagerment") as RandomNameManagerment;}
	}

	//获得随机名字
	public string getRandomName(bool isMan)
	{
		Random r1 = new Random();
		int firstIndex = r1.Next(0,RandomNameConfigManager.Instance.prefixNames.Length);

//		MonoBase.print("len="+RandomNameConfigManager.Instance.prefixNames.Length);

		string name;
		Random r2 = new Random();
		if(isMan)
		{
			int secondIndex = r2.Next(0,RandomNameConfigManager.Instance.manNames.Length);
			name=RandomNameConfigManager.Instance.prefixNames[firstIndex]+RandomNameConfigManager.Instance.manNames[secondIndex];

//			MonoBase.print("len="+RandomNameConfigManager.Instance.manNames.Length);
		}else
		{
			int secondIndex = r2.Next(0,RandomNameConfigManager.Instance.womanNames.Length);
			name=RandomNameConfigManager.Instance.prefixNames[firstIndex]+RandomNameConfigManager.Instance.womanNames[secondIndex];

//			MonoBase.print("len="+RandomNameConfigManager.Instance.womanNames.Length);
		}
		if(ShieldManagerment.Instance.isContainShield(name)) //如果有屏蔽字，重新随名字
		{
			return getRandomName(isMan);
		}else
		{
			return name;
		}
	}
	
	
}
