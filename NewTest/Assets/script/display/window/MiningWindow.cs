using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiningWindow : WindowBase {

    public UICenterOnChild centerOnChild;
    public GameObject[] pageObj;
    public GameObject[] balanceInPage;
    public GameObject leftArrow;
    public GameObject rightArrow;
    private int pageIndex;
    private Timer timer;
    public RoleView[] playerRoleInPage1;
    public RoleView[] playerRoleInPage2;
    public RoleView[] playerRoleInPage3;
    public UILabel accumulativeValue;
    public UILabel speedValue;
    public UILabel remainTime;
    public PvpPowerBar pvpBar;
    public ButtonBase changeTeamButton;
    public ButtonBase miningButton;
    public GameObject info;
    public UITexture[] backgorundsInPage;
    public UISprite[] pageIndexBarIcones;
    public UISprite typeIcon;
    public UISprite typeIcon2;
    public Transform launcher;
    bool isChange = false;
    public UITweener helpPanel;
    public GameObject fightButtonGroup;
    public GameObject fightInfo;
    public GameObject tips;
    public UILabel newEnemyNum;
    bool editTeam;
    public UILabel searchMoney;
    public UILabel searchMoney2;
    public Animation searchAnim;
    //敌人信息
    public UILabel enemyName;
    public UILabel enemyCombat;
    public UILabel enemyServerName;
    public UITexture enemyIcon;
    public UILabel enemyLevel;
    public UISprite pillageResType;
    public UILabel pillageResCount;
    public bool isSearchEnemy = false;  //是否是搜索出来的敌人而不是反击的敌人
    int war_type;
    protected override void begin() {
        base.begin();
        timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        timer.addOnTimer(refreshData);
        timer.start();
        if (isChange) {
            UpdatePageInfo();
            isChange = false;
        }
        //如果回到界面有奖品则刷新界面
        if (MiningManagement.Instance.HaveFightPrizes()) {
            UpdateInfo();
        }
        MaskWindow.UnlockUI();
    }

    protected override void DoEnable() {
        base.DoEnable();
        //如果回到界面有奖品则刷新界面
        if (MiningManagement.Instance.HaveFightPrizes()) {
            UpdateInfo();
        }
    }

    MineralInfo[] minerals = null;

    public void InitPage() {
        centerOnChild.onFinished = PageTurned;
        if (MiningManagement.Instance.NewEnemyNum != 0) {
            tips.SetActive(true);
            newEnemyNum.text = MiningManagement.Instance.NewEnemyNum.ToString();
        }


        searchMoney.text = MiningManagement.Instance.GetSearchMineralConsume();
        searchMoney2.text = MiningManagement.Instance.GetSearchMineralConsume();
        editTeam = false;
        //获取矿坑信息
        minerals = MiningManagement.Instance.GetMinerals();
        //加载队伍信息
        LoadTeam();
        if (minerals[0] != null) {
            pageObj[0].SetActive(true);
            ResourcesManager.Instance.LoadAssetBundleTexture("texture/backGround/" + MiningManagement.Instance.GetMineralBackground(0), backgorundsInPage[0]);
        }
        if (minerals[1] != null) {
            pageObj[1].SetActive(true);
            ResourcesManager.Instance.LoadAssetBundleTexture("texture/backGround/" + MiningManagement.Instance.GetMineralBackground(1), backgorundsInPage[1]);
        }
        OrderPagePostion();
        updatePvp();
        GetStartPageIndex();
        updateButton();
        UpdateInfo();
    }

    public void UpdatePageInfo() {
        editTeam = false;
      
        if (MiningManagement.Instance.NewEnemyNum != 0) {
            tips.SetActive(true);
            newEnemyNum.text = MiningManagement.Instance.NewEnemyNum.ToString();
        }
        searchMoney.text = MiningManagement.Instance.GetSearchMineralConsume();
        searchMoney2.text = MiningManagement.Instance.GetSearchMineralConsume(); 
        //获取矿坑信息
        minerals = MiningManagement.Instance.GetMinerals();
        //加载队伍信息
        LoadTeam();
        //矿坑存在则打开对应页面，否则关闭并且对页面重新排序
        if (minerals[0] != null) {
            pageObj[0].SetActive(true);
            ResourcesManager.Instance.LoadAssetBundleTexture("texture/backGround/" + MiningManagement.Instance.GetMineralBackground(0), backgorundsInPage[0]);
        } else {
            pageObj[0].SetActive(false);
        }
        if (minerals[1] != null) {
            pageObj[1].SetActive(true);
            ResourcesManager.Instance.LoadAssetBundleTexture("texture/backGround/" + MiningManagement.Instance.GetMineralBackground(1), backgorundsInPage[1]);
        } else {
            pageObj[1].SetActive(false);
        }

        if (pageIndex == 2 && searchAnim.transform.parent.gameObject.activeSelf) {
            searchAnim.transform.parent.gameObject.SetActive(false);
            searchAnim.Stop();
            fightButtonGroup.SetActive(true);
            fightInfo.SetActive(true);
            if (playerRoleInPage3 != null) {
                for (int i = 0; i < 5; i++) {
                    playerRoleInPage3[i].gameObject.SetActive(true);
                }
            }
        }
        
        OrderPagePostion();  //重新对页面排序
        updatePvp();  //更新pvpbar
        updateButton();//更新button状态
        UpdateInfo();//跟新页面信息
        centerOnChild.CenterOn(pageObj[pageIndex].transform);
    }




    public void SetShowPage(int index) {
        pageIndex = index;
        if (minerals[0] != null) {
            pageObj[0].SetActive(true);
            ResourcesManager.Instance.LoadAssetBundleTexture("texture/backGround/" + MiningManagement.Instance.GetMineralBackground(0), backgorundsInPage[0]);
        }
        if (minerals[1] != null) {
            pageObj[1].SetActive(true);
            ResourcesManager.Instance.LoadAssetBundleTexture("texture/backGround/" + MiningManagement.Instance.GetMineralBackground(1), backgorundsInPage[1]);
        }

        OrderPagePostion();
        launcher.localPosition = new Vector3(-1 * pageObj[index].transform.localPosition.x, launcher.localPosition.y, launcher.localPosition.z);
        launcher.GetComponent<UIPanel>().clipOffset = new Vector2(pageObj[index].transform.localPosition.x, 0);
    }

    void GetStartPageIndex() {
        for (int i = 0; i <= pageObj.Length - 1; i++) {
            if (pageObj[i] != null && pageObj[i].activeSelf) {
                pageIndex = i;
                break;
            }
        }

        centerOnChild.CenterOn(pageObj[pageIndex].transform);
    }

    void refreshData() {
        updatePvp();
        UpdateInfo();
    }

    public override void buttonEventBase(GameObject gameObj) {
        base.buttonEventBase(gameObj);

        if (gameObj.name == "close") {
            if (pageObj[2].activeSelf && !MiningManagement.Instance.HaveFightPrizes() && isSearchEnemy) {
                MessageWindow.ShowConfirm(LanguageConfigManager.Instance.getLanguage("mining_exit"), (msgHandle) => {
                    if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
                        this.finishWindow();
                    }
                }, true);
            } else if (MiningManagement.Instance.HaveFightPrizes()) {
                UiManager.Instance.createPrizeMessageLintWindow(MiningManagement.Instance.FightPrizes);
                MiningManagement.Instance.ClearFightPrizes();
                balanceInPage[pageIndex].SetActive(false);
                StartCoroutine(Utils.DelayRun(() => {
                    finishWindow();
                }, 1f));
            } else {
                finishWindow();
            }
        }
        if (gameObj.name == "leftArrow") {
            LeftDrag();
        }
        if (gameObj.name == "rightArrow") {
            RightDrag();
        }
        if (gameObj.name == "buttonMining") {
            UiManager.Instance.openDialogWindow<MiningTypeWindow>();
            isChange = true;
        }
        if (gameObj.name == "buttonPillage") {
            //			centerOnChild.onFinished = null;
            if (pageIndex == 2) {
                //隐藏其他元素，显示动画
                fightButtonGroup.SetActive(false);
                fightInfo.SetActive(false);
                HideTeam();
                searchAnim.transform.parent.gameObject.SetActive(true);
                searchAnim.Play();
            }
                war_type = 0;
            isSearchEnemy = true;   
            FPortManager.Instance.getFPort<SearchEnemyFport>().access(ShowEnemyInfo);
        }
        if (gameObj.name == "changeArmyButton") {
            editTeam = true;
            UiManager.Instance.openWindow<TeamEditWindow>(win => {
                if (pageIndex == 2){
                    win.setComeFrom(TeamEditWindow.FROM_LADDERS);
                }else if (ArmyManager.Instance.getArmy(4) == null) {
                    ArmyManager.Instance.InitMiningTeam();
                    ArmyManager.Instance.SaveMiningArmy(() => {
                        win.setShowTeam(4);
                        win.setComeFrom(TeamEditWindow.FROM_MINING, false, MiningManagement.Instance.GetMiningSample(pageIndex).sid);
                        isChange = true;
                    });
                }   else {
                    win.setShowTeam(MiningManagement.Instance.GetMinerals()[pageIndex].armyId);
                    win.setComeFrom(TeamEditWindow.FROM_MINING, false, MiningManagement.Instance.GetMiningSample(pageIndex).sid);
                    isChange = true;
                }

            });
        }

        if (gameObj.name == "balanceButton") {
            if (pageIndex == 0 || pageIndex == 1) {
                FPortManager.Instance.getFPort<MineralBalanceFport>().access(pageIndex, Balanced);
            } else {
                BalanceFightPrize();
            }
        }

        if (gameObj.name == "buttonPk") {
            pvpPoint = 1;//先设置好pvp消耗点
            //if (ArmyManager.Instance.getArmy(4) == null) {
            //    ArmyManager.Instance.InitMiningTeam();
            //    ArmyManager.Instance.SaveMiningArmy(Fight);
            //} else {
                Fight();
            //}
        }

        if (gameObj.name == "buttonAllPk") {
            pvpPoint = 3;//先设置好pvp消耗点
            Fight();
        }

        if (gameObj.name == "enemyButton") {
            isChange = true;
            MiningManagement.Instance.NewEnemyNum = 0;
            tips.SetActive(false);
            newEnemyNum.text = "";
            UiManager.Instance.openDialogWindow<MiningEnemiesWindow>();

        }
        if (gameObj.name == "buttonHelp") {
            helpPanel.Play(true);
        }

        if (gameObj.name == "buttonCloseHelp") {
            helpPanel.Play(false);
        }
    }

    public void SetWarType(int type) {
        war_type = type;
    }

    int pvpPoint;
    void Fight() {
        int fightType = 0;
        if (pvpPoint == 3) {
            fightType = 1;
        }

        if (UserManager.Instance.self.getPvPPoint() < pvpPoint) {
            UiManager.Instance.openDialogWindow<PvpUseWindow>((win) => {
                win.initInfo(fightType==1?2:1, null);
            });
            return;
        }
        MaskWindow.instance.setServerReportWait(true);

        GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleNoSwitchWindow;

        FPortManager.Instance.getFPort<MineralFightFport>().access(fightType, war_type, (b) => {
            if (b) {
                centerOnChild.onFinished = null;//因为界面要滑动，必须清除事件绑定
                UserManager.Instance.self.costPoint(pvpPoint, MissionEventCostType.COST_PVP);
                //刷新仇人列表
                FPortManager.Instance.getFPort<GetMineralEnemiesFport>().access(null);
            } else {
                MaskWindow.instance.setServerReportWait(false);
                //未能进入战斗，回到搜索界面

                StartCoroutine(Utils.DelayRun(() => {
                    RightDrag();
                    pageObj[2].SetActive(false);
                    UpdatePageInfo();
                },0.5f));
              

            }
            
            
        });
    }

    public void BalanceFightPrize() {
        if (MiningManagement.Instance.HaveFightPrizes()) {
            UiManager.Instance.createPrizeMessageLintWindow(MiningManagement.Instance.FightPrizes);
            MiningManagement.Instance.ClearFightPrizes();
            balanceInPage[pageIndex].SetActive(false);
            StartCoroutine(Utils.DelayRun(() => {
                RightDrag();
                pageObj[2].SetActive(false);
                UpdatePageInfo();
            }, 1.5f));
        }
    }
    public void ShowEnemyInfo(bool b) {
       
        
        searchAnim.transform.parent.gameObject.SetActive(false);
        searchAnim.Stop();
        searchMoney.text = MiningManagement.Instance.GetSearchMineralConsume();
        searchMoney2.text = MiningManagement.Instance.GetSearchMineralConsume(); 
        centerOnChild.onFinished = PageTurned;
        if (!b) {
            if (pageIndex == 2) {
                fightButtonGroup.SetActive(true);
                fightInfo.SetActive(true);
            }

            if (playerRoleInPage3 != null) {
                for (int i = 0; i < 5; i++) {
                    playerRoleInPage3[i].gameObject.SetActive(true);
                }
            }
            return;
        }


        
        pageIndex = 2;
        pageObj[2].gameObject.SetActive(true);
        fightButtonGroup.SetActive(true);
        fightInfo.SetActive(true);
        if (playerRoleInPage3 != null) {
            for (int i = 0; i < 5; i++) {
                playerRoleInPage3[i].gameObject.SetActive(true);
            }
        }
        EnemyMineralInfo enemyInfo = MiningManagement.Instance.GetEnemyMineral();
        ResourcesManager.Instance.LoadAssetBundleTexture(UserManager.Instance.getIconPath(enemyInfo.HeadIconId), enemyIcon);
        enemyLevel.text = enemyInfo.playerLevel;
        enemyName.text = enemyInfo.playerName;
        enemyServerName.text = enemyInfo.serverName;
        enemyCombat.text = enemyInfo.combat.ToString();
        pillageResCount.text = enemyInfo.count.ToString();
        ResourcesManager.Instance.LoadAssetBundleTexture("texture/backGround/" + MiningManagement.Instance.GetMineralBackgroundBySid(enemyInfo.sid), backgorundsInPage[2]);
        int type = MiningManagement.Instance.GetMiningSampleBySid(enemyInfo.sid).type;
        if (type == (int)MiningTypePage.MiningGold) {
            pillageResType.spriteName = "gold4";
        } else {
            pillageResType.spriteName = "gem1";
        }
        HideTeam();

        //		SetShowPage (pageIndex);
        UpdatePageInfo();

    }

    void ShowSearchAnimation() {
        HideTeam();
        fightButtonGroup.SetActive(false);
        fightInfo.SetActive(false);
    }
    void Balanced() {

        MiningManagement.Instance.ClearMineral(pageIndex);
        balanceInPage[pageIndex].SetActive(false);
        StartCoroutine(Utils.DelayRun(() => {
            RightDrag();
            UpdatePageInfo();
        }, 1.5f));

    }

    public override void OnOverAnimComplete() {
        base.OnOverAnimComplete();
        if (editTeam)
            centerOnChild.onFinished = null;
        //HideTeam ();
    }

    public void LeftDrag() {
        centerOnChild.onFinished = PageTurned;

        //如果有战斗奖励就领走
        if (pageObj[2].activeSelf && MiningManagement.Instance.HaveFightPrizes()) {
            UiManager.Instance.createPrizeMessageLintWindow(MiningManagement.Instance.FightPrizes);
            MiningManagement.Instance.ClearFightPrizes();
            balanceInPage[pageIndex].SetActive(false);
            pageObj[2].SetActive(false);
        }

        OrderPagePostion();
        Transform page = null;
        for (int i = pageIndex - 1; i >= 0; i--) {
            if (pageObj[i] != null && pageObj[i].activeSelf) {
                page = pageObj[i].transform;
                pageIndex = i;
                break;
            }
        }
        if (page == null) {
            MaskWindow.UnlockUI();
            return;
        }
        
        info.SetActive(false);
        centerOnChild.CenterOn(page);
        updateButton();
    }

    public void RightDrag() {
        centerOnChild.onFinished = PageTurned;

        //如果有战斗奖励就领走
        if (pageObj[2].activeSelf && MiningManagement.Instance.HaveFightPrizes()) {
            UiManager.Instance.createPrizeMessageLintWindow(MiningManagement.Instance.FightPrizes);
            MiningManagement.Instance.ClearFightPrizes();
            balanceInPage[pageIndex].SetActive(false);
            pageObj[2].SetActive(false);
        }

        OrderPagePostion();
        Transform page = null;
        for (int i = pageIndex + 1; i <= pageObj.Length - 1; i++) {
            if (pageObj[i] != null && pageObj[i].activeSelf) {
                page = pageObj[i].transform;
                pageIndex = i;
                break;
            }
        }
        if (page == null) {
            MaskWindow.UnlockUI();
            return;
        }

       
        info.SetActive(false);
        centerOnChild.CenterOn(page);
        updateButton();
    }

    private void updateButton() {

        bool showRightArrow = false;
        bool showLeftArrow = false;
        //检查左边是否还有可以显示的页面
        for (int i = pageIndex - 1; i >= 0; i--) {
            if (pageObj[i] != null && pageObj[i].activeSelf) {
                showLeftArrow = true;
                break;
            }
        }

        for (int i = pageIndex + 1; i <= pageObj.Length - 1; i++) {
            if (pageObj[i] != null && pageObj[i].activeSelf) {
                showRightArrow = true;
                break;
            }
        }

        if (showLeftArrow) {
            leftArrow.SetActive(true);
        } else {
            leftArrow.SetActive(false);
        }

        if (showRightArrow) {
            rightArrow.SetActive(true);
        } else {
            rightArrow.SetActive(false);
        }

        if (pageIndex == 3) {
            changeTeamButton.gameObject.SetActive(false);
        } else if ((pageIndex == 0 || pageIndex == 1) && MiningManagement.Instance.GetRemainTime(pageIndex) == 0) {
            changeTeamButton.gameObject.SetActive(false);
        } else {
            changeTeamButton.gameObject.SetActive(true);
        }

        if (pageIndex == 2 || pageIndex == 3) {
            info.SetActive(false);
        } else {
           
        }

        if (MiningManagement.Instance.GetAvailableLocal() == -1) {
            miningButton.disableButton(true);
        } else {
            miningButton.disableButton(false);
        }
        //初始化图标
        for (int i = 0; i < pageIndexBarIcones.Length; i++) {
            //判断矿坑是否存在
            if (i < minerals.Length && minerals[i] != null) {
                //获取矿藏类型
                int type = MiningManagement.Instance.GetMiningSample(i).type;
                if (type == (int)MiningTypePage.MiningGold) {
                    if (pageIndex == i) {
                        typeIcon.spriteName = "gold4";
                        typeIcon2.spriteName = "gold4";
                        pageIndexBarIcones[i].spriteName = "_icon_gold_1";
                    } else {
                        pageIndexBarIcones[i].spriteName = "_icon_gold_2";
                    }
                } else {
                    if (pageIndex == i) {
                        typeIcon.spriteName = "gem1";
                        typeIcon2.spriteName = "gem1";
                        pageIndexBarIcones[i].spriteName = "_0002_gem_1";
                    } else  {
                        pageIndexBarIcones[i].spriteName = "_0002_gem_2";
                    }
                }

            } else {

                //如果没有开矿，则加载默认图标
                if (i == 0 || i == 1) {
                    pageIndexBarIcones[i].spriteName = "_0000_mining_2";
                }
                if (pageIndex == 2) {
                    pageIndexBarIcones[pageIndex].spriteName = "_0001_search_1";
                } else if (i == 2) {
                    pageIndexBarIcones[i].spriteName = "_0001_search_2";
                }
                if (pageIndex == 3) {
                    pageIndexBarIcones[pageIndex].spriteName = "_0000_mining_1";
                } else if (i == 3) {
                    pageIndexBarIcones[i].spriteName = "_0000_mining_2";
                }
            }
        }

    }

    private void OrderPagePostion() {
        float posX = pageObj[pageObj.Length - 1].transform.localPosition.x;
        for (int i = pageObj.Length - 1; i >= 0; i--) {
            if (pageObj[i] != null && pageObj[i].activeSelf) {
                pageObj[i].transform.localPosition = new Vector3(posX, pageObj[i].transform.localPosition.y, pageObj[i].transform.localPosition.z);
                posX -= 640;
            }
        }
    }

    private void PageTurned() {
        //如果页面是矿坑页面则播放卡片动画
        HideTeam();
        if (pageIndex == 0) {
            if (MiningManagement.Instance.GetRemainTime(pageIndex) > 0) {
                PlayCardAnimation(playerRoleInPage1);
            } else {
                MaskWindow.UnlockUI();
            }
        }

        if (pageIndex == 1) {
            if (MiningManagement.Instance.GetRemainTime(pageIndex) > 0) {
                PlayCardAnimation(playerRoleInPage2);
            } else {
                MaskWindow.UnlockUI();
            }
        }

        if (pageIndex == 2) {
            PlayCardAnimation(playerRoleInPage3);
        }

        if (pageIndex == 3) {
            MaskWindow.UnlockUI();
        }

    }

    public void LoadTeam() {
        //加载矿坑队员

        for (int m = 0; m < minerals.Length; m++) {
            RoleView[] tmp = null;
            string[] players = null;
            if (minerals[m] == null) {
                continue;
            }
            if (minerals[m].localId == 0) {
                tmp = playerRoleInPage1;
                players = ArmyManager.Instance.getArmy(minerals[m].armyId).players;
            } else if (minerals[m].localId == 1) {
                tmp = playerRoleInPage2;
                players = ArmyManager.Instance.getArmy(minerals[m].armyId).players;
            }

            for (int i = 0; i < players.Length; i++) {
                tmp[i].hideInBattle = true;
                Card c = null;
                if (players[i] != "0")
                    c = StorageManagerment.Instance.getRole(players[i]);
                if (c != null) {
                    tmp[i].init(c, this, null);
                } else {
                    tmp[i].icon.mainTexture = null;
                    tmp[i].card = null;
                    tmp[i].gameObject.SetActive(false);
                }
            }
        }

        //加载敌人队伍
        if (MiningManagement.Instance.HaveFightPrizes())  //如果有奖品则不加载队伍
            return;

        if (MiningManagement.Instance.GetEnemyMineral() != null) {
            RoleView[] tmp = null;
            string[] players = null;
            string[] eveLv = null;
            tmp = playerRoleInPage3;
            players = MiningManagement.Instance.GetEnemyMineral().roles;
            eveLv = MiningManagement.Instance.GetEnemyMineral().evoLv;
            for (int i = 0; i < players.Length; i++) {
                tmp[i].hideInBattle = true;
                Card c = null;
                if (players[i] != "0") {
                    c = CardManagerment.Instance.createCard(StringKit.toInt(players[i]));
                    c = CardManagerment.Instance.createCardByEvoLevel(c,StringKit.toInt(eveLv[i]));
                }
                if (c != null) {
                    tmp[i].init(c, this, null);
                } else {
                    tmp[i].icon.mainTexture = null;
                    tmp[i].icon.gameObject.SetActive(false);
                    tmp[i].card = null;
                    tmp[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void PlayCardAnimation(RoleView[] team) {
        TweenPosition tp;
        for (int i = 0; i < 5; i++) {
            team[i].transform.localPosition += new Vector3(0, -500, 0);
            team[i].gameObject.SetActive(true);
            tp = TweenPosition.Begin(team[i].gameObject, 0.1f, team[i].transform.localPosition + new Vector3(0, 500, 0));
            tp.delay = i * 0.1f;
        }

        StartCoroutine(Utils.DelayRun(() => {
            MaskWindow.UnlockUI();
        }, 0.8f));
    }

    void HideTeam() {
        if (playerRoleInPage1 != null) {
            for (int i = 0; i < 5; i++) {
                playerRoleInPage1[i].gameObject.SetActive(false);
            }
        }

        if (playerRoleInPage2 != null) {
            for (int i = 0; i < 5; i++) {
                playerRoleInPage2[i].gameObject.SetActive(false);
            }
        }

        if (playerRoleInPage3 != null) {
            for (int i = 0; i < 5; i++) {
                playerRoleInPage3[i].gameObject.SetActive(false);
            }
        }
    }

    void updatePvp() {
        pvpBar.updateValue(UserManager.Instance.self.getPvPPoint(), UserManager.Instance.self.getPvPPointMax());
    }

    public override void DoDisable() {
        base.DoDisable();

        if (timer != null) {
            timer.stop();
            timer = null;
        }
    }

    void UpdateInfo() {
        if ((pageIndex == 0 && MiningManagement.Instance.minerals[0] != null) || (pageIndex == 1 && MiningManagement.Instance.minerals[1] != null)) {
            if (MiningManagement.Instance.GetRemainTime(pageIndex) > 0) {
                balanceInPage[pageIndex].SetActive(false);
                info.SetActive(true);
                remainTime.text = TimeKit.timeTransformDHMS(MiningManagement.Instance.GetRemainTime(pageIndex));
                accumulativeValue.text = MiningManagement.Instance.GetBalance(pageIndex).ToString();
                speedValue.text = LanguageConfigManager.Instance.getLanguage("s0043l3", ((int)(MiningManagement.Instance.GetSpeed(pageIndex) * 3600)).ToString() + "/");
            } else {
                info.SetActive(false);
                ShowBalance();
                balanceInPage[pageIndex].SetActive(true);
                HideTeam();
            }
        } else if (pageIndex == 2) {
            if (MiningManagement.Instance.HaveFightPrizes()) {
                ShowBalance();
                balanceInPage[pageIndex].SetActive(true);
                HideTeam();
                fightButtonGroup.SetActive(false);
                fightInfo.SetActive(false);
            }
        }

    }



    void ShowBalance() {
        if (balanceInPage[pageIndex].activeSelf)
            return;

        int type = 0;
        if (pageIndex == 0 || pageIndex == 1)
            type = MiningManagement.Instance.GetMiningSample(pageIndex).type;
        else if (pageIndex == 2)
            type = MiningManagement.Instance.FightPrizes[0].type;
        string icon1, icon2;
        if (type == (int)MiningTypePage.MiningGold) {
            icon1 = "gold1";
            icon2 = "gold2";
        } else {
            icon1 = "gem1";
            icon2 = "gem2";
        }

        UISprite[] ut = balanceInPage[pageIndex].GetComponent<MiningBalance>().balanceIcon;
        for (int i = 0; i < ut.Length; i++) {
            if (Random.Range(0, 10) < 7) {
                ut[i].spriteName = icon1;
            } else {
                ut[i].spriteName = icon2;
            }
        }
    }

    //断线重新连接
    public override void OnNetResume() {
        base.OnNetResume();
        UpdatePageInfo();
    }
}
