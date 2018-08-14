using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 副本3D场景构建核心类
/// 原理：根据当前副本玩家所在的索引位置，动态构建路径
/// 路径的构建由，路线片段构成
/// 路线片段包括：起点段，中段，左段，右段，结束段
/// </summary>
public class MissionRoad : MonoBehaviour
{
	public MissionRoadSegment segments_start;
	public MissionRoadSegment segments_end;
	public MissionRoadSegment segments_middle_line;
	public MissionRoadSegment segments_middle_left;
	public MissionRoadSegment segments_middle_right;
	//构建好路径后 回调
	public CallBack creatSegmentCmp;
	//路径构建的的容器
	public Transform segment_parent;

	public int totalStep;

	//玩家当前的步点
	[HideInInspector]
	public int currentStep=1;

	//当前构建好的路线 最远能达到的步点
	public int currentLastStep=0;

	//可以预先加载的步点数，ex:当前是1，那构建好的路径最远至少可以达到1+5=6；
	private int preMaxLoadStepCount=5;

	//卸载的步点数标准，比当前的步点少x个步点作为卸载路段的标准
	private int preMaxUnLoadStepCount=2;

	//记录当前路径最后路段的位置|和旋转 共同决定一下个路段的起始位置和旋转
	private Vector3 lastPostion;

	//记录当前路径最后路段的选择|和位置 共同决定一下个路段的起始位置和旋转
	private Vector3 lastRotatioin=Vector3.zero;


	//构建好的路径 所有的路段
	private List<MissionRoadSegment> segmentList;

	//当卸载路段时 放到cache中，作为奖励构建时的缓存，避免重复的生产路段
	private List<MissionRoadSegment> cacheSegment;

	//当前路径所包含的所有步点位置
	private List<Vector3> missionPoints;

	//是否是动态的构建路径
	public bool creatRoadDynamic=false;


	public string[] roadSegmentType;

	public int startStep=1;

	private MissionRoadSample roadSample;

	void Awake()
	{
		segments_start.M_init();
		segments_start.gameObject.SetActive(false);
		
		if (segments_end != null) {
			segments_end.M_init ();
			segments_end.gameObject.SetActive (false);
		}

		if(segments_middle_line!=null)
		{
			segments_middle_line.M_init();
			segments_middle_line.gameObject.SetActive(false);
		}
		if(segments_middle_left!=null)
		{
			segments_middle_left.M_init();
			segments_middle_left.gameObject.SetActive(false);
		}
		if(segments_middle_right!=null)
		{
			segments_middle_right.M_init();
			segments_middle_right.gameObject.SetActive(false);
		}

		segmentList=new List<MissionRoadSegment>();
		cacheSegment=new List<MissionRoadSegment>();
		missionPoints=new List<Vector3>();
	}
	void Destory()
	{
		StopAllCoroutines();
	}
	
	/// <summary>
	/// 初始化时 需要给一个路线的数据样例
	/// </summary>
	/// <param name="_roadSample">_road sample.</param>
	public void M_init(MissionRoadSample _roadSample)
	{
		roadSample=_roadSample;
		totalStep=_roadSample.pointCount;
		roadSegmentType=_roadSample.segmentTypes;
	}

	/// <summary>
	/// 根据点位 返回所对应的坐标
	/// </summary>
	/// <returns>The position.</returns>
	/// <param name="_currentStep">_current step.</param>
	public Vector3 M_getPosition(int _currentStep) 
	{
		_currentStep-=startStep;
		if(_currentStep>missionPoints.Count)
		{

		}
		if(_currentStep>missionPoints.Count)
		{
			return Vector3.zero;
		}
		return  missionPoints[_currentStep];
	}
	/// <summary>
	/// 起始点 初始化
	/// </summary>
	/// <param name="_currentStep">_current step.</param>
	/// <param name="_cmpFun">_cmp fun.</param>
	public void M_startStep(int _currentStep,CallBack _cmpFun)
	{
		int segmentIndex=M_calculateSegmentIndex(_currentStep);
		if(segmentIndex>0)
		{

			if(segmentIndex==1)
			{
				currentLastStep=0;				
				startStep=1;				
			}else
			{
				lastRotatioin=roadSample.M_getSegmentRotation(segmentIndex-2);
				currentLastStep=M_calculateSegmentTotalStep(segmentIndex-2);
				startStep=currentLastStep+1;
			}
			EnumRoadSegment curType=EnumRoadSegmentKit.M_getType(roadSegmentType[segmentIndex-1]);
			M_creatSegment(curType);
		}else
		{
			currentLastStep=0;				
			startStep=1;	
		}
		M_goStep(_currentStep,_cmpFun);
	}
	/// <summary>
	/// 跳转到指定步点，即构建路线直到满足指定的步点
	/// </summary>
	/// <param name="_currentStep">_current step.</param>
	/// <param name="_cmpFun">_cmp fun.</param>
	public void M_goStep(int _currentStep,CallBack _cmpFun) 
	{
		if(_currentStep<1&&_currentStep>totalStep)
		{
			return;
		}
		currentStep=_currentStep;
		creatSegmentCmp=_cmpFun;

		//M_clearBeforeSegment();
		StartCoroutine("M_updateSegmentView",currentStep);
	}
	/// <summary>
	/// 返回路段类型所对应的该路段中的步点总数
	/// </summary>
	/// <returns>The segment step count.</returns>
	/// <param name="_type">_type.</param>
	public int M_getSegmentStepCount(EnumRoadSegment _type)
	{
		MissionRoadSegment segment=M_getRoadSegment(_type);
		return segment.totalSteps;
	}
	/// <summary>
	/// 计算步点数所对应的路段索引
	/// </summary>
	/// <returns>The segment index.</returns>
	/// <param name="_step">_step.</param>
	public int M_calculateSegmentIndex(int _step)
	{
		int index=0;
		MissionRoadSegment tempSegment;
		for(int i=0,tempStep=0,length=roadSegmentType.Length;i<length;i++)
		{
			tempSegment=M_getRoadSegment(EnumRoadSegmentKit.M_getType(roadSegmentType[i]));
			tempStep+=tempSegment.totalSteps;
			if(tempStep>=_step)
			{
				index=i;
				break;
			}
		}	
		return index;
	}

