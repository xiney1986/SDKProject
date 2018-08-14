using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoddessAstrolabeWindow : WindowBase {

	private GameObject gaObj;//整个星空
	private GameObject gaAsObj;//星盘
	private GameObject gaPos;//坐标节点
	private GameObject starParent;//星星父节点
	[HideInInspector]public Camera gaCamera;//星盘摄像机
	private Transform pos;


	private List<GoddessAstrolabeSample> infoByFront;//前台配置
	private List<GoddessAstrolabeStarCtrl> stars;//场景中的星星
	private int lookingId = -1;//正在定位的星星
	private const float zByZPer = 1.7f;//3D摄像机z轴缩放系数
	private const float zByZPer2 = 5f;//3D摄像机z轴缩放系数
	private const float xMinPos = -50f;
	private const float xMaxPos = 50f;
	private const float yMinPos = -50f;
	private const float yMaxPos = 50f;
	private const float zMaxPos = -50f;//缩放距离
	private int nebulaId;//当前星系



	public GameObject GoddessAstrolabePrefab;//星空
	public GameObject titleLabelPrefab;//星星标题

	public Transform UIEffectRoot;
	/** 星辰碎片数目 */
	public UILabel stardustNum;
	/** 支线星星开启数目 */
	public UILabel starOpenNum;
	/** 主线星星开启数目 */
	public UILabel mainStarOpenNum;
	/** RMB持有数目 */
	public UILabel rmbNum;
	public goddessAstrolabeMoveItem item;
	[HideInInspector]public bool isCanMove = true;//摄像机是不是可以移动
	[HideInInspector]public int lookType = 1;//切换缩放状态1-2-1

	/** 未激活时主线原型 */
	public GameObject starMainObj;
	/** 未激活时支线原型 */
	public GameObject starObj;
	public GameObject[] starHp;//绿
	public GameObject[] starAtt;//红
	public GameObject[] starDef;//橙
	public GameObject[] starMag;//蓝
	public GameObject[] starAgi;//黄
	public GameObject[] starFunc;//功能星星
	public GameObject[] starGoods;//物品星星
	public GameObject lineActive;//激活
	public GameObject lineNotActive;//待激活
	public GameObject starLast;//最后星环

	protected override void begin ()
	{
		base.begin ();
		findStar();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}

	//断线重练处理
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		if (UIEffectRoot.childCount > 0) {
			for (int i = 0; i < UIEffectRoot.childCount; i++) {
				Destroy(UIEffectRoot.GetChild(i).gameObject);
			}
		}
		if (starParent.transform.childCount > 0) {
			for(int i = 0;i<starParent.transform.childCount;i++) {
				Destroy(starParent.transform.GetChild(i).gameObject);
			}
		}
		infoByFront = GoddessAstrolabeManagerment.Instance.getStarByNebulaId(nebulaId);
		creatStar();
		updateLabelDesc ();
		StartCoroutine (Utils.DelayRun (() => {
			findStar();
		}, 1f));
	}

	public void initUI(List<GoddessAstrolabeSample> _info,int _n)
	{
		initAstrolabe();
		infoByFront = _info;
		nebulaId = _n;
		creatStar();
		updateLabelDesc ();
	}

	//传入新激活的星星ID
	public void updateUI(int id)
	{
		infoByFront = GoddessAstrolabeManagerment.Instance.getStarByNebulaId(nebulaId);
		updateLabelDesc ();
		Vector3 vPos = Vector3.zero;

		for(int i = 0;i<stars.Count;i++) {
			if(stars[i].getInfo().id == id) {
				lookType = 1;
				Vector3 pos = new Vector3(stars[i].getPos().x,stars[i].getPos().y,(stars[i].getPos().y * zByZPer - zByZPer2));
				vPos = stars[i].getPos();
				iTween.MoveTo (gaCamera.gameObject, iTween.Hash ("position",pos, "easetype", iTween.EaseType.easeInOutCubic, "oncomplete", "over", "time", 0.3f));
				stars.Remove(stars[i]);
			}
		}

		StartCoroutine (Utils.DelayRun (() => {
			iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.3f));
			iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.3f));
