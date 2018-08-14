using UnityEngine;
using System.Collections;

public class GuideUI : MonoBase
{
	public Transform guideArrow;//指示箭头
	public UISprite arrowTex;
	public UILabel descript;//描述
	/** 遮罩,改变它的大小,来控制可点区域 */
	public Transform mask;
	/** 点击其他地方跳过强制指引按钮 */
	public GuideButton[] maskButtons;
	/** 全屏遮罩 */
	public GameObject maxMask;
	public GuideButton guideButton;//指引按钮,用来执行指引中触发全屏的事件
	public UILabel blinkLabel;//点击任意地方关闭
	/** 文字描述组 */
	public GameObject labelGroup;
	/** 友情提示放弃按钮 */
	public GuideButton[] friendlyMaskButton;
	/** 友情提示按钮 */
	public GuideButton friendlyButton;
	/** 友情引导遮罩,改变它的大小,来控制可点区域 */
	public Transform friendlyButtonMask;
	/** 友情引导可点击区域 */
	public UISprite clickKuang;

	private GuidePointSample point;
	private BoxCollider box;
	private Vector3 v3;

	float w = 0;
	float h = 0;
	float scale = 0;
	/** 需要显示箭头的对象 */
	private Transform specialTrans;
	/** 需要显示箭头的对象 */
	private Transform targetTrans;
	/** 是否是友善指引 */
	bool isFriendlyGuide = false;
	
	public void showGuide (int sid)
	{
		if (UiManager.Instance.getWindow<ExitWindow> () != null && UiManager.Instance.getWindow<ExitWindow> ().gameObject.activeSelf) {
			this.gameObject.SetActive (false);
		}
		if (UiManager.Instance.mainWindow != null) {
			UiManager.Instance.mainWindow.showIco ();
			UiManager.Instance.mainWindow.gridGuideIco ();
		}
		for (int i = 0; i < maskButtons.Length; i++) {
			maskButtons[i].onceGuide (false);
			maskButtons[i].initCallBack (null);
		}
		bool needShowArrow = false;

		point = GuidePointSampleManager.Instance.getGuidePointSampleBySid (sid);
		friendlyButtonMask.gameObject.SetActive (false);
		friendlyButton.gameObject.SetActive (false);
		clickKuang.gameObject.SetActive (false);

		#region 友情引导
		/**特殊情况的处理 */
		if (point.targetPath == "806001000") {//播放动画
			GuideManager.Instance.withoutFriendlyGuide ();
			UiManager.Instance.openWindow<NoticeWindow> ((win) => {
				win.entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeType.ONERMB_SID).entranceId;
				win.updateSelectButton (NoticeType.ONERMB_SID);//首冲条目写死
			});
			GuideManager.Instance.saveTimes(GuideManager.TypeCash);
			return;
		} 
		if (sid > 800000000) {

			isFriendlyGuide = true;
			closeGuideMask();
			friendlyButtonMask.gameObject.SetActive (true);
			clickKuang.gameObject.SetActive (true);

			for (int i = 0; i < friendlyMaskButton.Length; i++) {
				friendlyMaskButton[i].initCallBack (()=>{
					GuideManager.Instance.withoutFriendlyGuide ();
					closeGuide ();
				});
			}

			gameObject.SetActive (true);
			
			//先判断触控类型
			if (point.clickType == GuideClickType.SCREEN) {
				friendlyButton.onceGuide (true);
				friendlyButton.initCallBack (() => {
					GuideManager.Instance.doFriendlyGuideEvent ();
				});
				blinkLabel.gameObject.SetActive (true);
				friendlyButton.gameObject.SetActive (true);
			}
			else if (point.clickType == GuideClickType.SLIDE) {
				if (UiManager.Instance.mainWindow == null)
					return;
				clickKuang.gameObject.SetActive (false);
				UiManager.Instance.mainWindow.maskDragSV.enabled = false;
				StartCoroutine (Utils.DelayRun (() => {
					if (point.arrowRot == 180)
						UiManager.Instance.mainWindow.jumpToPage (2,true);
					else if (point.arrowRot == 0)
						UiManager.Instance.mainWindow.jumpToPage (0,true);
					else
						UiManager.Instance.mainWindow.jumpToPage (1,true);
				}, 0.5f));
				
			} else {
				friendlyButton.gameObject.SetActive (false);
			}

			//文字描述
			if (point.texLocal != GuideTexLocalType.NO) {
				if (sid >= GuideGlobal.ONCEGUIDE1)
					descript.text = GuideManager.Instance.getOnceGuideDesc ();
				else
					descript.text = GuideManager.Instance.getGuideDesc ();
				labelGroup.SetActive (true);
				labelGroup.transform.position = getTexLocal (point.texLocal);
			} else {
				labelGroup.SetActive (false);
			}



			guideArrow.transform.localEulerAngles = new Vector3 (0, 0, point.arrowRot);
			maxMask.gameObject.SetActive (false);
			if (get2DPointCoord (point.targetPath) == null) {
				MonoBase.print ("guideUI get2DPointCoord (point.targetPath) == null");
				return;
			}
			targetTrans = get2DPointCoord (point.targetPath);
			box = get2DPointCoord (point.targetPath).GetComponent<BoxCollider> ();

			if (box == null) {
				friendlyButtonMask.position = new Vector3 (get2DPointCoord (point.targetPath).position.x, get2DPointCoord (point.targetPath).position.y, friendlyButtonMask.position.z);
				return;
			}
			w = box.size.x;
			h = box.size.y;
			scale = UiManager.Instance.fScreenHW > 0.667f ? UiManager.Instance.screenScaleY : UiManager.Instance.screenScaleX;
			float sx = box.transform.localScale.x;
			float sy = box.transform.localScale.y;
			friendlyButtonMask.localScale = new Vector3 (w / 100f * scale * sx, h / 100f * scale * sy, 1);
			clickKuang.width = (int)w;
			clickKuang.height = (int)h;

			if (targetTrans != null) {
				UIDragScrollView dsv = targetTrans.GetComponent<UIDragScrollView> ();
				if (dsv != null) {
					dsv.enabled = false;
				}
			}

			return;
		} else {
			clickKuang.gameObject.SetActive (false);
			isFriendlyGuide = false;
		}

