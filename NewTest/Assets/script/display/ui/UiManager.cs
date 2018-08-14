using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
  * UI管理器
  * @author 李程
  * */
public enum GUI_anim_class
{
	None,
	AlphaInOut, // alpha 0-1 in and close 1-0 alpha out
	LineDownBack, // moveup to in and move down to leave with alpha in out.
	ScaleOutBack, //scale big to small and scale small to big leave whith alpha in out.
	OutBounce,
	LeftInRightOut,
	RightInLeftOut,
	LeftIn,
	RightIn,
	LeftInLeftOut,	
	RightInRightOut,	
}

public enum GUI_state
{
	NeverStart,
	Onstart,
	normal,
	Avticeing,
	Avticed,
	OnOver,
	Disable,
}

public enum screenSwitch_type
{
	switchToBack,
	switchToScreen,
 
}

public class UiManager
{
	
	const float m_animSpeed = 1f;
	public static Color32[] qualityColor = new Color32[]{
		new Color32 (255, 255, 255, 255),
		new Color32 (255, 255, 255, 255),
		new Color32 (36, 209, 0, 255),
		new Color32 (59, 206, 255, 255),
		new Color32 (143, 59, 255, 255),
		new Color32 (255, 53, 252, 255),
		new Color32 (255, 155, 0, 255),
		new Color32 (248, 5, 0, 255),
		new Color32 (248, 5, 0, 255),
		new Color32 (166, 5, 0, 255),
		new Color32 (166, 5, 0, 255),
		new Color32 (166, 5, 0, 255),	
	
	
	
	
	};
	struct maskRect
	{
		public 	Transform body;
		public 	UISprite top;
		public	UISprite left;
		public	UISprite right;
		public	UISprite bottom;
 
	}
	public   UIRoot NGUIroot;
	public   Camera gameCamera;
	maskRect m_maskRect;
	public  GameObject UIScaleRoot;
	public  GameObject UIEffectRoot;
	public  BetterList<WindowBase> windowList;
	public  LoadingWindow ActiveLoadingWindow;
	public  MainWindow mainWindow;
	public  MissionMainWindow missionMainWindow;
	public  BackGroundCtrl backGround;
	public  MaskWindow maskWindow;
    public  rechargeWindow rechargeWWindow;
	public  EmptyWindow emptyWindow;
	public MissionChooseWindow storyMissionWindow;
	public IntensifyCardWindow intensifyCardWindow;
	public CardBookWindow cardBookWindow;
	public StoreWindow storeWindow;
	public NoticeWindow noticeWindow;
	public TeamEditWindow teamEditWindow;
	public BattleWindow battleWindow;
	public GuildFightMainWindow guildFightMainWindow;
    public MagicWeaponStoreWindow magicWeaponStoreWindow;
	static UiManager m_instance;
    public PveUseWindow pveUseWindow;
    public GodsWarSuportWindow godsWarSuportWindow;
    public LaddersRankRewardWindow laddersRankRewardWindow;
    public LaddersChestsWindow laddersChestsWindow;
    public LevelupRewardWindow levelupRewardWindow;
    //跨服战相关
    public GodsWarFinalWindow godsWarFinalWindow;
    public GodsWarFinalRankWindow godsWarFinalRankWindow;
    public GodsWarMySuportWindow godsWarMySuportWindow;
    public GodsWarProgramWindow godsWarProgramWindow;
    //==========================================
    public GodsWarGroupStageWindow godsWarGroupStageWindow;
    public GodsWarReplayWindow godsWarReplayWindow;
    public GodsWarUserInfoWindow godsWarUserInfoWindow;
    public BuyWindow godsBuyWind;
    public BattlePrepareWindowNew battlePrepareWindowNew;
    public bool isInGodsBattle;
	public  float		fScreenHW;
	public  	 float	 screenScaleY;
	public 	 float	 screenScaleX;
	public const int HIDELAYER = 10;
	public const int UILAYER = 8;
	Stack<WindowBase> windowStack;
	
	IEnumerator  screenRote ()
	{
		Screen.orientation = ScreenOrientation.Portrait;
		yield return  new WaitForSeconds (0.1f);
		GameManager.Instance.ScreenReady ();

	}

