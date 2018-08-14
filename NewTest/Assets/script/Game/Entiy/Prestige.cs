using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Prestige  {
	 long exp;
	public long expNow;
	public long expDown;
	public long expUp;
	public int level;
	PrestigeSample sample;
	public List<PrestigeEffectValue> effects{
		get{

			return sample.prestigeEffect;
		}

	}

	public string prestigeName{
		get{
			return sample.prestigeName;
		}
	}

	public int  sid{
		get{
			return sample.sid;
		}
	}

	public string  iconID{
		get{
			return sample.iconID;
		}
	}
	public Prestige(long exp){

		this.exp=exp;

		level=EXPSampleManager.Instance.getLevel (PrestigeManagerment.EXPSID,exp);
		sample=PrestigeConfigManagerment.Instance.getPrestigeSampleByLevel(level);
		expDown=EXPSampleManager.Instance.getEXPDown(PrestigeManagerment.EXPSID,level);
		expUp=EXPSampleManager.Instance.getEXPUp(PrestigeManagerment.EXPSID,level);
		expNow=expUp-exp;
	}




}
