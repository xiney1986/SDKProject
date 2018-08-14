using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 新手引导管理器
 * @author 汤琦
 * 新手引导分2种类型
 * 1.流程引导
 * 2.一次性引导
 * 		a按玩家等级引导 经验只能在通关副本获得 所以只需要在通关副本时做处理(有等级限制)
 * 		b首次使用引导(无等级限制) 
 * 指引流程
 * 1.向后台通信,获取指引sid（GuideGetInfoFPort）
 * 2.通过获得的sid,得到跳转sid和流程索引,再看是否存在条件类型,如果有,且不满足则重新更新指引sid和流程索引（GuideManager init updateGuide）
 * 3.在需要指引的窗口类中(通常在begin中)直接调用（GuideManager guideEvent）
 * 4.在（GuideManager guideEvent中）会进行是否在指引中的判断（GuideManager isGuideComplete）,如果在指引中则会进入GuideUI中通过传入的sid提取配置进行指引
 * 5.在需要执行指引往前进行时,需要调用（GuideManager doGuide）,通常是点击事件那里调用,但如果有需要通信后才导致游戏进行的话,需要在通信后调用,比如角色进化等
 * 6.如果指引是剧情或者点击全屏幕进行时,会把点击事件传入GuideButton中
 * 7.GuideGlobal这个类,用来存放指引的特殊sid,在某些特殊情况下需要使用
 * */

public class GuideManager {
	/** 指引列表中的索引 */
	public int guideIndex;
	/** 指引sid */
	public int guideSid;
	/** 一次性指引sid */
	public int guideOnceSid;
	private const int TYPE = 1;// type 1两种都保存,2只保存步骤(int),3只保存一次性引导
	private const int TYPE_2 = 2;
	private const int TYPE_3 = 3;
	public List<int> onceGuideList = null;//一次性引导
	/** 友情引导指引过的缓存列表 */
	public List<int> friendlyGuideList = null;
	public GuideUI guideUI;
	public bool isOpenNewFunWin = false;//新功能提示窗口是否打开
	public bool isOpenMsgWin = false;//新功能提示窗口是否打开
	/** 刚登录的时候是不是在副本中 */
	public bool isLoginOnInMission = false;
	/**刚登陆的时候 拿到女神修炼CD时间 */
	public int goddessTranningTime=0;
	public bool isHaveGuide=false;//是否还有强制指引
	public List<int> openIndex;
	
	public static GuideManager Instance {
		get { return SingleManager.Instance.getObj ("GuideManager") as GuideManager;}
	}
	
	#region 指引初始化
	//初始化指引的指引sid
	public void init (int guideSid, int[] array, bool isNewUser) { 
		guideUI = UiManager.Instance.UIScaleRoot.transform.FindChild ("guideLayer").GetComponent<GuideUI> ();
		this.guideSid = guideSid;
		if (array != null) {
			this.onceGuideList = new List<int> ();
			foreach (int each in array) {
				this.onceGuideList.Add (each);
			}
			//ListKit.AddRange (this.onceGuideList, array);
		}
		else
			this.onceGuideList = new List<int> ();

		if (GameManager.Instance.skipGuide)
			this.guideSid = Mathf.Max (GuideGlobal.OVERSID, guideSid);
		else if (!isNewUser && UserManager.Instance.self.getUserLevel () <= 5) {
			this.guideSid = Mathf.Max (123001000, guideSid);
		}
		updateGuide ();
	}

	//根据指引sid得到指引列表中的索引index
	private void updateGuide () {

		//被砍的步骤按需要跳转,这里无法动态生成,只能每个版本写死
		if (guideSid >= 129001000 && guideSid <= 129007000) {
			setStep (123001000);
		}
		else if (guideSid >= 102001000 && guideSid <= 102004000) {
			setStep (126001000);
		}
		else if (guideSid >= 128001000 && guideSid <= 128003000) {
			setStep (101001000);
		}
		else if (guideSid >= 104001000 && guideSid <= 104005000) {
			setStep (118001000);
		}
		//11级四人阵被砍
		else if (guideSid >= 105001000 && guideSid <= 105003000) {
			setStep (133001000);
		}
		//因为讨伐提高了等级(8-11)，所以要按照实际等级改变步骤
		else if (guideSid >= 126001000 && guideSid <= 126006000 && UserManager.Instance.self.getUserLevel () < 11) {
			setStep (135001000);
		}
		//原本10级指引兑换被砍
		else if (guideSid >= 106001000 && guideSid <= 106009000) {
			setStep (126001000);
		}

		//初始化配置表中的跳转
		updateJumpGuideSid ();

		if (GameManager.Instance.skipGuide)
			return;
		Card mainCard = StorageManagerment.Instance.getRole (UserManager.Instance.self.mainCardUid);
		Skill[] ss = mainCard.getSkills ();
		bool flag = false;
		foreach (Skill s in ss) {
			if (s.getEXP () > 0) {
				flag = true;
				break;
			}
		}

		//这里开始新手其间断网导致的特殊情况处理
		if (!isLoginOnInMission) {
			//抽奖--第一个副本必须打通，如果潘多拉存在就跳步骤
			if (guideSid == 7001000 && FuBenManagerment.Instance.getLastStoryMissionSid () == 41001 && StorageManagerment.Instance.getRoleBySid (11111) != null && StorageManagerment.Instance.getRoleBySid (11111).Count >0 ) {
				setStep (8001000);
			}
			//队伍上阵--如果队伍上阵人数大于1，跳步骤
			else if (guideSid == 8001000 && ArmyManager.Instance.getArmy (ArmyManager.PVE_TEAMID).getCardList ().Count > 1) {
				setStep (9001000);
			}
			//队伍上阵--如果队伍上阵人数大于1，跳步骤
			else if (guideSid == 9001000 && ArmyManager.Instance.getArmy (ArmyManager.PVE_TEAMID).getCardList ().Count < 2) {
				setStep (8001000);
			}
			//到了第二个副本外献祭步骤，发现其实并不是在副本中时，而且仅仅打了第一个副本，回调
			else if (guideSid == 12001000 && FuBenManagerment.Instance.getLastStoryMissionSid () == 41001) {
				setStep (9001000);
			}
			//到了第二个副本外献祭步骤，发现献祭过了，就跳步骤
			else if (guideSid == 12001000 && FuBenManagerment.Instance.getLastStoryMissionSid () == 41002 && flag) {
				setStep (13001000);
			}
			//到了第三个副本外召唤女神步骤，发现其实并不是在副本中时，而且仅仅打了第二个副本，回调
			else if (guideSid == 16001000 && FuBenManagerment.Instance.getLastStoryMissionSid () == 41002) {
				setStep (13001000);
			}
			//到了第三个副本外召唤女神步骤，发现已经召唤女神了，跳到女神上阵步骤
			else if (guideSid == 16001000 && FuBenManagerment.Instance.getLastStoryMissionSid () == 41003 && BeastEvolveManagerment.Instance.isHaveBeast ()) {
				setStep (17001000);
			}
			//在指引女神上阵时，发现发现没召唤成功，回调
			else if (guideSid == 17001000 && !BeastEvolveManagerment.Instance.isHaveBeast ()) {
				setStep (16001000);
			}
			//在指引女神上阵时，发现已经上阵了女神，跳过
			else if (guideSid == 17001000 && BeastEvolveManagerment.Instance.isHaveBeast () && !string.IsNullOrEmpty (ArmyManager.Instance.getArmy (ArmyManager.PVE_TEAMID).beastid)
			         && ArmyManager.Instance.getArmy (ArmyManager.PVE_TEAMID).beastid != "0") {
				setStep (20001000);
			}
			//女神上阵后准备要去副本时发现女神没上阵，回调
			else if (guideSid == 20001000 && BeastEvolveManagerment.Instance.isHaveBeast () &&
			         (string.IsNullOrEmpty (ArmyManager.Instance.getArmy (ArmyManager.PVE_TEAMID).beastid) || ArmyManager.Instance.getArmy (ArmyManager.PVE_TEAMID).beastid == "0")) {
				setStep (17001000);
			}
			//到了第四个副本外步骤，发现其实并不是在副本中时，而且仅仅打了第三个副本，回调
			else if (guideSid == 23001000 && FuBenManagerment.Instance.getLastStoryMissionSid () == 41003) {
				setStep (20001000);
			}
			//发现已经开过星盘，跳过
			else if (guideSid == 101001000 && GoddessAstrolabeManagerment.Instance.getOpenStarNumByNebulaId(1) > 0) {
				setStep (108001000);
			}
			//发现已经提升过荣誉，跳过
			else if (guideSid == 110001000 && UserManager.Instance.self.honorLevel > 1) {
				setStep (103001000);
			}
		}

		initJumpGuideSid ();
		int lv = UserManager.Instance.self.getUserLevel ();

//		string strLv = "";
//		for (int i = 0; i < jumpLevel.Count; i++) {
//			strLv += "," + jumpLevel[i];
//		}
//		string strSid = "";
//		for (int i = 0; i < jumpSids.Count; i++) {
//			strSid += "," + jumpSids[i];
//		}

		//这里开始按等级跳步骤
		if (jumpLevel.Count == jumpSids.Count && lv >= jumpLevel [0] && lv <= jumpLevel [jumpLevel.Count - 1]) {

			for (int i = 0; i < jumpLevel.Count; i++) {
				if (lv == jumpLevel [i]) {
					//相等的情况下就不跳步骤了
					if (isEqualStep (jumpSids [i])) {
						break;
					}
					//发现指定等级达不到相应步骤，就跳到下一阶段指引去
					//发现指定等级相应步骤超过了，就跳回去
					else if (isLessThanStep (jumpSids [i]) || isMoreThanStep (jumpSids [i])) {
						if (i + 1 < jumpSids.Count) {
							setStep (jumpSids [i + 1]);
							break;
						}
						else {
							setStep (GuideGlobal.OVERSID);
							break;
						}
					}
				}
				else if (lv < jumpLevel [i]) {
					setStep (jumpSids [i]);
					break;
				}
			}
		}
		else if (lv > jumpLevel [jumpLevel.Count - 1]) {
			setStep (GuideGlobal.OVERSID);
		}
	}

