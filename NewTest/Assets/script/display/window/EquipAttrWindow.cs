using UnityEngine;
using System.Collections.Generic;
 
public class EquipAttrWindow : WindowBase { 
	public int equipStarOpenLevel = 55;
    public int equipRefineLevel = 30;
	Card chooseCard;
	public Equip chooseEquip;
	public UILabel level;
	public UILabel attrLabel1;
	public UILabel attrLabel2;
	public UISprite attrbg;
	public UITexture equipImage;
	public UILabel equipName;
	public UILabel partLabel;
	public UILabel qualityLabel;
	public UILabel descriptlabel;
	public UILabel suitTitle;
	public UILabel eatDesc;
	public UISprite quality;
	public UILabel[] part;//0武器1胸甲2鞋子3头盔4戒指
	public UILabel[] effect;
	public UISprite[] partBg;//品质背景框。0武器1胸甲2鞋子3头盔4戒指
	public UITexture[] partIco;//0武器1胸甲2鞋子3头盔4戒指
	public Color gray = new Color (0.5f, 0.5f, 0.5f, 1);
	public Color green = new Color (0.3f, 1f, 0f, 1);
	public const int STOREVIEW = 1;//仓库浏览模式
	public const int CARDVIEW = 2;//卡牌换装浏览模式 (装备查看--选择装备再次查看,更换)
	public const int CARDSHOW = 3;//装备展示模式
	public const int OTHER = 100;//其他 不显示按钮
	public const int AWARDINTO = 200;//
	public GameObject[] typeProject;//0普通装备，1祭品装备
	public GameObject intensifyButton;//强化按钮-角色装备进
	public GameObject upStarButton;//升星按钮-角色装备进
    public GameObject refineButton;//精炼按钮-角色装备进
	public UIDragScrollView drag;
	public SampleDynamicContent sampleContent;
	// 星星
	public GameObject[] stars;
	//** 升星属性*/
	public GameObject equipStarObj;
	//** 装备模板*/
	private EquipSample equipSample;
	//** 升星属性模板*/
	private EquipStarAttrSample starSample;
	//** 属性效果 */
	private AttrChangeSample[] starEffects;
	//** 属性*/
	public  UILabel[] starAttr;
	public  UISprite[] starAttrIcon;
	//** Arrow */
	public GameObject leftArrow;
	public GameObject rightArrow;
	//* 升星状态显示*/
	public UILabel[] starLevelState;
    /**精练状态显示 和楼上的交替出现 */
    public UILabel[] refineLevelState;
	public UILabel  desInfo;
	private int type;
	public GameObject[] buttonProject;
	private int currentPart;//当前的部位
	private CallBack closeCallback;
	public GameObject operatObjs;
	private Vector3 posLeft = new Vector3 (-161f, -372f, 0);
	private Vector3 posRight = new Vector3 (161f, -372f, 0);
	private Vector3 posMiddle = new Vector3 (0, -372f, 0);
	private SuitSample basicSuit;
    public UISprite[] shuxing;//精炼的属性
    public UILabel[] shuxing2;//精炼属性的值
    public GameObject shuxingdengji;//精炼属性等级
    public UILabel shuxingdengji2;//等级值
    public GameObject shuxingLook;//属性整体的显示
    public GameObject refinTitle;//精练标题
    public GameObject[] starTtile;//升星标题
    public GameObject centerPoint;//中间图片位置
    public UITexture bgTexture;
    public GameObject emptyGameObject;
    private Timer updateInfoTimer;//交替显示属性Timer
    private bool canStar;
    private bool equipStarShow;
    private bool equipRefineShow;
    private int length = 0;
    string[] str = null;
	public void updateEquip () {
		//更新物品
		if (chooseEquip != null) {
			Equip tmp = StorageManagerment.Instance.getEquip (chooseEquip.uid);
			if (tmp != null)
				chooseEquip = tmp;
		}
	    if (chooseEquip != null)
	    {
	        currentPart = chooseEquip.getPartId ();	
	        changeButtonShow ();
           // shuxingLook.SetActive(RefineSampleManager.Instance.getRefineSampleBySid(chooseEquip.sid) != null);
            if (chooseEquip.getrefineLevel() > 0) equipRefineShow = true;
            for (int i = 0; i < shuxing.Length; i++) {
                shuxing[i].gameObject.SetActive(false);
                shuxing2[i].gameObject.SetActive(false);
            }
	        shuxingdengji2.text = chooseEquip.getrefineLevel()+ "/" + chooseEquip.getRefineMaxLevel();
            if (chooseEquip.getrefineLevel() <= 0 || (UserManager.Instance.self.getUserLevel() < 30 && type != EquipAttrWindow.OTHER)) ;
	        else
	        {
	            int chooseEquiplevel = chooseEquip.getrefineLevel();
	            int[] a = new int[3];
	            string[] b = new string[3];
                RefinelvInfo newrfinfo = RefineSampleManager.Instance.getRefineSampleBySid(chooseEquip.sid).refinelvAttr[chooseEquiplevel];
                for (int j = 0; j < newrfinfo.items.Count; j++) {
                    AttrRefineChangeSample acs = newrfinfo.items[j];
                    for (int k = 0; k < 3; k++) {
                        if (b[k] == null) {
                            b[k] = acs.getAttrType();
                            a[k] += acs.getAttrRefineValue(0);
                            break;
                        }
                        if (b[k] == acs.getAttrType()) {
                            a[k] += acs.getAttrRefineValue(0);
                            break;
                        }
                    }
                }
	            for (int j = 0; j < 3; j++)
	            {
	                if (b[j] != null)
	                {
	                    equipRefineShow = true;
	                    shuxing[j].gameObject.SetActive(true);
	                    shuxing2[j].gameObject.SetActive(true);
	                    shuxing[j].spriteName = "attr_" + b[j];
	                    shuxing2[j].text = a[j].ToString();
	                }
	            }
	        }
	        ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + chooseEquip.getIconId (), equipImage);
	        level.text = "Lv." + chooseEquip.getLevel () + "/" + chooseEquip.getMaxLevel ();
	        equipName.text = chooseEquip.getName ();
	        AttrChange[] attrs = chooseEquip .getAttrChanges ();
	        attrLabel1.text = "";
	        attrLabel2.text = ""; 
	        if (attrs != null) {
	            if (attrs.Length > 0 && attrs [0] != null){
	                attrLabel1.text = attrs [0].num.ToString ();
	                attrbg.spriteName = ("attr_" + attrs [0].type);
	            }
	            if (attrs.Length > 1 && attrs [1] != null){
	                attrLabel2.text = attrs [1].num.ToString ();
	                attrbg.spriteName = ("attr_" + attrs [1].type);
	            }
	        } 
		
	        qualityLabel.text = LanguageConfigManager.Instance.getLanguage ("s0084") + QualityManagerment.getQualityName (chooseEquip .getQualityId ());	
	        partLabel.text = LanguageConfigManager.Instance.getLanguage ("s0083") + EquipPartType.getPartName (chooseEquip.getPartId ());
	        descriptlabel.text = "";
	        if (chooseEquip.getQualityId () >= 5 && chooseEquip.equpStarState > 0) {
	            basicSuit = SuitSampleManager.Instance.getSuitSampleBySid (chooseEquip.getSuitSid () * 100 + chooseEquip.equpStarState); 
	        }
	        else {
	            basicSuit = SuitSampleManager.Instance.getSuitSampleBySid (chooseEquip.getSuitSid ()); 
	        }
	        quality.spriteName = QualityManagerment.qualityIDToIconSpriteName (chooseEquip.getQualityId ());
	        if (ChooseTypeSampleManager.Instance.isToEat (chooseEquip, ChooseTypeSampleManager.TYPE_EQUIP_EXP)) {
	            suitTitle.text = LanguageConfigManager.Instance.getLanguage ("JustToEat00");
	            eatDesc.text = LanguageConfigManager.Instance.getLanguage ("JustToEat02", chooseEquip.getEatenExp ().ToString ());
	            typeProject [0].SetActive (false);
	            typeProject [1].SetActive (true);
	        }
	        else {
	            typeProject [0].SetActive (true);
	            typeProject [1].SetActive (false);
	            suitTitle.text = QualityManagerment.getQualityColor (chooseEquip.getQualityId ()) + basicSuit.name + "[FFFFFF]" + LanguageConfigManager.Instance.getLanguage ("s0010");
	            findAllpart (basicSuit);
	            findAllEffect (basicSuit, chooseEquip.equpStarState);
	        }
	    }
	}

	public void Initialize (Equip chooseItem, int type, CallBack closeCallback) {
		Initialize (null, chooseItem, type, closeCallback);
	}
	//玩家使用装备
	public void Initialize (Card card, Equip chooseItem, int type, CallBack closeCallback) { 
		this.type = type;
		this.closeCallback = closeCallback;
		chooseEquip = chooseItem;
        if (chooseEquip != null) {
            EquipSample sampleTmp = EquipmentSampleManager.Instance.getEquipSampleBySid(chooseEquip.sid);
            EquipStarAttrSample sample = sampleTmp == null ? null : EquipStarAttrSampleManager.Instance.getEquipStarAttrSampleBySid(sampleTmp.equipStarSid);
            if (sample != null)
            {
                str = sample.equipStarAtr;
                length = str.Length;
            }
            else
                length = 9;
        }
		chooseCard = card;
		setButtonPostion ();
		setEquipStarAttr ();
		updateEquip ();
	    updateEquipStarStateTip();
        updateInfoTimer = TimerManager.Instance.getTimer(1000);
        updateInfoTimer.addOnTimer(resetArenaInfo);
        updateInfoTimer.start();
	}

	public void setOperation (bool isMyEquip) {
		if (isMyEquip == false) {
			operatObjs.SetActive (isMyEquip);
		}

	}
	
	protected override void begin () {
		base.begin ();
		//updateEquipStarStateTip ();
		if (GuideManager.Instance.isEqualStep (124005000))
			GuideManager.Instance.guideEvent ();
		GuideManager.Instance.doFriendlyGuideEvent ();
		MaskWindow.UnlockUI ();
	}

    protected override void DoEnable()
    {
        base.DoEnable();
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "ChouJiang_BeiJing",bgTexture);
    }

    //改变窗口下方按钮的展示
	private void changeButtonShow () {
		for (int i = 0; i < buttonProject.Length; i++) {
			buttonProject [i].SetActive (false);
		}
		switch (type) {
		case STOREVIEW:
                if (chooseEquip.getLevel() < chooseEquip.getMaxLevel())
                {
                    buttonProject[0].SetActive(true);
                }
            int i = UserManager.Instance.self.getUserLevel();
            if (i >= 30 && RefineSampleManager.Instance.getRefineSampleBySid(chooseEquip.sid) != null)
                {
                    buttonProject[5].SetActive(true);
                }
			//强化等级满什么都不显示
			break;
		case CARDVIEW:

			//如果有要替换的装备,那么显示替换而不是装备
			if (EquipManagerment.Instance.activeInstandEquip != null) {
				if (chooseEquip.state == EquipStateType.INIT) {
					// 装备
					buttonProject [1].SetActive (true);
				}
				else {
					if (chooseEquip.getLevel () >= chooseEquip.getMaxLevel ()) {
						//满级后不显示强化
						buttonProject [4].SetActive (true);
						setButtonPostionFullLevel();
					}
					else {
						buttonProject [4].SetActive (true);
					}
				}
			}
			else {
				if (chooseEquip.state == EquipStateType.INIT) {
					buttonProject [1].SetActive (true);
				}
				else {
					//直接装备
					if (chooseEquip.getLevel () >= chooseEquip.getMaxLevel ()) {
						//满级后不显示强化
						buttonProject [1].SetActive (true);
					}
					else {
						buttonProject [2].SetActive (true);
					}
				}
			}
			break;
		case OTHER:
			break;
		case CARDSHOW:
			break;
		}
	}

	private void findAllEffect (SuitSample ss, int index, int stars=0) {
		int suitMinPartNum = 2;
		int suitMaxPartNum = 5;
		int currentPartNum = suitMaxPartNum;
		if (chooseCard != null) 
			currentPartNum = chooseCard.getSuitPartNumBySid (ss.sid);
		for (int i=0; i<effect.Length; i++) {
			effect [i].text = "";
		}
	    int indexNum = 0;
	    if (index != 0)
	    {
            string strs = ss.sid.ToString();
            indexNum = StringKit.toInt(strs.Substring(5, 2));
	    }
		int effectIndex = 0;
		for (int i=suitMinPartNum; i<=suitMaxPartNum; i++) {
			string des = SuitManagerment.Instance.getSuitDescribe (ss, i);
			if (string.IsNullOrEmpty (des))
				continue;
			effect [effectIndex].text = (stars == 0 ? LanguageConfigManager.Instance.getLanguage ("s0011") : stars + LanguageConfigManager.Instance.getLanguage ("equipStar07")) + i + ":" + des;
            if (chooseCard != null && i <= currentPartNum && (index == 0 || indexNum <= CardManagerment.Instance.getEquipStarLevel(chooseCard)))
				effect [effectIndex].color = green;
			else
				effect [effectIndex].color = gray;
			effectIndex += 1;
		} 
	}
	
	private void findAllpart (SuitSample ss) {
		for (int i=0; i<part.Length; i++) {
			partBg [i].gameObject.SetActive (false);
			partIco [i].gameObject.SetActive (false);
		}
		for (int i=0; i<ss.parts.Count; i++)
		{
		    EquipSample es = null;
            if(chooseEquip.getQualityId() <= 5)
			    es = EquipmentSampleManager.Instance.getEquipSampleBySid (ss.parts [i].ySid);
            else if(chooseEquip.getQualityId() == 6)
                es = EquipmentSampleManager.Instance.getEquipSampleBySid(ss.parts[i].rSid);
			if (es == null)continue; 
			partBg [i].gameObject.SetActive (true);
			partBg [i].spriteName = QualityManagerment.qualityIDToIconSpriteName (es.qualityId);
			partIco [i].gameObject.SetActive (true);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + es.iconId, partIco [i]);
		}	 
	}
	public override void buttonEventBase (GameObject gameObj) { 
		base.buttonEventBase (gameObj);
		setStarTipState();
		if (gameObj.name == "close") {
			if (type != AWARDINTO) {
				if(CardManagerment.Instance.showChatEquips!=null)
				CardManagerment.Instance.showChatEquips.Clear ();
				finishWindow ();
			}
		
			if (closeCallback != null) {
				closeCallback ();
			}
		}
		//穿装备
		else if (gameObj.name == "equip") {
			EquipOperateFPort eof = FPortManager.Instance.getFPort ("EquipOperateFPort") as EquipOperateFPort;
			eof.access (EquipManagerment.Instance.activeEquipMan.uid, EquipManagerment.Instance.activeEquipMan.sid, chooseEquip.uid, chooseEquip.getPartId (), equipResult);
		}
		//脱装备
		else if (gameObj.name == "unsnatch") {
			EquipOperateFPort eof = FPortManager.Instance.getFPort ("EquipOperateFPort") as EquipOperateFPort;
			eof.access (EquipManagerment.Instance.activeEquipMan.uid, EquipManagerment.Instance.activeEquipMan.sid, "0", chooseEquip.getPartId (), equipResult);
		}
		//替换装备
		else if (gameObj.name == "replace") {
			UiManager.Instance.openWindow<EquipChooseWindow> ((win) => {
				win.Initialize (EquipChooseWindow.FROM_CARDATTR); 
			});
		}
		//强化装备
		else if (gameObj.name == "intensify") {
			if (GuideManager.Instance.isEqualStep (124005000)) {
				GuideManager.Instance.doGuide ();
			}
			UiManager.Instance.openWindow<IntensifyEquipWindow> ((win) => {
				if (type == STOREVIEW) {
					win.Initialize (chooseEquip, IntensifyEquipWindow.EQUIPSTORE); 
				}
				if (type == CARDVIEW) {
					win.Initialize (chooseEquip, IntensifyEquipWindow.EQUIPVIEW);
				}
			});
		}
		else if (gameObj.name == "upStar") {
		    if (upStarButton.GetComponent<ButtonBase>().textLabel.text ==
		        LanguageConfigManager.Instance.getLanguage("equipStar02"))
		    {
		        UiManager.Instance.openWindow<EquipUpStarWindow>((win) => {
		            win.Initialize(chooseEquip);
		        });
		    }else if (upStarButton.GetComponent<ButtonBase>().textLabel.text ==
		              LanguageConfigManager.Instance.getLanguage("redEquip_qualityImprove"))
		    {
                UiManager.Instance.openWindow<EquipUpQualityWindow>((win) =>
                {
                    win.Initialize(chooseEquip);
                });
		    }
		}
        else if (gameObj.name == "refine")//装备精练
        {
            UiManager.Instance.openWindow<RefineWindow>((win) =>
            {
                win.initialize(chooseEquip);
            });
        }
	} 

	//穿装备和脱装备成功后的回调
	public void equipResult (List<AttrChange> attrs, int types) { 
		if (fatherWindow is CardBookWindow) {
			if(CardManagerment.Instance.showChatEquips!=null)
			CardManagerment.Instance.showChatEquips.Clear ();
			finishWindow ();
			CardBookWindow win = fatherWindow as CardBookWindow;
			EventDelegate.Add (win.OnStartAnimFinish, () => {
				win.equipNewItem (attrs, types);
			}, true);
		}
		else {
			WindowBase tempFatherWindow = fatherWindow.GetFatherWindow ();
			fatherWindow.destoryWindow ();
			if(CardManagerment.Instance.showChatEquips!=null)
			CardManagerment.Instance.showChatEquips.Clear ();
			finishWindow ();
			if (tempFatherWindow is CardBookWindow) {
				CardBookWindow win = tempFatherWindow as CardBookWindow;
				EventDelegate.Add (win.OnStartAnimFinish, () => {
					win.equipNewItem (attrs, types);
				}, true);
			}
		}
		EquipManagerment.Instance.finishEquipChange ();
	}

	public override void OnNetResume () {
		Equip equip = StorageManagerment.Instance.getEquip (chooseEquip.uid);
		Card card = StorageManagerment.Instance.getRole (EquipManagerment.Instance.activeEquipMan.uid);
		Initialize (card, equip, type, closeCallback);
	}
	///<summary>
	/// 设置按钮位置
	/// </summary>
	public void setButtonPostion () {
        if ((fatherWindow.GetType() == typeof(CardBookWindow)) && UserManager.Instance.self.getUserLevel() >= equipStarOpenLevel && chooseEquip.getQualityId() >= 5 && chooseEquip.equpStarState < length) {
			intensifyButton.transform.localPosition = posLeft;
			upStarButton.transform.localPosition = posRight;
            upStarButton.GetComponent<ButtonBase>().textLabel.text = LanguageConfigManager.Instance.getLanguage("equipStar02");
            if (UserManager.Instance.self.getUserLevel() < equipRefineLevel && RefineSampleManager.Instance.getRefineSampleBySid(chooseEquip.sid) == null)
            {
                refineButton.gameObject.SetActive(false);
            }
            refineButton.transform.localPosition = posMiddle;
			drag.enabled = true;
			setEquipStarDes ();
		}
		else {
            if (chooseEquip.equpStarState == length || chooseEquip.getQualityId() >= 5) {
				drag.enabled = true;
				setEquipStarDes ();
			}
			else {			
				drag.enabled = false;
			}
            if ((fatherWindow.GetType() == typeof(CardBookWindow)) && UserManager.Instance.self.getUserLevel() >= equipRefineLevel)
            {
                intensifyButton.transform.localPosition = posLeft;
                refineButton.transform.localPosition = posRight;                                
            }
            if (UserManager.Instance.self.getUserLevel() < equipRefineLevel || RefineSampleManager.Instance.getRefineSampleBySid(chooseEquip.sid) == null)
            {
                refineButton.gameObject.SetActive(false);
                intensifyButton.transform.localPosition = posMiddle;
            }
            if (chooseEquip.equpStarState == length && EquipmentSampleManager.Instance.getEquipSampleBySid(chooseEquip.sid) != null
                && EquipmentSampleManager.Instance.getEquipSampleBySid(chooseEquip.sid).redEquipSid != 0)
		    {
                refineButton.transform.localPosition = posMiddle;
                upStarButton.transform.localPosition = posRight;
                upStarButton.GetComponent<ButtonBase>().textLabel.text = LanguageConfigManager.Instance.getLanguage("redEquip_qualityImprove");
		    }
		    else upStarButton.SetActive (false);
		}
	}
	///<summary>
	/// 装备满级设置按钮位置
	/// </summary>
	public void setButtonPostionFullLevel () {
		intensifyButton.gameObject.SetActive (false);
        if ((fatherWindow.GetType() == typeof(CardBookWindow)) && UserManager.Instance.self.getUserLevel() >= equipStarOpenLevel && chooseEquip.getQualityId() >= 5 && chooseEquip.equpStarState < length) {
            refineButton.transform.localPosition = posLeft;
			upStarButton.transform.localPosition = posRight;
            upStarButton.GetComponent<ButtonBase>().textLabel.text = LanguageConfigManager.Instance.getLanguage("equipStar02");
			drag.enabled = true;
			setEquipStarDes ();
		}
		else {
            if (chooseEquip.equpStarState == length || chooseEquip.getQualityId() >= 5) {
				drag.enabled = true;
				setEquipStarDes ();
			}
			else {
				drag.enabled = false;
			}
            if (chooseEquip.getQualityId() == 5 && EquipmentSampleManager.Instance.getEquipSampleBySid(chooseEquip.sid) != null
                && EquipmentSampleManager.Instance.getEquipSampleBySid(chooseEquip.sid).redEquipSid != 0)
		    {
                refineButton.transform.localPosition = posMiddle;
                upStarButton.transform.localPosition = posRight;
                upStarButton.GetComponent<ButtonBase>().textLabel.text = LanguageConfigManager.Instance.getLanguage("redEquip_qualityImprove");
		    } else 
		    upStarButton.SetActive (false);
		}
	}
	///<summary>
	/// 设置升星属性
	/// </summary>
	public void setEquipStarAttr ()
	{
	    if (chooseEquip.equpStarState <= 0)
	        equipStarShow = false;
	    else
	    {
	        equipStarShow = true;
	        setStars();
	        equipSample = EquipmentSampleManager.Instance.getEquipSampleBySid(chooseEquip.sid);
	        starSample = EquipStarAttrSampleManager.Instance.getEquipStarAttrSampleBySid(equipSample.equipStarSid);
	        starEffects = starSample.getAttrChangeSample(chooseEquip.equpStarState);
	        for (int i = 0; i < starEffects.Length; i++)
	        {
	            starAttr[i].gameObject.SetActive(true);
	            starAttr[i].text = starEffects[i].getAttrValue(0).ToString();
	            starAttrIcon[i].gameObject.SetActive(true);
	            starAttrIcon[i].spriteName = ("attr_" + starEffects[i].getAttrType());
	        }
	    }
	}
	///<summary>
	/// 设置星星
	/// </summary>
	public void setStars () {
		int i = 0;
		float tmp = chooseEquip.equpStarState % 2 == 1 ? 0f : 25f;
		bool add = false;
		while (i < chooseEquip.equpStarState) {
            if(chooseEquip.getQualityId() == 5)
			    stars [i].transform.localPosition = new Vector3 (add ? tmp - (i + 1) / 2 * 50f : tmp + (i + 1) / 2 * 50f, 414f, 0f);
			add = !add;
		    if (chooseEquip.getQualityId() == 6)
		    {
		        if (i < stars.Length)
		            stars[i].transform.localPosition = new Vector3(-200 + i*50, 414, 0);
		    }
		    if (i - 9 >= 0)
		        stars[i-9].GetComponent<UISprite>().spriteName = "red_Star";
		    if (i < stars.Length)
		        stars[i++].SetActive(true);
		    else i++;
		}
		while (i < 9) {
			stars [i++].SetActive (false);
		}
	} 
	///<summary>
	/// 更新属性描述
	/// </summary>
	public void onCenterItem (GameObject obj) {
		int index = StringKit.toInt (obj.name) - 1;
		SuitSample tmp;
		if (index != 0) {
			tmp = SuitSampleManager.Instance.getSuitSampleBySid (chooseEquip.getSuitSid () * 100 + index);
		}
		else {
			tmp = SuitSampleManager.Instance.getSuitSampleBySid (chooseEquip.getSuitSid ());
		}
		if (isEquipStarSuitTrue (index)) {
			suitTitle.text = QualityManagerment.getQualityColor (chooseEquip.getQualityId ()) + tmp.name + "[FFFFFF]" + LanguageConfigManager.Instance.getLanguage ("s0010") + ("[3A9663]" + LanguageConfigManager.Instance.getLanguage ("equipStar08") + "[-]");
			desInfo.text = LanguageConfigManager.Instance.getLanguage ("equipStar15", index.ToString (), index.ToString ());
		}
		else if (index > 0) {
			suitTitle.text = QualityManagerment.getQualityColor (chooseEquip.getQualityId ()) + tmp.name + "[FFFFFF]" + LanguageConfigManager.Instance.getLanguage ("s0010") + ("[FF0000]" + LanguageConfigManager.Instance.getLanguage ("equipStar12") + "[-]");
			desInfo.text = LanguageConfigManager.Instance.getLanguage("equipStar15",index.ToString(),index.ToString());
		}else {
			desInfo.text = LanguageConfigManager.Instance.getLanguage("equipStar14");
			suitTitle.text = QualityManagerment.getQualityColor (chooseEquip.getQualityId ()) + tmp.name + "[FFFFFF]" + LanguageConfigManager.Instance.getLanguage ("s0010");
		}
		findAllEffect (tmp, index, index);
		if (chooseEquip.getQualityId () >= 5) {
            if (index == length)
				rightArrow.SetActive (false);
			else
				rightArrow.SetActive (true);
			if (index == 0)
				leftArrow.SetActive (false);
			else
				leftArrow.SetActive (true);
		}
		else {
			leftArrow.SetActive (false);
			rightArrow.SetActive (false);
		}
	}
	///<summary>
	/// 设置升星描述
	/// </summary>
	public void setEquipStarDes () {
            //Utils.RemoveAllChild(sampleContent.transform);
	    if (sampleContent.transform.childCount <= 0)
	    {
	        for (int i = 0; i < length; i++)
	        {
	            GameObject obj = NGUITools.AddChild(sampleContent.gameObject, emptyGameObject);
	            if (i + 1 < 10)
	                obj.name = "00" + (i + 1);
	            else
	                obj.name = "01" + (i - 9);
	        }
	    }
	    sampleContent.startIndex = CardManagerment.Instance.getEquipStarLevel (chooseCard);
	    sampleContent.maxCount = length + 1;
		sampleContent.callbackUpdateEach = null;
		sampleContent.onLoadFinish = null;
		sampleContent.onCenterItem = onCenterItem;
		sampleContent.init ();
	}
	///<summary>
	/// 判断是否已经激活升星套装效果
	/// </summary>
	private bool isEquipStarSuitTrue (int starLevel) {
		if (starLevel == 0)
			return false;
		if (chooseCard != null) {
			string[] sids = chooseCard.getEquips ();
			Equip curEquip;
			if (sids.Length != 5)
				return false;
			for (int i=0; i<sids.Length; i++) {
				if (CardManagerment.Instance.showChatEquips != null && CardManagerment.Instance.showChatEquips.Count > 0) {
					curEquip = CardManagerment.Instance.showChatEquips[i];
				}
				else {
					curEquip = StorageManagerment.Instance.getEquip (sids [i]);
				}
				if (isInSuit (curEquip)) {
					if (curEquip.equpStarState < starLevel)
						return false;
				}
				else {
					return false;
				}
			}
			return true;
		}
	    return false;
	}
	///<summary>
	/// 判断当前装备是否属于套装
	/// </summary>
	private bool isInSuit (Equip curEquip) {
		if (chooseEquip.getQualityId () >= 5 && chooseEquip.equpStarState > 0) {
			basicSuit = SuitSampleManager.Instance.getSuitSampleBySid (chooseEquip.getSuitSid () * 100 + chooseEquip.equpStarState); 
		}
		else {
			basicSuit = SuitSampleManager.Instance.getSuitSampleBySid (chooseEquip.getSuitSid ()); 
		}
		for (int i=0; i<5; i++) {
            if (curEquip.sid == basicSuit.parts[i].ySid || curEquip.sid == basicSuit.parts[i].rSid)
				return true;
		}
		return false;
	}
	public void updateEquipStarStateTip () {
        starLevelState[0].text = (chooseEquip.equpStarState == 0 ? "" : "[FF0000]" + chooseEquip.equpStarState + LanguageConfigManager.Instance.getLanguage("star_star_star"));
        starLevelState[0].gameObject.SetActive(chooseEquip.equpStarState != 0);
        refineLevelState[0].text = (chooseEquip.getrefineLevel() == 0 ? "" : "[FF0000]" + chooseEquip.getrefineLevel() + LanguageConfigManager.Instance.getLanguage("refine_024"));
        refineLevelState[0].gameObject.SetActive(chooseEquip.getrefineLevel() != 0);
		if (chooseCard != null) {
			if (chooseEquip.getQualityId () >= 5 && chooseEquip.equpStarState > 0) {
				basicSuit = SuitSampleManager.Instance.getSuitSampleBySid (chooseEquip.getSuitSid () * 100 + chooseEquip.equpStarState); 
			}
			else {
				basicSuit = SuitSampleManager.Instance.getSuitSampleBySid (chooseEquip.getSuitSid ()); 
			}
		}
	}
	private void setStarTipState(){
        starLevelState[0].gameObject.SetActive(false);
        refineLevelState[0].gameObject.SetActive(false);
	}
    public string showRefineType(string str)
    {
        if (str == AttrChangeType.HP)
        {
            return (LanguageConfigManager.Instance.getLanguage("refine_008"));
        }
        if (str == AttrChangeType.ATTACK)
        {
            return (LanguageConfigManager.Instance.getLanguage("refine_019"));
        }
        switch (str)
        {
            case AttrChangeType.DEFENSE:
                return (LanguageConfigManager.Instance.getLanguage("refine_018"));
            case AttrChangeType.AGILE:
                return (LanguageConfigManager.Instance.getLanguage("refine_016"));
            case AttrChangeType.MAGIC:
                return (LanguageConfigManager.Instance.getLanguage("refine_010"));
            case AttrChangeType.PER_AGILE:
                return (LanguageConfigManager.Instance.getLanguage("refine_016"));
            case AttrChangeType.PER_ATTACK:
                return (LanguageConfigManager.Instance.getLanguage("refine_019"));
            case AttrChangeType.PER_DEFENSE:
                return (LanguageConfigManager.Instance.getLanguage("refine_018"));
            case AttrChangeType.PER_HP:
                return (LanguageConfigManager.Instance.getLanguage("refine_008"));
            case AttrChangeType.PER_MAGIC:
                return (LanguageConfigManager.Instance.getLanguage("refine_010"));
            case AttrChangeType.DESC1:
                return (LanguageConfigManager.Instance.getLanguage("refine_013"));
            case AttrChangeType.DESC2:
                return (LanguageConfigManager.Instance.getLanguage("refine_021"));
            case AttrChangeType.DESC3:
                return (LanguageConfigManager.Instance.getLanguage("refine_014"));
            default:
                return (LanguageConfigManager.Instance.getLanguage("refine_021"));
        }
    }
    private void resetArenaInfo() {
        //bugfix 窗口隐藏时候不要更新,会报错
        if (this == null || !gameObject.activeInHierarchy) {
            if (updateInfoTimer != null) {
                updateInfoTimer.stop();
                updateInfoTimer = null;
            }
            return;
        }
        showStarEffect();
    }
    private void showStarEffect() {
        if (canStar) return;
        if (equipStarShow || equipRefineShow) centerPoint.transform.localPosition = Vector3.zero;
        else centerPoint.transform.localPosition=new Vector3(145f,0f,0f);
        if (equipStarShow && !equipRefineShow)//只有升星 没有精炼
        {
            canStar = true;
            //showEquipStar();
            starLevelState[0].alpha = 1;
            equipStarObj.SetActive(true);
            StartCoroutine(Utils.DelayRun(() => {
              canStar = false;
            }, 2f));
            //只有精炼
        } else if (!equipStarShow && equipRefineShow)
        {
            canStar = true;
            refineLevelState[0].alpha = 1;
            //showEquipRefine();
            if (!shuxingLook.gameObject.activeSelf) shuxingLook.gameObject.SetActive(true);
            StartCoroutine(Utils.DelayRun(() => {
                canStar = false;
               // shuxingLook.gameObject.SetActive(false);
            }, 2f));

        } else if (equipStarShow && equipRefineShow) //同时闪烁
        {
            canStar = true;
            showEquipStar();
            StartCoroutine(Utils.DelayRun(() =>
            {
                showEquipRefine();
            }, 6.5f));
            StartCoroutine(Utils.DelayRun(() =>
            {
                canStar = false;
               // shuxingLook.gameObject.SetActive(false);
            }, 13f));
        }
        else
        {
            canStar = true;
        }
    }
    /// <summary>
    /// 闪烁升星
    /// </summary>
    private void showEquipStar()
    {
        TweenAlpha tp0 = TweenAlpha.Begin(starLevelState[0].gameObject, 1f, 1);
        tp0.from = 0;
        StartCoroutine(Utils.DelayRun(() => {
            TweenAlpha tp1 = TweenAlpha.Begin(starLevelState[0].gameObject, 1f, 0);
            tp1.from = 1;
        }, 5f));
        equipStarObj.SetActive(true);
        for (int j = 0; j < starTtile.Length; j++) {
            TweenAlpha tp = TweenAlpha.Begin(starTtile[j].gameObject, 1f, 1);
            tp.from = 0;
        }
        StartCoroutine(Utils.DelayRun(() => {
            for (int j = 0; j < starTtile.Length; j++) {
                TweenAlpha tp = TweenAlpha.Begin(starTtile[j].gameObject, 1f, 0);
                tp.from = 1;
            }
        }, 5f));
        for (int i = 0; i < starAttr.Length; i++) {
            if (starAttr[i].gameObject.activeSelf) {
                TweenAlpha tp = TweenAlpha.Begin(starAttr[i].gameObject, 1f, 1);
                tp.from = 0;
                int i2 = i;
                StartCoroutine(Utils.DelayRun(() => {
                    TweenAlpha tp1 = TweenAlpha.Begin(starAttr[i2].gameObject, 1f, 0);
                    tp1.from = 1;
                }, 5f));
                TweenAlpha tp2 = TweenAlpha.Begin(starAttrIcon[i].gameObject, 1f, 1);
                tp2.from = 0;
                int i1 = i;
                StartCoroutine(Utils.DelayRun(() => {
                    TweenAlpha tp3 = TweenAlpha.Begin(starAttrIcon[i1].gameObject, 1f, 0);
                    tp3.from = 1;
                }, 5f));
            }
        }
    }
    /// <summary>
    /// 闪烁精炼
    /// </summary>
    private void showEquipRefine()
    {
        TweenAlpha tp = TweenAlpha.Begin(refineLevelState[0].gameObject, 1f, 1);
        tp.from = 0;
        StartCoroutine(Utils.DelayRun(() => {
            TweenAlpha tp1 = TweenAlpha.Begin(refineLevelState[0].gameObject, 1f, 0);
            tp1.from = 1;
        }, 5));
        if (!shuxingLook.gameObject.activeSelf) shuxingLook.gameObject.SetActive(true);
        TweenAlpha tp7 = TweenAlpha.Begin(refinTitle.gameObject, 1f, 1);
        tp7.from = 0;
        TweenAlpha tp5 = TweenAlpha.Begin(shuxingdengji.gameObject, 1f, 1);
        tp5.from = 0;
        TweenAlpha tp6 = TweenAlpha.Begin(shuxingdengji2.gameObject, 1f, 1);
        tp6.from = 0;
        for (int i = 0; i < shuxing.Length; i++) {
            if (shuxing[i].gameObject.activeSelf) {
                TweenAlpha tp55 = TweenAlpha.Begin(shuxing[i].gameObject, 1f, 1);
                tp55.from = 0;
                TweenAlpha tp66 = TweenAlpha.Begin(shuxing2[i].gameObject, 1f, 1);
                tp66.from = 0;
            }
        }
        StartCoroutine(Utils.DelayRun(() => {
            TweenAlpha tp77 = TweenAlpha.Begin(refinTitle.gameObject, 1f, 0);
            tp77.from = 1;
            TweenAlpha tp555 = TweenAlpha.Begin(shuxingdengji.gameObject, 1f, 0);
            tp555.from = 1;
            TweenAlpha tp666 = TweenAlpha.Begin(shuxingdengji2.gameObject, 1f, 0);
            tp666.from = 1;
            for (int i = 0; i < shuxing.Length; i++) {
                if (shuxing[i].gameObject.activeSelf) {
                    TweenAlpha tp55 = TweenAlpha.Begin(shuxing[i].gameObject, 1f, 0);
                    tp55.from = 1;
                    TweenAlpha tp66 = TweenAlpha.Begin(shuxing2[i].gameObject, 1f, 0);
                    tp66.from = 1;
                }
            }
        }, 5f));
    }
}