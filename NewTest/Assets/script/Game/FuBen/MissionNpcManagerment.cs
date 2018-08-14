using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 副本随机NPC生成管理器
/// </summary>
public class MissionNpcManagerment
{

	public static MissionNpcManagerment Instance {
		get{return SingleManager.Instance.getObj("MissionNpcManagerment") as MissionNpcManagerment;}
	}

	public List<NpcData> npcList;
	public int  maxObjCount = 4;//最大模型数量; 
	public int  nowObjCount = 0;//当前模型数量; 

	private List<Vector3> clip;	//根据点位创建npc节点,每个节点包含npclist的上下标
	private const int PERCLIP_NUM = 3;//每个节点包含的NPC数量
	private const int DISTANCE = 8;//可视节点距离,超过这个节点数的npc隐藏
	private float  thinkTime = 0.5f;//思考频率
	private float  _thinkTime = 0f;

	//visable state
	private const int TO_HIDE = -1;
	private const int NOCHANGE = 0;
	private const int TO_SHOW = 1;


	/// <summary>
	/// 向列表中添加一个新npc(逻辑数据)
	/// </summary>
	public void addNpc (NpcData npc)
	{
		if (npcList == null)
			npcList = new List<NpcData> ();
		npcList.Add (npc);
	}

	/// <summary>
	/// 清空NPC列表
	/// </summary>
	public void clearNpc ()
	{
		if (npcList == null)
			return;

		foreach (NpcData each in npcList) {
			if (each.ctrl != null) {
				MonoBase.Destroy (each.ctrl.gameObject);
				if (each.ctrl.TitleView != null)
					MonoBase.Destroy (each.ctrl.TitleView.gameObject);
			}

		}
		npcList = null;
	}

	/// <summary>
	/// 创建一个NPC图形实例
	/// </summary>
	public void createNPC (NpcData data)
	{
		MissionNpcCtrl ctrl = null;
		//	Transform old = MissionManager.instance.npcRoot.FindChild (data.name);

	//	Debug.LogWarning (data.name + " is  appear! ");
		passObj _obj = MonoBase.Create3Dobj (UserManager.Instance.getModelPath (StringKit.toInt (data.style))); 
		
		if (_obj.obj == null) {
			Debug.LogError ("role is null!!!");
			return;
		} 
		_obj.obj.transform.parent = MissionManager.instance.npcRoot;
		_obj.obj.transform.localPosition = Vector3.zero+new Vector3(0,0.1f,0);
		_obj.obj.transform.localScale = Vector3.one;
		_obj.obj.name = data.name;
		ctrl = _obj.obj.transform.gameObject.AddComponent<MissionNpcCtrl> ();
		ctrl.init (data);
	}

	/// <summary>
	/// 初始化后台返回的机器人数据
	/// </summary>
	public void initAI ()
	{
		if (MissionManager.instance == null) {
			clearNpc ();
			cleanData ();
			return;
		}
//		Debug.LogWarning ("player data:" + npcList.Count);
		//长度不合格走掉
		if (npcList.Count <= 0)
			return;
		//副本长度划分4段,每一段clip 包含点位,npc数据的开始和结束位
		int mlen = Mathf.Clamp (MissionInfoManager.Instance.mission.getAllPoint ().Length / 4, 2, int.MaxValue);
		int start = 0;
		int end = PERCLIP_NUM;
		clip = new List<Vector3> ();

		//遍历点位 创建clip并且填充内容v3(点位,NPC数据起始位,NPC数据结束位)
		for (int i=0; i<MissionInfoManager.Instance.mission.getAllPoint().Length; i+=mlen) {
			start = Mathf.Clamp (start, 0, npcList.Count);
			end = Mathf.Clamp (end, 0, npcList.Count);
			Vector3 v3 = new Vector3 (i, start, end);

			//有空的提前就结束
			if (v3 == Vector3.zero)
				break;

			clip.Add (v3);
		//	Debug.LogWarning ("**Add AI at " + i + " ,S=" + start + " , E=" + end);

			//是末尾就结束
			if (end == npcList.Count - 1)
				break;

			start += PERCLIP_NUM;
			end += PERCLIP_NUM;
		}
		updateNpc (0);
	}

