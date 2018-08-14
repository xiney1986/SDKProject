using UnityEngine;
using System.Collections;

public class CameraCloserCtrl : MonoBehaviour
{
		
	private float srcFieldView; 
	private Camera tempCamera;
	public float closerFieldView;
	void Start ()
	{
		tempCamera=camera;
		srcFieldView=tempCamera.fieldOfView;
	}
	/// <summary>
	/// 缩小
	/// </summary>
	/// <param name="immediately">If set to <c>true</c> immediately.</param>
	public void M_zoomOut(bool immediately)
	{
		return;
		if(tempCamera.fieldOfView==srcFieldView)
			return;
		if(immediately)
		{
			tempCamera.fieldOfView=srcFieldView;
		}else
		{
			iTween.ValueTo(gameObject,iTween.Hash("from",closerFieldView,"to",srcFieldView,"onupdate","M_updateValue","time",0.25f));
		}
	}
	/// <summary>
	/// 放大
	/// </summary>
	public void M_zoomIn()
	{
		return;
		if(tempCamera.fieldOfView==closerFieldView)
			return;
		iTween.ValueTo(gameObject,iTween.Hash("from",srcFieldView,"to",closerFieldView,"onupdate","M_updateValue","time",0.4f));
	}
	private void M_updateValue(float _value)
	{
		tempCamera.fieldOfView=_value;
	}
		                                     

}

