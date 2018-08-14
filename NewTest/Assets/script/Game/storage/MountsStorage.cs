using UnityEngine;
using System.Collections;

/**
 * 坐骑仓库
 * */
public class MountsStorage : Storage {

	public MountsStorage ()
	{
		
	}

	public override void parse (ErlArray arr) {
		ErlArray ea1 = arr.Value [1] as ErlArray;
		if (ea1.Value.Length <= 0) {
			init (StringKit.toInt (arr.Value [0].getValueString ()), null);
		} else {
			ArrayList al = new ArrayList (); 
			Mounts mounts;
			for (int i = 0; i < ea1.Value.Length; i++) { 
				mounts = MountsManagerment.Instance.createMounts ();
				mounts.bytesRead (0, ea1.Value [i] as ErlArray);
				al.Add (mounts);
			}  
			init (StringKit.toInt (arr.Value [0].getValueString ()), al);
		}
	}
}
