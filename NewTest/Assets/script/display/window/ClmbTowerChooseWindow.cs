using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ClmbTowerChooseWindow : WindowBase
{
	[HideInInspector]
	public int[] missionList;//章节列表
	public Transform missionsParentPos;//副本创建父集
	public Transform bgPos;//底层背景图父集
	public Transform bgCenterPos;//中间层背景图父集
	public UIPanel panel;//章节容器
	public GameObject itemPrefab;
	public GameObject newItemRayPrefab;//新副本底纹
	public UILabel fubenName;//章节名字
	public UITexture[] centerBackGround;//中间层背景图
    public UILabel tiaozhanNum;//今日挑战的次数
    public UILabel openNum;//今日开启宝箱的次数
    public ButtonBase beginTowerButton;//进入副本按键
    public GameObject goodsButtonPrefab;
    //帮助信息
    public UIPlayTween tweenHelp;
    public UIPlayTween tweenMessage;
    public UISprite atteckSprite;
    public barCtrl bloodBar;
	private List<TowerItem> itemList;
	private TowerItem lastItem;
	private int chapterSid;
    private Mission currectMissionSid;
	/** 背景是否可以移动 */
	private bool isBgCanMove = true;
    private bool tweenerMessageState;

    /// <summary>
    /// 宝箱开启未完成点击挑战的回调
    /// </summary>
    /// <param name="msg"></param>
    public void callBack(MessageHandle msg) {
        if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
            if (!FuBenManagerment.Instance.canBeAttack()) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow38"));
                });
                return;
            }
            if (atteckSprite.spriteName != "icon_PK") {//这里是重置副本TowerResetFPort
                TowerResetFPort pro = FPortManager.Instance.getFPort("TowerResetFPort") as TowerResetFPort;
                Chapter towChapter = FuBenManagerment.Instance.getTowerChapter();
                if (towChapter.reAttackNum >= towChapter.reAttackMaxNum) {
                    UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                        win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow21"));
                    });
                    return;
                }
                pro.access(() => {
                    UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                        win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow09"));
                    });
                    updateInfo();
                    MaskWindow.UnlockUI();
                });
                return;
            }
            currectMissionSid = MissionInfoManager.Instance.getMissionBySid(FuBenManagerment.Instance.getPassChapter());
            if (UserManager.Instance.self.getUserLevel() < MissionSampleManager.Instance.getMissionSampleBySid(currectMissionSid.sid).level) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow14"));
                });
            } else {//放弃抽奖进入副本
                ClmbTowerManagerment.Instance.giveUpAward();
                FuBenManagerment.Instance.selectedMissionSid = currectMissionSid.sid;
                intoTower();//到了这一步肯定是新进入FB
            }
        } else if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
            TowerBeginAwardInfo fport = FPortManager.Instance.getFPort("TowerBeginAwardInfo") as TowerBeginAwardInfo;
            fport.access(intoTowerShow);
        }
    }
    void intoTowerShow(int i) {
        if (i != 0) {//奖池里有东西的情况（）飘字提示有宝箱没有开完
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow30"));
            });
            ClmbTowerManagerment.Instance.countieOPenBox(ClmbTowerManagerment.Instance.missionSid);
        }
    }
    void intoTower(int i) {
        clickAttack();
    }
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			setBgIsCanMove (false);
			finishWindow ();
        } else if (gameObj.name == "shopButton") { 
            UiManager.Instance.openWindow<NvshenShopWindow>((win) =>{
                win.initContent();
            });
        } else if (gameObj.name == "teamEdit") {
            UiManager.Instance.openWindow<TeamEditWindow>((win) => {
                win.setComeFrom(1);
            });
        } else if (gameObj.name == "beginAttack") {//开始挑战
            TowerBeginAwardInfo fport = FPortManager.Instance.getFPort("TowerBeginAwardInfo") as TowerBeginAwardInfo;
            fport.access(intoTower);
            //clickAttack();
        } else if (gameObj.name == "buttonHelp") {
            tweenerMessageState = false;
            tweenerMessageGroupOut(tweenMessage);
            tweenHelp.gameObject.SetActive(true);
            tweenerMessageState = true;
            tweenerMessageGroupIn(tweenHelp);
        } else if (gameObj.name == "buttonCloseHelp") {
            tweenerMessageState = false;
            tweenerMessageGroupOut(tweenHelp);
        } else if (gameObj.name == "box") {
            UiManager.Instance.openDialogWindow<BoxShowWindow>();
        }
	}
    public void clickAttack() {
        if (ClmbTowerManagerment.Instance.turnSpriteData != null && atteckSprite.spriteName == "icon_PK") {
            UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                win.initWindow(2, LanguageConfigManager.Instance.getLanguage("towerShowWindow45"),
                    LanguageConfigManager.Instance.getLanguage("towerShowWindow46"), LanguageConfigManager.Instance.getLanguage("towerShowWindow47"), callBack);
                win.closeButton.SetActive(true);
            });
            //UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            //    win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow42"));
            //});
            return;
        }
        if (!FuBenManagerment.Instance.canBeAttack()) {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow38"));
            });
            return;
        }
        if (atteckSprite.spriteName == "icon_PK") {//这里才是真的挑战
            currectMissionSid = MissionInfoManager.Instance.getMissionBySid(FuBenManagerment.Instance.getPassChapter());
            if (UserManager.Instance.self.getUserLevel() < MissionSampleManager.Instance.getMissionSampleBySid(currectMissionSid.sid).level) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow14"));
                });
            } else {

                FuBenManagerment.Instance.selectedMissionSid = currectMissionSid.sid;
                intoTower();//到了这一步肯定是新进入FB
            }
        } else {//这里是重置副本TowerResetFPort
            TowerResetFPort pro = FPortManager.Instance.getFPort("TowerResetFPort") as TowerResetFPort;
            Chapter towChapter = FuBenManagerment.Instance.getTowerChapter();
            if (towChapter.reAttackNum >= towChapter.reAttackMaxNum) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow21"));
                });
                return;
            }
            pro.access(() => {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow09"));
                });
                updateInfo();
                MaskWindow.UnlockUI();
            });
        }
    }
    /** 动态消息进入动画  */
    public void tweenerMessageGroupIn(UIPlayTween tween) {
        tween.playDirection = AnimationOrTween.Direction.Forward;
        UITweener[] tws = tween.GetComponentsInChildren<UITweener>();
        foreach (UITweener each in tws) {
            each.delay = 0.4f;
        }
        tween.Play(true);
    }
    /** 动态消息退出动画  */
    public void tweenerMessageGroupOut(UIPlayTween tween) {
        tween.playDirection = AnimationOrTween.Direction.Reverse;
        UITweener[] tws = tween.GetComponentsInChildren<UITweener>();
        foreach (UITweener each in tws) {
            each.delay = 0;
        }
        tween.Play(true);
    }
    /** 动态消息完成做的事情  */
    public void tweenerMessageGroupFinsh() {
        if (tweenerMessageState) { // 展开
            tweenMessage.gameObject.SetActive(true);
        } else { // 收拢
            tweenMessage.gameObject.SetActive(false);
        }
        MaskWindow.UnlockUI();
    }

    public void intoTower() {
        FuBenIntoFPort port = FPortManager.Instance.getFPort("FuBenIntoFPort") as FuBenIntoFPort;
        if (currectMissionSid.getChapterType() == ChapterType.TOWER_FUBEN) {//说明是爬塔副本
            UiManager.Instance.openWindow<EmptyWindow>((win) => {
                MissionInfoManager.Instance.saveMission(currectMissionSid.sid, 1);
                ArmyManager.Instance.setActive(1);//一旦进去把竞技队伍设置为活动副本
                port.intoTowerFuben(currectMissionSid.sid, 1, ArmyManager.Instance.getActiveArmy().armyid, FuBenIntoFPort.intoTowerMission);
            });     
        }
    }
    
	protected override void DoEnable ()
	{
		base.DoEnable ();
		setBgIsCanMove (false);
		//UiManager.Instance.storyMissionWindow = this;
		for (int i = 0; i < missionsParentPos.childCount; i++) {
			Destroy (missionsParentPos.GetChild (i).gameObject);
		}
		refreshData ();
        UiManager.Instance.backGround.switchBackGround("tower_bg", () => { setBgIsCanMove(true); });
        updateInfo();

	}
    void updateInfo() {
        Chapter towChapter=FuBenManagerment.Instance.getTowerChapter();
        openNum.text = towChapter.relotteryNum.ToString() + "/" + towChapter.lotteryMaxNum.ToString();
       // bloodBar.updateValue(UserManager.Instance.self.getPvEPoint(), UserManager.Instance.self.getPvEPointMax());
        if (towChapter.isattack) {
           // beginTowerButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow07");
            atteckSprite.spriteName = "icon_PK";
        } else {
            //beginTowerButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow08");
            atteckSprite.spriteName = "reset";
        }
        tiaozhanNum.text = (towChapter.isattack?0:1).ToString() ;
    }

	public override void DoDisable ()
	{
		base.DoDisable ();
		UiManager.Instance.storyMissionWindow = null;
		UiManager.Instance.backGround.ReturnFromMissionChooseWindow ();
	}

	protected override void begin ()
	{
		base.begin ();
        updateUI(null);
        GuideManager.Instance.guideEvent();
		
	}
    void updateUI(BattleFormationCard[] battles) {
        createItem();
        if (!isAwakeformHide) {
            jumpToMission();
        } else {
            jumpToMissionFromHide();
        }
        GuideManager.Instance.guideEvent();
        StartCoroutine(Utils.DelayRun(() => {
            MaskWindow.UnlockUI();
        }, 0.5f));
    }

	public void refreshData ()
	{
        missionList = FuBenManagerment.Instance.getAllShowTowerChapter(1);
        chapterSid = FuBenManagerment.Instance.selectedChapterSid;// 得到爬塔层数列表
        fubenName.text = LanguageConfigManager.Instance.getLanguage("towerSHowWindow11");//
	}
	/**更新坐骑或普通行动力 */
	void createItem ()
	{
		if (itemList != null) {
			itemList.Clear ();
			itemList = null;
		}
		lastItem = null;
		itemList = new List<TowerItem> ();
		//解决部分机型高宽不分的BUG
		int width = Screen.width < Screen.height ? Screen.width : Screen.height;
		float y = 0;//每个副本点位之间的高度差
		float x = 0;//每一行X坐标
		float centerX = 0;//屏幕中间值
		GameObject a;
        for (int i = 0; i < missionList.Length; i++) {
            Mission mission = MissionInfoManager.Instance.getMissionBySid(missionList[i]);
            centerX = (width * UiManager.Instance.screenScaleX) / 2 - (width / 3);

            if (i % 2 == 0) {
                x = 144;
            } else {
                x = -126;
            }
           // x = UnityEngine.Random.Range(-2 * centerX, 2*centerX);
            a = Instantiate(itemPrefab) as GameObject;
            TowerItem item = a.GetComponent<TowerItem>();
            item.name = StringKit.intToFixString(i + 1);
            item.initButton(mission, i + 1,this);
            item.transform.parent = missionsParentPos;
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = new Vector3(x, y, 0);
            y += 182;
            if (mission.sid == FuBenManagerment.Instance.getPassChapter()) {//需要通的SID
                lastItem = item;
                NGUITools.AddChild(lastItem.gameObject, newItemRayPrefab);
            }
            itemList.Add(item);
        }
		//这里是连线的
		for (int i = 0; i < itemList.Count; i++) {
			if (i + 1 >= itemList.Count) {
				break;
			}
            if(i%2==0){
                itemList[i].line[0].SetActive(true);
                itemList[i].line[1].SetActive(false);
            } else {
                itemList[i].line[0].SetActive(false);
                itemList[i].line[1].SetActive(true);
            }
		}
	}

	public void jumpToMission()
	{
		if (lastItem != null) {
			missionsParentPos.GetComponent<UICenterOnChild> ().CenterOn (lastItem.transform);
		} else {
			missionsParentPos.GetComponent<UICenterOnChild> ().CenterOn (itemList [itemList.Count - 1].transform);
		}
	}

	public void jumpToMissionFromHide()
	{
		TowerItem jumtoItem = null;
		for (int i = 0; i < itemList.Count; i++) {
            if (itemList[i].mission.sid == FuBenManagerment.Instance.getPassChapter()) {
                if(i==0)jumtoItem=itemList[0];
                else jumtoItem=itemList[i-1];
			}
		}
		if(jumtoItem == null && lastItem == null)
			missionsParentPos.GetComponent<UICenterOnChild> ().CenterOn (itemList [itemList.Count - 1].transform);
		else if(jumtoItem == null && lastItem != null)
			missionsParentPos.GetComponent<UICenterOnChild> ().CenterOn (lastItem.transform);
		else
			missionsParentPos.GetComponent<UICenterOnChild> ().CenterOn (jumtoItem.transform);
        setBgIsCanMove(true);
	}

	void Update ()
	{
		if (isBgCanMove) {
			float a = panel.clipOffset.y;
			//bgPos.localPosition = new Vector3 (0, -a / 10, 0);
            UiManager.Instance.backGround.UpdateTowerChooseBG(a-2);
		}

	}

	/// <summary>
	/// 设置背景是否可以移动
	/// </summary>
	public void setBgIsCanMove (bool b)
	{
		isBgCanMove = b;
	}

	public TowerItem getLastItem ()
	{
		return lastItem;
	}
}
