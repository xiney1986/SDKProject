using UnityEngine;
using System.Collections;

public class MissionItem : ButtonBase
{
	[HideInInspector]
	public Mission mission;
	/** 背景 */
	public UISprite backGround;
	/** Boss头像 */
	public UITexture bossTexure;
	/** 序号文字 */
	public UILabel indexLabel;
	/** 星星排版 */
	public UIGrid starGrid;
	/** 副本名字 */
	public UILabel nameLabel;
	/** 等级限制 */
	public UILabel levelLimit;
	/** 星星集合 */
	public UISprite[] starSpriteList;
	/** 线 */
	public UISprite line;
	/** 奖励聚集点 */
	public GameObject goodsViewPos;
	/** 奖励预制体 */
	public GameObject goodsViewPrefab; 

	private int index;
	private bool isLevelComplete = false;//等级满足需求
	/** 展示的掉落 */
	private PrizeSample tmpPrize;
	/** 展示的掉落 */
	private GoodsView goodView;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (!FuBenManagerment.Instance.isCompleteLastMission (mission.sid) || !isLevelComplete) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage("S0360"));
			MaskWindow.UnlockUI ();
			return;
		}
        //爬塔不消耗体力
		if (UserManager.Instance.self.getPvEPoint () == 0) {//
			UiManager.Instance.openDialogWindow <PveUseWindow> ();
		} else {
			GuideManager.Instance.doGuide ();
			if (fatherWindow is MissionChooseWindow) {
				(fatherWindow as MissionChooseWindow).setBgIsCanMove (false);
			}
			//设置选择的副本d过程sid
			FuBenManagerment.Instance.selectedMissionSid = mission.sid;
			UiManager.Instance.openWindow<TeamPrepareWindow> ((win) => {
				win.Initialize (mission, TeamPrepareWindow.WIN_MISSION_ITEM_TYPE);
				win.missionWinItem.GetComponent<MissionWinItem> ().updateButton (mission);
			});
		}
	}


	/// <summary>
	/// 初始化副本入口信息
	/// </summary>
	public void initButton (Mission mission, int index)
	{
		this.mission = mission;
		this.index = index;
		initUI ();
	}

	/// <summary>
	/// 更新副本入口信息
	/// </summary>
	public void updateButton ()
	{
		initUI ();
	}

	/// <summary>
	/// 初始化副本入口信息
	/// </summary>
	void initUI ()
	{
		if (!FuBenManagerment.Instance.isCompleteLastMission (mission.sid)) {
			backGround.spriteName = "mission_notopen";
			backGround.MakePixelPerfect ();
			return;
		}
		indexLabel.text = index + "";
		indexLabel.gameObject.SetActive (true);
		string spName = mission.getMissionType ();
		
		if (spName == MissionShowType.NEW || spName == MissionShowType.COMPLET) {
			
		}
		if (spName == MissionShowType.LEVEL_LOW) {
			isLevelComplete = false;
			levelLimit.text = "Lv." + mission.getRequirLevel () + LanguageConfigManager.Instance.getLanguage ("s0160");
		} else {
			isLevelComplete = true;
			levelLimit.text = "";
		}
		
		if (StringKit.toInt (mission.getOther () [0]) == 1) {//BOSS
			if (mission.getBossSid() != 0) {
				Card boss = CardManagerment.Instance.createCard (mission.getBossSid ());
				if (boss != null) {
					indexLabel.gameObject.SetActive (false);
					ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + boss.getImageID (), bossTexure);
				}
			}
			bossTexure.gameObject.SetActive (true);
			backGround.spriteName = "mission_red";
			StartCoroutine (Utils.DelayRun (()=>{
				initAward ();
			},0.5f));
		}else if (StringKit.toInt (mission.getOther () [0]) == 2) {//精英
			if (mission.getBossSid() != 0) {
				Card boss = CardManagerment.Instance.createCard (mission.getBossSid ());
				if (boss != null) {
					indexLabel.gameObject.SetActive (false);
					ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + boss.getImageID (), bossTexure);
				}
			}
			bossTexure.gameObject.SetActive (true);
			backGround.spriteName = "mission_yellow";
			StartCoroutine (Utils.DelayRun (()=>{
				initAward ();
			},0.5f));
		} else {
			bossTexure.gameObject.SetActive (false);
			backGround.spriteName = "mission_blue";
		}
		backGround.MakePixelPerfect ();
		updateStar (FuBenManagerment.Instance.getMyStarNumByMissionSid(mission.sid),FuBenManagerment.Instance.getStarNumByMissionSid(mission.sid));
	}

	/// <summary>
	/// 更新副本难度信息
	/// </summary>
	void updateStar (int num, int allNum)
	{
		if (allNum == 0) {
			return;
		}
		if(allNum > starSpriteList.Length) {
			allNum = starSpriteList.Length;
		}
		for (int i = 0; i < allNum; i++) {
			if (i < num) {
				starSpriteList [i].spriteName = "star";
				starSpriteList [i].gameObject.SetActive (true);
			} else {
				starSpriteList [i].spriteName = "star_b";
				starSpriteList [i].gameObject.SetActive (true);
			}
		}

		starGrid.repositionNow = true;
	}

	/// <summary>
	/// 初始化奖励信息
	/// </summary>
	public void initAward () {
		PrizeSample[] tmpList = mission.getPrizes ();
		Equip tmpEquip = null;
		if (tmpPrize == null) {
			for (int i = 0; i < tmpList.Length; i++) {
				if (tmpList [i].type == PrizeType.PRIZE_EQUIPMENT) {
					tmpEquip = EquipManagerment.Instance.createEquip (tmpList [i].pSid);
					if (tmpEquip != null && !tmpEquip.isToEat ()) {
						if (getEquipPrizeSid (tmpEquip) != -1) {
							tmpPrize = new PrizeSample(3,getEquipPrizeSid (tmpEquip),1);
						}
						break;
					}
					else {
						continue;
					}
				}
			}
			if (tmpPrize != null && goodView == null) {
				GameObject item = NGUITools.AddChild (goodsViewPos, goodsViewPrefab);
				GoodsView button = item.GetComponent<GoodsView> ();
				button.fatherWindow = fatherWindow;
				button.onClickCallback = ()=>{
					DoClickEvent ();
				};
				button.init (tmpPrize);
				button.rightBottomText.text = "";
				goodView = button;
			}
		}
		awardDisplay ();
	}

	/// <summary>
	/// 奖励展示动画
	/// </summary>
	public void awardDisplay () {
		if (tmpPrize == null || goodView == null) {
			return;
		}
		goodsViewPos.gameObject.SetActive (true);
		TweenScale ts = TweenScale.Begin (goodsViewPos, 0.5f, new Vector3 (0.9f, 0.9f, 0.9f));
		ts.from = new Vector3 (0, 0, 0);
		int x = 0;
		if (this.transform.localPosition.x > 0) {
			x = -160;
		}
		else {
			x = 160;
			goodsViewPos.transform.localRotation = new Quaternion (0,180,0,0);
			goodView.transform.localRotation = new Quaternion (0,180,0,0);
		}
		TweenPosition tp = TweenPosition.Begin (goodsViewPos, 0.5f, new Vector3 (x, 0, 0));
		tp.from = new Vector3 (0, 0, 0);
		EventDelegate.Add (tp.onFinished, () => {
			TweenScale tp2 = TweenScale.Begin (goodsViewPos, 2f, new Vector3 (0.85f, 0.85f, 0.85f));
			tp2.from = new Vector3 (0.9f, 0.9f, 0.9f);
			tp2.style = UITweener.Style.PingPong;
		}, true);
	}

	/// <summary>
	/// 奖励隐藏
	/// </summary>
	public void awardHide () {
		goodsViewPos.transform.localPosition = Vector3.zero;
		goodsViewPos.transform.localScale = Vector3.zero;
		goodsViewPos.gameObject.SetActive (false);
	}

	/// <summary>
	/// 获得副本入口的序号
	/// </summary>
	public int getIndex () {
		return index;
	}

	/// <summary>
	/// 获得副本入口的坐标
	/// </summary>
	public Vector2 getPos () {
		return this.transform.localPosition;
	}

	/// <summary>
	/// 通过装备反推通用随机道具奖励Sid
	/// </summary>
	private int getEquipPrizeSid (Equip _equip) {
		switch (_equip.getPartId ()) {
		case EquipPartType.WEAPON:
			switch (_equip.getQualityId ()) {
			case 1:
				return 71071;
			case 2:
				return 71076;
			case 3:
				return 71081;
			case 4:
				return 71086;
			case 5:
				return 71091;
			default:
				return -1;
			}
		case EquipPartType.ARMOUR:
			switch (_equip.getQualityId ()) {
			case 1:
				return 71072;
			case 2:
				return 71077;
			case 3:
				return 71082;
			case 4:
				return 71087;
			case 5:
				return 71092;
			default:
				return -1;
			}
		case EquipPartType.SHOSE:
			switch (_equip.getQualityId ()) {
			case 1:
				return 71073;
			case 2:
				return 71078;
			case 3:
				return 71083;
			case 4:
				return 71088;
			case 5:
				return 71093;
			default:
				return -1;
			}
		case EquipPartType.HELMET:
			switch (_equip.getQualityId ()) {
			case 1:
				return 71074;
			case 2:
				return 71079;
			case 3:
				return 71084;
			case 4:
				return 71089;
			case 5:
				return 71094;
			default:
				return -1;
			}
		case EquipPartType.RING:
			switch (_equip.getQualityId ()) {
			case 1:
				return 71075;
			case 2:
				return 71080;
			case 3:
				return 71085;
			case 4:
				return 71090;
			case 5:
				return 71095;
			default:
				return -1;
			}
		default:
			return -1;
		}
	}
}
