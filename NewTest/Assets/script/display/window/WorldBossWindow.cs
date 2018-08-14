using UnityEngine;
using System.Collections;

public class WorldBossWindow :WindowBase {



	public GameObject worldBossPrefab;
	[HideInInspector]
	public Camera gaCamera;
	public bool isMove = false;
	private float  camMinX = -5f;
	private float camMaxX = 2f;
	private WorldBossIntegrate wbi;
	private int goldBoxCount;
	private int sliverBoxCount;


	public UILabel moneyCount;
	public barCtrl storeBar;
	public barCtrl pveBar;
	public UILabel pveValue;
	public UILabel storeValue;
	public GameObject[] pvpSprits;
	public UILabel goldBoxLabel;
	public UILabel sliverBoxLabel;
	public UILabel myRank;
	public UILabel myDamage;

	protected override void begin () {


		updateUserInfo ();
		updataBar();
		if (ResourcesManager.Instance.allowLoadFromRes) {
			cacheFinish ();
		}
		else {
			cacheModel ();
		}
	}


	void cacheModel () {
		string[] paths = new string[]{
			"mission/ez",
			"mission/girl",
			"mission/mage",
			"mission/maleMage",
			"mission/point",
			"mission/swordsman",
			"mission/archer",
		};
		ResourcesManager.Instance.cacheData (paths, (list) => {
			cacheFinish ();
		}, "other");
	}
	void cacheFinish () {
		if (wbi != null) {
			Utils.SetLayer (wbi.gameObject, LayerMask.NameToLayer ("3D"));
		}

		MaskWindow.UnlockUI ();
	}

	float startTime;
	float intervalTime;
	public void initialization () {
		//has initialization 
		startTime = Time.time;
		if (wbi != null) {
			return;
		}
//		am = ArenaManager.instance;
		User user = UserManager.Instance.self;
//		ResourcesManager.Instance.LoadAssetBundleTexture (user.getIconPath (), texHeadIcon);
		
		GameObject obj = Instantiate (worldBossPrefab) as GameObject; 
		wbi = obj.GetComponent<WorldBossIntegrate> ();
		gaCamera = wbi.camera;
	}


	void Update () {
		if (wbi == null)
			return;
		if (startTime + intervalTime < Time.time) {
			startTime = Time.time;
			intervalTime = Random.Range (1, 3);
			passObj  _obj = Creat3Drole (Random.Range (1, 6));
			iTween.MoveTo (_obj.obj, iTween.Hash ("position", _obj.obj.transform.position,"path",wbi.paths,"looktarget",wbi.paths[1], "easetype", "easeOutQuad", "time",5f));
			GameObject.Destroy (_obj.obj, 5f);
		}

		if(gaCamera != null){
			if(gaCamera.transform.position.x <= camMinX){
				gaCamera.transform.position = new Vector3(camMinX,gaCamera.transform.position.y,gaCamera.transform.position.z);
			}

			if(gaCamera.transform.position.x >= camMaxX){
				gaCamera.transform.position = new Vector3(camMaxX,gaCamera.transform.position.y,gaCamera.transform.position.z);
			}
		}


	}

	/// <summary>
	/// 创建3D角色
	/// </summary>
	/// <param name="iconId">Icon identifier.</param>
	public passObj Creat3Drole (int iconId) {
		passObj _obj = Create3Dobj (UserManager.Instance.getModelPath (iconId)); 
		_obj.obj.transform.parent = wbi.root.transform;
		_obj.obj.transform.localScale = Vector3.one;
		_obj.obj.transform.GetChild (0).localScale = Vector3.one;
		float x = Random.Range (wbi.spawnArea [0].position.x, wbi.spawnArea [1].position.x);
		float z = Random.Range (wbi.spawnArea [0].position.z, wbi.spawnArea [1].position.z);
		_obj.obj.transform.position = new Vector3 (x, 0f, z);
		_obj.obj.GetComponentInChildren<Animation> ().Play ("run");
		return _obj;
	}
		
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		}
		else if (gameObj.name == "move") {

		}
		else if (gameObj.name == "team") {

		}
		else if (gameObj.name == "quit") {

		}
		else if (gameObj.name == "revive") {

		}
		else if (gameObj.name == "award") {

		}
	}
		
	protected override void DoEnable () {
		//这里不需要背景变黑,调基类默认背景变黑
	}
		
	public override void DoDisable () { 
		base.DoDisable ();
			
//			if (timer != null)
//				timer.stop ();
//			timer = null; 
			
//			UiManager.Instance.removeAllEffect ();
	}

	/// <summary>
	/// 以下的代码是为了处理副本内短线重连
	/// </summary>
		
	public override void OnNetResume () {
		base.OnNetResume ();
	}

	void Destory3D () {
		if (wbi != null && !wbi.destroyed)
			Destroy (wbi.gameObject);
	}

	void Hide3D () {
		if (wbi != null) {
			Utils.SetLayer (wbi.gameObject, LayerMask.NameToLayer ("Hide"));
		}
	}

	void updateUserInfo(){
		moneyCount.text = UserManager.Instance.self.getMoney().ToString();
		if (MissionInfoManager.Instance.mission != null) {
			goldBoxCount = MissionInfoManager.Instance.mission.getTreasureNum (TreasureType.TREASURE_GOLD);
			goldBoxLabel.text = goldBoxCount.ToString ();
			sliverBoxCount = MissionInfoManager.Instance.mission.getTreasureNum (TreasureType.TREASURE_SILVER);
			sliverBoxLabel.text = sliverBoxCount.ToString ();
		}
	}

	private void updataBar ()
	{
		int numm=UserManager.Instance.self.getPvPPoint ();
		for(int i=0;i<pvpSprits.Length;i++){
			if(i<numm){
				pvpSprits[i].SetActive(true);
			}else pvpSprits[i].SetActive(false);
		}
		//pvpBar.updateValue (UserManager.Instance.self.getPvPPoint (), UserManager.Instance.self.getPvPPointMax ());
		bool flag=UserManager.Instance.self.getStorePvEPoint()>0;
		pveBar.gameObject.SetActive(!flag);
		storeBar.gameObject.SetActive(flag);
		if(flag){
			storeBar.updateValue (UserManager.Instance.self.getStorePvEPoint (), UserManager.Instance.self.getStorePvEPointMax ());
			storeValue.text = UserManager.Instance.self.getStorePvEPoint () + "/" + UserManager.Instance.self.getStorePvEPointMax ();
		}else{
			pveBar.updateValue (UserManager.Instance.self.getPvEPoint (), UserManager.Instance.self.getPvEPointMax ());
			pveValue.text = UserManager.Instance.self.getPvEPoint () + "/" + UserManager.Instance.self.getPvEPointMax ();
		}
	}

}