	public Vector3 M_calculateSegmentRotatioin(int _step)
	{
		int index=M_calculateSegmentIndex(_step);
		return roadSample.M_getSegmentRotation(index);
	}

	/// <summary>
	/// 构建路线 直到路线能达到的最远步点满足当前步点数
	/// 如果没有达到 则递归调用
	/// </summary>
	/// <returns>The segment view.</returns>
	/// <param name="_currentStep">_current step.</param>
	private IEnumerator M_updateSegmentView(int _currentStep)
	{
		if(currentLastStep>=totalStep)
		{
			if(creatSegmentCmp!=null)
			{
				creatSegmentCmp();
			}
			creatSegmentCmp=null;
			yield break;
		}

		if(creatRoadDynamic)
		{
			if(_currentStep+preMaxLoadStepCount<=currentLastStep)
			{
				if(creatSegmentCmp!=null)
				{
					creatSegmentCmp();
				}
				creatSegmentCmp=null;
				yield break;
			}
		}

		int segmentIndex=M_calculateSegmentIndex(currentLastStep+1);
		EnumRoadSegment curType=EnumRoadSegmentKit.M_getType(roadSegmentType[segmentIndex]);
		M_creatSegment(curType);

		
		yield return new WaitForSeconds(0.1f);

		if(creatRoadDynamic)
		{
			if(currentStep+preMaxLoadStepCount>currentLastStep)
			{
				_currentStep++;
				StartCoroutine("M_updateSegmentView",_currentStep);
			}else
			{
				if(creatSegmentCmp!=null)
				{
					creatSegmentCmp(); 
				}
				creatSegmentCmp=null;
			}
		}else
		{
			_currentStep++;
			StartCoroutine("M_updateSegmentView",_currentStep);
		}
	}
	/// <summary>
	/// 回收当前所有的路段 到cacheSegment中
	/// </summary>
	private void M_clearBeforeSegment()
	{
		int i=0,length=segmentList.Count;
		MissionRoadSegment roadSegment;
		for(i=length-1;i>=0;i--)
		{
			roadSegment=segmentList[i];
			if(roadSegment.maxStep<=currentStep-preMaxUnLoadStepCount)
			{
				roadSegment.gameObject.SetActive(false);
				segmentList.RemoveAt(i);
				cacheSegment.Add(roadSegment);
			}
		}
	}

