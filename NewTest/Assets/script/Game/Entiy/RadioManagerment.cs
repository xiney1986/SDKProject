using System;
using System.Collections.Generic;
 
/**
 * 广播管理器
 * @author longlingquan
 * */
public class RadioManagerment
{
	private List<string> list;

	public RadioManagerment ()
	{ 

	}
	 
	public static RadioManagerment Instance {
		get{return SingleManager.Instance.getObj("RadioManagerment") as RadioManagerment;}
	}
	
	public void playRadio (string str)
	{
		if (UiManager.Instance.mainWindow != null) {
			return;
		}
	}
	
} 

