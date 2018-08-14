using UnityEngine;
using System.Collections;

/**阵形数据类
  *@author 汤琦
  **/
public class Formation
{ 
	Vector3[] pos;
	private string formationTypeName;
	int pointCount = 0;
	string forObjectName;
	//加载点位
	public void loadPoint (bool isPlayerSide )
	{
		pos = new Vector3[15];
		Transform root = null;
		root =FormationManagerment.Instance. setFormationType (isPlayerSide).transform;
		for (int i=0; i<  pos.Length; i++) {
			pos [i] = root.FindChild ((i + 1).ToString ()).position; 
		}
	}

	//获得某个点
	public Vector3 getPosition (int posIndex)
	{ 
		return pos [posIndex - 1]; 
	} 
 
	//获得点位数量
	public int getPointCount ()
	{
		return pointCount;
	}
}
