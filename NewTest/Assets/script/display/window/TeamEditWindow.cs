using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 队伍编辑窗口
 * */ 
public class  TeamEditWindow : WindowBase
{
	//public static string saveTeam;
	public const int FOURLIMITLV = 8;//四人阵开放等级
	public const int FIVELIMITLV = 19;//五人阵开放等级
	public const int TIBULVLIMIT=21;//替补开放等级
	public const int SUBLV = 13;//替补开放等级
	public const int TEAMTWOLV = 999;//队伍2开放等级
	public const int TEAMTHREELV = 5;//竞技场队伍开放等级
	public const int FROM_MAINMENU = 1;//主界面
	public const int FROM_PVP = 2;//PVP
	public const int FROM_PVE = 3;//冒险
	public const int FROM_LADDERS= 4;//天梯
	public const int FROM_MINING = 5;//采矿
//    public const int FROM_TOWER = 6;//爬塔
	public const int FROM_GUILD = 6;//公会战

	[HideInInspector]
	public RoleView chooseButton;//选中要换的card按钮;
	[HideInInspector]
	public int comeFrom;
	int showTeamId;


	public SampleDynamicContent content;
	/** 卡片形象拖拽缓存用 */
	public UITexture dragCardObj;
	/** 战斗力 */
	public UILabel combatLabel;
	/** 战斗力背景 */
    public UISprite combatBg;
	/** 主力空穴 */
	public GameObject[] teamForEmtpy;
	/** 替补空穴 */
	public GameObject[] teamSubEmtpy;
	/** 主力上阵卡片 */
	public RoleView[] teamForRole;
	/** 替补上阵卡片 */
	public RoleView[] teamSubRole;
	/** 召唤兽仓库实例 */
	public Card beast;
	/** 召唤兽图片 */
	public UITexture beastImage;
	/** 没有召唤兽添加按钮 */
	public GameObject noBeastBg;
	/** 切换队伍按钮 */
	public ButtonBase buttonTeamChoose;
	/** 复活按钮 */
	public ButtonBase buttonRevive;
	/** 左箭头 */
	public GameObject leftArrow;
	/** 右箭头 */
	public GameObject rightArrow;
	/** 保存队伍按钮 */
	public ButtonBase saveButton;
	/** 四人阵限制 01文字说明 23加号 */
	public GameObject[] fourLimit;
	/** 五人阵限制 01文字说明 23加号 */
	public GameObject[] fiveLimit;
	/** 替补席位开放等级 */
	public UILabel[] openLvDec;
	/** 加号的状态 */
	public GameObject[] openAnmi;
	/** 加号 */
	public GameObject[] addFlag;
	private int formationLength;
    public UILabel teamName;
    public UILabel miningDesc;
    public UILabel miningSpeedValue;
    public UISprite miningType;
    public UILabel miningSpeed;
	public GameObject playerRole;
	public GameObject subRole;
    public ButtonBase beatButton;
	Army savingArmy;
	List<int> idss = new List<int>();
	bool isChange = false;//是否替换过卡片
	bool isStartmining = false;//是否开矿
	int miningSid; //矿藏sid
	int localId;
	public ButtonBase revive;

	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
	}

	public void updateActive (GameObject obj)
	{
		int pageNUm = StringKit.toInt (obj.name);
		ButtonFormationChoose button = obj.GetComponent<ButtonFormationChoose> ();
		int teamFormationId = FormationManagerment.Instance.getPlayerFormation () [pageNUm - 1];
		if(button.points!=null){
			FormationSample sample = FormationSampleManager.Instance.getFormationSampleBySid (teamFormationId);
			for(int i=0;i<button.points.Length;i++){
				if(sample.formationMap.Contains(button.points[i].gameObject.name))
					button.points[i].gameObject.SetActive(true);
				else
					button.points[i].gameObject.SetActive(false);
			}
		}
		button.teamID = teamFormationId;
	}
    public void initInfo(List<int> ids)
    {
        idss = ids;
    }
	void changeArrow (int index)
	{
		if (index == 0) {
			if (FormationManagerment.Instance.getPlayerFormation ().Count > 1)
				rightArrow.SetActive (true);
			else
				rightArrow.SetActive (false);
			leftArrow.SetActive (false);
		} else if (index == FormationManagerment.Instance.getPlayerFormation ().Count-1) {
			rightArrow.SetActive (false);
			leftArrow.SetActive (true);
		} else {
			rightArrow.SetActive (true);
			leftArrow.SetActive (true);
		}
	}

	void onCenterFormationPage (GameObject obj)
	{
		int pageNUm = StringKit.toInt (obj.name);
		ButtonFormationChoose button = obj.GetComponent<ButtonFormationChoose> ();
		changeArrow (pageNUm - 1);
		setFormation (button.teamID);
	}

	public override void OnAwake ()
	{
		base.OnAwake ();
		UiManager.Instance.teamEditWindow = this;
		EventDelegate.Add (onDestroy, ArmyManager.Instance.cleanAllEditArmy);

	}

	protected override void begin ()
	{ 
		base.begin ();
		ArmyManager.Instance.teamEditInMissonWin = false;
       // MaskWindow.UnlockUI();
		ArmyManager.Instance.checkFormation ();
		if (isChange) {
			if(comeFrom == FROM_MINING){
				ArmyManager.Instance.SaveMiningArmy (()=>{
					initWin();
					isChange = false;
				});
			}
			else if(comeFrom ==FROM_GUILD){
				ArmyManager.Instance.saveGuildFightArmy(()=>{
					initWin();
					isChange = false;
				});

			}
			else{
				ArmyManager.Instance.saveArmy (()=>{
					initWin();
					isChange = false;
				});
			}
		} else {
			initWin();
		}
        StartCoroutine(Utils.DelayRun(() => {
            MaskWindow.UnlockUI();
        }, 1f));
	}



	void initWin()
	{
		if (ArmyManager.Instance.ActiveEditArmy == null) {
			//复制用于编辑阵形
			ArmyManager.Instance.EditArmy1 = ArmyManager.Instance.DeepClone (ArmyManager.Instance.getArmy (1));
			ArmyManager.Instance.EditArmy2 = ArmyManager.Instance.DeepClone (ArmyManager.Instance.getArmy (2));	
			ArmyManager.Instance.EditArmy3 = ArmyManager.Instance.DeepClone (ArmyManager.Instance.getArmy (3));	 
			ArmyManager.Instance.EditArmy4 = ArmyManager.Instance.DeepClone (ArmyManager.Instance.getArmy (4));	 
			ArmyManager.Instance.EditArmy5 = ArmyManager.Instance.DeepClone (ArmyManager.Instance.getArmy (5));
			ArmyManager.Instance.EditArmy6 = ArmyManager.Instance.DeepClone (ArmyManager.Instance.getArmy (6));

			if (ArmyManager.Instance.getActiveArmy () == ArmyManager.Instance.getArmy (1)) {
				buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0066");
				ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy1;
			} else if (ArmyManager.Instance.getActiveArmy () == ArmyManager.Instance.getArmy (2)) {
				buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0067");
				ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy2;
			} else if (ArmyManager.Instance.getActiveArmy () == ArmyManager.Instance.getArmy (3)) {
				buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0068");
				ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy3;
			}else if(ArmyManager.Instance.getActiveArmy () == ArmyManager.Instance.getArmy (4)){
				buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("mining_team1");
				ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy4;
			}else if(ArmyManager.Instance.getActiveArmy () == ArmyManager.Instance.getArmy (5)){
					buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("mining_team2");
					ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy5;
			}else if(ArmyManager.Instance.getActiveArmy () == ArmyManager.Instance.getArmy (6)){
				buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_101");
				ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy6;
			}
			else{
			buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0066");
				ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy1;
			} 
			formationLength = ArmyManager.Instance.ActiveEditArmy.getLength ();
			//整型容器初始化
			content.maxCount = FormationManagerment.Instance.getPlayerFormation ().Count;
			content.onCenterItem = onCenterFormationPage;
			content.callbackUpdateEach = updateActive;
			content.fatherWindow = this;
			content.init ();

			switch(comeFrom){
			case FROM_PVP:
			case FROM_LADDERS:
				teamSwitch (3);
				setTitle (LanguageConfigManager.Instance.getLanguage ("s0068") + LanguageConfigManager.Instance.getLanguage ("teamEdit10"));
				break;
//            case FROM_TOWER:
//                teamSwitch(3);
//                setTitle(LanguageConfigManager.Instance.getLanguage("s0068l0") + LanguageConfigManager.Instance.getLanguage("teamEdit10"));
//                break;
			case FROM_PVE:
				teamSwitch (1);
				setTitle (LanguageConfigManager.Instance.getLanguage ("s0066") + LanguageConfigManager.Instance.getLanguage ("teamEdit10"));
				break;
			case FROM_MINING:
                miningDesc.gameObject.SetActive(true);
                ShowMiningSpeedInfo();
				teamSwitch (showTeamId);
				break;
			case FROM_GUILD:
				teamSwitch(6);
				setTitle(LanguageConfigManager.Instance.getLanguage ("GuildArea_101") + LanguageConfigManager.Instance.getLanguage ("teamEdit10"));
				break;
			default:
				teamSwitch (1);
				break;
			}
//			if (comeFrom == FROM_PVP || comeFrom == FROM_LADDERS) {
//				teamSwitch (3);
//				setTitle (LanguageConfigManager.Instance.getLanguage ("s0068") + LanguageConfigManager.Instance.getLanguage ("teamEdit10"));
//			}
//			else {
//				if (comeFrom == FROM_PVE) {
//					setTitle (LanguageConfigManager.Instance.getLanguage ("s0066") + LanguageConfigManager.Instance.getLanguage ("teamEdit10"));
//				}
//				teamSwitch (1);
//			}
		}
		if (!isAwakeformHide) {
			PlayCardAnimation ();
		} else {
			reLoadTeam ();
            //StartCoroutine (Utils.DelayRun (()=>{
            //    MaskWindow.UnlockUI();
            //},0.3f));
		}
		if (chooseButton != null) {
			chooseButton.updateCard ();
		}

		GuideManager.Instance.guideEvent ();
		
		//这里控制新手引导期间，不可以拖拽卡片，避免造成卡死
		if (GuideManager.Instance.isEqualStep (GuideGlobal.SPECIALSID3) || GuideManager.Instance.isEqualStep (GuideGlobal.SPECIALSID33)
		    || GuideManager.Instance.isEqualStep (GuideGlobal.SPECIALSID34) || GuideManager.Instance.isEqualStep (GuideGlobal.SPECIALSID35)) {
			buttonTeamChoose.GetComponent<BoxCollider> ().enabled = false;
			saveButton.GetComponent<BoxCollider> ().enabled = false;
			for (int i = 0; i < teamForRole.Length; i++) {
				teamForRole [i].GetComponent<TeamEditDragDropItem> ().enabled = false;
				teamForRole [i].GetComponent<TeamEditDragDropItem> ().cloneOnDrag = true;
			}
			for (int i = 0; i < teamSubRole.Length; i++) {
				teamSubRole [i].GetComponent<TeamEditDragDropItem> ().enabled = false;
				teamSubRole [i].GetComponent<TeamEditDragDropItem> ().cloneOnDrag = true;
			}
		} else {
			buttonTeamChoose.GetComponent<BoxCollider> ().enabled = true;
//			saveButton.GetComponent<BoxCollider> ().enabled = true;
			if(ArmyManager.Instance.CheckActiveEditArrayIsEmpty()){
				saveButton.disableButton(true);
			}else{
				saveButton.disableButton(false);
			}
			for (int i = 0; i < teamForRole.Length; i++) {
				teamForRole [i].GetComponent<TeamEditDragDropItem> ().enabled = true;
				teamForRole [i].GetComponent<TeamEditDragDropItem> ().cloneOnDrag = false;
			}
			for (int i = 0; i < teamSubRole.Length; i++) {
				teamSubRole [i].GetComponent<TeamEditDragDropItem> ().enabled = true;
				teamSubRole [i].GetComponent<TeamEditDragDropItem> ().cloneOnDrag = false;
			}
		}
		openEmptyForLevel ();
		openEmptyForLevelNew();
		GuideManager.Instance.doFriendlyGuideEvent ();
	}
    void ShowMiningSpeedInfo() {
        if (miningSid != 0) {
            miningSpeed.gameObject.SetActive(true);
            MiningSample ms = MiningManagement.Instance.GetMiningSampleBySid(this.miningSid);
            if (ms.type == (int)MiningTypePage.MiningGold) {
                miningType.spriteName = "gold4";
                float speed = ms.outputRate + ArmyManager.Instance.ActiveEditArmy.getAllCombat() / 300000f;
				speed = speed > (float)CommandConfigManager.Instance.getMoneySpeedOfArean() ? (float)CommandConfigManager.Instance.getMoneySpeedOfArean() : speed;
                miningSpeedValue.text = LanguageConfigManager.Instance.getLanguage("s0043l3", ((int)(speed * 3600)).ToString() + "/");
            } else {
                miningType.spriteName = "gem1";
                miningSpeedValue.text = LanguageConfigManager.Instance.getLanguage("s0043l3", ((int)(ms.outputRate * 3600)).ToString() + "/");
            }
        }
    }
	void openEmptyForLevelNew () {
		TeamEmtpyInfoFPort fport = FPortManager.Instance.getFPort<TeamEmtpyInfoFPort> ();
		fport.access (openEmptyForLevelNewBack);
	}

	void openEmptyForLevelNewBack (List<int> ids) {
		idss = ids;
		int[] openLv = TeamUnluckManager.Instance.getNeedLV ();
		for (int i=0; i<teamForEmtpy.Length; i++) {
			if (UserManager.Instance.self.getUserLevel () < openLv [i] && !idss.Contains (i + 1)) {
				openLvDec [i].gameObject.SetActive (true);
				openAnmi [i].SetActive (false);
				addFlag [i].SetActive (false);
				openLvDec [i].text = LanguageConfigManager.Instance.getLanguage ("teamEdit07", openLv [i].ToString ());
				teamSubEmtpy [i].GetComponent<BoxCollider> ().enabled = false;

			}
			else {
				openLvDec [i].gameObject.SetActive (false);
				teamSubEmtpy [i].GetComponent<BoxCollider> ().enabled = true;
				if (idss.Contains (i + 1)) {
					openAnmi [i].SetActive (false);
					addFlag [i].SetActive (true);
				}
				else {
					addFlag [i].SetActive (false);
					openAnmi [i].SetActive (true);
				}
			}
		}
	}

	void openEmptyForLevel ()
	{
		//这里开始控制阵型等级开放提示
		if (UserManager.Instance.self.getUserLevel () < FOURLIMITLV) {
			fourLimit [0].GetComponent<UILabel> ().text = LanguageConfigManager.Instance.getLanguage ("teamEdit07", FOURLIMITLV.ToString ());
			fourLimit [2].SetActive (false);
			teamForEmtpy [0].GetComponent<BoxCollider> ().enabled = false;
		} else {
			fourLimit [0].GetComponent<UILabel> ().text = "";
			fourLimit [2].SetActive (true);
			teamForEmtpy [0].GetComponent<BoxCollider> ().enabled = true;
		}
		if (UserManager.Instance.self.getUserLevel () < FIVELIMITLV) {
			fiveLimit [0].GetComponent<UILabel> ().text = LanguageConfigManager.Instance.getLanguage ("teamEdit07", FIVELIMITLV.ToString ());
			fiveLimit [2].SetActive (false);
			teamForEmtpy [4].GetComponent<BoxCollider> ().enabled = false;
		} else {
			fiveLimit [0].GetComponent<UILabel> ().text = "";
			fiveLimit [2].SetActive (true);
			teamForEmtpy [4].GetComponent<BoxCollider> ().enabled = true;
		}
	}

	public void setShowTeam(int teamId){
		showTeamId = teamId;
	}


	public void setComeFrom (int cfrom){
		setComeFrom(cfrom,false,0);
	}


	public void setComeFrom (int cfrom,bool isStartMining,int sid)
	{
		comeFrom = cfrom;
		//除非是从主界面来的，否则都不给切换队伍
		if (comeFrom == FROM_MAINMENU) {
			buttonTeamChoose.gameObject.SetActive (true);
			saveButton.transform.localPosition = new Vector3 (183f, saveButton.transform.localPosition.y, saveButton.transform.localPosition.z);
		}else if(comeFrom == FROM_MINING){
				buttonTeamChoose.gameObject.SetActive (true);
				playerRole.transform.localPosition = new Vector3(0f,-70f,0f);
				subRole.gameObject.SetActive(false);
				this.isStartmining = isStartMining;
				this.miningSid = sid;
				this.localId = MiningManagement.Instance.GetAvailableLocal();
				if(isStartMining)
					saveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("mining_go");
			}
		else if(comeFrom == FROM_GUILD)
		{
			buttonTeamChoose.gameObject.SetActive(false);
			saveButton.gameObject.SetActive(false);
		}
		else {
			buttonTeamChoose.gameObject.SetActive (false);
			saveButton.transform.localPosition = new Vector3 (0f, saveButton.transform.localPosition.y, saveButton.transform.localPosition.z);
		}

	}
	 
	private void saveArmy ()
	{
		if (!ArmyManager.Instance.saveArmy (closeWindow)) {
			closeWindow ();
		}
	}
	private void saveGuildFightArmy()
	{
		if (!ArmyManager.Instance.saveGuildFightArmy (closeWindow)) {
			closeWindow ();
		}
	}

	private void SaveMiningArmy(){

		if(ArmyManager.Instance.CheckActiveEditArrayIsEmpty()){
			closeWindow();
		}else if(isStartmining){
			if (!ArmyManager.Instance.SaveMiningArmy (StartMining)) {
				StartMining ();
			}
		}else{
			if (!ArmyManager.Instance.SaveMiningArmy (closeWindow)) {
				closeWindow ();
			}
		}



	}

	private void StartMining(){
		FPortManager.Instance.getFPort<StartMiningFPort>().access(miningSid,ArmyManager.Instance.ActiveEditArmy.armyid,localId,SwitchMiningWindow);
	}

	private void SwitchMiningWindow(){
		ArmyManager.Instance.getArmy(ArmyManager.Instance.ActiveEditArmy.armyid ).state = 1;
		UiManager.Instance.getWindow<MiningWindow>().SetShowPage(localId);
		closeWindow();
	}
	private void closeWindow ()
	{
		ArmyManager.Instance.cleanAllEditArmy ();
		finishWindow ();
	}
	
	public void updateChooseButton (Card _card)
	{
		if (chooseButton == null || (chooseButton.card != null && chooseButton.card.uid == _card.uid)) {
			return;
		}

		isChange = true;

		if (IsAlternateWidthItem (chooseButton.gameObject))
			ArmyManager.Instance.ActiveEditArmy.alternate [getIndexWidthItem (chooseButton.gameObject)] = _card.uid;
		else
			ArmyManager.Instance.ActiveEditArmy.players [getIndexWidthItem (chooseButton.gameObject)] = _card.uid;

		CallBack method = () => {
			chooseButton.gameObject.SetActive (true);
			chooseButton.init (_card, null, null);
			chooseButton = null; 
			rushCombat ();
			GuideManager.Instance.guideEvent ();
		};

		//是否可以进行交换装备星魂
		bool isSwap = false;

		isSwap = chooseButton.card != null && chooseButton.card.getEquips () != null && chooseButton.card.getEquips ().Length > 0;
		if (!isSwap) {
			isSwap = chooseButton.card != null && chooseButton.card.getStarSoulByAll () != null;
		}

		if (isSwap) {
			SwapPropsWindow.Show (chooseButton.card,_card,method);
		} else {
			method ();
		}
	}
	//roleview 或者 add 点击处理
		public void OnItemClick () {
		//8003000上阵卡片1 8006000上阵卡片2 12003000选主角献祭 129004000进化  112003000献祭  123003000穿装备 135008000四人阵上阵
		if (GuideManager.Instance.isEqualStep (8003000) || GuideManager.Instance.isEqualStep (8006000) || GuideManager.Instance.isEqualStep (12003000)
			|| GuideManager.Instance.isEqualStep (129004000) || GuideManager.Instance.isEqualStep (112003000) || GuideManager.Instance.isEqualStep (123003000)
		    || GuideManager.Instance.isEqualStep (135008000)) {
			GuideManager.Instance.doGuide ();
		        if (GuideManager.Instance.isEqualStep(135009000))
		        {
		            ArrayList al = StorageManagerment.Instance.getRoleBySid(11218);
                    if(al==null||al.Count<1)GuideManager.Instance.doGuide();
		        }
		}
		MaskWindow.LockUI ();
		GameObject obj = UICamera.lastHit.collider.gameObject;
		chooseButton = getDstRoleViewWidthItem (obj);
		if (chooseButton.gameObject.activeInHierarchy) {
			List<Card> list = new List<Card> ();
			foreach (RoleView rvw in teamForRole) {
				if (rvw.gameObject.activeInHierarchy && rvw.card != null) {
					list.Add (rvw.card);
				}
			}
			foreach (RoleView rvw in teamSubRole) {
				if (rvw.gameObject.activeInHierarchy && rvw.card != null) {
					list.Add (rvw.card);
				}
			}


            if (ArmyManager.Instance.ActiveEditArmy.armyid != 4 && ArmyManager.Instance.ActiveEditArmy.armyid != 5) {
                CardBookWindow.Show(list, chooseButton.card, CardBookWindow.CARDCHANGE, () => {
                    if (!IsDestoryed) {
                        chooseButton.updateCard();
                        restoreWindow();
                    }
                });
            } else {
                CardBookWindow.Show(list, chooseButton.card, CardBookWindow.MINING, () => {
                    if (!IsDestoryed) {
                        chooseButton.updateCard();
                        restoreWindow();
                    }
                });
            
            }
			
		}
		else {
			if(obj.transform.parent.gameObject.name == "suber"&&obj.name.StartsWith("team_edit_item_temp")){
				string[] strs = obj.name.Split ('_');;
				int index=StringKit.toInt (strs [strs.Length - 1])-1;

				if(openAnmi[index].activeInHierarchy){
					int[] needPropnUM=TeamUnluckManager.Instance.getNeedNum();
					int[] needRMB=TeamUnluckManager.Instance.getNeedRMB();
					int[] needProp=TeamUnluckManager.Instance.getNeedProp();
					int tm=TeamUnluckManager.Instance.getIndexx(idss.Count+1);
					int haveNum=StorageManagerment.Instance.getProp(needProp[tm])==null?0:StorageManagerment.Instance.getProp(needProp[tm]).getNum();
					UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
						win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"),
						                LanguageConfigManager.Instance.getLanguage ("teamEdit_err04l",needRMB[tm].ToString(), needPropnUM[tm].ToString(),haveNum.ToString(),PropSampleManager.Instance.getPropSampleBySid(needProp[tm]).name),(msgHandle) => {
							if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
								if(haveNum<needPropnUM[tm]){
									UiManager.Instance.openDialogWindow<MessageLineWindow>((winnn)=>{
										winnn.Initialize(LanguageConfigManager.Instance.getLanguage("teamEdit_err04l1",PropSampleManager.Instance.getPropSampleBySid(needProp[tm]).name));
									});
								}else if(UserManager.Instance.self.getRMB()<needRMB[tm]){
									UiManager.Instance.openDialogWindow<MessageLineWindow>((winnn)=>{
										winnn.Initialize(LanguageConfigManager.Instance.getLanguage("teamEdit_err04l11"));
									});
								}
								else{
									TeamEmtpyFPort fport = FPortManager.Instance.getFPort<TeamEmtpyFPort> ();
									fport.access (index+1,openEmptyForLevelNew);
								}
							}
						}
						);
					});				
				}else{
					//队伍中不能上阵和换人
					openNewWindoww();
				}
			}else{
				//队伍中不能上阵和换人
				openNewWindoww();
			}

		}
	}
	void openNewWindoww(){
		//队伍中不能上阵和换人
		if (ArmyManager.Instance.isEditArmyActive ()) {
			//公会战队伍单独处理响应弹窗
			if(comeFrom==FROM_GUILD)
			{
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_102"));
			}
			else
			{
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("teamEdit_err03"),
					                LanguageConfigManager.Instance.getLanguage ("teamEdit_err02"),(msgHandle) => {
						if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
							EventDelegate.Add (OnHide,()=>{
								this.destoryWindow ();
							});
							FuBenManagerment.Instance.inToFuben ();
						}
					}
					);
				});
			}
        } else if (comeFrom == FROM_MINING) {
            UiManager.Instance.openWindow<CardChooseWindow>((tmpWin) => {
                tmpWin.Initialize(CardChooseWindow.MINING);
            });
        } else {
			UiManager .Instance.openWindow <CardChooseWindow> ((tmpWin) => {
				tmpWin.Initialize (CardChooseWindow.ROLECHOOSE);
			});
		}
	}
	public void updateBeastButton (Card _card)
	{
		if (_card == null)
			return;
		beastImage.alpha = 1;
		noBeastBg.gameObject.SetActive (false);
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH ,_card, beastImage);
		beast = _card;
		ArmyManager.Instance.ActiveEditArmy.beastid = beast.uid;
		GuideManager.Instance.guideEvent ();
	}

	public void reLoadTeam ()
	{  
		reLoadCard ();
		beast = StorageManagerment.Instance.getBeast (ArmyManager.Instance.ActiveEditArmy.beastid);
		if (beast != null){
            beatButton.disableButton(false); 
            updateBeastButton(beast);
        }
        else if (ArmyManager.Instance.ActiveEditArmy.getPlayerNum() == 0) {
            beastImage.alpha = 0;
            beatButton.disableButton(true);
            noBeastBg.gameObject.SetActive(true);
           
        } else {
            beatButton.disableButton(false);
            beastImage.alpha = 0;
            noBeastBg.gameObject.SetActive(true);
        }
		if(ArmyManager.Instance.isdeath==0&&comeFrom==FROM_GUILD){
			buttonRevive.gameObject.SetActive(true);
		}
		//跳到指定阵形
		for (int i=0; i< FormationManagerment.Instance.getPlayerFormation().Count; i++) {
			
			if (ArmyManager.Instance.ActiveEditArmy.formationID == FormationManagerment.Instance.getPlayerFormation () [i])
				content.jumpTo (i);
			changeArrow (i);
		}
		rushCombat ();
	}

	/// <summary>
	/// 更新上阵卡片信息
	/// </summary>
	public void reLoadCard ()
	{
		//获得正式队员 
		string[] players = ArmyManager.Instance.ActiveEditArmy.players;
		for (int i=0; i<players.Length; i++) {
			teamForRole [i].hideInBattle = true;
			Card c = StorageManagerment.Instance.getRole (players [i]);
			if (c != null) {
				teamForRole [i].gameObject.SetActive (true);
				//特殊处理，如果是公会战队伍则显示血条信息
				if(ArmyManager.Instance.ActiveEditArmy.armyid == ArmyManager.PVP_GUILD)
				{
					teamForRole[i].init (c,this,null,true,ArmyManager.Instance.getCardHpByUid(c.uid));
				}
				else
					teamForRole [i].init (c, this, null);
			} else {
				teamForRole [i].card = null;
				teamForRole [i].gameObject.SetActive (false);
			}
		}
		//获得替补队员
		string[] substitute = ArmyManager.Instance.ActiveEditArmy.alternate;
		for (int i=0; i<substitute.Length; i++) {
			teamSubRole [i].hideInBattle = true;
			Card c = StorageManagerment.Instance.getRole (substitute [i]);
			if (c != null) {
				teamSubRole [i].gameObject.SetActive (true);
				//特殊处理，如果是公会战队伍则显示血条信息
				if(ArmyManager.Instance.ActiveEditArmy.armyid == ArmyManager.PVP_GUILD)
				{
					teamSubRole[i].init (c,this,null,true,ArmyManager.Instance.getCardHpByUid(c.uid));
				}
				else
				teamSubRole [i].init (c, this, null);
			} else {
				teamSubRole [i].card = null;
				teamSubRole [i].gameObject.SetActive (false);
			}
		}
	}

	void PlayCardAnimation ()
	{
		TweenPosition tp;
		for (int i = 0; i < 5; i++) {
			if (teamForRole [i].gameObject.activeInHierarchy) {
				teamForRole [i].transform.localPosition += new Vector3 (0, 300, 0);
				tp = TweenPosition.Begin (teamForRole [i].gameObject, 0.05f, teamForRole [i].transform.localPosition - new Vector3 (0, 300, 0));
				tp.delay = i * 0.1f;
			}

			if (!teamForEmtpy [i].gameObject.activeSelf) {
				teamForEmtpy [i].SetActive (true);
			}
//			teamForEmtpy [i].transform.localPosition += new Vector3 (0, 300, 0);
//			tp = TweenPosition.Begin (teamForEmtpy [i].gameObject, 0.2f, teamForEmtpy [i].transform.localPosition - new Vector3 (0, 300, 0));
//			tp.delay = i * 0.1f;


			if (teamSubRole [i].gameObject.activeInHierarchy) {
				teamSubRole [i].transform.localPosition += new Vector3 (0, 500, 0);
				tp = TweenPosition.Begin (teamSubRole [i].gameObject, 0.05f, teamSubRole [i].transform.localPosition - new Vector3 (0, 500, 0));
				tp.delay = i * 0.1f;
			}

			if (!teamSubEmtpy [i].gameObject.activeSelf) {
				teamSubEmtpy [i].SetActive (true);
			}
//			teamSubEmtpy [i].transform.localPosition += new Vector3 (0, 500, 0);
//			tp = TweenPosition.Begin (teamSubEmtpy [i].gameObject, 0.2f, teamSubEmtpy [i].transform.localPosition - new Vector3 (0, 500, 0));
//			tp.delay = i * 0.1f;
		}
		StartCoroutine (Utils.DelayRun (()=>{
			MaskWindow.UnlockUI();
		},1f));
	}

	int oldCombat = 0;//初始战斗力
	int newCombat = 0;//最新战斗力

	int oldAllCombat = 0;//初始全部战斗力
	int newAllCombat = 0;//最新全部战斗力

	bool showMainCombat= true;

	//刷新战斗力
	public void rushCombat ()
	{
		newCombat = ArmyManager.Instance.ActiveEditArmy.getMainCombat ();	
		newAllCombat = ArmyManager.Instance.ActiveEditArmy.getAllCombat();

		TweenLabelNumber tween=combatLabel.GetComponent<TweenLabelNumber>();
		tween.ResetToBeginning();

		if(showMainCombat)
		{
			tween.from=oldCombat;
			tween.to=newCombat;
			oldCombat=newCombat;
		}else
		{
			tween.from=oldAllCombat;
			tween.to=newAllCombat;
			oldAllCombat=newAllCombat;
		}


        ShowMiningSpeedInfo();

		tween.enabled=true;
		combatLabel.GetComponent<TweenLabelNumber> ().enabled = false;
		combatLabel.text = newAllCombat.ToString ();
		combatBg.spriteName = "allCombat";//这里是全队
		showMainCombat = false;
	}
	void Update()
	{
		if(Time.frameCount%250==0)
		{
			TweenLabelNumber tween=combatLabel.GetComponent<TweenLabelNumber>();
			//数字还在跳转 则不需要更新
			if(tween.enabled)
			{
				return;
			}
			if(UserManager.Instance.self.getUserLevel()>=13)
				showMainCombat=!showMainCombat;
			else
				showMainCombat = true;

            if (comeFrom == FROM_MINING) {
                showMainCombat = false;
            }
			if(showMainCombat)
			{
				combatLabel.text=newCombat.ToString();
                combatBg.spriteName = "mainCombat";//主力
			}else
			{
				combatLabel.text=newAllCombat.ToString();
                combatBg.spriteName = "allCombat";//全队
			}
		}
	}
	/// <summary>
	/// 转换卡片
	/// </summary>
	public List<Card> getCardsFromStrings (List<string> str)
	{
		List<Card> cards = new List<Card> ();
		for (int i=0; i<str.Count; i++) {
			if (str [i] != "0")
				cards.Add (StorageManagerment.Instance.getRole (str [i]));
		}
		return cards;
	}
	
	//设置阵型ID
	public void setFormation (int id)
	{
		if (ArmyManager.Instance.ActiveEditArmy == null)
			return; 
		ArmyManager.Instance.ActiveEditArmy.formationID = id;
		rushCombat ();
	}
	 
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		 
		if (gameObj.name == "teamSwitch") {

			if(comeFrom !=FROM_MINING){
				if(ArmyManager.Instance.ActiveEditArmy == ArmyManager.Instance.EditArmy1){
					teamSwitch(3);
					PlayCardAnimation ();
				}else if(ArmyManager.Instance.ActiveEditArmy == ArmyManager.Instance.EditArmy3){
					teamSwitch(1);
					PlayCardAnimation ();
				}
			}else{
				if(ArmyManager.Instance.ActiveEditArmy == ArmyManager.Instance.EditArmy4 ){
					teamSwitch(5);
					PlayCardAnimation ();
				}else if(ArmyManager.Instance.ActiveEditArmy == ArmyManager.Instance.EditArmy5){
					teamSwitch(4);
					PlayCardAnimation ();
				}
			}


		}
		else if (gameObj.name == "save" || gameObj.name == "close") {
			GuideManager.Instance.doGuide ();
			if (!(fatherWindow is ArenaAuditionsWindow)) {
				setFormation (content.getCenterObj ().GetComponent<ButtonFormationChoose> ().teamID);
			}			

			if(comeFrom == FROM_MINING)
			{
			    if (gameObj.name == "close")
			    {
                    ArmyManager.Instance.SaveMiningArmy(null);
                    closeWindow();
			    }
			    else
			    {
                    SaveMiningArmy();
			    }
				
			}else if(comeFrom==FROM_GUILD)
			{
				saveGuildFightArmy();
			}
			else{
				saveArmy ();
				//closeWindow();
			}
		}
		else if (gameObj.name == "beastChangeButton") {	
			if (ArmyManager.Instance.isEditArmyActive ()) {
				//公会战队伍单独处理响应弹窗
				if(ArmyManager.Instance.ActiveEditArmy.armyid == ArmyManager.PVP_GUILD)
				{
					UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_102"));
				}
				else
				{
					UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
						win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("teamEdit_err03"),
						                LanguageConfigManager.Instance.getLanguage ("teamEdit_err02"),(msgHandle) => {
							if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
								EventDelegate.Add (OnHide,()=>{
									this.destoryWindow ();
								});
								FuBenManagerment.Instance.inToFuben ();
							}
						}
						);
					});
				}
				return;
			}
			//女神上阵
			if (GuideManager.Instance.isEqualStep (17003000)) {
				GuideManager.Instance.doGuide ();
			}
			if (!BeastEvolveManagerment.Instance.isHaveBeast ()) {
				UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("noBeast"), null);
				return;
			}
			UiManager.Instance.openWindow<BeastAttrWindow> ((win) => {
				win.Initialize (beast, comeFrom == FROM_PVE ? BeastAttrWindow.FUBEN : BeastAttrWindow.CARDCHANGE); 
			});
		}
        else if (gameObj.name == "AddCombat")
        {
            UiManager.Instance.openWindow<CombatTipsWindow>();
        }
		else if(gameObj.name =="revive")
		{
			GuildManagerment.Instance.SendRivive(()=>{
				ArmyManager.Instance.EditArmy6 = ArmyManager.Instance.DeepClone (ArmyManager.Instance.getArmy (6));
				teamSwitch(6);
				buttonRevive.gameObject.SetActive(false);
			});
		}
	}

	private void teamSwitch (int id)
	{ 
		if (id == 1) {
			buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0066");
			ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy1;
		} else if (id == 2) {
			buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0067");
			ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy2;
		} else if (id == 3) {
			buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0068");
			ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy3;
		}else if (id == 4) {
			buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("mining_team1");
            teamName.text = LanguageConfigManager.Instance.getLanguage("mining_team1");
			ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy4;
		}else if (id == 5) {
			buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("mining_team2");
            teamName.text = LanguageConfigManager.Instance.getLanguage("mining_team2");
			if(ArmyManager.Instance.EditArmy5 == null){
				ArmyManager.Instance.InitMiningTeam();
			}
			ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy5;
		}else if (id == 6) {
			buttonTeamChoose.textLabel.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_101");
			if(ArmyManager.Instance.EditArmy6 == null){
				ArmyManager.Instance.InitGuildFightTeam();
			}
			ArmyManager.Instance.ActiveEditArmy = ArmyManager.Instance.EditArmy6;
		}


		if(ArmyManager.Instance.CheckActiveEditArrayIsEmpty() || (isStartmining && ArmyManager.Instance.ActiveEditArmy.state == 1)){
			saveButton.disableButton(true);
		}else{
			saveButton.disableButton(false);
		}
		if(ArmyManager.Instance.ActiveEditArmy == ArmyManager.Instance.EditArmy6){

			FPortManager.Instance.getFPort<GetGuildFightTeamHpInfoFPort>().access(reLoadTeam);
		}
		else
			reLoadTeam (); 
	}
	
	public void releaseCard (GameObject drag)
	{
//		MonoBase.print ("I releaseCard: " + drag.name);
		dragCardObj.gameObject.SetActive (false);
		
		ButtonTeamPlayer button = drag.GetComponent<ButtonTeamPlayer> ();
		if (button != null) { 
			
			if (button.cardImage.mainTexture != null)	
				button.cardImage.gameObject.SetActive (true);
			
			button.beDrag = false;	
			button.collider.enabled = true;
			if (button.card != null) {
				
				button.quality.gameObject.SetActive (true);
				button.level.text = "" + button.card.getLevel ();
				button.sign.gameObject.SetActive (true);
				
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + button.card.getImageID (), button.cardImage);
			}
		} 
	}

	public int getIndexWidthItem (GameObject obj)
	{
		string[] strs = obj.name.Split ('_');
		return StringKit.toInt (strs [strs.Length - 1]) - 1;
	}
	
	public bool IsAlternateWidthItem (GameObject obj)
	{
		return obj.transform.parent.gameObject.name == "suber";
	}
	
	public RoleView getDstRoleViewWidthItem (GameObject obj)
	{
		int index = getIndexWidthItem (obj);
		if (IsAlternateWidthItem (obj))
			return teamSubRole [index];
		else
			return teamForRole [index];
	}

	public RoleView getDstRoleViewWidthCard (Card card)
	{

		foreach (RoleView each in teamSubRole) {
			if (each == null || each.card == null)
				continue;

			if (each.card.uid == card.uid) {
				return each;
			}
		}

		foreach (RoleView each in teamForRole) {
			if (each == null || each.card == null)
				continue;
			
			if (each.card.uid == card.uid) {
				return each;
			}
		}

		return null;

	}

	public Vector3 getEmptyPositionWithItem (GameObject obj)
	{
		int index = getIndexWidthItem (obj);
		if (IsAlternateWidthItem (obj))
			return teamSubEmtpy [index].transform.localPosition;
		else
			return teamForEmtpy [index].transform.localPosition;
	}
}