	public void hideLoading ()
	{
		ActiveLoadingWindow.finishWindow ();
	}

	public void applyMask ()
	{
		// maskWindow.gameObject.SetActive(true);
	}

	public void cancelMask ()
	{
		//maskWindow.gameObject.SetActive(false);
	}

	public void applyMask2 ()
	{
		//maskWindow.gameObject.SetActive(true);
	}
    
	public void cancelMask2 ()
	{
		//maskWindow.gameObject.SetActive (false);
	}

	public void initNewPlayerGuideLayer ()
	{
		GameObject mask = UiManager.Instance.UIScaleRoot.transform.FindChild ("guideLayer").gameObject;
		mask.SetActive (true);
	}

	public IEnumerator   setMask ()
	{
		m_maskRect.body = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/mask").transform;
		m_maskRect.top = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/mask/Top").GetComponent<UISprite> ();
		m_maskRect.bottom = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/mask/Bottom").GetComponent<UISprite> ();	
		m_maskRect.left = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/mask/Left").GetComponent<UISprite> ();	
		m_maskRect.right = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/mask/Right").GetComponent<UISprite> ();	

//	m_maskRect.screenSwitch=GameObject.Find("/NGUI_manager/GameCamera/UIScaleRoot/mask/Switch").GetComponent<SwitchCtrl>();	
//	m_maskRect.screenSwitch.gameObject.SetActive(false);
		if (fScreenHW > 0.667f) {
			m_maskRect.body.localScale = new Vector3 (screenScaleY, screenScaleY, 1);

			
		} else {
			m_maskRect.body.localScale = new Vector3 (screenScaleX, screenScaleX, 1);
		}
		yield return new WaitForSeconds (0.1f);
		m_maskRect.top.color = Color.white;
		m_maskRect.bottom.color = Color.white;
		m_maskRect.left .color = Color.white;
		m_maskRect.right.color = Color.white;
		
	}
		
	public static float getAnimSpeed ()
	{
		return m_animSpeed;
	}

	public void openMainWindow ()
	{
		//ResourcesManager.Instance.	releaseUIResources();
		MaskWindow.LockUI();
		openWindow<MainWindow> ();
	}
	
	public static UiManager Instance {
		get {
			if (m_instance == null) {
				m_instance = new UiManager ();
				
			}
			return m_instance;
		}
		set {
			m_instance = value;
		}
		
	}

	public void init ()
	{
		windowStack = new Stack<WindowBase> ();
		MonoBase.Create3Dobj ("NGUI").obj.name = "NGUI";
		MonoBase.Create3Dobj ("Base/NGUI_manager").obj.name = "NGUI_manager";
		NGUIroot = GameObject.Find ("NGUI_manager").GetComponent<UIRoot> ();		
		MonoBase.DontDestroyOnLoad (NGUIroot.gameObject);
		UIScaleRoot = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot").gameObject;	  
		UIEffectRoot = GameObject.Find ("/NGUI_manager/GameCamera/UIScaleRoot/UIEffectRoot").gameObject;    
		gameCamera = GameObject .Find ("/NGUI_manager/GameCamera").GetComponent<Camera> ();
		windowList = new BetterList<WindowBase> ();
		maskWindow = UIScaleRoot.transform.FindChild ("maskWindow").gameObject.GetComponent<MaskWindow> ();  
		MonoBase.Create3Dobj ("Effect/Other/3D_background").obj.name = "3D_background";
		backGround = GameObject.Find ("3D_background").gameObject.GetComponent<BackGroundCtrl> ();
		GameManager.Instance.StartCoroutine (screenRote ());
		Time.timeScale = GameManager.Instance.gameSpeed;
		Debug.Log ("Ui init");
	}

	public static Vector2 getMousePosition ()
	{
		if (UiManager.Instance.gameCamera == null)
			return Vector2.zero;
		Vector3 tmp = UiManager.Instance.gameCamera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
		return new Vector2 (tmp.x, tmp.y);
	 
	}