		#endregion

		#region 强制引导
		
		//事件触控类型判断,表示有箭头指示(全屏指示)
		if (point.clickType == GuideClickType.SCREEN) { 
			if (sid >= GuideGlobal.ONCEGUIDE1) {
				gameObject.SetActive (true);
				guideButton.onceGuide (true);
				if (sid == GuideGlobal.ONCEGUIDE_INVITE1) {
					guideButton.initCallBack (() => {
						GuideManager.Instance.onceGuideEvent (GuideGlobal.ONCEGUIDE_INVITE2);
					});
				}
			} else
				guideButton.initCallBack (GuideManager.Instance.guideEvent);

			if (point.targetPath != "7") {
				guideButton.gameObject.SetActive (true);
				blinkLabel.gameObject.SetActive (true);
			}
		}
		//滑动指示
		else if (point.clickType == GuideClickType.SLIDE) {
			if (UiManager.Instance.mainWindow == null)
				return;
			MaskWindow.LockUI();
			UiManager.Instance.mainWindow.maskDragSV.enabled = false;
			StartCoroutine (Utils.DelayRun (() => {
				if (point.arrowRot == 180)
					UiManager.Instance.mainWindow.jumpToPage (2,true);
				else if (point.arrowRot == 0)
					UiManager.Instance.mainWindow.jumpToPage (0,true);
				else
					UiManager.Instance.mainWindow.jumpToPage (1,true);
			}, 0.5f));
			
		}
		//友善按钮
		else if (point.clickType == GuideClickType.FRIENDLY_BUTTON) {
			guideButton.GetComponent<UIDragScrollView> ().enabled = false;
			guideButton.gameObject.SetActive (false);
			for (int i = 0; i < maskButtons.Length; i++) {
				maskButtons[i].onceGuide (true);
				maskButtons[i].initCallBack (()=>{
					GuideManager.Instance.jumpGuideSid ();
					closeGuide ();
				});
			}
		} else {
			for (int i = 0; i < maskButtons.Length; i++) {
				maskButtons[i].initCallBack (null);
			}
			guideButton.GetComponent<UIDragScrollView> ().enabled = false;
			guideButton.gameObject.SetActive (false);
		}

