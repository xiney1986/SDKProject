using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 聚焦决赛区域竞猜点
/// </summary>
public class ArenaFocusGuess 
{

	/** 排序对象 */
	Comp comp=new Comp();
	/** 竞猜聚焦点集合 */
	List<FocusPointInfo> focusCuessList;
	/** 当前聚焦下标 */
	int currentFocusIndex;
	/** 当前聚焦点下标 */
	int focusCuessIndex;

	public ArenaFocusGuess()
	{
		focusCuessList=new List<FocusPointInfo> ();
	}

	/** 竞猜集合排序 */
	public void SortFocusCuess()
	{
		if (focusCuessList.Count <= 1) return;
		FocusPointInfo[] infos=focusCuessList.ToArray ();
		SetKit.sort (infos,comp);
		focusCuessList.Clear ();
		foreach(FocusPointInfo info in infos)
		{
			focusCuessList.Add(info);
		}
	}

	/// <summary>
	/// 聚焦竞猜点
	/// </summary>
	public FocusPointInfo focusCuessPoint()
	{
		if(focusCuessList.Count==0) return null;
		FocusPointInfo focusPointInfo=getActiveCuessPoint ();
		if(focusPointInfo==null)
		{
			focusPointInfo=getPassiveCuessPoint();
			if(focusPointInfo!=null)
			{
				AddCurrentFocusIndex();
			}
		}
		return focusPointInfo;
	}

	/** 获取不可竞猜的点信息 */
	private FocusPointInfo getPassiveCuessPoint()
	{
		if (currentFocusIndex <= focusCuessList.Count - 1)
			return focusCuessList [currentFocusIndex];
		return null;
	}

	/** 获取可竞猜的点信息 */
	private FocusPointInfo getActiveCuessPoint()
	{
		foreach(FocusPointInfo info in focusCuessList)
		{
			if(info.isGuessd()) return info;
		}
		return null;
	}
	
	/** 获取竞猜点信息 */
	public FocusPointInfo getFocusPointByPointName(string pointName)
	{
		foreach(FocusPointInfo info in focusCuessList)
		{
			if(pointName == info.getPointName()) return info;
		}
		return null;
	}

	/** 添加当前聚焦下标 */
	public void AddCurrentFocusIndex()
	{
		currentFocusIndex++;
		if (currentFocusIndex >= focusCuessList.Count) currentFocusIndex = 0;
	}
	
	/// <summary>
	/// 添加决赛区域竞猜点-用于竞猜定位
	/// <param name="vector3">位置</param>
	/// <param name="groupIndex">组下标-排序用</param>
	/// <param name="index">组里面的下标-排序用</param>
	/// <param name="isGuess">是否可竞猜</param>
	/// <param name="tapIndex">tap下标</param>
	/// <param name="pointName">区域点名</param>
	/// </summary>
	public void AddFocusCuessPoint(Vector3 vector3,int groupIndex,int index,bool isGuess,int tapIndex,string pointName)
	{
		FocusPointInfo focusPointInfo=new FocusPointInfo(vector3,groupIndex,index,isGuess,tapIndex,pointName);
		if (focusCuessList.Contains (focusPointInfo)) return;
		focusCuessList.Add(focusPointInfo);
	}

	/** 清理数据 */
	public void Clear()
	{
		focusCuessList.Clear ();
		currentFocusIndex = 0;
	}

	public class FocusPointInfo
	{
		/** 是否可竞猜 */
		bool isGuess;
		/** 位置权重 */
		int indexWeight;
		/** 聚焦点 */
		Vector3 focusPoint;
		/** tap下标 */
		int tapIndex;
		/** 区域点名 */
		string pointName;

		public FocusPointInfo(Vector3 focusPoint,int groupIndex,int index,bool isGuess,int tapIndex,string pointName)
		{
			setIndexWeight(groupIndex,index);
			this.isGuess=isGuess;
			this.focusPoint=focusPoint;
			this.tapIndex=tapIndex;
			this.pointName=pointName;
		}
		public void setIndexWeight(int groupIndex,int index)
		{
			indexWeight = groupIndex * 100 + index;
		}
		public bool isGuessd()
		{
			return isGuess;
		}
		public void setGuessd(bool isGuess)
		{
			this.isGuess = isGuess;
		}
		public int getIndexWeight()
		{
			return indexWeight;
		}
		public Vector3 getFocusPoint()
		{
			return focusPoint;
		}
		public int getTapIndex()
		{
			return tapIndex;
		}
		public string getPointName()
		{
			return pointName;
		}
	}

	class Comp : Comparator
	{
		public int compare(object o1,object o2)
		{
			if(o1==null) return 1;
			if(o2==null) return -1;
			if(!(o1 is FocusPointInfo)||!(o2 is FocusPointInfo)) return -1;
			FocusPointInfo info1=(FocusPointInfo)o1;
			FocusPointInfo info2=(FocusPointInfo)o2;
			if(info1.isGuessd()&&!info2.isGuessd())
			{
				return -1;
			}
			if(!info1.isGuessd()&&info2.isGuessd())
			{
				return 1;
			}
			if(info1.isGuessd()&&info2.isGuessd())
			{
				if(info1.getIndexWeight()>info2.getIndexWeight()) return 1;
				if(info1.getIndexWeight()<info2.getIndexWeight()) return -1;
				return 0;
			} 
			return 0;
		}
	}
}
