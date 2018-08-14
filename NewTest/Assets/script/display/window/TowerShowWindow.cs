using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 爬塔抽奖展示窗口
/// </summary>
public class TowerShowWindow : WindowBase
{
	/** 窗口跳转类型 */
	public const int SKIP_NONE_TYPE=0, // 跳转默认窗口
				     SKIP_NOTICE_TYPE=1; // 跳转公告窗口
    public const int ONEOPENLIMTVIPLEVL = 5;//一键翻牌VIP限制等级
	/** 跟节点 */
	public GameObject root;
	/** 卡片选择容器 */
	public GameObject content;
	/** 特效点 */
	public GameObject effectPoint;
	/** 英雄之章开启特效点 */
	public GameObject heroRoadEffectPoint;
	/** 特效资源 */
	GameObject effectObj;
	/** 英雄之章开启特效资源 */
	GameObject heroRoadEffectObj;
	public GameObject shareButton;
	/** 奖品展示点集合 */
	public GameObject[] panes;
	/** 卡片预制件 */
	public GameObject roleViewPrefab;
	/** 道具预制件 */
	public GameObject goodsViewPrefab;
	/** 紫色,橙色卡片额外特效预制 */
	public GameObject qualityEffectPrefab;
    public ButtonBase closeButton;//关闭按键
    public ButtonBase enterButton;//开启宝箱，领取奖励
    public ButtonBase openButton;//开启宝箱
    public ButtonBase buyButton;//购买按钮
    public ButtonBase oneTurnButton;//一键翻牌
    public UITexture myTexure;
    public UILabel canOPenNum;//今天开宝箱的次数
    public UILabel canSelectPai;//
    public UILabel towerNum;//宝箱层数
    public UILabel RmbNum;//玩家拥有的RMB
    public UILabel costTotalNum;//一键全翻的消耗
    public GameObject closeDoor;
    public GameObject doorEffect;
    public GameObject costObj;//消耗显示
    public UILabel towerLevel;
    private int conccentIndex=-1;//当前翻开的卡片
	/**  抽奖特效预制件 */
	GameObject luckDrawCardEffect;
	/** 当前激活的展示 */
	GameObject currentActivePane;
	/** 奖品卡片集合 */
	List<Card> cardList = null;
	/** 奖品装备集合 */
	List<Equip> equipList = null;
	/** 奖品道具集合 */
	List<Prop> propList = null;
    /**奖品秘宝合集 */
    List<MagicWeapon> magicweaponList = null;
    private List<GameObject> pointOjbs = null;
    List<SpriteCardCtrl> scList;
    List<SpriteCardCtrl> activeCtrl;//激活的控件
    List<Vector3> tempTransfrom;
    GameObject tempCtrlObj;

    int totalCost = 0;
	/***/
	private List<PrizeSample> results;
    private bool isFont = true;
    private PrizeSample currectPs=null;//当前翻牌得到的奖励
    private int missionSid = 0;//是否需要指定的sid
    private bool isFirst = false;
    private int currectMissionSid=0;
    private int intoType = 1;
    private MessageHandle msg;
    private bool isRushCard = false;//是否洗过牌了
    private List<int> canOpenList;
  


	//初始化
    public void init(List<PrizeSample> results, int type,int missionSid) {
        if (ClmbTowerManagerment.Instance.intoTpye == 1) {
            buyButton.gameObject.SetActive(false);
        }
        intoType = type;
        this.missionSid = missionSid;
        this.RmbNum.text = UserManager.Instance.self.getRMB().ToString();
        towerNum.text = missionSid >= 151010 ? LanguageConfigManager.Instance.getLanguage("towerShowWindow23", MissionSampleManager.Instance.getMissionSampleBySid(missionSid).name.Substring(2, 2)) : LanguageConfigManager.Instance.getLanguage("towerShowWindow23", MissionSampleManager.Instance.getMissionSampleBySid(missionSid).name.Substring(2, 1));
        if (ClmbTowerManagerment.Instance.intoTpye == 2) {
            if (ClmbTowerManagerment.Instance.getBoxMAxNum() - FuBenManagerment.Instance.getTowerChapter().relotteryNum > 0) {
                openButton.disableButton(false);
            } else {
                openButton.disableButton(true);
            }
        }
        if (type == 1) {//没有开启的宝箱
            closeDoor.SetActive(true);
            canOPenNum.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow43", ClmbTowerManagerment.Instance.getBoxMAxNum() - FuBenManagerment.Instance.getTowerChapter().relotteryNum + "");
            enterButton.disableButton(true);
            oneTurnButton.disableButton(true);
            costObj.SetActive(false);
            //if (costLabel.Length > 0) {
            //    for (int i = 0; i < costLabel.Length; i++) {
            //        costLabel[i].gameObject.SetActive(false);
            //        firstConst[i].gameObject.SetActive(false);
            //    }
            //}
        } else {//有开启的宝箱
            isRushCard = true;
            intoType = 2;
            openButton.disableButton(true);
            enterButton.disableButton(true);
            updatePrize(results, 2);
            isShowOneTurnButton();
            currentActivePane.SetActive(true);
        }

    }
    public void init(List<PrizeSample> results, List<TurnSpriteReward> turnSpriteAwards, CallBack cb,int sid) {
        if (ClmbTowerManagerment.Instance.intoTpye == 1) {
            buyButton.gameObject.SetActive(false);
            openButton.disableButton(true);
        }
        this.missionSid = sid;
        towerNum.text = missionSid >= 151010 ? LanguageConfigManager.Instance.getLanguage("towerShowWindow23", MissionSampleManager.Instance.getMissionSampleBySid(missionSid).name.Substring(2, 2)) : LanguageConfigManager.Instance.getLanguage("towerShowWindow23", MissionSampleManager.Instance.getMissionSampleBySid(missionSid).name.Substring(2, 1));
        RmbNum.text = UserManager.Instance.self.getRMB().ToString();
        ClmbTowerManagerment.Instance.isCanGetAward = true;
        if (ClmbTowerManagerment.Instance.intoTpye == 2) {
            if (ClmbTowerManagerment.Instance.getBoxMAxNum() - FuBenManagerment.Instance.getTowerChapter().relotteryNum > 0)
                openButton.disableButton(false);
            else
                openButton.disableButton(true);
        }
        enterButton.disableButton(true);
        //if (ClmbTowerManagerment.Instance.intoTpye == 1) {
        //    Debug.LogError("come from here");
        //    enterButton.disableButton(true);
        //}else{//有宝箱开启
        //    enterButton.disableButton(true);
        //}
        intoType = 3;
        isRushCard = true;
        updatePrize(results, 3);
        isShowOneTurnButton();
        currentActivePane.gameObject.SetActive(true);
        updateAwards(turnSpriteAwards);
        MaskWindow.UnlockUI();
    }
    void updateAwards(List<TurnSpriteReward> awards) {
        for (int i = 0; i < results.Count;i++ ) {
            for(int j=0;j<awards.Count;j++){
                GameObject obj = currentActivePane.transform.FindChild("point_" + (i + 1).ToString()).gameObject;
                if(awards[j].index==i+1){
                    PrizeSample ps = ClmbTowerManagerment.Instance.getPrizeFromAward(awards[j]);
                    if (ps.type == PrizeType.PRIZE_CARD) {
                        Card card = CardManagerment.Instance.createCard(ps.pSid);
                        initCardButton(obj, card, StringKit.toInt(ps.num), 1);
                    } else if (ps.type == PrizeType.PRIZE_EQUIPMENT) {
                        Equip equip = EquipManagerment.Instance.createEquip(ps.pSid);
                        initEquipButton(obj, equip, StringKit.toInt(ps.num), 1);
                    } else if (ps.type == PrizeType.PRIZE_PROP) {
                        Prop prop = PropManagerment.Instance.createProp(ps.pSid, StringKit.toInt(ps.num));
                        initPropButton(obj, prop, StringKit.toInt(ps.num), 1);
                    } else if (ps.type == PrizeType.PRIZE_MAGIC_WEAPON) {
                        MagicWeapon magicW = MagicWeaponManagerment.Instance.createMagicWeapon(ps.pSid);
                        initMagicWeaponButton(obj, magicW, StringKit.toInt(ps.num), 1);
                    }
                    BoxCollider bc = obj.transform.FindChild("view").gameObject.GetComponent<BoxCollider>();
                    bc.enabled = true;
                    SpriteCardCtrl sc = obj.GetComponent<SpriteCardCtrl>();
                    sc.gameObject.GetComponent<BoxCollider>().enabled = false;
                    sc.back.depth = 0;
                    sc.quanquan.SetActive(false);
                    sc.updateCost(false);
                    GameObject cardName = obj.transform.Find("cardName").gameObject;
                    cardName.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                    cardName.SetActive(true);
                }
            }
        }
    }
	protected override void begin () {
		base.begin ();
        MaskWindow.UnlockUI();
	}
    public override void OnNetResume() {
        FuBenInfoFPort port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
        port.info(updateInfo, ChapterType.TOWER_FUBEN);
       }

