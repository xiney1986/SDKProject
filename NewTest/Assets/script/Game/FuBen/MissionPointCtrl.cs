using UnityEngine;
using System.Collections;

/// <summary>
/// 点位控制
/// </summary>
public class MissionPointCtrl : MonoBase {

 public MissionPoint info;
 public bool actived=false;	
 

	
public void ScaleUp()
	{
		actived=true;
		iTween.ScaleTo (gameObject, iTween.Hash ("delay",0.4f,"scale", new Vector3(1,1,1), "easetype",  iTween.EaseType.easeOutElastic, "time", 0.6f));		
	}
	
public void ScaleDown()
	{
		actived=false;
		iTween.ScaleTo (gameObject, iTween.Hash ("scale", new Vector3(1,1,1), "easetype",  iTween.EaseType.easeOutElastic, "oncomplete", "ScaleAnimComplete", "time", 0.6f));		
	}
	
 
}