	public static Vector3 objToUIscaleRootPosition (Transform a)
	{
		
		if (UiManager.Instance.UIScaleRoot == null)
			return Vector3.zero;
		Matrix4x4 matri = UiManager.Instance.UIScaleRoot .transform.worldToLocalMatrix;
 
		return matri.MultiplyPoint3x4 (a.position);
		
	}

	public static void  getSpriteName (UISprite ICO, string URL)
	{
		//check format:
		if (ICO == null)
			return;
		if (URL == "" || URL == null)
			return;
		if (URL.Length < 6)
			return;
		
		//get substring like "10.jpg" or "/5.jpg"
		ICO.spriteName = URL.Substring (URL.Length - 6, 6);
		
		// if  index<10  del "/"
		if (ICO.spriteName.Substring (0, 1) == "/") {
		 
			ICO.spriteName = ICO.spriteName.Substring (1, ICO.spriteName.Length - 1);
		}
		
		//del  ".jpg"
		ICO.spriteName = ICO.spriteName.Substring (0, ICO.spriteName.Length - 4);
 
	}
	
	public   void setScreenDPI ()
	{
		float ss = (float)Screen.width / Screen.height;
		NGUIroot.manualHeight = ss < 0.666667f ? (int)(640f / ss) : 960;

		screenScaleX = 1;//Screen.width / 640f;
		screenScaleY = 1;//Screen.height / 960f;
		fScreenHW = (float)Screen.width / (float)Screen.height;
		

	}

	public void destoryWindowByName (string winName)
	{
		Transform tran = UIScaleRoot.transform.FindChild (winName);
		if (tran == null)
			return;
		GameObject.Destroy (tran.gameObject);
		
	}

	public void hideWindowByName (string winName)
	{
		Transform tran = UIScaleRoot.transform.FindChild (winName);
		if (tran == null)
			return;
		WindowBase wb = tran.gameObject.GetComponent<WindowBase> ();
		if (wb == null)
			return;
		
		wb.hideWindow ();
		
	}

	public bool isWindowShowByName (string winName)
	{
		Transform tran = UIScaleRoot.transform.FindChild (winName);
		return tran != null && tran.gameObject.activeInHierarchy;
	}

	public bool hasWndowByName (string winName)
	{
		Transform tran = UIScaleRoot.transform.FindChild (winName);
		return tran != null;
	}

	public Transform getWndowByName (string winName)
	{
		Transform tran = UIScaleRoot.transform.FindChild (winName);
		return tran;
	}

	public Vector3 MissionWorldToUIScreenPos (Vector3 pos)
	{
		Vector3 scp = MissionManager.instance.backGroundCamera.WorldToScreenPoint (pos);
		return gameCamera.ScreenToWorldPoint (scp);
	}
	
	public T getWindow<T> (string name)  where T : Component
	{
		Transform tran = UIScaleRoot.transform.FindChild (name);
		if (tran != null) {
			return tran.GetComponent<T> ();
		}
		return null;
	}

	public T getWindow<T> ()  where T : Component
	{
		Transform tran = UIScaleRoot.transform.FindChild (typeof(T).Name);
		if (tran != null) {
			return tran.GetComponent<T> ();
		}
		return null;
	}

	//获得使用中窗口的资源名字
	public 	List<string> getAllWindowResName ()
	{
		List<string> list = new List<string> ();
		foreach (WindowBase each in windowStack) {
			list.Add (each.useResData.ResourcesName);

		}
		return list;
	}

	public bool IsWindowExits<T> ()  where T : WindowBase
	{
		foreach (WindowBase win in windowStack) {
			if (win.GetType () == typeof(T)) {
				return true;
			}
		}
		return false;
	}
    
	public void switchWindow<T> () where T : WindowBase
	{
		switchWindow<T> (null);
	}
    
	public void switchWindow<T> (CallBack<T> onWindowAwake) where T : WindowBase
	{
		openWindow<T> (onWindowAwake, false, true, true);
	}

	public void openWindow<T> () where T : WindowBase
	{
		openWindow<T> (null);
	}
    
	public void openWindow<T> (CallBack<T> onWindowAwake) where T : WindowBase
	{

		openWindow<T> (onWindowAwake, false, false, true);
	}

