using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipUpQualityWindow : WindowBase
{ 
	/** 位置 */
	private Vector3 pos8 = new Vector3 (-70.6f,0f,0f);
	public GameObject attrLeft;
	public GameObject attrRight;
	public GameObject arrowObj;
	//**star pos*/
	private float starPos = 0.0f;
	private float starPos2 = 25.0f;
	/** 装备*/
	public  UITexture equipImage;
	public  UISprite  quality;
	public  UILabel   equipName;
	/** 属性*/
	public  UILabel newAttr;
	public  UILabel[] leftAttr;
	public  UILabel[] rightAttr;
	public  GameObject[] leftAttrPos;
	public  GameObject[] rightAttrPos;
	public  UISprite[] leftAttrIcon;
	public  UISprite[] rightAttrIcon;
	/** 消耗*/
    public UILabel[] costInfoLabels;
	/** 星星*/
	public GameObject[] stars;
	/** 升星按钮*/
	public ButtonBase equipStarButton;
	//* 升星状态显示*/
	public UILabel starLevelState;
	//** 特效*/
	public GameObject effectObj;
	//***/
    public GameObject goodsViewFather;
	public GoodsView[] consumeGoods;
    public UILabel titleLabel;
	/** 升星装备*/
	private Equip selectedEquip;
	/** 属性效果 */
	private AttrChangeSample[] oldEffects;
	private AttrChangeSample[] newEffects;
	/** 消耗信息*/
	private PrizeSample[] consumeInfo;
	private bool needRefresh = false;
	private bool sendMessage = true;

	protected override void begin ()
	{
		base.begin ();
		updateEquipStarStateTip ();
		MaskWindow.UnlockUI ();
	}
	public void Initialize (Equip chooseItem)
	{ 
		this.selectedEquip = chooseItem;
		updateEquip ();
        updateButton();
	    updateTitle();
		updateEquipStarAttributes ();
		setStars ();
		getConsumeInfo ();
	}

    /// <summary>
    /// 断线重连
    /// </summary>
    public override void OnNetResume()
    {
        base.OnNetResume();
        this.selectedEquip = StorageManagerment.Instance.getEquip(selectedEquip.uid);
        Initialize(selectedEquip);
        sendMessage = true;
    }

    void Update ()
	{
		if (starLevelState != null && starLevelState.gameObject != null && starLevelState.gameObject.activeSelf)
			starLevelState.alpha = sin ();
	}
	public override void buttonEventBase (GameObject gameObj)
	{ 
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			starLevelState.gameObject.SetActive(false);
			finishWindow();
		}

		if (gameObj.name == "upStar") {
			if(sendMessage){
                if (equipStarButton.textLabel.text == LanguageConfigManager.Instance.getLanguage("redEquip_advance"))//进阶
                {
                    consumeInfo = EquipmentSampleManager.Instance.getEquipSampleBySid(selectedEquip.sid).upQualityCost;
                    bool canAdvance = false;
                    if (consumeInfo != null)
                    {
                        for (int i = 0; i < consumeInfo.Length; i++)
                        {
                            if (consumeInfo[i].type == PrizeType.PRIZE_MONEY)
                            {
                                if (StringKit.toInt(consumeInfo[i].num) <= UserManager.Instance.self.getMoney())
                                    canAdvance = true;
                            } else if (consumeInfo[i].type == PrizeType.PRIZE_PROP) {
                                Prop tmp = StorageManagerment.Instance.getProp(consumeInfo[i].pSid);
                                if (tmp != null && tmp.getNum() >= StringKit.toInt(consumeInfo[i].num))
                                    canAdvance = true;
                            }
                        }
                        if (!canAdvance)
                        {
                            UiManager.Instance.openDialogWindow<TextTipWindow>((win) =>
                            {
                                win.init(LanguageConfigManager.Instance.getLanguage("redEquip_noResource"), 0.5f);
                            });
                            return;
                        }
                    }
                    UpQualityFPort fport = FPortManager.Instance.getFPort("UpQualityFPort") as UpQualityFPort;
			        fport.sendMessage(selectedEquip.uid, getEquipStarResult);
			        sendMessage = false;
                } else if (equipStarButton.textLabel.text == LanguageConfigManager.Instance.getLanguage("s0093"))//确定后返回卡片信息界面
                {
                    starLevelState.gameObject.SetActive(false);
                    finishWindow();
                }
			}
			MaskWindow.UnlockUI();
		}
	}
	public void updateEquip ()
	{
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + selectedEquip.getIconId (), equipImage);
		quality.spriteName = QualityManagerment.qualityIDToIconSpriteName (selectedEquip.getQualityId ());
		equipName.text = selectedEquip.getName ();
	}

    public void updateTitle()
    {
        if (selectedEquip.getQualityId() == 5)
            titleLabel.text = LanguageConfigManager.Instance.getLanguage("redEquip_advanceAttr");
        else if (selectedEquip.getQualityId() == 6)
        {
            titleLabel.text = LanguageConfigManager.Instance.getLanguage("equipAttrWindow_title");
        }
    }

    public void updateButton()
    {
        string[] strs = null;
        if (selectedEquip != null) {
            EquipSample sampleTmp = EquipmentSampleManager.Instance.getEquipSampleBySid(selectedEquip.sid);
            EquipStarAttrSample sample = sampleTmp == null ? null : EquipStarAttrSampleManager.Instance.getEquipStarAttrSampleBySid(sampleTmp.equipStarSid);
            if (sample != null) {
                strs = sample.equipStarAtr;
            }
        }
        if (selectedEquip.getQualityId() == 5 && strs != null && selectedEquip.equpStarState == strs.Length)
            equipStarButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("redEquip_advance");
        else if(selectedEquip.getQualityId() == 6)
            equipStarButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0093");
    }

    ///<summary>
	/// 设置星星
	/// </summary>
	public void setStars(){
		int i = 0;
		float tmp = selectedEquip.equpStarState % 2 == 1 ? starPos : starPos2;
		bool add = false;
		while (i < selectedEquip.equpStarState) {
			stars[i].transform.localPosition = new Vector3(add?tmp - (i+1)/2*50f :tmp + (i+1)/2*50f ,414f,0f);
			add = !add;
			stars[i++].SetActive(true);
		}
		while (i < 9) {
			stars[i++].SetActive(false);
		}
	} 
    /// <summary>
    /// 获取升红后装备的属性
    /// </summary>
    /// <param name="level"></param>
    /// <param name="sid"></param>
    /// <returns></returns>
    public AttrChange[] getAttrChanges(int level,int sid) {
        List<AttrChange> list = new List<AttrChange>();
        int hp = EquipManagerment.Instance.getEquipAttribute(sid, level, AttributeType.hp);
        if (hp != 0) {
            AttrChange attrHp = new AttrChange(AttrChangeType.HP, hp);
            list.Add(attrHp);
        }

        int attack = EquipManagerment.Instance.getEquipAttribute(sid, level, AttributeType.attack);
        if (attack != 0) {
            AttrChange attrAtt = new AttrChange(AttrChangeType.ATTACK, attack);
            list.Add(attrAtt);
        }

        int agi = EquipManagerment.Instance.getEquipAttribute(sid, level, AttributeType.agile);
        if (agi != 0) {
            AttrChange attrAgi = new AttrChange(AttrChangeType.AGILE, agi);
            list.Add(attrAgi);
        }

        int mag = EquipManagerment.Instance.getEquipAttribute(sid, level, AttributeType.magic);
        if (mag != 0) {
            AttrChange attrMag = new AttrChange(AttrChangeType.MAGIC, mag);
            list.Add(attrMag);
        }

        int def = EquipManagerment.Instance.getEquipAttribute(sid, level, AttributeType.defecse);
        if (def != 0) {
            AttrChange attrDef = new AttrChange(AttrChangeType.DEFENSE, def);
            list.Add(attrDef);
        }
        return list.ToArray();
    }

	///<summary>
	/// 更新属性说明（根据品质判断显示的属性信息）
	/// </summary>
	public void updateEquipStarAttributes(){
		newAttr.gameObject.SetActive(false);
	    int sid = EquipmentSampleManager.Instance.getNextQualityEquipSampleSid(selectedEquip.sid);
	    EquipSample redEquipSample = EquipmentSampleManager.Instance.getEquipSampleBySid(sid);

        AttrChange[] attrs = selectedEquip.getAttrChanges();
	    if (selectedEquip.getQualityId() == 5)
	    {
            if (attrs != null) {
                if (attrs.Length > 0 && attrs[0] != null) {
                    leftAttr[1].gameObject.SetActive(true);
                    leftAttr[1].text = attrs[0].num.ToString();
                    leftAttrIcon[1].gameObject.SetActive(true);
                    leftAttrIcon[1].spriteName = ("attr_" + attrs[0].type);
                }
                if (attrs.Length > 1 && attrs[1] != null) {
                    leftAttr[2].gameObject.SetActive(true);
                    leftAttr[2].text = attrs[1].num.ToString();
                    leftAttrIcon[2].gameObject.SetActive(true);
                    leftAttrIcon[2].spriteName = ("attr_" + attrs[1].type);
                }
            }
	        if (redEquipSample != null)
	        {
	            AttrChange[] attrsNew = getAttrChanges(selectedEquip.level, redEquipSample.sid);//获取升阶后红色装备的属性信息
                if (attrsNew != null) {
                    if (attrsNew.Length > 0 && attrsNew[0] != null) {
                        rightAttr[1].gameObject.SetActive(true);
                        rightAttr[1].text = attrs[0].num + "[3a9663]" + "+" + (attrsNew[0].num - attrs[0].num) + "[-]"; 
                        rightAttrIcon[1].gameObject.SetActive(true);
                        rightAttrIcon[1].spriteName = ("attr_" + attrsNew[0].type);
                    }
                    if (attrsNew.Length > 1 && attrsNew[1] != null) {
                        rightAttr[2].gameObject.SetActive(true);
                        rightAttr[2].text = attrs[1].num + "[3a9663]" + "+" + (attrsNew[1].num - attrs[1].num) + "[-]"; 
                        rightAttrIcon[2].gameObject.SetActive(true);
                        rightAttrIcon[2].spriteName = ("attr_" + attrsNew[1].type);
                    }
                }
	        }
	    } else if (selectedEquip.getQualityId() == 6)
        {
            attrLeft.transform.localPosition = pos8;
            arrowObj.SetActive(false);
            attrRight.SetActive(false);
            for (int i = 0; i < leftAttr.Length; i++)
            {
                leftAttr[i].gameObject.SetActive(false);
                leftAttrIcon[i].gameObject.SetActive(false);
            }
            if (attrs != null) {
                if (attrs.Length > 0 && attrs[0] != null) {
                    leftAttr[0].gameObject.SetActive(true);
                    leftAttr[0].text = attrs[0].num.ToString();
                    leftAttrIcon[0].gameObject.SetActive(true);
                    leftAttrIcon[0].spriteName = ("attr_" + attrs[0].type);
                }
                if (attrs.Length > 1 && attrs[1] != null) {
                    leftAttr[1].gameObject.SetActive(true);
                    leftAttr[1].text = attrs[1].num.ToString();
                    leftAttrIcon[1].gameObject.SetActive(true);
                    leftAttrIcon[1].spriteName = ("attr_" + attrs[1].type);
                }
            }
        }
    }

	///<summary>
	/// 获得升阶消耗信息
	/// </summary>
	public void getConsumeInfo()
	{
	    if (selectedEquip == null) 
            return;
	    consumeInfo = EquipmentSampleManager.Instance.getEquipSampleBySid(selectedEquip.sid).upQualityCost;
	    if (consumeInfo == null)
	    {
	        for (int i = 0; i < costInfoLabels.Length; i++)
	            costInfoLabels[i].text = "";
	        return;
	    }
	    for (int i = 0; i< consumeGoods.Length;i++)
	    {
            consumeGoods[i].gameObject.SetActive(false);
	    }
        goodsViewFather.transform.localPosition = new Vector3(95, -32, 0);
        if(consumeInfo.Length > 1)
            goodsViewFather.transform.localPosition = new Vector3(0,-32,0);
	    int MYRAID = 10000;
	    for (int i = 0; i < consumeInfo.Length; i++)
	    {
            consumeGoods[i].gameObject.SetActive(true);
	        consumeGoods[i].init(consumeInfo[i]);
	        consumeGoods[i].rightBottomText.text = "";
	        if (consumeInfo[i].type == PrizeType.PRIZE_MONEY)
	        {
	            int costNum = 0;
	            costNum = StringKit.toInt(consumeInfo[i].num);
                costInfoLabels[i].text = ((int)costNum > UserManager.Instance.self.getMoney() ? Colors.RED : Colors.GREEN) +
                            (UserManager.Instance.self.getMoney() >= MYRAID
                                ? (UserManager.Instance.self.getMoney() / MYRAID) + "W"
                                : UserManager.Instance.self.getMoney() +"")+ "/" +
                                  (costNum >= MYRAID ? (costNum / MYRAID) + "W" : costNum + "");
	        } else if (consumeInfo[i].type == PrizeType.PRIZE_PROP)
            {
                Prop tmp = StorageManagerment.Instance.getProp(consumeInfo[i].pSid);
                costInfoLabels[i].text = ((StringKit.toInt(consumeInfo[i].num) > (tmp == null ? 0 : tmp.getNum()))
                    ? Colors.RED
                    : Colors.GREEN) + (tmp == null ? 0 : tmp.getNum()) + "/" + consumeInfo[i].num;
                if (tmp == null)
                    costInfoLabels[i].text = Colors.RED + "0/" + consumeInfo[i].num;
            }
	    }
    }
	///<summary>
	/// 处理提升品质结果
	/// </summary>
	public void getEquipStarResult(bool isTrue){
		if (isTrue)
		{
		    selectedEquip = StorageManagerment.Instance.getEquip(selectedEquip.uid);
			updateEquip ();
			updateEquipStarAttributes ();
			getConsumeInfo ();
		    updateButton();
		    updateTitle();
			effectObj.SetActive(true);
			AudioManager.Instance.PlayAudio (147);
			StartCoroutine(Utils.DelayRun(()=>{
				AudioManager.Instance.PlayAudio (138);
			},1.5f));
			StartCoroutine(Utils.DelayRun(()=>{
				stars[0].SetActive(true);
				effectObj.SetActive(false);
				sendMessage = true;
			},2f));
			MaskWindow.UnlockUI ();
		}
		else {
			MaskWindow.UnlockUI();
            sendMessage = true;
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("redEquip_advanceFail"));
		}
	}
	public void updateEquipStarStateTip(){
		starLevelState.gameObject.SetActive (true);
		starLevelState.text = (selectedEquip.equpStarState == 0 ? "" : "[FF0000]+" + selectedEquip.equpStarState.ToString ());
	}
}
