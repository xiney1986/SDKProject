using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelegatedynamicContent : dynamicContent
{

	RCallBack<GameObject,GameObject,int> onUpdateItem;
	RCallBack<GameObject,int> onInitItem;
	public	CallBack<int> OnBorderCallBack;

	public void SetUpdateItemCallback (RCallBack<GameObject,GameObject,int> onUpdateItemCallback)
	{
		this.onUpdateItem = onUpdateItemCallback;
	}

	public void SetinitCallback (RCallBack<GameObject,int> callback)
	{
		this.onInitItem = callback;
	}

	public override void initButton (int i)
	{
		//回调不空,节点是空才创建
		if (onInitItem != null && nodeList [i] == null) {
			nodeList [i] = onInitItem (i);
		}
	}

	public override void OnBorder (int state)
	{
		if (OnBorderCallBack != null) {
			OnBorderCallBack (state);
		}
	}

	public override void updateItem (GameObject item, int index)
	{
		if (onUpdateItem != null) {
			onUpdateItem (item, index);
		}
	}
}
