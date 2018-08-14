using UnityEngine;
using System.Collections;

public class AlternateSwitchCtrl : MonoBehaviour
{
	public Animation anim;
	public ChangeAlternateWindow fatherWindow;
	public UITexture main;
	public UITexture sub;
	public CallBack callBack;
 
	public void over ()
	{
		anim.Stop ();
		gameObject.SetActive (false);
			main.alpha=1;
			sub.alpha=1;	
		if (callBack != null)
			callBack ();
		
			
	}
	
	public void beginSwitch (Texture mainImage, Texture subImage, CallBack callBack)
	{
		this.callBack = callBack;
		gameObject.SetActive (true);
		
		main.mainTexture = mainImage;
		sub.mainTexture = subImage;
		
		if(main.mainTexture==null)
			main.alpha=0;
		if(sub.mainTexture==null)
			sub.alpha=0;		
		
		anim.Play ();

	}
}