		//文字说明的位置判断
		if (point.texLocal != GuideTexLocalType.NO) {
			if (sid >= GuideGlobal.ONCEGUIDE1)
				descript.text = GuideManager.Instance.getOnceGuideDesc ();
			else
				descript.text = GuideManager.Instance.getGuideDesc ();
			labelGroup.SetActive (true);
			labelGroup.transform.position = getTexLocal (point.texLocal);
		} else {
			labelGroup.SetActive (false);
		}
		if (point.targetPath == "1") {//程序定位找主角
			bool isInSide = false;
			RoleView[] roles = GameObject.Find ("TeamEditWindow").GetComponent<TeamEditWindow> ().teamForRole;
			for (int i = 0; i < roles.Length; i++) {
				if (roles [i].card != null && roles [i].card.uid == UserManager.Instance.self.mainCardUid) {
					isInSide = true;
					specialTrans = roles [i].gameObject.transform;
					guideArrow.transform.localEulerAngles = new Vector3 (0, 0, point.arrowRot);
					needShowArrow = true;
					maxMask.gameObject.SetActive (false);
				}
			}
			if (!isInSide) {
				RoleView[] rolesSub = GameObject.Find ("TeamEditWindow").GetComponent<TeamEditWindow> ().teamSubRole;
				for (int i = 0; i < rolesSub.Length; i++) {
					if (rolesSub [i].card != null && rolesSub [i].card.uid == UserManager.Instance.self.mainCardUid) {
						specialTrans = rolesSub [i].gameObject.transform;
						guideArrow.transform.localEulerAngles = new Vector3 (0, 0, point.arrowRot);
						needShowArrow = true;
						maxMask.gameObject.SetActive (false);
					}
				}
			}
		} else if (point.targetPath == "7") {//播放动画

		} else if (point.targetPath == "6") {//定位卡片第一个被动技能
			CardBookWindow win = GameObject.Find ("CardBookWindow").GetComponent<CardBookWindow> ();
			GameObject a = win.transform.FindChild ("data/content/001/skill/skill/buttonSkill_1").gameObject;
			specialTrans = a.transform;
			guideArrow.transform.localEulerAngles = new Vector3 (0, 0, point.arrowRot);
			needShowArrow = true;
			maxMask.gameObject.SetActive (false);
		} else if (point.targetPath == "8") {//选择关卡
			MissionItem item = GameObject.Find ("MissionChooseWindow").GetComponent<MissionChooseWindow> ().getLastItem ();
			if (item != null) {
				specialTrans = item.transform;
				guideArrow.transform.localEulerAngles = new Vector3 (0, 0, point.arrowRot);
				needShowArrow = true;
				maxMask.gameObject.SetActive (false);
			}
		} else if (point.targetPath == "10") {//选择女神
			GameObject item = GameObject.Find ("GoddessWindow").GetComponent<GoddessWindow> ().getMyItem ().gameObject;
			if (item != null) {
				specialTrans = item.transform;
				guideArrow.transform.localEulerAngles = new Vector3 (0, 0, point.arrowRot);
				needShowArrow = true;
				maxMask.gameObject.SetActive (false);
			}
		} else {
			guideArrow.transform.localEulerAngles = new Vector3 (0, 0, point.arrowRot);
			needShowArrow = true;
			maxMask.gameObject.SetActive (false);
		}

