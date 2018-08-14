using UnityEngine;
using System.Collections;
//数量选择窗口的滑动数字变化控制器

public class NumerFlyCtrl : MonoBehaviour {
	
	public WindowBase fatherWindow;
	// Use this for initialization
	void OnDrag (Vector2 delta)
	{
		if(delta.y>0)
		{
			
			if(fatherWindow.GetType()==typeof(BuyWindow))
				(fatherWindow as BuyWindow).numberFly(true);

		}else
		{
			
			if(fatherWindow.GetType()==typeof(BuyWindow))
				(fatherWindow as BuyWindow).numberFly(false);

		}
	}
}
