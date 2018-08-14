using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IncreaseWayWindow : WindowBase
{
	/** 女神召唤 */
	public const string TYPE_BEAST_SUMMON = "BeastSummon";
	/** 女神进化 */
	public const string TYPE_BEAST_EVOLUTION = "BeastEvolution";
	/** 主卡进化 */
	public const string TYPE_MAINCARD_EVOLUTION = "MainEvolution";
	/** 主卡突破 */
	public const string TYPE_MAINCARD_SURMOUNT = "MainSurmount";
	/** 卡片进化 */
	public const string TYPE_CARD_EVOLUTION = "GeneralCardEvolution";
	/** 爵位 */
	public const string TYPE_KNIGHTHOOD = "Knighthood";
	/** 英雄之章 */
	public const string TYPE_HEROROAD = "HeroRoad";
	/** 队伍可穿装备 */
	public const string TYPE_TEAMEQUIP = "TeamEquip";
	/** 圣器 */
	public const string TYPE_HALLOWS = "Hallows";
	/** 星盘 */
	public const string TYPE_STAR = "Star";
	/** 新手礼包 */
	public const string TYPE_NEWGOODS = "NewGoods";
    /** 卡牌训练*/
    public const string TYPE_CARDTRAINING = "CardTraining";
	/** 骑术学习*/
	public const string TYPE_MOUNTS_PRACTICE = "MountsPractice";
	//**分享*/
	public const string TYPE_SHARE = "Share";
    //**天使*/
    public const string TYPE_ANGEL = "DefendingAngel";
	/** 诸神之战*/
    public const string TYPE_GODSWAR = "godsWar";

	/**卡牌兑换**/
	//public const string TYPE_CARDEXCHAGE = "CardExchange";
	public ButtonBase[] showButtons;
	public UILabel title_tips;
	public UILabel timeLabel;
	public UILabel tips_label;
	[HideInInspector]
	public int SpriteSum = 0;
	/// <summary>
	/// 诸神战入口banner
	/// </summary>
	public ButtonBase godsWarBanner;
	protected override void begin ()
	{
		base.begin ();
		updateUI ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}
	
	protected override void DoEnable ()
	{
		base.DoEnable ();
		IncreaseManagerment.Instance.increaseWayWindow = this;
	}
	
	public void updateUI ()
	{
		showLabel ();
	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
		IncreaseManagerment.Instance.increaseWayWindow = null;
	}
	public void showShare () {
		showButtons[16].gameObject.SetActive (false);
		if (FriendsShareManagerment.Instance.getShareInfo () != null)
			showButtons[16].gameObject.SetActive (true);
		else if (FriendsShareManagerment.Instance.getPraiseNum () > 0 && FriendsShareManagerment.Instance.getPraiseInfo () != null)
			showButtons[16].gameObject.SetActive (true);
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
		switch (gameObj.name) {
		case "button_close":
			finishWindow ();
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
			break;
		case TYPE_BEAST_SUMMON:
//			BeastEvolve summonBeast = IncreaseManagerment.Instance.getBeastSummon ();
//			if (summonBeast == null) {
//				MaskWindow.UnlockUI ();
//				return;
//			}
			closeWin (() => {
				UiManager.Instance.openWindow<GoddessWindow> ();
//				UiManager.Instance.openWindow<BeastSummonWindow> ((win) => {
//					win.Initialize (summonBeast);
//				});
			});
			break;
			
		case TYPE_BEAST_EVOLUTION:
			List<BeastEvolve> beastEvoList = IncreaseManagerment.Instance.getBeastEvolutionList ();
			closeWin (() => {
				UiManager.Instance.openWindow<BeastAttrWindow> ((win) => {
					win.Initialize (beastEvoList,6,beastEvoList[0].getBeast());
				});
			});
			break;
			
		case TYPE_MAINCARD_EVOLUTION:
			closeWin (() => {
				UiManager.Instance.openWindow<MainCardEvolutionWindow> ();
			});
			break;
			
		case TYPE_MAINCARD_SURMOUNT:
			closeWin (() => {
				UiManager.Instance.openWindow<MainCardSurmountWindow> ();
			});
			break;
			
		case TYPE_CARD_EVOLUTION:
			List<Card> cardList = IncreaseManagerment.Instance.getCardEvolutionList ();
			if (cardList.Count == 0)
				return;
			Card mainCard = cardList [0];
			IntensifyCardManager.Instance.isFromIncrease = true;
			Card foodCard = EvolutionManagerment.Instance.getFoodCardForEvo (mainCard);
			Prop foodProp = EvolutionManagerment.Instance.getCardByQuilty (mainCard);
			if ((foodCard != null && foodProp != null) || (foodCard != null && foodProp == null)) {
				IntensifyCardManager.Instance.setFoodCard (foodCard);
			} else if (foodCard == null && foodProp != null)
				IntensifyCardManager.Instance.setFoodProp (foodProp);
			
			closeWin (() => {
				IntensifyCardManager.Instance.intoIntensify (IntensifyCardManager.INTENSIFY_CARD_EVOLUTION, mainCard, null);
			});
			break;
			
		case TYPE_KNIGHTHOOD:
			closeWin (() => {
				UiManager.Instance.openWindow<HonorWindow> ((win) => {
					win.updateInfo ();
				});
			});
			
			break;
			
		case TYPE_HEROROAD:
			FuBenGetCurrentFPort port = FPortManager.Instance.getFPort ("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
			port.getInfo (OnHeroRoadGetCurrentFubenBack);
			break;
			
		case TYPE_TEAMEQUIP:
			closeWin (() => {
				CardBookWindow.Show (IncreaseManagerment.Instance.getAllArmyCards (),IncreaseManagerment.Instance.getTeamCardCanPutOnEquip(), CardBookWindow.VIEW, null);
			});
			break;
			
		case TYPE_HALLOWS:
			closeWin (() => {
				UiManager.Instance.openWindow<IntensifyHallowsWindow> ((win) => {
					win.inSideType = IntensifyHallowsWindow.TYPE_NEED;
				});
			});
			break;
			
		case TYPE_STAR:
			closeWin (() => {
				UiManager.Instance.openWindow<GoddessAstrolabeMainWindow> ();
			});
			break;

		case TYPE_NEWGOODS:
			closeWin (() => {
				UiManager.Instance.openWindow<StoreWindow> ();
			});
			break;
        case TYPE_CARDTRAINING :
            closeWin(() => {
                UiManager.Instance.openWindow<CardTrainingWindow>();
            });
            break;
//		case TYPE_CARDEXCHAGE:
//			closeWin(() => {
//				UiManager.Instance.openWindow<CardStoreWindow>();
//			});
//			break;
		case TYPE_MOUNTS_PRACTICE:
			closeWin(() => {
				UiManager.Instance.openWindow<MountsPracticeWindow>();
			});
			break;
        case TYPE_ANGEL:
            closeWin(() =>
            {
                UiManager.Instance.openWindow<VipWindow>();
            });
            break;
		case TYPE_SHARE:
			closeWin(() => {
				UiManager.Instance.openWindow<FriendsShareWindow> ((win) => {
					if (FriendsShareManagerment.Instance.getPraiseInfo () != null && !FriendsShareManagerment.Instance.getPraiseNumIsFull ())
						win.initWin (true, 1);
					else
						win.initWin (true, 0);
				});
			});
			break;
		case TYPE_GODSWAR:
			closeWin(()=>{
				string currentState = GodsWarManagerment.Instance.getGodsWarStateInfo();
				if(currentState!=GodsWarManagerment.STATE_NOTOPEN&&currentState!=GodsWarManagerment.STATE_SERVER_BUSY){//开启
					//没有资格且处于小组赛
					if(currentState == GodsWarManagerment.STATE_NOT_ZIGE_GROUP){
						UiManager.Instance.openWindow<GodsWarPreparWindow>();
					}
					//有资格且处于小组赛
					if(currentState == GodsWarManagerment.STATE_HAVE_ZIGE_GROUP)
					{
						UiManager.Instance.openWindow<GodsWarGroupStageWindow>();
					} 
					//处于淘汰赛（不管有没有资格）
					if(currentState == GodsWarManagerment.STATE_HAVE_ZIGE_TAOTAI||currentState == GodsWarManagerment.STATE_NOT_ZIGE_TAOTAI){
						UiManager.Instance.openWindow<GodsWarFinalWindow>((win)=>{
							if(GodsWarManagerment.Instance.taoTaiZige)
								win.setBigidAndYuming();
							else
								GodsWarManagerment.Instance.type = GodsWarManagerment.Instance.big_id;
						});
					}
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
					//处于清理分组界面直接进入准备界面
					if (currentState == GodsWarManagerment.STATE_PREPARE) {
						UiManager.Instance.openWindow<GodsWarPreparWindow>();
					}
				}
				else {//没有开启
					UiManager.Instance.openWindow<GodsWarPreparWindow>();
				}
			});
			break;
		}
		
	}
	
	private void OnHeroRoadGetCurrentFubenBack (bool b)
	{
		if (b) { 
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("s0388"));
		} else {
			closeWin (() => {
				UiManager.Instance.openWindow<HeroRoadWindow> ();
			});
		}
	}

	/// <summary>
	/// 初始化诸神战banner信息
	/// </summary>
	public void initGodsWarBannerInfo()
	{
		//GodsWarManagerment.Instance.getGodsWarStateInfo(()=>{ //获取状态信息改到主窗口里获取
		int stateOfDay = GodsWarManagerment.Instance.getWeekOfDayState();
		string currentState = GodsWarManagerment.Instance.getGodsWarStateInfo();

		if(currentState == GodsWarManagerment.STATE_NOTOPEN){//没有开放
			tips_label.text = LanguageConfigManager.Instance.getLanguage("godsWar_108");
			if(stateOfDay==1)
			{
				tips_label.text = LanguageConfigManager.Instance.getLanguage("godsWar_21");
			}
			tips_label.gameObject.transform.localPosition = new Vector3(tips_label.gameObject.transform.localPosition.x-15,tips_label.gameObject.transform.localPosition.y,0);
		}//神战开启

		//没有资格
		else if(currentState.StartsWith("not_zige")){
			tips_label.text = LanguageConfigManager.Instance.getLanguage("godsWar_89");
			if(currentState == GodsWarManagerment.STATE_NOT_ZIGE_FINAL||currentState == GodsWarManagerment.STATE_NOT_ZIGE_TAOTAI){//没有资格且处于决赛中或淘汰赛
				tips_label.text = LanguageConfigManager.Instance.getLanguage("godsWar_86");
			}
		}

		//有资格
		else if(currentState == GodsWarManagerment.STATE_HAVE_ZIGE_GROUP){//有资格并且处于小组赛
			FPortManager.Instance.getFPort<GodsWarGroupStageFPort> ().access (()=>{
				tips_label.text = LanguageConfigManager.Instance.getLanguage("godsWar_23",GodsWarManagerment.Instance.getChallengeCount().ToString(),GodsWarManagerment.Instance.maxChallengeCount.ToString());
			});
		}
		else if(currentState == GodsWarManagerment.STATE_HAVE_ZIGE_TAOTAI||currentState== GodsWarManagerment.STATE_HAVE_ZIGE_FINAL){//有资格并且处于淘汰或者决赛
			tips_label.text = LanguageConfigManager.Instance.getLanguage("godsWar_86");
		}

		//处于清理分组阶段时，只弹出准备界面
		if (currentState == GodsWarManagerment.STATE_PREPARE) {
			tips_label.text = LanguageConfigManager.Instance.getLanguage("godsWar_90");
		}
		//});
	}
	private void showLabel ()
	{
		int num = 0;
		title_tips.text=LanguageConfigManager.Instance.getLanguage("s0422");  
		if (GodsWarManagerment.Instance.isOnlineDay30())
		{
			godsWarBanner.gameObject.SetActive(true);
			timeLabel.text = GodsWarManagerment.Instance.getStateInfo();
			initGodsWarBannerInfo();
			num++;
			num++;
		}
		else{
			godsWarBanner.gameObject.SetActive(false);
		}
        if (FriendsShareManagerment.Instance.getShareInfo() != null || (FriendsShareManagerment.Instance.getPraiseNum() > 0 && FriendsShareManagerment.Instance.getPraiseInfo() != null))
        {
            showButtons[num].gameObject.SetActive(true);
            showButtons[num].textLabel.text = LanguageConfigManager.Instance.getLanguage("ShareNew");
            showButtons[num].name = TYPE_SHARE;
            num++;
        }
        if (UserManager.Instance.self.getVipLevel() > 5 && PlayerPrefs.GetString(PlayerPrefsComm.ANGEL_USER_NAME + UserManager.Instance.self.uid) == "not") {
            showButtons[num].gameObject.SetActive(true);
            showButtons[num].textLabel.text = LanguageConfigManager.Instance.getLanguage("Angel04");
            showButtons[num].name = TYPE_ANGEL;
            num++;
        }
		//新手礼包
		if(GuideManager.Instance.isMoreThanStep(GuideGlobal.NEWFUNSHOW02)) {
			if (IncreaseManagerment.Instance.isHaveNewGoodsCanOpen ()) {
				showButtons [num].gameObject.SetActive (true);
				showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way12");
				showButtons [num].name = TYPE_NEWGOODS;
				num++;
			}
		}
		//女神星盘
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW01)) {
			if (IncreaseManagerment.Instance.isHaveStarCanOpen ()) {
				showButtons [num].gameObject.SetActive (true);
				showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way11");
				showButtons [num].name = TYPE_STAR;
				num++;
			}
		}
		//爵位
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW10)) {
			if (IncreaseManagerment.Instance.getNewKnighthood ()) {
				showButtons [num].gameObject.SetActive (true);
				showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way03");
				showButtons [num].name = TYPE_KNIGHTHOOD;
				num++;
			}
		}
		//圣器可强化
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW04)) {
			if (IncreaseManagerment.Instance.isHallowsCanIntensify ()&&UserManager.Instance.self.getUserLevel()>9) {
				showButtons [num].gameObject.SetActive (true);
				showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way10");
				showButtons [num].name = TYPE_HALLOWS;
				num++;
			}
		}
		//队伍卡片可穿装
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW23) && IncreaseManagerment.Instance.isTeamCardCanPutOnEquip ()) {
			showButtons [num].gameObject.SetActive (true);
			showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way09");
			showButtons [num].name = TYPE_TEAMEQUIP;
			num++;
		}
		//可召唤召唤兽