   void  updateInfo()
    {
        TowerBeginAwardInfo fport = FPortManager.Instance.getFPort("TowerBeginAwardInfo") as TowerBeginAwardInfo;
        fport.access(ResumeUI);
    }
    void ResumeUI(int i) {
        isRushCard = true;
        if(ClmbTowerManagerment.Instance.getreAward()==null){
            init(null, 1,missionSid);
        }
         else{
             clearPanel();
             if(ClmbTowerManagerment.Instance.turnSpriteData.towerRewardList.Count==0){
                    init(ClmbTowerManagerment.Instance.getreAward(), 2, missionSid);
              } else {
                    init(ClmbTowerManagerment.Instance.getreAward(), ClmbTowerManagerment.Instance.turnSpriteData.towerRewardList,null,missionSid);
              }
         
         }
        isShowOneTurnButton();
    }
    void clearPanel() {
        if (results == null) return;
        for (int i = 0; i < results.Count; i++) {
            GameObject obj = currentActivePane.transform.FindChild("point_" + (i + 1).ToString()).gameObject;
            obj.transform.localEulerAngles=new Vector3(0f,0f,0f);
            GameObject viewObj = obj.transform.FindChild("view").gameObject;
            if (viewObj!=null) {
                viewObj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            }
            GameObject nameObj=obj.transform.FindChild("view").gameObject;
            if(nameObj!=null){
                nameObj.transform.localEulerAngles= new Vector3(0f, 0f, 0f);
            }
        }
    }
	public override void DoDisable () {
		base.DoDisable ();
		if (effectObj != null&&effectObj.activeSelf) {
			Transform childTrans=effectObj.transform.GetChild(0);
			childTrans.GetComponent<Animator>().enabled = false;
		}
		if (heroRoadEffectObj != null&&heroRoadEffectObj.activeSelf) {
			heroRoadEffectObj.GetComponent<Animator>().enabled = false;
		}
	}
	void correctTweenAlpha() {
		TweenAlpha tweenAlpha=gameObject.GetComponent<TweenAlpha> ();
		if (tweenAlpha != null&&tweenAlpha.to==0) {
			tweenAlpha = TweenAlpha.Begin (gameObject, 0, 1);
			tweenAlpha.from = 0;
		}
	}
	void Update ()
	{
	}
    /// <summary>
    /// 加载奖励
    /// </summary>
    /// <param name="listPrize"></param>
    private void loadPirze(List<PrizeSample> listPrize) {
		if (listPrize.Count == 0) return;
		ArrayList cards = new ArrayList ();
		ArrayList equips = new ArrayList ();
		ArrayList goods = new ArrayList ();
        ArrayList magicWeapon = new ArrayList();
        foreach (PrizeSample prize in listPrize) {
			if(prize.type==PrizeType.PRIZE_CARD) {
                 Card card = CardManagerment.Instance.createCard(prize.pSid);
				cards.Add(card);
			} else if(prize.type==PrizeType.PRIZE_EQUIPMENT) {
                Equip equip = EquipManagerment.Instance.createEquip(prize.pSid);
				equips.Add(equip);
			} else if(prize.type==PrizeType.PRIZE_PROP) {
                Prop prop = PropManagerment.Instance.createProp(prize.pSid,StringKit.toInt(prize.num));
				goods.Add(prop);
            } else if (prize.type ==PrizeType.PRIZE_MAGIC_WEAPON){
                MagicWeapon magicW = MagicWeaponManagerment.Instance.createMagicWeapon(prize.pSid);
                magicWeapon.Add(magicW);
            }
		}
		cardSort (cards);
		equipSort (equips);
		propSort (goods);
        magicSort(magicWeapon);
	}
	
