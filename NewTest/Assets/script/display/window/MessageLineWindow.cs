using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class MessageLineWindow : WindowBase
{
	List<string> msgList;
	public msgLineCtrl sample;
	int nextLine;//下个横幅位置
	bool started = false;
	bool isUnLockUi=true;

	protected override void begin ()
	{
		base.begin ();
		if(isUnLockUi)
			MaskWindow.UnlockUI ();
	}
	
	public void readOne ()
	{
		if (msgList == null)
			return;
		
		if (msgList.Count == 0) {
//			nextLine = 0;
			started = false;
			return;
		
		}
		
		foreach (string  each in msgList) {
			if (string.IsNullOrEmpty (each) == false) {
				
				if (started == false) 
					started = true;
				GameObject newline = Instantiate (sample.gameObject) as GameObject;
				newline.transform.parent = sample.transform.parent;
				newline.transform.localPosition = sample.transform.localPosition;
				newline.transform.localScale = Vector3.one;
				newline.GetComponent<msgLineCtrl> ().Initialize (this, nextLine, each);
				newline.SetActive (true);
				msgList.Remove (each);
				nextLine -= 50;
				if (nextLine <= -300) {
					nextLine = 0;
				}
				return;	
			}
		}
		
	}

	public void clear ()
	{
		started = false;

		if (sample == null)
			return;

		foreach (Transform each in sample.transform.parent) {
			if (each.GetInstanceID () != sample.transform.GetInstanceID ()) {


				Destroy (each.gameObject);


			}
		}
		if (msgList != null)
			msgList.Clear ();

	}
	public void  Initialize (string message)
	{
		Initialize (message,true);
	}
	public void Initialize (string[] message)
	{
		for(int i=0;i<message.Length;i++)
		{
			Initialize(message[i], false);
		}
	}
	public void  Initialize (string message,bool isUnLockUi)
	{
		this.isUnLockUi = isUnLockUi;
		if(!isUnLockUi)
			dialogCloseUnlockUI=false;
		if (msgList == null)
			msgList = new List<string> ();
		
		msgList.Add (message);
		if (started == false)
			readOne ();
		
	}
		
}