	/// <summary>
	/// 创建一个指定类型的路段
	/// </summary>
	/// <param name="_type">_type.</param>
	private void M_creatSegment(EnumRoadSegment _type)
	{

		Transform newSegment_transform=null;
		MissionRoadSegment targetSegment=null;
		float point_x;
		float point_z;

		switch(_type)
		{
			case EnumRoadSegment.Start:
			{
				targetSegment=segments_start;				
				M_creatSegment_StartEnd(targetSegment);		
			}break;

			case EnumRoadSegment.End:
			{
				targetSegment=segments_end;	
				M_creatSegment_StartEnd(targetSegment);	
			}break;

			case EnumRoadSegment.Middle_Line:
			{
				targetSegment=segments_middle_line;	
				newSegment_transform=M_creatSegment_Middle(_type,targetSegment.gameObject);					
				newSegment_transform.localPosition=lastPostion;
				newSegment_transform.localRotation=Quaternion.Euler(lastRotatioin);				
				targetSegment=newSegment_transform.GetComponent<MissionRoadSegment>();
			}break;

			case EnumRoadSegment.Middle_Left:
			{
				targetSegment=segments_middle_left;	
				newSegment_transform=M_creatSegment_Middle(_type,targetSegment.gameObject);					
				newSegment_transform.localPosition=lastPostion;
				lastRotatioin+=new Vector3(0,-90,0);
				newSegment_transform.localRotation=Quaternion.Euler(lastRotatioin);
				targetSegment=newSegment_transform.GetComponent<MissionRoadSegment>();
			}break;

			case EnumRoadSegment.Middle_Right:
			{
				targetSegment=segments_middle_right;				
				newSegment_transform=M_creatSegment_Middle(_type,targetSegment.gameObject);				
				newSegment_transform.localPosition=lastPostion;
				lastRotatioin+=new Vector3(0,90,0);
				newSegment_transform.localRotation=Quaternion.Euler(lastRotatioin);
				targetSegment=newSegment_transform.GetComponent<MissionRoadSegment>();
			}break;
		}
		if(targetSegment!=null)
		{
			point_x=targetSegment.distance*(int)Mathf.Cos(lastRotatioin.y*Mathf.Deg2Rad);
			point_z=targetSegment.distance*(int)Mathf.Sin(-lastRotatioin.y*Mathf.Deg2Rad);
			lastPostion+=new Vector3(point_x,0,point_z);
		
			segmentList.Add(targetSegment);
			targetSegment.maxStep=currentLastStep+targetSegment.totalSteps;
			currentLastStep+=targetSegment.totalSteps;
			Vector3[] targetSegmentPoints=targetSegment.M_getPoints();
			ListKit.AddRange(missionPoints,targetSegmentPoints);
		}
	}
	/// <summary>
	/// 根据类型 创建一个新的路段 起始或结束路段
	/// </summary>
	/// <param name="_targetSegment">_target segment.</param>
	private void M_creatSegment_StartEnd(MissionRoadSegment _targetSegment)
	{
		UIUtils.M_addChild(segment_parent,_targetSegment.transform);
		_targetSegment.gameObject.SetActive(true);
		_targetSegment.transform.localPosition=lastPostion;	
		_targetSegment.transform.localRotation=Quaternion.Euler(lastRotatioin);

	}
	/// <summary>
	/// 根据类型 创建一个新的路段 中间路段（左 中 右）
	/// </summary>
	/// <returns>The segment_ middle.</returns>
	/// <param name="_targetType">_target type.</param>
	/// <param name="_cloneSegmentGo">_clone segment go.</param>
	private Transform M_creatSegment_Middle(EnumRoadSegment _targetType,GameObject _cloneSegmentGo)
	{
		GameObject newSegment_go=M_getCacheSegment(_targetType);
		if(newSegment_go==null)
		{
			newSegment_go=Instantiate(_cloneSegmentGo) as GameObject;
		}
		newSegment_go.SetActive(true);
		UIUtils.M_addChild(segment_parent,newSegment_go.transform);		
		Transform newSegment_transform=newSegment_go.transform;					
		return newSegment_transform;
	}
	/// <summary>
	/// 根据路段类型 从缓存中取 如果没有 则返回null
	/// </summary>
	/// <returns>The cache segment.</returns>
	/// <param name="_type">_type.</param>
	private GameObject M_getCacheSegment(EnumRoadSegment _type)
	{
		GameObject newSegment_go=null;
		foreach(MissionRoadSegment item in cacheSegment)
		{
			if(item.type==_type)
			{
				newSegment_go=item.gameObject;
				cacheSegment.Remove(item);
				break;
			}
		}
		return newSegment_go;
	}
	/// <summary>
	/// 计算索引路段对应的总的到达的步点数
	/// </summary>
	/// <returns>The segment total step.</returns>
	/// <param name="_index">_index.</param>
	private int M_calculateSegmentTotalStep(int _index)
	{
		int stepCount=0;
		MissionRoadSegment tempSegment;  
		for(int i=0;i<=_index;i++)
		{
			tempSegment=M_getRoadSegment(EnumRoadSegmentKit.M_getType(roadSegmentType[i]));
			stepCount+=tempSegment.totalSteps;
		}	
		return stepCount;
	}
	/// <summary>
	/// 根据路段类型 返回其路段的组件
	/// </summary>
	/// <returns>The road segment.</returns>
	/// <param name="_type">_type.</param>
	private MissionRoadSegment M_getRoadSegment(EnumRoadSegment _type)
	{
		MissionRoadSegment targetSegment=null;
		switch(_type)
		{
			case EnumRoadSegment.Start:
				targetSegment=segments_start;		
			break;

			case EnumRoadSegment.End:
				targetSegment=segments_end;	
			break;
				
			case EnumRoadSegment.Middle_Line:
				targetSegment=segments_middle_line;	
			break;
				
			case EnumRoadSegment.Middle_Left:
				targetSegment=segments_middle_left;	
			break;
				
			case EnumRoadSegment.Middle_Right:
				targetSegment=segments_middle_right;				
			break;
		}
		return targetSegment;
	}
}