	/// <summary>
	/// 根据配置表进行跳步骤
	/// </summary>
	private void updateJumpGuideSid () {
		guideIndex = GuideConfigManager.Instance.getIndexByGuideSid (guideSid);
		guideSid = GuideConfigManager.Instance.getGotoSid (guideSid);
		guideIndex = GuideConfigManager.Instance.getIndexByGuideSid (guideSid);
	}

	/// <summary>
	/// 不满足条件时跳步骤
	/// </summary>
	public void jumpGuideSid () {
		updateJumpGuideSid ();
		guideEvent ();
	}

	List<int> jumpLevel;
	List<int> jumpSids;

	/// <summary>
	/// 初始化对应的等级指引跳步骤列表
	/// </summary>
	public void initJumpGuideSid () {
		jumpLevel = new List<int> ();
		jumpSids = new List<int> ();
		
		List<int> guideSidsList = GuideConfigManager.Instance.getGuideSids ();
		GuideCondition[] cons = null;
		for (int i = 0; i < guideSidsList.Count; i++) {
			cons = GuideConfigManager.Instance.getConditionByGuideSid (i);
			if (cons != null) {
				for (int j = 0; j < cons.Length; j++) {
					//只要配置表里面带“lv”关键字对应的步骤,所以配置配置表的时候要严格规范
					if (cons [j].conditionType == GuideConditionType.LEVEL && StringKit.toInt (cons [j].conditionValue) != 1000
						&& !jumpLevel.Contains (StringKit.toInt (cons [j].conditionValue))) {
						jumpLevel.Add (StringKit.toInt (cons [j].conditionValue));
						jumpSids.Add (guideSidsList [i]);
					}
				}
			}
		}
		
	}

	#endregion

	#region 指引显示,根据指引的文本类型选择（分为4种类型）

	public void showGuide () {
		guideUI.showGuide (guideSid);
	}

	public void showGuide (int sid) {
		this.guideOnceSid = sid;
		guideUI.showGuide (sid);
	}

	/// <summary>
	/// 隐藏指引的箭头和描述
	/// </summary>
	public void hideGuide () { 
		guideUI.hideGuide ();
	}
	#endregion
	
	#region 流程指引

	/// <summary>
	/// 满足条件就向后走一步
	/// </summary>
	public void doGuide () {
		if (!isGuideComplete ()) {
			hideGuide ();
			guideUI.maxMask.gameObject.SetActive (true);
			guideIndex++;
			guideSid = GuideConfigManager.Instance.guideSids [guideIndex];
			GuideSaveFPort port = FPortManager.Instance.getFPort ("GuideSaveFPort") as GuideSaveFPort;
			port.saveStep (TYPE_2, guideSid);
		}
	}

	/// <summary>
	/// 执行指定步骤
	/// </summary>
	private void doGuide (int sid) { 
		GuideSaveFPort port = FPortManager.Instance.getFPort ("GuideSaveFPort") as GuideSaveFPort;
		port.saveStep (TYPE_2, sid);
	}

	/// <summary>
	/// 判断副本流程是否完成
	/// </summary>
	public bool isMissionComplete () {
		GuideCondition[] cons = GuideConfigManager.Instance.getConditionByGuideSid (guideIndex);
		if (cons != null) {
			for (int i = 0; i < cons.Length; i++) {
				if (cons [i].conditionType == GuideConditionType.MISSIONSID) {
//					Debug.LogError ("------------>>>over mission sid==" + FuBenManagerment.Instance.getLastStoryMissionSid () + ",guidesid=" + guideSid + ",sidV=" + cons [i].conditionValue);
					if (StringKit.toInt (cons [i].conditionValue) == FuBenManagerment.Instance.getLastStoryMissionSid ())
						return true;
					else
						return false;
				}
			}
			return true;
		}
		else {
			return true;
		}
	}

	/// <summary>
	/// 判断流程引导是否完成,false没完成（需要引导）,ture已完成（不要引导）
	/// </summary>
	public bool isGuideComplete () {
		if (SweepManagement.Instance != null && SweepManagement.Instance.hasSweepMission) {
			return true;
		}
		//获得指引的条件类型
		GuideCondition[] cons = GuideConfigManager.Instance.getConditionByGuideSid (guideIndex);
		if (cons != null) {
			for (int i = 0; i < cons.Length; i++) {
				if (cons [i].conditionType == GuideConditionType.LEVEL && UserManager.Instance.self.getUserLevel () == StringKit.toInt (cons [i].conditionValue)) {
					if(!isHaveGuide) isHaveGuide = true;
					return false;
				}
				else if (cons [i].conditionType == GuideConditionType.WINDOWBASE) {
					if (GameObject.Find (cons [i].conditionValue)){
						if(!isHaveGuide) isHaveGuide = true;
						return false;
					}
					else
						return true;
				}
				else if (cons [i].conditionType == GuideConditionType.OVER) {
					if (!isOpenMsgWin) {
						//isOpenMsgWin = true;
                        //UiManager.Instance.openDialogWindow<GuideOverWindow> ((win) => {
                        //   win.initWindow (openNewFunCallBack);
                        //});
//						MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Guide_01"), specialHandle);
						MainWindow mainWin = UiManager.Instance.getWindow<MainWindow> ();
						if (mainWin != null && mainWin.gameObject.activeSelf) {
							mainWin.showGuideGoToFuBenEffect ();
						    openNewFunCallBack();
						}
					}
					return true;
				}
				else if (cons [i].conditionType == GuideConditionType.PROPSID || cons [i].conditionType == GuideConditionType.EQUIPSID ||
				         cons [i].conditionType == GuideConditionType.CARDSID) {
					if(!isHaveGuide) isHaveGuide = true;
					return false;
				}
			}
			return true;
		}
		if(!isHaveGuide) isHaveGuide = true;
		return false;
	}

