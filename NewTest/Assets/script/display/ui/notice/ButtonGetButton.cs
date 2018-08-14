using UnityEngine;
using System.Collections;

public class ButtonGetButton : ButtonBase {

	public InviteContent invit;
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		invit.buttonEventBase();
	}
}
