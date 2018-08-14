using UnityEngine;
using System.Collections;

public class NoticetHeroEatInfoFPort : BaseFPort
{

	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_HERO_EAT_INFO);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		if (parseKVMsg (message) && callback != null)
			callback ();
	}
	//解析ErlKVMessgae
	public bool parseKVMsg (ErlKVMessage message)
	{
		ErlArray info = message.getValue ("msg") as ErlArray;
		if (info != null) {
			if (info.Value.Length == 4) {
				int[] array = NoticeManagerment.Instance.getHeroEatInfo ();
				array = new int[info.Value.Length];
				for (int i=0; i<array.Length; i++) {
					array [i] = StringKit.toInt ((info.Value [i] as ErlType).getValueString ());
				}
				NoticeManagerment.Instance.setHeroEatInfo (array);
			}
			return true;
		}
		return false;
	}
}