	/// <summary>
	/// 阶段性增加AI
	/// </summary>
	public void updateNpc (int point)
	{
		if(clip==null || clip.Count<=0)
			return;

		//通过检测clip列表里是否有point来确定走到对应点位要不要加载新NPC
		//通过点位来增加NPC可以提高NPC的持续可见性,就算玩家长时间不动
		Vector3 tmp = Vector3.zero;
		for (int t=0; t<clip.Count; t++) {
			if (point == clip [t].x) {
				tmp = clip [t];
				break;
			}
		}
		if (tmp == Vector3.zero)
			return;

		// 生成npc在随机点位,范围大概为: || ------- you ------------------------------- ||
		for (int i =(int)tmp.y; i<(int)tmp.z; i++) {
			if(npcList[i] == null)
				break;
			npcList [i].state = NpcData.NPC_STATE_STANDY;
			//在玩家点位+-distance范围内随机
			int len = Mathf.Min (MissionInfoManager.Instance.mission.getPlayerPointIndex () + DISTANCE, MissionInfoManager.Instance.mission.getAllPoint ().Length - 1);
			int low = Mathf.Max (MissionInfoManager.Instance.mission.getPlayerPointIndex () - DISTANCE / 2, 0);
			npcList [i].mPointIndex = Random.Range (low, len);
		//	Debug.LogWarning ("npc stand at :" + npcList [i].mPointIndex);
		}

	}

	//切换场景时清理一些数据
	public void cleanData ()
	{
		nowObjCount = 0;
	}
	/// <summary>
	/// 检测该NPC是否可见
	/// </summary>
	int  checkVisable (NpcData data)
	{
		MissionPointInfo mPoint=MissionInfoManager.Instance.mission.getPointInfoByIndex (data.mPointIndex);
		if (data.ctrl == null) {
			//判断有没必要显示模型

			//模型数量限制
			if (nowObjCount >= maxObjCount) {
				return NOCHANGE;
			}

			/*
			if (Mathf.Abs (data.mPointIndex - MissionInfoManager.Instance.mission.getPlayerPointIndex ()) <= 8) {
				//足够靠近,显示模型
				createNPC (data);
				return TO_SHOW;
			}
			*/
			if(mPoint.PointInfo!=null &&  mPoint.PointInfo.pointOnRoad && MissionInfoManager.Instance.mission.getNextPoint(mPoint).PointInfo.pointOnRoad)
			{
				createNPC (data);
				return TO_SHOW;
			}
		} else {
			//判断有没必要隐藏模型
			/*
			if (Mathf.Abs (data.mPointIndex - MissionInfoManager.Instance.mission.getPlayerPointIndex ()) > 8) {
				data.ctrl.remove ();
				return TO_HIDE;
			}
			*/
			if(mPoint.PointInfo!=null && !mPoint.PointInfo.pointOnRoad)
			{
				data.ctrl.remove ();
				return TO_HIDE;
			}

		}

		return NOCHANGE;
	}

	public void removeAllGraphicData()
	{
		if(npcList==null)
			return;

		foreach(NpcData each in npcList){

			if(each.ctrl!=null)
			{
				each.ctrl.remove();
			}

		}

	}

	//主脑
	public void AI ()
	{
		if (npcList == null || npcList.Count == 0)
			return;

		if (_thinkTime < thinkTime) {
			_thinkTime += Time.deltaTime;
			return;
		} else {
			_thinkTime = 0;
		}

		foreach (NpcData each in npcList) {
			//忽略没入场AI
			if (each.mPointIndex == -1 || each.mPointIndex >= MissionInfoManager.Instance.mission.getAllPoint ().Length - 1)
				continue;

			//检测可见性,进行变换了的,本次不动作
			if (checkVisable (each) != NOCHANGE)
				continue;

			//忽略图形表现繁忙的物体
			if (each.ctrl != null && each.ctrl.GraphBusy)
				continue;

			int _event = 0;
			_event = Random.Range (0, 15);

			switch (_event) {
			case 1:
				//继续待机
				continue;
			case 2:
				//前进一格
				each.state = NpcData.NPC_STATE_MOVE;
				if (each.ctrl != null) {
				//	Debug.LogWarning (each.name + " is  move to " + (each.mPointIndex + 1));
					each.ctrl.npcMove ();
				}
				continue;
			case 3:
				//idle
				if (each.ctrl != null) {
					each.ctrl.Idle ();
				}
				continue;
			}
		}
	}

}