	/// <summary>
	/// 判断道具条件是否满足,true满足,false不满足
	/// </summary>
	private bool checkItemCondition () {
		GuideCondition[] cons = GuideConfigManager.Instance.getConditionByGuideSid (guideIndex);
		if (cons != null) {
			for (int i = 0; i < cons.Length; i++) {
				if (cons [i].conditionType == GuideConditionType.PROPSID) {
					Prop tmpNeed = StorageManagerment.Instance.getProp (StringKit.toInt (cons [i].conditionValue));
					if (tmpNeed != null && tmpNeed.getNum () >= cons [i].conditionNum) {
						return true;
					} else {
						updateJumpGuideSid ();
						return false;
					}
				}
				else if (cons [i].conditionType == GuideConditionType.EQUIPSID) {
					ArrayList tmpNeed = StorageManagerment.Instance.getEquipsBySid (StringKit.toInt (cons [i].conditionValue));
					if (tmpNeed != null && tmpNeed.Count >= cons [i].conditionNum) {
						return true;
					} else {
						updateJumpGuideSid ();
						return false;
					}
				}
				else if (cons [i].conditionType == GuideConditionType.CARDSID) {
					ArrayList tmpNeed = StorageManagerment.Instance.getRoleBySid (StringKit.toInt (cons [i].conditionValue));
					if (tmpNeed != null && tmpNeed.Count >= cons [i].conditionNum) {
						return true;
					} else {
						updateJumpGuideSid ();
						return false;
					}
				}
			}
		}
		return true;
	}
   /// <summary>
   /// 检查仓库是否已满
   /// </summary>
   /// <returns></returns>
    private bool isStorageFull() {
        bool isFull = false;
        string strErr = "";
        if ((StorageManagerment.Instance.getAllRole().Count + 1) > StorageManagerment.Instance.getRoleStorageMaxSpace()) {
            isFull = true;
        } else if ((StorageManagerment.Instance.getAllEquip().Count + 1) > StorageManagerment.Instance.getEquipStorageMaxSpace()) {
            isFull = true;
        } else if ((StorageManagerment.Instance.getAllProp().Count + 1) > StorageManagerment.Instance.getPropStorageMaxSpace()) {
            isFull = true;
        }
        return isFull;
    }
	/// <summary>
	/// 检测是否可以等级指引
	/// </summary>
	public bool showGuideNewFun () {
		if (isGuideComplete ()) {
			return false;
		}
		//获得指引的条件类型
		GuideCondition[] cons = GuideConfigManager.Instance.getConditionByGuideSid (guideIndex);
		if (cons != null) {
			for (int i = 0; i < cons.Length; i++) {
				if (cons [i].conditionType == GuideConditionType.LEVEL && UserManager.Instance.self.getUserLevel () == StringKit.toInt (cons [i].conditionValue)) {
					MainWindow mainWin = UiManager.Instance.getWindow<MainWindow> ();
					if (mainWin != null && mainWin.gameObject.activeSelf) {
						mainWin.jumpToPage (1, false);
//						mainWin.launcherPanel.transform.localPosition = new Vector3 (-615f, 0, 0);
//						mainWin.launcherPanel.clipOffset = new Vector2 (615, 0);
					}
					if (!isOpenNewFunWin) {
						MaskWindow.LockUI ();
						isOpenNewFunWin = true;
                        if (isStorageFull())UiManager.Instance.destoryWindowByName("MessageWindow");//如果有新手指引 又有弹窗 就把弹窗销毁
						UiManager.Instance.openDialogWindow<NewFunctionShowWindow> ((win) => {
							win.initWindow (openNewFunCallBack);
						});
					}
					return true;
				}
			}
			return false;
		}
		return false;
	}

	//特殊处理
	private void specialHandle (MessageHandle msg) {
		guideIndex++;
		guideSid = GuideConfigManager.Instance.guideSids [guideIndex];
		GuideSaveFPort port = FPortManager.Instance.getFPort ("GuideSaveFPort") as GuideSaveFPort;
		port.saveStep (TYPE_2, guideSid);
		guideEvent ();
	}

	void openNewFunCallBack () {
		guideIndex++;
		guideSid = GuideConfigManager.Instance.guideSids [guideIndex];
		GuideSaveFPort port = FPortManager.Instance.getFPort ("GuideSaveFPort") as GuideSaveFPort;
		port.saveStep (TYPE_2, guideSid);
		guideEvent ();
	}
	
	#endregion
	
	#region 一次性引导

	/// <summary>
	/// 完成一次性引导
	/// </summary>
	public void doOnceGuide (int sid) {
		if (!isOnceGuideComplete (sid)) {
			onceGuideList.Add (sid);
			GuideSaveFPort port = FPortManager.Instance.getFPort ("GuideSaveFPort") as GuideSaveFPort;
			port.saveStep (TYPE_3, sid);
			showGuide (sid);
		}
	}

	/// <summary>
	/// 判断一次性引导是否完结
	/// </summary>
	public bool isOnceGuideComplete (int guideSid) {
		if (onceGuideList == null || onceGuideList.Count < 1)
			return false;
		if (onceGuideList.Contains (guideSid))
			return true;
		return false;
	}
	#endregion
	
	#region 指引通用

	/// <summary>
	/// 显示对白窗口
	/// </summary>
	public void showTalkWindow (CallBack callback1, CallBack callback2) {
		UiManager.Instance.openWindow<EmptyWindow> ((win) => {
			UiManager.Instance.openDialogWindow<TalkWindow> ((win2) => {
				win2.Initialize (getPlotSid (), callback1, callback2);
			});
		});
	}

	/// <summary>
	/// 获得当前步骤剧情sid
	/// </summary>
	public int getPlotSid () {
		return GuideConfigManager.Instance.plotSids [guideIndex];
	}

	/// <summary>
	/// 获得指引的描述
	/// </summary>
	public string getGuideDesc () {
		return GuideConfigManager.Instance.infos [guideIndex].Replace ('~', '\n');
		;
	}

	/// <summary>
	/// 获得一次性指引的描述
	/// </summary>
	public string getOnceGuideDesc () {
		int tempIndex = GuideConfigManager.Instance.getIndexByGuideSid (guideOnceSid);
		return GuideConfigManager.Instance.infos [tempIndex].Replace ('~', '\n');
		;
	}

	/// <summary>
	/// 关闭全屏指引遮罩
	/// </summary>
	public void closeGuideMask () {
		if (guideUI == null)
			return;
		guideUI.maxMask.gameObject.SetActive (false);
		guideUI.mask.gameObject.SetActive (false);
	}

	/// <summary>
	/// 打开全屏指引遮罩
	/// </summary>
	public void openGuideMask () {
		if (guideUI == null)
			return;
		guideUI.maxMask.gameObject.SetActive (true);
		guideUI.mask.gameObject.SetActive (true);
	}

	/// <summary>
	/// 指定新手步骤是否大于等于现在的步骤
	/// </summary>
	public bool isOverStep (int _sid) {
		return guideIndex >= GuideConfigManager.Instance.getIndexByGuideSid (_sid);
	}

	/// <summary>
	/// 指定新手步骤是否等于现在的步骤
	/// </summary>
	public bool isEqualStep (int _sid) {
		return guideIndex == GuideConfigManager.Instance.getIndexByGuideSid (_sid);
	}

	/// <summary>
	/// 指定新手步骤是否小于现在的步骤
	/// </summary>
	public bool isLessThanStep (int _sid) {
		return guideIndex < GuideConfigManager.Instance.getIndexByGuideSid (_sid);
	}

	/// <summary>
	/// 指定新手步骤是否大于现在的步骤
	/// </summary>
	public bool isMoreThanStep (int _sid) {
		return guideIndex > GuideConfigManager.Instance.getIndexByGuideSid (_sid);
	}

	/// <summary>
	/// 指定新手步骤是否不等于现在的步骤
	/// </summary>
	public bool isDoesNotEqualStep (int _sid) {
		return guideIndex != GuideConfigManager.Instance.getIndexByGuideSid (_sid);
	}

	/// <summary>
	/// 更新步骤
	/// </summary>
	public void setStep (int _sid) {
		guideSid = _sid;
		guideIndex = GuideConfigManager.Instance.getIndexByGuideSid (_sid);
	}

	#endregion
	
	#region 新手指引进入副本

	/// <summary>
	/// 进入游戏，有指引的情况下进入指引（新手指引或等级指引）
	/// </summary>
	public void intoGuide () {
		initFubenData ();
	}

	//初始化副本信息
	private void initFubenData () {
		FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
		port.info (getFubenCurrentInfo, ChapterType.STORY);  
	}
	
