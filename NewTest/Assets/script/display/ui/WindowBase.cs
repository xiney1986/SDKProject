using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
/** 
  * 窗口基类
  * @author 李程
  * */
public enum enum_titleStyle
{
	NoTitle,
	DefaultTitle,
	CustomTitle,
	NewTitle,
}

[RequireComponent (typeof(UIPanel))]
public class WindowBase : MonoBase,IResUser,IResCacher
{

	bool openNewWindow = false;
	Vector2 OldScreenPosition;
	UIPanel m_panel;
	public Vector2 beginPosition;
	public GUI_anim_class windowAnim;
	public GUI_state windowState;
	public UISprite darkBackGroundSprite; //find NGUI_darkBackGround
	protected WindowBase fatherWindow;
	public bool neverBeActive = false;
	[HideInInspector]
	public bool
		DestroyWhenClose = false;
	public bool isDialogWindow = false;
    public bool showHelpButon = false; //显示帮助按钮
	public UIPlayTween TweenerGroup;
	public GameObject animRoot;
	public string  windowTitle;
	public enum_titleStyle  titleStyle;//自定义上下title
	UILabel  titlelabel;
	UISprite iconSprite;
	bool  _isNewWindow = true;
	[HideInInspector]
	public ResourcesData
		useResData;
	[HideInInspector]
	public bool
		isAwakeformHide = false;
	Vector2 ScreenScaleNow = new  Vector2 (1, 1);
	bool isDestoryed;
	bool isFinishing;

	[HideInInspector]
	public bool IsDestoryed{ get { return isDestoryed; } }
	
	[HideInInspector]
	public List<EventDelegate>
		onDestroy = new List<EventDelegate> ();
	[HideInInspector]
	public List<EventDelegate>
		OnHide = new List<EventDelegate> ();
	[HideInInspector]
	public List<EventDelegate>
		OnStartAnimFinish = new List<EventDelegate> ();
	[HideInInspector]
	public bool
		dialogCloseUnlockUI = true; // dialog窗口销毁时是否解锁
	public bool ReleaseAssetBundleWhenDestory = true;//自动释放资源包
    private CallBack cl;
	[ContextMenu ("Do Something")]
	public virtual void OnTapUpdate ()
	{

	}

	public void setWindowOutAnim (bool isNew)
	{
		openNewWindow = isNew;
		//关闭窗口右出,开新窗口左出
		if (isNew) {
			windowAnim = GUI_anim_class.RightInLeftOut;
		} else {
			windowAnim = GUI_anim_class.RightInRightOut;
		}

	}

	public bool isNewWindow ()
	{
		return _isNewWindow;
	}

	public void setResourcesData (ResourcesData resData)
	{
		useResData = resData;
	}

	public void releaseResourcesData ()
	{
		if (useResData != null && ResourcesManager.Instance!=null ) {
			ResourcesManager.Instance.UnloadAssetUIBundleBlock (useResData.ResourcesName);
			useResData = null;
		}
	}

