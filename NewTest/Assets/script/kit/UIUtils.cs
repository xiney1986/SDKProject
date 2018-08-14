using UnityEngine;
using System.Collections;

public class UIUtils
{

	public static void M_addChild(GameObject _parent,GameObject _child)
	{
		M_addChild(_parent.transform,_child.transform,true);
	}
	public static void M_addChild(GameObject _parent,Transform _child)
	{
		M_addChild(_parent.transform,_child,true);
	}


	public static void M_addChild(Transform _parent,GameObject _child)
	{
		M_addChild(_parent,_child.transform,true);
	}

	public static void M_addChild(Transform _parent,Transform _child)
	{
		M_addChild(_parent,_child,true);
	}


	public static void M_addChild(Transform _parent,Transform _child,bool _resetChild)
	{
		_child.parent=_parent;
		if(_resetChild)
		{
			_child.localPosition=Vector3.zero;
			_child.localScale=Vector3.one;
			_child.localRotation=Quaternion.Euler(Vector3.zero);
		}
	}

	/// <summary>
	/// M_adds the child.
	/// </summary>
	/// <param name="_parent">_parent.</param>
	/// <param name="_child">_child.</param>
	/// <param name="_position">_position.</param>
	/// <param name="_isLocalPosition">If set to <c>true</c> _is local position.</param>
	/// <param name="_resetScale">If set to <c>true</c> _reset scale.</param>
	public static void M_addChild(GameObject _parent,GameObject _child,Vector3 _position,bool _isLocalPosition,bool _resetScale)
	{
		Transform childTransform=_child.transform;
		childTransform.parent=_parent.transform;
		if(_isLocalPosition)
		{
			childTransform.localPosition=_position;
		}else
		{
			childTransform.position=_position;
		}
		if(_resetScale)
		{
			_child.transform.localScale=Vector3.one;
		}
		_child.transform.localRotation=Quaternion.Euler(Vector3.zero);
	}

	public static void M_removeAllChildren(Transform _parent)
	{
		int childCount=_parent.childCount;
		for(int i=childCount-1;i>=0;i--)
		{
			MonoBehaviour.DestroyImmediate(_parent.GetChild(i).gameObject);
		}
	}
	public static void M_removeAllChildren(GameObject _parent)
	{
		M_removeAllChildren(_parent.transform);
	}
	public static void M_trace(params object[] _parameters)
	{
		foreach(object item in _parameters)
		{
			MonoBase.print("==:"+item.ToString());
		}
	}
}

