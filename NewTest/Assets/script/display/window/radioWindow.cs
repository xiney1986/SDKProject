using UnityEngine;
using System.Collections;

public class radioWindow : WindowBase
{
	public UILabel message;
	public UISprite bg;
	float life = 10f;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void readMessage (string str)
	{
		message.text = str;
		bg.height = message.height;
		life = 10f;
	}
	
	void Update ()
	{
		life -= Time.deltaTime;
		
		if (life <= 0) {
			hideWindow ();
		}
	}
}
