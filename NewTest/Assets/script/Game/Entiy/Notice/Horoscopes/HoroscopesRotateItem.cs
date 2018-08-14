using UnityEngine;
using System.Collections;

public class HoroscopesRotateItem : MonoBehaviour {

	public ChooseHoroscopesWindow fawin;

	void OnTriggerEnter(Collider other) {
		//是否需要更新
//		if(fawin.getStarType()!= StringKit.toInt(other.name))
		fawin.updateUI(StringKit.toInt(other.name));
	}

	void OnTriggerExit( Collider other ) {
	}
}