	public void openWindow<T> (CallBack<T> onWindowAwake, bool hideOld) where T : WindowBase
	{
		openWindow<T> (onWindowAwake, false, false, hideOld);
	}

	public void openDialogWindow<T> () where T : WindowBase
	{
		openDialogWindow<T> (null);
	}

	public void openDialogWindow<T> (CallBack<T> onWindowAwake) where T : WindowBase
	{
		openWindow<T> (onWindowAwake, true, false, true);
	}

	private void openWindow<T> (CallBack<T> onWindowAwake, bool isDialog, bool closeOld, bool hideOld) where T : WindowBase
	{
		WindowBase oldWindow = null;
		if (!isDialog && windowStack.Count > 0) {
			while (windowStack.Count > 0) {
				oldWindow = windowStack.Peek ();
				if (oldWindow.IsDestoryed) {
					windowStack.Pop ();
				} else {
					break;
				}
			}

		}

		//主窗口特别处理
		if (typeof(T) == typeof(MainWindow)) {
			MainWindow mainWindow = null;
			foreach (WindowBase w in windowStack) {
				if (mainWindow == null && w is MainWindow && !w.IsDestoryed) {
					mainWindow = w as MainWindow;
				} else if (!w.IsDestoryed) {
					w.destoryWindow ();
				}
			}
			windowStack.Clear ();

			if (mainWindow != null) {
				windowStack.Push (mainWindow);
				mainWindow.restoreWindow ();
				if (onWindowAwake != null) {
					onWindowAwake (mainWindow as T);
				}
				return;
			}
		}


		string perfabName = typeof(T).Name;
		perfabName = perfabName.Substring (0, 1).ToLower () + perfabName.Substring (1);

		if (ResourcesManager.Instance.allowLoadFromRes) {

			GameManager.Instance.StartCoroutine (Utils.DelayRun (
				() => {
				passObj tmp = MonoBase .CreateNGUIObj ("UI/" + perfabName);
				OnWindowLoaded (tmp, onWindowAwake, isDialog);

				dealWithOldWindow (oldWindow, closeOld, hideOld);

			}, 0.1f));

		} else {
       
			ResourcesManager.Instance.cacheData ("UI/" + perfabName, (List<ResourcesData> cacheList) => {
				passObj tmp = MonoBase .CreateNGUIObj ("UI/" + perfabName);
				OnWindowLoaded (tmp, onWindowAwake, isDialog);

				dealWithOldWindow (oldWindow, closeOld, hideOld);

			}, "ui");
		} 
	
	}

    void dealWithOldWindow(WindowBase oldWindow, bool closeOld, bool hideOld) {
        if (oldWindow != null && !oldWindow.IsDestoryed) {
            oldWindow.setWindowOutAnim(true);
            if (CommandConfigManager.Instance.getOpenBattleFix()) {
                if (closeOld) {
                    if (oldWindow is BattleWindow) oldWindow.destoryWindow();
                    else oldWindow.finishWindow();
                } else if (hideOld)
                    oldWindow.hideWindow();
            } else {
                if (closeOld) {
                   oldWindow.finishWindow();
                } else if (hideOld)
                    oldWindow.hideWindow();
            }
            
        }

    }
    

