using UnityEngine;
using System.Collections;

public class AutoGoLabelCtrl : MonoBehaviour
{

	public UILabel[] AutoGo;
	private int state;

	public void change ()
	{
		if (AutoGo != null) {
			UILabel label;
			Vector3 v3;
			for (int i=0; i<AutoGo.Length;) {
				label = AutoGo [i];
				v3 = label.transform.position;
				if (state % 2 != 0) {
					v3 += new Vector3 (0, 10, 0);
				} else {
					v3 += new Vector3 (0, -10, 0);
				}
				i += 2;
			}
			state++;
		}
	}

	public void Update()
	{


		if (AutoGo != null) {
			UILabel label;
			Vector3 v3;
			for (int i=0; i<AutoGo.Length;) {
				label = AutoGo [i];
				v3 = label.transform.localPosition;
				if (state % 2 != 0) {
					label.transform.localPosition = new Vector3(v3.x,v3.y+10,v3.z);
					v3 += new Vector3 (0, 10, 0);
				} else {
					label.transform.localPosition = new Vector3(v3.x,v3.y-10,v3.z);
					v3 += new Vector3 (0, -10, 0);
				} 
				i += 2;
			}
			state++;
		}


//	 	Mathf.Sin (Time.realtimeSinceStartup)*30;


//		AutoGo [0].transform.localPosition = new Vector3 (AutoGo [0].transform.localPosition.x,y,AutoGo [0].transform.localPosition.z);

	}

}
