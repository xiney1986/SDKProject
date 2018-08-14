using UnityEngine;
using System.Collections;

public class MissionNpcCtrl : MissionRoleCtrl
{

	Quaternion TargetRotation;
	public NpcData data;
	public bool GraphBusy = false; //图像是否繁忙
	MountsSample mounts;

	void Awake ()
	{
        activeAnimCtrl =transform.GetChild (0).GetComponent<FuBenCardCtrl> ();
	}

	public void Idle ()
	{
		lookAtPlayer ();
		playIdle();
	}

	void lookAtPlayer ()
	{
		if (MissionManager.instance == null)
			return;

		Vector3 lookPos = new Vector3 (MissionManager.instance.backGroundCamera.transform.position.x, transform.position.y, MissionManager.instance.backGroundCamera.transform.position.z);
		TargetRotation = Quaternion.LookRotation (lookPos - transform.position, Vector3.up);
	}

	public void init (NpcData data)
	{
		setPos (MissionInfoManager.Instance.mission.getPointInfoByIndex (data.mPointIndex));
		//相互关联
		this.data = data;
		data.ctrl = this;
		//mounts=MountsManagerment.Instance.getMountsBySid(data.mountsSid);
        if (data.mountsSid != 0) mounts = MountsSampleManager.Instance.getMountsSampleBySid(data.mountsSid);
        else mounts = null;
		initRoleAniCtrl (activeAnimCtrl, mounts,data.vipLevel);
		MissionNpcManagerment.Instance.nowObjCount += 1;
	}
	
	public void remove ()
	{
//		Debug.LogWarning (data.name + " is  disApear! ");
		data.state = NpcData.NPC_STATE_FINISH;
		if (TitleView != null)
			Destroy (TitleView.gameObject);
		Destroy (data.ctrl.gameObject);
		data.ctrl = null;
		MissionNpcManagerment.Instance.nowObjCount -= 1;
	}
	
	public Vector3 setPos (MissionPointInfo pointInfo)
	{
		transform.position = new Vector3 (pointInfo.PointInfo.woldPos.x, transform.position.y, pointInfo.PointInfo.woldPos.z);
		MissionPointInfo nextPoint = MissionInfoManager.Instance.mission.getNextPoint (pointInfo);
		if (nextPoint.PointInfo.pointOnRoad) {
			Vector3 pos = new Vector3 (nextPoint.PointInfo.woldPos.x, transform.position.y, nextPoint.PointInfo.woldPos.z);
			TargetRotation = Quaternion.LookRotation (pos - transform.position, Vector3.up);
			return  pos;
		} else {
			return Vector3.zero;
		}
	}

	public void npcMove ()
	{
		//到最后一个点移除
		if (data.mPointIndex >= MissionInfoManager.Instance.mission.getAllPoint ().Length - 1) {
			remove ();
			return;
		}
		MissionPointInfo currentPointInfo = MissionInfoManager.Instance.mission.getPointInfoByIndex (data.mPointIndex);
		if (currentPointInfo != null && currentPointInfo.PointInfo != null) {
			if (!currentPointInfo.PointInfo.pointOnRoad) {
				remove ();
				return;
			}
		}
		MissionPointInfo nextPointInfo = MissionInfoManager.Instance.mission.getNextPoint (currentPointInfo);
		if (nextPointInfo != null && nextPointInfo.PointInfo != null) {
			if (!nextPointInfo.PointInfo.pointOnRoad) {
				remove ();
				return;
			}
		}
		playMove();
		iTween.MoveTo (gameObject, iTween.Hash ("position", setPos (MissionInfoManager.Instance.mission.getPointInfoByIndex (data.mPointIndex)), "oncomplete", "moveComplete", "easetype", "easeOutQuad", "time", 1f));
		GraphBusy = true;
	}

	void moveComplete ()
	{
		GraphBusy = false;
		data.mPointIndex += 1;
		playStand();
		data.state = NpcData.NPC_STATE_STANDY;
		//到最后一个点移除
		if (data.mPointIndex >= MissionInfoManager.Instance.mission.getAllPoint ().Length - 1)
			remove ();

	}

	private void playIdle() {
		if(mounts!=null) {
			animCtrl.playMIdle ();
			playMountsIdle();
		} else {
			animCtrl.playIdle ();
		}
	}

	private void playMove() {
		if(mounts!=null) {
			animCtrl.playMMove ();
			playMountsMove();
		} else {
			animCtrl.playMove ();
		}
	}

	private void playStand() {
		if(mounts!=null) {
			animCtrl.playMStand ();
			playMountsStand();
		} else {
			animCtrl.playStand ();
		}
	}

	void createTitleView ()
	{
		TitleView = NpcTitleView.Create ();
		TitleView.UpdateName (data.name, data.vipLevel, data.uid);
		if (data.titleSid > 1) {
			LaddersTitleSample sample_1 = LaddersConfigManager.Instance.config_Title.M_getTitleBySid (data.titleSid);
			LaddersMedalSample sample_2 = LaddersConfigManager.Instance.config_Medal.M_getMedalBySid (data.medalSid);
			TitleView.UpdatePlayerTitle (true, sample_1, sample_2);
		} else
			TitleView.UpdatePlayerTitle (false, null, null);

	}

	protected override void OnUpdate ()
	{
		if (data == null || MissionManager.instance == null || UiManager.Instance.missionMainWindow == null)
			return;

		if (TitleView == null) {
			createTitleView ();
		}
		//旋转角色
		if (data != null && data.state != -1)
			transform.rotation = Quaternion.Slerp (transform.rotation, TargetRotation, Time.deltaTime * 12f);

		//更新名字位置
		if (TitleView != null)
			TitleView.UpdatePos (transform.position, isCycling);

	}

}