	//获得保存的关卡信息
	private void getFubenCurrentInfo () { 
		FuBenGetCurrentFPort port = FPortManager.Instance.getFPort ("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
		port.getInfo (getContinueMission);
	}
	
	//处理存档的关卡信息 
	private void getContinueMission (bool b) { 
		if (b) {
			closeGuideMask ();
			MissionInfoManager.Instance.mission.updateBoss ();	
			initFubenByType (MissionInfoManager.Instance.mission.getChapterType ()); 
		}
		else {
			//特殊情况，当由指引进入副本的，由于必要获得相关道具，所以要进入指定的副本sid
			if (guideIndex < GuideGlobal.FIRSTINTOFUBEN) {
				intoMission (GuideGlobal.FIRST_MISSION_SID);
			}
			else {
				UiManager.Instance.openWindow<MainWindow> ();
			}
		}
	}
	
	//获得指定副本数据
	private void initFubenByType (int type) {
		if (type != ChapterType.STORY) {
			FuBenInfoFPort port = FPortManager.Instance.getFPort ("FuBenInfoFPort") as FuBenInfoFPort;
			port.info (continueMission, type);
		}
		else {
			continueMission ();
		}
	}
	
	//继续关卡
	private void continueMission () {
		FuBenIntoFPort port = FPortManager.Instance.getFPort ("FuBenIntoFPort") as FuBenIntoFPort;
		port.toContinue (continueIntoMission); 
	} 
	
	//继续关卡
	private void continueIntoMission () {
		if (UiManager.Instance.ActiveLoadingWindow != null)
			UiManager.Instance.ActiveLoadingWindow.finishWindow ();

		if (guideSid < GuideGlobal.SPECIALSID1 && guideSid != GuideGlobal.SPECIALSID5)
			UiManager.Instance.switchWindow<EmptyWindow> ((win) => {
				ScreenManager.Instance.loadScreen (4, null, () => {
					UiManager.Instance.switchWindow<MissionMainWindow> ((win2) => {
						guideEvent ();
					});});
			});
		else
			UiManager.Instance.switchWindow<EmptyWindow> ((win) => {
				ScreenManager.Instance.loadScreen (4, null, () => {
					UiManager.Instance.switchWindow<MissionMainWindow> ();});
			});
	}
	
	private void intoMission (int m_sid) {
		FuBenIntoFPort port = FPortManager.Instance.getFPort ("FuBenIntoFPort") as FuBenIntoFPort;
		port.intoFuben (m_sid, 1, intoMissionUseArray);
	}
	
	private void intoMissionUseArray (int sid, int missionLevel) {
		if (UiManager.Instance.ActiveLoadingWindow != null)
			UiManager.Instance.ActiveLoadingWindow.finishWindow ();

		MissionInfoManager.Instance.saveMission (sid, missionLevel);
		FuBenManagerment.Instance.intoMission (sid, missionLevel); 
		ArmyManager.Instance.setActive (1);
		//	ArmyManager.Instance.changeLastId (1);
		UiManager.Instance.switchWindow<EmptyWindow> ((win) => {
			ScreenManager.Instance.loadScreen (4, null, () => {
				UiManager.Instance.switchWindow<MissionMainWindow> ((win2) => {
					guideEvent ();
				});});
		});
	}

	#endregion
	
	public void initGuideButton (string buttonName, string functionName, GameObject obj) {
		guideUI.guideButton.gameObject.name = buttonName;
		guideUI.guideButton.initInfo (obj, functionName);
		guideUI.guideButton.gameObject.SetActive (true);
	}
	
	public void hideGuideUI () {
		guideUI.closeGuide ();
	}
	
	public void clearGuideButton () {
		guideUI.guideButton.gameObject.name = "";
		guideUI.guideButton.clearInfo ();
		guideUI.guideButton.gameObject.SetActive (false);
	}

	/// <summary>
	/// 执行流程指引,条件满足就显示流程指引,反之解锁
	/// </summary>
	public void guideEvent () {
		if (!isGuideComplete ()) {
			if (!checkItemCondition ()) {
				closeGuideMask ();
				return;
			}
			UiManager.Instance.initNewPlayerGuideLayer ();
	
			//新手指引第一次主动进入副本的情况，特殊处理
			if (guideSid != GuideGlobal.SPECIALSID1)// && guideSid != GuideGlobal.SPECIALSID5)
				openGuideMask ();
			else
				closeGuideMask ();
			//根据指引是否剧情sid来判断是否弹剧情
			if (GuideConfigManager.Instance.plotSids [guideIndex] != 0) {
				//新手指引第一次主动进入副本的情况，特殊处理
				if (guideSid == GuideGlobal.SPECIALSID1) {
					showTalkWindow (null, null);
				}
				else if (guideSid == 110002000) {//荣誉对话
					showTalkWindow (null, doGuide);
				}
				else {
					showTalkWindow (guideEvent, doGuide);
				}
			}
			else {
				showGuide ();
			}
		}
		else {
			closeGuideMask ();
		}
	}

	/// <summary>
	/// 执行一次性指引，条件满足就显示指引，反之
	/// </summary>
	public void onceGuideEvent (int sid) {
		doOnceGuide (sid);
	}

	#region 友情引导

	/** 是否进行友情引导--献祭 */
	public static bool isOnSacrifice = true;
	/** 是否进行友情引导--装备强化 */
	public static bool isOnEquip = true;
	/** 是否进行友情引导--进化 */
	public static bool isOnEvoOne = true;
	public static bool isOnEvoTwo = true;
	/** 是否进行友情引导--圣器强化 */
	public static bool isOnHallow = true;
	/** 是否进行友情引导--替补 */
	public static bool isOnSubstitute = true;
	/** 是否进行友情引导--宝箱 */
	public static bool isOnBox = true;
	/** 是否进行友情引导--主角强化 */
	public static bool isOnMainEvo = true;
	/** 是否进行友情引导--首冲 */
	public static bool isOncash=true;
	/** 是否进行友情引导--激活女神 */
	public static bool isGoddes=true;
	/**是否进行友情引导--兑换 */
	public static bool isExchagned=true;
	/** 正在进行友情引导的类型 */
	/** 是否进行友情引导--替补 */
	public static bool isOnSubstitute2 = true;
	/** 是否进行友情引导--替补 */
	public static bool isOnSubstitute3 = true;
	/** 是否进行友情引导--替补 */
	public static bool isOnSubstitute4 = true;
	/** 是否进行友情引导--替补 */
	public static bool isOnSubstitute5 = true;
	/**是否进行友情引导-- 有邮件 */
	public static bool isOnHaveMail=true;
	/**是否有女神可修炼 */
	public static bool isOnGddesTraning=true;
    /**是否进行友善引导--天国宝藏 */
    public static bool isOnMining = true;
    /**是否进行友善引导--超进化 */
    public static bool isOnSupperEvo = true;
    /**是否进行友善引导--守护天使 */
    public static bool isOnAngle = true;
	/**是否进行友善引导--诸神战小组赛 */
	public static bool isGodsWarOpenTip_1 = true;
	/**是否进行友善引导--诸神战淘汰赛 */
	public static bool isGodsWarOpenTip_2 = true;
	/**是否进行友善引导--诸神战决赛 */
	public static bool isGodsWarOpenTip_3 = true;
	/**是否进行友善引导--诸神战休赛 */
	public static bool isGodsWarOpenTip_4 = true;
    /**是否进行友善引导--装备兑换 */
    public static bool isEqExchanged = true;
    /**是否进行友善引导--神器装备 */
    public static bool isPutOnMagicWeapon = true;
    /**是否进行友善引导--女神宴*/
    public bool isHeroEatInfo = true;
    /**是否进行友善引导--卡牌进化 */
    public bool isEvoLution = true;
	/**是否进行友善引导--单挑boss */
	public bool isOneOnOneBoss = true;
    /**是否进行友善引导--装备精练 */
    public bool isJingLian = true;
	/**是否进行友善指引---神格 */
	public bool isShenGe=true;
	public bool isMoriJuezhan=true;

	public static int onType = 0;
	private bool doingSacrifice = false;
	private bool doingEquip = false;
	private bool doingEvoOne = false;
	private bool doingEvoTwo=false;
	private bool doingHallow = false;
	private bool doingSubstitute = false;
	private bool doingSubstitute2 = false;
	private bool doingSubstitute3 = false;
	private bool doingSubstitute4 = false;
	private bool doingSubstitute5 = false;
	private bool doingBox = false;
	private bool doingMainEvo = false;
	private bool doingcash=false;
	private bool doingGoddess=false;
	private bool doingExchanged=false;
	private bool doingHaveMail=false;
	private bool doingGoddessTraning=false;
    private bool doingMining = false;
    private bool doingSupperEvo = false;
    private bool doingAngel = false;
	private bool doingGodsWarOpenTip_1 = false;
	private bool doingGodsWarOpenTip_2 = false;
	private bool doingGodsWarOpenTip_3 = false;
	private bool doingGodsWarOpenTip_4 = false;
    private bool doingEqExchanged = false;
    private bool doingPutOnMagicWeapon = false;
    private bool doingHeroEatInfo = false;
    public bool doingEvoLution = false;
	private bool doingOneOnOneBoss = false;
    private bool doingJingLian = false;
	private bool doingShenGe=false;
	private bool doingMori=false;

	/** 友情引导的类型--献祭 */
	public const int TypeSacrifice = 1;
	/** 友情引导的类型--装备强化 */
	public const int TypeEquip = 2;
	/** 友情引导的类型--进化 */
	public const int TypeEvoOne = 3;
	/** 友情引导的类型--圣器强化 */
	public const int TypeHallow = 4;
	/** 友情引导的类型--替补 */
	public const int TypeSubstitute = 5;//占用55

	/** 友情引导的类型--宝箱 */
	public const int TypeBox = 6;
	/** 友情引导的类型--主角强化 */
	public const int TypeMainEvo = 7;
	/**友情指引的类型--首冲 */
	public const int TypeCash=8;
	/**友情指引的类型--女神 */
	public const int TypeGoddess=9;
	/**友情指引的类型--兑换 */
	public const int TypeExchanged=10;
	public const int TypeEvoTwo=11;//2次突破
	/** 友情引导的类型--替补 */
	public const int TypeSubstitute2 = 12;//占用55
	/** 友情引导的类型--替补 */
	public const int TypeSubstitute3 = 13;//占用55
	/** 友情引导的类型--替补 */
	public const int TypeSubstitute4 = 14;//占用55
	/** 友情引导的类型--替补 */
	public const int TypeSubstitute5 = 15;//占用55
	/**友情引导的类型--有邮件 */
	public const int TypeHaveMail=16;
	/**友情引导的烈性--有女神可以修炼 */
	public const int TypeGoddessTraning=17;
    /**友善引导的类型--天国宝藏 */
    public const int TypeMining = 18;
    /**友善引导的类型--超进化 */
    public const int TypeSupperEvo = 19;
    /**友善引导的类型--守护天使 */
    public const int TypeAngle = 20;
	/**友善引导的类型--诸神战小组赛 */
	public const int TypeGodsWar_1 = 21;
	/**友善引导的类型--诸神战淘汰赛 */
	public const int TypeGodsWar_2 = 22;
	/**友善引导的类型--诸神战决赛 */
	public const int TypeGodsWar_3 = 23;
	/**友善引导的类型--诸神战休赛 */
	public const int TypeGodsWar_4 = 24;
    /**友善引导的类型—装备合成 */
    public const int TypeEqExchanged = 25;
    /**友善引导的类型--神器装备 */
    public const int TypePutOnMagicWeapon = 26;
    /**友善引导的类型--女神宴*/
    public const int TypeHeroEatInfo = 27;
    /**友善引导的类型--卡牌进化*/
    public const int TypeEvoLution = 28;
	/**友善引导的类型--单挑boss*/
	public const int TypeOneOnOneBoss = 29;
    /** 友情引导的类型--装备精练 */
    public const int TypeJingLian = 30;
	public const int TypeShenGe=40;
	public const int TypeMori=41;


	/// <summary>
	/// 缓存对应按钮的点击次数
	/// </summary>
	/// <param name="type">类型.</param>
	public void saveTimes (int type) {
		int num = loadTimes (type);
		++num;
		PlayerPrefs.SetInt (UserManager.Instance.self.uid + PlayerPrefsComm.FRIEND_GUIDE_TIMES + type, num);
		PlayerPrefs.Save ();
	}

	/// <summary>
	/// 缓存对应按钮的点击次数
	/// </summary>
	/// <param name="type">类型.</param>
	public void saveTimesForGodsWar (int type) {
		System.DateTime date = TimeKit.getDateTime(ServerTimeKit.getSecondTime());
		int week = date.DayOfYear/7+1;
		//int tmpstr = loadTimesForGodsWar (type);
		int num=loadTimesForGodsWar (type);
		++num;
		string str=week+":"+num;
		PlayerPrefs.SetString (UserManager.Instance.self.uid + PlayerPrefsComm.FRIEND_GUIDE_TIMES + type, str);
		PlayerPrefs.Save ();
	}

	/// <summary>
	/// 获取对应按钮的点击次数
	/// </summary>
	/// <param name="type">类型.</param>
	public int loadTimes (int type) {
		return PlayerPrefs.GetInt (UserManager.Instance.self.uid + PlayerPrefsComm.FRIEND_GUIDE_TIMES + type, 0);
	}
	/// <summary>
	/// 获取对应按钮的点击次数
	/// </summary>
	/// <param name="type">类型.</param>
	public int loadTimesForGodsWar (int type) {
		string str=PlayerPrefs.GetString(UserManager.Instance.self.uid + PlayerPrefsComm.FRIEND_GUIDE_TIMES + type, "");
		if(str=="")return 0;
		int week=StringKit.toInt(str.Split(':')[0]);
		if(week==(TimeKit.getDateTime(ServerTimeKit.getSecondTime()).DayOfYear/7+1))return StringKit.toInt(str.Split(':')[1]);
		return 0;
		//return PlayerPrefs.GetInt (UserManager.Instance.self.uid + PlayerPrefsComm.FRIEND_GUIDE_TIMES + type, 0);
	}
	public int isHaveNewFriendlyGuide () {
		if (!isGuideComplete ()) {
			return 0;
		}
		int hallowNum = 0;
		if (StorageManagerment.Instance.getProp (71041) != null) {
			hallowNum += StorageManagerment.Instance.getProp (71041).getNum ();
		}
		if (StorageManagerment.Instance.getProp (71042) != null) {
			hallowNum += StorageManagerment.Instance.getProp (71042).getNum ();
		}
		if (StorageManagerment.Instance.getProp (71043) != null) {
			hallowNum += StorageManagerment.Instance.getProp (71043).getNum ();
		}
		int lv = UserManager.Instance.self.getUserLevel ();
        int now = ServerTimeKit.getSecondTime();
        //女神宴
        int[] heroEatInfo = NoticeManagerment.Instance.getHeroEatInfo();
		System.DateTime datee = TimeKit.getDateTime(now); 
		int week=TimeKit.getWeekCHA(datee.DayOfWeek);
		if(UserManager.Instance.self.getUserLevel()>=42&&isMoriJuezhan&&loadTimesForGodsWar(TypeMori)<1&&week==3&&datee.Hour>=9){
			doingMori=true;
			return TypeMori;



		}
		if(UserManager.Instance.self.getUserLevel()>=58&&isShenGe&&loadTimes(TypeShenGe)<1){
			doingShenGe=true;
			return TypeShenGe;
		}
        int pvePower = CommonConfigSampleManager.Instance.getSampleBySid<PvePowerMaxSample>(CommonConfigSampleManager.PvePowerMax_SID).pvePowerMax;
        if(now>=heroEatInfo[1]&&now<=heroEatInfo[2]&&isHeroEatInfo&&loadTimes(TypeHeroEatInfo)<1)//女神宴的新手指引
        {
            if (UserManager.Instance.self.getPvEPoint() < pvePower&&lv>=10&&heroEatInfo[3]==0)
            {
                doingHeroEatInfo = true;
                return TypeHeroEatInfo;
            }
        }
        //卡片进
        if (UserManager.Instance.self.getUserLevel()>=15&&isEvoLution&&IncreaseManagerment.Instance.geCommendCardEvolutionList().Count > 0 && loadTimes(TypeEvoLution) < 1)
	    {
            doingEvoLution = true;
            return TypeEvoLution;
	    }

		// 单挑boss//
		DateTime dt = TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());//获取服务器时间
		int dayOfWeek = TimeKit.getWeekCHA(dt.DayOfWeek);
		int nowOfDay = ServerTimeKit.getCurrentSecond();
		int[] timeInfo = CommandConfigManager.Instance.getOneOnOneBossTimeInfo();//开放时间
		int[] data = CommandConfigManager.Instance.getOneOnOneBossData();//开放日期
		if(isOneOnOneBoss && lv >= CommandConfigManager.Instance.getOneOnOneBossLimitLv() && loadTimes(TypeOneOnOneBoss) < 1)
		{
			for(int i=0;i<data.Length;i++)
			{
				if(dayOfWeek == data[i] && (nowOfDay >= timeInfo[0] && nowOfDay <= timeInfo[1]))
				{
					doingOneOnOneBoss = true;
					return TypeOneOnOneBoss;
				}
			}
		}
            //休赛预告
        if (isGodsWarOpenTip_4 && TimeKit.getWeekCHA(ServerTimeKit.getDateTime().DayOfWeek) == 7 && lv > 35 && loadTimesForGodsWar(TypeGodsWar_4) < 1 && GodsWarManagerment.Instance.isOnlineDay30())
        {
            doingGodsWarOpenTip_4 = true;
            return TypeGodsWar_4;
        }
        //小组赛阶段
		if (isGodsWarOpenTip_1 && GodsWarManagerment.Instance.getWeekOfDayState() == 2&& lv > 35 && loadTimesForGodsWar(TypeGodsWar_1) < 1 && GodsWarManagerment.Instance.getGodsWarStateInfo().StartsWith("have_zige")&& GodsWarManagerment.Instance.isOnlineDay30())
		{
			doingGodsWarOpenTip_1 = true;
			return TypeGodsWar_1;
		}
		//半决赛开启预告
		if (isGodsWarOpenTip_2 && GodsWarManagerment.Instance.getWeekOfDayState() == 3 && loadTimesForGodsWar(TypeGodsWar_2) < 1 && GodsWarManagerment.Instance.isTaoTaiOpen()&& GodsWarManagerment.Instance.isOnlineDay30())
		{
			doingGodsWarOpenTip_2 = true;
			return TypeGodsWar_2;
		}
		//决赛开启预告
		if (isGodsWarOpenTip_3 && GodsWarManagerment.Instance.getWeekOfDayState() == 4 && loadTimesForGodsWar(TypeGodsWar_3) < 1 && GodsWarManagerment.Instance.isFinalOpen()&& GodsWarManagerment.Instance.isOnlineDay30())
		{
			doingGodsWarOpenTip_3 = true;
			return TypeGodsWar_3;
		}
	    if (isPutOnMagicWeapon && lv >= 40 && StorageManagerment.Instance.checkHaveCommendMagicWeapon() &&
	        loadTimes(TypePutOnMagicWeapon) < 1)
	    {
	        doingPutOnMagicWeapon = true;
	        return TypePutOnMagicWeapon;
	    }
        if (isOnMining && lv >= 30 && loadTimes(TypeMining) < 1)
        {
            doingMining = true;
            return TypeMining;

        }
        if (isOnSupperEvo && IntensifyCardManager.Instance.isShowSuperEvoGuide() && loadTimes(TypeSupperEvo) < 1)
        {
            doingSupperEvo = true;
            return TypeSupperEvo;
        }
		//激活女神
		if(isGoddes&&lv>5&&UserManager.Instance.self.getMoney()>=200000&&UserManager.Instance.self.merit>=12000&&(StorageManagerment.Instance.getAllBeast()==null||(StorageManagerment.Instance.getAllBeast()!=null&&StorageManagerment.Instance.getAllBeast().Count<=3))&&loadTimes (TypeGoddess)<1){
			doingGoddess=true;
			return TypeGoddess;
		}
		//主角一次突破
		if(isOnEvoOne){
			Card mainCard=StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid);
			if(mainCard.getSurLevel()<1&&SurmountManagerment.Instance.isCanSurLevel(mainCard)&&SurmountManagerment.Instance.isCanSurByConOnly(mainCard)&&loadTimes (TypeEvoOne)<1){
				doingEvoOne=true;
				return TypeEvoOne;
			}
		}
		//主角二次突破
		if(isOnEvoTwo){
			Card mainCard=StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid);
			if(mainCard.getSurLevel()==1&&SurmountManagerment.Instance.isCanSurLevel(mainCard)&&SurmountManagerment.Instance.isCanSurByConOnly(mainCard)&&loadTimes (TypeEvoTwo)<1){
				doingEvoTwo=true;
				return TypeEvoTwo;
			}
		}
		//兑换
		if(isExchagned&&CardScrapManagerment.Instance.CanExchangeCard()){
			doingExchanged=true;
			return TypeExchanged;
		}
        //装备兑换
        if (isEqExchanged && EquipScrapManagerment.Instance.CanExchangeEq())
        {
            doingEqExchanged = true;
            return TypeEqExchanged;
        }
		//首冲
		if(isOncash&&lv==19&&RechargeManagerment.Instance.getOneRmbState()==1&&loadTimes (TypeCash) < 1){
			doingcash=true;
			return TypeCash;
		}
		//装备
		else if (isOnEquip && lv >= 5 && lv <= 20 && StorageManagerment.Instance.getAllEquipByEat () != null &&
			StorageManagerment.Instance.getAllEquipByEat ().Count >= 8 && loadTimes (TypeEquip) < 3 && isMoreThanStep (123001000)) {
			doingEquip = true;
			return TypeEquip;
		}
        if (isJingLian && lv >= 30 && loadTimes(TypeJingLian) < 1 && StorageManagerment.Instance.isHaveJingLianEquip())
        {
            doingJingLian = true;
            return TypeJingLian;
        }
		//献祭
		if (isOnSacrifice && lv >= 7 && lv <= 20 && StorageManagerment.Instance.getAllRole () != null &&
			StorageManagerment.Instance.getAllRole ().Count >= 10 && loadTimes (TypeSacrifice) < 3) {
			doingSacrifice = true;
			return TypeSacrifice;
		}
		//圣器