//		if (IncreaseManagerment.Instance.isBeastCanSummon ()) {
//			showButtons [num].gameObject.SetActive (true);
//			showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way02");
//			showButtons [num].name = TYPE_BEAST_SUMMON;
//			num++; 
//		}
		//可进化召唤兽
		if (IncreaseManagerment.Instance.isBeastCanEvolution ()) {
			showButtons [num].gameObject.SetActive (true);
			showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way01");
			showButtons [num].name = TYPE_BEAST_EVOLUTION;
			num++;
		}
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW07)) {
			//主角突破
			if (IncreaseManagerment.Instance.isMainCardCanSurmount ()) {
				showButtons [num].gameObject.SetActive (true);
				showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way07");
				showButtons [num].name = TYPE_MAINCARD_SURMOUNT;
				num++;
			}
			//主角进化
			if (IncreaseManagerment.Instance.isMainCardCanEvoluion ()) {
				showButtons [num].gameObject.SetActive (true);
				showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way06");
				showButtons [num].name = TYPE_MAINCARD_EVOLUTION;
				num++;
			}
		}
		//普卡进化
		if (UserManager.Instance.self.getUserLevel () > 5) {
			if (IncreaseManagerment.Instance.isCardCanEvoluion ()) {
				showButtons [num].gameObject.SetActive (true);
				showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way04");
				showButtons [num].name = TYPE_CARD_EVOLUTION;
				num++;
			}
		}
		//英雄之章
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW08)) {

			//2014.8.19 YXZH-4885 整合
			if (IncreaseManagerment.Instance.getHeroRoad ()) {
				showButtons [num].gameObject.SetActive (true);
				showButtons [num].textLabel.text = LanguageConfigManager.Instance.getLanguage ("Way05");
				showButtons [num].name = TYPE_HEROROAD;
				num++;
			}
		}
        //卡牌训练
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW31)) {
			if (IncreaseManagerment.Instance.isCardCanTraining())
			{
				showButtons[num].gameObject.SetActive(true);
				showButtons[num].textLabel.text = LanguageConfigManager.Instance.getLanguage("Way13");
				showButtons[num].name = TYPE_CARDTRAINING;
				num++;
			}
		}
//		//卡牌兑换
//		if(CardScrapManagerment.Instance.CanExchangeCard() && UserManager.Instance.self.getUserLevel () <= 40){
//			showButtons[num].gameObject.SetActive(true);
//			showButtons[num].textLabel.text = LanguageConfigManager.Instance.getLanguage("Way14");
//			showButtons[num].name = TYPE_CARDEXCHAGE;
//			num++;
//		}
		//骑术可修炼
		if(GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW34) && IncreaseManagerment.Instance.isEnoughPropForMountsPractice ()&&UserManager.Instance.self.getUserLevel()>=10){
			showButtons[num].gameObject.SetActive(true);
			showButtons[num].textLabel.text = LanguageConfigManager.Instance.getLanguage("Way15");
			showButtons[num].name = TYPE_MOUNTS_PRACTICE;
			num++;
		}
		
		SpriteSum = num;
	}
	
	private void initLabel ()
	{
		for (int i = 0; i<showButtons.Length; i++) {
			showButtons [i].gameObject.SetActive (false);
		}
	}
	
	private void closeWin (CallBack cb)
	{
		finishWindow ();
		if (cb != null)
			cb ();
//		EventDelegate.Add (OnHide, () => {
//			if (cb != null)
//				cb ();
//		});
	}
}