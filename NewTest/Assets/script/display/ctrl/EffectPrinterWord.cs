using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel))]
public class EffectPrinterWord : MonoBehaviour
{

	private UILabel label;
	void Awake ()
	{
		//label=GetComponent<UILabel>();
	}
	private string mText;
	public string text
	{
		set
		{
			mText=value;
			M_playEffect();
		}get
		{
			return mText;
		}
	}
	private void M_playEffect()
	{
		if(label==null)
		{
			label=GetComponent<UILabel>();
		}
		if(label==null)
		{
			return;
		}

		StopAllCoroutines();
		label.text=string.Empty;
		StartCoroutine("M_render");
	}
	private IEnumerator M_render()
	{
		string str=string.Empty;
		bool isPrint=true;
		foreach (char item in mText) {
			if (item == '[') {
				isPrint = false;
			} else if (item == ']') {
				isPrint = true;				
			} 
			if (isPrint) {
				label.text += str + item.ToString ();
				str = string.Empty;
				yield return new WaitForSeconds (0.055f);
			} else {
				str += item;
			}
		}
	}
}

