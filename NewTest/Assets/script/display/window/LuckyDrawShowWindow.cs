using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 抽奖卡片展示窗口
/// </summary>
public class LuckyDrawShowWindow: WindowBase
{

	/** 窗口跳转类型 */
	public const int SKIP_NONE_TYPE=0, // 跳转默认窗口
							SKIP_NOTICE_TYPE=1; // 跳转公告窗口

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
	public GameObject closeButton;
	public GameObject shareButton;
	/** 奖品展示点集合 */
	public GameObject[] panes;
	/** 卡片预制件 */
	public GameObject roleViewPrefab;
	/** 道具预制件 */
	public GameObject goodsViewPrefab;
	/** 紫色,橙色卡片额外特效预制 */
	public GameObject qualityEffectPrefab;
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
	/** 帧 */
	int setp;
	int nextSetp;
	/** 是否开启英雄之章 */
	bool isActiveHeroRoad;
	/** 抽奖实体对象 */
	private LuckyDraw lucky;
	/***/
	private LuckyDrawResults results;
	/***/
    public UILabel cardName;
	/** 跳转窗口类型 */
	public int skipType;
	/** 跳转窗口下标 */
	public int skipIndex;
	/** 显示下次免费抽奖倒计时 */
	public UILabel freeTimeLabel;
  


