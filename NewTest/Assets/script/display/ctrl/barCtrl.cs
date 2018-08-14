using UnityEngine;
using System.Collections;

//进度条控制器基类
public class barCtrl : MonoBase
{
	public UISprite FrontBar;
	public UISlider SliderBar;
	protected  float maxValue;
	protected  float newValue;
	protected UISprite sliderSprite;
//	public float minValue=0f;
	 bool isLock=false;//锁定后无法再用updateValue改变

	void Awake ()
	{

		if (SliderBar != null) {
			sliderSprite = SliderBar.foregroundWidget as UISprite;
		}
		init();
	}
	public virtual void  init()
	{

	}

	void Update ()
	{
		updateBar ();
		updateColor ();
	}
	public void lockBar()
	{
		isLock=true;
	}
	public virtual void  updateValue (float now, float max)
	{
		if(isLock==true)
			return;

		maxValue = max;
		newValue = now;
	}
	
	public virtual void  changeData (float data)
	{
		newValue = data;
	}
	public float  getNewValue(){
		return newValue;
	}

	public float getMaxValue(){
		return maxValue;
	}

	public void reset ()
	{
		maxValue=0;
		newValue=0;
		if (FrontBar != null) {
			FrontBar.fillAmount =0;
		}else if(SliderBar!=null){
			SliderBar.value=0;
		}
	}

	protected virtual void updateBar ()
	{
		if (maxValue == 0)
			return;
		
		float newData = newValue / maxValue;
		if (FrontBar != null) {
			if (newData != FrontBar.fillAmount) { 
				FrontBar.fillAmount = Mathf.Lerp (FrontBar.fillAmount, newData, Time.deltaTime * 6); 
			} 
		}
		
		if (SliderBar != null) {
			if (newData != SliderBar.value) { 
				SliderBar.value = Mathf.Lerp (SliderBar.value, newData, Time.deltaTime * 6); 
			} 
		}	
	}
 
	protected virtual void updateColor (){

	}
	
	
}
