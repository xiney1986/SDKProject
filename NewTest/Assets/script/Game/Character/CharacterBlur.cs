using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBlur : MonoBehaviour
{
	 
	UITexture tex;
	public UITexture[] blurs;

	CharacterCtrl ctrl;
	float timeCounter;

	public void init()
	{
		ctrl = GetComponent<CharacterCtrl> ();
	//	tex = ctrl.cardCtrl.tex;


		blurs = new UITexture[4];
		for (int i = 0; i < blurs.Length; i++) {
			UITexture t = NGUITools.AddChild<UITexture>(ctrl.transform.parent.gameObject);
			t.gameObject.layer = ctrl.gameObject.layer;
			t.mainTexture = ctrl.cardCtrl.cardPanel.renderer.material.mainTexture;
			t.material =new Material( ctrl.cardCtrl.cardPanel.renderer.material);
			t.width = 168;
			t.height = 168;
			t.transform.localPosition = ctrl.transform.localPosition;
			t.transform.localScale = ctrl.transform.localScale;
			t.transform.Rotate(new Vector3(90,0,0));
			t.depth =  i - 1;
			float alpha = (10 - i) / 3f;
			t.color = new Color(alpha,alpha,alpha,alpha * 1.2f);
			//t.color =Color.white;
			blurs[i] = t;
		}
	}

	
	// Update is called once per frame
	void Update ()
	{


	}

	public void Close()
	{
		foreach (UITexture t in blurs) {
			if(t==null || t.gameObject==null)
				continue;

			Destroy(t.gameObject);
		}
		Destroy (this);
	}
}