	/** 获取奖品数量 */
	private int getPrizesNumber() {
		int cardNumber=0;
		if(cardList!=null)
			cardNumber=cardList.Count;
		int equipNumber=0;
		if(equipList!=null)
			equipNumber=equipList.Count;
		int propNumber=0;
		if(propList!=null)
			propNumber=propList.Count;
        int magicNum = 0;
        if (magicweaponList != null)
            magicNum = magicweaponList.Count;
        int totalNumber = cardNumber + equipNumber + propNumber + magicNum ;
		return totalNumber;
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
        if (gameObj.name == "close") {
            closeForMessage();
		}
		if(gameObj.name=="shareButton"){
			UiManager.Instance.openDialogWindow<OneKeyShareWindow>((win) => { win.initWin(); });
        } 
        else if (gameObj.name == "enterButton") {//洗牌
            enterButton.disableButton(true);
            isRushCard = true;//洗过牌了
            isShowOneTurnButton();
            openButton.disableButton(true);
            startRuch();
        } else if (gameObj.name == "openButton") {//开启宝箱
            if (closeDoor.activeSelf) {
                closeDoor.SetActive(false);
            }
            isRushCard = false;
            isShowOneTurnButton();
            if (ClmbTowerManagerment.Instance.turnSpriteData != null) {
                currentActivePane.gameObject.SetActive(false);
            }
            NGUITools.AddChild(this.gameObject, doorEffect);
            Invoke("openAward", 3f);//拿奖池
            //openAward();
            openButton.disableButton(true);
        } else if (gameObj.name == "buyButton") {//购买次数
            /*打开购买窗口*/
            openBuyWindow();
        } else if (gameObj.name == "turnCardButton") { //一键全翻
            oneTurnButton.disableButton(true);
            openButton.disableButton(true);
            costObj.SetActive(false);
            ClmbTowerManagerment.Instance.isCanGetAward = true;
            canOpenList = new List<int>();
            for (int i = 0; i < currentActivePane.transform.childCount; i++) {
                GameObject cardObject = currentActivePane.transform.FindChild("point_" + (i + 1).ToString()).gameObject;
                if (cardObject.GetComponent<BoxCollider>().enabled == false) continue;
                canOpenList.Add((i + 1));
            }
            for (int i = 0; i < ClmbTowerManagerment.Instance.turnSpriteData.towerNotTurnRewardList.Count; i++) {
                TurnSpriteReward turnReward = ClmbTowerManagerment.Instance.turnSpriteData.towerNotTurnRewardList[i];
                if (!canOpenList.Contains(turnReward.index)) {
                    ClmbTowerManagerment.Instance.turnSpriteData.towerNotTurnRewardList.Remove(turnReward);
                }
            }
            TowerGetAwardFPort fport = FPortManager.Instance.getFPort("TowerGetAwardFPort") as TowerGetAwardFPort;
            fport.access(oneTurnCard, 0, missionSid, 1);

        } else if (gameObj.name.StartsWith("point")) {
            if (ClmbTowerManagerment.Instance.intoTpye == 1) {
                openButton.disableButton(true);
            } else {
                if (ClmbTowerManagerment.Instance.getBoxMAxNum() - FuBenManagerment.Instance.getTowerChapter().relotteryNum > 0) {
                    openButton.disableButton(false);
                } else {
                    openButton.disableButton(true);
                }
            }
            enterButton.disableButton(true);
            int index = StringKit.toInt(gameObj.name.Split('_')[1]);
            int numm = ClmbTowerManagerment.Instance.turnSpriteData.towerRewardList.Count;
            if (numm >= 1) {
                int[] rmb = CommandConfigManager.Instance.getTowerMoney();
                int needRMB = numm >= rmb.Length ? rmb[rmb.Length - 1] : rmb[numm];
                if (UserManager.Instance.self.getRMB() < needRMB) {
                    UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                        win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow20", needRMB.ToString()));
                    });
                    return;
                } else {
                    TowerGetAwardFPort fportt = FPortManager.Instance.getFPort("TowerGetAwardFPort") as TowerGetAwardFPort;
                    ClmbTowerManagerment.Instance.couccentIndex = index;
                    fportt.access(updateUI, index, missionSid,0);
                }
            } else {
                TowerGetAwardFPort fport = FPortManager.Instance.getFPort("TowerGetAwardFPort") as TowerGetAwardFPort;
                ClmbTowerManagerment.Instance.couccentIndex = index;
                fport.access(updateUI, index, missionSid,0);
            }

        }
	}
    private void oneTurnCard() {
        if (canOpenList != null && canOpenList.Count >= 1) {
            ClmbTowerManagerment.Instance.couccentIndex = canOpenList[0];
            updateUI();
            MaskWindow.LockUI();
            canOpenList.RemoveAt(0);
            Invoke("oneTurnCard", 0.3f);
        } else {
            MaskWindow.UnlockUI();
            if (ClmbTowerManagerment.Instance.intoTpye == 1) openButton.disableButton(true);
            else {
                if (ClmbTowerManagerment.Instance.getBoxMAxNum() - FuBenManagerment.Instance.getTowerChapter().relotteryNum > 0) {
                    openButton.disableButton(false);
                } else {
                    openButton.disableButton(true);
                }
            }
        }
    }
    public void isShowOneTurnButton() {
        if (isRushCard) {
            if (UserManager.Instance.self.getVipLevel() >= ONEOPENLIMTVIPLEVL) {
                if (totalCost == 0) costObj.SetActive(false);
                if (totalCost > UserManager.Instance.self.getRMB() || totalCost == 0) {
                    oneTurnButton.disableButton(true);
                } else {
                    oneTurnButton.disableButton(false);
                    costObj.SetActive(true);
                }
                oneTurnButton.gameObject.SetActive(true);
                enterButton.gameObject.SetActive(false);
            }
        } else {
            oneTurnButton.gameObject.SetActive(false);
            costObj.SetActive(true);
            enterButton.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 打开购买窗口
    /// </summary>
    private void openBuyWindow() {
        int index = FuBenManagerment.Instance.getTowerChapter().relotteryBuyNum;
        //可以购买的开启次数（与VIP等级相关）
        int maxNum = CommandConfigManager.Instance.getVipBuyBoxNum()[UserManager.Instance.self.getVipLevel()];//可购买的最大次数
        int canBuyMaxNum = maxNum - index;//可购买的次数
        int[] costs = CommandConfigManager.Instance.getTowerBuyCost();
        int money = 0;
        if (index == maxNum) {//达到购买上限
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("towerShowWindow49"));
            });
        } else if (index < maxNum) {
            int canBuyNum = 0;//可以购买的次数
            if (UserManager.Instance.self.getRMB() < (index >= costs.Length?costs[costs.Length -1]:costs[index])) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("Guild_4"));
                });
            } else {
                for (int i = index; i < maxNum; i++) {//计算RMB最大可购买次数
                    if (i >= costs.Length)
                        money += costs[costs.Length -1];
                    else 
                        money += costs[i];
                    if (UserManager.Instance.self.getRMB() >= money) {
                        canBuyNum++;
                        continue;
                    }
                }
                canBuyMaxNum = canBuyNum;
                UiManager.Instance.openDialogWindow<TowerBuyWindow>((win) => {
                    TowerBuyWindow.BuyStruct buyStruct = new TowerBuyWindow.BuyStruct();
                    buyStruct.iconId = constResourcesPath.TIMES_ICONPATH;
                    buyStruct.titleTextName = LanguageConfigManager.Instance.getLanguage("towerShowWindow48");
                    buyStruct.unitPrice = index;
                    buyStruct.goodsType = "BoxTimes";
                    win.init(buyStruct, canBuyMaxNum, 1, PrizeType.PRIZE_RMB, (msg) => {
                        updateTimesAndRMB(msg);
                    });
                });
            }
        }
    }
    /// <summary>
    /// 次数购买回调
    /// </summary>
    /// <param name="mh"></param>
    void updateTimesAndRMB(MessageHandle mh) {
        if (mh.msgEvent == msg_event.dialogOK) {
            TowerLoBuyPort fport = FPortManager.Instance.getFPort("TowerLoBuyPort") as TowerLoBuyPort;
            fport.access(() => {
                ClmbTowerManagerment.Instance.relotteryBuyNum += mh.msgNum;
                RmbNum.text = UserManager.Instance.self.getRMB().ToString();
            },mh.msgNum);
            canOPenNum.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow43", ClmbTowerManagerment.Instance.getBoxMAxNum() - FuBenManagerment.Instance.getTowerChapter().relotteryNum + mh.msgNum + "");
            if (ClmbTowerManagerment.Instance.getBoxMAxNum() - FuBenManagerment.Instance.getTowerChapter().relotteryNum + mh.msgNum > 0) {
                if (ClmbTowerManagerment.Instance.turnSpriteData != null) {
                    int tempNum = ClmbTowerManagerment.Instance.turnSpriteData.towerRewardList.Count;
                    if (tempNum != 0)
                        openButton.disableButton(false);
                } else {
                    openButton.disableButton(false);
                }
            }
        }
    }
    void OneTurnBackUpdateUI() { 
        
    }
    void updateUI() {
        //其他的全部不可用
        int totalNum=results.Count;
        int tempNum = ClmbTowerManagerment.Instance.turnSpriteData.towerRewardList.Count;
        RmbNum.text = UserManager.Instance.self.getRMB().ToString();
        int[] rmb = CommandConfigManager.Instance.getTowerMoney();
        totalCost = 0;
        for (int i = tempNum; i < totalNum; i++) {
            totalCost += (i >= rmb.Length ? rmb[rmb.Length - 1] : rmb[i]);
        }
        if (totalCost == 0) {
            costObj.SetActive(false);
            oneTurnButton.disableButton(true);
        }
        if (!isRushCard) {
            if (totalCost > UserManager.Instance.self.getRMB() || totalCost == 0) {
                oneTurnButton.disableButton(true);
            } else { 
                oneTurnButton.disableButton(false);
                costObj.SetActive(true);
            }
        }
        costTotalNum.text = totalCost.ToString();
        int needRMB = tempNum >= rmb.Length ? rmb[rmb.Length - 1] : rmb[tempNum];
        PrizeSample ps = ClmbTowerManagerment.Instance.getSelectAward();
        ClmbTowerManagerment.Instance.turnSpriteData.towerNotTurnRewardList.Remove(ClmbTowerManagerment.Instance.couccentAward);
        ClmbTowerManagerment.Instance.couccentAward = null;
        if(ps==null){
            return;
        }
        //currectPs = ps;
        GameObject obj = currentActivePane.transform.FindChild("point_" + ClmbTowerManagerment.Instance.couccentIndex.ToString()).gameObject;
        if (ps.type == PrizeType.PRIZE_CARD) {
            Card card = CardManagerment.Instance.createCard(ps.pSid);
            initCardButton(obj, card, StringKit.toInt(ps.num), 3);
        } else if (ps.type == PrizeType.PRIZE_EQUIPMENT) {
            Equip equip = EquipManagerment.Instance.createEquip(ps.pSid);
            initEquipButton(obj, equip, StringKit.toInt(ps.num), 3);
        } else if (ps.type == PrizeType.PRIZE_PROP) {
            Prop prop = PropManagerment.Instance.createProp(ps.pSid,StringKit.toInt(ps.num));
            initPropButton(obj, prop, StringKit.toInt(ps.num), 3);
        } else if (ps.type == PrizeType.PRIZE_MAGIC_WEAPON) {
            MagicWeapon magicW = MagicWeaponManagerment.Instance.createMagicWeapon(ps.pSid);
            initMagicWeaponButton(obj, magicW, StringKit.toInt(ps.num), 3);
        }
        ClmbTowerManagerment.Instance.couccentIndex = -1;  
        SpriteCardCtrl ctrl = obj.GetComponent<SpriteCardCtrl>();
		ctrl.prize = ps;
        ctrl.updateCost(false);//隐藏价格（点击翻牌）
        for(int i = 0;i< currentActivePane.transform.childCount;i++){//更新价格
            GameObject gameObj = currentActivePane.transform.FindChild("point_" + (i + 1).ToString()).gameObject;
            if (gameObj.name != obj.name) {
                SpriteCardCtrl scc = gameObj.GetComponent<SpriteCardCtrl>();
                if (scc.quanquan.activeSelf) {
                    if (tempNum != 0) {
                        scc.freeCost.gameObject.SetActive(false);
                        scc.costLabel.gameObject.SetActive(true);
                        if (tempNum >= results.Count) {
                            scc.costLabel.gameObject.SetActive(false);
                        } else
                            scc.costLabel.text = LanguageConfigManager.Instance.getLanguage("prefabzc54", needRMB.ToString());
                    } else {
                        scc.freeCost.gameObject.SetActive(true);
                        scc.costLabel.gameObject.SetActive(false);
                    }
                }
            }
        }
        obj.transform.FindChild("view").gameObject.transform.localEulerAngles = new Vector3(0f,180f,0f);
        ctrl.turnToFrontt(showButton);
    }
    void showButton(GameObject obj) {
        obj.transform.FindChild("cardName").gameObject.SetActive(true);
        obj.GetComponent<SpriteCardCtrl>().gameObject.GetComponent<BoxCollider>().enabled = false;
        obj.GetComponent<SpriteCardCtrl>().updateCost(false);
        BoxCollider bc = obj.transform.FindChild("view").gameObject.GetComponent<BoxCollider>();
        bc.enabled = true;
		UiManager.Instance.createPrizeMessageLintWindow(obj.GetComponent<SpriteCardCtrl>().prize);
//        UiManager.Instance.createPrizeMessageLintWindow(currectPs);
//        currectPs = null;
        //if (ClmbTowerManagerment.Instance.needBuy()) enterButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0151");
        //else enterButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow01");
        //if (ClmbTowerManagerment.Instance.intoTpye == 1) {
        //    enterButton.disableButton(true);
        //} else {
        //    enterButton.disableButton(false);
        //}
    }
    private IEnumerator beginRuffleStep2() {
        foreach (SpriteCardCtrl ctrl in scList) {
            ctrl.gameObject.transform.FindChild("cardName").gameObject.SetActive(false);
            if (ctrl.gameObject.transform.FindChild("epicEffect") != null) ctrl.gameObject.transform.FindChild("epicEffect").gameObject.SetActive(false);
            ctrl.turnToBackk(null);
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < scList.Count; i++) {
            StartCoroutine(scList[i].moveToPosition(Vector3.zero, 0.5f));
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < scList.Count; i++) {
            StartCoroutine(movetoPosrion(scList[i].gameObject,tempTransfrom[i],0.5f));
        }
        yield return new WaitForSeconds(0.5f);
        MaskWindow.UnlockUI();

    }
    //翻牌以后回到原来的位置
    private IEnumerator movetoPosrion(GameObject obg,Vector3 v3,float time) {
        TweenPosition.Begin(obg,time,v3);
        yield return new WaitForSeconds(0.5f);
    }
    /** 卡片排序 */
    void cardSort(ArrayList cards) {
        if (cardList == null) cardList = new List<Card>();
        cardList.Clear();
        Card temp;
        for (int i = 0; i < cards.Count; i++) {
            temp = cards[i] as Card;
            cardList.Add(temp);
        }
    }
    /** 装备排序 */
    void equipSort(ArrayList equips) {
        if (equipList == null) equipList = new List<Equip>();
        equipList.Clear();
        Equip temp;
        for (int i = 0; i < equips.Count; i++) {
            temp = equips[i] as Equip;
            equipList.Add(temp);
        }
    }
    /** 道具排序 */
    void propSort(ArrayList props) {
        if (propList == null) propList = new List<Prop>();
        propList.Clear();
        Prop temp;
        for (int i = 0; i < props.Count; i++) {
            temp = props[i] as Prop;
            propList.Add(temp);
        }
    }
    void magicSort(ArrayList magics) {
        if (magicweaponList == null) magicweaponList = new List<MagicWeapon>();
        magicweaponList.Clear();
        MagicWeapon temp;
        for (int i = 0; i < magics.Count; i++) {
            temp = magics[i] as MagicWeapon;
            magicweaponList.Add(temp);
        }
    }
    /** 初始化卡片 */
    public void initCardButton(GameObject gameObj, Card card, int index, int type) {
        SpriteCardCtrl scc = gameObj.GetComponent<SpriteCardCtrl>();
        int totalNum = results.Count;
        int tempNum = ClmbTowerManagerment.Instance.turnSpriteData.towerRewardList.Count;
        int[] rmb = CommandConfigManager.Instance.getTowerMoney();
        int needRMB = tempNum >= rmb.Length ? rmb[rmb.Length - 1] : rmb[tempNum];
        GameObject obj = null;
        if (gameObj.transform.FindChild("view") != null) {
            obj = gameObj.transform.FindChild("view").gameObject;
        } else {
            obj = NGUITools.AddChild(gameObj, roleViewPrefab);
        }
        obj.name = "view";
        obj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        RoleView view = obj.GetComponent<RoleView>();
        if(type!=1&&type!=2){//正面
            updateName(gameObj, null, null, card, null, index, type);
            if (tempNum != 0) {
                scc.freeCost.gameObject.SetActive(false);
                scc.costLabel.gameObject.SetActive(true);
                if (tempNum >= results.Count) {
                    scc.costLabel.gameObject.SetActive(false);
                } else
                    scc.costLabel.text = LanguageConfigManager.Instance.getLanguage("prefabzc54", needRMB.ToString());
            } else {
                scc.freeCost.gameObject.SetActive(true);
                scc.costLabel.gameObject.SetActive(false);
            }
        }
        if (cardList.Count == 1) {
            view.linkEffectPoint();
        } else {
            if (card.getQualityId() >= QualityType.EPIC) {
                GameObject objEffect= NGUITools.AddChild(gameObj, qualityEffectPrefab);
                objEffect.name = "epicEffect";
            }
        }
        view.showType = CardBookWindow.SHOW;
        view.hideInBattle = false;
        BoxCollider vb = view.gameObject.GetComponent<BoxCollider>();
        vb.enabled = false;
        view.init(card, this, (roleView) => {
            TweenAlpha lname = TweenAlpha.Begin(gameObject, 1f, 0);
            lname.from = 1;
            CardBookWindow.Show(cardList, index, view.showType, (() => {
                lname = TweenAlpha.Begin(gameObject, 1.5f, 1);
                lname.from = 0;
            }));
        });
        UITexture ut = scc.back;
        if(type==1){
            ut.depth = 0;
            scc.quanquan.SetActive(false);
            scc.updateCost(false);
            gameObj.GetComponent<BoxCollider>().enabled=false;
        }else if(type==2){//反面
            ut.depth = 300;
            scc.quanquan.SetActive(true);
            gameObj.GetComponent<BoxCollider>().enabled = true;
            if (tempNum != 0) {
                scc.freeCost.gameObject.SetActive(false);
                scc.costLabel.gameObject.SetActive(true);
                if (tempNum >= results.Count) {
                    scc.costLabel.gameObject.SetActive(false);
                } else
                    scc.costLabel.text = LanguageConfigManager.Instance.getLanguage("prefabzc54", needRMB.ToString());
            } else {
                scc.freeCost.gameObject.SetActive(true);
                scc.costLabel.gameObject.SetActive(false);
            }
        }
        ResourcesManager.Instance.LoadAssetBundleTexture("texture/shopList/kapai", scc.back);

    }
    /** 初始化秘宝 */
    public void initMagicWeaponButton(GameObject gameObj, MagicWeapon magicWeap, int index, int type) {
        updateGoods(gameObj, null, magicWeap, null, index,type);
        updateName(gameObj, null, magicWeap, null, null, index,type);
    }
    /** 初始化装备 */
    public void initEquipButton(GameObject gameObj, Equip equip, int index, int type) {
        updateGoods(gameObj, null, null, equip, index,type);
        updateName(gameObj, null, null, null, equip, index, type);
    }
    /** 初始化道具 */
    public void initPropButton(GameObject gameObj, Prop prop, int index, int type) {
        updateGoods(gameObj, prop, null, null, index,type);
        updateName(gameObj, prop, null, null, null, index, type);
    }
    void updateGoods(GameObject gameObj, Prop prop, MagicWeapon magicWeap, Equip eq, int index,int type) {//最后一个参数正面反面true 正面
        SpriteCardCtrl scc = gameObj.GetComponent<SpriteCardCtrl>();
        int totalNum = results.Count;
        int tempNum = ClmbTowerManagerment.Instance.turnSpriteData.towerRewardList.Count;
        int[] rmb = CommandConfigManager.Instance.getTowerMoney();
        int needRMB = tempNum >= rmb.Length ? rmb[rmb.Length - 1] : rmb[tempNum];
        GameObject obj=null;
        if (gameObj.transform.FindChild("view") != null) {
            obj = gameObj.transform.FindChild("view").gameObject;
        } else {
            obj = NGUITools.AddChild(gameObj, goodsViewPrefab);
        }
        obj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        obj.name = "view";
        GoodsView goods = obj.GetComponent<GoodsView>();
        goods.fatherWindow = this;
        if (magicWeap != null) {
            goods.init(magicWeap);
            obj.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }else if(prop!=null){
            goods.init(prop, 0);
            obj.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        } else if (eq != null) {
            goods.init(eq, 0);
            obj.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }
        BoxCollider bcc = goods.GetComponent<BoxCollider>();//让物品不可以点
        ResourcesManager.Instance.LoadAssetBundleTexture("texture/shopList/kapai", scc.back);
        BoxCollider bc = gameObj.GetComponent<BoxCollider>();
        UITexture ut = scc.back;
        if(type==1){
            bcc.enabled = true;
            ut.depth = 0;
            scc.quanquan.SetActive(false);
            scc.updateCost(false);
            bc.enabled = false;
            gameObj.transform.localEulerAngles = new Vector3(0f,0f,0f);
        }else if(type==2){
            bcc.enabled = false;
            ut.depth = 300;
            scc.quanquan.SetActive(true);
            if (tempNum != 0) {
                scc.freeCost.gameObject.SetActive(false);
                scc.costLabel.gameObject.SetActive(true);
                if (tempNum >= results.Count) {
                    scc.costLabel.gameObject.SetActive(false);
                } else
                    scc.costLabel.text = LanguageConfigManager.Instance.getLanguage("prefabzc54", needRMB.ToString());
            } else {
                scc.freeCost.gameObject.SetActive(true);
                scc.costLabel.gameObject.SetActive(false);
            }
            bc.enabled = true;
        }else if(type==3){
            bcc.enabled = false;
            ut.depth = 300;
            scc.quanquan.SetActive(true);
            if (tempNum != 0) {
                scc.freeCost.gameObject.SetActive(false);
                scc.costLabel.gameObject.SetActive(true);
                if (tempNum >= results.Count) {
                    scc.costLabel.gameObject.SetActive(false);
                } else
                    scc.costLabel.text = LanguageConfigManager.Instance.getLanguage("prefabzc54", needRMB.ToString());
            } else {
                scc.freeCost.gameObject.SetActive(true);
                scc.costLabel.gameObject.SetActive(false);
            }
            bc.enabled = true;
        }
        bc.size = new Vector3(ut.width, ut.height, 0f);
        if (magicWeap != null) {
            goods.onClickCallback = () => {
                UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                    win.init(magicWeap, MagicWeaponType.FORM_OTHER);
                });
            };
        } else if (prop != null) {
            goods.onClickCallback = () => {
                UiManager.Instance.openDialogWindow<PropAttrWindow>((win) => {
                    win.Initialize(prop);
                });
                
            };
        } else if (eq != null) {
            goods.onClickCallback = () => {
                TweenAlpha lname = TweenAlpha.Begin(gameObject, 1f, 0);
                lname.from = 1;
                UiManager.Instance.openWindow<EquipAttrWindow>((winEquip) => {
                    winEquip.Initialize(eq, EquipAttrWindow.OTHER, () => {
                        lname = TweenAlpha.Begin(gameObject, 1.5f, 1);
                        lname.from = 0;
                    });
                });
            };
        } 
    }
    void updateName(GameObject gameObj,Prop prop,MagicWeapon mw,Card card,Equip eq,int index,int type){
        GameObject card_name = gameObj.transform.FindChild("cardName").gameObject;
        if (prop != null) {
            string color = QualityManagerment.getQualityColor(prop.getQualityId());
            card_name.GetComponent<UILabel>().text = color + prop.getName() + "[-]" + "X" + prop.getNum();
        }else if(mw!=null){
            string color = QualityManagerment.getQualityColor(mw.getMagicWeaponQuality());
            card_name.GetComponent<UILabel>().text = color + mw.getName() + "[-]" + "X" + 1;
        }else if(eq!=null){
            string color = QualityManagerment.getQualityColor(eq.getQualityId());
            card_name.GetComponent<UILabel>().text = color + eq.getName() + "[-]" + "X" + 1;
        }else if(card!=null){
            string color = QualityManagerment.getQualityColor(card.getQualityId());
            if (card.getQualityId() == QualityType.COMMON) {
                card_name.GetComponent<UILabel>().text = card.getName() + "X" + 1;
            } else {
                card_name.GetComponent<UILabel>().text = color + card.getName() + "X" + 1;
            }
        }
        card_name.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        card_name.transform.localPosition = new Vector3(0, -80, 0);
        if(card!=null){
            card_name.transform.localScale = new Vector3(1.3f, 1.3f, 1);
            card_name.transform.localPosition = new Vector3(0, -131, 0);
        }
        if (type==1) {
            card_name.SetActive(true);
            card_name.gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
        else {
            card_name.gameObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            card_name.SetActive(false);
        } 
    }
    /// <summary>
    /// 开启宝箱 就是拿奖池
    /// </summary>
    void openAward() {
        //Chapter towerChapter = FuBenManagerment.Instance.getTowerChapter();
        //if (ClmbTowerManagerment.Instance.intoTpye==2&&ClmbTowerManagerment.Instance.getBoxMAxNum()  <= towerChapter.relotteryNum) {
        //    if (!ClmbTowerManagerment.Instance.checkCanBeBuy()) {
        //        openButton.disableButton(true);
        //        return;
        //    }
        //} else {
        //    updateData();
        //}
        updateData();
        
    }
    void updateData() {
        if (ClmbTowerManagerment.Instance.turnSpriteData != null) {
            ClmbTowerManagerment.Instance.turnSpriteData = null;
            TowerAwardClearFPort ffport = FPortManager.Instance.getFPort("TowerAwardClearFPort") as TowerAwardClearFPort;
            ffport.accessGiveUp(() => {
                open();
            }, -1);
        } else {
            open();
        }
    }
    void open() {
        Chapter towerChapter = FuBenManagerment.Instance.getTowerChapter();
        //和后台通信开启宝箱
        TowerAwardInfoFPort fport = FPortManager.Instance.getFPort("TowerAwardInfoFPort") as TowerAwardInfoFPort;
        fport.access(() => {
            //宝箱成功开启
            enterButton.disableButton(false);//激活洗牌按钮
            Chapter towChapter = FuBenManagerment.Instance.getTowerChapter();
            if (!FuBenManagerment.Instance.isFistIntoAward(ClmbTowerManagerment.Instance.boxMissionSid)) {
                towChapter.relotteryNum += 1;
                ClmbTowerManagerment.Instance.relotteryNum += 1;
                canOPenNum.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow43", ClmbTowerManagerment.Instance.getBoxMAxNum() - towChapter.relotteryNum + "");
            }

            //if (ClmbTowerManagerment.Instance.getBoxMAxNum() - FuBenManagerment.Instance.getTowerChapter().relotteryNum > 0) {
            //    openButton.disableButton(false);
            //} else {
            //    openButton.disableButton(true);
            //}
            if (currentActivePane != null) currentActivePane.gameObject.SetActive(false);
            ClmbTowerManagerment.Instance.couccentAward = null;
            ClmbTowerManagerment.Instance.isCanGetAward = false;
            initPool();
        }, 0, missionSid);
    }
    /// <summary>
    /// 点击关闭按键时候的提示
    /// </summary>
    void closeForMessage() {
        if (ClmbTowerManagerment.Instance.intoTpye==1&&!ClmbTowerManagerment.Instance.isCanGetAward) {
            UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("s0093"),
                    LanguageConfigManager.Instance.getLanguage("towerShowWindow05"), (msgHandle) => {
                        if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {//放弃开宝箱机会
                            ClmbTowerManagerment.Instance.giveUpAward();
                            finishWindow();
                            return;
                        } else if (msgHandle.buttonID == MessageHandle.BUTTON_LEFT) {
                            MaskWindow.UnlockUI();
                            return;
                        }
                    });
            });
        } else if (ClmbTowerManagerment.Instance.intoTpye==2 && !ClmbTowerManagerment.Instance.isCanGetAward) {//从外面进去的 没有放弃一说
            finishWindow();
        } 
        else {//可以领奖
            ClmbTowerManagerment.Instance.isCanGetAward = false;
            ClmbTowerManagerment.Instance.turnSpriteData = null;
            TowerAwardClearFPort ffport = FPortManager.Instance.getFPort("TowerAwardClearFPort") as TowerAwardClearFPort;
            ffport.accessGiveUp(() => {
                finishWindow();
                if (ClmbTowerManagerment.Instance.intoTpye == 1) ClmbTowerManagerment.Instance.getAwardSuccessCallBack();
            }, -1);
           
        }
    }
    void initPool() {
        //第一步把要的奖励给弄出来
        updatePrize(ClmbTowerManagerment.Instance.getreAward(),1);
        currentActivePane.gameObject.SetActive(true);
        MaskWindow.UnlockUI();
        enterButton.disableButton(false);
        if (ClmbTowerManagerment.Instance.turnSpriteData != null) {
            int tempNum = ClmbTowerManagerment.Instance.turnSpriteData.towerRewardList.Count;
            if (totalCost > UserManager.Instance.self.getRMB()) {//RMB不够
                oneTurnButton.disableButton(true);
            } else {
                if (tempNum != results.Count) {
                    oneTurnButton.disableButton(false);
                    costObj.SetActive(true);
                } else oneTurnButton.disableButton(true);
            }
        } else {
            oneTurnButton.disableButton(true);
            costObj.SetActive(false);
        }
    }
    /// <summary>
    /// 把奖励放在屏幕上
    /// </summary>
    /// <param name="ps"></param>
    void updatePrize(List<PrizeSample> ps,int type) {
        this.results = ps;
        costTotalNum.text = "";
        totalCost = 0;
        int tempNum = ClmbTowerManagerment.Instance.turnSpriteData.towerRewardList.Count;//已经翻过的次数
        int[] rmb = CommandConfigManager.Instance.getTowerMoney();
        for (int i = tempNum; i < results.Count; i++) {
            totalCost += (i >= rmb.Length ? rmb[rmb.Length - 1] : rmb[i]);
        }
        costTotalNum.text = totalCost.ToString();
        Chapter towChapter = FuBenManagerment.Instance.getTowerChapter();
        canOPenNum.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow43", ClmbTowerManagerment.Instance.getBoxMAxNum() - towChapter.relotteryNum + "");
        //canSelectPai.gameObject.SetActive(true);
       // canSelectPai.text = LanguageConfigManager.Instance.getLanguage("notice39", (results.Count - tempNum)+"", results.Count+"");
        loadPirze(ps);//各种list里面有东西了
        setActivePanel();
        int currentIndex = 0;
        if (activeCtrl!=null)
        activeCtrl.Clear();
        activeCtrl = new List<SpriteCardCtrl>();
        currentIndex=initCard(currentIndex, type);//1就是初始化起来
        currentIndex = initProp(currentIndex, type);
        currentIndex = initEquip(currentIndex, type);
        currentIndex = initMagicWeapon(currentIndex, type);
        
    }
    /// <summary>
    /// 设置活动的panel 
    /// </summary>
    /// <returns></returns>
    private bool setActivePanel() {
        int totalNumber = getPrizesNumber();
        if (totalNumber <= 0) return false;
        int index = totalNumber - 1;
        if (index > panes.Length - 1) index = panes.Length - 1;
        if (index < 0) index = 0;
        GameObject pane = panes[index];
        currentActivePane = pane;
        return true;
    }
    private int initCard(int index,int type){
        if (cardList != null && cardList.Count > 0) {
            Transform childTrans;
            Card card;
            Transform trans = currentActivePane.transform;
            for (int i = 0; i < cardList.Count; i++) {
                card = cardList[i] as Card;
                index++;
                childTrans = trans.FindChild("point_" + index);
                activeCtrl.Add(childTrans.gameObject.GetComponent<SpriteCardCtrl>());
                initCardButton(childTrans.gameObject, card, i, type);
            }
        }
        return index;
    }
    private int initProp(int index,int type) {
        if (propList != null && propList.Count > 0) {
            Transform childTrans;
            Prop prop;
            Transform trans = currentActivePane.transform;
            for (int i = 0; i < propList.Count; i++) {
                prop = propList[i] as Prop;
                index++;
                childTrans = trans.FindChild("point_" + index);
                activeCtrl.Add(childTrans.gameObject.GetComponent<SpriteCardCtrl>());
                initPropButton(childTrans.gameObject, prop, i, type);
            }
        }
        return index;
    }
    private int initEquip(int index,int type ) {
        if (equipList != null && equipList.Count > 0) {
            Transform childTrans;
            Equip equip;
            Transform trans = currentActivePane.transform;
            for (int i = 0; i < equipList.Count; i++) {
                equip = equipList[i] as Equip;
                index++;
                childTrans = trans.FindChild("point_" + index);
                activeCtrl.Add(childTrans.gameObject.GetComponent<SpriteCardCtrl>());
                initEquipButton(childTrans.gameObject, equip, i, type);
            }
        }
        return index;
    }
    private int initMagicWeapon(int index,int type) {
        if (magicweaponList != null && magicweaponList.Count > 0) {
            Transform childTrans;
            MagicWeapon mw;
            Transform trans = currentActivePane.transform;
            for (int i = 0; i < magicweaponList.Count; i++) {
                mw = magicweaponList[i] as MagicWeapon;
                index++;
                childTrans = trans.FindChild("point_" + index);
                activeCtrl.Add(childTrans.gameObject.GetComponent<SpriteCardCtrl>());
                initMagicWeaponButton(childTrans.gameObject, mw, i, type);
            }
        }
        return index;
    }
    private void startRuch() {
        for (int i = 0; i < activeCtrl.Count;i++ ) {
            SpriteCardCtrl scc = activeCtrl[i];
            Transform tr=scc.gameObject.transform;
            tr.FindChild("cardName").gameObject.SetActive(false);
            tr.FindChild("view").gameObject.GetComponent<BoxCollider>().enabled = false;
            scc.oldPostion =new Vector3(tr.localPosition.x,tr.localPosition.y,0f);
            scc.turnToBackk(finishToBack);
        }
    }
    private void finishToBack(GameObject obj){
        StartCoroutine(beginRuffleStep(obj));
    }
    private IEnumerator beginRuffleStep(GameObject obj) {
        SpriteCardCtrl scc = obj.GetComponent<SpriteCardCtrl>();
        scc.freeCost.gameObject.transform.localRotation = new Quaternion(0, 180, 0, 0);
        scc.costLabel.gameObject.transform.localRotation = new Quaternion(0, 180, 0, 0);
        StartCoroutine(scc.moveToPosition(Vector3.zero, 0.5f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(movetoPosrion(obj.gameObject, scc.oldPostion, 0.5f));
        yield return new WaitForSeconds(0.5f);
        obj.gameObject.GetComponent<BoxCollider>().enabled = true;
        scc.freeCost.gameObject.SetActive(true);
        //if (ClmbTowerManagerment.Instance.needBuy()) enterButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0151");
        //else enterButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow01");
        StartCoroutine(Utils.DelayRun(() => {
            MaskWindow.UnlockUI();
        }, 0.5f));
    }
}
