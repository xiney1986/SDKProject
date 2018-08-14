using UnityEngine;
using System.Collections;

public enum fuben_characterState
{
	move,
	stand,
	talk,
}

public class MissionCharacterCtrl : MissionRoleCtrl
{
	public fuben_characterState state = fuben_characterState.stand;
	public Quaternion TargetRotation;
	float idleTime = 4f;
	Mounts mounts;

	public override void initRoleAniCtrl (FuBenCardCtrl _animCtrl, Mounts mounts,int vipl)
	{
		this.mounts=mounts;
		base.initRoleAniCtrl (_animCtrl, mounts,vipl);
		Vector3 lookPos = new Vector3 (MissionManager.instance.backGroundCamera.transform.position.x, transform.position.y, MissionManager.instance.backGroundCamera.transform.position.z);
		TargetRotation = Quaternion.LookRotation (lookPos - activeAnimCtrl.transform.position, Vector3.up);
	}

	public void removeNameLabel ()
	{
		if (TitleView != null)
			Destroy (TitleView.gameObject);
	}

	public void changeState (fuben_characterState _state)
	{
		state = _state;
		idleTime = 4f;
	}

	public void happy ()
	{
		if(mounts!=null) {
			animCtrl.playMHappy ();
			playMountsHappy();
		} else {
			animCtrl.playHappy ();
		}
	}

	public void playMove ()
	{
		if(mounts!=null) {
			animCtrl.playMMove ();
			playMountsMove();
		} else {
			animCtrl.playMove ();
		}
	}

	public void playStand () {
		if(mounts!=null) {
			animCtrl.playMStand ();
			playMountsStand();
		} else {
			animCtrl.playStand ();
		}
	}

	public void playIdle() {
		if(mounts!=null) {
			animCtrl.playMIdle ();
			playMountsIdle();
		} else {
			animCtrl.playIdle ();
		}
	}

	public void beginMove (bool autoMove)
	{
		Vector3 pos = new Vector3 (MissionManager.instance.mapInfo.getPlayerPointInfo ().woldPos.x, transform.position.y, MissionManager.instance.mapInfo.getPlayerPointInfo ().woldPos.z);
		isMove = true;
		lastPosition = transform.position;
		rotateCamera (true);
		float moveSpeed=getMoveSpeed();
		if (autoMove)
			iTween.MoveTo (gameObject, iTween.Hash ("position", pos, "oncomplete", "moveArriveAndAutoMove", "easetype", "easeOutQuad", "time", moveSpeed));
		else
			iTween.MoveTo (gameObject, iTween.Hash ("position", pos, "oncomplete", "moveArrive", "easetype", "easeOutQuad", "time", moveSpeed));
		playMove();
		Vector3 lookPos = new Vector3 (MissionManager.instance.mapInfo.getPlayerPointInfo ().woldPos.x, transform.position.y, MissionManager.instance.mapInfo.getPlayerPointInfo ().woldPos.z);
		//animCtrl.transform.LookAt(MissionManager.instance. mapInfo.getPoint ().woldPos);
		TargetRotation = Quaternion.LookRotation (lookPos - activeAnimCtrl.transform.position, Vector3.up);

	}

	/** 获取移动速度 */
	private float getMoveSpeed() {
		// 附加速度比例
		float addSpeedPer=0f;
		// 第一版坐骑
		if(mounts!=null) {
			addSpeedPer+=mounts.getAddSpeedPer();
		}
		float moveSpeed=1;
		moveSpeed=moveSpeed-(moveSpeed*addSpeedPer);
		if(moveSpeed<=0.3f) moveSpeed=0.3f;
		return moveSpeed;
	}

