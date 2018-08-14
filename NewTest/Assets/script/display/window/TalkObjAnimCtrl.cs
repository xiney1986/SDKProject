using UnityEngine;
using System.Collections;

public class TalkObjAnimCtrl : ObjAnimCtrl
{

	public Transform pos;
 
	public void ObjAnimBegin ()
	{
		//in..
 
		if (AnimType == ObjAnimType.LeftIn)
			iTween.ValueTo (gameObject, iTween.Hash ("delay", delay, "from", transform.localPosition, "to", pos.localPosition, "onupdate", "move", "oncomplete", "animOver", "easetype", "easeOutQuad", "time", 0.2f));			
		
		if (AnimType == ObjAnimType.RightIn)
			iTween.ValueTo (gameObject, iTween.Hash ("delay", delay, "from", transform.localPosition, "to", pos.localPosition, "onupdate", "move", "oncomplete", "animOver", "easetype", "easeOutQuad", "time", 0.2f));			
		
		
		
		///out 
		
 
		if (AnimType == ObjAnimType.LeftOut)
			iTween.ValueTo (gameObject, iTween.Hash ("delay", delay, "from", transform.localPosition, "to", pos.localPosition - new Vector3 (offset, 0f, 0.0f), "onupdate", "move", "oncomplete", "animOver", "easetype", "easeOutQuad", "time", 0.2f));			
		
		if (AnimType == ObjAnimType.RightOut)
			iTween.ValueTo (gameObject, iTween.Hash ("delay", delay, "from", transform.localPosition, "to", pos.localPosition - new Vector3 (-offset, 0f, 0.0f), "onupdate", "move", "oncomplete", "animOver", "easetype", "easeOutQuad", "time", 0.2f));				
		
		
	}

	void move (Vector3 _pos)
	{
		transform.localPosition = _pos;
	}
	 	
	protected	override void animOver ()
	{
		base.animOver ();
		
		
	}
	
}
