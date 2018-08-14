using UnityEngine;
using System.Collections;

public class worldArrowCtrl : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		AnimationEvent _event = new AnimationEvent ();//定义动画事件
		_event.time = animation.GetClip ("go").length-0.01f;//事件触发时间
		_event.functionName = "arrowChangeNormal";//事件回调方法
		animation.GetClip ("go").AddEvent (_event);//添加此动画时间到动画剪辑中

	}
	
	void arrowChangeNormal()
	{
		animation.Play("anim1");
	}
	public	void play()
	{
		animation.Play("go");
	}
}
