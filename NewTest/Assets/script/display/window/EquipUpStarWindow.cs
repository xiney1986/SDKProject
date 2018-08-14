using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipUpStarWindow : WindowBase
{ 
	/** 位置 */
	private Vector3 pos1 = new Vector3 (66f, 240f, 0f);
	private Vector3 pos2 = new Vector3 (66f, 180f, 0f);
	private Vector3 pos3 = new Vector3 (66f, 120f, 0f);
	private Vector3 pos4 = new Vector3 (-196f,-28f,0f);
	private Vector3 pos5 = new Vector3 (56f,-28f,0f);
	private Vector3 pos6 = new Vector3 (-196f,0f,0f);
	private Vector3 pos7 = new Vector3 (56f,0f,0f);
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
	public  UILabel consumeCrystal;
	public  UILabel consumeStone;
    public UILabel consumeMoney;
	/** 消耗Icon*/
	public  UITexture crystalIcon;
	public  UITexture stoneIcon;
	/** 星星*/
	public GameObject[] stars;
	/** 升星按钮*/
	public ButtonBase equipStarButton;
	//* 升星状态显示*/
	public UILabel starLevelState;
	//** 特效*/
	public GameObject effectObj;
	//** 特效*/
	public GameObject effectStar;
    public GameObject[] moveStarEffects;
	//***/
	public GoodsView[] consumeGoods;
    public GameObject goodsViewFather;
	/** 升星装备*/
	private Equip selectedEquip;
	/** 升星属性模板*/
	private EquipStarAttrSample starSample;
	/** 装备模板*/
	private EquipSample equipSample;
	/** 属性效果 */
	private AttrChangeSample[] oldEffects;
	private AttrChangeSample[] newEffects;
	/** 消耗信息*/
	private int[] consumeInfo;
	private bool needRefresh = false;
	private bool sendMessage = true;
    private string[] str;
    private int length;
	protected override void begin ()
	{
		base.begin ();
		updateEquipStarStateTip ();
		MaskWindow.UnlockUI ();
	}
	public void Initialize (Equip chooseItem)
	{ 
		this.selectedEquip = chooseItem;
		equipSample = EquipmentSampleManager.Instance.getEquipSampleBySid (selectedEquip.sid);
		starSample = EquipStarAttrSampleManager.Instance.getEquipStarAttrSampleBySid(equipSample.equipStarSid);
        if (starSample != null) {
            str = starSample.equipStarAtr;
            length = str.Length;
        } else
            length = 9;
		updateEquip ();
        updateButton();
		updateEquipStarAttributes ();
		setStars ();
		getConsumeInfo ();
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

		if (gameObj.name == "upStar")
		{
		    if (selectedEquip == null)
		        return;
		    EquipSample sampleTmp = EquipmentSampleManager.Instance.getEquipSampleBySid(selectedEquip.sid);
            EquipSample equipSample = sampleTmp == null? null:EquipmentSampleManager.Instance.getEquipSampleBySid(sampleTmp.redEquipSid);
            if (selectedEquip.equpStarState == length && equipSample == null) {//达到最高星级并且不可升阶
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("equipStar11"));
				return;
			}
			if(sendMessage){

                if (equipStarButton.textLabel.text == LanguageConfigManager.Instance.getLanguage("equipStar02"))
			    {
			        EquipStarFport fport = FPortManager.Instance.getFPort("EquipStarFport") as EquipStarFport;
			        fport.sendMessage(selectedEquip.uid, getEquipStarResult);
			        sendMessage = false;
                } else if (equipStarButton.textLabel.text == LanguageConfigManager.Instance.getLanguage("redEquip_qualityImprove"))
                {
                    UiManager.Instance.openWindow<EquipUpQualityWindow>((win) =>
                    {
                        win.Initialize(selectedEquip);
                    });
                }
			}
			//MaskWindow.UnlockUI();
		}
	}
	public void updateEquip ()
	{
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + selectedEquip.getIconId (), equipImage);
		quality.spriteName = QualityManagerment.qualityIDToIconSpriteName (selectedEquip.getQualityId ());
		equipName.text = selectedEquip.getName ();
	}

    public void updateButton()
    {
        if (selectedEquip != null) {
            EquipSample sampleTmp = EquipmentSampleManager.Instance.getEquipSampleBySid(selectedEquip.sid);
            EquipSample equipTmpSample = sampleTmp == null ? null : EquipmentSampleManager.Instance.getEquipSampleBySid(sampleTmp.redEquipSid);
            if (equipTmpSample != null && selectedEquip.equpStarState == length) {
                equipStarButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("redEquip_qualityImprove");
            }else
                equipStarButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("equipStar02");
        }
    }

    ///<summary>
	/// 设置星星
	/// </summary>
	public void setStars(){
		int i = 0;
		float tmp = selectedEquip.equpStarState % 2 == 1 ? starPos : starPos2;
		bool add = false;
		while (i < selectedEquip.equpStarState) {
            if(selectedEquip.getQualityId() == 5 ||(selectedEquip.getQualityId() == 6 && selectedEquip.equpStarState <= 9))
			    stars[i].transform.localPosition = new Vector3(add?tmp - (i+1)/2*50f :tmp + (i+1)/2*50f ,414f,0f);
			add = !add;
		    if (selectedEquip.getQualityId() == 6 && selectedEquip.equpStarState > 9)
		    {
                if(i < stars.Length)
		            stars[i].transform.localPosition = new Vector3(-200 + i*50, 414, 0);
		    }
		    if (i - 9 >= 0)
                stars[i-9].GetComponent<UISprite>().spriteName = "red_Star";
            if (i < stars.Length)
			    stars[i++].SetActive(true);
            else 
                i++;
		}
        while (i < 9) {
			stars[i++].SetActive(false);
		}
	} 
	///<summary>
	/// 移动星星
	/// </summary>
	private void moveStars(){
		float tmp = selectedEquip.equpStarState % 2 == 1 ? starPos : starPos2;
		bool add = false;
		int i = 8;
		while(i >= selectedEquip.equpStarState) {
			stars[i--].SetActive(false);
		}
		while (i >= 0) {
			stars[i].transform.localPosition = new Vector3(add?tmp + (i+1)/2*50f :tmp - (i+1)/2*50f ,414f,0f);
			add = !add;
			stars[i--].SetActive(true);
		}
		stars [0].SetActive (false);
	}

	///<summary>
	/// 更新升星属性说明
	/// </summary>
	public void updateEquipStarAttributes(){
		newAttr.gameObject.SetActive(false);
		if (selectedEquip.equpStarState == 0) {
			newEffects = starSample.getAttrChangeSample (1);
			newAttr.gameObject.SetActive (true);
			newAttr.transform.localPosition = pos2;
			newAttr.text = (selectedEquip.equpStarState + 1) + LanguageConfigManager.Instance.getLanguage ("equipStar06");
			rightAttr [1].gameObject.SetActive (true);
			rightAttr [1].text = "[3a9663]" + "+" + newEffects [0].getAttrValue (0).ToString () + "[-]";
			rightAttrIcon [1].gameObject.SetActive (true);
			rightAttrIcon [1].spriteName = ("attr_" + newEffects [0].getAttrType ());
		}
		else if (selectedEquip.equpStarState < length) {
			oldEffects = starSample.getAttrChangeSample (selectedEquip.equpStarState);
			newEffects = starSample.getAttrChangeSample (selectedEquip.equpStarState + 1);
			int index = selectedEquip.equpStarState;
			int i;
			if(index < 3){
				i = 0;
				leftAttr [1].gameObject.SetActive (true);
				leftAttr [1].text = oldEffects [i].getAttrValue (0).ToString ();
				leftAttrIcon [1].gameObject.SetActive (true);
				leftAttrIcon [1].spriteName = ("attr_" + oldEffects [i].getAttrType ());
				rightAttr [1].gameObject.SetActive (true);
				rightAttr [1].text = oldEffects [i].getAttrValue (0).ToString () + "[3a9663]" + "+" + (newEffects [i].getAttrValue (0) - oldEffects [i].getAttrValue (0)) + "[-]";
				rightAttrIcon [1].gameObject.SetActive (true);
				rightAttrIcon [1].spriteName = ("attr_" + newEffects [i].getAttrType ());
            } else if (index == 3 || index == 9) {
				attrLeft.transform.localPosition = pos4;
				attrRight.transform.localPosition = pos5;
				for (i=0; i < oldEffects.Length; i++) {
					leftAttr [i].gameObject.SetActive (true);
					leftAttr [i].text = oldEffects [i].getAttrValue (0).ToString ();
					leftAttrIcon [i].gameObject.SetActive (true);
					leftAttrIcon [i].spriteName = ("attr_" + oldEffects [i].getAttrType ());
					rightAttr [i].gameObject.SetActive (true);
					rightAttr [i].text = oldEffects [i].getAttrValue (0).ToString () + "[3a9663]" + "+" + (newEffects [i].getAttrValue (0) - oldEffects [i].getAttrValue (0)) + "[-]";
					rightAttrIcon [i].gameObject.SetActive (true);
					rightAttrIcon [i].spriteName = ("attr_" + newEffects [i].getAttrType ());
				}
				if (i < newEffects.Length) {
					leftAttr[1].gameObject.SetActive(false);
					leftAttrIcon [1].gameObject.SetActive (false);
					newAttr.gameObject.SetActive (true);
					newAttr.transform.localPosition = (i == 1 ? pos2 : pos3);
					newAttr.text = (selectedEquip.equpStarState + 1).ToString () + LanguageConfigManager.Instance.getLanguage ("equipStar06");
					rightAttr [i].gameObject.SetActive (true);
					rightAttr [i].text = "[3a9663]" + "+" + newEffects [i].getAttrValue (0).ToString () + "[-]";
					rightAttrIcon [i].gameObject.SetActive (true);
					rightAttrIcon [i].spriteName = ("attr_" + newEffects [i].getAttrType ());
				}
			}else if(index < 6){
				attrLeft.transform.localPosition = pos4;
				attrRight.transform.localPosition = pos5;
				for (i=0; i < oldEffects.Length; i++) {
					leftAttr [i].gameObject.SetActive (true);
					leftAttr [i].text = oldEffects [i].getAttrValue (0).ToString ();
					leftAttrIcon [i].gameObject.SetActive (true);
					leftAttrIcon [i].spriteName = ("attr_" + oldEffects [i].getAttrType ());
					rightAttr [i].gameObject.SetActive (true);
					rightAttr [i].text = oldEffects [i].getAttrValue (0).ToString () + "[3a9663]" + "+" + (newEffects [i].getAttrValue (0) - oldEffects [i].getAttrValue (0)) + "[-]";
					rightAttrIcon [i].gameObject.SetActive (true);
					rightAttrIcon [i].spriteName = ("attr_" + newEffects [i].getAttrType ());
				}
			}else if (index < 9)
			{
			    attrLeft.transform.localPosition = pos6;
			    attrRight.transform.localPosition = pos7;
			    for (i = 0; i < oldEffects.Length; i++)
			    {
			        leftAttr[i].gameObject.SetActive(true);
			        leftAttr[i].text = oldEffects[i].getAttrValue(0).ToString();
			        leftAttrIcon[i].gameObject.SetActive(true);
			        leftAttrIcon[i].spriteName = ("attr_" + oldEffects[i].getAttrType());
			        rightAttr[i].gameObject.SetActive(true);
			        rightAttr[i].text = oldEffects[i].getAttrValue(0).ToString() + "[3a9663]" + "+" +
			                            (newEffects[i].getAttrValue(0) - oldEffects[i].getAttrValue(0)) + "[-]";
			        rightAttrIcon[i].gameObject.SetActive(true);
			        rightAttrIcon[i].spriteName = ("attr_" + newEffects[i].getAttrType());
			    }
			    if (i < newEffects.Length)
			    {
			        newAttr.gameObject.SetActive(true);
			        newAttr.transform.localPosition = (i == 1 ? pos2 : pos3);
			        newAttr.text = (selectedEquip.equpStarState + 1).ToString() +
			                       LanguageConfigManager.Instance.getLanguage("equipStar06");
			        rightAttr[i].gameObject.SetActive(true);
			        rightAttr[i].text = "[3a9663]" + "+" + newEffects[i].getAttrValue(0).ToString() + "[-]";
			        rightAttrIcon[i].gameObject.SetActive(true);
			        rightAttrIcon[i].spriteName = ("attr_" + newEffects[i].getAttrType());
			    }
            }
            else
            {
                attrLeft.transform.localPosition = pos4;
                attrRight.transform.localPosition = pos5;
                for (i = 0; i < oldEffects.Length; i++)
                {
                    leftAttr[i].gameObject.SetActive(true);
                    leftAttr[i].text = oldEffects[i].getAttrValue(0).ToString();
                    leftAttrIcon[i].gameObject.SetActive(true);
                    leftAttrIcon[i].spriteName = ("attr_" + oldEffects[i].getAttrType());
                    if (i < newEffects.Length)
                    {
                        rightAttr[i].gameObject.SetActive(true);
                        rightAttr[i].text = oldEffects[i].getAttrValue(0).ToString() + "[3a9663]" + "+" +
                                            (newEffects[i].getAttrValue(0) - oldEffects[i].getAttrValue(0)) + "[-]";
                        rightAttrIcon[i].gameObject.SetActive(true);
                        rightAttrIcon[i].spriteName = ("attr_" + newEffects[i].getAttrType());
                    }
                    else
                    {
                        rightAttr[i].gameObject.SetActive(false);
                        rightAttrIcon[i].gameObject.SetActive(false);
                    }
                }
                if (i < newEffects.Length) {
                    attrLeft.transform.localPosition = pos6;
                    attrRight.transform.localPosition = pos7;
                    newAttr.gameObject.SetActive(true);
                    newAttr.transform.localPosition = (i == 1 ? pos2 : pos3);
                    newAttr.text = (selectedEquip.equpStarState + 1).ToString() +
                                   LanguageConfigManager.Instance.getLanguage("equipStar06");
                    rightAttr[i].gameObject.SetActive(true);
                    rightAttr[i].text = "[3a9663]" + "+" + newEffects[i].getAttrValue(0).ToString() + "[-]";
                    rightAttrIcon[i].gameObject.SetActive(true);
                    rightAttrIcon[i].spriteName = ("attr_" + newEffects[i].getAttrType());
                }
            }
		}
		else if (selectedEquip.equpStarState == length) {
			attrLeft.transform.localPosition = pos8;
			arrowObj.SetActive(false);
			oldEffects = starSample.getAttrChangeSample (selectedEquip.equpStarState);
			attrRight.SetActive(false);
			int i;
			for (i=0; i < oldEffects.Length; i++) {
				leftAttr [i].gameObject.SetActive (true);
				leftAttr [i].text = oldEffects [i].getAttrValue (0).ToString ();
				leftAttrIcon [i].gameObject.SetActive (true);
				leftAttrIcon [i].spriteName = ("attr_" + oldEffects [i].getAttrType ());
			}
		}
	}
	///<summary>
	/// 获得消耗信息
	/// </summary>
	public void getConsumeInfo()
	{
        consumeInfo = EquipStarConfigManager.Instance.getCrystalCondition(selectedEquip.sid,selectedEquip.equpStarState);
	    goodsViewFather.gameObject.transform.localPosition = new Vector3(75f, -32, 0);
		Prop prop = PropManagerment.Instance.createProp(consumeInfo[0]);
		Prop consumeProp = StorageManagerment.Instance.getProp(consumeInfo[0]);
		//ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId(), crystalIcon);
		if(consumeProp != null)
			consumeGoods [0].init (consumeProp,0);
		else
			consumeGoods [0].init (prop);
		consumeCrystal.text = ((consumeInfo[1]> (consumeProp == null ? 0 : consumeProp.getNum()))?Colors.RED:Colors.GREEN) + (consumeProp == null ? 0 : consumeProp.getNum()) + "/" + consumeInfo[1];
		equipStarButton.disableButton (consumeInfo [1] > (consumeProp == null ? 0 : consumeProp.getNum()));
        consumeInfo = EquipStarConfigManager.Instance.getStoneCondition(selectedEquip.sid,selectedEquip.equpStarState);
		prop = PropManagerment.Instance.createProp(consumeInfo[0]);
		consumeProp = StorageManagerment.Instance.getProp(consumeInfo[0]);
		//ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId(), stoneIcon);
		if(consumeProp != null)
			consumeGoods [1].init (consumeProp,0);
		else
			consumeGoods [1].init (prop);
		consumeStone.text = ((consumeInfo[1]>(consumeProp == null ? 0 : consumeProp.getNum()))?Colors.RED:Colors.GREEN) + (consumeProp == null ? 0 : consumeProp.getNum()) + "/" + consumeInfo[1];
		if(!equipStarButton.isDisable())
			equipStarButton.disableButton (consumeInfo [1] > (consumeProp == null ? 0 : consumeProp.getNum()));
	    PrizeSample[] costMoney = EquipStarConfigManager.Instance.getMoneyCostBySid(selectedEquip.sid);
        //EquipSample sampleTmp = EquipmentSampleManager.Instance.getEquipSampleBySid(selectedEquip.sid);
        //    EquipStarAttrSample sample = sampleTmp == null ? null : EquipStarAttrSampleManager.Instance.getEquipStarAttrSampleBySid(sampleTmp.equipStarSid);
        //if (sample != null && sample.equipStarAtr.Length <= 9)
        //    return;
        //if (selectedEquip.getQualityId() <= 5)
        //    return;
	    if (costMoney == null)
	        return;
	    long costNum = 0;
	    int MYRAID = 10000;
        goodsViewFather.gameObject.transform.localPosition = new Vector3(0, -32, 0);
	    if (selectedEquip.equpStarState == costMoney.Length)
	    {
	        costNum = 0;
            consumeGoods[2].init(new PrizeSample(costMoney[costMoney.Length -1].type,
                costMoney[costMoney.Length -1].pSid, costNum));
	    }
	    else
	    {
	        costNum = StringKit.toLong(costMoney[selectedEquip.equpStarState].num);
	        consumeGoods[2].init(new PrizeSample(costMoney[selectedEquip.equpStarState].type,
	            costMoney[selectedEquip.equpStarState].pSid, costNum));
	    }
	    consumeGoods[2].rightBottomText.text = "";
        consumeGoods[2].gameObject.SetActive(true);
	    consumeMoney.text = ((int) costNum > UserManager.Instance.self.getMoney() ? Colors.RED : Colors.GREEN) +
	                        (UserManager.Instance.self.getMoney() >= MYRAID
	                            ? (UserManager.Instance.self.getMoney()/MYRAID) + "W"
	                            : UserManager.Instance.self.getMoney() +"") + "/" +
	                              (costNum >= MYRAID ? (costNum/MYRAID) + "W" : costNum + "");
        if (!equipStarButton.isDisable())
            equipStarButton.disableButton((int)costNum > UserManager.Instance.self.getMoney());
	}
	///<summary>
	/// 处理升星结果
	/// </summary>
	public void getEquipStarResult(bool isTrue){
		if (isTrue)
		{
		    effectStar = moveStarEffects[0];
		    if (selectedEquip.getQualityId() == 5)
		    {
                moveStarEffects[0].SetActive(true);
                moveStarEffects[1].SetActive(false);
		        moveStars();
		    } else if (selectedEquip.getQualityId() == 6 && selectedEquip.equpStarState > 9)
		    {
                effectStar = moveStarEffects[1];
                moveStarEffects[0].SetActive(false);
                moveStarEffects[1].SetActive(true);
		        setStars();
		        stars[selectedEquip.equpStarState - 10].transform.GetComponent<UISprite>().spriteName = "star";
            } else if (selectedEquip.getQualityId() == 6 && selectedEquip.equpStarState <= 9)
            {
                moveStarEffects[0].SetActive(true);
                moveStarEffects[1].SetActive(false);
                moveStars();
            }
		    updateEquip ();
			updateEquipStarAttributes ();
			updateEquipStarStateTip ();
			getConsumeInfo ();
		    updateButton();
		    effectStar.transform.localPosition = selectedEquip.equpStarState > 9
		        ? new Vector3(stars[selectedEquip.equpStarState - 10].transform.localPosition.x, 412, 161)
		        : new Vector3(selectedEquip.equpStarState%2 == 1 ? 0f : 25f, 412f, 161f);
			effectObj.SetActive(true);
			AudioManager.Instance.PlayAudio (147);
			StartCoroutine(Utils.DelayRun(()=>{
				AudioManager.Instance.PlayAudio (138);
			},1.5f));
			StartCoroutine(Utils.DelayRun(()=>{
                setStars();
				stars[0].SetActive(true);
				effectObj.SetActive(false);
				sendMessage = true;
				MaskWindow.UnlockUI ();
			},2.46f));
			//MaskWindow.UnlockUI ();
		}
		else {
            sendMessage = true;
			MaskWindow.UnlockUI();
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("equipStar09"));
		}
		MaskWindow.UnlockUI();
	}
	public void updateEquipStarStateTip(){
		starLevelState.gameObject.SetActive (true);
		starLevelState.text = (selectedEquip.equpStarState == 0 ? "" : "[FF0000]+" + selectedEquip.equpStarState.ToString ());
	}
}
