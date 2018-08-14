using UnityEngine;
using System.Collections;

public class EmptyWindow : WindowBase {

	public override void OnAwake ()
	{
		base.OnAwake ();
		UiManager.Instance.emptyWindow=this;
	}
	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume () {
		base.OnNetResume();
		finishWindow();
	}
}
