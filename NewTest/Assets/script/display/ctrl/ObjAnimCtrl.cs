using UnityEngine;
using System.Collections;

public enum ObjAnimType
{
	LeftIn,
	RightIn,	
	BottomIn,
	TopIn,
	LeftOut,
	RightOut,	
	BottomOut,
	TopOut,
}

public class ObjAnimCtrl : MonoBase
{
	public	float delay;
	public	float time = 0.2f;
	public	float offset = 0.5f;
	public ObjAnimType AnimType;


	
	public  virtual void ObjAnimBegin ()
	{

		//in..
		if (AnimType == ObjAnimType.BottomIn)
			iTween.MoveFrom (gameObject, iTween.Hash ("delay", delay, "position", transform.position + new Vector3 (0, -offset, 0.0f), "oncomplete", "animOver", "easetype", "easeOutQuad", "time", time));	
		
		if (AnimType == ObjAnimType.TopIn)
			iTween.MoveFrom (gameObject, iTween.Hash ("delay", delay, "position", transform.position + new Vector3 (0, offset, 0.0f), "oncomplete", "animOver", "easetype", "easeOutQuad", "time", time));		
		
		if (AnimType == ObjAnimType.LeftIn)
			iTween.MoveFrom (gameObject, iTween.Hash ("delay", delay, "position", transform.position + new Vector3 (-offset, 0f, 0.0f), "oncomplete", "animOver", "easetype", "easeOutQuad", "time", time));			
		
		if (AnimType == ObjAnimType.RightIn)
			iTween.MoveFrom (gameObject, iTween.Hash ("delay", delay, "position", transform.position + new Vector3 (offset, 0f, 0.0f), "oncomplete", "animOver", "easetype", "easeOutQuad", "time", time));			
		
		
		
		///out 
		
		if (AnimType == ObjAnimType.BottomOut)
			iTween.MoveTo (gameObject, iTween.Hash ("delay", delay, "position", transform.position + new Vector3 (0, offset, 0.0f), "oncomplete", "AnimContinueOver", "easetype", "easeOutQuad", "time", time));	
		
		if (AnimType == ObjAnimType.TopOut)
			iTween.MoveTo (gameObject, iTween.Hash ("delay", delay, "position", transform.position + new Vector3 (0, -offset, 0.0f), "oncomplete", "AnimContinueOver", "easetype", "easeOutQuad", "time", time));		
		
		if (AnimType == ObjAnimType.LeftOut)
			iTween.MoveTo (gameObject, iTween.Hash ("delay", delay, "position", transform.position + new Vector3 (offset, 0f, 0.0f), "oncomplete", "AnimContinueOver", "easetype", "easeOutQuad", "time", time));			
		
		if (AnimType == ObjAnimType.RightOut)
			iTween.MoveTo (gameObject, iTween.Hash ("delay", delay, "position", transform.position + new Vector3 (-offset, 0f, 0.0f), "oncomplete", "AnimContinueOver", "easetype", "easeOutQuad", "time", time));				
		
		
	}

	protected	virtual void animOver ()
	{
		enabled = false;
	}

	void Update ()
	{
 
	}
 
}
