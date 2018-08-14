using UnityEngine;
using System.Collections;

public class FuBenCardCtrl : MonoBase
{ 
	public Animation anim;
	public GameObject Shadows;

	void Start ()
	{

		AnimationEvent _event = new AnimationEvent ();//定义动画事件
		_event.time = anim.GetClip ("idle").length - 0.2f;//事件触发时间
		_event.functionName = "playStand";//事件回调方法
		anim.GetClip ("idle").AddEvent (_event);//添加此动画时间到动画剪辑中

		AnimationClip midleClip=anim.GetClip ("midle");
		if(midleClip!=null) {
			_event = new AnimationEvent ();//定义动画事件
			_event.time = midleClip.length - 0.2f;//事件触发时间
			_event.functionName = "playMStand";//事件回调方法
			midleClip.AddEvent (_event);//添加此动画时间到动画剪辑中
		}

		_event = new AnimationEvent ();//定义动画事件
		_event.time = anim.GetClip ("happy").length - 0.2f;//事件触发时间
		_event.functionName = "playStand";//事件回调方法
		anim.GetClip ("happy").AddEvent (_event);//添加此动画时间到动画剪辑中

		AnimationClip mhappyClip=anim.GetClip ("mhappy");
		if(mhappyClip!=null) {
			_event = new AnimationEvent ();//定义动画事件
			_event.time = mhappyClip.length - 0.2f;//事件触发时间
			_event.functionName = "playMStand";//事件回调方法
			mhappyClip.AddEvent (_event);//添加此动画时间到动画剪辑中
		}

		_event = new AnimationEvent ();//定义动画事件
		AnimationClip failClip=anim.GetClip ("fail");
		if(failClip!=null) {
			_event.time = failClip.length - 0.1f;//事件触发时间
			_event.functionName = "playFailLoop";//事件回调方法
			failClip.AddEvent (_event);//添加此动画时间到动画剪辑中
		}
	}

	public void playFail ()
	{
		anim.Play ("fail");
	}

	public void playCycing ()
	{
		anim.Play ("cycing");
	}

	 void playFailLoop ()
	{
		anim.Play ("failLoop");
	}

	/// <summary>
	/// 骑乘坐骑跑
	/// </summary>
	public void playMMove ()
	{
		anim.Stop ();
		anim.Play ("mrun");
		AudioManager.Instance.PlayAudio (137);
	}

	public void playMove ()
	{
		anim.Stop ();
		anim.Play ("run");
		AudioManager.Instance.PlayAudio (137);
	}
	/// <summary>
	/// 骑乘坐骑标准姿势
	/// </summary>
	public void playMStand ()
	{ 
		anim.CrossFade ("mstand");
	}
	public void playStand ()
	{ 
		anim.CrossFade ("stand");
	}
	/// <summary>
	/// 骑乘坐骑休息
	/// </summary>
	public void playMIdle ()
	{
		anim.CrossFade ("midle");
	}
	public void playIdle ()
	{
		anim.CrossFade ("idle");
	}
	/// <summary>
	/// 骑乘坐骑happy
	/// </summary>
	public void playMHappy ()
	{
		anim.CrossFade ("mhappy");
	}
	public void playHappy ()
	{
		anim.CrossFade ("happy");
	}
	/// <summary>
	/// 设置控制器阴影是否激活
	/// </summary>
	public void setShadowsActive(bool isActive) {
		if(Shadows==null) return;
		Shadows.SetActive(isActive);
	}
}
