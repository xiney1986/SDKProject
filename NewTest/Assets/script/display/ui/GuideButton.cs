using UnityEngine;
using System.Collections;

/**
 * 新手指引按钮
 * @author 汤琦
 * */
public class GuideButton : MonoBehaviour
{
	private GameObject target;//执行对象
	private string functionName;//执行对象上绑定的方法
	public UISprite indicateIcon;//指示图标
	private CallBack callback;
	private bool isOnce = false;
	
	void OnClick () 
	{
		if(isOnce)
		{
			GuideManager.Instance.hideGuideUI();
			isOnce = false;
			if(callback != null)
				callback();
			Send();
			return;
		}
		if(callback != null)
		{

            if (!gameObject.name.StartsWith("collider")) this.gameObject.SetActive(false);
			GuideManager.Instance.doGuide(); 
			callback();
			Send();
		}


	}
	public void onceGuide(bool isOnce)
	{
		this.isOnce = isOnce;
	}
	
	public void initCallBack(CallBack _callback)
	{
		this.callback = _callback;
	}
	
	//初始化
	public void initInfo(GameObject _target,string _functionName)
	{
		this.target = _target;
		this.functionName = _functionName;
	}
	
	public void clearInfo()
	{
		this.target = null;
		this.functionName = "";
	}
	
	//执行
	void Send ()
	{
		if (string.IsNullOrEmpty(functionName)) 
			return;
		if (target == null) 
			target = gameObject;
		else
		{
			target.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