	private Vector3 lastPosition = Vector3.zero;
	private bool isMove = false;
	//角色摄像机控制
	private void rotateCamera (bool _isMove)
	{
		if (_isMove) {
			Camera targetCamera = MissionManager.instance.backGroundCamera;
			CameraCloserCtrl cameraCtrl = targetCamera.GetComponent<CameraCloserCtrl> ();
			cameraCtrl.M_zoomIn ();
			//targetCamera.LookAt(animCtrl.transform);
			//targetCamera.Translate(Vector3.back * 10);
			//iTween.MoveBy (targetCamera, new Vector3(0,0,-10), 1f);
		} else {
			Camera targetCamera = MissionManager.instance.backGroundCamera;
			CameraCloserCtrl cameraCtrl = targetCamera.GetComponent<CameraCloserCtrl> ();
			cameraCtrl.M_zoomOut (false);
			//targetCamera.LookAt(animCtrl.transform);
			//transform.Translate(Vector3.forward * 10);
			//iTween.MoveBy(targetCamera,new Vector3(0,0,10), 0.5f);
		}

	}

	void moveArrive ()
	{ 

		changeState (fuben_characterState.stand);
		MissionManager.instance.updateNextPoint (true, false);
		//行动完毕
		playStand();
		isMove = false;
		rotateCamera (false);
		Vector3 lookPos = new Vector3 (MissionManager.instance.backGroundCamera.transform.position.x, activeAnimCtrl.transform.position.y, MissionManager.instance.backGroundCamera.transform.position.z);
		//animCtrl.transform.rotation.LookAt(lookPos);
		
		TargetRotation = Quaternion.LookRotation (lookPos - activeAnimCtrl.transform.position, Vector3.up);

		if (MissionNpcManagerment.Instance != null)
			MissionNpcManagerment.Instance.updateNpc (MissionInfoManager.Instance.mission.getPlayerPointIndex ());

	}

	/** 试炼跑到终点后做的事情 */
	void moveArriveByPractice () { 
		changeState (fuben_characterState.stand);
		MissionPointInfo point = MissionInfoManager.Instance.mission.GetPlayerNextPoint ();	//获取下个节点的逻辑信息
		if (point == null) {
			return;
		}
		MissionPoint info = point.PointInfo;
		if (info != null && info.eventObj != null) {
			Destroy (info.eventObj.gameObject);
		}
	}


	void moveArriveAndAutoMove ()
	{ 
		changeState (fuben_characterState.stand);
		MissionManager.instance.updateNextPoint (true, true);
		//行动完毕
		isMove = false;
	}

	void idleCheck ()
	{
		if (state == fuben_characterState.stand) {
			idleTime -= Time.deltaTime;
			if (idleTime <= 0) {
				playIdle();
				idleTime = 4f;
			}
			
		}
	}
	void createTitleView(){
		TitleView = NpcTitleView.Create ();
		TitleView.UpdateName (UserManager.Instance.self.nickname, UserManager.Instance.self.vipLevel, UserManager.Instance.self.uid);
		if (UserManager.Instance.self.prestige > 0) {
			LaddersTitleSample sample_1 = LaddersConfigManager.Instance.config_Title.M_getTitle (UserManager.Instance.self.prestige);
			LaddersMedalSample sample_2 = LaddersConfigManager.Instance.config_Medal.M_getMedalBySid (LaddersManagement.Instance.currentPlayerMedalSid);
			TitleView.UpdatePlayerTitle (true, sample_1, sample_2);
		} else {
			TitleView.UpdatePlayerTitle (false, null, null);
		}

	}
	protected override void OnUpdate ()
	{
		base.OnUpdate ();
		if (MissionManager.instance == null || UiManager.Instance.missionMainWindow == null)
			return;

		if (TitleView == null) {
			createTitleView();
		}

		if (activeAnimCtrl != null) {
			idleCheck ();
			activeAnimCtrl.transform.rotation = Quaternion.Slerp (activeAnimCtrl.transform.rotation, TargetRotation, Time.deltaTime * 12f);
		}

		TitleView.UpdatePos (transform.position, isCycling);
//		if (isMove) {
//			Vector3 distanceV = transform.position - lastPosition;
//			distanceV = new Vector3 (-distanceV.x * 0.003f, 0, -distanceV.z * 0.003f);
//			MissionManager.instance.roadEnvironmentCameraT.position += distanceV;
//		}
	}

  
}
