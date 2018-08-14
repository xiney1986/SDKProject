using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 猎杀星魂容器
/// </summary>
public class StarSoulHuntContent : MonoBase {

    /**强化特效预制件 */
    public GameObject strengEffectPerfab;
	/* fields */
	/** */
	public GameObject goodsViewProfab;
	/** RMB星云预制件 */
	public GameObject rmbEffectperfab;
	/** 飞行效果预制件 */
	public GameObject flyItemPrefab;
	/** 碎片图标预制 */
	public GameObject debrisIconPrefab;
	/** 碎片获取名 */
	public UILabel debrisGetText;
	/** 碎片获取标签 */
	public UILabel debrisGetValue;
	/** 碎片获取标签  */
	public GameObject debrisGetDesc;
	/** 仓库按钮特效点 */
	public GameObject effectFocusLightPoint;
	/** 飞行容器 */
	public GameObject flyContentPoint;
	/** 普通tap容器 */
	public TapContentBase commonTapContent;
	/** vipTap容器 */
	public TapContentBase vipTapContent;
	/** 单次裂魂 */
	public ButtonBase buttonHunt;
	/** 多次裂魂 */
	public ButtonBase buttonHuntN;
	/** 拾取设定 */
	public ButtonBase buttonGetSetting;
	/** 碎片商店 */
	public ButtonBase buttonExChangeShop;
	/** 一键拾取按钮 */
	public ButtonBase oneKeyGetButton;
	/** 单次消耗标签 */
	public UILabel consumeLabel;
	/**  消耗图标 */
	public UISprite consumeIcon;
	/** rmb标签 */
	public UILabel rmbLabel;
    //**星魂碎片标签*/
    public UILabel suipianLabel;
	/** yxb标签 */
	public UILabel moneyLabel;
	/** 猎杀背景(普通猎魂背景,砖石猎杀背景) */
	public GameObject[] huntBg;
	/** 星云图片点 */
	public UISprite[] nebulaSpritePoints;
	/** RMB星云特效点 */
	public GameObject nebulaEffectPointsforRmb;
	/** 星云获得猎魂特效点 */
	public GameObject nebulaHuntEffectPoint;
	/** 星云特效panel */
	public UIPanel nebulaPanel;
	/** 星云特效scrollView */
	public UIScrollView nebulaScrollView;
	/** 星云文本 */
	public UILabel nebulaText;
	/** 裂魂仓库容器节点 */
	public GameObject awardContent;
	/** 存储星魂经验值标签 */
	public UILabel starSoulExp;
	/** 增量星魂经验条目 */
	public GameObject incItem;
	/** 增量星魂经验值标签 */
	public UILabel incStarSoulExp;
	//**限时积分*/
	public int integral=0;
	//**限时猎魂sid*/
	public int hountSid=0;
	/** 当前tap下标--0开始 */
	int currentTapIndex;
	/** 星云特效点 */
	LinkedList<GameObject> nebulaEffectList=new LinkedList<GameObject>();
	/** N的裂魂次数 */
	int huntNumber;
	/** 当前抽奖库下标(也用于控制星云特效)--目前只用于普通猎魂 */
	int currentNebulaIndex;
	/** 猎魂仓库版本号 */
	int storageVersion=-1;
	/** 总增量兑换经验(播放动画用) */
	int totalIncExchangeExp;
	/** 当前激活的Tap */
	TapContentBase tapContent;
    List<PrizeSample> showPrizes = new List<PrizeSample>();

	/* methods */
	 void Awake() {

	}
	void Start () {
		StartCoroutine (Utils.DelayRun (() => {
			awardContent.gameObject.SetActive(true);
		}, 0.6f));
		buttonHunt.onClickEvent = HandleHuntEvent;
		buttonHuntN.onClickEvent = HandleHuntNEvent;
		buttonGetSetting.onClickEvent = HandleGetSettingEvent;
		buttonExChangeShop.onClickEvent=HandleExChangeShopEvent;
		oneKeyGetButton.onClickEvent=HandleOneKeyGet;
	}

