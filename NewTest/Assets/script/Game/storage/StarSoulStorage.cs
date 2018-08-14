using System;
using System.Collections;

/// <summary>
/// 星魂仓库
/// </summary>
public class StarSoulStorage : Storage {

	/* fields */

	/* methods */
	public StarSoulStorage () {
		
	}
	public override void parse (ErlArray arr) {
		ErlArray ea1 = arr.Value [1] as ErlArray;
		if (ea1.Value.Length <= 0) {
			init (StringKit.toInt (arr.Value [0].getValueString ()), null);
		} else {
			ArrayList al = new ArrayList (); 
			StarSoul starSoul;
			for (int i = 0; i < ea1.Value.Length; i++) { 
				starSoul = StarSoulManager.Instance.createStarSoul ();
				starSoul.bytesRead (0, ea1.Value [i] as ErlArray);
				al.Add (starSoul);
			}  
			init (StringKit.toInt (arr.Value [0].getValueString ()), al);
		}
	}
}