	public void openWindow (string windowName, CallBack<WindowBase> onWindowAwake, bool isDialog, bool closeOld, bool hideOld)
	{

		WindowBase oldWindow = null;
		if (!isDialog && windowStack.Count > 0) {
			while (windowStack.Count > 0) {
				oldWindow = windowStack.Peek ();
				if (oldWindow.IsDestoryed) {
					windowStack.Pop ();
				} else {
					break;
				}
			}

		}

		//主窗口特别处理
		if (windowName == typeof(MainWindow).Name) {
			MainWindow mainWindow = null;
			foreach (WindowBase w in windowStack) {
				if (mainWindow == null && w is MainWindow && !w.IsDestoryed) {
					mainWindow = w as MainWindow;
				} else if (!w.IsDestoryed) {
					w.destoryWindow ();
				}
			}
			windowStack.Clear ();

			if (mainWindow != null) {
				windowStack.Push (mainWindow);
				mainWindow.restoreWindow ();
				if (onWindowAwake != null) {
					onWindowAwake (mainWindow as WindowBase);
				}
				return;
			}
		}


		string perfabName = windowName;
		perfabName = perfabName.Substring (0, 1).ToLower () + perfabName.Substring (1);

		if (ResourcesManager.Instance.allowLoadFromRes) {

			GameManager.Instance.StartCoroutine (Utils.DelayRun (
                () =>
			{
				passObj tmp = MonoBase.CreateNGUIObj ("UI/" + perfabName);
				OnWindowLoaded (windowName, tmp, onWindowAwake, isDialog);

				dealWithOldWindow (oldWindow, closeOld, hideOld);

			}, 0.1f));

		} else {
			ResourcesManager.Instance.cacheData ("UI/" + perfabName, (List<ResourcesData> cacheList) =>
			{
				passObj tmp = MonoBase.CreateNGUIObj ("UI/" + perfabName);
//				tmp.data.ResourcesBundle.Unload(false);
				OnWindowLoaded (windowName, tmp, onWindowAwake, isDialog);
				dealWithOldWindow (oldWindow, closeOld, hideOld); 
			}, "ui");
		}
	}

	public T BackToWindow<T> () where T : WindowBase
	{
		if (windowStack.Count == 0)
			return null;

		WindowBase win = windowStack.Pop ();
		if (!win.IsDestoryed) {
			win.isDialogWindow = true;
			win.finishWindow ();
		}
		while (windowStack.Count > 0) {
			win = windowStack.Peek ();
			if (!win.IsDestoryed && win.GetType () == typeof(T)) {
				win.restoreWindow ();
				return (T)win;
			} else if (!win.IsDestoryed) {
				win.destoryWindow ();
			}
			windowStack.Pop ();
		}

		if (windowStack.Count == 0)
			openMainWindow ();
		return null;
	}
    public T BackToWindow<T>(CallBack back) where T : WindowBase {
        if (windowStack.Count == 0)
            return null;

        WindowBase win = windowStack.Pop();
        if (!win.IsDestoryed) {
            win.isDialogWindow = true;
            win.finishWindow();
        }
        while (windowStack.Count > 0) {
            win = windowStack.Peek();
            if (!win.IsDestoryed && win.GetType() == typeof(T)) {
                win.restoreWindow(back);
                return (T)win;
            } else if (!win.IsDestoryed) {
                win.destoryWindow();
            }
            windowStack.Pop();
        }

        if (windowStack.Count == 0)
            openMainWindow();
        return null;
    }

	public WindowBase BackToWindow (WindowBase window)
	{
		if (windowStack.Count == 0)
			return window;
        
		WindowBase win = windowStack.Pop ();
		if (!win.IsDestoryed) {
			win.isDialogWindow = true;
			win.finishWindow ();
		}
		while (windowStack.Count > 0) {
			win = windowStack.Peek ();
			if (!win.IsDestoryed && win == window) {
				win.restoreWindow ();
				return window;
			} else if (!win.IsDestoryed) {
				win.destoryWindow ();
			}
			windowStack.Pop ();
		}

		if (windowStack.Count == 0)
			openMainWindow ();
		return window;
	}

	//WindowBase专用,用于窗口出栈
	public void OnBackPressed ()
	{
		windowStack.Pop ();
		WindowBase win = null;
		while (windowStack.Count > 0) {
			win = windowStack.Peek ();
			if (win.IsDestoryed || win.windowState == GUI_state.OnOver) {
				windowStack.Pop ();
	
			} else {
				break;
			}
		}

		if (win != null) {

			win.restoreWindow ();

		}



	}

	private void OnWindowLoaded<T> (passObj tmp, CallBack<T> onWindowAwake, bool isDialog) where T : WindowBase
	{
		if (tmp.obj == null)
			return;

		tmp.obj.name = typeof(T).Name;
		WindowBase win = tmp.obj.GetComponent<T> ();
		win.setResourcesData (tmp.data);
		if (win.isDialogWindow != isDialog) {
			throw new System.Exception ("winodw is not dialog");
		}
		// win.isDialogWindow = isDialog;

		if (windowStack.Count > 0) {
			win.SetFatherWindow (windowStack.Peek ());
		}
		if (!isDialog) {
			windowStack.Push (win);
		}

		if (onWindowAwake != null) {
			onWindowAwake ((T)win);
		}
	}

