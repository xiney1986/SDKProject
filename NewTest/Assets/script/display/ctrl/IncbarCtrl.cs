using UnityEngine;
using System;
using System.Collections;

/** 
  * 增量显示条控制器 
  * */
public class IncbarCtrl : MonoBase
{

	/** 背景条 */
	public UISlider backBar;
	/** 增量条 */
	public barCtrl incBar;
	/** 前景条 */
	public barCtrl frontBar;
//	/** 上一次的滑动条值 */
//	float lastBarValue;
	/** 当前的滚动条值 */
	float currentBarValue;
	/** 数据是否发生变化 */
	bool isActive;
	/** 新滑动条值 */
	float now;
	/** 最大滑动条值 */
	float max;

	public void updateValue (float old,float inc, float max)
	{
		this.now = old+inc;
		this.max = max;
		frontBar.reset ();
		incBar.reset ();
		if(now!=incBar.getNewValue())
		{
			frontBar.updateValue (old,max);
			isActive = true;
		}
	}

	void Update ()
	{
		updateBar ();
	}

	private void updateBar(){
		if (!isActive)
			return;
		float incNewData=frontBar.getNewValue() / frontBar.getMaxValue();
		if (Mathf.Abs (incNewData - currentBarValue) < 0.005f){
			currentBarValue = incNewData;
			isActive=false;
			incBar.updateValue(now,max);
		}
		if (incNewData != currentBarValue) { 
			currentBarValue=Mathf.Lerp (currentBarValue, incNewData, Time.deltaTime * 10);
		}
	}
}
