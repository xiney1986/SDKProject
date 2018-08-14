
using System;
using UnityEngine;

public class ButtonHoroscopesPray:ButtonBase
{
	[HideInInspector]
	public HoroscopesPrayContent
		content;
	[HideInInspector]
	public CallBack<int>
		callback;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		StarPrayFPort sp = FPortManager.Instance.getFPort ("StarPrayFPort") as StarPrayFPort;
		sp.access (callback);
	}
}