	private void OnWindowLoaded (string windowName, passObj tmp, CallBack<WindowBase> onWindowAwake, bool isDialog)
	{
		if (tmp.obj == null)
			return;

		tmp.obj.name = windowName;
		WindowBase win = tmp.obj.GetComponent<WindowBase> ();
		win.setResourcesData (tmp.data);
		if (win.isDialogWindow != isDialog) {
			throw new System.Exception ("winodw is not dialog");
		}
		// win.isDialogWindow = isDialog;

		if (windowStack.Count > 0) {
			win.SetFatherWindow (windowStack.Peek ());
		}
		if (!isDialog) {
			windowStack.Push (win);
		}

		if (onWindowAwake != null) {
			onWindowAwake ((WindowBase)win);
		}
	}


	//销毁所有栈中的窗口,但排除参数中的窗口
	public void clearWindows (params WindowBase[] excludes)
	{
		List<WindowBase> list = new List<WindowBase> ();
		ListKit.AddRange (list, excludes);
		foreach (WindowBase win in windowStack) {
			if (!win.IsDestoryed && !list.Contains (win)) {
				win.destoryWindow ();
			}
		}
	}

	//销毁所有的窗口,但排除参数中的
	public void clearWindowsName (params string[] excludes)
	{
		List<string> list = new List<string> ();
		ListKit.AddRange (list, excludes);
		list.Add ("chatButton");
		list.Add ("guideLayer");
		list.Add ("mask");
		list.Add ("maskWindow");
		list.Add ("MousePointSprite");
		list.Add ("UIEffectRoot");
		list.Add ("BackGroundWindow");
		int count = UIScaleRoot.transform.childCount;
		for (int i = 0; i < count; i++) {
			GameObject obj = UIScaleRoot.transform.GetChild (i).gameObject;
			if (!list.Contains (obj.name)) {
				GameObject.Destroy (obj);
			}
		}
	}

	public WindowBase CurrentWindow {
		get {
			return windowStack.Peek ();
		}
	}

	//网络重练后调用
	public void OnNetResume ()
	{
		if (MissionManager.instance != null)
			MissionManager.instance.OnNetResume ();
        GodsWarManagerment.Instance.getGodsWarStateInfo(() => { });
	    if (godsWarGroupStageWindow != null && GodsWarManagerment.Instance.StateInfo != 1)
	    {
            if (UiManager.Instance.godsBuyWind != null) {
                UiManager.Instance.godsBuyWind.destoryWindow();
            }
            if (UiManager.Instance.godsWarReplayWindow != null) {
                UiManager.Instance.godsWarReplayWindow.destoryWindow();
            }
            if (UiManager.Instance.godsWarUserInfoWindow != null) {
                UiManager.Instance.godsWarUserInfoWindow.destoryWindow();
            }
            if (UiManager.Instance.godsWarMySuportWindow != null) {
                UiManager.Instance.godsWarMySuportWindow.destoryWindow();
            }
            if (UiManager.Instance.godsWarProgramWindow != null) {
                UiManager.Instance.godsWarProgramWindow.destoryWindow();
            }
            UiManager.Instance.destoryWindowByName("MessageWindow");
            UiManager.Instance.BackToWindow<MainWindow>();
	    }
		WindowBase win = windowStack.Peek ();
		if (win != null && win.gameObject != null && win.gameObject.activeSelf)
			win.OnNetResume ();
        if (UiManager.Instance != null && UiManager.Instance.pveUseWindow != null) {
            UiManager.Instance.pveUseWindow.getBaseData();
        }
        //if (Instance!=null&&Instance.godsWarSuportWindow!=null)
        //{
        //    UiManager.Instance.openMainWindow();
        //}
	    if (Instance != null && Instance.laddersRankRewardWindow != null)
	    {
	        Instance.laddersRankRewardWindow.destoryWindow();
	        laddersRankRewardWindow = null;
	    }
	    if (laddersChestsWindow != null)
	    {
	        laddersChestsWindow.destoryWindow();
	        laddersChestsWindow = null;
	    }
	    if (levelupRewardWindow != null)
	    {
	        levelupRewardWindow.destoryWindow();
	        levelupRewardWindow = null;
	    }

	}