//		#if UNITY_ANDROID || UNITY_IPHONE
//			Handheld.Vibrate();
//		#endif

			StartCoroutine (Utils.DelayRun (() => {
				GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
				obj.transform.parent = starParent.transform;
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = vPos;
			}, 0.4f));

		}, 0.5f));

		StartCoroutine (changeStarById(id));
	}

	/// <summary>
	/// 更新文本信息
	/// </summary>
	void updateLabelDesc () {
		GoddessAstrolabeManagerment instance = GoddessAstrolabeManagerment.Instance;
		//所有主线
		int mainStar = instance.getMainStarNUmByNebulaId (nebulaId);
		//激活的主线
		int mainOpenStar = instance.getOpenMainStarNumByNebulaId (nebulaId);
		mainStarOpenNum.text = mainOpenStar + "/" + mainStar;
		starOpenNum.text = (instance.getOpenStarNumByNebulaId (nebulaId) - mainOpenStar) + "/" + (infoByFront.Count - mainStar);
		rmbNum.text = "" + UserManager.Instance.self.getRMB ();
		stardustNum.text = "" +GoddessAstrolabeManagerment.Instance.getStarScore();
	}

	//初始化星空
	public void initAstrolabe()
	{
		UiManager.Instance.backGround.switchBackGround ("Stars-009-Cyan");
		gaObj = Instantiate(GoddessAstrolabePrefab) as GameObject;
		gaObj.transform.localPosition = Vector3.zero;
		gaCamera = gaObj.transform.FindChild ("Camera").GetComponent<Camera>();
		starParent = gaObj.transform.FindChild ("starParent").gameObject;
		gaAsObj = gaObj.transform.FindChild ("Cube").gameObject;
		gaPos = gaAsObj.transform.FindChild ("pos").gameObject;
		pos = gaPos.transform.FindChild ("pos").gameObject.transform;
	}



	public override void DoDisable ()
	{
		base.DoDisable ();
		Destroy (gaObj);
	}

	//激活指定星星
	IEnumerator changeStarById(int id)
	{
		yield return new WaitForSeconds (0.2f);
		//先清理旧的
		for(int i = 0;i<UIEffectRoot.childCount;i++) {
			Transform t = UIEffectRoot.GetChild(i);
			if(t.name == id.ToString())
				Destroy(t.gameObject);
		}

		for(int i = 0;i<starParent.transform.childCount;i++) {
			Transform t = starParent.transform.GetChild(i);
			if(t.name == id.ToString())
				Destroy(t.gameObject);
		}

		yield return new WaitForSeconds (0.2f);

		//可以开始创建了
		for(int i = 0;i<infoByFront.Count;i++) {
			if(infoByFront[i].id == id) {
				GameObject a = instantiateStar(infoByFront[i]);
				if(GoddessAstrolabeManagerment.Instance.getLastStarIdById(nebulaId) == infoByFront[i].id) {
					GameObject last = Instantiate(starLast) as GameObject;
					last.transform.parent = a.transform;
					last.transform.localScale = Vector3.one;
					last.GetComponent<Animator>().Play(0);
					if (nebulaId == 6) {
						TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("astrolabe04"));
					} else {
						TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("astrolabe03"));
					}
				}
				GoddessAstrolabeStarCtrl b = a.AddComponent<GoddessAstrolabeStarCtrl>();
				b.init(infoByFront[i],this);
				b.transform.parent = starParent.transform;
				b.transform.localScale = Vector3.one;
				b.transform.FindChild ("star").gameObject.particleSystem.Play();
				
				pos.localPosition = getTransform(infoByFront[i].position);
				b.transform.position = pos.position;
				b.name = infoByFront[i].id + "";
				yield return new WaitForSeconds (0.8f);
				setLine(b);
				stars.Add(b);
                if (b.getInfo().awardType != 3)
                    UiManager.Instance.createMessageLintWindow(b.getInfo().awardDesc);
                else {
                    PrizeSample ps = new PrizeSample(b.getInfo().award.awardType, b.getInfo().award.awardSid, b.getInfo().award.awardNum);
                    UiManager.Instance.createPrizeMessageLintWindow(ps);
                }


				yield return new WaitForSeconds (0.5f);
				//给可激活的星星画线
				List<GoddessAstrolabeStarCtrl> next = getNextStar(b.getInfo().next);
				if(next != null) {
					for(int j = 0;j<next.Count;j++) {
						next[j].showUI();
						setLine(next[j]);
					}
				}
				yield return new WaitForSeconds (0.5f);

				getStarPosByMainType();
//				findStar();
//				UiManager.Instance.createMessageWindowByOneButton(b.getInfo().awardDesc,(MessageHandle)=>{
//					findStar();
//				});
			}
		}

	}

	//填充星空
	private void creatStar()
	{
		if(infoByFront == null)
			return;
		stars = new List<GoddessAstrolabeStarCtrl>();

		for(int i = 0;i<infoByFront.Count;i++) {
			GameObject a = instantiateStar(infoByFront[i]);
//			GameObject a = MonoBase.Create3Dobj("Goddess/Star").obj;
			if(GoddessAstrolabeManagerment.Instance.getLastStarIdById(nebulaId) == infoByFront[i].id) {
				GameObject last = Instantiate(starLast) as GameObject;
				last.transform.parent = a.transform;
				last.transform.localScale = Vector3.one;
				last.GetComponent<Animator>().Play(0);
			}
			GoddessAstrolabeStarCtrl b = a.AddComponent<GoddessAstrolabeStarCtrl>();
			b.init(infoByFront[i],this);
			b.transform.parent = starParent.transform;
			b.transform.localScale = Vector3.one;
			b.transform.FindChild ("star").gameObject.particleSystem.Play();

			pos.localPosition = getTransform(infoByFront[i].position);
			b.transform.position = pos.position;
			b.name = infoByFront[i].id + "";

			stars.Add(b);
		}

		if(stars == null || stars.Count == 0)
			return;

		for(int i = 0;i<stars.Count;i++) {
			setLine(stars[i]);
		}
	}

	//实例化各种星星
	private GameObject instantiateStar(GoddessAstrolabeSample sa)
	{
		if(!sa.isOpen) {
			if (sa.mainType == 1) {
				return Instantiate(starMainObj) as GameObject;
			} else {
				return Instantiate(starObj) as GameObject;
			}
		}

		switch(sa.colorId) {
		case 1:
			return Instantiate(starHp[sa.sizeType - 1]) as GameObject;
			
		case 2:
			return Instantiate(starAtt[sa.sizeType - 1]) as GameObject;
			
		case 3:
			return Instantiate(starDef[sa.sizeType - 1]) as GameObject;
			
		case 4:
			return Instantiate(starMag[sa.sizeType - 1]) as GameObject;
			
		case 5:
			return Instantiate(starAgi[sa.sizeType - 1]) as GameObject;
			
		case 6:
			return Instantiate(starFunc[sa.sizeType - 1]) as GameObject;
			
		case 7:
			return Instantiate(starGoods[sa.sizeType - 1]) as GameObject;
			
		default:
			return Instantiate(starObj) as GameObject;
		}
	}

	//画线
	private void setLine(GoddessAstrolabeStarCtrl _gas)
	{
		GoddessAstrolabeSample ss = _gas.getInfo();

		if(ss.father == 0 || !GoddessAstrolabeManagerment.Instance.getFatherStarIsOpen(ss)) {
			return;
		}
		if(getFatherStar(ss.father) != null) {
			if(_gas.getInfo().mainType == 1) {
				LineRenderer renderActive = (Instantiate(lineActive) as GameObject).GetComponent<LineRenderer> ();
			
				renderActive.transform.parent = _gas.transform;
				renderActive.SetPosition (0, _gas.getPos());
				renderActive.SetPosition (1, getFatherStar(ss.father).getPos());
				renderActive.enabled = true;
			}
			else{
				LineRenderer renderNotActive = (Instantiate(lineNotActive) as GameObject).GetComponent<LineRenderer> ();
			
				renderNotActive.transform.parent = _gas.transform;
				renderNotActive.SetPosition (0, _gas.getPos());
				renderNotActive.SetPosition (1, getFatherStar(ss.father).getPos());
				renderNotActive.enabled = true;
			}
		}
	}

	//找出父节点
	public GoddessAstrolabeStarCtrl getFatherStar(int fatherId)
	{
		if(stars == null)
			return null;
		for(int i = 0;i<stars.Count;i++) {
			if(stars[i].getInfo().id == fatherId)
				return stars[i];
		}
		return null;
	}

	//找出子节点
	public List<GoddessAstrolabeStarCtrl> getNextStar(int[] nextId)
	{
		List<GoddessAstrolabeStarCtrl> list = new List<GoddessAstrolabeStarCtrl>();
		if(stars == null)
			return null;
		for(int i = 0;i<stars.Count;i++) {
			for(int j = 0;j<nextId.Length;j++) {
				if(stars[i].getInfo().id == nextId[j])
					list.Add(stars[i]);
			}
		}
		return list;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		//定位
		if(gameObj.name == "buttonRecommend")
		{
			isCanMove = false;
			findStar();
			MaskWindow.UnlockUI();
		}
		else if(gameObj.name == "close")
		{
			UIEffectRoot.gameObject.SetActive (false);
	
			//UiManager.Instance.backGroundWindow.restoreBackGround();
			finishWindow();
			//UiManager.Instance.openMainWindow();
		}
		else if(gameObj.name == "buttonZoom")
		{
//			gaCamera.transform.localRotation = new Quaternion(0,0,0,0);
			isCanMove = false;
			if(lookType == 1) {
				lookType = 2;
				Vector3 pos = new Vector3(gaCamera.transform.position.x,gaCamera.transform.position.y,zMaxPos);
				iTween.MoveTo ( gaCamera.gameObject, iTween.Hash ("position",pos, "easetype", iTween.EaseType.easeInOutCubic, "time", 0.3f));
			}
			else {
				lookType = 1;
				Vector3 pos = new Vector3(gaCamera.transform.position.x,gaCamera.transform.position.y,(gaCamera.transform.position.y * zByZPer - zByZPer2));
				iTween.MoveTo ( gaCamera.gameObject, iTween.Hash ("position",pos, "easetype", iTween.EaseType.easeInOutCubic, "time", 0.3f));
			}
			MaskWindow.UnlockUI();
		}
	}

	private Vector3 getTransform(float[] pos)
	{
		return new Vector3 (pos[0],pos[1]);
	}

	private void findStar()
	{
		lookType = 1;
		Vector3 pos;
		List<Vector3> list = getStarPos();
		if(list == null || list.Count <= 0) {
			//没有可激活星星时定位到最后一颗
			if(stars != null && stars.Count > 0) {
				GoddessAstrolabeStarCtrl a = null;
				for(int i = 0;i<stars.Count;i++) {
					if(stars[i].getInfo().id == GoddessAstrolabeManagerment.Instance.getLastStarIdById(nebulaId)) {
						a = stars[i];
					}
				}
				if(a == null)
					pos = new Vector3(stars[0].getPos().x,stars[0].getPos().y,(stars[0].getPos().y * zByZPer - zByZPer2));
				else
					pos = new Vector3(a.getPos().x,a.getPos().y,(a.getPos().y * zByZPer - zByZPer2));
			}
			else
				pos = new Vector3(0,0,-5);
		}
		else {
			if(lookingId >= list.Count - 1)
				lookingId = 0;
			else 
				lookingId++;
			pos = new Vector3(list[lookingId].x,list[lookingId].y,(list[lookingId].y * zByZPer - zByZPer2));
		}

//		gaCamera.transform.localRotation = new Quaternion(0,0,0,0);
		iTween.MoveTo ( gaCamera.gameObject, iTween.Hash ("position",pos, "easetype", iTween.EaseType.easeInOutCubic, "oncomplete", "over", "time", 0.3f));
	}

	//获得下一颗可激活主星的坐标，如果没有，就回去原点
	private void getStarPosByMainType()
	{
		if(stars == null)
			return;
		bool isHave = false;
		Vector3 pos = new Vector3(0,0,-5);
		GoddessAstrolabeStarCtrl a = null;
		for(int i = 0;i<stars.Count;i++) {
			if(stars[i].getInfo().id == GoddessAstrolabeManagerment.Instance.getLastStarIdById(nebulaId)) {
				a = stars[i];
			}
			if (stars[i].getInfo().mainType == 1) {
				GoddessAstrolabeSample ss = stars[i].getInfo();
				if(!GoddessAstrolabeManagerment.Instance.getFatherStarIsOpen(ss))
					continue;
				else {
					if(stars[i].getInfo().isOpen) {
						continue;
					}
					else{
						isHave = true;
						pos = new Vector3(stars[i].getPos().x,stars[i].getPos().y,(stars[i].getPos().y * zByZPer - zByZPer2));
						break;
					}
				}
			}
		}
		if (!isHave) {
			findStar ();
		} else {
//			gaCamera.transform.localRotation = new Quaternion(0,0,0,0);
			iTween.MoveTo ( gaCamera.gameObject, iTween.Hash ("position",pos, "easetype", iTween.EaseType.easeInOutCubic, "oncomplete", "over", "time", 0.3f));
		}
	}

	private List<Vector3> getStarPos()
	{
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		if(stars == null)
			return null;
		for(int i = 0;i<stars.Count;i++) {
			GoddessAstrolabeSample ss = stars[i].getInfo();
			
			if(!GoddessAstrolabeManagerment.Instance.getFatherStarIsOpen(ss))
				continue;
			else {
				if(stars[i].getInfo().isOpen) {
					continue;
				}
				else{
					//排序,主星优先
					if (stars[i].getInfo().mainType == 1)
						list.Add(stars[i].getPos());
					else
						list2.Add(stars[i].getPos());
				}
			}
		}
		ListKit.AddRange(list,list2);
		return list;
	}
	
	void Update()
	{
		if(isCanMove) {
			if(lookType == 1)
				gaCamera.transform.position = new Vector3(gaCamera.transform.position.x,gaCamera.transform.position.y,(gaCamera.transform.position.y * zByZPer - zByZPer2));
			else
				gaCamera.transform.position = new Vector3(gaCamera.transform.position.x,gaCamera.transform.position.y,zMaxPos);
		}

		if(gaCamera.transform.position.x <= xMinPos){
			if(lookType == 1)
				gaCamera.transform.position = new Vector3(xMinPos,gaCamera.transform.position.y,(gaCamera.transform.position.y * zByZPer - zByZPer2));
			else
				gaCamera.transform.position = new Vector3(xMinPos,gaCamera.transform.position.y,zMaxPos);
		}
		if(gaCamera.transform.position.x >= xMaxPos) {
			if(lookType == 1)
				gaCamera.transform.position = new Vector3(xMaxPos,gaCamera.transform.position.y,(gaCamera.transform.position.y * zByZPer - zByZPer2));
			else
				gaCamera.transform.position = new Vector3(xMaxPos,gaCamera.transform.position.y,zMaxPos);
		}
		if(gaCamera.transform.position.y <= yMinPos) {
			if(lookType == 1)
				gaCamera.transform.position = new Vector3(gaCamera.transform.position.x,yMinPos,(gaCamera.transform.position.y * zByZPer - zByZPer2));
			else
				gaCamera.transform.position = new Vector3(gaCamera.transform.position.x,yMinPos,zMaxPos);
		}
		if(gaCamera.transform.position.y >= yMaxPos) {
			if(lookType == 1)
				gaCamera.transform.position = new Vector3(gaCamera.transform.position.x,yMaxPos,(gaCamera.transform.position.y * zByZPer - zByZPer2));
			else
				gaCamera.transform.position = new Vector3(gaCamera.transform.position.x,yMaxPos,zMaxPos);
		}
	}
}