	public void TweenerGroupIn ()
	{
		TweenerGroup.playDirection = AnimationOrTween.Direction.Forward;

		UITweener[] tws = TweenerGroup.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0.4f;
		}
		TweenerGroup.Play (true);
		
	}

	public	void TweenerGroupOut ()
	{
		TweenerGroup.playDirection = AnimationOrTween.Direction.Reverse;
		UITweener[] tws = TweenerGroup.GetComponentsInChildren<UITweener> ();
		foreach (UITweener each in tws) {
			each.delay = 0;
		}

		TweenerGroup.Play (true);
	}

	public	void OnTweenerGroupFinish ()
	{

 
	}

	public void SetChildScale ()
	{
		return;
		/*
		if (_isNewWindow == false)
			return;

 
 
		if (autoFillGroup .Length > 0) {
			
			
			foreach (GameObject each in autoFillGroup) {
				if (each == null)
					continue;

				if (UiManager.Instance.fScreenHW > 0.667f && (UiManager.Instance.screenScaleX != 1f || UiManager.Instance.screenScaleY != 1f)) {
					setFillHeight (each);
					
					continue;
				} else {
				
					setFillWidth (each);
					continue;
				}

			}
 
			
			
		}
		
		
		
 
		if (allFillGroup.Length > 0) {
			
			
			foreach (GameObject each in allFillGroup) {
				if (each == null)
					continue;
			
				if (UiManager.Instance.screenScaleX != 1f || UiManager.Instance.screenScaleY != 1f) {
				
					each.transform.localPosition = new  Vector3 (each.transform.localPosition.x * UiManager.Instance.screenScaleX, each.transform.localPosition.y * UiManager.Instance.screenScaleY, each.transform.localPosition.z);
					each.transform.localScale = new Vector3 (each.transform.localScale.x * UiManager.Instance.screenScaleX, each.transform.localScale.y * UiManager.Instance.screenScaleY, 1f);
			
				}

			}
			

			
		}

		if (fillHeightGroup .Length > 0) {
			
			foreach (GameObject each in fillHeightGroup) {
				if (each == null)
					continue;
			
				if (UiManager.Instance.screenScaleX != 1f || UiManager.Instance.screenScaleY != 1f) {
				
					setFillHeight (each);
				}

			}
			
			
			
		}
		
		
		if (fillWidthGroup .Length > 0) {
			
			foreach (GameObject each in fillWidthGroup) {
				if (each == null)
					continue;
			
				if (UiManager.Instance.screenScaleX != 1f || UiManager.Instance.screenScaleY != 1f) {
				
					setFillWidth (each);
				}

			}
			
			
 
		}

 

 */
		
	}
	
	public virtual IEnumerator cacheData ()
	{
		
		yield break;
		
	}

	public void Notify (object sender)
	{
		
	}
	
	public WindowBase GetFatherWindow ()
	{
		return fatherWindow;
	}

	public virtual	void OnDrop (GameObject drag)
	{


		if (typeof(TeamEditWindow) == GetType ()) {		
			(this as TeamEditWindow).releaseCard (drag);
		}


	}
	
	public void SetFatherWindow (WindowBase window)
	{
		fatherWindow = window;
	}
 
	public virtual void  OnAwake ()
	{

	}
	// Use this for initialization
	void Awake ()
	{
		_isNewWindow = true;

		if (!isDialogWindow) {
			//默认是左进左出
			windowAnim = GUI_anim_class.RightInRightOut;

			//常规窗口有titlebar
			if (titleStyle == enum_titleStyle.DefaultTitle) {
				GameObject obj = Create3Dobj ("UI/titleTweenGroupNew").obj;
				TweenerGroup = obj.GetComponent<UIPlayTween> ();
				obj.transform.parent = transform;
				obj.transform.localScale = Vector3.one;
				//语言包转换
				titlelabel = obj.transform.FindChild ("top/titleLabel").GetComponent<UILabel> ();
				iconSprite = obj.transform.FindChild ("top/iconSprite").GetComponent<UISprite> ();
				titlelabel.text = LanguageConfigManager.Instance.coverText (windowTitle);
				obj.transform.FindChild ("top/close").GetComponent<ButtonBase> ().fatherWindow = this;
			} else if (titleStyle == enum_titleStyle.NewTitle) {
				GameObject obj = Create3Dobj ("UI/titleTweenGroupNew").obj;
				TweenerGroup = obj.GetComponent<UIPlayTween> ();
				obj.transform.parent = transform;
				obj.transform.localScale = Vector3.one;
				//语言包转换
				titlelabel = obj.transform.FindChild ("top/titleLabel").GetComponent<UILabel> ();
				iconSprite = obj.transform.FindChild ("top/iconSprite").GetComponent<UISprite> ();
				titlelabel.text = LanguageConfigManager.Instance.coverText (windowTitle);
				obj.transform.FindChild ("top/close").GetComponent<ButtonBase> ().fatherWindow = this;
                if (showHelpButon) {
                    GameObject helpObj = obj.transform.FindChild("top/buttonHelp").gameObject;
                    helpObj.SetActive(true);
                    helpObj.GetComponent<ButtonBase>().fatherWindow = this;
                    //obj.transform.FindChild("top/buttonHelp").GetComponent<ButtonBase>().fatherWindow = this;
                }
			} 
		}
		prepareWindowForAnim ();
		OnAwake ();
	
	}

	public void setTitle (string str)
	{
		if (titlelabel != null) {
			titlelabel.text = str;
		}
	}

	public string getTitle ()
	{
		return titlelabel.text;
	}

	public void setTitle (string sname, string strtitile)
	{
		if (iconSprite != null) {
			iconSprite.gameObject.SetActive (true);
			iconSprite.spriteName = sname;
		}
		if (titlelabel != null) {
			titlelabel.text = strtitile;
			titlelabel.pivot = UIWidget.Pivot.Left;
			titlelabel.effectStyle = UILabel.Effect.Outline;
			titlelabel.effectColor = new Color32 (0, 1, 0, 255);
			titlelabel.color = new Color32 (0, 213, 255, 255);
		}
	}
	/// <summary>
	/// 关闭动画完成后销毁窗口
	/// </summary>
	public void finishWindow ()
	{
		if (!isFinishing) {
			isFinishing = true;
			DestroyWhenClose = true;
			hideWindow ();

			if (!isDialogWindow && !openNewWindow)
				UiManager.Instance.OnBackPressed ();
		}
	}

	public void restoreWindow ()
	{
		if (!isDialogWindow)
			windowAnim = GUI_anim_class.LeftInRightOut;

		prepareWindowForAnim ();
		gameObject.SetActive (true);
		openNewWindow = false;
	}
    public void restoreWindow(CallBack callBac)
    {
        cl = callBac;
        if (!isDialogWindow)
            windowAnim = GUI_anim_class.LeftInRightOut;

        prepareWindowForAnim();
        gameObject.SetActive(true);
        openNewWindow = false;
    }

	void prepareWindowForAnim ()
	{
//		print ("prepareWindowForAnim");
		transform.parent = UiManager.Instance.UIScaleRoot.transform;
		OldScreenPosition = Vector2.zero;

		if (animRoot != null) {
			if (windowAnim == GUI_anim_class.LineDownBack || windowAnim == GUI_anim_class.OutBounce)
				OldScreenPosition = new Vector2 (beginPosition.x, beginPosition.y + 500);
			if (windowAnim == GUI_anim_class.LeftInRightOut || windowAnim == GUI_anim_class.LeftIn || windowAnim == GUI_anim_class.LeftInLeftOut)
				OldScreenPosition = new Vector2 (beginPosition.x - 640, beginPosition.y);		
			if (windowAnim == GUI_anim_class.RightInLeftOut || windowAnim == GUI_anim_class.RightIn || windowAnim == GUI_anim_class.RightInRightOut)
				OldScreenPosition = new Vector2 (beginPosition.x + 640, beginPosition.y);	

			animRoot.transform.localPosition = new Vector3 (OldScreenPosition.x, OldScreenPosition.y, animRoot.transform.localPosition.z);
		} 
	
		//	animRoot = transform.FindChild ("root").gameObject;


		if (windowAnim == GUI_anim_class.OutBounce) {
			changeScale (new Vector3 (0.1f, 0.1f, 0.1f));
		} else {
			changeScale (Vector3.one);
		}
		//深度调整
		transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, -GetComponent<UIPanel> ().depth);
		transform.localScale = Vector3.one;
	}

	public  void changePosition (Vector2 position)
	{
		animRoot.transform.localPosition = new Vector3 (position.x, position.y, animRoot.transform.localPosition.z);	
		
	}

	protected virtual void DoEnable ()
	{
		UiManager.Instance.backGround.switchToDark();
	}
	
	protected virtual void DoUpdate ()
	{
	 
		
 

	}

	public  virtual void buttonEventBase (GameObject gameObj)
	{
		
		
		
	}
	
	public  virtual void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		
		
	}
	
	protected virtual void begin ()
	{

			print ("window begin");

	}
	
	void windowStartAnimComplete ()
	{
		changeAlpha (1);
		changeState (GUI_state.normal);
		// DragCollider.enabled=true;
		OnStartAnimComplete ();

		
		StartCoroutine (Utils.DelayRunNextFrame (() => {
			begin ();}));


//		print (transform.localPosition);
		//	isAwakeformHide = false;
	}

	public virtual void  OnStartAnimComplete ()
	{
	    if (cl != null) cl();
	    cl = null;
		EventDelegate.Execute (OnStartAnimFinish);
	}
	
	public virtual void  OnOverAnimComplete ()
	{

		EventDelegate.Execute (OnHide);
	}
	
	void MoveWindow (Vector2 position)
	{
		
		changePosition (position);
		
		
	}
	
	void ScaleWindow (float scaleP)
	{
		
		changeScale (new Vector3 (scaleP, scaleP, scaleP));
		
		
	}
	
	void AlphaWindow (float alpha)
	{
		changeAlpha (alpha);
	}
	
	void changeScale (Vector3 scaleP)
	{
		if (animRoot != null)
			animRoot.transform.localScale = new  Vector3 (scaleP.x, scaleP.y, scaleP.z);
	}
	
	public void changeAlpha (float p_alpha)
	{
		if (windowAnim != GUI_anim_class.AlphaInOut)
			return;
		if (m_panel == null)
			m_panel = GetComponent<UIPanel> ();
		if (m_panel == null)
			return;
		m_panel.alpha = p_alpha;
	}
	
	void windowOverAnimComplete ()
	{
		OnOverAnimComplete ();
		changeState (GUI_state.NeverStart);
		if (DestroyWhenClose == true) {
			
			if (gameObject != null)
				Destroy (gameObject);		
			
		} else {
			isAwakeformHide = true;
			gameObject.SetActive (false);
		}
	}
 
	public virtual void destoryWindow ()
	{
		changeState (GUI_state.NeverStart);
		if (this != null && gameObject != null)
			Destroy (gameObject);	
		isDestoryed = true;
	}

	void StartAnim ()
	{

		if (windowState == GUI_state.Onstart)
			return;	

		//MaskWindow.LockUI();

		changeState (GUI_state.Onstart);
		changeScale (new Vector3 (1, 1, 1));
		
		if (gameObject.GetComponents<iTween> () != null) {
			foreach (iTween each in gameObject.GetComponents<iTween>()) {
				DestroyImmediate (each);
			}
		}

		if (TweenerGroup != null)
			TweenerGroupIn ();
		
		
		
		if (darkBackGroundSprite != null) {

			UITweener tween = darkBackGroundSprite.GetComponent<UITweener> ();
		 
			if (tween != null)
				tween.Play (true);
 
		}
		
		if (windowAnim == GUI_anim_class.None) {
			transform.localPosition = new Vector3 (beginPosition.x, beginPosition.y, transform.localPosition.z);
			windowStartAnimComplete ();
			return;
		}
		 
		if (windowAnim == GUI_anim_class.OutBounce) {
				 
			//	transform.localPosition = new Vector3 (OldScreenPosition.x, OldScreenPosition.y, transform.localPosition.z); 
			iTween.ValueTo (animRoot, iTween.Hash ("from", OldScreenPosition, "to", beginPosition, "onupdatetarget", gameObject, "onupdate", "MoveWindow", "easetype", "easeoutbounce", "oncompletetarget", gameObject, "oncomplete", "windowStartAnimComplete", "time", 0.5f * UiManager.getAnimSpeed ()));
	
			return;
		}	
		if (windowAnim == GUI_anim_class.LineDownBack) {
			 
			//	transform.localPosition = new Vector3 (OldScreenPosition.x, OldScreenPosition.y, transform.localPosition.z);
			
			
			iTween.ValueTo (animRoot, iTween.Hash ("from", OldScreenPosition, "to", beginPosition, "onupdatetarget", gameObject, "onupdate", "MoveWindow", "easetype", "easeoutback", "oncompletetarget", gameObject, "oncomplete", "windowStartAnimComplete", "time", 0.5f * UiManager.getAnimSpeed ()));
		 
			
			return;

		}
		if (windowAnim == GUI_anim_class.ScaleOutBack) {
			changeScale (new Vector3 (0.5f, 0.5f, 0.5f));
			iTween.ValueTo (animRoot, iTween.Hash ("from", 0.5f, "to", 1f, "onupdatetarget", gameObject, "onupdate", "ScaleWindow", "easetype", "easeoutback", "oncompletetarget", gameObject, "oncomplete", "windowStartAnimComplete", "time", 0.5f * UiManager.getAnimSpeed ()));
			return;

		}
		 
		if (windowAnim == GUI_anim_class.AlphaInOut) {
		 
		
			iTween.ValueTo (animRoot, iTween.Hash ("from", 0.0f, "to", 1f, "onupdatetarget", gameObject, "onupdate", "AlphaWindow", "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "windowStartAnimComplete", "time", 0.5f * UiManager.getAnimSpeed ()));
			 
			return;
		}
		
		if (windowAnim == GUI_anim_class.LeftInRightOut || windowAnim == GUI_anim_class.LeftIn || windowAnim == GUI_anim_class.LeftInLeftOut) {
		 

			iTween.ValueTo (animRoot, iTween.Hash ("from", OldScreenPosition, "to", beginPosition, "onupdatetarget", gameObject, "onupdate", "MoveWindow", "easetype", iTween.EaseType.easeInOutCubic, "oncompletetarget", gameObject, "oncomplete", "windowStartAnimComplete", "time", 0.6f * UiManager.getAnimSpeed ()));
		 			 
			return;
		}
		
		if (windowAnim == GUI_anim_class.RightInLeftOut || windowAnim == GUI_anim_class.RightIn || windowAnim == GUI_anim_class.RightInRightOut) {

			iTween.ValueTo (animRoot, iTween.Hash ("from", OldScreenPosition, "to", beginPosition, "onupdatetarget", gameObject, "onupdate", "MoveWindow", "easetype", iTween.EaseType.easeInOutCubic, "oncompletetarget", gameObject, "oncomplete", "windowStartAnimComplete", "time", 0.6f * UiManager.getAnimSpeed ()));
		  
			return;
		}		
		
	}
	
	public void hideWindow ()
	{
		if (windowState == GUI_state.OnOver || windowState == GUI_state.NeverStart) {
			OnOverAnimComplete ();
			return;	
		}
		OnBeginCloseWindow ();
		EndAnim ();

	}
	
	public void hideWindow (float time)
	{
		if (windowState != GUI_state.normal)
			return;
		
		StartCoroutine (delayDelete (time));
		
	}
	
	IEnumerator delayDelete (float time)
	{
		yield return  new WaitForSeconds (time);
		
		hideWindow ();
		
	}
	
	public virtual void OnBeginCloseWindow ()
	{
		
		
		//	print("I begin close window");
		
	}
	
	void EndAnim ()
	{
		if (darkBackGroundSprite != null) {
			UITweener tween = darkBackGroundSprite.GetComponent<UITweener> ();
			tween.Play (false);
 
		}
			
		changeState (GUI_state.OnOver);
				
		if (windowAnim == GUI_anim_class.None || windowAnim == GUI_anim_class.LeftIn || windowAnim == GUI_anim_class.RightIn) {
			windowOverAnimComplete ();
			return;
		}

		if (TweenerGroup != null)
			TweenerGroupOut ();
		
		if (windowAnim == GUI_anim_class.OutBounce) {
				 
			iTween.ValueTo (animRoot, iTween.Hash ("from", beginPosition, "to", OldScreenPosition, "onupdatetarget", gameObject, "onupdate", "MoveWindow", "easetype", "easeinback", "oncompletetarget", gameObject, "oncomplete", "windowOverAnimComplete", "time", 0.5f * UiManager.getAnimSpeed ()));
	

		}	
		if (windowAnim == GUI_anim_class.LineDownBack) {
				 
			iTween.ValueTo (animRoot, iTween.Hash ("from", beginPosition, "to", OldScreenPosition, "onupdatetarget", gameObject, "onupdate", "MoveWindow", "easetype", "easeinback", "oncompletetarget", gameObject, "oncomplete", "windowOverAnimComplete", "time", 0.5f * UiManager.getAnimSpeed ()));
			 

		}
		if (windowAnim == GUI_anim_class.ScaleOutBack) {
				 
			iTween.ValueTo (animRoot, iTween.Hash ("from", 1f, "to", 0.1f, "onupdatetarget", gameObject, "onupdate", "ScaleWindow", "easetype", "easeinback", "oncompletetarget", gameObject, "oncomplete", "windowOverAnimComplete", "time", 0.5f * UiManager.getAnimSpeed ()));
			 

		}
		 
		if (windowAnim == GUI_anim_class.AlphaInOut) {
				 
			iTween.ValueTo (animRoot, iTween.Hash ("from", 1f, "to", 0f, "onupdatetarget", gameObject, "onupdate", "AlphaWindow", "easetype", iTween.EaseType.easeInOutQuad, "oncompletetarget", gameObject, "oncomplete", "windowOverAnimComplete", "time", 0.5f * UiManager.getAnimSpeed ()));
			 

		}
		
		if (windowAnim == GUI_anim_class.LeftInRightOut || windowAnim == GUI_anim_class.RightInRightOut) {
				 
			iTween.ValueTo (animRoot, iTween.Hash ("from", beginPosition, "to", OldScreenPosition + new Vector2 (1280, 0), "onupdatetarget", gameObject, "onupdate", "MoveWindow", "easetype", iTween.EaseType.easeInOutCubic, "oncompletetarget", gameObject, "oncomplete", "windowOverAnimComplete", "time", 0.6f * UiManager.getAnimSpeed ()));
	

		}	
		
		if (windowAnim == GUI_anim_class.RightInLeftOut || windowAnim == GUI_anim_class.LeftInLeftOut) {
				 
			iTween.ValueTo (animRoot, iTween.Hash ("from", beginPosition, "to", OldScreenPosition + new Vector2 (-1280, 0), "onupdatetarget", gameObject, "onupdate", "MoveWindow", "easetype", iTween.EaseType.easeInOutCubic, "oncompletetarget", gameObject, "oncomplete", "windowOverAnimComplete", "time", 0.6f * UiManager.getAnimSpeed ()));
	

		}	
		
		
		
		
	}

	public virtual void OnNetResume ()
	{
		
	}

	public virtual void DoDisable ()
	{

		//特殊处理,上级界面存在
		if (isDialogWindow && dialogCloseUnlockUI)
			MaskWindow.UnlockUI ();

	}
 
	void OnEnable ()
	{
		DoEnable ();
		startWindow ();

	}
	
	public void startWindow ()
	{
		StartAnim ();
		_isNewWindow = false;
	}

	void OnDisable ()
	{
		
		if (windowState == GUI_state.normal)
			windowState = GUI_state.NeverStart;
		DoDisable ();

	}

	void OnDestroy ()
	{
		isDestoryed = true;
        
		EventDelegate.Execute (onDestroy);

		if (ReleaseAssetBundleWhenDestory)
			releaseResourcesData ();
        iTween.Stop(gameObject,true);
//		if (ResourcesManager.Instance != null)
//			ResourcesManager.Instance.releaseTextureResources ();
	}
	
	void Start ()
	{
		OnStart ();
	}

	public virtual void OnStart ()
	{

	}

	public virtual void DoClickEvent ()
	{
		
	 

	}
	
	public virtual void DoDragEvent ()
	{
		//	NGUI_manager.changeActiveWindow(this);
		
 
		
	}
 
	public virtual void On_PVZItemOptionMessageBoxChange (int num, string id)
	{
	
 
		
		
	}
 
	void OnClick ()
	{
		//print (gameObject.name);
		//	NGUI_manager.changeActiveWindow(this);
		//sinaWYX_manager.ErrorCode+=gameObject.name;
		DoClickEvent ();
		
		
	}
	
	public void  changeState (GUI_state state)
	{
		
		 
		
		windowState = state;
		
	}
	
	public GUI_state getState ()
	{
		
		
		return windowState;
	}
	
	
	// Update is called once per frame
	void Update ()
	{
//		 print ("@@@@"+transform.localPosition);
 
		DoUpdate ();
		
	}

}