	public void removeAllEffect ()
	{
		if (UIEffectRoot == null)
			return;

		foreach (Transform each in  UIEffectRoot.transform) {
			if (each == null)
				continue;
			GameObject.Destroy (each.gameObject);
		}
	}
 
	public static Vector2 leftDownToleftUp_point (Vector2 orgPoint)
	{
		Vector2 newPoint;
		
		newPoint = new Vector2 (orgPoint.x, Screen.height - orgPoint.y);
		
		
		return newPoint;
	}
	
	public static Rect leftDownToleftUp_point (Rect orgPoint)
	{
		Rect newPoint;
		
		newPoint = new Rect (orgPoint.x, Screen.height - orgPoint.y - orgPoint.height, orgPoint.width, orgPoint.height);
		
		
		return newPoint;
	}
	
	public static Vector2 getMousePositionInscreen ()
	{
		
		float x = Input.mousePosition.x;
		float y = -(Screen.height - Input.mousePosition.y);
		
		return new Vector2 (x, y);
	 
		
	}

	public void createPrizeMessageLintWindow (PrizeSample prize)
	{
		PropMessageLineWindow lineWindow = getWindow<PropMessageLineWindow> ();
		if ( lineWindow!= null) {
			lineWindow.Initialize (prize);
		} else {
			openDialogWindow<PropMessageLineWindow> ((win) => {
				win.Initialize (prize);
			});
		}
	}

	public void createPrizeMessageLintWindow (PrizeSample[] prizes)
	{
        PropMessageLineWindow lineWindow = getWindow<PropMessageLineWindow>();
        if (lineWindow != null) {
            lineWindow.Initialize(prizes);
        } else {
            openDialogWindow<PropMessageLineWindow>((win) => {
                win.Initialize(prizes);
            });
        }
	}




	//创建TIPS
	public void createMessageLintWindow (string str)
	{
		MessageLineWindow lineWindow = getWindow<MessageLineWindow> ();
		if ( lineWindow!= null) {
			lineWindow.Initialize (str);
		} else {
			openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (str);
			});
		}
	}

	public void createMessageLintWindow(string[] strArr)
	{
		MessageLineWindow lineWindow = getWindow<MessageLineWindow> ();
		if ( lineWindow!= null) {
			lineWindow.Initialize (strArr);
		} else {
			openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (strArr);
			});
		}
	}


	public void createMessageLintWindowNotUnLuck (string str)
	{
		MessageLineWindow lineWindow = getWindow<MessageLineWindow>();
		if (lineWindow != null) {
			lineWindow.dialogCloseUnlockUI = false;
			lineWindow.Initialize (str);
		} else {
			openDialogWindow<MessageLineWindow> ((win) => {
				win.dialogCloseUnlockUI = false;
				win.Initialize (str);
			});
		}
	}

	//创建带图片的消息窗
	public void createMessageTextureWindow (string path, string msg)
	{
		openDialogWindow<MessageTextureWindow> ((win) => {
			win.init (path, msg);
		});
	}
	//创建TIPS
	public void createMessageTextureWindow (string path,string str,bool dialogCloseUnlockUI)
	{
		openDialogWindow<MessageLineWindow> ((win) => {
			win.dialogCloseUnlockUI=dialogCloseUnlockUI;
			win.Initialize (str);
		});
	}

	public void createMessageTextureWindow (string path, string msg, float fromY, float toY)
	{
		openDialogWindow<MessageTextureWindow> ((win) => {
			win.init (path, msg, fromY, toY);
		});
	}

	//创建单个按钮的提示窗
	public void createMessageWindowByOneButton (string str, CallBackMsg callback)
	{
		openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, str, callback);
		});
	}

	//创建两个按钮(确定取消)的提示窗
	public void createMessageWindowByTwoButton (string str, CallBackMsg callback)
	{
		openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), str, callback);
		});
	}
}
