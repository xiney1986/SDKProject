using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillManager
{  

	public static SkillManager Instance {
		get{return SingleManager.Instance.getObj("SkillManager") as SkillManager;}
	}
	 
	
	public SkillData CreateSkillData (int id, int sid)
	{ 
		SkillData _data = new SkillData ();
		_data.sample = SkillManagerment.Instance.createSkill (sid);
		_data.id = id;
		return _data; 
	}  
}
