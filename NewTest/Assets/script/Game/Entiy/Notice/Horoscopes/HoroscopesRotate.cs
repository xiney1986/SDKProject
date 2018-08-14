using UnityEngine;
using System.Collections;

public class HoroscopesRotate : MonoBehaviour
{
	public GameObject sprite; //选择星座的旋转图片
    public ChooseHoroscopesWindow fawin;

	private Rigidbody obj;
	private Vector3 Torque;
//	private bool isOnDrag = false;
	public UIPanel panel;

	void Update()
	{
		float a = (panel.clipOffset.x/100) - 3;
		//a-3是因为白羊为1，但是出图的位置在-60度
		sprite.transform.eulerAngles=new Vector3(0,0,a*30);
	}
//	void OnDrag (Vector2 delta)
//	{
//		Torque = new Vector3(0,0,-delta.x);
//	}

//	void OnPress (bool isDown)
//	{
//		isOnDrag = isDown;
//	}

	public void initItem(int type)
	{
		panel.clipOffset = new Vector2(100*type ,panel.clipOffset.y);
//		sprite.transform.Rotate(0,0,30*type);
//		obj = sprite.GetComponent<Rigidbody>();
	}

//	void FixedUpdate() {
//		if(isOnDrag && obj!=null)
//			obj.AddRelativeTorque(Torque);
//	}

//	public bool getOnPress()
//	{
//		return isOnDrag;
//	}


}
