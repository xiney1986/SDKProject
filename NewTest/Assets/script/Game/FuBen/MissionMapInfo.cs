using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 副本图形点位数据集
/// </summary>
public class MissionMapInfo :object,IResUser
{

	//public Vector2 size;//地图大小，这个关系到点位的坐标计算
	MissionPointInfo[] pointData;
	public string packPath;//包路径
	public ResourcesData resourcesData;
	public List<MissionPoint> pointList;
	public Transform pointRoot;
	public Transform drawPoint;

	public MissionMapInfo ()
	{ 
		
	}

	public void clean ()
	{
		pointData = null;
		resourcesData = null;
		pointList = null;
		pointRoot = null;
		drawPoint = null;
	}
	/// <summary>
	/// 获得所有点位逻辑数据
	/// </summary>
	public MissionPointInfo[]  getAllPointData ()
	{
		return pointData;
	}

	public void setResourcesData (ResourcesData resData)
	{
		resourcesData = resData;
	}
	

	/// <summary>
	/// 获得玩家上一个点位的图像数据
	/// </summary>
	public MissionPoint getPlayerBeforePointInfo ()
	{
		if (MissionInfoManager.Instance.mission.getPlayerPointIndex() < 1)
			return null;
		return pointData [MissionInfoManager.Instance.mission.getPlayerPointIndex() - 1].PointInfo;
	}

	/// <summary>
	/// 得到玩家当前站的图形点位数据
	/// </summary>
	public MissionPoint getPlayerPointInfo ()
	{ 
		return pointData [MissionInfoManager.Instance.mission.getPlayerPointIndex()].PointInfo;
	}




	public MissionPoint getPlayerNextPointInfo (MissionPoint nowPoint)
	{ 
		return null;
	}




	/// <summary>
	/// 得到玩家下一个图像点位
	/// </summary>
	public MissionPoint getPlayerNextPointInfo ()
	{ 
		if(MissionInfoManager.Instance.mission.getPlayerPointIndex()+1>=pointData.Length)
			return null;

		return pointData [MissionInfoManager.Instance.mission.getPlayerPointIndex()+1].PointInfo;
	}


	public bool moveNextPoint ()
	{  
		if (MissionInfoManager.Instance.mission.getPlayerPointIndex() + 1 >= pointData.Length)
			return false; 
		//activationPoint += 1;
		return true; 
	}

	
	public void releaseResourcesData ()
	{ 
		resourcesData = null; 
	}

	/// <summary>
	/// 根据配置数据找到对应位置的显示对象数据
	/// </summary>
/*
	public MissionPoint getPointInfoByLoc (int[] loc)
	{
		foreach (MissionPoint info in pointList) {
			if (info.loc [0] == loc [0] && info.loc [1] == loc [1])
				return info;
		}
		return null;
	}
	*/

	public void createAllPointInfo ()
	{ 
		pointList = new List<MissionPoint> ();	
		
		pointData = MissionInfoManager.Instance.mission.points; 
		int mapId = MissionInfoManager.Instance.mission.getPlayerPoint ().getMapId ();
		//遍历所有id;
		for (int i=0; i< pointData.Length; i++) {  
			if (pointData [i].getMapId () != mapId)
				continue;

			//已经有该id，不再制作点位
			bool repy = false;
			foreach (MissionPoint info in pointList) {
				if (info.loc [0] == pointData [i].getPointLoction () [0] && info.loc [1] == pointData [i].getPointLoction () [1]) {
					pointData [i].PointInfo = info;
					repy = true;
					break; 
				} 
			}
			if (repy == true)
				continue;

			MissionPoint pointInfo = new MissionPoint ();
			pointInfo.loc = pointData [i].getPointLoction ();  
 
			//0,0就随机玩,一般策划不会配0,0
			if (pointInfo.loc [0] == 0 && pointInfo.loc [1] == 0)
				drawPoint.localPosition = new Vector3 (Random.Range (50, 2000), 0, Random.Range (50, 2000));
			else
				drawPoint.localPosition = new Vector3 (pointInfo.loc [0], 0, pointInfo.loc [1]);
			pointInfo.woldPos = drawPoint.position;
			pointInfo.localPosition = drawPoint.localPosition;
			pointInfo.pointIndex=i;
			pointData [i].PointInfo = pointInfo;
			pointList.Add (pointInfo);
		}  
	}	
	
	
}