		//这种情况是有箭头指示的
		if (needShowArrow) {
			//表示指示坐标要用2D方法取
			if (point.pointType == GuidePointType.POINT2D) {
				
				if (point.targetPath == "1" || point.targetPath == "6" || point.targetPath == "8" || point.targetPath == "10") {
					if (specialTrans == null)
						return;
					targetTrans = specialTrans;
					v3 = specialTrans.position;
					box = specialTrans.gameObject.GetComponent<BoxCollider> ();
				} else {
					if (get2DPointCoord (point.targetPath) == null) {
						MonoBase.print ("guideUI get2DPointCoord (point.targetPath) == null");
						return;
					}
					targetTrans = get2DPointCoord (point.targetPath);
					box = get2DPointCoord (point.targetPath).GetComponent<BoxCollider> ();
				}
				
				if (box == null) {
					mask.position = new Vector3 (get2DPointCoord (point.targetPath).position.x, get2DPointCoord (point.targetPath).position.y, mask.position.z);
					return;
				}
				w = box.size.x;
				h = box.size.y;
				scale = UiManager.Instance.fScreenHW > 0.667f ? UiManager.Instance.screenScaleY : UiManager.Instance.screenScaleX;
				float sx = box.transform.localScale.x;
				float sy = box.transform.localScale.y;
				mask.localScale = new Vector3 (w / 100f * scale * sx, h / 100f * scale * sy, 1);
			}
			//表示指示坐标要用3D方法取
			else {
				targetTrans = get3DPointCoordTransform (point.targetPath);
			}
		}
		if (targetTrans != null) {
			UIDragScrollView dsv = targetTrans.GetComponent<UIDragScrollView> ();
			if (dsv != null) {
				dsv.enabled = false;
			}
		}
		//强制引导最后一步，只要箭头，没有遮罩，5秒后玩家没操作就自动进入下一步
//		if (GuideManager.Instance.isEqualStep (30001000)) {
//			closeGuideMask ();
//			StartCoroutine (Utils.DelayRun (()=>{
//				if (GuideManager.Instance.isEqualStep (30001000)) {
//					GuideManager.Instance.doGuide ();
//				}
//			},5f));
//		}
	}

	#endregion
	
	private Transform get2DPointCoord (string path)
	{
		if (UiManager.Instance.UIScaleRoot.transform.FindChild (path) == null)
			return null;
		return UiManager.Instance.UIScaleRoot.transform.FindChild (path);
	}
	
	private Vector3 get3DPointCoord (string path)
	{
		Vector3 v3 = MissionManager.instance.backGroundCamera.WorldToScreenPoint (GameObject.Find (path).transform.GetChild (0).transform.position);
		v3.z = 0;
		return UiManager.Instance.gameCamera.ScreenToWorldPoint (v3);
	}

	private Transform get3DPointCoordTransform (string path)
	{
		return GameObject.Find (path).transform.GetChild (0).transform;
	}
	
	private Vector3 getTexLocal (int localType)
	{
		switch (localType) {
		case GuideTexLocalType.TOP://上
			return new Vector3 (0, 0.5f, 0);
		case GuideTexLocalType.CENTER://中
			return new Vector3 (0, 0, 0);
		case GuideTexLocalType.BOTTOM:
			return new Vector3 (0, -0.5f, 0);
		default:
			return new Vector3 (0, 0, 0);
		}
	}
	
	public void hideGuide ()
	{
		point = null;
		labelGroup.SetActive (false);
		guideArrow.gameObject.SetActive (false);
		blinkLabel.gameObject.SetActive (false);
		friendlyButtonMask.gameObject.SetActive (false);
		friendlyButton.gameObject.SetActive (false);
		clickKuang.gameObject.SetActive (false);

		if (targetTrans != null) {
			UIDragScrollView dsv = targetTrans.GetComponent<UIDragScrollView> ();
			if (dsv != null) {
				dsv.enabled = true;
			}
			targetTrans = null;
		}
	}
	
	public void closeGuide ()
	{
		point = null;
		labelGroup.SetActive (false);
		guideArrow.gameObject.SetActive (false);
		blinkLabel.gameObject.SetActive (false);
		guideButton.gameObject.SetActive (false);
		maxMask.SetActive (false);
		mask.gameObject.SetActive (false);
		friendlyButtonMask.gameObject.SetActive (false);
		friendlyButton.gameObject.SetActive (false);
		clickKuang.gameObject.SetActive (false);

		if (targetTrans != null) {
			UIDragScrollView dsv = targetTrans.GetComponent<UIDragScrollView> ();
			if (dsv != null) {
				dsv.enabled = true;
			}
			targetTrans = null;
		}
	}

	public void closeGuideMask ()
	{
		guideButton.gameObject.SetActive (false);
		maxMask.SetActive (false);
		mask.gameObject.SetActive (false);
	}

	void Update ()
	{
		blinkLabel.alpha = sin (); 

		if (targetTrans != null) {
			if (point.pointType == GuidePointType.POINT2D) {
				guideArrow.position = new Vector3 (targetTrans.position.x, targetTrans.position.y, guideArrow.position.z);
				if (mask.gameObject.activeSelf) {
					mask.position = new Vector3 (guideArrow.position.x, guideArrow.position.y, mask.position.z);
				}
				if (friendlyButtonMask.gameObject.activeSelf) {
					friendlyButtonMask.position = new Vector3 (guideArrow.position.x, guideArrow.position.y, mask.position.z);
				}
				if (clickKuang.gameObject.activeSelf) {
					clickKuang.transform.position = new Vector3 (targetTrans.position.x, targetTrans.position.y, clickKuang.transform.position.z);
				}
			} else {
				Vector3 v = MissionManager.instance.backGroundCamera.WorldToScreenPoint (targetTrans.position);
				v.z = 0;
				guideArrow.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (v);
			}
			if (point.clickType == GuideClickType.SLIDE || isFriendlyGuide) {
				arrowTex.alpha = 0;
			} else if (!guideArrow.gameObject.activeInHierarchy && !isFriendlyGuide) {
				arrowTex.alpha = 1;
				guideArrow.gameObject.SetActive (true);
			}
		}
	}
}
