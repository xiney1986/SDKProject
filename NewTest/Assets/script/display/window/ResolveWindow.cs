using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 分解窗口 （晶炼）
 * */
public class ResolveWindow : WindowBase {
	public static List<PrizeSample> resolveResult = new List<PrizeSample> ();
	private Color NROMAL = new Color (1f, 0.98f, 0.7f, 1);
	private Color LINE = new Color (0.3f, 0.25f, 0.14f, 1);
	private Color normalColor = new Color (0.98f, 0.9765f, 247f / 255f, 1f);
	private Color abnormalColor = new Color (0.631f, 0.4545f, 86f / 255f, 1f);
	private int currentEquipContentMNum = 20;//装备页面一页中的最多元素个数
	private int currentCardContentMNum = 8;//卡片页面一页中的最多元素个数
	public List<PrizeSample> prizeList = new List<PrizeSample>();
	public UISprite indicate;//指示
	public ResolveEquipContent equipContent;
	public GameObject cardButtonPrefab;
	public GameObject equipButtonPrefab;
	public ButtonBase buttonSell;
	public UILabel rmbLabel;
	public UILabel moneyLabel;
	public UILabel selectButtonText;
	/** <46级 */
	public TapContentBase tapBase;
    public TapContentBase tapBase2;//
	/** >=60级 */
	public TapContentBase tapBase1;
	/** 当前使用的tap */
	TapContentBase tapContent;
	private bool isShowMessageType = false;
	private int tapIndex;//分页按钮索引
	private ArrayList cardofHaveStarSoul;
	public ResolveCardContentNew cardContentNew;
	public List<Card> selectedCardList = new List<Card> ();
	public List<Equip> selectedEquipList = new List<Equip> ();
    public List<MagicWeapon> selectMagicList = new List<MagicWeapon>();
	private string selectType = "card";
	private ArrayList allList ;
	private ArrayList showList ;
	private ArrayList selectList = new ArrayList ();
	public ResolveEffect resolveEffect;
	private int equipResolveLevel = 60;
    private int magicWeaponLv = 45;
    private int refineweapon = 30;
    private int moneyIndex = 0;
    private long moneyNum = 0;


	//晶炼产生物品位置变量
	public GameObject[] objPos;
    public GameObject resolveResultsFather;

	protected override void begin () {
		base.begin ();
		updataData ();
		MaskWindow.UnlockUI ();
	}
	
