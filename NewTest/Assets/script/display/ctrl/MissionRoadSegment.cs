using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// 一个路线片段的组件 主要是用于指定该种类的路线的 点位集合，长度，类型（起始，中，左，右，结束）
/// </summary>
public class MissionRoadSegment : MonoBehaviour
{
	public EnumRoadSegment type;
	public float distance;
	public Transform[] pointTransforms;

	[HideInInspector]
	public int maxStep;

	[HideInInspector]
	public int totalSteps;

	public void M_init()
	{
		totalSteps=pointTransforms.Length; 
	}
	/// <summary>
	/// 返回该线路片段所有的点位[世界坐标]
	/// </summary>
	/// <returns>The points.</returns>
	public Vector3[] M_getPoints()
	{
		Vector3[] points=new Vector3[totalSteps];
		Transform[] childTransforms;
		int i,j,length;
		for(i=0;i<totalSteps;i++)
		{
			points[i]=pointTransforms[i].position;
		}
		return points;
	}

	////////////自动生成点
	//public float t;
	//[ContextMenu("tempTest")]
	//public void tempTest () {
	//	Transform[] trans	= GetComponentsInChildren<Transform>();
	//	float v				= renderer.bounds.size.x / trans.Length;
	//	Debug.Log (v);
	//	for (int i = 1; i < trans.Length; i++) {
	//		trans[i].name			= "Point_" + i.ToString();
	//		trans[i].localPosition = new Vector3 (0, 0, t * (i - 2));
	//		pointTransforms[i - 1]	= trans[i];
	//	}
	//}

}
/// <summary>
/// 枚举 路线片段的类型
/// </summary>
public enum EnumRoadSegment
{
	Start,
	Middle_Line,
	Middle_Left,
	Middle_Right,
	End
}
/// <summary>
/// 路线片段枚举 工具 主要是用于转换
/// </summary>
public class EnumRoadSegmentKit
{
	/// <summary>
	/// 把string转换对应的枚举
	/// </summary>
	/// <returns>The type.</returns>
	/// <param name="_type">_type.</param>
	public static EnumRoadSegment M_getType(string _type)
	{
		return  (EnumRoadSegment)Enum.Parse(typeof(EnumRoadSegment),_type);
	}
	/// <summary>
	/// 返回随机路段中段的类型
	/// </summary>
	/// <returns>The random middle random.</returns>
	public static EnumRoadSegment M_getRandomMiddleRandom()
	{
		int i=UnityEngine.Random.Range(1,4);
		if(i%3==0)
		{
			return EnumRoadSegment.Middle_Left;
		}else if(i%2==0)
		{
			return EnumRoadSegment.Middle_Right;
		}else
		{
			return EnumRoadSegment.Middle_Line;
		}
	}
}

