using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperDrawManagerment {

	public SuperDraw superDraw;
	public SuperDrawAudio audio;
	public bool isAudio = false;
	public int  propSid;

	public SuperDrawManagerment ()
	{
		propSid = CommonConfigSampleManager.Instance.getSampleBySid<SuperDrawMaxSample>(CommonConfigSampleManager.SuperDraw_SID).prizeSid;
//		if(superDraw==null)
//			superDraw = new SuperDraw ();
		if(audio==null)
			audio = new SuperDrawAudio ();
	}
	//单例
	public static SuperDrawManagerment Instance {

		get{ return SingleManager.Instance.getObj ("SuperDrawManagerment") as SuperDrawManagerment;}
	}

	/// <summary>
	/// 获取道具的总量
	/// </summary>
	public int getPropSumBySid(int sid)
	{
		
		Prop s;
		int num = 0;
		ArrayList list = StorageManagerment.Instance.getPropsBySid(sid);
		for(int j=0;j<list.Count;j++)
		{
			s = list[j] as Prop;
			num = s.getNum();
		}
		return num;
		
	}
}