	//初始化
	public void init (LuckyDrawResults results,LuckyDraw lucky)
	{
		init (results, lucky, 0, 0);
	}
	//初始化
	public void init (LuckyDrawResults results,LuckyDraw lucky,int skipType,int skipIndex)
	{
		this.skipType = skipType;
		this.skipIndex = skipIndex;
		this.lucky = lucky;
		this.results=results;
		loadPirze (results);
		initContent ();
		CreateObjByCache ();
		Invoke("ShowAlert",3.5f);
	}
	private void ShowAlert()
	{
		if(results.isTemp())
		{
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage ("s0260"));
		}
	}
	private void CreateObjByCache(){
		//effectObj=NGUITools.AddChild (effectPoint,luckDrawCardEffectPrefab);
        //EffectCtrl effectCtrl = EffectManager.Instance.CreateEffect(effectPoint.transform,"Effect/UiEffect/Luckdrawcardeffect_a");
        EffectManager.Instance.CreateObjByCache("Effect/UiEffect/Luckdrawcardeffect_a", ((passObj) => {
            luckDrawCardEffect = passObj.obj;
            luckDrawCardEffect.transform.parent = effectPoint.transform;
            luckDrawCardEffect.transform.localPosition = Vector3.zero;
            luckDrawCardEffect.transform.localScale = Vector3.one;
        }));
		EffectManager.Instance.CreateObjByCache("Effect/UiEffect/hero_RoadEnable",((passObj)=>{
			heroRoadEffectObj=passObj.obj;
			heroRoadEffectObj.transform.parent=heroRoadEffectPoint.transform;
			heroRoadEffectObj.transform.localPosition=Vector3.zero;
			heroRoadEffectObj.transform.localScale=Vector3.one;
		}));
	}
	protected override void begin () {
		base.begin ();
		StartCoroutine (loadEffectObj());
		if (isSetpOver ())
			MaskWindow.UnlockUI ();
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
	/** 加载特效资源 */
	IEnumerator loadEffectObj() {
		yield return effectObj||heroRoadEffectObj;
		correctTweenAlpha ();
		StartCoroutine (Utils.DelayRun (() => {
			NextSetp();
		}, 0.2f));
	}
	void correctTweenAlpha() {
		TweenAlpha tweenAlpha=gameObject.GetComponent<TweenAlpha> ();
		if (tweenAlpha != null&&tweenAlpha.to==0) {
			tweenAlpha = TweenAlpha.Begin (gameObject, 0, 1);
			tweenAlpha.from = 0;
		}
	}
	/** 动画是否播放结束 */
	public bool isSetpOver(){
		if (setp != 0 && setp == nextSetp)
			return true;
		return false;
	}
	void Update ()
	{
		if (setp == nextSetp)
			return;
		if (setp == 0) {
			effectPoint.SetActive(true);
			StartCoroutine (Utils.DelayRun (() =>
			{
				NextSetp ();
			}, 3.5f));
		}
		else if (setp == 1) {
			if(!isActiveHeroRoad)
				currentActivePane.transform.localPosition=new Vector3(currentActivePane.transform.localScale.x, currentActivePane.transform.localScale.y-50, 1);
			currentActivePane.SetActive(true);
			TweenScale ts = TweenScale.Begin (currentActivePane, 0.2f, new Vector3 (currentActivePane.transform.localScale.x, currentActivePane.transform.localScale.y, 1));
			ts.method = UITweener.Method.EaseIn;
			ts.from = new Vector3 (0, 0, 1);
			EventDelegate.Add (ts.onFinished, () =>
			{
				StartCoroutine (Utils.DelayRun (() =>{
					NextSetp ();
				}, 0.1f));
			}, true);
		}
		else if(setp==2) {
			if (isActiveHeroRoad) {
				heroRoadEffectPoint.SetActive(true);
				StartCoroutine (Utils.DelayRun (() =>{
					NextSetp ();
				}, 0.5f));
			}
			else {
				NextSetp ();
			}
		}
		else if(setp==3) {
			if(lucky!=null&&lucky.freeTimeChange){
				lucky.freeTimeChange = false;
				freeTimeLabel.text = Language ("s0585") + TimeKit.timeTransform ((lucky.getNextFreeTime () - ServerTimeKit.getSecondTime ()) * 1000);
			}
			closeButton.SetActive(true);
			shareButton.SetActive(true);
			GuideManager.Instance.guideEvent();
			MaskWindow.UnlockUI();
		}
		setp++;
	}
	private void loadPirze(LuckyDrawResults results) {
		List<SinglePrize> listPrize=results.getSinglePrizes ();
		if (listPrize.Count == 0) return;
		ArrayList cards = new ArrayList ();
		ArrayList equips = new ArrayList ();
		ArrayList goods = new ArrayList ();
        ArrayList magicWeapon = new ArrayList();
		foreach (SinglePrize prize in listPrize) {
			if(prize.type==LuckyDrawPrize.TYPE_CARD) {
				Card card = StorageManagerment.Instance.getRole (prize.uid); 
				if (card == null) {
					card = CardManagerment.Instance.createCard (prize.sid);
				}
				if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed (card)) {
					isActiveHeroRoad=true;
				}
				cards.Add(card);
			} else if(prize.type==LuckyDrawPrize.TYPE_EQUIP) {
				Equip equip = StorageManagerment.Instance.getEquip (prize.uid); 
				if (equip == null) {
					equip = EquipManagerment.Instance.createEquip (prize.sid);
				}
				equips.Add(equip);
			} else if(prize.type==LuckyDrawPrize.TYPE_TOOL) {
				Prop prop = StorageManagerment.Instance.getProp (prize.sid); 
				if (prop == null) {
					prop = PropManagerment.Instance.createProp (prize.sid);
				}
				goods.Add(prop);
            } else if (prize.type == LuckyDrawPrize.TYPE_MAGIC_WEAPON) {
                MagicWeapon magicW = MagicWeaponManagerment.Instance.createMagicWeapon(prize.sid);
                magicWeapon.Add(magicW);
                if (magicweaponList == null) magicweaponList = new List<MagicWeapon>();
                magicweaponList.Add(magicW);
            }
		}
		if(cards.Count>0)
			cardSort (cards);
		if(equips.Count>0)
			equipSort (equips);
		if(goods.Count>0)
			propSort (goods);
	}
	/** 卡片排序 */
	void cardSort(ArrayList cards) {
		if(cards.Count>1) {
			SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
			sc.siftConditionArr = new Condition[0];
			sc.sortCondition = new Condition (SortType.SORT);
			sc.sortCondition.conditions.Add (SortType.SORT_QUALITYDOWN);
			cards = SortManagerment.Instance.cardSort (cards, sc);
		}
		if(cardList==null) cardList = new List<Card>();
		cardList.Clear ();
		Card temp;
		for (int i=0; i<cards.Count; i++) {
			temp=cards[i] as Card;
			cardList.Add(temp);
		}
	}
	/** 装备排序 */
	void equipSort(ArrayList equips) {
		if(equips.Count>1) {
			SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
			sc.siftConditionArr = new Condition[0];
			sc.sortCondition = new Condition (SortType.SORT);
			sc.sortCondition.conditions.Add (SortType.SORT_QUALITYDOWN);
			equips = SortManagerment.Instance.equipSort (equips, sc);
		}
		if(equipList==null) equipList = new List<Equip>();
		equipList.Clear ();
		Equip temp;
		for (int i=0; i<equips.Count; i++) {
			temp=equips[i] as Equip;
			equipList.Add(temp);
		}
	}
	/** 道具排序 */
	void propSort(ArrayList props) {
		if(props.Count>1) {
			SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
			sc.siftConditionArr = new Condition[0];
			sc.sortCondition = new Condition (SortType.SORT);
			sc.sortCondition.conditions.Add (SortType.SORT_QUALITYDOWN);
			props = SortManagerment.Instance.propSort (props, sc);
		}
		if(propList==null) propList = new List<Prop>();
		propList.Clear ();
		Prop temp;
		for (int i=0; i<props.Count; i++) {
			temp=props[i] as Prop;
			propList.Add(temp);
		}
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
		int totalNumber=cardNumber+equipNumber+propNumber;
		return totalNumber;
	}
	/** 初始化奖品展示区 */
	public void initContent() {
		int totalNumber=getPrizesNumber();
		int index=totalNumber-1;
		if(index>panes.Length-1) index=panes.Length-1;
		if(index<0) index=0;
		int currentIndex=0;
		if (cardList != null && cardList.Count > 0) {
			GameObject pane=panes[index];
			currentActivePane=pane;
			Transform childTrans;
			Card card;
			Transform trans=pane.transform;
			for(int i=0;i<cardList.Count;i++) {
				card = cardList [i] as Card;
				currentIndex++;
				childTrans=trans.FindChild("point_"+currentIndex);
				initCardButton(childTrans.gameObject,card,i);
			}
		}
		if (propList != null && propList.Count > 0) {
			GameObject pane=panes[index];
			pane.transform.localScale=new Vector3(0.8f,0.8f,1);
			currentActivePane=pane;
			Transform childTrans;
			Prop prop;
			Transform trans=pane.transform;
			for(int i=0;i<propList.Count;i++) {
				prop = propList [i] as Prop;
				currentIndex++;
				childTrans=trans.FindChild("point_"+currentIndex);
				initPropButton(childTrans.gameObject,prop,i);
			}
		}
		if (equipList != null && equipList.Count > 0) {
			GameObject pane=panes[index];
			pane.transform.localScale=new Vector3(0.8f,0.8f,1);
			currentActivePane=pane;
			Transform childTrans;
			Equip equip;
			Transform trans=pane.transform;
			for(int i=0;i<equipList.Count;i++) {
				equip = equipList [i] as Equip;
				currentIndex++;
				childTrans=trans.FindChild("point_"+currentIndex);
				initEquipButton(childTrans.gameObject,equip,i);
			}
		}
        if(magicweaponList!=null&&magicweaponList.Count>0){
            GameObject pane = panes[index];
            pane.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            currentActivePane = pane;
            Transform childTrans;
            MagicWeapon mw;
            Transform trans = pane.transform;
            for (int i = 0; i < magicweaponList.Count; i++) {
                mw = magicweaponList[i] as MagicWeapon;
                currentIndex++;
                childTrans = trans.FindChild("point_" + currentIndex);
                initMagicWeaponButton(childTrans.gameObject, mw, i);
            }
        }
	}
	/** 初始化卡片 */
	public void initCardButton (GameObject gameObj,Card card,int index) {
		GameObject obj=NGUITools.AddChild (gameObj, roleViewPrefab);
		RoleView view = obj.GetComponent<RoleView> ();
        if (cardName != null) {
            string color = QualityManagerment.getQualityColor(card.getQualityId());
            if (card.getQualityId() == QualityType.COMMON) {
                cardName.text = card.getName();
            }else{
                 cardName.text =color+card.getName(); 
            }
               
            GameObject card_name = NGUITools.AddChild(obj, cardName.gameObject);
            card_name.transform.localScale = new Vector3(1.3f, 1.3f, 1);
            card_name.transform.localPosition = new Vector3(0, -131, 0);
            card_name.SetActive(true);
        }
		if (cardList.Count == 1) {
			view.linkEffectPoint ();
		} else {
			if(card.getQualityId()>=QualityType.EPIC){
				NGUITools.AddChild (gameObj, qualityEffectPrefab);
			}
		}
		view.showType = CardBookWindow.SHOW;
		view.hideInBattle = false;
		view.init (card, this, (roleView)=>{
			TweenAlpha lname = TweenAlpha.Begin (gameObject, 1f, 0);
			lname.from = 1;
			CardBookWindow.Show(cardList,index,view.showType,(()=>{
				lname = TweenAlpha.Begin (gameObject, 1.5f, 1);
				lname.from = 0;
			}));
		});
	}
    /** 初始化秘宝 */
    public void initMagicWeaponButton(GameObject gameObj, MagicWeapon magicWeap, int index) {
        GameObject obj = NGUITools.AddChild(gameObj, goodsViewPrefab);
        obj.transform.localScale = new Vector3(1.4f, 1.4f, 1);
        GoodsView goods = obj.GetComponent<GoodsView>();
        goods.fatherWindow = this;
        goods.linkQualityEffectPoint();
        goods.init(magicWeap);
        goods.onClickCallback = goods.DefaultClickEvent;
        //goods.onClickCallback = () => {
        //    TweenAlpha lname = TweenAlpha.Begin(gameObject, 1f, 0);
        //    lname.from = 1;
        //    UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
        //        win.Initialize(magicWeap, EquipAttrWindow.OTHER, () => {
        //            lname = TweenAlpha.Begin(gameObject, 1.5f, 1);
        //            lname.from = 0;
        //        });
        //    });
        //};
        if (cardName != null) {
            string color = QualityManagerment.getQualityColor(magicWeap.getMagicWeaponQuality());
            cardName.text = color + magicWeap.getName() + "[-]";
            GameObject card_name = NGUITools.AddChild(obj, cardName.gameObject);
            card_name.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            card_name.transform.localPosition = new Vector3(0, -70, 0);
            card_name.SetActive(true);
        }
    }
	/** 初始化装备 */
	public void initEquipButton (GameObject gameObj,Equip equip,int index) {
		GameObject obj=NGUITools.AddChild (gameObj, goodsViewPrefab);
		obj.transform.localScale=new Vector3(1.4f,1.4f,1);
		GoodsView goods = obj.GetComponent<GoodsView> ();
		goods.fatherWindow = this;
		goods.linkQualityEffectPoint ();
		goods.init(equip,0);
		goods.onClickCallback = () => {
			TweenAlpha lname = TweenAlpha.Begin (gameObject, 1f, 0);
			lname.from = 1;
			UiManager.Instance.openWindow<EquipAttrWindow> ((winEquip) => {
				winEquip.Initialize (equip, EquipAttrWindow.OTHER, ()=>{
					lname = TweenAlpha.Begin (gameObject, 1.5f, 1);
					lname.from = 0;
				});
			});
		};
		if (cardName != null) {
			string color = QualityManagerment.getQualityColor(equip.getQualityId());
			cardName.text = color + equip.getName() + "[-]";     
			GameObject card_name = NGUITools.AddChild(obj, cardName.gameObject);
			card_name.transform.localScale = new Vector3(0.8f,0.8f,1);
			card_name.transform.localPosition = new Vector3(0, -70, 0);
			card_name.SetActive(true);
		}
	}
	/** 初始化道具 */
	public void initPropButton (GameObject gameObj,Prop prop,int index) {
		GameObject obj=NGUITools.AddChild (gameObj, goodsViewPrefab);
		obj.transform.localScale=new Vector3(1.4f,1.4f,1);
		GoodsView goods = obj.GetComponent<GoodsView> ();
		goods.fatherWindow = this;
		goods.linkQualityEffectPoint ();
		goods.init(prop,0);
		goods.onClickCallback = () => {
			UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
				win.Initialize (prop);
			});
		};
		if (cardName != null) {
			string color = QualityManagerment.getQualityColor(prop.getQualityId());
			cardName.text = color + prop.getName() + "[-]";     
			GameObject card_name = NGUITools.AddChild(obj, cardName.gameObject);
			card_name.transform.localScale = new Vector3(0.8f,0.8f,1);
			card_name.transform.localPosition = new Vector3(0, -70, 0);
			card_name.SetActive(true);
		}
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
            if (magicweaponList != null && magicweaponList.Count > 0) {
                finishWindow();
                return;
            }
			if(GuideManager.Instance.guideSid == 7005000) {
				GuideManager.Instance.doGuide ();
				UiManager.Instance.openMainWindow();
				return;
			}
			root.SetActive(false);
			if(skipType==SKIP_NOTICE_TYPE) {
                finishWindow();

                //UiManager.Instance.switchWindow<NoticeWindow>((win) =>{
                //   win.entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid(skipIndex).entranceId;
                 //   win.updateSelectButton(skipIndex);
                 //    win.firstBoot(0);
               // });

			} else {
                //Debug.LogError("switchWindow");
				UiManager.Instance.switchWindow<LuckyDrawDetailWindow>((win) =>{
					win.setLuckyDraw(lucky);
				});
			}
		}
		if(gameObj.name=="shareButton"){
			UiManager.Instance.openDialogWindow<OneKeyShareWindow>((win) => { win.initWin(); });
		}
	}
	public void NextSetp () {
		nextSetp++;
	}
}
