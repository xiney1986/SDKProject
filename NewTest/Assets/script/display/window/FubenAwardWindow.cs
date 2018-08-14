using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FubenAwardWindow : WindowBase
{

	/** 排序对象 */
	AwardItemComp comp = new AwardItemComp ();
	//rotation
	public GameObject Rotation_1;
	public float rotationTime = 0.2f;
	public float angel = 1;

	public Mission mission;
	//user
	public GameObject userContent;
//	public UITexture userIcon;
	public UILabel userLevel;
	public barCtrl expbar;
	public UILabel expbarLabel;

	//rating
	public GameObject ratingContent;
	public UITexture ratingLevel;

	//普通奖励容器
	public GameObject generalAwardContent;
	//exp
	public GameObject expContent;
	public UILabel expValue;
	public UISprite expIcon;

	//money
	public GameObject moneyContent;
	public UILabel moneyValue;
	public UISprite moneyIcon;

	//start
	public GameObject starContent;
	public UILabel starValue;

	//heroRoad
	public GameObject heroRoadAwardPrefab;
    public GameObject towerAwardPerfab;//爬塔副本预制体
	public GameObject heroRoadObj;
    public GameObject towerObj;//爬塔副本奖励挂接点
	HeroRoadAwardContent heroRoadAwardContent;
    TowerAwardContent towerAwardContent;

	//goods
	public GameObject missionClearingPoint;
	GameObject missionClearing;
	public GameObject AwardContent;
	public GameObject AwardItemsContent;
	public GameObject BoxAwardContent;
	public GameObject BoxAwardItemsContent;
	public GameObject awardArrow;
	public GameObject boxAwardArrow;
	public GameObject btnClose;
	public GameObject shareButton;
	public GameObject goodsViewPrefab;

	/** 除宝箱外的奖励 */
	List<GameObject> AwardItems;
	/** 宝箱奖励 */
	List<GameObject> BoxAwardItems;
	/**紧急开关 */
	public bool isOpenDestory;
	int expGap;
	int starGap;
	int moneyGap;
	int honorGap;
	int rmbGap;
	int setp;
	int nextSetp;
	Award[] awards;
	Award aw;
	bool heroRoadSet = false;
    bool towerSet = false;
	private bool isHeroRoad = false; //英雄之章
    private bool isTower = false;//是不是爬塔副本
	private const int MINLEVEL = 8;//等级8级以下不弹出窗口
	private const int MAXLEVEL = 20;//等级20级以下弹出窗口

	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();
        towerSet = true;
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		if (MissionManager.instance != null && DestroyWhenClose)
		{
			MissionManager.instance.showAll ();
			MissionManager.instance.setBackGround();
		}
		heroRoadSet = false;
        towerSet = false;
		if (missionClearing != null)
			missionClearing.GetComponent<Animator> ().enabled = false;
	}
	
	protected override void begin ()
	{
		base.begin ();
		playAudio();
		if (!isHeroRoad&&!isTower) {
			playFubenBattleAnim ();
		}
		StartCoroutine (Utils.DelayRun (() => {
			NextSetp ();
			heroRoadSet = true;
            towerSet = true;
		}, 0.2f));
		if (isSetpOver ())
			MaskWindow.UnlockUI ();
		GuideManager.Instance.closeGuideMask ();
	}

	private void playFubenBattleAnim ()
	{
		if (missionClearingPoint.transform.childCount == 0) {
			passObj _obj = MonoBase.Create3Dobj ("Effect/UiEffect/battleAnim");
			_obj.obj.transform.parent = missionClearingPoint.transform;
			_obj.obj.transform.localPosition = Vector3.zero;
			_obj.obj.transform.localScale = Vector3.one;
			BattleAnimCtrl battleAnimCtrl = _obj.obj.GetComponent<BattleAnimCtrl> ();
			battleAnimCtrl.missionClearing.transform.localPosition = Vector3.zero;
			battleAnimCtrl.missionClearing.SetActive (true);
			missionClearingPoint.SetActive (true);
			missionClearing = battleAnimCtrl.missionClearing;
		}
	}

	private void begiLoad ()
	{

	}

	public void init (Award[] awards)
	{
		this.awards = awards;
		AwardItems = new List<GameObject> ();
		BoxAwardItems = new List<GameObject> ();
		User user = UserManager.Instance.self;
        if (HeroRoadManagerment.Instance.currentHeroRoad == null && MissionInfoManager.Instance.isTowerFuben()) {
			int deadNum = MissionInfoManager.Instance.mission.deadNum;
			if (deadNum >= 0 && deadNum <= 2)
				ResourcesManager.Instance.LoadAssetBundleTexture ("texture/missionComplete/missioComplete_S", ratingLevel, (obj) => {
					setRatingLevelActive (false);
				});
			else if (deadNum >= 3 && deadNum <= 5)
				ResourcesManager.Instance.LoadAssetBundleTexture ("texture/missionComplete/missioComplete_A", ratingLevel, (obj) => {
					setRatingLevelActive (false);
				});
			else if (deadNum >= 5 && deadNum <= 10)
				ResourcesManager.Instance.LoadAssetBundleTexture ("texture/missionComplete/missioComplete_B", ratingLevel, (obj) => {
					setRatingLevelActive (false);
				});
			else
				ResourcesManager.Instance.LoadAssetBundleTexture ("texture/missionComplete/missioComplete_C", ratingLevel, (obj) => {
					setRatingLevelActive (false);
				});
			//新手其间强制给S级
			if (GuideManager.Instance.isLessThanStep (GuideGlobal.NEWOVERSID)) {
				ResourcesManager.Instance.LoadAssetBundleTexture ("texture/missionComplete/missioComplete_S", ratingLevel, (obj) => {
					setRatingLevelActive (false);
				});
			}
		}
		aw = null;
		//box
		aw = getAwardByType (AwardManagerment.MNGV);
		if (aw != null) {
			CreateGoodsByAward (BoxAwardItems, aw);
		}
		//first
		Award firstAward = getAwardByType (AwardManagerment.FIRST);
		//fuben
		Award fubenAward = getAwardByType (AwardManagerment.FB_END);
		aw = Award.mergeAward (firstAward, fubenAward);

		if (aw != null) {
			moneyGap = aw.moneyGap > 0 ? aw.moneyGap : 0;
			expGap = aw.expGap > 0 ? aw.expGap : 0;
			honorGap = aw.honorGap > 0 ? aw.honorGap : 0;
			rmbGap = aw.rmbGap > 0 ? aw.rmbGap : 0;
			if (rmbGap > 0) {
				moneyIcon.spriteName = "rmb";
			}
			starGap = aw.starGap;
			CreateGoodsByAward (AwardItems, aw);
		}
		SortAwardItem (AwardItems);
		SortAwardItem (BoxAwardItems);
		for (int i = 0; i < AwardItems.Count; i++) {
			GameObject obj = AwardItems [i];
			obj.transform.parent = AwardItemsContent.transform;
			obj.transform.localPosition = new Vector3 (i * 94, 0, 0);
			obj.transform.localScale = new Vector3 (0.7f, 0.7f, 1);
		}
		for (int i = 0; i < BoxAwardItems.Count; i++) {
			GameObject obj = BoxAwardItems [i];
			obj.transform.parent = BoxAwardItemsContent.transform;
			obj.transform.localPosition = new Vector3 (i * 94, 0, 0);
			obj.transform.localScale = new Vector3 (0.7f, 0.7f, 1);
		}
		//user
//		if (aw != null && aw.playerLevelUpInfo != null) {
//			userLevel.text = "Lv" + aw.playerLevelUpInfo.oldLevel;
//			expbarCtrl.GetComponent<UISlider> ().value = Mathf.Max (0.1f, (float)aw.playerLevelUpInfo.oldExp / aw.playerLevelUpInfo.oldExpUp);
//		} else {
		userLevel.text = "Lv." + user.getUserLevel ();
		expbar.updateValue (UserManager.Instance.self.getLevelExp (), UserManager.Instance.self.getLevelAllExp ());
		expbarLabel.text = UserManager.Instance.self.getLevelExp () +"/"+UserManager.Instance.self.getLevelAllExp ();
//		}

		if (HeroRoadManagerment.Instance.currentHeroRoad != null) {
			isHeroRoad = true;
			GameObject heroRoadAwardObj = Instantiate (heroRoadAwardPrefab) as GameObject;
			heroRoadAwardObj.transform.parent = heroRoadObj.transform;
			heroRoadAwardObj.transform.localPosition = Vector3.zero;
			heroRoadAwardObj.transform.localScale = Vector3.one;
			heroRoadAwardContent = heroRoadAwardObj.GetComponent<HeroRoadAwardContent> ();
			heroRoadAwardContent.initHeroRoad (honorGap, rmbGap);
			heroRoadAwardContent.closeButton.GetComponent<ButtonBase> ().setFatherWindow (this);
        } else if (MissionInfoManager.Instance.isTowerFuben()) {
            isTower = true;
            GameObject towerAwardObj = Instantiate(towerAwardPerfab) as GameObject;
            towerAwardObj.transform.parent = towerObj.transform;
            towerAwardObj.transform.localPosition = Vector3.zero;
            towerAwardObj.transform.localScale = Vector3.one;
            towerAwardContent = towerAwardObj.GetComponent<TowerAwardContent>();
            towerAwardContent.initTowerAward(firstAward, fubenAward,this);
            towerAwardContent.closeButton.GetComponent<ButtonBase>().setFatherWindow(this);
        }
	}

	void setRatingLevelActive (bool isActive)
	{
		if (ratingLevel != null)
			ratingLevel.gameObject.SetActive (isActive);
	}

	/** 奖励条目排序 */
	void SortAwardItem (List<GameObject> awardItem)
	{
		if (awardItem.Count <= 1)
			return;
		GameObject[] objs = awardItem.ToArray ();
		SetKit.sort (objs, comp);
		awardItem.Clear ();
		foreach (GameObject obj in objs) {
			awardItem.Add (obj);
		}
	}

	void Update ()
	{
		Rotation_1.transform.localRotation = Quaternion.AngleAxis (angel, Vector3.forward);
		angel += 0.05f;
		if (angel > 360.0f)
			angel = 0.0f;
		if (isHeroRoad) {
			if (heroRoadAwardContent != null && heroRoadSet)
				heroRoadAwardContent.heroRoadAnimation ();
			return;
		}
        if(isTower){
            if (towerAwardContent != null && towerSet) {
                towerAwardContent.heroRoadAnimation();
                return;
            }
        }
		User user = UserManager.Instance.self;
		if (setp == nextSetp)
			return;
		//评级
		if (setp == 0) {
			ratingContent.SetActive (true);
			ratingLevel.gameObject.SetActive (true);
			TweenScale ts = TweenScale.Begin (ratingLevel.gameObject, 0.2f, Vector3.one);
			ts.method = UITweener.Method.EaseIn;
			ts.from = new Vector3 (5, 5, 1);
			EventDelegate.Add (ts.onFinished, () =>
			{
				iTween.ShakePosition (transform.parent.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
				iTween.ShakePosition (transform.parent.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
				StartCoroutine (Utils.DelayRun (() =>
				{
					NextSetp ();
				}, 0.1f));
			}, true);
		}
		//user
		else if (setp == 1) {
			userContent.SetActive (true);
			TweenPosition tp = TweenPosition.Begin (userContent, 0.15f, userContent.transform.localPosition);
			tp.from = new Vector3 (-500, userContent.transform.localPosition.y, 0);
			EventDelegate.Add (tp.onFinished, () =>
			{
				expbar.gameObject.SetActive (true);
				expbar.updateValue (UserManager.Instance.self.getLevelExp (), UserManager.Instance.self.getLevelAllExp ());
				expbarLabel.text = UserManager.Instance.self.getLevelExp ()+"/"+UserManager.Instance.self.getLevelAllExp ();
				NextSetp ();
//				if (expGap > 0) {
//					expbarCtrl.gameObject.SetActive (true);
//					if (aw != null && aw.playerLevelUpInfo != null) {
//						expbarCtrl.init (aw.playerLevelUpInfo);
//						expbarCtrl.setLevelUpCallBack ((nowLevel) => {
//							userLevel.text = "Lv" + nowLevel;
//						});
//					} else {
//						expbarCtrl.updateValue (user.getLevelExp (),user.getLevelAllExp ());
//					}
//					StartCoroutine (Utils.DelayRun (() =>
//					{
//						NextSetp ();
//					}, 0.1f));
//				} else {
//					NextSetp ();
//				}
			}, true);   
		} 
		//generalAwardContent
		else if (setp == 2) {
			generalAwardContent.SetActive (true);
			TweenPosition tp = TweenPosition.Begin (generalAwardContent, 0.15f, generalAwardContent.transform.localPosition);
			tp.from = new Vector3 (-500, generalAwardContent.transform.localPosition.y, 0);
			EventDelegate.Add (tp.onFinished, () => {
				bool isShowEffect = false;
				//计算经验
				int num = expGap;
				if (num > 0) {
					TweenLabelNumber tln = TweenLabelNumber.Begin (expValue.gameObject, 0.15f, num);
					EventDelegate.Add (tln.onFinished, () => {
						GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
						obj.transform.parent = expContent.transform;
						obj.transform.localScale = Vector3.one;
						obj.transform.localPosition = new Vector3 (0, 0, -600);
						isShowEffect = true;
					}, true);
				} 
				//计算金币
				int gap = moneyGap;
				if (gap > 0) {
					TweenLabelNumber tln = TweenLabelNumber.Begin (moneyValue.gameObject, 0.15f, gap);
					EventDelegate.Add (tln.onFinished, () => {
						GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
						obj.transform.parent = moneyContent.transform;
						obj.transform.localScale = Vector3.one;
						obj.transform.localPosition = new Vector3 (0, 0, -600);
						isShowEffect = true;
					}, true);
				}
				//计算星屑
				int star = starGap;
				if (star > 0) {
					TweenLabelNumber tln = TweenLabelNumber.Begin (starValue.gameObject, 0.15f, star);
					EventDelegate.Add (tln.onFinished, () => {
						GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
						obj.transform.parent = starContent.transform;
						obj.transform.localScale = Vector3.one;
						obj.transform.localPosition = new Vector3 (0, 0, -600);
						isShowEffect = true;
					}, true);
				}
				if (isShowEffect) {
					StartCoroutine (Utils.DelayRun (() => {
						NextSetp ();}, 0.15f));
				}
				else {
					NextSetp ();
				}
			}, true);
		}
        //allItems
        else if (setp == 3) {
			if (AwardItems == null || AwardItems.Count == 0) {
				NextSetp ();
			}
			else {
                if (AwardItems.Count > 4)
                    awardArrow.SetActive(true);
                AwardContent.SetActive(true);
                TweenPosition tp = TweenPosition.Begin(AwardContent, 0.15f, AwardContent.transform.localPosition);
                tp.from = new Vector3(0, -500, 0);
                EventDelegate.Add(tp.onFinished, () => {
                    float time = GoodsInAnimation(AwardItems);
                    StartCoroutine(Utils.DelayRun(() => {
                        NextSetp();
                    }, time));
                }, true);
                }
		}
		else if (setp == 4) {
			if (BoxAwardItems == null || BoxAwardItems.Count == 0) {
				NextSetp ();
			}
			else {
				if (BoxAwardItems.Count > 4)
					boxAwardArrow.SetActive (true);
				BoxAwardContent.SetActive (true);
				TweenPosition tp = TweenPosition.Begin (BoxAwardContent, 0.15f, BoxAwardContent.transform.localPosition);
				tp.from = new Vector3 (0, -500, 0);
				EventDelegate.Add (tp.onFinished, () =>
				{
					float time = GoodsInAnimation (BoxAwardItems);
					StartCoroutine (Utils.DelayRun (() =>
					{
						NextSetp ();
					}, time));
				}, true);
			}
		}
		else if (setp == 5) {
//			btnClose.SetActive (true);
//			if (MissionManager.instance.tmpStorageVersion != StorageManagerment.Instance.tmpStorageVersion) {
//				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage("s0122"));
//			}
			if (GuideManager.Instance.isOverStep (GuideGlobal.NEWOVERSID)) {
				MaskWindow.LockUI();
				getLevelUpAward ();
			} else {
				MaskWindow.UnlockUI ();
			}
			NextSetp ();
//			UiManager.Instance.openDialogWindow<LevelupRewardWindow> ((win) => {
//				win.init (null);
//			});
			//MaskWindow.UnlockUI ();
		}
		else if (setp == 6) {
			btnClose.SetActive(true);
			/**分享功能**/
			if(StringKit.toInt (MissionInfoManager.Instance.getMission().getOther () [0]) == 1){
			    shareButton.SetActive(true);
				btnClose.transform.localPosition=new Vector3(120.0f,-394.5f,0);
			}
			MaskWindow.UnlockUI();
		}
		setp++;
	}
	
	public Award getAwardByType (string type)
	{ 
		foreach (Award each in awards) {
			if (each.type == type) 
				return each; 
		} 
		return null;
	}

	/// <summary>
	/// 筛选出奖励的装备
	/// </summary>
	public List<Equip> getAwardByEquip ()
	{
		List<Equip> aa = new List<Equip> ();
		foreach (Award each in awards) {
			if (each.equips != null && each.equips.Count > 0) {
				foreach (EquipAward o in each.equips) {
					aa.Add (StorageManagerment.Instance.getEquip (o.id));
				}
			}
		} 
		return aa;
	}

	private void CreateGoodsByAward (List<GameObject> awards, Award aw)
	{
		GameObject obj;
		int nameIndex = 0;
		if (aw.props != null && aw.props.Count > 0) {
			Dictionary<int,int> map = new Dictionary<int, int> ();
			foreach (PropAward o in aw.props) {
				if (map.ContainsKey (o.sid))
					map [o.sid] += o.num;
				else
					map.Add (o.sid, o.num);
			}
			foreach (int key in map.Keys) {
				obj = CreateGoodsItem (key, map [key], 0);
				nameIndex++;
				obj.name = "goodsbutton_" + nameIndex;
				awards.Add (obj);
			}
		}
		if (aw.equips != null && aw.equips.Count > 0) {
			Dictionary<int,int> map = new Dictionary<int, int> ();
			foreach (EquipAward o in aw.equips) {
				if (map.ContainsKey (o.sid))
					map [o.sid] += 1;
				else
					map.Add (o.sid, 1);
			}
			foreach (int key in map.Keys) {
				obj = CreateGoodsItem (key, map [key], 1);
				nameIndex++;
				obj.name = "goodsbutton_" + nameIndex;
				awards.Add (obj);
			}
		}
		if (aw.cards != null && aw.cards.Count > 0) {
			Dictionary<int,int> map = new Dictionary<int, int> ();
			foreach (CardAward o in aw.cards) {
				if (map.ContainsKey (o.sid))
					map [o.sid] += 1;
				else
					map.Add (o.sid, 1);
			}
			foreach (int key in map.Keys) {
				obj = CreateGoodsItem (key, map [key], 2);
				nameIndex++;
				obj.name = "goodsbutton_" + nameIndex;
				awards.Add (obj);
			}
		}
        if (aw.magicWeapons != null && aw.magicWeapons.Count > 0) {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (MagicwWeaponAward o in aw.magicWeapons) {
                if (map.ContainsKey(o.sid))
                    map[o.sid] += 1;
                else
                    map.Add(o.sid, 1);
            }
            foreach (int key in map.Keys) {
                obj = CreateGoodsItem(key, map[key], 3);
                nameIndex++;
                obj.name = "goodsbutton_" + nameIndex;
                awards.Add(obj);
            }
        }
		if (aw.starsouls != null && aw.starsouls.Count > 0) {
			Dictionary<int,int> map = new Dictionary<int, int> ();
			foreach (StarSoulAward o in aw.starsouls) {
				if (map.ContainsKey (o.sid))
					map [o.sid] += 1;
				else
					map.Add (o.sid, 1);
			}
			foreach (int key in map.Keys) {
				obj = CreateStarSoulGoodsItem (key, map [key]);
				nameIndex++;
				obj.name = "goodsbutton_" + nameIndex;
				awards.Add (obj);
			}
		}
	}
	//创建星魂奖励
	private GameObject CreateStarSoulGoodsItem (int sid, int num)
	{
		GameObject obj = Instantiate (goodsViewPrefab) as GameObject;
		obj.transform.localScale = new Vector3 (0.7f, 0.7f, 1);
		GoodsView view = obj.transform.GetComponent<GoodsView> ();
		StarSoul starSoul = StarSoulManager.Instance.createStarSoul (sid);
		view.init (starSoul);
		view.fatherWindow = this;
		return obj;
	}

	//0道具,1装备,2卡片.3神器
	private GameObject CreateGoodsItem (int sid, int count, int type)
	{
		GameObject obj = Instantiate (goodsViewPrefab) as GameObject;
		obj.transform.localScale = new Vector3 (0.7f, 0.7f, 1);
		GoodsView view = obj.transform.GetComponent<GoodsView> ();
		view.linkQualityEffectPoint ();
		view.fatherWindow = this;
		if (type == 0) {
			Prop p = PropManagerment.Instance.createProp (sid, count);
			view.init (p);
		} else if (type == 1) {
			Equip e = EquipManagerment.Instance.createEquip (sid);
			view.init (e);
			view.onClickCallback = () => {
				UiManager.Instance.openWindow<EquipAttrWindow> ((winEquip) => {
					winEquip.Initialize (e, EquipAttrWindow.OTHER, null);
				});
			};
		} else if (type == 2) {
			Card c = CardManagerment.Instance.createCard (sid);
			view.init (c);
			view.onClickCallback = () => {
				CardBookWindow.Show (c, CardBookWindow.SHOW, null);
			};
        } else if (type == 3) {
            MagicWeapon mc = MagicWeaponManagerment.Instance.createMagicWeapon(sid);
            view.init(mc);
            view.onClickCallback = () => {
                hideWindow();
                UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                    win.init(mc, MagicWeaponType.FORM_OTHER);
                });
            };
        }
		return obj;
	}
    
	public void NextSetp ()
	{
		nextSetp++;
	}

	/** 动画是否播放结束 */
	public bool isSetpOver ()
	{
		if (setp != 0 && setp == nextSetp)
			return true;
		if (isHeroRoad && heroRoadAwardContent != null) {
			if (heroRoadAwardContent.isSetpOver ())
				return true;
		}
        if(isTower&&towerAwardContent!=null){
            if (towerAwardContent.isSetpOver()) {
                return true;
            }
        }
		return false;
	}

	private float GoodsInAnimation (List<GameObject> list)
	{

		float time = 0.3f;
		foreach (GameObject obj in list) {
			obj.SetActive (true);
			GoodsInFireworksEffect (obj);
			TweenScale.Begin (obj, time, obj.transform.localScale).from = new Vector3 (5, 5, 0);
			time += 0.1f;
		}
		return time;
	}
	/// <summary>
	/// 获取升级奖励
	/// </summary>
	private void getLevelUpAward()
	{
		int lastLevelupSid = LevelupRewardManagerment.Instance.lastRewardSid;
		if (lastLevelupSid > 0) {
			lastLevelupSid++;//to show next levelupReward
		}
		LevelupSample rewardSample = LevelupRewardSampleManager.Instance.getSampleBySid (lastLevelupSid);
		if (rewardSample == null) {
			return;
		}
		if (rewardSample.level <= UserManager.Instance.self.getUserLevel ()) {
			UiManager.Instance.openDialogWindow<LevelupRewardWindow> ((win) => {
				win.init (null);
			});
		}
	}

	private void GoodsInFireworksEffect (GameObject obj)
	{
		GoodsView view = obj.transform.GetComponent<GoodsView> ();
		if (view != null)
			view.showFireworksEffectByQuality ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);


        if (gameObj.name == "close" || gameObj.name == "towClose") {
			if(isOpenDestory&&MissionInfoManager.Instance.mission!=null&&MissionInfoManager.Instance.mission.getChapterType()==ChapterType.STORY){
				//很特殊的操作,破坏了逻辑结构,一般不推荐这么搞
				if (UiManager.Instance.missionMainWindow != null)
					UiManager.Instance.missionMainWindow.destoryWindow ();
			}
			if(isStorageFull())
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage("s0122"));
			if (MissionManager.instance != null)
				MissionManager.instance.missionEnd ();
			else {
				UiManager.Instance.switchWindow<EmptyWindow> ((win) => {
					ScreenManager.Instance.loadScreen (1, null, GameManager.Instance.outMission);	
				});
			}
		} else if (gameObj.name == "heroRoadClose") {

			LoadingWindow.isShowProgress = false;

			UiManager.Instance.switchWindow<EmptyWindow> ((win) => {

				EventDelegate.Add (win.OnStartAnimFinish, () => {

					ScreenManager.Instance.loadScreen (1, null, GameManager.Instance.outHeroRoadBattle);
				});
			});
		}
		else if(gameObj.name=="shareButton"){
			UiManager.Instance.openDialogWindow<OneKeyShareWindow>((win) => { win.initWin(); });
		}
	}

	/** 设置开箱子特效所需的数据 */
	void setAwardBoxData ()
	{
		Award aw = getAwardByType (AwardManagerment.FB_END);
		UiManager.Instance.missionMainWindow.awardBoxTime = 0.0f;
		UiManager.Instance.missionMainWindow.aw = aw;
		UiManager.Instance.missionMainWindow.boxAward = getAwardByType (AwardManagerment.MNGV);
	}

	class AwardItemComp : Comparator
	{

		public int compare (object o1, object o2)
		{
			//显示物品从左到右按品质顺序显示,相同品质,按 装备,卡片,道具的顺序显示
			if (o1 == null)
				return 1;
			if (o2 == null)
				return -1;
			if (!(o1 is GameObject) || !(o2 is GameObject))
				return 0;
			GameObject obj1 = (GameObject)o1;
			GameObject obj2 = (GameObject)o2;
			GoodsView view1 = obj1.transform.GetComponent<GoodsView> ();
			GoodsView view2 = obj2.transform.GetComponent<GoodsView> ();
			if ((view1 == null) || (view2 == null))
				return 0;
			int quality1 = view1.getQuality ();
			;
			int quality2 = view2.getQuality ();
			int stortType1 = view1.getStortType ();
			int stortType2 = view2.getStortType ();
			if (quality1 == quality2) {
				if (stortType1 > stortType2)
					return -1;
				if (stortType1 < stortType2)
					return 1;
				return 0;
			} else {
				if (quality1 > quality2)
					return -1;
				if (quality1 < quality2)
					return 1;
				return 0;
			}
		}
	}
	///<summary>
	/// 检查仓库是否已满
	/// </summary>
	private bool isStorageFull()
	{
		bool isFull = false;
		string strErr = "";
		if ((StorageManagerment.Instance.getAllRole ().Count + 1) > StorageManagerment.Instance.getRoleStorageMaxSpace ()) {
			isFull = true;
		}
		else if ((StorageManagerment.Instance.getAllEquip ().Count + 1) > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
			isFull = true;
		}
		else if ((StorageManagerment.Instance.getAllProp ().Count + 1) > StorageManagerment.Instance.getPropStorageMaxSpace ()) {
			isFull = true;
		}
		return isFull;
	}
	/// <summary>
	/// 播放通关音效
	/// </summary>
	private void playAudio()
	{
		AudioManager.Instance.PlayAudio(139);
	}
}