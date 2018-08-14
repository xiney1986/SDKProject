using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 恶魔挑战卡片选择界面
 * */
public class CardSelectWindow : WindowBase
{
	public ContentCardChoose content;
	public Card  instandCard;
    public ButtonBase fightButton;
	int showType;
	 SortCondition sc ;
	ArrayList list = null;
	int storageVersion=-1;
    public bool isSelect = false;
    public Card selectCard = null;
    public Card beast = null;
    public UITexture beastTexture;
    public UISprite upArrow;
    public UISprite downArrow;
    public UILabel descLabel;
    ArrayList cardList = new ArrayList();

    Timer timer;
	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
	}
	protected override void begin ()
	{ 
		base.begin ();
        sc = SortConditionManagerment.Instance.getConditionsByKey(SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
		if (content.nodeList == null || !isAwakeformHide || StorageManagerment.Instance.RoleStorageVersion!=storageVersion) {
            sc.siftConditionArr = null;
            sc.sortCondition = new Condition (SortType.SORT);
		    sc.sortCondition.conditions.Add (SortType.SORT_LEVELDOWN);
			updateContent();
	
		}

		if (SortManagerment.Instance.isCardChooseModifyed) {
			SortManagerment.Instance.isCardChooseModifyed=false;
			updateContent ();
		}
        if (!isAwakeformHide) {
            beast = StorageManagerment.Instance.getBeast(ArmyManager.Instance.DeepClone(ArmyManager.Instance.getArmy(1)).beastid);
            if (beast != null && AttackBossOneOnOneManager.Instance.choosedBeast == null) {
                AttackBossOneOnOneManager.Instance.choosedBeast = beast;
                updateUI(beast);
            } else {
                updateUI(AttackBossOneOnOneManager.Instance.choosedBeast);
            }
        }

        timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        timer.addOnTimer(content.updateArrow);
        timer.start();
		MaskWindow.UnlockUI ();
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		AttackBossOneOnOneManager.Instance.selectedCard = null;
		finishWindow();
	}

    public void updateUI( Card beast)
    {
        descLabel.text = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_424");
        if(beast != null)
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH, beast, beastTexture);
        if (cardList == null || cardList.Count <= 0) {
            fightButton.disableButton(true);
        } else
            fightButton.disableButton(false);
    }
	public void updateContent()
	{
		storageVersion=StorageManagerment.Instance.RoleStorageVersion;
        list = SortManagerment.Instance.cardSort(StorageManagerment.Instance.getAllRoleByNotToEat (), sc, CardStateType.STATE_USING);
        List<string> tempList = AttackBossOneOnOneManager.Instance.getUsedCardList();
        for (int i = 0; i < list.Count; i++) {
            Card card = list[i] as Card;
            if (card.getQualityId() > QualityType.COMMON && !tempList.Contains(card.uid)) {
                cardList.Add(card);
                continue;
            }
        }
        content.reLoad(cardList);
	}

	public void Initialize (int type )
	{ 
		showType = type;
        list = null;
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
        timer.stop();
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close"){
            finishWindow();
		}
        if (gameObj.name == "buttonFight") {
            Prop prop = StorageManagerment.Instance.getProp(CommandConfigManager.Instance.getHuiJiMoneySid());
//            if (prop != null && prop.getNum() >= CommandConfigManager.Instance.getMaxNum()) {
//                UiManager.Instance.openDialogWindow<TextTipWindow>((win) => {
//                    win.init(LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_021"), 1.5f);
//                });
//                return;
//            }
            if (prop != null && prop.getNum() >= CommandConfigManager.Instance.getMaxNum()) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_423"));
                });
                return;
            }
            BossAttackFPort fport = FPortManager.Instance.getFPort("BossAttackFPort") as BossAttackFPort;
            if (AttackBossOneOnOneManager.Instance.choosedBeast == null) {
                fport.access(CommandConfigManager.Instance.getBossFightSid(), AttackBossOneOnOneManager.Instance.selectedCard.uid, "0", () => {
                    MaskWindow.instance.setServerReportWait(true);
                    GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleNoSwitchWindow;
                });
            } else {
                fport.access(CommandConfigManager.Instance.getBossFightSid(), AttackBossOneOnOneManager.Instance.selectedCard.uid, AttackBossOneOnOneManager.Instance.choosedBeast.uid, () => {
                    MaskWindow.instance.setServerReportWait(true);
                    GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattleNoSwitchWindow;
                });
            }
        }
        if (gameObj.name == "buttonSelectBeast") {
            ArrayList beastList = StorageManagerment.Instance.getAllBeast();
            if (beastList.Count <= 0) {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_016"));
                });
                return;
            }
            if (beast != null) {
                UiManager.Instance.openWindow<BeastAttrWindow>((win) => {
                    win.Initialize(beast, BeastAttrWindow.FUBEN);
                });
            } else {
                UiManager.Instance.openWindow<BeastAttrWindow>((win) => {
                    win.Initialize(beastList[0] as Card, BeastAttrWindow.FUBEN);
                });
            }
        }

	}
	
}