//		else if (isOnHallow && lv >= 9 && lv <= 20 && BeastEvolveManagerment.Instance.getSkillLv () < 5 && hallowNum >= 8) {
		if (isOnHallow && lv >= 10 && lv <= 20 && ( BeastEvolveManagerment.Instance.getLaveHallowConut () > 0 || hallowNum >= 10)&&FuBenManagerment.Instance.isNewMission (ChapterType.STORY,CommandConfigManager.Instance.getPassMissionSid()+1)&&!FuBenManagerment.Instance.isNewMission (ChapterType.STORY,CommandConfigManager.Instance.getPassMissionSid())) {  //检查条件：是否有未使用的剩余次数，2、各种宝石的合计数量是否大于10
			doingHallow = true;
			return TypeHallow;
		}
		//主角培养
		if (isOnMainEvo && lv >= 21 && lv <= 30 && StorageManagerment.Instance.getProp (71054) != null &&
			StorageManagerment.Instance.getProp (71054).getNum () > 10 && loadTimes (TypeMainEvo) < 1&&
			StorageManagerment.Instance.getRole (UserManager.Instance.self.mainCardUid).getEvoLevel () < 10 && isMoreThanStep (107001000)) {
			doingMainEvo = true;
			return TypeMainEvo;
		}
		if(isOnSubstitute&&lv>=TeamUnluckManager.Instance.getMinLv()&&openIndex.Count==0&&loadTimes(TypeSubstitute)<1){
			doingSubstitute = true;
			return TypeSubstitute;
			
		}
		if(isOnSubstitute2&&lv>=TeamUnluckManager.Instance.getMinLv()&&openIndex.Count==1&&loadTimes(11+openIndex.Count)<1&&TeamUnluckManager.Instance.getConditionSuccess(openIndex)){
			doingSubstitute2 = true;
			return TypeSubstitute2;
			
		}
		if(isOnSubstitute3&&lv>=TeamUnluckManager.Instance.getMinLv()&&openIndex.Count==2&&loadTimes(11+openIndex.Count)<1&&TeamUnluckManager.Instance.getConditionSuccess(openIndex)){
			doingSubstitute3 = true;
			return TypeSubstitute3;
			
		}
		if(isOnSubstitute4&&lv>=TeamUnluckManager.Instance.getMinLv()&&openIndex.Count==3&&loadTimes(11+openIndex.Count)<1&&TeamUnluckManager.Instance.getConditionSuccess(openIndex)){
			doingSubstitute4 = true;
			return TypeSubstitute4;
			
		}
		if(isOnSubstitute5&&lv>=TeamUnluckManager.Instance.getMinLv()&&openIndex.Count==4&&loadTimes(11+openIndex.Count)<1&&TeamUnluckManager.Instance.getConditionSuccess(openIndex)){
			doingSubstitute5 = true;
			return TypeSubstitute5;
			
		}
		//开宝箱
		if (isOnBox && lv > 15 && loadTimes (TypeBox) < 1 &&
			(StorageManagerment.Instance.getProp (71031) != null || StorageManagerment.Instance.getProp (71032) != null)) {
			doingBox = true;
			return TypeBox;
       }
        //女神修炼
         if (isOnGddesTraning && lv >= GoddessTrainingSampleManager.Instance.getDataBySid(22).lvCondition && BeastEvolveManagerment.Instance.haveCanTranningBeast() && goddessTranningTime - ServerTimeKit.getSecondTime() < 0)
        {
            doingGoddessTraning = true;
            return TypeGoddessTraning;
        }//邮箱
        if (isOnHaveMail&&(MailManagerment.Instance.getUnReadMailNum () + StorageManagerment.Instance.getAllTemp ().Count) > 0){
			doingHaveMail=true;
			return TypeHaveMail;
        } 
        if (isOnAngle && PlayerPrefs.GetString(PlayerPrefsComm.ANGEL_USER_NAME + UserManager.Instance.self.uid) == "not" && UserManager.Instance.self.getVipLevel() > 0&&loadTimes(TypeAngle)<1) {
            doingAngel = true;
            return TypeAngle;
        }
		withoutFriendlyGuide ();
		return 0;
	}

	/// <summary>
	/// 只有设置了状态值，才能走对应的友善引导
	/// </summary>
	/// <param name="type">Type.</param>
	public void setOnType (int type) {
		onType = type;
	}

	/// <summary>
	/// 尝试执行友情指引
	/// </summary>
	public void doFriendlyGuideEvent () {
		if (!isGuideComplete () || onType == 0) {
			return;
		}
		int lv = UserManager.Instance.self.getUserLevel ();

        if(onType==TypeSupperEvo&&doingSupperEvo&&isOnSupperEvo){
            //if (forEachSidComplete(TypeSupperEvo, 818001000, 818002000)) return;
            withoutFriendlyGuide();

            IntensifyCardManager.Instance.setMainCard(IntensifyCardManager.Instance.getSuperEvoGuideCard());
            IntensifyCardManager.Instance.intoIntensify(IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO, null);
            saveTimes(TypeSupperEvo);
           
        }
		if(onType==TypeMori&&doingMori&&isMoriJuezhan){
			if(forEachSidComplete(TypeMori,861001000,861003000))return;
		}
		if(onType==TypeShenGe&&doingShenGe&&isShenGe){
			if (forEachSidComplete(TypeShenGe, 851001000, 851003000))
				return;
		}
        //卡片进化
        if(onType==TypeEvoLution&&doingEvoLution&&isEvoLution)
        {
            if (forEachSidComplete(TypeEvoLution, 821001000, 821005000))
                return;
        }
        //女神宴
        if(onType==TypeHeroEatInfo&&doingHeroEatInfo&&isHeroEatInfo)
        {
            withoutFriendlyGuide();
            UiManager.Instance.openWindow<NoticeWindow>((win) => {
                win.updateSelectButton(NoticeType.STICKNOTICE);
                win.entranceId = NoticeEntranceType.DAILY_NOTICE;
            });
            saveTimes(TypeHeroEatInfo);
        }
		//休赛预告
        if (onType == TypeGodsWar_4 && doingGodsWarOpenTip_4 && isGodsWarOpenTip_4)
	    {
            withoutFriendlyGuide();
			string currentState = GodsWarManagerment.Instance.getGodsWarStateInfo();
            if (currentState == GodsWarManagerment.STATE_PREPARE)
	        {
                UiManager.Instance.openWindow<GodsWarPreparWindow>((win) => {
                    saveTimesForGodsWar(TypeGodsWar_4);
                });
	        }
            //&&currentState!=GodsWarManagerment.STATE_SERVER_BUSY)
			if(currentState!=GodsWarManagerment.STATE_NOTOPEN){//开启
				//有资格且处于决赛
				if(currentState == GodsWarManagerment.STATE_HAVE_ZIGE_FINAL){
					UiManager.Instance.openWindow<GodsWarFinalWindow>((win)=>{
						//有资格且处于决赛，则默认进入所在组决赛界面
						GodsWarManagerment.Instance.type = GodsWarManagerment.Instance.big_id;
						GodsWarManagerment.Instance.tapIndex = 0;
					});
				}
				//没有资格且处于决赛
				if(currentState == GodsWarManagerment.STATE_NOT_ZIGE_FINAL){
					UiManager.Instance.openWindow<GodsWarFinalWindow>((win)=>{
						//默认进入黄金-神魔大战组
						GodsWarManagerment.Instance.type = 2;
						GodsWarManagerment.Instance.tapIndex = 0;
					});
				}
			}else {
				UiManager.Instance.openWindow<GodsWarPreparWindow>((win)=>{
					saveTimesForGodsWar(TypeGodsWar_4);
				});
			}
	    }
		//小组赛预告
		if (onType == TypeGodsWar_1 && doingGodsWarOpenTip_1 && isGodsWarOpenTip_1)
		{
			withoutFriendlyGuide();
			UiManager.Instance.openWindow<GodsWarGroupStageWindow>((win)=>{
				saveTimesForGodsWar(TypeGodsWar_1);
			});   
		}
		//半决赛预告
		if (onType == TypeGodsWar_2 && doingGodsWarOpenTip_2 && isGodsWarOpenTip_2)
		{
			withoutFriendlyGuide(); 
			UiManager.Instance.openWindow<GodsWarFinalWindow>((win)=>{
				if(GodsWarManagerment.Instance.taoTaiZige)
					win.setBigidAndYuming();
				else
					GodsWarManagerment.Instance.type = GodsWarManagerment.Instance.big_id;

				saveTimesForGodsWar(TypeGodsWar_2);
			});
		}
		//冠军赛预告
		if (onType == TypeGodsWar_3 && doingGodsWarOpenTip_3 && isGodsWarOpenTip_3)
		{
			withoutFriendlyGuide(); 
			string currentState = GodsWarManagerment.Instance.getGodsWarStateInfo();
			if(currentState == GodsWarManagerment.STATE_HAVE_ZIGE_FINAL){
				UiManager.Instance.openWindow<GodsWarFinalWindow>((win)=>{
					//有资格且处于决赛，则默认进入所在组决赛界面
					GodsWarManagerment.Instance.type = GodsWarManagerment.Instance.big_id;
					GodsWarManagerment.Instance.tapIndex = 0;
				});
			}
			if(currentState == GodsWarManagerment.STATE_NOT_ZIGE_FINAL){
				UiManager.Instance.openWindow<GodsWarFinalWindow>((win)=>{
					GodsWarManagerment.Instance.type = 2;
					GodsWarManagerment.Instance.tapIndex = 0;
				});
			}
            if (currentState == GodsWarManagerment.STATE_HAVE_ZIGE_TAOTAI || currentState == GodsWarManagerment.STATE_NOT_ZIGE_TAOTAI) {
                UiManager.Instance.openWindow<GodsWarFinalWindow>((win) => {
                    if (GodsWarManagerment.Instance.taoTaiZige)
                        win.setBigidAndYuming();
                    else
                        GodsWarManagerment.Instance.type = GodsWarManagerment.Instance.big_id;
                });
            }
			saveTimesForGodsWar(TypeGodsWar_3);
			
		}
            //通用神器装备
        else if (onType==TypePutOnMagicWeapon&&doingPutOnMagicWeapon&&isPutOnMagicWeapon)
        {
            if (forEachSidComplete(TypePutOnMagicWeapon, 820001000, 820003000))
            {
                return;
            }
        }
        else if(onType==TypeMining&&doingMining&&isOnMining){
            if (forEachSidComplete(TypeMining, 817001000, 817002000))
                return;
		}
		else 
		//激活女神
		if(onType==TypeGoddess&&doingGoddess&&isGoddes){
			if (forEachSidComplete (TypeGoddess, 807001000, 807001000))
				return;
		}
		else if(onType==TypeEvoOne&&doingEvoOne&&isOnEvoOne){
			if(forEachSidComplete(TypeEvoOne,809001000,809003000))return;
		}
		else if(onType==TypeEvoTwo&&doingEvoTwo&&isOnEvoTwo){
			if(forEachSidComplete(TypeEvoTwo,810001000,810003000))return;
		}
		//卡片兑换
		else if(onType==TypeExchanged&&doingExchanged&&isExchagned){
			if (forEachSidComplete (TypeExchanged, 808001000, 808003000))
				return;
		}
        else if (onType==TypeEqExchanged&&doingEqExchanged&&isEqExchanged)
        {
            if (forEachSidComplete(TypeEqExchanged, 819001000, 819003000)) return;
        }
		//首冲
		else if(onType==TypeCash&&doingcash&&isOncash){
			if (forEachSidComplete (TypeCash, 806001000, 806001000))
				return;
		}
		//装备
		else if (onType == TypeEquip && doingEquip && isOnEquip) {
			if (forEachSidComplete (TypeEquip, 800001000, 800004000))
				return;
		}
        else if (onType==TypeJingLian&&doingJingLian&&isJingLian)
        {
            if (forEachSidComplete(TypeJingLian, 841001000, 841007000))
                return;
        }
		//献祭
		else if (onType == TypeSacrifice && doingSacrifice && isOnSacrifice) {
			if (forEachSidComplete (TypeSacrifice, 801001000, 801003000))
				return;
		}
		//圣器
		else if (onType == TypeHallow && doingHallow && isOnHallow) {
			if (forEachSidComplete (TypeHallow, 802001000, 802002000))
				return;
		}
		//主角培养
		else if (onType == TypeMainEvo && doingMainEvo && isOnMainEvo) {
			if (forEachSidComplete (TypeMainEvo, 803001000, 803003000))
				return;
		}
		//替补
		else if (onType == TypeSubstitute && doingSubstitute && isOnSubstitute) {
			if (forEachSidComplete (TypeSubstitute, 804001000, 804002000))
				return;
		}
		//替补
		else if (onType == TypeSubstitute2 && doingSubstitute2 && isOnSubstitute2) {
			if (forEachSidComplete (TypeSubstitute2, 811001000, 811002000))
				return;
		}
		//替补
		else if (onType == TypeSubstitute3 && doingSubstitute3 && isOnSubstitute3) {
			if (forEachSidComplete (TypeSubstitute3, 812001000, 812002000))
				return;
		}
		//替补
		else if (onType == TypeSubstitute4 && doingSubstitute4 && isOnSubstitute4) {
			if (forEachSidComplete (TypeSubstitute4, 813001000, 813002000))
				return;
		}
		//替补
		else if (onType == TypeSubstitute5 && doingSubstitute5 && isOnSubstitute5) {
			if (forEachSidComplete (TypeSubstitute5, 814001000, 814002000))
				return;
		}
		//开宝箱
		else if (onType == TypeBox && doingBox && isOnBox) {
			if (forEachSidComplete (TypeBox, 805001000, 805003000))
				return;
		}else if(onType==TypeHaveMail&&doingHaveMail&&isOnHaveMail){
			if(forEachSidComplete(TypeHaveMail,815001000,815002000)){
				return;
			}
		}else if(onType==TypeGoddessTraning&&doingGoddessTraning&&isOnGddesTraning){
			if(forEachSidComplete(TypeGoddessTraning,816001000,816002000)){
				return;
			}
        } else if (onType == TypeAngle && doingAngel && isOnAngle) {
            if (forEachSidComplete(TypeAngle, 818001000, 818002000)) {
                return;
            }
        }
		// 单挑boss//
		else if(onType == TypeOneOnOneBoss && doingOneOnOneBoss && isOneOnOneBoss)
		{
			if (forEachSidComplete(TypeOneOnOneBoss, 831001000, 831003000)) {
				return;
			}
		}
		
		withoutFriendlyGuide ();
	}

	/** 在循环里面找对应步骤，合适就执行，不合适就抛 */
	private bool forEachSidComplete (int type, int startSid, int overSid) {
		onType = type;
		for (int i = startSid; i <= overSid; i+=1000) {    
			if (!isFriendlyGuideComplete (i)) {
				doFriendlyGuideEvent (i);
				return true;
			}
		}
		withoutFriendlyGuide ();
		return false;
	}

	/// <summary>
	/// 执行指定友情指引
	/// </summary>
	public void doFriendlyGuideEvent (int sid) {
		if (friendlyGuideList == null) {
			friendlyGuideList = new List<int> ();
		}
		if(sid==861003000){
			saveTimesForGodsWar(TypeMori);
		}
		if(sid==851003000){
			saveTimes(TypeShenGe);
		}
	    if (sid == 820003000)
	    {
	        saveTimes(TypePutOnMagicWeapon);
	    }
		else if (sid == 805003000) {
			saveTimes (TypeBox);
		}
        else if(sid==818002000){
            saveTimes(TypeAngle);
        }
		else if (sid == 804002000) {
			saveTimes (TypeSubstitute);
		}
		else if (sid == 811002000) {
			saveTimes (TypeSubstitute2);
		}
		else if (sid == 812002000) {
			saveTimes (TypeSubstitute3);
		}
		else if (sid == 813002000) {
			saveTimes (TypeSubstitute4);
		}
		else if (sid == 814002000) {
			saveTimes (TypeSubstitute5);
		}
		else if (sid == 803003000) {
			saveTimes (TypeMainEvo);
		}else if(sid == 809003000){
			saveTimes(TypeEvoOne);
		}else if(sid==810003000){
			saveTimes(TypeEvoTwo);
        }
        else if (sid == 817002000)
        {
            saveTimes(TypeMining);
        } else if (sid == 818002000) {
            saveTimes(TypeAngle);
        }
        else if (sid == 821001000)
        {
            saveTimes(TypeHeroEatInfo);
        } else if (sid == 821005000) {
            saveTimes(TypeEvoLution);
        }
		else if(sid == 831003000)
		{
			saveTimes(TypeOneOnOneBoss);
		} else if (sid == 841002000)
		{
		    saveTimes(TypeJingLian);
		}
		friendlyGuideList.Add (sid);
		showGuide (sid);
		//MaskWindow.UnlockUI ();
	}

    public int getOnTypp()
    {
        return onType;
    }
	/// <summary>
	/// 判断友情指引是否完结
	/// </summary>
	public bool isFriendlyGuideComplete (int guideSid) {
		if (friendlyGuideList == null || friendlyGuideList.Count < 1)
			return false;
		if (friendlyGuideList.Contains (guideSid))
			return true;
		return false;
	}

	/// <summary>
	/// 放弃友情指引
	/// </summary>
	public void withoutFriendlyGuide () {
		if (onType != 0 && guideUI != null && guideUI.friendlyButtonMask.gameObject.activeSelf) {
			guideUI.closeGuide ();
		}
		if (friendlyGuideList != null && friendlyGuideList.Count > 0) {
			friendlyGuideList.Clear ();
		}
		switch (onType) {
		case TypeMori:
			isMoriJuezhan=false;
			break;
		case TypeShenGe:
			isShenGe=false;
			break;
        case TypePutOnMagicWeapon:
            isPutOnMagicWeapon = false;
            break;
		case TypeGodsWar_1:
			isGodsWarOpenTip_1 = false;
			break;
		case TypeGodsWar_2:
			isGodsWarOpenTip_2 = false;
			break;
		case TypeGodsWar_3:
			isGodsWarOpenTip_3 = false;
			break;
		case TypeGodsWar_4:
			isGodsWarOpenTip_4 = false;
			break;
        case TypeAngle:
                isOnAngle = false;
                break;
         case TypeSupperEvo:
               isOnSupperEvo = false;
               break;
		case TypeGoddess:
			isGoddes = false;
			break;

		case TypeExchanged:
			isExchagned = false;
			break;
        case TypeEqExchanged:
            isEqExchanged = false;
            break;    
		case TypeCash:
			isOncash = false;
			break;

		case TypeSacrifice:
			isOnSacrifice = false;
			break;
			
		case TypeEquip:
			isOnEquip = false;
			break;
            case TypeJingLian:
		        isJingLian = false;
		        break;
		case TypeEvoOne:
			isOnEvoOne = false;
			break;
			
		case TypeHallow:
			isOnHallow = false;
			break;
			
		case TypeSubstitute:
			isOnSubstitute = false;
			break;
			
		case TypeBox:
			isOnBox = false;
			break;
			
		case TypeMainEvo:
			isOnMainEvo = false;
			break;
		case TypeHaveMail:
			isOnHaveMail=false;
			break;
		case TypeGoddessTraning:
			isOnGddesTraning=false;
			break;
        case TypeMining:
                isOnMining=false;
                break;
        case TypeHeroEatInfo:
                isHeroEatInfo = false;
                break;
        case TypeEvoLution:
		        doingEvoLution = false;
                isEvoLution = false;
                break;
		case TypeOneOnOneBoss:
			doingOneOnOneBoss = false;
			isOneOnOneBoss = false;
			break;
		default:
			onType = 0;
			break;
		}
		onType = 0;
	}

	/// <summary>
	/// 获取对应友情指引对话
	/// </summary>
	public string getFriendlyGuideStr (int type) {
		if (type != 0) {
			return LanguageConfigManager.Instance.getLanguage ("GuideF_" + type);
		}
		else {
			return "";
		}
	}

	#endregion
}