	/** Update */
	void Update () {
		rotateNebulaEffect ();
	}
	/** 旋转星云特效 */
	private void rotateNebulaEffect() {
		if (nebulaEffectList.Count == 0||currentTapIndex != StarSoulConfigManager.HUNT_MONEY_TYPE)
			return;
		float offsetx = nebulaPanel.clipOffset.x;
		LinkedListNode<GameObject> first = nebulaEffectList.First;
		LinkedListNode<GameObject> last = nebulaEffectList.Last;
		Transform firstTransform = first.Value.transform;
		Transform lastTransform = last.Value.transform;
		if (firstTransform.localPosition.x - offsetx > -280) {
			lastTransform.localPosition = new Vector3 (firstTransform.localPosition.x - 230, lastTransform.localPosition.y, lastTransform.localPosition.z);
			nebulaEffectList.RemoveLast ();
			nebulaEffectList.AddFirst (last);
		} else if (lastTransform.localPosition.x - offsetx < 280) {
			firstTransform.localPosition = new Vector3 (lastTransform.localPosition.x + 230, firstTransform.localPosition.y, firstTransform.localPosition.z);
			nebulaEffectList.RemoveFirst ();
			nebulaEffectList.AddLast (first);
		}
		LinkedListNode<GameObject> node = nebulaEffectList.First;
		while (node != null) {
			Transform nodeTransform = node.Value.transform;
			float len = Math.Abs (nodeTransform.localPosition.x - offsetx);
			float scale = Math.Max ((560 - len) / 560, 0.5f);			
			nodeTransform.localScale = new Vector3 (scale*1.2f, scale*1.2f, 1);
			node = node.Next;
		}
	}
	/***/
	public void init(WindowBase fatherWindow,int tapIndex) {
		initButton (fatherWindow);
		this.currentTapIndex = tapIndex;
		UpdateUI ();
	}
	/** 初始化button */
	public void initButton(WindowBase fatherWindow) {
		commonTapContent.fatherwindow = fatherWindow;
		vipTapContent.fatherwindow=fatherWindow;
		buttonHunt.fatherWindow = fatherWindow;
		buttonHuntN.fatherWindow = fatherWindow;
		buttonGetSetting.fatherWindow = fatherWindow;
		buttonExChangeShop.fatherWindow=fatherWindow;
		oneKeyGetButton.fatherWindow=fatherWindow;
	}
	/** 更新button文本 */
	public void updateLabelTextUI() {
		incItem.gameObject.SetActive(false);
		StarSoulConfigManager configManager=StarSoulConfigManager.Instance;
		StarSoulManager manager=StarSoulManager.Instance;
		buttonHunt.textLabel.text = LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_button_Hunt",Convert.ToString(1));
		UserManager userManger=UserManager.Instance;
		StorageManagerment smanager = StorageManagerment.Instance;
		moneyLabel.text=Convert.ToString(userManger.self.getMoney());
		rmbLabel.text=Convert.ToString(userManger.self.getRMB());
        suipianLabel.text = Convert.ToString(manager.getDebrisNumber());
		int maxCount = smanager.getHuntStarSoulStorageMaxSpace ();
		int remainNum=smanager.getFreeSize();
		maxCount = remainNum >= maxCount ? maxCount : remainNum;
		huntNumber = maxCount;
		if (currentTapIndex == StarSoulConfigManager.HUNT_MONEY_TYPE) {
			huntNumber=userManger.self.getMoney()/configManager.getHuntConsumeMoney (currentNebulaIndex);
			if(huntNumber>maxCount) huntNumber=maxCount;
			string moneyColorText=configManager.isEnoughByHuntMoney (1,currentNebulaIndex)?"[FFFFFF]":"[FF0000]";
			consumeLabel.text=moneyColorText + LanguageConfigManager.Instance.getLanguage ("s0449",Convert.ToString(configManager.getHuntConsumeMoney (currentNebulaIndex)));
			consumeIcon.spriteName="icon_money";
		}
		else if(currentTapIndex == StarSoulConfigManager.HUNT_RMB_TYPE) {
			huntNumber=userManger.self.getRMB()/configManager.getHuntConsumeRmb ();
			if(huntNumber>maxCount) huntNumber=maxCount;
			string rmbColorText=configManager.isEnoughByHuntRMB (1)?"[FFFFFF]":"[FF0000]";
			consumeLabel.text=rmbColorText + LanguageConfigManager.Instance.getLanguage ("s0449",Convert.ToString(configManager.getHuntConsumeRmb ()));
			consumeIcon.spriteName="rmb";
		}
		if (huntNumber <= 1)
			huntNumber = maxCount;
		starSoulExp.text = manager.getStarSoulExp ().ToString();
	}
	/** 更新星云特效UI显示 */
	public void updateNebulaEffectUI() {
		if (currentTapIndex == StarSoulConfigManager.HUNT_MONEY_TYPE) {
			updateNebulaActiveStateforRMB(false);
			updateNebulaActiveState(true);
			//nebulaText.text = QualityManagerment.getQualityColor(currentNebulaIndex+1)+"【"+LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_Nebula"+currentNebulaIndex)+"】";
            nebulaText.text = "【" + LanguageConfigManager.Instance.getLanguage("StarSoulWindow_Nebula" + currentNebulaIndex) + "】";
            StartCoroutine(moveNebulaScrollView());
		}
		else if(currentTapIndex == StarSoulConfigManager.HUNT_RMB_TYPE) {
			updateNebulaActiveState(false);
			updateNebulaActiveStateforRMB(true);
			nebulaText.text = QualityManagerment.getQualityColor(5)+"【"+LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_Nebula5")+"】";
			GameObject obj;
			if(nebulaEffectPointsforRmb.transform.childCount==0){
				NGUITools.AddChild(nebulaEffectPointsforRmb,rmbEffectperfab);
			}
		}
	}
	/** 移动星云拖拽视图 */
	public IEnumerator moveNebulaScrollView() {
		int[] indexs = countNebulaIndex ();
		Transform item;
		int index;
		for(int i=0,len=indexs.Length;i<len;i++) {
			index=indexs[i];
			string name=nebulaSpritePoints[index].spriteName;
			if(index==currentNebulaIndex){
				if(name.Split('_').Length!=2){
					nebulaSpritePoints[index].spriteName=name+"_onclick";
					nebulaSpritePoints[index].width=286;
					nebulaSpritePoints[index].height=286;
				}
					
			}else{
				if(name.Split('_').Length==2){
					nebulaSpritePoints[index].spriteName=name.Split('_')[0];
					nebulaSpritePoints[index].width=152;
					nebulaSpritePoints[index].height=146;
				}
			}
		}
		resetNebulaEffectPoint ();
		Transform beginItem = nebulaSpritePoints [currentNebulaIndex].transform;
		SpringPanel.Begin (nebulaScrollView.gameObject, -new Vector3(beginItem.localPosition.x,255,beginItem.localPosition.z), 6);
		yield break;
	}
	/** 重置星云特效点位置 */
	private void resetNebulaEffectPoint() {
		nebulaEffectList.Clear();
		GameObject item;
		for(int i=0;i<nebulaSpritePoints.Length;i++) {
			item=nebulaSpritePoints[i].gameObject;
			item.transform.localPosition = new Vector3 ((i - 2) * 230, 0, 0);
			nebulaEffectList.AddLast (item);
		}
	}
	/** 计算当前临近3个抽奖库的星云特效下标 */
	private int[] countNebulaIndex() {
		int[] indexs = new int[3];
		indexs [0] = currentNebulaIndex - 1;
		if (indexs [0] < 0)
			indexs [0] = nebulaSpritePoints.Length-1;
		indexs [1] = currentNebulaIndex;
		indexs [2] = currentNebulaIndex + 1;
		if (indexs [2] > nebulaSpritePoints.Length - 1)
			indexs [2] = 0;
		return indexs;
	}
	/// <summary>
	/// 更新星云激活状态
	/// </summary>
	/// <param name="isActive">是否激活</param>
	public void updateNebulaActiveState(bool isActive){
		foreach (UISprite item in nebulaSpritePoints) {
			item.gameObject.SetActive(isActive);	
		}
	}
	/// <summary>
	/// 更新RMB星云激活状态
	/// </summary>
	/// <param name="isActive">是否激活</param>
	public void updateNebulaActiveStateforRMB(bool isActive){
		nebulaEffectPointsforRmb.SetActive(isActive);	
	}
	/** 更新button文本 */
	public void updateButton() {
		if(awardContent.transform.childCount==0)
			oneKeyGetButton.disableButton (true);
		else
			oneKeyGetButton.disableButton (false);
	}
	/** 更新UI */
	public void UpdateUI() {

		UITweener position = buttonExChangeShop.GetComponentInChildren<TweenPosition>();
		UITweener rotation = buttonExChangeShop.GetComponentInChildren<TweenRotation>();
		StartCoroutine(Utils.DelayRun(()=>{
			position.enabled = false;
			rotation.enabled = false;
		},2.5f));

		if(flyContentPoint.transform.childCount>0)
			Utils.RemoveAllChild (flyContentPoint.transform);
		if (currentTapIndex == StarSoulConfigManager.HUNT_MONEY_TYPE)
			setCurrentNebulaIndex(StarSoulManager.Instance.getHuntQuality ());
		updateTapContent();
		tapContent.resetTap ();
		tapContent.changeTapPage (tapContent.tapButtonList [currentTapIndex]);
		updateAwardContent (StorageManagerment.Instance.getAllHuntStarSoul());
		updateLabelTextUI ();
		//showAward ();
	}
    //void FixedUpdate()
    //{
    //    showAward();
    //}
	/** 更新Tap容器 */
	private void updateTapContent() {
//		if(!StarSoulConfigManager.Instance.isEnoughByShowHuntVipLevel()) {
//			tapContent=commonTapContent;
//			currentTapIndex=StarSoulConfigManager.HUNT_MONEY_TYPE;
//			commonTapContent.gameObject.SetActive(true);
//			vipTapContent.gameObject.SetActive(false);
//		} else {
			tapContent=vipTapContent;
			commonTapContent.gameObject.SetActive(false);
			vipTapContent.gameObject.SetActive(true);
			if(!StarSoulConfigManager.Instance.isEnoughByHuntVipLevel()) {
				TapButtonBase tapButtonBase=vipTapContent.getTapButtonByIndex(1);
				if(tapButtonBase!=null) {
					tapButtonBase.doEventButNoActive=true;
				}
			}
//		}
	}
	/// <summary>
	/// 添加星魂仓库容器列表
	/// </summary>
	/// <param name="starSoulList">Star soul list.</param>
	public void addAwardContent(ArrayList starSoulList,int debrisNumber) {
		if (starSoulList == null || starSoulList.Count == 0)
			return;
		CreateStarSoulItemByAll (starSoulList,false);
		StartCoroutine (Utils.DelayRunNextFrame(()=>{
			GameObject[] nodeObjs=new GameObject[starSoulList.Count];
			StarSoul starSoul;
			Transform awardTrans = awardContent.transform;
			Transform childTrans;
			for (int i=0,len=starSoulList.Count; i<len; i++) {
				starSoul=(StarSoul)starSoulList[i];
				childTrans=awardTrans.FindChild (starSoul.uid);
				childTrans.gameObject.SetActive(false);
				if(childTrans!=null)
					nodeObjs[i]=childTrans.gameObject;
			}
			StopAllCoroutines();
			StartCoroutine (playPopHuntStoreEffect(nebulaHuntEffectPoint.transform,nodeObjs,debrisNumber,()=>{
				MaskWindow.UnlockUI();
			}));
			;
		}));
	}
	/// <summary>
	/// 更新奖励容器
	/// </summary>
	/// <param name="starSoulList">星魂列表</param>
	public void updateAwardContent(ArrayList starSoulList) {
		if (StorageManagerment.Instance.huntStarSoulStorageVersion != storageVersion) {
			storageVersion = StorageManagerment.Instance.starSoulStorageVersion;
			if (awardContent.transform.childCount > 0)
				Utils.RemoveAllChild (awardContent.transform);
			CreateStarSoulItemByAll(starSoulList,true);
		}
	}
	/// <summary>
	/// 协同创建星魂列表
	/// </summary>
	/// <param name="starSoulList">构建星魂列表</param>
	private void CreateStarSoulItemByAll(ArrayList starSoulList,bool bo) {
		if (starSoulList == null || starSoulList.Count == 0)
			return;
		GameObject obj;
//		foreach (StarSoul item in starSoulList) {
//			obj=CreateStarSoulItem (item);
//			obj.name=item.uid;
//
//		}
		for(int i=0;i<starSoulList.Count;i++){
			obj=CreateStarSoulItem (starSoulList[i] as StarSoul);
			obj.name=(starSoulList[i] as StarSoul).uid;
			if(i<5){
				obj.transform.localPosition=new Vector3(i*110f,0f,0f);
			}else{
				obj.transform.localPosition=new Vector3((i-5)*110f,-105f,0f);
			}
		}
		if(bo){
			//awardContent.gameObject.SetActive(false);
			StartCoroutine (Utils.DelayRun (() => {
				awardContent.GetComponent<UIGrid> ().repositionNow = true;
				awardContent.gameObject.SetActive(true);
			}, 0.5f));
			//awardContent.gameObject.SetActive(true);
		}else{
			awardContent.GetComponent<UIGrid> ().repositionNow = true;
		}



	}
	/// <summary>
	/// 创建星魂条目
	/// </summary>
	/// <param name="starSoul">星魂</param>
	private GameObject CreateStarSoulItem (StarSoul starSoul) {
		Transform trans = awardContent.transform.FindChild (starSoul.uid);
		if (trans != null) { // 避免出现相同uid的星魂
			GameObject.Destroy(trans.gameObject);
		}
		GameObject obj = NGUITools.AddChild(awardContent,goodsViewProfab) as GameObject;
		obj.transform.localScale = new Vector3 (0.85f,0.85f,1);
		GoodsView view = obj.transform.GetComponent<GoodsView> ();
		view.LockOnClick = false;
		view.init (starSoul,GoodsView.BOTTOM_TEXT_NAME);
		view.fatherWindow = tapContent.fatherwindow;
		view.onClickCallback = ()=>{
			StorageManagerment smanager=StorageManagerment.Instance;
			if(starSoul.getStarSoulType()==0) { // 经验类星魂
				bool isQuality=starSoul.getQualityId()>=QualityType.EPIC?true:false;
				if (isQuality) {
					UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
						win.msg.msgInfo=starSoul.uid;
						win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_storeContent_changeQuality"), doOnClickCallback);
					});
					return;
				}
			} else { // 非经验类星魂
				if (smanager.isStarSoulStorageFull (1)) { // 星魂仓库满
					UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
						win.Initialize (LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_StarSoul_Storage_Full"));
					});
					return;
				}
			}
			MessageHandle msg = new MessageHandle ();
			msg.buttonID = MessageHandle.BUTTON_RIGHT;
			msg.msgInfo=starSoul.uid;
			doOnClickCallback(msg);
		};
		view.longPassCallback=()=>{
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow> (
				(win) => {
				win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.None);
			});
		};
		return obj;
	}
	/// <summary>
	/// 执行星魂单击回调
	/// </summary>
	/// <param name="starSoul">星魂</param>
	private void doOnClickCallback(MessageHandle msg) {
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			string starSoulUid = msg.msgInfo as string;
			if (starSoulUid != null) {
				// 与服务器通讯
				(FPortManager.Instance.getFPort ("StarSoulPopStorageFPort") as StarSoulPopStorageFPort).popStorageAccess (starSoulUid, doChangeExpFinshedCall);
			}
		}
	}
	/// <summary>
	/// 拾取星魂成功回调
	/// </summary>
	/// <param name="exchangeExp">拾取后自动兑换的经验</param>
	/// <param name="exchangeStarSouls">被兑换的星魂列表</param>
	/// <param name="popStorageStarSouls">自动放入星魂仓库的星魂列表</param>
	private void doChangeExpFinshedCall(int exchangeExp,StarSoul[] exchangeStarSouls,StarSoul[] popStorageStarSouls) {
		Transform trans=awardContent.transform;
		StarSoul starSoul;
		Transform childTransform;
		if(exchangeExp>0) {
			totalIncExchangeExp+=exchangeExp;
			incItem.gameObject.SetActive(true);
			incItem.transform.localPosition=starSoulExp.transform.localPosition+new Vector3(starSoulExp.width+10f,0f,0f);
			TweenLabelNumber tln = TweenLabelNumber.Begin (incStarSoulExp.gameObject, 0.5f,totalIncExchangeExp);
			EventDelegate.Add (tln.onFinished, () => {
				totalIncExchangeExp=0;
			}, true);
		}
		if (exchangeStarSouls != null || popStorageStarSouls != null) {
			StopAllCoroutines();
			if (exchangeStarSouls != null) {
				GameObject[] nodeObjs=new GameObject[exchangeStarSouls.Length];
				for(int i=0;i<exchangeStarSouls.Length;i++){
					starSoul=exchangeStarSouls[i];
					if(starSoul==null) continue;
					childTransform=trans.FindChild (starSoul.uid);
					if(childTransform!=null)
						nodeObjs[i]=childTransform.gameObject;
				}
				StartCoroutine (playPopStoreEffect(nodeObjs,starSoulExp.gameObject,()=>{
					UpdateUI();
					MaskWindow.UnlockUI ();
				}));
			}
			if (popStorageStarSouls != null) {
				GameObject[] nodeObjs=new GameObject[popStorageStarSouls.Length];
				for(int i=0;i<popStorageStarSouls.Length;i++){
					starSoul=popStorageStarSouls[i];
					if(starSoul==null) continue;
					childTransform=trans.FindChild (starSoul.uid);
					if(childTransform!=null)
						nodeObjs[i]=childTransform.gameObject;
				}
				StartCoroutine (playPopStoreEffect(nodeObjs,effectFocusLightPoint,()=>{
					UpdateUI();
					MaskWindow.UnlockUI ();
				}));
			}
		} else {
			UpdateUI();
			MaskWindow.UnlockUI ();
		}
	}
	/// <summary>
	/// 更新猎杀背景
	/// </summary>
	/// <param name="tapIndex">Tap index.</param>
	private void updateHuntBg() {
		resetHuntBgActive ();
		huntBg [currentTapIndex].SetActive (true);
	}
	/** 重置猎杀背景 */
	private void resetHuntBgActive() {
		foreach (GameObject item in huntBg) {
			item.SetActive(false);
		}
	}
	/** tap点击事件 */
	public void tapButtonEventBase (GameObject gameObj) {
		string[] strs=gameObj.name.Split (':');
		if (strs.Length != 2)
			return;
		int tapIndex=int.Parse (strs[1])-1;
		if (!checkClickTapCondition (tapIndex)) {
			return;
		}
		this.currentTapIndex=tapIndex;
		PlayerPrefs.SetInt (UserManager.Instance.self.uid + PlayerPrefsComm.STARSOUL_HUNT_TAP, currentTapIndex);
		updateHuntBg ();
		updateLabelTextUI ();
		updateNebulaEffectUI ();
	}
	/** 校验tap点击事件 */
	private bool checkClickTapCondition(int tapIndex) {
		UserManager userManger=UserManager.Instance;
		if (tapIndex == StarSoulConfigManager.HUNT_RMB_TYPE) { // 钻石裂魂
			if(!StarSoulConfigManager.Instance.isEnoughByHuntVipLevel()) {
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_Hunt_VIPInfo", Convert.ToString(StarSoulConfigManager.Instance.getHuntRmbVipLevel())));
				});
				return false;
			}
		}
		return true;
	}
	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {

	}
	/** 单次裂魂 */
	private void HandleHuntEvent(GameObject gameObj) {
		if(!StarSoulConfigManager.Instance.checkHuntCondition(currentTapIndex,1,false))
			return;
		// 与服务器通讯
		if(hountSid!=0)
			(FPortManager.Instance.getFPort ("StarSoulHuntFPort") as StarSoulHuntFPort).huntAccess (hountSid,doHuntFinshedCall);
		else 
			(FPortManager.Instance.getFPort ("StarSoulHuntFPort") as StarSoulHuntFPort).huntAccess (currentTapIndex,doHuntFinshedCall);
	}
	/** 一键猎魂 */
	private void HandleHuntNEvent(GameObject gameObj) {
        if (UserManager.Instance.self.getUserLevel() < StarSoulConfigManager.Instance.huntUserLevel) { // 等级不足
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("s0468", Convert.ToString(StarSoulConfigManager.Instance.huntUserLevel)));
            });
            return;
        }
		if (!NoticeManagerment.Instance.huntNeverTip) {
			UiManager.Instance.openDialogWindow<AlchemyBuyTipWindow> ((win) => {
				win.callback = ToHunt;
                int num = this.currentTapIndex == 0 ? StarSoulConfigManager.Instance.getHuntConsumeMoney(this.currentNebulaIndex) : StarSoulConfigManager.Instance.getHuntConsumeRmb();
                win.initHountTip(this.currentTapIndex, num * StorageManagerment.Instance.getHuntStarSoulStorage().getFreeSize());
			});
		}
		else {
			ToHunt();
		}