	public bool isSelect (object obj) {
		for (int i = 0; i < selectList.Count; i++) {
			if (selectList [i] == obj) {
				return true;
			}
		}
		return false;
	}
	//断线重连
	public override void OnNetResume () {
		base.OnNetResume ();
		messageBack (null);
	}
	public void setShowIndicate (bool isShow) {
		indicate.gameObject.SetActive (isShow);
	}
	public void Initialize () {
		updateInit ();
	}
	private void updateInit () {
		cardContentNew.initInfo (this);
		buttonSell.disableButton (true);
		updateTapContent();
		tapContent.resetTap ();
		tapContent.changeTapPage (tapContent.tapButtonList [0]);
	}
	/** 更新Tap容器 */
	private void updateTapContent() {
        if (UserManager.Instance.self.getUserLevel() < refineweapon)
        {
            tapBase1.gameObject.SetActive(false);
            tapBase.gameObject.SetActive(false);
            tapBase2.gameObject.SetActive(true);
            tapContent = tapBase2;
        }
        else if (UserManager.Instance.self.getUserLevel() < magicWeaponLv)
        {
            tapBase.gameObject.SetActive(true);
            tapBase1.gameObject.SetActive(false);
            tapBase2.gameObject.SetActive(false);
            tapContent = tapBase;
        } else {
            tapBase.gameObject.SetActive(false);
            tapBase1.gameObject.SetActive(true);
            tapBase2.gameObject.SetActive(false);
            tapContent = tapBase1;
        }
		
	}
	//更新碎片显示信息
	private void updateInfo (List<PrizeSample> pList){
		int sid;
		Prop tmp;
	    moneyIndex = 0;
		for (int i=0; i<prizeList.Count; i++) {
			ResourcesManager.Instance.LoadAssetBundleTexture (prizeList[i].getIconPath (), resolveEffect.icon[i]);
			sid = prizeList[i].pSid;
			if(sid != 0){
				tmp = StorageManagerment.Instance.getProp (sid);
				resolveEffect.iconText[i].text = "[6E473D]" + (tmp == null ? 0 : tmp.getNum ()) + "[-]";
			}else{
				resolveEffect.iconText[i].text = "[6E473D]" + UserManager.Instance.self.getMoney () + "[-]";
			    moneyIndex = i;
			}
		}

		if (pList != null) {
			int count;
			for(int i=0;i<prizeList.Count;i++){
				count = 0;
				for(int j=0;j<pList.Count;j++){
					if(prizeList[i].pSid == pList[j].pSid){
						count += StringKit.toInt( pList[j].num );
					}
				}
				if(count > 0)
					resolveEffect.iconText [i].text += "[3A9663]+" + count + "[-]";
			    if (i == moneyIndex)
			        moneyNum = (long) (UserManager.Instance.self.getMoney() + count);

			}
		}
	}
	//计算所有卡片
	public void recalculateAllCard (ArrayList list) {
		showList = new ArrayList ();
		
		//生成新的引用list

		foreach (Card each in list) {
			showList.Add (each);
		}
		Card tempCard;
		//剔除保护中、装备中、上阵中的卡片
		for (int i = 0; i < showList.Count; i++) {
			tempCard = showList [i] as Card;
//            if ((tempCard.state & CardStateType.STATE_USING) == 1 || (tempCard.state & CardStateType.STATE_MINING) == 4 || 
//                ((tempCard.state & (CardStateType.STATE_MINING|CardStateType.STATE_USING)) == 5)) {
//				showList.Remove (showList [i]);
//				i--;
//				continue;
//			}
//			else 
			if (tempCard.uid == UserManager.Instance.self.mainCardUid) {
				showList.Remove (showList [i]);
				i--;
				continue;
			}
			else if (tempCard.getResolveResults () == null) {
				showList.Remove (showList [i]);
				i--;
				continue;
			}
			else if (IntensifyCardManager.Instance.isEatCard (tempCard)) {
				showList.Remove (showList [i]);
				i--;
				continue;
			}
			else if (GuideManager.Instance.isEqualStep (114004000)) {
				int guideIndex = GuideConfigManager.Instance.getIndexByGuideSid (114004000);
				GuideCondition[] cons = GuideConfigManager.Instance.getConditionByGuideSid (guideIndex);
				if (cons != null && cons [0].conditionType == GuideConditionType.CARDSID) {
					if (tempCard.sid != StringKit.toInt (cons [0].conditionValue)) {
						showList.Remove (showList [i]);
						i--;
					}
				}
			}
		}
	}
	//计算所有装备
	public void recalculateAllEquip (ArrayList list) {
		if (showList == null)
			showList = new ArrayList ();
		showList.Clear ();
		foreach (Equip each in list) {
			showList.Add (each);
		}
		//剔除保护中、装备中、上阵中的卡片
		for (int i = 0; i < showList.Count; i++) {
			if (((showList [i] as Equip).state & CardStateType.STATE_USING) == 1) {
				showList.Remove (showList [i]);
				i--;
				continue;
			}
			else if (((showList[i] as Equip).state & CardStateType.STATE_LOCK) == 2)
			{
			    showList.Remove(showList[i]);
			    i--;
			    continue;
			}
			else if ((showList[i] as Equip).getResolveResults() == null)
			{
			    showList.Remove(showList[i]);
			    i--;
			    continue;
			}
			else if ((showList[i] as Equip).getQualityId() < 3)
			{
			    showList.Remove(showList[i]);
			    i--;
			    continue;
			}
			else if ((showList[i] as Equip).getEXP() > 0)
			{
			    showList.Remove(showList[i]);
			    i--;
			    continue;
			}
		}
	}
	//向选择到的装备集中添加装备
	public void onSelectEquip (Equip equip) {
		selectList.Add (equip);
		selectedEquipList.Add (equip);
		changeButton ();
	}
	//向选择到的装备集中移除装备
	public void offSelectEquip (Equip equip) {
		selectList.Remove (equip);
		selectedEquipList.Remove (equip);
		changeButton ();
	}
    public void offSelectmagic(MagicWeapon magic) {
        selectList.Remove(magic);
        selectMagicList.Remove(magic);
        changeButton();
    }
	//向选择到的卡片集中添加卡片
	public void onSelectCard (Card card) {
		selectList.Add (card);
		selectedCardList.Add (card);
		changeButton ();
	}
	//向选择到的卡片集中移除卡片
	public void offSelectCard (Card card) {
		selectList.Remove (card);
		selectedCardList.Remove (card);
		changeButton ();
	}
    public void onSelectMagic(MagicWeapon magic) {
        selectList.Add(magic);
        selectMagicList.Add(magic);
        changeButton();
    }
	public void changeButton () {
		List<PrizeSample> awardList = new List<PrizeSample> ();
		awardList.Clear ();
		for (int i = 0; i < selectList.Count; i++) {
			if (selectList [i] is Card) {
				Card card = selectList[i] as Card;
				PrizeSample[] prizeSamples=card.getResolveResults();
				if(prizeSamples!=null) {
					for (int j = 0; j < prizeSamples.Length; j++) {
						awardList.Add(prizeSamples[j]);
					}
				}
			}
			if (selectList [i] is Equip) {
				Equip equip=selectList[i] as Equip;
				PrizeSample[] prizeSamples=equip.getResolveResults();
				if(prizeSamples!=null) {
					for (int j = 0; j < prizeSamples.Length; j++) {
						awardList.Add (prizeSamples[j]);
					}
				}
			}
            if(selectList[i] is MagicWeapon){
                MagicWeapon ms = selectList[i] as MagicWeapon;
                PrizeSample[] prizeSamples = ms.getReSourcePrizes();
                if (prizeSamples != null) {//固定的奖励
                    for (int j = 0; j < prizeSamples.Length; j++) {
                        awardList.Add(prizeSamples[j]);
                    }
                }
            }
		}
		updateInfo (awardList);
		for (int i = 0; i < resolveEffect.iconText.Length; i++) {
			if (resolveEffect.iconText [i].text.Contains ("+")) {
				buttonSell.disableButton (false);
				break;
			}
			else if (i == resolveEffect.iconText.Length - 1) {
				buttonSell.disableButton (true);
			}
		}
	}
	//分页按钮事件
	public  void updataData () {
		if (GuideManager.Instance.isEqualStep (114003000)) {
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
		}
		//SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDSTORE_WINDOW);
        SortCondition sc = new SortCondition();
		sc.addSiftCondition (new Condition (SortType.QUALITY, new int[]{1,2,3,4,5,6})); // 写死品质
		allList = SortManagerment.Instance.cardSort (StorageManagerment.Instance.getAllRole (), sc);
		recalculateAllCard (allList);
		//	indicateCardShow (showList);
		if ((showList == null || showList.Count == 0) && GuideManager.Instance.isEqualStep (114004000)) {
			GuideManager.Instance.jumpGuideSid ();
		}
	}
	//分解回调
	private void resolveBack () {
		cardContentNew.playEffect ();
		StartCoroutine (Utils.DelayRun (() => {		
			if (isShowMessageType) {
				UiManager.Instance.openDialogWindow<ResolveMessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("resolve09"), messageBack,resolveResult);
				});
			}
			else {
				UiManager.Instance.openDialogWindow<ResolveMessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("resolve09"), messageBack,resolveResult);
				});
			}
		}, 2.913f));
	}
	//分解后刷新
	private void messageBack (MessageHandle msg) {
		isShowMessageType = false;
		buttonSell.disableButton (true);
		updateInfo (null);
		selectList.Clear ();
		selectedCardList.Clear ();
		selectedEquipList.Clear ();
        selectMagicList.Clear();
		updataData ();
	}
	//发送通信
	private void sellFPort (MessageHandle msg) {
		if (msg == null) {
			MaskWindow.UnlockUI ();
			return;
		}
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		toFPort ();
	}
	//真正的发送消息
	private void toFPort () {
		ResolveGoodsFPort port = FPortManager.Instance.getFPort ("ResolveGoodsFPort") as ResolveGoodsFPort;
		port.resolveGoods (change (selectList), cardofHaveStarSoul, resolveBack);
	}
	
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {
			if (GuideManager.Instance.isEqualStep (114007000)) {
				GuideManager.Instance.doGuide ();
				GuideManager.Instance.guideEvent ();
			}
            if (moneyNum > CommandConfigManager.Instance.getMaxMoneyNum())
		    {
		        UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
		        {
		            win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"),
		                LanguageConfigManager.Instance.getLanguage("s0093"),
		                LanguageConfigManager.Instance.getLanguage("goldWillLimitTip"),
		                (msg) =>
		                {
		                    if (msg.msgEvent == msg_event.dialogOK)
		                    {
		                        if (selectType == "card")
		                        {
		                            sendCardToResolve();
		                        }
		                        else if (selectType == "equip")
		                        {
		                            sendEquipToResolve();
		                        }
		                        else if (selectType == "magic")
		                        {
		                            sendMagicToResolve();
		                        }
		                    }
		                    else
		                        MaskWindow.UnlockUI();
		                });
		        });
		    }
		    else
		    {
                if (selectType == "card") {
                    sendCardToResolve();
                } else if (selectType == "equip") {
                    sendEquipToResolve();
                } else if (selectType == "magic") {
                    sendMagicToResolve();
                }
		    }
		}
		else if (gameObj.name == "close") {
			GuideManager.Instance.doGuide ();
			finishWindow ();
        }
        else if (gameObj.name == "1" || gameObj.name == "2" || gameObj.name == "3" || gameObj.name == "4" || gameObj.name == "5" || 
            gameObj.name == "6" || gameObj.name == "7" || gameObj.name == "8" || gameObj.name == "9" || gameObj.name == "10"
            || gameObj.name == "11" || gameObj.name == "12")
        {
			int sid = prizeList [StringKit.toInt (gameObj.name) - 1].pSid;
			if (sid != 0) {
				Prop prop = PropManagerment.Instance.createProp (sid);
				if (prop != null) {
					UiManager.Instance.openDialogWindow<PropAttrWindow> ((win) => {
						win.Initialize (prop);
					});
				}
			}else{
				MaskWindow.UnlockUI();
			}
		}
		else if (gameObj.name == "buttonSelect") {
			UiManager.Instance.openWindow<ResolveCardChooseWindow> ((win) => {
				//win.InitializeCard(showList,this);
				if (selectType == "equip") {
                    ArrayList 
					allList = StorageManagerment.Instance.getAllEquip ();
					recalculateAllEquip (allList);
					win.InitializeEquip (showList, this);
				}
				else if (selectType == "card") {
					updataData ();
					win.InitializeCard (showList, this);
                } else if (selectType == "magic") {
                    ArrayList allList = StorageManagerment.Instance.getallResolveMagicWeapon();
                    showList = allList;
                    win.InitializeMagic(showList,this);
                }
			});
		}
		else if (gameObj.name == "buttonHelp") {
			UiManager.Instance.openDialogWindow<GeneralDesWindow> ((win) => {
				if (selectType == "card")
					win.initialize (LanguageConfigManager.Instance.getLanguage ("resolve12"), LanguageConfigManager.Instance.getLanguage ("resolve13"), LanguageConfigManager.Instance.getLanguage ("resolve01"));
				if (selectType == "equip")
					win.initialize (LanguageConfigManager.Instance.getLanguage ("resolve16"), LanguageConfigManager.Instance.getLanguage ("resolve13"), LanguageConfigManager.Instance.getLanguage ("resolve02"));
                if (selectType == "magic") {
                    win.initialize(LanguageConfigManager.Instance.getLanguage("resolve17"), LanguageConfigManager.Instance.getLanguage("resolve13"), LanguageConfigManager.Instance.getLanguage("resolve02l0"));
                }
			});
		}
	}
	public override void tapButtonEventBase (GameObject gameObj, bool enable) {
		base.tapButtonEventBase (gameObj, enable);
		if (gameObj.name == "chooseCard" && enable == true)
		{
		    resolveResultsFather.transform.localPosition = new Vector3(-94f, -4.5f, 0);
            resolveResultsFather.GetComponent<UIPanel>().clipOffset = new Vector2(0, 0);
			setPosition(3);
			selectType = "card";
			setPrizeShow(selectType);
			selectButtonText.text = LanguageConfigManager.Instance.getLanguage ("resolve11");
			selectedEquipList.Clear ();
            selectMagicList.Clear();
			selectList.Clear ();
			updateEquipContent ();
            updateMagicContent();
			updateInfo (null);
			buttonSell.disableButton (true);
		}
		else if (gameObj.name == "chooseEquip" && enable == true) {
            resolveResultsFather.transform.localPosition = new Vector3(-94f, -4.5f, 0);
            resolveResultsFather.GetComponent<UIPanel>().clipOffset = new Vector2(0,0);
			setPosition(5);
			selectType = "equip";
			setPrizeShow(selectType);
			selectButtonText.text = LanguageConfigManager.Instance.getLanguage ("resolve14");
			selectedCardList.Clear ();
            selectMagicList.Clear();
			selectList.Clear ();
			updateCardContent ();
            updateMagicContent();
			updateInfo (null);
			buttonSell.disableButton (true);
        } else if (gameObj.name == "chooseMagic"&&enable==true) {
            resolveResultsFather.transform.localPosition = new Vector3(-94f, -4.5f, 0);
            resolveResultsFather.GetComponent<UIPanel>().clipOffset = new Vector2(0, 0);
            setPosition(6);
            selectType = "magic";//秘宝
            setPrizeShow(selectType);
            selectButtonText.text = LanguageConfigManager.Instance.getLanguage("resolve140l");
            selectedCardList.Clear();
            selectedEquipList.Clear();
            selectList.Clear();
            updateCardContent();
            updateEquipContent();
            updateInfo(null);
            buttonSell.disableButton(true);
        }
	}
	private bool isResolve () {
		for (int i = 0; i < resolveEffect.iconText.Length; i++) {
			if (resolveEffect.iconText [i].text.Contains ("+")) {
				int num = StringKit.toInt (resolveEffect.iconText [i].text.Split ('+') [0]) + StringKit.toInt (resolveEffect.iconText [i].text.Split ('+') [1]);
				if (num >= 999999) {
					UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
						win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("s0129"), sellFPort);
					});
					return true;
				}
			}
		}
		return false;
	} 
	//发送购买通信格式转换
	private string change (ArrayList list) {
		string strCard = "card";
		string strEquip = "equipment";
        string strMagic = "artifact";
        string strsp = "goods";
		cardofHaveStarSoul = new ArrayList ();
		for (int i = 0; i < list.Count; i++) {
			if (list [i] is Card) {
				if ((list [i] as Card).getStarSoulByAll () != null && ((list [i] as Card).getStarSoulByAll ()).Length > 0)
					cardofHaveStarSoul.Add (list [i] as Card);
				strCard += "," + (list [i] as Card).uid;
			}
			else if (list [i] is Equip) {
				strEquip += "," + (list [i] as Equip).uid;
			}else if(list[i] is MagicWeapon){
                MagicWeapon ms = list[i] as MagicWeapon;
                strMagic += "," + ms.uid;
            }
		}
		return strCard + "|" + strEquip+"|"+strMagic+"|"+strsp;
	}
	///<summary>
	/// 更新卡片容器
	/// </summary>
	public void updateCardContent () {
		cardContentNew.updateCtrl (selectedCardList);
	}
	///<summary>
	/// 更新装备容器
	/// </summary>
	public void updateEquipContent () {
		cardContentNew.updateCtrl (selectedEquipList);
	}
    public void updateMagicContent() {
        cardContentNew.updateCtrl(selectMagicList);
    }
	private void sendCardToResolve () {
		Card tempCard;
		ArrayList tempCardList = new ArrayList ();
		if (selectList != null) {
			for (int i=0; i<selectList.Count; i++) {
				if (!(selectList [i] is Card))
					continue;
				tempCard = selectList [i] as Card;
				if (tempCard.getEXP () > 0 || tempCard.getHPExp () > 0 || tempCard.getATTExp () > 0 || tempCard.getDEFExp () > 0 ||
					tempCard.getMAGICExp () > 0 || tempCard.getAGILEExp () > 0 || tempCard.getSkillsExp () > 0 || tempCard.getEvoLevel () > 0||
                    (CardSampleManager.Instance.checkBlood(tempCard.sid,tempCard.uid)&& tempCard.cardBloodLevel >0))
					tempCardList.Add (tempCard);
			}
		}
		if (tempCardList != null && tempCardList.Count > 0)
			UiManager.Instance.openDialogWindow<MessageWithContentWindow> ((win) => {
				win.initWindow (LanguageConfigManager.Instance.getLanguage ("resolve08"), tempCardList, sellFPort);
			});
		else {
			toFPort ();
		}
	}
	private void sendEquipToResolve () {
		toFPort ();
	}
    private  void sendMagicToResolve(){
        toFPort();
    }
	//
	private void setPosition(int num){
		if (num == 5) {
			objPos [0].gameObject.SetActive(true);
			objPos [1].gameObject.SetActive(true);
			objPos [2].gameObject.SetActive(true);
			objPos [3].gameObject.SetActive(true);
			objPos [4].gameObject.SetActive(true);
            objPos[5].gameObject.SetActive(true);
            objPos[6].gameObject.SetActive(true);
            objPos[7].gameObject.SetActive(true);
            objPos[8].gameObject.SetActive(true);
            objPos[9].gameObject.SetActive(true);
            objPos[10].gameObject.SetActive(true);
            objPos[11].gameObject.SetActive(true);
			objPos [0].transform.localPosition = new Vector3 (-125f, 65f, 0f);
			objPos [1].transform.localPosition = new Vector3 (40f, 65f, 0f);
			objPos [2].transform.localPosition = new Vector3 (-125f, -65f, 0f);
			objPos [3].transform.localPosition = new Vector3 (40f, -65f, 0f);
			objPos [4].transform.localPosition = new Vector3 (205f, -65f, 0f);
            objPos[5].transform.localPosition = new Vector3(-125f, 0f, 0f);
            objPos[6].transform.localPosition = new Vector3(-5f, 0f, 0f);
            objPos[7].transform.localPosition = new Vector3(115f, 0f, 0f);
            objPos[8].transform.localPosition = new Vector3(235f, 0f, 0f);
            objPos[9].transform.localPosition = new Vector3(-125f, -130f, 0f);
            objPos[10].transform.localPosition = new Vector3(40f, -130f, 0f);
            objPos[11].transform.localPosition = new Vector3(205f, -130f, 0f);
		}
		else if(num==3){
			objPos [0].gameObject.SetActive(true);
			objPos [1].gameObject.SetActive(true);
			objPos [2].gameObject.SetActive(true);
			objPos [3].gameObject.SetActive(false);
			objPos [4].gameObject.SetActive(false);
            objPos [5].gameObject.SetActive(false);
            objPos[6].gameObject.SetActive(false);
            objPos[7].gameObject.SetActive(false);
            objPos[8].gameObject.SetActive(false);
            objPos[9].gameObject.SetActive(false);
            objPos[10].gameObject.SetActive(false);
            objPos[11].gameObject.SetActive(false);
			objPos [0].transform.localPosition = new Vector3 (-115f, 40f, 0f);
			objPos [1].transform.localPosition = new Vector3 (120f, 40f, 0f);
			objPos [2].transform.localPosition = new Vector3 (-115f, -34f, 0f);
			objPos [3].transform.localPosition = new Vector3 (120f, -34f, 0f);
        } else if(num==6) {
            objPos[0].gameObject.SetActive(true);
            objPos[1].gameObject.SetActive(true);
            objPos[2].gameObject.SetActive(true);
            objPos[3].gameObject.SetActive(true);
            objPos[4].gameObject.SetActive(true);
            objPos[5].gameObject.SetActive(false);
            objPos[6].gameObject.SetActive(false);
            objPos[7].gameObject.SetActive(false);
            objPos[8].gameObject.SetActive(false);
            objPos[9].gameObject.SetActive(false);
            objPos[10].gameObject.SetActive(false);
            objPos[11].gameObject.SetActive(false);
            objPos[0].transform.localPosition = new Vector3(-115f, 40f, 0f);
            objPos[1].transform.localPosition = new Vector3(52f, 40f, 0f);
            objPos[2].transform.localPosition = new Vector3(-60f, -34f, 0f);
            objPos[3].transform.localPosition = new Vector3(140f, -34f, 0f);
            objPos[4].transform.localPosition = new Vector3(220f, 35f, 0f);
            objPos[5].transform.localPosition = new Vector3(205f, 40f, 0f);
        }
	}
	private void setPrizeShow(string type){
		prizeList.Clear ();
		if (type == "card") {
			prizeList.Add (new PrizeSample (PrizeType.PRIZE_PROP, 71144, 0));
			prizeList.Add (new PrizeSample (PrizeType.PRIZE_PROP, 71143, 0));
			prizeList.Add (new PrizeSample (PrizeType.PRIZE_MONEY, 0, 0));
		}
		else if (type == "equip") {
			prizeList.Add (new PrizeSample (PrizeType.PRIZE_PROP, 71167, 0));
			prizeList.Add (new PrizeSample (PrizeType.PRIZE_MONEY, 0, 0));
			prizeList.Add (new PrizeSample (PrizeType.PRIZE_PROP, 71163, 0));
			prizeList.Add (new PrizeSample (PrizeType.PRIZE_PROP, 71165, 0));
			prizeList.Add (new PrizeSample (PrizeType.PRIZE_PROP, 71166, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71210, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71222, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71223, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71224, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71286, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71287, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71288, 0));
		}else if(type=="magic"){
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71176, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71177, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71178, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71179, 0));
            prizeList.Add(new PrizeSample(PrizeType.PRIZE_PROP, 71180, 0));
        }
	}
}