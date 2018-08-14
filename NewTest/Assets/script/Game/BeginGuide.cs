using UnityEngine;
using System.Collections;

public class BeginGuide : MonoBehaviour
{
//	public Texture firstText;
	public Texture cacheText;
	public GUIText progress;
    public GUIText text1;
    public GUITexture text;
	public GUITexture  backGround;
	
	// Use this for initialization
	void Start ()
	{

		progress.pixelOffset= new Vector2(Screen.width*0.5f,Screen.height*0.1f);
        text1.pixelOffset = new Vector2(progress.pixelOffset.x, progress.pixelOffset.y-25);
        //progress.fontSize = 22 * Screen.width / 640;
        int by = ((float)Screen.width / (float)Screen.height) > 0.667f ? 1: 0;

		float screenScaleX = Screen.width / 640f;
		float screenScaleY = Screen.height / 960f;
		float sizeX=640;
		float sizeY=960;
		if(by==1){
			 sizeX=640*screenScaleY;
			 sizeY= 960*screenScaleY;
		}else{
			sizeX=640*screenScaleX;
			sizeY= 960*screenScaleX;
		}
			backGround.pixelInset = new Rect ((Screen.width-sizeX)*0.5f,(Screen.height-sizeY)*0.5f,sizeX,sizeY);	

	} 
	

	public	void showCacheText()
	{
		text.texture=cacheText;
		text.pixelInset=new Rect(Screen.width*0.5f-125,Screen.height*0.2f,255,37);
	}
}