//		if(!StarSoulConfigManager.Instance.checkHuntCondition(currentTapIndex,1,true)) // 只做一次猎魂的判断,一键猎魂过程中猎魂仓库满后台会自动停止
//			return;
//		// 与服务器通讯
//		(FPortManager.Instance.getFPort ("StarSoulHuntFPort") as StarSoulHuntFPort).autoHuntAccess (currentTapIndex, doHuntFinshedCall);
	}
	private void ToHunt(){
		if(!StarSoulConfigManager.Instance.checkHuntCondition(currentTapIndex,1,true)) // 只做一次猎魂的判断,一键猎魂过程中猎魂仓库满后台会自动停止
			return;
		// 与服务器通讯
		if(hountSid!=0)
			(FPortManager.Instance.getFPort ("StarSoulHuntFPort") as StarSoulHuntFPort).autoHuntAccess (hountSid, doHuntFinshedCall);
		else
			(FPortManager.Instance.getFPort ("StarSoulHuntFPort") as StarSoulHuntFPort).autoHuntAccess (currentTapIndex, doHuntFinshedCall);
		}
	/// <summary>
	/// 猎魂完成回调
	/// </summary>
	/// <param name="starSouls">猎杀的星魂</param>
	/// <param name="debrisNumber">猎杀的星魂碎片</param>
	/// <param name="huntLibSid">裂魂库SID</param>
	public void doHuntFinshedCall(ArrayList starSouls,int debrisNumber,int huntLibSid) {
		if (currentTapIndex == StarSoulConfigManager.HUNT_MONEY_TYPE)
			setCurrentNebulaIndex(StarSoulManager.Instance.getHuntQuality ());
		StartCoroutine (Utils.DelayRun (() => {
			updateLabelTextUI();
			addAwardContent (starSouls,debrisNumber);
		}, 0.5f));
        showAward();
	}
	/** 拾取设定 */
	private void HandleGetSettingEvent(GameObject gameObj) {
		UiManager.Instance.openDialogWindow<StarSoulGetChooseWindow> ((win)=>{
			win.init();
		});
	}
	/** 碎片商店 */
	private void HandleExChangeShopEvent(GameObject gameObj) {
		nebulaPanel.gameObject.transform.localScale=new Vector3(0.1f,0.1f,1f);
		UiManager.Instance.openWindow<StarSoulDebrisShopWindow> ();
	}
	/** 一键拾取按钮 */
	private void HandleOneKeyGet(GameObject gameObj) {
        //关闭碎片显示
        debrisGetDesc.SetActive(false);
        debrisGetValue.text = "0";
		StorageManagerment smanager=StorageManagerment.Instance;
		int starSoulNum=smanager.getHuntStarSoulNum ();
		if (starSoulNum == 0) { // 猎魂仓库没有东西
			MaskWindow.UnlockUI();
			return;	
		}
		// 非经验类星魂数量
		int popStorageNum=smanager.getHuntStarSoulNumByType ();
		if (smanager.isStarSoulStorageFull (popStorageNum)) { // 星魂仓库满
			// 转化经验的数量
			int exchangeExpNum=smanager.getHuntStarSoulNum()-popStorageNum;
			if(exchangeExpNum==0) {
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("StarSoulWindow_StarSoul_Storage_Full"));
				});
				return;	
			}
		}
		// 与服务器通讯
		(FPortManager.Instance.getFPort ("StarSoulPopStorageFPort") as StarSoulPopStorageFPort).autoPopStorageAccess (doChangeExpFinshedCall);
	}
	/// <summary>
	/// 碎片飞入碎片商店特效
	/// </summary>
	/// <param name="sourceObj">源目标点</param>
	/// <param name="targetTransform">飞向的目标点节点</param>
	IEnumerator playExChangeShopEffect (Transform sourceTrans,GameObject targetObj,int debrisNumber,CallBack callback) {
        Vector3 moveToPos = Vector3.zero;
        GameObject flyItem = NGUITools.AddChild(flyContentPoint, flyItemPrefab) as GameObject;
        //flyItem.SetActive(true);
        FlyCtrl flyCtrl = flyItem.GetComponent<FlyCtrl>();
        flyCtrl.gameObject.transform.position = targetObj.transform.position + new Vector3(0, 0.3f, 0);
        flyCtrl.Initialize(debrisIconPrefab, moveToPos, targetObj);
        //flyCtrl.overCallBack = (callTargetGameObj) =>
        //{
        //    debrisGetDesc.SetActive(true);
        //    debrisGetText.text = "[FF0000]" + "+";
        //    TweenLabelNumber tln = TweenLabelNumber.Begin(debrisGetValue.gameObject, 0.1f, debrisNumber);
        //    //EventDelegate.Add(tln.onFinished, () =>
        //    //{
        //    //    StartCoroutine(Utils.DelayRun(() =>
        //    //    {
        //    //        debrisGetDesc.SetActive(false);
        //    //        debrisGetValue.text = "0";
        //    //    }, 0.6f));
        //    //}, true);
        //};
        debrisGetDesc.SetActive(false);
        debrisGetValue.text = "0";
        playEffect(debrisNumber);
		yield break;
	}
    private void playEffect(int debrisNumber)
    {
        GameObject obj = NGUITools.AddChild(flyContentPoint, strengEffectPerfab);
        obj.transform.localScale = new Vector3(0.92f,1f,1f);
        obj.transform.localPosition = new Vector3(150f, -636f, 0);
        obj.transform.localRotation = new Quaternion(-26, 180f, -56f, 1f);
        debrisGetDesc.SetActive(true);
        debrisGetText.text = "[FF0000]" + "+";
        TweenLabelNumber tln = TweenLabelNumber.Begin(debrisGetValue.gameObject, 0.1f, debrisNumber);
        StartCoroutine(Utils.DelayRun(() =>
        {
            if (flyContentPoint.transform.childCount > 0)
                Utils.RemoveAllChild(flyContentPoint.transform);
            debrisGetDesc.SetActive(false);
            debrisGetValue.text = "0";
        }, 1.95f));
    }
	/// <summary>
	/// 抽取星魂放入猎魂库特效
	/// </summary>
	/// <param name="sourceObj">源目标点</param>
	/// <param name="targetTransform">飞向的目标点节点</param>
	/// <param name="debrisNumber">碎片</param>
	/// <param name="callback">特效完后回调</param>
	IEnumerator playPopHuntStoreEffect (Transform sourceTrans,GameObject[] targetObjs,int debrisNumber,CallBack callback) {
		if (targetObjs != null) {
			Vector3 moveToPos=Vector3.zero;
			GameObject targetGameObj;
			EffectManager effectManager = EffectManager.Instance;
			yield return new WaitForSeconds (0.2f);
			for (int i=0; i<targetObjs.Length; i++) {
				targetGameObj=targetObjs[i];
				if(targetGameObj==null) continue;
				GameObject flyItem = NGUITools.AddChild(flyContentPoint,flyItemPrefab) as GameObject;
				flyItem.SetActive(true);
				FlyCtrl flyCtrl=flyItem.GetComponent<FlyCtrl>();
				GoodsView view = targetGameObj.GetComponent<GoodsView> ();
				flyCtrl.gameObject.transform.position = sourceTrans.transform.position;
				flyCtrl.Initialize (view.gameObject, moveToPos,targetGameObj);
				flyCtrl.overCallBack=(callTargetGameObj)=>{
					if(callTargetGameObj!=null) {
						callTargetGameObj.SetActive(true);
					}
				};
			}
			yield return new WaitForSeconds (0.3f);
			if (debrisNumber > 0) {
				StartCoroutine (playExChangeShopEffect(nebulaHuntEffectPoint.transform,buttonExChangeShop.gameObject,debrisNumber,()=>{

				}));
			}
			yield return new WaitForSeconds (0.3f);
			updateNebulaEffectUI ();
			yield return new WaitForSeconds (0.8f);
		}
		if (callback != null) {
			callback();
			callback=null;
		}
		yield break;
	}
	/// <summary>
	/// 拾取星魂特效
	/// </summary>
	/// <param name="nodeNames">播放特效的节点名列表</param>
	/// <param name="targetGameObj">飞行目标对象</param>
	/// <param name="callback">特效完后回调</param>
	IEnumerator playPopStoreEffect (GameObject[] nodeObjs,GameObject targetGameObj,CallBack callback) {
		if (nodeObjs != null) {
			Vector3 moveToPos=new Vector3 (0, -0.1f, 0);
			GameObject childGameObj;
			EffectManager effectManager = EffectManager.Instance;
			for (int i=0; i<nodeObjs.Length; i++) {
				childGameObj=nodeObjs[i];
				if(childGameObj==null) continue;
				GameObject flyItem = NGUITools.AddChild(flyContentPoint,flyItemPrefab) as GameObject;
				FlyCtrl flyCtrl=flyItem.GetComponent<FlyCtrl>();
				GoodsView view = childGameObj.GetComponent<GoodsView> ();
				effectManager.CreateEffect (childGameObj.transform,"Effect/UiEffect/SummonBeast2");
				flyCtrl.gameObject.transform.position = childGameObj.transform.position;
				flyCtrl.gameObject.SetActive (true);
				flyCtrl.Initialize (view.gameObject, moveToPos,targetGameObj);
				flyCtrl.overCallBack=(callTargetGameObj)=>{
					if(callTargetGameObj.transform.childCount==0) {
						effectManager.CreateEffect(callTargetGameObj.transform,"Effect/UiEffect/WeakFlare");
					}
				};
				GameObject.Destroy(childGameObj);
				view.clean ();
			}
			yield return new WaitForSeconds (1.4f);
		}
		if (callback != null) {
			callback();
			callback=null;
		}
	}
	/** 当前星云特效下标 */
	public void setCurrentNebulaIndex(int currentNebulaIndex) {
		if (currentNebulaIndex < 0)
			currentNebulaIndex = 0;
		this.currentNebulaIndex = currentNebulaIndex;
	}
	private void showAward(){
		int index = 0;
		int inter = 0;
		NoticeSample noticeSample = null;
        showPrizes.Clear();
		while (index < NoticeManagerment.Instance.noticeLimit.Count) {
			noticeSample = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeManagerment.Instance.noticeLimit[index].sid);
			if(noticeSample!=null && noticeSample.name.Contains(LanguageConfigManager.Instance.getLanguage("StarSoulWindow_tap_hunt"))){
				hountSid = noticeSample.sid;
				inter = NoticeManagerment.Instance.noticeLimit[index].integral;
				RankAward rankAward = LucklyActivityAwardConfigManager.Instance.updateAwardDateByIntegral(hountSid,inter);
				if(rankAward != null)
					ListKit.AddRange(showPrizes,AllAwardViewManagerment.Instance.exchangeArrayToList(rankAward.prizes));
				NoticeManagerment.Instance.noticeLimit.RemoveAt(index);
				index--;
			}
			index++;
		}
		if(hountSid !=0)
			(FPortManager.Instance.getFPort ("LuckyXianshiFPort") as LuckyXianshiFPort).access (hountSid, setIntegralRank,3);

        //if(showPrizes.Count > 0){
        //    UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{
        //        win.Initialize(showPrizes,LanguageConfigManager.Instance.getLanguage("notice_xianshi_01",integral.ToString()));
        //    });
        //}
	}
	public void setIntegralRank(int _integral,int _rank){
		integral = _integral;
        if (showPrizes.Count > 0)
        {
            UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win) =>
            {
                win.Initialize(showPrizes, LanguageConfigManager.Instance.getLanguage("notice_xianshi_01", integral.ToString()));
            });
        }
	}
}