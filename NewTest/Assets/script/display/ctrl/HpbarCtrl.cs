using UnityEngine;
using System.Collections;

/** 
  * 血条显示控制器 
  * @author 李程
  * */

public class HpbarCtrl : barCtrl
{


	// Use this for initialization
	CallBack deadCallback;
	public UISprite 	background;
	public GameObject 	barPanel;
	  Material 	bar_mat;
	float barMatValue;
	Color barMatColor;


	public override void init ()
	{
		base.init ();
		if (barPanel != null) {
			barPanel.renderer.material = new Material (barPanel.renderer.material);
			bar_mat=barPanel.renderer.material ;
			barMatValue = bar_mat.GetFloat ("_Value");
			barMatColor = bar_mat.GetColor ("_Color");
		}
	}

	public void setDeadCallBack (CallBack deadFunc)
	{
		deadCallback = deadFunc;
	}
	// Update is called once per frame
	protected	override  void updateBar ()
	{
		if (maxValue == 0)
			return;	

		float newData = newValue / maxValue;
		
		if (FrontBar != null) {		
			
			if (newData != FrontBar.fillAmount) { 
				FrontBar.fillAmount = Mathf.Lerp (FrontBar.fillAmount, newData, Time.deltaTime * 6  ); 
			} 
			//if very close,don't do Lerp any more; 
			if (Mathf.Abs (newData - FrontBar.fillAmount) < 0.05f)
				FrontBar.fillAmount = newData;
		 
			if (FrontBar.fillAmount == 0 && deadCallback != null) {
				deadCallback ();
			}
			
		} else if (SliderBar != null) {
			
			if (newData != SliderBar.value) { 
				SliderBar.value = Mathf.Lerp (SliderBar.value, newData, Time.deltaTime * 6  ); 
			} 
			//if very close,don't do Lerp any more; 
			if (Mathf.Abs (newData - SliderBar.value) < 0.05f)
				SliderBar.value = newData;
		 
			if (SliderBar.value == 0 && deadCallback != null) {
				//character dead
				deadCallback ();
			}
			
		} else if (bar_mat != null) {

			//if very close,don't do Lerp any more; 
			if (Mathf.Abs (newData - barMatValue) < 0.005f)
				barMatValue = newData;

			if (newData != barMatValue) { 
				barMatValue=Mathf.Lerp (barMatValue, newData, Time.deltaTime * 10);
				bar_mat.SetFloat ("_Value", barMatValue);
			} 

			
			if (barMatValue == 0 && deadCallback != null) {
				//character dead
				deadCallback ();
			}
			
		}
	}
	
	protected override void updateColor ()
	{
 
//		if (FrontBar != null) {		
//			//change the hp bar color!
//		
//			FrontBar.color = new Color (1 - FrontBar.fillAmount, FrontBar.fillAmount, 0, 1);
//		
//
//		} else if (SliderBar != null) {		
//			//change the hp bar color!
//			if (SliderBar.value < 0.5f && sliderSprite .color != Color.red) {
//				sliderSprite.color = Color.red;
//			}
//			if (SliderBar.value > 0.5f && sliderSprite.color != Color.green) {
//				sliderSprite.color = Color.green;
//			}  
//		} else if (bar_mat != null) {		
//
//
//			//change the hp bar color!
//			if (barMatValue < 0.5f && barMatColor != Color.red) {
//				barMatColor = Color.red;
//			}
//			if (barMatValue > 0.5f && barMatColor != Color.green) {
//				barMatColor = Color.green;
//			}  
//
//			bar_mat.SetColor ("_Color", barMatColor);
//		}
		
		
	}
}
